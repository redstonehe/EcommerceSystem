using System;
using System.Data;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.Models
{
    /// <summary>
    /// 新闻模型类
    /// </summary>
    public class NewsModel
    {
        public NewsInfo NewsInfo { get; set; }
        public List<NewsTypeInfo> NewsTypeList { get; set; }
    }

    /// <summary>
    /// 新闻列表模型类
    /// </summary>
    public class NewsListModel
    {
        public PageModel PageModel { get; set; }
        public DataTable NewsList { get; set; }
        public string NewsTitle { get; set; }
        public int NewsTypeId { get; set; }
        public List<NewsTypeInfo> NewsTypeList { get; set; }
    }
}