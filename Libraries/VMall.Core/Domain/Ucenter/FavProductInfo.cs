using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Utility;
using Entity.Base;

namespace VMall.Core
{
    //消息表
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
        /// 会员uid
        /// </summary>
        private int _uid = 0;
        [SqlField]
        public int Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        /// <summary>
        /// 状态，
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
        /// 添加时间
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