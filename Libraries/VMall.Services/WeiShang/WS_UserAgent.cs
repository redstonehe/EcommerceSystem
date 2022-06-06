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
    /// agent : 代理管理
    /// </summary>
    public class WS_UserAgent : BaseRequest
    {
        /// <summary>
        /// 代理申请
        /// </summary>
        /// /openapi/userAgent/apply4Admin
        /// <returns></returns>
        public static string apply4Admin(string parentPhone, string agentType, int areaid, string name, string weixin, string phone, string idcard, string address,PartUserInfo user)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", user.Uid);

            Params.Add("parentPhone", parentPhone);
            Params.Add("parentId", "");
            Params.Add("agentType", agentType);
            Params.Add("district", areaid);
            Params.Add("name", name);
            Params.Add("weixin", weixin);
            Params.Add("phone", phone);

            Params.Add("email", "");
            Params.Add("idcard", idcard);
            Params.Add("address", address);
            
            string Result = WSclient.Execute(Params, "/openapi/userAgent/apply4Admin");
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return Result;
        }
        
        /// <summary>
        /// 获取代理用户角色信息
        /// </summary>
        /// /openapi/userAgent/getRoles
        /// <param name="pName"></param>
        /// <returns></returns>
        public static string getRoles()
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", "1");

            string Result = WSclient.Execute(Params, "/openapi/userAgent/getRoles");
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return Result;

        }

        /// <summary>
        /// 得到所有代理用户信息
        /// </summary>
        /// /openapi/userAgent/getAllUsers
        /// <param name="pName"></param>
        /// <returns></returns>
        public static string getAllUsers()
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", "1");

            string Result = WSclient.Execute(Params, "/openapi/userAgent/getAllUsers");
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return Result;

        }
        /// <summary>
        /// 根据用户id查看代理用户明细信息
        /// </summary>
        /// /openapi/userAgent/userId/{id}
        /// <param name="pName"></param>
        /// <returns></returns>
        public static string GetAgentUserById(string id)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", "1");
            //Params.Add("id", "1");
            string Result = WSclient.Execute(Params, string.Format("/openapi/userAgent/userId/{0}",id));
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return Result;

        }
        
    }
}
