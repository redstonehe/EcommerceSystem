using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VMall.Core;
using VMall.Services;

using System.Drawing;

using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using VMall.Services.DirSale;
using System.Web.Script.Serialization;

namespace VMall.Services.WeiShang
{
    /// <summary>
    /// user : 用户管理
    /// </summary>
    public class WS_User : BaseRequest
    {
        /// <summary>
        /// 获得我的代理用户信息
        /// </summary>
        /// /openapi/shop/user/userInfo
        /// <returns></returns>
        public static string userInfo()
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", "1");

            Params.Add("userId", 1);

            string FromDirSale = WSclient.Execute(Params, "/openapi/shop/user/userInfo");
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return FromDirSale;
        }

        /// <summary>
        /// 更新代理用户头像，名称等个人信息
        /// </summary>
        /// /openapi/shop/update 
        /// <returns></returns>
        public static string update()
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", "1");

            Params.Add("id", "price");
            Params.Add("name", "desc");
            Params.Add("img", "price");
            Params.Add("description", "price");
            Params.Add("wechat", "price");
            Params.Add("banner", "price");

            string FromDirSale = WSclient.Execute(Params, "/openapi/shop/update/openapi/shop/update");
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return FromDirSale;
        }

        /// <summary>
        /// 获得用户当前状态
        /// </summary>
        /// /openapi/getUserType 
        /// <returns></returns>
        public static string getUserType(PartUserInfo user)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", user.Uid);

            string FromDirSale = WSclient.Execute(Params, "/openapi/getUserType");
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return FromDirSale;
        }
    }
}
