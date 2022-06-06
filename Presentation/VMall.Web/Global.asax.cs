using log4net;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static ILog logger = LogManager.GetLogger(typeof(MvcApplication));
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            //这里是area
            //Route route = routes.MapRoute(
            //    "Api_default",
            //    "Api/{controller}/{action}/{id}",
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            //    new string[] { "VMall.Web.Api.Controllers" }
            //);
           // Route route = routes.MapRoute(
           //    "Api_default",
           //    "Api/{controller}/{action}",
           //    new { controller = "Home", action = "Index", area = "Api" },
           //    new string[] { "VMall.Web.Api.Controllers" }
           //);
            //route.DataTokens["area"] = "Api";
            //这里是area

            /*使用二级域名m.xxx.com时候配置该路由*/
            //routes.Add("DomainRoute", new DomainRoute(
            //    "{area}.zhwlmall.com",                            // Domain with parameters
            //    "{controller}/{action}/{id}",                   // URL with parameters
            //    new
            //    {
            //        area = "m",
            //        Namespaces = new string[] { "VMall.Web.Mobile.Controllers" },
            //        controller = "Home",
            //        action = "Index",
            //        id = ""
            //    }  // Parameter defaults
            //));

            /*使用二级域名m.xxx.com时候配置该路由*/
            //routes.Add("DomainRoute", new DomainRoute(
            //    "{area}.zhongheweilian.com",                            // Domain with parameters
            //    "{controller}/{action}/{id}",                   // URL with parameters
            //    new
            //    {
            //        area = "Api",
            //        Namespaces = new string[] { "VMall.Web.Areas.WebApi.Controllers" },
            //        controller = "Home",
            //        action = "Index",
            //        id = ""
            //    }  // Parameter defaults
            //));

            //routes.MapRoute(
            //    "Api_default",
            //    "Api/{controller}/{action}",
            //    new { controller = "home", action = "index", area = "Api" },
            //    null,
            //    new string[] { "VMall.Web.Api.Controllers" });

            //// 默认路由 此路由不能删除
            //routes.MapRoute("Api_default2",
            //                 "Api/{controller}/{action}",
            //                  new { action = "index", area = "Api" },
            //                  null,
            //                  new string [] { "VMall.Web.Api.Controllers" });
            //商品路由
            routes.MapRoute("product",
                            "product/{pid}.html",
                            new { controller = "catalog", action = "product" },
                            new[] { "VMall.Web.Controllers" });
            //分类路由
            routes.MapRoute("category",
                            "list/{filterAttr}-{cateId}-{brandId}-{filterPrice}-{onlyStock}-{sortColumn}-{sortDirection}-{page}.html",
                            new { controller = "catalog", action = "category" },
                            new[] { "VMall.Web.Controllers" });
            //分类路由
            routes.MapRoute("shortcategory",
                            "list/{cateId}.html",
                            new { controller = "catalog", action = "category" },
                            new[] { "VMall.Web.Controllers" });
            //频道路由
            routes.MapRoute("channel",
                            "channel/{chId}-{gid}-{cateId}-{brandId}-{filterPrice}-{onlyStock}-{sortColumn}-{sortDirection}-{page}.html",
                            new { controller = "channel", action = "index" },
                            new[] { "VMall.Web.Controllers" });
            //频道路由
            routes.MapRoute("shortchannel",
                            "channel/{chId}.html",
                            new { controller = "channel", action = "index" },
                            new[] { "VMall.Web.Controllers" });
            //商城搜索路由
            routes.MapRoute("mallsearch",
                            "search",
                            new { controller = "catalog", action = "search" },
                            new[] { "VMall.Web.Controllers" });
            //店铺路由
            routes.MapRoute("store",
                            "store/{storeId}.html",
                            new { controller = "store", action = "index" },
                            new[] { "VMall.Web.Controllers" });
            //店铺分类路由
            routes.MapRoute("storeclass",
                            "storeclass/{storeId}-{storeCid}-{startPrice}-{endPrice}-{sortColumn}-{sortDirection}-{page}.html",
                            new { controller = "store", action = "class" },
                            new[] { "VMall.Web.Controllers" });
            //店铺分类路由
            routes.MapRoute("shortstoreclass",
                            "storeclass/{storeId}-{storeCid}.html",
                            new { controller = "store", action = "class" },
                            new[] { "VMall.Web.Controllers" });
            //店铺搜索路由
            routes.MapRoute("storesearch",
                            "searchstore",
                            new { controller = "store", action = "search" },
                            new[] { "VMall.Web.Controllers" });
            //默认路由(此路由不能删除)
            routes.MapRoute("default",
                            "{controller}/{action}",
                            new { controller = "home", action = "index" },
                            new[] { "VMall.Web.Controllers" });
        }

        protected void Application_Start()
        {
            //将默认视图引擎替换为ThemeRazorViewEngine引擎
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ThemeRazorViewEngine());

            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            
            //路由测试
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);

            //Exception ex = Server.GetLastError().GetBaseException(); //获取异常源 

            //启动事件机制
            BMAEvent.Start();
            //服务器宕机启动后重置在线用户表
            if (Environment.TickCount > 0 && Environment.TickCount < 900000)
                OnlineUsers.ResetOnlineUserTable();
        }
        /// <summary>
        ///  程序出错时通过lognet写日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception objExp = HttpContext.Current.Server.GetLastError();
            logger.Error("程序发生未捕获异常", HttpContext.Current.Error);
            //LogHelper.ErrorLog("\r\n客户机IP:" + Request.UserHostAddress + "\r\n错误地址:" + Request.Url
            //    + "\r\n异常信息:" + Server.GetLastError().Message, objExp);
        }
    }
}