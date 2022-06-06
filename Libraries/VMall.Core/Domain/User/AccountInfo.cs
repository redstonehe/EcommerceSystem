using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMall.Core
{
    /// <summary>
    /// 账户信息类
    /// </summary>
    public class AccountInfo
    {
        private int _accountid=0;//帐号id
        private int _userid=0;//用户id
        private decimal _totalin=0M;//总收入
        private decimal _totalout=0M;//总支出
        private decimal _banlance = 0M;//当前余额
        private decimal _lockbanlance=0M;//锁定结余
        private bool _isinternaltransfer = false;//能否内部转账
        private bool _ismembertransfer = false;//能否会员间转账
        private DateTime _createtime = DateTime.Now;//创建时间
        private string _accountname = string.Empty;//帐号名称
        /// <summary>
        /// 帐号id
        /// </summary>
        public int AccountId
        {
            set { _accountid = value; }
            get { return _accountid; }
        }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId
        {
            set { _userid = value; }
            get { return _userid; }
        }
        /// <summary>
        /// 总收入
        /// </summary>
        public decimal TotalIn
        {
            set { _totalin = value; }
            get { return _totalin; }
        }
        /// <summary>
        /// 总支出
        /// </summary>
        public decimal TotalOut
        {
            set { _totalout = value; }
            get { return _totalout; }
        }
        /// <summary>
        /// 总结余
        /// </summary>
        public decimal Banlance
        {
            set { _banlance = value; }
            get { return _banlance; }
        }
        /// <summary>
        /// 锁定结余
        /// </summary>
        public decimal LockBanlance
        {
            set { _lockbanlance = value; }
            get { return _lockbanlance; }
        }

        /// <summary>
        /// 能否内部转账
        /// </summary>
        public bool IsInternalTransfer
        {
            set { _isinternaltransfer = value; }
            get { return _isinternaltransfer; }
        }
        /// <summary>
        /// 能否会员间转账
        /// </summary>
        public bool IsMemberTransfer
        {
            set { _ismembertransfer = value; }
            get { return _ismembertransfer; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName
        {
            set { _accountname = value.Trim(); }
            get { return _accountname; }
        }
    }
}
