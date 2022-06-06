using System;
using Utility;
using Entity.Base;

namespace VMall.Core
{
    /// <summary>
    /// 部分用户信息类
    /// </summary>
    [Serializable]
    [SqlTable("hlh_users")]
    public class PartUserInfo
    {
        private int _uid = 0;//用户id
        private string _username = "";//用户名称
        private string _email = "";//用户邮箱
        private string _mobile = "";//用户手机
        private string _password = "";//用户密码
        private int _userrid = 0;//用户等级id
        private int _storeid = 0;//店铺id
        private int _mallagid = 0;//商城管理员组id
        private string _nickname = "";//用户昵称
        private string _avatar = string.Empty;//用户头像
        private int _paycredits = 0;//支付积分
        private int _rankcredits = 0;//等级积分
        private int _verifyemail = 0;//是否验证邮箱
        private int _verifymobile = 0;//是否验证手机
        private DateTime _liftbantime = new DateTime(1900, 1, 1);//解禁时间
        private string _salt = string.Empty;//盐值
        private bool _isDirSaleUser = false;//是不是直销系统会员
        private int _pid = 0;//推荐人id
        private int _ptype = 1;//推荐人类型
        //private int _isdirsaleuser = 0;//是否同时是直销会员
        private int _dirsaleuid = 0;//直销会员id
        private int _isfxuser = 0;// 该用户是否是分销用户，0表示不是， 1 是普通分销用户 2是高级分销用户

        private string _dirsalepwd = "";//保存直销密码

        private string _otherloginid = string.Empty;//外部登录id 如微信绑定的openid

        private int _parentlevel = 0;//推荐层级 用于显示推荐关系时用于数据显示 不来源于数据库 程序中定义得出 0为顶层 1为第一级子推荐人 以此类推 最多支持3层显示

        private string _paypassword = "";//支付密码
        private int _maxcashcount = 0;//最大汇购卡数量
        private int _agenttype = 0;//0-非代理，1-事业伙伴，2-星级，3-Vip，4-大区，5-合伙人
        private int _usersource = 0;//会员来源，是否引流或者散客进入，0-散客进入，1-会员引流

        private int _ds2agentrank = 0;//保存直销会员原来的代理等级 

        private int _agentpid = 0;//微商系统推荐人Id
        private int _agentptype = 1;//微商系统推荐人Id类型 1.来自微商系统 2.来自直销系统

        private int _busicenttype = 0;//商务中心类型,0.默认。1 .自营商城商务中心（1980代报单）

        private int _isactive = 0;//是否激活
        private DateTime _activetime = new DateTime(1900, 1, 1);//激活时间
        private int _mallsource = 0;//会员平台来源

        /// <summary>
        ///会员平台来源
        /// </summary>
        public int MallSource
        {
            set { _mallsource = value; }
            get { return _mallsource; }
        }
        /// <summary>
        /// 激活时间
        /// </summary>
        [SqlField]
        public DateTime ActiveTime
        {
            set { _activetime = value; }
            get { return _activetime; }
        }

        /// <summary>
        /// 是否激活
        /// </summary>
        [SqlField]
        public int IsActive
        {
            set { _isactive = value; }
            get { return _isactive; }
        }

        /// <summary>
        /// 商务中心类型
        /// </summary>
        [SqlField]
        public int BusiCentType
        {
            set { _busicenttype = value; }
            get { return _busicenttype; }
        }
        /// <summary>
        /// 微商系统推荐人Id
        /// </summary>
        [SqlField]
        public int AgentPid
        {
            set { _agentpid = value; }
            get { return _agentpid; }

        }
        /// <summary>
        /// 微商系统推荐人Id类型 1.来自微商系统 2.来自直销系统
        /// </summary>
        [SqlField]
        public int AgentPType
        {
            set { _agentptype = value; }
            get { return _agentptype; }

        }
        /// <summary>
        /// 保存直销会员原来的代理等级 
        /// </summary>
        [SqlField]
        public int Ds2AgentRank
        {
            set { _ds2agentrank = value;}
            get { return _ds2agentrank; }
        }

        /// <summary>
        /// 会员来源，是否引流或者散客进入，0-散客进入，1-会员引流
        /// </summary>
        [SqlField]
        public int UserSource
        {
            set { _usersource = value; }
            get { return _usersource; }
        }

        /// <summary>
        ///代理等级
        /// </summary>
        [SqlField]
        public int AgentType
        {
            set { _agenttype = value; }
            get { return _agenttype; }
        }
        
        /// <summary>
        ///最大汇购卡数量
        /// </summary>
        [SqlField]
        public int MaxCashCount
        {
            set { _maxcashcount = value; }
            get { return _maxcashcount; }
        }
        /// <summary>
        /// 用户支付密码
        /// </summary>
        [SqlField]
        public string PayPassword
        {
            set { _paypassword = value.TrimEnd(); }
            get { return _paypassword; }
        }
        /// <summary>
        /// 外部登录id
        /// </summary>
        [SqlField]
        public string OtherLoginId
        {
            set { _otherloginid = value; }
            get { return _otherloginid; }
        }

        /// <summary>
        /// 推荐层级
        /// </summary>
        [SqlField]
        public int ParentLevel
        {
            set { _parentlevel = value; }
            get { return _parentlevel; }

        }

        /// <summary>
        /// 直销会员id
        /// </summary>
        [SqlField]
        public int DirSaleUid
        {
            set { _dirsaleuid = value; }
            get { return _dirsaleuid; }

        }
        /// <summary>
        /// 直销密码
        /// </summary>
        [SqlField]
        public string DirSalePwd
        {
            set { _dirsalepwd = value; }
            get { return _dirsalepwd; }
        }
        ///// <summary>
        ///// 是否同时是直销会员 0 不是直销会员 1 同时是直销会员
        ///// </summary>
        //public int IsDirSaleUser
        //{
        //    set { _isdirsaleuser = value; }
        //    get { return _isdirsaleuser; }
        //}
        /// <summary>
        /// 推荐人类型 1 为商城会员推荐 2为直销会员推荐
        /// </summary>
        [SqlField]
        public int Ptype
        {
            set { _ptype = value; }
            get { return _ptype; }
        }
        /// <summary>
        /// 推荐人id
        /// </summary>
        [SqlField]
        public int Pid
        {
            set { _pid = value; }
            get { return _pid; }
        }
        /// <summary>
        ///用户id
        /// </summary>
        [SqlField(IsPrimaryKey = true,IsAutoId=true)]
        public int Uid
        {
            set { _uid = value; }
            get { return _uid; }
        }
        /// <summary>
        ///用户名称
        /// </summary>
        [SqlField]
        public string UserName
        {
            set { _username = value.TrimEnd(); }
            get { return _username; }
        }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        [SqlField]
        public string Email
        {
            set { _email = value.TrimEnd(); }
            get { return _email; }
        }
        /// <summary>
        /// 用户手机
        /// </summary>
        [SqlField]
        public string Mobile
        {
            set { _mobile = value.TrimEnd(); }
            get { return _mobile; }
        }
        /// <summary>
        /// 用户密码
        /// </summary>
        [SqlField]
        public string Password
        {
            set { _password = value.TrimEnd(); }
            get { return _password; }
        }
        ///<summary>
        ///用户等级id
        ///</summary>
        [SqlField]
        public int UserRid
        {
            get { return _userrid; }
            set { _userrid = value; }
        }
        /// <summary>
        /// 店铺id
        /// </summary>
        [SqlField]
        public int StoreId
        {
            get { return _storeid; }
            set { _storeid = value; }
        }
        ///<summary>
        ///商城管理员组id
        ///</summary>
        [SqlField]
        public int MallAGid
        {
            get { return _mallagid; }
            set { _mallagid = value; }
        }
        /// <summary>
        /// 用户昵称
        /// </summary>
        [SqlField]
        public string NickName
        {
            set { _nickname = value.TrimEnd(); }
            get { return _nickname; }
        }
        /// <summary>
        /// 用户头像
        /// </summary>
        [SqlField]
        public string Avatar
        {
            get { return _avatar; }
            set { _avatar = value.TrimEnd(); }
        }
        ///<summary>
        ///支付积分
        ///</summary>
        [SqlField]
        public int PayCredits
        {
            get { return _paycredits; }
            set { _paycredits = value; }
        }
        /// <summary>
        /// 等级积分
        /// </summary>
        [SqlField]
        public int RankCredits
        {
            get { return _rankcredits; }
            set { _rankcredits = value; }
        }
        /// <summary>
        /// 是否验证邮箱
        /// </summary>
        [SqlField]
        public int VerifyEmail
        {
            get { return _verifyemail; }
            set { _verifyemail = value; }
        }
        /// <summary>
        /// 是否验证手机
        /// </summary>
        [SqlField]
        public int VerifyMobile
        {
            get { return _verifymobile; }
            set { _verifymobile = value; }
        }
        /// <summary>
        /// 解禁时间
        /// </summary>
        [SqlField]
        public DateTime LiftBanTime
        {
            get { return _liftbantime; }
            set { _liftbantime = value; }
        }
        ///<summary>
        ///盐值
        ///</summary>
        [SqlField]
        public string Salt
        {
            get { return _salt; }
            set { _salt = value; }
        }

        /// <summary>
        /// 是不是直销系统会员
        /// </summary>
        [SqlField]
        public bool IsDirSaleUser
        {
            get { return _isDirSaleUser; }
            set { _isDirSaleUser = value; }
        }
        /// <summary>
        /// 是不是分销会员
        /// </summary>
        [SqlField]
        public int IsFXUser
        {
            get { return _isfxuser; }
            set { _isfxuser = value; }
        }
    }

    /// <summary>
    /// 完整用户信息
    /// </summary>
    public class UserInfo : PartUserInfo
    {
        private DateTime _lastvisittime = DateTime.Now;//最后访问时间
        private string _lastvisitip = "";//最后访问ip
        private int _lastvisitrgid = -1;//最后访问区域id
        private DateTime _registertime = DateTime.Now;//用户注册时间
        private string _registerip = "";//用户注册ip
        private int _registerrgid = -1;//用户注册区域id
        private int _gender = 0;//用户性别(0代表未知，1代表男，2代表女)
        private string _realname = "";//用户真实名称
        private DateTime _bday = DateTime.Now;//用户出生日期
        private string _idcard = "";//身份证号
        private int _regionid = 0;//区域id
        private string _address = "";//所在地
        private string _bio = "";//简介
        private string _bankname = "";//银行名称
        private string _bankcardcode = "";//银行卡号
        private string _bankusername = "";//银行开户人
        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime LastVisitTime
        {
            set { _lastvisittime = value; }
            get { return _lastvisittime; }
        }
        /// <summary>
        /// 最后访问ip
        /// </summary>
        public string LastVisitIP
        {
            set { _lastvisitip = value; }
            get { return _lastvisitip; }
        }
        /// <summary>
        /// 最后访问区域id
        /// </summary>
        public int LastVisitRgId
        {
            set { _lastvisitrgid = value; }
            get { return _lastvisitrgid; }
        }
        /// <summary>
        /// 用户注册时间
        /// </summary>
        public DateTime RegisterTime
        {
            set { _registertime = value; }
            get { return _registertime; }
        }
        /// <summary>
        /// 用户注册ip
        /// </summary>
        public string RegisterIP
        {
            set { _registerip = value; }
            get { return _registerip; }
        }
        /// <summary>
        /// 用户注册区域id
        /// </summary>
        public int RegisterRgId
        {
            set { _registerrgid = value; }
            get { return _registerrgid; }
        }
        ///<summary>
        ///用户性别(0代表未知，1代表男，2代表女)
        ///</summary>
        public int Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }
        /// <summary>
        /// 用户真实名称
        /// </summary>
        public string RealName
        {
            set { _realname = value; }
            get { return _realname; }
        }
        ///<summary>
        ///用户出生日期
        ///</summary>
        public DateTime Bday
        {
            get { return _bday; }
            set { _bday = value; }
        }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdCard
        {
            set { _idcard = value; }
            get { return _idcard; }
        }
        ///<summary>
        ///区域id
        ///</summary>
        public int RegionId
        {
            get { return _regionid; }
            set { _regionid = value; }
        }
        ///<summary>
        ///所在地
        ///</summary>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        ///<summary>
        ///简介
        ///</summary>
        public string Bio
        {
            get { return _bio.TrimEnd(); }
            set { _bio = value; }
        }
        ///<summary>
        ///银行名称
        ///</summary>
        public string BankName
        {
            get { return _bankname.TrimEnd(); }
            set { _bankname = value; }
        }
        ///<summary>
        ///银行卡号
        ///</summary>
        public string BankCardCode
        {
            get { return _bankcardcode.TrimEnd(); }
            set { _bankcardcode = value; }
        }
        ///<summary>
        ///银行开户人
        ///</summary>
        public string BankUserName
        {
            get { return _bankusername.TrimEnd(); }
            set { _bankusername = value; }
        }
    }

    /// <summary>
    /// 用户细节信息类
    /// </summary>
    public class UserDetailInfo
    {
        private int _uid;//用户id
        private DateTime _lastvisittime = DateTime.Now;//最后访问时间
        private string _lastvisitip = "";//最后访问ip
        private int _lastvisitrgid = -1;//最后访问区域id
        private DateTime _registertime = DateTime.Now;//用户注册时间
        private string _registerip = "";//用户注册ip
        private int _registerrgid = -1;//用户注册区域id
        private int _gender = 0;//用户性别(0代表未知，1代表男，2代表女)
        private string _realname = "";//用户真实名称
        private DateTime _bday = DateTime.Now;//用户出生日期
        private string _idcard = "";//身份证号
        private int _regionid = 0;//区域id
        private string _address = "";//所在地
        private string _bio = "";//简介
        private string _bankname = "";//银行名称
        private string _bankcardcode = "";//银行卡号
        private string _bankusername = "";//银行开户人
        /// <summary>
        ///用户id
        /// </summary>
        public int Uid
        {
            set { _uid = value; }
            get { return _uid; }
        }
        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime LastVisitTime
        {
            set { _lastvisittime = value; }
            get { return _lastvisittime; }
        }
        /// <summary>
        /// 最后访问ip
        /// </summary>
        public string LastVisitIP
        {
            set { _lastvisitip = value; }
            get { return _lastvisitip; }
        }
        /// <summary>
        /// 最后访问区域id
        /// </summary>
        public int LastVisitRgId
        {
            set { _lastvisitrgid = value; }
            get { return _lastvisitrgid; }
        }
        /// <summary>
        /// 用户注册时间
        /// </summary>
        public DateTime RegisterTime
        {
            set { _registertime = value; }
            get { return _registertime; }
        }
        /// <summary>
        /// 用户注册ip
        /// </summary>
        public string RegisterIP
        {
            set { _registerip = value; }
            get { return _registerip; }
        }
        /// <summary>
        /// 用户注册区域id
        /// </summary>
        public int RegisterRgId
        {
            set { _registerrgid = value; }
            get { return _registerrgid; }
        }
        ///<summary>
        ///用户性别(0代表未知，1代表男，2代表女)
        ///</summary>
        public int Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }
        /// <summary>
        /// 用户真实名称
        /// </summary>
        public string RealName
        {
            set { _realname = value; }
            get { return _realname; }
        }
        ///<summary>
        ///用户出生日期
        ///</summary>
        public DateTime Bday
        {
            get { return _bday; }
            set { _bday = value; }
        }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdCard
        {
            set { _idcard = value; }
            get { return _idcard; }
        }
        ///<summary>
        ///区域id
        ///</summary>
        public int RegionId
        {
            get { return _regionid; }
            set { _regionid = value; }
        }
        ///<summary>
        ///所在地
        ///</summary>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        ///<summary>
        ///简介
        ///</summary>
        public string Bio
        {
            get { return _bio.TrimEnd(); }
            set { _bio = value; }
        }
        ///<summary>
        ///银行名称
        ///</summary>
        public string BankName
        {
            get { return _bankname.TrimEnd(); }
            set { _bankname = value; }
        }
        ///<summary>
        ///银行卡号
        ///</summary>
        public string BankCardCode
        {
            get { return _bankcardcode.TrimEnd(); }
            set { _bankcardcode = value; }
        }
        ///<summary>
        ///银行开户人
        ///</summary>
        public string BankUserName
        {
            get { return _bankusername.TrimEnd(); }
            set { _bankusername = value; }
        }
    }

    /// <summary>
    /// 会员类型枚举
    /// </summary>
    public enum UserPanertType
    {
        /// <summary>
        /// 汇购会员
        /// </summary>
        HaiHuiUser = 1,
        /// <summary>
        /// 直销系统会员
        /// </summary>
        DirSaleUser = 2,
        /// <summary>
        /// 天鹰网会员
        /// </summary>
        TianYingUser = 3,
    }
}
