using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HuiGouMall.Web.payment.listen
{
    using Com.LaKaLa;
    using System.Web.Configuration;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using HuiGouMall.Services;
    using HuiGouMall.Core;
    public partial class PayReturnNotify : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> retMap = new Dictionary<string, string>();
            retMap.Add("retCode", CrossBorderConstant.ERROR_0001);
            retMap.Add("retMsg", CrossBorderConstant.ERROR_0001_MSG);
            try
            {
                // 1. 接收平台请求
                string reqJson = string.Empty;
                Dictionary<string, string> jsonMap = new Dictionary<string, string>();
                bool isHasData = false;
                if (Request.HttpMethod.Equals("GET"))
                {
                    //GET请求
                    var paramCollection = Request.QueryString;
                    foreach (string key in paramCollection)
                    {
                        jsonMap.Add(key, paramCollection.Get(key));
                        isHasData = true;
                    }
                }
                else
                {
                    //POST请求
                    byte[] paramByte = Request.BinaryRead(Request.TotalBytes);
                    reqJson = Encoding.UTF8.GetString(paramByte);
                    JObject jsonObject = (JObject)JsonConvert.DeserializeObject(reqJson);
                    JToken token = (JToken)jsonObject;
                    foreach (JProperty p in token)
                    {
                        if (jsonMap.ContainsKey(p.Name))
                        {
                            jsonMap.Remove(p.Name);
                        }
                        jsonMap.Add(p.Name, p.Value.ToString());
                        isHasData = true;
                    }
                }
                if (!isHasData)
                {
                    retMap.Add("retCode", CrossBorderConstant.ERROR_0002);
                    retMap.Add("retMsg", CrossBorderConstant.ERROR_0002_MSG);

                    Response.Write(JsonConvert.SerializeObject(retMap));
                    Response.Flush();

                    Session["_return_notify_retMap_"] = retMap;
                    Server.Transfer("/payment/Error.aspx");
                }

                // 2. 获取商户私钥，平台公钥，验证签名，解密
                if (jsonMap == null || !jsonMap.ContainsKey("reqType") || !jsonMap.ContainsKey("merId")
                    || !jsonMap.ContainsKey("ts") || !jsonMap.ContainsKey("ver") || !jsonMap.ContainsKey("encKey")
                    || !jsonMap.ContainsKey("encData") || !jsonMap.ContainsKey("mac"))
                {
                    //参数上送错误
                    retMap.Remove("retCode");
                    retMap.Remove("retMsg");
                    retMap.Add("retCode", CrossBorderConstant.ERROR_0002);
                    retMap.Add("retMsg", CrossBorderConstant.ERROR_0002_MSG);

                    Response.Write(JsonConvert.SerializeObject(retMap));
                    Response.Flush();

                    Session["_return_notify_retMap_"] = retMap;
                    Server.Transfer("/payment/Error.aspx");
                }

                string merId = string.Empty;
                jsonMap.TryGetValue("merId", out merId);
                if (merId == string.Empty)
                {
                    //商户号为空
                    retMap.Remove("retCode");
                    retMap.Remove("retMsg");
                    retMap.Add("retCode", CrossBorderConstant.ERROR_0002);
                    retMap.Add("retMsg", CrossBorderConstant.ERROR_0002_MSG);

                    Response.Write(JsonConvert.SerializeObject(retMap));
                    Response.Flush();

                    Session["_return_notify_retMap_"] = retMap;
                    Server.Transfer("/payment/Error.aspx");
                }
                //获取密钥
                string platPublicKey = WebConfigurationManager.AppSettings["pingtaiPublicKey"];            //拉卡拉平台公钥
                string merPrivateKey = WebConfigurationManager.AppSettings["merPrivateKey" + "." + merId];            //商户私钥

                if (null == platPublicKey || platPublicKey == string.Empty)
                {
                    //没有配置平台公钥
                    retMap.Remove("retCode");
                    retMap.Remove("retMsg");
                    retMap.Add("retCode", CrossBorderConstant.ERROR_0001);
                    retMap.Add("retMsg", CrossBorderConstant.ERROR_0001_MSG);

                    Response.Write(JsonConvert.SerializeObject(retMap));
                    Response.Flush();

                    return;
                }

                // 解密、验证mac
                retMap = decryptReqData(jsonMap, platPublicKey, merPrivateKey);
                string retCode = string.Empty;
                retMap.TryGetValue("retCode", out retCode);
                if (!CrossBorderConstant.SUCCESS.Equals(retCode))
                {
                    //失败
                    retMap.Remove("retCode");
                    retMap.Remove("retMsg");
                    retMap.Add("retCode", CrossBorderConstant.ERROR_0001);
                    retMap.Add("retMsg", CrossBorderConstant.ERROR_0001_MSG);

                    Response.Write(JsonConvert.SerializeObject(retMap));
                    Response.Flush();

                    return;
                }
                //解密验签成功
                string reqData = string.Empty;
                retMap.TryGetValue("reqData", out reqData);

                //解析json
                Dictionary<string, string> paramsData = new Dictionary<string, string>();
                JObject jObject = (JObject)JsonConvert.DeserializeObject(reqData);
                JToken jToken = (JToken)jObject;
                foreach (JProperty p in jToken)
                {
                    if (paramsData.ContainsKey(p.Name))
                    {
                        paramsData.Remove(p.Name);
                    }
                    paramsData.Add(p.Name, p.Value.ToString());
                }
                Dictionary<string, string> map = new Dictionary<string, string>();
                string merOrderId = string.Empty;
                string transactionId = string.Empty;
                paramsData.TryGetValue("merOrderId", out merOrderId);
                paramsData.TryGetValue("transactionId", out transactionId);
                map.Add("merOrderId", merOrderId);
                map.Add("transactionId", transactionId);

                string resData = JsonConvert.SerializeObject(map);
                string merKey = string.Empty;
                retMap.TryGetValue("merKey", out merKey);
                // 5. 签名，加密
                Dictionary<string, string> resMap = encryRetData(resData, jsonMap, merKey, merPrivateKey);
                // 6. 返回响应
                //处理返回信息  更新订单状态 支付单号 支付时间 merOrderId
                //OrderInfo orderInfo = Orders.GetOrderByOSN(resMap["merOrderId"]);
                //if (orderInfo == null)
                //{
                //    Server.Transfer("/payment/Error.aspx");
                //}
                //Logs.WriteOperateLog("LaKaLaPaylog", "拉卡拉支付前后台修改状态记录", "修改信息");
                //Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, resMap["transactionId"], DateTime.Now);
                //Logs.WriteOperateLog("LaKaLaPaylog", "拉卡拉支付后台修改状态记录", "修改信息为：订单号：" + resMap["merOrderId"] + "交易号" + resMap["transactionId"] + "交易状态" + OrderState.Confirming);
                //记录返回日志
                //HuiGouMall.Core.Logs.WriteOperateLog("LaKaLaPayReturnNotify", "拉卡拉支付后台回调通知记录", "回调返回信息为：" + JsonConvert.SerializeObject(resMap));
                
                //Response.Write(JsonConvert.SerializeObject(resMap));

                

                //Response.Flush();
                return;
            }
            catch (Exception err)
            {
                //系统异常
                Response.Write(JsonConvert.SerializeObject(retMap));
                Response.Flush();
                return;
            }

        }

        private Dictionary<String, String> encryRetData(string resJsonData, Dictionary<String, String> reqMap, string merDesStr, string merPrivateKey)
        {
            string retCode = CrossBorderConstant.SUCCESS;
            string retMsg = CrossBorderConstant.SUCCESS_MSG;

            string ts = string.Empty;
            string ver = string.Empty;
            string merId = string.Empty;
            string reqType = string.Empty;
            reqMap.TryGetValue("ts", out ts);
            reqMap.TryGetValue("ver", out ver);
            reqMap.TryGetValue("merId", out merId);
            reqMap.TryGetValue("reqType", out reqType);
            Dictionary<String, String> jsonMap = new Dictionary<String, String>();
            jsonMap.Add("merId", merId);
            jsonMap.Add("ver", ver);
            jsonMap.Add("ts", ts);
            jsonMap.Add("reqType", reqType);

            if (resJsonData == string.Empty || ts == string.Empty || ver == string.Empty
                || reqType == string.Empty || merDesStr == string.Empty)
            {
                jsonMap.Add("retCode", CrossBorderConstant.ERROR_0002);
                jsonMap.Add("retMsg", CrossBorderConstant.ERROR_0002_MSG);
                return jsonMap;
            }
            Dictionary<String, String> resMap_tmp = new Dictionary<String, String>();
            JObject jObject = (JObject)JsonConvert.DeserializeObject(resJsonData);
            JToken jToken = (JToken)jObject;
            foreach (JProperty p in jToken)
            {
                if (resMap_tmp.ContainsKey(p.Name))
                {
                    resMap_tmp.Remove(p.Name);
                }
                resMap_tmp.Add(p.Name, p.Value.ToString());
            }

            if (resMap_tmp != null && resMap_tmp.ContainsKey("retCode"))
            {
                resMap_tmp.TryGetValue("retCode", out retCode);
                resMap_tmp.TryGetValue("retMsg", out retMsg);
                if (null == retCode || retCode == string.Empty || "".Equals(retCode))
                {
                    retCode = CrossBorderConstant.SUCCESS;
                    retMsg = CrossBorderConstant.SUCCESS_MSG;
                }
            }
            resMap_tmp.Remove("retCode");
            resMap_tmp.Remove("retMsg");
            resJsonData = JsonConvert.SerializeObject(resMap_tmp);

            // 1.使用用户上送的对称密钥加密响应业务JSON，生成加密JSON2
            string encData = Tools.ToHexString(DESCrypto.Encrypt(Encoding.UTF8.GetBytes(resJsonData), merDesStr));

            // 2.拼接时间戳、业务类型、加密JSON2 做SHA1，响应方私钥加密，HEX，得MAC
            string mac_tmp = Tools.getSHA1(ts + reqType + encData);
            string mac = Tools.ToHexString(RSAUtil.EncryptByPrivateKey(Encoding.UTF8.GetBytes(mac_tmp), merPrivateKey));
            if (mac == string.Empty)
            {
                //加密返回数据失败
                jsonMap.Add("retCode", CrossBorderConstant.ERROR_0012);
                jsonMap.Add("retMsg", CrossBorderConstant.ERROR_0012_MSG);
                return jsonMap;
            }
            jsonMap.Add("encData", encData);
            jsonMap.Add("mac", mac);
            jsonMap.Add("retCode", retCode);
            jsonMap.Add("retMsg", retMsg);
            return jsonMap;
        }

        private Dictionary<String, String> decryptReqData(Dictionary<String, String> reqParams, string platPublicKey, string merPrivateKey)
        {
            string ts = string.Empty;
            string reqType = string.Empty;
            string reqEncKey = string.Empty;
            string reqEncData = string.Empty;
            string reqMac = string.Empty;

            reqParams.TryGetValue("ts", out ts);
            reqParams.TryGetValue("reqType", out reqType);
            reqParams.TryGetValue("encKey", out reqEncKey);
            reqParams.TryGetValue("encData", out reqEncData);
            reqParams.TryGetValue("mac", out reqMac);

            //用请求方公钥验签（拼接时间戳、业务类型、“加密json1”做SHA1，用请求方公钥解密mac反hex的字节数组，比较是否一致）
            string concatStr = ts + reqType + reqEncData;
            string macData = Tools.getSHA1(concatStr);
            string reqMacStr = Encoding.UTF8.GetString(RSAUtil.DecryptByPublicKey(Tools.ParseHexString(reqMac), platPublicKey));
            if (reqMacStr == string.Empty)
            {
                //解密mac失败
                reqParams.Add("retCode", CrossBorderConstant.ERROR_0016);
                reqParams.Add("retMsg", CrossBorderConstant.ERROR_0016_MSG);
                return reqParams;
            }

            //mac校验
            if (!macData.Equals(reqMacStr))
            {
                //mac校验失败
                reqParams.Add("retCode", CrossBorderConstant.ERROR_0015);
                reqParams.Add("retMsg", CrossBorderConstant.ERROR_0015_MSG);
                return reqParams;
            }

            // 用响应方私钥解密加密密钥密文，比对时间戳，取后32个字符反HEX，得对称密钥
            string merKey = Encoding.UTF8.GetString(RSAUtil.DecryptByPrivateKey(Tools.ParseHexString(reqEncKey), merPrivateKey));//商户对称密钥
            if (merKey == string.Empty)
            {
                reqParams.Add("retCode", CrossBorderConstant.ERROR_0017);
                reqParams.Add("retMsg", CrossBorderConstant.ERROR_0017_MSG);
                return reqParams;
            }
            merKey = merKey.Substring(merKey.Length - 32, 32);

            // 对称密钥解密“加密json1”，得到明文“请求业务json”
            string reqData = Encoding.UTF8.GetString(DESCrypto.Decrypt(Tools.ParseHexString(reqEncData), merKey));
            if (reqData == string.Empty)
            {
                //解密业务参数失败
                reqParams.Add("retCode", CrossBorderConstant.ERROR_0011);
                reqParams.Add("retMsg", CrossBorderConstant.ERROR_0011_MSG);
                return reqParams;
            }
            reqParams.Remove("retCode");
            reqParams.Add("retCode", CrossBorderConstant.SUCCESS);

            reqParams.Add("reqData", reqData);
            reqParams.Add("merKey", merKey);
            return reqParams;
        }
    }
}