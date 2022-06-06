using System;
using System.Data;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.Mobile.Models
{
    /// <summary>
    /// 安全验证模型类
    /// </summary>
    public class SafeVerifyModel
    {
        /// <summary>
        /// 动作
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 验证方式
        /// </summary>
        public string Mode { get; set; }
    }

    /// <summary>
    /// 账户信息模型类
    /// </summary>
    public class AccountInfoModel
    {
        /// <summary>
        /// 账户信息
        /// </summary>
        public List<AccountInfo> AccountInfoList { get; set; }

    }
    /// <summary>
    /// 安全更新模型类
    /// </summary>
    public class SafeUpdateModel
    {
        /// <summary>
        /// 动作
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// V
        /// </summary>
        public string V { get; set; }
    }

    /// <summary>
    /// 安全成功模型类
    /// </summary>
    public class SafeSuccessModel
    {
        /// <summary>
        /// 动作
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 订单列表模型类
    /// </summary>
    public class OrderListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 订单列表
        /// </summary>
        public DataTable OrderList { get; set; }
        /// <summary>
        /// 订单商品列表
        /// </summary>
        public List<OrderProductInfo> OrderProductList { get; set; }
        /// <summary>
        /// 开始添加时间
        /// </summary>
        public string StartAddTime { get; set; }
        /// <summary>
        /// 结束添加时间
        /// </summary>
        public string EndAddTime { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderState { get; set; }
    }

    /// <summary>
    /// 订单列表模型类
    /// </summary>
    public class AjaxOrderListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 订单列表
        /// </summary>
        //public List<Dictionary<string, object>> OrderList { get; set; }
        public DataTable OrderList { get; set; }
        /// <summary>
        /// 订单商品列表
        /// </summary>
        public List<OrderProductInfo> OrderProductList { get; set; }
    }

    /// <summary>
    /// 订单信息模型类
    /// </summary>
    public class OrderInfoModel
    {
        /// <summary>
        /// 订单信息
        /// </summary>
        public OrderInfo OrderInfo { get; set; }
        /// <summary>
        /// 区域信息
        /// </summary>
        public RegionInfo RegionInfo { get; set; }
        /// <summary>
        /// 订单商品列表
        /// </summary>
        public List<OrderProductInfo> OrderProductList { get; set; }
    }

    /// <summary>
    /// 订单动作列表模型类
    /// </summary>
    public class OrderActionListModel
    {
        /// <summary>
        /// 订单信息
        /// </summary>
        public OrderInfo OrderInfo { get; set; }
        /// <summary>
        /// 订单处理列表
        /// </summary>
        public List<OrderActionInfo> OrderActionList { get; set; }
    }

    /// <summary>
    /// 收藏夹商品列表模型类
    /// </summary>
    public class FavoriteProductListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public DataTable ProductList { get; set; }
    }

    /// <summary>
    /// 收藏夹店铺列表模型类
    /// </summary>
    public class AjaxFavoriteStoreListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 店铺列表
        /// </summary>
        public List<Dictionary<string, object>> StoreList { get; set; }
    }

    /// <summary>
    /// 浏览商品列表模型类
    /// </summary>
    public class AjaxBrowseProductListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<PartProductInfo> ProductList { get; set; }
    }

    /// <summary>
    /// 配送地址列表模型类
    /// </summary>
    public class ShipAddressListModel
    {
        /// <summary>
        /// 配送地址列表
        /// </summary>
        public List<FullShipAddressInfo> ShipAddressList { get; set; }
        /// <summary>
        /// 配送地址数量
        /// </summary>
        public int ShipAddressCount { get; set; }
    }

    /// <summary>
    /// 配送地址模型类
    /// </summary>
    public class ShipAddressModel
    {
        public string Alias { get; set; }
        public string Consignee { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ZipCode { get; set; }
        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        public int RegionId { get; set; }
        public string Address { get; set; }
        public int IsDefault { get; set; }
    }

    /// <summary>
    /// 优惠劵列表模型类
    /// </summary>
    public class CouponListModel
    {
        /// <summary>
        /// 列表类型
        /// </summary>
        public int ListType { get; set; }
        /// <summary>
        /// 优惠劵列表
        /// </summary>
        public DataTable CouponList { get; set; }
    }

    /// <summary>
    /// 评价订单模型类
    /// </summary>
    public class ReviewOrderModel
    {
        /// <summary>
        /// 订单信息
        /// </summary>
        public OrderInfo OrderInfo { get; set; }
        /// <summary>
        /// 订单商品列表
        /// </summary>
        public List<OrderProductInfo> OrderProductList { get; set; }
        /// <summary>
        /// 店铺评价信息
        /// </summary>
        public StoreReviewInfo StoreReviewInfo { get; set; }
    }

    /// <summary>
    /// 分享会员详情模型类
    /// </summary>
    public class SubRecommendListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }

        /// <summary>
        /// 开始添加时间
        /// </summary>
        public string StartAddTime { get; set; }
        /// <summary>
        /// 结束添加时间
        /// </summary>
        public string EndAddTime { get; set; }

        /// <summary>
        /// 会员列表
        /// </summary>
        public List<UserInfo> UserList { get; set; }
        /// <summary>
        /// 订单列表
        /// </summary>
        public List<OrderInfo> OrderList { get; set; }
        /// <summary>
        /// 订单商品列表
        /// </summary>
        public List<OrderProductInfo> OrderProductList { get; set; }
        /// <summary>
        /// 账户列表
        /// </summary>
        public List<AccountInfo> AccountList { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderState { get; set; }
        /// <summary>
        /// 会员id
        /// </summary>
        public int Uid { get; set; }
    }
    /// <summary>
    /// 分享会员详情模型类
    /// </summary>
    public class SubRecommendDetailModel {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        
        /// <summary>
        /// 开始添加时间
        /// </summary>
        public string StartAddTime { get; set; }
        /// <summary>
        /// 结束添加时间
        /// </summary>
        public string EndAddTime { get; set; }
        
        /// <summary>
        /// 订单列表
        /// </summary>
        public List<OrderInfo> OrderList { get; set; }
        /// <summary>
        /// 订单商品列表
        /// </summary>
        public List<OrderProductInfo> OrderProductList { get; set; }
        /// <summary>
        /// 账户列表
        /// </summary>
        public List<AccountInfo> AccountList { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderState { get; set; }
        /// <summary>
        /// 会员id
        /// </summary>
        public int Uid { get; set; }
    }
    /// <summary>
    /// 账户详情模型类
    /// </summary>
    public class AccountDetailModel
    {
        /// <summary>
        /// 账户名称
        /// </summary>
        public int AccountId{ get; set; }
        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 账户详情列表
        /// </summary>
        public List<AccountDetailInfo> AccountDetailList { get; set; }
    }
    /// <summary>
    /// 退货申请模型类
    /// </summary>
    public class ReturnModel
    {
        /// <summary>
        /// 退货状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 状态详情
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 订单信息
        /// </summary>
        public OrderInfo OrderInfo { get; set; }

    }
    /// <summary>
    /// 换货申请模型类
    /// </summary>
    public class ChangeModel
    {
        /// <summary>
        /// 换货状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 状态详情
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 订单信息
        /// </summary>
        public OrderInfo OrderInfo { get; set; }
        /// <summary>
        /// 配送地址列表
        /// </summary>
        public List<FullShipAddressInfo> ShipAddressList { get; set; }
    }
    /// <summary>
    /// 汇购卡券详情模型类
    /// </summary>
    public class CashDetailModel
    {
        /// <summary>
        /// 账户名称
        /// </summary>
        public string CashName { get; set; }
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 账户详情列表
        /// </summary>
        public List<CashCouponDetailInfo> CashDetailList { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DrawCashModel
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
        /// JumpUrl
        /// </summary>
        public string JumpUrl { get; set; }
        /// <summary>
        /// 订单信息
        /// </summary>
        public AccountInfo AccountInfo { get; set; }

    }
    /// <summary>
    /// 提现历史记录模型类
    /// </summary>
    public class DrawCashHistoryModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 历史记录列表
        /// </summary>
        public List<HaiMiDrawCashInfo> HistoryList { get; set; }
        /// <summary>
        /// 帐户类型
        /// </summary>
        public string AccountName { get; set; }
    }
    /// <summary>
    /// 代理申请详情
    /// </summary>
    public class SubmitApplyModel
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 详情
        /// </summary>
        public string Message { get; set; }


    }
    /// <summary>
    /// 代理库存
    /// </summary>
    public class AgentStockModel
    {
        /// <summary>
        /// 状态
        /// </summary>
        public DataTable AgentStockList { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public DataTable SendOrderList { get; set; }

    }
    /// <summary>
    /// 提现历史记录模型类
    /// </summary>
    public class AgentStockDetailModel
    {
        public int Pid { get; set; }
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 历史记录列表
        /// </summary>
        public List<AgentStockDetailInfo> DetailList { get; set; }
    }
    /// <summary>
    /// 我的代理模型类
    /// </summary>
    public class MyAgentModel
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 状态 0 不是代理1是代理且没推送到直销 2是代理是直销
        /// </summary>
        public int AgentState { get; set; }
        /// <summary>
        /// 状态详情
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// JumpUrl
        /// </summary>
        public string JumpUrl { get; set; }

        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }

        /// <summary>
        /// 会员列表
        /// </summary>
        public List<UserInfo> UserList { get; set; }


    }

    /// <summary>
    /// 代理结果模型类
    /// </summary>
    public class AgentResultModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int JoinUid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParentCode { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string ManagerCode { get; set; }

    }
    /// <summary>
    /// 要货单模型类
    /// </summary>
    public class AgentSendOrderModel
    {
        /// <summary>
        /// 产品列表
        /// </summary>
        public List<PartProductInfo> ProductList { get; set; }
        /// <summary>
        /// 产品
        /// </summary>
        public PartProductInfo Product { get; set; }
        /// <summary>
        /// 地址列表
        /// </summary>
        public List<FullShipAddressInfo> ShipAddress { get; set; }
        /// <summary>
        /// 产品库存
        /// </summary>
        public AgentStockInfo AgentStock { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ASid { get; set; }
    }

    /// <summary>
    /// 申请历史记录模型类
    /// </summary>
    public class Kf2SeverbHistoryModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 历史记录列表
        /// </summary>
        public List<Kftransfer2severbInfo> HistoryList { get; set; }

    }

    /// <summary>
    /// 消息列表模型类
    /// </summary>
    public class InformListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 消息列表
        /// </summary>
        public List<InformInfo> InformList { get; set; }
    }
    /// <summary>
    /// 投诉列表模型类
    /// </summary>
    public class ComplainListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<ComplainInfo> ComplainList { get; set; }
    }

    /// <summary>
    /// 确认赠品订单模型类
    /// </summary>
    public class ConfirmOrderGiftModel
    {
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
        public int Id { get; set; }
        public int Pid { get; set; }
        /// <summary>
        /// 店铺
        /// </summary>
        public StoreOrder StoreOrder { get; set; }
        /// <summary>
        /// 产品
        /// </summary>
        public PartProductInfo Product { get; set; }
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
        /// 是否配送发货
        /// </summary>
        public bool isSend { get; set; }
       
    }
}