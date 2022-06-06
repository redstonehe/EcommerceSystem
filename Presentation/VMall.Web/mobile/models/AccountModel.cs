using System;
using System.Collections.Generic;

using VMall.Core;

namespace VMall.Web.Mobile.Models
{
    /// <summary>
    /// 登陆模型类
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// 返回地址
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 影子账号名
        /// </summary>
        public string ShadowName { get; set; }
        /// <summary>
        /// 是否允许记住用户
        /// </summary>
        public bool IsRemember { get; set; }
        /// <summary>
        /// 是否启用验证码
        /// </summary>
        public bool IsVerifyCode { get; set; }
        /// <summary>
        /// 开放授权插件
        /// </summary>
        public List<PluginInfo> OAuthPluginList { get; set; }
    }

    /// <summary>
    /// 注册模型类
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// 返回地址
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 影子账号名
        /// </summary>
        public string ShadowName { get; set; }
        /// <summary>
        /// 是否启用验证码
        /// </summary>
        public bool IsVerifyCode { get; set; }
    }

    /// <summary>
    /// 找回密码模型类
    /// </summary>
    public class FindPwdModel
    {
        /// <summary>
        /// 影子账号名
        /// </summary>
        public string ShadowName { get; set; }
        /// <summary>
        /// 是否启用验证码
        /// </summary>
        public bool IsVerifyCode { get; set; }
    }

    /// <summary>
    /// 选择找回密码方式模型类
    /// </summary>
    public class SelectFindPwdTypeModel
    {
        public PartUserInfo PartUserInfo { get; set; }
    }

    /// <summary>
    /// 重置密码模型类
    /// </summary>
    public class ResetPwdModel
    {
        public string V { get; set; }
    }
    /// <summary>
    /// 微信授权类
    /// </summary>
    public class AccessModels
    {
        public string errcode { get; set; }
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }
        public string unionid { get; set; }

    }
    /// <summary>
    /// 微信用户资料
    /// </summary>
    public class WXUserInfosModels
    {
        public string errcode { get; set; }
        public string openid { get; set; }
        public string nickname { get; set; }
        public string sex { get; set; }

        public string province { get; set; }
        public string city { get; set; }

        public string country { get; set; }
        public string headimgurl { get; set; }
        public string privilege { get; set; }
        public string unionid { get; set; }

    }
}