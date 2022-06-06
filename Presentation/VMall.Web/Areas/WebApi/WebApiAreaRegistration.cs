using System.Web.Mvc;

namespace VMall.Web.Areas.WebApi
{
    public class WebApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WebApi";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            /*使用二级域名m.xxx.com时候不需要注册Area 同时主站路由增加二级域名路由配置来注册Area区域（利用DomainRoute）*/
            /*现使用www.xxx.com/ 的形式不要 注册area*/
            /*现使用webapi.xxx.com 的形式 不要注册area  20181126修改  IIS需要配置绑定 webapi.xxx.com*/
            context.MapRoute(
                "WebApi_Restful",
                "WebApi/{controller}/{id}",
                new {  id = UrlParameter.Optional }
            );
            context.MapRoute(
                "WebApi_default",
                "WebApi/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
