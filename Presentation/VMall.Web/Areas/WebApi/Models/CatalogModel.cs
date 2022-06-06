using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

using VMall.Core;

namespace VMall.Web.Areas.WebApi.Models
{
    /// <summary>
    /// 分类模型类
    /// </summary>
    [DataContract]
    public class CategoryModel
    {
        /// <summary>
        /// 返回状态
        /// </summary>
        [DataMember(Order = 1)]
        public string State { get; set; }
        /// <summary>
        /// 分页对象
        /// </summary>
        [DataMember(Order = 2)]
        public int ProductTotalCount { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember(Order = 3)]
        public List<StoreProductInfo> ProductList { get; set; }
    }
}