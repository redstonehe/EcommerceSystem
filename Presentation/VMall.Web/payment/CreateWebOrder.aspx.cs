using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HuiGouMall.Web.payment
{
    
    using System.Web.Configuration;
    using System.Text;
    using Newtonsoft.Json;
    using HuiGouMall.Core;
    using HuiGouMall.Services;
    using HuiGouMall.Web.Framework;
    using HuiGouMall.Web.Models;
    public partial class CreateWebOrder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
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

                string payOrderSN = orderList.Count > 1 ? "HB" + orderList.FirstOrDefault().OSN : orderList.FirstOrDefault().OSN;
                if (orderList.Count > 1)
                {
                    #region 记录合并订单

                    MergePayOrderInfo MPOInfo = MergePayOrder.GetMergeOrderBySubOid(orderList.FirstOrDefault().Oid);
                    if (MPOInfo == null)
                    {
                        List<MergePayOrderInfo> merOrderList = new List<MergePayOrderInfo>();
                        foreach (OrderInfo info in orderList)
                        {
                            MergePayOrderInfo payOrder = new MergePayOrderInfo();
                            payOrder.CreationDate = DateTime.Now;
                            payOrder.MergeOSN = "HB" + orderList.FirstOrDefault().OSN;
                            payOrder.SubOid = info.Oid;
                            payOrder.SubOSN = info.OSN;
                            payOrder.SubOrderAmount = info.SurplusMoney;
                            payOrder.Uid = info.Uid;
                            payOrder.StoreId = info.StoreId;
                            payOrder.StoreName = info.StoreName;
                            payOrder.PayFriendName = info.PayFriendName;
                            payOrder.PaySystemName = info.PaySystemName;
                            merOrderList.Add(payOrder);
                        }

                        MergePayOrder.CreateMergePayOrder(merOrderList);
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = orderList.FirstOrDefault().Oid,
                            Uid = orderList.FirstOrDefault().Uid,
                            RealName = "系统合并",
                            ActionType = (int)OrderActionType.Pay,
                            ActionTime = DateTime.Now,
                            ActionDes = "拉卡拉支付订单合并，合并后提交支付平台的订单号：HB" + orderList.FirstOrDefault().OSN + "|合并的订单号：" + sbStr.ToString().TrimEnd(',')
                        });
                        Logs.WriteOperateLog("LaKaLaMergePayOSN", "拉卡拉合并支付记录", "合并后传递的订单号：HB" + orderList.FirstOrDefault().OSN + "|合并的订单号：" + sbStr.ToString().TrimEnd(','));
                    }

                    #endregion
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
                string pageUrl = "http://" + WebHelper.GetHost() + ":" + HttpContext.Current.Request.Url.Port + "/payment/listen/PayPageReturn.aspx";//Request.Form.Get("pageUrl");//跳转商户地址
                string bgUrl = "http://" + WebHelper.GetHost() + ":" + HttpContext.Current.Request.Url.Port + "/payment/listen/PayReturnNotify.aspx"; //Request.Form.Get("bgUrl");//后台应答地址
                string ext1 = string.Join("/", orderList.Select(x => x.Oid).ToArray());
                int payType = 1;//支付类型 1为海汇内部订单 2为外部订单
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
                paramsDic.Add("ext1", ext1);//传递订单号，方便回写订单状态
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
                Logs.WriteOperateLog("LaKaLaFault", "拉卡拉请求异常", "错误信息为" + ex.Message);
                Server.Transfer("/payment/Error.aspx");
            }
        }
    }
}