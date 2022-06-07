using System;
namespace VMall.Core
{
    /// <summary>
    /// ���������
    /// </summary>
    [Serializable]
    public partial class OrderApplyInfo
    {
        public OrderApplyInfo()
        { }
        #region Model
        private int _id;
        private DateTime _creationdate = DateTime.Now;
        private DateTime _lastmodified = DateTime.Now;
        private int _operateuid = 0;
        private int _pid = 0;
        private string _usercode = "";
        private string _realname = "";
        private string _idcard = "";
        private string _parentcode = "";
        private string _managercode = "";
        private int _placeside = 0;
        private string _consignee = "";
        private string _consignmobile = "";
        private int _regionid = 0;
        private string _address = "";
        private string _payimg = "";
        private int _adminoperuid = 0;
        private int _state = 1;
        private string _detaildesc = "";
        private int _resultoid = 0;
        private string _resultosn = "";
        /// <summary>
        /// ��¼id
        /// </summary>
        public int id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime creationdate
        {
            set { _creationdate = value; }
            get { return _creationdate; }
        }
        /// <summary>
        /// ����޸�ʱ��
        /// </summary>
        public DateTime lastmodified
        {
            set { _lastmodified = value; }
            get { return _lastmodified; }
        }
        /// <summary>
        /// ����������uid
        /// </summary>
        public int operateuid
        {
            set { _operateuid = value; }
            get { return _operateuid; }
        }
        /// <summary>
        /// ��Ʒid
        /// </summary>
        public int pid
        {
            set { _pid = value; }
            get { return _pid; }
        }
        /// <summary>
        /// �û���ţ��û������ֻ��ţ�
        /// </summary>
        public string usercode
        {
            set { _usercode = value; }
            get { return _usercode; }
        }
        /// <summary>
        /// ����
        /// </summary>
        public string realname
        {
            set { _realname = value; }
            get { return _realname; }
        }
        /// <summary>
        /// ���֤
        /// </summary>
        public string idcard
        {
            set { _idcard = value; }
            get { return _idcard; }
        }
        /// <summary>
        /// �Ƽ��˱��
        /// </summary>
        public string parentcode
        {
            set { _parentcode = value; }
            get { return _parentcode; }
        }
        /// <summary>
        /// �����˱��
        /// </summary>
        public string managercode
        {
            set { _managercode = value; }
            get { return _managercode; }
        }
        /// <summary>
        /// ���÷�����1ΪA����2ΪB��
        /// </summary>
        public int placeside
        {
            set { _placeside = value; }
            get { return _placeside; }
        }
        /// <summary>
        /// �ջ���
        /// </summary>
        public string consignee
        {
            set { _consignee = value; }
            get { return _consignee; }
        }
        /// <summary>
        /// �ջ��绰
        /// </summary>
        public string consignmobile
        {
            set { _consignmobile = value; }
            get { return _consignmobile; }
        }
        /// <summary>
        /// ����id
        /// </summary>
        public int regionid
        {
            set { _regionid = value; }
            get { return _regionid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string address
        {
            set { _address = value; }
            get { return _address; }
        }
        /// <summary>
        /// ֧��ƾ֤ͼƬ
        /// </summary>
        public string payimg
        {
            set { _payimg = value; }
            get { return _payimg; }
        }
        /// <summary>
        /// ��̨������uid
        /// </summary>
        public int adminoperuid
        {
            set { _adminoperuid = value; }
            get { return _adminoperuid; }
        }
        /// <summary>
        /// ״̬
        /// </summary>
        public int state
        {
            set { _state = value; }
            get { return _state; }
        }
        /// <summary>
        /// ����˵��
        /// </summary>
        public string detaildesc
        {
            set { _detaildesc = value; }
            get { return _detaildesc; }
        }
        /// <summary>
        /// ���ؽ��oid
        /// </summary>
        public int resultoid
        {
            set { _resultoid = value; }
            get { return _resultoid; }
        }
        /// <summary>
        /// ���ؽ��osn
        /// </summary>
        public string resultosn
        {
            set { _resultosn = value; }
            get { return _resultosn; }
        }
        #endregion Model

    }
}

