using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台广告控制器类
    /// </summary>
    public partial class AccountController : BaseMallAdminController
    {
        private static int AdminLoginFailTimes = 5;//后台最大密码错误次数
        /// <summary>
        /// 后台默认页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (WorkContext.Uid > 0 && WorkContext.MallAGid > 1)
                return RedirectToAction("index", "home");
            return View("IndexNew");
        }
        /// <summary>
        /// 后台默认页
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexNew()
        {
            if (WorkContext.Uid > 0 && WorkContext.MallAGid > 1)
                return RedirectToAction("index", "home");
            return View();
        }
        /// <summary>
        /// 管理员登录
        /// </summary>
        [HttpPost]
        public ActionResult Login()
        {
            string returnUrl = WebHelper.GetQueryString("returnUrl");
            if (returnUrl.Length == 0)
                returnUrl = "/malladmin";

            //int cookieUid = MallUtils.GetUidCookie();
            //if (cookieUid > 0)
            //    return RedirectToAction("index", "home");
            if (AdminLoginFailTimes != 0 && LoginFailLogs.GetLoginFailTimesByIp(WebHelper.GetIP()) >= AdminLoginFailTimes)
                return AjaxResult("fail", "您已经输入错误" + AdminLoginFailTimes + "次密码，请15分钟后再登陆!");

            string accountName = WebHelper.GetFormString("userName");
            string password = WebHelper.GetFormString("password");
            string verifyCode = WebHelper.GetFormString("verifyCode");
            int isRemember = WebHelper.GetFormInt("isRemember");


            #region 验证

            //验证账户名
            Regex regNum = new Regex(@"[\u4e00-\u9fa5]+");
            if (string.IsNullOrWhiteSpace(accountName))
            {
                return AjaxResult("fail", "账户名不能为空");
            }
            else if (regNum.IsMatch(accountName))
            {
                return AjaxResult("fail", "账户名不能包含中文字符");
            }
            else if (accountName.Length < 2 || accountName.Length > 50)
            {
                return AjaxResult("fail", "账户名必须大于2且不大于50个字符");
            }
            else if ((!SecureHelper.IsSafeSqlString(accountName, false)))
            {
                return AjaxResult("fail", "账户名不存在");
            }

            //验证密码
            if (string.IsNullOrWhiteSpace(password))
            {
                return AjaxResult("fail", "密码不能为空");
            }
            else if (password.Length < 6 || password.Length > 32)
            {
                return AjaxResult("fail", "密码必须大于6且不大于32个字符");
            }

            //验证验证码

            if (string.IsNullOrWhiteSpace(verifyCode))
            {
                return AjaxResult("fail", "验证码不能为空");
            }
            else if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
            {
                return AjaxResult( "fail", "验证码不正确");
            }

            //当以上验证全部通过时

            PartUserInfo partUserInfo = null;

            if (BMAConfig.MallConfig.LoginType.Contains("2") && ValidateHelper.IsEmail(accountName))//邮箱登陆
            {

                partUserInfo = Users.GetPartUserByEmail(accountName);
                if (partUserInfo == null)
                    return AjaxResult("fail", "邮箱不存在");
            }
            else if (BMAConfig.MallConfig.LoginType.Contains("3") && ValidateHelper.IsMobile(accountName))//手机登陆
            {

                partUserInfo = Users.GetPartUserByMobile(accountName);
                if (partUserInfo == null)
                    return AjaxResult("fail", "手机不存在");
            }
            else if (BMAConfig.MallConfig.LoginType.Contains("1"))//用户名登陆
            {

                partUserInfo = Users.GetPartUserByName(accountName);
                if (partUserInfo == null)
                    return AjaxResult("fail", "用户名不存在");
            }

            //判断密码是否正确
            if (partUserInfo != null && Users.CreateUserPassword(password, partUserInfo.Salt) != partUserInfo.Password)
            {
                LoginFailLogs.AddLoginFailTimes(WebHelper.GetIP(), DateTime.Now);//增加登陆失败次数
                return AjaxResult("fail", "密码不正确");
            }

            #endregion

            //当用户不满足后台权限
            if (partUserInfo.MallAGid < 2)
                return AjaxResult("fail", "用户名或密码不正确");

            //删除登陆失败日志
            LoginFailLogs.DeleteLoginFailLogByIP(WorkContext.IP);
            //更新用户最后访问
            Users.UpdateUserLastVisit(partUserInfo.Uid, DateTime.Now, WorkContext.IP, WorkContext.RegionId);
            //更新购物车中用户id
            Carts.UpdateCartUidBySid(partUserInfo.Uid, WorkContext.Sid);
            //将用户信息写入cookie中
            MallUtils.SetUserCookie(partUserInfo, 0);

            //写入无页面操作后注销Mark
            MallUtils.SetLoginTimeoutMark(MallUtils.GetSidCookie(), TypeHelper.StringToInt(WebConfigurationManager.AppSettings["NoActionLoginTimeOut"]));

            return AjaxResult("success", "登录成功");

        }

    }
}