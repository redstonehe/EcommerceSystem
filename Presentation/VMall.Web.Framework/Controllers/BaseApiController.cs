using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using VMall.Core;
using VMall.Services;
using System.Web.Script.Serialization;

namespace VMall.Web.Framework
{
    public class BaseApiController : Controller
    {
        public string appSid = "";
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            this.ValidateRequest = false;

            //获得用户唯一标示符sid
            appSid = MallUtils.GetSidCookie();
            if (appSid.Length == 0)
            {
                //生成sid
                appSid = Sessions.GenerateSid();
                //将sid保存到cookie中
                MallUtils.SetSidCookie(appSid);
            }
        }

        private readonly string TimeStamp = WebHelper.GetConfigSettings("TimeStamp");// ConfigurationManager.AppSettings["TimeStamp"];


        /// <summary>
        /// 验证入口
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            //前端请求api时会将token存放在名为"auth"的请求头中
            var authHeader = filterContext.RequestContext.HttpContext.Request.Headers["auth"];
            if (authHeader == null)
            {
                filterContext.Result =new EmptyResult();
                //filterContext.Result = new RedirectResult("/Error");
                //filterContext.HttpContext.Response.Redirect("/API/Home/Error");
            }
            //请求参数
            //string requestTime = httpContext.Request["rtime"]; //请求时间经过DESC签名
            //if (string.IsNullOrEmpty(requestTime))
            //    return false;


            //请求时间RSA解密后加上时间戳的时间即该请求的有效时间
            //DateTime Requestdt = DateTime.Parse(DESCryption.Decode(requestTime)).AddMinutes(int.Parse(TimeStamp));
            //DateTime Newdt = DateTime.Now; //服务器接收请求的当前时间
            //if (Requestdt < Newdt)
            //{
            //    return false;
            //}
            //else
            //{
            //进行其他操作
            //var userinfo = JwtHelp.GetJwtDecode(authHeader);
            //举个例子  生成jwtToken 存入redis中    
            //这个地方用jwtToken当作key 获取实体val   然后看看jwtToken根据redis是否一样
            //if (userinfo.UserName == "admin" && userinfo.Pwd == "123")
            //    return true;
            //}
            base.OnAuthorization(filterContext);
        }

        ///// <summary>
        ///// 验证核心代码
        ///// </summary>
        ///// <param name="httpContext"></param>
        ///// <returns></returns>
        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        //{

        //    //前端请求api时会将token存放在名为"auth"的请求头中
        //    var authHeader = httpContext.Request.Headers["auth"];
        //    if (authHeader == null)
        //        return false;

        //    //请求参数
        //    //string requestTime = httpContext.Request["rtime"]; //请求时间经过DESC签名
        //    //if (string.IsNullOrEmpty(requestTime))
        //    //    return false;


        //    //请求时间RSA解密后加上时间戳的时间即该请求的有效时间
        //    //DateTime Requestdt = DateTime.Parse(DESCryption.Decode(requestTime)).AddMinutes(int.Parse(TimeStamp));
        //    //DateTime Newdt = DateTime.Now; //服务器接收请求的当前时间
        //    //if (Requestdt < Newdt)
        //    //{
        //    //    return false;
        //    //}
        //    //else
        //    //{
        //    //进行其他操作
        //    var userinfo = JwtHelp.GetJwtDecode(authHeader);
        //    //举个例子  生成jwtToken 存入redis中    
        //    //这个地方用jwtToken当作key 获取实体val   然后看看jwtToken根据redis是否一样
        //    if (userinfo.UserName == "admin" && userinfo.Pwd == "123")
        //        return true;
        //    //}

        //    return false;
        //}


        /// <summary>
        /// ajax请求结果
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        protected ActionResult Json(object obj, JsonRequestBehavior behavior)
        {
            return base.Json(obj, behavior);
        }
        /// <summary>
        /// ajax请求结果
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        protected ActionResult AjaxResult(string state, string content)
        {
            return AjaxResult(state, content, false);
        }

        /// <summary>
        /// ajax请求结果
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="content">内容</param>
        /// <param name="isObject">是否为对象</param>
        /// <returns></returns>
        protected ActionResult AjaxResult(string state, string content, bool isObject)
        {
            return Content(string.Format("{0}\"state\":\"{1}\",\"content\":{2}{3}{4}{5}", "{", state, isObject ? "" : "\"", content, isObject ? "" : "\"", "}"));
        }

        /// <summary>
        /// 将对象转换成Json格式数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected ActionResult ConvertObject2Json(object obj)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
            string szJson = "";
            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, obj);
                szJson = Encoding.UTF8.GetString(stream.ToArray());
            }
            return Content(szJson);
        }

        /// <summary>
        /// App接口请求返回结果
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        protected ActionResult AppResult(bool state, string content, object data = null)
        {
            return base.Json(new { result = state ? 0 : -1, msg = content, info = data }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// api接口返回
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="result"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult WebApiResult(bool result, string msg, object data = null)
        {
            return Content(new JavaScriptSerializer().Serialize(new
            {
                result = result ? 0 : -1,
                msg = msg,
                info = data
            }));
        }
        /// <summary>
        /// api接口返回
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="result"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult WebApiResult(int result, string msg, object data)
        {
            return Content(new JavaScriptSerializer().Serialize(new
            {
                code = result,
                msg = msg,
                info = data
            }));
        }
        /// <summary>
        /// 解析Json数据转换成List集合
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        protected static T JsonToObject<T>(string Content)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(Content);
        }
        /// <summary>
        /// 解析List集合转换为Json字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected static string ObjectToJson<T>(T obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //JSON序列化  
            return serializer.Serialize(obj);
        }
    }
}
