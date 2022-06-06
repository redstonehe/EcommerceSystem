using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.Api.Controllers
{
    public partial class AccountController : BaseApiController
    {
        public ActionResult TestLogin()
        {
            return AjaxResult("success", "登录成功");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ActionResult Login(string userName, string password)
        {
            StringBuilder errorList = new StringBuilder("[");
            //验证账户名
            if (string.IsNullOrWhiteSpace(userName))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不能为空", "}");
            }
            else if (userName.Length < 4 || userName.Length > 50)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名必须大于3且不大于50个字符", "}");
            }
            else if ((!SecureHelper.IsSafeSqlString(userName, false)))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "账户名不存在", "}");
            }

            //验证密码
            if (string.IsNullOrWhiteSpace(password))
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码不能为空", "}");
            }
            else if (password.Length < 4 || password.Length > 32)
            {
                errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码必须大于3且不大于32个字符", "}");
            }

            //当以上验证全部通过时
            bool isDirSaleUser = false;
            PartUserInfo partUserInfo = null;
            if (errorList.Length == 1)
            {
                if (BMAConfig.MallConfig.LoginType.Contains("2") && ValidateHelper.IsEmail(userName))//邮箱登陆
                {
                    partUserInfo = Users.GetPartUserByEmail(userName);
                    if (partUserInfo == null)
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "邮箱不存在", "}");
                }
                else if (BMAConfig.MallConfig.LoginType.Contains("3") && ValidateHelper.IsMobile(userName))//手机登陆
                {
                    partUserInfo = Users.GetPartUserByMobile(userName);
                    if (partUserInfo == null)
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "手机不存在", "}");
                }
                else if (BMAConfig.MallConfig.LoginType.Contains("1"))//用户名登陆
                {
                    partUserInfo = Users.GetPartUserByName(userName);
                    if (partUserInfo == null)
                        errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "用户名不存在", "}");
                }
                //判断密码是否正确
                if (partUserInfo != null && Users.CreateUserPassword(password, partUserInfo.Salt) != partUserInfo.Password)
                {
                    //LoginFailLogs.AddLoginFailTimes(WorkContext.IP, DateTime.Now);//增加登陆失败次数
                    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "password", "密码不正确", "}");
                }

                //判断是不是直销会员登录
                if (partUserInfo == null)
                {
                    string errMsg = string.Empty;
                    int userId = Users.CheckLogin(userName, password, Session.SessionID, 1, 1, out errMsg);
                    if (userId > 0)
                    {
                        partUserInfo = new PartUserInfo();
                        partUserInfo.Uid = userId;
                        partUserInfo.Password = SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);
                        //UserInfo userinfo = MemberHelp.GetMember(UserId);
                        errorList.Clear();
                        isDirSaleUser = true;
                    }
                }
            }

            if (errorList.Length > 1)//验证失败时
            {
                return AjaxResult("error", errorList.Remove(errorList.Length - 1, 1).Append("]").ToString(), true);
            }
            else//验证成功时
            {
                //当用户等级是禁止访问等级时
                if (partUserInfo.UserRid == 1)
                    return AjaxResult("lockuser", "您的账号当前被锁定,不能访问");                

                return AjaxResult("success", "登录成功");
            }
        }
    }
}
