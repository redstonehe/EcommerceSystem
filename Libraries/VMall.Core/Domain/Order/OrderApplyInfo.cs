using System;
namespace VMall.Core
{
    /// <summary>
    /// 报单申请表
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
        /// 记录id
        /// </summary>
        public int id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime creationdate
        {
            set { _creationdate = value; }
            get { return _creationdate; }
        }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime lastmodified
        {
            set { _lastmodified = value; }
            get { return _lastmodified; }
        }
        /// <summary>
        /// 报单操作人uid
        /// </summary>
        public int operateuid
        {
            set { _operateuid = value; }
            get { return _operateuid; }
        }
        /// <summary>
        /// 产品id
        /// </summary>
        public int pid
        {
            set { _pid = value; }
            get { return _pid; }
        }
        /// <summary>
        /// 用户编号（用户名或手机号）
        /// </summary>
        public string usercode
        {
            set { _usercode = value; }
            get { return _usercode; }
        }
        /// <summary>
        /// 姓名
        /// </summary>
        public string realname
        {
            set { _realname = value; }
            get { return _realname; }
        }
        /// <summary>
        /// 身份证
        /// </summary>
        public string idcard
        {
            set { _idcard = value; }
            get { return _idcard; }
        }
        /// <summary>
        /// 推荐人编号
        /// </summary>
        public string parentcode
        {
            set { _parentcode = value; }
            get { return _parentcode; }
        }
        /// <summary>
        /// 安置人编号
        /// </summary>
        public string managercode
        {
            set { _managercode = value; }
            get { return _managercode; }
        }
        /// <summary>
        /// 安置分区，1为A区，2为B区
        /// </summary>
        public int placeside
        {
            set { _placeside = value; }
            get { return _placeside; }
        }
        /// <summary>
        /// 收货人
        /// </summary>
        public string consignee
        {
            set { _consignee = value; }
            get { return _consignee; }
        }
        /// <summary>
        /// 收货电话
        /// </summary>
        public string consignmobile
        {
            set { _consignmobile = value; }
            get { return _consignmobile; }
        }
        /// <summary>
        /// 地区id
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
        /// 支付凭证图片
        /// </summary>
        public string payimg
        {
            set { _payimg = value; }
            get { return _payimg; }
        }
        /// <summary>
        /// 后台操作人uid
        /// </summary>
        public int adminoperuid
        {
            set { _adminoperuid = value; }
            get { return _adminoperuid; }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public int state
        {
            set { _state = value; }
            get { return _state; }
        }
        /// <summary>
        /// 详情说明
        /// </summary>
        public string detaildesc
        {
            set { _detaildesc = value; }
            get { return _detaildesc; }
        }
        /// <summary>
        /// 返回结果oid
        /// </summary>
        public int resultoid
        {
            set { _resultoid = value; }
            get { return _resultoid; }
        }
        /// <summary>
        /// 返回结果osn
        /// </summary>
        public string resultosn
        {
            set { _resultosn = value; }
            get { return _resultosn; }
        }
        #endregion Model

    }
}

