using System;
using System.Web.Mvc;
using VMall.Web.Mobile.Models;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using System.Web;
using System.Text;
using System.Web.Script.Serialization;

namespace VMall.Web.Mobile.Controllers
{
    /// <summary>
    /// 首页控制器类
    /// </summary>
    public partial class HomeController : BaseMobileController
    {
        JavaScriptSerializer js = new JavaScriptSerializer();
        private static readonly object _object = new object();

        /// <summary>
        /// 首页
        /// </summary>
        public ActionResult Index()
        {
            //首页的数据需要在其视图文件中直接调用，所以此处不再需要视图模型
            //判断浏览器来源，微信浏览器采用授权登录，手机浏览器跳到首页
            string userAgent = Request.UserAgent;
            if (userAgent.ToLower().Contains("micromessenger") && MallUtils.GetUidCookie() < 1)
            {
                int uid = TypeHelper.StringToInt(WebHelper.GetCookie("puid"));
                return RedirectToAction("IndexForWeiXin", "Home", new { uid = uid });
            }
            else
            {
                return RedirectToAction("Mindex", "Home", new { uid = WorkContext.Uid });
            }
        }

        /// <summary>
        ///  微信登录授权页面
        /// </summary>
        public void IndexForWeiXin(int uid)
        {
            string url = "https://open.weixin.qq.com/connect/oauth2/authorize";
            string appid = WxLoginConfig.APPID;
            string redirect_uri0 = string.Format("http://{0}/mob/Home/Access", BMAConfig.MallConfig.SiteUrl);//需要跳转的地址
            string response_type = "code";
            string scope = "snsapi_userinfo";
            string redirect_uri = HttpUtility.UrlEncode(redirect_uri0, Encoding.UTF8);
            StringBuilder sb = new StringBuilder();
            sb.Append("?appid=").Append(appid);
            sb.Append("&redirect_uri=").Append(redirect_uri);
            sb.Append("&response_type=").Append(response_type);
            sb.Append("&scope=").Append(scope);
            sb.Append("&state=").Append(uid);
            sb.Append("#wechat_redirect");
            string str = sb.ToString();
            //LogHelper.WriteOperateLog("WeiXinLogin", "微信登录", "uid：" + uid);
            LogHelper.WriteOperateLog("WeiXinLogin", "微信登录", "授权url：" + url + str);
            Response.Redirect(url + str);
            //string url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx40994de24213c04a&redirect_uri=http%3a%2f%2fwww.qdm1.com%2fmob%2fHome%2fAccess&response_type=code&scope=snsapi_base#wechat_redirect";

            //Response.Redirect(url);
        }
        /// <summary>
        ///  拉取微信授权
        /// </summary>
        /// <returns></returns>
        public ActionResult Access()
        {
            lock (_object)
            {
                string code = Request["code"];
                string puid = Request["state"];
                try
                {
                    if (HttpRuntime.Cache["code"] != null)
                    {
                        if (code == HttpRuntime.Cache["code"].ToString())
                        {
                            string Uid = HttpRuntime.Cache["Uid"].ToString();
                            string openId = HttpRuntime.Cache["openid"].ToString();
                            string status = HttpRuntime.Cache["status"].ToString();
                            PartUserInfo partUserInfo = Users.GetPartUserById(TypeHelper.StringToInt(Uid));

                            //将用户信息写入cookie中
                            MallUtils.SetUserCookie(partUserInfo, 7);
                            //保存openid
                            HttpCookie cookie = new HttpCookie("MyCook");
                            DateTime dt = DateTime.Now;
                            TimeSpan ts = new TimeSpan(30, 0, 0, 0, 0);//过期时间为1分钟
                            cookie.Expires = dt.Add(ts);//设置过期时间
                            cookie.Values.Add("openid", openId);
                            //cookies.Add(cookie);
                            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                            HttpRuntime.Cache.Insert("openid", openId);
                            Response.Cookies["status"].Value = status;
                            Response.Cookies["status"].Expires = System.DateTime.Now.AddDays(30);
                            return RedirectToAction("Mindex", "Home", new { uid = Uid });
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteOperateLog("WeiXinLoginError", "微信登录异常", "异常信息：" + ex.Message, (int)LogLevelEnum.ERROR);
                    return RedirectToAction("Mindex", "Home", new { uid = WorkContext.Uid });
                }
                HttpRuntime.Cache.Insert("code", code);
                string appid = WxLoginConfig.APPID;
                string secret = WxLoginConfig.APPSECRET;
                string grant_type = "authorization_code";
                string url = "https://api.weixin.qq.com/sns/oauth2/access_token";
                StringBuilder sb = new StringBuilder();
                sb.Append("appid=").Append(appid);
                sb.Append("&secret=").Append(secret);
                sb.Append("&code=").Append(code);
                sb.Append("&grant_type=").Append(grant_type);
                string str = sb.ToString();
                string result = WebHelper.DoGet(url, str);

                LogHelper.WriteOperateLog("WeiXinLogin", "微信登录", "获取openId和access_token：" + result);
                AccessModels access = js.Deserialize<AccessModels>(result); //获取openId和access_token
                if (access.errcode != null)
                {
                    return Content("获取失败！");
                }
                string access_token = access.access_token;
                string refresh_token = access.refresh_token;
                string openid = access.openid;
                HttpCookie cookies = new HttpCookie("MyCook");
                DateTime dts = DateTime.Now;
                TimeSpan tss = new TimeSpan(30, 0, 0, 0, 0);//过期时间为1分钟
                cookies.Expires = dts.Add(tss);//设置过期时间
                cookies.Values.Add("openid", openid);
                //cookies.Add(cookie);
                System.Web.HttpContext.Current.Response.Cookies.Add(cookies);

                PartUserInfo otherPartUser = Users.GetPartUserInfoByOtherLoginId(openid);
                if (otherPartUser != null)  //判断微信号是否已经绑定,已经绑定说明已经是会员，微信注册
                {
                    HttpRuntime.Cache.Insert("Uid", otherPartUser.Uid);
                    //将用户信息写入cookie中
                    MallUtils.SetUserCookie(otherPartUser, 15);
                    HttpRuntime.Cache.Insert("status", "1");
                    Response.Cookies["status"].Value = "1";
                    Response.Cookies["status"].Expires = System.DateTime.Now.AddDays(30);
                }
                else
                {
                    otherPartUser = new PartUserInfo() { Uid = -1 };
                    HttpRuntime.Cache.Insert("status", "0");
                    Response.Cookies["status"].Value = "0";
                    Response.Cookies["status"].Expires = System.DateTime.Now.AddMinutes(30);
                }
                #region
                //else//不存在该微信号绑定的帐号，则用该微信注册会员，锁定推荐人，推荐人从二维码带过来，没有推荐人就默认推荐人为公司，uid为1
                //{
                //    UserInfo newPartUser = new UserInfo() { };
                //    //根据创建新会员
                //    //取cookie中保存的推荐uid
                //    string CookiePuid = puid;// "";// WebHelper.GetCookie("puid");
                //    if (string.IsNullOrEmpty(puid))
                //        CookiePuid = WebHelper.GetCookie("puid");// puid;
                //    PartUserInfo pUserInfo = new PartUserInfo();
                //    int userSource = 0;
                //    if (TypeHelper.StringToInt(CookiePuid) > 0)
                //    {
                //        pUserInfo = Users.GetPartUserById(TypeHelper.StringToInt(CookiePuid));
                //        userSource = 1;
                //        //推荐人不为VIP推荐失败,默认挂靠公司点位，uid=1
                //        if (pUserInfo == null)
                //        {
                //            pUserInfo = Users.GetPartUserById(1);
                //            userSource = 0;
                //        }
                //        else
                //        {
                //            if (pUserInfo.AgentType < 1)//推荐人不为VIP
                //            {
                //                pUserInfo = Users.GetPartUserById(1);
                //                userSource = 0;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        pUserInfo = Users.GetPartUserById(1);//没有推荐人就默认推荐人为公司，uid为1
                //        userSource = 0;
                //    }
                //    #region 初始化用户信息
                //    string nickName = string.Empty;
                //    //生成随机初始密码并发送短信
                //    string password = Randoms.CreateRandomValue(6);
                //    newPartUser.Salt = Randoms.CreateRandomValue(6);
                //    newPartUser.Password = Users.CreateUserPassword(password.Trim(), newPartUser.Salt);
                //    newPartUser.UserRid = UserRanks.GetLowestUserRank().UserRid;
                //    newPartUser.StoreId = 0;
                //    newPartUser.MallAGid = 1;//非管理员组
                //    newPartUser.UserName = "Mi_" + Randoms.CreateRandomValue(9);
                //    newPartUser.NickName = newPartUser.UserName;
                //    newPartUser.Avatar = string.Empty;
                //    newPartUser.PayCredits = 0;
                //    newPartUser.RankCredits = 0;
                //    newPartUser.VerifyEmail = 0;
                //    newPartUser.VerifyMobile = 0;
                //    newPartUser.DirSalePwd = SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);
                //    newPartUser.LastVisitIP = WorkContext.IP;
                //    newPartUser.LastVisitRgId = WorkContext.RegionId;
                //    newPartUser.LastVisitTime = DateTime.Now;
                //    newPartUser.RegisterIP = WorkContext.IP;
                //    newPartUser.RegisterRgId = WorkContext.RegionId;
                //    newPartUser.RegisterTime = DateTime.Now;
                //    newPartUser.Gender = 0;
                //    newPartUser.RealName = string.Empty;
                //    newPartUser.Bday = new DateTime(1900, 1, 1);
                //    newPartUser.IdCard = string.Empty;
                //    newPartUser.RegionId = -1;
                //    newPartUser.Address = string.Empty;
                //    newPartUser.Bio = string.Empty;

                //    newPartUser.MallSource = 1;
                //    newPartUser.UserSource = userSource;
                //    #endregion

                //    #region 处理用户注册推荐人关系

                //    newPartUser.Pid = pUserInfo.Uid;
                //    newPartUser.Ptype = 1;//推荐人类型

                //    #endregion
                //    //创建用户
                //    newPartUser.Uid = Users.CreateUser(newPartUser);

                //    //添加用户失败
                //    if (newPartUser.Uid < 1)
                //        return AjaxResult("exception", "创建用户失败,请联系管理员");
                //    //绑定微信用户信息
                //    string url2 = "https://api.weixin.qq.com/sns/userinfo"; //获取微信用户信息
                //    string lang = "zh_CN";
                //    StringBuilder sb2 = new StringBuilder();
                //    sb2.Append("access_token=").Append(access_token);
                //    sb2.Append("&openid=").Append(openid);
                //    sb2.Append("&lang=").Append(lang);
                //    var str1 = sb2.ToString();
                //    //LogHelper.WriteOperateLog("WeiXinBinding", "微信绑定", "获取微信用户信息提交参数：" + str1);
                //    string result2 = WebHelper.DoGet(url2, str1);
                //    LogHelper.WriteOperateLog("WeiXinBinding", "微信绑定", "获取微信用户信息：" + result2);
                //    try
                //    {
                //        result2 = result2.Substring(0, result2.IndexOf(",\"privilege\"")) + "}";
                //    }
                //    catch { }
                //    WXUserInfosModels userInfos = js.Deserialize<WXUserInfosModels>(result2);
                //    if (userInfos.errcode != null) return Content("获取授权失败！");
                //    //int integral = 0;
                //    string WxnickName = userInfos.nickname; //昵称
                //    int sex = Convert.ToInt16(userInfos.sex);//1为男 2为女 0为未知
                //    //member.Address = userInfos.country + userInfos.province + userInfos.city;
                //    string headimgurl = userInfos.headimgurl; //用户头像
                //    // HttpRuntime.Cache.Insert("headimgurl", headimgurl);
                //    //Response.Cookies["headimgurl"].Value = headimgurl;
                //    //userInfos.privilege; //用户特权信息，json 数组，如微信沃卡用户为（chinaunicom）
                //    //userInfos.unionid; //只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段

                //    //headimgurl = headimgurl.Substring(26, headimgurl.Length - 26);

                //    newPartUser.NickName = WxnickName;
                //    newPartUser.Avatar = headimgurl;
                //    newPartUser.OtherLoginId = openid;
                //    Users.UpdatePartUserForWeiXin(newPartUser);


                //    //更新购物车中用户id
                //    //Carts.UpdateCartUidBySid(newPartUser.Uid, WorkContext.Sid);
                //    //将用户信息写入cookie
                //    MallUtils.SetUserCookie(newPartUser, 15);

                //    //发送注册成功和初始密码短信
                //    //if (newPartUser.Mobile.Length > 0)
                //    //    SMSes.SendRegisterSuccessSMS(newPartUser.Mobile, password);
                //    //同步上下午
                //    WorkContext.Uid = newPartUser.Uid;
                //    WorkContext.UserName = newPartUser.UserName;
                //    WorkContext.UserEmail = newPartUser.Email;
                //    WorkContext.UserMobile = newPartUser.Mobile;
                //    WorkContext.NickName = newPartUser.NickName;

                //    otherPartUser = newPartUser;

                //    HttpRuntime.Cache.Insert("Uid", otherPartUser.Uid);
                //    HttpRuntime.Cache.Insert("status", "1");
                //    Response.Cookies["status"].Value = "1";
                //    Response.Cookies["status"].Expires = System.DateTime.Now.AddMinutes(30);
                //}
                #endregion
                
                return RedirectToAction("Mindex", "Home", new { uid = otherPartUser.Uid });
            }

        }

        /// <summary>
        ///  正常手机浏览器登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Mindex()
        {
            //获取推荐Uid
            int uid = GetRouteInt("uid");

            int cookpuid = TypeHelper.StringToInt(WebHelper.GetCookie("puid"));
            if (uid == 0)
                uid = WebHelper.GetQueryInt("uid");
            if (uid <= 0)
            {
                uid = cookpuid;
            }
            //不登录情况下,且不存在缓存puid,保存url的uid来锁定推荐关系
            if (WorkContext.Uid < 1)
            {

                WebHelper.SetCookie("puid", uid.ToString(), 60 * 24 * 30);
            }
            return View("index2019");
        }

        /// <summary>
        /// 分享首页
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="type">type为1 表示直接访问首页，扫二维码进来或者分享首页进来，type=2表示非首页，如产品详情页</param>
        /// <returns></returns>
        public ActionResult ShareIndex(int uid, int type = 1)
        {
            //LogHelper.WriteOperateLog("WeiXinShare", "微信分享", "uid：" + uid);
            //首页的数据需要在其视图文件中直接调用，所以此处不再需要视图模型
            //判断浏览器来源，微信浏览器采用授权登录，手机浏览器跳到首页
            string userAgent = Request.UserAgent;
            //if (userAgent.ToLower().Contains("micromessenger") && MallUtils.GetUidCookie() < 1)
            //{
            //    if(type==2)
            //        uid = TypeHelper.StringToInt(WebHelper.GetCookie("puid"));
            //    return RedirectToAction("IndexForWeiXin", "Home", new { uid = uid });
            //}
            //else
            //{
            return RedirectToAction("Mindex", "Home", new { uid = uid });
            //}
        }
    }
}
