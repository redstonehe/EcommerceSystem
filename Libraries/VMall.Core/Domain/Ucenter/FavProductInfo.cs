using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Utility;
using Entity.Base;

namespace VMall.Core
{
    //��Ϣ��
    [Serializable]
    [SqlTable("hlh_favoriteproducts")]
    public class FavProductInfo
    {
        /// <summary>
        /// id
        /// </summary>
        private int _recordid;
        [SqlField(IsPrimaryKey = true, IsAutoId = true)]
        public int RecordId
        {
            get { return _recordid; }
            set { _recordid = value; }
        }

        /// <summary>
        /// ��Աuid
        /// </summary>
        private int _uid = 0;
        [SqlField]
        public int Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        /// <summary>
        /// ״̬��
        /// </summary>
        private int _state = 0;
        [SqlField]
        public int State
        {
            get { return _state; }
            set { _state = value; }
        }
        /// <summary>
        /// _pid
        /// </summary>
        private int _pid = 0;
        [SqlField]
        public int Pid
        {
            get { return _pid; }
            set { _pid = value; }
        }
        /// <summary>
        /// ���ʱ��
        /// </summary>
        private DateTime _addtime = DateTime.Now;
        [SqlField]
        public DateTime AddTime
        {
            get { return _addtime; }
            set { _addtime = value; }
        }

    }
}