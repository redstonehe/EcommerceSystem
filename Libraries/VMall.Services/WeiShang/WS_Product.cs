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
    /// product : 商品管理
    /// </summary>
    public class WS_Product:BaseRequest
    {
        /// <summary>
        /// 获取所有在售商品列表
        /// </summary>
        /// /openapi/catalog/allProducts 
        /// <returns></returns>
        public static string ProductList()
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", "1");
            Params.Add("order", "price");
            Params.Add("direction", "desc");
            string FromDirSale = WSclient.Execute(Params, "/openapi/catalog/allProducts");
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return FromDirSale;
        }

        /// <summary>
        /// 查看商品详情
        /// </summary>
        /// /openapi/product/findById
        /// <param name="accountName"></param>
        /// <returns></returns>
        public static string findById(string productId)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", "1");

            Params.Add("productId", productId);
            string FromDirSale = WSclient.Execute(Params, "/openapi/product/findById");
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
