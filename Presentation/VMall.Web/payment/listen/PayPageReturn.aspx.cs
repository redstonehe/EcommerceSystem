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
    public partial class PayPageReturn : System.Web.UI.Page
    {
        private static string appKey = "22dbd1b598b311e59d7e08606ed9d972";
        protected void Page_Load(object sender, EventArgs e)
        {
            string reqJson = string.Empty;
            Dictionary<String, String> jsonMap = new Dictionary<String, String>();
            foreach (string key in Request.Form)
            {
                string value = Request.Params.Get(key);
                jsonMap.Add(key, value);
            }

            //获取商户编号
            string merId = string.Empty;
            jsonMap.TryGetValue("merId", out merId);

            //获取密钥
            string platPublicKey = WebConfigurationManager.AppSettings["pingtaiPublicKey"];            //拉卡拉平台公钥
            string merPrivateKey = WebConfigurationManager.AppSettings["merPrivateKey" + "." + merId];            //商户私钥

            string encData = string.Empty;
            bool isGetEncData = jsonMap.TryGetValue("encData", out encData);
            if (isGetEncData && encData != string.Empty)
            {
                //解密、验证mac
                Dictionary<String, String> retMap = decryptReqData(jsonMap, platPublicKey, merPrivateKey);
                //解密验签成功
                string reqData = string.Empty;
                retMap.TryGetValue("reqData", out reqData);
                jsonMap.Concat(retMap);
                if (reqData != string.Empty)
                {
                    JObject jsonObject = (JObject)JsonConvert.DeserializeObject(reqData);
                    JToken token = (JToken)jsonObject;
                    foreach (JProperty p in token)
                    {
                        if (jsonMap.ContainsKey(p.Name))
                        {
                            jsonMap.Remove(p.Name);
                        }
                        jsonMap.Add(p.Name, p.Value.ToString());
                    }
                    //jsonMap.Concat((Dictionary<String, String>)JsonConvert.DeserializeObject(reqData));
                }

                Session["__display_data__"] = jsonMap;
                //记录返回日志
                StringBuilder sb = new StringBuilder();
                foreach (var entry in jsonMap)
                {
                    sb.Append("参数名：" + entry.Key + "参数值" + entry.Value + "\r\t");
                }
                Logs.WriteOperateLog("LaKaLaPayPageNotify", "拉卡拉支付前台页面返回记录", "回调返回信息为：" + sb.ToString());
                //OrderInfo orderInfo = Orders.GetOrderByOSN(jsonMap["merOrderId"]);
                //if (orderInfo == null)
                //{
                //    Server.Transfer("/payment/Error.aspx");
                //}

                string successOids = string.Empty;
                string failOids = string.Empty;
                if (jsonMap["payResult"] == "1") //返回支付成功才修改订单状态
                {
                    foreach (string oid in StringHelper.SplitString(jsonMap["ext1"], "/"))
                    {
                        //订单信息
                        OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                        if (orderInfo != null)
                        {
                            successOids += oid + ",";
                            Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, jsonMap["transactionId"], DateTime.Now);
                            //写入txt文本log
                            Logs.WriteOperateLog("LaKaLaPaylog", "拉卡拉支付后台修改状态记录", "修改信息为：订单号：" + jsonMap["merOrderId"] + "交易号" + jsonMap["transactionId"] + "交易状态" + OrderState.Confirming + "|订单来源：" + orderInfo.OrderSource.ToString() + "|是否请求外部中转接口" + (orderInfo.OrderSource.ToString().IndexOfAny(new char[] { (char)((int)OrderSource.天鹰网) }) > 0).ToString());

                            //如果是外部系统订单 则需要中转支付状态给外部系统 
                            //订单号 merOrderId， 流水号：transactionId 订单金额：orderAmount 支付金额：payAmount 订单手续费：orderFee 支付成功时间：dealTime  ，支付状态payResult +isok签名验证
                            bool isOutOrder = false;
                            if (orderInfo.OrderSource.ToString().IndexOfAny(new char[] { '2' }) >= 0)//天鹰网订单需要中转支付状态
                            {
                                string signStr = jsonMap["merOrderId"] + jsonMap["transactionId"] + jsonMap["payAmount"] + jsonMap["orderFee"] + jsonMap["dealTime"] + jsonMap["payResult"] + appKey;
                                string sign = SecureHelper.MD5(signStr);
                                isOutOrder = true;
                                string url = string.Format("http://www.tymall.net.cn/order/payret.shtml?merOrderId={0}&transactionId={1}&orderAmount={2}&payAmount={3}&orderFee={4}&dealTime={5}&payResult={6}&isok={7}", jsonMap["merOrderId"], jsonMap["transactionId"], string.Empty, jsonMap["payAmount"], jsonMap["orderFee"], jsonMap["dealTime"], jsonMap["payResult"], sign);

                                WebHelper.DoGet(url);
                                HuiGouMall.Core.Logs.WriteOperateLog("LaKaLaOutOrderPaylog", "拉卡拉支外部订单转发记录", "修改信息为：订单号：" + jsonMap["merOrderId"] + "交易号" + jsonMap["transactionId"] + "支付状态" + jsonMap["payResult"]);
                            }

                            OrderActions.CreateOrderAction(new OrderActionInfo()
                            {
                                Oid = orderInfo.Oid,
                                Uid = orderInfo.Uid,
                                RealName = isOutOrder ? "外部订单：类型" + orderInfo.OrderSource : "本人",
                                ActionType = (int)OrderActionType.Pay,
                                ActionTime = TypeHelper.StringToDateTime(jsonMap["dealTime"]),//交易时间,
                                ActionDes = "你使用拉卡拉支付订单成功，拉卡拉交易号为:" + jsonMap["transactionId"]
                            });

                            //支付成功后自动确认
                            Orders.UpdateOrderState(orderInfo.Oid, OrderState.Confirmed);
                            OrderActions.CreateOrderAction(new OrderActionInfo()
                            {
                                Oid = orderInfo.Oid,
                                Uid = orderInfo.Uid,
                                RealName = "系统",
                                ActionType = (int)OrderActionType.Confirm,
                                ActionTime = DateTime.Now,//交易时间,
                                ActionDes = "您的订单已经确认,正在备货中"
                            });


                            if (orderInfo.StoreId.ToString() == WebSiteConfig.HealthenStoreId && orderInfo.OrderState >= (int)OrderState.Confirmed)
                            {
                                PartUserInfo userInfo = Users.GetPartUserById(orderInfo.Uid);
                                PayUtils.SendOutOrder(orderInfo, userInfo);
                            }
                        }
                        else
                        {
                            failOids += oid + ",";
                        }
                    }
                }
                Logs.WriteOperateLog("LaKaLaPayoids", "拉卡拉支付订单号记录", "成功订单号：" + successOids + "|失败订单号：" + failOids);
                //Server.Transfer("/payment/RageResult.aspx");
                Response.Redirect("/order/ResultPay?oids=" + jsonMap["ext1"] + "&paystatus=" + jsonMap["payResult"]);
            }

        }

        private Dictionary<String, String> decryptReqData(Dictionary<String, String> reqParams, string platPublicKey, string merPrivateKey)
        {
            //响应码
            string retCode = string.Empty;
            string retMsg = string.Empty;

            string ts = string.Empty;
            string reqType = string.Empty;
            string reqEncData = string.Empty;
            string reqMac = string.Empty;
            string merId = string.Empty;
            string ver = string.Empty;

            reqParams.TryGetValue("retCode", out retCode);
            reqParams.TryGetValue("retMsg", out retMsg);

            reqParams.TryGetValue("ts", out ts);
            reqParams.TryGetValue("reqType", out reqType);
            reqParams.TryGetValue("encData", out reqEncData);
            reqParams.TryGetValue("mac", out reqMac);
            reqParams.TryGetValue("merId", out merId);
            reqParams.TryGetValue("ver", out ver);

            //用请求方公钥验签（拼接时间戳、业务类型、“加密json1”做SHA1，用请求方公钥解密mac反hex的字节数组，比较是否一致）
            string concatStr = retCode + retMsg + merId + ver + ts + reqType + reqEncData;
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
            string merKey = MerchantKeyMap.getKey(merId);//商户对称密钥
            if (merKey == string.Empty)
            {
                return reqParams;
            }

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