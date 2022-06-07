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
        /// ��־��¼ʱ��
        /// </summary>
        private DateTime _logdate = DateTime.Now;

        [SqlField]
        public DateTime LogDate
        {
            get { return _logdate; }
            set { _logdate = value; }

        }
        /// <summary>
        /// �̺߳�
        /// </summary>
        private string _thread = "";

        [SqlField]
        public string Thread
        {
            get { return _thread.Trim(); }
            set { _thread = value.Trim(); }

        }
        /// <summary>
        /// ��־�ȼ�
        /// </summary>
        private string _level = "";

        [SqlField]
        public string Level
        {
            get { return _level.Trim(); }
            set { _level = value.Trim(); }

        }
        /// <summary>
        /// ��־��¼������
        /// </summary>
        private string _logger = "";

        [SqlField]
        public string Logger
        {
            get { return _logger.Trim(); }
            set { _logger = value.Trim(); }

        }
        /// <summary>
        /// ��־����
        /// </summary>
        private string _title = "";

        [SqlField]
        public string Title
        {
            get { return _title.Trim(); }
            set { _title = value.Trim(); }

        }
        /// <summary>
        /// ��־��Ϣ
        /// </summary>
        private string _message = "";

        [SqlField]
        public string Message
        {
            get { return _message.Trim(); }
            set { _message = value.Trim(); }

        }
        /// <summary>
        /// �쳣��Ϣ
        /// </summary>
        private string _exception = "";

        [SqlField]
        public string Exception
        {
            get { return _exception.Trim(); }
            set { _exception = value.Trim(); }

        }
        /// <summary>
        /// ��������
        /// </summary>
        private int _actiontype = 0;

        [SqlField]
        public int ActionType
        {
            get { return _actiontype; }
            set { _actiontype = value; }

        }
        /// <summary>
        /// ������
        /// </summary>
        private int _operator = 0;

        [SqlField]
        public int Operator
        {
            get { return _operator; }
            set { _operator = value; }

        }
        /// <summary>
        /// ��������
        /// </summary>
        private string _operand = "";

        [SqlField]
        public string Operand
        {
            get { return _operand.Trim(); }
            set { _operand = value.Trim(); }

        }
        /// <summary>
        /// IP��ַ
        /// </summary>
        private string _ip = "";

        [SqlField]
        public string IP
        {
            get { return _ip.Trim(); }
            set { _ip = value.Trim(); }

        }
        /// <summary>
        /// ������
        /// </summary>
        private string _machinename = "";

        [SqlField]
        public string MachineName
        {
            get { return _machinename.Trim(); }
            set { _machinename = value.Trim(); }

        }
        /// <summary>
        /// �����
        /// </summary>
        private string _browser = "";

        [SqlField]
        public string Browser
        {
            get { return _browser.Trim(); }
            set { _browser = value.Trim(); }

        }
        /// <summary>
        /// ��¼��־��λ��
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