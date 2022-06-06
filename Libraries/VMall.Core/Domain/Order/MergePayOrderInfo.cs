using System;

namespace VMall.Core
{
    /// <summary>
    /// 合并支付类型
    /// </summary>
    public class MergePayOrderInfo
    {
        //合并记录id  mergeid (int )
        //创建时间 creationdate (datatime)
        //合并订单号 mergeosn (varchar(30))
        //子订单id suboid (int)
        //子订单号 subosn (varchar(30))
        //子订单金额 suborderamount (decimal)
        //用户id uid (int)
        //店铺id storeid (int)
        //店铺名称 storename (nvarchar(120))
        //支付方式系统名  paysystemname (varchar(20))
        //支付方式友好名  payfriendname (nvarchar(60))


        private int _mergeid = 0;//合并记录id
        private DateTime _creationdate = DateTime.Now;//创建时间 
        private string _mergeosn = string.Empty;//合并订单号
        private int _suboid = 0;//子订单id 
        private string _subosn = string.Empty;//子订单号
        private decimal _suborderamount = 0M;//子订单金额
        private int _uid = 0;//用户id
        private int _storeid = 0;//店铺id
        private string _storename = string.Empty;//店铺名称
        private string _paysystemname = string.Empty;//支付方式系统名
        private string _payfriendname = string.Empty;//支付方式昵称

        /// <summary>
        /// 退款id
        /// </summary>
        public int MergeId
        {
            get { return _mergeid; }
            set { _mergeid = value; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationDate
        {
            get { return _creationdate; }
            set { _creationdate = value; }
        }
        /// <summary>
        /// 合并订单编号
        /// </summary>
        public string MergeOSN
        {
            get { return _mergeosn; }
            set { _mergeosn = value.TrimEnd(); }
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
        /// 子订单编号
        /// </summary>
        public string SubOSN
        {
            get { return _subosn; }
            set { _subosn = value.TrimEnd(); }
        }
        /// <summary>
        /// 子订单金额
        /// </summary>
        public decimal SubOrderAmount
        {
            get { return _suborderamount; }
            set { _suborderamount = value; }
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


    }
}
