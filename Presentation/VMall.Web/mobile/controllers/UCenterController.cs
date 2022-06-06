using System;
using System.Text;
using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

using VMall.Core;
using System.Linq;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Mobile.Models;

namespace VMall.Web.Mobile.Controllers
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.IO;
    using System.Drawing.Drawing2D;
    using System.Web.Script.Serialization;
    using System.Web;

    /// <summary>
    /// 用户中心控制器类
    /// </summary>
    public partial class UCenterController : BaseMobileController
    {
        private static object ctx = new object();//锁对象

        private static string dirSaleApiUrl = WebSiteConfig.DirsaleApiUrl;//api配置地址
        JavaScriptSerializer js = new JavaScriptSerializer();
        HaiMiDrawCash haiMiDrawCashBLL = new HaiMiDrawCash();
        OrderGift orderGiftBLL = new OrderGift();

        #region 微信绑定
        /// <summary>
        /// 微信登录授权页面
        /// //https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxa4f53f72ba719bb8&redirect_uri=http%3a%2f%2fv.xxxx.com%2findex.html&response_type=code&scope=snsapi_userinfo#wechat_redirect 引导会员打开该链接
        /// </summary>
        public void GetCode()
        {
            string url = "https://open.weixin.qq.com/connect/oauth2/authorize";
            string appid = WxLoginConfig.APPID;
            string redirect_uri0 = string.Format("http://{0}/mob/Ucenter/Access", BMAConfig.MallConfig.SiteUrl);//需要跳转的地址
            string response_type = "code";
            string scope = "snsapi_userinfo";
            string redirect_uri = HttpUtility.UrlEncode(redirect_uri0, Encoding.UTF8);
            StringBuilder sb = new StringBuilder();
            sb.Append("?appid=").Append(appid);
            sb.Append("&redirect_uri=").Append(redirect_uri);
            sb.Append("&response_type=").Append(response_type);
            sb.Append("&scope=").Append(scope).Append("#wechat_redirect");
            string str = sb.ToString();
            //LogHelper.WriteOperateLog("WeiXinBinding", "微信绑定", "授权url：" + url + str);
            Response.Redirect(url + str);

        }

        /// <summary>
        /// 拉取微信授权 自动登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Access() //获取openId,获取用户信息
        {
            //LogHelper.WriteOperateLog("WeiXinBinding", "微信绑定", "========开始回调===========");
            lock (ctx)
            {
                string code = Request["code"];
                //LogHelper.WriteOperateLog("WeiXinBinding", "微信绑定", "========开始回调。获取code===========" + code);
                try
                {
                    if (HttpRuntime.Cache["code"] != null)
                    {
                        if (code == HttpRuntime.Cache["code"].ToString())
                        {
                            //LogHelper.WriteOperateLog("WeiXinBinding", "微信绑定", "========开始回调。获取cache.code===========" + HttpRuntime.Cache["code"].ToString());
                            //string Uid = HttpRuntime.Cache["Uid"].ToString();
                            string openId = HttpRuntime.Cache["openid"].ToString();
                            string status = HttpRuntime.Cache["status"].ToString();
                            //LogHelper.WriteOperateLog("WeiXinBinding", "微信绑定", "========开始回调。获取uid========workcontentuid" + WorkContext.Uid);
                            PartUserInfo partUserInfo = Users.GetPartUserById(WorkContext.Uid);
                            //将用户信息写入cookie中
                            MallUtils.SetUserCookie(partUserInfo, 7);
                            HttpCookie cookie = new HttpCookie("MyCook");
                            DateTime dt = DateTime.Now;
                            TimeSpan ts = new TimeSpan(30, 0, 0, 0, 0);//过期时间为1分钟
                            cookie.Expires = dt.Add(ts);//设置过期时间
                            cookie.Values.Add("openid", openId);
                            //cookies.Add(cookie);
                            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                            Response.Cookies["status"].Value = status;
                            Response.Cookies["status"].Expires = System.DateTime.Now.AddDays(30);
                            return Redirect("/ucenter/index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteOperateLog("WeiXinbindingError", "微信绑定异常", "异常信息：" + ex.Message, (int)LogLevelEnum.ERROR);
                    return PromptView(Url.Action("GetCode"), "授权出错。");
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
                //LogHelper.WriteOperateLog("WeiXinBinding", "微信绑定", "获取openId和access_token：" + result);
                AccessModels access = js.Deserialize<AccessModels>(result); //获取openId和access_token
                if (access.errcode != null)
                {
                    return Content("获取失败！" + result + code);
                }
                string access_token = access.access_token;
                string refresh_token = access.refresh_token;
                string openid = access.openid;

                //string url1 = "https://api.weixin.qq.com/sns/oauth2/refresh_token";//刷新access_token（如果需要）
                //string sb1 = string.Format("appid={0}&grant_type={1}&refresh_token={2}", openid, refresh_token, refresh_token);
                //string result1 = DoGet(url1, sb1);
                //RefreshModel Refresh = js.Deserialize<RefreshModel>(result1);
                //access_token = Refresh.access_token;
                //string access_token = "OezXcEiiBSKSxW0eoylIeIjYwEbBjH_4bJTNEmlGf5Ol_WsFnZP-7GWr1dDW05fn34emOpQgd5pTd4dcDDzSd8ZfxS4bQz77RGEQOA5_1_QsCwUQ01R39l_MANyXh8ILOGd2d9CTuosP0xjCgdVbgw";
                //string openid = "oa6tIt6-tpbqdcemnwh13IWXIh5g";

                HttpRuntime.Cache.Insert("openid", openid);
                HttpCookie cookies = new HttpCookie("MyCook");
                DateTime dts = DateTime.Now;
                TimeSpan tss = new TimeSpan(30, 0, 0, 0, 0);//过期时间为1分钟
                cookies.Expires = dts.Add(tss);//设置过期时间
                cookies.Values.Add("openid", openid);
                //cookies.Add(cookie);
                System.Web.HttpContext.Current.Response.Cookies.Add(cookies);
                PartUserInfo otherPartUser = Users.GetPartUserInfoByOtherLoginId(openid);

                if (otherPartUser != null)  //判断微信号是否已经绑定
                {
                    HttpRuntime.Cache.Insert("Uid", otherPartUser.Uid);
                    //LogHelper.WriteOperateLog("WeiXinBinding", "微信绑定", "user：" + openid + "===" + otherPartUser.Uid);
                    //将用户信息写入cookie中
                    MallUtils.SetUserCookie(otherPartUser, 1);
                    HttpRuntime.Cache.Insert("status", 1);
                    Response.Cookies["status"].Value = "1";
                    Response.Cookies["status"].Expires = System.DateTime.Now.AddDays(30);
                }
                else
                {
                    string url2 = "https://api.weixin.qq.com/sns/userinfo"; //获取微信用户信息
                    string lang = "zh_CN";
                    StringBuilder sb2 = new StringBuilder();
                    sb2.Append("access_token=").Append(access_token);
                    sb2.Append("&openid=").Append(openid);
                    sb2.Append("&lang=").Append(lang);
                    var str1 = sb2.ToString();
                    string result2 = WebHelper.DoGet(url2, str1);
                    //LogHelper.WriteOperateLog("WeiXinBinding", "微信绑定", "获取微信用户信息：" + result2);
                    try
                    {
                        result2 = result2.Substring(0, result2.IndexOf(",\"privilege\"")) + "}";
                    }
                    catch { }
                    WXUserInfosModels userInfos = js.Deserialize<WXUserInfosModels>(result2);
                    if (userInfos.errcode != null) return Content("获取授权失败！");
                    //int integral = 0;
                    string nickName = userInfos.nickname; //昵称
                    int sex = Convert.ToInt16(userInfos.sex);//1为男 2为女 0为未知
                    //member.Address = userInfos.country + userInfos.province + userInfos.city;
                    string headimgurl = userInfos.headimgurl; //用户头像
                    // HttpRuntime.Cache.Insert("headimgurl", headimgurl);
                    //Response.Cookies["headimgurl"].Value = headimgurl;
                    //userInfos.privilege; //用户特权信息，json 数组，如微信沃卡用户为（chinaunicom）
                    //userInfos.unionid; //只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段

                    headimgurl = headimgurl.Substring(26, headimgurl.Length - 26);

                    PartUserInfo otherUser = Users.GetPartUserById(WorkContext.Uid);
                    otherUser.NickName = nickName;
                    otherUser.Avatar = headimgurl;
                    otherUser.OtherLoginId = openid;
                    Users.UpdatePartUserForWeiXin(otherUser);

                    //更新头像后删除原来的分享二维码
                    FileInfo delCut = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "/upload/bgqrcode/QRcode-" + otherUser.Uid + ".jpg");
                    if (delCut.Exists)
                        delCut.Delete();

                    HttpRuntime.Cache.Insert("Uid", otherUser.Uid);
                    HttpRuntime.Cache.Insert("status", 0);
                    Response.Cookies["status"].Value = "0";
                    Response.Cookies["status"].Expires = System.DateTime.Now.AddDays(30);
                }
                return RedirectToAction("Index", "Ucenter");
            }
        }

        //public string Binding()
        //{
        //    string openid = "";
        //    HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies["MyCook"];
        //    if (cookie != null)
        //    {
        //        openid = cookie["openid"];
        //    }
        //    else
        //    {
        //        return "请重新获取授权";
        //    }
        //    string loginName = Request["loginName"];
        //    string passWord = Request["passWord"];
        //    Account account = accountBll.CommonLoginUser(loginName, passWord);//先对loginName与passWord进行加密处理
        //    if (account == null) return "账号密码错误！";
        //    else
        //    {
        //        Guid accountId = account.Id;
        //        OtherAccount otherAccounts = otherAccountBll.GetModel(string.Format("AccountId='{0}'", accountId));
        //        otherAccount = otherAccountBll.GetModel(string.Format("OtherId='{0}'", openid));
        //        account = accountBll.GetModel(string.Format("LoginName='{0}' and Password='{1}'", loginName, passWord));
        //        if (otherAccount != null) return "该微信号已绑定！"; //先判断微信号是否已经绑定
        //        if (otherAccounts != null) return "该健客用户名已绑定！"; //判断用户名是否已经绑定  
        //        OtherAccount otherAccountss = new OtherAccount();
        //        otherAccountss.Id = Guid.NewGuid();
        //        otherAccountss.OtherId = openid;
        //        otherAccountss.CreationDate = System.DateTime.Now;
        //        otherAccountss.AccountId = accountId;
        //        otherAccountss.OtherType = 2;
        //        otherAccountBll.Add(otherAccountss);             //添加到数据库  
        //        AccountLogin accountLogin = new AccountLogin();
        //        accountLogin.AccountId = accountId.ToString();
        //        string domen = GetDomiancs.GetDomain(System.Web.HttpContext.Current.Request.Url.Host);
        //        LoginCookie.SaveLogin(accountLogin, domen, 43200);
        //        // HttpRuntime.Cache.Insert("status", 1);
        //        Response.Cookies["status"].Value = "1";
        //        return "绑定成功";
        //    }
        //}

        public ActionResult WeiXinUnBind()
        {
            bool flag = Users.WeiXinUnBind(WorkContext.Uid);
            if (flag)
                return Content("1");
            return Content("0");
        }
        #endregion

        #region 用户信息
        /// <summary>
        /// 首页
        /// </summary>
        public ActionResult Index()
        {
            //UserInfo userinfo = Users.GetUserById(WorkContext.Uid);
            //ViewData["headimg"] = userinfo != null ? userinfo.Avatar : "";
            return View();
        }

        /// <summary>
        /// 二维码
        /// </summary>
        /// <returns></returns>
        public ActionResult ShareCode()
        {
            return RedirectToAction("index", "codeimg", new RouteValueDictionary { { "shareid", WorkContext.Uid }, { "t", Randoms.CreateRandomValue(4, true).ToLower() } });
            ////二维码
            //string parentName = string.IsNullOrEmpty(WorkContext.UserName) ? (string.IsNullOrEmpty(WorkContext.UserEmail) ? (string.IsNullOrEmpty(WorkContext.UserMobile) ? "" : WorkContext.UserMobile) : WorkContext.UserEmail) : WorkContext.UserName;
            //bool isMobile = true;//WebHelper.IsMobile();
            //string shareUrl = "http://" + Request.Url.Host + ":" + Request.Url.Port + (isMobile ? "/mob" : "") + "/account/register?pname=" + parentName.Trim() + "&returnUrl=http://" + Request.Url.Host + ":" + Request.Url.Port + (isMobile ? "/mob" : "");
            ////string codeImgPath = CreateCode_Simple(shareUrl, WorkContext.PartUserInfo.Uid, WorkContext.PartUserInfo.Salt, isMobile);
            //string bgQRcode = IOHelper.CreatQRCodeWithBG2(shareUrl, WorkContext.Uid, WorkContext.PartUserInfo);

            //ViewData["codeImg"] = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/upload/bgqrcode/" + bgQRcode;

            //return View();
        }

        /// <summary>
        /// 账户
        /// </summary>
        public ActionResult Account()
        {
            return View();
        }
        public ActionResult UserInfo()
        {
            UserInfo userInfo = Users.GetUserById(WorkContext.Uid);
            UserDetailInfo userDetail = Users.GetUserDetailById(WorkContext.Uid);
            userInfo.BankName = userDetail.BankName;
            userInfo.BankCardCode = userDetail.BankCardCode;
            userInfo.BankUserName = userDetail.BankUserName;
            return View(userInfo);
        }

        /// <summary>
        /// 编辑用户信息
        /// </summary>
        public ActionResult EditUser()
        {
            string nickName = WebHelper.GetFormString("nickName");
            string realName = WebHelper.GetFormString("realName");
            string idCard = WebHelper.GetFormString("idCard");
            string bankname = WebHelper.GetFormString("BankName");
            string bankcardcode = WebHelper.GetFormString("BankCardCode");
            string bankusername = WebHelper.GetFormString("BankUserName");
            StringBuilder errorList = new StringBuilder("[");

            //验证昵称
            if (nickName.Length > 20)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "nickName", "昵称的长度不能大于20", "}");
            }
            else if (FilterWords.IsContainWords(nickName))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "nickName", "昵称中包含禁止单词", "}");
            }

            //验证真实姓名
            if (realName.Length > 5)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "realName", "真实姓名的长度不能大于5", "}");
            }


            //验证身份证号
            if (idCard.Length > 18)//!ValidateHelper.IsIdCard(idCard)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "idCard", "请输入正确的身份证号", "}");
            }
            // 验证银行名称
            if (bankname.Length > 25)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "bank", "银行名称长度不能大于25", "}");
            }
            // 验证银行卡号
            if (bankcardcode.Length > 30)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "bank", "银行卡号长度不能大于30", "}");
            }
            // 验证银行开户人
            if (bankusername.Length > 15)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "bank", "银行开户人长度不能大于15", "}");
            }
            if (errorList.Length == 1)
            {
                Users.UpdateUser(WorkContext.Uid, WorkContext.UserName, WebHelper.HtmlEncode(nickName), WorkContext.Avatar, 0, WebHelper.HtmlEncode(realName), DateTime.Now, idCard, WorkContext.RegionId, "", "", bankname, bankcardcode, bankusername);
                //if (userName.Length > 0 && nickName.Length > 0 && avatar.Length > 0 && realName.Length > 0 && bday != "1900-1-1" && idCard.Length > 0 && regionId > 0 && address.Length > 0)
                //{
                //    Credits.SendCompleteUserInfoCredits(ref WorkContext.PartUserInfo, DateTime.Now);
                //}
                return AjaxResult("success", "信息更新成功");
            }
            else
            {
                return AjaxResult("error", errorList.Remove(errorList.Length - 1, 1).Append("]").ToString(), true);
            }
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeHeadPic()
        {
            //PartUserInfo userInfo = Users.GetPartUserById(WorkContext.Uid);
            return View();
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        /// <param name="FileData"></param>
        /// <param name="extensionValue"></param>
        /// <param name="extensionText"></param>
        /// <returns></returns>
        [HttpPost]
        public string changephoto()
        {
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            string[] strPhotoList = UpfilePublic.UploadPhoto(Request.Files["uploadInput"], "", "");

            if (string.IsNullOrEmpty(strPhotoList[1]))
            {
                return "<script>alert('获取图片URL失败');window.location.href = '/mob/ucenter';</script>";
            }
            //ReturnMessage Message = FTPCouponBLL.UploadIndexFile("memberavatar/", strPhotoList[1], FtpCore.FTPEntity());
            //if (Message.Success)
            //{

            PartUserInfo userInfo = Users.GetPartUserById(WorkContext.Uid);
            if (userInfo == null)
                return "<script>alert('获取账号失败');window.location.href ='/mob/ucenter';</script>";
            string strOldPhoto = userInfo.Avatar;


            userInfo.Avatar = strPhotoList[1];
            if (!Users.UpdatePartUserAvatar(userInfo))
            {
                return "<script>alert('图像修改失败');window.location.href = '/mob/ucenter';</script>";
            }
            //删除以前的头像
            if (!string.IsNullOrEmpty(strOldPhoto))
            {
                if (!strOldPhoto.Contains("http://thirdwx.qlogo.cn/"))
                {
                    FileInfo delCut = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "/upload/userheadpic/" + strOldPhoto);
                    if (delCut.Exists)
                        delCut.Delete();
                }
            }
            //更新头像后删除原来的分享二维码
            FileInfo delOleQRCode = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "/upload/bgqrcode/QRcode-" + WorkContext.Uid + ".jpg");
            if (delOleQRCode.Exists)
                delOleQRCode.Delete();

            return "<script>alert('头像修改成功!');window.location.href = '/mob/ucenter';</script>";
            //}
            //return "<script>alert('图像修改失败');window.location.href = '/mob';</script>";

        }
        #endregion

        #region 安全中心

        /// <summary>
        /// 安全验证
        /// </summary>
        public ActionResult SafeVerify()
        {
            string action = WebHelper.GetQueryString("act").ToLower();
            string mode = WebHelper.GetQueryString("mode").ToLower();

            if (action.Length == 0 || !CommonHelper.IsInArray(action, new string[4] { "updatepassword", "updatemobile", "updateemail", "updatepaypassword" }) || (mode.Length > 0 && !CommonHelper.IsInArray(mode, new string[3] { "password", "mobile", "email" })))
                return HttpNotFound();

            SafeVerifyModel model = new SafeVerifyModel();
            model.Action = action;

            if (mode.Length == 0)
            {
                if (WorkContext.PartUserInfo.VerifyMobile == 1)//通过手机验证
                    model.Mode = "mobile";
                else if (WorkContext.PartUserInfo.VerifyEmail == 1)//通过邮箱验证
                    model.Mode = "email";
                else//通过密码验证
                    model.Mode = "password";
            }
            else
            {
                if (mode == "mobile" && WorkContext.PartUserInfo.VerifyMobile == 1)
                    model.Mode = "mobile";
                else if (mode == "email" && WorkContext.PartUserInfo.VerifyEmail == 1)
                    model.Mode = "email";
                else
                    model.Mode = "password";
            }
            if (action == "updatepaypassword")
            {
                UserInfo user = Users.GetUserById(WorkContext.Uid);
                if (string.IsNullOrEmpty(WorkContext.PartUserInfo.Mobile))
                    return PromptView("account", "您还没绑定手机，请先绑定手机。");

                else if (string.IsNullOrEmpty(user.IdCard))
                    return PromptView("account", "您还没完善身份证信息，请先完善身份证信息。");
                else
                    model.Mode = "mobile";
            }
            return View(model);
        }

        /// <summary>
        /// 验证密码
        /// </summary>
        public ActionResult VerifyPassword()
        {
            string action = WebHelper.GetQueryString("act").ToLower();
            string password = WebHelper.GetFormString("password");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            if (action.Length == 0 || !CommonHelper.IsInArray(action, new string[4] { "updatepassword", "updatemobile", "updateemail", "updatepaypassword" }))
                return AjaxResult("noaction", "动作不存在");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            //检查密码
            if (string.IsNullOrWhiteSpace(password))
            {
                return AjaxResult("password", "密码不能为空");
            }
            //if (Users.CreateUserPassword(password, WorkContext.PartUserInfo.Salt) != WorkContext.Password)
            //{
            //    return AjaxResult("password", "密码不正确");
            //}
            if (password != "123")
            {
                return AjaxResult("password", "密码不正确");
            }

            string v = MallUtils.AESEncrypt(string.Format("{0},{1},{2},{3}", WorkContext.Uid, action, DateTime.Now, Randoms.CreateRandomValue(6)));
            string url = Url.Action("safeupdate", new RouteValueDictionary { { "v", v } });
            return AjaxResult("success", url);
        }

        /// <summary>
        /// 发送验证手机短信
        /// </summary>
        public ActionResult SendVerifyMobile()
        {
            //if (WorkContext.PartUserInfo.VerifyMobile == 0)
            //    return AjaxResult("unverifymobile", "手机号没有通过验证,所以不能发送验证短信");

            string moibleCode = Randoms.CreateRandomValue(6);
            //发送验证手机短信
            SMSes.SendSCVerifySMS(WorkContext.UserMobile, moibleCode);
            //将验证值保存在session中
            Sessions.SetItem(WorkContext.Sid, "ucsvMoibleCode", moibleCode);

            return AjaxResult("success", "短信已经发送,请查收");
        }

        /// <summary>
        /// 发送验证手机语音验证码
        /// </summary>
        public ActionResult SendVerifyMobileVoice()
        {
            //if (WorkContext.PartUserInfo.VerifyMobile == 0)
            //    return AjaxResult("unverifymobile", "手机号没有通过验证,所以不能发送验证短信");

            string moibleCode = Randoms.CreateRandomValue(4);
            //发送验证手机短信
            if (SMSes.SendSCVerifySMSVoice(WorkContext.UserMobile, moibleCode))
            {
                //将验证值保存在session中
                Sessions.SetItem(WorkContext.Sid, "ucsvMoibleCode", moibleCode);
                return AjaxResult("success", "短信已经发送,请查收");
            }
            return AjaxResult("error", "系统繁忙，请稍后再试！！！");
        }
        /// <summary>
        /// 验证手机
        /// </summary>
        public ActionResult VerifyMobile()
        {
            string action = WebHelper.GetQueryString("act").ToLower();
            string moibleCode = WebHelper.GetFormString("moibleCode");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            if (action.Length == 0 || !CommonHelper.IsInArray(action, new string[4] { "updatepassword", "updatemobile", "updateemail", "updatepaypassword" }))
                return AjaxResult("noaction", "动作不存在");
            //if (WorkContext.PartUserInfo.VerifyMobile == 0)
            //    return AjaxResult("unverifymobile", "手机号没有通过验证,所以不能进行验证");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            //检查手机码
            if (string.IsNullOrWhiteSpace(moibleCode))
            {
                return AjaxResult("moiblecode", "手机码不能为空");
            }
            if (Sessions.GetValueString(WorkContext.Sid, "ucsvMoibleCode") != moibleCode)
            {
                return AjaxResult("moiblecode", "手机码不正确");
            }

            string v = MallUtils.AESEncrypt(string.Format("{0},{1},{2},{3}", WorkContext.Uid, action, DateTime.Now, Randoms.CreateRandomValue(6)));
            string url = Url.Action("safeupdate", new RouteValueDictionary { { "v", v } });
            return AjaxResult("success", url);
        }

        /// <summary>
        /// 发送验证邮箱邮件
        /// </summary>
        public ActionResult SendVerifyEmail()
        {
            string action = WebHelper.GetQueryString("act").ToLower();
            string verifyCode = WebHelper.GetFormString("verifyCode");

            if (action.Length == 0 || !CommonHelper.IsInArray(action, new string[4] { "updatepassword", "updatemobile", "updateemail", "updatepaypassword" }))
                return AjaxResult("noaction", "动作不存在");
            if (WorkContext.PartUserInfo.VerifyEmail == 0)
                return AjaxResult("unverifyemail", "邮箱没有通过验证,所以不能发送验证邮件");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            string v = MallUtils.AESEncrypt(string.Format("{0},{1},{2},{3}", WorkContext.Uid, action, DateTime.Now, Randoms.CreateRandomValue(6)));
            string url = string.Format("http://{0}{1}", Request.Url.Authority, Url.Action("safeupdate", new RouteValueDictionary { { "v", v } }));
            //发送验证邮件
            Emails.SendSCVerifyEmail(WorkContext.UserEmail, WorkContext.UserName, url);
            return AjaxResult("success", "邮件已经发送,请前往你的邮箱进行验证");
        }

        /// <summary>
        /// 安全更新
        /// </summary>
        public ActionResult SafeUpdate()
        {
            string v = WebHelper.GetQueryString("v").Replace(" ", "+");
            //解密字符串
            string realV = SecureHelper.AESDecrypt(v, WorkContext.MallConfig.SecretKey);

            //数组第一项为uid，第二项为动作，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return HttpNotFound();

            int uid = TypeHelper.StringToInt(result[0]);
            string action = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return HttpNotFound();
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return PromptView("此链接已经失效，请重新验证");

            SafeUpdateModel model = new SafeUpdateModel();
            model.Action = action;
            model.V = WebHelper.UrlEncode(v);

            return View(model);
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        public ActionResult UpdatePassword()
        {
            string v = WebHelper.GetQueryString("v").Replace(" ", "+");
            //解密字符串
            string realV = SecureHelper.AESDecrypt(v, WorkContext.MallConfig.SecretKey);

            //数组第一项为uid，第二项为动作，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return AjaxResult("noauth", "您的权限不足");

            int uid = TypeHelper.StringToInt(result[0]);
            string action = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return AjaxResult("noauth", "您的权限不足");
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return AjaxResult("expired", "密钥已过期,请重新验证");

            string password = WebHelper.GetFormString("password");
            string confirmPwd = WebHelper.GetFormString("confirmPwd");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            //检查密码
            if (string.IsNullOrWhiteSpace(password))
            {
                return AjaxResult("password", "密码不能为空");
            }
            if (password.Length < 4 || password.Length > 32)
            {
                return AjaxResult("password", "密码必须大于3且不大于32个字符");
            }
            if (password != confirmPwd)
            {
                return AjaxResult("confirmpwd", "两次密码不相同");
            }

            string p = Users.CreateUserPassword(password, WorkContext.PartUserInfo.Salt);
            //设置新密码
            Users.UpdatePartUserPwd(WorkContext.PartUserInfo, password);
            //同步cookie中密码
            MallUtils.SetCookiePassword(p);

            string url = Url.Action("safesuccess", new RouteValueDictionary { { "act", "updatePassword" } });
            return AjaxResult("success", url);
        }

        /// <summary>
        /// 更新支付密码
        /// </summary>
        public ActionResult UpdatePayPassword()
        {
            string v = WebHelper.GetQueryString("v").Replace(" ", "+");
            //解密字符串
            string realV = SecureHelper.AESDecrypt(v, WorkContext.MallConfig.SecretKey);

            //数组第一项为uid，第二项为动作，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return AjaxResult("noauth", "您的权限不足");

            int uid = TypeHelper.StringToInt(result[0]);
            string action = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return AjaxResult("noauth", "您的权限不足");
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return AjaxResult("expired", "密钥已过期,请重新验证");

            string password = WebHelper.GetFormString("password");
            string confirmPwd = WebHelper.GetFormString("confirmPwd");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            //检查密码
            if (string.IsNullOrWhiteSpace(password))
            {
                return AjaxResult("password", "密码不能为空");
            }
            if (password.Length < 4 || password.Length > 32)
            {
                return AjaxResult("password", "密码必须大于3且不大于32个字符");
            }
            if (password != confirmPwd)
            {
                return AjaxResult("confirmpwd", "两次密码不相同");
            }

            string p = Users.CreateUserPassword(password, WorkContext.PartUserInfo.Salt);
            //设置新密码
            Users.UpdatePartUserPayPwd(WorkContext.PartUserInfo, password);

            string url = Url.Action("safesuccess", new RouteValueDictionary { { "act", "updatePayPassword" } });
            return AjaxResult("success", url);
        }

        /// <summary>
        /// 发送更新手机确认短信
        /// </summary>
        public ActionResult SendUpdateMobile()
        {
            string v = WebHelper.GetQueryString("v").Replace(" ", "+");
            //解密字符串
            string realV = SecureHelper.AESDecrypt(v, WorkContext.MallConfig.SecretKey);

            //数组第一项为uid，第二项为动作，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return AjaxResult("noauth", "您的权限不足");

            int uid = TypeHelper.StringToInt(result[0]);
            string action = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return AjaxResult("noauth", "您的权限不足");
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return AjaxResult("expired", "密钥已过期,请重新验证");

            string mobile = WebHelper.GetFormString("mobile");

            //检查手机号
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return AjaxResult("mobile", "手机号不能为空");
            }
            if (!ValidateHelper.IsMobile(mobile))
            {
                return AjaxResult("mobile", "手机号格式不正确");
            }
            int tempUid = Users.GetUidByMobile(mobile);
            if (tempUid > 0 && tempUid != WorkContext.Uid)
                return AjaxResult("mobile", "手机号已经存在");

            string mobileCode = Randoms.CreateRandomValue(6);
            //发送短信
            SMSes.SendSCUpdateSMS(mobile, mobileCode);
            //将验证值保存在session中
            Sessions.SetItem(WorkContext.Sid, "ucsuMobile", mobile);
            Sessions.SetItem(WorkContext.Sid, "ucsuMobileCode", mobileCode);

            return AjaxResult("success", "短信已发送,请查收");
        }
        /// <summary>
        /// 发送更新手机确认语音验证码
        /// </summary>
        public ActionResult SendUpdateMobileVoice()
        {
            string v = WebHelper.GetQueryString("v").Replace(" ", "+");
            //解密字符串
            string realV = SecureHelper.AESDecrypt(v, WorkContext.MallConfig.SecretKey);

            //数组第一项为uid，第二项为动作，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return AjaxResult("noauth", "您的权限不足");

            int uid = TypeHelper.StringToInt(result[0]);
            string action = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return AjaxResult("noauth", "您的权限不足");
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return AjaxResult("expired", "密钥已过期,请重新验证");

            string mobile = WebHelper.GetFormString("mobile");

            //检查手机号
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return AjaxResult("mobile", "手机号不能为空");
            }
            if (!ValidateHelper.IsMobile(mobile))
            {
                return AjaxResult("mobile", "手机号格式不正确");
            }
            int tempUid = Users.GetUidByMobile(mobile);
            if (tempUid > 0 && tempUid != WorkContext.Uid)
                return AjaxResult("mobile", "手机号已经存在");

            string mobileCode = Randoms.CreateRandomValue(4);
            //发送短信

            if (SMSes.SendSCUpdateSMSVoice(mobile, mobileCode))
            {

                //将验证值保存在session中
                Sessions.SetItem(WorkContext.Sid, "ucsuMobile", mobile);
                Sessions.SetItem(WorkContext.Sid, "ucsuMobileCode", mobileCode);

                return AjaxResult("success", "语音验证码已发送");
            }
            return AjaxResult("error", "系统繁忙，请稍后再试！！！");
        }
        /// <summary>
        /// 更新手机号
        /// </summary>
        public ActionResult UpdateMobile()
        {
            string v = WebHelper.GetQueryString("v").Replace(" ", "+");
            //解密字符串
            string realV = SecureHelper.AESDecrypt(v, WorkContext.MallConfig.SecretKey);

            //数组第一项为uid，第二项为动作，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return AjaxResult("noauth", "您的权限不足");

            int uid = TypeHelper.StringToInt(result[0]);
            string action = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return AjaxResult("noauth", "您的权限不足");
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return AjaxResult("expired", "密钥已过期,请重新验证");

            string mobile = WebHelper.GetFormString("mobile");
            string moibleCode = WebHelper.GetFormString("moibleCode");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            //检查手机号
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return AjaxResult("mobile", "手机号不能为空");
            }
            if (Sessions.GetValueString(WorkContext.Sid, "ucsuMobile") != mobile)
            {
                return AjaxResult("mobile", "接收手机不一致");
            }
            if (Users.IsExistMobile(mobile) || AccountUtils.IsUserExistForDirSale(mobile))
            {
                return AjaxResult("mobile", "该手机号已经存在");
            }

            //检查手机码
            if (string.IsNullOrWhiteSpace(moibleCode))
            {
                return AjaxResult("moiblecode", "手机码不能为空");
            }
            if (Sessions.GetValueString(WorkContext.Sid, "ucsuMobileCode") != moibleCode)
            {
                return AjaxResult("moiblecode", "手机码不正确");
            }

            //更新手机号
            Users.UpdateUserMobileByUid(WorkContext.Uid, mobile);
            //发放验证手机积分
            Credits.SendVerifyMobileCredits(ref WorkContext.PartUserInfo, DateTime.Now);

            string url = Url.Action("safesuccess", new RouteValueDictionary { { "act", "updateMobile" } });
            return AjaxResult("success", url);
        }

        /// <summary>
        /// 发送更新邮箱确认邮件
        /// </summary>
        public ActionResult SendUpdateEmail()
        {
            string v = WebHelper.GetQueryString("v").Replace(" ", "+");
            //解密字符串
            string realV = SecureHelper.AESDecrypt(v, WorkContext.MallConfig.SecretKey);

            //数组第一项为uid，第二项为动作，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return AjaxResult("noauth", "您的权限不足");

            int uid = TypeHelper.StringToInt(result[0]);
            string action = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return AjaxResult("noauth", "您的权限不足");
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return AjaxResult("expired", "密钥已过期,请重新验证");

            string email = WebHelper.GetFormString("email");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            //检查验证码
            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("verifycode", "验证码不能为空");
            }
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("verifycode", "验证码不正确");
            }

            //检查邮箱
            if (string.IsNullOrWhiteSpace(email))
            {
                return AjaxResult("email", "邮箱不能为空");
            }
            if (!ValidateHelper.IsEmail(email))
            {
                return AjaxResult("email", "邮箱格式不正确");
            }
            if (!SecureHelper.IsSafeSqlString(email, false))
            {
                return AjaxResult("email", "邮箱已经存在");
            }
            int tempUid = Users.GetUidByEmail(email);
            if (tempUid > 0 && tempUid != WorkContext.Uid)
                return AjaxResult("email", "邮箱已经存在");


            string v2 = MallUtils.AESEncrypt(string.Format("{0},{1},{2},{3}", WorkContext.Uid, email, DateTime.Now, Randoms.CreateRandomValue(6)));
            string url = string.Format("http://{0}{1}", Request.Url.Authority, Url.Action("updateemail", new RouteValueDictionary { { "v", v2 } }));

            //发送验证邮件
            Emails.SendSCUpdateEmail(email, WorkContext.UserName, url);
            return AjaxResult("success", "邮件已经发送，请前往你的邮箱进行验证");
        }

        /// <summary>
        /// 更新邮箱
        /// </summary>
        public ActionResult UpdateEmail()
        {
            string v = WebHelper.GetQueryString("v").Replace(" ", "+");
            //解密字符串
            string realV = SecureHelper.AESDecrypt(v, WorkContext.MallConfig.SecretKey);

            //数组第一项为uid，第二项为邮箱名，第三项为验证时间,第四项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 4)
                return HttpNotFound();

            int uid = TypeHelper.StringToInt(result[0]);
            string email = result[1];
            DateTime time = TypeHelper.StringToDateTime(result[2]);

            //判断当前用户是否为验证用户
            if (uid != WorkContext.Uid)
                return HttpNotFound();
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return PromptView("此链接已经失效，请重新验证");
            int tempUid = Users.GetUidByEmail(email);
            if (tempUid > 0 && tempUid != WorkContext.Uid)
                return PromptView("此链接已经失效，邮箱已经存在");

            //更新邮箱名
            Users.UpdateUserEmailByUid(WorkContext.Uid, email);
            //发放验证邮箱积分
            Credits.SendVerifyEmailCredits(ref WorkContext.PartUserInfo, DateTime.Now);

            return RedirectToAction("safesuccess", new RouteValueDictionary { { "act", "updateEmail" }, { "remark", email } });
        }

        /// <summary>
        /// 安全成功
        /// </summary>
        public ActionResult SafeSuccess()
        {
            string action = WebHelper.GetQueryString("act").ToLower();
            string remark = WebHelper.GetQueryString("remark");

            if (action.Length == 0 || !CommonHelper.IsInArray(action, new string[4] { "updatepassword", "updatemobile", "updateemail", "updatepaypassword" }))
                return HttpNotFound();

            SafeSuccessModel model = new SafeSuccessModel();
            model.Action = action;
            model.Remark = remark;

            return View(model);
        }

        #endregion

        #region 订单

        /// <summary>
        /// 订单列表
        /// </summary>
        public ActionResult OrderList()
        {
            int page = WebHelper.GetQueryInt("page");
            string startAddTime = WebHelper.GetQueryString("startAddTime");
            string endAddTime = WebHelper.GetQueryString("endAddTime");
            int orderState = WebHelper.GetQueryInt("orderState");

            PageModel pageModel = new PageModel(15, page, Orders.GetUserOrderCount(WorkContext.Uid, startAddTime, endAddTime, orderState));

            DataTable orderList = Orders.GetUserOrderList(WorkContext.Uid, pageModel.PageSize, pageModel.PageNumber, startAddTime, endAddTime, orderState);
            StringBuilder oidList = new StringBuilder();
            foreach (DataRow row in orderList.Rows)
            {
                oidList.AppendFormat("{0},", row["oid"]);
            }
            if (oidList.Length > 0)
                oidList.Remove(oidList.Length - 1, 1);

            OrderListModel model = new OrderListModel()
            {
                PageModel = pageModel,
                OrderList = orderList,
                OrderProductList = Orders.GetOrderProductList(oidList.ToString()),
                StartAddTime = startAddTime,
                EndAddTime = endAddTime,
                OrderState = orderState
            };
            if (page >= 2)
                return PartialView("orderlistAjax", model);
            return View(model);
        }


        /// <summary>
        /// 订单信息
        /// </summary>
        public ActionResult OrderInfo()
        {
            int oid = WebHelper.GetQueryInt("oid");
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return PromptView("订单不存在");

            OrderInfoModel model = new OrderInfoModel();
            model.OrderInfo = orderInfo;
            model.RegionInfo = Regions.GetRegionById(orderInfo.RegionId);
            model.OrderProductList = AdminOrders.GetOrderProductList(oid);

            return View(model);
        }

        /// <summary>
        /// 订单动作列表
        /// </summary>
        public ActionResult OrderActionList()
        {
            int oid = WebHelper.GetQueryInt("oid");
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return PromptView("订单不存在");

            OrderActionListModel model = new OrderActionListModel();
            model.OrderInfo = orderInfo;
            model.OrderActionList = OrderActions.GetOrderActionList(oid);

            return View(model);
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        public ActionResult CancelOrder()
        {
            int oid = WebHelper.GetFormInt("oid");
            int cancelReason = WebHelper.GetFormInt("cancelReason");

            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return AjaxResult("noorder", "订单不存在");

            if (!(orderInfo.OrderState == (int)OrderState.WaitPaying || (orderInfo.OrderState <= (int)OrderState.Confirmed && orderInfo.PayMode == 1)))
                return AjaxResult("donotcancel", "订单当前不能取消");
            // if (orderInfo.MainOid > 0 || orderInfo.SubOid > 0)
            if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId") || orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId"))
                //return AjaxResult("donotcancel", "该订单为必选套餐包，不能单独取消");
                return RedirectToAction("AgentCancelOrder", new RouteValueDictionary { { "oid", orderInfo.Oid } });

            CouponInfo couponInfo = Coupons.GetCouponBywhere(string.Format(" [createoid]={0} ", orderInfo.Oid));
            if (couponInfo != null)
            {
                if (couponInfo.Oid > 0)
                    return AjaxResult("donotcancel", "订单赠送优惠券已使用，当前不能取消订单");
            }
            ExChangeCouponsInfo exInfo = ExChangeCoupons.GetModel(string.Format(" [createoid]={0} ", orderInfo.Oid));
            if (exInfo != null)
            {
                if (exInfo.oid > 0)
                    return AjaxResult("donotcancel", "订单赠送兑换码已使用，不能取消订单");
            }
            if (TypeHelper.StringToInt(orderInfo.ExtOrderId) > 0)
                return AjaxResult("donotcancel", "领用赠品订单不能取消");
            List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
            //判断是否满足红包退回
            if (orderInfo.OrderState == (int)OrderState.Confirmed)
            {
                //存在推广产品配置退回99红包

                if (orderProductList.Exists(x => x.Pid == TypeHelper.StringToInt(WebSiteConfig.ActiveProduct, 0)))
                {
                    List<AccountInfo> info = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid);
                    OrderProductInfo orderProductInfo = orderProductList.Find(x => x.Pid == TypeHelper.StringToInt(WebSiteConfig.ActiveProduct, 0));
                    if (info.Find(x => x.AccountId == (int)AccountType.红包账户).Banlance >= 99 * orderProductInfo.BuyCount)
                        AccountUtils.ReturnActiveHongBao(WorkContext.PartUserInfo, orderInfo, 99 * orderProductInfo.BuyCount, WorkContext.Uid, DateTime.Now);
                    else
                    {
                        return AjaxResult("donotcancel", "订单当前不能取消,红包金额不够，请联系客服");
                    }
                }

                if (orderProductList.Exists(x => StringHelper.StrContainsForNum(WebHelper.GetConfigSettings("CoffeeQuanPid"), x.Pid.ToString())))
                { return AjaxResult("donotcancel", "订单含有汇购卡券产品不能取消，请联系客服"); }
            }



            //取消订单
            Orders.CancelOrder(ref WorkContext.PartUserInfo, orderInfo, WorkContext.Uid, DateTime.Now);

            //int completeCount = Orders.GetUserOrderCount(WorkContext.Uid, "", "", (int)OrderState.Completed);
            //if (completeCount <= 0)
            //    OrderUtils.UpdateFXUserSates(WorkContext.Uid, 0);

            string sqlStr = string.Format(" orderstate>={0} and orderstate<={1}  and uid={2} ", (int)OrderState.Confirmed, (int)OrderState.Completed, orderInfo.Uid);
            string sqlStr2 = string.Format(" orderstate>={0} and orderstate<={1} and uid={2} and orderamount>=15800 ", (int)OrderState.Confirmed, (int)OrderState.Completed, orderInfo.Uid);
            List<OrderInfo> UserOrderList = Orders.GetOrderListByWhere(sqlStr);
            List<OrderInfo> UserOrderList2 = Orders.GetOrderListByWhere(sqlStr2);
            if (UserOrderList.Count <= 0)
                OrderUtils.UpdateFXUserSates(WorkContext.Uid, 0);
            if (UserOrderList2.Count <= 0 && UserOrderList.Count > 0)
                OrderUtils.UpdateFXUserSates(WorkContext.Uid, 1);

            //如果已支付并支付金额大于0取消订单 要发起退款操作  非在线支付 取消订单应生成退款请求
            if (orderInfo.OrderState == (int)OrderState.Confirmed && orderInfo.SurplusMoney > 0)
            {
                if (orderInfo.PayMode == 1 && orderInfo.PaySystemName == "wechatpay")
                {
                    PluginInfo PayPlugin = Plugins.GetPayPluginBySystemName(orderInfo.PaySystemName);
                    return RedirectToAction("ReFund", "wechat", new RouteValueDictionary { { "oid", orderInfo.Oid } });
                }
                else if (orderInfo.PayMode == 1 && orderInfo.PaySystemName == "alipay")//支付宝退款需后台操作，此处只生成退款记录，
                {

                    OrderRefunds.ApplyRefund(new OrderRefundInfo()
                    {
                        StoreId = orderInfo.StoreId,
                        StoreName = orderInfo.StoreName,
                        Oid = oid,
                        OSN = orderInfo.OSN,
                        Uid = orderInfo.Uid,
                        State = 0,
                        ApplyTime = DateTime.Now,
                        PayMoney = orderInfo.SurplusMoney,
                        RefundMoney = orderInfo.SurplusMoney,
                        RefundSN = "",
                        RefundFriendName = orderInfo.PayFriendName,
                        RefundSystemName = orderInfo.PaySystemName,
                        PayFriendName = orderInfo.PayFriendName,
                        PaySystemName = orderInfo.PaySystemName,
                        RefundTime = DateTime.Now,
                        PaySN = orderInfo.PaySN,
                        RefundTranSN = "",//记录退款流水号 
                        ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",支付成功取消订单订单退款"
                    });
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = oid,
                        Uid = WorkContext.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Cancel,
                        ActionTime = DateTime.Now,
                        ActionDes = "您取消了订单,请等待系统退款,退款会在3个工作日内将退款返回至帐号中"
                    });
                }
                else if (orderInfo.PayMode == 1 && orderInfo.PaySystemName == "IPSpay")//环迅退款需后台操作，此处只生成退款记录，
                {

                    OrderRefunds.ApplyRefund(new OrderRefundInfo()
                    {
                        StoreId = orderInfo.StoreId,
                        StoreName = orderInfo.StoreName,
                        Oid = oid,
                        OSN = orderInfo.OSN,
                        Uid = orderInfo.Uid,
                        State = 0,
                        ApplyTime = DateTime.Now,
                        PayMoney = orderInfo.SurplusMoney,
                        RefundMoney = orderInfo.SurplusMoney,
                        RefundSN = "",
                        RefundFriendName = orderInfo.PayFriendName,
                        RefundSystemName = orderInfo.PaySystemName,
                        PayFriendName = orderInfo.PayFriendName,
                        PaySystemName = orderInfo.PaySystemName,
                        RefundTime = DateTime.Now,
                        PaySN = orderInfo.PaySN,
                        RefundTranSN = "",//记录退款流水号 
                        ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",支付成功取消订单订单退款"
                    });
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = oid,
                        Uid = WorkContext.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Cancel,
                        ActionTime = DateTime.Now,
                        ActionDes = "您取消了订单,请等待系统退款,退款会在3个工作日内将退款返回至帐号中"
                    });
                }
                else if (orderInfo.PayMode == 2)
                {
                    OrderRefunds.ApplyRefund(new OrderRefundInfo()
                    {
                        StoreId = orderInfo.StoreId,
                        StoreName = orderInfo.StoreName,
                        Oid = oid,
                        OSN = orderInfo.OSN,
                        Uid = orderInfo.Uid,
                        State = 0,
                        ApplyTime = DateTime.Now,
                        PayMoney = orderInfo.SurplusMoney,
                        RefundMoney = orderInfo.SurplusMoney,
                        RefundSN = "",
                        RefundFriendName = orderInfo.PayFriendName,
                        RefundSystemName = orderInfo.PaySystemName,
                        PayFriendName = orderInfo.PayFriendName,
                        PaySystemName = orderInfo.PaySystemName,
                        RefundTime = DateTime.Now,
                        PaySN = orderInfo.PaySN,
                        RefundTranSN = "",//记录退款流水号 
                        ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",支付成功取消订单订单退款"
                    });
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = oid,
                        Uid = WorkContext.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Cancel,
                        ActionTime = DateTime.Now,
                        ActionDes = "您取消了付款成功的订单,付款方式为银行汇款，请联系客服进行退款"
                    });
                }
            }
            else
            {
                //创建订单处理
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = oid,
                    Uid = WorkContext.Uid,
                    RealName = "本人",
                    ActionType = (int)OrderActionType.Cancel,
                    ActionTime = DateTime.Now,
                    ActionDes = "您取消了订单"
                });
            }
            return AjaxResult("success", oid.ToString());
        }

        /// <summary>
        /// 环球捕手申请退款-前台申请退款，后台审核后退款，此处指生成退款记录，不进行退款操作
        /// </summary>
        public ActionResult BSApplyOrderRdFund()
        {
            int oid = WebHelper.GetFormInt("oid");
            int cancelReason = WebHelper.GetFormInt("cancelReason");

            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return AjaxResult("noorder", "订单不存在");

            if (orderInfo.OrderState != (int)OrderState.Completed)
                return AjaxResult("donotcancel", "订单不是已完成状态，当前不能取消");

            //取消订单
            //将订单状态设为取消状态
            Orders.UpdateOrderState(orderInfo.Oid, OrderState.Cancelled);//将订单状态设为取消状态
            //创建订单处理
            OrderActions.CreateOrderAction(new OrderActionInfo()
            {
                Oid = oid,
                Uid = WorkContext.Uid,
                RealName = "本人",
                ActionType = (int)OrderActionType.Cancel,
                ActionTime = DateTime.Now,
                ActionDes = "您申请了订单退款，订单已被取消，等待系统审核后为您退款，退款将在3-7个工作日原路退回！"
            });

            //如果已支付并支付金额大于0取消订单 要发起退款操作  非在线支付 取消订单应生成退款请求

            //微信退款流程
            if (orderInfo.PayMode == 1 && orderInfo.PaySystemName == "wechatpay")//退款需后台操作，此处只生成退款记录，
            {
                OrderRefunds.ApplyRefund(new OrderRefundInfo()
                {
                    StoreId = orderInfo.StoreId,
                    StoreName = orderInfo.StoreName,
                    Oid = oid,
                    OSN = orderInfo.OSN,
                    Uid = orderInfo.Uid,
                    State = 0,
                    ApplyTime = DateTime.Now,
                    PayMoney = orderInfo.SurplusMoney,
                    RefundMoney = orderInfo.SurplusMoney,
                    RefundSN = "",
                    RefundFriendName = orderInfo.PayFriendName,
                    RefundSystemName = orderInfo.PaySystemName,
                    PayFriendName = orderInfo.PayFriendName,
                    PaySystemName = orderInfo.PaySystemName,
                    RefundTime = DateTime.Now,
                    PaySN = orderInfo.PaySN,
                    RefundTranSN = "",//记录退款流水号 
                    ReMark = "环球捕手订单申请退款,微信退款暂不完成，原因:基本账户余额不足，审核后确认退款。"
                });
            }

            return AjaxResult("success", oid.ToString());
        }


        /// <summary>
        /// 代理订单取消订单
        /// </summary>
        public ActionResult AgentCancelOrder()
        {
            int oid = WebHelper.GetQueryInt("oid");
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return AjaxResult("noorder", "订单不存在");

            if (!(orderInfo.OrderState == (int)OrderState.WaitPaying || (orderInfo.OrderState <= (int)OrderState.Confirmed && orderInfo.PayMode == 1)))
                return AjaxResult("donotcancel", "订单当前不能取消");

            //if (orderInfo.MainOid > 0)
            //    return AjaxResult("donotcancel", "该订单为必选套餐包，不能单独取消");

            CouponInfo couponInfo = Coupons.GetCouponBywhere(string.Format(" [createoid]={0} ", orderInfo.Oid));
            if (couponInfo != null)
            {
                if (couponInfo.Oid > 0)
                    return AjaxResult("donotcancel", "订单赠送优惠券已使用，当前不能取消订单");
            }
            ExChangeCouponsInfo exInfo = ExChangeCoupons.GetModel(string.Format(" [createoid]={0} ", orderInfo.Oid));
            if (exInfo != null)
            {
                if (exInfo.oid > 0)
                    return AjaxResult("donotcancel", "订单赠送兑换码已使用，不能取消订单");
            }
            List<OrderInfo> AllAgentOrders = Orders.GetOrderListByWhere(string.Format(" (oid ={0} OR oid={1} OR oid={2} ) ", orderInfo.Oid, orderInfo.MainOid, orderInfo.SubOid));

            PartUserInfo user = Users.GetPartUserById(WorkContext.Uid);
            bool isTrue = true;
            if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId") && user.UserSource == 1)//单套餐包取消判断下级会员
            {
                List<UserInfo> chilerenuserList = Users.GetSubRecommendListByPid(user, 1, 2000).FindAll(x => x.AgentType > 0);
                if (chilerenuserList.Count > 0)
                    return AjaxResult("donotcancel", "存在下级代理会员，订单不能取消");
            }

            OrderInfo aorder = AllAgentOrders.Find(x => x.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"));
            if (aorder != null)
            {
                AgentStockDetailInfo detailInfo = new AgentStockDetail().GetModel(string.Format(" uid={0}  and ordercode='{1}' ", user.Uid, aorder.OSN));
                if (detailInfo != null)//存在代理库存记录，说明为代理定级订单
                {
                    List<UserInfo> chilerenuserList = Users.GetSubRecommendListByPid(user, 1, 2000).FindAll(x => x.AgentType > 0);
                    if (chilerenuserList.Count > 0)
                        return PromptView("存在下级代理会员，该订单不能取消");

                    List<OrderProductInfo> oplist = Orders.GetOrderProductList(aorder.Oid);
                    List<AgentSendOrderInfo> sendList = new AgentSendOrder().GetList(string.Format(" uid={0} and pid in ({1})", user.Uid, string.Join(",", oplist.Select(x => x.Pid))));
                    if (sendList.Count > 0)
                        return PromptView("存在要货订单，该订单不能取消");

                    List<AgentStockInfo> stockList = new AgentStock().GetList(string.Format(" uid={0} and pid in ({1}) ", user.Uid, string.Join(",", oplist.Select(x => x.Pid))));
                    foreach (var item in oplist)
                    {
                        AgentStockInfo agent = stockList.Find(x => x.Pid == item.Pid);
                        if (agent != null)
                        {
                            if (agent.Balance < item.BuyCount)
                            {
                                isTrue = false;
                                break;
                            }
                        }
                    }
                }
            }
            if (!isTrue)
                return AjaxResult("donotcancel", "取消数量大于库存余额，订单不能取消");

            //代理订单取消

            foreach (var item in AllAgentOrders)
            {
                //取消订单
                Orders.CancelOrder(ref WorkContext.PartUserInfo, item, WorkContext.Uid, DateTime.Now);
                //int completeCount = Orders.GetUserOrderCount(WorkContext.Uid, "", "", (int)OrderState.Confirmed);
                string sqlStr = string.Format(" orderstate>={0} and orderstate<={1}  and uid={2} ", (int)OrderState.Confirmed, (int)OrderState.Completed, item.Uid);
                string sqlStr2 = string.Format(" orderstate>={0} and orderstate<={1} and uid={2} and orderamount>=15800 ", (int)OrderState.Confirmed, (int)OrderState.Completed, item.Uid);
                List<OrderInfo> UserOrderList = Orders.GetOrderListByWhere(sqlStr);
                List<OrderInfo> UserOrderList2 = Orders.GetOrderListByWhere(sqlStr2);
                if (UserOrderList.Count <= 0)
                    OrderUtils.UpdateFXUserSates(WorkContext.Uid, 0);
                if (UserOrderList2.Count <= 0 && UserOrderList.Count > 0)
                    OrderUtils.UpdateFXUserSates(WorkContext.Uid, 1);
            }

            foreach (var item in AllAgentOrders)
            {
                //如果已支付并支付金额大于0取消订单 要发起退款操作  非在线支付 取消订单应生成退款请求
                if ((item.OrderState == (int)OrderState.Confirmed || item.OrderState == (int)OrderState.Completed) && item.SurplusMoney > 0)
                {
                    if (item.PayMode == 1 && item.PaySystemName == "wechatpay")
                    {
                        PluginInfo PayPlugin = Plugins.GetPayPluginBySystemName(item.PaySystemName);
                        string oids = string.Join(",", AllAgentOrders.Select(x => x.Oid));
                        return RedirectToAction("ReFundForBatch", "wechat", new RouteValueDictionary { { "oids", oids } });
                    }
                    else if (item.PayMode == 1 && item.PaySystemName == "alipay")//支付宝退款需后台操作，此处只生成退款记录，
                    {

                        OrderRefunds.ApplyRefund(new OrderRefundInfo()
                        {
                            StoreId = item.StoreId,
                            StoreName = item.StoreName,
                            Oid = oid,
                            OSN = item.OSN,
                            Uid = item.Uid,
                            State = 0,
                            ApplyTime = DateTime.Now,
                            PayMoney = item.SurplusMoney,
                            RefundMoney = item.SurplusMoney,
                            RefundSN = "",
                            RefundFriendName = item.PayFriendName,
                            RefundSystemName = item.PaySystemName,
                            PayFriendName = item.PayFriendName,
                            PaySystemName = item.PaySystemName,
                            RefundTime = DateTime.Now,
                            PaySN = item.PaySN,
                            RefundTranSN = "",//记录退款流水号 
                            ReMark = Enum.GetName(typeof(OrderSource), item.OrderSource) + ",支付成功取消订单订单退款"
                        });
                        //创建订单处理
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = oid,
                            Uid = WorkContext.Uid,
                            RealName = "本人",
                            ActionType = (int)OrderActionType.Cancel,
                            ActionTime = DateTime.Now,
                            ActionDes = "您取消了订单,请等待系统退款,退款会在1-3个工作日内将退款返回至帐号中"
                        });
                    }
                    else if (item.PayMode == 1 && item.PaySystemName == "IPSpay")//环迅退款需后台操作，此处只生成退款记录，
                    {

                        OrderRefunds.ApplyRefund(new OrderRefundInfo()
                        {
                            StoreId = item.StoreId,
                            StoreName = item.StoreName,
                            Oid = oid,
                            OSN = item.OSN,
                            Uid = item.Uid,
                            State = 0,
                            ApplyTime = DateTime.Now,
                            PayMoney = item.SurplusMoney,
                            RefundMoney = item.SurplusMoney,
                            RefundSN = "",
                            RefundFriendName = item.PayFriendName,
                            RefundSystemName = item.PaySystemName,
                            PayFriendName = item.PayFriendName,
                            PaySystemName = item.PaySystemName,
                            RefundTime = DateTime.Now,
                            PaySN = item.PaySN,
                            RefundTranSN = "",//记录退款流水号 
                            ReMark = Enum.GetName(typeof(OrderSource), item.OrderSource) + ",支付成功取消订单订单退款"
                        });
                        //创建订单处理
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = oid,
                            Uid = WorkContext.Uid,
                            RealName = "本人",
                            ActionType = (int)OrderActionType.Cancel,
                            ActionTime = DateTime.Now,
                            ActionDes = "您取消了订单,请等待系统退款,退款会在1-3个工作日内将退款返回至帐号中"
                        });
                    }
                    else if (item.PayMode == 2)
                    {
                        OrderRefunds.ApplyRefund(new OrderRefundInfo()
                        {
                            StoreId = item.StoreId,
                            StoreName = item.StoreName,
                            Oid = oid,
                            OSN = item.OSN,
                            Uid = item.Uid,
                            State = 0,
                            ApplyTime = DateTime.Now,
                            PayMoney = item.SurplusMoney,
                            RefundMoney = item.SurplusMoney,
                            RefundSN = "",
                            RefundFriendName = item.PayFriendName,
                            RefundSystemName = item.PaySystemName,
                            PayFriendName = item.PayFriendName,
                            PaySystemName = item.PaySystemName,
                            RefundTime = DateTime.Now,
                            PaySN = item.PaySN,
                            RefundTranSN = "",//记录退款流水号 
                            ReMark = Enum.GetName(typeof(OrderSource), item.OrderSource) + ",支付成功取消订单订单退款"
                        });
                        //创建订单处理
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = oid,
                            Uid = WorkContext.Uid,
                            RealName = "本人",
                            ActionType = (int)OrderActionType.Cancel,
                            ActionTime = DateTime.Now,
                            ActionDes = "您取消了付款成功的订单,付款方式为银行汇款，请联系客服进行退款"
                        });
                    }
                }
                else
                {
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = oid,
                        Uid = WorkContext.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Cancel,
                        ActionTime = DateTime.Now,
                        ActionDes = "您取消了订单"
                    });
                }

            }

            return AjaxResult("success", oid.ToString());
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        [HttpGet]
        public ActionResult ConfirmReceiving(int oid = -1)
        {
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null)
                return AjaxResult("error", "[{\"key\":\"emptyorder\",\"msg\":\"订单不存在\"}]", true);
            if (orderInfo.OrderState != (int)OrderState.Sended)
                return AjaxResult("error", "[{\"key\":\"errororderstate\",\"msg\":\"订单还未发货，不能完成订单\"}]", true);

            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            Orders.CompleteOrderNew(ref partUserInfo, orderInfo, DateTime.Now, WorkContext.IP);
            CreateOrderAction(oid, "本人", OrderActionType.Complete, "订单已完成，感谢您在" + WorkContext.MallConfig.MallName + "购物，欢迎您再次光临");
            //AddMallAdminLog("完成订单", "完成订单,订单ID为:" + oid);
            return AjaxResult("success", string.Format("订单已完成，感谢您在{0}购物，欢迎再次光临", WorkContext.MallConfig.SiteTitle));

        }

        /// <summary>
        /// 退货申请
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ReturnApply(int oid = -1)
        {
            bool state = true;
            string message = string.Empty;
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null)
            {
                state = false;
                message = "订单不存在";
                return View("returninfo", new ReturnModel()
                {
                    State = state,
                    Message = message
                });
            }
            if (orderInfo.OrderState != (int)OrderState.Completed && (TypeHelper.ObjectToDateTime(orderInfo.ReceivingTime).AddDays(7) <= DateTime.Now) && orderInfo.ReturnType == 1)
            {
                state = false;
                message = "订单当前不满足退货条件，不能退货";
                return View("returninfo", new ReturnModel()
                {
                    State = state,
                    Message = message
                });
            }
            if (orderInfo.CashDiscount > 0)
            {
                state = false;
                message = "订单使用了汇购卡进行支付，不能退货";
                return View("returninfo", new ReturnModel()
                {
                    State = state,
                    Message = message
                });
            }
            List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
            if (orderProductList.Exists(x => StringHelper.StrContainsForNum(WebHelper.GetConfigSettings("CoffeeQuanPid"), x.Pid.ToString())))
            {
                state = false;
                message = "汇购卡不能退货";
                return View("returninfo", new ReturnModel()
                {
                    State = state,
                    Message = message
                });

            }
            CouponInfo couponInfo = Coupons.GetCouponBywhere(string.Format(" [createoid]={0} ", orderInfo.Oid));
            if (couponInfo != null)
            {
                if (couponInfo.Oid > 0)
                {
                    state = false;
                    message = "订单赠送优惠券已使用，当前不能取消订单";
                    return View("returninfo", new ReturnModel()
                    {
                        State = state,
                        Message = message
                    });
                }
            }
            ExChangeCouponsInfo exInfo = ExChangeCoupons.GetModel(string.Format(" [createoid]={0} ", orderInfo.Oid));
            if (exInfo != null)
            {
                if (exInfo.oid > 0)
                {
                    state = false;
                    message = "订单赠送兑换码已使用，不能取消订单";
                    return View("returninfo", new ReturnModel()
                    {
                        State = state,
                        Message = message
                    });
                }
            }
            List<OrderInfo> AllAgentOrders = Orders.GetOrderListByWhere(string.Format(" (oid ={0} OR oid={1} OR oid={2} ) ", orderInfo.Oid, orderInfo.MainOid, orderInfo.SubOid));

            PartUserInfo user = Users.GetPartUserById(WorkContext.Uid);
            bool isTrue = true;
            if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId") && user.UserSource == 1)//单套餐包取消判断下级会员
            {
                List<UserInfo> chilerenuserList = Users.GetSubRecommendListByPid(user, 1, 2000).FindAll(x => x.AgentType > 0);
                if (chilerenuserList.Count > 0)
                {
                    state = false;
                    message = "存在下级代理会员，订单不能退货";
                    return View("returninfo", new ReturnModel()
                    {
                        State = state,
                        Message = message
                    });
                }
            }
            OrderInfo aorder = AllAgentOrders.Find(x => x.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"));
            if (aorder != null)
            {
                AgentStockDetailInfo detailInfo = new AgentStockDetail().GetModel(string.Format(" uid={0}  and ordercode='{1}' ", user.Uid, orderInfo.OSN));
                if (detailInfo != null)//存在代理库存记录，说明为代理定级订单
                {
                    List<UserInfo> chilerenuserList = Users.GetSubRecommendListByPid(user, 1, 2000).FindAll(x => x.AgentType > 0);
                    if (chilerenuserList.Count > 0)
                        return PromptView("存在下级代理会员，该订单不能取消");

                    List<OrderProductInfo> oplist = Orders.GetOrderProductList(aorder.Oid);
                    List<AgentSendOrderInfo> sendList = new AgentSendOrder().GetList(string.Format(" uid={0} and pid in ({1})", user.Uid, string.Join(",", oplist.Select(x => x.Pid))));
                    if (sendList.Count > 0)
                    {
                        state = false;
                        message = "存在要货订单，订单不能退货";
                        return View("returninfo", new ReturnModel()
                        {
                            State = state,
                            Message = message
                        });
                    }

                    List<AgentStockInfo> stockList = new AgentStock().GetList(string.Format(" uid={0} and pid in ({1}) ", user.Uid, string.Join(",", oplist.Select(x => x.Pid))));
                    foreach (var item in oplist)
                    {
                        AgentStockInfo agent = stockList.Find(x => x.Pid == item.Pid);
                        if (agent != null)
                        {
                            if (agent.Balance < item.BuyCount)
                            {
                                isTrue = false;
                                break;
                            }
                        }
                    }
                }
            }

            if (!isTrue)
            {
                state = false;
                message = "退货数量大于库存余额，订单不能取消";
                return View("returninfo", new ReturnModel()
                {
                    State = state,
                    Message = message
                });
            }
            //if (orderInfo.MainOid > 0)
            //{
            //    state = false;
            //    message = "该订单不能单独退货";
            //    return View("returninfo", new ReturnModel()
            //    {
            //        State = state,
            //        Message = message
            //    });
            //}
            ReturnModel model = new ReturnModel()
            {
                State = state,
                Message = message,
                OrderInfo = orderInfo
            };

            return View("returninfo", model);

        }
        /// <summary>
        /// 退货申请
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReturnApply(int oid = -1, string returnDesc = "")
        {
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null)
                return AjaxResult("error", "[{\"key\":\"emptyorder\",\"msg\":\"订单不存在\"}]", true);
            if (orderInfo.OrderState != (int)OrderState.Completed && (TypeHelper.ObjectToDateTime(orderInfo.ReceivingTime).AddDays(7) <= DateTime.Now))
                return AjaxResult("error", "[{\"key\":\"errororderstate\",\"msg\":\"订单当前不能退货\"}]", true);

            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            //if (orderInfo.MainOid > 0)
            if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId") || orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId"))
                return RedirectToAction("AgentReturnApply", new RouteValueDictionary { { "oid", orderInfo.Oid } });

            AdminOrders.ReturnApply(orderInfo, WorkContext.Uid, DateTime.Now, returnDesc);
            CreateOrderAction(oid, "本人", OrderActionType.Return, "订单已申请退货，请等待系统审核");
            return AjaxResult("success", "退货申请成功");

        }
        /// <summary>
        /// 退货申请--代理订单
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult AgentReturnApply(int oid = -1, string returnDesc = "")
        {
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null)
                return AjaxResult("error", "[{\"key\":\"emptyorder\",\"msg\":\"订单不存在\"}]", true);
            if (orderInfo.OrderState != (int)OrderState.Completed && (TypeHelper.ObjectToDateTime(orderInfo.ReceivingTime).AddDays(7) <= DateTime.Now))
                return AjaxResult("error", "[{\"key\":\"errororderstate\",\"msg\":\"订单当前不能退货\"}]", true);

            List<OrderInfo> AllAgentOrders = Orders.GetOrderListByWhere(string.Format(" (oid ={0} OR oid={1} OR oid={2} ) ", orderInfo.Oid, orderInfo.MainOid, orderInfo.SubOid));

            PartUserInfo user = Users.GetPartUserById(WorkContext.Uid);
            bool isTrue = true;
            if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId") && user.UserSource == 1)//单套餐包取消判断下级会员
            {
                List<UserInfo> chilerenuserList = Users.GetSubRecommendListByPid(user, 1, 2000).FindAll(x => x.AgentType > 0);
                if (chilerenuserList.Count > 0)
                    return AjaxResult("donotcancel", "存在下级代理会员，订单不能取消");
            }

            OrderInfo aorder = AllAgentOrders.Find(x => x.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"));
            if (aorder != null)
            {
                AgentStockDetailInfo detailInfo = new AgentStockDetail().GetModel(string.Format(" uid={0}  and ordercode='{1}' ", user.Uid, aorder.OSN));
                if (detailInfo != null)//存在代理库存记录，说明为代理定级订单
                {
                    List<UserInfo> chilerenuserList = Users.GetSubRecommendListByPid(user, 1, 2000).FindAll(x => x.AgentType > 0);
                    if (chilerenuserList.Count > 0)
                        return PromptView("存在下级代理会员，该订单不能取消");

                    List<OrderProductInfo> oplist = Orders.GetOrderProductList(aorder.Oid);
                    List<AgentSendOrderInfo> sendList = new AgentSendOrder().GetList(string.Format(" uid={0} and pid in ({1})", user.Uid, string.Join(",", oplist.Select(x => x.Pid))));
                    if (sendList.Count > 0)
                        return PromptView("存在要货订单，该订单不能取消");

                    List<AgentStockInfo> stockList = new AgentStock().GetList(string.Format(" uid={0} and pid in ({1}) ", user.Uid, string.Join(",", oplist.Select(x => x.Pid))));
                    foreach (var item in oplist)
                    {
                        AgentStockInfo agent = stockList.Find(x => x.Pid == item.Pid);
                        if (agent != null)
                        {
                            if (agent.Balance < item.BuyCount)
                            {
                                isTrue = false;
                                break;
                            }
                        }
                    }
                }
            }

            if (!isTrue)
                return AjaxResult("donotcancel", "退货数量大于库存余额，订单不能取消");
            //代理订单取消
            foreach (var item in AllAgentOrders)
            {
                AdminOrders.ReturnApply(item, WorkContext.Uid, DateTime.Now, returnDesc);
                CreateOrderAction(item.Oid, "本人", OrderActionType.Return, "订单已申请退货，请等待系统审核");
            }

            return AjaxResult("success", "退货申请成功");

        }
        /// <summary>
        /// 退货申请--旧方法
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult ReturnApplyOld(int oid = -1)
        {
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null)
                return AjaxResult("error", "[{\"key\":\"emptyorder\",\"msg\":\"订单不存在\"}]", true);
            if (orderInfo.OrderState != (int)OrderState.Completed)
                return AjaxResult("error", "[{\"key\":\"errororderstate\",\"msg\":\"订单当前不能退货\"}]", true);

            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);

            //判断是否满足红包退回
            if (orderInfo.OrderState == (int)OrderState.Completed)
            {
                //存在推广产品配置退回99红包
                List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
                if (orderProductList.Exists(x => x.Pid == TypeHelper.StringToInt(WebSiteConfig.ActiveProduct, 0)))
                {
                    List<AccountInfo> info = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid);
                    OrderProductInfo orderProductInfo = orderProductList.Find(x => x.Pid == TypeHelper.StringToInt(WebSiteConfig.ActiveProduct, 0));
                    if (info.Find(x => x.AccountId == (int)AccountType.红包账户).Banlance >= 99 * orderProductInfo.BuyCount)
                        AccountUtils.ReturnActiveHongBao(WorkContext.PartUserInfo, orderInfo, 99 * orderProductInfo.BuyCount, WorkContext.Uid, DateTime.Now);
                    else
                    {
                        return AjaxResult("donotcancel", "订单当前不能取消,红包金额不够，请联系客服");
                    }
                }
            }

            //int completeCount = Orders.GetUserOrderCount(WorkContext.Uid, "", "", (int)OrderState.Completed);
            //if (completeCount <= 0)
            //    OrderUtils.UpdateFXUserSates(WorkContext.Uid, 0);
            string sqlStr = string.Format(" orderstate>={0} and orderstate<={1}  and uid={2} ", (int)OrderState.Confirmed, (int)OrderState.Completed, orderInfo.Uid);
            string sqlStr2 = string.Format(" orderstate>={0} and orderstate<={1} and uid={2} and orderamount>=15800 ", (int)OrderState.Confirmed, (int)OrderState.Completed, orderInfo.Uid);
            List<OrderInfo> UserOrderList = Orders.GetOrderListByWhere(sqlStr);
            List<OrderInfo> UserOrderList2 = Orders.GetOrderListByWhere(sqlStr2);
            if (UserOrderList.Count <= 0)
                OrderUtils.UpdateFXUserSates(WorkContext.Uid, 0);
            if (UserOrderList2.Count <= 0 && UserOrderList.Count > 0)
                OrderUtils.UpdateFXUserSates(WorkContext.Uid, 1);
            AdminOrders.ReturnOrder(ref partUserInfo, orderInfo, WorkContext.Uid, DateTime.Now);
            CreateOrderAction(oid, "本人", OrderActionType.Return, "订单已申请退货，请等待系统处理");
            return AjaxResult("success", "退货申请成功");

        }

        /// <summary>
        /// 换货申请
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeApply(int oid = -1)
        {
            bool state = true;
            string message = string.Empty;
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null)
            {
                state = false;
                message = "订单不存在";
                return View("changeinfo", new ChangeModel()
                {
                    State = state,
                    Message = message
                });
            }
            if (orderInfo.OrderState != (int)OrderState.Completed && (TypeHelper.ObjectToDateTime(orderInfo.ReceivingTime).AddDays(15) <= DateTime.Now))
            {
                state = false;
                message = "订单当前不满足换货条件，不能换货";
                return View("changeinfo", new ChangeModel()
                {
                    State = state,
                    Message = message
                });
            }
            List<FullShipAddressInfo> addressList = ShipAddresses.GetFullShipAddressList(orderInfo.Uid);


            ChangeModel model = new ChangeModel()
            {
                State = state,
                Message = message,
                OrderInfo = orderInfo,
                ShipAddressList = addressList
            };

            return View("changeinfo", model);

        }
        /// <summary>
        /// 换货申请
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeApply(int said, int changeType, string changeDesc = "", int oid = -1)
        {
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null)
                return AjaxResult("error", "[{\"key\":\"emptyorder\",\"msg\":\"订单不存在\"}]", true);
            if (orderInfo.OrderState != (int)OrderState.Completed && (TypeHelper.ObjectToDateTime(orderInfo.ReceivingTime).AddDays(15) <= DateTime.Now))
                return AjaxResult("error", "[{\"key\":\"errororderstate\",\"msg\":\"订单当前不能换货\"}]", true);

            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            Orders.ChangeApply(orderInfo, WorkContext.Uid, DateTime.Now, said, changeType, changeDesc);
            CreateOrderAction(oid, "本人", OrderActionType.Return, "订单已申请换货，请等待系统处理");
            return AjaxResult("success", "换货申请成功");

        }

        /// <summary>
        /// 换货确认收货
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult ChangReceive(int oid = -1)
        {
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null)
                return AjaxResult("error", "[{\"key\":\"emptyorder\",\"msg\":\"订单不存在\"}]", true);
            OrderChangeInfo info = OrderChange.GetChangeOrderByOid(oid);
            if (info == null)
                return AjaxResult("error", "[{\"key\":\"errororderstate\",\"msg\":\"订单未申请换货\"}]", true);
            if (info.State != 4 || orderInfo.ChangeType != 4)
                return AjaxResult("error", "[{\"key\":\"errororderstate\",\"msg\":\"订单换货未发货，不能确认收货\"}]", true);


            //if (orderInfo.OrderState != (int)OrderState.Sended)
            //    return AjaxResult("error", "[{\"key\":\"errororderstate\",\"msg\":\"订单还未发货，不能完成订单\"}]", true);
            //更新订单表changeType状态
            Orders.UpdateOrderChangeType(orderInfo.Oid, 5);//将订单换货类型refundtype设为5
            //更新orderchange表换货物流信息以及已处理
            OrderChange.UpdateOrderChange(info.ChangeId, 5, DateTime.Now);

            CreateOrderAction(oid, "本人", OrderActionType.ChangeSend, "换货收货完成，欢迎您再次光临");

            return AjaxResult("success", "换货收货完成，欢迎再次光临");
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        [HttpGet]
        public ActionResult DeleteOrder(int oid = -1)
        {
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null)
                return AjaxResult("error", "[{\"key\":\"emptyorder\",\"msg\":\"订单不存在\"}]", true);
            Orders.DeleteOrder(oid, true);
            return AjaxResult("success", "删除成功");
        }

        /// <summary>
        /// 延长收货期
        /// </summary>
        [HttpGet]
        public ActionResult ExtendReceive(int oid = -1)
        {
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null)
                return AjaxResult("error", "[{\"key\":\"emptyorder\",\"msg\":\"订单不存在\"}]", true);
            if (orderInfo.OrderState != (int)OrderState.Sended)
                return AjaxResult("error", "[{\"key\":\"errororderstate\",\"msg\":\"订单还未发货，不能延长收货\"}]", true);
            if (orderInfo.IsExtendReceive == 1)
                return AjaxResult("error", "[{\"key\":\"errorextendstate\",\"msg\":\"订单已经申请延长收货，不能重复申请\"}]", true);
            Orders.ExtendReceive(oid);
            return AjaxResult("success", "延长收货成功");
        }
        /// <summary>
        /// 创建订单动作
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="orderActionType"></param>
        /// <param name="actionDes"></param>
        private void CreateOrderAction(int oid, OrderActionType orderActionType, string actionDes)
        {
            OrderActions.CreateOrderAction(new OrderActionInfo()
            {
                Oid = oid,
                Uid = WorkContext.Uid,
                RealName = AdminUsers.GetUserDetailById(WorkContext.Uid).RealName,
                ActionType = (int)orderActionType,
                ActionTime = DateTime.Now,
                ActionDes = actionDes
            });
        }
        /// <summary>
        /// 创建订单动作
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="orderActionType"></param>
        /// <param name="actionDes"></param>
        private void CreateOrderAction(int oid, string realname, OrderActionType orderActionType, string actionDes)
        {
            OrderActions.CreateOrderAction(new OrderActionInfo()
            {
                Oid = oid,
                Uid = WorkContext.Uid,
                RealName = realname,
                ActionType = (int)orderActionType,
                ActionTime = DateTime.Now,
                ActionDes = actionDes
            });
        }
        #endregion

        #region 商品收藏夹

        /// <summary>
        /// 收藏夹商品列表
        /// </summary>
        public ActionResult FavoriteProductList()
        {
            int page = WebHelper.GetQueryInt("page");//当前页数

            PageModel pageModel = new PageModel(10, page, FavoriteProducts.GetFavoriteProductCount(WorkContext.Uid));
            FavoriteProductListModel model = new FavoriteProductListModel()
            {
                ProductList = FavoriteProducts.GetFavoriteProductList(pageModel.PageSize, pageModel.PageNumber, WorkContext.Uid),
                PageModel = pageModel
            };
            if (page >= 2)
                return PartialView("favprolistajax", model);
            return View(model);
            //return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加商品到收藏夹
        /// </summary>
        public ActionResult AddProductToFavorite()
        {
            //商品id
            int pid = WebHelper.GetQueryInt("pid");
            //商品信息
            PartProductInfo partProductInfo = Products.GetPartProductById(pid);
            if (partProductInfo == null)
                return AjaxResult("noproduct", "请选择商品");
            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(partProductInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return AjaxResult("nostore", "店铺不存在");

            //当收藏夹中已经存在此商品时
            if (FavoriteProducts.IsExistFavoriteProduct(WorkContext.Uid, pid))
                return AjaxResult("exist", "商品已经存在");

            //收藏夹已满时
            if (WorkContext.MallConfig.FavoriteProductCount <= FavoriteProducts.GetFavoriteProductCount(WorkContext.Uid))
                return AjaxResult("full", "收藏夹已满");

            bool result = FavoriteProducts.AddProductToFavorite(WorkContext.Uid, pid, 0, DateTime.Now);

            if (result)//添加成功
                return AjaxResult("success", "收藏成功");
            else//添加失败
                return AjaxResult("error", "收藏失败");
        }

        /// <summary>
        /// 删除收藏夹中的商品
        /// </summary>
        public ActionResult DelFavoriteProduct()
        {
            int pid = WebHelper.GetQueryInt("pid");//商品id
            bool result = FavoriteProducts.DeleteFavoriteProductByUidAndPid(WorkContext.Uid, pid);
            if (result)//删除成功
                return AjaxResult("success", pid.ToString());
            else//删除失败
                return AjaxResult("error", "删除失败");
        }

        #endregion

        #region 店铺收藏夹

        /// <summary>
        /// 收藏夹店铺列表
        /// </summary>
        public ActionResult AjaxFavoriteStoreList()
        {
            int page = WebHelper.GetQueryInt("page");//当前页数

            PageModel pageModel = new PageModel(10, page, FavoriteStores.GetFavoriteStoreCount(WorkContext.Uid));
            AjaxFavoriteStoreListModel model = new AjaxFavoriteStoreListModel()
            {
                StoreList = CommonHelper.DataTableToList(FavoriteStores.GetFavoriteStoreList(pageModel.PageSize, pageModel.PageNumber, WorkContext.Uid)),
                PageModel = pageModel
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加店铺到收藏夹
        /// </summary>
        public ActionResult AddStoreToFavorite()
        {
            //店铺id
            int storeId = WebHelper.GetQueryInt("storeId");
            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(storeId);
            if (storeInfo == null || storeInfo.State != (int)StoreState.Open)
                return AjaxResult("nostore", "店铺不存在");

            //当收藏夹中已经存在此店铺时
            if (FavoriteStores.IsExistFavoriteStore(WorkContext.Uid, storeId))
                return AjaxResult("exist", "店铺已经存在");

            //收藏夹已满时
            if (WorkContext.MallConfig.FavoriteStoreCount <= FavoriteStores.GetFavoriteStoreCount(WorkContext.Uid))
                return AjaxResult("full", "收藏夹已满");

            bool result = FavoriteStores.AddStoreToFavorite(WorkContext.Uid, storeId, DateTime.Now);

            if (result)//添加成功
                return AjaxResult("success", "收藏成功");
            else//添加失败
                return AjaxResult("error", "收藏失败");
        }

        /// <summary>
        /// 删除收藏夹中的店铺
        /// </summary>
        public ActionResult DelFavoriteStore()
        {
            int storeId = WebHelper.GetQueryInt("storeId");//店铺id
            bool result = FavoriteStores.DeleteFavoriteStoreByUidAndStoreId(WorkContext.Uid, storeId);
            if (result)//删除成功
                return AjaxResult("success", storeId.ToString());
            else//删除失败
                return AjaxResult("error", "删除失败");
        }

        #endregion

        #region 浏览商品

        /// <summary>
        /// 浏览商品列表
        /// </summary>
        public ActionResult AjaxBrowseProductList()
        {
            int page = WebHelper.GetQueryInt("page");//当前页数

            PageModel pageModel = new PageModel(10, page, BrowseHistories.GetUserBrowseProductCount(WorkContext.Uid));
            AjaxBrowseProductListModel model = new AjaxBrowseProductListModel()
            {
                ProductList = BrowseHistories.GetUserBrowseProductList(pageModel.PageSize, pageModel.PageNumber, WorkContext.Uid),
                PageModel = pageModel
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 配送地址

        /// <summary>
        /// 配送地址列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AjaxShipAddressList()
        {
            List<FullShipAddressInfo> shipAddressList = ShipAddresses.GetFullShipAddressList(WorkContext.Uid);
            int shipAddressCount = shipAddressList.Count;

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"count\":");
            sb.AppendFormat("\"{0}\",\"list\":[", shipAddressCount);
            foreach (FullShipAddressInfo fullShipAddressInfo in shipAddressList)
            {
                sb.AppendFormat("{0}\"saId\":\"{1}\",\"user\":\"{2}&nbsp;&nbsp;&nbsp;{3}\",\"address\":\"{4}&nbsp;{5}&nbsp;{6}&nbsp;{7}\"{8},", "{", fullShipAddressInfo.SAId, fullShipAddressInfo.Consignee, fullShipAddressInfo.Mobile.Length > 0 ? fullShipAddressInfo.Mobile : fullShipAddressInfo.Phone, fullShipAddressInfo.ProvinceName, fullShipAddressInfo.CityName, fullShipAddressInfo.CountyName, fullShipAddressInfo.Address, "}");
            }
            if (shipAddressCount > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");

            return AjaxResult("success", sb.ToString(), true);
        }

        /// <summary>
        /// 配送地址列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ShipAddressList()
        {
            ShipAddressListModel model = new ShipAddressListModel();

            model.ShipAddressList = ShipAddresses.GetFullShipAddressList(WorkContext.Uid);
            model.ShipAddressCount = model.ShipAddressList.Count;

            return View(model);
        }

        /// <summary>
        /// 添加配送地址
        /// </summary>
        public ActionResult AddShipAddress()
        {
            if (WebHelper.IsGet())
            {
                ShipAddressModel model = new ShipAddressModel();
                return View(model);
            }
            else
            {
                int regionId = WebHelper.GetFormInt("regionId");
                string alias = WebHelper.GetFormString("alias");
                string consignee = WebHelper.GetFormString("consignee");
                string mobile = WebHelper.GetFormString("mobile");
                string phone = WebHelper.GetFormString("phone");
                string email = WebHelper.GetFormString("email");
                string zipcode = WebHelper.GetFormString("zipcode");
                string address = WebHelper.GetFormString("address");
                int isDefault = WebHelper.GetFormInt("isDefault");

                string verifyResult = VerifyShipAddress(regionId, alias, consignee, mobile, phone, email, zipcode, address);

                if (verifyResult.Length == 0)
                {
                    //检查配送地址数量是否达到系统所允许的最大值
                    int shipAddressCount = ShipAddresses.GetShipAddressCount(WorkContext.Uid);
                    if (shipAddressCount >= WorkContext.MallConfig.MaxShipAddress)
                        return AjaxResult("full", "收货地址的数量已经达到系统所允许的最大值");

                    ShipAddressInfo shipAddressInfo = new ShipAddressInfo();
                    shipAddressInfo.Uid = WorkContext.Uid;
                    shipAddressInfo.RegionId = regionId;
                    shipAddressInfo.IsDefault = isDefault == 0 ? 0 : 1;
                    shipAddressInfo.Alias = WebHelper.HtmlEncode(alias);
                    shipAddressInfo.Consignee = WebHelper.HtmlEncode(consignee);
                    shipAddressInfo.Mobile = mobile;
                    shipAddressInfo.Phone = phone;
                    shipAddressInfo.Email = email;
                    shipAddressInfo.ZipCode = zipcode;
                    shipAddressInfo.Address = WebHelper.HtmlEncode(address);
                    int saId = ShipAddresses.CreateShipAddress(shipAddressInfo);
                    return AjaxResult("success", saId.ToString());
                }
                else
                {
                    return AjaxResult("error", verifyResult, true);
                }
            }
        }

        /// <summary>
        /// 编辑配送地址
        /// </summary>
        public ActionResult EditShipAddress()
        {
            if (WebHelper.IsGet())
            {
                int saId = WebHelper.GetQueryInt("saId");
                FullShipAddressInfo fullShipAddressInfo = ShipAddresses.GetFullShipAddressBySAId(saId, WorkContext.Uid);
                if (fullShipAddressInfo == null)
                    return PromptView(Url.Action("shipaddresslist"), "地址不存在");

                ShipAddressModel model = new ShipAddressModel();
                model.Alias = fullShipAddressInfo.Alias;
                model.Consignee = fullShipAddressInfo.Consignee;
                model.Mobile = fullShipAddressInfo.Mobile;
                model.Phone = fullShipAddressInfo.Phone;
                model.Email = fullShipAddressInfo.Email;
                model.ZipCode = fullShipAddressInfo.ZipCode;
                model.ProvinceId = fullShipAddressInfo.ProvinceId;
                model.CityId = fullShipAddressInfo.CityId;
                model.RegionId = fullShipAddressInfo.RegionId;
                model.Address = fullShipAddressInfo.Address;
                model.IsDefault = fullShipAddressInfo.IsDefault;

                return View(model);
            }
            else
            {
                int saId = WebHelper.GetQueryInt("saId");
                int regionId = WebHelper.GetFormInt("regionId");
                string alias = WebHelper.GetFormString("alias");
                string consignee = WebHelper.GetFormString("consignee");
                string mobile = WebHelper.GetFormString("mobile");
                string phone = WebHelper.GetFormString("phone");
                string email = WebHelper.GetFormString("email");
                string zipcode = WebHelper.GetFormString("zipcode");
                string address = WebHelper.GetFormString("address");
                int isDefault = WebHelper.GetFormInt("isDefault");

                string verifyResult = VerifyShipAddress(regionId, alias, consignee, mobile, phone, email, zipcode, address);
                if (verifyResult.Length == 0)
                {
                    ShipAddressInfo shipAddressInfo = ShipAddresses.GetShipAddressBySAId(saId, WorkContext.Uid);
                    //检查地址
                    if (shipAddressInfo == null)
                        return AjaxResult("noexist", "配送地址不存在");

                    shipAddressInfo.Uid = WorkContext.Uid;
                    shipAddressInfo.RegionId = regionId;
                    shipAddressInfo.IsDefault = isDefault == 0 ? 0 : 1;
                    shipAddressInfo.Alias = WebHelper.HtmlEncode(alias);
                    shipAddressInfo.Consignee = WebHelper.HtmlEncode(consignee);
                    shipAddressInfo.Mobile = mobile;
                    shipAddressInfo.Phone = phone;
                    shipAddressInfo.Email = email;
                    shipAddressInfo.ZipCode = zipcode;
                    shipAddressInfo.Address = WebHelper.HtmlEncode(address);
                    ShipAddresses.UpdateShipAddress(shipAddressInfo);
                    return AjaxResult("success", "编辑成功");
                }
                else
                {
                    return AjaxResult("error", verifyResult, true);
                }
            }
        }

        /// <summary>
        /// 删除配送地址
        /// </summary>
        public ActionResult DelShipAddress()
        {
            int saId = WebHelper.GetQueryInt("saId");
            bool result = ShipAddresses.DeleteShipAddress(saId, WorkContext.Uid);
            if (result)//删除成功
                return AjaxResult("success", saId.ToString());
            else//删除失败
                return AjaxResult("error", "删除失败");
        }

        /// <summary>
        /// 设置默认配送地址
        /// </summary>
        public ActionResult SetDefaultShipAddress()
        {
            int saId = WebHelper.GetQueryInt("saId");
            bool result = ShipAddresses.UpdateShipAddressIsDefault(saId, WorkContext.Uid, 1);
            if (result)//设置成功
                return AjaxResult("success", saId.ToString());
            else//设置失败
                return AjaxResult("error", "设置失败");
        }

        /// <summary>
        /// 验证配送地址
        /// </summary>
        private string VerifyShipAddress(int regionId, string alias, string consignee, string mobile, string phone, string email, string zipcode, string address)
        {
            StringBuilder errorList = new StringBuilder("[");

            //检查区域
            RegionInfo regionInfo = Regions.GetRegionById(regionId);
            if (regionInfo == null || regionInfo.Layer != 3)
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "regionId", "请选择有效的区域", "}");

            //检查地址别名
            if (alias.Length > 25)
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "alias", "最多只能输入25个字", "}");

            //检查收货人
            if (string.IsNullOrWhiteSpace(consignee))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "consignee", "收货人不能为空", "}");
            else if (consignee.Length > 10)
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "consignee", "最多只能输入10个字", "}");

            //检查手机号和固话号
            if (string.IsNullOrWhiteSpace(mobile) && string.IsNullOrWhiteSpace(phone))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "mobile", "手机号和固话号必填一项", "}");
            }
            else
            {
                if (!ValidateHelper.IsMobile(mobile))
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "mobile", "手机号格式不正确", "}");
                if (!ValidateHelper.IsPhone(phone))
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "phone", "固话号格式不正确", "}");
            }

            //检查邮箱
            if (!ValidateHelper.IsEmail(email))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "email", "邮箱格式不正确", "}");

            //检查邮编
            if (!ValidateHelper.IsZipCode(zipcode))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "zipcode", "邮编格式不正确", "}");

            //检查详细地址
            if (string.IsNullOrWhiteSpace(address))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "address", "详细地址不能为空", "}");
            else if (address.Length > 75)
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "address", "最多只能输入75个字", "}");

            if (errorList.Length > 1)
                return errorList.Remove(errorList.Length - 1, 1).Append("]").ToString();
            else
                return "";
        }

        #endregion

        #region 用户支付积分

        /// <summary>
        /// 支付积分
        /// </summary>
        public ActionResult PayCredit()
        {
            return View();
        }

        #endregion

        #region 优惠劵

        /// <summary>
        /// 优惠劵列表
        /// </summary>
        public ActionResult CouponList()
        {
            int type = WebHelper.GetQueryInt("type");

            CouponListModel model = new CouponListModel()
            {
                ListType = type,
                CouponList = Coupons.GetCouponList(WorkContext.Uid, type)
            };

            return View(model);
        }

        #endregion

        #region  汇购卡券
        public ActionResult CashCouponList()
        {
            //int type = WebHelper.GetQueryInt("type");

            //CouponListModel model = new CouponListModel()
            //{
            //    ListType = type,
            //    CouponList = Coupons.GetCouponList(WorkContext.Uid, type)
            //};
            List<CashCouponInfo> model = CashCoupon.GetList(string.Format(" uid={0} AND CouponType=1  ", WorkContext.Uid));
            return View(model);
        }
        public ActionResult CashDetail(int CashId)
        {
            int page = WebHelper.GetQueryInt("page");
            if (page == 0)
                page = 1;
            int pageSize = 10;
            CashDetailModel model = new CashDetailModel();
            model.PageModel = new PageModel(pageSize, page, CashCouponDetail.GetRecordCount(string.Format(" CashId={0} AND Uid={1}", CashId, WorkContext.Uid)));
            //select * from tb where rowid between (" + pageNumber + @"-1)*" + pageSize + @"+1 and (" + pageNumber + @")*" + pageSize + " order by tb.reviewid desc
            model.CashDetailList = CashCouponDetail.GetListByPage(string.Format(" CashId={0} AND Uid={1}", CashId, WorkContext.Uid), "", (page - 1) * pageSize + 1, page * pageSize);
            if (page >= 2)
                return PartialView("cashdetailajax", model);
            return View(model);
        }

        #endregion

        #region  订单评价

        /// <summary>
        /// 评价订单
        /// </summary>
        public ActionResult ReviewOrder()
        {
            int oid = WebHelper.GetQueryInt("oid");

            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed)
                return PromptView("订单当前不能评价");
            if (orderInfo.IsReview == 1)
                return PromptView("此订单已经评价");

            ReviewOrderModel model = new ReviewOrderModel()
            {
                OrderInfo = orderInfo,
                OrderProductList = Orders.GetOrderProductList(oid),
                StoreReviewInfo = Stores.GetStoreReviewByOid(oid)
            };
            return View(model);
        }

        /// <summary>
        /// 评价商品
        /// </summary>
        public ActionResult ReviewProduct()
        {
            int oid = WebHelper.GetQueryInt("oid");//订单id
            int recordId = WebHelper.GetQueryInt("recordId");//订单商品记录id
            int star = WebHelper.GetFormInt("star");//星星
            string message = WebHelper.GetFormString("message");//评价内容

            if (star > 5 || star < 0)
                return AjaxResult("wrongstar", "请选择正确的星星");

            if (message.Length == 0)
                return AjaxResult("emptymessage", "请填写评价内容");
            if (message.Length > 100)
                return AjaxResult("muchmessage", "评价内容最多输入100个字");
            //禁止词
            string bannedWord = FilterWords.GetWord(message);
            if (bannedWord != "")
                return AjaxResult("bannedWord", "评价内容中不能包含违禁词");

            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return AjaxResult("noexistorder", "订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed)
                return AjaxResult("nocomplete", "订单还未完成,不能评价");

            OrderProductInfo orderProductInfo = null;
            List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(oid);
            foreach (OrderProductInfo item in orderProductList)
            {
                if (item.RecordId == recordId)
                {
                    orderProductInfo = item;
                    break;
                }
            }
            if (orderProductInfo == null)
                return AjaxResult("noproduct", "商品不存在");
            //商品已评价
            if (orderProductInfo.IsReview == 1)
                return AjaxResult("reviewed", "商品已经评价");

            int payCredits = Credits.SendReviewProductCredits(ref WorkContext.PartUserInfo, orderProductInfo, DateTime.Now);
            ProductReviewInfo productReviewInfo = new ProductReviewInfo()
            {
                Pid = orderProductInfo.Pid,
                Uid = orderProductInfo.Uid,
                OPRId = orderProductInfo.RecordId,
                Oid = orderProductInfo.Oid,
                ParentId = 0,
                State = 0,
                StoreId = orderProductInfo.StoreId,
                Star = star,
                Quality = 0,
                Message = WebHelper.HtmlEncode(FilterWords.HideWords(message)),
                ReviewTime = DateTime.Now,
                PayCredits = payCredits,
                PName = orderProductInfo.Name,
                PShowImg = orderProductInfo.ShowImg,
                BuyTime = orderProductInfo.AddTime,
                IP = WorkContext.IP
            };
            ProductReviews.ReviewProduct(productReviewInfo);

            orderProductInfo.IsReview = 1;
            if (Orders.IsReviewAllOrderProduct(orderProductList) && Stores.GetStoreReviewByOid(oid) != null)
                Orders.UpdateOrderIsReview(oid, 1);

            return AjaxResult("success", recordId.ToString());
        }

        /// <summary>
        /// 评价店铺
        /// </summary>
        public ActionResult ReviewStore()
        {
            int oid = WebHelper.GetQueryInt("oid");//订单id
            int descriptionStar = WebHelper.GetFormInt("descriptionStar");//商品描述星星
            int serviceStar = WebHelper.GetFormInt("serviceStar");//商家服务星星
            int shipStar = WebHelper.GetFormInt("shipStar");//商家配送星星

            if (descriptionStar > 5 || descriptionStar < 0)
                return AjaxResult("wrongdescriptionstar", "请选择正确的商品描述星星");
            if (serviceStar > 5 || serviceStar < 0)
                return AjaxResult("wrongservicestar", "请选择正确的商家服务星星");
            if (shipStar > 5 || shipStar < 0)
                return AjaxResult("wrongshipstar", "请选择正确的商家配送星星");

            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.Uid != WorkContext.Uid)
                return AjaxResult("noexistorder", "订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed)
                return AjaxResult("nocomplete", "订单还未完成,不能评价");

            StoreReviewInfo storeReviewInfo = Stores.GetStoreReviewByOid(oid);
            if (storeReviewInfo != null)
                return AjaxResult("reviewed", "店铺已经评价");

            storeReviewInfo = new StoreReviewInfo()
            {
                Oid = oid,
                StoreId = orderInfo.StoreId,
                DescriptionStar = descriptionStar,
                ServiceStar = serviceStar,
                ShipStar = shipStar,
                Uid = WorkContext.Uid,
                ReviewTime = DateTime.Now,
                IP = WorkContext.IP
            };
            Stores.CreateStoreReview(storeReviewInfo);

            if (Orders.IsReviewAllOrderProduct(Orders.GetOrderProductList(oid)))
                Orders.UpdateOrderIsReview(oid, 1);

            return AjaxResult("success", "店铺评价成功");
        }

        #endregion

        #region 帐号信息
        /// <summary>
        /// 用户信息
        /// </summary>
        public ActionResult accountInfo()
        {
            AccountInfoModel model = new AccountInfoModel();
            model.AccountInfoList = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid);

            return View(model);
        }
        /// <summary>
        /// 账户详情
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public ActionResult accountDetail(int uid, int accountId, string accountName)
        {
            int page = WebHelper.GetQueryInt("page");
            if (page == 0)
                page = 1;
            int pageSize = 10;

            AccountDetailModel model = new AccountDetailModel();
            model.AccountId = accountId;
            model.AccountName = accountName;
            if (accountId == (int)AccountType.代理账户 || accountId == (int)AccountType.佣金账户)
            {
                model.PageModel = new PageModel(pageSize, page, VMall.Services.Account.GetAccountDetailCount(uid, accountId));
                model.AccountDetailList = VMall.Services.Account.GetAccountDetailList(uid, accountId, page, pageSize);
            }
            else
            {
                if (!WorkContext.IsDirSaleUser)//汇购会员
                {
                    model.PageModel = new PageModel(pageSize, page, VMall.Services.Account.GetAccountDetailCount(uid, accountId));
                    model.AccountDetailList = VMall.Services.Account.GetAccountDetailList(uid, accountId, page, pageSize);
                }
                else//直销会员通过接口取流水
                {
                    int totalCount = 0;
                    model.AccountDetailList = AccountUtils.GetDetail(WorkContext.DirSaleUid, accountId, page, pageSize, ref totalCount);
                    model.PageModel = new PageModel(pageSize, page, totalCount);
                }
            }
            //List<AccountDetailInfo> detailInfo = Account.GetAccountDetailList(uid, accountId);
            if (page >= 2)
                return PartialView("accountdetailajax", model);

            return View(model);
        }

        #endregion

        #region 分享会员
        /// <summary>
        /// 获得分享会员列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SubRecommendList()
        {
            int page = WebHelper.GetQueryInt("page");
            if (page == 0)
                page = 1;

            string startAddTime = WebHelper.GetQueryString("startAddTime");
            string endAddTime = WebHelper.GetQueryString("endAddTime");
            int pageSize = 15;
            List<UserInfo> subUserList = Users.GetSubRecommendListByPid(WorkContext.PartUserInfo, page, pageSize);

            PageModel pageModel = new PageModel(pageSize, page, Users.GetUserCount(WorkContext.PartUserInfo));

            //#region 二维码
            ////二维码
            //string parentName = string.IsNullOrEmpty(WorkContext.UserName) ? (string.IsNullOrEmpty(WorkContext.UserEmail) ? (string.IsNullOrEmpty(WorkContext.UserMobile) ? "" : WorkContext.UserMobile) : WorkContext.UserEmail) : WorkContext.UserName;
            //bool isMobile = true;// WebHelper.IsMobile();
            //string shareUrl = "http://" + Request.Url.Host + ":" + Request.Url.Port + (isMobile ? "/mob" : "") + "/account/register?pname=" + parentName.Trim() + "&returnUrl=http://" + Request.Url.Host + ":" + Request.Url.Port + (isMobile ? "/mob" : "");
            ////string codeImgPath = CreateCode_Simple(shareUrl, WorkContext.PartUserInfo.Uid, WorkContext.PartUserInfo.Salt, isMobile);
            ////ViewData["codeImg"] = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/upload/usersharecode/" + (isMobile ? "/m/" : "/pc/") + codeImgPath;
            //string bgQRcode = IOHelper.CreatQRCodeWithBG2(shareUrl, WorkContext.Uid, WorkContext.PartUserInfo);

            //ViewData["codeImg"] = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/upload/bgqrcode/" + bgQRcode;

            ////图表化结构图
            ////PartUserInfo parent = WorkContext.PartUserInfo;
            ////List<PartUserInfo> children = Users.GetSubRecommendListByPid(WorkContext.Uid);
            ////children.Add(parent);//将父级加入到list中 
            ////ViewData["treeStr"] = GetTreeOrgChart(children);//数据拼接成组织层级 

            ////var list = from f in children orderby f.Uid select new { id = f.Uid, pid = f.Pid, name = f.NickName };
            //#endregion

            List<OrderInfo> orderList = new List<OrderInfo>();
            List<OrderProductInfo> opList = new List<OrderProductInfo>();
            SubRecommendListModel list = new SubRecommendListModel();
            list.UserList = subUserList;
            //subUserList.ForEach(y =>
            //{
            //    string sqlStr = string.Format(" orderstate>={0} and  orderstate<={1} and uid={2}", (int)OrderState.Confirmed, (int)OrderState.Completed, y.Uid);
            //    List<OrderInfo> UserOrderList = Orders.GetOrderListByWhere(sqlStr);
            //    orderList.AddRange(UserOrderList);
            //    StringBuilder oidList = new StringBuilder();
            //    foreach (OrderInfo row in UserOrderList)
            //    {
            //        oidList.AppendFormat("{0},", row.Oid);
            //    }
            //    if (oidList.Length > 0)
            //        oidList.Remove(oidList.Length - 1, 1);

            //    List<OrderProductInfo> userOPList = Orders.GetOrderProductList(oidList.ToString());
            //    userOPList.ForEach(x =>
            //    {
            //        PartProductInfo pro = Products.GetPartProductById(x.Pid);
            //        if (pro != null)
            //        {
            //            x.PV = pro.PV;
            //            x.HaiMi = pro.HaiMi;
            //        }
            //    });
            //    opList.AddRange(userOPList);
            //});

            list.OrderList = orderList;
            list.OrderProductList = opList;
            list.PageModel = pageModel;
            if (page >= 2)
                return PartialView("subrecommendlistajax", list);

            return View(list);
        }

        /// <summary>
        /// 分享会员详情
        /// </summary>
        /// <returns></returns>
        public ActionResult SubRecommendDetail()
        {
            int uid = WebHelper.GetQueryInt("uid");
            int page = WebHelper.GetQueryInt("page");
            if (page == 0)
                page = 1;

            string startAddTime = WebHelper.GetQueryString("startAddTime");
            string endAddTime = WebHelper.GetQueryString("endAddTime");
            int orderState = (int)OrderState.Confirmed;

            string sqlStr = string.Format(" orderstate>={0} and  orderstate<={1} and uid={2}", (int)OrderState.Confirmed, (int)OrderState.Completed, uid);
            List<OrderInfo> UserOrderList = Orders.GetOrderListByWhere(sqlStr);
            int pageSize = 7;
            PageModel pageModel = new PageModel(pageSize, page, Orders.GetOrderListCoutByWhere(sqlStr));
            List<OrderInfo> orderList = Orders.GetOrderListByWhereForPage(pageSize, page, sqlStr);
            //DataTable orderList = Orders.GetUserOrderList(uid, pageModel.PageSize, pageModel.PageNumber, startAddTime, endAddTime, orderState);

            StringBuilder oidList = new StringBuilder();
            foreach (OrderInfo row in orderList)
            {
                oidList.AppendFormat("{0},", row.Oid);
            }
            //foreach (DataRow row in orderList.Rows)
            //{
            //    oidList.AppendFormat("{0},", row["oid"]);
            //}
            if (oidList.Length > 0)
                oidList.Remove(oidList.Length - 1, 1);

            List<OrderProductInfo> opList = Orders.GetOrderProductList(oidList.ToString());
            opList.ForEach(x =>
            {
                PartProductInfo pro = Products.GetPartProductById(x.Pid);
                if (pro != null)
                {
                    x.PV = pro.PV;
                    x.HaiMi = pro.HaiMi;
                }
            });
            SubRecommendDetailModel model = new SubRecommendDetailModel()
            {
                Uid = uid,
                PageModel = pageModel,
                OrderList = orderList,
                OrderProductList = opList,
                StartAddTime = startAddTime,
                EndAddTime = endAddTime,
                OrderState = orderState

            };
            if (page >= 2)
                return PartialView("subrecommenddetailajax", model);

            return View(model);
        }

        #endregion

        #region 海米提现
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DrawCash()
        {
            bool state = true;
            string message = string.Empty;
            AccountInfo accountInfo = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid).Find(x => x.AccountId == 10);
            UserInfo user = Users.GetUserById(WorkContext.Uid);
            if (user == null)
                return View("drawcash", new DrawCashModel()
                {
                    State = false,
                    Message = "会员信息错误"
                });
            if (!user.IsDirSaleUser && string.IsNullOrEmpty(user.IdCard))
                return View("drawcash", new DrawCashModel()
                {
                    State = false,
                    Message = "身份证信息未完善，请完善身份证信息，并保证身份证信息真实有效！",
                    JumpUrl = Url.Action("userinfo")
                });
            if (!user.IsDirSaleUser && string.IsNullOrEmpty(user.PayPassword))
                return View("drawcash", new DrawCashModel()
                {
                    State = false,
                    Message = "未设置支付密码！",
                    JumpUrl = Url.Action("safeverify", new { act = "updatePayPassword" })
                });
            //if (accountInfo.Banlance < 100)

            //    return View("drawcash", new DrawCashModel()
            //    {
            //        State = false,
            //        Message = "提现余额不足"
            //    });

            DrawCashModel model = new DrawCashModel()
            {
                State = state,
                Message = message,
                AccountInfo = accountInfo,

            };

            return View("drawcash", model);

            //ViewData["TotalAmount"] = accountInfo.Banlance.ToString("0.00");
            //return View();
        }
        /// <summary>
        /// 提交海米提现申请
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="BankName"></param>
        /// <param name="regionId"></param>
        /// <param name="BankAddress"></param>
        /// <param name="BankCardCode"></param>
        /// <param name="BankUserName"></param>
        /// <param name="PayPassWord"></param>
        /// <returns></returns>
        public ActionResult ApplyDarwCash(string Amount, string BankName, string regionId, string BankAddress, string BankCardCode, string BankUserName, string PayPassWord)
        {
            if (string.IsNullOrEmpty(Amount) || string.IsNullOrEmpty(BankName) || string.IsNullOrEmpty(regionId) || string.IsNullOrEmpty(BankAddress) || string.IsNullOrEmpty(BankCardCode) || string.IsNullOrEmpty(BankUserName) || string.IsNullOrEmpty(PayPassWord))
                return Content("400");
            List<RegionInfo> regions = Regions.GetRegionAndParentRegionById(TypeHelper.StringToInt(regionId));
            if (regions.Count < 2)
                return Content("400");
            AccountInfo accountInfo = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid).Find(x => x.AccountId == 10);
            if (accountInfo.Banlance < 100)
                return Content("300");
            if (WorkContext.PartUserInfo.IsDirSaleUser)
            {
                if (OrderUtils.GetPayPassword(PayPassWord, WorkContext.PartUserInfo.DirSaleUid) != SecureHelper.EncryptString(PayPassWord, DirSaleUserInfo.EncryptKey))
                    return Content("500");
            }
            else
            {
                if (Users.CreateUserPassword(PayPassWord, WorkContext.PartUserInfo.Salt) != WorkContext.PartUserInfo.PayPassword)
                    return Content("500");
            }
            HaiMiDrawCashInfo info = new HaiMiDrawCashInfo();
            info.Uid = WorkContext.Uid;
            info.AccountId = (int)AccountType.海米账户;
            info.DrawCashSN = "TX" + DateTime.Now.ToString("yyyyMMddHHmmss") + Randoms.CreateRandomValue(5);
            info.Amount = TypeHelper.StringToDecimal(Amount);
            info.Poundage = TypeHelper.StringToDecimal(Amount) * BMAConfig.MallConfig.HMFeeRate;
            info.TaxAmount = TypeHelper.StringToDecimal(Amount) * BMAConfig.MallConfig.HMTaxRate;
            info.ActualAmount = info.Amount - info.Poundage - info.TaxAmount;
            info.State = 1;
            info.BankName = BankName;
            info.BankProvice = regions.Find(x => x.Layer == 1) != null ? regions.Find(x => x.Layer == 1).Name : "";
            info.BankCity = regions.Find(x => x.Layer == 2) != null ? regions.Find(x => x.Layer == 2).Name : "";
            info.BankAddress = BankAddress;
            info.BankCardCode = BankCardCode;
            info.BankUserName = BankUserName;

            haiMiDrawCashBLL.Add(info);
            //更新直销的账户
            if (WorkContext.PartUserInfo.IsDirSaleUser)
            {
                AccountUtils.UpdateAccountForDir(WorkContext.PartUserInfo.DirSaleUid, (int)AccountType.海米账户, 0, info.Amount, info.DrawCashSN, string.Format("海米提现：提现金额:{0}，税费{1},手续费{2},实际提现金额{3}", info.Amount, info.TaxAmount, info.Poundage, info.ActualAmount));
            }
            else
            {
                VMall.Services.Account.UpdateAccountForOut(new AccountInfo()
                {
                    AccountId = (int)AccountType.海米账户,
                    UserId = WorkContext.Uid,
                    TotalOut = info.Amount
                });
                VMall.Services.Account.CreateAccountDetail(new AccountDetailInfo()
                {
                    AccountId = (int)AccountType.海米账户,
                    UserId = WorkContext.Uid,
                    CreateTime = DateTime.Now,
                    DetailType = (int)DetailType.提现支出,
                    OutAmount = info.Amount,
                    OrderCode = info.DrawCashSN,
                    AdminUid = 0,//system
                    Status = 1,
                    DetailDes = string.Format("海米提现：提现金额:{0}，税费{1},手续费{2},实际提现金额{3}", info.Amount, info.TaxAmount, info.Poundage, info.ActualAmount)
                });
            }
            return Content("200");
        }

        /// <summary>
        /// 提现历史记录
        /// </summary>
        /// <returns></returns>
        public ActionResult DrawHistory(int accountId, int pageSize = 10, int pageNumber = 1)
        {
            DrawCashHistoryModel model = new DrawCashHistoryModel();
            string condition = " Uid=" + WorkContext.Uid + " and AccountId=" + accountId;
            PageModel pageModel = new PageModel(pageSize, pageNumber, haiMiDrawCashBLL.GetRecordCount(condition));
            List<HaiMiDrawCashInfo> list = haiMiDrawCashBLL.GetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize);
            model.PageModel = pageModel;
            model.HistoryList = list;
            model.AccountName = VMall.Services.Account.GetAccountName(accountId);
            if (pageNumber >= 2)
                return PartialView("drawhistoryajax", model);
            return View(model);
        }

        #endregion

        #region 提现
        /// <summary>
        ///  提现
        /// </summary>
        /// <returns></returns>
        public ActionResult RewardDraw()
        {
            bool state = true;
            string message = string.Empty;
            AccountInfo accountInfo = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid).Find(x => x.AccountId == (int)AccountType.商城奖金账户);
            if (accountInfo == null)
                accountInfo = new AccountInfo();
            UserInfo user = Users.GetUserById(WorkContext.Uid);
            if (user == null)
                return View("rewarddraw", new DrawCashModel()
                {
                    State = false,
                    Message = "会员信息错误"
                });
            if (user.IsDirSaleUser)
            {
                return View("rewarddraw", new DrawCashModel()
                {
                    State = false,
                    Message = "商城仅支持普通会员的体现，更高级别的会员请前往经销商系统体现。"
                });

            }
            if (!user.IsDirSaleUser && string.IsNullOrEmpty(user.IdCard))
                return View("rewarddraw", new DrawCashModel()
                {
                    State = false,
                    Message = "身份证信息未完善，请完善身份证信息，并保证身份证信息真实有效！",
                    JumpUrl = Url.Action("userinfo")
                });

            if (!user.IsDirSaleUser && string.IsNullOrEmpty(user.PayPassword))
                return View("rewarddraw", new DrawCashModel()
                {
                    State = false,
                    Message = "未设置支付密码！",//safeverify?act=updatePayPassword
                    JumpUrl = Url.Action("safeverify", new { act = "updatePayPassword" })
                });

            DrawCashModel model = new DrawCashModel()
            {
                State = state,
                Message = message,
                AccountInfo = accountInfo,
            };
            return View("rewarddraw", model);
        }
        /// <summary>
        /// 提交代理提现申请
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="BankName"></param>
        /// <param name="regionId"></param>
        /// <param name="BankAddress"></param>
        /// <param name="BankCardCode"></param>
        /// <param name="BankUserName"></param>
        /// <param name="PayPassWord"></param>
        /// <returns></returns>
        public ActionResult ApplyRewardDarw(string Amount, string BankName, string regionId, string BankAddress, string BankCardCode, string BankUserName, string PayPassWord)
        {
            if (string.IsNullOrEmpty(Amount) || string.IsNullOrEmpty(BankName) || string.IsNullOrEmpty(regionId) || string.IsNullOrEmpty(BankAddress) || string.IsNullOrEmpty(BankCardCode) || string.IsNullOrEmpty(BankUserName) || string.IsNullOrEmpty(PayPassWord))
                return Content("400");
            List<RegionInfo> regions = Regions.GetRegionAndParentRegionById(TypeHelper.StringToInt(regionId));
            if (regions.Count < 2)
                return Content("400");

            AccountInfo accountInfo = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid).Find(x => x.AccountId == (int)AccountType.商城奖金账户);
            if (accountInfo.Banlance < 500)
                return Content("300");
            if (accountInfo.Banlance < TypeHelper.StringToDecimal(Amount))
                return Content("300");
            if (WorkContext.PartUserInfo.IsDirSaleUser)
            {
                if (OrderUtils.GetPayPassword(PayPassWord, WorkContext.PartUserInfo.DirSaleUid) != SecureHelper.EncryptString(PayPassWord, DirSaleUserInfo.EncryptKey))
                    return Content("500");
            }
            else
            {
                if (Users.CreateUserPassword(PayPassWord, WorkContext.PartUserInfo.Salt) != WorkContext.PartUserInfo.PayPassword)
                    return Content("500");
            }
            HaiMiDrawCashInfo info = new HaiMiDrawCashInfo();
            info.Uid = WorkContext.Uid;
            info.AccountId = (int)AccountType.商城奖金账户;
            info.DrawCashSN = "TX" + DateTime.Now.ToString("yyyyMMddHHmmss") + Randoms.CreateRandomValue(6);
            info.Amount = TypeHelper.StringToDecimal(Amount);
            info.Poundage = TypeHelper.StringToDecimal(Amount) * BMAConfig.MallConfig.YJFeeRate;
            //info.TaxAmount = TypeHelper.StringToDecimal(Amount) * BMAConfig.MallConfig.HMTaxRate;
            info.ActualAmount = info.Amount - info.Poundage - info.TaxAmount;
            info.State = 1;
            info.BankName = BankName;
            info.BankProvice = regions.Find(x => x.Layer == 1) != null ? regions.Find(x => x.Layer == 1).Name : "";
            info.BankCity = regions.Find(x => x.Layer == 2) != null ? regions.Find(x => x.Layer == 2).Name : "";
            info.BankAddress = BankAddress;
            info.BankCardCode = BankCardCode;
            info.BankUserName = BankUserName;

            haiMiDrawCashBLL.Add(info);
            //更新账户

            VMall.Services.Account.UpdateAccountForOut(new AccountInfo()
            {
                AccountId = (int)AccountType.商城奖金账户,
                UserId = WorkContext.Uid,
                TotalOut = info.Amount
            });
            VMall.Services.Account.CreateAccountDetail(new AccountDetailInfo()
            {
                AccountId = (int)AccountType.商城奖金账户,
                UserId = WorkContext.Uid,
                CreateTime = DateTime.Now,
                DetailType = (int)DetailType.提现支出,
                OutAmount = info.Amount,
                OrderCode = info.DrawCashSN,
                AdminUid = 0,//system
                Status = 1,
                DetailDes = string.Format(AccountType.商城奖金账户 + "提现：提现金额:{0},手续费{1},实际提现金额{2}", info.Amount, info.Poundage, info.ActualAmount)
            });

            return Content("200");
        }


        #endregion
        #region 转账
        /// <summary>
        ///  转账
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountTranfer()
        {
            bool state = true;
            string message = string.Empty;
            AccountInfo accountInfo = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid).Find(x => x.AccountId == (int)AccountType.商城奖金账户);
            if (accountInfo == null)
                accountInfo = new AccountInfo();
            UserInfo user = Users.GetUserById(WorkContext.Uid);
            if (user == null)
                return View("accounttranfer", new DrawCashModel()
                {
                    State = false,
                    Message = "会员信息错误"
                });
            if (user.IsDirSaleUser)
            {
                return View("accounttranfer", new DrawCashModel()
                {
                    State = false,
                    Message = "商城仅支持普通会员的转账，更高级别的会员请前往经销商系统。"
                });

            }
            if (!user.IsDirSaleUser && string.IsNullOrEmpty(user.IdCard))
                return View("accounttranfer", new DrawCashModel()
                {
                    State = false,
                    Message = "身份证信息未完善，请完善身份证信息，并保证身份证信息真实有效！",
                    JumpUrl = Url.Action("userinfo")
                });

            if (!user.IsDirSaleUser && string.IsNullOrEmpty(user.PayPassword))
                return View("accounttranfer", new DrawCashModel()
                {
                    State = false,
                    Message = "未设置支付密码！",//safeverify?act=updatePayPassword
                    JumpUrl = Url.Action("safeverify", new { act = "updatePayPassword" })
                });

            DrawCashModel model = new DrawCashModel()
            {
                State = state,
                Message = message,
                AccountInfo = accountInfo,
            };
            return View("accounttranfer", model);
        }
        /// <summary>
        /// 提交转账申请
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="BankName"></param>
        /// <param name="regionId"></param>
        /// <param name="BankAddress"></param>
        /// <param name="BankCardCode"></param>
        /// <param name="BankUserName"></param>
        /// <param name="PayPassWord"></param>
        /// <returns></returns>
        public ActionResult ApplyAccountTranfer(string Amount, string PayPassWord)
        {
            if (string.IsNullOrEmpty(Amount) || string.IsNullOrEmpty(PayPassWord))
                return Content("400");

            decimal tranferAmount = TypeHelper.StringToDecimal(Amount);
            AccountInfo accountInfo = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid).Find(x => x.AccountId == (int)AccountType.商城奖金账户);
            if (accountInfo==null)
                return Content("300");
            AccountInfo accountInfoIn = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid).Find(x => x.AccountId == (int)AccountType.商城钱包);
            if (accountInfoIn == null)
                return Content("300");
            if (accountInfo.Banlance < tranferAmount)
                return Content("300");
            if (WorkContext.PartUserInfo.IsDirSaleUser)
            {
                if (OrderUtils.GetPayPassword(PayPassWord, WorkContext.PartUserInfo.DirSaleUid) != SecureHelper.EncryptString(PayPassWord, DirSaleUserInfo.EncryptKey))
                    return Content("500");
            }
            else
            {
                if (Users.CreateUserPassword(PayPassWord, WorkContext.PartUserInfo.Salt) != WorkContext.PartUserInfo.PayPassword)
                    return Content("500");
            }
            string tranSN = "TF" + Randoms.CreateRandomValue(10,true);
            //更新账户
            VMall.Services.Account.UpdateAccountForOut(new AccountInfo()
            {
                AccountId = (int)AccountType.商城奖金账户,
                UserId = WorkContext.Uid,
                TotalOut = tranferAmount
            });
            VMall.Services.Account.CreateAccountDetail(new AccountDetailInfo()
            {
                AccountId = (int)AccountType.商城奖金账户,
                UserId = WorkContext.Uid,
                CreateTime = DateTime.Now,
                DetailType = (int)DetailType.账户间转账支出,
                OutAmount = tranferAmount,
                OrderCode = tranSN,
                AdminUid = 0,//system
                Status = 1,
                DetailDes = string.Format(AccountType.商城奖金账户 + "转账：转出金额:{0},", tranferAmount)
            });
            VMall.Services.Account.UpdateAccountForIn(new AccountInfo()
            {
                AccountId = (int)AccountType.商城钱包,
                UserId = WorkContext.Uid,
                TotalIn = tranferAmount
            });
            VMall.Services.Account.CreateAccountDetail(new AccountDetailInfo()
            {
                AccountId = (int)AccountType.商城钱包,
                UserId = WorkContext.Uid,
                CreateTime = DateTime.Now,
                DetailType = (int)DetailType.账户间转账收入,
                InAmount = tranferAmount,
                OrderCode = tranSN,
                AdminUid = 0,//system
                Status = 1,
                DetailDes = string.Format(AccountType.商城钱包 + "转账：转入金额:{0}", tranferAmount)
            });
            return Content("200");
        }
        
        #endregion

        #region 重销查询
        /// <summary>
        /// 重销查询
        /// </summary>
        /// <returns></returns>
        public ActionResult ChongXiaoQuery()
        {
            if (WorkContext.IsDirSaleUser)
            {
                decimal PVTotal1 = 0;// Orders.GetListForChongXiao(WorkContext.Uid);
                decimal PVTotal2 = AccountUtils.GetUserReorderPV(WorkContext.PartUserInfo.DirSaleUid, DateTime.Now.ToString("yyyy-MM-dd"));
                ViewData["PVTotal"] = PVTotal1 + PVTotal2;

                return View();
            }
            else
                return Content("");
        }
        #endregion

        #region 兑换码

        /// <summary>
        ///兑换码列表
        /// </summary>
        public ActionResult ExCodeList()
        {
            List<ExChangeCouponsInfo> exlist = ExChangeCoupons.GetList(string.Format(" uid={0}", WorkContext.Uid));

            return View(exlist);
        }


        #endregion

        #region 微商代理
        /// <summary>
        /// 申请代理 
        /// </summary>
        /// <param name="oids">订单id</param>
        /// <returns></returns>
        public ActionResult MyAgent()
        {
            bool state = true;
            int agentstate = 0;
            string message = string.Empty;
            List<UserInfo> userList = new List<Core.UserInfo>();
            PartUserInfo user = Users.GetPartUserById(WorkContext.Uid);
            if (user == null)
            {
                state = false;
                message = "会员信息错误";
            }
            if (user.AgentType <= 0)
            {
                state = false;
                message = "您还未拥有代理资格";
            }
            if (user.AgentType > 0 && !user.IsDirSaleUser)
            {
                state = true;
                agentstate = 1;
                message = "ok";
                return RedirectToAction("AgentInfo", new { uid = user.Uid, type = 1 });
            }
            if (user.AgentType > 0 && user.IsDirSaleUser)
            {
                state = true;
                agentstate = 2;
                message = "ok";
                userList = Users.GetSubRecommendListByPid(user, 1, 2000).FindAll(x => x.IsDirSaleUser == false && x.AgentType > 0);
            }

            MyAgentModel model = new MyAgentModel()
            {
                State = state,
                AgentState = agentstate,
                Message = message,
                UserList = userList
            };

            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">1 为自己安置，2为上级安置</param>
        /// <returns></returns>
        public ActionResult AgentInfo(int uid, int type = 1)
        {
            PartUserInfo user = Users.GetPartUserById(uid);
            if (user == null)
                return PromptView(WorkContext.UrlReferrer, "会员不存在");
            if (type == 1)
            {
                if (WorkContext.IsDirSaleUser)
                {
                    return PromptView(WorkContext.UrlReferrer, "会员已经是代理会员，无需再加入！");
                }
            }
            else
            {

            }
            PartUserInfo parentuser = AccountUtils.GetParentUserForDirSale(user);
            if (parentuser.Uid <= 0)
                return View();
            string pcode = AccountUtils.GetUserCode(parentuser.DirSaleUid);
            //string parentCode = OrderUtils.GetParentCode(WorkContext.Uid);
            ViewData["parentCode"] = pcode;
            ViewData["joinuid"] = uid;

            return View();
        }
        /// <summary>
        /// 加入直销
        /// </summary>
        /// <returns></returns>
        public ActionResult CreatDSMember()
        {
            //if (WorkContext.IsDirSaleUser)
            //{
            //    return PromptView(WorkContext.UrlReferrer, "会员已经是代理会员，无需再加入！");
            //}
            int joinuid = WebHelper.GetFormInt("joinuid");
            PartUserInfo joinUser = Users.GetPartUserById(joinuid);
            if (joinUser == null)
                return PromptView(WorkContext.UrlReferrer, "会员信息错误！");
            if (joinUser.IsDirSaleUser)
                return PromptView(WorkContext.UrlReferrer, "会员已经是代理会员，无需再加入！");

            string parentCode = WebHelper.GetFormString("parentCode");
            string managerCode = WebHelper.GetFormString("managerCode");
            int placeSide = WebHelper.GetFormInt("placeSide");
            string userPhone = WebHelper.GetFormString("userPhone");
            string realName = WebHelper.GetFormString("realName");
            string userCard = WebHelper.GetFormString("userCard");
            MemberInfo member = AccountUtils.CreateMember(WorkContext.PartUserInfo, realName, managerCode, placeSide, userCard, userPhone);
            return RedirectToAction("agentresult", new { ParentCode = member.ParentCode, ManagerCode = member.ManagerCode, joinuid = joinuid });

        }
        /// <summary>
        /// 加入结果
        /// </summary>
        /// <returns></returns>
        public ActionResult AgentResult(string ParentCode, string ManagerCode, int joinuid)
        {
            AgentResultModel model = new AgentResultModel();
            model.JoinUid = joinuid;
            model.ParentCode = ParentCode;
            model.ManagerCode = ManagerCode;
            return View("agentresult", model);
        }

        /// <summary>
        /// 提交申请代理 
        /// </summary>
        /// <param name="oids">订单id</param>
        /// <returns></returns>
        public ActionResult SubmitApply()
        {
            string agentType = WebHelper.GetFormString("agentType");
            string parentPhone = WebHelper.GetFormString("parentPhone");
            string name = WebHelper.GetFormString("name");
            string weixin = WebHelper.GetFormString("weixin");
            string userPhone = WebHelper.GetFormString("userPhone");
            string userCard = WebHelper.GetFormString("userCard");
            int regionId = WebHelper.GetFormInt("regionId");
            string address = WebHelper.GetFormString("address");
            SubmitApplyModel model = new SubmitApplyModel();
            if (!ValidateHelper.IsMobile(parentPhone) || !ValidateHelper.IsMobile(userPhone))
            {
                return View("applyresult", new SubmitApplyModel()
                {
                    State = false,
                    Message = "手机格式不正确"
                });
            }
            if (!ValidateHelper.IsIdCard(userCard))
            {
                return View("applyresult", new SubmitApplyModel()
                {
                    State = false,
                    Message = "身份证格式不正确"
                });
            }

            model.State = true;
            model.Message = "";

            return View("applyresult", model);
        }
        /// <summary>
        /// 库存查询
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStock()
        {
            string condition = string.Format(" T.[uid] = {0} ", WorkContext.Uid);
            int PageNumber = 1;
            int PageSize = 50;
            AgentStockModel model = new AgentStockModel();
            model.AgentStockList = new AgentStock().GetAgentStockList(string.Format(" uid={0} ", WorkContext.Uid), "", 1, 20);
            model.SendOrderList = new AgentSendOrder().AdminGetListByPage(condition, "", (PageNumber - 1) * PageSize + 1, PageNumber * PageSize);
            return View(model);
        }

        /// <summary>
        /// 库存详情
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStockDetail(int pid, int pageSize = 10, int page = 1)
        {
            AgentStockDetailModel model = new AgentStockDetailModel();
            string condition = string.Format(" uid={0} and pid={1} ", WorkContext.Uid, pid);
            PageModel pageModel = new PageModel(pageSize, page, new AgentStockDetail().GetRecordCount(condition));
            List<AgentStockDetailInfo> list = new AgentStockDetail().GetListByPage(condition, "creationdate desc ", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize);
            model.DetailList = list;
            model.PageModel = pageModel;
            model.Pid = pid;
            if (page >= 2)
                return PartialView("getstockdetailajax", model);
            return View(model);
        }

        /// <summary>
        /// 生成要货单
        /// </summary>
        /// <returns></returns>
        public ActionResult AddSendOrder(int pid)
        {
            AgentStockInfo info = new AgentStock().GetModel(string.Format(" pid={0} and uid={1} ", pid, WorkContext.Uid));
            if (info == null)
                return PromptView("库存不存在");
            List<PartProductInfo> productList = Products.GetProductListByWhere(string.Format(" storeid={0} and state=0 ", WebHelper.GetConfigSettingsInt("AgentStoreId")));

            AgentSendOrderModel model = new AgentSendOrderModel();
            model.ProductList = productList;
            model.Product = Products.GetPartProductById(info.Pid);
            model.ShipAddress = ShipAddresses.GetFullShipAddressList(info.Uid);
            model.AgentStock = info;
            return View(model);
        }
        /// <summary>
        /// 确认要货单
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmSendOrder(int pid, int asid)
        {
            AgentStockInfo info = new AgentStock().GetModel(asid);
            if (info == null)
                return PromptView("库存不存在");

            AgentSendOrderModel model = new AgentSendOrderModel();

            model.Product = Products.GetPartProductById(pid);
            model.ShipAddress = ShipAddresses.GetFullShipAddressList(info.Uid);
            model.AgentStock = info;
            model.ASid = asid;
            return View(model);
        }
        /// <summary>
        /// 生成要货单
        /// </summary>
        /// <returns></returns>
        public ActionResult CreatSendOrder()
        {
            int pid = WebHelper.GetFormInt("pid");
            int count = WebHelper.GetFormInt("count");
            int addressid = WebHelper.GetFormInt("addressid");
            int asid = WebHelper.GetFormInt("asid");

            if (addressid == 0)
                return PromptView("您还没有配送地址，请先添加");
            PartProductInfo productInfo = Products.GetPartProductById(pid);
            if (productInfo == null)
                return PromptView("产品不存在或产品已下架");
            AgentStockInfo info = new AgentStock().GetModel(asid);
            if (info == null)
                return PromptView("库存不存在");
            if (info.AgentAmount <= 0)
                return PromptView("库存金额余额不足");
            if (count <= 0)
                return PromptView("数量不能小于0");

            decimal singlePrice = new AgentStock().SingleAgentPrice(WorkContext.PartUserInfo, info.Pid);//原库存产品价格
            decimal itemPrice = new AgentStock().SingleAgentPriceFor70(WorkContext.PartUserInfo, pid);//换货产品价格
            int change = (int)Math.Floor(info.AgentAmount / itemPrice);//当前产品可换货多少当前选择产品
            if (count > change)
                return PromptView("库存金额余额不支持当前数量！");

            decimal changeAmount = itemPrice * count;
            decimal stockRemain = AgentStock.CutDecimalWithN((info.AgentAmount - changeAmount) / singlePrice, 4);

            info.Balance = stockRemain;
            info.AgentAmount = info.AgentAmount - changeAmount;

            FullShipAddressInfo address = ShipAddresses.GetFullShipAddressBySAId(addressid, WorkContext.Uid);

            new AgentStock().Update(info);
            string sendOSN = "TH8" + DateTime.Now.ToString("yyyyMMddHHmmss") + Randoms.CreateRandomValue(3, true);
            new AgentSendOrder().Add(new AgentSendOrderInfo()
            {
                Pid = pid,
                Uid = info.Uid,
                SendOSN = sendOSN,
                SendCount = count,
                RegionId = address.RegionId,
                Address = address.Address,
                Consignee = address.Consignee,
                Mobile = address.Mobile
            });

            new AgentStockDetail().AddDetail(info.Uid, info.Pid, 3, 0, changeAmount, info.AgentAmount, sendOSN, string.Format("填写要货单，产品{0},数量{1},金额：{2}", productInfo.Name, count, changeAmount), info.Uid, 0);
            return RedirectToAction("sendresult");
        }

        public ActionResult sendresult()
        {
            return View();
        }
        /// <summary>
        ///  代理提现
        /// </summary>
        /// <returns></returns>
        public ActionResult IncomeWithDraw()
        {
            bool state = true;
            string message = string.Empty;
            AccountInfo accountInfo = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid).Find(x => x.AccountId == (int)AccountType.代理账户);
            if (accountInfo == null)
                accountInfo = new AccountInfo();
            UserInfo user = Users.GetUserById(WorkContext.Uid);
            if (user == null)
                return View("incomedraw", new DrawCashModel()
                {
                    State = false,
                    Message = "会员信息错误"
                });
            if (!user.IsDirSaleUser && string.IsNullOrEmpty(user.IdCard))
                return View("incomedraw", new DrawCashModel()
                {
                    State = false,
                    Message = "身份证信息未完善，请完善身份证信息，并保证身份证信息真实有效！",
                    JumpUrl = Url.Action("userinfo")
                });
            //if (user.AgentType<=0)
            //{
            //    return View("incomedraw", new DrawCashModel()
            //    {
            //        State = false,
            //        Message = "您还未拥有代理资格！"
            //        //JumpUrl = Url.Action("")
            //    });
            //}
            //if (!user.IsDirSaleUser) {
            //    return View("incomedraw", new DrawCashModel()
            //    {
            //        State = false,
            //        Message = "未填写安置信息！",
            //        JumpUrl = Url.Action("myanent")
            //    });
            //}
            if (!user.IsDirSaleUser && string.IsNullOrEmpty(user.PayPassword))
                return View("incomedraw", new DrawCashModel()
                {
                    State = false,
                    Message = "未设置支付密码！",
                    JumpUrl = Url.Action("safeverify", new { act = "updatePayPassword" })
                });
            //if (accountInfo.Banlance < 100)

            //    return View("incomedraw", new DrawCashModel()
            //    {
            //        State = false,
            //        Message = "提现余额不足"
            //    });
            DrawCashModel model = new DrawCashModel()
            {
                State = state,
                Message = message,
                AccountInfo = accountInfo,
            };
            return View("incomedraw", model);
        }
        /// <summary>
        /// 提交代理提现申请
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="BankName"></param>
        /// <param name="regionId"></param>
        /// <param name="BankAddress"></param>
        /// <param name="BankCardCode"></param>
        /// <param name="BankUserName"></param>
        /// <param name="PayPassWord"></param>
        /// <returns></returns>
        public ActionResult ApplyIncomeDarw(string Amount, string BankName, string BankCardCode, string BankUserName, string PayPassWord)
        {
            if (string.IsNullOrEmpty(Amount) || string.IsNullOrEmpty(BankName) || string.IsNullOrEmpty(BankCardCode) || string.IsNullOrEmpty(BankUserName) || string.IsNullOrEmpty(PayPassWord))
                return Content("400");
            //List<RegionInfo> regions = Regions.GetRegionAndParentRegionById(TypeHelper.StringToInt(regionId));
            //if (regions.Count < 2)
            //    return Content("400");
            AccountInfo accountInfo = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid).Find(x => x.AccountId == (int)AccountType.代理账户);
            if (accountInfo.Banlance < 100)
                return Content("300");
            if (WorkContext.PartUserInfo.IsDirSaleUser)
            {
                if (OrderUtils.GetPayPassword(PayPassWord, WorkContext.PartUserInfo.DirSaleUid) != SecureHelper.EncryptString(PayPassWord, DirSaleUserInfo.EncryptKey))
                    return Content("500");
            }
            else
            {
                if (Users.CreateUserPassword(PayPassWord, WorkContext.PartUserInfo.Salt) != WorkContext.PartUserInfo.PayPassword)
                    return Content("500");
            }
            HaiMiDrawCashInfo info = new HaiMiDrawCashInfo();
            info.Uid = WorkContext.Uid;
            info.AccountId = (int)AccountType.代理账户;
            info.DrawCashSN = "YJTX" + DateTime.Now.ToString("yyyyMMddHHmmss") + Randoms.CreateRandomValue(5);
            info.Amount = TypeHelper.StringToDecimal(Amount);
            //info.Poundage = TypeHelper.StringToDecimal(Amount) * BMAConfig.MallConfig.YJFeeRate;
            //info.TaxAmount = TypeHelper.StringToDecimal(Amount) * BMAConfig.MallConfig.HMTaxRate;
            info.ActualAmount = info.Amount - info.Poundage - info.TaxAmount;
            info.State = 1;
            info.BankName = BankName;
            info.BankProvice = "";// regions.Find(x => x.Layer == 1) != null ? regions.Find(x => x.Layer == 1).Name : "";
            info.BankCity = "";// regions.Find(x => x.Layer == 2) != null ? regions.Find(x => x.Layer == 2).Name : "";
            info.BankAddress = "";// BankAddress;
            info.BankCardCode = BankCardCode;
            info.BankUserName = BankUserName;

            haiMiDrawCashBLL.Add(info);
            //更新佣金账户

            VMall.Services.Account.UpdateAccountForOut(new AccountInfo()
            {
                AccountId = (int)AccountType.代理账户,
                UserId = WorkContext.Uid,
                TotalOut = info.Amount
            });
            VMall.Services.Account.CreateAccountDetail(new AccountDetailInfo()
            {
                AccountId = (int)AccountType.代理账户,
                UserId = WorkContext.Uid,
                CreateTime = DateTime.Now,
                DetailType = (int)DetailType.提现支出,
                OutAmount = info.Amount,
                OrderCode = info.DrawCashSN,
                AdminUid = 0,//system
                Status = 1,
                DetailDes = string.Format("代理提现：提现金额:{0}，税费{1},手续费{2},实际提现金额{3}", info.Amount, info.TaxAmount, info.Poundage, info.ActualAmount)
            });

            return Content("200");
        }

        /// <summary>
        /// 代理提现历史记录
        /// </summary>
        /// <returns></returns>
        public ActionResult IncomtDrawHistory(int pageSize = 10, int pageNumber = 1)
        {
            DrawCashHistoryModel model = new DrawCashHistoryModel();
            string condition = " Uid=" + WorkContext.Uid + " and AccountId=" + (int)AccountType.代理账户;
            PageModel pageModel = new PageModel(pageSize, pageNumber, haiMiDrawCashBLL.GetRecordCount(condition));
            List<HaiMiDrawCashInfo> list = haiMiDrawCashBLL.GetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize);
            model.PageModel = pageModel;
            model.HistoryList = list;
            return View(model);
        }

        /// <summary>
        ///  佣金提现
        /// </summary>
        /// <returns></returns>
        public ActionResult CommissionWithDraw()
        {
            bool state = true;
            string message = string.Empty;
            AccountInfo accountInfo = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid).Find(x => x.AccountId == (int)AccountType.佣金账户);
            if (accountInfo == null)
                accountInfo = new AccountInfo();
            UserInfo user = Users.GetUserById(WorkContext.Uid);
            if (user == null)
                return View("commissiondraw", new DrawCashModel()
                {
                    State = false,
                    Message = "会员信息错误"
                });
            if (!user.IsDirSaleUser && string.IsNullOrEmpty(user.IdCard))
                return View("commissiondraw", new DrawCashModel()
                {
                    State = false,
                    Message = "身份证信息未完善，请完善身份证信息，并保证身份证信息真实有效！",
                    JumpUrl = Url.Action("userinfo")
                });

            if (!user.IsDirSaleUser && string.IsNullOrEmpty(user.PayPassword))
                return View("commissiondraw", new DrawCashModel()
                {
                    State = false,
                    Message = "未设置支付密码！",
                    JumpUrl = Url.Action("safeverify", new { act = "updatePayPassword" })
                });
            //if (accountInfo.Banlance < 100)

            //    return View("incomedraw", new DrawCashModel()
            //    {
            //        State = false,
            //        Message = "提现余额不足"
            //    });
            DrawCashModel model = new DrawCashModel()
            {
                State = state,
                Message = message,
                AccountInfo = accountInfo,
            };
            return View("commissiondraw", model);
        }
        /// <summary>
        /// 提交佣金提现申请
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="BankName"></param>
        /// <param name="regionId"></param>
        /// <param name="BankAddress"></param>
        /// <param name="BankCardCode"></param>
        /// <param name="BankUserName"></param>
        /// <param name="PayPassWord"></param>
        /// <returns></returns>
        public ActionResult ApplyCommissionDarw(string Amount, string BankName, string BankCardCode, string BankUserName, string PayPassWord)
        {
            lock (ctx)
            {
                if (string.IsNullOrEmpty(Amount) || string.IsNullOrEmpty(BankName) || string.IsNullOrEmpty(BankCardCode) || string.IsNullOrEmpty(BankUserName) || string.IsNullOrEmpty(PayPassWord))
                    return Content("400");

                AccountInfo accountInfo = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid).Find(x => x.AccountId == (int)AccountType.佣金账户);
                if (accountInfo.Banlance < 100)
                    return Content("300");
                if (WorkContext.PartUserInfo.IsDirSaleUser)
                {
                    if (OrderUtils.GetPayPassword(PayPassWord, WorkContext.PartUserInfo.DirSaleUid) != SecureHelper.EncryptString(PayPassWord, DirSaleUserInfo.EncryptKey))
                        return Content("500");
                }
                else
                {
                    if (Users.CreateUserPassword(PayPassWord, WorkContext.PartUserInfo.Salt) != WorkContext.PartUserInfo.PayPassword)
                        return Content("500");
                }
                HaiMiDrawCashInfo info = new HaiMiDrawCashInfo();
                info.Uid = WorkContext.Uid;
                info.AccountId = (int)AccountType.佣金账户;
                info.DrawCashSN = "YJTX" + DateTime.Now.ToString("yyyyMMddHHmmss") + Randoms.CreateRandomValue(5);
                info.Amount = TypeHelper.StringToDecimal(Amount);
                info.Poundage = TypeHelper.StringToDecimal(Amount) * BMAConfig.MallConfig.YJFeeRate;
                //info.TaxAmount = TypeHelper.StringToDecimal(Amount) * BMAConfig.MallConfig.YJFeeRate;
                info.ActualAmount = info.Amount - info.Poundage - info.TaxAmount;
                info.State = 1;
                info.BankName = BankName;
                info.BankProvice = "";// regions.Find(x => x.Layer == 1) != null ? regions.Find(x => x.Layer == 1).Name : "";
                info.BankCity = "";// regions.Find(x => x.Layer == 2) != null ? regions.Find(x => x.Layer == 2).Name : "";
                info.BankAddress = "";//BankAddress;
                info.BankCardCode = BankCardCode;
                info.BankUserName = BankUserName;

                haiMiDrawCashBLL.Add(info);
                //更新佣金账户
                VMall.Services.Account.UpdateAccountForOut(new AccountInfo()
                {
                    AccountId = (int)AccountType.佣金账户,
                    UserId = WorkContext.Uid,
                    TotalOut = info.Amount
                });
                VMall.Services.Account.CreateAccountDetail(new AccountDetailInfo()
                {
                    AccountId = (int)AccountType.佣金账户,
                    UserId = WorkContext.Uid,
                    CreateTime = DateTime.Now,
                    DetailType = (int)DetailType.提现支出,
                    OutAmount = info.Amount,
                    OrderCode = info.DrawCashSN,
                    AdminUid = 0,//system
                    Status = 1,
                    DetailDes = string.Format("佣金账户提现：提现金额:{0}，手续费{1},实际提现金额{2}", info.Amount, info.Poundage, info.ActualAmount)
                });

                return Content("200");
            }
        }

        /// <summary>
        /// 佣金提现历史记录
        /// </summary>
        /// <returns></returns>
        public ActionResult CommissionDrawHistory(int pageSize = 10, int pageNumber = 1)
        {
            DrawCashHistoryModel model = new DrawCashHistoryModel();
            string condition = " Uid=" + WorkContext.Uid + " and AccountId=" + (int)AccountType.佣金账户;
            PageModel pageModel = new PageModel(pageSize, pageNumber, haiMiDrawCashBLL.GetRecordCount(condition));
            List<HaiMiDrawCashInfo> list = haiMiDrawCashBLL.GetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize);
            model.PageModel = pageModel;
            model.HistoryList = list;
            return View(model);
        }

        #endregion

        #region 咖啡余额申请转移施惠葆
        Kftransfer2severb kf2severbBll = new Kftransfer2severb();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Kf2SeverbApply()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Kf2SeverbApplySubmit()
        {
            //'Amount': Amount, 'CardNumber': CardNumber, 'CardUserName': CardUserName, 'CardMobile': CardMobile, 'verifyCode': verifyCode 
            decimal Amount = TypeHelper.StringToDecimal(WebHelper.GetFormString("Amount"));
            string CardNumber = WebHelper.GetFormString("CardNumber");
            string CardUserName = WebHelper.GetFormString("CardUserName");
            string CardMobile = WebHelper.GetFormString("CardMobile");
            string Remark = WebHelper.GetFormString("Remark");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            StringBuilder errorList = new StringBuilder("[");
            if (Amount <= 0 || CardNumber == "" || CardUserName == "" || CardMobile == "" || verifyCode == "")
                return AjaxResult("error", "[{\"key\":\"parmaerror\",\"msg\":\"缺少必填参数\"}]", true);
            //验证码
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
                return AjaxResult("error", "[{\"key\":\"codeerror\",\"msg\":\"验证码不正确\"}]", true);
            string sendOSN = "K2S" + DateTime.Now.ToString("yyyyMMddHHmmss") + Randoms.CreateRandomValue(3, true);
            Kftransfer2severbInfo info = new Kftransfer2severbInfo()
            {
                ApplySN = sendOSN,
                CreateTime = DateTime.Now,
                Uid = WorkContext.Uid,
                Amount = Amount,
                CardNumber = CardNumber,
                CardMobile = CardMobile,
                CardUserName = CardUserName,
                CardType = 1,
                Remark = Remark

            };
            kf2severbBll.AddModel(info);

            return AjaxResult("success", "提交成功");
            //else
            //    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"提交失败\"}]", true);
        }

        /// <summary>
        /// 报单历史记录
        /// </summary>
        /// <returns></returns>
        public ActionResult Kf2SeverbHistory(int uid, int pageSize = 10, int pageNumber = 1)
        {
            Kf2SeverbHistoryModel model = new Kf2SeverbHistoryModel();
            string condition = " Uid=" + uid;
            PageModel pageModel = new PageModel(pageSize, pageNumber, kf2severbBll.GetCount(condition));

            model.PageModel = pageModel;
            model.HistoryList = kf2severbBll.GetList(condition, pageModel.PageSize, pageModel.PageIndex); ;
            return View(model);
        }

        #endregion

        #region 会员帮助中心
        /// <summary>
        /// 帮助
        /// </summary>
        /// <returns></returns>
        public ActionResult Help()
        {
            return View();
        }
        #endregion

        #region 用户消息

        Complain complainBLL = new Complain();
        Inform informBLL = new Inform();
        Informtype infoTypeBLL = new Informtype();

        /// <summary>
        /// 用户消息列表
        /// </summary>
        public ActionResult InformList()
        {
            int page = WebHelper.GetQueryInt("page");//当前页数
            string condition = string.Format(" uid= {0}", WorkContext.Uid);
            PageModel pageModel = new PageModel(10, page, informBLL.GetCount(condition));
            InformListModel model = new InformListModel()
            {
                InformList = informBLL.GetList(condition, pageModel.PageSize, pageModel.PageNumber),
                //GetFavoriteProductList(pageModel.PageSize, pageModel.PageNumber, WorkContext.Uid),
                PageModel = pageModel
            };
            if (page >= 2)
                return PartialView("informlistajax", model);
            return View(model);
        }

        /// <summary>
        /// 消息详情
        /// </summary>
        public ActionResult InformDetail()
        {
            //id
            int id = WebHelper.GetQueryInt("id");
            //商品信息
            InformInfo info = informBLL.GetModel(id);
            if (info == null)
                return AjaxResult("noproduct", "消息不存在");
            info.state = 1;
            informBLL.Update(info, "state");
            return View(info);
        }

        #endregion

        #region 用户投诉

        /// <summary>
        /// 用户投诉列表
        /// </summary>
        public ActionResult ComplainList()
        {
            int page = WebHelper.GetQueryInt("page");//当前页数
            string condition = string.Format(" uid= {0}", WorkContext.Uid);
            PageModel pageModel = new PageModel(10, page, complainBLL.GetCount(condition));
            ComplainListModel model = new ComplainListModel()
            {
                ComplainList = complainBLL.GetList(condition, pageModel.PageSize, pageModel.PageNumber),
                PageModel = pageModel
            };
            if (page >= 2)
                return PartialView("complainlistajax", model);
            return View(model);
        }

        /// <summary>
        /// 投诉详情
        /// </summary>
        public ActionResult ComplainDetail()
        {
            //id
            int id = WebHelper.GetQueryInt("id");
            //商品信息
            ComplainInfo info = complainBLL.GetModel(id);
            if (info == null)
                return AjaxResult("noproduct", "消息不存在");
            return View(info);
        }

        /// <summary>
        /// 添加投诉
        /// </summary>
        public ActionResult AddComplain()
        {
            return View();
        }
        /// <summary>
        /// 添加投诉
        /// </summary>
        public ActionResult CreateComplain()
        {
            //if (WebHelper.IsGet())
            //{
            //    ComplainInfo info = new ComplainInfo();
            //    return View(info);
            //}
            //else
            //{

            int uid = WorkContext.Uid;
            DateTime complaintime = DateTime.Now;
            string complainNickName = WorkContext.PartUserInfo.NickName;
            string complainMsg = WebHelper.GetFormString("complainMsg");
            string ip = WebHelper.GetIP();

            ComplainInfo info = new ComplainInfo();
            info.uid = uid;
            info.state = 0;
            info.complaintime = complaintime;
            info.complainnickname = complainNickName;
            info.complainmsg = complainMsg;
            info.complainip = ip;
            decimal obj = (decimal)complainBLL.Add(info);
            if (obj <= 0)
                return AjaxResult("error", "提交失败");

            else
                return Content("<script>alert('提交成功');window.location.href='/mob/ucenter/ComplainList'</script>");
            //}
        }
        #endregion

        #region 引流赠品

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderGiftDetail()
        {
            OrderGiftInfo info = orderGiftBLL.GetModel(string.Format("uid={0}", WorkContext.Uid));
            return View(info);
        }

        /// <summary>
        /// 确认订单
        /// </summary>
        public ActionResult ConfirmOrderGift()
        {
            int pid = WebHelper.GetQueryInt("pid");
            if (pid == 0)
                pid = TypeHelper.StringToInt(WebHelper.GetFormString("pid"));
            int pCount = 1;// TypeHelper.StringToInt(WebHelper.GetFormString("pCount"));
            int id = WebHelper.GetQueryInt("id");
            if (id == 0)
                id = TypeHelper.StringToInt(WebHelper.GetFormString("id"));

            //配送地址id
            int saId = WebHelper.GetFormInt("saId");
            //支付插件名称
            string payName = WebHelper.GetFormString("payName");

            //订单商品列表
            PartProductInfo product = Products.GetPartProductById(pid);
            if (product == null)
                return PromptView("商品已下架或商品不存在，请选择正确商品");

            List<OrderProductInfo> orderProductList = new List<OrderProductInfo>();
            OrderProductInfo orderProduct = new OrderProductInfo();
            orderProduct.Uid = WorkContext.Uid;
            orderProduct.Pid = pid;
            orderProduct.RealCount = 1;
            orderProduct.BuyCount = 1;
            orderProduct.AddTime = DateTime.Now;
            orderProduct.StoreId = product.StoreId;
            orderProduct.StoreSTid = product.StoreSTid;
            orderProduct.Name = product.Name;

            ConfirmOrderGiftModel model = new ConfirmOrderGiftModel();
            if (saId > 0)
                model.DefaultFullShipAddressInfo = ShipAddresses.GetFullShipAddressBySAId(saId, WorkContext.Uid);
            if (model.DefaultFullShipAddressInfo == null)
                model.DefaultFullShipAddressInfo = ShipAddresses.GetDefaultFullShipAddress(WorkContext.Uid);

            if (payName.Length > 0)
                model.DefaultPayPluginInfo = Plugins.GetPayPluginBySystemName(payName);
            if (model.DefaultPayPluginInfo == null)
                model.DefaultPayPluginInfo = Plugins.GetDefaultPayPlugin();
            model.PayPluginList = Plugins.GetPayPluginList();

            bool isSend = true;
            //需要添加的商品列表
            List<OrderProductInfo> addOrderProductList = new List<OrderProductInfo>();
            //初始化订单商品
            OrderProductInfo mainOrderProductInfo = Carts.BuildOrderProduct(product);
            mainOrderProductInfo.Sid = "";
            mainOrderProductInfo.Uid = WorkContext.Uid;
            mainOrderProductInfo.AddTime = DateTime.Now;
            mainOrderProductInfo.DiscountPrice = product.ShopPrice;
            int buyCount = pCount;
            mainOrderProductInfo.RealCount = buyCount;
            mainOrderProductInfo.BuyCount = buyCount;
            //将商品添加到"需要添加的商品列表"中
            addOrderProductList.Add(mainOrderProductInfo);

            model.AllShipFee = model.DefaultFullShipAddressInfo != null ? Orders.GetShipFee(model.DefaultFullShipAddressInfo.ProvinceId, model.DefaultFullShipAddressInfo.CityId, addOrderProductList, ref isSend) : 0;
            //if (!isSend)
            //    return AjaxResult("oversend", "订单中有不支持配送产品，请重新选择地址");

            decimal ShipFee = model.DefaultFullShipAddressInfo != null ? 15 : 0;

            model.isSend = isSend;
            //model.AllShipFee = ShipFee;

            model.AllProductAmount = product.ShopPrice;
            model.AllOrderAmount = model.AllProductAmount;
            model.Product = product;
            model.Pid = pid;
            model.Id = id;
            model.AllOrderAmount = model.AllProductAmount + model.AllShipFee;

            return View(model);
        }

        private static object _locker = new object();//锁对象

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SubmitOrderGift()
        {
            lock (_locker)
            {
                #region 获取表单参数
                int id = TypeHelper.StringToInt(WebHelper.GetFormString("Id"));
                int pid = TypeHelper.StringToInt(WebHelper.GetFormString("pid"));
                int pCount = 1;// TypeHelper.StringToInt(WebHelper.GetFormString("pCount"));
                int saId = WebHelper.GetFormInt("saId");//配送地址id
                string payName = WebHelper.GetFormString("payName");//支付方式名称
                string buyerRemark = WebHelper.GetFormString("buyerRemark");//备注

                #endregion

                #region 提交参数验证

                //验证支付方式是否为空
                PluginInfo payPluginInfo = Plugins.GetPayPluginBySystemName(payName);
                if (payPluginInfo == null)
                    return AjaxResult("empaypay", "请选择支付方式");

                //验证配送地址是否为空
                FullShipAddressInfo fullShipAddressInfo = ShipAddresses.GetFullShipAddressBySAId(saId, WorkContext.Uid);
                if (fullShipAddressInfo == null)
                    return AjaxResult("emptysaid", "请选择配送地址");

                OrderGiftInfo info = orderGiftBLL.GetModel(id);

                if (info == null)
                    return AjaxResult("errorcount", "记录不存在");
                if (info.UseCount >= info.GiftCount)
                    return AjaxResult("errorcount", "可领用数量为0");
                if (info.StartTime >= DateTime.Now)
                    return AjaxResult("errortime", "未到领用时间");
                if (info.EndTime <= DateTime.Now)
                    return AjaxResult("errortime", "赠品已过期");
                if (info.LastModify >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) && info.LastModify <= new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1))
                    return AjaxResult("errortime", "当月已领用");
                #endregion

                ConfirmOrderModel model = new ConfirmOrderModel();
                bool isSend = true;

                //验证已经通过,进行订单保存
                string oidList = "";
                decimal AllMoney = 0;
                //创建订单
                PartProductInfo partProductInfo = Products.GetPartProductById(pid);
                //将商品添加到购物车
                Carts.ClearCart(WorkContext.Uid, "");

                //需要添加的商品列表
                List<OrderProductInfo> addOrderProductList = new List<OrderProductInfo>();
                //初始化订单商品
                OrderProductInfo mainOrderProductInfo = Carts.BuildOrderProduct(partProductInfo);
                mainOrderProductInfo.Sid = "";
                mainOrderProductInfo.Uid = WorkContext.Uid;
                mainOrderProductInfo.AddTime = DateTime.Now;
                mainOrderProductInfo.DiscountPrice = partProductInfo.ShopPrice;
                int buyCount = pCount;
                mainOrderProductInfo.RealCount = buyCount;
                mainOrderProductInfo.BuyCount = buyCount;
                //将商品添加到"需要添加的商品列表"中
                addOrderProductList.Add(mainOrderProductInfo);

                model.AllShipFee = Orders.GetShipFee(fullShipAddressInfo.ProvinceId, fullShipAddressInfo.CityId, addOrderProductList, ref isSend);// Orders.GetShipFee(fullShipAddressInfo.ProvinceId, fullShipAddressInfo.CityId, addOrderProductList, ref isSend);
                if (!isSend)
                    return AjaxResult("oversend", "订单中有不支持配送产品，请重新选择地址");

                //将需要添加的商品持久化
                Carts.AddOrderProductList(addOrderProductList);

                PartUserInfo orderUser = Users.GetPartUserById(WorkContext.Uid);
                StoreInfo storeInfo = Stores.GetStoreById(partProductInfo.StoreId);
                DateTime bestTime = DateTime.Now;
                PartUserInfo operateUserInfo = null;
                operateUserInfo = WorkContext.PartUserInfo;
                OrderInfo orderInfo = Orders.CreateOrder_ForMC(orderUser, storeInfo, partProductInfo, payPluginInfo, fullShipAddressInfo.Consignee, fullShipAddressInfo.Mobile, fullShipAddressInfo.RegionId, fullShipAddressInfo.Address, WorkContext.IP, pCount, pid, buyerRemark, "", fullShipAddressInfo, addOrderProductList, id);

                if (orderInfo != null)
                {
                    oidList = orderInfo.Oid + ",";
                    AllMoney = orderInfo.SurplusMoney;
                }
                else
                    return AjaxResult("error", "提交失败!请重新提交");

                return AjaxResult("success", Url.Action("payshow", "order", new RouteValueDictionary { { "oidList", oidList.TrimEnd(',') } }));

            }
        }
        #endregion

        #region 基类方法

        protected sealed override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            //不允许游客访问
            if (WorkContext.Uid < 1)
            {
                if (WorkContext.IsHttpAjax)//如果为ajax请求
                    filterContext.Result = Content("nologin");
                else//如果为普通请求
                    filterContext.Result = RedirectToAction("login", "account", new RouteValueDictionary { { "returnUrl", WorkContext.Url } });
            }
        }
        #endregion
    }
}
