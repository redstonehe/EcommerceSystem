using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VMall.Core;

namespace VMall.Web.Areas.WebApi.Models
{
    /// <summary>
    /// 购物车模型类
    /// </summary>
    public class CartModel
    {
        /// <summary>
        /// 商品总数量
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 商品合计
        /// </summary>
        public decimal ProductAmount { get; set; }
        /// <summary>
        /// 满减
        /// </summary>
        public int FullCut { get; set; }
        /// <summary>
        /// 订单合计
        /// </summary>
        public decimal OrderAmount { get; set; }
        /// <summary>
        /// 关税
        /// </summary>
        public decimal taxAmount { get; set; }
        /// <summary>
        /// 店铺购物车列表
        /// </summary>
        public List<StoreCartInfo> StoreCartList { get; set; }
    }
    /// <summary>
    /// 确认订单模型类
    /// </summary>
    public class AppConfirmOrderModel
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
        public List<AppStoreOrder> StoreOrderList { get; set; }

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
        /// 全部配送费用
        /// </summary>
        public int AllShipFee { get; set; }
        /// <summary>
        /// 全部满减
        /// </summary>
        public int AllFullCut { get; set; }
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
        //public bool IsVerifyCode { get; set; }
    }

    /// <summary>
    /// 店铺订单
    /// </summary>
    public class AppStoreOrder
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
        public int FullCut { get; set; }
        /// <summary>
        /// 关税费用
        /// </summary>
        public decimal TaxFee { get; set; }
        /// <summary>
        /// 配送费用
        /// </summary>
        public int ShipFee { get; set; }
        /// <summary>
        /// 商品总数量
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 商品总重量
        /// </summary>
        public int TotalWeight { get; set; }
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
}