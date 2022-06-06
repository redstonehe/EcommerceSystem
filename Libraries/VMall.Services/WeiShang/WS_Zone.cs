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
    /// zone : 地区管理
    /// </summary>
    public class WS_Zone : BaseRequest
    {
        /// <summary>
        /// 获取某个id对应的地区信息
        /// </summary>
        /// /openapi/zone/{id} 
        /// <returns></returns>
        public static string getZone(int zid)
        {
            APIDictionary Params = new APIDictionary();
            //Params.Add("power", 16);
            Params.Add("ExtUid", "1");
            //Params.Add("id", "price");
            string FromDirSale = WSclient.Execute(Params, string.Format("/openapi/zone/{0}",zid));
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
