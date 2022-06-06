using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMall.Core
{
    /// <summary>
    /// 订单结算预览类
    /// </summary>
    public class OrderSettleInfo
    {
        private int _oid = -1;
        private int _orderUserParentId = -1;
        private string _orderUserParentName = string.Empty;
        private string _orderUserParentMobile = string.Empty;
        private decimal _recommandAward = 0m;
        private List<OrderSettleDetail> _settleDetail = new List<OrderSettleDetail>();

        /// <summary>
        /// Oid
        /// </summary>
        public int Oid
        {
            get { return _oid; }
            set { _oid = value; }
        }
        /// <summary>
        /// OrderUserParentId
        /// </summary>
        public int OrderUserParentId
        {
            get { return _orderUserParentId; }
            set { _orderUserParentId = value; }
        }
        /// <summary>
        /// OrderUserParentName
        /// </summary>
        public string OrderUserParentName
        {
            get { return _orderUserParentName; }
            set { _orderUserParentName = value; }
        }
        /// <summary>
        /// OrderUserParentMobile
        /// </summary>
        public string OrderUserParentMobile
        {
            get { return _orderUserParentMobile; }
            set { _orderUserParentMobile = value; }
        }
        /// <summary>
        /// RecommandAward
        /// </summary>
        public decimal RecommandAward
        {
            get { return _recommandAward; }
            set { _recommandAward = value; }
        }
        /// <summary>
        /// SettleDetail
        /// </summary>
        public List<OrderSettleDetail> SettleDetail
        {
            get { return _settleDetail; }
            set { _settleDetail = value; }
        }
    }
    /// <summary>
    /// 订单结算详情类
    /// </summary>
    public class OrderSettleDetail
    {
        private int _fromParent1Id = -1;
        private string _fromParent1Name = string.Empty;
        private string _fromParent1Mobile = string.Empty;
        private decimal _parent1Principal = 0m;
        private decimal _parent1Profit = 0m;
        private int _fromParent2Id = -1;
        private string _fromParent2Name = string.Empty;
        private string _fromParent2Mobile = string.Empty;
        private decimal _parent2Principal = 0m;
        private decimal _parent2Profit = 0m;
        private int _fromParent3Id = -1;
        private string _fromParent3Name = string.Empty;
        private string _fromParent3Mobile = string.Empty;
        private decimal _parent3Principal = 0m;
        private decimal _parent3Profit = 0m;
        private int _fromParent4Id = -1;
        private string _fromParent4Name = string.Empty;
        private string _fromParent4Mobile = string.Empty;
        private decimal _parent4Principal = 0m;
        private decimal _parent4Profit = 0m;

        /// <summary>
        /// FromParent1Id
        /// </summary>
        public int FromParent1Id 
        {
            get { return _fromParent1Id; }
            set { _fromParent1Id = value; }
        }
        /// <summary>
        /// FromParent1Name
        /// </summary>
        public string FromParent1Name 
        {
            get { return _fromParent1Name; }
            set { _fromParent1Name = value; }
        }
        /// <summary>
        /// FromParent1Mobile
        /// </summary>
        public string FromParent1Mobile 
        {
            get { return _fromParent1Mobile; }
            set { _fromParent1Mobile = value; }
        }
        /// <summary>
        /// Parent1Principal
        /// </summary>
        public decimal Parent1Principal 
        {
            get { return _parent1Principal; }
            set { _parent1Principal = value; }
        }
        /// <summary>
        /// Parent1Profit
        /// </summary>
        public decimal Parent1Profit 
        {
            get { return _parent1Profit; }
            set { _parent1Profit = value; }
        }
        /// <summary>
        /// FromParent2Id
        /// </summary>
        public int FromParent2Id
        {
            get { return _fromParent2Id; }
            set { _fromParent2Id = value; }
        }
        /// <summary>
        /// FromParent2Name
        /// </summary>
        public string FromParent2Name
        {
            get { return _fromParent2Name; }
            set { _fromParent2Name = value; }
        }
        /// <summary>
        /// FromParent2Mobile
        /// </summary>
        public string FromParent2Mobile
        {
            get { return _fromParent2Mobile; }
            set { _fromParent2Mobile = value; }
        }
        /// <summary>
        /// Parent2Principal
        /// </summary>
        public decimal Parent2Principal
        {
            get { return _parent2Principal; }
            set { _parent2Principal = value; }
        }
        /// <summary>
        /// Parent2Profit
        /// </summary>
        public decimal Parent2Profit
        {
            get { return _parent2Profit; }
            set { _parent2Profit = value; }
        }
        /// <summary>
        /// FromParent3Id
        /// </summary>
        public int FromParent3Id
        {
            get { return _fromParent3Id; }
            set { _fromParent3Id = value; }
        }
        /// <summary>
        /// FromParent3Name
        /// </summary>
        public string FromParent3Name
        {
            get { return _fromParent3Name; }
            set { _fromParent3Name = value; }
        }
        /// <summary>
        /// FromParent3Mobile
        /// </summary>
        public string FromParent3Mobile
        {
            get { return _fromParent3Mobile; }
            set { _fromParent3Mobile = value; }
        }
        /// <summary>
        /// Parent3Principal
        /// </summary>
        public decimal Parent3Principal
        {
            get { return _parent3Principal; }
            set { _parent3Principal = value; }
        }
        /// <summary>
        /// Parent3Profit
        /// </summary>
        public decimal Parent3Profit
        {
            get { return _parent3Profit; }
            set { _parent3Profit = value; }
        }
        /// <summary>
        /// FromParent4Id
        /// </summary>
        public int FromParent4Id
        {
            get { return _fromParent4Id; }
            set { _fromParent4Id = value; }
        }
        /// <summary>
        /// FromParent4Name
        /// </summary>
        public string FromParent4Name
        {
            get { return _fromParent4Name; }
            set { _fromParent4Name = value; }
        }
        /// <summary>
        /// FromParent4Mobile
        /// </summary>
        public string FromParent4Mobile
        {
            get { return _fromParent4Mobile; }
            set { _fromParent4Mobile = value; }
        }
        /// <summary>
        /// Parent4Principal
        /// </summary>
        public decimal Parent4Principal
        {
            get { return _parent4Principal; }
            set { _parent4Principal = value; }
        }
        /// <summary>
        /// Parent4Profit
        /// </summary>
        public decimal Parent4Profit
        {
            get { return _parent4Profit; }
            set { _parent4Profit = value; }
        }
    }
}
