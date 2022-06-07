using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Utility;
using Entity.Base;

namespace VMall.Core
{
    //������Ʒ��
    [Serializable]
    [SqlTable("hlh_OrderGift")]
    public class OrderGiftInfo
    {

        /// <summary>
        /// ��¼id
        /// </summary>

        private int _id;


        [SqlField(IsPrimaryKey = true, IsAutoId = true)]
        public int ID
        {
            get { return _id; }
            set { _id = value; }

        }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        private DateTime _creationdate = DateTime.Now;

        [SqlField]
        public DateTime CreationDate
        {
            get { return _creationdate; }
            set { _creationdate = value; }

        }
        /// <summary>
        /// ��������id
        /// </summary>
        private int _createoid = 0;

        [SqlField]
        public int CreateOid
        {
            get { return _createoid; }
            set { _createoid = value; }

        }
        /// <summary>
        /// ��Աid
        /// </summary>
        private int _uid = 0;

        [SqlField]
        public int Uid
        {
            get { return _uid; }
            set { _uid = value; }

        }
        /// <summary>
        /// ״̬
        /// </summary>
        private int _state = 0;

        [SqlField]
        public int State
        {
            get { return _state; }
            set { _state = value; }

        }
        /// <summary>
        /// ��Ʒid
        /// </summary>
        private int _giftpid = 0;

        [SqlField]
        public int GiftPid
        {
            get { return _giftpid; }
            set { _giftpid = value; }

        }
        /// <summary>
        /// ��Ʒ����
        /// </summary>
        private int _giftcount = 0;

        [SqlField]
        public int GiftCount
        {
            get { return _giftcount; }
            set { _giftcount = value; }

        }
        /// <summary>
        /// ʹ������
        /// </summary>
        private int _usecount = 0;

        [SqlField]
        public int UseCount
        {
            get { return _usecount; }
            set { _usecount = value; }

        }
        /// <summary>
        /// ��ʼʱ��
        /// </summary>
        private DateTime _starttime = DateTime.Now;

        [SqlField]
        public DateTime StartTime
        {
            get { return _starttime; }
            set { _starttime = value; }

        }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        private DateTime _endtime = DateTime.Now;

        [SqlField]
        public DateTime EndTime
        {
            get { return _endtime; }
            set { _endtime = value; }

        }
        /// <summary>
        /// ����޸�ʱ��
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