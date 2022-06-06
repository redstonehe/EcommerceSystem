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
    /// bank : 收益管理
    /// </summary>
    public class WS_Bank : BaseRequest
    {
        /// <summary>
        /// 我的收益查询
        /// </summary>
        /// /openapi/userBank/withdrawAll/2b
        /// <returns></returns>
        public static string withdrawAll(PartUserInfo user)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", user.Uid);
            
            string FromDirSale = WSclient.Execute(Params, "/openapi/userBank/withdrawAll/2b");
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return FromDirSale;
        }

        /// <summary>
        /// 待收收益列表
        /// </summary>
        /// /openapi/userBank/order/list/2b
        /// <returns></returns>
        public static string withoutdrawList(PartUserInfo user)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", user.Uid);

            Params.Add("showAll", true);

            string FromDirSale = WSclient.Execute(Params, "/openapi/userBank/order/list/2b");
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
