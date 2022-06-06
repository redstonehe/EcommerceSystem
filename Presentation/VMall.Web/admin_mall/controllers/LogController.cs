using System;
using System.Web;
using System.Data;
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
    /// 商城后台日志控制器类
    /// </summary>
    public partial class LogController : BaseMallAdminController
    {
        private Logs logsBLL = new Logs();
        /// <summary>
        /// 商城管理日志列表
        /// </summary>
        /// <param name="accountName">操作人</param>
        /// <param name="operation">操作动作</param>
        /// <param name="startTime">操作开始时间</param>
        /// <param name="endTime">操作结束时间</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult MallAdminLogList(string accountName, string operation, string startTime, string endTime, int pageNumber = 1, int pageSize = 15)
        {
            int uid = AdminUsers.GetUidByAccountName(accountName);

            string condition = MallAdminLogs.GetMallAdminLogListCondition(uid, operation, startTime, endTime);

            PageModel pageModel = new PageModel(pageSize, pageNumber, MallAdminLogs.GetMallAdminLogCount(condition));

            MallAdminLogListModel model = new MallAdminLogListModel()
            {
                MallAdminLogList = MallAdminLogs.GetMallAdminLogList(pageModel.PageSize, pageModel.PageNumber, condition),
                PageModel = pageModel,
                AccountName = accountName,
                Operation = operation,
                StartTime = startTime,
                EndTime = endTime
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&accountName={3}&operation={4}&startTime={5}&endTime={6}",
                                                          Url.Action("malladminloglist"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          accountName, operation, startTime, endTime));
            return View(model);
        }

        /// <summary>
        /// 删除商城管理日志
        /// </summary>
        public ActionResult DelMallAdminLog(int[] logIdList)
        {
            MallAdminLogs.DeleteMallAdminLogById(logIdList);
            AddMallAdminLog("删除商城管理日志", "删除商城管理日志,日志ID为:" + CommonHelper.IntArrayToString(logIdList));
            return PromptView("商城管理日志删除成功");
        }

        /// <summary>
        /// 店铺管理日志列表
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="operation">操作动作</param>
        /// <param name="startTime">操作开始时间</param>
        /// <param name="endTime">操作结束时间</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult StoreAdminLogList(string storeName, string operation, string startTime, string endTime, int storeId = -1, int pageNumber = 1, int pageSize = 15)
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            string condition = StoreAdminLogs.GetStoreAdminLogListCondition(storeId, operation, startTime, endTime);
            PageModel pageModel = new PageModel(pageSize, pageNumber, StoreAdminLogs.GetStoreAdminLogCount(condition));
            StoreAdminLogListModel model = new StoreAdminLogListModel()
            {
                StoreAdminLogList = StoreAdminLogs.GetStoreAdminLogList(pageModel.PageSize, pageModel.PageNumber, condition),
                PageModel = pageModel,
                StoreId = storeId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "全部店铺" : storeName,
                Operation = operation,
                StartTime = startTime,
                EndTime = endTime
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&storeId={3}&storeName={4}&operation={5}&startTime={6}&endTime={7}",
                                                          Url.Action("storeadminloglist"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          storeId, storeName, operation, startTime, endTime));
            return View(model);
        }

        /// <summary>
        /// 删除店铺管理日志
        /// </summary>
        public ActionResult DelStoreAdminLog(int[] logIdList)
        {
            StoreAdminLogs.DeleteStoreAdminLogById(logIdList);
            AddMallAdminLog("删除店铺管理日志", "删除店铺管理日志,日志ID为:" + CommonHelper.IntArrayToString(logIdList));
            return PromptView("店铺管理日志删除成功");
        }

        /// <summary>
        /// 积分日志列表
        /// </summary>
        /// <param name="accountName">账户名</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult CreditLogList(string accountName, string startTime, string endTime, int pageNumber = 1, int pageSize = 15)
        {
            int uid = AdminUsers.GetUidByAccountName(accountName);

            string condition = AdminCredits.AdminGetCreditLogListCondition(uid, startTime, endTime);

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminCredits.AdminGetCreditLogCount(condition));

            CreditLogListModel model = new CreditLogListModel()
            {
                CreditLogList = AdminCredits.AdminGetCreditLogList(pageModel.PageSize, pageModel.PageNumber, condition),
                PageModel = pageModel,
                AccountName = accountName,
                StartTime = startTime,
                EndTime = endTime
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&accountName={3}&startTime={4}&endTime={5}",
                                                          Url.Action("creditloglist"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          accountName, startTime, endTime));
            return View(model);
        }

        /// <summary>
        /// 发放积分
        /// </summary>
        /// <param name="rUserName">接收用户名</param>
        /// <param name="payCredits">支付积分</param>
        /// <param name="rankCredits">等级积分</param>
        /// <returns></returns>
        public ActionResult SendCredits(string rUserName, int payCredits = 0, int rankCredits = 0)
        {
            PartUserInfo partUserInfo = AdminUsers.GetPartUserByName(rUserName);
            if (partUserInfo == null)
                return PromptView("请输入正确的用户名");

            AdminCredits.AdminSendCredits(partUserInfo, payCredits, rankCredits, WorkContext.Uid, DateTime.Now);
            AddMallAdminLog("发放积分", "支付积分为:" + payCredits + ",等级积分为:" + rankCredits);
            return PromptView("积分发放成功");
        }





        /// <summary>
        /// 获得列表搜索条件
        /// </summary>
        /// <returns></returns>
        public string GetCondition(string title, DateTime? startDate, DateTime? endDate)
        {
            StringBuilder condition = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(title))
                condition.AppendFormat(" AND [Title] like '%{0}%' ", title.Trim());
            if (startDate.HasValue)
                condition.AppendFormat(" AND [LogDate] >= '{0}' ", startDate.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            if (endDate.HasValue)
                condition.AppendFormat(" AND [LogDate] <= '{0}' ", endDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));

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
                sortColumn = "[Id]";
            if (string.IsNullOrWhiteSpace(sortDirection))
                sortDirection = "DESC";

            return string.Format("{0} {1} ", sortColumn, sortDirection);
        }


        /// <summary>
        /// 商城错误日志列表
        /// </summary>
        /// <param name="accountName">操作人</param>
        /// <param name="operation">操作动作</param>
        /// <param name="startTime">操作开始时间</param>
        /// <param name="endTime">操作结束时间</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult ErrorLogList(string title, DateTime? StartTime, DateTime? EndTime, int pageNumber = 1, int pageSize = 15)
        {
            string condition = GetCondition(title, StartTime, EndTime);

            PageModel pageModel = new PageModel(pageSize, pageNumber, logsBLL.GetCount(condition));

            ErrorLogListModel model = new ErrorLogListModel()
            {
                ErrorLogList = logsBLL.GetList(condition, pageModel.PageSize, pageModel.PageNumber,"", AdminGetListSort("","")),
                PageModel = pageModel,
                Title = title,
                StartTime = StartTime,
                EndTime = EndTime
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&Title={3}&startTime={4}&endTime={5}",
                                                          Url.Action("ErrorLogList"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          title, StartTime, EndTime));
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ErrorDetail(int id)
        {
            LogsInfo logs = logsBLL.GetModel(id);
            return View(logs);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult ErrorDetailJson(int id)
        {
            LogsInfo logs = logsBLL.GetModel(id);
           
           // return AjaxResult("nopermit", "您没有当前操作的权限");
            return base.Json(logs, JsonRequestBehavior.AllowGet);
        }
    }
}
