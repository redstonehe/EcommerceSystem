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
    [SqlTable("hlh_inform")]
    public class InformInfo
    {

        /// <summary>
        /// id
        /// </summary>
        private int _id;
        [SqlField(IsPrimaryKey = true, IsAutoId = true)]
        public int id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// 类型id
        /// </summary>
        private int _typeid=0;
        [SqlField]
        public int typeid
        {
            get { return _typeid; }
            set { _typeid = value; }
        }
        /// <summary>
        /// 会员uid
        /// </summary>
        private int _uid=0;
        [SqlField]
        public int uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        /// <summary>
        /// 状态，0.未读1.已读
        /// </summary>
        private int _state=0;
        [SqlField]
        public int state
        {
            get { return _state; }
            set { _state = value; }
        }
        /// <summary>
        /// 是否展示
        /// </summary>
        private int _isshow=0;
        [SqlField]
        public int isshow
        {
            get { return _isshow; }
            set { _isshow = value; }
        }
        /// <summary>
        /// 是否置顶
        /// </summary>
        private int _istop=0;
        [SqlField]
        public int istop
        {
            get { return _istop; }
            set { _istop = value; }
        }
        /// <summary>
        /// 排序
        /// </summary>
        private int _displayorder=0;
        [SqlField]
        public int displayorder
        {
            get { return _displayorder; }
            set { _displayorder = value; }
        }
        /// <summary>
        /// 添加时间
        /// </summary>
        private DateTime _addtime = DateTime.Now;
        [SqlField]
        public DateTime addtime
        {
            get { return _addtime; }
            set { _addtime = value; }
        }
        /// <summary>
        /// 标题
        /// </summary>
        private string _title="";
        [SqlField]
        public string title
        {
            get { return _title.Trim(); }
            set { _title = value.Trim(); }
        }
        /// <summary>
        /// 消息内容
        /// </summary>
        private string _content="";
        [SqlField]
        public string content
        {
            get { return _content.Trim(); }
            set { _content = value.Trim(); }
        }
        /// <summary>
        /// 已读时间
        /// </summary>
        private DateTime _readtime = DateTime.Now;
        [SqlField]
        public DateTime readtime
        {
            get { return _readtime; }
            set { _readtime = value; }
        }

    }
}