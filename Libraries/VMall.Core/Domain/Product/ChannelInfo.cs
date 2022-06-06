using System;
using Utility;
using Entity.Base;

namespace VMall.Core
{
    /// <summary>
    /// 频道信息类
    /// </summary>
    [Serializable]
    [SqlTable("hlh_channel")]
    public class ChannelInfo
    {
        private int _chid;//分类id
        private DateTime _creationdate = DateTime.Now;//创建时间
        private int _displayorder = 0;//分类排序
        private string _name = "";//分类名称
        private int _state = 0;//状态
        private string _description = "";//频道描述
        private int _mallsource = 0;//来源

        private int _linktype = 0;//链接类型
        private string _linkurl = "";//链接地址
        /// <summary>
        /// 链接类型
        /// </summary>
        [SqlField]
        public int LinkType
        {
            set { _linktype = value; }
            get { return _linktype; }
        }

        /// <summary>
        /// 链接地址
        /// </summary>
        [SqlField]
        public string LinkUrl
        {
            set { _linkurl = value; }
            get { return _linkurl; }
        }
        /// <summary>
        /// 来源
        /// </summary>
        [SqlField]
        public int MallSource
        {
            set { _mallsource = value; }
            get { return _mallsource; }
        }
        /// <summary>
        /// 分类id
        /// </summary>
        [SqlField(IsPrimaryKey = true, IsAutoId = true)]
        public int ChId
        {
            set { _chid = value; }
            get { return _chid; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SqlField]
        public DateTime CreationDate
        {
            set { _creationdate = value; }
            get { return _creationdate; }
        }
        /// <summary>
        /// 分类排序
        /// </summary>
        [SqlField]
        public int DisplayOrder
        {
            set { _displayorder = value; }
            get { return _displayorder; }
        }
        /// <summary>
        /// 分类名称
        /// </summary>
        [SqlField]
        public string Name
        {
            set { _name = value.TrimEnd(); }
            get { return _name; }
        }
        /// <summary>
        /// 状态
        /// </summary>
        [SqlField]
        public int State
        {
            set { _state = value; }
            get { return _state; }
        }
        /// <summary>
        /// 频道描述
        /// </summary>
        [SqlField]
        public string Description
        {
            set { _description = value.TrimEnd(); }
            get { return _description; }
        }
    }
}

