using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMall.Web
{
    using System.Web.Configuration;

    public class WebSiteConfig
    {

        /// <summary>
        /// 直销系统api地址
        /// </summary>
        public static string DirsaleApiUrl
        {
            get { return WebConfigurationManager.AppSettings["DirsaleApiUrl"]; }
        }

        /// <summary>
        /// 天鹰网接口密钥
        /// </summary>
        public static string TYappkey
        {
            get { return WebConfigurationManager.AppSettings["TYappkey"]; }
        }

        /// <summary>
        /// 海之圣旗舰店店铺id
        /// </summary>
        public static string HealthenStoreId
        {
            get { return WebConfigurationManager.AppSettings["HealthenStoreId"]; }
        }

        /// <summary>
        /// 全球购店铺id
        /// </summary>
        public static string QuanQiuGouStoreId
        {
            get { return WebConfigurationManager.AppSettings["QuanQiuGouStoreId"]; }
        }

        /// <summary>
        /// 无页面操作后注销登陆时间
        /// </summary>
        public static string NoActionLoginTimeOut
        {
            get { return WebConfigurationManager.AppSettings["NoActionLoginTimeOut"]; }
        }

        /// <summary>
        /// 测试购买账号
        /// </summary>
        public static string TestUidList
        {
            get { return WebConfigurationManager.AppSettings["TestUidList"]; }
        }

        /// <summary>
        /// 样式、图片、脚本资源存放地址
        /// </summary>
        public static string ResourcesPortal
        {
            get { return WebConfigurationManager.AppSettings["ResourcesPortal"]; }
        }

        /// <summary>
        /// 总站
        /// </summary>
        public static string MallPortal
        {
            get { return WebConfigurationManager.AppSettings["HHWPortal"]; }
        }

        /// <summary>
        /// 会员入口
        /// </summary>
        public static string MemberPortal
        {
            get { return WebConfigurationManager.AppSettings["MemberPortal"]; }
        }


        /// <summary>
        /// 图片存放地址
        /// </summary>
        public static string ProdImage
        {
            get { return WebConfigurationManager.AppSettings["ProdImage"]; }
        }

        /// <summary>
        /// 支付站地址
        /// </summary>
        public static string payPortal
        {
            get { return WebConfigurationManager.AppSettings["PayPortal"]; }
        }

        /// <summary>
        /// 推广产品
        /// </summary>
        public static string ActiveProduct
        {
            get { return WebConfigurationManager.AppSettings["ActiveProduct"]; }
        }

        /// <summary>
        /// 汇购卡券Pid
        /// </summary>
        public static string CoffeeQuanPid
        {
            get { return WebConfigurationManager.AppSettings["CoffeeQuanPid"]; }
        }
    }
}