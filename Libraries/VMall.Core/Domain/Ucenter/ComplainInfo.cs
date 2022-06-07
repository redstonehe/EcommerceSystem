using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Utility;
using Entity.Base;

namespace VMall.Core
{
    //��ƷͶ�߱�
    [Serializable]
    [SqlTable("hlh_complain")]
    public class ComplainInfo
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
        /// ״̬,0.δ�ظ�1.�ѻظ�
        /// </summary>
        private int _state = 0;
        [SqlField]
        public int state
        {
            get { return _state; }
            set { _state = value; }
        }
        /// <summary>
        /// Ͷ��uid
        /// </summary>
        private int _uid = 0;
        [SqlField]
        public int uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        /// <summary>
        /// Ͷ���ǳ�
        /// </summary>
        private string _complainnickname = "";
        [SqlField]
        public string complainnickname
        {
            get { return _complainnickname.Trim(); }
            set { _complainnickname = value.Trim(); }
        }
        /// <summary>
        /// Ͷ��ʱ��
        /// </summary>
        private DateTime _complaintime = DateTime.Now;
        [SqlField]
        public DateTime complaintime
        {
            get { return _complaintime; }
            set { _complaintime = value; }
        }
        /// <summary>
        /// Ͷ����Ϣ
        /// </summary>
        private string _complainmsg = "";
        [SqlField]
        public string complainmsg
        {
            get { return _complainmsg.Trim(); }
            set { _complainmsg = value.Trim(); }
        }
        /// <summary>
        /// Ͷ��ip
        /// </summary>
        private string _complainip = "";
        [SqlField]
        public string complainip
        {
            get { return _complainip.Trim(); }
            set { _complainip = value.Trim(); }
        }
        /// <summary>
        /// ��id
        /// </summary>
        private int _complainpid = 0;
        [SqlField]
        public int complainpid
        {
            get { return _complainpid; }
            set { _complainpid = value; }
        }
        /// <summary>
        /// �ظ���id
        /// </summary>
        private int _replyuid = 0;
        [SqlField]
        public int replyuid
        {
            get { return _replyuid; }
            set { _replyuid = value; }
        }
        /// <summary>
        /// �ظ�ʱ��
        /// </summary>
        private DateTime _replytime = DateTime.Now;
        [SqlField]
        public DateTime replytime
        {
            get { return _replytime; }
            set { _replytime = value; }
        }
        /// <summary>
        /// �ظ����ǳ�
        /// </summary>
        private string _replynickname = "";
        [SqlField]
        public string replynickname
        {
            get { return _replynickname.Trim(); }
            set { _replynickname = value.Trim(); }
        }
        /// <summary>
        /// �ظ���Ϣ
        /// </summary>
        private string _replymsg = "";
        [SqlField]
        public string replymsg
        {
            get { return _replymsg.Trim(); }
            set { _replymsg = value.Trim(); }
        }
        /// <summary>
        /// �ظ�ip
        /// </summary>
        private string _replyip = "";
        [SqlField]
        public string replyip
        {
            get { return _replyip.Trim(); }
            set { _replyip = value.Trim(); }
        }

    }
}