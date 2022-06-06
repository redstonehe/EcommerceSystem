using System;
using System.Collections.Generic;

using VMall.Core;
using System.Data;

namespace VMall.Web.Models
{
    /// <summary>
    /// 确认订单模型类
    /// </summary>
    public class ConfirmOrderModel
    {
        /// <summary>
        /// 选中的购物车项键列表
        /// </summary>
        public string SelectedCartItemKeyList { get; set; }
        /// <summary>
        /// 选中的店铺项键列表
        /// </summary>
        public string SelectedStoreKeyList { get; set; }
        /// <summary>
        /// 默认完整用户配送地址
        /// </summary>
        public FullShipAddressInfo DefaultFullShipAddressInfo { get; set; }

        /// <summary>
        /// 默认支付插件
        /// </summary>
        public PluginInfo DefaultPayPluginInfo { get; set; }
        /// <summary>
        /// 支付插件列表
        /// </summary>
        public List<PluginInfo> PayPluginList { get; set; }

        /// <summary>
        /// 店铺订单列表
        /// </summary>
        public List<StoreOrder> StoreOrderList { get; set; }

        /// <summary>
        /// 支付积分名称
        /// </summary>
        public string PayCreditName { get; set; }
        /// <summary>
        /// 用户支付积分
        /// </summary>
        public int UserPayCredits { get; set; }
        /// <summary>
        /// 最大使用支付积分
        /// </summary>
        public int MaxUsePayCredits { get; set; }

        /// <summary>
        /// 支付费用
        /// </summary>
        public decimal PayFee { get; set; }

        /// <summary>
        /// 关税费用
        /// </summary>
        public decimal TaxFee { get; set; }

        /// <summary>
        /// 红包减免费用
        /// </summary>
        public decimal AllHonBaoCutFee { get; set; }

        /// <summary>
        /// 全部配送费用
        /// </summary>
        public decimal AllShipFee { get; set; }
        /// <summary>
        /// 全部满减
        /// </summary>
        public decimal AllFullCut { get; set; }
        /// <summary>
        /// 全部商品合计
        /// </summary>
        public decimal AllProductAmount { get; set; }
        /// <summary>
        /// 全部订单合计
        /// </summary>
        public decimal AllOrderAmount { get; set; }

        /// <summary>
        /// 全部商品总数量
        /// </summary>
        public int AllTotalCount { get; set; }
        /// <summary>
        /// 全部商品总重量
        /// </summary>
        public int AllTotalWeight { get; set; }

        /// <summary>
        /// 是否显示验证码
        /// </summary>
        public bool IsVerifyCode { get; set; }

        /// <summary>
        /// 用户账号列表
        /// </summary>
        public List<AccountInfo> UserAccountInfo { get; set; }
        /// <summary>
        /// 是否使用汇购卡券
        /// </summary>
        public bool isUserCsahCoupon { get; set; }
        /// <summary>
        /// 汇购卡列表
        /// </summary>
        public List<CashCouponInfo> CashCouponList { get; set; }

        /// <summary>
        /// 购物车可用汇购卡券金额
        /// </summary>
        public decimal AvailCashCouponAmount { get; set; }

        public bool SelectCashPay { get; set; }

        public bool UseExCode { get; set; }
        /// <summary>
        /// 可用兑换码列表
        /// </summary>
        public List<ExChangeCouponsInfo> VaildEexCodeList { get; set; }
        /// <summary>
        /// 所有有效兑换码列表
        /// </summary>
        public List<ExChangeCouponsInfo> AllExCodeList { get; set; }
        /// <summary>
        /// 所有选择订单产品列表
        /// </summary>
        public List<OrderProductInfo> AllSelectOrderProductList { get; set; }
        /// <summary>
        /// 是否使用代理优惠-代理账户抵现支付
        /// </summary>
        public bool isUserAgentDiscount { get;set;}
        /// <summary>
        /// 可用代理抵现金额
        /// </summary>
        public decimal AvailAgentAmount { get; set; }
        /// <summary>
        /// 是否使用佣金账户优惠-佣金账户抵现支付
        /// </summary>
        public bool isUserCommisionDiscount { get; set; }
    }

    /// <summary>
    /// 确认订单模型类
    /// </summary>
    public class ConfirmOrder_ChongZhiModel
    {
        /// <summary>
        /// 选中的产品id
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 选中的产品
        /// </summary>
        public PartProductInfo  Product { get; set; }
        /// <summary>
        /// 省份id
        /// </summary>
        public int areaid { get; set; }
        /// <summary>
        /// 充值手机
        /// </summary>
        public string CZMobile { get; set; }
        /// <summary>
        /// 充值手机详情
        /// </summary>
        public string mobiletips { get; set; }

        public List<PartProductInfo> ProductList { get; set; }
        /// <summary>
        /// 默认支付插件
        /// </summary>
        public PluginInfo DefaultPayPluginInfo { get; set; }
        /// <summary>
        /// 支付插件列表
        /// </summary>
        public List<PluginInfo> PayPluginList { get; set; }

        
        /// <summary>
        /// 红包减免费用
        /// </summary>
        public decimal AllHonBaoCutFee { get; set; }

        /// <summary>
        /// 全部配送费用
        /// </summary>
        public decimal AllShipFee { get; set; }
        
        /// <summary>
        /// 全部商品合计
        /// </summary>
        public decimal AllProductAmount { get; set; }
        /// <summary>
        /// 全部订单合计
        /// </summary>
        public decimal AllOrderAmount { get; set; }

        /// <summary>
        /// 用户账号列表
        /// </summary>
        public List<AccountInfo> UserAccountInfo { get; set; }
        
    }

    /// <summary>
    /// 店铺订单
    /// </summary>
    public class StoreOrder
    {
        /// <summary>
        /// 店铺购物车信息
        /// </summary>
        public StoreCartInfo StoreCartInfo { get; set; }
        /// <summary>
        /// 商品合计
        /// </summary>
        public decimal ProductAmount { get; set; }
        /// <summary>
        /// 满减
        /// </summary>
        public decimal FullCut { get; set; }
        /// <summary>
        /// 关税费用
        /// </summary>
        public decimal TaxFee { get; set; }
        /// <summary>
        /// 红包减免
        /// </summary>
        public decimal HongBaoCutFee { get; set; }
        /// <summary>
        /// 配送费用
        /// </summary>
        public decimal ShipFee { get; set; }
        /// <summary>
        /// 商品总数量
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 商品总重量
        /// </summary>
        public int TotalWeight { get; set; }
        /// <summary>
        /// 店铺产品运费模版列表
        /// </summary>
        public List<StoreShipTemplateInfo> ShipTempList { get; set; }
        /// <summary>
        /// 是否配送发货
        /// </summary>
        public bool isSend { get; set; }
    }

    /// <summary>
    /// 提交结果模型类
    /// </summary>
    public class SubmitResultModel
    {
        public string OidList { get; set; }
        public List<OrderInfo> OrderList { get; set; }

        public PluginInfo PayPlugin { get; set; }

        public decimal AllPayMoney { get; set; }

        public string OnlinePayOidList { get; set; }
    }

    /// <summary>
    /// 支付展示模型类
    /// </summary>
    public class PayShowModel
    {
        public string OidList { get; set; }
        public List<OrderInfo> OrderList { get; set; }
        public PluginInfo PayPlugin { get; set; }
        public string ShowView { get; set; }
        public decimal AllSurplusMoney { get; set; }
    }

    /// <summary>
    /// 支付结果模型类
    /// </summary>
    public class PayResultModel
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 支付成功订单列表
        /// </summary>
        public List<OrderInfo> SuccessOrderList { get; set; }
        /// <summary>
        /// 支付失败订单列表
        /// </summary>
        public List<OrderInfo> FailOrderList { get; set; }
    }

    public class JoinPlanModel { 
        
    }
    public class FenQiResultModel
    {
        /// <summary>
        /// 支付成功订单列表
        /// </summary>
        public string SuccessOidList { get; set; }
        /// <summary>
        /// 支付失败订单列表
        /// </summary>
        public string FailOidList { get; set; }

    }

    /// <summary>
    /// 确认订单模型类
    /// </summary>
    public class ConfirmOrder_AgentModel
    {
        /// <summary>
        /// 选中的产品id
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 选中的产品
        /// </summary>
        public PartProductInfo Product { get; set; }
        /// <summary>
        /// 默认完整用户配送地址
        /// </summary>
        public FullShipAddressInfo DefaultFullShipAddressInfo { get; set; }

        /// <summary>
        /// 默认支付插件
        /// </summary>
        public PluginInfo DefaultPayPluginInfo { get; set; }
        /// <summary>
        /// 支付插件列表
        /// </summary>
        public List<PluginInfo> PayPluginList { get; set; }

        /// <summary>
        /// 店铺订单列表
        /// </summary>
        public List<StoreOrder> StoreOrderList { get; set; }

        /// <summary>
        /// 全部配送费用
        /// </summary>
        public decimal AllShipFee { get; set; }
        /// <summary>
        /// 全部满减
        /// </summary>
        public decimal AllFullCut { get; set; }
        /// <summary>
        /// 全部商品合计
        /// </summary>
        public decimal AllProductAmount { get; set; }
        /// <summary>
        /// 全部订单合计
        /// </summary>
        public decimal AllOrderAmount { get; set; }

        /// <summary>
        /// 全部商品总数量
        /// </summary>
        public int AllTotalCount { get; set; }
        /// <summary>
        /// 全部商品总重量
        /// </summary>
        public int AllTotalWeight { get; set; }

        /// <summary>
        /// 所有选择订单产品列表
        /// </summary>
        public List<OrderProductInfo> AllSelectOrderProductList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSpecialAgentUser { get; set; }

    }
    /// <summary>
    /// 物流查询模型类
    /// </summary>
    public class OrderViewModel
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 状态详情
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 订单信息
        /// </summary>
        public List<OrderInfo> OrderInfoList { get; set; }

    }

}