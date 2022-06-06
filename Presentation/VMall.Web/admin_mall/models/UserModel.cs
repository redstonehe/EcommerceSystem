using System;
using System.Data;
using System.Web.Mvc;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using System.Text.RegularExpressions;

namespace VMall.Web.MallAdmin.Models
{
    /// <summary>
    /// 用户列表模型类
    /// </summary>
    public class UserListModel
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        public DataTable UserList { get; set; }
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户UID
        /// </summary>
        public int Uid { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 用户手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 用户等级id
        /// </summary>
        public int UserRid { get; set; }
        /// <summary>
        /// 商城管理员组id
        /// </summary>
        public int MallAGid { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public int UserType { get; set; }
        /// <summary>
        /// 代理级别
        /// </summary>
        public int AgentType { get; set; }
        /// <summary>
        /// 平台来源
        /// </summary>
        public int MallSource { get; set; }
    }

    /// <summary>
    /// 账户直销资料模型类
    /// </summary>
    public class DSDetailModel
    {
        public int UserId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string UserCard { get; set; }
        public string UserPhone { get; set; }
        public int Rank { get; set; }
        public string RankName { get; set; }
        public string ParentCode { get; set; }
        public string ManagerCode { get; set; }
        public DateTime RegisterDate { get; set; }
    }

    /// <summary>
    /// 用户模型类
    /// </summary>
    public class UserModel : IValidatableObject
    {
        /// <summary>
        /// 最大汇购卡数
        /// </summary>
        [Required(ErrorMessage = "最大汇购卡数不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "最大汇购卡数不能为负数")]
        [DisplayName("最大汇购卡数")]
        public int MaxCashCount { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [StringLength(20, ErrorMessage = "名称长度不能大于20")]
        public string UserName { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [Email]
        public string Email { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        [Mobile]
        [Required(ErrorMessage = "手机不能为空")]
        public string Mobile { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(32, MinimumLength = 4, ErrorMessage = "密码长度必须大于3且小于33")]
        public string Password { get; set; }
        /// <summary>
        /// 确认密码
        /// </summary>
        [Compare("Password", ErrorMessage = "密码必须相同")]
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// 用户等级id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择正确的用户等级")]
        [DisplayName("用户等级")]
        public int UserRid { get; set; }
        /// <summary>
        /// 商城管理员组id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择正确的管理员组")]
        [DisplayName("管理员组")]
        public int MallAGid { get; set; }
        /// <summary>
        /// 推荐人UID
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "请选择正确的推荐人UID")]
        [DisplayName("推荐人UID")]
        public int Pid { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(20, ErrorMessage = "名称长度不能大于20")]
        public string NickName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 支付积分
        /// </summary>
        [Required(ErrorMessage = "支付积分不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "支付积分不能为负数")]
        [DisplayName("支付积分")]
        public int PayCredits { get; set; }
        /// <summary>
        /// 性别(0代表未知，1代表男，2代表女)
        /// </summary>
        [Range(0, 2, ErrorMessage = "请选择确切的性别")]
        [DisplayName("性别")]
        public int Gender { get; set; }
        /// <summary>
        /// 真实名称
        /// </summary>
        [StringLength(5, ErrorMessage = "名称长度不能大于5")]
        public string RealName { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        [DisplayName("出生日期")]
        public DateTime? Bday { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [IdCard]
        public string IdCard { get; set; }
        /// <summary>
        /// 所在地区域
        /// </summary>
        [DisplayName("区域")]
        public int RegionId { get; set; }
        /// <summary>
        /// 所在地详细机制
        /// </summary>
        [StringLength(75, ErrorMessage = "所在地长度不能大于75")]
        public string Address { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        [StringLength(150, ErrorMessage = "简介长度不能大于125")]
        public string Bio { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        [StringLength(25, ErrorMessage = "银行名称不能大于25")]
        public string BankName { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        [StringLength(30, ErrorMessage = "银行卡号不能大于30")]
        public string BankCardCode { get; set; }
        /// <summary>
        /// 银行开户人
        /// </summary>
        [StringLength(15, ErrorMessage = "银行开户人不能大于15")]
        public string BankUserName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (!SecureHelper.IsSafeSqlString(UserName))
            {
                errorList.Add(new ValidationResult("用户名中包含不安全的字符,请删除!", new string[] { "UserName" }));
            }
            if (!string.IsNullOrEmpty(UserName))
            {
                Regex regNum = new Regex(@"[\u4e00-\u9fa5]+");
                if (regNum.IsMatch(UserName))
                {
                    errorList.Add(new ValidationResult("用户名中不能包含中文!", new string[] { "UserName" }));
                }
            }
            return errorList;
        }
    }
    /// <summary>
    /// 用户模型类
    /// </summary>
    public class EditUserDetailModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 性别(0代表未知，1代表男，2代表女)
        /// </summary>
        [Range(0, 2, ErrorMessage = "请选择确切的性别")]
        [DisplayName("性别")]
        public int Gender { get; set; }
        /// <summary>
        /// 真实名称
        /// </summary>
        [StringLength(5, ErrorMessage = "名称长度不能大于5")]
        public string RealName { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        [DisplayName("出生日期")]
        public DateTime? Bday { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [IdCard]
        public string IdCard { get; set; }
        /// <summary>
        /// 所在地区域
        /// </summary>
        [DisplayName("区域")]
        public int RegionId { get; set; }
        /// <summary>
        /// 所在地详细机制
        /// </summary>
        [StringLength(75, ErrorMessage = "所在地长度不能大于75")]
        public string Address { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        [StringLength(150, ErrorMessage = "简介长度不能大于125")]
        public string Bio { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        [StringLength(25, ErrorMessage = "银行名称不能大于25")]
        public string BankName { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        [StringLength(30, ErrorMessage = "银行卡号不能大于30")]
        public string BankCardCode { get; set; }
        /// <summary>
        /// 银行开户人
        /// </summary>
        [StringLength(15, ErrorMessage = "银行开户人不能大于15")]
        public string BankUserName { get; set; }
    }

    /// <summary>
    /// 用户模型类
    /// </summary>
    public class UserRank2Model
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 用户等级id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择正确的用户等级")]
        [DisplayName("用户等级")]
        public int UserRid { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(20, ErrorMessage = "名称长度不能大于20")]
        public string NickName { get; set; }

        /// <summary>
        /// 真实名称
        /// </summary>
        [StringLength(5, ErrorMessage = "名称长度不能大于5")]
        public string RealName { get; set; }
        /// <summary>
        /// 商务中心类型
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "请选择正确的类型")]
        [DisplayName("商务中心类型")]
        public int BusiCentType { get; set; }
        

    }
    /// <summary>
    /// 用户模型类
    /// </summary>
    public class UserCachModel
    {
        /// <summary>
        /// 最大汇购卡数
        /// </summary>
        [Required(ErrorMessage = "最大汇购卡数不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "最大汇购卡数不能为负数")]
        [DisplayName("最大汇购卡数")]
        public int MaxCashCount { get; set; }
    }
    /// <summary>
    /// 账户列表模型类
    /// </summary>
    public class AccountInfoListModel
    {
        /// <summary>
        /// 账户信息
        /// </summary>
        public List<AccountInfo> AccountInfoList { get; set; }
        public int Uid { get; set; }
        public string Username { get; set; }
    }
    /// <summary>
    /// 账户详情模型类
    /// </summary>
    public class AccountDetailModel
    {
        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 账户详情列表
        /// </summary>
        public List<AccountDetailInfo> AccountDetailList { get; set; }
        public int Uid { get; set; }
        public int AccountId { get; set; }
        public string OrderCode { get; set; }
    }

    /// <summary>
    /// 汇购卡退款申请那个模型类
    /// </summary>
    public class CashRefundApplyReturnModel
    {
        public CashRefundApplyReturnModel()
        {
            RefundPayFee = 0M;
            RefundDesc = "";
        }

        [Range(0, double.MaxValue, ErrorMessage = "手续费不能小于0")]
        [Required(ErrorMessage = "手续费不能为空")]
        public decimal RefundPayFee { get; set; }

        [StringLength(150, ErrorMessage = "退款说明不能超过150")]
        public string RefundDesc { get; set; }


    }
    /// <summary>
    /// 帐号充值模型类
    /// </summary>
    public class AccountRechargeModel
    {
        /// <summary>
        /// 帐号id
        /// </summary>
        [Required(ErrorMessage = "帐号id不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "帐号id不能为负数")]
        [DisplayName("帐号id")]
        public int AccountId { get; set; }//
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "用户id不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "用户id不能为负数")]
        [DisplayName("用户id")]
        public int UserId { get; set; }//用户id
        /// <summary>
        /// 充值金额
        /// </summary>
        [Required(ErrorMessage = "充值金额不能为空")]
        [Range(0, double.MaxValue, ErrorMessage = "充值金额不能为负数")]
        [DisplayName("充值金额")]
        public decimal InAmount { get; set; }

        public string AccountName { get; set; }//帐号名称

        /// <summary>
        /// 类型
        /// </summary>
        [Required(ErrorMessage = "类型不能为空")]
        [Range(0, 100, ErrorMessage = "类型不能为负数")]
        [DisplayName("类型")]
        public int DetailType { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        [StringLength(150, ErrorMessage = "备注说明长度不能大于1505")]
        [DisplayName("备注说明")]
        public string DetailDes { get; set; }
    }

    /// <summary>
    /// 帐号修改模型类
    /// </summary>
    public class AccountModifyModel
    {
        /// <summary>
        /// 帐号id
        /// </summary>
        [Required(ErrorMessage = "帐号id不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "帐号id不能为负数")]
        [DisplayName("帐号id")]
        public int AccountId { get; set; }//
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "用户id不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "用户id不能为负数")]
        [DisplayName("用户id")]
        public int UserId { get; set; }//用户id
        /// <summary>
        /// 金额
        /// </summary>
        [Required(ErrorMessage = "充值金额不能为空")]
        [Range(0, double.MaxValue, ErrorMessage = "充值金额不能为负数")]
        [DisplayName("充值金额")]
        public decimal Amount { get; set; }

        public string AccountName { get; set; }//帐号名称

        public string OrderCode { get; set; }//订单号
        /// <summary>
        /// 类型
        /// </summary>
        [Range(0, 100, ErrorMessage = "类型不能为负数")]
        [DisplayName("类型")]
        public int Type { get; set; }
        ///// <summary>
        ///// 类型
        ///// </summary>
        //[Required(ErrorMessage = "类型不能为空")]
        //[Range(0, 100, ErrorMessage = "类型不能为负数")]
        //[DisplayName("类型")]
        //public int DetailType { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        [StringLength(150, ErrorMessage = "备注说明长度不能大于1505")]
        [DisplayName("备注说明")]
        [Required(ErrorMessage = "备注说明不能为空")]
        public string DetailDes { get; set; }
    }

    /// <summary>
    /// 修改代理等级、代理折扣资格模型类
    /// </summary>
    public class RankModifyModel : IValidatableObject
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [Required(ErrorMessage = "用户id不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "用户id不能为负数")]
        [DisplayName("用户id")]
        public int UserId { get; set; }//用户id
        /// <summary>
        /// 用户名
        /// </summary>
        [DisplayName("用户名")]
        public string UserName { get; set; }//

        /// <summary>
        /// 手机
        /// </summary>
        [DisplayName("手机")]
        public string Moblie { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("姓名")]
        public string RealName { get; set; }

        public int OldAgentType { get; set; }//原来代理等级

        public int OldDS2AgentRank { get; set; }//原来折扣资格
        /// <summary>
        /// 新代理等级
        /// </summary>
        [DisplayName("新代理等级")]
        public int NewAgentType { get; set; }
        /// <summary>
        /// 新折扣资格
        /// </summary>
        [DisplayName("新折扣资格")]
        public int NewDS2AgentRank { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (NewAgentType > 0 && NewDS2AgentRank > 0)
                errorList.Add(new ValidationResult("代理等级和折扣资格不能同时存在!", new string[] { "NewAgentType" }));
            return errorList;
        }
    }

    /// <summary>
    /// 修改代理网络
    /// </summary>
    public class AgentNetModifyModel 
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [Required(ErrorMessage = "用户id不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "用户id不能为负数")]
        [DisplayName("用户id")]
        public int UserId { get; set; }//用户id
        /// <summary>
        /// 用户名
        /// </summary>
        [DisplayName("用户名")]
        public string UserName { get; set; }//

        /// <summary>
        /// 手机
        /// </summary>
        [DisplayName("手机")]
        public string Moblie { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("姓名")]
        public string RealName { get; set; }
        public int OldParentId { get; set; }//原来推荐id
        public int OldParentType { get; set; }//原来推荐类型
        public string OldParentName { get; set; }//原来推荐名
        public string OldRealName { get; set; }//原来推荐人姓名
        /// <summary>
        /// 新推荐id
        /// </summary>
        [DisplayName("新推荐id")]
        public int NewParentId { get; set; }
        /// <summary>
        /// 新推荐名
        /// </summary>
        [DisplayName("新推荐名")]
        public string NewParentName { get; set; }

    }
    /// <summary>
    /// 重销模型类
    /// </summary>
    public class ReorderPVModel
    {
        /// <summary>
        /// 账户信息
        /// </summary>
        public int Uid { get; set; }

        public decimal CurrentTotalPV { get; set; }
        public DateTime TargetDate { get; set; }
        public PartUserInfo UserInfo { get; set; }
        public DataTable ReorderList { get; set; }
    }

    /// <summary>
    /// 代理库存
    /// </summary>
    public class AgentStockModel
    {
        /// <summary>
        /// 状态
        /// </summary>
        public DataTable AgentStockList { get; set; }

    }
    /// <summary>
    /// 提现历史记录模型类
    /// </summary>
    public class AgentStockDetailModel
    {
        public int Pid { get; set; }
        public int Uid { get; set; }
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 历史记录列表
        /// </summary>
        public List<AgentStockDetailInfo> DetailList { get; set; }
    }
    /// <summary>
    ///  库存变更
    /// </summary>
    public class StockModifyModel
    {
        /// <summary>
        /// 产品id
        /// </summary>
        [Required(ErrorMessage = "产品id不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "产品id不能为负数")]
        [DisplayName("产品id")]
        public int Pid { get; set; }//
        /// <summary>
        /// 用户id
        /// </summary>
        [Required(ErrorMessage = "用户id不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "用户id不能为负数")]
        [DisplayName("用户id")]
        public int Uid { get; set; }//用户id

        public AgentStockInfo info { get; set; }//用户id

        /// <summary>
        /// 类型
        /// </summary>
        [Range(0, 100, ErrorMessage = "类型不能为负数")]
        [DisplayName("类型")]
        public int Type { get; set; }
        /// <金额
        /// </summary>
        [Required(ErrorMessage = "充值金额不能为空")]
        [Range(0, double.MaxValue, ErrorMessage = "充值金额不能为负数")]
        [DisplayName("充值金额")]
        public decimal Amount { get; set; }

        public string ProductName { get; set; }//产品名称
        public string UserName { get; set; }//产品名称
        public string OrderCode { get; set; }//订单号
        /// <summary>
        /// 备注说明
        /// </summary>
        [Required(ErrorMessage = "充值金额不能为空")]
        [StringLength(150, ErrorMessage = "备注说明长度不能大于1505")]
        [DisplayName("备注说明")]
        public string DetailDes { get; set; }
    }

    public class NetChartModel
    {
        public int LevelCount { get; set; }
        public int Uid { get; set; }
        public PartUserInfo user { get; set; }
    }

    public class OrderGiftListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 排序列
        /// </summary>
        public string SortColumn { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public string SortDirection { get; set; }
        /// <summary>
        /// 商品咨询列表
        /// </summary>
        public DataTable OrderGiftList { get; set; }
        /// <summary>
        /// uid
        /// </summary>
        public int Uid { get; set; }
        /// <summary>
        /// State
        /// </summary>
        public int State { get; set; }
        
        /// <summary>
        /// 咨询开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 咨询结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}
