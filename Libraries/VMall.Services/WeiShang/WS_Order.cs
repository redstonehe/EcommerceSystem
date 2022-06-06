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
    /// order : 订单管理
    /// </summary>
    public class WS_Order : BaseRequest
    {
        /// <summary>
        /// 订单提交后返回详情相关的vo
        /// </summary>
        /// /openapi/order/submit
        /// <returns></returns>
        public static string Submit(int oid, PartUserInfo user)
        {
            try
            {
                APIDictionary Params = new APIDictionary();
                Params.Add("ExtUid", user.Uid);

                Params.Add("skuIds", "");
                Params.Add("shopRemarks", "");
                Params.Add("skuId", "caksrrvb");
                Params.Add("qty", 1);
                Params.Add("proLiteralId", "5xfi5gm3");
                Params.Add("consignee", "测试");
                Params.Add("phone", "13555555555");
                Params.Add("zoneId", "2143");
                Params.Add("street", "测试路3号");
                //Params.Add("addressId", "1501");
                Params.Add("remark", "订单测试哈哈啊");
                string FromDirSale = WSclient.Execute(Params, "/openapi/order/submit");
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                string orderid = string.Empty;
                if (token["errorCode"].ToString() == "200")
                {
                    JToken order = token["data"].SelectToken("mainOrderVO").SelectToken("orders");
                    orderid = order.First().SelectToken("id").ToString();
                    //更新orderid 到order 的email字段 ordersource 41
                    LogHelper.WriteOperateLog("微商订单_success", "微商订单成功", "订单ID：" + oid + "，微商订单id：" + orderid);
                    Orders.UpdateWSOrder(oid, (int)OrderSource.微商系统, orderid);
                }
                else
                {
                    LogHelper.WriteOperateLog("微商订单_fail", "微商订单失败", "错误信息为：订单ID：" + oid + "，接口返回：" + FromDirSale);
                }
                return orderid;
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("微商订单_fail", "微商订单失败", "错误信息为：订单ID：" + oid + "，原因：" + ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据订单id获取订单详情
        /// </summary>
        /// /openapi/order/{id}
        /// <returns></returns>
        public static string GetOrder(int oid, PartUserInfo user)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", user.Uid);
            OrderInfo order = Orders.GetOrderByOid(oid);
            if (order != null)
                return "";
            Params.Add("id", order.ExtOrderId);

            string FromDirSale = WSclient.Execute(Params, string.Format("/openapi/order/{0}", oid));
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return FromDirSale;
        }

        /// <summary>
        /// 订单审核
        /// </summary>
        /// /openapi/api/order/audit
        /// <returns></returns>
        public static string OrderAudit(int oid, PartUserInfo user)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", user.Uid);
            OrderInfo order = Orders.GetOrderByOid(oid);
            if (order != null)
                return "";
            Params.Add("orderId", order.ExtOrderId);

            string FromDirSale = WSclient.Execute(Params, "/openapi/api/order/audit");
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return FromDirSale;
        }

        /// <summary>
        /// 根据订单id取消某订单
        /// </summary>
        /// /openapi/api/order/cancel
        /// <returns></returns>
        public static string OrderCancel(int oid,PartUserInfo user)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", user.Uid);
            OrderInfo order = Orders.GetOrderByOid(oid);
            if (order != null)
                return "";
            Params.Add("orderId", order.ExtOrderId);
            Params.Add("orderId", "1220170320142387403987");

            string FromDirSale = WSclient.Execute(Params, "/openapi/api/order/cancel");
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return FromDirSale;
        }

        /// <summary>
        /// 收货签收
        /// </summary>
        /// /openapi/api/order/confirmShipped
        /// <returns></returns>
        public static string ConfirmShipped(int oid)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", "1");

            Params.Add("orderId", "1220170320142387403987");

            string FromDirSale = WSclient.Execute(Params, "/openapi/api/order/confirmShipped");
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return FromDirSale;
        }
        
        /// <summary>
        /// 审核发货
        /// </summary>
        /// /openapi/order/auditAndShip
        /// <returns></returns>
        public static string AuditAndShip(int oid)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", "1");

            Params.Add("logisticsCompany", "1220170320142387403987");//物流公司
            Params.Add("logisticsOrderNo", "1220170320142387403987");//物流单号

            Params.Add("orderId", "1220170320142387403987");

            string FromDirSale = WSclient.Execute(Params, "/openapi/order/auditAndShip");
            //JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            //JToken token = (JToken)jsonObject;
            //if (token["Result"].ToString() == "0")
            //{
            //    FromDirSale = token["Info"].ToString();
            //}
            return FromDirSale;
        }

        /// <summary>
        /// 订单发货
        /// </summary>
        /// /openapi/order/shipped
        /// <returns></returns>
        public static string Shipped(int oid)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("ExtUid", "1");

            Params.Add("logisticsCompany", "1220170320142387403987");//物流公司
            Params.Add("logisticsOrderNo", "1220170320142387403987");//物流单号
            Params.Add("orderId", "1220170320142387403987");

            string FromDirSale = WSclient.Execute(Params, " /openapi/order/shipped");
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
