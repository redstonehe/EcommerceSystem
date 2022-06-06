using System;

namespace VMall.Core
{
    /// <summary>
    /// 店铺信息类
    /// </summary>
    public class StoreInfo
    {
        private int _storeid;//店铺id
        private int _state;//状态(0代表营业,1代表关闭)
        private string _name;//名称
        private int _regionid;//区域id
        private int _storerid;//等级id
        private int _storeiid;//行业id
        private string _logo;//logo
        private DateTime _createtime;//创建时间
        private string _mobile;//手机
        private string _phone;//固定电话
        private string _qq;//qq
        private string _ww;//阿里旺旺
        private decimal _depoint;//商品描述评分
        private decimal _sepoint;//商家服务评分
        private decimal _shpoint;//商家配送评分
        private int _honesties;//店铺诚信值
        private DateTime _stateendtime;//状态结束时间
        private string _theme;//主题
        private string _banner;//Banner
        private string _announcement;//公告
        private string _description;//描述

        private decimal _amount1 = 0;// 优惠价格梯度1
        private decimal _discount1;//优惠折扣1
        private decimal _amount2 = 0;// 优惠价格梯度2
        private decimal _discount2;//优惠折扣2
        private decimal _amount3 = 0;// 优惠价格梯度3
        private decimal _discount3;//优惠折扣3
        private decimal _amount4 = 0;// 优惠价格梯度4
        private decimal _discount4;//优惠折扣4
        private decimal _amount5 = 0;// 优惠价格梯度5
        private decimal _discount5;//优惠折扣5
        private int _mallsource;//平台来源

        /// <summary>
        /// 平台来源
        /// </summary>
        public int MallSource
        {
            get { return _mallsource; }
            set { _mallsource = value; }
        }
        /// <summary>
        /// 优惠价格梯度1
        /// </summary>
        public decimal Amount1
        {
            get { return _amount1; }
            set { _amount1 = value; }
        }/// <summary>
        /// 优惠折扣1
        /// </summary>
        public decimal Discount1
        {
            get { return _discount1; }
            set { _discount1 = value; }
        }
        /// <summary>
        /// 优惠价格梯度2
        /// </summary>
        public decimal Amount2
        {
            get { return _amount2; }
            set { _amount2 = value; }
        }/// <summary>
        /// 优惠折扣2
        /// </summary>
        public decimal Discount2
        {
            get { return _discount2; }
            set { _discount2 = value; }
        }
        /// <summary>
        /// 优惠价格梯度3
        /// </summary>
        public decimal Amount3
        {
            get { return _amount3; }
            set { _amount3 = value; }
        }/// <summary>
        /// 优惠折扣3
        /// </summary>
        public decimal Discount3
        {
            get { return _discount3; }
            set { _discount3 = value; }
        }
        /// <summary>
        /// 优惠价格梯度4
        /// </summary>
        public decimal Amount4
        {
            get { return _amount4; }
            set { _amount4 = value; }
        }/// <summary>
        /// 优惠折扣4
        /// </summary>
        public decimal Discount4
        {
            get { return _discount4; }
            set { _discount4 = value; }
        }
        /// <summary>
        /// 优惠价格梯度5
        /// </summary>
        public decimal Amount5
        {
            get { return _amount5; }
            set { _amount5 = value; }
        }/// <summary>
        /// 优惠折扣5
        /// </summary>
        public decimal Discount5
        {
            get { return _discount5; }
            set { _discount5 = value; }
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
        /// 状态(0代表营业,1代表关闭)
        /// </summary>
        public int State
        {
            get { return _state; }
            set { _state = value; }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value.TrimEnd(); }
        }
        /// <summary>
        /// 区域id
        /// </summary>
        public int RegionId
        {
            get { return _regionid; }
            set { _regionid = value; }
        }
        /// <summary>
        /// 等级id
        /// </summary>
        public int StoreRid
        {
            get { return _storerid; }
            set { _storerid = value; }
        }
        /// <summary>
        /// 行业id
        /// </summary>
        public int StoreIid
        {
            get { return _storeiid; }
            set { _storeiid = value; }
        }
        /// <summary>
        /// logo
        /// </summary>
        public string Logo
        {
            get { return _logo; }
            set { _logo = value.TrimEnd(); }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile
        {
            get { return _mobile; }
            set { _mobile = value.TrimEnd(); }
        }
        /// <summary>
        /// 固定电话
        /// </summary>
        public string Phone
        {
            get { return _phone; }
            set { _phone = value.TrimEnd(); }
        }
        /// <summary>
        /// qq
        /// </summary>
        public string QQ
        {
            get { return _qq; }
            set { _qq = value.TrimEnd(); }
        }
        /// <summary>
        /// 阿里旺旺
        /// </summary>
        public string WW
        {
            get { return _ww; }
            set { _ww = value.TrimEnd(); }
        }
        /// <summary>
        /// 商品描述评分
        /// </summary>
        public decimal DePoint
        {
            get { return _depoint; }
            set { _depoint = value; }
        }
        /// <summary>
        /// 商家服务评分
        /// </summary>
        public decimal SePoint
        {
            get { return _sepoint; }
            set { _sepoint = value; }
        }
        /// <summary>
        /// 商家配送评分
        /// </summary>
        public decimal ShPoint
        {
            get { return _shpoint; }
            set { _shpoint = value; }
        }
        /// <summary>
        /// 店铺诚信值
        /// </summary>
        public int Honesties
        {
            get { return _honesties; }
            set { _honesties = value; }
        }
        /// <summary>
        /// 状态结束时间
        /// </summary>
        public DateTime StateEndTime
        {
            get { return _stateendtime; }
            set { _stateendtime = value; }
        }
        /// <summary>
        /// 主题
        /// </summary>
        public string Theme
        {
            get { return _theme; }
            set { _theme = value.TrimEnd(); }
        }
        /// <summary>
        /// Banner
        /// </summary>
        public string Banner
        {
            get { return _banner; }
            set { _banner = value.TrimEnd(); }
        }
        /// <summary>
        /// 公告
        /// </summary>
        public string Announcement
        {
            get { return _announcement; }
            set { _announcement = value.TrimEnd(); }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value.TrimEnd(); }
        }
    }
}
