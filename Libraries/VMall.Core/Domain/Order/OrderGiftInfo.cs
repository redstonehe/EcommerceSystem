using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Utility;
using Entity.Base;

namespace VMall.Core
{
    //订单赠品表
    [Serializable]
    [SqlTable("hlh_OrderGift")]
    public class OrderGiftInfo
    {

        /// <summary>
        /// 记录id
        /// </summary>

        private int _id;


        [SqlField(IsPrimaryKey = true, IsAutoId = true)]
        public int ID
        {
            get { return _id; }
            set { _id = value; }

        }
        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime _creationdate = DateTime.Now;

        [SqlField]
        public DateTime CreationDate
        {
            get { return _creationdate; }
            set { _creationdate = value; }

        }
        /// <summary>
        /// 创建订单id
        /// </summary>
        private int _createoid = 0;

        [SqlField]
        public int CreateOid
        {
            get { return _createoid; }
            set { _createoid = value; }

        }
        /// <summary>
        /// 会员id
        /// </summary>
        private int _uid = 0;

        [SqlField]
        public int Uid
        {
            get { return _uid; }
            set { _uid = value; }

        }
        /// <summary>
        /// 状态
        /// </summary>
        private int _state = 0;

        [SqlField]
        public int State
        {
            get { return _state; }
            set { _state = value; }

        }
        /// <summary>
        /// 赠品id
        /// </summary>
        private int _giftpid = 0;

        [SqlField]
        public int GiftPid
        {
            get { return _giftpid; }
            set { _giftpid = value; }

        }
        /// <summary>
        /// 赠品数量
        /// </summary>
        private int _giftcount = 0;

        [SqlField]
        public int GiftCount
        {
            get { return _giftcount; }
            set { _giftcount = value; }

        }
        /// <summary>
        /// 使用数量
        /// </summary>
        private int _usecount = 0;

        [SqlField]
        public int UseCount
        {
            get { return _usecount; }
            set { _usecount = value; }

        }
        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime _starttime = DateTime.Now;

        [SqlField]
        public DateTime StartTime
        {
            get { return _starttime; }
            set { _starttime = value; }

        }
        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime _endtime = DateTime.Now;

        [SqlField]
        public DateTime EndTime
        {
            get { return _endtime; }
            set { _endtime = value; }

        }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        private DateTime _lastmodify = new DateTime(1900,1,1);

        [SqlField]
        public DateTime LastModify
        {
            get { return _lastmodify; }
            set { _lastmodify = value; }

        }

    }
}