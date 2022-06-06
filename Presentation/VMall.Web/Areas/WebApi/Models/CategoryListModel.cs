using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

using VMall.Core;

namespace VMall.Web.Areas.WebApi.Models
{
    /// <summary>
    /// 分类列表模型类
    /// </summary>
    [DataContract]
    public class CategoryListModel
    {
        //[DataMember(Order = 1)]
        //public CategoryInfo CategoryInfo { get; set; }
        //[DataMember(Order = 2)]
        [DataMember(Order = 1)]
        public List<CategoryInfo> CategoryList { get; set; }
    }
}