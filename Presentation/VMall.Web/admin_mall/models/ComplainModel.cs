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
    /// 商品咨询列表模型类
    /// </summary>
    public class ComplainListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 排序列
        /// </summary>
        public string SortColumn { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public string SortDirection { get; set; }
        /// <summary>
        /// 商品咨询列表
        /// </summary>
        public DataTable ComplainList { get; set; }
        /// <summary>
        /// uid
        /// </summary>
        public int Uid { get; set; }
        /// <summary>
        /// State
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 咨询信息
        /// </summary>
        public string ComplainMsg { get; set; }
        /// <summary>
        /// 咨询开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 咨询结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }

    /// <summary>
    /// 回复商品咨询模型类
    /// </summary>
    [Bind(Exclude = "ComplainInfo")]
    public class ReplyComplainModel
    {
        public ComplainInfo ComplainInfo { get; set; }

        [Required(ErrorMessage = "回复内容不能为空")]
        [StringLength(100, ErrorMessage = "最多只能输入100个字")]
        public string ReplyMessage { get; set; }
    }
}
