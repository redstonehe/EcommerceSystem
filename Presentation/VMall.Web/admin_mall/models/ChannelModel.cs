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
    public class GroupListModel
    {
        public int ChannelId { get; set; }
        public List<GroupProductInfo> GroupList { get; set; }
    }
    /// <summary>
    /// 模型类
    /// </summary>
    public class GroupProductModel 
    {
        public GroupProductModel()
        {
            StartTime = EndTime = DateTime.Now;
        }
        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不能为空")]
        [StringLength(25, ErrorMessage = "标题长度不能大于25")]
        public string GroupTitle { get; set; }

        /// <summary>
        /// logo
        /// </summary>
        //[Required(ErrorMessage = "logo不能为空")]
        [StringLength(100, ErrorMessage = "logo长度不能大于100")]
        public string GroupLogo { get; set; }

        /// <summary>
        /// 分区id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "分区id")]
        public int ChannelId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required(ErrorMessage = "开始时间不能为空")]
        [DisplayName("开始时间")]
        public DateTime StartTime { get; set; }

        /// 结束时间
        /// </summary>
        [Required(ErrorMessage = "结束时间不能为空")]
        [DateTimeNotLess("StartTime", "开始时间")]
        [DisplayName("结束时间")]
        public DateTime EndTime { get; set; }
        
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 组产品
        /// </summary>

        [StringLength(800, ErrorMessage = "组产品长度不能大于800")]
        public string Products { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 网址
        /// </summary>
        [StringLength(100, ErrorMessage = "网址长度不能大于100")]
        [Url]
        public string Link { get; set; }
        
        /// <summary>
        /// 扩展字段1
        /// </summary>
        [StringLength(250, ErrorMessage = "内容长度不能大于250")]
        public string ExtField1 { get; set; }

        /// <summary>
        /// 扩展字段2
        /// </summary>
        [StringLength(250, ErrorMessage = "内容长度不能大于250")]
        public string ExtField2 { get; set; }

        /// <summary>
        /// 扩展字段3
        /// </summary>
        [StringLength(250, ErrorMessage = "内容长度不能大于250")]
        public string ExtField3 { get; set; }

        /// <summary>
        /// 扩展字段4
        /// </summary>
        [StringLength(250, ErrorMessage = "内容长度不能大于250")]
        public string ExtField4 { get; set; }

        /// <summary>
        /// 扩展字段5
        /// </summary>
        [StringLength(250, ErrorMessage = "内容长度不能大于250")]
        public string ExtField5 { get; set; }

    }

    public class GroupProductListModel {

        /// <summary>
        /// 
        /// </summary>
        public List<PartProductInfo> ProductList { get; set; }
        public PageModel PageModel { get; set; }
        public int GroupId { get; set; }
        public int ChannelId { get; set; }
        
    }

    /// <summary>
    /// 频道列表模型类
    /// </summary>
    public class ChannelListModel
    {
        public PageModel PageModel { get; set; }
        public List<ChannelInfo> ChannelList { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        /// <summary>
        /// 频道名称
        /// </summary>
        public string ChannelName { get; set; }
    }

    /// <summary>
    /// 频道模型类
    /// </summary>
    public class ChannelModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(15, ErrorMessage = "名称长度不能大于15")]
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(50, ErrorMessage = "描述长度不能大于100")]
        public string Description { get; set; }
        /// <summary>
        /// 品牌排序
        /// </summary>
        [DisplayName("排序")]
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DisplayName("状态")]
        public int State { get; set; }
        /// <summary>
        /// 链接类型
        /// </summary>
        [DisplayName("链接类型")]
        public int LinkType { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        [DisplayName("链接地址")]
        [StringLength(100, ErrorMessage = "链接地址长度不能大于100")]
        public string LinkUrl { get; set; }
    }
}
