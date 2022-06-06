using System;

namespace VMall.Core
{
    /// <summary>
    /// 频道信息类
    /// </summary>
    public class ChannelProductInfo
    {
        private int _id;//记录id
        private DateTime _creationdate = DateTime.Now;//创建时间
        private int _chid=0;//分类id
        private int _pid = 0;//产品id
        private int _state = 0;//状态

        /// <summary>
        /// 记录id
        /// </summary>
        public int Id
        {
            set { _id = value; }
            get { return _id; }
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
        /// 分类id
        /// </summary>
        public int ChId
        {
            set { _chid = value; }
            get { return _chid; }
        }
        /// <summary>
        /// 产品id
        /// </summary>
        public int Pid
        {
            set { _pid = value; }
            get { return _pid; }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public int State
        {
            set { _state = value; }
            get { return _state; }
        }
    }
}

