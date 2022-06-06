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
    /// 店铺列表模型类
    /// </summary>
    public class StoreListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 店铺列表
        /// </summary>
        public DataTable StoreList { get; set; }
        /// <summary>
        /// 排序列
        /// </summary>
        public string SortColumn { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public string SortDirection { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 店铺等级id
        /// </summary>
        public int StoreRid { get; set; }
        /// <summary>
        /// 店铺行业id
        /// </summary>
        public int StoreIid { get; set; }
        /// <summary>
        /// 店铺状态
        /// </summary>
        public int State { get; set; }
    }

    /// <summary>
    /// 添加店铺模型类
    /// </summary>
    public class AddStoreModel : IValidatableObject
    {
        public AddStoreModel()
        {
            StateEndTime = DateTime.Now;
        }

        /// <summary>
        /// 店铺名称
        /// </summary>
        [Required(ErrorMessage = "店铺名称不能为空")]
        [StringLength(30, ErrorMessage = "店铺名称长度不能大于30")]
        public string StoreName { get; set; }
        /// <summary>
        /// 店长类型(0代表个人,1代表公司)
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 店长名称
        /// </summary>
        [Required(ErrorMessage = "店长名称不能为空")]
        [StringLength(50, ErrorMessage = "店长名称长度不能大于50")]
        public string StoreKeeperName { get; set; }
        /// <summary>
        /// 标识号
        /// </summary>
        [Required(ErrorMessage = "标识号不能为空")]
        [StringLength(25, ErrorMessage = "标识号长度不能大于25")]
        public string IdCard { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [Required(ErrorMessage = "地址不能为空")]
        [StringLength(150, ErrorMessage = "地址长度不能大于150")]
        public string Address { get; set; }
        /// <summary>
        /// 状态截止时间
        /// </summary>
        [DisplayName("状态截止时间")]
        public DateTime StateEndTime { get; set; }
        /// <summary>
        /// 店铺归属平台(0代表汇购,1代表尚睿淳咖啡,2代表有机胚芽微商订货系统，3代表汇购优选系统)
        /// </summary>
        public int MallSource { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (StateEndTime <= DateTime.Now)
            {
                errorList.Add(new ValidationResult("状态截止时间必须大于当前时间!", new string[] { "StateEndTime" }));
            }

            return errorList;
        }
    }

    /// <summary>
    /// 编辑店铺模型类
    /// </summary>
    public class EditStoreModel : IValidatableObject
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(30, ErrorMessage = "名称长度不能大于30")]
        public string StoreName { get; set; }
        /// <summary>
        /// 区域id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择区域")]
        [DisplayName("区域")]
        public int RegionId { get; set; }
        /// <summary>
        /// 等级id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择等级")]
        [DisplayName("等级")]
        public int StoreRid { get; set; }
        /// <summary>
        /// 行业id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择行业")]
        [DisplayName("行业")]
        public int StoreIid { get; set; }
        /// <summary>
        /// logo
        /// </summary>
        [StringLength(50, ErrorMessage = "logo长度不能大于50")]
        public string Logo { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        [Mobile]
        public string Mobile { get; set; }
        /// <summary>
        /// 固定电话
        /// </summary>
        [Phone]
        public string Phone { get; set; }
        /// <summary>
        /// qq
        /// </summary>
        [StringLength(11, ErrorMessage = "qq长度不能大于11")]
        public string QQ { get; set; }
        /// <summary>
        /// 阿里旺旺
        /// </summary>
        [StringLength(50, ErrorMessage = "阿里旺旺长度不能大于50")]
        public string WW { get; set; }
        /// <summary>
        /// 状态(0代表营业,1代表关闭)
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 状态截止时间
        /// </summary>
        public string StateEndTime { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// Banner
        /// </summary>
        [StringLength(50, ErrorMessage = "banner长度不能大于50")]
        public string Banner { get; set; }
        /// <summary>
        /// 公告
        /// </summary>
        [StringLength(100, ErrorMessage = "详细地址长度不能大于100")]
        public string Announcement { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(150, ErrorMessage = "描述长度不能大于150")]
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (State == 0)
            {
                if (string.IsNullOrWhiteSpace(StateEndTime))
                    errorList.Add(new ValidationResult("请输入时间!", new string[] { "StateEndTime" }));
                else if (TypeHelper.StringToDateTime(StateEndTime) <= DateTime.Now)
                    errorList.Add(new ValidationResult("状态截止时间必须大于当前时间!", new string[] { "StateEndTime" }));
            }

            return errorList;
        }
    }

    /// <summary>
    /// 编辑店铺优惠模型类
    /// </summary>
    public class EditStoreDiscountModel : IValidatableObject
    {
        public int StoreId { get; set; }
        /// <summary>
        /// 优惠价格梯度1
        /// </summary>
        [DisplayName("优惠价格梯度1")]
        [Range(0, double.MaxValue, ErrorMessage = "优惠价格梯度1不能超过最大值")]
        public decimal Amount1 { get; set; }
        /// <summary>
        /// 优惠折扣1
        /// </summary>
        [DisplayName("优惠折扣1")]
        [Range(0, double.MaxValue, ErrorMessage = "优惠折扣1不能超过最大值")]
        public decimal Discount1 { get; set; }
        /// <summary>
        /// 优惠价格梯度2
        /// </summary>
        [DisplayName("优惠价格梯度2")]
        [Range(0, double.MaxValue, ErrorMessage = "优惠价格梯度2不能超过最大值")]
        public decimal Amount2 { get; set; }
        /// <summary>
        /// 优惠折扣2
        /// </summary>
        [DisplayName("优惠折扣2")]
        [Range(0, double.MaxValue, ErrorMessage = "优惠折扣2不能超过最大值")]
        public decimal Discount2 { get; set; }
        /// <summary>
        /// 优惠价格梯度3
        /// </summary>
        [DisplayName("优惠价格梯度3")]
        [Range(0, double.MaxValue, ErrorMessage = "优惠价格梯度3不能超过最大值")]
        public decimal Amount3 { get; set; }
        /// <summary>
        /// 优惠折扣3
        /// </summary>
        [DisplayName("优惠折扣3")]
        [Range(0, double.MaxValue, ErrorMessage = "优惠折扣3不能超过最大值")]
        public decimal Discount3 { get; set; }
        /// <summary>
        /// 优惠价格梯度4
        /// </summary>
        [DisplayName("优惠价格梯度4")]
        [Range(0, double.MaxValue, ErrorMessage = "优惠价格梯度4不能超过最大值")]
        public decimal Amount4 { get; set; }
        /// <summary>
        /// 优惠折扣4
        /// </summary>
        [DisplayName("优惠折扣4")]
        [Range(0, double.MaxValue, ErrorMessage = "优惠折扣4不能超过最大值")]
        public decimal Discount4 { get; set; }
        /// <summary>
        /// 优惠价格梯度5
        /// </summary>
        [DisplayName("优惠价格梯度5")]
        [Range(0, double.MaxValue, ErrorMessage = "优惠价格梯度5不能超过最大值")]
        public decimal Amount5 { get; set; }
        /// <summary>
        /// 优惠折扣5
        /// </summary>
        [DisplayName("优惠折扣5")]
        [Range(0, double.MaxValue, ErrorMessage = "优惠折扣5不能超过最大值")]
        public decimal Discount5 { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (Amount2 > 0 && Amount2 <= Amount1)
                errorList.Add(new ValidationResult("优惠价格梯度2不能小于优惠价格梯度1", new string[] { "Amount2" }));
            //else if (Discount2 > 0 && Discount1 <= Discount2)
            //    errorList.Add(new ValidationResult("优惠折扣2不能小于优惠折扣1", new string[] { "Discount2" }));


            return errorList;
        }
    }

    /// <summary>
    /// 店长模型类
    /// </summary>
    public class StoreKeeperModel
    {
        /// <summary>
        /// 店长类型(0代表个人,1代表公司)
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 店长名称
        /// </summary>
        [Required(ErrorMessage = "店长名称不能为空")]
        [StringLength(50, ErrorMessage = "店长名称长度不能大于50")]
        public string StoreKeeperName { get; set; }
        /// <summary>
        /// 标识号
        /// </summary>
        [Required(ErrorMessage = "标识号不能为空")]
        [StringLength(25, ErrorMessage = "标识号长度不能大于25")]
        public string IdCard { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [Required(ErrorMessage = "地址不能为空")]
        [StringLength(150, ErrorMessage = "地址长度不能大于150")]
        public string Address { get; set; }
    }

    /// <summary>
    /// 店铺管理员模型类
    /// </summary>
    public class StoreAdminerModel
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required(ErrorMessage = "请输入账号")]
        public string AccountName { get; set; }
        /// <summary>
        /// 已有账号
        /// </summary>
        public DataTable AdminAccountList { get; set; }
    }


    /// <summary>
    /// 店铺分类列表模型类
    /// </summary>
    public class StoreClassListModel
    {
        public int StoreId { get; set; }
        public List<StoreClassInfo> StoreClassList { get; set; }
    }

    /// <summary>
    /// 店铺分类模型类
    /// </summary>
    public class StoreClassModel
    {
        /// <summary>
        /// 店铺分类名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(30, ErrorMessage = "名称长度不能大于30")]
        public string StoreClassName { get; set; }

        /// <summary>
        /// 父店铺分类id
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "请选择分类")]
        public int ParentId { get; set; }

        /// <summary>
        /// 店铺分类排序
        /// </summary>
        [Required(ErrorMessage = "排序不能为空")]
        [DisplayName("排序")]
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// 店铺配送模板列表模型类
    /// </summary>
    public class StoreShipTemplateListModel
    {
        public int StoreId { get; set; }
        public List<StoreShipTemplateInfo> StoreShipTemplateList { get; set; }
    }


    /// <summary>
    /// 添加店铺配送模板模型类
    /// </summary>
    public class AddStoreShipTemplateModel : IValidatableObject
    {
        public AddStoreShipTemplateModel()
        {
            StartValue = 1;
            AddValue = 1;
        }
        /// <summary>
        /// 模板标题
        /// </summary>
        [Required(ErrorMessage = "模板标题不能为空")]
        [StringLength(50, ErrorMessage = "模板标题长度不能大于50")]
        public string TemplateTitle { get; set; }
        /// <summary>
        /// 是否免运费(0代不免运费,1代表免运费，2代表满足条件后满运费)
        /// </summary>
        public int Free { get; set; }
        /// <summary>
        /// 计费类型(0代表按件数计算,1代表按重量计算)
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 包邮起步价格
        /// </summary>
        [DisplayName("包邮起步价格")]
        [Range(0, double.MaxValue, ErrorMessage = "起步值不能为负数")]
        public Decimal FreeStartPrice { get; set; }
        /// <summary>
        /// 模板备注
        /// </summary>
        [DisplayName("模板备注")]
        [StringLength(250, ErrorMessage = "模板备注长度不能大于250")]
        public string TemplateRemark { get; set; }
        /// <summary>
        /// 起步值
        /// </summary>
        [Required(ErrorMessage = "起步值不能为空")]
        [Range(1, int.MaxValue, ErrorMessage = "起步值不能为负数")]
        [DisplayName("起步值")]
        public int StartValue { get; set; }
        /// <summary>
        /// 起步价
        /// </summary>
        [Required(ErrorMessage = "起步价不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "起步价不能为负数")]
        [DisplayName("起步价")]
        public int StartFee { get; set; }
        /// <summary>
        /// 加值
        /// </summary>
        [Required(ErrorMessage = "加值不能为空")]
        [Range(1, int.MaxValue, ErrorMessage = "必须大于0")]
        [DisplayName("加值")]
        public int AddValue { get; set; }
        /// <summary>
        /// 加价
        /// </summary>
        [Required(ErrorMessage = "加价不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "加价不能为负数")]
        [DisplayName("加价")]
        public int AddFee { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if ((Free == 2 || Free == 3) && FreeStartPrice <= 0)
                errorList.Add(new ValidationResult("满足包邮条件下包邮起步价格不能等于0", new string[] { "FreeStartPrice" }));
            return errorList;
        }
    }

    /// <summary>
    /// 编辑店铺配送模板模型类
    /// </summary>
    public class EditStoreShipTemplateModel : IValidatableObject
    {
        /// <summary>
        /// 模板标题
        /// </summary>
        [Required(ErrorMessage = "模板标题不能为空")]
        [StringLength(50, ErrorMessage = "模板标题长度不能大于50")]
        public string TemplateTitle { get; set; }
        /// <summary>
        /// 是否免运费
        /// </summary>
        public int Free { get; set; }
        /// <summary>
        /// 计费类型(0代表按件数计算,1代表按重量计算)
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 包邮起步价格
        /// </summary>
        [DisplayName("包邮起步价格")]
        [Range(0, double.MaxValue, ErrorMessage = "起步值不能为负数")]
        public Decimal FreeStartPrice { get; set; }
        /// <summary>
        /// 模板备注
        /// </summary>
        [DisplayName("模板备注")]
        [Required(ErrorMessage = "模板备注")]
        [StringLength(250, ErrorMessage = "模板备注长度不能大于250")]
        public string TemplateRemark { get; set; }
        /// <summary>
        /// 不发货地区
        /// </summary>
        [DisplayName("不发货地区")]
        [StringLength(150, ErrorMessage = "不发货地区长度不能大于250")]
        public string NoSendArea { get; set; }
        /// <summary>
        /// 不发货城市
        /// </summary>
        [DisplayName("不发货城市")]
        [StringLength(150, ErrorMessage = "不发货城市长度不能大于500")]
        public string NoSendCity { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (Free == 2 && FreeStartPrice <= 0)
                errorList.Add(new ValidationResult("满足包邮条件下包邮起步价格不能等于0", new string[] { "FreeStartPrice" }));
            return errorList;
        }
    }

    /// <summary>
    /// 店铺配送费用列表模型类
    /// </summary>
    public class StoreShipFeeListModel
    {
        public int StoreSTid { get; set; }
        public int Type { get; set; }
        public int StoreId { get; set; }
        public List<StoreShipFeeInfo> StoreShipFeeList { get; set; }
        public int Free { get; set; }
    }


    /// <summary>
    /// 店铺配送费用模型类
    /// </summary>
    public class StoreShipFeeModel
    {
        public StoreShipFeeModel()
        {
            ShipType = 0;
            StartValue = 1;
            AddValue = 1;

        }
        /// <summary>
        /// 区域
        /// </summary>
        [Required(ErrorMessage = "请选择区域")]
        [StringLength(500, ErrorMessage = "区域组合长度不能大于500")]
        [DisplayName("区域")]
        public string RegionId { get; set; }

        /// <summary>
        /// 城市id
        /// </summary>
        [StringLength(500, ErrorMessage = "城市组合长度不能大于500")]
        [DisplayName("城市")]
        public string CityId { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>

        public string RegionName { get; set; }
        /// <summary>
        /// 起步值-首重/首件
        /// </summary>
        [Required(ErrorMessage = "起步值不能为空")]
        [Range(0, 10000, ErrorMessage = "起步值不能为负数")]
        [DisplayName("起步值")]
        public float StartValue { get; set; }
        /// <summary>
        /// 起步价 -首费
        /// </summary>
        [Required(ErrorMessage = "起步价不能为空")]
        [Range(0, 10000, ErrorMessage = "起步价不能为负数")]
        [DisplayName("起步价")]
        public decimal StartFee { get; set; }
        /// <summary>
        /// 加值  -续重/续件
        /// </summary>
        [Required(ErrorMessage = "加值不能为空")]
        [Range(0.01, 10000, ErrorMessage = "加值必须大于0")]
        [DisplayName("加值")]
        public float AddValue { get; set; }
        /// <summary>
        /// 加价--续费
        /// </summary>
        [Required(ErrorMessage = "加价不能为空")]
        [Range(0, 10000, ErrorMessage = "加价不能为负数")]
        [DisplayName("加价")]
        public decimal AddFee { get; set; }

        /// <summary>
        /// 运费类型
        /// </summary>
        [Required(ErrorMessage = "运费类型不能为空")]
        [Range(0, 1, ErrorMessage = "运费类型不正确")]
        [DisplayName("运费类型")]
        public int ShipType { get; set; }

        [HiddenInput]
        public int Type { get; set; }

        public int Free { get; set; }
    }
}
