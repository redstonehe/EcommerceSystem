using api.Models;
using api.Secure;
using System.Web.Http;
using VMall.Web.Areas.WebApi.Models;
using System.Web.Http;
namespace api.Controllers
{
    //标记该controller要求身份验证
    [ApiAuthorize]
    public class UserController : ApiController
    {
        public string Get()
        {
            //获取回用户信息(在ApiAuthorize中通过解析token的payload并保存在RouteData中)
            AuthInfo authInfo =  this.RequestContext.RouteData.Values["auth"] as AuthInfo;
            if (authInfo == null)
                return "无效的验收信息";
            else
                return string.Format("你好:{0},成功取得数据",authInfo.UserName);
        }
    }
}
