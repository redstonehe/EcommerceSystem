using System;
using System.Data;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.MallAdmin.Models
{
    /// <summary>
    /// 新闻类型模型类
    /// </summary>
    public class InformTypeModel
    {
        public InformTypeModel()
        {
            InformTypeName = "";
            DisplayOrder = 0;
        }
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(30, ErrorMessage = "名称长度不能大于30")]
        public string InformTypeName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// 新闻列表模型类
    /// </summary>
    public class InformListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 新闻列表
        /// </summary>
        public DataTable InformList { get; set; }
        /// <summary>
        /// 排序列
        /// </summary>
        public string SortColumn { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public string SortDirection { get; set; }
        /// <summary>
        /// 新闻类型id
        /// </summary>
        public int InformTypeId { get; set; }
        /// <summary>
        /// 新闻标题
        /// </summary>
        public string InformTitle { get; set; }
    }

    /// <summary>
    /// 新闻模型类
    /// </summary>
    public class InformModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不能为空")]
        [StringLength(50, ErrorMessage = "标题长度不能大于50")]
        public string Title { get; set; }
        /// <summary>
        /// 新闻类型id
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "请选择新闻类型")]
        [DisplayName("新闻类型")]
        public int InformTypeId { get; set; }
        /// <summary>
        /// uid
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "请选择会员uid")]
        public int Uid { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [AllowHtml]
        public string Body { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public int IsShow { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public int IsTop { get; set; }
        
        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 是否批量发送 1批量 0 不批量
        /// </summary>
        public int IsBatchSend { get; set; }

    }
}
