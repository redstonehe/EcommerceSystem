using System;

namespace VMall.Core
{
    /// <summary>
    /// 汇购卡使用详情
    /// </summary>
    [Serializable]
    public partial class CashCouponDetailInfo
    {
        public CashCouponDetailInfo()
        { }
        #region Model
        private int _detailid;
        private DateTime _creationdate = DateTime.Now;
        private int _cashid = 0;
        private int _uid = 0;
        private int _detailtype = 0;
        private decimal _inamount = 0.00M;
        private decimal _outamount = 0.00M;
        private int _oid = 0;
        private string _osn = "";
        private string _detaildes = "";
        private int _status = 0;
        private int _dirsaleuid = 0;
        /// <summary>
        /// 记录id
        /// </summary>
        public int DetailId
        {
            set { _detailid = value; }
            get { return _detailid; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationDate
        {
            set { _creationdate = value; }
            get { return _creationdate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int CashId
        {
            set { _cashid = value; }
            get { return _cashid; }
        }
        /// <summary>
        /// 会员id
        /// </summary>
        public int Uid
        {
            set { _uid = value; }
            get { return _uid; }
        }
        /// <summary>
        /// 交易类型
        /// </summary>
        public int DetailType
        {
            set { _detailtype = value; }
            get { return _detailtype; }
        }
        /// <summary>
        /// 收入
        /// </summary>
        public decimal InAmount
        {
            set { _inamount = value; }
            get { return _inamount; }
        }
        /// <summary>
        /// 支出
        /// </summary>
        public decimal OutAmount
        {
            set { _outamount = value; }
            get { return _outamount; }
        }
        /// <summary>
        /// 订单id
        /// </summary>
        public int Oid
        {
            set { _oid = value; }
            get { return _oid; }
        }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OSN
        {
            set { _osn = value; }
            get { return _osn; }
        }
        /// <summary>
        /// 交易描述
        /// </summary>
        public string DetailDes
        {
            set { _detaildes = value; }
            get { return _detaildes; }
        }
        /// <summary>
        /// 当前状态
        /// </summary>
        public int Status
        {
            set { _status = value; }
            get { return _status; }
        }
        /// <summary>
        /// 直销会员id
        /// </summary>
        public int DirSaleUid
        {
            set { _dirsaleuid = value; }
            get { return _dirsaleuid; }
        }
        #endregion Model

    }

    public enum CashDetailType
    {
        /// <summary>
        /// 
        /// </summary>
        创建订单生成 = 1,
        /// <summary>
        /// 
        /// </summary>
        订单使用抵现 = 2,
        /// <summary>
        /// 
        /// </summary>
        订单抵现退还 = 3,
        /// <summary>
        /// 
        /// </summary>
        线下刷卡生成 = 4,
        /// <summary>
        /// 
        /// </summary>
        汇购卡退款余额清零 = 5
    }
}

