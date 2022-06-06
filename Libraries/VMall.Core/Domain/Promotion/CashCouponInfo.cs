using System;
using Utility;
using Entity.Base;

namespace VMall.Core
{
    /// <summary>
    /// 汇购卡表
    /// </summary>
    [Serializable]
    [SqlTable("hlh_CashCoupon")]
    public partial class CashCouponInfo
    {

        public CashCouponInfo()
        { }
        #region Model
        private int _cashid;
        private string _cashcouponsn = "";
        private DateTime _creationdate = DateTime.Now;
        private DateTime _lastmodifity = DateTime.Now;
        private int _coupontype = 0;
        private int _storeid = 0;
        private int _channelid = 0;
        private int _uid = 0;
        private int _createoid = 0;
        private string _createosn = "";
        private decimal _cashamount = 0.00M;
        private decimal _totalin = 0.00M;
        private decimal _totalout = 0.00M;
        private decimal _banlance = 0.00M;
        private DateTime _validtime = DateTime.Now;
        private int _dirsaleuid = 0;
        /// <summary>
        /// 汇购卡id
        /// </summary>
        [SqlField(IsPrimaryKey = true, IsAutoId = true)]
        public int CashId
        {
            set { _cashid = value; }
            get { return _cashid; }
        }
        /// <summary>
        /// 汇购卡卡号
        /// </summary>
        [SqlField]
        public string CashCouponSN
        {
            set { _cashcouponsn = value; }
            get { return _cashcouponsn; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SqlField]
        public DateTime CreationDate
        {
            set { _creationdate = value; }
            get { return _creationdate; }
        }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        [SqlField]
        public DateTime LastModifity
        {
            set { _lastmodifity = value; }
            get { return _lastmodifity; }
        }
        /// <summary>
        /// 卡券类型
        /// </summary>
        [SqlField]
        public int CouponType
        {
            set { _coupontype = value; }
            get { return _coupontype; }
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
        /// 频道id
        /// </summary>
        [SqlField]
        public int ChannelId
        {
            set { _channelid = value; }
            get { return _channelid; }
        }
        /// <summary>
        /// 会员id
        /// </summary>
        [SqlField]
        public int Uid
        {
            set { _uid = value; }
            get { return _uid; }
        }
        /// <summary>
        /// 创建该汇购卡的订单id
        /// </summary>
        [SqlField]
        public int CreateOid
        {
            set { _createoid = value; }
            get { return _createoid; }
        }
        /// <summary>
        /// 创建该汇购卡的订单号
        /// </summary>
        [SqlField]
        public string CreateOSN
        {
            set { _createosn = value; }
            get { return _createosn; }
        }
        /// <summary>
        /// 汇购卡金额面值
        /// </summary>
        [SqlField]
        public decimal CashAmount
        {
            set { _cashamount = value; }
            get { return _cashamount; }
        }
        /// <summary>
        /// 总收入
        /// </summary>
        [SqlField]
        public decimal TotalIn
        {
            set { _totalin = value; }
            get { return _totalin; }
        }
        /// <summary>
        /// 总支出
        /// </summary>
        [SqlField]
        public decimal TotalOut
        {
            set { _totalout = value; }
            get { return _totalout; }
        }
        /// <summary>
        /// 结余
        /// </summary>
        [SqlField]
        public decimal Banlance
        {
            set { _banlance = value; }
            get { return _banlance; }
        }
        /// <summary>
        /// 有效期限
        /// </summary>
        [SqlField]
        public DateTime ValidTime
        {
            set { _validtime = value; }
            get { return _validtime; }
        }
        /// <summary>
        /// 直销会员id
        /// </summary>
        [SqlField]
        public int DirSaleUid
        {
            set { _dirsaleuid = value; }
            get { return _dirsaleuid; }
        }
        #endregion Model

    }
}

