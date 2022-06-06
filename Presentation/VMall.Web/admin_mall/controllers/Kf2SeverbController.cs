using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;
using System.Web.Routing;
using System.Text;
using System.Web.Configuration;
using System.Data;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台会员报单控制器类
    /// </summary>
    public partial class Kf2SeverbController : BaseMallAdminController
    {
        private static object _locker = new object();//锁对象
        private Kftransfer2severb kf2severbBLL = new Kftransfer2severb();

        #region

        /// <summary>
        /// 获得列表搜索条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="osn">订单编号</param>
        /// <param name="uid">用户id</param>
        /// <param name="consignee">收货人</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public string GetCondition(string ApplySN, string CardUserName, string CardMobile, int State)
        {
            StringBuilder condition = new StringBuilder();
            
            if (!string.IsNullOrWhiteSpace(ApplySN))
                condition.AppendFormat(" AND T.[ApplySN] like '{0}%' ", ApplySN.Trim());
            if (!string.IsNullOrWhiteSpace(CardUserName))
                condition.AppendFormat(" AND T.[CardUserName] like '%{0}%' ", CardUserName.Trim());
            if (!string.IsNullOrWhiteSpace(CardMobile))
                condition.AppendFormat(" AND T.[CardMobile] like '{0}%' ", CardMobile.Trim());
            if (State >= 0)
                condition.AppendFormat(" AND T.[State] = {0} ", State);
            condition.Append(" AND 1=1 ");
            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="OperateUid"></param>
        /// <param name="OperUserName"></param>
        /// <param name="UserCode"></param>
        /// <param name="RealName"></param>
        /// <param name="Consignee"></param>
        /// <param name="ConsigneeMobile"></param>
        /// <param name="State"></param>
        /// <param name="Pid"></param>
        /// <param name="productName"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public ActionResult Kf2SeverbApplyList(string ApplySN = "", string CardUserName = "", string CardMobile = "", int State = -1, int pageSize = 15, int pageNumber = 1)
        {
            string condition = GetCondition(ApplySN, CardUserName, CardMobile, State);
            PageModel pageModel = new PageModel(pageSize, pageNumber, kf2severbBLL.AdminGetRecordCount(condition));
            Kf2SeverbApplyListModel model = new Kf2SeverbApplyListModel()
            {
                Kf2SeverbApplyList = kf2severbBLL.AdminGetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                ApplySN = ApplySN,
                CardUserName = CardUserName,
                CardMobile = CardMobile,
                State = State
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&ApplySN={3}&CardUserName={4}&CardMobile={5}&State={6}",
                                                          Url.Action("Kf2SeverbApplyList"),
                                                          pageModel.PageNumber, pageModel.PageSize, ApplySN, CardUserName, CardMobile, State));

            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemList.Add(new SelectListItem() { Text = "审核未通过", Value = "0" });
            itemList.Add(new SelectListItem() { Text = "等待审核", Value = "1" });
            itemList.Add(new SelectListItem() { Text = "审核成功,待处理", Value = "2" });
            itemList.Add(new SelectListItem() { Text = "处理成功", Value = "3" });
            ViewData["StateList"] = itemList;
            return View("list", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApplySN"></param>
        /// <param name="CardUserName"></param>
        /// <param name="CardMobile"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public ActionResult OutPutList(string ApplySN = "", string CardUserName = "", string CardMobile = "", int State = -1)
        {
            string condition = GetCondition(ApplySN, CardUserName, CardMobile, State);

            string sqlText = @"	 SELECT T.ApplySN 申请单号,
	                        CONVERT(DATETIME,T.CreateTime,101)  提交时间,
	                        b.username 会员编号,b.mobile 会员手机,ud.realname 会员姓名,
	                        T.Amount 金额,T.CardNumber 卡号,T.CardUserName 持卡人姓名,
	                        T.CardMobile 持卡人手机,T.Remark 备注,
	                        CASE T.State WHEN 0 THEN '审核不通过' WHEN 1 THEN '等待审核' WHEN 2 THEN '审核通过，待处理' WHEN 3 THEN '处理成功'                                 ELSE '未知' END 状态,
                            T.HandleResult 处理结果,
                            T.LastModify 处理时间 
	                        from hlh_kftransfer2severb T LEFT JOIN hlh_users b on T.uid=b.uid inner join hlh_userdetails ud on ud.uid=T.uid WHERE " + condition + "  ORDER BY T.CreateTime DESC";
            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "尚睿淳转出申请-" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 更新处理状态
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult UpdateState(int Id = -1, int state = 2, string remark = "申请处理成功")
        {
            Kftransfer2severbInfo Info = kf2severbBLL.GetModel(Id);
            if (state == 0 || state == 2)
                remark = "审核结果：" + remark;
            if (state == 3)
                remark = "。<br/>处理结果：" + remark;
            if (Info == null)
                return Content("-1");// PromptView("记录不存在");
            Info.State = state;
            Info.LastModify = DateTime.Now;
            Info.HandleResult = Info.HandleResult + remark;
            if (state == 3)
                Info.HandleTime = DateTime.Now;
            bool result = kf2severbBLL.Update(Info, "State,HandleResult,HandleTime,LastModify,");
            if (result)
                return Content("2");// PromptView("更新成功");
            else
                return Content("0");// PromptView("更新失败");
        }
        #endregion

    }
}
