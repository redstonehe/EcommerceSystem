using System;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.Mobile.Models
{
    /// <summary>
    /// 优惠劵类型列表模型类
    /// </summary>
    public class CouponTypeListModel
    {
        public List<CouponTypeInfo> CouponTypeList { get; set; }
    }

    /// <summary>
    /// 店铺分类模型类
    /// </summary>
    public class StoreClassModel
    {
        /// <summary>
        /// 店铺分类id
        /// </summary>
        public int StoreCid { get; set; }
        /// <summary>
        /// 排序列
        /// </summary>
        public int SortColumn { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public int SortDirection { get; set; }
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<PartProductInfo> ProductList { get; set; }
        /// <summary>
        /// 店铺分类信息
        /// </summary>
        public StoreClassInfo StoreClassInfo { get; set; }
    }

    /// <summary>
    /// 店铺搜索模型类
    /// </summary>
    public class StoreSearchModel
    {
        /// <summary>
        /// 搜索词
        /// </summary>
        public string Word { get; set; }
        /// <summary>
        /// 排序列
        /// </summary>
        public int SortColumn { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public int SortDirection { get; set; }
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<PartProductInfo> ProductList { get; set; }
    }
}