using api.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.Http;
using VMall.Web.Areas.WebApi.Models;
using VMall.Core;

namespace api.Controllers
{
    public class LoginController : ApiController
    {
        public LoginResult Post([FromBody]LoginRequest request)
        {
            LoginResult rs = new LoginResult();
            //假设用户名为"admin"，密码为"123"
            if (request.UserName == "admin" && request.Password == "123")
            {
                //如果用户登录成功，则可以得到该用户的身份数据。当然实际开发中，这里需要在数据库中获得该用户的角色及权限
                AuthInfo authInfo = new AuthInfo
                {
                    IsAdmin = true,
                    Roles = new List<string> { "admin", "owner" },
                    UserName = "admin"
                };
                try
                {
                    //生成token,SecureKey是配置的web.config中，用于加密token的key，打死也不能告诉别人
                    byte[] key = Encoding.Default.GetBytes(WebHelper.GetConfigSettings("SecureKey"));
                    //采用HS256加密算法
                    string token = JWT.JsonWebToken.Encode(authInfo, key, JWT.JwtHashAlgorithm.HS256);
                    rs.Token = token;
                    rs.Success = true;

                }
                catch
                {
                    rs.Success = false;
                    rs.Message = "登陆失败";
                }
            }
            else
            {
                rs.Success = false;
                rs.Message = "用户名或密码不正确";
            }
            return rs;
        }
    }
}
