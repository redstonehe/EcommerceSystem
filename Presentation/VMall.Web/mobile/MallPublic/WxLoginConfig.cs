using System;
using System.Collections.Generic;
using System.Web;

namespace VMall.Web.Mobile
{
    /**
    * 	配置账号信息
    */
    public class WxLoginConfig
    {
        //=======【基本信息设置】=====================================
        /* 微信公众号信息配置 用于绑定微信
        * APPID：绑定支付的APPID（必须配置）
        * MCHID：商户号（必须配置）
        * APPSECRET：公众帐号secert（仅JSAPI支付的时候需要配置）
        */
        public const string APPID = "wx4ba227d36b5bbf42";
        public const string APPSECRET = "4b33b436405a34b1f6a42afb180e9144";
        public const string MCHID = "1537367451";

    }
}