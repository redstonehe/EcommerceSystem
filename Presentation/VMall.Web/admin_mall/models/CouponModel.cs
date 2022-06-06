using System;
using System.Data;
using System.Web.Mvc;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.MallAdmin.Models
{
    /// <summary>
    /// 优惠劵类型列表模型类
    /// </summary>
    public class CouponTypeListModel
    {
        /// <summary>
        /// 分页模型
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 优惠劵类型列表
        /// </summary>
        public DataTable CouponTypeList { get; set; }
        /// <summary>
        /// 店铺id
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 优惠劵类型名称
        /// </summary>
        public string CouponTypeName { get; set; }
    }

    /// <summary>
    /// 优惠劵类型模型类
    /// </summary>
    public class CouponTypeModel : IValidatableObject
    {
        public CouponTypeModel()
        {
            StoreId = -1;
            StoreName = "选择店铺";
            LimitStoreCid = -1;
            SendStartTime = DateTime.Now;
            SendEndTime = DateTime.Now;
            UseStartTime = DateTime.Now;
            UseEndTime = DateTime.Now;
            UseModel = 1;
            UserRankLower = 7;
        }
        /// <summary>
        /// 频道id
        /// </summary>

        [DisplayName("频道专区")]
        public string ChannelId { get; set; }

        /// <summary>
        /// 频道名称
        /// </summary>
        [DisplayName("频道专区")]
        public string ChannelName { get; set; }
        /// <summary>
        /// 店铺id
        /// </summary>
        [Range(-1, int.MaxValue, ErrorMessage = "请选择店铺")]
        [DisplayName("店铺")]
        public int StoreId { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 优惠劵类型名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(25, ErrorMessage = "名称长度不能大于25")]
        public string CouponTypeName { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [Required(ErrorMessage = "金额不能为空")]
        [Range(1, int.MaxValue, ErrorMessage = "金额必须大于0")]
        [DisplayName("金额")]
        public int Money { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [Required(ErrorMessage = "数量不能为空")]
        [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
        [DisplayName("数量")]
        public int Count { get; set; }
        /// <summary>
        /// 发放方式
        /// </summary>
        public int SendModel { get; set; }
        /// <summary>
        /// 获得方式
        /// </summary>
        public int GetModel { get; set; }
        /// <summary>
        /// 使用方式
        /// </summary>
        public int UseModel { get; set; }
        /// <summary>
        /// 最小用户等级
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择正确的用户等级")]
        [DisplayName("最低用户等级")]
        public int UserRankLower { get; set; }
        /// <summary>
        /// 最小订单金额为空
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "最小订单金额不能为负数")]
        [DisplayName("最小订单金额")]
        public int OrderAmountLower { get; set; }
        /// <summary>
        /// 限制店铺分类id
        /// </summary>
        public int LimitStoreCid { get; set; }
        /// <summary>
        /// 限制商品
        /// </summary>
        public int LimitProduct { get; set; }
        /// <summary>
        /// 发放开始时间
        /// </summary>
        [DisplayName("发放开始时间")]
        public DateTime? SendStartTime { get; set; }
        /// <summary>
        /// 发放结束时间
        /// </summary>
        [DisplayName("发放结束时间")]
        public DateTime? SendEndTime { get; set; }
        /// <summary>
        /// 使用时间类型
        /// </summary>
        public int UseTimeType { get; set; }
        /// <summary>
        /// 使用过期时间
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "使用过期时间不能为负数")]
        [DisplayName("使用过期时间")]
        public int UseExpireTime { get; set; }
        /// <summary>
        /// 使用开始时间
        /// </summary>
        [DisplayName("使用开始时间")]
        public DateTime? UseStartTime { get; set; }
        /// <summary>
        /// 使用结束时间
        /// </summary>
        [DisplayName("使用结束时间")]
        public DateTime? UseEndTime { get; set; }

        public int State { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            //验证发放时间
            if (SendModel == 0)
            {
                if (SendStartTime == null || SendEndTime == null)
                {
                    errorList.Add(new ValidationResult("发放时间不能为空!", new string[] { "SendEndTime" }));
                    return errorList;
                }
                if (SendEndTime <= SendStartTime)
                {
                    errorList.Add(new ValidationResult("发放结束时间必须大于发放开始时间!", new string[] { "SendEndTime" }));
                    return errorList;
                }
            }

            //验证使用时间
            if (UseTimeType == 0)
            {
                if (UseStartTime == null || UseEndTime == null)
                {
                    errorList.Add(new ValidationResult("使用时间不能为空!", new string[] { "UseEndTime" }));
                    return errorList;
                }
                else if (UseEndTime <= UseStartTime)
                {
                    errorList.Add(new ValidationResult("使用结束时间必须大于使用开始时间!", new string[] { "UseEndTime" }));
                    return errorList;
                }
            }

            if (SendModel == 0 && UseTimeType == 0)
            {
                if (UseStartTime < SendStartTime)
                    errorList.Add(new ValidationResult("使用开始时间必须小于发放开始时间!", new string[] { "UseStartTime" }));
                else if (UseEndTime < SendEndTime)
                    errorList.Add(new ValidationResult("使用结束时间必须小于发放结束时间!", new string[] { "UseEndTime" }));
            }

            return errorList;
        }
    }

    /// <summary>
    /// 优惠劵商品列表模型类
    /// </summary>
    public class CouponProductListModel
    {
        /// <summary>
        /// 优惠劵商品列表
        /// </summary>
        public DataTable CouponProductList { get; set; }
        public PageModel PageModel { get; set; }
        public int CouponTypeId { get; set; }
        public int StoreId { get; set; }
    }

    /// <summary>
    /// 优惠劵列表模型类
    /// </summary>
    public class CouponListModel
    {
        /// <summary>
        /// 分页模型
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 优惠劵列表
        /// </summary>
        public DataTable CouponList { get; set; }
        /// <summary>
        /// 优惠劵编号
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 优惠劵类型id
        /// </summary>
        public int CouponTypeId { get; set; }
    }

    /// <summary>
    /// 优惠劵发放模型类
    /// </summary>
    public class SendCouponModel : IValidatableObject
    {
        public SendCouponModel()
        { 
            CopyTid = "";
        }
        /// <summary>
        /// 优惠劵类型id
        /// </summary>
        public int CouponTypeId { get; set; }

        [Required(ErrorMessage = "数量不能为空")]
        [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
        [DisplayName("数量")]
        public int Count { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public int UType { get; set; }

        /// <summary>
        /// 用户值
        /// </summary>
        public string UValue { get; set; }

        public string CopyTid { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (UType == 0)
            {
                if (Count > 10)
                    errorList.Add(new ValidationResult("最多只能发放10张优惠劵!", new string[] { "Count" }));

                if (string.IsNullOrWhiteSpace(UValue))
                {
                    errorList.Add(new ValidationResult("请输入用户id!", new string[] { "UValue" }));
                }
                else
                {
                    PartUserInfo partUserInfo = Users.GetPartUserById(TypeHelper.StringToInt(UValue));
                    if (partUserInfo == null)
                        errorList.Add(new ValidationResult("请输入正确的用户id!", new string[] { "UValue" }));
                }
            }
            else if (UType == 1)
            {
                if (Count > 10)
                    errorList.Add(new ValidationResult("最多只能发放10张优惠劵!", new string[] { "Count" }));

                if (string.IsNullOrWhiteSpace(UValue))
                {
                    errorList.Add(new ValidationResult("请输入账户名!", new string[] { "UValue" }));
                }
                else
                {
                    if (AdminUsers.GetUidByAccountName(UValue) < 1)
                        errorList.Add(new ValidationResult("账户名不存在!", new string[] { "UValue" }));
                }
            }

            return errorList;
        }
    }

    /// <summary>
    /// 汇购卡券列表模型类
    /// </summary>
    public class CashCouponListModel
    {
        /// <summary>
        /// 分页模型
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 卡券列表
        /// </summary>
        public DataTable CashCouponList { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CSN { get; set; }
        /// <summary>
        /// 有效类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 关联用户
        /// </summary>
        public string UserName { get; set; }
    }

    /// <summary>
    /// 汇购卡券模型类
    /// </summary>
    public class CashCouponModel : IValidatableObject
    {
        public CashCouponModel()
        {

            CreationDate = DateTime.Now;
            CouponType = 1;

        }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        [Required(ErrorMessage = "创建时间不能为空")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// 汇购卡券类型
        /// </summary>
        [DisplayName("汇购卡券类型")]
        [Range(0, int.MaxValue, ErrorMessage = "汇购卡券类型不能小于0")]
        public int CouponType { get; set; }

        /// <summary>
        /// 会员id
        /// </summary>
        [DisplayName("会员id")]
        [Range(0, int.MaxValue, ErrorMessage = "会员id不能小于0")]
        public int Uid { get; set; }
        /// <summary>
        /// 会员名（手机、邮箱、用户名）
        /// </summary>
        [DisplayName("会员名")]
        [Required(ErrorMessage = "会员名不能为空")]
        public string UserName { get; set; }

        /// <summary>
        /// 汇购卡金额面值
        /// </summary>
        [DisplayName("面值")]
        [Required(ErrorMessage = "面值不能为空")]
        [Range(1, 99999, ErrorMessage = "面值不能小于1大于99999")]
        public decimal CashAmount { get; set; }

        /// <summary>
        /// 直销会员id
        /// </summary>
        [DisplayName("直销会员id")]
        public int DirSaleUid { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (!CashCoupon.isDSUser(UserName))
                errorList.Add(new ValidationResult("会员不存在或未登陆过!", new string[] { "UserName" }));

            return errorList;
        }
    }

    /// <summary>
    /// 汇购卡券列表模型类
    /// </summary>
    public class CashCouponDetailListModel
    {
        /// <summary>
        /// 分页模型
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 卡券列表
        /// </summary>
        public List<CashCouponDetailInfo> CashCouponDetailList { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public int CashId { get; set; }

    }

    /// <summary>
    /// 海米提现列表模型类
    /// </summary>
    public class HaiMiDrawCashListModel
    {
        /// <summary>
        /// 分页模型
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 卡券列表
        /// </summary>
        public DataTable DrawCashList { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CSN { get; set; }
        /// <summary>
        /// 有效类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 账户id
        /// </summary>
        public int AccountId { get; set; }
        /// <summary>
        /// 关联用户
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }
    }

    /// <summary>
    /// 佣金查询模型类
    /// </summary>
    public class IncomeAccountModel
    {
        /// <summary>
        /// 分页模型
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 卡券列表
        /// </summary>
        public DataTable IncomeAccountList { get; set; }
        /// <summary>
        /// 账户id
        /// </summary>
        public int AccountId { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string RealName { get; set; }
        public int Type { get; set; }

    }

    /// <summary>
    /// 兑换码列表模型类
    /// </summary>
    public class ExChangeCouponListModel
    {
        /// <summary>
        /// 分页模型
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 兑换码列表
        /// </summary>
        public DataTable ExChangeCouponList { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string EXSN { get; set; }
        /// <summary>
        /// 有效类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 关联用户
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 兑换码类型id
        /// </summary>
        public int CodeTypeId { get; set; }
    }

    /// <summary>
    /// 兑换码模型类
    /// </summary>
    public class ExChangeCouponModel : IValidatableObject
    {
        public ExChangeCouponModel()
        {
            ValidTime = DateTime.Now;
            Type = 2;
        }
        /// <summary>
        /// 过期时间
        /// </summary>
        [DisplayName("创建时间")]
        [Required(ErrorMessage = "创建时间不能为空")]
        public DateTime ValidTime { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [DisplayName("类型")]
        [Range(0, int.MaxValue, ErrorMessage = "类型不能小于0")]
        public int Type { get; set; }
        /// <summary>
        /// 兑换码类型id
        /// </summary>
        [DisplayName("类型")]
        [Range(1, int.MaxValue, ErrorMessage = "类型不能小于1")]
        public int CodeTypeId { get; set; }
        /// <summary>
        /// 会员id
        /// </summary>
        [DisplayName("会员id")]
        [Range(0, int.MaxValue, ErrorMessage = "会员id不能小于0")]
        public int Uid { get; set; }
        /// <summary>
        /// 会员名（手机、邮箱、用户名）
        /// </summary>
        [DisplayName("会员名")]
        [Required(ErrorMessage = "会员名不能为空")]
        public string UserName { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (!ExChangeCoupons.isExistUser(UserName))
                errorList.Add(new ValidationResult("会员不存在!", new string[] { "UserName" }));

            return errorList;
        }
    }

    /// <summary>
    /// 兑换码类型列表模型类
    /// </summary>
    public class CodeTypeListModel
    {
        /// <summary>
        /// 分页模型
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 兑换码类型列表
        /// </summary>
        public DataTable CodeTypeList { get; set; }
        /// <summary>
        /// 店铺id
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 兑换码类型名称
        /// </summary>
        public string CodeTypeName { get; set; }
    }

    /// <summary>
    /// 兑换码类型模型类
    /// </summary>
    public class CodeTypeModel : IValidatableObject
    {
        public CodeTypeModel()
        {
            StoreId = 0;
            StoreName = "";
            LimitStoreCid = -1;
            SendStartTime = DateTime.Now;
            SendEndTime = DateTime.Now;
            UseStartTime = DateTime.Now;
            UseEndTime = DateTime.Now;
            UseModel = 1;
            GetModel = 0;
            UserRankLower = 7;
            Money = 0;
            Count = 0;
            SendModel = 1;
            LimitProduct = 1;
        }

        /// <summary>
        /// 店铺id
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "请选择店铺")]
        [DisplayName("店铺")]
        public int StoreId { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 兑换码类型名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(25, ErrorMessage = "名称长度不能大于25")]
        public string CodeTypeName { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [Required(ErrorMessage = "金额不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "金额必须大于0")]
        [DisplayName("金额")]
        public int Money { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [Required(ErrorMessage = "数量不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "数量必须大于0")]
        [DisplayName("数量")]
        public int Count { get; set; }
        /// <summary>
        /// 发放方式
        /// </summary>
        public int SendModel { get; set; }
        /// <summary>
        /// 获得方式
        /// </summary>
        public int GetModel { get; set; }
        /// <summary>
        /// 使用方式
        /// </summary>
        public int UseModel { get; set; }
        /// <summary>
        /// 最小用户等级
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择正确的用户等级")]
        [DisplayName("最低用户等级")]
        public int UserRankLower { get; set; }
        /// <summary>
        /// 最小订单金额为空
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "最小订单金额不能为负数")]
        [DisplayName("最小订单金额")]
        public int OrderAmountLower { get; set; }
        /// <summary>
        /// 限制店铺分类id
        /// </summary>
        public int LimitStoreCid { get; set; }
        /// <summary>
        /// 限制商品
        /// </summary>
        public int LimitProduct { get; set; }
        /// <summary>
        /// 发放开始时间
        /// </summary>
        [DisplayName("发放开始时间")]
        public DateTime? SendStartTime { get; set; }
        /// <summary>
        /// 发放结束时间
        /// </summary>
        [DisplayName("发放结束时间")]
        public DateTime? SendEndTime { get; set; }
        /// <summary>
        /// 使用时间类型
        /// </summary>
        public int UseTimeType { get; set; }
        /// <summary>
        /// 使用过期时间
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "使用过期时间不能为负数")]
        [DisplayName("使用过期时间")]
        public int UseExpireTime { get; set; }
        /// <summary>
        /// 使用开始时间
        /// </summary>
        [DisplayName("使用开始时间")]
        public DateTime? UseStartTime { get; set; }
        /// <summary>
        /// 使用结束时间
        /// </summary>
        [DisplayName("使用结束时间")]
        public DateTime? UseEndTime { get; set; }

        public int State { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            //验证发放时间
            if (SendModel == 0)
            {
                if (SendStartTime == null || SendEndTime == null)
                {
                    errorList.Add(new ValidationResult("发放时间不能为空!", new string[] { "SendEndTime" }));
                    return errorList;
                }
                if (SendEndTime <= SendStartTime)
                {
                    errorList.Add(new ValidationResult("发放结束时间必须大于发放开始时间!", new string[] { "SendEndTime" }));
                    return errorList;
                }
            }

            //验证使用时间
            if (UseTimeType == 0)
            {
                if (UseStartTime == null || UseEndTime == null)
                {
                    errorList.Add(new ValidationResult("使用时间不能为空!", new string[] { "UseEndTime" }));
                    return errorList;
                }
                else if (UseEndTime <= UseStartTime)
                {
                    errorList.Add(new ValidationResult("使用结束时间必须大于使用开始时间!", new string[] { "UseEndTime" }));
                    return errorList;
                }
            }

            if (SendModel == 0 && UseTimeType == 0)
            {
                if (UseStartTime < SendStartTime)
                    errorList.Add(new ValidationResult("使用开始时间必须小于发放开始时间!", new string[] { "UseStartTime" }));
                else if (UseEndTime < SendEndTime)
                    errorList.Add(new ValidationResult("使用结束时间必须小于发放结束时间!", new string[] { "UseEndTime" }));
            }

            return errorList;
        }
    }

    /// <summary>
    /// 兑换码商品列表模型类
    /// </summary>
    public class CodeProductListModel
    {
        /// <summary>
        /// 兑换码商品列表
        /// </summary>
        public DataTable CodeProductList { get; set; }
        public PageModel PageModel { get; set; }
        public int CodeTypeId { get; set; }
        public int StoreId { get; set; }
    }


}
