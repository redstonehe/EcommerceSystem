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

namespace VMall.Web.Mobile.Controllers
{
    /// <summary>
    /// 移动微信支付控制器类
    /// </summary>
    public class WeChatController : BaseMobileController
    {
        /// <summary>
        /// 支付
        /// </summary>
        public void Pay()
        {
            //订单id列表
            string oidList = WebHelper.GetQueryString("oidList");

            //LogHelper.WriteOperateLog("WeChatPay", "微信支付请求记录", "oidList:" + oidList);
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
            //网页授权获取code
            string code = Request.QueryString["code"];
            if (string.IsNullOrEmpty(code))
            {
                string code_url = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state=lk#wechat_redirect", PayConfig.AppId, string.Format("http://{0}/mob/wechat/pay?oidList=" + oidList, BMAConfig.MallConfig.SiteUrl));
                //LogUtil.WriteLog("code_url:" + code_url);
                //LogHelper.WriteOperateLog("WeChatPay", "微信支付请求记录", "code_url:" + code_url);
                //return Redirect(code_url);
                Response.Redirect(code_url);
                //Server.Execute(code_url);
            }

            //LogUtil.WriteLog(" ============ 开始 获取微信用户相关信息 =====================");
            //LogHelper.WriteOperateLog("WeChatPay", "微信支付请求记录", " ============ 开始 获取微信用户相关信息 =====================");

            #region 获取支付用户 OpenID================
            string url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", PayConfig.AppId, PayConfig.AppSecret, code);
            string returnStr = HttpUtil.Send("", url);
            //LogUtil.WriteLog("Send 页面  returnStr 第一个：" + returnStr);

            OpenModel openModel = JsonConvert.DeserializeObject<OpenModel>(returnStr);

            url = string.Format("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={0}&grant_type=refresh_token&refresh_token={1}", PayConfig.AppId, openModel.refresh_token);
            returnStr = HttpUtil.Send("", url);
            openModel = JsonConvert.DeserializeObject<OpenModel>(returnStr);

            //LogUtil.WriteLog("Send 页面  access_token：" + openModel.access_token);
            //LogUtil.WriteLog("Send 页面  openid=" + openModel.openid);
            //LogHelper.WriteOperateLog("WeChatPay", "微信支付请求记录", "Send 页面  access_token：" + openModel.access_token + "\r\nSend 页面  openid=" + openModel.openid);

            //url = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}", obj.access_token, obj.openid);
            //returnStr = HttpUtil.Send("", url);
            //LogUtil.WriteLog("Send 页面  returnStr：" + returnStr);
            #endregion

            //LogUtil.WriteLog(" ============ 结束 获取微信用户相关信息 =====================");
            //LogHelper.WriteOperateLog("WeChatPay", "微信支付请求记录", " ============ 开始 获取微信用户相关信息 =====================");
            //LogUtil.WriteLog("============ 单次支付开始 ===============");
            //LogHelper.WriteOperateLog("WeChatPay", "微信支付请求记录", "============ 单次支付开始 ===============");
            #region 支付操作============================


            #region 基本参数===========================
            //商户订单号
            string outTradeNo = orderList.Count > 1 ? "H" + orderList.FirstOrDefault().OSN : orderList.FirstOrDefault().OSN; //商户交易平台订单ID// oidList + Randoms.CreateRandomValue(10, false);
            //订单名称
            string subject = BMAConfig.MallConfig.SiteTitle + "购物";
            //付款金额
            string totalFee = Convert.ToInt32(allSurplusMoney * 100).ToString();
            //openId
            string openId = openModel.openid;
            //时间戳 
            string timeStamp = TenpayUtil.getTimestamp();
            //随机字符串 
            string nonceStr = TenpayUtil.getNoncestr();
            //服务器异步通知页面路径
            string notifyUrl = string.Format("http://{0}/mob/wechat/notify", BMAConfig.MallConfig.SiteUrl);

            //LogUtil.WriteLog("totalFee" + totalFee);

            //创建支付应答对象
            RequestHandler packageReqHandler = new RequestHandler(System.Web.HttpContext.Current);
            //初始化
            packageReqHandler.init();

            //设置package订单参数  具体参数列表请参考官方pdf文档，请勿随意设置
            packageReqHandler.setParameter("body", subject); //商品信息 127字符
            packageReqHandler.setParameter("appid", PayConfig.AppId);
            packageReqHandler.setParameter("mch_id", PayConfig.MchId);
            packageReqHandler.setParameter("nonce_str", nonceStr.ToLower());
            packageReqHandler.setParameter("notify_url", notifyUrl);
            packageReqHandler.setParameter("openid", openId);
            packageReqHandler.setParameter("out_trade_no", outTradeNo); //商家订单号
            packageReqHandler.setParameter("spbill_create_ip", Request.UserHostAddress); //用户的公网ip，不是商户服务器IP
            packageReqHandler.setParameter("total_fee", totalFee); //商品金额,以分为单位(money * 100).ToString()
            packageReqHandler.setParameter("trade_type", "JSAPI");
            //if (!string.IsNullOrEmpty(this.Attach))
            //    packageReqHandler.setParameter("attach", this.Attach);//自定义参数 127字符
            packageReqHandler.setParameter("attach", string.Join(",", orderList.Select(x => x.Oid).ToArray()));//订单附加数据
            #endregion

            #region sign===============================
            string sign = packageReqHandler.CreateMd5Sign("key", PayConfig.AppKey);
            //LogUtil.WriteLog("WeiPay 页面  sign：" + sign);
            #endregion

            #region 获取package包======================
            packageReqHandler.setParameter("sign", sign);

            string data = packageReqHandler.parseXML();
            //LogUtil.WriteLog("WeiPay 页面  package（XML）：" + data);
            LogHelper.WriteOperateLog("WeChatPay", "微信支付请求记录", "WeiPay 页面  package（XML）：" + data);
            string prepayXml = HttpUtil.Send(data, "https://api.mch.weixin.qq.com/pay/unifiedorder");
            //LogUtil.WriteLog("WeiPay 页面  package（Back_XML）：" + prepayXml);
            LogHelper.WriteOperateLog("WeChatPay", "微信支付请求记录", "WeiPay 页面  package（Back_XML）：" + prepayXml);
            //获取预支付ID
            string prepayId = "";
            string package = "";
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(prepayXml);
            XmlNode xn = xdoc.SelectSingleNode("xml");
            XmlNodeList xnl = xn.ChildNodes;
            if (xnl.Count > 7)
            {
                prepayId = xnl[7].InnerText;
                package = string.Format("prepay_id={0}", prepayId);
                //LogUtil.WriteLog("WeiPay 页面  package：" + package);
                //LogHelper.WriteOperateLog("WeChatPay", "微信支付请求记录", "WeiPay 页面  package：" + package);
            }
            #endregion

            #region 设置支付参数 输出页面  该部分参数请勿随意修改 ==============
            RequestHandler paySignReqHandler = new RequestHandler(System.Web.HttpContext.Current);
            paySignReqHandler.setParameter("appId", PayConfig.AppId);
            paySignReqHandler.setParameter("timeStamp", timeStamp);
            paySignReqHandler.setParameter("nonceStr", nonceStr);
            paySignReqHandler.setParameter("package", package);
            paySignReqHandler.setParameter("signType", "MD5");
            string paySign = paySignReqHandler.CreateMd5Sign("key", PayConfig.AppKey);

            //LogUtil.WriteLog("WeiPay 页面  paySign：" + paySign);
            #endregion
            #endregion

            Dictionary<string, string> model = new Dictionary<string, string>();
            model.Add("oidList", oidList);
            model.Add("timeStamp", timeStamp);
            model.Add("nonceStr", nonceStr);
            model.Add("package", package);
            model.Add("paySign", paySign);
            //return PartialView("~/plugins/VMall.PayPlugin.WeChat/views/wechat/pay.cshtml", model);
            Session["AppId"] = PayConfig.AppId;
            Session["data"] = model;

            Server.Transfer("/payment/wechatpay.aspx");
        }

        //public void Pay() {
        //    //Log.Info(this.GetType().ToString(), "page load");
        //    //订单id列表
        //    string oidList = WebHelper.GetQueryString("oidList");
        //    string paySystemName = "";
        //    decimal allSurplusMoney = 0M;
        //    List<OrderInfo> orderList = new List<OrderInfo>();
        //    StringBuilder sbStr = new StringBuilder();
        //    foreach (string oid in StringHelper.SplitString(oidList))
        //    {
        //        //订单信息
        //        OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
        //        if (orderInfo != null && orderInfo.OrderState == (int)OrderState.WaitPaying && orderInfo.PayMode == 1)
        //            orderList.Add(orderInfo);
        //        else
        //            //return Redirect("/mob");
        //            Server.Transfer("/payment/Error.aspx");
        //        allSurplusMoney += orderInfo.SurplusMoney;
        //        paySystemName = orderInfo.PaySystemName;
        //        sbStr.Append(orderInfo.OSN + ",");
        //    }

        //    if (orderList.Count < 1 || allSurplusMoney == 0M)
        //        //return Redirect("/mob");
        //        Server.Transfer("/payment/Error.aspx");
        //    if (orderList.Count > 1)
        //    {
        //        //记录合并订单
        //        //LogHelper.WriteOperateLog("ZdiMergePayOSN", "微信支付合并支付记录", "合并后传递的订单号：H" + orderList.FirstOrDefault().OSN + "|合并的订单号：" + sbStr.ToString().TrimEnd(','));
        //        #region 记录合并订单

        //        MergePayOrderInfo MPOInfo = MergePayOrder.GetMergeOrderBySubOid(orderList.FirstOrDefault().Oid);
        //        if (MPOInfo == null)
        //        {
        //            List<MergePayOrderInfo> merOrderList = new List<MergePayOrderInfo>();
        //            foreach (OrderInfo info in orderList)
        //            {
        //                MergePayOrderInfo payOrder = new MergePayOrderInfo();
        //                payOrder.CreationDate = DateTime.Now;
        //                payOrder.MergeOSN = "H" + orderList.FirstOrDefault().OSN;
        //                payOrder.SubOid = info.Oid;
        //                payOrder.SubOSN = info.OSN;
        //                payOrder.SubOrderAmount = info.SurplusMoney;
        //                payOrder.Uid = info.Uid;
        //                payOrder.StoreId = info.StoreId;
        //                payOrder.StoreName = info.StoreName;
        //                payOrder.PayFriendName = info.PayFriendName;
        //                payOrder.PaySystemName = info.PaySystemName;
        //                merOrderList.Add(payOrder);
        //            }

        //            MergePayOrder.CreateMergePayOrder(merOrderList);

        //            LogHelper.WriteOperateLog("WeChatMergePayOSN", "微信合并支付记录", "合并后传递的订单号：H" + orderList.FirstOrDefault().OSN + "|合并的订单号：" + sbStr.ToString().TrimEnd(','));
        //        }

        //        #endregion

        //    }
        //    string outTradeNo = orderList.Count > 1 ? "H" + orderList.FirstOrDefault().OSN : orderList.FirstOrDefault().OSN;
        //    string oids = string.Join(",", orderList.Select(x => x.Oid).ToArray());

        //    NativePay nativePay = new NativePay();

        //    //生成扫码支付模式一url
        //    // string url1 = nativePay.GetPrePayUrl("123456789");

        //    //生成扫码支付模式二url 

        //    string url2 = nativePay.GetPayUrl(outTradeNo, allSurplusMoney, oids, outTradeNo);
        //    if (string.IsNullOrEmpty(url2))
        //    {
        //        Server.Transfer("/payment/Error.aspx");
        //    }
        //    else
        //    {
        //        //将url生成二维码图片
        //        //Session["NativePayimageUrl"] = IOHelper.CreateCodeForFile(url2, false);
        //        Session["NativePayimageUrl"] = string.Format("/getQRCode?url={0}", url2);
        //        Session["NativePayOSN"] = outTradeNo;
        //        Session["NativePayOids"] = oids;
        //        Session["NativePayOPrice"] = allSurplusMoney.ToString("0.00");
        //        //return PartialView("~/plugins/VMall.PayPlugin.WeChat/views/wechat/pay.cshtml", model);
        //        Server.Transfer("/payment/NativeForM.aspx?data=" + url2);
        //    }
        //}

        public void getQRCode(string url)
        {
            var qrc = IOHelper.CreateCodeForImg(url);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            qrc.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] bytes = ms.GetBuffer();  //byte[]   bytes=   ms.ToArray(); 这两句都可以，至于区别么，下面有解释
            ms.Close();
            Response.BinaryWrite(bytes);
            Response.End();
            return;
        }

        /// <summary>
        /// 通知调用
        /// </summary>
        public ActionResult Notify()
        {
            //创建ResponseHandler实例
            ResponseHandler resHandler = new ResponseHandler(System.Web.HttpContext.Current);
            resHandler.setKey(PayConfig.AppKey);
            //LogHelper.WriteOperateLog("WeChatPayNotify", "微信支付通知记录", "============ 通知开始 ===============");
            //判断签名
            try
            {
                string error = "";
                if (resHandler.isWXsign(out error))
                {
                    #region 协议参数=====================================
                    //--------------协议参数--------------------------------------------------------
                    //SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查
                    string return_code = resHandler.getParameter("return_code");
                    //返回信息，如非空，为错误原因签名失败参数格式校验错误
                    string return_msg = resHandler.getParameter("return_msg");
                    //微信分配的公众账号 ID
                    string appid = resHandler.getParameter("appid");

                    //以下字段在 return_code 为 SUCCESS 的时候有返回--------------------------------
                    //微信支付分配的商户号
                    string mch_id = resHandler.getParameter("mch_id");
                    //微信支付分配的终端设备号
                    string device_info = resHandler.getParameter("device_info");
                    //微信分配的公众账号 ID
                    string nonce_str = resHandler.getParameter("nonce_str");
                    //业务结果 SUCCESS/FAIL
                    string result_code = resHandler.getParameter("result_code");
                    //错误代码 
                    string err_code = resHandler.getParameter("err_code");
                    //结果信息描述
                    string err_code_des = resHandler.getParameter("err_code_des");

                    //以下字段在 return_code 和 result_code 都为 SUCCESS 的时候有返回---------------
                    //-------------业务参数---------------------------------------------------------
                    //用户在商户 appid 下的唯一标识
                    string openid = resHandler.getParameter("openid");
                    //用户是否关注公众账号，Y-关注，N-未关注，仅在公众账号类型支付有效
                    string is_subscribe = resHandler.getParameter("is_subscribe");
                    //JSAPI、NATIVE、MICROPAY、APP
                    string trade_type = resHandler.getParameter("trade_type");
                    //银行类型，采用字符串类型的银行标识
                    string bank_type = resHandler.getParameter("bank_type");
                    //订单总金额，单位为分
                    string total_fee = resHandler.getParameter("total_fee");
                    //货币类型，符合 ISO 4217 标准的三位字母代码，默认人民币：CNY
                    string fee_type = resHandler.getParameter("fee_type");
                    //微信支付订单号
                    string transaction_id = resHandler.getParameter("transaction_id");
                    //商户系统的订单号，与请求一致。
                    string out_trade_no = resHandler.getParameter("out_trade_no");
                    //商家数据包，原样返回
                    string attach = resHandler.getParameter("attach");
                    //支 付 完 成 时 间 ， 格 式 为yyyyMMddhhmmss，如 2009 年12 月27日 9点 10分 10 秒表示为 20091227091010。时区为 GMT+8 beijing。该时间取自微信支付服务器
                    string time_end = resHandler.getParameter("time_end");

                    #endregion
                    string successOids = string.Empty;
                    string failOids = string.Empty;
                    //支付成功
                    if (!out_trade_no.Equals("") && return_code.Equals("SUCCESS") && result_code.Equals("SUCCESS"))
                    {
                        //LogUtil.WriteLog("Notify 页面  支付成功，支付信息：商家订单号：" + out_trade_no + "、支付金额(分)：" + total_fee + "、自定义参数：" + attach);
                        LogHelper.WriteOperateLog("WeChatPayNotify", "异步通知", "Notify 页面  支付成功，支付信息：支付方式：" + trade_type + "、商家订单号：" + out_trade_no + "、支付金额(分)：" + total_fee + "、自定义参数：" + attach + "、微信流水号：" + transaction_id);
                        /**
                         *  这里输入用户逻辑操作，比如更新订单的支付状态
                         * 
                         * **/

                        List<OrderInfo> orderList = new List<OrderInfo>();
                        //foreach (string oid in StringHelper.SplitString(StringHelper.SubString(out_trade_no, out_trade_no.Length - 10)))
                        foreach (string oid in StringHelper.SplitString(attach))
                        {
                            OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                            orderList.Add(orderInfo);
                        }
                        decimal allSurplusMoney = 0M;
                        foreach (OrderInfo orderInfo in orderList)
                        {
                            allSurplusMoney += orderInfo.SurplusMoney;
                        }

                        if (orderList.Count > 0 && allSurplusMoney <= TypeHelper.StringToDecimal(total_fee))
                        {
                            foreach (OrderInfo orderInfo in orderList)
                            {
                                if (orderInfo.SurplusMoney > 0 && orderInfo.OrderState == (int)OrderState.WaitPaying && orderInfo.PayMode == 1)
                                {
                                    /**
                                    *  这里输入用户逻辑操作，比如更新订单的支付状态
                                    * 
                                    * **/
                                    //Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, transaction_id, DateTime.Now);
                                    //OrderActions.CreateOrderAction(new OrderActionInfo()
                                    //{
                                    //    Oid = orderInfo.Oid,
                                    //    Uid = orderInfo.Uid,
                                    //    RealName = "本人",
                                    //    ActionType = (int)OrderActionType.Pay,
                                    //    ActionTime = TypeHelper.StringToDateTime(time_end),
                                    //    ActionDes = "你使用微信支付订单成功，微信交易号为:" + transaction_id
                                    //});

                                    successOids += orderInfo.Oid + ",";
                                    Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, transaction_id, DateTime.Now);
                                    bool isOutOrder = false;

                                    OrderActions.CreateOrderAction(new OrderActionInfo()
                                    {
                                        Oid = orderInfo.Oid,
                                        Uid = orderInfo.Uid,
                                        RealName = isOutOrder ? "外部订单：类型" + orderInfo.OrderSource : "本人",
                                        ActionType = (int)OrderActionType.Pay,
                                        ActionTime = TypeHelper.StringToDateTime(time_end),//交易时间,
                                        ActionDes = "您使用微信支付支付订单成功，交易号为:" + transaction_id
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
                        LogHelper.WriteOperateLog("WeChatPayNotify", "异步通知", "============处理业务逻辑成功，通知结束 ===============");
                        //LogUtil.WriteLog("============ 单次支付结束 ===============");
                        return Content("success");
                    }
                    else
                    {
                        LogHelper.WriteOperateLog("WeChatPayNotifyFail", "微信支付通知失败", "Notify 页面  支付失败，支付信息   total_fee= " + total_fee + "、err_code_des=" + err_code_des + "、result_code=" + result_code);
                        //LogUtil.WriteLog("Notify 页面  支付失败，支付信息   total_fee= " + total_fee + "、err_code_des=" + err_code_des + "、result_code=" + result_code);
                        return new EmptyResult();
                    }
                }
                else
                {
                    LogHelper.WriteOperateLog("WeChatPayNotifyFail", "微信支付通知失败", "Notify 页面  isWXsign= false ，错误信息：" + error);
                    //LogUtil.WriteLog("Notify 页面  isWXsign= false ，错误信息：" + error);
                    return new EmptyResult();
                }
            }
            catch (Exception ee)
            {
                LogHelper.WriteOperateLog("WeChatPayNotifyError", "微信支付通知异常", "Notify 页面  发送异常错误：" + ee.Message, (int)LogLevelEnum.ERROR);
                // LogUtil.WriteLog("Notify 页面  发送异常错误：" + ee.Message);
                return new EmptyResult();
            }
        }

        public ActionResult ReFund(int oid)
        {
            //LogHelper.WriteOperateLog("WeChatReFund", "微信支付退款进入", "退款订单id:" + oid);

            try
            {
                if (oid < 1)
                {
                    return AjaxResult("error", "订单号不存在");
                    //Server.Transfer("/payment/Error.aspx");
                }
                string paySystemName = "";

                OrderInfo orderInfo = Orders.GetOrderByOid(oid);

                string orderSN = string.Empty;
                decimal totalOrderFee = 0M;
                //查询合并订单中是否有该订单记录,有记录则为合并订单,需要传递合并订单号 而非此订单号,支付平台保存的是合并订单号
                MergePayOrderInfo merOrder = MergePayOrder.GetMergeOrderBySubOid(oid);
                if (merOrder != null)
                {
                    orderSN = merOrder.MergeOSN;
                    List<MergePayOrderInfo> mergeList = MergePayOrder.GetMergeOrderListByMergeOSN(merOrder.MergeOSN);
                    totalOrderFee = mergeList.Sum(x => x.SubOrderAmount);
                    LogHelper.WriteOperateLog("WeChatReFund", "微信支付订单金额", "退款订单id:" + oid + "合并支付id" + merOrder.MergeOSN + "总金额" + totalOrderFee);
                }
                else
                {
                    orderSN = orderInfo.OSN;
                    totalOrderFee = orderInfo.SurplusMoney;
                }

                paySystemName = orderInfo.PaySystemName;

                //调用订单退款接口,如果内部出现异常则在页面上显示异常原因
                try
                {
                    // LogUtil.WriteLog("Refund is processing...");

                    WxPayData data = new WxPayData();
                    if (!string.IsNullOrEmpty(orderInfo.PaySN))//微信订单号存在的条件下，则已微信订单号为准
                    {
                        data.SetValue("transaction_id", orderInfo.PaySN);
                    }
                    else//微信订单号不存在，才根据商户订单号去退款
                    {
                        data.SetValue("out_trade_no", orderSN);
                    }

                    data.SetValue("total_fee", int.Parse(totalOrderFee.ToString().Replace(".", "")));//订单总金额
                    data.SetValue("refund_fee", int.Parse(orderInfo.SurplusMoney.ToString().Replace(".", "")));//退款金额
                    string refundsn = WxPayApi.GenerateOutTradeNo();
                    data.SetValue("out_refund_no", refundsn);//随机生成商户退款单号
                    data.SetValue("op_user_id", WxPayConfig.MCHID);//操作员，默认为商户号

                    WxPayData result = WxPayApi.Refund(data);//提交退款申请给API，接收返回数据

                    //LogHelper.WriteOperateLog("WeChatRefund", "Refund process complete", "返回result : " + result.ToXml());
                    if (result.CheckSign())
                    {
                        //获取返回数据
                        #region 协议参数=====================================
                        //--------------协议参数--------------------------------------------------------
                        //SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查
                        string return_code = result.GetValue("return_code").ToString();
                        //返回信息，如非空，为错误原因签名失败参数格式校验错误
                        string return_msg = result.GetValue("return_msg").ToString();


                        //以下字段在 return_code 为 SUCCESS 的时候有返回--------------------------------

                        //业务结果 SUCCESS/FAIL
                        string result_code = result.GetValue("result_code").ToString();
                        //错误代码 
                        string err_code = result.GetValue("err_code") != null ? result.GetValue("err_code").ToString() : "";
                        //结果信息描述
                        string err_code_des = result.GetValue("err_code_des") != null ? result.GetValue("err_code_des").ToString() : "";
                        //微信分配的公众账号 ID
                        string appid = result.GetValue("appid").ToString();
                        //微信支付分配的商户号
                        string mch_id = result.GetValue("mch_id").ToString();
                        //微信支付分配的终端设备号
                        string device_info = result.GetValue("device_info") != null ? result.GetValue("device_info").ToString() : "";
                        //随机字符串
                        string nonce_str = result.GetValue("nonce_str").ToString();
                        //签名
                        string sign = result.GetValue("sign").ToString();
                        //微信订单号
                        string transaction_id = result.GetValue("transaction_id") != null ? result.GetValue("transaction_id").ToString() : "";
                        //商户系统的订单号，与请求一致。
                        string out_trade_no = result.GetValue("out_trade_no") != null ? result.GetValue("out_trade_no").ToString() : "";
                        //商户退款单号	
                        string out_refund_no = result.GetValue("out_refund_no") != null ? result.GetValue("out_refund_no").ToString() : "";
                        //微信退款单号
                        string refund_id = result.GetValue("refund_id") != null ? result.GetValue("refund_id").ToString() : "";
                        //退款渠道
                        string refund_channel = result.GetValue("refund_channel") != null ? result.GetValue("refund_channel").ToString() : "";
                        //退款金额
                        string refund_fee = result.GetValue("refund_fee") != null ? result.GetValue("refund_fee").ToString() : "";
                        //订单总金额，单位为分
                        string total_fee = result.GetValue("total_fee") != null ? result.GetValue("total_fee").ToString() : "";
                        //货币类型，符合 ISO 4217 标准的三位字母代码，默认人民币：CNY
                        string fee_type = result.GetValue("fee_type") != null ? result.GetValue("fee_type").ToString() : "";
                        //现金支付金额
                        string cash_fee = result.GetValue("cash_fee") != null ? result.GetValue("cash_fee").ToString() : "";
                        //现金退款金额	
                        string cash_refund_fee = result.GetValue("cash_refund_fee") != null ? result.GetValue("cash_refund_fee").ToString() : "";


                        #endregion

                        #region 接收退款请求的响应信息

                        //返回退款成功状态才进行后续操作 
                        if (!refund_id.Equals("") && return_code.Equals("SUCCESS") && result_code.Equals("SUCCESS"))
                        {
                            LogHelper.WriteOperateLog("WeChatReFund", "微信支付退款通知", "退款成功，订单ID:" + oid + ",订单号：" + out_trade_no + ",退款订单号:" + refundsn + ",微信支付退款流水号为:" + refund_id);
                            //订单为退货 退款表已有记录 只更新退款状态
                            if (orderInfo.OrderState == (int)OrderState.Returned)
                            {
                                OrderRefundInfo refund = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                                if (refund != null)
                                {
                                    AdminOrderRefunds.RefundOrder(refund.RefundId, refundsn, orderInfo.PaySystemName, orderInfo.PayFriendName, DateTime.Now, refund_id, Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",退货订单退款");
                                }
                                else
                                {
                                    OrderRefunds.ApplyRefund(new OrderRefundInfo()
                                    {
                                        StoreId = orderInfo.StoreId,
                                        StoreName = orderInfo.StoreName,
                                        Oid = oid,
                                        OSN = orderInfo.OSN,
                                        Uid = orderInfo.Uid,
                                        State = 1,
                                        ApplyTime = DateTime.Now,
                                        PayMoney = orderInfo.SurplusMoney,
                                        RefundMoney = TypeHelper.StringToDecimal(refund_fee) / 100,
                                        RefundSN = refundsn,
                                        RefundFriendName = orderInfo.PayFriendName,
                                        RefundSystemName = orderInfo.PaySystemName,
                                        PayFriendName = orderInfo.PayFriendName,
                                        PaySystemName = orderInfo.PaySystemName,
                                        RefundTime = DateTime.Now,
                                        PaySN = orderInfo.PaySN,
                                        RefundTranSN = refund_id,//记录退款流水号 
                                        ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",退货订单退款"
                                    });
                                }
                                OrderActions.CreateOrderAction(new OrderActionInfo()
                                {
                                    Oid = orderInfo.Oid,
                                    Uid = orderInfo.Uid,
                                    RealName = "系统",
                                    ActionType = (int)OrderActionType.Refund,
                                    ActionTime = DateTime.Now,//交易时间,
                                    ActionDes = "退款成功，微信退款流水号为:" + refund_id + "，系统会在1-3个工作日内将退款返回至帐号中。"
                                });
                            }
                            else  //订单为已支付未发货 插入退款表记录 状态为已退款完成  //取消订单
                            {
                                OrderRefunds.ApplyRefund(new OrderRefundInfo()
                                {
                                    StoreId = orderInfo.StoreId,
                                    StoreName = orderInfo.StoreName,
                                    Oid = oid,
                                    OSN = orderInfo.OSN,
                                    Uid = orderInfo.Uid,
                                    State = 1,
                                    ApplyTime = DateTime.Now,
                                    PayMoney = orderInfo.SurplusMoney,
                                    RefundMoney = TypeHelper.StringToDecimal(refund_fee) / 100,
                                    RefundSN = refundsn,
                                    RefundFriendName = orderInfo.PayFriendName,
                                    RefundSystemName = orderInfo.PaySystemName,
                                    PayFriendName = orderInfo.PayFriendName,
                                    PaySystemName = orderInfo.PaySystemName,
                                    RefundTime = DateTime.Now,
                                    PaySN = orderInfo.PaySN,
                                    RefundTranSN = refund_id,//记录退款流水号 
                                    ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",支付成功取消订单订单退款"
                                });
                                OrderActions.CreateOrderAction(new OrderActionInfo()
                                {
                                    Oid = orderInfo.Oid,
                                    Uid = orderInfo.Uid,
                                    RealName = "本人",
                                    ActionType = (int)OrderActionType.Cancel,
                                    ActionTime = DateTime.Now,//交易时间,
                                    ActionDes = "您取消了付款成功的订单，微信退款流水号为:" + refund_id + "，系统会在1-3个工作日内将退款返回至原帐号中。"
                                });
                            }
                        }
                        //退款提交不成功
                        else if (result_code.Equals("FAIL"))
                        {
                            OrderRefundInfo refund = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                            if (refund != null)
                            {
                                string ReMark = "微信退款提交失败，失败原因:" + err_code_des + ".";
                                OrderRefunds.UpdateOrderReFundForNo(refund.RefundId, 0, ReMark);
                            }
                            else
                            {
                                OrderRefunds.ApplyRefund(new OrderRefundInfo()
                                {
                                    StoreId = orderInfo.StoreId,
                                    StoreName = orderInfo.StoreName,
                                    Oid = oid,
                                    OSN = orderInfo.OSN,
                                    Uid = orderInfo.Uid,
                                    State = 0,
                                    ApplyTime = DateTime.Now,
                                    PayMoney = orderInfo.SurplusMoney,
                                    RefundMoney = orderInfo.SurplusMoney,
                                    RefundSN = refundsn,
                                    RefundFriendName = orderInfo.PayFriendName,
                                    RefundSystemName = orderInfo.PaySystemName,
                                    PayFriendName = orderInfo.PayFriendName,
                                    PaySystemName = orderInfo.PaySystemName,
                                    RefundTime = DateTime.Now,
                                    PaySN = orderInfo.PaySN,
                                    RefundTranSN = "",//记录退款流水号 
                                    ReMark = "微信退款提交失败，失败原因:" + err_code_des + "."
                                });
                            }
                            return AjaxResult("error", err_code_des);
                        }
                        else
                        {
                            return AjaxResult("error", err_code_des);
                        }
                        return AjaxResult("success", oid.ToString());

                        #endregion
                        //Response.Write("<span style='color:#00CD00;font-size:20px'>" + result + "</span>");
                    }
                    else
                    {
                        //LogUtil.WriteLog("Notify 页面  isWXsign= false ，错误信息： 签名错误");
                        LogHelper.WriteOperateLog("WeChatRefundFail", "微信支付退款请求失败", "错误信息为：Notify 页面  isWXsign= false ，错误信息： 签名错误");
                        return new EmptyResult();
                    }
                }
                catch (WxPayException ex)
                {
                    LogHelper.WriteOperateLog("WeChatRefundError", "微信支付退款请求异常", "错误信息为：" + ex, (int)LogLevelEnum.ERROR);
                    return AjaxResult("error", ex.ToString());
                    //Response.Write("<span style='color:#FF0000;font-size:20px'>" + ex.ToString() + "</span>");
                }
                catch (Exception ex)
                {
                    LogHelper.WriteOperateLog("WeChatRefundError", "微信支付退款请求异常", "错误信息为：" + ex, (int)LogLevelEnum.ERROR);
                    return AjaxResult("error", ex.ToString());
                    //Response.Write("<span style='color:#FF0000;font-size:20px'>" + ex.ToString() + "</span>");
                }

                //读取表单数据

                //string seqId = "88" + curTime + ran;//退款订单号




            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("WeChatRefundError", "微信支付退款请求异常", "错误信息为" + ex.Message, (int)LogLevelEnum.ERROR);
                return AjaxResult("noorder", "报错了");
            }
        }
        /// <summary>
        /// 组合退款--测试
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult FundTest(string oids)
        {
            try
            {
                foreach (string itemoid in StringHelper.SplitString(oids, ","))
                {
                    //订单信息
                    int oid = TypeHelper.StringToInt(itemoid);
                    OrderInfo orderInfo = Orders.GetOrderByOid(oid);
                    if (oid < 1)
                        continue;
                    string paySystemName = "";

                    string orderSN = string.Empty;
                    decimal totalOrderFee = 0M;
                    //查询合并订单中是否有该订单记录,有记录则为合并订单,需要传递合并订单号 而非此订单号,支付平台保存的是合并订单号
                    MergePayOrderInfo merOrder = MergePayOrder.GetMergeOrderBySubOid(oid);
                    if (merOrder != null)
                    {
                        orderSN = merOrder.MergeOSN;
                        List<MergePayOrderInfo> mergeList = MergePayOrder.GetMergeOrderListByMergeOSN(merOrder.MergeOSN);
                        totalOrderFee = mergeList.Sum(x => x.SubOrderAmount);
                        LogHelper.WriteOperateLog("WeChatReFund", "微信支付订单金额", "退款订单id:" + oid + "合并支付id" + merOrder.MergeOSN + "总金额" + totalOrderFee);
                    }
                    else
                    {
                        orderSN = orderInfo.OSN;
                        totalOrderFee = orderInfo.SurplusMoney;
                    }

                    paySystemName = orderInfo.PaySystemName;

                    //调用订单退款接口,如果内部出现异常则在页面上显示异常原因
                    try
                    {
                        WxPayData data = new WxPayData();
                        if (!string.IsNullOrEmpty(orderInfo.PaySN))//微信订单号存在的条件下，则已微信订单号为准
                        {
                            data.SetValue("transaction_id", orderInfo.PaySN);
                        }
                        else//微信订单号不存在，才根据商户订单号去退款
                        {
                            data.SetValue("out_trade_no", orderSN);
                        }

                        data.SetValue("total_fee", int.Parse(totalOrderFee.ToString().Replace(".", "")));//订单总金额
                        data.SetValue("refund_fee", int.Parse(orderInfo.SurplusMoney.ToString().Replace(".", "")));//退款金额
                        string refundsn = WxPayApi.GenerateOutTradeNo();
                        data.SetValue("out_refund_no", refundsn);//随机生成商户退款单号
                        data.SetValue("op_user_id", WxPayConfig.MCHID);//操作员，默认为商户号

                        bool result = true;//提交退款申请给API，接收返回数据

                        //LogHelper.WriteOperateLog("WeChatRefund", "Refund process complete", "返回result : " + result.ToXml());
                        if (result)
                        {
                            #region 接收退款请求的响应信息
                            #region 协议参数=====================================
                            //--------------协议参数--------------------------------------------------------
                            //SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查

                            //以下字段在 return_code 为 SUCCESS 的时候有返回--------------------------------

                            //业务结果 SUCCESS/FAIL

                            //商户系统的订单号，与请求一致。
                            string out_trade_no = "777777777777";
                            //商户退款单号	
                            string out_refund_no = "888888888888";
                            //微信退款单号
                            string refund_id = "999999999999";

                            //退款金额
                            string refund_fee = orderInfo.SurplusMoney.ToString();

                            #endregion
                            //返回退款成功状态才进行后续操作 
                            if (result)
                            {
                                LogHelper.WriteOperateLog("WeChatReFund", "微信支付退款通知", "退款成功，订单ID:" + oid + ",订单号：" + out_trade_no + ",退款订单号:" + refundsn + ",微信支付退款流水号为:" + refund_id);
                                //订单为退货 退款表已有记录 只更新退款状态
                                if (orderInfo.OrderState == (int)OrderState.Returned)
                                {
                                    OrderRefundInfo refund = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                                    if (refund != null)
                                    {
                                        AdminOrderRefunds.RefundOrder(refund.RefundId, refundsn, orderInfo.PaySystemName, orderInfo.PayFriendName, DateTime.Now, refund_id, Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",退货订单退款");
                                    }
                                    else
                                    {
                                        OrderRefunds.ApplyRefund(new OrderRefundInfo()
                                        {
                                            StoreId = orderInfo.StoreId,
                                            StoreName = orderInfo.StoreName,
                                            Oid = oid,
                                            OSN = orderInfo.OSN,
                                            Uid = orderInfo.Uid,
                                            State = 1,
                                            ApplyTime = DateTime.Now,
                                            PayMoney = orderInfo.SurplusMoney,
                                            RefundMoney = TypeHelper.StringToDecimal(refund_fee) / 100,
                                            RefundSN = refundsn,
                                            RefundFriendName = orderInfo.PayFriendName,
                                            RefundSystemName = orderInfo.PaySystemName,
                                            PayFriendName = orderInfo.PayFriendName,
                                            PaySystemName = orderInfo.PaySystemName,
                                            RefundTime = DateTime.Now,
                                            PaySN = orderInfo.PaySN,
                                            RefundTranSN = refund_id,//记录退款流水号 
                                            ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",退货订单退款"
                                        });
                                    }
                                    OrderActions.CreateOrderAction(new OrderActionInfo()
                                    {
                                        Oid = orderInfo.Oid,
                                        Uid = orderInfo.Uid,
                                        RealName = "系统",
                                        ActionType = (int)OrderActionType.Refund,
                                        ActionTime = DateTime.Now,//交易时间,
                                        ActionDes = "退款成功，微信退款流水号为:" + refund_id + "，系统会在1-3个工作日内将退款返回至帐号中。"
                                    });
                                }
                                else  //订单为已支付未发货 插入退款表记录 状态为已退款完成  //取消订单
                                {
                                    OrderRefundInfo refund = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                                    if (refund != null)
                                    {
                                        AdminOrderRefunds.RefundOrder(refund.RefundId, refundsn, orderInfo.PaySystemName, orderInfo.PayFriendName, DateTime.Now, refund_id, Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",退货订单退款");
                                    }
                                    else
                                    {
                                        OrderRefunds.ApplyRefund(new OrderRefundInfo()
                                        {
                                            StoreId = orderInfo.StoreId,
                                            StoreName = orderInfo.StoreName,
                                            Oid = oid,
                                            OSN = orderInfo.OSN,
                                            Uid = orderInfo.Uid,
                                            State = 1,
                                            ApplyTime = DateTime.Now,
                                            PayMoney = orderInfo.SurplusMoney,
                                            RefundMoney = TypeHelper.StringToDecimal(refund_fee),
                                            RefundSN = refundsn,
                                            RefundFriendName = orderInfo.PayFriendName,
                                            RefundSystemName = orderInfo.PaySystemName,
                                            PayFriendName = orderInfo.PayFriendName,
                                            PaySystemName = orderInfo.PaySystemName,
                                            RefundTime = DateTime.Now,
                                            PaySN = orderInfo.PaySN,
                                            RefundTranSN = refund_id,//记录退款流水号 
                                            ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",支付成功取消订单订单退款"
                                        });
                                        OrderActions.CreateOrderAction(new OrderActionInfo()
                                        {
                                            Oid = orderInfo.Oid,
                                            Uid = orderInfo.Uid,
                                            RealName = "本人",
                                            ActionType = (int)OrderActionType.Cancel,
                                            ActionTime = DateTime.Now,//交易时间,
                                            ActionDes = "您取消了付款成功的订单，微信退款流水号为:" + refund_id + "，系统会在1-3个工作日内将退款返回至原帐号中。"
                                        });
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                            #endregion

                        }
                        else
                        {
                            LogHelper.WriteOperateLog("WeChatRefundFail", "微信支付退款请求失败", "错误信息为：Notify 页面  请求失败 ，错误信息： 签名错误");
                            continue;
                        }

                    }

                    catch (Exception ex)
                    {
                        LogHelper.WriteOperateLog("WeChatRefundError_Batch", "微信支付退款请求异常", "错误信息为：" + ex, (int)LogLevelEnum.ERROR);
                        continue;
                    }
                }
                return AjaxResult("success", "取消成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("WeChatRefundError_Batch", "微信支付退款请求异常", "错误信息为" + ex.Message, (int)LogLevelEnum.ERROR);
                return AjaxResult("noorder", "取消失败");
            }
        }
        /// <summary>
        /// 批量退款
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult ReFundForBatch(string oids)
        {
            //LogHelper.WriteOperateLog("WeChatReFund", "微信支付退款进入", "退款订单id:" + oid);

            try
            {
                foreach (string itemoid in StringHelper.SplitString(oids, ","))
                {
                    //订单信息
                    int oid = TypeHelper.StringToInt(itemoid);
                    OrderInfo orderInfo = Orders.GetOrderByOid(oid);
                    if (oid < 1)
                        continue;
                    string paySystemName = "";

                    string orderSN = string.Empty;
                    decimal totalOrderFee = 0M;
                    //查询合并订单中是否有该订单记录,有记录则为合并订单,需要传递合并订单号 而非此订单号,支付平台保存的是合并订单号
                    MergePayOrderInfo merOrder = MergePayOrder.GetMergeOrderBySubOid(oid);
                    if (merOrder != null)
                    {
                        orderSN = merOrder.MergeOSN;
                        List<MergePayOrderInfo> mergeList = MergePayOrder.GetMergeOrderListByMergeOSN(merOrder.MergeOSN);
                        totalOrderFee = mergeList.Sum(x => x.SubOrderAmount);
                        LogHelper.WriteOperateLog("WeChatReFund_Batch", "微信支付订单金额", "退款订单id:" + oid + "合并支付id" + merOrder.MergeOSN + "总金额" + totalOrderFee);
                    }
                    else
                    {
                        orderSN = orderInfo.OSN;
                        totalOrderFee = orderInfo.SurplusMoney;
                    }

                    paySystemName = orderInfo.PaySystemName;

                    //调用订单退款接口,如果内部出现异常则在页面上显示异常原因
                    try
                    {
                        WxPayData data = new WxPayData();
                        if (!string.IsNullOrEmpty(orderInfo.PaySN))//微信订单号存在的条件下，则已微信订单号为准
                        {
                            data.SetValue("transaction_id", orderInfo.PaySN);
                        }
                        else//微信订单号不存在，才根据商户订单号去退款
                        {
                            data.SetValue("out_trade_no", orderSN);
                        }

                        data.SetValue("total_fee", int.Parse(totalOrderFee.ToString().Replace(".", "")));//订单总金额
                        data.SetValue("refund_fee", int.Parse(orderInfo.SurplusMoney.ToString().Replace(".", "")));//退款金额
                        string refundsn = WxPayApi.GenerateOutTradeNo();
                        data.SetValue("out_refund_no", refundsn);//随机生成商户退款单号
                        data.SetValue("op_user_id", WxPayConfig.MCHID);//操作员，默认为商户号

                        WxPayData result = WxPayApi.Refund(data);//提交退款申请给API，接收返回数据

                        //LogHelper.WriteOperateLog("WeChatRefundError_Batch", "Refund process complete", "返回result : " + result.ToXml());
                        if (result.CheckSign())
                        {
                            //获取返回数据
                            #region 协议参数=====================================
                            //--------------协议参数--------------------------------------------------------
                            //SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查
                            string return_code = result.GetValue("return_code").ToString();
                            //返回信息，如非空，为错误原因签名失败参数格式校验错误
                            string return_msg = result.GetValue("return_msg").ToString();


                            //以下字段在 return_code 为 SUCCESS 的时候有返回--------------------------------

                            //业务结果 SUCCESS/FAIL
                            string result_code = result.GetValue("result_code").ToString();
                            //错误代码 
                            string err_code = result.GetValue("err_code") != null ? result.GetValue("err_code").ToString() : "";
                            //结果信息描述
                            string err_code_des = result.GetValue("err_code_des") != null ? result.GetValue("err_code_des").ToString() : "";
                            //微信分配的公众账号 ID
                            string appid = result.GetValue("appid").ToString();
                            //微信支付分配的商户号
                            string mch_id = result.GetValue("mch_id").ToString();
                            //微信支付分配的终端设备号
                            string device_info = result.GetValue("device_info") != null ? result.GetValue("device_info").ToString() : "";
                            //随机字符串
                            string nonce_str = result.GetValue("nonce_str").ToString();
                            //签名
                            string sign = result.GetValue("sign").ToString();
                            //微信订单号
                            string transaction_id = result.GetValue("transaction_id") != null ? result.GetValue("transaction_id").ToString() : "";
                            //商户系统的订单号，与请求一致。
                            string out_trade_no = result.GetValue("out_trade_no") != null ? result.GetValue("out_trade_no").ToString() : "";
                            //商户退款单号	
                            string out_refund_no = result.GetValue("out_refund_no") != null ? result.GetValue("out_refund_no").ToString() : "";
                            //微信退款单号
                            string refund_id = result.GetValue("refund_id") != null ? result.GetValue("refund_id").ToString() : "";
                            //退款渠道
                            string refund_channel = result.GetValue("refund_channel") != null ? result.GetValue("refund_channel").ToString() : "";
                            //退款金额
                            string refund_fee = result.GetValue("refund_fee") != null ? result.GetValue("refund_fee").ToString() : "";
                            //订单总金额，单位为分
                            string total_fee = result.GetValue("total_fee") != null ? result.GetValue("total_fee").ToString() : "";
                            //货币类型，符合 ISO 4217 标准的三位字母代码，默认人民币：CNY
                            string fee_type = result.GetValue("fee_type") != null ? result.GetValue("fee_type").ToString() : "";
                            //现金支付金额
                            string cash_fee = result.GetValue("cash_fee") != null ? result.GetValue("cash_fee").ToString() : "";
                            //现金退款金额	
                            string cash_refund_fee = result.GetValue("cash_refund_fee") != null ? result.GetValue("cash_refund_fee").ToString() : "";


                            #endregion

                            #region 接收退款请求的响应信息

                            //返回退款成功状态才进行后续操作 
                            if (!refund_id.Equals("") && return_code.Equals("SUCCESS") && result_code.Equals("SUCCESS"))
                            {
                                LogHelper.WriteOperateLog("WeChatReFund_Batch", "微信支付退款通知", "退款成功，订单ID:" + oid + ",订单号：" + out_trade_no + ",退款订单号:" + refundsn + ",微信支付退款流水号为:" + refund_id);
                                //订单为退货 退款表已有记录 只更新退款状态
                                if (orderInfo.OrderState == (int)OrderState.Returned)
                                {
                                    OrderRefundInfo refund = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                                    if (refund != null)
                                    {
                                        AdminOrderRefunds.RefundOrder(refund.RefundId, refundsn, orderInfo.PaySystemName, orderInfo.PayFriendName, DateTime.Now, refund_id, Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",退货订单退款");
                                    }
                                    else
                                    {
                                        OrderRefunds.ApplyRefund(new OrderRefundInfo()
                                        {
                                            StoreId = orderInfo.StoreId,
                                            StoreName = orderInfo.StoreName,
                                            Oid = oid,
                                            OSN = orderInfo.OSN,
                                            Uid = orderInfo.Uid,
                                            State = 1,
                                            ApplyTime = DateTime.Now,
                                            PayMoney = orderInfo.SurplusMoney,
                                            RefundMoney = TypeHelper.StringToDecimal(refund_fee) / 100,
                                            RefundSN = refundsn,
                                            RefundFriendName = orderInfo.PayFriendName,
                                            RefundSystemName = orderInfo.PaySystemName,
                                            PayFriendName = orderInfo.PayFriendName,
                                            PaySystemName = orderInfo.PaySystemName,
                                            RefundTime = DateTime.Now,
                                            PaySN = orderInfo.PaySN,
                                            RefundTranSN = refund_id,//记录退款流水号 
                                            ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",退货订单退款"
                                        });
                                    }
                                    OrderActions.CreateOrderAction(new OrderActionInfo()
                                    {
                                        Oid = orderInfo.Oid,
                                        Uid = orderInfo.Uid,
                                        RealName = "系统",
                                        ActionType = (int)OrderActionType.Refund,
                                        ActionTime = DateTime.Now,//交易时间,
                                        ActionDes = "退款成功，微信退款流水号为:" + refund_id + "，系统会在1-3个工作日内将退款返回至帐号中。"
                                    });
                                }
                                else  //订单为已支付未发货 插入退款表记录 状态为已退款完成  //取消订单
                                {
                                    OrderRefundInfo refund = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                                    if (refund != null)
                                    {
                                        AdminOrderRefunds.RefundOrder(refund.RefundId, refundsn, orderInfo.PaySystemName, orderInfo.PayFriendName, DateTime.Now, refund_id, Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",退货订单退款");
                                    }
                                    else
                                    {
                                        OrderRefunds.ApplyRefund(new OrderRefundInfo()
                                        {
                                            StoreId = orderInfo.StoreId,
                                            StoreName = orderInfo.StoreName,
                                            Oid = oid,
                                            OSN = orderInfo.OSN,
                                            Uid = orderInfo.Uid,
                                            State = 1,
                                            ApplyTime = DateTime.Now,
                                            PayMoney = orderInfo.SurplusMoney,
                                            RefundMoney = TypeHelper.StringToDecimal(refund_fee) / 100,
                                            RefundSN = refundsn,
                                            RefundFriendName = orderInfo.PayFriendName,
                                            RefundSystemName = orderInfo.PaySystemName,
                                            PayFriendName = orderInfo.PayFriendName,
                                            PaySystemName = orderInfo.PaySystemName,
                                            RefundTime = DateTime.Now,
                                            PaySN = orderInfo.PaySN,
                                            RefundTranSN = refund_id,//记录退款流水号 
                                            ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",支付成功取消订单订单退款"
                                        });
                                        OrderActions.CreateOrderAction(new OrderActionInfo()
                                        {
                                            Oid = orderInfo.Oid,
                                            Uid = orderInfo.Uid,
                                            RealName = "本人",
                                            ActionType = (int)OrderActionType.Cancel,
                                            ActionTime = DateTime.Now,//交易时间,
                                            ActionDes = "您取消了付款成功的订单，微信退款流水号为:" + refund_id + "，系统会在1-3个工作日内将退款返回至原帐号中。"
                                        });
                                    }
                                }
                            }
                            //退款提交不成功
                            else if (result_code.Equals("FAIL"))
                            {
                                OrderRefundInfo refund = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                                if (refund != null)
                                {
                                    string ReMark = "微信退款提交失败，失败原因:" + err_code_des + ".";
                                    OrderRefunds.UpdateOrderReFundForNo(refund.RefundId, 0, ReMark);
                                }
                                else
                                {
                                    OrderRefunds.ApplyRefund(new OrderRefundInfo()
                                    {
                                        StoreId = orderInfo.StoreId,
                                        StoreName = orderInfo.StoreName,
                                        Oid = oid,
                                        OSN = orderInfo.OSN,
                                        Uid = orderInfo.Uid,
                                        State = 0,
                                        ApplyTime = DateTime.Now,
                                        PayMoney = orderInfo.SurplusMoney,
                                        RefundMoney = orderInfo.SurplusMoney,
                                        RefundSN = refundsn,
                                        RefundFriendName = orderInfo.PayFriendName,
                                        RefundSystemName = orderInfo.PaySystemName,
                                        PayFriendName = orderInfo.PayFriendName,
                                        PaySystemName = orderInfo.PaySystemName,
                                        RefundTime = DateTime.Now,
                                        PaySN = orderInfo.PaySN,
                                        RefundTranSN = "",//记录退款流水号 
                                        ReMark = "微信退款提交失败，失败原因:" + err_code_des + "."
                                    });
                                }
                                continue;
                            }
                            else
                            {
                                LogHelper.WriteOperateLog("WeChatReFund_Batch", "微信支付退款通知", "退款失败，订单ID:" + oid + ",订单号：" + out_trade_no + ",退款订单号:" + refundsn + ",返回失败原因:" + err_code_des);
                                continue;
                            }

                            #endregion
                        }
                        else
                        {
                            //LogUtil.WriteLog("Notify 页面  isWXsign= false ，错误信息： 签名错误");
                            LogHelper.WriteOperateLog("WeChatRefundFail_Batch", "微信支付退款请求失败", "错误信息为：Notify 页面  isWXsign= false ，错误信息： 签名错误");
                            continue;
                        }
                    }
                    catch (WxPayException ex)
                    {
                        LogHelper.WriteOperateLog("WeChatRefundError_Batch", "微信支付退款请求异常", "错误信息为：" + ex, (int)LogLevelEnum.ERROR);
                        continue;

                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteOperateLog("WeChatRefundError_Batch", "微信支付退款请求异常", "错误信息为：" + ex, (int)LogLevelEnum.ERROR);
                        continue;
                    }
                }
                return AjaxResult("success", "取消成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("WeChatRefundError_Batch", "微信支付退款请求异常", "错误信息为" + ex.Message, (int)LogLevelEnum.ERROR);
                return AjaxResult("noorder", "取消失败");
            }
        }
    }
}
