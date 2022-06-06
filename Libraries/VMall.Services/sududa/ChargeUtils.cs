using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VMall.Core;
using VMall.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;

using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using VMall.Services.DirSale;
using System.Web.Script.Serialization;

namespace VMall.Services
{
    public class ChargeUtils
    {
        private static object ctx = new object();//锁对象

        public static SDDAPIClient SDDclient = new SDDAPIClient(WebHelper.GetConfigSettings("SuDuDaAPIUrl"), WebHelper.GetConfigSettings("SuDuDaAppID"), WebHelper.GetConfigSettings("SuDuDaAppKey"));

        /// <summary>
        /// 获取商品进价表
        /// </summary>

        /// <returns></returns>
        public static string ProductList()
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("power", 16);
            string FromDirSale = SDDclient.Execute(Params, "/api/product", 1);
            JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            JToken token = (JToken)jsonObject;
            if (token["Result"].ToString() == "0")
            {
                FromDirSale = token["Info"].ToString();
            }
            return FromDirSale;
        }

        /// <summary>
        /// 手机号码归属地
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public static string sys_phone(string mobile)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("phone", mobile);
            string FromDirSale = SDDclient.Execute(Params, "/api/sys_phone", 1);
            JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            JToken token = (JToken)jsonObject;

            string type = "";
            string city = "";
            string area = "";
            if (token["sududa"].SelectToken("status").ToString() == "1")
            {
                type = token["sududa"].SelectToken("type").ToString();
                city = token["sududa"].SelectToken("city").ToString();
                area = token["sududa"].SelectToken("area").ToString();
            }
            return type + "-" + city + "-" + area;
        }

        /// <summary>
        /// 在线充值
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static bool recharge(string orderid, int productid, string to, string area, int count, string ip, out string tips)
        {
            try
            {
                tips = "";
                //return false;
                APIDictionary Params = new APIDictionary();
                Params.Add("orderid", orderid);
                Params.Add("productid", productid);
                Params.Add("to", to);
                Params.Add("area", area);
                Params.Add("count", count);
                Params.Add("ip", ip);

                string FromDirSale = SDDclient.Execute(Params, "/api/recharge", 2);
                LogHelper.WriteOperateLog("充值请求参数", "充值参数列表", "订单号：" + orderid + "，产品id：" + productid + ",充值号码：" + to + "，地区id：" + area + "，返回信息：" + FromDirSale);
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                string status = token["sududa"].SelectToken("status").ToString();
                if (status != "-9")
                {
                    decimal balance = TypeHelper.ObjectToDecimal(token["sududa"].SelectToken("balance"));
                    tips = token["sududa"].SelectToken("tips").ToString();
                    LogHelper.WriteOperateLog("SDDRechargeSuccess", "速度达充值提交成功", "，订单号：" + orderid + ",状态：" + status + ",状态提示：" + tips + ",用户余额：" + balance);
                    return true;
                }
                else
                {
                    LogHelper.WriteOperateLog("SDDRechargeFail", "速度达充值失败", "接口调用返回信息：状态：" + status + ",状态提示：" + tips + "，错误订单号：" + orderid);
                    tips = token["sududa"].SelectToken("tips").ToString();
                    return false;
                }
            }
            catch (Exception ex) {
                LogHelper.WriteOperateLog("SDDRechargeError", "速度达充值异常", "请求异常，错误订单号" + orderid + "，异常信息：" + ex.Message);
                tips = "请求异常";
                return false;
            }

        }



    }
}
