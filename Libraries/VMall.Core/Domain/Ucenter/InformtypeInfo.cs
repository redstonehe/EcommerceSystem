using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Utility;
using Entity.Base;

namespace VMall.Core
{
    //消息类型表
    [Serializable]
    [SqlTable("hlh_informtype")]
    public class InformtypeInfo
    {

        /// <summary>
        /// 消息类型id
        /// </summary>
        private int _typeid;
        [SqlField(IsPrimaryKey = true, IsAutoId = true)]
        public int typeid
        {
            get { return _typeid; }
            set { _typeid = value; }
        }
        /// <summary>
        /// 名称
        /// </summary>
        private string _name;
        [SqlField]
        public string name
        {
            get { return _name.Trim(); }
            set { _name = value.Trim(); }
        }
        /// <summary>
        /// 排序
        /// </summary>
        private int _displayorder;
        [SqlField]
        public int displayorder
        {
            get { return _displayorder; }
            set { _displayorder = value; }
        }
        /// <summary>
        /// 类型商城来源
        /// </summary>
        private int _mallsource;
        [SqlField]
        public int mallsource
        {
            get { return _mallsource; }
            set { _mallsource = value; }
        }

    }
}