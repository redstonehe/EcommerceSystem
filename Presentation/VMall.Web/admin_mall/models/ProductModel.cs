﻿using System;
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
    /// 商品列表模型类
    /// </summary>
    public class ProductListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public DataTable ProductList { get; set; }
        /// <summary>
        /// 排序列
        /// </summary>
        public string SortColumn { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public string SortDirection { get; set; }
        /// <summary>
        /// 店铺id
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 分类id
        /// </summary>
        public int CateId { get; set; }
        /// <summary>
        /// 品牌id
        /// </summary>
        public int BrandId { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public int Pid { get; set; }
    }

    /// <summary>
    /// 添加商品模型类
    /// </summary>
    public class AddProductModel
    {
        public AddProductModel()
        {
            CateId = -1;
            CategoryName = "选择分类";
            ChannelId = "";
            ChannelName = "选择专区";
            BrandId = -1;
            BrandName = "选择品牌";
            StoreId = -1;
            StoreName = "选择店铺";
            StoreCid = -1;
            StoreSTid = -1;
            IsBest = false;
            IsHot = false;
            IsNew = false;
            State = 1;
            SaleType = 0;
            Unit = "";
        }
        /// <summary>
        /// 商品规格
        /// </summary>
        [DisplayName("商品规格")]
        [StringLength(50, ErrorMessage = "名称长度不能大于50")]
        public string Unit { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        [StringLength(15, ErrorMessage = "货号长度不能大于15")]
        public string PSN { get; set; }

        /// <summary>
        /// 分类id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择分类")]
        [DisplayName("商品分类")]
        public int CateId { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 频道id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择专区")]
        [DisplayName("商品专区")]
        public string ChannelId { get; set; }

        /// <summary>
        /// 频道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 品牌id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择品牌")]
        [DisplayName("商品品牌")]
        public int BrandId { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 店铺id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择店铺")]
        [DisplayName("店铺")]
        public int StoreId { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 店铺分类id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择店铺分类")]
        [DisplayName("店铺分类")]
        public int StoreCid { get; set; }

        /// <summary>
        /// 店铺配送模板id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择店铺配送模板")]
        [DisplayName("店铺配送模板")]
        public int StoreSTid { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(100, ErrorMessage = "名称长度不能大于100")]
        public string ProductName { get; set; }

        /// <summary>
        /// 商城价
        /// </summary>
        [DisplayName("商城价")]
        [Required(ErrorMessage = "商城价不能为空")]
        public decimal ShopPrice { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        [DisplayName("市场价")]
        [Required(ErrorMessage = "市场价不能为空")]
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 成本价
        /// </summary>
        [DisplayName("成本价")]
        [Required(ErrorMessage = "成本价不能为空")]
        public decimal CostPrice { get; set; }

        /// <summary>
        /// PV值
        /// </summary>
        [DisplayName("PV")]
        [Required(ErrorMessage = "PV值不能为空")]
        public decimal PV { get; set; }

        /// <summary>
        /// 鹿毛
        /// </summary>
        [DisplayName("鹿毛")]
        [Required(ErrorMessage = "鹿毛不能为空")]
        public decimal HaiMi { get; set; }

        /// <summary>
        /// 红包减免
        /// </summary>
        [DisplayName("积分比例")]
        [Required(ErrorMessage = "积分比例不能为空")]
        public decimal HongBaoCut { get; set; }

        /// <summary>
        /// 最低起购数量
        /// </summary>
        [DisplayName("最低起购数量")]
        [Required(ErrorMessage = "最低起购数量不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "请填写最低起购数量")]
        public int MinBuyCount { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        [DisplayName("重量")]
        [Required(ErrorMessage = "重量不能为空")]
        public int Weight { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        [Required(ErrorMessage = "排序不能为空")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        [DisplayName("库存数量")]
        [Required(ErrorMessage = "库存数量不能为空")]
        [IntNotLess("StockLimit", "库存警戒线")]
        public int StockNumber { get; set; }

        /// <summary>
        /// 库存警戒线
        /// </summary>
        [DisplayName("库存警戒线")]
        [Required(ErrorMessage = "库存警戒线不能为空")]
        public int StockLimit { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        [Range(0, 1, ErrorMessage = "请选择商品状态")]
        [DisplayName("商品状态")]
        public int State { get; set; }

        /// <summary>
        /// 是否为精品
        /// </summary>
        public bool IsBest { get; set; }

        /// <summary>
        /// 是否为热销
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        /// 是否为新品
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 全球购产品类型
        /// </summary>
        [Range(0, 10, ErrorMessage = "请选择全球购商品类型")]
        [DisplayName("全球购商品类型")]
        public int SaleType { get; set; }

        /// <summary>
        /// 关税税率
        /// </summary>
        [Range(0, 100, ErrorMessage = "请填写关税税率")]
        [DisplayName("关税税率")]
        public decimal Taxrate { get; set; }
        /// <summary>
        /// 结算比例
        /// </summary>
        [Range(0, 100, ErrorMessage = "请填写结算比例")]
        [DisplayName("结算比例")]
        public int SettlePercent { get; set; }
        /// <summary>
        /// 复购周期
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "请填写复购周期")]
        [DisplayName("复购周期")]
        public int ReBuyCycle { get; set; }
        /// <summary>
        /// 关联复购产品
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "请填写关联复购产品")]
        [DisplayName("关联复购产品")]
        public int RelateReBuyPid { get; set; }
        /// <summary>
        /// 电脑版商品介绍
        /// </summary>
        [AllowHtml]
        public string Description { get; set; }
        /// <summary>
        /// 商品参数
        /// </summary>
        [AllowHtml]
        public string ProductParam { get; set; }
        /// <summary>
        /// 手机版商品介绍
        /// </summary>
        [AllowHtml]
        public string MobileDescription { get; set; }

        /// <summary>
        /// 属性id列表
        /// </summary>
        public int[] AttrIdList { get; set; }

        /// <summary>
        /// 属性值id列表
        /// </summary>
        public int[] AttrValueIdList { get; set; }

        /// <summary>
        /// 属性输入值列表
        /// </summary>
        public string[] AttrInputValueList { get; set; }

        /// <summary>
        /// 产品视频外链
        /// </summary>
        public string VideoUrl { get; set; }
    }

    /// <summary>
    /// 编辑商品模型类
    /// </summary>
    public class EditProductModel
    {
        public EditProductModel()
        {
            BrandId = -1;
            BrandName = "选择品牌";
            StoreCid = -1;
            StoreSTid = -1;
            IsBest = false;
            IsHot = false;
            IsNew = false;
            State = 1;
            SaleType = 0;
            Unit = "";
        }
        /// <summary>
        /// 商品规格
        /// </summary>
        [DisplayName("商品规格")]
        [StringLength(50, ErrorMessage = "名称长度不能大于50")]
        public string Unit { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        [StringLength(15, ErrorMessage = "货号长度不能大于15")]
        public string PSN { get; set; }

        /// <summary>
        /// 品牌id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择品牌")]
        [DisplayName("商品品牌")]
        public int BrandId { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 店铺分类id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择店铺分类")]
        [DisplayName("店铺分类")]
        public int StoreCid { get; set; }

        /// <summary>
        /// 店铺配送模板id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择店铺配送模板")]
        [DisplayName("店铺配送模板")]
        public int StoreSTid { get; set; }

        /// <summary>
        /// 商品副名称
        /// </summary>
        [StringLength(100, ErrorMessage = "名称长度不能大于100")]
        public string SubTitle { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(100, ErrorMessage = "名称长度不能大于100")]
        public string ProductName { get; set; }

        /// <summary>
        /// 商城价
        /// </summary>
        [DisplayName("商城价")]
        [Required(ErrorMessage = "商城价不能为空")]
        public decimal ShopPrice { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        [DisplayName("市场价")]
        [Required(ErrorMessage = "市场价不能为空")]
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 成本价
        /// </summary>
        [DisplayName("成本价")]
        [Required(ErrorMessage = "成本价不能为空")]
        public decimal CostPrice { get; set; }


        /// <summary>
        /// PV值
        /// </summary>
        [DisplayName("PV")]
        [Required(ErrorMessage = "PV值不能为空")]
        public decimal PV { get; set; }

        /// <summary>
        /// 鹿毛
        /// </summary>
        [DisplayName("鹿毛")]
        [Required(ErrorMessage = "鹿毛不能为空")]
        public decimal HaiMi { get; set; }

        /// <summary>
        /// 积分比例
        /// </summary>
        [DisplayName("积分比例")]
        [Required(ErrorMessage = "积分比例不能为空")]
        public decimal HongBaoCut { get; set; }

        /// <summary>
        /// 最低起购数量
        /// </summary>
        [DisplayName("最低起购数量")]
        [Required(ErrorMessage = "最低起购数量不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "请填写最低起购数量")]
        public int MinBuyCount { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        [DisplayName("重量")]
        [Required(ErrorMessage = "重量不能为空")]
        public int Weight { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        [Required(ErrorMessage = "排序不能为空")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        [DisplayName("库存数量")]
        [Required(ErrorMessage = "库存数量不能为空")]
        [IntNotLess("StockLimit", "库存警戒线")]
        public int StockNumber { get; set; }

        /// <summary>
        /// 库存警戒线
        /// </summary>
        [DisplayName("库存警戒线")]
        [Required(ErrorMessage = "库存警戒线不能为空")]
        public int StockLimit { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        [Range(0, 1, ErrorMessage = "请选择商品状态")]
        [DisplayName("商品状态")]
        public int State { get; set; }

        /// <summary>
        /// 全球购产品类型
        /// </summary>
        [Range(0, 10, ErrorMessage = "请选择全球购商品类型")]
        [DisplayName("全球购商品类型")]
        public int SaleType { get; set; }
        /// <summary>
        /// 关税税率
        /// </summary>
        [Range(0, 100, ErrorMessage = "请填写关税税率")]
        [DisplayName("关税税率")]
        public decimal Taxrate { get; set; }
        /// <summary>
        /// 结算比例
        /// </summary>
        [Range(0, 100, ErrorMessage = "请填写结算比例")]
        [DisplayName("结算比例")]
        public int SettlePercent { get; set; }
        /// <summary>
        /// 复购周期
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "请填写复购周期")]
        [DisplayName("复购周期")]
        public int ReBuyCycle { get; set; }
        /// <summary>
        /// 关联复购产品
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "请填写关联复购产品")]
        [DisplayName("关联复购产品")]
        public int RelateReBuyPid { get; set; }

        /// <summary>
        /// cateid
        /// </summary>
        public int CateId { get; set; }

        /// <summary>
        /// SKUGid
        /// </summary>
        public int SKUGid { get; set; }

        /// <summary>
        /// 是否为精品
        /// </summary>
        public bool IsBest { get; set; }

        /// <summary>
        /// 是否为热销
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        /// 是否为新品
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 电脑商品介绍
        /// </summary>
        [AllowHtml]
        public string Description { get; set; }
        /// <summary>
        /// 商品参数
        /// </summary>
        [AllowHtml]
        public string ProductParam { get; set; }
        /// <summary>
        /// 手机版商品介绍
        /// </summary>
        [AllowHtml]
        public string MobileDescription { get; set; }
        /// <summary>
        /// 排序区间-显示，编辑页面不修改
        /// </summary>
        public int ShowOrder { get; set; }
        /// <summary>
        /// 产品视频外链
        /// </summary>
        public string VideoUrl { get; set; }
    }

    /// <summary>
    /// 添加SKU模型类
    /// </summary>
    public class AddSKUModel
    {
        public AddSKUModel()
        {
            CateId = -1;
            CategoryName = "选择分类";
            ChannelId = "";
            ChannelName = "选择频道";
            BrandId = -1;
            BrandName = "选择品牌";
            StoreId = -1;
            StoreName = "选择店铺";
            StoreCid = -1;
            StoreSTid = -1;
            IsBest = false;
            IsHot = false;
            IsNew = false;
        }

        /// <summary>
        /// 分类id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择分类")]
        [DisplayName("商品分类")]
        public int CateId { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 频道id
        /// </summary>
        //[Range(1, int.MaxValue, ErrorMessage = "请选择频道")]
        [DisplayName("商品频道")]
        public string ChannelId { get; set; }

        /// <summary>
        /// 频道名称
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// 品牌id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择品牌")]
        [DisplayName("商品品牌")]
        public int BrandId { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 店铺id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择店铺")]
        [DisplayName("店铺")]
        public int StoreId { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 店铺分类id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择店铺分类")]
        [DisplayName("店铺分类")]
        public int StoreCid { get; set; }

        /// <summary>
        /// 店铺配送模板id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择店铺配送模板")]
        [DisplayName("店铺配送模板")]
        public int StoreSTid { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(100, ErrorMessage = "名称长度不能大于100")]
        public string ProductName { get; set; }

        /// <summary>
        /// 商城价
        /// </summary>
        [DisplayName("商城价")]
        [Required(ErrorMessage = "商城价不能为空")]
        public decimal ShopPrice { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        [DisplayName("市场价")]
        [Required(ErrorMessage = "市场价不能为空")]
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 成本价
        /// </summary>
        [DisplayName("成本价")]
        [Required(ErrorMessage = "成本价不能为空")]
        public decimal CostPrice { get; set; }


        /// <summary>
        /// PV值
        /// </summary>
        [DisplayName("PV")]
        [Required(ErrorMessage = "PV值不能为空")]
        public decimal PV { get; set; }

        /// <summary>
        /// 鹿毛
        /// </summary>
        [DisplayName("鹿毛")]
        [Required(ErrorMessage = "鹿毛不能为空")]
        public decimal HaiMi { get; set; }

        /// <summary>
        /// 红包减免
        /// </summary>
        [DisplayName("积分比例")]
        [Required(ErrorMessage = "积分比例不能为空")]
        public decimal HongBaoCut { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        [DisplayName("重量")]
        [Required(ErrorMessage = "重量不能为空")]
        public int Weight { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        [Required(ErrorMessage = "排序不能为空")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 是否为精品
        /// </summary>
        public bool IsBest { get; set; }

        /// <summary>
        /// 是否为热销
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        /// 是否为新品
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        [AllowHtml]
        public string Description { get; set; }

        /// <summary>
        /// 商品参数
        /// </summary>
        [AllowHtml]
        public string ProductParam { get; set; }

        /// <summary>
        /// 手机版商品介绍
        /// </summary>
        [AllowHtml]
        public string MobileDescription { get; set; }
        /// <summary>
        /// 属性id列表
        /// </summary>
        public int[] AttrIdList { get; set; }

        /// <summary>
        /// 属性值id列表
        /// </summary>
        public int[] AttrValueIdList { get; set; }

        /// <summary>
        /// 属性输入值列表
        /// </summary>
        public string[] AttrInputValueList { get; set; }
    }
    /// <summary>
    /// 添加SKU模型类
    /// </summary>
    public class AddProductToSKUModel {
        public AddProductToSKUModel()
        {
            Pid = -1;
            targetpid = -1;
            attrid = -1;
            attrvalueid = -1;
           
        }
        /// <summary>
        /// 分类id
        /// </summary>
        public int Pid { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public int targetpid { get; set; }

        /// <summary>
        /// 频道id
        /// </summary>
       
        public int attrid { get; set; }
        /// <summary>
        /// 频道名称
        /// </summary>
        public int attrvalueid { get; set; }
        
    }
    /// <summary>
    /// 商品图片列表模型类
    /// </summary>
    public class ProductImageListModel
    {
        /// <summary>
        /// 商品图片列表
        /// </summary>
        public List<ProductImageInfo> ProductImageList { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int Pid { get; set; }
        /// <summary>
        /// 店铺id
        /// </summary>
        public int StoreId { get; set; }
    }

    /// <summary>
    /// 商品关键词列表模型类
    /// </summary>
    public class ProductKeywordListModel
    {
        /// <summary>
        /// 商品关键词列表
        /// </summary>
        public List<ProductKeywordInfo> ProductKeywordList { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        public int Pid { get; set; }
    }

    /// <summary>
    /// 关联商品列表模型类
    /// </summary>
    public class RelateProductListModel
    {
        /// <summary>
        /// 关联商品列表
        /// </summary>
        public DataTable RelateProductList { get; set; }
        /// <summary>
        /// 主商品id
        /// </summary>
        public int Pid { get; set; }
        /// <summary>
        /// 店铺id
        /// </summary>
        public int StoreId { get; set; }
    }

    /// <summary>
    /// 定时商品列表模型类
    /// </summary>
    public class TimeProductListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 定时商品列表
        /// </summary>
        public DataTable TimeProductList { get; set; }
        /// <summary>
        /// 店铺id
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
    }

    /// <summary>
    /// 定时商品模型类
    /// </summary>
    public class TimeProductModel : IValidatableObject
    {
        /// <summary>
        /// 商品id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择商品")]
        public int Pid { get; set; }
        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime? OnSaleTime { get; set; }
        /// <summary>
        /// 下架时间
        /// </summary>
        public DateTime? OutSaleTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (OnSaleTime == null && OutSaleTime == null)
            {
                errorList.Add(new ValidationResult("上架时间不能为空!", new string[] { "OnSaleTime" }));
                errorList.Add(new ValidationResult("下架时间不能为空!", new string[] { "OutSaleTime" }));
            }
            else if (OnSaleTime != null && OutSaleTime != null && OnSaleTime.Value >= OutSaleTime.Value)
            {
                errorList.Add(new ValidationResult("下架时间必须大于上架时间!", new string[] { "OutSaleTime" }));
            }

            return errorList;
        }
    }
}
