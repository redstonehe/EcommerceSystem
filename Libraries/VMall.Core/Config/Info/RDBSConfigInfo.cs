using System;

namespace VMall.Core
{
    /// <summary>
    /// 关系型数据库配置信息
    /// </summary>
    [Serializable]
    public class RDBSConfigInfo : IConfigInfo
    {
        private string _rdbsconnectstring;//关系数据库连接字符串
        private string _dirsaleconnectstring;//直销系统关系数据库连接字符串
        private string _rdbstablepre;//关系数据库对象前缀
        private string _rdbsdbname;//关系数据库名
        /// <summary>
        /// 关系数据库连接字符串
        /// </summary>
        public string RDBSConnectString
        {
            get { return _rdbsconnectstring; }
            set { _rdbsconnectstring = value; }
        }

        /// <summary>
        /// 直销系统关系数据库连接字符串
        /// </summary>
        public string DirSaleConnectString
        {
            get { return _dirsaleconnectstring; }
            set { _dirsaleconnectstring = value; }
        }

        /// <summary>
        /// 关系数据库对象前缀
        /// </summary>
        public string RDBSTablePre
        {
            get { return _rdbstablepre; }
            set { _rdbstablepre = value; }
        }
        /// <summary>
        /// 关系数据库名
        /// </summary>
        public string RDBSDBName
        {
            get { return _rdbsdbname; }
            set { _rdbsdbname = value; }
        }
    }
}
