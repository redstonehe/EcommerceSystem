using System;
using Utility;
using Entity.Base;
namespace VMall.Core
{
    /// <summary>
    /// 套装促销活动信息类
    /// </summary>
    [Serializable]
    [SqlTable("hlh_suitpromotions")]
    public class SuitPromotionInfo
    {
        private int _pmid;//活动id
        private int _storeid;//店铺id
        private DateTime _starttime1;//开始时间1
        private DateTime _endtime1;//结束时间1
        private DateTime _starttime2;//开始时间2
        private DateTime _endtime2;//结束时间2
        private DateTime _starttime3;//开始时间3
        private DateTime _endtime3;//结束时间3
        private int _userranklower;//用户等级下限
        private int _state;//状态
        private string _name;//名称
        private int _quotaupper;//配额上限
        private int _onlyonce;//限购一次
        private decimal _suitpv = 0M;//套装pv
        private decimal _suithaimi = 0M;//套装海米
        private decimal _suithongbaocut = 0M;//套装红包减免

        /// <summary>
        /// 套装红包减免
        /// </summary>
        [SqlField]
        public decimal SuitHongBaoCut
        {
            get { return _suithongbaocut; }
            set { _suithongbaocut = value; }
        }
        /// <summary>
        /// 套装pv
        /// </summary>
        [SqlField]
        public decimal SuitPV
        {
            get { return _suitpv; }
            set { _suitpv = value; }
        }
        /// <summary>
        /// 套装海米
        /// </summary>
        [SqlField]
        public decimal SuitHaiMi
        {
            get { return _suithaimi; }
            set { _suithaimi = value; }
        }
        /// <summary>
        /// 活动id
        /// </summary>
        [SqlField(IsPrimaryKey = true, IsAutoId = true)]
        public int PmId
        {
            get { return _pmid; }
            set { _pmid = value; }
        }
        /// <summary>
        /// 店铺id
        /// </summary>
        [SqlField]
        public int StoreId
        {
            get { return _storeid; }
            set { _storeid = value; }
        }
        /// <summary>
        /// 开始时间1
        /// </summary>
        [SqlField]
        public DateTime StartTime1
        {
            get { return _starttime1; }
            set { _starttime1 = value; }
        }
        /// <summary>
        /// 结束时间1
        /// </summary>
        [SqlField]
        public DateTime EndTime1
        {
            get { return _endtime1; }
            set { _endtime1 = value; }
        }
        /// <summary>
        /// 开始时间2
        /// </summary>
        [SqlField]
        public DateTime StartTime2
        {
            get { return _starttime2; }
            set { _starttime2 = value; }
        }
        /// <summary>
        /// 结束时间2
        /// </summary>
        [SqlField]
        public DateTime EndTime2
        {
            get { return _endtime2; }
            set { _endtime2 = value; }
        }
        /// <summary>
        /// 开始时间3
        /// </summary>
        [SqlField]
        public DateTime StartTime3
        {
            get { return _starttime3; }
            set { _starttime3 = value; }
        }
        /// <summary>
        /// 结束时间3
        /// </summary>
        [SqlField]
        public DateTime EndTime3
        {
            get { return _endtime3; }
            set { _endtime3 = value; }
        }
        /// <summary>
        /// 用户等级下限
        /// </summary>
        [SqlField]
        public int UserRankLower
        {
            get { return _userranklower; }
            set { _userranklower = value; }
        }
        /// <summary>
        /// 状态
        /// </summary>
        [SqlField]
        public int State
        {
            get { return _state; }
            set { _state = value; }
        }
        /// <summary>
        /// 名称
        /// </summary>
        [SqlField]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 配额上限
        /// </summary>
        [SqlField]
        public int QuotaUpper
        {
            get { return _quotaupper; }
            set { _quotaupper = value; }
        }
        /// <summary>
        /// 限购一次
        /// </summary>
        [SqlField]
        public int OnlyOnce
        {
            get { return _onlyonce; }
            set { _onlyonce = value; }
        }
    }
}
