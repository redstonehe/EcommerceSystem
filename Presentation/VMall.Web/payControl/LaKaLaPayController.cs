using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VMall.Web.payControl
{
    //using Com.LaKaLa;
    using System.Web.Configuration;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using VMall.Core;
    using VMall.Services;
    using VMall.Web.Framework;
    using VMall.Web.Models;

    public class LaKaLaPayController : BaseWebController
    {
        private static string appKey = "22dbd1b598b311e59d7e08606ed9d972";


        #region 拉卡拉支付
        /// <summary>
        /// 创建订单提交请求
        /// </summary>
        public void CreatWebOrder()
        {
            try
            {
                string curTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                string validTime = DateTime.Now.AddDays(2).ToString("yyyyMMddHHmmss");
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 6; i++)
                {
                    Random rd = new Random();
                    sb.Append(rd.Next(6));
                }
                string ran = sb.ToString();


                StringBuilder sbStr = new StringBuilder();
                //根据订单id获取订单信息
                string oidList = WebHelper.GetQueryString("oidList");
                if (string.IsNullOrEmpty(oidList))
                {
                    Server.Transfer("/payment/Error.aspx");
                }
                string paySystemName = "";
                decimal allSurplusMoney = 0M;
                List<OrderInfo> orderList = new List<OrderInfo>();
                foreach (string oid in StringHelper.SplitString(oidList))
                {
                    //订单信息
                    OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                    if (orderInfo != null && orderInfo.Uid == MallUtils.GetUidCookie() && orderInfo.OrderState == (int)OrderState.WaitPaying && orderInfo.PayMode == 1 && (paySystemName.Length == 0 || paySystemName == orderInfo.PaySystemName))
                        orderList.Add(orderInfo);
                    else
                        Server.Transfer("/payment/Error.aspx");

                    paySystemName = orderInfo.PaySystemName;
                    allSurplusMoney += orderInfo.SurplusMoney;

                    sbStr.Append(orderInfo.OSN + ",");
                }
                if (orderList.Count > 1)
                {
                    //记录合并订单
                    Logs.WriteOperateLog("LaKaLaMergePayOSN", "拉卡拉合并支付记录", "合并后传递的订单号：HB" + orderList.FirstOrDefault().OSN + "|合并的订单号：" + sbStr.ToString().TrimEnd(','));
                }
                if (!orderList.Any())
                {
                    Server.Transfer("/payment/Error.aspx");
                }
                //读取表单数据
                string ts = curTime + ran;
                string ver = "1.0.0";//协议版本号
                string reqType = "B0002";//请求业务类型

                string merOrderId = orderList.Count > 1 ? "HB" + orderList.FirstOrDefault().OSN : orderList.FirstOrDefault().OSN; //"SH" + curTime;//订单编码
                string currency = "CNY";//订单币种
                //取后台数据
                string orderAmount = allSurplusMoney.ToString(); //Request.Form.Get("orderAmount");//订单金额

                string busiRange = "122030";//跨境业务类型
                //string orderSummary = ""; //Request.Form.Get("orderSummary");//订单概述
                string orderTime = orderList.FirstOrDefault().AddTime.ToString("yyyyMMddHHmmss");//Request.Form.Get("orderTime");//订单日期
                string timeZone = "GMT+8";//TimeZoneInfo.Utc.ToSerializedString(); //;// Request.Form.Get("timeZone");//时区
                string pageUrl = "http://www.hhwtop.com/payment/listen/PayPageReturn.aspx";//Request.Form.Get("pageUrl");//跳转商户地址
                string bgUrl = "http://www.hhwtop.com/payment/listen/PayReturnNotify.aspx"; //Request.Form.Get("bgUrl");//后台应答地址
                string ext1 = string.Join("/", orderList.Select(x => x.Oid).ToArray());
                //string ext2 = Request.Form.Get("ext2");
                string orderEffTime = validTime;//Request.Form.Get("orderEffTime");//订单有效时间

                //读取配置文件
                string pingtaiPublicKey = WebConfigurationManager.AppSettings["pingtaiPublicKey"];            //拉卡拉平台公钥
                string merId = WebConfigurationManager.AppSettings["merId"];            //商户号
                string merPrivateKey = WebConfigurationManager.AppSettings["merPrivateKey" + "." + merId];            //商户私钥

                //1.商户随机3DES对称密钥
                string merDesStr = MerchantKeyMap.getKey(merId);
                //2.时间戳
                string dateStr = ts;

                string encKey = "";
                String encKeyStr = dateStr + merDesStr;

                encKey = Tools.ToHexString(RSAUtil.EncryptByPublicKey(Encoding.UTF8.GetBytes(encKeyStr), pingtaiPublicKey));

                Dictionary<String, String> paramsDic = new Dictionary<String, String>();
                paramsDic.Add("merOrderId", merOrderId);
                paramsDic.Add("ts", ts);
                //paramsDic.Add("custId", custId);
                paramsDic.Add("currency", currency);
                paramsDic.Add("orderAmount", orderAmount);
                paramsDic.Add("bizCode", busiRange);
                //paramsDic.Add("orderSummary", orderSummary);
                paramsDic.Add("orderTime", orderTime);
                paramsDic.Add("timeZone", timeZone);
                paramsDic.Add("pageUrl", pageUrl);
                paramsDic.Add("bgUrl", bgUrl);
                paramsDic.Add("ext1", ext1);
                //paramsDic.Add("ext2", ext2);
                paramsDic.Add("orderEffTime", orderEffTime);

                string json = JsonConvert.SerializeObject(paramsDic);
                //加密请求业务json
                string json1 = Tools.ToHexString(DESCrypto.Encrypt(Encoding.UTF8.GetBytes(json), merDesStr));
                //拼接时间戳、业务类型、加密json1、做SHA1,请求方私钥加密，HEX，等MAC
                // 拼接
                string macStr = merId + ver + dateStr + reqType + json1;
                // SHA1
                macStr = Tools.getSHA1(macStr);
                string mac = "";
                mac = Tools.ToHexString(RSAUtil.EncryptByPrivateKey(Encoding.UTF8.GetBytes(macStr), merPrivateKey));
                String url = WebConfigurationManager.AppSettings["ppayGateUrl.WEBEnd"];
                if (!url.StartsWith("http"))
                {
                    url = "http://" + Request.ServerVariables.Get("LOCAL_ADDR") + ":" + Request.ServerVariables.Get("Server_Port") + url;
                }

                Dictionary<String, String> reqMap = new Dictionary<String, String>();
                reqMap.Add("ver", ver);
                reqMap.Add("merId", merId);
                reqMap.Add("ts", dateStr);
                reqMap.Add("pageUrl", pageUrl);
                reqMap.Add("reqType", reqType);
                reqMap.Add("encKey", encKey);
                reqMap.Add("encData", json1);
                reqMap.Add("mac", mac);

                Session["jumpUrl"] = url;
                Session["jumpData"] = reqMap;


                //Server.Execute("/views/jumpUrl.aspx");
                Server.Transfer("/payment/jumpUrl.aspx");
            }
            catch (Exception ex)
            {
                Logs.WriteOperateLog("LaKaLaError", "拉卡拉请求异常", "错误信息为" + ex.Message);
                Server.Transfer("/payment/Error.aspx");
            }
        }

        /// <summary>
        /// 同步通知（返回给前台）
        /// </summary>
        public void PayReturn()
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
                Logs.WriteOperateLog("LaKaLaPayReturn", "拉卡拉支付前台页面返回记录", "回调返回信息为：" + sb.ToString());
                //OrderInfo orderInfo = Orders.GetOrderByOSN(jsonMap["merOrderId"]);
                //if (orderInfo == null)
                //{
                //    Server.Transfer("/payment/Error.aspx");
                //}

                string successOids = string.Empty;
                string failOids = string.Empty;
                if (jsonMap["payResult"] == "1") //返回支付成功才修改订单状态
                {
                    Logs.WriteOperateLog("LaKaLaPayReturn", "拉卡拉支付同步通知", "支付成功，订单号：" + jsonMap["ext1"]);
                    foreach (string oid in StringHelper.SplitString(jsonMap["ext1"], "/"))
                    {
                        //订单信息
                        OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                        if (orderInfo != null)
                        {
                            successOids += oid + ",";
                            Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, jsonMap["transactionId"], DateTime.Now);
                            Logs.WriteOperateLog("LaKaLaPayReturn", "拉卡拉支付同步通知", "支付成功，订单号：" + orderInfo.Oid);
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
                                HlhMall.Core.Logs.WriteOperateLog("LaKaLaOutOrderPaySend", "拉卡拉支外部订单转发记录", "修改信息为：订单号：" + jsonMap["merOrderId"] + "交易号" + jsonMap["transactionId"] + "支付状态" + jsonMap["payResult"]);
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

        /// <summary>
        /// 异步通知（返回给服务器）
        /// </summary>
        public void PayNotify()
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
                retMap = decryptReqDataNotify(jsonMap, platPublicKey, merPrivateKey);
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

                string successOids = string.Empty;
                string failOids = string.Empty;
                if (jsonMap["payResult"] == "1") //返回支付成功才修改订单状态
                {
                    Logs.WriteOperateLog("LaKaLaPayNotify", "拉卡拉支付异步通知", "支付成功，订单号：" + jsonMap["ext1"]);
                    foreach (string oid in StringHelper.SplitString(jsonMap["ext1"], "/"))
                    {
                        //订单信息
                        OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                        if (orderInfo != null)
                        {
                            if (orderInfo.OrderState == (int)OrderState.WaitPaying && orderInfo.PayMode == 1)
                            {
                                successOids += oid + ",";
                                Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, jsonMap["transactionId"], DateTime.Now);
                                Logs.WriteOperateLog("LaKaLaPayNotify", "拉卡拉支付异步通知", "支付成功，订单号：" + orderInfo.Oid);
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
                                    HlhMall.Core.Logs.WriteOperateLog("LaKaLaOutOrderPaySend", "拉卡拉支外部订单转发记录", "修改信息为：订单号：" + jsonMap["merOrderId"] + "交易号" + jsonMap["transactionId"] + "支付状态" + jsonMap["payResult"]);
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
                                //与直销系统订单同步
                                if (orderInfo.StoreId.ToString() == WebSiteConfig.HealthenStoreId && orderInfo.OrderState >= (int)OrderState.Confirmed)
                                {
                                    PartUserInfo userInfo = Users.GetPartUserById(orderInfo.Uid);
                                    PayUtils.SendOutOrder(orderInfo, userInfo);
                                }

                            }

                        }
                        else
                        {
                            failOids += oid + ",";
                        }
                    }
                    Logs.WriteOperateLog("LaKaLaPayoids", "拉卡拉支付订单号记录", "成功订单号：" + successOids + "|失败订单号：" + failOids);
                }

                //HlhMall.Core.Logs.WriteOperateLog("LaKaLaPayReturnNotify", "拉卡拉支付后台回调通知记录", "回调返回信息为：" + JsonConvert.SerializeObject(resMap));


                Response.Write("success");

                //Response.Flush();
                return;
            }
            catch (Exception err)
            {
                Logs.WriteOperateLog("LaKaLaPayNotify", "拉卡拉支付异步通知", "异步通知返回信息为：" + JsonConvert.SerializeObject(retMap) + "|错误信息" + err.Message);
                //系统异常
                Response.Write(JsonConvert.SerializeObject(retMap));
                Response.Flush();
                return;
            }
        }
        #endregion

        public ActionResult ReFund(int oid)
        {
            try
            {
                string curTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                string validTime = DateTime.Now.AddDays(2).ToString("yyyyMMddHHmmss");
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 6; i++)
                {
                    Random rd = new Random();
                    sb.Append(rd.Next(6));
                }
                string ran = sb.ToString();

                StringBuilder sbStr = new StringBuilder();
                //根据订单id获取订单信息
                //string oid = WebHelper.GetQueryString("oid");

                if (oid < 1)
                {
                    Server.Transfer("/payment/Error.aspx");
                }
                string paySystemName = "";

                //OrderRefundInfo orderRFInfo = new OrderRefundInfo();//OrderRefunds.GetOrderRefundByOid(oid);
                OrderInfo orderInfo = Orders.GetOrderByOid(oid);

                string orderSN = string.Empty;
                //查询合并订单中是否有该订单记录,有记录则为合并订单,需要传递合并订单号 而非此订单号,支付平台保存的是合并订单号
                MergePayOrderInfo merOrder = MergePayOrder.GetMergeOrderBySubOid(oid);
                if (merOrder != null)
                {
                    orderSN = merOrder.MergeOSN;
                }
                else
                {
                    orderSN = orderInfo.OSN;
                }

                //if (orderRFInfo != null && orderRFInfo.State == 0 && (paySystemName.Length == 0 || paySystemName == orderRFInfo.PaySystemName)) {

                //}
                //else
                //    Server.Transfer("/payment/Error.aspx");

                paySystemName = orderInfo.PaySystemName;

                //读取表单数据
                string ts = curTime + ran;
                string ver = "1.0.0";//协议版本号
                string reqType = "B0006";//退款业务

                string orderTime = orderInfo.PayTime.ToString("yyyyMMddHHmmss");//订单日期 即支付日期
                string merOrderId = orderSN;//orderInfo.OSN;//商户订单号
                string seqId = "88" + curTime + ran;//退款订单号
                string orderAmount = orderInfo.SurplusMoney.ToString();//订单金额
                string retTime = curTime;//退款日期
                string retAmt = orderInfo.SurplusMoney.ToString();//退款金额

                string retCny = "CNY";//订单币种
                string ext1 = oid.ToString();
                //取后台数据

                //读取配置文件
                string pingtaiPublicKey = WebConfigurationManager.AppSettings["pingtaiPublicKey"];//拉卡拉平台公钥
                string merId = WebConfigurationManager.AppSettings["merId"];//商户号
                string merPrivateKey = WebConfigurationManager.AppSettings["merPrivateKey" + "." + merId];   //商户私钥

                //1.商户随机3DES对称密钥
                string merDesStr = MerchantKeyMap.getKey(merId);
                //2.时间戳
                string dateStr = ts;

                string encKey = "";
                String encKeyStr = dateStr + merDesStr;

                encKey = Tools.ToHexString(RSAUtil.EncryptByPublicKey(Encoding.UTF8.GetBytes(encKeyStr), pingtaiPublicKey));

                Dictionary<String, String> paramsDic = new Dictionary<String, String>();
                paramsDic.Add("ts", ts);
                paramsDic.Add("orderTime", orderTime);
                paramsDic.Add("merOrderId", merOrderId);
                paramsDic.Add("seqId", seqId);
                paramsDic.Add("orderAmount", orderAmount);
                paramsDic.Add("retTime", retTime);
                paramsDic.Add("retAmt", retAmt);
                paramsDic.Add("retCny", retCny);
                paramsDic.Add("ext1", ext1);

                string json = JsonConvert.SerializeObject(paramsDic);
                //加密请求业务json
                string json1 = Tools.ToHexString(DESCrypto.Encrypt(Encoding.UTF8.GetBytes(json), merDesStr));
                //拼接时间戳、业务类型、加密json1、做SHA1,请求方私钥加密，HEX，等MAC
                // 拼接
                string macStr = merId + ver + dateStr + reqType + json1;
                // SHA1
                macStr = Tools.getSHA1(macStr);
                string mac = "";
                mac = Tools.ToHexString(RSAUtil.EncryptByPrivateKey(Encoding.UTF8.GetBytes(macStr), merPrivateKey));
                String url = WebConfigurationManager.AppSettings["ppayGateUrl.WEBEnd"];
                if (!url.StartsWith("http"))
                {
                    url = "http://" + Request.ServerVariables.Get("LOCAL_ADDR") + ":" + Request.ServerVariables.Get("Server_Port") + url;
                }

                Dictionary<String, String> reqMap = new Dictionary<String, String>();
                reqMap.Add("ver", ver);
                reqMap.Add("merId", merId);
                reqMap.Add("ts", dateStr);
                reqMap.Add("reqType", reqType);
                reqMap.Add("encKey", encKey);
                reqMap.Add("encData", json1);
                reqMap.Add("mac", mac);
                //{"retCode":"0002","retMsg":"请求参数错误,商户订单号为空"} 
                //{"retCode":"0601","retMsg":"原订单不存在"}
                //{"retCode":"0000","encData":"16E7A92FCC134292426257E9912BD6D35439BA58E6D9B626229B6410E3FFEE26741E2F5B2FB52A9ED13797935451CFEBFE74C3BB55C0B63F7CD121B6582B227DD831AA9BA0CCE6A38F51AC57CC20A4DB7B14293A43671CF1F7FAFEAF85D513F98D45EE87CDBD058E6696C521C6905DC91FBAE844570336207E75F8A5506E3EC56F1DCD0C683E49FE0B2CD10BC0D6ADE3B2B2106EBB01A71BB1AFA8104F28E6241970DDD2649919425421120D41492A52E6BCC626007350709CE7744ECB5D7DA14C98817789BEB2D28BB9976DDA60D752","ts":"20151218212734555555","retMsg":"成功:退款已受理","encKey":"BF563E0A74ACDE3D3A424113A57CF8E7E8898922F05CF604B9ADAD2F4A3FE4E85D65A60DEB764D10611956C5F82259F6FBA88A59197DAD8DBAFF13F274A9808488D71B442C979713F732BCECC5A3697A47C56E05D3088580E044C9E4BB7C9273110536A1930D80C2E20F33F24072EE6047C44A0DCC7CA742E9C379AE6135785B","reqType":"B0006","merId":"DOPCHN000062","mac":"8598D6B8BA72BEBAC467D4FE1B5944E582EF29BEFFF8D52C1830C68FB9B8EB32955E6AD8F4192E778B64AF44FB20E92319318555454523C16006FE9C7E827E60E2C6D3D786363EF10AFE5B7C339D0906AB4AD0EB9C2DAAAA53D69B5DABC0F35CBC82898AA4DDFC5525E4839A517813E28017614A6B5E782D26590C8BABE1796C","ver":"1.0.0"}
                string result = new WebUtils().DoPost(url, reqMap);

                #region 接收退款请求的响应信息
                string reqJson = string.Empty;
                Dictionary<String, String> jsonMap = new Dictionary<String, String>();

                JObject jsonObject1 = (JObject)JsonConvert.DeserializeObject(result);
                JToken token1 = (JToken)jsonObject1;

                foreach (JProperty item in token1)
                {

                    jsonMap.Add(item.Name, item.Value.ToString());
                }

                if (jsonMap["retCode"] != "0000")
                {
                    return Content(jsonMap["retMsg"]);
                }

                //foreach (string key in Request.Form)
                //{
                //    string value = Request.Params.Get(key);
                //    jsonMap.Add(key, value);
                //}

                //获取商户编号
                string merIdRe = string.Empty;
                jsonMap.TryGetValue("merId", out merIdRe);

                //获取密钥
                string platPublicKey = WebConfigurationManager.AppSettings["pingtaiPublicKey"];            //拉卡拉平台公钥
                string merPrivateKeyRe = WebConfigurationManager.AppSettings["merPrivateKey" + "." + merIdRe];            //商户私钥

                string encData = string.Empty;
                bool isGetEncData = jsonMap.TryGetValue("encData", out encData);
                if (isGetEncData && encData != string.Empty)
                {
                    //解密、验证mac
                    Dictionary<String, String> retMap = decryptReqData(jsonMap, platPublicKey, merPrivateKeyRe);
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
                    if (jsonMap["retStatu"] == "0") //返回退款成功状态才进行后续操作  0退款成功，1处理中，2 失败
                    {
                        Logs.WriteOperateLog("LaKaLaReFund", "拉卡拉退款通知", "退款成功，订单ID:" + jsonMap["ext1"] + ",订单号：" + jsonMap["merOrderId"] + ",退款订单号:" + jsonMap["seqId"] + ",拉卡拉退款流水号为:" + jsonMap["transactionId"]);
                        //订单为已完成状态 退款表已有记录 只更新退款状态
                        if (orderInfo.OrderState == (int)OrderState.Completed)
                        {
                            OrderRefundInfo refund = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                            if (refund != null)
                            {
                                AdminOrderRefunds.RefundOrder(refund.RefundId, jsonMap["seqId"], orderInfo.PaySystemName, orderInfo.PayFriendName, DateTime.Now, jsonMap["transactionId"], orderInfo.OrderSource + ",退货订单退款");
                            }
                            else
                            {
                                OrderRefunds.ApplyRefund(new OrderRefundInfo()
                                {
                                    StoreId = orderInfo.StoreId,
                                    StoreName = orderInfo.StoreName,
                                    Oid = TypeHelper.StringToInt(jsonMap["ext1"]),
                                    OSN = orderInfo.OSN,
                                    Uid = orderInfo.Uid,
                                    State = 1,
                                    ApplyTime = DateTime.Now,
                                    PayMoney = orderInfo.SurplusMoney,
                                    RefundMoney = TypeHelper.StringToDecimal(jsonMap["retAmt"]),
                                    RefundSN = jsonMap["seqId"],
                                    RefundFriendName = orderInfo.PayFriendName,
                                    RefundSystemName = orderInfo.PaySystemName,
                                    PayFriendName = orderInfo.PayFriendName,
                                    PaySystemName = orderInfo.PaySystemName,
                                    RefundTime = DateTime.Now,
                                    PaySN = orderInfo.PaySN,
                                    RefundTranSN = jsonMap["transactionId"],//记录退款流水号 
                                    ReMark = orderInfo.OrderSource + ",退货订单退款"
                                });
                            }
                            OrderActions.CreateOrderAction(new OrderActionInfo()
                            {
                                Oid = orderInfo.Oid,
                                Uid = orderInfo.Uid,
                                RealName = "本人",
                                ActionType = (int)OrderActionType.Refund,
                                ActionTime = DateTime.Now,//交易时间,
                                ActionDes = "退款成功，拉卡拉退款流水号为:" + jsonMap["transactionId"] + "，系统会在3个工作日内将退款返回至原银行帐号中。"
                            });

                        }
                        else  //订单为已支付未发货 插入退款表记录 状态为已完成
                        {
                            OrderRefunds.ApplyRefund(new OrderRefundInfo()
                            {
                                StoreId = orderInfo.StoreId,
                                StoreName = orderInfo.StoreName,
                                Oid = TypeHelper.StringToInt(jsonMap["ext1"]),
                                OSN = orderInfo.OSN,
                                Uid = orderInfo.Uid,
                                State = 1,
                                ApplyTime = DateTime.Now,
                                PayMoney = orderInfo.SurplusMoney,
                                RefundMoney = TypeHelper.StringToDecimal(jsonMap["retAmt"]),
                                RefundSN = jsonMap["seqId"],
                                RefundFriendName = orderInfo.PayFriendName,
                                RefundSystemName = orderInfo.PaySystemName,
                                PayFriendName = orderInfo.PayFriendName,
                                PaySystemName = orderInfo.PaySystemName,
                                RefundTime = DateTime.Now,
                                PaySN = orderInfo.PaySN,
                                RefundTranSN = jsonMap["transactionId"],//记录退款流水号 
                                ReMark = orderInfo.OrderSource + ",支付成功未发货订单退款"
                            });
                            OrderActions.CreateOrderAction(new OrderActionInfo()
                            {
                                Oid = orderInfo.Oid,
                                Uid = orderInfo.Uid,
                                RealName = "本人",
                                ActionType = (int)OrderActionType.Cancel,
                                ActionTime = DateTime.Now,//交易时间,
                                ActionDes = "你取消了付款成功的订单，拉卡拉退款流水号为:" + jsonMap["transactionId"] + "，系统会在3个工作日内将退款返回至原银行帐号中。"
                            });
                        }
                    }
                    return Content(result);
                }
                return Content("报错了");
                #endregion

            }
            catch (Exception ex)
            {
                Logs.WriteOperateLog("LaKaLaError", "拉卡拉请求异常", "错误信息为" + ex.Message);
                return Content("报错了");
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

        private Dictionary<String, String> decryptReqDataNotify(Dictionary<String, String> reqParams, string platPublicKey, string merPrivateKey)
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
