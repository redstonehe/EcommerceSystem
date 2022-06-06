using JWT;
using JWT.Help;
using JWT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using VMall.Core;
using VMall.Services;

namespace VMall.Web.Areas.WebApi.Controllers
{
    public class JwtController : Controller
    {
        // GET: Jwt
        public ActionResult Index(string username, string password)
        {
            #region 验证
            string accountName = username;
            DataResult errorResult = new DataResult() { Token = "", Success = false, Message = "生成token失败" };
            //验证账户名
            Regex regNum = new Regex(@"[\u4e00-\u9fa5]+");
            if (string.IsNullOrWhiteSpace(accountName))
                return Json(errorResult, JsonRequestBehavior.AllowGet);
            //errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不能为空", "}");

            else if (regNum.IsMatch(accountName))
                return Json(errorResult, JsonRequestBehavior.AllowGet);
            //errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不能包含中文字符", "}");

            else if (accountName.Length < 2 || accountName.Length > 50)
                return Json(errorResult, JsonRequestBehavior.AllowGet);
            //errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名必须大于2且不大于50个字符", "}");

            else if ((!SecureHelper.IsSafeSqlString(accountName, false)))
                return Json(errorResult, JsonRequestBehavior.AllowGet);
            //errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不存在", "}");

            //验证密码
            if (string.IsNullOrWhiteSpace(password))
                return Json(errorResult, JsonRequestBehavior.AllowGet);
            //errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码不能为空", "}");

            else if (password.Length < 6 || password.Length > 32)
                return Json(errorResult, JsonRequestBehavior.AllowGet);
            //errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码必须大于6且不大于32个字符", "}");

            //当以上验证全部通过时
            PartUserInfo partUserInfo = null;
            if (ValidateHelper.IsMobile(accountName))//手机登陆
            {
                partUserInfo = Users.GetPartUserByMobile(accountName);
                if (partUserInfo == null)
                    return Json(errorResult, JsonRequestBehavior.AllowGet);
                //errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "手机不存在", "}");
            }
            if(partUserInfo==null)
                return Json(errorResult, JsonRequestBehavior.AllowGet);
            string ip = WebHelper.GetIP();
            RegionInfo region = Regions.GetRegionByIP(ip);
            int regionId = region != null ? region.RegionId : -1;
            //判断密码是否正确
            if (partUserInfo != null && Users.CreateUserPassword(password, partUserInfo.Salt) != partUserInfo.Password)
            {
                LoginFailLogs.AddLoginFailTimes(ip, DateTime.Now);//增加登陆失败次数
                return Json(errorResult, JsonRequestBehavior.AllowGet);
                //errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码不正确", "}");
            }

            #endregion

            //验证成功时
            //当用户等级是禁止访问等级时
            if (partUserInfo.UserRid == 1)
                return Json(errorResult, JsonRequestBehavior.AllowGet);
            //return AjaxResult("lockuser", "您的账号当前被锁定,不能访问");

            //删除登陆失败日志
            LoginFailLogs.DeleteLoginFailLogByIP(ip);
            //更新用户最后访问
            Users.UpdateUserLastVisit(partUserInfo.Uid, DateTime.Now, ip, regionId);

            if (string.IsNullOrEmpty(partUserInfo.DirSalePwd))
            {
                partUserInfo.DirSalePwd = SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);
                Users.UpdatePartUserDirSalePwd(partUserInfo);
            }

            DataResult result = new DataResult();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            var now = provider.GetNow();
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // or use JwtValidator.UnixEpoch
            var secondsSinceEpoch = Math.Round((now - unixEpoch).TotalSeconds);

            //假设用户名为"admin"，密码为"123"  ，根据密码取出uid，并返回
            //if (username == "admin" && password == "123")
            if (partUserInfo.Uid > 0)
            {
                int expTime = 60 * 60 * 2;//过期时间，两小时
                var payload = new Dictionary<string, object>
                {
                    { "iss","ZHMallAuthor" },
                    { "sub","auth_user" },
                    { "aud","api_user" },
                    { "iat",secondsSinceEpoch },
                    { "exp",secondsSinceEpoch+expTime },
                    { "username",username },
                    { "uid", partUserInfo.Uid },
                    { "deviceid", "" }
                    //{ "jti","ZHMallAuthor" }
                    //{ "ExpiryDateTime", DateTime.Now.AddMinutes(1) }
                    //{ "exp", DateTimeOffset.UtcNow.AddSeconds(2).ToUnixTimeSeconds() }
                };
                result.Token = JwtHelp.SetJwtEncode(payload);
                result.Success = true;
                result.Message = "成功";
            }
            else
            {
                result.Token = "";
                result.Success = false;
                result.Message = "生成token失败";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建jwtToken
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public ActionResult CreateToken(string username, string pwd)
        {
            DataResult result = new DataResult();

            IDateTimeProvider provider = new UtcDateTimeProvider();
            var now = provider.GetNow();

            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // or use JwtValidator.UnixEpoch
            var secondsSinceEpoch = Math.Round((now - unixEpoch).TotalSeconds);

            //假设用户名为"admin"，密码为"123"  ，根据密码取出uid，并返回
            if (username == "admin" && pwd == "123")
            {
                int expTime = 60 * 60 * 2;//过期时间，两小时
                var payload = new Dictionary<string, object>
                {
                    { "iss","ZHMallAuthor" },
                    { "sub","auth_user" },
                    { "aud","api_user" },
                    { "iat",secondsSinceEpoch },
                    { "exp",secondsSinceEpoch+expTime },
                    { "username",username },
                    { "uid", 1 },
                    { "deviceid", "" }
                    //{ "jti","ZHMallAuthor" }
                    //{ "ExpiryDateTime", DateTime.Now.AddMinutes(1) }
                    //{ "testStr", "hello" },
                    //{"testObj" ,new { name="111"} },
                    //{ "exp", DateTimeOffset.UtcNow.AddSeconds(2).ToUnixTimeSeconds() }
                };

                result.Token = JwtHelp.SetJwtEncode(payload);
                result.Success = true;
                result.Message = "成功";
            }
            else
            {
                result.Token = "";
                result.Success = false;
                result.Message = "生成token失败";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}