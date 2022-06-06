using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.MallAdmin.Models
{
    /// <summary>
    /// banner模型类
    /// </summary>
    public class BannerModel
    {
        public BannerModel()
        {
            StartTime = EndTime = DateTime.Now;
            ExtField1 = "";
            ExtField2 = "";
            ExtField3 = "";
        }

        /// <summary>
        /// 类型
        /// </summary>
        public int BannerType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required(ErrorMessage = "开始时间不能为空")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Required(ErrorMessage = "结束时间不能为空")]
        [DateTimeNotLess("StartTime", "开始时间")]
        [DisplayName("结束时间")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public int IsShow { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(50, ErrorMessage = "标题长度不能大于50")]
        public string BannerTitle { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Required(ErrorMessage = "地址不能为空")]
        [StringLength(125, ErrorMessage = "地址长度不能大于125")]
        [Url]
        public string Url { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        [Required(ErrorMessage = "图片不能为空")]
        [StringLength(125, ErrorMessage = "图片长度不能大于125")]
        public string Img { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Required(ErrorMessage = "排序不能为空")]
        [DisplayName("排序")]
        public int DisplayOrder { get; set; }
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
    }

    /// <summary>
    /// banner列表模型类
    /// </summary>
    public class BannerListModel
    {
        public string Title { get; set; }
        public int Type { get; set; }
        public PageModel PageModel { get; set; }
        public List<BannerInfo> BannerList { get; set; }
    }
}
