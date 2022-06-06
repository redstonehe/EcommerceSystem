using VMall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VMall.Web.Areas.WebApi.Models
{
    [DataContract]
    public class AccountModel
    {
        [DataMember(Order = 1)]
        public LoginStatus ResultStatus { get; set; }
        [DataMember(Order = 2)]
        public PartUserInfo ResultInfo { get; set; }
    }

    [DataContract]
    public class LoginStatus
    {
        [DataMember(Order = 1)]
        public string State { get; set; }
        [DataMember(Order = 2)]
        public string Content { get; set; }
    }

    [DataContract]
    public class AccountRegisterModel
    {
        [DataMember(Order = 1)]
        public RegisterStatus ResultStatus { get; set; }
        [DataMember(Order = 2)]
        public UserInfo ResultInfo { get; set; }
    }

    [DataContract]
    public class RegisterStatus
    {
        [DataMember(Order = 1)]
        public string State { get; set; }
        [DataMember(Order = 2)]
        public string Content { get; set; }
    }

    [DataContract]
    public class returnResult
    {
        [DataMember(Order = 1)]
        public string State { get; set; }
        [DataMember(Order = 2)]
        public string Content { get; set; }
        [DataMember(Order = 3)]
        public object Info { get; set; }
    }

    /// <summary>
    /// 会员类型枚举
    /// </summary>
    public enum UserPanertType
    {
        /// <summary>
        /// 汇购网会员
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