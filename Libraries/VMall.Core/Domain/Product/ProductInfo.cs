using System;
using Utility;
using Entity.Base;

namespace VMall.Core
{
    /// <summary>
    /// 商品部分信息类
    /// </summary>
    [Serializable]
    [SqlTable("hlh_products")]
    public class PartProductInfo
    {
        private int _pid;//商品id
        private string _psn = "";//商品货号
        private int _cateid = 0;//商品分类id
        private int _brandid = 0;//商品品牌id
        private int _storeid = 0;//店铺id
        private int _storecid = 0;//店铺分类id
        private int _storestid = 0;//店铺配送模板id
        private int _skugid = 0;//商品sku组id
        private string _name = "";//商品名称
        private decimal _shopprice = 0M;//商品商城价
        private decimal _marketprice = 0M;//商品市场价
        private decimal _costprice = 0M;//商品成本价
        private int _state = 0;//0代表上架，1代表下架，2代表回收站
        private int _isbest = 0;//商品是否精品
        private int _ishot = 0;//商品是否热销
        private int _isnew = 0;//商品是否新品
        private int _displayorder = 0;//商品排序
        private int _weight = 0;//商品重量
        private string _showimg = "";//商品展示图片
        private int _salecount = 0;//销售数
        private int _visitcount = 0;//访问数
        private int _reviewcount = 0;//评价数
        private int _star1 = 0;//评价星星1
        private int _star2 = 0;//评价星星2
        private int _star3 = 0;//评价星星3
        private int _star4 = 0;//评价星星4
        private int _star5 = 0;//评价星星5
        private DateTime _addtime = DateTime.Now;//商品添加时间
        private decimal _pv = 0M;//pv值
        private decimal _haimi = 0M;//海米值
        private int _saletype = 0;//全球购产品类型 0-普通商品 1一般贸易 2保税商品 3海外直邮
        private decimal _taxrate = 0M;//产品关税率
        private decimal _hongbaocut = 0M;//红包减免

        private int _minbuycount = 0;//最低起购件数  0代表没有限制 大于0代表最低起购件数

        private string _subtitle = "";//产品副标题

        private int _showorder = 0;//排序区间
        private string _unit = string.Empty;//产品规格

        private int _settlepercent = 0;//结算比例
        private int _rebuycycle = 0;//复购周期(单位：天)
        private int _relaterebuypid = 0;//关联复购产品

        /// <summary>
        /// 关联复购产品
        /// </summary>
        [SqlField]
        public int RelateReBuyPid
        {
            set { _relaterebuypid = value; }
            get { return _relaterebuypid; }
        }
        /// <summary>
        /// 复购周期(单位：天)
        /// </summary>
        [SqlField]
        public int ReBuyCycle
        {
            set { _rebuycycle = value; }
            get { return _rebuycycle; }
        }
        /// <summary>
        /// 结算比例
        /// </summary>
        [SqlField]
        public int SettlePercent
        {
            set { _settlepercent = value; }
            get { return _settlepercent; }
        }
        /// <summary>
        /// 产品规格
        /// </summary>
        [SqlField]
        public string Unit
        {
            set { _unit = value; }
            get { return _unit; }
        }
        /// <summary>
        /// 排序区间
        /// </summary>
        [SqlField]
        public int ShowOrder
        {
            set { _showorder = value; }
            get { return _showorder; }
        }
        /// <summary>
        /// 产品副标题
        /// </summary>
        [SqlField]
        public string SubTitle
        {
            set { _subtitle = value; }
            get { return _subtitle; }
        }
        /// <summary>
        /// 最低起购件数  0代表没有限制 大于0代表最低起购件数
        /// </summary>
        [SqlField]
        public int MinBuyCount
        {
            set { _minbuycount = value; }
            get { return _minbuycount; }
        }
        /// <summary>
        /// 红包减免
        /// </summary>
        [SqlField]
        public decimal HongBaoCut
        {
            set { _hongbaocut = value; }
            get { return _hongbaocut; }
        }
        /// <summary>
        /// 商品id
        /// </summary>
         [SqlField(IsPrimaryKey = true, IsAutoId = true)]
        public int Pid
        {
            set { _pid = value; }
            get { return _pid; }
        }
        /// <summary>
        /// 商品货号
        /// </summary>
         [SqlField]
         public string PSN
        {
            set { _psn = value.TrimEnd(); }
            get { return _psn; }
        }
        /// <summary>
        /// 商品分类id
        /// </summary>
         [SqlField]
         public int CateId
        {
            set { _cateid = value; }
            get { return _cateid; }
        }
        /// <summary>
        /// 商品品牌id
        /// </summary>
         [SqlField]
         public int BrandId
        {
            set { _brandid = value; }
            get { return _brandid; }
        }
        /// <summary>
        /// 店铺id
        /// </summary>
         [SqlField]
         public int StoreId
        {
            set { _storeid = value; }
            get { return _storeid; }
        }
        /// <summary>
        /// 店铺分类id
        /// </summary>
         [SqlField]
         public int StoreCid
        {
            set { _storecid = value; }
            get { return _storecid; }
        }
        /// <summary>
        /// 店铺配送模板id
        /// </summary>
         [SqlField]
         public int StoreSTid
        {
            set { _storestid = value; }
            get { return _storestid; }
        }
        /// <summary>
        /// 商品sku组id
        /// </summary>
         [SqlField]
         public int SKUGid
        {
            set { _skugid = value; }
            get { return _skugid; }
        }
        /// <summary>
        /// 商品名称
        /// </summary>
         [SqlField]
         public string Name
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 商品商城价
        /// </summary>
         [SqlField]
         public decimal ShopPrice
        {
            set { _shopprice = value; }
            get { return _shopprice; }
        }
        /// <summary>
        /// 商品市场价
        /// </summary>
         [SqlField]
         public decimal MarketPrice
        {
            set { _marketprice = value; }
            get { return _marketprice; }
        }
        /// <summary>
        /// 商品成本价
        /// </summary>
         [SqlField]
         public decimal CostPrice
        {
            set { _costprice = value; }
            get { return _costprice; }
        }
        /// <summary>
        /// 0代表上架，1代表下架，2代表回收站
        /// </summary>
         [SqlField]
         public int State
        {
            set { _state = value; }
            get { return _state; }
        }
        /// <summary>
        /// 商品是否精品
        /// </summary>
         [SqlField]
         public int IsBest
        {
            set { _isbest = value; }
            get { return _isbest; }
        }
        /// <summary>
        /// 商品是否热销
        /// </summary>
         [SqlField]
         public int IsHot
        {
            set { _ishot = value; }
            get { return _ishot; }
        }
        /// <summary>
        /// 商品是否新品
        /// </summary>
         [SqlField]
         public int IsNew
        {
            set { _isnew = value; }
            get { return _isnew; }
        }
        /// <summary>
        /// 商品排序
        /// </summary>
         [SqlField]
         public int DisplayOrder
        {
            set { _displayorder = value; }
            get { return _displayorder; }
        }
        /// <summary>
        /// 商品重量
        /// </summary>
         [SqlField]
         public int Weight
        {
            set { _weight = value; }
            get { return _weight; }
        }
        /// <summary>
        /// 商品展示图片
        /// </summary>
         [SqlField]
         public string ShowImg
        {
            set { _showimg = value; }
            get { return _showimg; }
        }
        /// <summary>
        /// 销售数
        /// </summary>
         [SqlField]
         public int SaleCount
        {
            set { _salecount = value; }
            get { return _salecount; }
        }
        /// <summary>
        /// 访问数
        /// </summary>
         [SqlField]
         public int VisitCount
        {
            set { _visitcount = value; }
            get { return _visitcount; }
        }
        /// <summary>
        /// 评价数
        /// </summary>
         [SqlField]
         public int ReviewCount
        {
            set { _reviewcount = value; }
            get { return _reviewcount; }
        }
        /// <summary>
        /// 评价星星1
        /// </summary>
         [SqlField]
         public int Star1
        {
            set { _star1 = value; }
            get { return _star1; }
        }
        /// <summary>
        /// 评价星星2
        /// </summary>
         [SqlField]
         public int Star2
        {
            set { _star2 = value; }
            get { return _star2; }
        }
        /// <summary>
        /// 评价星星3
        /// </summary>
         [SqlField]
         public int Star3
        {
            set { _star3 = value; }
            get { return _star3; }
        }
        /// <summary>
        /// 评价星星4
        /// </summary>
         [SqlField]
         public int Star4
        {
            set { _star4 = value; }
            get { return _star4; }
        }
        /// <summary>
        /// 评价星星5
        /// </summary>
         [SqlField]
         public int Star5
        {
            set { _star5 = value; }
            get { return _star5; }
        }
        /// <summary>
        /// 商品添加时间
        /// </summary>
         [SqlField]
         public DateTime AddTime
        {
            set { _addtime = value; }
            get { return _addtime; }
        }
        /// <summary>
        /// PV
        /// </summary>
         [SqlField]
         public decimal PV
        {
            set { _pv = value; }
            get { return _pv; }
        }
        /// <summary>
        /// 海米值
        /// </summary>
         [SqlField]
         public decimal HaiMi
        {
            set { _haimi = value; }
            get { return _haimi; }
        }
        /// <summary>
        /// 全球购产品类型  0-普通商品 1一般贸易 2保税商品 3海外直邮
        /// </summary>
        [SqlField]
        public int SaleType
        {
            set { _saletype = value; }
            get { return _saletype; }
        }
        /// <summary>
        /// 产品关税率 浮点数 50 代表 50%
        /// </summary>
        [SqlField]
        public decimal TaxRate
        {
            set { _taxrate = value; }
            get { return _taxrate; }
        }
        public int GetStarLevel()
        {
            int goodStars = Star1 + Star2 + Star3;
            int allStars = goodStars + Star4 + Star5;

            if (allStars == 0)
                return 100;
            return goodStars * 100 / allStars;
        }
    }

    /// <summary>
    /// 商品信息类
    /// </summary>
    public class ProductInfo : PartProductInfo
    {
        private string _description = "";//商品介绍-电脑版
        private string _productparam = string.Empty;//商品参数
        private string _mobiledescription = "";//商品介绍-电脑版
        private string _videourl = "";//产品视频外链
        /// <summary>
        /// 电脑版商品介绍
        /// </summary>
        public string Description
        {
            set { _description = value; }
            get { return _description; }
        }
        /// <summary>
        /// 商品参数
        /// </summary>
        public string ProductParam
        {
            set { _productparam = value; }
            get { return _productparam; }
        }
        /// <summary>
        /// 手机版商品介绍
        /// </summary>
        public string MobileDescription
        {
            set { _mobiledescription = value; }
            get { return _mobiledescription; }
        }
        /// <summary>
        /// 手机版商品介绍
        /// </summary>
        public string VideoUrl
        {
            set { _videourl = value; }
            get { return _videourl; }
        }
    }

    /// <summary>
    /// 店铺商品信息类
    /// </summary>
    public class StoreProductInfo : PartProductInfo
    {
        private string _storename = "";//店铺名称

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName
        {
            set { _storename = value.TrimEnd(); }
            get { return _storename; }
        }
    }
}

