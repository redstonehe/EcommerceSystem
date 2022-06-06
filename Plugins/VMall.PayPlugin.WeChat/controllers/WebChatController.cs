using System;
using System.Xml;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.PayPlugin.WeChat;

using Newtonsoft.Json;
using System.Web.Configuration;
using System.Web;

namespace VMall.Web.Controllers
{
    /// <summary>
    /// 移动微信支付控制器类
    /// </summary>
    public class WebChatController : BaseWebController
    {
        /// <summary>
        /// 支付
        /// </summary>
        public void WebPay()
        {
            //Log.Info(this.GetType().ToString(), "page load");
            //订单id列表
            string oidList = WebHelper.GetQueryString("oidList");
            string paySystemName = "";
            decimal allSurplusMoney = 0M;
            List<OrderInfo> orderList = new List<OrderInfo>();
            StringBuilder sbStr = new StringBuilder();
            foreach (string oid in StringHelper.SplitString(oidList))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                if (orderInfo != null && orderInfo.OrderState == (int)OrderState.WaitPaying && orderInfo.PayMode == 1)
                    orderList.Add(orderInfo);
                else
                    //return Redirect("/mob");
                    Server.Transfer("/payment/Error.aspx");
                allSurplusMoney += orderInfo.SurplusMoney;
                paySystemName = orderInfo.PaySystemName;
                sbStr.Append(orderInfo.OSN + ",");
            }

            if (orderList.Count < 1 || allSurplusMoney == 0M)
                //return Redirect("/mob");
                Server.Transfer("/payment/Error.aspx");
            if (orderList.Count > 1)
            {
                //记录合并订单
                //LogHelper.WriteOperateLog("ZdiMergePayOSN", "微信支付合并支付记录", "合并后传递的订单号：H" + orderList.FirstOrDefault().OSN + "|合并的订单号：" + sbStr.ToString().TrimEnd(','));
                #region 记录合并订单

                MergePayOrderInfo MPOInfo = MergePayOrder.GetMergeOrderBySubOid(orderList.FirstOrDefault().Oid);
                if (MPOInfo == null)
                {
                    List<MergePayOrderInfo> merOrderList = new List<MergePayOrderInfo>();
                    foreach (OrderInfo info in orderList)
                    {
                        MergePayOrderInfo payOrder = new MergePayOrderInfo();
                        payOrder.CreationDate = DateTime.Now;
                        payOrder.MergeOSN = "H" + orderList.FirstOrDefault().OSN;
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

                    LogHelper.WriteOperateLog("WeChatMergePayOSN", "微信合并支付记录", "合并后传递的订单号：H" + orderList.FirstOrDefault().OSN + "|合并的订单号：" + sbStr.ToString().TrimEnd(','));
                }

                #endregion

            }
            string outTradeNo = orderList.Count > 1 ? "H" + orderList.FirstOrDefault().OSN : orderList.FirstOrDefault().OSN;
            string oids = string.Join(",", orderList.Select(x => x.Oid).ToArray());

            NativePay nativePay = new NativePay();

            //生成扫码支付模式一url
            // string url1 = nativePay.GetPrePayUrl("123456789");

            //生成扫码支付模式二url 

            string url2 = nativePay.GetPayUrl(outTradeNo, allSurplusMoney, oids, outTradeNo);
            if (string.IsNullOrEmpty(url2))
            {
                Server.Transfer("/payment/Error.aspx");
            }
            else
            {
                //将url生成二维码图片
                //Session["NativePayimageUrl"] = IOHelper.CreateCodeForFile(url2, false);
                Session["NativePayimageUrl"] = string.Format("/getQRCode?url={0}", url2);
                Session["NativePayOSN"] = outTradeNo;
                Session["NativePayOids"] = oids;
                Session["NativePayOPrice"] = allSurplusMoney.ToString("0.00");
                //return PartialView("~/plugins/VMall.PayPlugin.WeChat/views/wechat/pay.cshtml", model);
                Server.Transfer("/payment/NativePayPage.aspx?data=" + url2);
            }
        }

        public void getQRCode(string url) {
            var qrc = IOHelper.CreateCodeForImg(url);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            qrc.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] bytes = ms.GetBuffer();  //byte[]   bytes=   ms.ToArray(); 这两句都可以，至于区别么，下面有解释
            ms.Close();
            Response.BinaryWrite(bytes);
            Response.End();
            return;
        }

    }
}
