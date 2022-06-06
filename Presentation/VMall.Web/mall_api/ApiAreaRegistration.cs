using System;

namespace VMall.Web.Api
{
    public class ApiAreaRegistration : System.Web.Mvc.AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Api";
            }
        }

        public override void RegisterArea(System.Web.Mvc.AreaRegistrationContext context)
        {

            //context.MapRoute(
            //                    "MYAPI_default",
            //                  "MYAPI/{controller}/{action}/{id}",
            //                  new { controller = "home", action = "index", area = "MYAPI" },
            //                  new[] { "VMall.Web.Api.Controllers" });

            // 默认路由 此路由不能删除
            //context.MapRoute(
            //    "Api_default",
            //    "Api/{controller}/{action}/{id}",
            //    new { controller = "home", action = "index", id = "", area = "Api" },
            //    new[] { "VMall.Web.Api.Controllers" });

            // 默认路由 此路由不能删除
            //context.MapRoute(
            //    "Api_default2",
            //    "Api/{controller}/{action}",
            //    new { action = "index", area = "Api" },
            //    new[] { "VMall.Web.Api.Controllers" });
        }
    }
}
