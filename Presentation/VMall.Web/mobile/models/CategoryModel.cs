using System;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.Mobile.Models
{
    /// <summary>
    /// 分类列表模型类
    /// </summary>
    public class CategoryListModel
    {
        public int ChId { get; set; }
        public List<ChannelInfo> ChannelList { get; set; }
        public CategoryInfo CategoryInfo { get; set; }
        public List<CategoryInfo> CategoryList { get; set; }
        public List<CategoryInfo> AllCateory { get; set; }
        
    }
}