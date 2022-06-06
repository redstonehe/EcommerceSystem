using System;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Mobile.Models;

namespace VMall.Web.Mobile.Controllers
{
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web.Script.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    /// <summary>
    /// 账号控制器类
    /// </summary>
    public partial class AccountController : BaseMobileController
    {
        private static object _locker = new object();//锁对象
        private static string dirSaleApiUrl = WebSiteConfig.DirsaleApiUrl;
        JavaScriptSerializer js = new JavaScriptSerializer();


        /// <summary>
        /// 登录
        /// </summary>
        public ActionResult Login()
        {
            string returnUrl = WebHelper.GetQueryString("returnUrl");
            if (returnUrl.Length == 0)
                returnUrl = Url.Action("index", "home");
            if (returnUrl.Contains("Mindex"))
                returnUrl = Url.Action("index", "home");

            if (WorkContext.MallConfig.LoginType == "")
                return PromptView(returnUrl, "商城目前已经关闭登陆功能!");
            if (WorkContext.Uid > 0)
                //return PromptView(returnUrl, "您已经登录，无须重复登录!");
                return RedirectToAction("index", "home");
            if (WorkContext.MallConfig.LoginFailTimes != 0 && LoginFailLogs.GetLoginFailTimesByIp(WorkContext.IP) >= WorkContext.MallConfig.LoginFailTimes)
                return PromptView(returnUrl, "您已经输入错误" + WorkContext.MallConfig.LoginFailTimes + "次密码，请15分钟后再登陆!");

            //get请求
            if (WebHelper.IsGet())
            {
                LoginModel model = new LoginModel();

                model.ReturnUrl = returnUrl;
                model.ShadowName = WorkContext.MallConfig.ShadowName;
                model.IsRemember = WorkContext.MallConfig.IsRemember == 1;
                model.IsVerifyCode = CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages);
                model.OAuthPluginList = Plugins.GetOAuthPluginList();

                return View(model);
            }

            //ajax请求
            string accountName = WebHelper.GetFormString(WorkContext.MallConfig.ShadowName);
            string password = WebHelper.GetFormString("password");
            string verifyCode = WebHelper.GetFormString("verifyCode");
            int isRemember = WebHelper.GetFormInt("isRemember");

            StringBuilder errorList = new StringBuilder("[");
            #region 验证

            //验证账户名
            Regex regNum = new Regex(@"[\u4e00-\u9fa5]+");
            if (string.IsNullOrWhiteSpace(accountName))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不能为空", "}");
            }
            else if (regNum.IsMatch(accountName))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不能包含中文字符", "}");
            }
            else if (accountName.Length < 2 || accountName.Length > 50)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名必须大于2且不大于50个字符", "}");
            }
            else if ((!SecureHelper.IsSafeSqlString(accountName, false)))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不存在", "}");
            }

            //验证密码
            if (string.IsNullOrWhiteSpace(password))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码不能为空", "}");
            }
            else if (password.Length < 6 || password.Length > 32)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码必须大于6且不大于32个字符", "}");
            }

            //验证验证码
            if (CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages))
            {
                if (string.IsNullOrWhiteSpace(verifyCode))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "verifyCode", "验证码不能为空", "}");
                }
                else if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "verifyCode", "验证码不正确", "}");
                }
            }

            //当以上验证全部通过时
            int loginType = 1;
            int accountNameType = 1;
            bool isDirSaleUser = false;
            PartUserInfo partUserInfo = null;
            if (errorList.Length == 1)
            {
                if (BMAConfig.MallConfig.LoginType.Contains("2") && ValidateHelper.IsEmail(accountName))//邮箱登陆
                {
                    accountNameType = 2;
                    partUserInfo = Users.GetPartUserByEmail(accountName);
                    if (partUserInfo == null)
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "邮箱不存在", "}");
                }
                else if (BMAConfig.MallConfig.LoginType.Contains("3") && ValidateHelper.IsMobile(accountName))//手机登陆
                {
                    accountNameType = 3;
                    partUserInfo = Users.GetPartUserByMobile(accountName);
                    if (partUserInfo == null)
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "手机不存在", "}");
                }
                else if (BMAConfig.MallConfig.LoginType.Contains("1"))//用户名登陆
                {
                    accountNameType = 1;
                    partUserInfo = Users.GetPartUserByName(accountName);
                    if (partUserInfo == null)
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "用户名不存在", "}");
                }
                //判断密码是否正确
                if (partUserInfo != null && Users.CreateUserPassword(password, partUserInfo.Salt) != partUserInfo.Password)
                {
                    //直销会员密码不正确去直销系统再次拉取并更新最新的密码
                    if (partUserInfo.IsDirSaleUser)
                    {
                        //string url = dirSaleApiUrl + "/api/User/UserLogin?userName=" + accountName + "&password=" + password;

                        //string FromDirSale = WebHelper.DoGet(url);
                        string FromDirSale = AccountUtils.UserLogin(accountName, password);
                        JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                        JToken token = (JToken)jsonObject;
                        if (token["Result"].ToString() == "0")
                        {
                            //string p = Users.CreateUserPassword(password, partUserInfo.Salt);
                            Users.UpdatePartUserPwd(partUserInfo, password);
                        }
                        else
                        {
                            LoginFailLogs.AddLoginFailTimes(WorkContext.IP, DateTime.Now);//增加登陆失败次数
                            errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码不正确", "}");
                        }
                    }
                    else
                    {
                        LoginFailLogs.AddLoginFailTimes(WorkContext.IP, DateTime.Now);//增加登陆失败次数
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码不正确", "}");
                    }

                }


                //如果汇购系统中不存在该会员 则去直销系统登陆 并在汇购中创建同样的一个辅助账号 同步双方的账号信息
                int dirSaleUid = 0;
                string dirSaleUserCode = "";
                int dirSalePid = 0;
                string dirSaleMobile = "";
                bool isNewUserByDirSale = false;//是否通过直销会员新建汇购会员
                string dirSaleNickName = string.Empty;
                int Rank = 0;//直销会员级别代码 1=铜卡 2=银卡会员，3=金卡会员，4=白金卡会员。
                if (partUserInfo == null)
                {
                    //直销会员通过接口登陆 返回uid信息

                    //string url = dirSaleApiUrl + "/api/User/UserLogin?userName=" + accountName + "&password=" + password;

                    //string FromDirSale = WebHelper.DoGet(url);
                    string FromDirSale = AccountUtils.UserLogin(accountName, password);
                    JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                    JToken token = (JToken)jsonObject;
                    if (token["Result"].ToString() == "0")
                    {
                        dirSaleUid = TypeHelper.ObjectToInt(token["Info"].SelectToken("UserId"));
                        dirSaleUserCode = token["Info"].SelectToken("UserCode").ToString();
                        dirSaleNickName = token["Info"].SelectToken("NickName").ToString();
                        dirSalePid = TypeHelper.ObjectToInt(token["Info"].SelectToken("ParentId"));
                        dirSaleMobile = token["Info"].SelectToken("UserPhone").ToString();
                        Rank = token["Info"].SelectToken("Rank") != null ? TypeHelper.ObjectToInt(token["Info"].SelectToken("Rank")) : 0;
                        PartUserInfo dirSaleInfo = Users.GetPartUserInfoByDirSaleUid(dirSaleUid);
                        if (dirSaleInfo != null)
                        {
                            if (!Users.IsExistUserName(dirSaleUserCode))
                                dirSaleInfo.UserName = dirSaleUserCode;
                            if (!Users.IsExistMobile(dirSaleMobile))
                                dirSaleInfo.Mobile = dirSaleMobile;
                            Users.UpdatePartUser(dirSaleInfo);
                        }
                        else
                        {
                            isNewUserByDirSale = true;
                        }
                    }
                    else
                    {
                        errorList.Clear();
                        errorList.Append("[");
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", token["Msg"].ToString(), "}");
                    }
                }
                UserInfo userInfo = new UserInfo();
                if (dirSaleUid > 0 && isNewUserByDirSale)//需要创建辅助账号
                {
                    #region 绑定用户信息
                    if (accountNameType == 1)
                    {
                        userInfo.UserName = accountName;
                        userInfo.Email = string.Empty;
                        userInfo.Mobile = dirSaleMobile;
                    }
                    else if (accountNameType == 2)
                    {
                        userInfo.UserName = string.Empty;
                        userInfo.Email = accountName;
                        userInfo.Mobile = string.Empty;
                    }
                    else if (accountNameType == 3)
                    {
                        userInfo.UserName = dirSaleUserCode;
                        userInfo.Email = string.Empty;
                        userInfo.Mobile = accountName;
                    }

                    userInfo.Salt = Randoms.CreateRandomValue(6);
                    userInfo.Password = Users.CreateUserPassword(password, userInfo.Salt);
                    userInfo.UserRid = UserRanks.GetLowestUserRank().UserRid;
                    userInfo.StoreId = 0;
                    userInfo.MallAGid = 1;//非管理员组


                    try
                    {
                        if (ValidateHelper.IsMobile(accountName))
                        {
                            userInfo.NickName = accountName.Substring(0, 3) + "****" + accountName.Substring(7, 4);
                        }
                        else if (ValidateHelper.IsEmail(accountName))
                        {
                            userInfo.NickName = accountName.Split('@')[0];
                        }
                        else
                        {
                            userInfo.NickName = accountName;
                        }
                    }
                    catch
                    {
                        userInfo.NickName = "hk_" + Randoms.CreateRandomValue(7);
                    }

                    userInfo.Avatar = "";
                    userInfo.PayCredits = 0;
                    userInfo.RankCredits = 0;
                    userInfo.VerifyEmail = 0;
                    userInfo.VerifyMobile = 0;

                    userInfo.LastVisitIP = WorkContext.IP;
                    userInfo.LastVisitRgId = WorkContext.RegionId;
                    userInfo.LastVisitTime = DateTime.Now;
                    userInfo.RegisterIP = WorkContext.IP;
                    userInfo.RegisterRgId = WorkContext.RegionId;
                    userInfo.RegisterTime = DateTime.Now;

                    userInfo.Gender = WebHelper.GetFormInt("gender");
                    userInfo.RealName = WebHelper.HtmlEncode(WebHelper.GetFormString("realName"));
                    userInfo.Bday = new DateTime(1900, 1, 1);
                    userInfo.IdCard = WebHelper.GetFormString("idCard");
                    userInfo.RegionId = WebHelper.GetFormInt("regionId");
                    userInfo.Address = WebHelper.HtmlEncode(WebHelper.GetFormString("address"));
                    userInfo.Bio = WebHelper.HtmlEncode(WebHelper.GetFormString("bio"));

                    #endregion

                    userInfo.Pid = dirSalePid;
                    userInfo.Ptype = (int)UserPanertType.DirSaleUser;

                    userInfo.IsDirSaleUser = true;
                    userInfo.DirSaleUid = dirSaleUid;
                    userInfo.MaxCashCount = 5;
                    if (Rank == 4)//4白金卡=VIP3,3金卡=星级经销商2，2银卡=事业伙伴1
                        userInfo.Ds2AgentRank = 3;
                    else if (Rank == 3)
                        userInfo.Ds2AgentRank = 2;
                    else if (Rank == 2)
                        userInfo.Ds2AgentRank = 1;
                    else
                        userInfo.Ds2AgentRank = 0;
                    //创建用户
                    userInfo.Uid = Users.CreateUser(userInfo);

                    //添加用户失败
                    if (userInfo.Uid < 1)
                        return AjaxResult("exception", "登陆失败,请联系管理员");

                    //发放注册积分
                    Credits.SendRegisterCredits(ref userInfo, DateTime.Now);

                    //更新购物车中用户id
                    //Carts.UpdateCartUidBySid(userInfo.Uid, WorkContext.Sid);
                    //将用户信息写入cookie
                    //MallUtils.SetUserCookie(userInfo, 0);

                    ////发送注册欢迎信息
                    //if (WorkContext.MallConfig.IsWebcomeMsg == 1)
                    //{
                    //    if (userInfo.Email.Length > 0)
                    //        Emails.SendWebcomeEmail(userInfo.Email);
                    //    if (userInfo.Mobile.Length > 0)
                    //        SMSes.SendWebcomeSMS(userInfo.Mobile);
                    //}



                    partUserInfo = Users.GetUserById(userInfo.Uid);
                    if (partUserInfo == null)
                    {
                        return AjaxResult("exception", "登陆失败,请联系管理员");
                    }
                    errorList.Clear();
                    isDirSaleUser = true;
                    WorkContext.Uid = partUserInfo.Uid;
                    loginType = (int)UserPanertType.DirSaleUser;
                    //WorkContext.LoginUserType = loginType;
                    WorkContext.IsDirSaleUser = true;

                    WorkContext.UserName = partUserInfo.UserName;
                    WorkContext.UserEmail = partUserInfo.Email;
                    WorkContext.UserMobile = partUserInfo.Mobile;
                    WorkContext.NickName = partUserInfo.NickName;

                }
            }
            #endregion

            if (errorList.Length > 1)//验证失败时
            {
                return AjaxResult("error", errorList.Remove(errorList.Length - 1, 1).Append("]").ToString(), true);
            }
            else//验证成功时
            {
                //当用户等级是禁止访问等级时
                if (partUserInfo.UserRid == 1)
                    return AjaxResult("lockuser", "您的账号当前被锁定,不能访问");

                //删除登陆失败日志
                LoginFailLogs.DeleteLoginFailLogByIP(WorkContext.IP);
                //更新用户最后访问
                Users.UpdateUserLastVisit(partUserInfo.Uid, DateTime.Now, WorkContext.IP, WorkContext.RegionId);
                //更新购物车中用户id
                Carts.UpdateCartUidBySid(partUserInfo.Uid, WorkContext.Sid);
                //将用户信息写入cookie中
                MallUtils.SetUserCookie(partUserInfo, (WorkContext.MallConfig.IsRemember == 1 && isRemember == 1) ? 7 : -1);
                //如果用户是直销用户，将这个标示写入cookie中
                MallUtils.SetBMACookie("isdsu", isDirSaleUser ? "1" : "0");
                //密码加密写入cookies
                //MallUtils.SetBMACookie("random", GetPasswordCookie(password));
                //写入登陆类型
                WebHelper.SetCookie("logintype", loginType.ToString());

                if (string.IsNullOrEmpty(partUserInfo.DirSalePwd))
                {
                    partUserInfo.DirSalePwd = SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);
                    Users.UpdatePartUserDirSalePwd(partUserInfo);
                }

                //写入无页面操作后注销Mark
                //MallUtils.SetLoginTimeoutMark(WorkContext.Sid, TypeHelper.StringToInt(WebSiteConfig.NoActionLoginTimeOut));

                return AjaxResult("success", "登录成功");
            }
        }

        //F1DA442F-753F-413B-B5BD-3172AFB0734A Guid
        public static string Passwordkey = "123456F1DA442F753F413BB5BD3172AFB0734A";
        /// <summary>
        /// 设置cookie中的密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string GetPasswordCookie(string password)
        {
            string entrypassword = Passwordkey.Substring(0, 18) + password + Passwordkey.Substring(18, Passwordkey.Length - 18);
            return MallUtils.AESEncrypt(entrypassword);
        }

        #region 旧注册方法
        ///// <summary>
        ///// 注册
        ///// </summary>
        //public ActionResult Register()
        //{
        //    string returnUrl = WebHelper.GetQueryString("returnUrl");

        //    //保存推荐人在cookie中
        //    string pname = WebHelper.GetQueryString("pname");
        //    string CookiePuid = WebHelper.GetCookie("puid");
        //    if (TypeHelper.StringToInt(CookiePuid) > 0 && string.IsNullOrEmpty(pname) && string.IsNullOrEmpty(WebHelper.GetCookie("pname")))
        //    {
        //        PartUserInfo pUserInfo = Users.GetPartUserById(TypeHelper.StringToInt(CookiePuid));
        //        pname = string.IsNullOrEmpty(pUserInfo.UserName) ? (string.IsNullOrEmpty(pUserInfo.Email) ? (string.IsNullOrEmpty(pUserInfo.Mobile) ? "" : pUserInfo.Mobile) : pUserInfo.Email) : pUserInfo.UserName;
        //    }
        //    if (string.IsNullOrEmpty(pname) && !string.IsNullOrEmpty(WebHelper.GetCookie("pname")))
        //        pname = WebHelper.GetCookie("pname");


        //    if (returnUrl.Length == 0)
        //        returnUrl = Url.Action("index", "home");
        //    if (returnUrl.Contains("Mindex"))
        //        returnUrl = Url.Action("index", "home");

        //    if (WorkContext.MallConfig.RegType.Length == 0)
        //        return PromptView(returnUrl, "商城目前已经关闭注册功能!");
        //    if (WorkContext.Uid > 0)
        //        //return PromptView(returnUrl, "你已经是本商城的注册用户，无需再注册!");
        //        return RedirectToAction("index", "home");
        //    if (WorkContext.MallConfig.RegTimeSpan > 0)
        //    {
        //        DateTime registerTime = Users.GetRegisterTimeByRegisterIP(WorkContext.IP);
        //        if ((DateTime.Now - registerTime).Minutes <= WorkContext.MallConfig.RegTimeSpan)
        //            return PromptView(returnUrl, "你注册太频繁，请间隔一定时间后再注册!");
        //    }

        //    //get请求
        //    if (WebHelper.IsGet())
        //    {
        //        RegisterModel model = new RegisterModel();

        //        model.ReturnUrl = returnUrl;
        //        model.ShadowName = WorkContext.MallConfig.ShadowName;
        //        model.IsVerifyCode = CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages);

        //        string showname = GetUserNameByCode(pname);
        //        ViewData["showname"] = showname;

        //        WebHelper.SetCookie("pname", pname, 60 * 24 * 30);

        //        ViewData["pname"] = pname;


        //        return View(model);
        //    }

        //    //ajax请求
        //    string accountName = WebHelper.GetFormString(WorkContext.MallConfig.ShadowName).Trim().ToLower();
        //    string password = WebHelper.GetFormString("password");
        //    string confirmPwd = WebHelper.GetFormString("confirmPwd");
        //    string verifyCode = WebHelper.GetFormString("verifyCode");

        //    StringBuilder errorList = new StringBuilder("[");
        //    #region 验证

        //    #region 账号、密码、验证码输入合法性验证
        //    //账号验证
        //    Regex regNum = new Regex(@"[\u4e00-\u9fa5]+");
        //    if (regNum.IsMatch(accountName))
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不能包含中文字符", "}");
        //    }
        //    if (string.IsNullOrWhiteSpace(accountName))
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不能为空", "}");
        //    }
        //    else if (accountName.Length < 2 || accountName.Length > 50)
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名必须大于2且不大于50个字符", "}");
        //    }
        //    else if (accountName.Contains(" "))
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名中不允许包含空格", "}");
        //    }
        //    else if (accountName.Contains(":"))
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名中不允许包含冒号", "}");
        //    }
        //    else if (accountName.Contains("<"))
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名中不允许包含'<'符号", "}");
        //    }
        //    else if (accountName.Contains(">"))
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名中不允许包含'>'符号", "}");
        //    }
        //    else if ((!SecureHelper.IsSafeSqlString(accountName, false)))
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名已经存在", "}");
        //    }
        //    else if (CommonHelper.IsInArray(accountName, WorkContext.MallConfig.ReservedName, "\n"))
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名已经存在", "}");
        //    }
        //    else if (FilterWords.IsContainWords(accountName))
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名包含禁止单词", "}");
        //    }

        //    //密码验证
        //    if (string.IsNullOrWhiteSpace(password))
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码不能为空", "}");
        //    }
        //    else if (password.Length < 6 || password.Length > 32)
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码必须大于6且不大于32个字符", "}");
        //    }
        //    else if (password != confirmPwd)
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "两次输入的密码不一样", "}");
        //    }

        //    //验证码验证
        //    if (CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages))
        //    {
        //        if (string.IsNullOrWhiteSpace(verifyCode))
        //        {
        //            errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "verifyCode", "验证码不能为空", "}");
        //        }
        //        else if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
        //        {
        //            errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "verifyCode", "验证码不正确", "}");
        //        }
        //    }
        //    #endregion

        //    #region 其它验证
        //    int gender = WebHelper.GetFormInt("gender");
        //    if (gender < 0 || gender > 2)
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "gender", "请选择正确的性别", "}");

        //    string nickName = WebHelper.GetFormString("nickName");
        //    if (nickName.Length > 10)
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "nickName", "昵称的长度不能大于10", "}");
        //    }
        //    else if (FilterWords.IsContainWords(nickName))
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "nickName", "昵称中包含禁止单词", "}");
        //    }

        //    if (WebHelper.GetFormString("realName").Length > 5)
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "realName", "真实姓名的长度不能大于5", "}");
        //    }

        //    string bday = WebHelper.GetFormString("bday");
        //    if (bday.Length == 0)
        //    {
        //        string bdayY = WebHelper.GetFormString("bdayY");
        //        string bdayM = WebHelper.GetFormString("bdayM");
        //        string bdayD = WebHelper.GetFormString("bdayD");
        //        bday = string.Format("{0}-{1}-{2}", bdayY, bdayM, bdayD);
        //    }
        //    if (bday.Length > 0 && bday != "--" && !ValidateHelper.IsDate(bday))
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "bday", "请选择正确的日期", "}");

        //    string idCard = WebHelper.GetFormString("idCard");
        //    if (idCard.Length > 0 && !ValidateHelper.IsIdCard(idCard))
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "idCard", "请输入正确的身份证号", "}");
        //    }

        //    int regionId = WebHelper.GetFormInt("regionId");
        //    if (regionId > 0)
        //    {
        //        if (Regions.GetRegionById(regionId) == null)
        //            errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "regionId", "请选择正确的地址", "}");
        //        if (WebHelper.GetFormString("address").Length > 75)
        //        {
        //            errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "address", "详细地址的长度不能大于75", "}");
        //        }
        //    }

        //    if (WebHelper.GetFormString("bio").Length > 150)
        //    {
        //        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "bio", "简介的长度不能大于150", "}");
        //    }
        //    #endregion



        //    //当以上验证都通过时
        //    UserInfo userInfo = null;

        //    #region 用户名唯一性验证
        //    if (errorList.Length == 1)
        //    {
        //        if (WorkContext.MallConfig.RegType.Contains("2") && ValidateHelper.IsEmail(accountName))//验证邮箱
        //        {
        //            string emailProvider = CommonHelper.GetEmailProvider(accountName);
        //            if (WorkContext.MallConfig.AllowEmailProvider.Length != 0 && (!CommonHelper.IsInArray(emailProvider, WorkContext.MallConfig.AllowEmailProvider, "\n")))
        //            {
        //                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "不能使用'" + emailProvider + "'类型的邮箱", "}");
        //            }
        //            else if (CommonHelper.IsInArray(emailProvider, WorkContext.MallConfig.BanEmailProvider, "\n"))
        //            {
        //                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "不能使用'" + emailProvider + "'类型的邮箱", "}");
        //            }
        //            else if (Users.IsExistEmail(accountName) || IsUserExistForDirSale(accountName))
        //            {
        //                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "邮箱已经存在", "}");
        //            }
        //            else
        //            {
        //                userInfo = new UserInfo();
        //                userInfo.UserName = string.Empty;
        //                userInfo.Email = accountName;
        //                userInfo.Mobile = string.Empty;
        //            }
        //        }
        //        else if (WorkContext.MallConfig.RegType.Contains("3") && ValidateHelper.IsMobile(accountName))//验证手机
        //        {
        //            if (Users.IsExistMobile(accountName) || IsUserExistForDirSale(accountName))
        //            {
        //                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "手机号已经存在", "}");
        //            }
        //            else
        //            {
        //                userInfo = new UserInfo();
        //                userInfo.UserName = string.Empty;
        //                userInfo.Email = string.Empty;
        //                userInfo.Mobile = accountName;
        //            }
        //        }
        //        else if (WorkContext.MallConfig.RegType.Contains("1"))//验证用户名
        //        {
        //            if (Users.IsExistUserName(accountName) || IsUserExistForDirSale(accountName))
        //            {
        //                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "用户名已经存在", "}");
        //            }
        //            else
        //            {
        //                userInfo = new UserInfo();
        //                userInfo.UserName = accountName;
        //                userInfo.Email = string.Empty;
        //                userInfo.Mobile = string.Empty;
        //            }
        //        }
        //    }
        //    #endregion

        //    #endregion

        //    if (errorList.Length > 1)//验证失败
        //    {
        //        return AjaxResult("error", errorList.Remove(errorList.Length - 1, 1).Append("]").ToString(), true);
        //    }
        //    else//验证成功
        //    {
        //        #region 绑定用户信息

        //        userInfo.Salt = Randoms.CreateRandomValue(6);
        //        userInfo.Password = Users.CreateUserPassword(password, userInfo.Salt);
        //        userInfo.UserRid = UserRanks.GetLowestUserRank().UserRid;
        //        userInfo.StoreId = 0;
        //        userInfo.MallAGid = 1;//非管理员组
        //        if (nickName.Length > 0)
        //            userInfo.NickName = WebHelper.HtmlEncode(nickName);
        //        else
        //        //userInfo.NickName = "hhw" + Randoms.CreateRandomValue(7);
        //        {
        //            try
        //            {
        //                if (ValidateHelper.IsMobile(accountName))
        //                {
        //                    userInfo.NickName = accountName.Substring(0, 3) + "****" + accountName.Substring(7, 4);
        //                }
        //                else if (ValidateHelper.IsEmail(accountName))
        //                {
        //                    userInfo.NickName = accountName.Split('@')[0];
        //                }
        //                else
        //                {
        //                    userInfo.NickName = accountName;
        //                }
        //            }
        //            catch
        //            {
        //                userInfo.NickName = "hk_" + Randoms.CreateRandomValue(7);
        //            }
        //        }
        //        userInfo.Avatar = "";
        //        userInfo.PayCredits = 0;
        //        userInfo.RankCredits = 0;
        //        userInfo.VerifyEmail = 0;
        //        userInfo.VerifyMobile = 0;

        //        userInfo.DirSalePwd = SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);

        //        userInfo.LastVisitIP = WorkContext.IP;
        //        userInfo.LastVisitRgId = WorkContext.RegionId;
        //        userInfo.LastVisitTime = DateTime.Now;
        //        userInfo.RegisterIP = WorkContext.IP;
        //        userInfo.RegisterRgId = WorkContext.RegionId;
        //        userInfo.RegisterTime = DateTime.Now;

        //        userInfo.Gender = WebHelper.GetFormInt("gender");
        //        userInfo.RealName = WebHelper.HtmlEncode(WebHelper.GetFormString("realName"));
        //        userInfo.Bday = bday.Length > 0 ? TypeHelper.StringToDateTime(bday) : new DateTime(1900, 1, 1);
        //        userInfo.IdCard = WebHelper.GetFormString("idCard");
        //        userInfo.RegionId = WebHelper.GetFormInt("regionId");
        //        userInfo.Address = WebHelper.HtmlEncode(WebHelper.GetFormString("address"));
        //        userInfo.Bio = WebHelper.HtmlEncode(WebHelper.GetFormString("bio"));

        //        #endregion

        //        #region 处理用户注册推荐人关系
        //        //
        //        string parentname = WebHelper.GetQueryString("pname");
        //        int pType;
        //        int uIdByPname = GetUidByPname(parentname, out pType);
        //        if (string.IsNullOrEmpty(parentname) || uIdByPname < 1)//推荐人为空或推荐人在后台不合法，给定默认推荐人
        //        {
        //            //userInfo.Pid = GetUidForDefault();//推荐人不合法不能注册成功
        //            return AjaxResult("error", "[{\"key\":\"pnameError\",\"msg\":\"推荐人不合法，请输入合法的推荐人\"}]", true);

        //        }
        //        else //推荐人合法
        //        {
        //            userInfo.Pid = uIdByPname;
        //            userInfo.Ptype = pType;//推荐人类型
        //        }

        //        #endregion

        //        //创建用户
        //        userInfo.Uid = Users.CreateUser(userInfo);

        //        //添加用户失败
        //        if (userInfo.Uid < 1)
        //            return AjaxResult("exception", "创建用户失败,请联系管理员");

        //        //发放注册积分
        //        Credits.SendRegisterCredits(ref userInfo, DateTime.Now);
        //        //更新购物车中用户id
        //        Carts.UpdateCartUidBySid(userInfo.Uid, WorkContext.Sid);
        //        //将用户信息写入cookie
        //        MallUtils.SetUserCookie(userInfo, 0);

        //        //写入无页面操作后注销Mark
        //        MallUtils.SetLoginTimeoutMark(WorkContext.Sid, TypeHelper.StringToInt(WebSiteConfig.NoActionLoginTimeOut));

        //        //发送注册欢迎信息
        //        if (WorkContext.MallConfig.IsWebcomeMsg == 1)
        //        {
        //            if (userInfo.Email.Length > 0)
        //                Emails.SendWebcomeEmail(userInfo.Email);
        //            if (userInfo.Mobile.Length > 0)
        //                SMSes.SendWebcomeSMS(userInfo.Mobile);
        //        }

        //        //同步上下午
        //        WorkContext.Uid = userInfo.Uid;
        //        WorkContext.UserName = userInfo.UserName;
        //        WorkContext.UserEmail = userInfo.Email;
        //        WorkContext.UserMobile = userInfo.Mobile;
        //        WorkContext.NickName = userInfo.NickName;

        //        return AjaxResult("success", "注册成功");
        //    }
        //}
        #endregion

        #region 新注册方法 --仅支持手机号注册
        /// <summary>
        /// 手机号注册
        /// </summary>
        public ActionResult Register()
        {
            string returnUrl = WebHelper.GetQueryString("returnUrl");

            //保存推荐人在cookie中
            string pname = WebHelper.GetQueryString("pname");
            string CookiePuid = WebHelper.GetCookie("puid");
            if (TypeHelper.StringToInt(CookiePuid) > 0 && string.IsNullOrEmpty(pname) && string.IsNullOrEmpty(WebHelper.GetCookie("pname")))
            {
                PartUserInfo pUserInfo = Users.GetPartUserById(TypeHelper.StringToInt(CookiePuid));
                pname = string.IsNullOrEmpty(pUserInfo.UserName) ? (string.IsNullOrEmpty(pUserInfo.Email) ? (string.IsNullOrEmpty(pUserInfo.Mobile) ? "" : pUserInfo.Mobile) : pUserInfo.Email) : pUserInfo.UserName;
            }
            if (string.IsNullOrEmpty(pname) && !string.IsNullOrEmpty(WebHelper.GetCookie("pname")))
                pname = WebHelper.GetCookie("pname");

            if (returnUrl.Length == 0)
                returnUrl = Url.Action("index", "home");
            if (returnUrl.Contains("Mindex"))
                returnUrl = Url.Action("index", "home");

            if (WorkContext.MallConfig.RegType.Length == 0)
                return PromptView(returnUrl, "商城目前已经关闭注册功能!");
            if (WorkContext.Uid > 0)
                //return PromptView(returnUrl, "你已经是本商城的注册用户，无需再注册!");
                return RedirectToAction("index", "home");
            if (WorkContext.MallConfig.RegTimeSpan > 0)
            {
                DateTime registerTime = Users.GetRegisterTimeByRegisterIP(WorkContext.IP);
                if ((DateTime.Now - registerTime).Minutes <= WorkContext.MallConfig.RegTimeSpan)
                    return PromptView(returnUrl, "您注册太频繁，请间隔一定时间后再注册!");
            }

            //get请求
            if (WebHelper.IsGet())
            {
                RegisterModel model = new RegisterModel();
                model.ReturnUrl = returnUrl;
                model.ShadowName = WorkContext.MallConfig.ShadowName;
                model.IsVerifyCode = CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages);
                //string VCode = Users.VerifyCodeForRegister();
                //string VCodeCall = Users.VerifyCodeForRegisterCall();
                //ViewData["VerifyCodeForRegister"] = VCode;
                //ViewData["VerifyCodeForRegisterCall"] = VCodeCall;
                string showname = GetUserNameByCode(pname);
                ViewData["showname"] = showname;
                WebHelper.SetCookie("pname", pname, 60 * 24 * 30);
                ViewData["pname"] = pname;
                return View(model);
            }

            //ajax请求
            string accountName = WebHelper.GetFormString(WorkContext.MallConfig.ShadowName).Trim().ToLower();
            //string password = WebHelper.GetFormString("password");
            //string confirmPwd = WebHelper.GetFormString("confirmPwd");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            StringBuilder errorList = new StringBuilder("[");
            #region 验证
            //账号验证
            if (Sessions.GetValueString(WorkContext.Sid, "resigterMobile") != accountName)
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "接收手机不一致", "}");
            if (string.IsNullOrWhiteSpace(accountName))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "手机号码不能为空", "}");
            if (!ValidateHelper.IsMobile(accountName))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "手机号格式不正确", "}");
            //手机验证码验证
            if (string.IsNullOrWhiteSpace(verifyCode))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "verifyCode", "验证码不能为空", "}");
            else if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "mobileVerifyCode"))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "verifyCode", "验证码不正确", "}");

            //当以上验证都通过时
            UserInfo userInfo = null;

            //用户名唯一性验证
            if (errorList.Length == 1)
            {
                if (Users.IsExistMobile(accountName) || AccountUtils.IsUserExistForDirSale(accountName))
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "手机号已经存在", "}");
                else
                    userInfo = new UserInfo() { UserName = string.Empty, Email = string.Empty, Mobile = accountName };
            }
            #endregion

            if (errorList.Length > 1)//验证失败
            {
                return AjaxResult("error", errorList.Remove(errorList.Length - 1, 1).Append("]").ToString(), true);
            }
            else//验证成功
            {
                #region 初始化用户信息
                string nickName = string.Empty;
                //生成随机初始密码并发送短信
                string password = Randoms.CreateRandomValue(6);
                userInfo.Salt = Randoms.CreateRandomValue(6);
                userInfo.Password = Users.CreateUserPassword(password, userInfo.Salt);
                userInfo.UserRid = UserRanks.GetLowestUserRank().UserRid;
                userInfo.StoreId = 0;
                userInfo.MallAGid = 1;//非管理员组
                if (nickName.Length > 0)
                    userInfo.NickName = WebHelper.HtmlEncode(nickName);
                else
                    userInfo.NickName = accountName.Substring(0, 3) + "***" + accountName.Substring(7, 4);
                userInfo.Avatar = string.Empty;
                userInfo.PayCredits = 0;
                userInfo.RankCredits = 0;
                userInfo.VerifyEmail = 0;
                userInfo.VerifyMobile = 1;
                userInfo.DirSalePwd = SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);
                userInfo.LastVisitIP = WorkContext.IP;
                userInfo.LastVisitRgId = WorkContext.RegionId;
                userInfo.LastVisitTime = DateTime.Now;
                userInfo.RegisterIP = WorkContext.IP;
                userInfo.RegisterRgId = WorkContext.RegionId;
                userInfo.RegisterTime = DateTime.Now;
                userInfo.Gender = 0;
                userInfo.RealName = string.Empty;
                userInfo.Bday = new DateTime(1900, 1, 1);
                userInfo.IdCard = string.Empty;
                userInfo.RegionId = -1;
                userInfo.Address = string.Empty;
                userInfo.Bio = string.Empty;
                #endregion

                #region 处理用户注册推荐人关系
                string parentname = WebHelper.GetQueryString("pname");
                if (string.IsNullOrEmpty(parentname.Trim()))
                    parentname = WebHelper.GetFormString("pname");
                if (string.IsNullOrEmpty(parentname.Trim()))
                    return AjaxResult("error", "[{\"key\":\"pnameError\",\"msg\":\"推荐人不正确，请输入正确的推荐人!\"}]", true);
                int pType;
                int uIdByPname = AccountUtils.GetUidByPname(parentname.Trim(), out pType);
                if (string.IsNullOrEmpty(parentname.Trim()) || uIdByPname < 1)//推荐人为空或推荐人在后台不合法
                    return AjaxResult("error", "[{\"key\":\"pnameError\",\"msg\":\"推荐人不正确，请输入正确的推荐人\"}]", true);
                else //推荐人合法
                {
                    userInfo.Pid = uIdByPname;
                    userInfo.Ptype = pType;//推荐人类型
                }
                #endregion
                //创建用户
                userInfo.Uid = Users.CreateUser(userInfo);

                //添加用户失败
                if (userInfo.Uid < 1)
                    return AjaxResult("exception", "创建用户失败,请联系管理员");

                //推荐送10元红包 11.12之前有效
                if (DateTime.Now < new DateTime(2016, 11, 13))
                {
                    //更新汇购会员账户
                    if (userInfo.Ptype == 1)
                    {
                        Account.UpdateAccountForIn(new AccountInfo()
                        {
                            AccountId = (int)AccountType.红包账户,
                            UserId = userInfo.Pid,
                            TotalIn = 10
                        });
                        Account.CreateAccountDetail(new AccountDetailInfo()
                        {
                            AccountId = (int)AccountType.红包账户,
                            UserId = userInfo.Pid,
                            CreateTime = DateTime.Now,
                            DetailType = (int)DetailType.活动赠送,
                            InAmount = 10,
                            OrderCode = "",
                            AdminUid = 1,//system
                            Status = 1,
                            DetailDes = "活动赠送红包：赠送金额:10"
                        });
                    }
                    //更新直销的账户
                    else if (userInfo.Ptype == 2)
                    {
                        AccountUtils.UpdateAccountForDir(userInfo.Pid, (int)AccountType.红包账户, 10, 0, "", "活动赠送红包：赠送金额:10");
                    }
                }

                //发放注册积分
                //Credits.SendRegisterCredits(ref userInfo, DateTime.Now);
                //更新购物车中用户id
                Carts.UpdateCartUidBySid(userInfo.Uid, WorkContext.Sid);
                //将用户信息写入cookie
                MallUtils.SetUserCookie(userInfo, 0);

                //写入无页面操作后注销Mark
                //MallUtils.SetLoginTimeoutMark(WorkContext.Sid, TypeHelper.StringToInt(WebSiteConfig.NoActionLoginTimeOut));

                //发送注册成功和初始密码短信
                if (userInfo.Mobile.Length > 0)
                    SMSes.SendRegisterSuccessSMS(userInfo.Mobile, password);
                //发送会员消息
                new Inform().Add(new InformInfo() {
                    typeid = (int)InformTypeEnum.会员消息,
                    uid=userInfo.Uid,
                    state=0,
                    addtime=DateTime.Now,
                    title = "注册成功！",
                    content=string.Format("欢迎您成为{0}会员，请尽快登录个人中心修改密码。",WorkContext.MallConfig.SiteTitle)
                });
                //同步上下午
                WorkContext.Uid = userInfo.Uid;
                WorkContext.UserName = userInfo.UserName;
                WorkContext.UserEmail = userInfo.Email;
                WorkContext.UserMobile = userInfo.Mobile;
                WorkContext.NickName = userInfo.NickName;
                return AjaxResult("success", "注册成功");
            }
        }


        public ActionResult test(string mobile)
        {
            if (!LimitHelper.Execution(WorkContext.IP, (int)LimitEnum.Register))
            {
                return AjaxResult("error", "IP频繁", false);
            }
            if (!LimitHelper.Execution(mobile, (int)LimitEnum.Order))
            {
                return AjaxResult("error", "该手机号频繁", false);
            }
            if (Users.IsExistMobile(mobile) || AccountUtils.IsUserExistForDirSale(mobile))
            {
                return AjaxResult("error", "该手机号已经被注册", false);
            }
            //获得用户唯一标示符sid
            string sid = MallUtils.GetSidCookie();
            //当sid为空时
            if (sid == null)
            {
                //生成sid
                sid = Sessions.GenerateSid();
                //将sid保存到cookie中
                MallUtils.SetSidCookie(sid);
            }
            // HttpRuntime.Cache.Insert(sid+"|"+mobile, result, null, DateTime.Now.AddMinutes(30), System.Web.Caching.Cache.NoSlidingExpiration);
            int sendCount = Sessions.GetValueInt(sid, "testmobileVerifyCodeCount");
            if (sendCount > 3)
                return AjaxResult("error", "发送失败，请稍后再试！！！", false);
            //生成6位随机数字
            string verifyValue = Randoms.CreateRandomValue(6, true).ToLower();
            //if (SMSes.SendRegisterSMS(mobile, verifyValue)) {

            bool flag = true;
            if (flag)
            {
                //将验证值保存到session中
                Sessions.SetItem(sid, "testresigterMobile", mobile);
                Sessions.SetItem(sid, "testmobileVerifyCode", verifyValue);
                Sessions.SetItem(sid, "testmobileVerifyCodeCount", sendCount + 1);
                return AjaxResult("success", "", false);
            }
            return AjaxResult("error", "系统繁忙，请稍后再试！！！", false);


            //string IP = IPAddress; //System.Web.HttpContext.Current.Request.UserHostAddress;//WebHelper.GetIP2();
            //string IP2 = System.Web.HttpContext.Current.Request.UserHostAddress;//WebHelper.GetIP2();
            //string IP3 = WebHelper.GetIP2();
            //string IP4 = WebHelper.GetIP();
            //string IP5 = WebHelper.GetUserIp();
            //string IP6 = WorkContext.IP;
            //StringBuilder sb = new StringBuilder("11");
            ////foreach (string o in Request.ServerVariables)
            ////{
            ////    sb.Append(o + "=" + Request.ServerVariables[o] + "\r\n");
            ////}
            //LogHelper.WriteOperateLog("IPtest", "IP", "请求IP:" + IP + "==ip2===" + IP2 + "==ip3==" + IP3 + "==ip4==" + IP4 + "==ip5==" + IP5 + "===ip6==" + IP6);
            //return Content(IP);
        }

        /// <summary>
        /// 取得客户端真实IP。如果有代理则取第一个非内网地址
        /// </summary>
        public static string IPAddress
        {
            get
            {
                string result = String.Empty;
                result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (result != null && result != String.Empty)
                {
                    //可能有代理
                    if (result.IndexOf(".") == -1)    //没有“.”肯定是非IPv4格式
                        result = null;
                    else
                    {
                        if (result.IndexOf(",") != -1)
                        {
                            //有“,”，估计多个代理。取第一个不是内网的IP。
                            result = result.Replace(" ", "").Replace("'", "");
                            string[] temparyip = result.Split(",;".ToCharArray());
                            for (int i = 0; i < temparyip.Length; i++)
                            {
                                if (ValidateHelper.IsIP(temparyip[i])
                                    && temparyip[i].Substring(0, 3) != "10."
                                    && temparyip[i].Substring(0, 7) != "192.168"
                                    && temparyip[i].Substring(0, 7) != "172.16.")
                                {
                                    return temparyip[i];    //找到不是内网的地址
                                }
                            }
                        }
                        else if (ValidateHelper.IsIP(result)) //代理即是IP格式 ,IsIPAddress判断是否是IP的方法,
                            return result;
                        else
                            result = null;    //代理中的内容 非IP，取IP
                    }

                }

                string IpAddress = (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null && System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != String.Empty) ? System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                if (null == result || result == String.Empty)
                    result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                if (result == null || result == String.Empty)
                    result = System.Web.HttpContext.Current.Request.UserHostAddress;

                return result;
            }
        }

        #endregion



        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="Vcode"></param>
        /// <returns></returns>
        public ActionResult SendMoblieVerify(string mobile, string Vcode)
        {
            //验证来源
            string referrer = HttpContext.Request.UrlReferrer.ToString();
            string targetUrl = WebHelper.IsMobile() ? string.Format("http://{0}/mob/account/register", WorkContext.MallConfig.SiteUrl) : string.Format("http://{0}/account/register", WorkContext.MallConfig.SiteUrl);
            if (!referrer.Contains(targetUrl))
                return AjaxResult("error", "系统繁忙");
            //验证验证码

            if (string.IsNullOrWhiteSpace(Vcode))
            {
                return AjaxResult("error", "系统繁忙");
            }
            else if (Vcode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult("error", "验证码错误");
            }
            if (!LimitHelper.Execution(WorkContext.IP, (int)LimitEnum.RegisterForIP, 60 * 24, 10, true))
            {
                LogHelper.WriteOperateLog("IPBlackList", "IP", "请求IP:" + WorkContext.IP);
                return AjaxResult("error", "系统繁忙", false);
            }
            if (!LimitHelper.Execution(mobile, (int)LimitEnum.RegisterForMobile, 60 * 24, 10, true))
            {
                LogHelper.WriteOperateLog("MobileBlackList", "Mobile", "发送Mobile:" + mobile);
                return AjaxResult("error", "系统繁忙", false);
            }

            if (Users.IsExistMobile(mobile) || AccountUtils.IsUserExistForDirSale(mobile))
            {
                return AjaxResult("error", "该手机号已经被注册", false);
            }
            //获得用户唯一标示符sid
            string sid = MallUtils.GetSidCookie();
            //当sid为空时
            if (sid == null)
            {
                //生成sid
                sid = Sessions.GenerateSid();
                //将sid保存到cookie中
                MallUtils.SetSidCookie(sid);
            }
            // HttpRuntime.Cache.Insert(sid+"|"+mobile, result, null, DateTime.Now.AddMinutes(30), System.Web.Caching.Cache.NoSlidingExpiration);
            int sendCount = Sessions.GetValueInt(sid, "mobileVerifyCodeCount");
            if (sendCount > 3)
                return AjaxResult("error", "系统繁忙", false);
            //生成6位随机数字
            string verifyValue = Randoms.CreateRandomValue(6, true).ToLower();
            //if (SMSes.SendRegisterSMS(mobile, verifyValue)) {
            if (SMSes.SendMobileMessage(verifyValue, mobile))
            {
                //将验证值保存到session中
                Sessions.SetItem(sid, "resigterMobile", mobile);
                Sessions.SetItem(sid, "mobileVerifyCode", verifyValue);
                Sessions.SetItem(sid, "mobileVerifyCodeCount", sendCount + 1);
                return AjaxResult("success", "", false);
            }
            return AjaxResult("error", "系统繁忙，请稍后再试！！！", false);
            //return AjaxResult("success", "", false);
        }
        /// <summary>
        /// 异步获取用户姓名
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string ajaxGetNameByCode()
        {
            string pName = WebHelper.GetFormString("pName");
            return GetUserNameByCode(pName);
        }

        /// <summary>
        /// 获取用户姓名
        /// </summary>
        /// <returns></returns>
        public string GetUserNameByCode(string pName)
        {
            bool flag = true;
            string showname = "";
            UserInfo userinfo;
            if (ValidateHelper.IsEmail(pName))
            {
                int uid = Users.GetUidByEmail(pName);
                userinfo = Users.GetUserById(uid);
                if (userinfo != null)
                {

                    showname = userinfo.RealName;
                    flag = false;

                }
            }
            else if (ValidateHelper.IsMobile(pName))
            {
                int uid = Users.GetUidByMobile(pName);
                userinfo = Users.GetUserById(uid);
                if (userinfo != null)
                {

                    showname = userinfo.RealName;
                    flag = false;

                }
            }
            else
            {
                int uid = Users.GetUidByUserName(pName);
                userinfo = Users.GetUserById(uid);
                if (userinfo != null)
                {

                    showname = userinfo.RealName;
                    flag = false;

                }
            }
            //海汇会员中不存在
            if (flag)
            {
                showname = AccountUtils.GetUserNameByCodeForDir(pName);
            }
            return showname;
        }


        #region 注册确定推荐人关系
        ///// <summary>
        ///// 根据推荐人名称获取会员id
        ///// </summary>
        ///// <param name="pName"></param>
        ///// <returns></returns>
        //public int GetUidByPname(string pName, out int pType)
        //{
        //    int uid = 0;
        //    pType = 1;

        //    PartUserInfo userinfo;
        //    if (ValidateHelper.IsEmail(pName))
        //    {
        //        userinfo = Users.GetPartUserByEmail(pName);
        //        if (userinfo != null)
        //        {
        //            if (userinfo.IsDirSaleUser)
        //            {
        //                uid = userinfo.DirSaleUid;
        //                pType = (int)UserPanertType.DirSaleUser;
        //            }
        //            else
        //            {
        //                uid = userinfo.Uid;
        //                pType = (int)UserPanertType.HaiHuiUser;
        //            }
        //        }
        //    }
        //    else if (ValidateHelper.IsMobile(pName))
        //    {

        //        userinfo = Users.GetPartUserByMobile(pName);
        //        if (userinfo != null)
        //        {
        //            if (userinfo.IsDirSaleUser)
        //            {
        //                uid = userinfo.DirSaleUid;
        //                pType = (int)UserPanertType.DirSaleUser;
        //            }
        //            else
        //            {
        //                uid = userinfo.Uid;
        //                pType = (int)UserPanertType.HaiHuiUser;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        userinfo = Users.GetPartUserByName(pName);
        //        if (userinfo != null)
        //        {
        //            if (userinfo.IsDirSaleUser)
        //            {
        //                uid = userinfo.DirSaleUid;
        //                pType = (int)UserPanertType.DirSaleUser;
        //            }
        //            else
        //            {
        //                uid = userinfo.Uid;
        //                pType = (int)UserPanertType.HaiHuiUser;
        //            }
        //        }
        //    }
        //    //海汇会员中不存在
        //    if (uid < 1)
        //    {
        //        string url = dirSaleApiUrl + "/api/User/GetUserIdByName?userName=" + pName;
        //        string FromDirSale = WebHelper.DoGet(url);
        //        JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
        //        JToken token = (JToken)jsonObject;
        //        if (token["Result"].ToString() == "0")
        //        {
        //            uid = TypeHelper.ObjectToInt(token["Info"]);
        //            pType = (int)UserPanertType.DirSaleUser;
        //        }
        //    }
        //    return uid;
        //}

        /// <summary>
        /// 为新注册用户默认分配推荐人
        /// </summary>
        /// <returns></returns>
        public int GetUidForDefault()
        {
            lock (_locker)
            {
                return GetMaxUid();
            }
            //return 0;
        }

        /// <summary>
        /// 获得最大用户Id
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public int GetMaxUid()
        {

            //SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@uid", uid) };
            string SqlStr = "SELECT MAX(uid) FROM dbo.hlh_users";
            //commandText = string.Format("SELECT TOP {0} {3} FROM [{1}onlineusers] WHERE [olid] NOT IN (SELECT TOP {2} [olid] FROM [{1}onlineusers] ORDER BY {4}) ORDER BY {4}",
            //                                    pageSize,
            //                                    RDBSHelper.RDBSTablePre,
            //                                    (pageNumber - 1) * pageSize,
            //                                    RDBSFields.ONLINE_USERS,
            //                                    sort);
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, SqlStr));
        }
        #endregion

        /// <summary>
        /// 退出
        /// </summary>
        public ActionResult Logout()
        {
            if (WorkContext.Uid > 0)
            {
                WebHelper.DeleteCookie("UserCookie");
                WebHelper.DeleteCookie("logintype");
                WebHelper.DeleteCookie(WorkContext.Sid + "LoginTimeOut");
                Sessions.RemoverSession(WorkContext.Sid);
                OnlineUsers.DeleteOnlineUserBySid(WorkContext.Sid);
            }
            //return Redirect(Url.Action("searchindex", "catalog"));
            return Redirect(Url.Action("index", "home"));
        }
        /// <summary>
        /// 清除登陆
        /// </summary>
        public ActionResult ClearUserCookie()
        {
            if (WorkContext.Uid > 0)
            {
                WebHelper.DeleteCookie("puid");
                WebHelper.DeleteCookie("UserCookie");
                WebHelper.DeleteCookie("logintype");
                WebHelper.DeleteCookie(WorkContext.Sid + "LoginTimeOut");
                Sessions.RemoverSession(WorkContext.Sid);
                OnlineUsers.DeleteOnlineUserBySid(WorkContext.Sid);
            }
            //return Redirect(Url.Action("searchindex", "catalog"));
            return Content("清除登陆成功");
        }
        /// <summary>
        /// 找回密码
        /// </summary>
        public ActionResult FindPwd()
        {
            //get请求
            if (WebHelper.IsGet())
            {
                FindPwdModel model = new FindPwdModel();

                model.ShadowName = WorkContext.MallConfig.ShadowName;
                model.IsVerifyCode = CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages);

                return View(model);
            }

            //ajax请求
            string accountName = WebHelper.GetFormString(WorkContext.MallConfig.ShadowName);
            string verifyCode = WebHelper.GetFormString("verifyCode");

            StringBuilder errorList = new StringBuilder("[");
            //账号验证
            Regex regNum = new Regex(@"[\u4e00-\u9fa5]+");
            if (string.IsNullOrWhiteSpace(accountName))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不能为空", "}");
            }
            else if (regNum.IsMatch(accountName))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不能包含中文字符", "}");
            }
            else if (accountName.Length < 2 || accountName.Length > 50)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名必须大于2且不大于50个字符", "}");
            }
            else if ((!SecureHelper.IsSafeSqlString(accountName, false)))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不存在", "}");
            }

            //验证码验证
            if (CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages))
            {
                if (string.IsNullOrWhiteSpace(verifyCode))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "verifyCode", "验证码不能为空", "}");
                }
                else if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "verifyCode", "验证码不正确", "}");
                }
            }

            //当以上验证都通过时
            PartUserInfo partUserInfo = null;
            if (ModelState.IsValid)
            {
                if (ValidateHelper.IsEmail(accountName))//验证邮箱
                {
                    partUserInfo = Users.GetPartUserByEmail(accountName);
                    if (partUserInfo == null && !AccountUtils.IsUserExistForDirSale(accountName))
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "邮箱不存在", "}");
                    else if (partUserInfo.IsDirSaleUser == true)
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "该帐号不支持找回密码，请联系客服修改", "}");
                }
                else if (ValidateHelper.IsMobile(accountName))//验证手机
                {
                    partUserInfo = Users.GetPartUserByMobile(accountName);
                    if (partUserInfo == null && !AccountUtils.IsUserExistForDirSale(accountName))
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "手机号不存在", "}");
                    else if (partUserInfo.IsDirSaleUser == true)
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "该帐号不支持找回密码，请联系客服修改", "}");
                }
                else//验证用户名
                {
                    partUserInfo = Users.GetPartUserByName(accountName);
                    if (partUserInfo == null && !AccountUtils.IsUserExistForDirSale(accountName))
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "用户名不存在", "}");
                    else if (partUserInfo.IsDirSaleUser == true)
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "该帐号不支持找回密码，请联系客服修改", "}");
                }
            }

            if (errorList.Length == 1)
            {
                if (partUserInfo.Email.Length == 0 && partUserInfo.Mobile.Length == 0)
                    return AjaxResult("nocanfind", "由于您没有设置邮箱和手机，所以不能找回此账号的密码");

                return AjaxResult("success", Url.Action("selectfindpwdtype", new RouteValueDictionary { { "uid", partUserInfo.Uid } }));
            }
            else
            {
                return AjaxResult("error", errorList.Remove(errorList.Length - 1, 1).Append("]").ToString(), true);
            }
        }

        /// <summary>
        /// 选择找回密码方式
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectFindPwdType()
        {
            int uid = WebHelper.GetQueryInt("uid");
            PartUserInfo partUserInfo = Users.GetPartUserById(uid);
            if (partUserInfo == null)
                return PromptView("用户不存在");

            SelectFindPwdTypeModel model = new SelectFindPwdTypeModel();
            model.PartUserInfo = partUserInfo;
            return View(model);
        }

        /// <summary>
        /// 发送找回密码邮件
        /// </summary>
        public ActionResult SendFindPwdEmail()
        {
            int uid = WebHelper.GetQueryInt("uid");

            PartUserInfo partUserInfo = Users.GetPartUserById(uid);
            if (partUserInfo == null)
                return AjaxResult("nouser", "用户不存在");
            if (partUserInfo.Email.Length == 0)
                return AjaxResult("nocanfind", "由于您没有设置邮箱，所以不能通过邮箱找回此账号的密码");

            //发送找回密码邮件
            string v = MallUtils.AESEncrypt(string.Format("{0},{1},{2}", partUserInfo.Uid, DateTime.Now, Randoms.CreateRandomValue(6)));
            string url = string.Format("http://{0}{1}", Request.Url.Authority, Url.Action("resetpwd", new RouteValueDictionary { { "v", v } }));
            Emails.SendFindPwdEmail(partUserInfo.Email, partUserInfo.UserName, url);
            return AjaxResult("success", "邮件已发送,请查收");
        }

        /// <summary>
        /// 发送找回密码短信
        /// </summary>
        public ActionResult SendFindPwdMobile()
        {
            int uid = WebHelper.GetQueryInt("uid");

            PartUserInfo partUserInfo = Users.GetPartUserById(uid);
            if (partUserInfo == null)
                return AjaxResult("nouser", "用户不存在");
            if (partUserInfo.Mobile.Length == 0)
                return AjaxResult("nocanfind", "由于您没有设置手机，所以不能通过手机找回此账号的密码");

            //发送找回密码短信
            string moibleCode = Randoms.CreateRandomValue(6);
            Sessions.SetItem(WorkContext.Sid, "findPwdMoibleCode", moibleCode);
            SMSes.SendFindPwdMobile(partUserInfo.Mobile, moibleCode);
            return AjaxResult("success", "验证码已发送,请查收");
        }
        /// <summary>
        /// 发送找回密码语音验证码
        /// </summary>
        public ActionResult SendFindPwdMobileVoice()
        {
            int uid = WebHelper.GetQueryInt("uid");

            PartUserInfo partUserInfo = Users.GetPartUserById(uid);
            if (partUserInfo == null)
                return AjaxResult("nouser", "用户不存在");
            if (partUserInfo.Mobile.Length == 0)
                return AjaxResult("nocanfind", "由于您没有设置手机，所以不能通过手机找回此账号的密码");

            //发送找回密码短信
            string moibleCode = Randoms.CreateRandomValue(4);
            if (SMSes.SendFindPwdMobileVoice(partUserInfo.Mobile, moibleCode))
            {
                Sessions.SetItem(WorkContext.Sid, "findPwdMoibleCode", moibleCode);
                return AjaxResult("success", "语音验证码已发送");
            }
            return AjaxResult("error", "系统繁忙，请稍后再试！！！");
        }
        /// <summary>
        /// 验证找回密码手机
        /// </summary>
        public ActionResult VerifyFindPwdMobile()
        {
            int uid = WebHelper.GetQueryInt("uid");
            string mobileCode = WebHelper.GetFormString("mobileCode");

            PartUserInfo partUserInfo = Users.GetPartUserById(uid);
            if (partUserInfo == null)
                return AjaxResult("nouser", "用户不存在");
            if (partUserInfo.Mobile.Length == 0)
                return AjaxResult("nocanfind", "由于您没有设置手机，所以不能通过手机找回此账号的密码");

            //检查手机码
            if (string.IsNullOrWhiteSpace(mobileCode))
            {
                return AjaxResult("emptymobilecode", "手机验证码不能为空");
            }
            else if (Sessions.GetValueString(WorkContext.Sid, "findPwdMoibleCode") != mobileCode)
            {
                return AjaxResult("wrongmobilecode", "手机验证码不正确");
            }

            string v = MallUtils.AESEncrypt(string.Format("{0},{1},{2}", partUserInfo.Uid, DateTime.Now, Randoms.CreateRandomValue(6)));
            string url = string.Format("http://{0}{1}", Request.Url.Authority, Url.Action("resetpwd", new RouteValueDictionary { { "v", v } }));
            return AjaxResult("success", url);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        public ActionResult ResetPwd()
        {
            string v = WebHelper.GetQueryString("v").Replace(" ", "+");
            //v = WebHelper.UrlDecode(v);
            //解密字符串
            string realV = MallUtils.AESDecrypt(v);

            //数组第一项为uid，第二项为验证时间,第三项为随机值
            string[] result = StringHelper.SplitString(realV);
            if (result.Length != 3)
                return HttpNotFound();

            int uid = TypeHelper.StringToInt(result[0]);
            DateTime time = TypeHelper.StringToDateTime(result[1]);

            PartUserInfo partUserInfo = Users.GetPartUserById(uid);
            if (partUserInfo == null)
                return PromptView("用户不存在");
            //判断验证时间是否过时
            if (DateTime.Now.AddMinutes(-30) > time)
                return PromptView("此链接已经失效，请重新验证");

            //get请求
            if (WebHelper.IsGet())
            {
                ResetPwdModel model = new ResetPwdModel();
                model.V = v;
                return View(model);
            }

            //ajax请求
            string password = WebHelper.GetFormString("password");
            string confirmPwd = WebHelper.GetFormString("confirmPwd");

            StringBuilder errorList = new StringBuilder("[");
            //验证
            if (string.IsNullOrWhiteSpace(password))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码不能为空", "}");
            }
            else if (password.Length < 4 || password.Length > 32)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码必须大于3且不大于32个字符", "}");
            }
            else if (password != confirmPwd)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "confirmPwd", "两次输入的密码不一样", "}");
            }

            if (errorList.Length == 1)
            {
                //生成用户新密码
                string p = Users.CreateUserPassword(password, partUserInfo.Salt);
                //设置用户新密码
                Users.UpdatePartUserPwd(partUserInfo, password);
                //清空当前用户信息
                WebHelper.DeleteCookie("UserCookie");
                Sessions.RemoverSession(WorkContext.Sid);
                OnlineUsers.DeleteOnlineUserBySid(WorkContext.Sid);

                return AjaxResult("success", Url.Action("login"));
            }
            else
            {
                return AjaxResult("error", errorList.Remove(errorList.Length - 1, 1).Append("]").ToString(), true);
            }
        }
    }
}
