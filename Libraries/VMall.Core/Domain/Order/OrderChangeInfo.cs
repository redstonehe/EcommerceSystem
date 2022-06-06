using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMall.Core
{
    /// <summary>
    /// 换货订单类
    /// </summary>
    public class OrderChangeInfo
    {
        // changeid （int）  记录id
        //creationdate （datetime） 创建时间
        //lastmodifity （datetime） 最后修改时间
        //storeid (int)  店铺id
        //storename (nvarchar(60)) 店铺名称
        //oid (int) 订单id
        //osn (varchar(30)) 订单号
        //uid (int) 会员id
        //state (tinyint) 状态
        //changetype (int) 换货类型（换同款、换新款）
        //changeoid (int)换货后新生成的订单id
        //changeosn (varchar(30))  换货后新生成的订单号
        //changedesc nvarchar(300) 换货原因或描述

        //changeshipsn char(30) 换货物流单号
        //changeshipcoid   smallint 换货物流公司id
        //changeshipconame nchar(30)  换货物流公司名称
        //changeshiptime  datetime  换货配送时间


        private int _changeid = 0;//记录id
        private DateTime _creationdate = DateTime.Now;//创建时间
        private DateTime _lastmodifty = DateTime.Now;//最后修改时间
        private int _storeid = 0;//店铺id
        private string _storename = string.Empty;//店铺名称
        private int _oid = 0;//订单id
        private string _osn = string.Empty;//订单号
        private int _uid = 0;//会员id
        private int _state = 0;//状态(0代表未处理,1代表已处理)
        private int _changetype = 0;//换货类型（0代表同款规格换货、1代表新规格换货）
        private int _said = 0;// 配送地址id
        private int _changeoid = 0;//换货后新生成的订单id
        private string _changeosn = string.Empty;//换货后新生成的订单号
        private string _changedesc = string.Empty;//换货原因或描述


        private string _changeshipsn = string.Empty;//换货物流单号
        private int _changeshipcoid = 0;//换货物流公司id
        private string _changeshipconame = string.Empty;// 换货物流公司名称
        private DateTime _changeshiptime = DateTime.Now;//换货配送时间

        /// <summary>
        /// 换货物流单号
        /// </summary>
        public string ChangeShipSN
        {
            get { return _changeshipsn; }
            set { _changeshipsn = value.TrimEnd(); }
        }
        /// <summary>
        /// 换货物流公司名称
        /// </summary>
        public string ChangeShipCoName
        {
            get { return _changeshipconame; }
            set { _changeshipconame = value.TrimEnd(); }
        }
        /// <summary>
        /// 换货物流公司id
        /// </summary>
        public int ChangeShipCoId
        {
            get { return _changeshipcoid; }
            set { _changeshipcoid = value; }
        }
        /// <summary>
        /// 换货配送时间
        /// </summary>
        public DateTime ChangeShipTime
        {
            get { return _changeshiptime; }
            set { _changeshiptime = value; }
        }

        /// <summary>
        /// 记录id
        /// </summary>
        public int ChangeId
        {
            get { return _changeid; }
            set { _changeid = value; }
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
        /// 换货类型（0代表同款规格换货、1代表新规格换货）
        /// </summary>
        public int ChangeType
        {
            get { return _changetype; }
            set { _changetype = value; }
        }
        /// <summary>
        ///  配送地址id
        /// </summary>
        public int SAId
        {
            get { return _said; }
            set { _said = value; }
        }
        /// <summary>
        /// 换货后新生成的订单id
        /// </summary>
        public int ChangeOid
        {
            get { return _changeoid; }
            set { _changeoid = value; }
        }
        /// <summary>
        /// 换货后新生成的订单号
        /// </summary>
        public string ChangeOSN
        {
            get { return _changeosn; }
            set { _changeosn = value.TrimEnd(); }
        }
        /// <summary>
        /// 换货原因或描述
        /// </summary>
        public string ChangeDesc
        {
            get { return _changedesc; }
            set { _changedesc = value.TrimEnd(); }
        }
    }
}
