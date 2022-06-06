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
    /// 商城后台订单控制器类
    /// </summary>
    public partial class AgentSendOrderController : BaseMallAdminController
    {
        private static object _locker = new object();//锁对象
        private AgentSendOrder AgentSendOrderBLL = new AgentSendOrder();

        /// <summary>
        /// 获得订单列表搜索条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="osn">订单编号</param>
        /// <param name="uid">用户id</param>
        /// <param name="consignee">收货人</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public string GetCondition(int storeId, string osn, int uid, string consignee, string consigneeMobile, int orderState, DateTime? startDate, DateTime? endDate, DateTime? payStartDate, DateTime? payEndDate, int pid, string productName, int isKFXH = 0)
        {
            StringBuilder condition = new StringBuilder();

            //if (storeId > 0)
            //    condition.AppendFormat(" AND [storeid] = {0} ", storeId);
            if (!string.IsNullOrWhiteSpace(osn))
                condition.AppendFormat(" AND [sendosn] like '{0}%' ", osn.Trim());
            if (uid > 0)
                condition.AppendFormat(" AND T.[uid] = {0} ", uid);
            if (!string.IsNullOrWhiteSpace(consignee))
                condition.AppendFormat(" AND [consignee] like '{0}%' ", consignee.Trim());
            if (!string.IsNullOrWhiteSpace(consigneeMobile))
                condition.AppendFormat(" AND [mobile] = '{0}' ", consigneeMobile.Trim());
            if (orderState >= 0)
                condition.AppendFormat(" AND [sendstate] = {0} ", orderState);
            if (startDate.HasValue)
                condition.AppendFormat(" AND [creationdate] >= '{0}' ", startDate.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            if (endDate.HasValue)
                condition.AppendFormat(" AND [creationdate] <= '{0}' ", endDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));
            if (payStartDate.HasValue)
                condition.AppendFormat(" AND [paytime] >= '{0}' ", payStartDate.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            if (payEndDate.HasValue)
                condition.AppendFormat(" AND [paytime] <= '{0}' ", payEndDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));
            if (pid > 0)
                condition.AppendFormat(" AND T.[pid] ={0}  ", pid);
            if (!string.IsNullOrEmpty(productName))
                condition.AppendFormat(" AND T.[pid] IN (SELECT pid FROM hlh_products WHERE name LIKE '%{0}%' ) ", productName.Trim());
            if (isKFXH == 0)
                condition.AppendFormat(" AND [cashdiscount]=0 ");
            if (isKFXH == 1)
                condition.AppendFormat(" AND [cashdiscount]>0 ");
            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }
        /// <summary>
        /// 订单列表
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="osn">订单编号</param>
        /// <param name="accountName">账户名</param>
        /// <param name="consignee">收货人</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        ///  <param name="isKFXH">是否咖啡选货</param>
        /// <returns></returns>
        public ActionResult SendOrderList(string storeName, string osn, DateTime? startDate, DateTime? endDate, DateTime? payStartDate, DateTime? payEndDate, string accountName, string consignee, string consigneeMobile, string sortColumn, string sortDirection, int storeId = -1, int orderState = -1, int pageSize = 15, int pageNumber = 1, int pid = 0, string productName = "", int isKFXH = -1)
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            //获取用户id
            int uid = Users.GetUidByAccountName(accountName);
            if (!string.IsNullOrEmpty(HttpContext.Request.Form["IsOutPut"]) && HttpContext.Request.Form["IsOutPut"].Trim() == "1")
            {
                //ViewData["IsOutPut"] = "0";
                return OutPutOrder(storeId, osn, consignee, consigneeMobile, orderState, startDate, endDate, payStartDate, payEndDate, pid, productName, isKFXH, accountName);
            }

            string condition = GetCondition(storeId, osn, uid, consignee, consigneeMobile, orderState, startDate, endDate, payStartDate, payEndDate, pid, productName, isKFXH);
            string sort = AdminOrders.GetOrderListSort(sortColumn, sortDirection);

            PageModel pageModel = new PageModel(pageSize, pageNumber, new AgentSendOrder().GetRecordCount(condition));

            SendOrderListModel model = new SendOrderListModel()
            {
                SendOrderList = new AgentSendOrder().AdminGetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                StoreId = storeId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "全部店铺" : storeName,
                OSN = osn,
                AccountName = accountName,
                Consignee = consignee,
                ConsigneeMobile = consigneeMobile,
                OrderState = orderState,
                StartDate = startDate,
                EndDate = endDate,
                PayStartDate = payStartDate,
                PayEndDate = payEndDate,
                Pid = pid,
                ProductName = productName,
                IsKFXH = isKFXH
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&storeId={5}&storeName={6}&OSN={7}&AccountName={8}&Consignee={9}&OrderState={10}&consigneeMobile={11}&pid={12}&productName={13}&isKFXH={14}",
                                                          Url.Action("sendorderlist"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          sortColumn, sortDirection,
                                                          storeId, storeName,
                                                          osn, accountName, consignee, orderState, consigneeMobile, pid, productName, isKFXH));

            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemList.Add(new SelectListItem() { Text = "未发货", Value = "0" });
            itemList.Add(new SelectListItem() { Text = "已发货", Value = "1" });

            ViewData["sendStateList"] = itemList;


            List<SelectListItem> itemListKFXF = new List<SelectListItem>();
            itemListKFXF.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemListKFXF.Add(new SelectListItem() { Text = "是", Value = "1" });
            itemListKFXF.Add(new SelectListItem() { Text = "否", Value = "0" });
            ViewData["KFXFList"] = itemListKFXF;
            return View(model);
        }

        #region 订单导出

        /// <summary>
        /// 导出订单
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="consignee"></param>
        /// <param name="orderState"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult OutPutOrder(int storeId, string osn, string consignee, string consigneeMobile, int orderState, DateTime? startDate, DateTime? endDate, DateTime? payStartDate, DateTime? payEndDate, int pid, string productName, int isKFXH, string accountName)
        {
            int uid = Users.GetUidByAccountName(accountName);
            string condition = GetCondition(storeId, osn, uid, consignee, consigneeMobile, orderState, startDate, endDate, payStartDate, payEndDate, pid, productName, isKFXH);
            string sqlText = string.Empty;

            sqlText = @"SELECT RTRIM(T.sendosn) 要货单号,RTRIM(T.consignee) 收货人,
RTRIM(r.provincename)+RTRIM(r.cityname)+RTRIM(r.name)+RTRIM(T.[address]) 收货地址,
RTRIM(T.mobile) 收货人电话,
CASE  WHEN c.mobile<>'' THEN RTRIM(c.mobile) WHEN c.username<>'' THEN RTRIM(c.username) WHEN c.email<>'' THEN RTRIM(c.email) ELSE '' END  会员编号,
CASE c.agenttype WHEN 4 THEN '大区经销商' WHEN 3 THEN 'VIP经销商' WHEN 2 THEN '星级经销商' WHEN 1 THEN '失业伙伴' ELSE '普通会员' END 会员类型 ,
RTRIM(ud.realname) 会员名称,
b.psn 产品编码,
b.name 产品名称,
 T.sendcount 数量,
CASE T.sendstate WHEN 0 THEN  '未发货' ELSE '已发货' END 状态,
T.buyerremark 备注,
CONVERT(varchar(100),T.creationdate,120) 申请时间,
RTRIM(T.shipsn) 快递单号,
T.shipconame 快递公司
  from hlh_agentsendorder T left join hlh_products b on T.pid=b.pid left join hlh_users c on T.uid=c.uid INNER JOIN hlh_userdetails ud ON T.[uid]=ud.[uid]  INNER JOIN hlh_regions r ON T.regionid=r.regionid " + (string.IsNullOrEmpty(condition) ? "" : "where " + condition) + " ORDER by T.[creationdate] asc";

            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "代理要货单-" + DateTime.Now.ToString("yyyyMMdd")));
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

        private void dg_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Header && e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Footer)
            {
                foreach (System.Web.UI.WebControls.TableCell item in e.Item.Cells)
                    item.Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            }
        }

        #endregion

        #region 修改订单信息
        /// <summary>
        /// 更新快递信息
        /// </summary>
        [HttpGet]
        public ActionResult UpdateShipInfo(int id = -1)
        {
            AgentSendOrderInfo orderInfo = AgentSendOrderBLL.GetModel(id);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.SendState != 1)
                return PromptView(Url.Action("SendOrderList"), "未发货不能修改快递信息");

            UpdateShipInfoModel model = new UpdateShipInfoModel();
            model.OldShipSN = orderInfo.ShipSN;
            model.OldShipCoName = orderInfo.ShipCoName;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 更新快递信息
        /// </summary>
        [HttpPost]
        public ActionResult UpdateShipInfo(UpdateShipInfoModel model, int id = -1)
        {
            AgentSendOrderInfo orderInfo = AgentSendOrderBLL.GetModel(id);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.SendState != 1)
                return PromptView(Url.Action("SendOrderList"), "未发货不能修改快递信息");

            ShipCompanyInfo shipCompanyInfo = ShipCompanies.GetShipCompanyById(model.ShipCoId);
            if (shipCompanyInfo == null)
                ModelState.AddModelError("ShipCoId", "请选择配送公司");

            if (ModelState.IsValid)
            {
                orderInfo.ShipSN = model.ShipSN;
                orderInfo.ShipCoid = model.ShipCoId;
                orderInfo.ShipCoName = model.ShipCoName;
                AgentSendOrderBLL.Update(orderInfo);
                AddMallAdminLog("修改要货单快递信息", "修改要货单快递单信息,要货单号为:" + orderInfo.SendOSN + ",原快递信息：" + model.OldShipCoName + "-" + orderInfo.ShipSN + ",新快递信息:" + shipCompanyInfo.Name + "-" + model.ShipSN);
                return PromptView(Url.Action("SendOrderList"), "修改快递信息成功");
            }
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }
        /// <summary>
        /// 更新快递单号
        /// </summary>
        [HttpGet]
        public ActionResult UpdateOrderShipSN(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Sended)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "不能修改快递单号");

            UpdateOrderShipSNModel model = new UpdateOrderShipSNModel();
            model.ShipSN = orderInfo.ShipSN;
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 更新快递单号
        /// </summary>
        [HttpPost]
        public ActionResult UpdateOrderShipSN(UpdateOrderShipSNModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Sended)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "不能修改快递单号");

            if (ModelState.IsValid)
            {
                Orders.UpdateOrderShipSN(orderInfo.Oid, model.ShipSN);

                AddMallAdminLog("更改快递单号", "更改快递单号,订单ID为:" + oid + ",原快递单号：" + orderInfo.ShipSN + ",新快递单号" + model.ShipSN);

                return PromptView(Url.Action("orderinfo", new { oid = oid }), "更改快递单号成功");
            }
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }




        #endregion

        #region 发货


        /// <summary>
        /// 发货
        /// </summary>
        [HttpGet]
        public ActionResult SendProduct(int id = -1)
        {
            AgentSendOrderInfo orderInfo = new AgentSendOrder().GetModel(id);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.SendState == 1)
                return PromptView("已发货，不能重复发货");

            SendOrderProductModel model = new SendOrderProductModel();

            return View(model);
        }

        /// <summary>
        /// 发货
        /// </summary>
        [HttpPost]
        public ActionResult SendProduct(SendOrderProductModel model, int id = -1)
        {
            AgentSendOrderInfo orderInfo = new AgentSendOrder().GetModel(id);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.SendState == 1)
                return PromptView("已发货，不能重复发货");

            ShipCompanyInfo shipCompanyInfo = ShipCompanies.GetShipCompanyById(model.ShipCoId);
            if (shipCompanyInfo == null)
                ModelState.AddModelError("ShipCoId", "请选择配送公司");

            if (ModelState.IsValid)
            {
                orderInfo.SendState = 1;
                orderInfo.ShipSN = model.ShipSN;
                orderInfo.ShipCoid = model.ShipCoId;
                orderInfo.ShipCoName = shipCompanyInfo.Name;
                orderInfo.ShipTime = DateTime.Now;
                new AgentSendOrder().Update(orderInfo);
                AddMallAdminLog("要货单发货", "发货,ID为:" + id);
                return PromptView(Url.Action("sendorderlist"), "发货成功");
            }
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }


        #endregion

        /// <summary>
        /// 撤销申请（需确认仓库没发货）
        /// </summary>
        public ActionResult CancelAgentSend(int id = -1)
        {
            AgentSendOrderInfo sendInfo = new AgentSendOrder().GetModel(id);
            if (sendInfo == null)
                return PromptView("订单不存在");
            if (sendInfo.SendState == 1)
                return PromptView("已发货，不能撤销申请");
            PartProductInfo productInfo = Products.GetPartProductById(sendInfo.Pid);
            if (productInfo == null)
                return PromptView("产品不存在或产品已下架");
            AgentStockInfo info = new AgentStock().GetModel(string.Format(" uid={0} and pid={1} ", sendInfo.Uid, sendInfo.Pid));
            if (info == null)
            {
                List<AgentStockInfo> infoList = new AgentStock().GetList(string.Format(" uid={0}  ", sendInfo.Uid));
                if (!infoList.Any())
                {
                    return PromptView("库存不存在");
                }
                info = infoList.FirstOrDefault();
            }
            PartUserInfo userInfo = Users.GetPartUserById(sendInfo.Uid);

            decimal singlePrice = new AgentStock().SingleAgentPrice(userInfo, info.Pid);//退回库存产品价格
            decimal itemPrice = new AgentStock().SingleAgentPriceFor70(userInfo, sendInfo.Pid);//退回发货产品价格
           
            decimal changeAmount = itemPrice * sendInfo.SendCount;
            decimal stockRemain = AgentStock.CutDecimalWithN((info.AgentAmount + changeAmount) / singlePrice, 4);

            info.Balance = stockRemain;
            info.AgentAmount = info.AgentAmount + changeAmount;

            new AgentStock().Update(info);
            new AgentSendOrder().Delete(sendInfo.Id);

            new AgentStockDetail().AddDetail(info.Uid, info.Pid, 4,changeAmount, 0,  info.AgentAmount, "", string.Format("要货单撤销，产品{0},数量{1},金额：{2}", productInfo.Name, sendInfo.SendCount, changeAmount), info.Uid, 0);
            
            AddMallAdminLog("撤销要货单", "撤销要货单,ID为:" + id);
            return PromptView(Url.Action("sendorderlist"), "撤销要货单成功");

        }



        private void CreateOrderAction(int oid, OrderActionType orderActionType, string actionDes)
        {

        }
    }
}
