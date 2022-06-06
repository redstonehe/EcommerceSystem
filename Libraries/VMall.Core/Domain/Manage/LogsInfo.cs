using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Utility;
using Entity.Base;

namespace VMall.Core
{
    //hlh_Logs
    [Serializable]
    [SqlTable("hlh_Logs")]
    public class LogsInfo
    {

        /// <summary>
        /// Id
        /// </summary>

        private int _id;


        [SqlField(IsPrimaryKey = true, IsAutoId = true)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }

        }
        /// <summary>
        /// 日志记录时间
        /// </summary>
        private DateTime _logdate = DateTime.Now;

        [SqlField]
        public DateTime LogDate
        {
            get { return _logdate; }
            set { _logdate = value; }

        }
        /// <summary>
        /// 线程号
        /// </summary>
        private string _thread = "";

        [SqlField]
        public string Thread
        {
            get { return _thread.Trim(); }
            set { _thread = value.Trim(); }

        }
        /// <summary>
        /// 日志等级
        /// </summary>
        private string _level = "";

        [SqlField]
        public string Level
        {
            get { return _level.Trim(); }
            set { _level = value.Trim(); }

        }
        /// <summary>
        /// 日志记录类名称
        /// </summary>
        private string _logger = "";

        [SqlField]
        public string Logger
        {
            get { return _logger.Trim(); }
            set { _logger = value.Trim(); }

        }
        /// <summary>
        /// 日志标题
        /// </summary>
        private string _title = "";

        [SqlField]
        public string Title
        {
            get { return _title.Trim(); }
            set { _title = value.Trim(); }

        }
        /// <summary>
        /// 日志消息
        /// </summary>
        private string _message = "";

        [SqlField]
        public string Message
        {
            get { return _message.Trim(); }
            set { _message = value.Trim(); }

        }
        /// <summary>
        /// 异常信息
        /// </summary>
        private string _exception = "";

        [SqlField]
        public string Exception
        {
            get { return _exception.Trim(); }
            set { _exception = value.Trim(); }

        }
        /// <summary>
        /// 动作类型
        /// </summary>
        private int _actiontype = 0;

        [SqlField]
        public int ActionType
        {
            get { return _actiontype; }
            set { _actiontype = value; }

        }
        /// <summary>
        /// 操作者
        /// </summary>
        private int _operator = 0;

        [SqlField]
        public int Operator
        {
            get { return _operator; }
            set { _operator = value; }

        }
        /// <summary>
        /// 操作对象
        /// </summary>
        private string _operand = "";

        [SqlField]
        public string Operand
        {
            get { return _operand.Trim(); }
            set { _operand = value.Trim(); }

        }
        /// <summary>
        /// IP地址
        /// </summary>
        private string _ip = "";

        [SqlField]
        public string IP
        {
            get { return _ip.Trim(); }
            set { _ip = value.Trim(); }

        }
        /// <summary>
        /// 机器名
        /// </summary>
        private string _machinename = "";

        [SqlField]
        public string MachineName
        {
            get { return _machinename.Trim(); }
            set { _machinename = value.Trim(); }

        }
        /// <summary>
        /// 浏览器
        /// </summary>
        private string _browser = "";

        [SqlField]
        public string Browser
        {
            get { return _browser.Trim(); }
            set { _browser = value.Trim(); }

        }
        /// <summary>
        /// 记录日志的位置
        /// </summary>
        private string _location = "";

        [SqlField]
        public string Location
        {
            get { return _location.Trim(); }
            set { _location = value.Trim(); }

        }

    }
}