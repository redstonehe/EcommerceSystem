using System;

namespace VMall.Core
{
    /// <summary>
    /// 订单信息类
    /// </summary>
    public class OrderInfo
    {
        private int _oid;//订单id
        private string _osn;//订单编号
        private int _uid;//用户id

        private int _orderstate;//订单状态

        private decimal _productamount;//商品合计
        private decimal _orderamount;//订单合计
        private decimal _surplusmoney;//剩余金钱

        private int _parentid;//父id
        private int _isreview;//是否评价
        private DateTime _addtime = DateTime.Now;//添加时间
        private int _storeid;//店铺id
        private string _storename;//店铺名称
        private string _shipsn;//配送单号
        private int _shipcoid;//配送公司id
        private string _shipconame;//配送公司名称
        private DateTime _shiptime;//配送时间
        private string _paysn;//支付单号
        private string _paysystemname;//支付方式系统名
        private string _payfriendname;//支付方式昵称
        private int _paymode;//支付方式(0代表货到付款，1代表在线付款，2代表线下付款)
        private DateTime _paytime;//支付时间

        private int _regionid;//配送区域id
        private string _consignee;//收货人
        private string _mobile;//手机号
        private string _phone;//固话号
        private string _email;//邮箱
        private string _zipcode = string.Empty;//邮政编码
        private string _address;//详细地址
        private DateTime _besttime = DateTime.Now;//最佳送货时间

        private decimal _shipfee;//配送费用
        private decimal _payfee;//支付费用
        private int _fullcut;//满减
        private decimal _discount;//折扣
        private int _paycreditcount;//支付积分数量
        private decimal _paycreditmoney;//支付积分金额
        private decimal _couponmoney;//优惠劵金额
        private int _weight;//重量

        private string _buyerremark;//买家备注
        private string _ip;//ip地址
        private int _ordersource = 0;//订单来源：1为汇购 2为天鹰 3为全球购 10直销后台
        private decimal _taxmount = 0M;//关税合计
        private DateTime _receivingtime;//确认收货时间
        private string _paydevice = string.Empty;//支付设备终端类型
        private string _invoice = string.Empty;//发票抬头
        private decimal _haimidiscount = 0M;//海米抵现 
        private decimal _hongbaodiscount = 0M;//红包减免抵现
        private int _settlestate = 0;//订单结算状态  0为订单未参与过结算 1 预结算(预收入).2已结算(确定收入).3 预结算取消(预收入撤销)

        private int _isextendreceive = 0;//是否延长收货，0为不延长，1为延长，期限5天
        private int _isdelete = 0;//是否删除订单（非物理删除，只是前台不显示） 0不删除 1为删除

        private int _returntype = 0;//1为退货审核中 2为退货审核完成 3为退货收货确认 4退款完成
        private int _changetype = 0;// 1为换货审核 2为换货审核完成 3为换货收货确认 4换货发货
        private decimal _cashdiscount = 0M;//汇购卡优惠
        private string _extorderid = string.Empty;//外部订单号id
        private int _mainoid =0;//主订单id
        private int _suboid = 0;//子订单id

        private decimal _agentdiscount = 0;//代理账户优惠
        private decimal _commisiondiscount = 0;//佣金账户优惠

        private int _mallsource = 0;//订单商城来源
        private int _actualuid = 0;//订单实际uid
        private string _invoicemore = "";//订单发票增量信息
        private string _adminremark = "";//订单后台操作备注
        /// <summary>
        ///订单后台操作备注
        /// </summary>
        public string AdminRemark
        {
            set { _adminremark = value; }
            get { return _adminremark; }
        }
        /// <summary>
        ///订单发票增量信息
        /// </summary>
        public string InvoiceMore
        {
            set { _invoicemore = value; }
            get { return _invoicemore; }
        }
        /// <summary>
        ///订单实际uid
        /// </summary>
        public int ActualUid
        {
            set { _actualuid = value; }
            get { return _actualuid; }
        }

        /// <summary>
        ///订单商城来源
        /// </summary>
        public int MallSource
        {
            set { _mallsource = value; }
            get { return _mallsource; }
        }

        public OrderInfo()
        {
            DateTime time = new DateTime(1900, 1, 1);
            _parentid = 0;
            _shipsn = "";
            _shipcoid = 0;
            _shipconame = "";
            _shiptime = time;
            _paysn = "";
            _paytime = time;
            _discount = 0M;
            _receivingtime = time;
        }
        /// <summary>
        /// 代理账户优惠
        /// </summary>
        public decimal AgentDiscount
        {
            get { return _agentdiscount; }
            set { _agentdiscount = value; }
        }
        /// <summary>
        /// 佣金账户优惠
        /// </summary>
        public decimal CommisionDiscount
        {
            get { return _commisiondiscount; }
            set { _commisiondiscount = value; }
        }
        /// <summary>
        /// 子订单id
        /// </summary>
        public int SubOid
        {
            get { return _suboid; }
            set { _suboid = value; }
        }
        /// <summary>
        /// 主订单id
        /// </summary>
        public int MainOid
        {
            get { return _mainoid; }
            set { _mainoid = value; }
        }
        /// <summary>
        /// 外部订单号id
        /// </summary>
        public string ExtOrderId {
            get { return _extorderid; }
            set { _extorderid = value.Trim(); }
        }
        /// <summary>
        /// 汇购卡优惠
        /// </summary>
        public decimal CashDiscount
        {
            get { return _cashdiscount; }
            set { _cashdiscount = value; }
        }

        /// <summary>
        /// 1为退货审核 2为审核完成
        /// </summary>
        public int ReturnType
        {
            get { return _returntype; }
            set { _returntype = value; }
        }

        /// <summary>
        ///  1为换货审核 2为换货审核完成 3为换货收货确认 4换货发货
        /// </summary>
        public int ChangeType
        {
            get { return _changetype; }
            set { _changetype = value; }
        }

        /// <summary>
        /// 是否延长收货，0为不延长，1为延长，期限5天
        /// </summary>
        public int IsExtendReceive
        {
            get { return _isextendreceive; }
            set { _isextendreceive = value; }
        }
        /// <summary>
        /// 是否删除订单（非物理删除，只是前台不显示） 0不删除 1为删除
        /// </summary>
        public int IsDelete
        {
            get { return _isdelete; }
            set { _isdelete = value; }
        }

        /// <summary>
        /// 订单结算状态  0为订单未参与过结算 1 预结算(预收入).2已结算(确定收入).3 预结算取消(预收入撤销)
        /// </summary>
        public int SettleState
        {
            get { return _settlestate; }
            set { _settlestate = value; }
        }
        /// <summary>
        /// 海米抵现
        /// </summary>
        public decimal HaiMiDiscount
        {
            get { return _haimidiscount; }
            set { _haimidiscount = value; }
        }
        /// <summary>
        /// 红包减免抵现
        /// </summary>
        public decimal HongBaoDiscount
        {
            get { return _hongbaodiscount; }
            set { _hongbaodiscount = value; }
        }
        /// <summary>
        /// 发票抬头
        /// </summary>
        public string Invoice
        {
            get { return _invoice.Trim(); }
            set { _invoice = value.TrimEnd(); }
        }

        /// <summary>
        /// 订单id
        /// </summary>
        public int Oid
        {
            get { return _oid; }
            set { _oid = value; }
        }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OSN
        {
            get { return _osn; }
            set { _osn = value.TrimEnd(); }
        }
        /// <summary>
        /// 用户id
        /// </summary>
        public int Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderState
        {
            get { return _orderstate; }
            set { _orderstate = value; }
        }

        /// <summary>
        /// 商品合计
        /// </summary>
        public decimal ProductAmount
        {
            get { return _productamount; }
            set { _productamount = value; }
        }
        /// <summary>
        /// 订单合计
        /// </summary>
        public decimal OrderAmount
        {
            get { return _orderamount; }
            set { _orderamount = value; }
        }
        /// <summary>
        /// 剩余金钱
        /// </summary>
        public decimal SurplusMoney
        {
            get { return _surplusmoney; }
            set { _surplusmoney = value; }
        }

        /// <summary>
        /// 父id
        /// </summary>
        public int ParentId
        {
            get { return _parentid; }
            set { _parentid = value; }
        }
        /// <summary>
        /// 是否评价
        /// </summary>
        public int IsReview
        {
            get { return _isreview; }
            set { _isreview = value; }
        }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime
        {
            get { return _addtime; }
            set { _addtime = value; }
        }
        /// <summary>
        /// 店铺id
        /// </summary>
        public int StoreId
        {
            get { return _storeid; }
            set { _storeid = value; }
        }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName
        {
            get { return _storename; }
            set { _storename = value.TrimEnd(); }
        }
        /// <summary>
        /// 配送单号
        /// </summary>
        public string ShipSN
        {
            get { return _shipsn; }
            set { _shipsn = value.TrimEnd(); }
        }
        /// <summary>
        /// 配送公司id
        /// </summary>
        public int ShipCoId
        {
            get { return _shipcoid; }
            set { _shipcoid = value; }
        }
        /// <summary>
        /// 配送公司名称
        /// </summary>
        public string ShipCoName
        {
            get { return _shipconame; }
            set { _shipconame = value.TrimEnd(); }
        }
        /// <summary>
        /// 配送时间
        /// </summary>
        public DateTime ShipTime
        {
            get { return _shiptime; }
            set { _shiptime = value; }
        }
        /// <summary>
        /// 支付单号
        /// </summary>
        public string PaySN
        {
            get { return _paysn; }
            set { _paysn = value.TrimEnd(); }
        }
        /// <summary>
        /// 支付方式系统名
        /// </summary>
        public string PaySystemName
        {
            get { return _paysystemname; }
            set { _paysystemname = value.TrimEnd(); }
        }
        /// <summary>
        /// 支付方式昵称
        /// </summary>
        public string PayFriendName
        {
            get { return _payfriendname; }
            set { _payfriendname = value.TrimEnd(); }
        }
        /// <summary>
        /// 支付方式(0代表货到付款，1代表在线付款，2代表线下付款)
        /// </summary>
        public int PayMode
        {
            get { return _paymode; }
            set { _paymode = value; }
        }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PayTime
        {
            get { return _paytime; }
            set { _paytime = value; }
        }

        /// <summary>
        /// 配送区域id
        /// </summary>
        public int RegionId
        {
            get { return _regionid; }
            set { _regionid = value; }
        }
        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee
        {
            get { return _consignee; }
            set { _consignee = value.TrimEnd(); }
        }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile
        {
            get { return _mobile; }
            set { _mobile = value.TrimEnd(); }
        }
        /// <summary>
        /// 固话号
        /// </summary>
        public string Phone
        {
            get { return _phone; }
            set { _phone = value.TrimEnd(); }
        }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get { return _email; }
            set { _email = value.TrimEnd(); }
        }
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string ZipCode
        {
            get { return _zipcode; }
            set { _zipcode = value.TrimEnd(); }
        }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address
        {
            get { return _address; }
            set { _address = value.TrimEnd(); }
        }
        /// <summary>
        /// 最佳送货时间
        /// </summary>
        public DateTime BestTime
        {
            get { return _besttime; }
            set { _besttime = value; }
        }

        /// <summary>
        /// 配送费用
        /// </summary>
        public decimal ShipFee
        {
            get { return _shipfee; }
            set { _shipfee = value; }
        }
        /// <summary>
        /// 支付费用
        /// </summary>
        public decimal PayFee
        {
            get { return _payfee; }
            set { _payfee = value; }
        }
        /// <summary>
        /// 满减
        /// </summary>
        public int FullCut
        {
            get { return _fullcut; }
            set { _fullcut = value; }
        }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount
        {
            get { return _discount; }
            set { _discount = value; }
        }
        /// <summary>
        /// 支付积分数量
        /// </summary>
        public int PayCreditCount
        {
            get { return _paycreditcount; }
            set { _paycreditcount = value; }
        }
        /// <summary>
        /// 支付积分金额
        /// </summary>
        public decimal PayCreditMoney
        {
            get { return _paycreditmoney; }
            set { _paycreditmoney = value; }
        }
        /// <summary>
        /// 优惠劵金额
        /// </summary>
        public decimal CouponMoney
        {
            get { return _couponmoney; }
            set { _couponmoney = value; }
        }
        /// <summary>
        /// 重量
        /// </summary>
        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        /// <summary>
        /// 买家备注
        /// </summary>
        public string BuyerRemark
        {
            get { return _buyerremark; }
            set { _buyerremark = value.TrimEnd(); }
        }
        /// <summary>
        /// ip地址
        /// </summary>
        public string IP
        {
            get { return _ip; }
            set { _ip = value.TrimEnd(); }
        }
        public int OrderSource
        {
            get { return _ordersource; }
            set { _ordersource = value; }
        }
        /// <summary>
        /// 关税合计
        /// </summary>
        public decimal TaxAmount
        {
            get { return _taxmount; }
            set { _taxmount = value; }
        }
        /// <summary>
        /// 确认收货时间
        /// </summary>
        public DateTime ReceivingTime
        {
            get { return _receivingtime; }
            set { _receivingtime = value; }
        }
        /// <summary>
        /// 支付设备终端类型
        /// </summary>
        public string PayDevice
        {
            get { return _paydevice; }
            set { _paydevice = value.TrimEnd(); }
        }
    }
}

