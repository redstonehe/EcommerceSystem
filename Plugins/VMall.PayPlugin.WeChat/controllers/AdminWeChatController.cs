using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using VMall.Core;
using VMall.Web.Framework;
using VMall.PayPlugin.WeChat;
using VMall.Services;
using System.Collections.Generic;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台微信支付控制器类
    /// </summary>
    public class AdminWeChatController : BaseMallAdminController
    {
        /// <summary>
        /// 配置
        /// </summary>
        [HttpGet]
        [ChildActionOnly]
        public ActionResult Config()
        {
            ConfigModel model = new ConfigModel();

            PluginSetInfo pluginSetInfo = PluginUtils.GetPluginSet();
            model.WPMchId = pluginSetInfo.WPMchId;
            model.WPAppId = pluginSetInfo.WPAppId;
            model.WPAppSecret = pluginSetInfo.WPAppSecret;
            model.WPAppKey = pluginSetInfo.WPAppKey;
            model.OpenMchId = pluginSetInfo.OpenMchId;
            model.OpenAppId = pluginSetInfo.OpenAppId;
            model.OpenAppSecret = pluginSetInfo.OpenAppSecret;
            model.OpenAppKey = pluginSetInfo.OpenAppKey;

            return View("~/plugins/VMall.PayPlugin.WeChat/views/adminwechat/config.cshtml", model);
        }
        /// <summary>
        /// 配置
        /// </summary>
        [HttpGet]
        [ChildActionOnly]
        public ActionResult Config2()
        {
            ConfigModel model = new ConfigModel();

            PluginSetInfo pluginSetInfo = PluginUtils.GetPluginSet();
            model.WPMchId = pluginSetInfo.WPMchId;
            model.WPAppId = pluginSetInfo.WPAppId;
            model.WPAppSecret = pluginSetInfo.WPAppSecret;
            model.WPAppKey = pluginSetInfo.WPAppKey;
            model.OpenMchId = pluginSetInfo.OpenMchId;
            model.OpenAppId = pluginSetInfo.OpenAppId;
            model.OpenAppSecret = pluginSetInfo.OpenAppSecret;
            model.OpenAppKey = pluginSetInfo.OpenAppKey;
                       //~/plugins/BrnMall.PayPlugin.WeChat/views/adminwechat/config.cshtml
            return View("~/plugins/VMall.PayPlugin.WeChat/views/adminwechat/config2.cshtml", model);
        }
        /// <summary>
        /// 配置
        /// </summary>
        [HttpPost]
        public ActionResult Config(ConfigModel model)
        {
            if (ModelState.IsValid)
            {
                PluginSetInfo pluginSetInfo = new PluginSetInfo();
                pluginSetInfo.WPMchId = model.WPMchId.Trim();
                pluginSetInfo.WPAppId = model.WPAppId.Trim();
                pluginSetInfo.WPAppSecret = model.WPAppSecret.Trim();
                pluginSetInfo.WPAppKey = model.WPAppKey.Trim();
                pluginSetInfo.OpenMchId = model.OpenMchId.Trim();
                pluginSetInfo.OpenAppId = model.OpenAppId.Trim();
                pluginSetInfo.OpenAppSecret = model.OpenAppSecret.Trim();
                pluginSetInfo.OpenAppKey = model.OpenAppKey.Trim();
                PluginUtils.SavePluginSet(pluginSetInfo);

                AddMallAdminLog("修改微信支付插件配置信息");
                return PromptView(Url.Action("config", "plugin", new { configController = "AdminWeChat", configAction = "Config" }), "插件配置修改成功");
            }
            return PromptView(Url.Action("config", "plugin", new { configController = "AdminWeChat", configAction = "Config" }), "信息有误，请重新填写");
        }

        public ActionResult ReFund(int oid)
        {
            //LogHelper.WriteOperateLog("WeChatReFund", "微信支付退款进入", "退款订单id:" + oid);

            try
            {
                if (oid < 1)
                {
                    return PromptView("订单号不存在");
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
                    //LogUtil.WriteLog("Refund is processing...");

                    WxPayData data = new WxPayData();
                    if (!string.IsNullOrEmpty(orderInfo.PaySN))//微信订单号存在的条件下，则已微信订单号为准
                    {
                        data.SetValue("transaction_id", orderInfo.PaySN);
                    }
                    else//微信订单号不存在，才根据商户订单号去退款
                    {
                        data.SetValue("out_trade_no", orderSN);
                    }

                    OrderRefundInfo refundInfo = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                    if (refundInfo.Oid <= 0)
                        return PromptView("订单退款申请不存在");
                    if (refundInfo.RefundMoney > orderInfo.SurplusMoney)
                        return PromptView("退款金额不能大于订单金额");
                    if (refundInfo.RefundMoney <= 0)
                        return PromptView("退款金额不能小于0");

                    data.SetValue("total_fee", int.Parse(totalOrderFee.ToString().Replace(".", "")));//订单总金额
                    data.SetValue("refund_fee", int.Parse(refundInfo.RefundMoney.ToString().Replace(".", "")));//退款金额
                    string refundsn = WxPayApi.GenerateOutTradeNo();
                    data.SetValue("out_refund_no", refundsn);//随机生成商户退款单号
                    data.SetValue("op_user_id", WxPayConfig.MCHID);//操作员，默认为商户号

                    WxPayData result = WxPayApi.Refund(data);//提交退款申请给API，接收返回数据

                    LogHelper.WriteOperateLog("WeChatRefund", "Refund process complete", "返回result : " + result.ToXml());
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
                            //订单为退货状态 退款表已有记录 只更新退款状态
                            if (orderInfo.OrderState == (int)OrderState.Returned)
                            {
                                //退款更新--更新orders.returnedtype为4 orderreturn.state为4
                                Orders.UpdateOrderForRefund(orderInfo.Oid);
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
                                    RealName = "本人",
                                    ActionType = (int)OrderActionType.Refund,
                                    ActionTime = DateTime.Now,//交易时间,
                                    ActionDes = "退款成功，微信退款流水号为:" + refund_id + "，系统会在3个工作日内将退款返回至帐号中。"
                                });
                            }
                            else  //订单为已支付未发货 插入退款表记录 状态为已完成--取消订单退款
                            {
                                OrderRefundInfo refund = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
                                if (refund != null)
                                {
                                    AdminOrderRefunds.RefundOrder(refund.RefundId, refundsn, orderInfo.PaySystemName, orderInfo.PayFriendName, DateTime.Now, refund_id, Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",支付成功取消订单订单退款");
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
                                }
                                OrderActions.CreateOrderAction(new OrderActionInfo()
                                {
                                    Oid = orderInfo.Oid,
                                    Uid = orderInfo.Uid,
                                    RealName = "本人",
                                    ActionType = (int)OrderActionType.Cancel,
                                    ActionTime = DateTime.Now,//交易时间,
                                    ActionDes = "您取消了付款成功的订单，微信退款流水号为:" + refund_id + "，系统会在3个工作日内将退款返回至原帐号中。"
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
                            return PromptView(err_code_des);
                        }
                        return PromptView(Url.Action("RefundList", "Order"), "退款成功");

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
                    LogHelper.WriteOperateLog("WeChatRefundError", "微信支付退款请求异常", "错误信息为" + ex.Message, (int)LogLevelEnum.ERROR);
                    return PromptView("系统错误，" + ex.ToString());
                    //Response.Write("<span style='color:#FF0000;font-size:20px'>" + ex.ToString() + "</span>");
                }
                catch (Exception ex)
                {
                    LogHelper.WriteOperateLog("WeChatRefundError", "微信支付退款请求异常", "错误信息为" + ex.Message, (int)LogLevelEnum.ERROR);
                    return PromptView("系统错误，" + ex.ToString());
                    //Response.Write("<span style='color:#FF0000;font-size:20px'>" + ex.ToString() + "</span>");
                }

                //读取表单数据

                //string seqId = "88" + curTime + ran;//退款订单号

            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("WeChatRefundError", "微信支付退款请求异常", "错误信息为" + ex.Message, (int)LogLevelEnum.ERROR);
                return PromptView("系统错误，" + ex.Message);
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
                            LogHelper.WriteOperateLog("WeChatRefundFail", "微信支付退款请求失败", "错误信息为：Notify 页面  isWXsign= false ，错误信息： 签名错误");
                            continue;
                        }

                    }

                    catch (Exception ex)
                    {
                        LogHelper.WriteOperateLog("WeChatRefundError_Batch", "微信支付退款请求异常", "错误信息为：" + ex, (int)LogLevelEnum.ERROR);
                        continue;
                    }
                }
                return PromptView("取消成功");

            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("WeChatRefundError_Batch", "微信支付退款请求异常", "错误信息为" + ex.Message, (int)LogLevelEnum.ERROR);
                return PromptView("取消失败");
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
                return PromptView(Url.Action("RefundList", "Order"), "退款成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("WeChatRefundError_Batch", "微信支付退款请求异常", "错误信息为" + ex.Message, (int)LogLevelEnum.ERROR);
                return PromptView(Url.Action("RefundList", "Order"), "退款失败");
            }
        }
    }
}
