using System;
using System.Data;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VMall.Web.MallAdmin.Models
{
    /// <summary>
    /// 商品评价列表模型类
    /// </summary>
    public class ProductReviewListModel
    {
        public PageModel PageModel { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public DataTable ProductReviewList { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int Pid { get; set; }
        public string PName { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    /// <summary>
    /// 商品评价回复列表模型类
    /// </summary>
    public class ProductReviewReplyListModel
    {
        public DataTable ProductReviewReplyList { get; set; }
    }

    /// <summary>
    /// 增加商品评价模型类
    /// </summary>
    public class AddProductReviewModel
    {
        public AddProductReviewModel()
        {
            Star = 5;
            DefaultTime=BuyTime = ReviewTime = new DateTime(2016, 4, 18,19,0,0);
        }
        /// <summary>
        /// 商品编号
        /// </summary>
        [Required(ErrorMessage = "商品编号不能为空")]
        [Range(1, int.MaxValue, ErrorMessage = "请选择商品")]
        public int Pid { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        [Range(1, 5, ErrorMessage = "请选择正确的评分")]
        [Required(ErrorMessage = "评分不能为空")]
        [DisplayName("商品评分")]
        public int Star{ get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        [Required(ErrorMessage = "评价内容不能为空")]
        [StringLength(200, ErrorMessage = " 评价内容不能大于200字")]
        public string Message { get; set; }

        /// <summary>
        /// 默认时间
        /// </summary>
        [DisplayName("默认时间")]
        public DateTime DefaultTime { get; set; }

        /// <summary>
        /// 购买时间
        /// </summary>
        [Required(ErrorMessage = "购买时间不能为空")]
        [DisplayName("购买时间")]
        [DateTimeNotLess("DefaultTime", "默认时间")]
        public DateTime BuyTime { get; set; }

        /// <summary>
        /// 评价时间
        /// </summary>
        [Required(ErrorMessage = "评价时间不能为空")]
        [DisplayName("评价时间")]
        [DateTimeNotLess("BuyTime", "开始时间")]
        public DateTime ReviewTime { get; set; }

        /// <summary>
        /// 显示会员姓名
        /// </summary>
        [Required(ErrorMessage = "会员姓名不能为空")]
        [StringLength(20, ErrorMessage = "会员姓名不能超过20")]
        [DisplayName("会员姓名")]
        public string ShowNickName { get; set; }

    }
}
