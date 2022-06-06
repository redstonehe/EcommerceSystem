using System;

namespace VMall.Web.Mobile
{
    public class AreaRegistration : System.Web.Mvc.AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "mob";
            }
        }

        public override void RegisterArea(System.Web.Mvc.AreaRegistrationContext context)
        {
            /*使用二级域名m.xxx.com时候不需要注册Area 同时主站路由增加二级域名路由配置来注册Area区域（利用DomainRoute）*/
            /*现使用www.xxx.com/mob 的形式不要 注册area*/
           
            //商品路由
            context.MapRoute("moblie_product",
                            "mob/product/{pid}.html",
                            new { controller = "catalog", action = "product", area = "mob" },
                            new[] { "VMall.Web.Mobile.Controllers" });

            //分类路由--条件筛选
            context.MapRoute("moblie_category",
                            "mob/list/{filterAttr}-{cateId}-{brandId}-{filterPrice}-{onlyStock}-{sortColumn}-{sortDirection}-{page}.html",
                            new { controller = "catalog", action = "list", area = "mob" },
                            new[] { "VMall.Web.Mobile.Controllers" });
            //分类路由
            context.MapRoute("moblie_shortcategory",
                            "mob/list/{cateId}.html",
                            new { controller = "catalog", action = "list", area = "mob" },
                            new[] { "VMall.Web.Mobile.Controllers" });

            //频道路由-筛选
            context.MapRoute("moblie_channel",
                            "mob/channel/{chId}-{gid}-{cateId}-{brandId}-{filterPrice}-{onlyStock}-{sortColumn}-{sortDirection}-{page}.html",
                            new { controller = "channel", action = "index", area = "mob" },
                            new[] { "VMall.Web.Mobile.Controllers" });
            //商城404路由
            context.MapRoute("moblie_error",
                            "mob/error.html",
                            new { controller = "error", action = "index", area = "mob" },
                            new[] { "VMall.Web.Mobile.Controllers" });
            //频道路由
            context.MapRoute("moblie_shortchannel",
                            "mob/channel/{chId}.html",
                            new { controller = "channel", action = "index", area = "mob" },
                            new[] { "VMall.Web.Mobile.Controllers" });

            // 默认路由 此路由不能删除
            context.MapRoute("mobile_default",
                             "mob/{controller}/{action}",
                              new { controller = "home", action = "index", area = "mob" },
                              new[] { "VMall.Web.Mobile.Controllers" });
            
        }
    }
}
