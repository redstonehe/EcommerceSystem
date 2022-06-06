using System;
using System.Text;
using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using VMall.Core;

using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Models;


namespace VMall.Web.Controllers
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.IO;
    using System.Web.Script.Serialization;
    using System.Drawing.Drawing2D;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 用户中心控制器类
    /// </summary>
    public partial class UCenterController : BaseWebController
    {
        private static object ctx = new object();//锁对象

        private static string dirSaleApiUrl = WebSiteConfig.DirsaleApiUrl;//api配置地址

        HaiMiDrawCash haiMiDrawCashBLL = new HaiMiDrawCash();
        private OrderApply OrderBLL = new OrderApply();

        #region 用户信息

        /// <summary>
        /// 用户信息
        /// </summary>
        public ActionResult UserInfo()
        {
            UserInfoModel model = new UserInfoModel();

            model.UserInfo = Users.GetUserById(WorkContext.Uid);
            model.UserDetailInfo = Users.GetUserDetailById(WorkContext.Uid);
            model.UserRankInfo = WorkContext.UserRankInfo;

            RegionInfo regionInfo = null;
            if (model.UserInfo != null)
            {
                regionInfo = Regions.GetRegionById(model.UserInfo.RegionId);
            }
            else
            {
                model.UserInfo = new UserInfo();
                model.UserInfo.UserName = WorkContext.NickName;
            }

            if (regionInfo != null)
            {
                ViewData["provinceId"] = regionInfo.ProvinceId;
                ViewData["cityId"] = regionInfo.CityId;
                ViewData["countyId"] = regionInfo.RegionId;
            }
            else
            {
                ViewData["provinceId"] = -1;
                ViewData["cityId"] = -1;
                ViewData["countyId"] = -1;
            }

            //二维码
            string parentName = string.IsNullOrEmpty(WorkContext.UserName) ? (string.IsNullOrEmpty(WorkContext.UserEmail) ? (string.IsNullOrEmpty(WorkContext.UserMobile) ? "" : WorkContext.UserMobile) : WorkContext.UserEmail) : WorkContext.UserName;
            bool isMobile = true;// WebHelper.IsMobile();
            string shareUrl = "http://" + Request.Url.Host + ":" + Request.Url.Port + (isMobile ? "/mob" : "") + "/account/register?pname=" + parentName.Trim() + "&returnUrl=http://" + Request.Url.Host + ":" + Request.Url.Port + (isMobile ? "/mob" : "");
            //string codeImgPath = CreateCode_Simple(shareUrl, WorkContext.PartUserInfo.Uid, WorkContext.PartUserInfo.Salt, isMobile);
            //ViewData["codeImg"] = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/upload/usersharecode/" + (isMobile ? "/m/" : "/pc/") + codeImgPath;
            string bgQRcode = IOHelper.CreatQRCodeWithBG2(shareUrl, WorkContext.Uid, WorkContext.PartUserInfo);

            ViewData["codeImg"] = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/upload/bgqrcode/" + bgQRcode;

            return View(model);
        }

        /// <summary>
        /// 编辑用户信息
        /// </summary>
        public ActionResult EditUser()
        {
            string userName = WebHelper.GetFormString("userName");
            string nickName = WebHelper.GetFormString("nickName");
            string avatar = WebHelper.GetFormString("avatar");
            string realName = WebHelper.GetFormString("realName");
            int gender = WebHelper.GetFormInt("gender");
            string idCard = WebHelper.GetFormString("idCard");
            string bday = WebHelper.GetFormString("bday");
            int regionId = WebHelper.GetFormInt("regionId");
            string address = WebHelper.GetFormString("address");
            string bio = WebHelper.GetFormString("bio");
            string bankname = WebHelper.GetFormString("BankName");
            string bankcardcode = WebHelper.GetFormString("BankCardCode");
            string bankusername = WebHelper.GetFormString("BankUserName");
            StringBuilder errorList = new StringBuilder("[");
            //验证用户名
            if (WorkContext.UserName.Length == 0 && userName.Length > 0)
            {
                if (userName.Length < 4 || userName.Length > 10)
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名必须大于3且不大于10个字符", "}");
                }
                else if (userName.Contains(" "))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名中不允许包含空格", "}");
                }
                else if (userName.Contains(":"))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名中不允许包含冒号", "}");
                }
                else if (userName.Contains("<"))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名中不允许包含'<'符号", "}");
                }
                else if (userName.Contains(">"))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名中不允许包含'>'符号", "}");
                }
                else if ((!SecureHelper.IsSafeSqlString(userName, false)))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名已经存在", "}");
                }
                else if (CommonHelper.IsInArray(userName, WorkContext.MallConfig.ReservedName, "\n"))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名已经存在", "}");
                }
                else if (FilterWords.IsContainWords(userName))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名包含禁止单词", "}");
                }
                else if (Users.IsExistUserName(userName))
                {
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "userName", "用户名已经存在", "}");
                }
            }
            else
            {
                userName = WorkContext.UserName;
            }

            //验证昵称
            if (nickName.Length > 10)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "nickName", "昵称的长度不能大于10", "}");
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

            //验证性别
            if (gender < 0 || gender > 2)
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "gender", "请选择正确的性别", "}");

            //验证身份证号
            if (idCard.Length > 0 && !ValidateHelper.IsIdCard(idCard))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "idCard", "请输入正确的身份证号", "}");
            }
            // 验证银行名称
            if (bankname.Length > 25)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "bank", "银行名称长度不能大于5", "}");
            }
            // 验证银行卡号
            if (bankcardcode.Length > 30)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "bank", "银行卡号长度不能大于5", "}");
            }
            // 验证银行开户人
            if (bankusername.Length > 15)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "bank", "银行开户人长度不能大于5", "}");
            }
            //验证出生日期
            if (bday.Length == 0)
            {
                string bdayY = WebHelper.GetFormString("bdayY");
                string bdayM = WebHelper.GetFormString("bdayM");
                string bdayD = WebHelper.GetFormString("bdayD");
                bday = string.Format("{0}-{1}-{2}", bdayY, bdayM, bdayD);
            }
            if (bday.Length > 0 && bday != "--" && !ValidateHelper.IsDate(bday))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "bday", "请选择正确的日期", "}");

            //验证区域
            if (regionId > 0)
            {
                RegionInfo regionInfo = Regions.GetRegionById(regionId);
                if (regionInfo == null || regionInfo.Layer != 3)
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "regionId", "请选择正确的地址", "}");
            }

            //验证详细地址
            if (address.Length > 75)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "address", "详细地址的长度不能大于75", "}");
            }

            //验证简介
            if (bio.Length > 150)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "bio", "简介的长度不能大于150", "}");
            }

            if (errorList.Length == 1)
            {
                if (bday.Length == 0 || bday == "--")
                    bday = "1900-1-1";

                if (regionId < 1)
                    regionId = 0;

                Users.UpdateUser(WorkContext.Uid, userName, WebHelper.HtmlEncode(nickName), WebHelper.HtmlEncode(avatar), gender, WebHelper.HtmlEncode(realName), TypeHelper.StringToDateTime(bday), idCard, regionId, WebHelper.HtmlEncode(address), WebHelper.HtmlEncode(bio), bankname, bankcardcode, bankusername);
                if (userName.Length > 0 && nickName.Length > 0 && avatar.Length > 0 && realName.Length > 0 && bday != "1900-1-1" && idCard.Length > 0 && regionId > 0 && address.Length > 0)
                {
                    Credits.SendCompleteUserInfoCredits(ref WorkContext.PartUserInfo, DateTime.Now);
                }
                return AjaxResult("success", "信息更新成功");
            }
            else
            {
                return AjaxResult("error", errorList.Remove(errorList.Length - 1, 1).Append("]").ToString(), true);
            }
        }

        #endregion

        #region 安全中心

        /// <summary>
        /// 账户安全信息
        /// </summary>
        public ActionResult SafeInfo()
        {
            //UserInfo user = Users.GetUserById(WorkContext.uid);
            return View(WorkContext.PartUserInfo);
        }

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
                model.Mode = "password";

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
            if (Users.CreateUserPassword(password, WorkContext.PartUserInfo.Salt) != WorkContext.Password)
            {
                return AjaxResult("password", "密码不正确");
            }

            string v = MallUtils.AESEncrypt(string.Format("{0},{1},{2},{3}", WorkContext.Uid, action, DateTime.Now, Randoms.CreateRandomValue(6)));
            string url = Url.Action("safeupdate", new RouteValueDictionary { { "v", v } });
            return AjaxResult("success", url);
        }

        /// <summary>
        /// 发送验证手机短信验证码
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

            if (action.Length == 0 || !CommonHelper.IsInArray(action, new string[3] { "updatepassword", "updatemobile", "updateemail" }))
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

            if (action.Length == 0 || !CommonHelper.IsInArray(action, new string[3] { "updatepassword", "updatemobile", "updateemail" }))
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

            PageModel pageModel = new PageModel(7, page, Orders.GetUserOrderCount(WorkContext.Uid, startAddTime, endAddTime, orderState));

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

            return View(model);



            // OrderState a = OrderState.Changed;
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

            //int completeCount = Orders.GetUserOrderCount(WorkContext.Uid, "", "", (int)OrderState.Confirmed);
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
                //微信退款流程
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
                        ActionDes = "您取消了订单,请等待系统退款,退款会在1-3个工作日内将退款返回至帐号中"
                    });
                }
                else if (orderInfo.PayMode == 1 && orderInfo.PaySystemName == "IPSpay")//环迅支付退款需后台操作，此处只生成退款记录，
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
                        ActionDes = "您取消了订单,请等待系统退款,退款会在1-3个工作日内将退款返回至帐号中"
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
            return AjaxResult("success", string.Format("订单已完成，感谢您在{0}购物，欢迎再次光临",WorkContext.MallConfig.SiteTitle));

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
                message = "订单使用了汇购卡券支付，不能退货";
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
            if (orderInfo.OrderState != (int)OrderState.Completed && (TypeHelper.ObjectToDateTime(orderInfo.ReceivingTime).AddDays(7) <= DateTime.Now))
                return AjaxResult("error", "[{\"key\":\"errororderstate\",\"msg\":\"订单当前不能退货\"}]", true);

            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            //int completeCount = Orders.GetUserOrderCount(WorkContext.Uid, "", "", (int)OrderState.Completed);
            //if (completeCount <= 0)
            //    OrderUtils.UpdateFXUserSates(partUserInfo.Uid, 0);

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
            string storeName = WebHelper.GetQueryString("storeName").Trim();//店铺名称
            string productName = WebHelper.GetQueryString("productName").Trim();//商品名称

            if (!SecureHelper.IsSafeSqlString(storeName) || !SecureHelper.IsSafeSqlString(productName))
                return PromptView(WorkContext.UrlReferrer, "您搜索的内容不存在");

            PageModel pageModel = new PageModel(10, page, (storeName.Length > 0 || productName.Length > 0) ? FavoriteProducts.GetFavoriteProductCount(WorkContext.Uid, storeName, productName) : FavoriteProducts.GetFavoriteProductCount(WorkContext.Uid));

            FavoriteProductListModel model = new FavoriteProductListModel()
            {
                ProductList = (storeName.Length > 0 || productName.Length > 0) ? FavoriteProducts.GetFavoriteProductList(pageModel.PageSize, pageModel.PageNumber, WorkContext.Uid, storeName, productName) : FavoriteProducts.GetFavoriteProductList(pageModel.PageSize, pageModel.PageNumber, WorkContext.Uid),
                PageModel = pageModel,
                ProductName = productName
            };

            return View(model);
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
        public ActionResult FavoriteStoreList()
        {
            int page = WebHelper.GetQueryInt("page");//当前页数

            PageModel pageModel = new PageModel(10, page, FavoriteStores.GetFavoriteStoreCount(WorkContext.Uid));

            FavoriteStoreListModel model = new FavoriteStoreListModel()
            {
                StoreList = FavoriteStores.GetFavoriteStoreList(pageModel.PageSize, pageModel.PageNumber, WorkContext.Uid),
                PageModel = pageModel
            };

            return View(model);
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
        /// 配送地址信息
        /// </summary>
        public ActionResult ShipAddressInfo()
        {
            int saId = WebHelper.GetQueryInt("saId");
            FullShipAddressInfo fullShipAddressInfo = ShipAddresses.GetFullShipAddressBySAId(saId, WorkContext.Uid);
            //检查地址
            if (fullShipAddressInfo == null)
                return AjaxResult("noexist", "地址不存在");

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}\"saId\":\"{1}\",\"uid\":\"{2}\",\"regionId\":\"{3}\",\"isDefault\":\"{4}\",\"alias\":\"{5}\",\"consignee\":\"{6}\",\"mobile\":\"{7}\",\"phone\":\"{8}\",\"email\":\"{9}\",\"zipCode\":\"{10}\",\"address\":\"{11}\",\"provinceId\":\"{12}\",\"provinceName\":\"{13}\",\"cityId\":\"{14}\",\"cityName\":\"{15}\",\"countyId\":\"{16}\",\"countyName\":\"{17}\"{18}", "{", fullShipAddressInfo.SAId, fullShipAddressInfo.Uid, fullShipAddressInfo.RegionId, fullShipAddressInfo.IsDefault, fullShipAddressInfo.Alias, fullShipAddressInfo.Consignee, fullShipAddressInfo.Mobile, fullShipAddressInfo.Phone, fullShipAddressInfo.Email, fullShipAddressInfo.ZipCode, fullShipAddressInfo.Address, fullShipAddressInfo.ProvinceId, fullShipAddressInfo.ProvinceName, fullShipAddressInfo.CityId, fullShipAddressInfo.CityName, fullShipAddressInfo.CountyId, fullShipAddressInfo.CountyName, "}");

            return AjaxResult("success", sb.ToString(), true);
        }

        /// <summary>
        /// 添加配送地址
        /// </summary>
        public ActionResult AddShipAddress()
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

        /// <summary>
        /// 编辑配送地址
        /// </summary>
        public ActionResult EditShipAddress()
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
            if (string.IsNullOrWhiteSpace(alias))
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "alias", "别名不能为空", "}");
            else if (alias.Length > 25)
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
        /// 支付积分日志列表
        /// </summary>
        public ActionResult PayCreditLogList()
        {
            int type = WebHelper.GetQueryInt("type", 2);
            int page = WebHelper.GetQueryInt("page");

            PageModel pageModel = new PageModel(10, page, Credits.GetPayCreditLogCount(WorkContext.Uid, type));
            PayCreditLogListModel model = new PayCreditLogListModel()
            {
                ListType = type,
                PageModel = pageModel,
                PayCreditLogList = Credits.GetPayCreditLogList(WorkContext.Uid, type, pageModel.PageSize, pageModel.PageNumber)
            };

            return View(model);
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

        /// <summary>
        /// 激活优惠劵
        /// </summary>
        public ActionResult ActivateCoupon()
        {
            string activateKey1 = WebHelper.GetFormString("activateKey1");
            string activateKey2 = WebHelper.GetFormString("activateKey2");
            string activateKey3 = WebHelper.GetFormString("activateKey3");
            string activateKey4 = WebHelper.GetFormString("activateKey4");

            if (activateKey1.Length != 4 || activateKey2.Length != 4 || activateKey3.Length != 4 || activateKey4.Length != 4)
                return AjaxResult("errorcouponsn", "优惠劵编号不正确");

            //优惠劵编号
            string couponSN = activateKey1 + activateKey2 + activateKey3 + activateKey4;
            //优惠劵
            CouponInfo couponInfo = Coupons.GetCouponByCouponSN(couponSN);
            if (couponInfo == null)
                return AjaxResult("noexist", "优惠劵不存在");
            if (couponInfo.Uid > 0)
                return AjaxResult("used", "优惠劵已使用");
            //优惠劵类型
            CouponTypeInfo couponTypeInfo = Coupons.GetCouponTypeById(couponInfo.CouponTypeId);
            if (couponTypeInfo == null)
                return AjaxResult("nocoupontype", "优惠劵类型不存在");
            if (couponTypeInfo.UseExpireTime == 0 && couponTypeInfo.UseEndTime <= DateTime.Now)
                return AjaxResult("expired", "此优惠劵已过期");
            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(couponTypeInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return AjaxResult("nostore", "店铺不存在");

            Coupons.ActivateCoupon(couponInfo.CouponId, WorkContext.Uid, DateTime.Now, WorkContext.IP);
            return AjaxResult("success", "优惠劵激活成功");
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
            model.CashDetailList = CashCouponDetail.GetListByPage(string.Format(" CashId={0} AND Uid={1}", CashId, WorkContext.Uid), "", 1, 10);
            return View(model);
        }

        #endregion

        #region 商品咨询

        /// <summary>
        /// 商品咨询列表
        /// </summary>
        public ActionResult ProductConsultList()
        {
            int page = WebHelper.GetQueryInt("page");

            PageModel pageModel = new PageModel(10, page, ProductConsults.GetUserProductConsultCount(WorkContext.Uid));
            UserProductConsultListModel model = new UserProductConsultListModel()
            {
                PageModel = pageModel,
                ProductConsultList = ProductConsults.GetUserProductConsultList(WorkContext.Uid, pageModel.PageSize, pageModel.PageNumber)
            };

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

        /// <summary>
        /// 商品评价列表
        /// </summary>
        public ActionResult ProductReviewList()
        {
            int page = WebHelper.GetQueryInt("page", 1);

            PageModel pageModel = new PageModel(10, page, ProductReviews.GetUserProductReviewCount(WorkContext.Uid));
            UserProductReviewListModel model = new UserProductReviewListModel()
            {
                PageModel = pageModel,
                ProductReviewList = ProductReviews.GetUserProductReviewList(WorkContext.Uid, pageModel.PageSize, pageModel.PageNumber)
            };

            return View(model);
        }

        #endregion

        #region 账户信息
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
            model.AccountName = accountName;
            if (accountId == (int)AccountType.代理账户 || accountId == (int)AccountType.佣金账户)
            {
                model.PageModel = new PageModel(pageSize, page, Account.GetAccountDetailCount(uid, accountId));
                model.AccountDetailList = Account.GetAccountDetailList(uid, accountId, page, pageSize);
            }
            else
            {
                if (!WorkContext.IsDirSaleUser)//汇购会员
                {
                    model.PageModel = new PageModel(pageSize, page, Account.GetAccountDetailCount(uid, accountId));
                    model.AccountDetailList = Account.GetAccountDetailList(uid, accountId, page, pageSize);
                }
                else//直销会员通过接口取流水
                {
                    int totalCount = 0;
                    model.AccountDetailList = AccountUtils.GetDetail(WorkContext.DirSaleUid, accountId, page, pageSize, ref totalCount);
                    model.PageModel = new PageModel(pageSize, page, totalCount);
                }
            }
            //List<AccountDetailInfo> detailInfo = Account.GetAccountDetailList(uid, accountId);
            return View(model);
        }

        #endregion

        #region 分享会员列表
        /// <summary>
        /// 获得分享会员列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SubRecommendList()
        {
            // List<UserInfo> subUserList = Users.GetSubRecommendListByPid(WorkContext.PartUserInfo);


            int page = WebHelper.GetQueryInt("page");
            if (page == 0)
                page = 1;

            string startAddTime = WebHelper.GetQueryString("startAddTime");
            string endAddTime = WebHelper.GetQueryString("endAddTime");
            int pageSize = 10;
            List<UserInfo> subUserList = Users.GetSubRecommendListByPid(WorkContext.PartUserInfo, page, pageSize);

            PageModel pageModel = new PageModel(pageSize, page, Users.GetUserCount(WorkContext.PartUserInfo));


            //二维码
            string parentName = string.IsNullOrEmpty(WorkContext.UserName) ? (string.IsNullOrEmpty(WorkContext.UserEmail) ? (string.IsNullOrEmpty(WorkContext.UserMobile) ? "" : WorkContext.UserMobile) : WorkContext.UserEmail) : WorkContext.UserName;
            bool isMobile = true;// WebHelper.IsMobile();
            string shareUrl = "http://" + Request.Url.Host + ":" + Request.Url.Port + (isMobile ? "/mob" : "") + "/account/register?pname=" + parentName.Trim() + "&returnUrl=http://" + Request.Url.Host + ":" + Request.Url.Port + (isMobile ? "/mob" : "");
            //string codeImgPath = CreateCode_Simple(shareUrl, WorkContext.PartUserInfo.Uid, WorkContext.PartUserInfo.Salt, isMobile);
            //ViewData["codeImg"] = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/upload/usersharecode/" + (isMobile ? "/m/" : "/pc/") + codeImgPath;

            string bgQRcode = IOHelper.CreatQRCodeWithBG2(shareUrl, WorkContext.Uid, WorkContext.PartUserInfo);

            ViewData["codeImg"] = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/upload/bgqrcode/" + bgQRcode;

            //图表化结构图
            //UserInfo parent = Users.GetUserById(WorkContext.Uid);
            //List<UserInfo> children = Users.GetSubRecommendListByPid(WorkContext.PartUserInfo,1,100);
            //children.Add(parent);//将父级加入到list中 
            //ViewData["treeStr"] = GetTreeOrgChart(children);//数据拼接成组织层级 

            //var list = from f in children orderby f.Uid select new { id = f.Uid, pid = f.Pid, name = f.NickName };


            List<OrderInfo> orderList = new List<OrderInfo>();
            List<OrderProductInfo> opList = new List<OrderProductInfo>();
            SubRecommendListModel list = new SubRecommendListModel();
            list.UserList = subUserList;
            subUserList.ForEach(y =>
            {
                string sqlStr = string.Format(" orderstate>={0} and  orderstate<={1} and uid={2}", (int)OrderState.Confirmed, (int)OrderState.Completed, y.Uid);
                List<OrderInfo> UserOrderList = Orders.GetOrderListByWhere(sqlStr);
                orderList.AddRange(UserOrderList);
                StringBuilder oidList = new StringBuilder();
                foreach (OrderInfo row in UserOrderList)
                {
                    oidList.AppendFormat("{0},", row.Oid);
                }
                if (oidList.Length > 0)
                    oidList.Remove(oidList.Length - 1, 1);

                List<OrderProductInfo> userOPList = Orders.GetOrderProductList(oidList.ToString());
                userOPList.ForEach(x =>
                {
                    PartProductInfo pro = Products.GetPartProductById(x.Pid);
                    if (pro != null)
                    {
                        x.PV = pro.PV;
                        x.HaiMi = pro.HaiMi;
                    }
                });
                opList.AddRange(userOPList);
            });

            list.OrderList = orderList;
            list.OrderProductList = opList;
            list.PageModel = pageModel;


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
            int pageSize = 10;
            PageModel pageModel = new PageModel(pageSize, page, Orders.GetUserOrderCount(uid, startAddTime, endAddTime, orderState));

            DataTable orderList = Orders.GetUserOrderList(uid, pageModel.PageSize, pageModel.PageNumber, startAddTime, endAddTime, orderState);
            StringBuilder oidList = new StringBuilder();
            foreach (DataRow row in orderList.Rows)
            {
                oidList.AppendFormat("{0},", row["oid"]);
            }
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


            return View(model);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult getSubList()
        //{
        //    UserInfo parent = Users.GetUserById(WorkContext.Uid);
        //    List<UserInfo> children = Users.GetSubRecommendListByPid(WorkContext.PartUserInfo);
        //    children.Add(parent);//将父级加入到list中 序列化到前台页面

        //    //List<PartUserInfo> list = (List<PartUserInfo>)from f in children orderby f.Uid select new { id = f.Uid, pid = f.Pid, name = f.NickName };
        //    //string str = GetTreeOrgChart(list);

        //    var s = children.Select(x => new { id = x.Pid, pid = x.Pid });
        //    int ss = s.Count();
        //    return Json(from f in children orderby f.Uid select new { id = f.Uid, pid = f.Pid, name = f.NickName });

        //}

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
            AccountInfo accountInfo = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid).Find(x => x.AccountId == (int)AccountType.海米账户);
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
            info.AccountId = (int)AccountType.佣金账户;
            info.DrawCashSN = "HMTX" + DateTime.Now.ToString("yyyyMMddHHmmss") + Randoms.CreateRandomValue(5);
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
                Account.UpdateAccountForOut(new AccountInfo()
                {
                    AccountId = (int)AccountType.海米账户,
                    UserId = WorkContext.Uid,
                    TotalOut = info.Amount
                });
                Account.CreateAccountDetail(new AccountDetailInfo()
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
            model.AccountName = Account.GetAccountName(accountId);
            return View(model);
        }

        #endregion

        #region 分享会员列表辅助方法
        /// <summary>
        /// 解析List集合转换为Json字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected static string ObjectToJson<T>(T obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //JSON序列化  
            return serializer.Serialize(obj);
        }

        //参数为整个表树形集合
        public string GetTreeOrgChart(List<UserInfo> list)
        {
            var strHtml_OrgChart = new StringBuilder { };
            List<UserInfo> itemNode = list.FindAll(t => t.ParentLevel == 0);
            foreach (PartUserInfo entity in itemNode)
            {
                //string itemid = "uid" + entity.Uid;
                //strHtml_OrgChart.Append("var " + itemid + "={};");
                //strHtml_OrgChart.Append(" " + itemid + ".id='" + entity.Uid + "'; " + "" + itemid + ".name='" + entity.NickName + "'; " + itemid + ".data={}; " + itemid + ".children=[];");

                ////创建子节点
                //strHtml_OrgChart.Append(GetTreeNodeOrgChart(entity.Uid, list));

                string itemid = "uid" + entity.Uid;
                strHtml_OrgChart.Append("var " + itemid + " = new OrgNode();");
                strHtml_OrgChart.Append("" + itemid + ".Text = \"" + entity.NickName + "\";");
                strHtml_OrgChart.Append("" + itemid + ".Description = \"" + entity.NickName + "\";");
                //strHtml_OrgChart.Append("" + itemid + ".Link = \"#\";");
                //创建子节点
                strHtml_OrgChart.Append(GetTreeNodeOrgChart(entity.Uid, list));
            }

            return strHtml_OrgChart.ToString();
        }

        /// <summary>
        /// 创建子节点
        /// </summary>
        /// <param name="parentID">父节点主键</param>
        /// <param name="list">菜单集合</param>
        /// <returns></returns>
        public string GetTreeNodeOrgChart(int ParentId, List<UserInfo> list)
        {
            StringBuilder sb_TreeNode = new StringBuilder();
            List<UserInfo> itemNode = list.FindAll(t => t.Pid == ParentId && t.ParentLevel == 1);
            if (itemNode.Count > 0)
            {
                foreach (PartUserInfo entity in itemNode)
                {
                    //string itemid = "uid" + entity.Uid;
                    //string itemParentId = "uid" + ParentId;

                    //sb_TreeNode.Append("var " + itemid + "={};");
                    //sb_TreeNode.Append(" " + itemid + ".id='" + entity.Uid + "'; " + "" + itemid + ".name='" + entity.NickName + "'; " + itemid + ".data={}; " + itemid + ".children=[];");
                    //sb_TreeNode.Append(GetTreeNodeOrgChart(entity.Uid, list));
                    //sb_TreeNode.Append("" + itemParentId + ".children.push(" + itemid + ") ;");


                    string itemid = "uid" + entity.Uid;
                    string itemParentId = "uid" + ParentId;
                    sb_TreeNode.Append("var " + itemid + " = new OrgNode();");
                    sb_TreeNode.Append("" + itemid + ".Text = \"" + entity.NickName + "\";");
                    sb_TreeNode.Append("" + itemid + ".Description = \"" + entity.NickName + "\";");
                    //sb_TreeNode.Append("" + itemid + ".Link = \"#\";");
                    sb_TreeNode.Append("" + itemParentId + ".Nodes.Add(" + itemid + ");");
                    //创建子节点
                    List<UserInfo> children = Users.GetSubRecommendListByPid(entity, 1, 100);
                    sb_TreeNode.Append(GetTreeNodeOrgChart(entity.Uid, children));
                }
            }
            return sb_TreeNode.ToString();
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
            MemberInfo member = AccountUtils.CreateMember(joinUser, realName, managerCode, placeSide, userCard, userPhone);

            return RedirectToAction("agentresult", new { ParentCode = member.ParentCode, ManagerCode = member.ManagerCode, joinuid = joinuid });
            //return View("agentresult", member);
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
        /// 获得我的代理证书
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAgentCert()
        {

            //代理证书
            string parentName = string.IsNullOrEmpty(WorkContext.UserName) ? (string.IsNullOrEmpty(WorkContext.UserEmail) ? (string.IsNullOrEmpty(WorkContext.UserMobile) ? "" : WorkContext.UserMobile) : WorkContext.UserEmail) : WorkContext.UserName;
            bool isMobile = true;// WebHelper.IsMobile();
            string shareUrl = "";

            string bgQRcode = IOHelper.CreatQRCodeWithBG2(shareUrl, WorkContext.Uid, WorkContext.PartUserInfo);

            ViewData["codeImg"] = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/upload/bgqrcode/" + bgQRcode;

            return View();
        }

        #region 提现

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
            info.DrawCashSN = "DLTX" + DateTime.Now.ToString("yyyyMMddHHmmss") + Randoms.CreateRandomValue(5);
            info.Amount = TypeHelper.StringToDecimal(Amount);
            //info.Poundage = TypeHelper.StringToDecimal(Amount) * BMAConfig.MallConfig.YJFeeRate;
            //info.TaxAmount = TypeHelper.StringToDecimal(Amount) * BMAConfig.MallConfig.HMTaxRate;
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
            Account.UpdateAccountForOut(new AccountInfo()
            {
                AccountId = (int)AccountType.代理账户,
                UserId = WorkContext.Uid,
                TotalOut = info.Amount
            });
            Account.CreateAccountDetail(new AccountDetailInfo()
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
                Account.UpdateAccountForOut(new AccountInfo()
                {
                    AccountId = (int)AccountType.佣金账户,
                    UserId = WorkContext.Uid,
                    TotalOut = info.Amount
                });
                Account.CreateAccountDetail(new AccountDetailInfo()
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
                decimal PVTotal1 = 0;//Orders.GetListForChongXiao(WorkContext.Uid);
                decimal PVTotal2 = AccountUtils.GetUserReorderPV(WorkContext.PartUserInfo.DirSaleUid, DateTime.Now.ToString("yyyy-MM-dd"));
                ViewData["PVTotal"] = PVTotal1 + PVTotal2;

                return View();
            }
            else
                return Content("");
        }
        #endregion

        #region 商务中心
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DSOrderApply()
        {
            if (WorkContext.PartUserInfo.BusiCentType != 1)
                return PromptView("非商务中心不能代报单");
            string allowImgType = string.Empty;
            string[] imgTypeList = StringHelper.SplitString(BMAConfig.MallConfig.UploadImgType, ",");
            foreach (string imgType in imgTypeList)
                allowImgType += string.Format("*{0};", imgType.ToLower());

            string dsorderpid = WebHelper.GetConfigSettings("DSOrderPid_1980");
            List<PartProductInfo> productList = Products.GetPartProductList(dsorderpid);
            List<SelectListItem> itemList = new List<SelectListItem>();
            productList.ForEach(x =>
            {
                itemList.Add(new SelectListItem() { Text = x.Name, Value = x.Pid.ToString() });
            });
            ViewData["productList"] = itemList;
            ViewData["allowImgType"] = allowImgType;
            ViewData["maxImgSize"] = BMAConfig.MallConfig.UploadImgSize;
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DSOrderApplySubmit()
        {
            if (WorkContext.PartUserInfo.BusiCentType != 1)
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"非商务中心不能代报单\"}]", true);
            //'pid': pid, 'userCode': userCode, 'realName': realName, 'idCard': idCard, 'regionId': regionId, 'address': address, 'parentCode': parentCode, 'managerCode': managerCode, 'placeSide': placeSide, 'payImg': payImg, 'verifyCode': verifyCode

            int pid = TypeHelper.StringToInt(WebHelper.GetFormString("pid"));
            string userCode = WebHelper.GetFormString("userCode");
            string realName = WebHelper.GetFormString("realName");
            string idCard = WebHelper.GetFormString("idCard");
            string consignee = WebHelper.GetFormString("consignee");
            string consignMobile = WebHelper.GetFormString("consignMobile");
            int regionId = TypeHelper.StringToInt(WebHelper.GetFormString("regionId"));
            string address = WebHelper.GetFormString("address");
            string parentCode = WebHelper.GetFormString("parentCode");
            string managerCode = WebHelper.GetFormString("managerCode");
            int placeSide = WebHelper.GetFormInt("placeSide");
            string payImg = WebHelper.GetFormString("payImg");
            string verifyCode = WebHelper.GetFormString("verifyCode");

            StringBuilder errorList = new StringBuilder("[");
            #region 提交参数验证
            if (pid <= 0 || userCode == "" || realName == "" || idCard == "" || consignee == "" || consignMobile == "" || regionId <= 0 || address == "" || parentCode == "" || payImg == "" || verifyCode == "" || placeSide <= 0)
                return AjaxResult("error", "[{\"key\":\"parmaerror\",\"msg\":\"缺少必填参数\"}]", true);
            //验证码
            if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
                return AjaxResult("error", "[{\"key\":\"codeerror\",\"msg\":\"验证码不正确\"}]", true);

            PartProductInfo product = Products.GetPartProductById(pid);
            if (product == null)
                return AjaxResult("error", "[{\"key\":\"parmaerror\",\"msg\":\"产品不存在\"}]", true);
            #region 会员编号验证

            if (ValidateHelper.IsMobile(userCode))
            {
                if (Users.IsExistMobile(userCode) || AccountUtils.IsUserExistForDirSale(userCode))
                    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"手机号已经存在\"}]", true);
            }
            else
            {
                if (Users.IsExistUserName(userCode) || AccountUtils.IsUserExistForDirSale(userCode))
                    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"会员编号已经存在\"}]", true);
                Regex regNum = new Regex(@"[\u4e00-\u9fa5]+");
                if (regNum.IsMatch(userCode))
                    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"账户名不能包含中文字符\"}]", true);
                else if (userCode.Length < 4 || userCode.Length > 20)
                    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"会员编号必须大于3且不大于20个字符\"}]", true);
                else if (userCode.Contains(" ") || userCode.Contains(":") || userCode.Contains("<") || userCode.Contains(">"))
                    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"会员编号中不允许符号字符\"}]", true);
                else if ((!SecureHelper.IsSafeSqlString(userCode, false)) || CommonHelper.IsInArray(userCode, WorkContext.MallConfig.ReservedName, "\n"))
                    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"会员编号已经存在\"}]", true);
                else if (FilterWords.IsContainWords(userCode))
                    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"会员编号已经存在\"}]", true);
            }
            #endregion
            if (!ValidateHelper.IsIdCard(idCard))
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"身份证格式错误\"}]", true);
            if (ValidateHelper.IsMobile(parentCode))
            {
                if (!Users.IsExistMobile(parentCode) || !AccountUtils.IsUserExistForDirSale(parentCode))
                    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"推荐人不存在\"}]", true);
            }
            else
            {
                if (!Users.IsExistUserName(parentCode) || !AccountUtils.IsUserExistForDirSale(parentCode))
                    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"推荐人不存在\"}]", true);
            }
            if (!string.IsNullOrEmpty(managerCode))
            {
                if (ValidateHelper.IsMobile(managerCode))
                {
                    if (!Users.IsExistMobile(managerCode) || !AccountUtils.IsUserExistForDirSale(managerCode))
                        return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"安置人不存在\"}]", true);
                }
                else
                {
                    if (!Users.IsExistUserName(managerCode) || !AccountUtils.IsUserExistForDirSale(managerCode))
                        return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"安置人不存在\"}]", true);
                }
            }

            #endregion
            OrderApplyInfo info = new OrderApplyInfo()
            {
                operateuid = WorkContext.Uid,
                pid = pid,
                usercode = userCode,
                realname = realName,
                idcard = idCard,
                regionid = regionId,
                address = address,
                parentcode = parentCode,
                managercode = managerCode,
                placeside = placeSide,
                payimg = payImg,
                consignee = consignee,
                consignmobile = consignMobile
            };
            int result = new OrderApply().Add(info);
            if (result > 0)
                return AjaxResult("success", "提交成功");
            else
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"提交失败\"}]", true);
        }

        /// <summary>
        /// 报单历史记录
        /// </summary>
        /// <returns></returns>
        public ActionResult DsorderHistory(int operateuid, int pageSize = 10, int pageNumber = 1)
        {
            DsOrderHistoryModel model = new DsOrderHistoryModel();
            string condition = " operateuid=" + operateuid;
            PageModel pageModel = new PageModel(pageSize, pageNumber, OrderBLL.GetRecordCount(condition));
            DataTable list = OrderBLL.AdminGetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize);

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

        #region 公共方法

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
