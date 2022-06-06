using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Utility;
using Entity.Base;

namespace VMall.Core
{
    //hlh_kftransfer2severb
    [Serializable]
    [SqlTable("hlh_kftransfer2severb")]
    public class Kftransfer2severbInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        private int _id;
        [SqlField(IsPrimaryKey = true, IsAutoId = true)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// 申请单号
        /// </summary>
        private string _applysn = "";
        [SqlField]
        public string ApplySN
        {
            get { return _applysn; }
            set { _applysn = value; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime _createtime = DateTime.Now;
        [SqlField]
        public DateTime CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }
        /// <summary>
        /// 会员id
        /// </summary>
        private int _uid = 0;
        [SqlField]
        public int Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        /// <summary>
        /// 申请金额
        /// </summary>
        private decimal _amount = 0M;
        [SqlField]
        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
        /// <summary>
        /// 会员卡号，可填写多个
        /// </summary>
        private string _cardnumber = "";
        [SqlField]
        public string CardNumber
        {
            get { return _cardnumber; }
            set { _cardnumber = value; }
        }
        /// <summary>
        /// 卡类型
        /// </summary>
        private int _cardtype = 1;
        [SqlField]
        public int CardType
        {
            get { return _cardtype; }
            set { _cardtype = value; }
        }
        /// <summary>
        /// 卡绑定姓名
        /// </summary>
        private string _cardusername = "";
        [SqlField]
        public string CardUserName
        {
            get { return _cardusername; }
            set { _cardusername = value; }
        }
        /// <summary>
        /// 卡绑定手机
        /// </summary>
        private string _cardmobile = "";
        [SqlField]
        public string CardMobile
        {
            get { return _cardmobile; }
            set { _cardmobile = value; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        private string _remark = "";
        [SqlField]
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }
        /// <summary>
        /// 操作员id
        /// </summary>
        private int _adminuid = 1;
        [SqlField]
        public int AdminUid
        {
            get { return _adminuid; }
            set { _adminuid = value; }
        }
        /// <summary>
        /// 处理时间
        /// </summary>
        private DateTime _handletime = DateTime.Now;
        [SqlField]
        public DateTime HandleTime
        {
            get { return _handletime; }
            set { _handletime = value; }
        }
        /// <summary>
        /// 状态
        /// </summary>
        private int _state = 1;
        [SqlField]
        public int State
        {
            get { return _state; }
            set { _state = value; }
        }
        /// <summary>
        /// 处理结果
        /// </summary>
        private string _handleresult = "";
        [SqlField]
        public string HandleResult
        {
            get { return _handleresult; }
            set { _handleresult = value; }
        }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        private DateTime _lastmodify = DateTime.Now;
        [SqlField]
        public DateTime LastModify
        {
            get { return _lastmodify; }
            set { _lastmodify = value; }
        }

    }
}