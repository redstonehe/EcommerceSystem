using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.PayPlugin.Alipay;
using System.Text;
using System.Linq;

namespace VMall.Web.Controllers
{
    /// <summary>
    /// 前台支付宝控制器类
    /// </summary>
    public class AlipayController : BaseWebController
    {
        /// <summary>
        /// 支付
        /// </summary>
        public ActionResult Pay()
        {
            //订单id列表
            string oidList = WebHelper.GetQueryString("oidList");
            //LogHelper.WriteOperateLog("AliPay", "支付宝支付请求记录", "oidList:" + oidList);
            string paySystemName = "";
            decimal allSurplusMoney = 0M;
            List<OrderInfo> orderList = new List<OrderInfo>();
            StringBuilder sbStr = new StringBuilder();
            foreach (string oid in StringHelper.SplitString(oidList))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                if (orderInfo != null && orderInfo.Uid == WorkContext.Uid && orderInfo.OrderState == (int)OrderState.WaitPaying && orderInfo.PayMode == 1)
                    orderList.Add(orderInfo);
                else
                    return Redirect("/payment/Error.aspx");
                // Server.Transfer("/payment/Error.aspx");
                allSurplusMoney += orderInfo.SurplusMoney;
                paySystemName = orderInfo.PaySystemName;
                sbStr.Append(orderInfo.OSN + ",");
            }

            if (orderList.Count < 1 || allSurplusMoney == 0M)
                return Redirect("/payment/Error.aspx");

            if (orderList.Count > 1)
            {
                //记录合并订单
                LogHelper.WriteOperateLog("AliMergePayOSN", "支付宝支付合并支付记录", "合并后传递的订单号：H" + orderList.FirstOrDefault().OSN + "|合并的订单号：" + sbStr.ToString().TrimEnd(','));
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

                }

                #endregion

            }

            //支付类型，必填，不能修改
            string paymentType = "1";

            //服务器异步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数
            string notifyUrl = string.Format("http://{0}/Alipay/Notify", BMAConfig.MallConfig.SiteUrl);
            //页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/
            string returnUrl = string.Format("http://{0}/Alipay/Return", BMAConfig.MallConfig.SiteUrl);

            //收款支付宝帐户
            string sellerEmail = AlipayConfig.Seller;
            //合作者身份ID
            string partner = AlipayConfig.Partner;
            //交易安全检验码
            string key = AlipayConfig.Key;

            //商户订单号
            string outTradeNo = orderList.Count > 1 ? "H" + orderList.FirstOrDefault().OSN : orderList.FirstOrDefault().OSN; //商户交易平台订单ID //oidList + Randoms.CreateRandomValue(10, false);
            //订单名称
            string subject = BMAConfig.MallConfig.SiteTitle + "购物";
            //付款金额
            string totalFee = allSurplusMoney.ToString();
            //订单描述
            string body = "";

            //防钓鱼时间戳,若要使用请调用类文件submit中的query_timestamp函数
            string antiPhishingKey = "";
            //客户端的IP地址,非局域网的外网IP地址，如：221.0.0.1
            string exterInvokeIP = "";

            //把请求参数打包成数组
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();
            parms.Add("partner", partner);
            parms.Add("_input_charset", AlipayConfig.InputCharset);
            parms.Add("service", "create_direct_pay_by_user");
            parms.Add("payment_type", paymentType);
            parms.Add("notify_url", notifyUrl);
            parms.Add("return_url", returnUrl);
            parms.Add("seller_email", sellerEmail);
            parms.Add("out_trade_no", outTradeNo);
            parms.Add("subject", subject);
            parms.Add("total_fee", totalFee);
            parms.Add("body", body);
            parms.Add("show_url", "");
            parms.Add("anti_phishing_key", antiPhishingKey);
            parms.Add("exter_invoke_ip", exterInvokeIP);
            parms.Add("extra_common_param", string.Join(",", orderList.Select(x => x.Oid).ToArray()));
            StringBuilder tmpstr = new StringBuilder();
            foreach (var data in parms)
            {
                tmpstr.Append(data.Key + ":" + data.Value + "\r\t");
            }
            LogHelper.WriteOperateLog("AliPay参数", "AliPay请求参数", "|信息:" + parms.Count + "\r\n 参数：" + tmpstr + "\r\n");

            //建立请求
            string sHtmlText = AlipaySubmit.BuildRequest(parms, AlipayConfig.SignType, AlipayConfig.Key, AlipayConfig.Code, AlipayConfig.Gateway, AlipayConfig.InputCharset, "get", "确认");
            return Content(sHtmlText);
        }

        /// <summary>
        /// 返回调用
        /// </summary>
        public ActionResult Return()
        {
            SortedDictionary<string, string> paras = AlipayCore.GetRequestGet();

            if (paras.Count > 0)//判断是否有带返回参数
            {
                bool verifyResult = AlipayNotify.Verify(paras, Request.QueryString["notify_id"], Request.QueryString["sign"], AlipayConfig.SignType, AlipayConfig.Key, AlipayConfig.Code, AlipayConfig.VeryfyUrl, AlipayConfig.Partner);

                string successOids = string.Empty;
                string failOids = string.Empty;
                bool flag = false;
                if (verifyResult && (Request.QueryString["trade_status"] == "TRADE_FINISHED" || Request.QueryString["trade_status"] == "TRADE_SUCCESS"))//验证成功
                {
                    flag = true;
                    LogHelper.WriteOperateLog("AliPayReturn", "同步通知", "Return 页面  支付成功，支付信息：商家订单号：" + Request.QueryString["out_trade_no"] + "、支付金额(分)：" + TypeHelper.StringToDecimal(Request.QueryString["total_fee"]) + "、自定义参数：" + Request.QueryString["extra_common_param"] + "、流水号：" + Request.QueryString["trade_no"]);
                    string out_trade_no = Request.QueryString["out_trade_no"];//商户订单号
                    string tradeSN = Request.QueryString["trade_no"];//支付宝交易号
                    decimal tradeMoney = TypeHelper.StringToDecimal(Request.QueryString["total_fee"]);//交易金额
                    DateTime tradeTime = TypeHelper.StringToDateTime(Request.QueryString["notify_time"]);//交易时间

                    List<OrderInfo> orderList = new List<OrderInfo>();
                    foreach (string oid in StringHelper.SplitString(Request.QueryString["extra_common_param"], ","))
                    {
                        OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                        orderList.Add(orderInfo);
                    }
                    decimal allSurplusMoney = 0M;
                    foreach (OrderInfo orderInfo in orderList)
                    {
                        allSurplusMoney += orderInfo.SurplusMoney;
                    }

                    if (orderList.Count > 0 && allSurplusMoney <= tradeMoney)
                    {
                        foreach (OrderInfo orderInfo in orderList)
                        {
                            if (orderInfo.SurplusMoney > 0 && orderInfo.OrderState == (int)OrderState.WaitPaying && orderInfo.PayMode == 1)
                            {
                                //Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, tradeSN, DateTime.Now);
                                //OrderActions.CreateOrderAction(new OrderActionInfo()
                                //{
                                //    Oid = orderInfo.Oid,
                                //    Uid = orderInfo.Uid,
                                //    RealName = "本人",
                                //    ActionType = (int)OrderActionType.Pay,
                                //    ActionTime = tradeTime,
                                //    ActionDes = "你使用支付宝支付订单成功，支付宝交易号为:" + tradeSN
                                //});
                                successOids += orderInfo.Oid + ",";
                                Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, tradeSN, tradeTime);
                                bool isOutOrder = false;

                                OrderActions.CreateOrderAction(new OrderActionInfo()
                                {
                                    Oid = orderInfo.Oid,
                                    Uid = orderInfo.Uid,
                                    RealName = isOutOrder ? "外部订单：类型" + orderInfo.OrderSource : "本人",
                                    ActionType = (int)OrderActionType.Pay,
                                    ActionTime = tradeTime,//交易时间,
                                    ActionDes = "您使用支付宝支付订单成功，交易号为:" + tradeSN
                                });

                                //支付成功后自动确认
                                Orders.ConfirmOrder(orderInfo);
                                //OrderActions.CreateOrderAction(new OrderActionInfo()
                                //{
                                //    Oid = orderInfo.Oid,
                                //    Uid = orderInfo.Uid,
                                //    RealName = "系统",
                                //    ActionType = (int)OrderActionType.Confirm,
                                //    ActionTime = DateTime.Now,//交易时间,
                                //    ActionDes = "您的订单已经确认,正在备货中"
                                //});
                                //PartUserInfo userInfo = Users.GetPartUserById(orderInfo.Uid);
                                //OrderInfo orderInfoCopy = Orders.GetOrderByOid(orderInfo.Oid);
                                //旗舰店订单发送到直销后台--暂未开启
                                //if (orderInfoCopy.StoreId.ToString() == WebConfigurationManager.AppSettings["HealthenStoreId"] && orderInfoCopy.OrderState >= (int)OrderState.Confirmed)
                                //{
                                //    OrderUtils.SendOutOrder(orderInfoCopy, userInfo);
                                //}

                                //消费及创建三级分销会员资格
                                //LogHelper.WriteOperateLog("WeChatPayNotify", "微信支付通知记录", "============ 通知结束结束 ===============");
                                //bool flag = userInfo.IsFXUser >= 1;//orderInfoCopy.StoreId.ToString() == WebConfigurationManager.AppSettings["HealthenStoreId"] && orderInfoCopy.OrderState >= (int)OrderState.Confirmed && orderInfoCopy.SurplusMoney > 877;// false;
                                //if (!flag && orderInfoCopy.OrderAmount < 15800)
                                //{
                                //    OrderUtils.UpdateFXUserSates(userInfo.Uid, 1);
                                //}
                                //if (!userInfo.IsDirSaleUser)
                                //{
                                //    if (userInfo.IsFXUser < 2 && orderInfoCopy.OrderAmount >= 15800)
                                //        OrderUtils.UpdateFXUserSates(userInfo.Uid, 2);
                                //    else if (userInfo.IsFXUser == 0 && orderInfoCopy.OrderAmount < 15800)
                                //        OrderUtils.UpdateFXUserSates(userInfo.Uid, 1);
                                //}
                            }
                        }
                    }
                    //Response.Redirect("/order/ResultPay?oids=" + jsonMap["ext1"] + "&paystatus=" + jsonMap["payResult"]);
                    return RedirectToAction("ResultPay", "order", new RouteValueDictionary { { "oids", Request.QueryString["extra_common_param"] }, { "paystatus", flag ? 1 : 0 } });
                }
                else//验证失败
                {
                    LogHelper.WriteOperateLog("AlitPayReturnFail", "支付宝支付通知失败", "Return 页面  支付失败，支付信息   total_fee= " + TypeHelper.StringToDecimal(Request.QueryString["total_fee"]) + "、trade_status=" + Request.QueryString["trade_status"]);
                    return new EmptyResult();
                }
            }
            else
            {
                LogHelper.WriteOperateLog("AlitPayReturnFail", "支付宝支付通知失败", "Return 页面 ，错误信息：=====");
                return new EmptyResult();
            }
        }

        /// <summary>
        /// 通知调用
        /// </summary>
        public ActionResult Notify()
        {
            SortedDictionary<string, string> paras = AlipayCore.GetRequestPost();

            if (paras.Count > 0)//判断是否有带返回参数
            {
                bool verifyResult = AlipayNotify.Verify(paras, Request.Form["notify_id"], Request.Form["sign"], AlipayConfig.SignType, AlipayConfig.Key, AlipayConfig.Code, AlipayConfig.VeryfyUrl, AlipayConfig.Partner);

                string successOids = string.Empty;
                string failOids = string.Empty;
                bool flag = false;
                if (verifyResult && (Request.Form["trade_status"] == "TRADE_FINISHED" || Request.Form["trade_status"] == "TRADE_SUCCESS"))//验证成功
                {
                    flag = true;
                    LogHelper.WriteOperateLog("AliPayNotify", "异步通知", "Notify 页面  支付成功，支付信息：商家订单号：" + Request.Form["out_trade_no"] + "、支付金额：" + TypeHelper.StringToDecimal(Request.Form["total_fee"]) + "、自定义参数：" + Request.Form["extra_common_param"] + "、支付流水号：" + Request.Form["trade_no"]);
                    string out_trade_no = Request.Form["out_trade_no"];//商户订单号
                    string tradeSN = Request.Form["trade_no"];//支付宝交易号
                    decimal tradeMoney = TypeHelper.StringToDecimal(Request.Form["total_fee"]);//交易金额
                    DateTime tradeTime = TypeHelper.StringToDateTime(Request.Form["gmt_payment"]);//交易时间

                    List<OrderInfo> orderList = new List<OrderInfo>();
                    foreach (string oid in StringHelper.SplitString(Request.Form["extra_common_param"]))
                    {
                        OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                        orderList.Add(orderInfo);
                    }
                    decimal allSurplusMoney = 0M;
                    foreach (OrderInfo orderInfo in orderList)
                    {
                        allSurplusMoney += orderInfo.SurplusMoney;
                    }

                    if (orderList.Count > 0 && allSurplusMoney <= tradeMoney)
                    {
                        foreach (OrderInfo orderInfo in orderList)
                        {
                            if (orderInfo.SurplusMoney > 0 && orderInfo.OrderState == (int)OrderState.WaitPaying && orderInfo.PayMode == 1)
                            {
                                //Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, tradeSN, DateTime.Now);
                                //OrderActions.CreateOrderAction(new OrderActionInfo()
                                //{
                                //    Oid = orderInfo.Oid,
                                //    Uid = orderInfo.Uid,
                                //    RealName = "本人",
                                //    ActionType = (int)OrderActionType.Pay,
                                //    ActionTime = tradeTime,
                                //    ActionDes = "你使用支付宝支付订单成功，支付宝交易号为:" + tradeSN
                                //});
                                successOids += orderInfo.Oid + ",";
                                Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, tradeSN, tradeTime);
                                bool isOutOrder = false;

                                OrderActions.CreateOrderAction(new OrderActionInfo()
                                {
                                    Oid = orderInfo.Oid,
                                    Uid = orderInfo.Uid,
                                    RealName = isOutOrder ? "外部订单：类型" + orderInfo.OrderSource : "本人",
                                    ActionType = (int)OrderActionType.Pay,
                                    ActionTime = tradeTime,//交易时间,
                                    ActionDes = "您使用支付宝支付订单成功，交易号为:" + tradeSN
                                });

                                //支付成功后自动确认
                                Orders.ConfirmOrder(orderInfo);
                                //OrderActions.CreateOrderAction(new OrderActionInfo()
                                //{
                                //    Oid = orderInfo.Oid,
                                //    Uid = orderInfo.Uid,
                                //    RealName = "系统",
                                //    ActionType = (int)OrderActionType.Confirm,
                                //    ActionTime = DateTime.Now,//交易时间,
                                //    ActionDes = "您的订单已经确认,正在备货中"
                                //});
                                //PartUserInfo userInfo = Users.GetPartUserById(orderInfo.Uid);
                                //OrderInfo orderInfoCopy = Orders.GetOrderByOid(orderInfo.Oid);
                                //旗舰店订单发送到直销后台--暂未开启
                                //if (orderInfoCopy.StoreId.ToString() == WebConfigurationManager.AppSettings["HealthenStoreId"] && orderInfoCopy.OrderState >= (int)OrderState.Confirmed)
                                //{
                                //    OrderUtils.SendOutOrder(orderInfoCopy, userInfo);
                                //}

                                //消费及创建三级分销会员资格
                                //LogHelper.WriteOperateLog("WeChatPayNotify", "微信支付通知记录", "============ 通知结束结束 ===============");
                                //bool flag = userInfo.IsFXUser >= 1;//orderInfoCopy.StoreId.ToString() == WebConfigurationManager.AppSettings["HealthenStoreId"] && orderInfoCopy.OrderState >= (int)OrderState.Confirmed && orderInfoCopy.SurplusMoney > 877;// false;
                                //if (!flag && orderInfoCopy.OrderAmount < 15800)
                                //{
                                //    OrderUtils.UpdateFXUserSates(userInfo.Uid, 1);
                                //}
                                //if (!userInfo.IsDirSaleUser)
                                //{
                                //    if (userInfo.IsFXUser < 2 && orderInfoCopy.OrderAmount >= 15800)
                                //        OrderUtils.UpdateFXUserSates(userInfo.Uid, 2);
                                //    else if (userInfo.IsFXUser == 0 && orderInfoCopy.OrderAmount < 15800)
                                //        OrderUtils.UpdateFXUserSates(userInfo.Uid, 1);
                                //}
                               
                            }
                        }
                        return Content("success");
                    }

                    return Content("fail");
                }
                else//验证失败
                {
                    return Content("fail");
                }
            }
            else
            {
                return Content("无通知参数");
            }
        }

        /// <summary>
        /// 退款--后台操作，因支付宝需要退款回调，所以不能放在后台控制器内
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult ReFund(int oid)
        {
            //LogHelper.WriteOperateLog("WeChatPayReFund", "微信支付退款进入", "退款订单id:" + oid);
            try
            {
                if (oid < 1)
                {
                    return AjaxResult("error", "订单不存在");
                    //Server.Transfer("/payment/Error.aspx");
                }
                string paySystemName = "";

                OrderInfo orderInfo = Orders.GetOrderByOid(oid);
                if (orderInfo == null)
                {
                    return AjaxResult("error", "订单不存在");
                }
                //string orderSN = string.Empty;
                ////查询合并订单中是否有该订单记录,有记录则为合并订单,需要传递合并订单号 而非此订单号,支付平台保存的是合并订单号
                //MergePayOrderInfo merOrder = MergePayOrder.GetMergeOrderBySubOid(oid);
                //if (merOrder != null)
                //    orderSN = merOrder.MergeOSN;
                //else
                //    orderSN = orderInfo.OSN;

                paySystemName = orderInfo.PaySystemName;

                //调用订单退款接口,如果内部出现异常则在页面上显示异常原因
                try
                {
                    OrderRefundInfo refundInfo = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                    
                    if (refundInfo.RefundMoney > orderInfo.SurplusMoney)
                        return AjaxResult("error", "退款金额不能大于订单金额");
                    if (refundInfo.RefundMoney <= 0)
                        return AjaxResult("error", "退款金额不能小于0");

                    decimal refundAmount=0M;
                    if (refundInfo.Oid <= 0)
                        refundAmount = orderInfo.SurplusMoney;
                    else
                        refundAmount = refundInfo.RefundMoney;

                    //卖家支付宝帐户
                    string sellerEmail = AlipayConfig.Seller;
                    //卖家用户ID
                    string seller_user_id = AlipayConfig.Partner;
                    //合作者身份ID
                    string partner = AlipayConfig.Partner;
                    //服务器异步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数
                    string notifyUrl = string.Format("http://{0}/Alipay/RefundNotify", BMAConfig.MallConfig.SiteUrl);
                    //交易安全检验码
                    //string key = AlipayConfig.Key;
                    // 退款请求时间
                    DateTime refund_date = DateTime.Now;
                    //退款批次号
                    string batch_no = GenerateBatchNo();
                    //总笔数
                    int batch_num = 1;
                    //单笔数据集
                    string detail_data = orderInfo.PaySN + "^" + refundAmount + "^订单退款";// "2011011201037066^5.00^协商退款";//单笔数据集

                    //把请求参数打包成数组
                    SortedDictionary<string, string> parms = new SortedDictionary<string, string>();
                    parms.Add("partner", partner);
                    parms.Add("_input_charset", AlipayConfig.InputCharset);
                    parms.Add("service", "refund_fastpay_by_platform_pwd");

                    parms.Add("notify_url", notifyUrl);
                    parms.Add("refund_date", refund_date.ToString("yyyy-MM-dd HH:mm:ss"));
                    parms.Add("seller_email", sellerEmail);
                    parms.Add("seller_user_id", AlipayConfig.Partner);

                    parms.Add("batch_no", batch_no);
                    parms.Add("batch_num", batch_num.ToString());
                    parms.Add("detail_data", detail_data);

                    StringBuilder tmpstr = new StringBuilder();
                    foreach (var data in parms)
                    {
                        tmpstr.Append(data.Key + ":" + data.Value + "\r\t");
                    }
                    LogHelper.WriteOperateLog("AliRefund", "AliRefund请求参数", "|信息:" + parms.Count + "\r\n 参数：" + tmpstr + "\r\n");

                    //建立请求

                    string sHtmlText = AlipaySubmit.BuildRequest(parms, AlipayConfig.SignType, AlipayConfig.Key, AlipayConfig.Code, AlipayConfig.Gateway, AlipayConfig.InputCharset, "get", "确认");

                    OrderRefundInfo info = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                    if (info == null)
                        return AjaxResult("error", "系统退款申请不存在");
                    //LogHelper.WriteOperateLog("AliRefund", "Refund process complete", "返回result : " + sHtmlText);
                    AdminOrderRefunds.UpdateRefundSN(info.RefundId, batch_no);
                    return Content(sHtmlText);

                }
                catch (Exception ex)
                {
                    LogHelper.WriteOperateLog("AliRefundError", "支付宝支付退款请求异常", "错误信息为：" + ex, (int)LogLevelEnum.ERROR);
                    return AjaxResult("error", ex.ToString());
                    //Response.Write("<span style='color:#FF0000;font-size:20px'>" + ex.ToString() + "</span>");
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("AliRefundError", "支付宝支付退款请求异常", "错误信息为" + ex.Message, (int)LogLevelEnum.ERROR);
                return AjaxResult("noorder", "报错了");
            }
        }

        /// <summary>
        /// 退款通知调用
        /// </summary>
        public ActionResult RefundNotify()
        {
            SortedDictionary<string, string> paras = AlipayCore.GetRequestPost();

            if (paras.Count > 0)//判断是否有带返回参数
            {
                bool verifyResult = AlipayNotify.Verify(paras, Request.Form["notify_id"], Request.Form["sign"], AlipayConfig.SignType, AlipayConfig.Key, AlipayConfig.Code, AlipayConfig.VeryfyUrl, AlipayConfig.Partner);

                string successOids = string.Empty;
                string failOids = string.Empty;

                if (verifyResult && (Request.Form["success_num"] == "1" || Request.Form["result_details"].IndexOf("SUCCESS") > 0))//验证成功
                {

                    LogHelper.WriteOperateLog("AliRefundNotify", "异步通知", "ReturnNotify 页面 成功数量：" + Request.Form["success_num"] + " ，返回信息：" + Request.Form["result_details"]);

                    string result = Request.Form["result_details"];
                    string batch_no = Request.Form["batch_no"];
                    string refundtransn = result.Split('^')[0];
                    string refund_fee = result.Split('^')[1];
                    LogHelper.WriteOperateLog("AliRefundNotify", "异步通知", "ReturnNotify 退款流水号 ：" + refundtransn + "退款批次号" + batch_no);
                    OrderRefundInfo info = AdminOrderRefunds.GetRefundInfoByRefundSN(batch_no);
                    if (info == null)
                    {
                        return Content("fail");
                    }
                    OrderInfo orderInfo = Orders.GetOrderByOid(info.Oid);
                    //订单为退货状态 退款表已有记录 只更新退款状态
                    if (orderInfo.OrderState == (int)OrderState.Returned)
                    {
                        //退款更新--更新orders.returnedtype为4 orderreturn.state为4
                        Orders.UpdateOrderForRefund(orderInfo.Oid);
                        //OrderRefundInfo refund = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                        //if (refund != null)
                        //{
                        //    AdminOrderRefunds.RefundOrder(refund.RefundId, batch_no, orderInfo.PaySystemName, orderInfo.PayFriendName, DateTime.Now, refundtransn, Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",退货订单退款");
                        //}
                        //else
                        //{
                        //    OrderRefunds.ApplyRefund(new OrderRefundInfo()
                        //    {
                        //        StoreId = orderInfo.StoreId,
                        //        StoreName = orderInfo.StoreName,
                        //        Oid = orderInfo.Oid,
                        //        OSN = orderInfo.OSN,
                        //        Uid = orderInfo.Uid,
                        //        State = 1,
                        //        ApplyTime = DateTime.Now,
                        //        PayMoney = orderInfo.SurplusMoney,
                        //        RefundMoney = TypeHelper.StringToDecimal(refund_fee),
                        //        RefundSN = batch_no,
                        //        RefundFriendName = orderInfo.PayFriendName,
                        //        RefundSystemName = orderInfo.PaySystemName,
                        //        PayFriendName = orderInfo.PayFriendName,
                        //        PaySystemName = orderInfo.PaySystemName,
                        //        RefundTime = DateTime.Now,
                        //        PaySN = orderInfo.PaySN,
                        //        RefundTranSN = refundtransn,//记录退款流水号 
                        //        ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",退货订单退款"
                        //    });
                        //}
                        //OrderActions.CreateOrderAction(new OrderActionInfo()
                        //{
                        //    Oid = orderInfo.Oid,
                        //    Uid = orderInfo.Uid,
                        //    RealName = "本人",
                        //    ActionType = (int)OrderActionType.Refund,
                        //    ActionTime = DateTime.Now,//交易时间,
                        //    ActionDes = "退款成功，支付宝退款流水号为:" + refundtransn 
                        //});
                    }
                    //else  //订单为已支付未发货 退款表记录更新 状态为已完成--取消订单退款
                    //{
                    //    OrderRefunds.ApplyRefund(new OrderRefundInfo()
                    //    {
                    //        StoreId = orderInfo.StoreId,
                    //        StoreName = orderInfo.StoreName,
                    //        Oid = orderInfo.Oid,
                    //        OSN = orderInfo.OSN,
                    //        Uid = orderInfo.Uid,
                    //        State = 1,
                    //        ApplyTime = DateTime.Now,
                    //        PayMoney = orderInfo.SurplusMoney,
                    //        RefundMoney = TypeHelper.StringToDecimal(refund_fee),
                    //        RefundSN = refundsn,
                    //        RefundFriendName = orderInfo.PayFriendName,
                    //        RefundSystemName = orderInfo.PaySystemName,
                    //        PayFriendName = orderInfo.PayFriendName,
                    //        PaySystemName = orderInfo.PaySystemName,
                    //        RefundTime = DateTime.Now,
                    //        PaySN = orderInfo.PaySN,
                    //        RefundTranSN = transaction_id,//记录退款流水号 
                    //        ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",支付成功取消订单订单退款"
                    //    });
                    //    OrderActions.CreateOrderAction(new OrderActionInfo()
                    //    {
                    //        Oid = orderInfo.Oid,
                    //        Uid = orderInfo.Uid,
                    //        RealName = "本人",
                    //        ActionType = (int)OrderActionType.Cancel,
                    //        ActionTime = DateTime.Now,//交易时间,
                    //        ActionDes = "您取消了付款成功的订单，微信退款流水号为:" + transaction_id + "，系统会在3个工作日内将退款返回至原帐号中。"
                    //    });
                    //}

                    AdminOrderRefunds.UpdateRefundTranSN(info.RefundId, refundtransn);
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = orderInfo.Oid,
                        Uid = orderInfo.Uid,
                        RealName = "系统",
                        ActionType = (int)OrderActionType.Cancel,
                        ActionTime = DateTime.Now,//交易时间,
                        ActionDes = "支付宝退款成功！" 
                    });
                    return Content("success");
                }
                else//验证失败
                {
                    return Content("fail");
                }
            }
            else
            {
                return Content("无通知参数");
            }
        }


        /// <summary>
        /// 生成退款批次号
        /// </summary>
        /// <returns></returns>
        public static string GenerateBatchNo()
        {
            var ran = new Random();
            return string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(99999999));
        }
    }
}
