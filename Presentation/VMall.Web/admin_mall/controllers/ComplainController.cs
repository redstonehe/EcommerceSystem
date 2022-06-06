using System;
using System.Web.Mvc;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;
using System.Text;
using System.Data;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台用户投诉控制器类
    /// </summary>
    public partial class ComplainController : BaseMallAdminController
    {
        Complain complainBLL = new Complain();


        /// <summary>
        /// 获得列表搜索条件
        /// </summary>
        /// <returns></returns>
        public string GetCondition(string complainMsg, int State, DateTime? startDate, DateTime? endDate, int uid = 0)
        {
            StringBuilder condition = new StringBuilder();
            if (uid > 0)
                condition.AppendFormat(" AND T.[uid] = {0} ", uid);
            if (!string.IsNullOrWhiteSpace(complainMsg))
                condition.AppendFormat(" AND T.[complainmsg] like '%{0}%' ", complainMsg.Trim());

            if (State > 0)
                condition.AppendFormat(" AND T.[state] = {0} ", State);
            if (startDate.HasValue)
                condition.AppendFormat(" AND T.[complaintime] >= '{0}' ", startDate.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            if (endDate.HasValue)
                condition.AppendFormat(" AND T.[complaintime] <= '{0}' ", endDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));

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
        public ActionResult ComplainList(string complainMsg, DateTime? StartTime, DateTime? EndTime, string sortColumn, string sortDirection, int pageNumber = 1, int pageSize = 15, int uid = 0, int State = -1)
        {
            string condition = GetCondition(complainMsg, State, StartTime, EndTime);
            string sort = AdminGetListSort(sortColumn, sortDirection);
            if (!string.IsNullOrEmpty(HttpContext.Request.Form["IsOutPut"]) && HttpContext.Request.Form["IsOutPut"].Trim() == "1")
            {
                //ViewData["IsOutPut"] = "0";
                return OutPutComplainList(condition);
            }

            PageModel pageModel = new PageModel(pageSize, pageNumber, complainBLL.AdminGetRecordCount(condition));
            ComplainListModel model = new ComplainListModel()
            {
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                ComplainList = complainBLL.AdminGetListByPage(condition, sort, (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                State = State,
                Uid = uid,
                ComplainMsg = complainMsg,
                StartTime = StartTime,
                EndTime = EndTime
            };

            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&State={5}&Uid={6}&ComplainMsg={7}&StartTime={8}&EndTime={9}",
                                                           Url.Action("ComplainList"),
                                                           pageModel.PageNumber, pageModel.PageSize,
                                                           sortColumn, sortDirection,
                                                           State, uid, complainMsg, StartTime, EndTime));
            return View(model);
        }

        /// <summary>
        /// 投诉反馈列表-导出报表
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="consignee"></param>
        /// <param name="orderState"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult OutPutComplainList(string condition)
        {
            bool noCondition = string.IsNullOrWhiteSpace(condition);

            string commandText = string.Format(@"SELECT T.id ID,u.mobile 手机,T.complainnickname 昵称,T.complaintime 投诉时间,T.complainmsg 内容,T.replytime 回复时间,T.replynickname 回复人,T.replymsg 回复内容 FROM dbo.hlh_complain T LEFT JOIN dbo.hlh_users u  ON u.uid = T.uid 
 WHERE 1=1 {0}  order by T.[id] desc", noCondition ? "" : " AND " + condition);

            DataTable dt = AdminOrders.GetOrderByCondition(commandText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "投诉反馈列表-" + DateTime.Now.ToString("yyyyMMdd")));
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.ContentType = "application/vnd.xls";
            Response.Write("<meta http-equiv=Content-Type content=\"text/html; charset=GB2312\">");
            System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
            System.Web.UI.WebControls.DataGrid dg = new System.Web.UI.WebControls.DataGrid();
            dg.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(dg_ItemDataBound);
            dg.DataSource = dt;
            dg.DataBind();
            dg.RenderControl(oHtmlTextWriter);
            Response.Write(oStringWriter.ToString());
            Response.Flush();
            Response.End();
            return Content("下载完毕");
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="consultId">商品咨询id</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public ActionResult UpdateProductConsultState(int consultId = -1, int state = -1)
        {
            bool result = AdminProductConsults.UpdateProductConsultState(consultId, state);
            if (result)
            {
                AddMallAdminLog("更新商品咨询状态", "更新商品咨询状态,咨询ID和状态为:" + consultId + "_" + state);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 回复投诉
        /// </summary>
        [HttpGet]
        public ActionResult Reply(int Id = -1)
        {
            ComplainInfo info = complainBLL.GetModel(Id);
            if (info == null)
                return PromptView("投诉不存在");

            ReplyComplainModel model = new ReplyComplainModel();
            model.ComplainInfo = info;
            model.ReplyMessage = info.replymsg;

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 回复投诉
        /// </summary>
        [HttpPost]
        public ActionResult Reply(ReplyComplainModel model, int Id = -1)
        {
            ComplainInfo info = complainBLL.GetModel(Id);
            if (info == null)
                return PromptView("投诉不存在");

            if (ModelState.IsValid)
            {
                info.state = 1;
                info.replytime = DateTime.Now;
                info.replyip = WebHelper.GetIP();
                info.replynickname = "系统运营人员";
                info.replymsg = model.ReplyMessage;
                complainBLL.Update(info, "state,replytime,replyip,replynickname,replymsg");
                AddMallAdminLog("回复会员投诉", "回复会员投诉,会员投诉内容为:" + info.complainmsg);
                return PromptView("会员投诉回复成功");
            }

            model.ComplainInfo = info;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

    }
}
