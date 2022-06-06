using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.MallAdmin.Models
{
    /// <summary>
    /// 订单列表模型类
    /// </summary>
    public class OrderListModel
    {
        public PageModel PageModel { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public DataTable OrderList { get; set; }
        /// <summary>
        /// 店铺id
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OSN { get; set; }
        /// <summary>
        /// 账户名
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee { get; set; }
        /// <summary>
        /// 收货人手机
        /// </summary>
        public string ConsigneeMobile { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderState { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 支付开始时间
        /// </summary>
        public DateTime? PayStartDate { get; set; }
        /// <summary>
        /// 支付结束时间
        /// </summary>
        public DateTime? PayEndDate { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public int Pid { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 是否咖啡选货
        /// </summary>
        public int IsKFXH { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayName { get; set; }

    }

    /// <summary>
    /// 销售业绩模型类
    /// </summary>
    public class SaleResultModel
    {
        public PageModel PageModel { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public DataTable OrderList { get; set; }
        /// <summary>
        /// 店铺id
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OSN { get; set; }
        /// <summary>
        /// 账户名
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderState { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 是否咖啡选货
        /// </summary>
        public int IsKFXH { get; set; }
        /// <summary>
        /// 是否购买汇购卡订单
        /// </summary>
        public int IsBuyHGCard { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayName { get; set; }
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
        /// 用户信息
        /// </summary>
        public UserInfo UserInfo { get; set; }
        /// <summary>
        /// 用户等级
        /// </summary>
        public UserRankInfo UserRankInfo { get; set; }
        /// <summary>
        /// 订单商品列表
        /// </summary>
        public List<OrderProductInfo> OrderProductList { get; set; }
        /// <summary>
        /// 订单处理列表
        /// </summary>
        public List<OrderActionInfo> OrderActionList { get; set; }
        /// <summary>
        /// 订单退货信息
        /// </summary>
        public OrderReturnInfo OrderReturnInfo { get; set; }
        /// <summary>
        /// 订单换货信息
        /// </summary>
        public OrderChangeInfo OrderChangeInfo { get; set; }
        ///// <summary>
        ///// 订单结算信息
        ///// </summary>
        //public OrderSettleInfo OrderSettlePreview { get; set; }
    }

    /// <summary>
    /// 支付订单模型类
    /// </summary>
    public class PayOrderModel
    {
        [Required(ErrorMessage = "支付单号不能为空")]
        public string PaySN { get; set; }
    }
    /// <summary>
    /// 更新快递单号模型类
    /// </summary>
    public class UpdateOrderShipSNModel
    {

        [Required(ErrorMessage = "快递单号不能为空")]
        public string ShipSN { get; set; }
    }
    /// <summary>
    /// 更新快递单号模型类
    /// </summary>
    public class UpdateShipInfoModel
    {
        public UpdateShipInfoModel()
        {
            ShipCoId = -1;
            ShipCoName = "选择配送公司";
        }

        [Required(ErrorMessage = "快递单号不能为空")]
        [StringLength(30, ErrorMessage = "发货单号长度不能超过30")]
        public string ShipSN { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "请选择配送公司")]
        [Required(ErrorMessage = "请选择配送公司")]
        public int ShipCoId { get; set; }

        /// <summary>
        /// 配送公司名称
        /// </summary>
        public string ShipCoName { get; set; }
        /// <summary>
        /// 旧的配送公司名称
        /// </summary>
        public string OldShipCoName { get; set; }
        /// <summary>
        /// 旧的配送单号
        /// </summary>
        public string OldShipSN { get; set; }

    }
    /// <summary>
    /// 更新订单配送费用模型类
    /// </summary>
    public class UpdateOrderShipFeeModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "配送费用不能小于0")]
        [Required(ErrorMessage = "配送费用不能为空")]
        public decimal ShipFee { get; set; }
    }

    /// <summary>
    /// 确认退货模型类
    /// </summary>
    public class ConfirmReturnModel
    {
        public ConfirmReturnModel()
        {
            ReturnShipFee = 0M;
            ReturnShipDesc = "";
        }
        [Range(0, double.MaxValue, ErrorMessage = "配送费用不能小于0")]
        [Required(ErrorMessage = "配送费用不能为空")]
        public decimal ReturnShipFee { get; set; }

        [StringLength(150, ErrorMessage = "退货运费说明不能超过150")]
        public string ReturnShipDesc { get; set; }
    }

    /// <summary>
    /// 更新订单折扣模型类
    /// </summary>
    public class UpdateOrderDiscountModel
    {
        [Range(0, int.MaxValue, ErrorMessage = "订单折扣不能小于0")]
        [Required(ErrorMessage = "订单折扣不能为空")]
        public decimal Discount { get; set; }
    }

    /// <summary>
    /// 订单发货模型类
    /// </summary>
    public class SendOrderProductModel
    {
        public SendOrderProductModel()
        {
            ShipCoId = -1;
            ShipCoName = "选择配送公司";
        }

        [Required(ErrorMessage = "发货单号不能为空")]
        [StringLength(30, ErrorMessage = "发货单号长度不能超过30")]
        public string ShipSN { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "请选择配送公司")]
        [Required(ErrorMessage = "请选择配送公司")]
        public int ShipCoId { get; set; }

        /// <summary>
        /// 配送公司名称
        /// </summary>
        public string ShipCoName { get; set; }
    }

    /// <summary>
    /// 完成订单模型类
    /// </summary>
    public class CompleteOrderModel
    {
        public string PaySN { get; set; }
    }
    /// <summary>
    /// 银行汇款确认模型类
    /// </summary>
    public class BankTransferConfirmModel
    {
        [Required(ErrorMessage = "汇款单号不能为空")]
        public string PaySN { get; set; }
    }
    /// <summary>
    /// 打印订单模型类
    /// </summary>
    public class PrintOrderModel
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
    /// 确认退货收货模型类
    /// </summary>
    public class ConfirmReceiveReturnModel
    {
        public ConfirmReceiveReturnModel()
        {
            ReturnShipFee = 0M;
            NewReturnShipFee = 0M;
            ReturnShipDesc = "";
            NewReturnShipDesc = "";
        }

        [Range(0, double.MaxValue, ErrorMessage = "配送费用不能小于0")]
        [Required(ErrorMessage = "配送费用不能为空")]
        public decimal ReturnShipFee { get; set; }

        [StringLength(150, ErrorMessage = "退货运费说明不能超过150")]
        public string ReturnShipDesc { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "追加配送费用不能小于0")]
        [Required(ErrorMessage = "追加配送费用不能为空")]
        public decimal NewReturnShipFee { get; set; }

        [StringLength(150, ErrorMessage = "追加退货运费说明不能超过150")]
        public string NewReturnShipDesc { get; set; }
    }

    /// <summary>
    /// 订单退款列表模型类
    /// </summary>
    public class OrderReturnListModel
    {
        public List<OrderReturnInfo> OrderReturnList { get; set; }
        public PageModel PageModel { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int state { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OSN { get; set; }
    }
    /// <summary>
    /// 订单退款列表模型类
    /// </summary>
    public class OrderRefundListModel
    {
        public List<OrderRefundInfo> OrderRefundList { get; set; }
        public PageModel PageModel { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int state { get; set; }
        public string paySystemName { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OSN { get; set; }
    }

    /// <summary>
    /// 订单换货列表模型类
    /// </summary>
    public class OrderChangeListModel
    {
        public OrderChangeListModel()
        {
            state = -1;
        }
        public List<OrderChangeInfo> OrderChangeList { get; set; }
        public PageModel PageModel { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int state { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OSN { get; set; }
    }

    /// <summary>
    /// 订单发货模型类
    /// </summary>
    public class ChangeOrderSendModel
    {
        public ChangeOrderSendModel()
        {
            ShipCoId = -1;
            ShipCoName = "选择配送公司";
        }

        [Required(ErrorMessage = "发货单号不能为空")]
        [StringLength(30, ErrorMessage = "发货单号长度不能超过30")]
        public string ShipSN { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "请选择配送公司")]
        [Required(ErrorMessage = "请选择配送公司")]
        public int ShipCoId { get; set; }

        /// <summary>
        /// 配送公司名称
        /// </summary>
        public string ShipCoName { get; set; }
    }
    /// <summary>
    /// 银行汇款退款确认模型类
    /// </summary>
    public class ConfirmRefundForBanKModel
    {

        [Required(ErrorMessage = "退款单号不能为空")]
        [StringLength(30, ErrorMessage = "退款单号长度不能超过30")]
        public string RefundTranSN { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OrderDetailForRefundModel
    {
        public OrderInfo OrderInfo { get; set; }
        public List<OrderProductInfo> OrderProductList { get; set; }
        public List<OrderActionInfo> OrderActionList { get; set; }
        public UserInfo UserInfo { get; set; }
    }

    /// <summary>
    /// 订单列表模型类
    /// </summary>
    public class SendOrderListModel
    {
        public PageModel PageModel { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public DataTable SendOrderList { get; set; }
        /// <summary>
        /// 店铺id
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OSN { get; set; }
        /// <summary>
        /// 账户名
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee { get; set; }
        /// <summary>
        /// 收货人手机
        /// </summary>
        public string ConsigneeMobile { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderState { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 支付开始时间
        /// </summary>
        public DateTime? PayStartDate { get; set; }
        /// <summary>
        /// 支付结束时间
        /// </summary>
        public DateTime? PayEndDate { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public int Pid { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 是否咖啡选货
        /// </summary>
        public int IsKFXH { get; set; }

    }
    /// <summary>
    /// 拿货详情模型类
    /// </summary>
    public class StockFromDetailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public UserInfo UserInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OrderInfo OrderInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OrderProductInfo OrderProductInfo { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public PartUserInfo FromParentUser1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PartUserInfo FromParentUser2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PartUserInfo FromParentUser3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PartUserInfo FromParentUser4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FromParentCount1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FromParentCount2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FromParentCount3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FromParentCount4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<AgentStockDetailInfo> deteilList { get; set; }
    }
    /// <summary>
    /// 拿货详情模型类
    /// </summary>
    public class SettlePreviewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public UserInfo UserInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OrderInfo OrderInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<OrderProductInfo> OrderProductList { get; set; }
        /// <summary>
        /// 订单结算信息
        /// </summary>
        public OrderSettleInfo OrderSettlePreview { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<AccountDetailInfo> deteilList { get; set; }
    }


    /// <summary>
    /// 模型类
    /// </summary>
    public class OrderApplyListModel
    {
        public PageModel PageModel { get; set; }

        public DataTable OrderApplyList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int OperateUid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OperUserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee { get; set; }
        /// <summary>
        /// 收货人手机
        /// </summary>
        public string ConsigneeMobile { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public int Pid { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class OrderApplyModel
    {
        /// <summary>
        /// 订单信息
        /// </summary>
        public OrderApplyInfo OrderApply { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RegionInfo RegionInfo { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo UserInfo { get; set; }
        
        /// <summary>
        /// 订单商品
        /// </summary>
        public PartProductInfo OrderProduct { get; set; }
       
    }

    /// <summary>
    /// 模型类
    /// </summary>
    public class Kf2SeverbApplyListModel
    {
        public PageModel PageModel { get; set; }

        public DataTable Kf2SeverbApplyList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ApplySN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CardUserName { get; set; }
        
        /// <summary>
        /// 手机
        /// </summary>
        public string CardMobile { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int State { get; set; }
        
    }
}
