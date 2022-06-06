using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMall.Core
{
    /// <summary>
    /// 退货订单类
    /// </summary>
    public class OrderReturnInfo
    {
        //returnid （int）  记录id
        //creationdate （datetime） 创建时间
        //lastmodifity （datetime） 最后修改时间
        //storeid (int)  店铺id
        //storename (nvarchar(60)) 店铺名称
        //oid (int) 订单id
        //osn (varchar(30)) 订单号
        //uid (int) 会员id
        //state (tinyint) 状态  0默认值 1为退款审核 2为审核完成
        //returndesc nvarchar(300) 退货原因或描述
        //returnshipfee decimal(18, 2) 退货承担运费
        //returnshipdesc nvarchar(300) 退货运费描述

        private int _returnid = 0;//记录id
        private DateTime _creationdate = DateTime.Now;//创建时间
        private DateTime _lastmodifty = DateTime.Now;//最后修改时间
        private int _storeid = 0;//店铺id
        private string _storename = string.Empty;//店铺名称
        private int _oid = 0;//订单id
        private string _osn = string.Empty;//订单号
        private int _uid = 0;//会员id
        private int _state = 0;//状态(0默认值 1为退款审核 2为审核完成)

        private string _returndesc = string.Empty;//退货原因或描述
        private decimal _returnshipfee = 0M;//退货承担运费
        private string _returnshipdesc = string.Empty;//退货运费描述

        /// <summary>
        /// 记录id
        /// </summary>
        public int ReturnId
        {
            get { return _returnid; }
            set { _returnid = value; }
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
        /// 最后修改时间
        /// </summary>
        public DateTime LastModifity
        {
            get { return _lastmodifty; }
            set { _lastmodifty = value; }
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
        /// 订单id
        /// </summary>
        public int Oid
        {
            get { return _oid; }
            set { _oid = value; }
        }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OSN
        {
            get { return _osn; }
            set { _osn = value.TrimEnd(); }
        }
        /// <summary>
        /// 会员id
        /// </summary>
        public int Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        /// <summary>
        /// 状态(0代表未处理,1代表已处理)
        /// </summary>
        public int State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// 退货运费描述
        /// </summary>
        public string ReturnDesc
        {
            get { return _returndesc; }
            set { _returndesc = value.TrimEnd(); }
        }
        /// <summary>
        /// 退货承担运费
        /// </summary>
        public decimal ReturnShipFee
        {
            get { return _returnshipfee; }
            set { _returnshipfee = value; }
        }
        
        /// <summary>
        /// 退货运费描述
        /// </summary>
        public string ReturnShipDesc
        {
            get { return _returnshipdesc; }
            set { _returnshipdesc = value.TrimEnd(); }
        }
    }
}
