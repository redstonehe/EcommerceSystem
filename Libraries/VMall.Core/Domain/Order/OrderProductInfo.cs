using System;

namespace VMall.Core
{
    /// <summary>
    /// 订单商品信息类
    /// </summary>
    public class OrderProductInfo
    {
        private int _recordid;//记录id
        private int _oid = 0;//订单id
        private int _uid = 0;//用户id
        private string _sid = "";//sessionId
        private int _pid=0;//商品id
        private string _psn=string.Empty;//商品编码
        private int _cateid=0;//分类id
        private int _brandid=0;//品牌id
        private int _storeid=0;//店铺id
        private int _storecid=0;//店铺分类id
        private int _storestid = 0;//店铺配送模板id
        private string _name=string.Empty;//商品名称
        private string _showimg=string.Empty;//商品展示图片
        private decimal _discountprice=0M;//商品折扣价格
        private decimal _shopprice=0M;//商品商城价格
        private decimal _costprice=0M;//商品成本价格
        private decimal _marketprice=0M;//商品市场价格
        private int _weight=0;//商品重量
        private int _isreview=0;//是否评价(0代表未评价，1代表已评价)
        private int _realcount = 0;//真实数量
        private int _buycount = 0;//商品购买数量
        private int _sendcount = 0;//商品邮寄数量
        private int _type = 0;//商品类型(0为普遍商品,1为普通商品赠品,2为套装商品赠品,3为套装商品,4满赠商品)
        private int _paycredits = 0;//支付积分
        private int _coupontypeid = 0;//赠送优惠劵类型id
        private int _extcode1 = 0;//普通商品时为单品促销活动id,赠品时为赠品促销活动id,套装商品时为套装促销活动id,满赠赠品时为满赠促销活动id
        private int _extcode2 = 0;//普通商品时为买送促销活动id,赠品时为赠品赠送数量,套装商品时为套装商品数量
        private int _extcode3 = 0;//普通商品时为赠品促销活动id,套装商品时为赠品促销活动id
        private int _extcode4 = 0;//普通商品时为满赠促销活动id
        private int _extcode5 = 0;//普通商品时为满减促销活动id
        private DateTime _addtime=DateTime.Now;//添加时间
        private int _saletype = 0;//产品类型 0-普通商品 1一般贸易 2保税商品 3海外直邮
        private decimal _taxrate = 0M;//产品关税税率
        //扩展字段-不从数据库中取 实际要用到再填充
        private decimal _pv = 0M;//pv值
        private decimal _haimi = 0M;//海米值
        private decimal _hongbaocut = 0M;//红包减免
        private int _minbuycount = 0;//最低购买数量

        private decimal _productpv = 0M;//产品实际pv
        private decimal _producthaimi = 0M;//产品实际海米
        private decimal _producthbcut = 0M;//产品实际最大红包减免

        //扩展字段-不从数据库中取 实际用到再填充
        private int _productstate = -1;//产品上下架状态 0上架 1下架 2代表回收站
        private int _productstocknum = 0;//产品库存量

        private int _fromparentid = 0;//汇购优选代理的上级出货人ID
        private int _fromparent = 0;//汇购优选代理从直接推荐人处获得的产品数量
        private int _fromcompany = 0;//汇购优选代理从公司获得的产品数量

        private int _fromparentid1 = 0;// 事业伙伴出货uid
        private decimal _fromparentamount1 = 0M;//事业伙伴出货库存金额
        private int _fromparentid2 = 0;// 星级出货uid
        private decimal _fromparentamount2 = 0M;//星级出货库存金额
        private int _fromparentid3 = 0;// VIP出货id
        private decimal _fromparentamount3 = 0M;// VIP出货库存金额
        private int _fromparentid4 = 0;// 大区出货id 
        private decimal _fromparentamount4 = 0M;//大区出货库存金额

        private decimal _fromcompanyamount = 0M;//公司出货产品金额
        private int _mallsource = 0;//商城来源

        /// <summary>
        /// 商城来源
        /// </summary>
        public int MallSource
        {
            get { return _mallsource; }
            set { _mallsource = value; }
        }
        /// <summary>
        /// 事业伙伴出货uid
        /// </summary>
        public int FromParentId1
        {
            get { return _fromparentid1; }
            set { _fromparentid1 = value; }
        }
        /// <summary>
        /// 事业伙伴出货库存金额
        /// </summary>
        public decimal FromParentAmount1
        {
            get { return _fromparentamount1; }
            set { _fromparentamount1 = value; }
        }
        /// <summary>
        /// 星级出货uid
        /// </summary>
        public int FromParentId2
        {
            get { return _fromparentid2; }
            set { _fromparentid2 = value; }
        } 
        /// <summary>
        /// 星级出货库存金额
        /// </summary>
        public decimal FromParentAmount2
        {
            get { return _fromparentamount2; }
            set { _fromparentamount2 = value; }
        }
        /// <summary>
        /// VIP出货id
        /// </summary>
        public int FromParentId3
        {
            get { return _fromparentid3; }
            set { _fromparentid3 = value; }
        }
        /// <summary>
        /// VIP出货库存金额
        /// </summary>
        public decimal FromParentAmount3
        {
            get { return _fromparentamount3; }
            set { _fromparentamount3 = value; }
        }
        /// <summary>
        /// 大区出货id 
        /// </summary>
        public int FromParentId4
        {
            get { return _fromparentid4; }
            set { _fromparentid4 = value; }
        }
        /// <summary>
        ///大区出货库存金额
        /// </summary>
        public decimal FromParentAmount4
        {
            get { return _fromparentamount4; }
            set { _fromparentamount4 = value; }
        }

        /// <summary>
        /// 公司出货产品金额
        /// </summary>
        public decimal FromCompanyAmount
        {
            get { return _fromcompanyamount; }
            set { _fromcompanyamount = value; }
        }
        /// <summary>
        /// 汇购优选代理的上级出货人ID
        /// </summary>
        public int FromParentId
        {
            get { return _fromparentid; }
            set { _fromparentid = value; }
        }

        /// <summary>
        /// 汇购优选代理从直接推荐人处获得的产品数量
        /// </summary>
        public int FromParent
        {
            get { return _fromparent; }
            set { _fromparent = value; }
        }

        /// <summary>
        /// 汇购优选代理从公司获得的产品数量
        /// </summary>
        public int FromCompany
        {
            get { return _fromcompany; }
            set { _fromcompany = value; }
        }

        /// <summary>
        /// 产品上下架状态 0上架 1下架
        /// </summary>
        public int ProductState
        {
            get { return _productstate; }
            set { _productstate = value; }
        }
        /// <summary>
        /// 产品库存量
        /// </summary>
        public int ProductStockNum
        {
            get { return _productstocknum; }
            set { _productstocknum = value; }
        }

        /// <summary>
        /// 产品实际最大红包减免
        /// </summary>
        public decimal ProductHBCut
        {
            get { return _producthbcut; }
            set { _producthbcut = value; }
        }
        /// <summary>
        /// 产品实际pv
        /// </summary>
        public decimal ProductPV
        {
            get { return _productpv; }
            set { _productpv = value; }
        }
        /// <summary>
        /// 产品实际海米
        /// </summary>
        public decimal ProductHaiMi
        {
            get { return _producthaimi; }
            set { _producthaimi = value; }
        }
        /// <summary>
        /// 记录id
        /// </summary>
        public int RecordId
        {
            get { return _recordid; }
            set { _recordid = value; }
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
        /// 用户id
        /// </summary>
        public int Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        /// <summary>
        /// sessionId
        /// </summary>
        public string Sid
        {
            get { return _sid; }
            set { _sid = value.TrimEnd(); }
        }
        /// <summary>
        /// 商品id
        /// </summary>
        public int Pid
        {
            get { return _pid; }
            set { _pid = value; }
        }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string PSN
        {
            get { return _psn; }
            set { _psn = value.TrimEnd(); }
        }
        /// <summary>
        /// 分类id
        /// </summary>
        public int CateId
        {
            get { return _cateid; }
            set { _cateid = value; }
        }
        /// <summary>
        /// 品牌id
        /// </summary>
        public int BrandId
        {
            get { return _brandid; }
            set { _brandid = value; }
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
        /// 店铺分类id
        /// </summary>
        public int StoreCid
        {
            get { return _storecid; }
            set { _storecid = value; }
        }
        /// <summary>
        /// 店铺配送模板id
        /// </summary>
        public int StoreSTid
        {
            set { _storestid = value; }
            get { return _storestid; }
        }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 商品展示图片
        /// </summary>
        public string ShowImg
        {
            get { return _showimg; }
            set { _showimg = value; }
        }
        /// <summary>
        /// 商品折扣价格
        /// </summary>
        public decimal DiscountPrice
        {
            get { return _discountprice; }
            set { _discountprice = value; }
        }
        /// <summary>
        /// 商品商城价格
        /// </summary>
        public decimal ShopPrice
        {
            get { return _shopprice; }
            set { _shopprice = value; }
        }
        /// <summary>
        /// 商品成本价格
        /// </summary>
        public decimal CostPrice
        {
            get { return _costprice; }
            set { _costprice = value; }
        }
        /// <summary>
        /// 商品市场价格
        /// </summary>
        public decimal MarketPrice
        {
            get { return _marketprice; }
            set { _marketprice = value; }
        }
        /// <summary>
        /// 商品重量
        /// </summary>
        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
        /// <summary>
        /// 是否评价(0代表未评价，1代表已评价)
        /// </summary>
        public int IsReview
        {
            get { return _isreview; }
            set { _isreview = value; }
        }
        /// <summary>
        /// 真实数量
        /// </summary>
        public int RealCount
        {
            get { return _realcount; }
            set { _realcount = value; }
        }
        /// <summary>
        /// 商品购买数量
        /// </summary>
        public int BuyCount
        {
            get { return _buycount; }
            set { _buycount = value; }
        }
        /// <summary>
        /// 商品邮寄数量
        /// </summary>
        public int SendCount
        {
            get { return _sendcount; }
            set { _sendcount = value; }
        }
        /// <summary>
        /// 商品类型(0为普遍商品,1为普通商品赠品,2为套装商品赠品,3为套装商品,4满赠商品)
        /// </summary>
        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /// <summary>
        /// 支付积分
        /// </summary>
        public int PayCredits
        {
            get { return _paycredits; }
            set { _paycredits = value; }
        }
        /// <summary>
        /// 赠送优惠劵类型id
        /// </summary>
        public int CouponTypeId
        {
            get { return _coupontypeid; }
            set { _coupontypeid = value; }
        }
        /// <summary>
        /// 普通商品时为单品促销活动id,赠品时为赠品促销活动id,套装商品时为套装促销活动id,满赠赠品时为满赠促销活动id
        /// </summary>
        public int ExtCode1
        {
            get { return _extcode1; }
            set { _extcode1 = value; }
        }
        /// <summary>
        /// 普通商品时为买送促销活动id,赠品时为赠品赠送数量,套装商品时为套装商品数量
        /// </summary>
        public int ExtCode2
        {
            get { return _extcode2; }
            set { _extcode2 = value; }
        }
        /// <summary>
        /// 普通商品时为赠品促销活动id,套装商品时为赠品促销活动id
        /// </summary>
        public int ExtCode3
        {
            get { return _extcode3; }
            set { _extcode3 = value; }
        }
        /// <summary>
        /// 普通商品时为满赠促销活动id
        /// </summary>
        public int ExtCode4
        {
            get { return _extcode4; }
            set { _extcode4 = value; }
        }
        /// <summary>
        /// 普通商品时为满减促销活动id
        /// </summary>
        public int ExtCode5
        {
            get { return _extcode5; }
            set { _extcode5 = value; }
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
        /// 产品类型 0-普通商品 1一般贸易 2保税商品 3海外直邮
        /// </summary>
        public int SaleType
        {
            get { return _saletype; }
            set { _saletype = value; }
        }
        /// <summary>
        /// 产品关税税率
        /// </summary>
        public decimal TaxRate
        {
            get { return _taxrate; }
            set { _taxrate = value; }
        }
        /// <summary>
        /// PV
        /// </summary>
        public decimal PV
        {
            set { _pv = value; }
            get { return _pv; }
        }
        /// <summary>
        /// 海米值
        /// </summary>
        public decimal HaiMi
        {
            set { _haimi = value; }
            get { return _haimi; }
        }
        /// <summary>
        /// 红包减免
        /// </summary>
        public decimal HongBaoCut
        {
            set { _hongbaocut = value; }
            get { return _hongbaocut; }
        }
        /// <summary>
        /// 最低起购数量
        /// </summary>
        public int MinBuyCount
        {
            set { _minbuycount = value; }
            get { return _minbuycount; }
        }
    }
}

