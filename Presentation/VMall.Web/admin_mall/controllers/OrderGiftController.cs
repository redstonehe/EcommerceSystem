using System;
using System.Web.Mvc;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;
using System.Text;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台用户投诉控制器类
    /// </summary>
    public partial class OrderGiftController : BaseMallAdminController
    {
        OrderGift giftBLL = new OrderGift();

        /// <summary>
        /// 获得列表搜索条件
        /// </summary>
        /// <returns></returns>
        public string GetCondition( int State, DateTime? startDate, DateTime? endDate, int uid = 0)
        {
            StringBuilder condition = new StringBuilder();
            if (uid > 0)
                condition.AppendFormat(" AND T.[uid] = {0} ", uid);
            
            if (startDate.HasValue)
                condition.AppendFormat(" AND T.[StartTime] >= '{0}' ", startDate.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            if (endDate.HasValue)
                condition.AppendFormat(" AND T.[EndTime] <= '{0}' ", endDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));

            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }
        /// <summary>
        /// 后台获得商品咨询列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public string AdminGetListSort(string sortColumn, string sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
                sortColumn = "[id]";
            if (string.IsNullOrWhiteSpace(sortDirection))
                sortDirection = "DESC";

            return string.Format("{0} {1} ", sortColumn, sortDirection);
        }
        /// <summary>
        /// 列表
        /// </summary>
        public ActionResult OrderGiftList( DateTime? StartTime, DateTime? EndTime, string sortColumn, string sortDirection, int pageNumber = 1, int pageSize = 15, int uid = 0,int State=-1)
        {
            string condition = GetCondition( State, StartTime, EndTime);
            string sort = AdminGetListSort(sortColumn, sortDirection);

            PageModel pageModel = new PageModel(pageSize, pageNumber, giftBLL.AdminGetRecordCount(condition));
            OrderGiftListModel model = new OrderGiftListModel()
            {
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                OrderGiftList = giftBLL.AdminGetListByPage(condition, sort, (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                State = State,
                Uid = uid,
                StartTime = StartTime,
                EndTime = EndTime
            };

            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&State={5}&Uid={6}&StartTime={7}&EndTime={8}",
                                                           Url.Action("OrderGiftList"),
                                                           pageModel.PageNumber, pageModel.PageSize,
                                                           sortColumn, sortDirection,
                                                           State, uid, StartTime, EndTime));
            return View(model);
        }
        
    }
}
