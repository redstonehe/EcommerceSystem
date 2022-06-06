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
using System.Linq.Expressions;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台订单控制器类
    /// </summary>
    public partial class OrderController : BaseMallAdminController
    {
        private static IOrderStrategy _iorderstrategy = BMAOrder.Instance;//订单策略
        private static object _locker = new object();//锁对象
        private AgentStock agentBll = new AgentStock();

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
        public string GetCondition(int storeId, string osn, int uid, string consignee, string consigneeMobile, string orderState, DateTime? startDate, DateTime? endDate, DateTime? payStartDate, DateTime? payEndDate, int pid, string productName, int isKFXH = 0, string payName = "")
        {
            StringBuilder condition = new StringBuilder();

            if (storeId > 0)
                condition.AppendFormat(" AND [storeid] = {0} ", storeId);
            if (!string.IsNullOrWhiteSpace(osn))
                condition.AppendFormat(" AND [osn] like '{0}%' ", osn.Trim());
            if (uid > 0)
                condition.AppendFormat(" AND [uid] = {0} ", uid);
            if (!string.IsNullOrWhiteSpace(consignee))
                condition.AppendFormat(" AND [consignee] like '{0}%' ", consignee.Trim());
            if (!string.IsNullOrWhiteSpace(consigneeMobile))
                condition.AppendFormat(" AND [mobile] = '{0}' ", consigneeMobile.Trim());
            if (!orderState.Trim().StartsWith("0"))
                condition.AppendFormat(" AND [orderstate] IN ({0}) ", orderState);
            if (startDate.HasValue)
                condition.AppendFormat(" AND [addtime] >= '{0}' ", startDate.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            if (endDate.HasValue)
                condition.AppendFormat(" AND [addtime] <= '{0}' ", endDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));
            if (payStartDate.HasValue)
                condition.AppendFormat(" AND [paytime] >= '{0}' ", payStartDate.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            if (payEndDate.HasValue)
                condition.AppendFormat(" AND [paytime] <= '{0}' ", payEndDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));
            if (pid > 0)
                condition.AppendFormat(" AND [oid] IN (SELECT oid FROM hlh_orderproducts WHERE pid ={0} ) ", pid);
            if (!string.IsNullOrEmpty(productName))
                condition.AppendFormat(" AND [oid] IN (SELECT oid FROM hlh_orderproducts WHERE name LIKE '%{0}%' ) ", productName.Trim());
            if (isKFXH == 0)
                condition.AppendFormat(" AND [cashdiscount]=0 ");
            if (isKFXH == 1)
                condition.AppendFormat(" AND [cashdiscount]>0 ");
            if (!string.IsNullOrEmpty(payName))
                condition.AppendFormat(" AND [paysystemname] ='{0}' ", payName);
            condition.Append(" AND [storeid] in ( select storeid from hlh_stores where mallsource in(0,2,3) ) ");
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
        public ActionResult OrderList(string storeName, string osn, DateTime? startDate, DateTime? endDate, DateTime? payStartDate, DateTime? payEndDate, string accountName, string consignee, string consigneeMobile, string sortColumn, string sortDirection, int storeId = -1, string orderState = "0", int pageSize = 15, int pageNumber = 1, int pid = 0, string productName = "", int isKFXH = -1, string payName = "")
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            //获取用户id
            int uid = Users.GetUidByAccountName(accountName);
            if (!string.IsNullOrEmpty(HttpContext.Request.Form["IsOutPut"]) && HttpContext.Request.Form["IsOutPut"].Trim() == "1")
            {
                //ViewData["IsOutPut"] = "0";
                return OutPutOrder(storeId, uid, osn, consignee, consigneeMobile, orderState, startDate, endDate, payStartDate, payEndDate, pid, productName, isKFXH, payName);
            }


            string condition = GetCondition(storeId, osn, uid, consignee, consigneeMobile, orderState, startDate, endDate, payStartDate, payEndDate, pid, productName, isKFXH, payName);
            string sort = AdminOrders.GetOrderListSort(sortColumn, sortDirection);

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminOrders.GetOrderCount(condition));

            OrderListModel model = new OrderListModel()
            {
                OrderList = AdminOrders.GetOrderList(pageModel.PageSize, pageModel.PageNumber, condition, sort),
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
                IsKFXH = isKFXH,
                PayName = payName
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&storeId={5}&storeName={6}&OSN={7}&AccountName={8}&Consignee={9}&OrderState={10}&consigneeMobile={11}&pid={12}&productName={13}&isKFXH={14}&payName={15}",
                                                          Url.Action("orderlist"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          sortColumn, sortDirection,
                                                          storeId, storeName,
                                                          osn, accountName, consignee, orderState, consigneeMobile, pid, productName, isKFXH, payName));

            List<SelectListItem> itemList = new List<SelectListItem>();
            //itemList.Add(new SelectListItem() { Text = "全部", Value = "0" });
            //itemList.Add(new SelectListItem() { Text = "已提交", Value = ((int)OrderState.Submitted).ToString() });
            itemList.Add(new SelectListItem() { Text = "等待付款", Value = ((int)OrderState.WaitPaying).ToString() });
            //itemList.Add(new SelectListItem() { Text = "待确认", Value = ((int)OrderState.Confirming).ToString() });
            itemList.Add(new SelectListItem() { Text = "已确认", Value = ((int)OrderState.Confirmed).ToString() });
            itemList.Add(new SelectListItem() { Text = "备货中", Value = ((int)OrderState.PreProducting).ToString() });
            itemList.Add(new SelectListItem() { Text = "已发货", Value = ((int)OrderState.Sended).ToString() });
            itemList.Add(new SelectListItem() { Text = "已完成", Value = ((int)OrderState.Completed).ToString() });
            //itemList.Add(new SelectListItem() { Text = "已锁定", Value = ((int)OrderState.Locked).ToString() });
            itemList.Add(new SelectListItem() { Text = "已取消", Value = ((int)OrderState.Cancelled).ToString() });
            itemList.Add(new SelectListItem() { Text = "已退货", Value = ((int)OrderState.Returned).ToString() });

            ViewData["orderStateList"] = itemList;
            //ViewData["IsOutPut"] = "0";

            List<SelectListItem> itemListKFXF = new List<SelectListItem>();
            itemListKFXF.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemListKFXF.Add(new SelectListItem() { Text = "是", Value = "1" });
            itemListKFXF.Add(new SelectListItem() { Text = "否", Value = "0" });
            ViewData["KFXFList"] = itemListKFXF;
            List<SelectListItem> itemPayList = new List<SelectListItem>();
            int type = 1;
            PluginType pluginType = (PluginType)type;
            List<PluginInfo> payPluginList = AdminPlugins.GetInstalledPluginList(pluginType);
            itemPayList.Add(new SelectListItem() { Text = "全部", Value = "" });
            foreach (var item in payPluginList)
            {
                itemPayList.Add(new SelectListItem() { Text = item.FriendlyName, Value = item.SystemName });
            }
            ViewData["PayList"] = itemPayList;
            return View(model);
        }


        #region 订单导出
        /// <summary>
        /// 导出订单
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="accountName"></param>
        /// <param name="consignee"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortDirection"></param>
        /// <param name="storeId"></param>
        /// <param name="orderState"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        //public ActionResult DownloadOrderList(string storeName, string osn, DateTime? startDate, DateTime? endDate, string accountName, string consignee,
        //    string sortColumn, string sortDirection, int storeId = -1, int orderState = 0)
        //{
        //    return OutPutOrder(storeName, osn, consignee, orderState, startDate, endDate);
        //}

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
        public ActionResult OutPutOrder(int storeId, int uid, string osn, string consignee, string consigneeMobile, string orderState, DateTime? startDate, DateTime? endDate, DateTime? payStartDate, DateTime? payEndDate, int pid, string productName, int isKFXH, string payName)
        {
            //if (storeId == -1)
            //    storeId = WorkContext.PartUserInfo.StoreId;
            string condition = "";
            if (!startDate.HasValue)
            {
                startDate = DateTime.Parse("2016-01-01 00:00:00");
            }
            condition += " WHERE o.addtime>='" + startDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            if (endDate.HasValue)
                condition += "|o.addtime<='" + endDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + "'";

            if (!payStartDate.HasValue)
            {
                payStartDate = DateTime.Parse("2016-01-01 00:00:00");
            }
            condition += " | o.addtime>='" + payStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            if (payEndDate.HasValue)
                condition += "|o.addtime<='" + payEndDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss") + "'";
            if (storeId > 0)
            {
                if (WorkContext.PartUserInfo.StoreId > 0)
                    condition += "|o.storeid = " + WorkContext.PartUserInfo.StoreId + "";
                if (storeId > 0 && WorkContext.PartUserInfo.StoreId <= 0)
                    condition += "|o.storeid = " + storeId + "";
            }
            if (uid > 0)
                condition += "| o.[uid] =  " + uid;
            if (!string.IsNullOrEmpty(osn.Trim()))
            {
                condition += "|o.osn='" + osn.Trim() + "'";
            }
            if (!string.IsNullOrEmpty(consignee.Trim()))
            {
                condition += "|o.consignee='" + consignee.Trim() + "'";
            }
            if (!string.IsNullOrEmpty(consigneeMobile.Trim()))
            {
                condition += "|o.mobile='" + consigneeMobile.Trim() + "'";
            }
            if (!orderState.Trim().StartsWith("0"))
            {
                condition += "|o.orderstate IN (" + orderState + ")";//  condition.AppendFormat(" AND [orderstate] IN ({0}) ", orderState);
            }
            if (pid > 0)
                condition += " | o.[oid] IN (SELECT oid FROM hlh_orderproducts WHERE pid =" + pid + " ) ";
            if (!string.IsNullOrEmpty(productName))
                condition += " | o.[oid] IN (SELECT oid FROM hlh_orderproducts WHERE name LIKE '%" + productName + "%' ) ";
            if (isKFXH == 0)
                condition += "| o.cashdiscount=0 ";
            if (isKFXH == 1)
                condition += "| o.cashdiscount>0 ";
            if (!string.IsNullOrEmpty(payName))
                condition += " | o.[paysystemname] ='" + payName.Trim() + "'";
            condition += " | o.[storeid] in ( select storeid from hlh_stores where mallsource in(0,2,3) ) ";
            if (!string.IsNullOrEmpty(condition))
                condition = condition.Replace("|", " and ");
            // CASE  WHEN paytime<'1900-01-02 00:00:00.000' THEN '' ELSE CONVERT(varchar(100),paytime,120)   END 
            //CONVERT(DATETIME,o.paytime,101) 支付时间 
            string sqlText = string.Empty;
            //if (storeId != TypeHelper.StringToInt(WebHelper.GetConfigSettings("ChongZhiStore")))
            //{
            sqlText = @"SELECT CASE o.mallsource WHEN 0 THEN  '自营商城' ELSE '微商订货系统' END 平台来源,o.storename 店铺名称,
                                RTRIM(o.osn) 订单编号,
                                RTRIM(o.consignee) 收货人,
                                (SELECT rtrim(provincename)+RTRIM(cityname)+RTRIM(name) FROM dbo.hlh_regions WHERE regionid=o.regionid)+o.[address]                                 收货地址,
                                RTRIM(o.mobile) 收货人电话, 
                                ISNULL((SELECT RTRIM(username) username FROM dbo.hlh_users WHERE uid=o.uid),'')  会员编号,
                                ISNULL((SELECT RTRIM(mobile) FROM dbo.hlh_users WHERE uid=o.uid),'')  会员手机号,
                                ISNULL((SELECT  RTRIM(realname) FROM dbo.hlh_userdetails WHERE uid=o.uid),'') 会员姓名,
                                op.psn 产品编码,
                                op.name 产品名称,
                                op.discountprice  单价,
                                CASE op.type WHEN 3 THEN  op.realcount ELSE op.buycount END 数量,
                                CASE op.type WHEN 3 THEN  op.discountprice*op.realcount ELSE op.discountprice*op.buycount END 金额,
                                o.shipfee 物流费,
                                CASE o.orderstate WHEN 10 THEN '已提交' WHEN 30 THEN '等待付款' WHEN 50 THEN '确认中' WHEN 70 THEN '已确认' WHEN 90                                THEN '备货中' WHEN 110 THEN '已发货' WHEN 140 THEN '已完成' WHEN 180 THEN '锁定' WHEN 200 THEN '取消' WHEN 160 THEN                                '退货' ELSE '未知' END 订单状态,
                                CASE  WHEN CHARINDEX ('-身份证',o.buyerremark)>0 THEN SUBSTRING(o.buyerremark,0,CHARINDEX('-身份证',o.buyerremark))                                ELSE o.buyerremark END 备注,
                                CASE  WHEN CHARINDEX ('-身份证',o.buyerremark)>0 THEN  SUBSTRING(o.buyerremark,CHARINDEX('-身份证',o.buyerremark)                                  +5,LEN(o.buyerremark)-CHARINDEX('-身份证',o.buyerremark)+5) ELSE '' END 身份证,
                                CONVERT(varchar(100),o.addtime,120) 订单时间,
                                RTRIM(o.shipsn) 快递单号,
                                o.shipconame 快递公司,
                                o.payfriendname 支付方式,
                                CASE  WHEN o.paytime<'1900-01-02 00:00:00.000' THEN '' ELSE CONVERT(varchar(100),o.paytime,120)  END   支付时间,
                                o.adminremark 后台操作备注 
                                FROM hlh_orders o INNER JOIN hlh_orderproducts op ON o.oid=op.oid  " + condition + " ORDER BY o.addtime";
            //}
            //            else
            //            {
            //                sqlText = @"SELECT '自营商城' 平台来源,o.storename 店铺名称,
            //o.osn 订单编号,
            //o.consignee 收货人,
            //RTRIM(o.[address]) 收货地址,
            //o.mobile 收货人电话, 
            //CASE  WHEN u.mobile<>'' THEN u.mobile WHEN u.username<>'' THEN u.username WHEN u.email<>'' THEN u.email ELSE '' END  会员编号,
            //CASE u.isdirsaleuser WHEN 1 THEN '直销会员' ELSE '非直销会员' END 会员类型 ,ud.realname 会员名称,
            //op.psn 产品编码,
            //op.name 产品名称,
            //op.discountprice  单价,
            //CASE op.type WHEN 3 THEN  op.realcount ELSE op.buycount END 数量,
            //CASE op.type WHEN 3 THEN  op.discountprice*op.realcount ELSE op.discountprice*op.buycount END 金额,
            //o.shipfee 物流费,
            //op.productpv PV,
            //op.producthaimi 海米,
            //CASE o.orderstate WHEN 10 THEN '已提交' WHEN 30 THEN '等待付款' WHEN 50 THEN '确认中' WHEN 70 THEN '已确认' WHEN 90 THEN '备货中' WHEN 110 THEN '已发货' WHEN 140 THEN '已完成' WHEN 180 THEN '锁定' WHEN 200 THEN '取消' WHEN 160 THEN '退货' ELSE '未知' END 订单状态,
            //CASE  WHEN CHARINDEX ('-身份证',o.buyerremark)>0 THEN SUBSTRING(o.buyerremark,0,CHARINDEX('-身份证',o.buyerremark)) ELSE o.buyerremark END 备注,
            //   CASE  WHEN CHARINDEX ('-身份证',o.buyerremark)>0 THEN  SUBSTRING(o.buyerremark,CHARINDEX('-身份证',o.buyerremark)+5,LEN(o.buyerremark)-CHARINDEX('-身份证',o.buyerremark)+5) ELSE '' END 身份证,
            //CONVERT(varchar(100),o.addtime,120) 订单时间,
            //o.shipsn 快递单号,
            //o.shipconame 快递公司,
            //CASE  WHEN o.paytime<'1900-01-02 00:00:00.000' THEN '' ELSE CONVERT(varchar(100),o.paytime,120)  END   支付时间 
            //FROM hlh_orders o INNER JOIN hlh_orderproducts op ON o.oid=op.oid  
            //INNER JOIN hlh_users u ON o.[uid]=u.[uid] INNER JOIN hlh_userdetails ud ON o.[uid]=ud.[uid] " + condition + " ORDER BY o.addtime";
            //            }
            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "Orders_" + DateTime.Now.ToString("yyyyMMdd")));
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

        #region 导入
        public ActionResult excelImport()
        {
            StringBuilder sb = new StringBuilder();
            HttpPostedFileBase file = Request.Files[0];
            string path = MallUtils.SaveOrderExcel(file);
            if (path == "-1" || path == "-2" || path == "-3")
                return Content(path);
            //ExcelHelper_NPOI excel_helper = new ExcelHelper_NPOI(AppDomain.CurrentDomain.BaseDirectory + "test.xlsx");
            //ExcelHelper_NPOI excel_helper = new ExcelHelper_NPOI(path);
            DataTable dt = ExcelHelper_NPOI.ExcelToDataTable2(path, true);

            //List<string> tableList = GetColumnsByDataTable(dt);
            int i = 0;
            int j = 0;
            foreach (DataRow item in dt.Rows)
            {
                j++;
                try
                {
                    var osn = item[0].ToString().Trim();
                    if (osn.StartsWith("1"))//自营商城订单
                    {
                        OrderInfo order = Orders.GetOrderByOSN(osn);
                        if (order != null)
                        {
                            if (order.OrderState == (int)OrderState.PreProducting && string.IsNullOrWhiteSpace(order.ShipSN))
                            {
                                var shipName = item[1].ToString().Trim();
                                var shipNo = item[2].ToString().Trim();
                                if (!string.IsNullOrEmpty(shipName) && !string.IsNullOrEmpty(shipNo))
                                {
                                    ShipCompanyInfo shipCompanyInfo = ShipCompanies.GetShipCompanyByName(string.Format(" name like '%{0}%' ", shipName));
                                    if (shipCompanyInfo == null)
                                        shipCompanyInfo = ShipCompanies.GetShipCompanyByName("");
                                    AdminOrders.SendOrder(order.Oid, OrderState.Sended, shipNo, shipCompanyInfo.ShipCoId, shipCompanyInfo.Name, DateTime.Now);
                                    CreateOrderAction(order.Oid, OrderActionType.Send, "您的订单已经发货,发货方式为:" + shipCompanyInfo.Name + ",物流单号：" + shipNo);
                                    AddMallAdminLog("导入发货", "发货,订单号为:" + order.OSN);
                                    i++;
                                }
                            }
                        }
                    }
                    if (osn.StartsWith("8"))//代理拿货单
                    {
                        AgentSendOrderInfo agentSend = new AgentSendOrder().GetModel(string.Format(" sendosn='{0}' ", osn));
                        if (agentSend != null)
                        {
                            if (agentSend.SendState == 0 && string.IsNullOrWhiteSpace(agentSend.ShipSN))
                            {
                                var shipName = item[1].ToString().Trim();
                                var shipNo = item[2].ToString().Trim();
                                if (!string.IsNullOrEmpty(shipName) && !string.IsNullOrEmpty(shipNo))
                                {
                                    ShipCompanyInfo shipCompanyInfo = ShipCompanies.GetShipCompanyByName(string.Format(" name like '%{0}%' ", shipName));
                                    if (shipCompanyInfo == null)
                                        shipCompanyInfo = ShipCompanies.GetShipCompanyByName("");
                                    agentSend.SendState = 1;
                                    agentSend.ShipSN = shipNo;
                                    agentSend.ShipCoid = shipCompanyInfo.ShipCoId;
                                    agentSend.ShipCoName = shipCompanyInfo.Name;
                                    agentSend.ShipTime = DateTime.Now;
                                    new AgentSendOrder().Update(agentSend);
                                    AddMallAdminLog("要货单导入发货", "发货,要货单号为:" + agentSend.SendOSN);
                                    i++;
                                }
                            }

                        }
                    }
                    //sb.Append(item[2].ToString() + "\r\t");
                }
                catch (Exception ex)
                { }
            }
            return Content("总记录数：" + j + "条，处理有效数据：" + i + "条");
        }
        #endregion

        /// <summary>
        /// 订单批量备货
        /// </summary>
        public ActionResult BatchPreProduct(int[] oid)
        {
            if (oid.Length == 0 || oid == null)
                return PromptView("请选择操作订单");
            List<OrderInfo> orderList = Orders.GetOrderListByWhere(string.Format(" [oid] IN ({0}) ", CommonHelper.IntArrayToString(oid)));
            if (orderList.Exists(x => x.OrderState != (int)OrderState.Confirmed))
                return PromptView("选中订单存在不为已确认状态订单，请重新筛选");
            foreach (var item in oid)
            {
                OrderInfo orderInfo = AdminOrders.GetOrderByOid(item);
                if (orderInfo != null)
                {
                    AdminOrders.PreProduct(orderInfo);
                    //订单备货后活动优惠券立即赠送
                    //发放单品促销活动优惠劵
                    List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
                    PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
                    foreach (OrderProductInfo orderProductInfo in orderProductList)
                    {
                        if (orderProductInfo.Type == 0)
                        {
                            if (orderProductInfo.CouponTypeId > 0)
                            {
                                if (orderInfo.CouponMoney <= 0)
                                //双11蟹券处理--蟹券每满200送一张 400送两张
                                {
                                    decimal sendCount = Math.Floor(orderInfo.SurplusMoney / 200);
                                    for (int i = 0; i < sendCount; i++)
                                    {
                                        Coupons.SendSinglePromotionCoupon(partUserInfo, orderProductInfo.CouponTypeId, orderInfo, orderInfo.IP);
                                    }
                                }
                            }
                        }
                    }
                    CreateOrderAction(orderInfo.Oid, OrderActionType.PreProduct, "您的订单已经备货完成");
                }
            }
            AddMallAdminLog("批量备货", "批量备货,订单ID为:" + CommonHelper.IntArrayToString(oid));

            return PromptView("批量备货成功");
        }

        /// <summary>
        /// 订单信息
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public ActionResult OrderInfo(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");

            OrderInfoModel model = new OrderInfoModel();
            model.OrderInfo = orderInfo;
            model.RegionInfo = Regions.GetRegionById(orderInfo.RegionId);
            UserInfo userInfo = Users.GetUserById(orderInfo.Uid);
            if (userInfo == null)
            {
                userInfo = new UserInfo();
            }
            model.UserInfo = userInfo;
            model.UserRankInfo = new UserRankInfo();//AdminUserRanks.GetUserRankById(model.UserInfo.UserRid);
            model.OrderProductList = AdminOrders.GetOrderProductList(oid);
            model.OrderActionList = OrderActions.GetOrderActionList(oid);
            if (orderInfo.ReturnType == 1)
                model.OrderReturnInfo = OrderReturn.GetOrderReturnByOid(oid);
            if (orderInfo.ChangeType == 1)
                model.OrderChangeInfo = OrderChange.GetChangeOrderByOid(oid);

            string[] sizeList = StringHelper.SplitString(WorkContext.MallConfig.ProductShowThumbSize);

            ViewData["size"] = sizeList[sizeList.Length / 2];
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }


        #region 修改订单信息
        /// <summary>
        /// 修改订单收货时间--针对过了退货周期的订单，修改后再次开放退货入口，已结算是否考虑退回？（出现情况较少）
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult ModifyReceivTime(int oid)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单未收货不能修改收货时间");
            if (orderInfo.OrderState == (int)OrderState.Completed && orderInfo.ReturnType == 1)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单已申请退货正在审核不能修改收货时间");
            bool result = Orders.UpdateOrderAdminReceivTime(oid, DateTime.Now);
            AddMallAdminLog("修改订单收货时间", "修改订单收货时间,订单ID为:" + oid + ",订单号：" + orderInfo.OSN + ",修改收货时间:" + DateTime.Now);
            if (result)
                return Content("1");
            else
                return Content("0");
        }

        /// <summary>
        /// 增加订单后台备注
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public ActionResult AddAdminRemark(int oid, string remark)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            remark += ",操作人:" + WorkContext.PartUserInfo.NickName + ",操作时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "。";
            bool result = Orders.UpdateOrderAdminRemark(oid, remark);
            if (result)
                return Content("1");
            else
                return Content("0");
        }

        /// <summary>
        /// 更新快递信息
        /// </summary>
        [HttpGet]
        public ActionResult UpdateShipInfo(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Sended)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "不能修改快递信息");

            UpdateShipInfoModel model = new UpdateShipInfoModel();
            model.OldShipSN = orderInfo.ShipSN;
            model.OldShipCoName = orderInfo.ShipCoName;
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 更新快递信息
        /// </summary>
        [HttpPost]
        public ActionResult UpdateShipInfo(UpdateShipInfoModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Sended)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "不能修改快递信息");

            ShipCompanyInfo shipCompanyInfo = ShipCompanies.GetShipCompanyById(model.ShipCoId);
            if (shipCompanyInfo == null)
                ModelState.AddModelError("ShipCoId", "请选择配送公司");

            if (ModelState.IsValid)
            {
                Orders.UpdateShipInfo(oid, model.ShipSN, model.ShipCoId, shipCompanyInfo.Name);

                AddMallAdminLog("修改快递信息", "修改快递单信息,订单ID为:" + oid + ",原快递信息：" + model.OldShipCoName + "-" + orderInfo.ShipSN + ",新快递信息:" + shipCompanyInfo.Name + "-" + model.ShipSN);
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "修改快递信息成功");
            }
            ViewData["orderInfo"] = orderInfo;
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

        /// <summary>
        /// 更新订单配送费用
        /// </summary>
        [HttpGet]
        public ActionResult UpdateOrderShipFee(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (!(orderInfo.OrderState == (int)OrderState.WaitPaying || (orderInfo.OrderState == (int)OrderState.Confirming && orderInfo.PayMode == 0)))
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "不能修改订单配送费用");

            UpdateOrderShipFeeModel model = new UpdateOrderShipFeeModel();
            model.ShipFee = orderInfo.ShipFee;
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 更新订单配送费用
        /// </summary>
        [HttpPost]
        public ActionResult UpdateOrderShipFee(UpdateOrderShipFeeModel model, int oid = -1)
        {
            lock (_locker)
            {
                OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
                if (orderInfo == null)
                    return PromptView("订单不存在");
                if (!(orderInfo.OrderState == (int)OrderState.WaitPaying || (orderInfo.OrderState == (int)OrderState.Confirming && orderInfo.PayMode == 0)))
                    return PromptView(Url.Action("orderinfo", new { oid = oid }), "不能修改订单配送费用");

                if (ModelState.IsValid)
                {
                    decimal change = model.ShipFee - orderInfo.ShipFee;
                    Orders.UpdateOrderShipFee(orderInfo.Oid, model.ShipFee, orderInfo.OrderAmount + change, orderInfo.SurplusMoney + change);
                    CreateOrderAction(oid, OrderActionType.UpdateShipFee, "您订单的配送费用已经修改");
                    AddMallAdminLog("更新订单配送费用", "更新订单配送费用,订单ID为:" + oid);

                    if ((orderInfo.SurplusMoney + change) <= 0)
                        AdminOrders.UpdateOrderState(oid, OrderState.Confirming);

                    return PromptView(Url.Action("orderinfo", new { oid = oid }), "更新订单配送费用成功");
                }
                ViewData["orderInfo"] = orderInfo;
                return View(model);
            }
        }

        /// <summary>
        /// 更新订单折扣
        /// </summary>
        [HttpGet]
        public ActionResult UpdateOrderDiscount(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (!(orderInfo.OrderState == (int)OrderState.WaitPaying || (orderInfo.OrderState == (int)OrderState.Confirming && orderInfo.PayMode == 0)))
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "不能修改订单折扣");

            UpdateOrderDiscountModel model = new UpdateOrderDiscountModel();
            model.Discount = orderInfo.Discount;
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 更新订单折扣
        /// </summary>
        [HttpPost]
        public ActionResult UpdateOrderDiscount(UpdateOrderDiscountModel model, int oid = -1)
        {
            lock (_locker)
            {
                OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
                if (orderInfo == null)
                    return PromptView("订单不存在");
                if (!(orderInfo.OrderState == (int)OrderState.WaitPaying || (orderInfo.OrderState == (int)OrderState.Confirming && orderInfo.PayMode == 0)))
                    return PromptView(Url.Action("orderinfo", new { oid = oid }), "不能修改订单折扣");

                if (model.Discount > (orderInfo.SurplusMoney + orderInfo.Discount))
                    ModelState.AddModelError("Discount", "折扣不能大于需支付金额");

                if (ModelState.IsValid)
                {
                    decimal surplusMoney = orderInfo.SurplusMoney + orderInfo.Discount - model.Discount;
                    Orders.UpdateOrderDiscount(orderInfo.Oid, model.Discount, surplusMoney);
                    CreateOrderAction(oid, OrderActionType.UpdateDiscount, "您订单的折扣已经修改");
                    AddMallAdminLog("更新订单折扣", "更新订单折扣,订单ID为:" + oid);

                    if (surplusMoney <= 0)
                        AdminOrders.UpdateOrderState(oid, OrderState.Confirming);

                    return PromptView(Url.Action("orderinfo", new { oid = oid }), "更新订单折扣成功");
                }
                ViewData["orderInfo"] = orderInfo;
                return View(model);
            }
        }


        /// <summary>
        /// 订单取消删除
        /// </summary>
        public ActionResult CancelDelete(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.IsDelete == 0)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单是未删除状态，不能取消删除");

            Orders.DeleteOrder(oid, false);

            return PromptView(Url.Action("orderinfo", new { oid = oid }), "取消删除成功");
        }

        /// <summary>
        /// 延长收货期
        /// </summary>

        public ActionResult ExtendReceive(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Sended)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未发货，不能延长收货");
            if (orderInfo.IsExtendReceive == 1)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单已经申请延长收货，不能重复申请");
            Orders.ExtendReceive(oid);
            return PromptView(Url.Action("orderinfo", new { oid = oid }), "延长收货成功");
        }



        #endregion

        #region 订单处理流程，更新订单状态
        /// <summary>
        /// 支付订单
        /// </summary>
        [HttpGet]
        public ActionResult PayOrder(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");

            if (orderInfo.PayMode != 2)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "付款操作只适用于线下付款");
            if (orderInfo.OrderState != (int)OrderState.WaitPaying)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "当前不能支付订单");

            PayOrderModel model = new PayOrderModel();
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 支付订单
        /// </summary>
        [HttpPost]
        public ActionResult PayOrder(PayOrderModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");

            if (orderInfo.PayMode != 2)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "付款操作只适用于线下付款");
            if (orderInfo.OrderState != (int)OrderState.WaitPaying)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "当前不能支付订单");

            if (ModelState.IsValid)
            {
                AdminOrders.PayOrder(oid, OrderState.Confirming, model.PaySN, DateTime.Now);
                CreateOrderAction(oid, OrderActionType.Pay, "您的订单成功支付,请等待确认");
                AddMallAdminLog("支付订单", "支付订单,订单ID为:" + oid);
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单支付成功");
            }
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }


        /// <summary>
        /// 银行汇款订单确认
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult BankTransferConfirm(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.WaitPaying)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "不能进行汇款确认操作");

            BankTransferConfirmModel model = new BankTransferConfirmModel();
            model.PaySN = orderInfo.PaySN;
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BankTransferConfirm(BankTransferConfirmModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.WaitPaying)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "不能进行汇款确认操作");

            if (ModelState.IsValid)
            {

                Orders.PayOrder(orderInfo.Oid, OrderState.Confirmed, model.PaySN, DateTime.Now);
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = WorkContext.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.Pay,
                    ActionTime = DateTime.Now,//交易时间,
                    ActionDes = "您使用银行汇款支付订单成功，交易号为:" + model.PaySN
                });
                AddMallAdminLog("汇款订单确认", "确认汇款,订单ID为:" + oid + ",汇款单号：" + model.PaySN);
                Orders.ConfirmOrder(orderInfo);
                //订单赠送兑换码
                List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
                if (!orderProductList.Exists(x => StringHelper.StrContainsForNum(WebHelper.GetConfigSettings("CoffeeQuanPid"), x.Pid.ToString())))//存在汇购卡，不赠送兑换码
                {
                    //不使用汇购卡支付，不属于尚睿淳店铺，时间在5月16之前，订单金额大于299

                    List<int> testuid = new List<int>() { 1, 8081, 18782, 23073, 27460, 31506, 32138, 32861, 32977, 32979, 32986, 33492, 33497, 33499, 33500, 15171, 29361 };
                    if (((DateTime.Now < new DateTime(2017, 5, 16) && DateTime.Now >= new DateTime(2017, 4, 1)) || testuid.Exists(x => x == orderInfo.Uid)) && orderInfo.OrderAmount >= 299 && orderInfo.CashDiscount <= 0 && orderInfo.StoreId.ToString() != WebHelper.GetConfigSettings("SRCStoreId"))
                    {
                        ExChangeCouponsInfo codeInfo = new ExChangeCouponsInfo();
                        codeInfo.createoid = orderInfo.Oid;
                        codeInfo.createtime = DateTime.Now;
                        codeInfo.createuid = orderInfo.Uid;
                        codeInfo.state = 1;
                        codeInfo.validtime = new DateTime(2017, 6, 1);
                        codeInfo.uid = orderInfo.Uid;
                        if (orderInfo.OrderAmount >= 299 && orderInfo.OrderAmount < 1999)
                        {
                            codeInfo.codetypeid = 3;
                            codeInfo.type = 3;
                            codeInfo.exsn = "YDH" + Randoms.CreateRandomValue(8, true).ToLower();

                        }
                        else if (orderInfo.OrderAmount >= 1999 && orderInfo.OrderAmount < 4599)
                        {
                            codeInfo.codetypeid = 4;
                            codeInfo.type = 4;
                            codeInfo.exsn = "JDH" + Randoms.CreateRandomValue(8, true).ToLower();
                        }
                        else if (orderInfo.OrderAmount >= 4599)
                        {
                            codeInfo.codetypeid = 5;
                            codeInfo.type = 5;
                            codeInfo.exsn = "ZDH" + Randoms.CreateRandomValue(8, true).ToLower();
                        }
                        ExChangeCoupons.Add(codeInfo);
                    }
                }
                //OrderActions.CreateOrderAction(new OrderActionInfo()
                //{
                //    Oid = orderInfo.Oid,
                //    Uid = orderInfo.Uid,
                //    RealName = "系统",
                //    ActionType = (int)OrderActionType.Confirm,
                //    ActionTime = DateTime.Now,//交易时间,
                //    ActionDes = "您的订单已经确认,正在备货中"
                //});
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "银行汇款订单确认成功");
            }
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }
        /// <summary>
        /// 确认订单
        /// </summary>
        public ActionResult ConfirmOrder(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Confirming && orderInfo.PayMode == 1)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "买家还未付款，不能确认订单");

            Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, "", DateTime.Now);
            AdminOrders.ConfirmOrder(orderInfo);

            PartUserInfo userInfo = Users.GetPartUserById(orderInfo.Uid);
            if (!userInfo.IsDirSaleUser)
            {
                if (userInfo.IsFXUser < 2 && orderInfo.OrderAmount >= 15800)
                    OrderUtils.UpdateFXUserSates(userInfo.Uid, 2);
                else if (userInfo.IsFXUser == 0 && orderInfo.OrderAmount < 15800)
                    OrderUtils.UpdateFXUserSates(userInfo.Uid, 1);
            }
            CreateOrderAction(oid, OrderActionType.Confirm, "您的订单已经确认,正在备货中");
            AddMallAdminLog("确认订单", "确认订单,订单ID为:" + oid);
            return PromptView(Url.Action("orderinfo", new { oid = oid }), "确认订单成功");
        }

        /// <summary>
        /// 备货
        /// </summary>
        public ActionResult PreOrderProduct(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Confirmed)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未确认，请先确认");

            AdminOrders.PreProduct(orderInfo);

            //订单备货后活动优惠券立即赠送
            //发放单品促销活动优惠劵
            List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0)
                {
                    if (orderProductInfo.CouponTypeId > 0)
                    {
                        if (orderInfo.CouponMoney <= 0)
                        //双11蟹券处理--蟹券每满200送一张 400送两张
                        {
                            decimal sendCount = Math.Floor(orderInfo.SurplusMoney / 200);
                            for (int i = 0; i < sendCount; i++)
                            {
                                Coupons.SendSinglePromotionCoupon(partUserInfo, orderProductInfo.CouponTypeId, orderInfo, orderInfo.IP);
                            }
                        }
                    }
                }
            }
            CreateOrderAction(oid, OrderActionType.PreProduct, "您的订单已经备货完成");
            AddMallAdminLog("备货", "备货,订单ID为:" + oid);
            return PromptView(Url.Action("orderinfo", new { oid = oid }), "备货成功");
        }

        /// <summary>
        /// 发货
        /// </summary>
        [HttpGet]
        public ActionResult SendOrderProduct(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.PreProducting)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未完成备货,不能发货");

            SendOrderProductModel model = new SendOrderProductModel();
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 发货
        /// </summary>
        [HttpPost]
        public ActionResult SendOrderProduct(SendOrderProductModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.PreProducting)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未完成备货,不能发货");

            ShipCompanyInfo shipCompanyInfo = ShipCompanies.GetShipCompanyById(model.ShipCoId);
            if (shipCompanyInfo == null)
                ModelState.AddModelError("ShipCoId", "请选择配送公司");

            if (ModelState.IsValid)
            {
                AdminOrders.SendOrder(oid, OrderState.Sended, model.ShipSN, model.ShipCoId, shipCompanyInfo.Name, DateTime.Now);
                CreateOrderAction(oid, OrderActionType.Send, "您的订单已经发货,发货方式为:" + shipCompanyInfo.Name + ",物流单号：" + model.ShipSN);
                AddMallAdminLog("发货", "发货,订单ID为:" + oid);
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "发货成功");
            }
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 完成订单
        /// </summary>
        [HttpGet]
        public ActionResult CompleteOrder(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Sended)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未发货，不能完成订单");

            if (orderInfo.PayMode != 0)
            {
                PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
                AdminOrders.CompleteOrder(ref partUserInfo, orderInfo, DateTime.Now, WorkContext.IP);
                CreateOrderAction(oid, OrderActionType.Complete, "订单已完成，感谢您在" + WorkContext.MallConfig.MallName + "购物，欢迎您再次光临");
                AddMallAdminLog("完成订单", "完成订单,订单ID为:" + oid);
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "完成订单成功");
            }
            else
            {
                CompleteOrderModel model = new CompleteOrderModel();
                ViewData["orderInfo"] = orderInfo;
                return View(model);
            }
        }

        /// <summary>
        /// 完成订单
        /// </summary>
        [HttpPost]
        public ActionResult CompleteOrder(CompleteOrderModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Sended)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未发货，不能完成订单");
            if (orderInfo.PayMode != 0)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "只有货到付款订单才需要填写支付单号");

            if (string.IsNullOrWhiteSpace(model.PaySN))
                ModelState.AddModelError("PaySN", "请填写支付单号");

            if (ModelState.IsValid)
            {
                AdminOrders.PayOrder(oid, OrderState.Sended, model.PaySN, DateTime.Now);

                PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
                AdminOrders.CompleteOrder(ref partUserInfo, orderInfo, DateTime.Now, WorkContext.IP);
                CreateOrderAction(oid, OrderActionType.Complete, "订单已完成，感谢您在" + WorkContext.MallConfig.MallName + "购物，欢迎您再次光临");
                AddMallAdminLog("完成订单", "完成订单,订单ID为:" + oid);
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "完成订单成功");
            }
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 退货
        /// </summary>
        public ActionResult ReturnOrderProduct(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Sended && orderInfo.OrderState != (int)OrderState.Completed)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单当前不能退货");

            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            //int completeCount = Orders.GetUserOrderCount(WorkContext.Uid, "", "", (int)OrderState.Completed);
            //if (completeCount <= 0)
            //    OrderUtils.UpdateFXUserSates(WorkContext.Uid, 0);

            //判断是否满足红包退回
            if (orderInfo.OrderState == (int)OrderState.Completed)
            {
                //存在推广产品配置退回99红包
                List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
                if (orderProductList.Exists(x => x.Pid == TypeHelper.StringToInt(WebConfigurationManager.AppSettings["ActiveProduct"], 0)))
                {
                    List<AccountInfo> info = AccountUtils.GetAccountList(partUserInfo.Uid, partUserInfo.IsDirSaleUser, partUserInfo.DirSaleUid);
                    OrderProductInfo orderProductInfo = orderProductList.Find(x => x.Pid == TypeHelper.StringToInt(WebConfigurationManager.AppSettings["ActiveProduct"], 0));
                    if (info.Find(x => x.AccountId == (int)AccountType.红包账户).Banlance >= 99 * orderProductInfo.BuyCount)
                        AccountUtils.ReturnActiveHongBao(partUserInfo, orderInfo, 99 * orderProductInfo.BuyCount, partUserInfo.Uid, DateTime.Now);
                    else
                    {
                        return PromptView("订单当前不能取消,红包金额不够，请联系客服");

                    }
                }
            }
            string sqlStr = string.Format(" orderstate>={0} and orderstate<={1}  and uid={2} ", (int)OrderState.Confirmed, (int)OrderState.Completed, orderInfo.Uid);
            string sqlStr2 = string.Format(" orderstate>={0} and orderstate<={1} and uid={2} and orderamount>=15800 ", (int)OrderState.Confirmed, (int)OrderState.Completed, orderInfo.Uid);
            List<OrderInfo> UserOrderList = Orders.GetOrderListByWhere(sqlStr);
            List<OrderInfo> UserOrderList2 = Orders.GetOrderListByWhere(sqlStr2);
            if (UserOrderList.Count <= 0)
                OrderUtils.UpdateFXUserSates(WorkContext.Uid, 0);
            if (UserOrderList2.Count <= 0 && UserOrderList.Count > 0)
                OrderUtils.UpdateFXUserSates(WorkContext.Uid, 1);

            AdminOrders.ReturnOrder(ref partUserInfo, orderInfo, WorkContext.Uid, DateTime.Now);
            CreateOrderAction(oid, OrderActionType.Return, "订单已申请退货，请等待系统处理");
            AddMallAdminLog("退货申请", "退货申请,订单ID为:" + oid);
            return PromptView(Url.Action("orderinfo", new { oid = oid }), "退货申请成功");
        }

        /// <summary>
        /// 锁定订单
        /// </summary>
        public ActionResult LockOrder(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (!(orderInfo.OrderState == (int)OrderState.WaitPaying || (orderInfo.OrderState == (int)OrderState.Confirming && orderInfo.PayMode == 0)))
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单当前不能锁定");

            AdminOrders.LockOrder(orderInfo);
            CreateOrderAction(oid, OrderActionType.Lock, "订单已锁定");
            AddMallAdminLog("锁定", "锁定,订单ID为:" + oid);
            return PromptView(Url.Action("orderinfo", new { oid = oid }), "锁定成功");
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        public ActionResult CancelOrder(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState >= (int)OrderState.Sended)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单当前不能取消");
            CouponInfo couponInfo = Coupons.GetCouponBywhere(string.Format(" [createoid]={0} ", orderInfo.Oid));
            if (couponInfo != null)
            {
                if (couponInfo.Oid > 0)
                    return PromptView("订单赠送优惠券已使用，当前不能取消订单");
            }
            ExChangeCouponsInfo exInfo = ExChangeCoupons.GetModel(string.Format(" [createoid]={0} ", orderInfo.Oid));
            if (exInfo != null)
            {
                if (exInfo.oid > 0)
                    return PromptView("订单赠送兑换码已使用，不能取消订单");
            }
            //OrderInfo subOrder = Orders.GetOrdertByWhere(string.Format(" mainoid={0} ", orderInfo.Oid));
            //if (subOrder != null)
            //{
            //    if (subOrder.OrderState >= (int)OrderState.Confirmed && subOrder.OrderState <= (int)OrderState.Completed)
            //        return AjaxResult("donotcancel", "需先取消必选套餐");
            //}
            //if (orderInfo.MainOid > 0 || orderInfo.SubOid > 0)
            if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId") || orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId"))
            {
                return RedirectToAction("AgentCancelOrder", new RouteValueDictionary { { "oid", orderInfo.Oid } });
            }


            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);

            List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
            //判断是否满足红包退回
            if (orderInfo.OrderState == (int)OrderState.Confirmed)
            {
                //存在推广产品配置退回99红包

                if (orderProductList.Exists(x => x.Pid == TypeHelper.StringToInt(WebConfigurationManager.AppSettings["ActiveProduct"], 0)))
                {
                    List<AccountInfo> info = AccountUtils.GetAccountList(partUserInfo.Uid, partUserInfo.IsDirSaleUser, partUserInfo.DirSaleUid);
                    OrderProductInfo orderProductInfo = orderProductList.Find(x => x.Pid == TypeHelper.StringToInt(WebConfigurationManager.AppSettings["ActiveProduct"], 0));
                    if (info.Find(x => x.AccountId == (int)AccountType.红包账户).Banlance >= 99 * orderProductInfo.BuyCount)
                        AccountUtils.ReturnActiveHongBao(partUserInfo, orderInfo, 99 * orderProductInfo.BuyCount, partUserInfo.Uid, DateTime.Now);
                    else
                    {
                        return PromptView("订单当前不能取消,红包金额不够，请联系客服");
                    }
                }
                if (orderProductList.Exists(x => StringHelper.StrContainsForNum(WebHelper.GetConfigSettings("CoffeeQuanPid"), x.Pid.ToString())))
                    return PromptView("订单含有汇购卡券产品不能取消，请联系客服");
            }
            AdminOrders.CancelOrder(ref partUserInfo, orderInfo, WorkContext.Uid, DateTime.Now);
            //更新分销资格状态
            string sqlStr = string.Format(" orderstate>={0} and orderstate<={1}  and uid={2} ", (int)OrderState.Confirmed, (int)OrderState.Completed, orderInfo.Uid);
            string sqlStr2 = string.Format(" orderstate>={0} and orderstate<={1} and uid={2} and orderamount>=15800 ", (int)OrderState.Confirmed, (int)OrderState.Completed, orderInfo.Uid);
            List<OrderInfo> UserOrderList = Orders.GetOrderListByWhere(sqlStr);
            List<OrderInfo> UserOrderList2 = Orders.GetOrderListByWhere(sqlStr2);
            if (UserOrderList.Count <= 0)
                OrderUtils.UpdateFXUserSates(orderInfo.Uid, 0);
            if (UserOrderList2.Count <= 0 && UserOrderList.Count > 0)
                OrderUtils.UpdateFXUserSates(orderInfo.Uid, 1);

            //如果已支付并支付金额大于0取消订单 要发起退款操作  非在线支付 取消订单应生成退款请求
            if (orderInfo.OrderState >= (int)OrderState.Confirmed && orderInfo.SurplusMoney > 0)
            {
                if (orderInfo.PayMode == 1 && orderInfo.PaySystemName == "wechatpay")
                {
                    OrderRefunds.ApplyRefund(new OrderRefundInfo()
                    {
                        StoreId = orderInfo.StoreId,
                        StoreName = orderInfo.StoreName,
                        Oid = oid,
                        OSN = orderInfo.OSN,
                        Uid = orderInfo.Uid,
                        State = 0,
                        ApplyTime = DateTime.Now,
                        PayMoney = orderInfo.SurplusMoney,
                        RefundMoney = orderInfo.SurplusMoney,
                        RefundSN = "",
                        RefundFriendName = orderInfo.PayFriendName,
                        RefundSystemName = orderInfo.PaySystemName,
                        PayFriendName = orderInfo.PayFriendName,
                        PaySystemName = orderInfo.PaySystemName,
                        RefundTime = DateTime.Now,
                        PaySN = orderInfo.PaySN,
                        RefundTranSN = "",//记录退款流水号 
                        ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",支付成功取消订单订单退款"
                    });
                    PluginInfo PayPlugin = Plugins.GetPayPluginBySystemName(orderInfo.PaySystemName);
                    return RedirectToAction("ReFund", "AdminWeChat", new RouteValueDictionary { { "oid", orderInfo.Oid } });
                }
                else if (orderInfo.PayMode == 1 && orderInfo.PaySystemName == "alipay")//支付宝退款需后台操作，此处只生成退款记录，
                {
                    OrderRefunds.ApplyRefund(new OrderRefundInfo()
                    {
                        StoreId = orderInfo.StoreId,
                        StoreName = orderInfo.StoreName,
                        Oid = oid,
                        OSN = orderInfo.OSN,
                        Uid = orderInfo.Uid,
                        State = 0,
                        ApplyTime = DateTime.Now,
                        PayMoney = orderInfo.SurplusMoney,
                        RefundMoney = orderInfo.SurplusMoney,
                        RefundSN = "",
                        RefundFriendName = orderInfo.PayFriendName,
                        RefundSystemName = orderInfo.PaySystemName,
                        PayFriendName = orderInfo.PayFriendName,
                        PaySystemName = orderInfo.PaySystemName,
                        RefundTime = DateTime.Now,
                        PaySN = orderInfo.PaySN,
                        RefundTranSN = "",//记录退款流水号 
                        ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",支付成功取消订单订单退款"
                    });
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = oid,
                        Uid = orderInfo.Uid,
                        RealName = "系统",
                        ActionType = (int)OrderActionType.Cancel,
                        ActionTime = DateTime.Now,
                        ActionDes = "订单取消,请等待系统退款,退款会在1-3个工作日内将退款返回至帐号中"
                    });
                }
                else if (orderInfo.PayMode == 1 && orderInfo.PaySystemName == "IPSpay")//环迅退款需后台操作，此处只生成退款记录，
                {
                    OrderRefunds.ApplyRefund(new OrderRefundInfo()
                    {
                        StoreId = orderInfo.StoreId,
                        StoreName = orderInfo.StoreName,
                        Oid = oid,
                        OSN = orderInfo.OSN,
                        Uid = orderInfo.Uid,
                        State = 0,
                        ApplyTime = DateTime.Now,
                        PayMoney = orderInfo.SurplusMoney,
                        RefundMoney = orderInfo.SurplusMoney,
                        RefundSN = "",
                        RefundFriendName = orderInfo.PayFriendName,
                        RefundSystemName = orderInfo.PaySystemName,
                        PayFriendName = orderInfo.PayFriendName,
                        PaySystemName = orderInfo.PaySystemName,
                        RefundTime = DateTime.Now,
                        PaySN = orderInfo.PaySN,
                        RefundTranSN = "",//记录退款流水号 
                        ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",支付成功取消订单订单退款"
                    });
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = oid,
                        Uid = orderInfo.Uid,
                        RealName = "系统",
                        ActionType = (int)OrderActionType.Cancel,
                        ActionTime = DateTime.Now,
                        ActionDes = "订单取消,请等待系统退款,退款会在1-3个工作日内将退款返回至帐号中"
                    });
                }
                else if (orderInfo.PayMode == 2)
                {
                    OrderRefunds.ApplyRefund(new OrderRefundInfo()
                    {
                        StoreId = orderInfo.StoreId,
                        StoreName = orderInfo.StoreName,
                        Oid = oid,
                        OSN = orderInfo.OSN,
                        Uid = orderInfo.Uid,
                        State = 0,
                        ApplyTime = DateTime.Now,
                        PayMoney = orderInfo.SurplusMoney,
                        RefundMoney = orderInfo.SurplusMoney,
                        RefundSN = "",
                        RefundFriendName = orderInfo.PayFriendName,
                        RefundSystemName = orderInfo.PaySystemName,
                        PayFriendName = orderInfo.PayFriendName,
                        PaySystemName = orderInfo.PaySystemName,
                        RefundTime = DateTime.Now,
                        PaySN = orderInfo.PaySN,
                        RefundTranSN = "",//记录退款流水号 
                        ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",支付成功取消订单订单退款"
                    });
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = oid,
                        Uid = orderInfo.Uid,
                        RealName = "系统",
                        ActionType = (int)OrderActionType.Cancel,
                        ActionTime = DateTime.Now,
                        ActionDes = "付款成功的订单取消,付款方式为银行汇款，请联系客服进行退款"
                    });
                }
            }
            else
            {
                CreateOrderAction(oid, OrderActionType.Cancel, "订单已取消");
            }
            AddMallAdminLog("取消订单", "取消订单,订单ID为:" + oid);
            return PromptView(Url.Action("orderinfo", new { oid = oid }), "取消订单成功");
        }

        /// <summary>
        /// 代理订单取消订单
        /// </summary>
        public ActionResult AgentCancelOrder(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState >= (int)OrderState.Sended)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单已发货，不能取消");
            CouponInfo couponInfo = Coupons.GetCouponBywhere(string.Format(" [createoid]={0} ", orderInfo.Oid));
            if (couponInfo != null)
            {
                if (couponInfo.Oid > 0)
                    return PromptView("订单赠送优惠券已使用，当前不能取消订单");
            }
            ExChangeCouponsInfo exInfo = ExChangeCoupons.GetModel(string.Format(" [createoid]={0} ", orderInfo.Oid));
            if (exInfo != null)
            {
                if (exInfo.oid > 0)
                    return PromptView("订单赠送兑换码已使用，不能取消订单");
            }
            //OrderInfo subOrder = Orders.GetOrdertByWhere(string.Format(" mainoid={0} ", orderInfo.Oid));
            //if (subOrder != null)
            //{
            //    if (subOrder.OrderState >= (int)OrderState.Confirmed && subOrder.OrderState <= (int)OrderState.Completed)
            //        return PromptView("donotcancel", "需先取消必选套餐");
            //}

            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            List<OrderInfo> AllAgentOrders = Orders.GetOrderListByWhere(string.Format(" (oid ={0} OR oid={1} OR oid={2}) ", orderInfo.Oid, orderInfo.MainOid, orderInfo.SubOid));

            PartUserInfo user = Users.GetPartUserById(orderInfo.Uid);


            bool isTrue = true;
            if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId") && user.UserSource == 1)//单套餐包取消判断下级会员
            {
                List<UserInfo> chilerenuserList = Users.GetSubRecommendListByPid(user, 1, 2000).FindAll(x => x.AgentType > 0);
                if (chilerenuserList.Count > 0)
                    return PromptView("存在下级代理会员，该订单不能取消");
            }

            OrderInfo aorder = AllAgentOrders.Find(x => x.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"));
            if (aorder != null)
            {
                AgentStockDetailInfo detailInfo = new AgentStockDetail().GetModel(string.Format(" uid={0}  and ordercode='{1}' ", user.Uid, aorder.OSN));
                if (detailInfo != null)//存在代理库存记录，说明为代理定级订单
                {
                    List<UserInfo> chilerenuserList = Users.GetSubRecommendListByPid(user, 1, 2000).FindAll(x => x.AgentType > 0);
                    if (chilerenuserList.Count > 0)
                        return PromptView("存在下级代理会员，该订单不能取消");

                    List<OrderProductInfo> oplist = Orders.GetOrderProductList(aorder.Oid);
                    List<AgentSendOrderInfo> sendList = new AgentSendOrder().GetList(string.Format(" uid={0} and pid in ({1})", user.Uid, string.Join(",", oplist.Select(x => x.Pid))));
                    if (sendList.Count > 0)
                        return PromptView("存在要货订单，该订单不能取消");

                    List<AgentStockInfo> stockList = new AgentStock().GetList(string.Format(" uid={0} and pid in ({1}) ", user.Uid, string.Join(",", oplist.Select(x => x.Pid))));
                    foreach (var item in oplist)
                    {
                        AgentStockInfo agent = stockList.Find(x => x.Pid == item.Pid);
                        if (agent != null)
                        {
                            if (agent.Balance < item.BuyCount)
                            {
                                isTrue = false;
                                break;
                            }
                        }
                    }
                }
            }


            if (!isTrue)
                return PromptView("产品数量大于库存余额，订单不能取消");
            //代理订单取消

            foreach (var item in AllAgentOrders)
            {
                //取消订单
                AdminOrders.CancelOrder(ref partUserInfo, item, WorkContext.Uid, DateTime.Now);
                //更新分销资格状态
                string sqlStr = string.Format(" orderstate>={0} and orderstate<={1}  and uid={2} ", (int)OrderState.Confirmed, (int)OrderState.Completed, item.Uid);
                string sqlStr2 = string.Format(" orderstate>={0} and orderstate<={1} and uid={2} and orderamount>=15800 ", (int)OrderState.Confirmed, (int)OrderState.Completed, item.Uid);
                List<OrderInfo> UserOrderList = Orders.GetOrderListByWhere(sqlStr);
                List<OrderInfo> UserOrderList2 = Orders.GetOrderListByWhere(sqlStr2);
                if (UserOrderList.Count <= 0)
                    OrderUtils.UpdateFXUserSates(item.Uid, 0);
                if (UserOrderList2.Count <= 0 && UserOrderList.Count > 0)
                    OrderUtils.UpdateFXUserSates(item.Uid, 1);
            }
            foreach (var item in AllAgentOrders)
            {
                //如果已支付并支付金额大于0取消订单 要发起退款操作  非在线支付 取消订单应生成退款请求
                if ((item.OrderState >= (int)OrderState.Confirmed && item.OrderState <= (int)OrderState.Completed) && item.SurplusMoney > 0)
                {
                    if (item.PayMode == 1 && item.PaySystemName == "wechatpay")
                    {
                        PluginInfo PayPlugin = Plugins.GetPayPluginBySystemName(item.PaySystemName);
                        string oids = string.Join(",", AllAgentOrders.Select(x => x.Oid));
                        return RedirectToAction("ReFundForBatch", "AdminWeChat", new { oids = oids });
                        //return RedirectToAction("ReFundForBatch", "wechat", new RouteValueDictionary { { "oids", oids },{ "area" = "mob" } });
                    }
                    else if (item.PayMode == 1 && item.PaySystemName == "alipay")//支付宝退款需后台操作，此处只生成退款记录，
                    {
                        OrderRefunds.ApplyRefund(new OrderRefundInfo()
                        {
                            StoreId = item.StoreId,
                            StoreName = item.StoreName,
                            Oid = oid,
                            OSN = item.OSN,
                            Uid = item.Uid,
                            State = 0,
                            ApplyTime = DateTime.Now,
                            PayMoney = item.SurplusMoney,
                            RefundMoney = item.SurplusMoney,
                            RefundSN = "",
                            RefundFriendName = item.PayFriendName,
                            RefundSystemName = item.PaySystemName,
                            PayFriendName = item.PayFriendName,
                            PaySystemName = item.PaySystemName,
                            RefundTime = DateTime.Now,
                            PaySN = item.PaySN,
                            RefundTranSN = "",//记录退款流水号 
                            ReMark = Enum.GetName(typeof(OrderSource), item.OrderSource) + ",支付成功取消订单订单退款"
                        });
                        //创建订单处理
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = oid,
                            Uid = item.Uid,
                            RealName = "系统",
                            ActionType = (int)OrderActionType.Cancel,
                            ActionTime = DateTime.Now,
                            ActionDes = "订单取消,请等待系统退款,退款会在1-3个工作日内将退款返回至帐号中"
                        });
                    }
                    else if (item.PayMode == 1 && item.PaySystemName == "IPSpay")//环迅退款需后台操作，此处只生成退款记录，
                    {
                        OrderRefunds.ApplyRefund(new OrderRefundInfo()
                        {
                            StoreId = item.StoreId,
                            StoreName = item.StoreName,
                            Oid = oid,
                            OSN = item.OSN,
                            Uid = item.Uid,
                            State = 0,
                            ApplyTime = DateTime.Now,
                            PayMoney = item.SurplusMoney,
                            RefundMoney = item.SurplusMoney,
                            RefundSN = "",
                            RefundFriendName = item.PayFriendName,
                            RefundSystemName = item.PaySystemName,
                            PayFriendName = item.PayFriendName,
                            PaySystemName = item.PaySystemName,
                            RefundTime = DateTime.Now,
                            PaySN = item.PaySN,
                            RefundTranSN = "",//记录退款流水号 
                            ReMark = Enum.GetName(typeof(OrderSource), item.OrderSource) + ",支付成功取消订单订单退款"
                        });
                        //创建订单处理
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = oid,
                            Uid = item.Uid,
                            RealName = "系统",
                            ActionType = (int)OrderActionType.Cancel,
                            ActionTime = DateTime.Now,
                            ActionDes = "订单取消,请等待系统退款,退款会在1-3个工作日内将退款返回至帐号中"
                        });
                    }
                    else if (item.PayMode == 2)
                    {
                        OrderRefunds.ApplyRefund(new OrderRefundInfo()
                        {
                            StoreId = item.StoreId,
                            StoreName = item.StoreName,
                            Oid = oid,
                            OSN = item.OSN,
                            Uid = item.Uid,
                            State = 0,
                            ApplyTime = DateTime.Now,
                            PayMoney = item.SurplusMoney,
                            RefundMoney = item.SurplusMoney,
                            RefundSN = "",
                            RefundFriendName = item.PayFriendName,
                            RefundSystemName = item.PaySystemName,
                            PayFriendName = item.PayFriendName,
                            PaySystemName = item.PaySystemName,
                            RefundTime = DateTime.Now,
                            PaySN = item.PaySN,
                            RefundTranSN = "",//记录退款流水号 
                            ReMark = Enum.GetName(typeof(OrderSource), item.OrderSource) + ",支付成功取消订单订单退款"
                        });
                        //创建订单处理
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = oid,
                            Uid = item.Uid,
                            RealName = "系统",
                            ActionType = (int)OrderActionType.Cancel,
                            ActionTime = DateTime.Now,
                            ActionDes = "付款成功的订单取消,付款方式为银行汇款，请联系客服进行退款"
                        });
                    }
                }
                else
                {
                    CreateOrderAction(oid, OrderActionType.Cancel, "订单已取消");
                }
            }

            AddMallAdminLog("取消代理组合订单", "取消代理组合订单,订单ID为:" + string.Join(",", AllAgentOrders.Select(x => x.Oid)));
            return PromptView(Url.Action("orderinfo", new { oid = oid }), "取消代理组合订单成功");
        }

        #endregion

        #region 微商模块
        /// <summary>
        /// 拿货详情
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult StockFromDetail(int oid, int pid)
        {
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            List<OrderProductInfo> opInfoList = Orders.GetOrderProductList(oid);
            OrderProductInfo opInfo = opInfoList.Find(x => x.Pid == pid);
            StockFromDetailModel model = new StockFromDetailModel();
            model.UserInfo = Users.GetUserById(orderInfo.Uid);
            model.OrderInfo = orderInfo;
            model.OrderProductInfo = opInfo;
            if (opInfo.FromParentId1 > 0)
            {
                model.FromParentUser1 = Users.GetPartUserById(opInfo.FromParentId1);
                decimal parentuser1 = agentBll.SingleAgentPrice(model.FromParentUser1, pid);
                model.FromParentCount1 = (int)(opInfo.FromParentAmount1 / parentuser1);
            }
            if (opInfo.FromParentId2 > 0)
            {
                model.FromParentUser2 = Users.GetPartUserById(opInfo.FromParentId2);
                decimal parentuser2 = agentBll.SingleAgentPrice(model.FromParentUser2, pid);
                model.FromParentCount2 = (int)(opInfo.FromParentAmount2 / parentuser2);
            }
            if (opInfo.FromParentId3 > 0)
            {
                model.FromParentUser3 = Users.GetPartUserById(opInfo.FromParentId3);
                decimal parentuser3 = agentBll.SingleAgentPrice(model.FromParentUser3, pid);
                model.FromParentCount3 = (int)(opInfo.FromParentAmount3 / parentuser3);
            }
            if (opInfo.FromParentId4 > 0)
            {
                model.FromParentUser4 = Users.GetPartUserById(opInfo.FromParentId4);
                decimal parentuser4 = agentBll.SingleAgentPrice(model.FromParentUser4, pid);
                model.FromParentCount4 = (int)(opInfo.FromParentAmount4 / parentuser4);
            }
            model.deteilList = new AgentStockDetail().GetListByPage(string.Format(" ordercode='{0}' ", orderInfo.OSN), " creationdate asc ", 1, 15);
            return View(model);
        }
        /// <summary>
        ///  结算数据预览
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult SettlePreview(int oid)
        {
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            List<OrderProductInfo> opInfoList = Orders.GetOrderProductList(oid);

            SettlePreviewModel model = new SettlePreviewModel();
            model.UserInfo = Users.GetUserById(orderInfo.Uid);
            model.OrderInfo = orderInfo;
            model.OrderProductList = opInfoList;
            //订单结算数据预览
            model.OrderSettlePreview = AdminOrders.GetOrderSettlePreview(oid);
            model.deteilList = Account.GetAccountDetailListByWhere(string.Format(" ordercode='{0}' ", orderInfo.OSN));
            return View(model);
        }


        #region 生成要货单
        /// <summary>
        /// 生成要货单
        /// </summary>
        /// <returns></returns>
        public ActionResult CreatSendOrderAdmin(int oid, int pid)
        {
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);

            AgentStockInfo info = new AgentStock().GetModel(string.Format(" pid={0} and uid={1} ", pid, orderInfo.Uid));
            if (info == null)
                return PromptView("库存不存在");

            PartProductInfo productInfo = Products.GetPartProductById(info.Pid);
            if (productInfo == null)
                return PromptView("产品不存在或产品已下架");
            if (info.Balance == 0)
                return PromptView("库存为0，不需要生成");
            int count = (int)info.Balance;
            int remain = (int)info.Balance - count;
            info.Balance = remain;
            new AgentSendOrder().Add(new AgentSendOrderInfo()
            {
                CreationDate = DateTime.Now,
                Pid = info.Pid,
                Uid = info.Uid,
                SendOSN = "8" + DateTime.Now.ToString("yyyyMMddHHmmss") + Randoms.CreateRandomValue(3, true),
                SendCount = count,
                RegionId = orderInfo.RegionId,
                Address = orderInfo.Address,
                Consignee = orderInfo.Consignee,
                Mobile = orderInfo.Mobile
            });
            new AgentStock().Update(info);
            new AgentStockDetail().Add(new AgentStockDetailInfo()
            {
                CreationDate = DateTime.Now,
                Uid = info.Uid,
                Pid = info.Pid,
                DetailType = 3,
                OutAmount = count,
                CurrentBalance = remain,
                OrderCode = "",
                DetailDesc = string.Format("发货单，产品{0},数量{1}", productInfo.Name, count),
                FromUser = info.Uid,
                ToUser = 0
            });
            return PromptView("生成要货单成功");

        }
        #endregion
        #endregion
        /// <summary>
        /// 打印订单
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public ActionResult PrintOrder(int oid)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");

            PrintOrderModel model = new PrintOrderModel()
            {
                OrderInfo = orderInfo,
                RegionInfo = Regions.GetRegionById(orderInfo.RegionId),
                OrderProductList = AdminOrders.GetOrderProductList(oid),
            };

            return View(model);
        }

        #region 订单退货、退款

        /// <summary>
        /// 取消退货
        /// </summary>
        public ActionResult CancelReturn(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed && orderInfo.ReturnType != 1)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未提交退货申请，不能取消退货");
            OrderReturnInfo info = OrderReturn.GetOrderReturnByOid(orderInfo.Oid);
            if (info == null)
            {
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未提交退货申请，不能取消退货");
            }
            Orders.UpdateOrderReturnType(orderInfo.Oid, 0);//将订单returntype设为默认状态 0

            OrderReturn.UpdateOrderReturn(info.ReturnId, 0, DateTime.Now);
            CreateOrderAction(oid, OrderActionType.Return, "订单退货申请审核不通过。");
            return PromptView(Url.Action("orderinfo", new { oid = oid }), "取消退货成功");
        }
        /// <summary>
        /// 退货确认
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ConfirmReturn(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed && orderInfo.ReturnType != 1)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "当前订单不满足退货确认条件");

            ConfirmReturnModel model = new ConfirmReturnModel();

            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }
        /// <summary>
        /// 退货确认
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConfirmReturn(ConfirmReturnModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed && orderInfo.ReturnType != 1)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "当前订单不满足退货确认条件");
            OrderReturnInfo info = OrderReturn.GetOrderReturnByOid(orderInfo.Oid);
            if (info == null)
            {
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未提交退货申请，不能确认退货");
            }
            if (ModelState.IsValid)
            {
                PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);

                //判断是否满足红包退回
                if (orderInfo.OrderState == (int)OrderState.Completed)
                {
                    //存在推广产品配置退回99红包
                    List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
                    if (orderProductList.Exists(x => x.Pid == TypeHelper.StringToInt(WebConfigurationManager.AppSettings["ActiveProduct"], 0)))
                    {
                        List<AccountInfo> accountinfo = AccountUtils.GetAccountList(partUserInfo.Uid, partUserInfo.IsDirSaleUser, partUserInfo.DirSaleUid);
                        OrderProductInfo orderProductInfo = orderProductList.Find(x => x.Pid == TypeHelper.StringToInt(WebConfigurationManager.AppSettings["ActiveProduct"], 0));
                        if (accountinfo.Find(x => x.AccountId == (int)AccountType.红包账户).Banlance >= 99 * orderProductInfo.BuyCount)
                            AccountUtils.ReturnActiveHongBao(partUserInfo, orderInfo, 99 * orderProductInfo.BuyCount, partUserInfo.Uid, DateTime.Now);
                        else
                        {
                            return PromptView("订单当前不能取消,退回红包金额不够");
                        }
                    }
                }

                string sqlStr = string.Format(" orderstate>={0} and orderstate<={1}  and uid={2} ", (int)OrderState.Confirmed, (int)OrderState.Completed, orderInfo.Uid);
                string sqlStr2 = string.Format(" orderstate>={0} and orderstate<={1} and uid={2} and orderamount>=15800 ", (int)OrderState.Confirmed, (int)OrderState.Completed, orderInfo.Uid);
                List<OrderInfo> UserOrderList = Orders.GetOrderListByWhere(sqlStr);
                List<OrderInfo> UserOrderList2 = Orders.GetOrderListByWhere(sqlStr2);
                if (UserOrderList.Count <= 0)
                    OrderUtils.UpdateFXUserSates(partUserInfo.Uid, 0);
                if (UserOrderList2.Count <= 0 && UserOrderList.Count > 0)
                    OrderUtils.UpdateFXUserSates(partUserInfo.Uid, 1);


                Orders.UpdateOrderReturnTypeTo2(ref partUserInfo, orderInfo, WorkContext.Uid, DateTime.Now, 2); ;//将订单returntype设为审核通过2

                OrderReturn.ConfirmOrderReturn(info.ReturnId, 2, DateTime.Now, model.ReturnShipFee, model.ReturnShipDesc == null ? "" : model.ReturnShipDesc);

                AddMallAdminLog("退货确认", "退货确认，订单ID为:" + oid + "承担运费：" + model.ReturnShipFee + ",运费说明" + model.ReturnShipDesc == null ? "" : model.ReturnShipDesc + ",操作人：" + WorkContext.Uid);
                CreateOrderAction(oid, OrderActionType.Return, "订单退货申请已审核通过，请联系客服获取寄送地址");
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "退货确认成功");
            }

            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }
        /// <summary>
        /// 退货确认--代理订单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult AgentConfirmReturn(ConfirmReturnModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed && orderInfo.ReturnType != 1)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "当前订单不满足退货确认条件");

            if (ModelState.IsValid)
            {
                PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);

                List<OrderInfo> AllAgentOrders = Orders.GetOrderListByWhere(string.Format(" (oid ={0} OR oid={1} OR oid={2} ) ", orderInfo.Oid, orderInfo.MainOid, orderInfo.SubOid));
                //代理订单取消

                foreach (var item in AllAgentOrders)
                {
                    OrderReturnInfo info = OrderReturn.GetOrderReturnByOid(item.Oid);

                    string sqlStr = string.Format(" orderstate>={0} and orderstate<={1}  and uid={2} ", (int)OrderState.Confirmed, (int)OrderState.Completed, item.Uid);
                    string sqlStr2 = string.Format(" orderstate>={0} and orderstate<={1} and uid={2} and orderamount>=15800 ", (int)OrderState.Confirmed, (int)OrderState.Completed, item.Uid);
                    List<OrderInfo> UserOrderList = Orders.GetOrderListByWhere(sqlStr);
                    List<OrderInfo> UserOrderList2 = Orders.GetOrderListByWhere(sqlStr2);
                    if (UserOrderList.Count <= 0)
                        OrderUtils.UpdateFXUserSates(partUserInfo.Uid, 0);
                    if (UserOrderList2.Count <= 0 && UserOrderList.Count > 0)
                        OrderUtils.UpdateFXUserSates(partUserInfo.Uid, 1);
                    //将订单returntype设为审核通过2
                    Orders.UpdateOrderReturnTypeTo2(ref partUserInfo, item, WorkContext.Uid, DateTime.Now, 2); ;

                    OrderReturn.ConfirmOrderReturn(info.ReturnId, 2, DateTime.Now, model.ReturnShipFee, model.ReturnShipDesc == null ? "" : model.ReturnShipDesc);
                    CreateOrderAction(oid, OrderActionType.Return, "订单退货申请已审核通过，请联系客服获取寄送地址");
                }

                AddMallAdminLog("组合退货确认", "组合退货确认，订单ID为:" + string.Join(",", AllAgentOrders.Select(x => x.Oid)) + "承担运费：" + model.ReturnShipFee + ",运费说明" + model.ReturnShipDesc == null ? "" : model.ReturnShipDesc + ",操作人：" + WorkContext.Uid);

                return PromptView(Url.Action("orderinfo", new { oid = oid }), "退货确认成功");
            }

            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 订单退货列表
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="osn">订单编号</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ActionResult ReturnList(string storeName, string osn, int state = 1, int storeId = -1, int pageSize = 15, int pageNumber = 1)
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            if (!string.IsNullOrEmpty(HttpContext.Request.Form["IsOutPut"]) && HttpContext.Request.Form["IsOutPut"].Trim() == "1")
            {
                //ViewData["IsOutPut"] = "0";
                return ReturnOutPutOrder(osn, state, storeId);
            }
            string condition = OrderReturn.GetOrderReturnListCondition(storeId, osn, state);
            PageModel pageModel = new PageModel(pageSize, pageNumber, OrderReturn.GetOrderReturnCount(condition));
            List<OrderReturnInfo> returnList = OrderReturn.GetOrderReturnList(pageModel.PageSize, pageModel.PageNumber, condition);

            OrderReturnListModel model = new OrderReturnListModel()
            {
                state = state,
                OrderReturnList = returnList,
                PageModel = pageModel,
                StoreId = storeId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "选择店铺" : storeName,
                OSN = osn
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&storeId={3}&storeName={4}&OSN={5}&state={6}",
                                                          Url.Action("ReturnList"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          storeId, storeName, osn, state));


            return View(model);
        }


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
        public ActionResult ReturnOutPutOrder(string osn, int state, int storeId = -1)
        {
            //if (storeId == -1)
            //    storeId = WorkContext.PartUserInfo.StoreId;
            string condition = " where 1=1 ";
            if (storeId > 0)
            {
                if (WorkContext.PartUserInfo.StoreId > 0)
                    condition += "|o.storeid = " + WorkContext.PartUserInfo.StoreId + "";
                if (storeId > 0 && WorkContext.PartUserInfo.StoreId <= 0)
                    condition += "|o.storeid = " + storeId + "";
            }
            if (!string.IsNullOrEmpty(osn.Trim()))
                condition += "|o.osn='" + osn.Trim() + "'";

            if (state > 0)
                condition += "|o.state = " + state + "";

            condition += " | o.[storeid] in ( select storeid from hlh_stores where mallsource in(0,2,3) ) ";
            if (!string.IsNullOrEmpty(condition))
                condition = condition.Replace("|", " and ");

            string sqlText = string.Empty;

            sqlText = @"SELECT  o.storename 店铺名称,
                                RTRIM(o.osn) 订单编号,
                                CONVERT(varchar(100),o.creationdate,120) 创建时间,
                                CONVERT(varchar(100),o.lastmodifity,120) 最后修改时间,
                                ISNULL((SELECT RTRIM(username) username FROM dbo.hlh_users WHERE uid=o.uid),'')  会员编号,
                                ISNULL((SELECT RTRIM(mobile) FROM dbo.hlh_users WHERE uid=o.uid),'')  会员手机号,
                                ISNULL((SELECT  RTRIM(realname) FROM dbo.hlh_userdetails WHERE uid=o.uid),'') 会员姓名,
                                CASE o.state WHEN 1 THEN '待审核' WHEN 2 THEN '审核通过' WHEN 3 THEN '已收货确认'  ELSE '未知' END 退货状态,
                                o.returndesc 退货原因,
                                o.returnshipfee 退货运费,
                                o.returnshipdesc 退货物流备注
                                FROM hlh_orderreturn o INNER JOIN hlh_orderproducts op ON o.oid=op.oid " + condition + " ORDER BY o.creationdate";

            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "退货列表_" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 退货收货确认
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ConfirmReceiveReturn(int oid = -1, int returnid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            OrderReturnInfo info = OrderReturn.GetOrderReturnByOid(orderInfo.Oid);
            if (info == null)
                return PromptView("订单不能进行退货收货确认");
            if (orderInfo.OrderState != (int)OrderState.Completed && orderInfo.ReturnType != 2)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "当前订单不满足退货收货确认条件");

            ConfirmReceiveReturnModel model = new ConfirmReceiveReturnModel();
            model.ReturnShipFee = info.ReturnShipFee;
            model.ReturnShipDesc = info.ReturnShipDesc;
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }
        /// <summary>
        /// 退货收货确认
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConfirmReceiveReturn(ConfirmReceiveReturnModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            OrderReturnInfo info = OrderReturn.GetOrderReturnByOid(orderInfo.Oid);
            if (info == null)
                return PromptView("订单不能进行退货收货确认");
            if (orderInfo.OrderState != (int)OrderState.Returned && orderInfo.ReturnType != 1)
                return PromptView(Url.Action("ReturnList", new { oid = oid }), "当前订单不满足退货确认条件");

            if (ModelState.IsValid)
            {
                Orders.UpdateOrderReturnType(orderInfo.Oid, 3);//将订单退货类型refundtype设为3

                OrderReturn.ConfirmOrderReturn2(info.ReturnId, 3, DateTime.Now, info.ReturnShipFee, info.ReturnShipDesc, model.NewReturnShipFee, model.NewReturnShipDesc == null ? "" : model.NewReturnShipDesc);
                AddMallAdminLog("退货收货确认", "退货收货确认，订单ID为:" + oid + ",操作人：" + WorkContext.Uid);
                OrderRefunds.ApplyRefund(new OrderRefundInfo()
                {
                    StoreId = orderInfo.StoreId,
                    StoreName = orderInfo.StoreName,
                    Oid = oid,
                    OSN = orderInfo.OSN,
                    Uid = orderInfo.Uid,
                    State = 0,
                    ApplyTime = DateTime.Now,
                    PayMoney = orderInfo.SurplusMoney,
                    RefundMoney = orderInfo.SurplusMoney - (info.ReturnShipFee + model.NewReturnShipFee),
                    RefundSN = "",
                    RefundFriendName = orderInfo.PayFriendName,
                    RefundSystemName = orderInfo.PaySystemName,
                    PayFriendName = orderInfo.PayFriendName,
                    PaySystemName = orderInfo.PaySystemName,
                    RefundTime = DateTime.Now,
                    PaySN = orderInfo.PaySN,
                    RefundTranSN = "",//记录退款流水号 
                    ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",退货订单退款"
                });


                CreateOrderAction(oid, OrderActionType.Return, "订单退货已收到，请等待系统审核退款");
                return PromptView(Url.Action("ReturnList", new { oid = oid }), "退货确认收货成功");

            }

            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        #region 退款
        /// <summary>
        /// 订单退款列表
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="osn">订单编号</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ActionResult RefundList(string storeName, string paySystemName, string osn, int state = 0, int storeId = -1, int pageSize = 15, int pageNumber = 1)
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            if (!string.IsNullOrEmpty(HttpContext.Request.Form["IsOutPut"]) && HttpContext.Request.Form["IsOutPut"].Trim() == "1")
            {
                //ViewData["IsOutPut"] = "0";
                return RefundOutPutOrder(paySystemName, osn, state, storeId);
            }
            string condition = AdminOrderRefunds.GetOrderRefundListCondition(storeId, osn, state, paySystemName);
            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminOrderRefunds.GetOrderRefundCount(condition));
            List<OrderRefundInfo> refundList = AdminOrderRefunds.GetOrderRefundList(pageModel.PageSize, pageModel.PageNumber, condition);

            OrderRefundListModel model = new OrderRefundListModel()
            {
                OrderRefundList = refundList,
                PageModel = pageModel,
                StoreId = storeId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "选择店铺" : storeName,
                paySystemName = paySystemName,
                OSN = osn
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&storeId={3}&storeName={4}&OSN={5}&paySystemName={6}",
                                                          Url.Action("refundlist"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          storeId, storeName, osn, paySystemName));


            return View(model);
        }

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
        public ActionResult RefundOutPutOrder(string paySystemName, string osn, int state, int storeId = -1)
        {
            //if (storeId == -1)
            //    storeId = WorkContext.PartUserInfo.StoreId;
            string condition = " where 1=1 ";
            if (storeId > 0)
            {
                if (WorkContext.PartUserInfo.StoreId > 0)
                    condition += "|o.storeid = " + WorkContext.PartUserInfo.StoreId + "";
                if (storeId > 0 && WorkContext.PartUserInfo.StoreId <= 0)
                    condition += "|o.storeid = " + storeId + "";
            }
            if (!string.IsNullOrEmpty(osn.Trim()))
                condition += "|o.osn='" + osn.Trim() + "'";

            if (!string.IsNullOrEmpty(paySystemName.Trim()))
                condition += "|o.paySystemName='" + paySystemName.Trim() + "'";

            if (state > -1)
                condition += "|o.state = " + state + "";

            condition += " | o.[storeid] in ( select storeid from hlh_stores where mallsource in(0,2,3) ) ";
            if (!string.IsNullOrEmpty(condition))
                condition = condition.Replace("|", " and ");

            string sqlText = string.Empty;

            sqlText = @"SELECT  o.storename 店铺名称,
                                RTRIM(o.osn) 订单编号,
                                CONVERT(varchar(100),o.applytime,120) 提交时间,
                                ISNULL((SELECT RTRIM(username) username FROM dbo.hlh_users WHERE uid=o.uid),'')  会员编号,
                                ISNULL((SELECT RTRIM(mobile) FROM dbo.hlh_users WHERE uid=o.uid),'')  会员手机号,
                                ISNULL((SELECT  RTRIM(realname) FROM dbo.hlh_userdetails WHERE uid=o.uid),'') 会员姓名,
                                o.paymoney 支付金额,
                                o.refundmoney 退款金额,
                                o.refundsn 退款单号,
                                o.refundtransn 退款流水号,
                                o.refundfriendname 退款方式,
                                o.refundtime 退款时间,
                                o.paysn 支付时间,
                                o.payfriendname 支付方式,
                                o.remark 操作备注,
                                CASE o.state WHEN 0 THEN '未处理' WHEN 1 THEN '已处理'  ELSE '未知' END 退款状态
                                FROM hlh_orderrefunds o " + condition + " ORDER BY o.applytime";

            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "退款列表_" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 确认退款 在线支付原方式调用支付平台退款接口退款  线下支付人工退款后确定改变状态
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmRefund(int oid, int refundId)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);

            if (orderInfo == null || refundId < 1)
                return PromptView(Url.Action("RefundList"), "订单不存在");
            OrderRefundInfo info = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
            if (orderInfo.PaySN.Length < 1 && orderInfo.SurplusMoney <= 0 && info.RefundMoney <= 0)
                return PromptView(Url.Action("RefundList"), "订单当前不能退款");
            if (info.RefundTranSN.Length > 0)
            {
                return PromptView(Url.Action("RefundList"), "订单已经退款");
            }

            PartUserInfo userInfo = Users.GetPartUserById(orderInfo.Uid);
            if (orderInfo.PayMode == 1 && orderInfo.PaySystemName == "alipay")
            {
                //bool flag = OrderUtils.DiscartFXUser(userInfo);
                //if (flag)
                //{
                PluginInfo PayPlugin = Plugins.GetPayPluginBySystemName(orderInfo.PaySystemName);
                return RedirectToAction("ReFund", ((IPayPlugin)PayPlugin.Instance).PayController, new RouteValueDictionary { { "oid", orderInfo.Oid }, { "area", "" } });
                //}
            }
            if (orderInfo.PayMode == 1 && orderInfo.OrderState == (int)OrderState.Returned && orderInfo.PaySystemName == "wechatpay")
            {
                //bool flag = OrderUtils.DiscartFXUser(userInfo);
                //if (flag)
                //{
                PluginInfo PayPlugin = Plugins.GetPayPluginBySystemName(orderInfo.PaySystemName);
                return RedirectToAction("ReFund", ((IPayPlugin)PayPlugin.Instance).ConfigController, new RouteValueDictionary { { "oid", orderInfo.Oid } });
                //}
                //return Redirect("/AdminLaKaLa/ReFund?oid=" + orderInfo.Oid);
                // return RedirectToAction("ReFund", "AdminLaKaLa", new RouteValueDictionary { { "oid", orderInfo.Oid } });
            }
            if (orderInfo.PayMode == 1 && orderInfo.OrderState == (int)OrderState.Cancelled && orderInfo.PaySystemName == "wechatpay" && orderInfo.PaySN != "")
            {
                if (string.IsNullOrEmpty(info.RefundTranSN) && info.ReMark.Contains("基本账户余额不足"))
                {
                    PluginInfo PayPlugin = Plugins.GetPayPluginBySystemName(orderInfo.PaySystemName);
                    return RedirectToAction("ReFund", ((IPayPlugin)PayPlugin.Instance).ConfigController, new RouteValueDictionary { { "oid", orderInfo.Oid } });
                }
            }
            if (orderInfo.PayMode == 2 || orderInfo.PaySystemName == "IPSpay")
                return RedirectToAction("ConfirmRefundForBank", new RouteValueDictionary { { "refundId", refundId }, { "oid", orderInfo.Oid } });

            //AdminOrderRefunds.RefundOrder(refundId, string.Empty, orderInfo.PaySystemName, orderInfo.PayFriendName, DateTime.Now, string.Empty, Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",退货订单退款");
            return PromptView(Url.Action("RefundList"), "退款成功");
        }

        /// <summary>
        /// 确认银行汇款退款--输入退款汇款单号/环迅退款-手动退
        /// </summary>
        [HttpGet]
        public ActionResult ConfirmRefundForBank(int refundId, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);

            if (orderInfo == null || refundId < 1)
                return PromptView(Url.Action("RefundList"), "订单不存在");
            OrderRefundInfo info = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
            if (orderInfo.PaySN.Length < 1 && orderInfo.SurplusMoney <= 0 && info.RefundMoney <= 0)
                return PromptView(Url.Action("RefundList"), "订单当前不能退款");
            if (info.RefundTranSN.Length > 0)
                return PromptView(Url.Action("RefundList"), "订单已经退款");

            ConfirmRefundForBanKModel model = new ConfirmRefundForBanKModel();


            return View(model);
        }

        /// <summary>
        /// 确认银行汇款退款--输入退款汇款单号/环迅退款-手动退
        /// </summary>
        [HttpPost]
        public ActionResult ConfirmRefundForBank(ConfirmRefundForBanKModel model, int refundId, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);

            if (orderInfo == null || refundId < 1)
                return PromptView(Url.Action("RefundList"), "订单不存在");
            OrderRefundInfo info = AdminOrderRefunds.GetRefundInfoByOSN(orderInfo.OSN);
            if (orderInfo.PaySN.Length < 1 && orderInfo.SurplusMoney <= 0 && info.RefundMoney <= 0)
                return PromptView(Url.Action("RefundList"), "订单当前不能退款");
            if (info.RefundTranSN.Length > 0)
                return PromptView(Url.Action("RefundList"), "订单已经退款");

            if (ModelState.IsValid)
            {
                if (orderInfo.PayMode == 2 && orderInfo.OrderState == (int)OrderState.Returned)
                    Orders.UpdateOrderForRefund(orderInfo.Oid);

                AdminOrderRefunds.UpdateRefundTranSN(refundId, model.RefundTranSN);

                return PromptView(Url.Action("RefundList"), "更新退款信息成功");
            }
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 导出环迅退款列表
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="consignee"></param>
        /// <param name="orderState"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult OutPutOrderRefundForIPS()
        {
            string sqlText = string.Empty;

            sqlText = @"select '' 序号,paysn IPS订单号,refundmoney 申请退款金额,remark 备注 from hlh_orderrefunds where state=0 and paysystemname='IPSpay'   ORDER BY applytime asc  ";

            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "环迅退款列表-" + DateTime.Now.ToString("yyyyMMdd")));
            //Response.AddHeader("content-disposition", "attachment;filename=Order.xls");
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


        #endregion

        private void CreateOrderAction(int oid, OrderActionType orderActionType, string actionDes)
        {
            OrderActions.CreateOrderAction(new OrderActionInfo()
            {
                Oid = oid,
                Uid = WorkContext.Uid,
                RealName = AdminUsers.GetUserDetailById(WorkContext.Uid).RealName,
                ActionType = (int)orderActionType,
                ActionTime = DateTime.Now,
                ActionDes = actionDes
            });
        }

        #endregion

        #region 订单换货


        /// <summary>
        /// 取消换货
        /// </summary>
        public ActionResult CancelChange(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed && orderInfo.ChangeType != 1)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未提交换货申请，不能取消换货");
            OrderChangeInfo info = OrderChange.GetChangeOrderByOid(orderInfo.Oid);
            if (info == null)
            {
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未提交换货申请，不能取消换货");
            }
            Orders.UpdateOrderChangeType(orderInfo.Oid, 0);//将订单changetype设为默认状态 0

            OrderChange.UpdateOrderChange(info.ChangeId, 0, DateTime.Now, info.ChangeDesc);

            return PromptView(Url.Action("orderinfo", new { oid = oid }), "取消换货成功");
        }
        /// <summary>
        /// 换货确认，审核通过
        /// </summary>
        /// <returns></returns>

        public ActionResult ConfirmChange(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (orderInfo.OrderState != (int)OrderState.Completed && orderInfo.ChangeType != 1)
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未提交换货申请，不能确认换货");
            OrderChangeInfo info = OrderChange.GetChangeOrderByOid(orderInfo.Oid);
            if (info == null)
            {
                return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未提交换货申请，不能确认换货");
            }
            Orders.UpdateOrderChangeType(orderInfo.Oid, 2);//将订单changetype设为审核通过状态 2;

            OrderChange.UpdateOrderChange(info.ChangeId, 2, DateTime.Now, info.ChangeDesc);

            CreateOrderAction(oid, OrderActionType.Change, "订单换货申请已审核通过，请联系客服获取寄送地址");
            return PromptView(Url.Action("orderinfo", new { oid = oid }), "换货确认成功");


        }

        /// <summary>
        /// 订单换货列表
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="state"></param>
        /// <param name="storeId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public ActionResult OrderChangeList(string storeName, string osn, int state = -1, int storeId = -1, int pageSize = 15, int pageNumber = 1)
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            string condition = OrderChange.GetOrderChangeListCondition(storeId, osn, state);
            PageModel pageModel = new PageModel(pageSize, pageNumber, OrderChange.GetOrderChangeCount(condition));

            List<OrderChangeInfo> changeList = OrderChange.GetOrderChangeList(pageModel.PageSize, pageModel.PageNumber, condition);

            OrderChangeListModel model = new OrderChangeListModel()
            {
                OrderChangeList = changeList,
                PageModel = pageModel,
                StoreId = storeId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "选择店铺" : storeName,
                OSN = osn
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&storeId={3}&storeName={4}&OSN={5}&state={6}",
                                                          Url.Action("orderchangelist"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          storeId, storeName, osn, state));


            return View(model);
        }

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
        public ActionResult ChangeOutPutOrder(string osn, int state, int storeId = -1)
        {
            //if (storeId == -1)
            //    storeId = WorkContext.PartUserInfo.StoreId;
            string condition = " where 1=1 ";
            if (storeId > 0)
            {
                if (WorkContext.PartUserInfo.StoreId > 0)
                    condition += "|o.storeid = " + WorkContext.PartUserInfo.StoreId + "";
                if (storeId > 0 && WorkContext.PartUserInfo.StoreId <= 0)
                    condition += "|o.storeid = " + storeId + "";
            }
            if (!string.IsNullOrEmpty(osn.Trim()))
                condition += "|o.osn='" + osn.Trim() + "'";

            if (state > 0)
                condition += "|o.state = " + state + "";

            condition += " | o.[storeid] in ( select storeid from hlh_stores where mallsource in(0,2,3) ) ";
            if (!string.IsNullOrEmpty(condition))
                condition = condition.Replace("|", " and ");

            string sqlText = string.Empty;

            sqlText = @"SELECT  o.storename 店铺名称,
                                RTRIM(o.osn) 订单编号,
                                CONVERT(varchar(100),o.creationdate,120) 创建时间,
                                CONVERT(varchar(100),o.lastmodifity,120) 最后修改时间,
                                ISNULL((SELECT RTRIM(username) username FROM dbo.hlh_users WHERE uid=o.uid),'')  会员编号,
                                ISNULL((SELECT RTRIM(mobile) FROM dbo.hlh_users WHERE uid=o.uid),'')  会员手机号,
                                ISNULL((SELECT  RTRIM(realname) FROM dbo.hlh_userdetails WHERE uid=o.uid),'') 会员姓名,
                                o.changedesc 换货原因,
                                (SELECT s.consignee+'  ' + s.mobile+'  ' + (SELECT rtrim(provincename)+RTRIM(cityname)+RTRIM(name) FROM                                             dbo.hlh_regions WHERE regionid=s.regionid)+s.address   FROM dbo.hlh_shipaddresses s WHERE said=o.said) 收货信息,
                                o.changeshipsn 发货单号,
                                o.changeshipconame 发货快递,
                                CONVERT(varchar(100),o.changeshiptime,120) 发货时间,
                                CASE o.state WHEN 0 THEN '审核不通过' WHEN 1 THEN '待审核' WHEN 2 THEN '审核通过' WHEN 3 THEN '收到换货，待发货'                                   WHEN 4 THEN '换货发货完成' WHEN 5 THEN '换货确认收货'  ELSE '未知' END 换货状态
                                FROM hlh_orderchange o" + condition + " ORDER BY o.creationdate";

            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "换货列表_" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 换货收货确认
        /// </summary>
        /// <param name="model"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult ConfirmReceiveChange(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            OrderChangeInfo info = OrderChange.GetChangeOrderByOid(orderInfo.Oid);
            if (info == null)
                return PromptView("订单不能进行换货收货确认");
            if (orderInfo.OrderState != (int)OrderState.Completed && orderInfo.ChangeType != 2)
                return PromptView(Url.Action("OrderChangeList", new { oid = oid }), "当前订单不满足换货确认收货条件");

            Orders.UpdateOrderChangeType(orderInfo.Oid, 3);//将订单换货类型refundtype设为3

            OrderChange.UpdateOrderChange(info.ChangeId, 3, DateTime.Now, info.ChangeDesc);
            AddMallAdminLog("换货收货确认", "换货收货确认，订单ID:" + oid + "订单号:" + orderInfo.OSN + ",操作人：" + WorkContext.Uid);

            CreateOrderAction(oid, OrderActionType.Change, "订单换货已收到，请等待系统发货");
            return PromptView(Url.Action("OrderChangeList", new { oid = oid }), "退货确认收货成功");

        }

        /// <summary>
        /// 换货发货
        /// </summary>
        [HttpGet]
        public ActionResult ChangeOrderSend(int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView(Url.Action("OrderChangeList"), "订单不存在");
            OrderChangeInfo info = OrderChange.GetChangeOrderByOid(oid);
            if (info == null)
                return PromptView(Url.Action("OrderChangeList"), "订单未申请换货");
            if (info.State < 3)
                return PromptView(Url.Action("OrderChangeList"), "订单换货未收到货，请先确认收到换货");
            if (info.State == 4)
                return PromptView(Url.Action("OrderChangeList"), "订单换货已完成");
            //if (orderInfo.OrderState != (int)OrderState.PreProducting)
            //    return PromptView(Url.Action("orderinfo", new { oid = oid }), "订单还未完成备货,不能发货");

            ChangeOrderSendModel model = new ChangeOrderSendModel();
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        /// <summary>
        /// 换货发货
        /// </summary>
        [HttpPost]
        public ActionResult ChangeOrderSend(ChangeOrderSendModel model, int oid = -1)
        {
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView(Url.Action("OrderChangeList"), "订单不存在");
            OrderChangeInfo info = OrderChange.GetChangeOrderByOid(oid);
            if (info == null)
                return PromptView(Url.Action("OrderChangeList"), "订单未申请换货");
            if (info.State < 3)
                return PromptView(Url.Action("OrderChangeList"), "订单换货未收到货，请先确认收到换货");
            if (info.State == 4)
                return PromptView(Url.Action("OrderChangeList"), "订单换货已完成");

            ShipCompanyInfo shipCompanyInfo = ShipCompanies.GetShipCompanyById(model.ShipCoId);
            if (shipCompanyInfo == null)
                ModelState.AddModelError("ShipCoId", "请选择配送公司");

            if (ModelState.IsValid)
            {
                //更新订单表changeType状态
                Orders.UpdateOrderChangeType(orderInfo.Oid, 4);//将订单换货类型refundtype设为4
                //更新orderchange表换货物流信息以及已处理
                OrderChange.UpdateOrderChangeForSend(info.ChangeId, 4, DateTime.Now, model.ShipSN, model.ShipCoId, model.ShipCoName);

                CreateOrderAction(oid, OrderActionType.ChangeSend, "您换货的订单的已经发货,发货方式为:" + shipCompanyInfo.Name + ",物流单号：" + model.ShipSN);
                AddMallAdminLog("换货发货", "换货发货,原订单ID为:" + oid + ",原订单号" + orderInfo.OSN);
                return PromptView(Url.Action("OrderChangeList"), "换货发货成功");
            }
            ViewData["orderInfo"] = orderInfo;
            return View(model);
        }

        #endregion

        #region 订单部分退款申请
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SingleProductRefund(int oid, int pid, int returnCount)
        {
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(oid);
            if (orderProductList.Count == 1 && orderProductList.FirstOrDefault().BuyCount == returnCount)
                return PromptView("整单退款请勿申请部分退款");

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();

            OrderProductInfo returnProduct = orderProductList.Find(x => x.Pid == pid);
            if (returnProduct == null)
                return PromptView("订单产品不存在");
            //if (returnCount >= returnProduct.BuyCount)
            //    return PromptView("退货数量不能大于订单产品数量");

            decimal returnAmount = returnProduct.DiscountPrice * returnCount;
            if (returnAmount > orderInfo.SurplusMoney)
                return PromptView("退款金额不能大于等于订单金额");

            if (StringHelper.StrContainsForNum(WebHelper.GetConfigSettings("CoffeeQuanPid"), returnProduct.Pid.ToString()))
                return PromptView("汇购卡退货请在用户管理-汇购卡菜单操作");

            if (orderInfo.OrderState != (int)OrderState.Completed && orderInfo.ReturnType == 1)
                return PromptView("订单当前不满足退货条件，不能退货");

            OrderInfo newOrder = TransExpV2<OrderInfo, OrderInfo>.Trans(orderInfo);
            newOrder.Oid = 0;
            newOrder.OSN = Orders.GenerateOSN(newOrder.StoreId, newOrder.Uid, newOrder.RegionId, DateTime.Now, newOrder.PayMode.ToString(), newOrder.OrderSource.ToString(), newOrder.OrderAmount);
            newOrder.ParentId = orderInfo.Oid;
            newOrder.OrderState = 140;
            newOrder.ProductAmount = returnAmount;
            newOrder.OrderAmount = returnAmount;
            newOrder.SurplusMoney = returnAmount;
            newOrder.ShipFee = 0;
            newOrder.SettleState = 2;
            //添加订单
            int newoid = 0;
            if (orderProductList.Count == 1)
                newoid = new OrderReturn().CreateAdminReturnOrder(newOrder, 1, returnProduct, returnAmount, returnCount);
            else
                newoid = new OrderReturn().CreateAdminReturnOrder(newOrder, 2, returnProduct, returnAmount, returnCount);
            if (newoid > 0)
            {
                newOrder.Oid = newoid;
                //创建订单处理
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = newOrder.Oid,
                    Uid = newOrder.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.Return,
                    ActionTime = DateTime.Now,
                    ActionDes = "系统处理退货订单，关联订单号：" + orderInfo.OSN
                });

                #region 记录合并订单
                //记录拆分退款合并记录，退款时统计付款总金额用到，原订单存在合并支付，修改原订单合并金额记录，并添加新订单记录
                List<OrderInfo> orderList = Orders.GetOrderListByWhere(string.Format(" oid IN ({0}) ", orderInfo.Oid + "," + newOrder.Oid));
                MergePayOrderInfo MPOInfo = MergePayOrder.GetMergeOrderBySubOid(orderInfo.Oid);
                if (MPOInfo == null)
                {
                    List<MergePayOrderInfo> merOrderList = new List<MergePayOrderInfo>();
                    foreach (OrderInfo info in orderList)
                    {
                        MergePayOrderInfo payOrder = new MergePayOrderInfo();
                        payOrder.CreationDate = DateTime.Now;
                        payOrder.MergeOSN = orderInfo.OSN;
                        payOrder.SubOid = info.Oid;
                        payOrder.SubOSN = info.OSN;
                        payOrder.SubOrderAmount = info.SurplusMoney;
                        payOrder.Uid = info.Uid;
                        payOrder.StoreId = info.StoreId;
                        payOrder.StoreName = info.StoreName;
                        payOrder.PayFriendName = info.PayFriendName;
                        payOrder.PaySystemName = info.PaySystemName;
                        merOrderList.Add(payOrder);
                    }
                    MergePayOrder.CreateMergePayOrder(merOrderList);
                }
                else
                {
                    MergePayOrder.UpdateMergeAmount_SingleReturn(MPOInfo.MergeId, orderInfo.SurplusMoney - returnAmount);
                    List<MergePayOrderInfo> merOrderList2 = new List<MergePayOrderInfo>();
                    MergePayOrderInfo payOrder = new MergePayOrderInfo();
                    payOrder.CreationDate = DateTime.Now;
                    payOrder.MergeOSN = MPOInfo.MergeOSN;
                    payOrder.SubOid = newOrder.Oid;
                    payOrder.SubOSN = newOrder.OSN;
                    payOrder.SubOrderAmount = newOrder.SurplusMoney;
                    payOrder.Uid = newOrder.Uid;
                    payOrder.StoreId = newOrder.StoreId;
                    payOrder.StoreName = newOrder.StoreName;
                    payOrder.PayFriendName = newOrder.PayFriendName;
                    payOrder.PaySystemName = newOrder.PaySystemName;
                    merOrderList2.Add(payOrder);
                    MergePayOrder.CreateMergePayOrder(merOrderList2);
                }
                PartUserInfo partUserInfo = Users.GetPartUserById(newOrder.Uid);
                //创建负数PV冲抵
                List<int> noToQDStoreIds = new List<int>();
                noToQDStoreIds.Add(WebHelper.GetConfigSettingsInt("SRCStoreId"));
                noToQDStoreIds.Add(WebHelper.GetConfigSettingsInt("AgentStoreId"));
                noToQDStoreIds.Add(WebHelper.GetConfigSettingsInt("AgentSuitStoreId"));
                noToQDStoreIds.Add(WebHelper.GetConfigSettingsInt("AgentSystemStoreId"));
                noToQDStoreIds.Add(WebHelper.GetConfigSettingsInt("ChongZhiStore"));

                if (partUserInfo.IsDirSaleUser && orderInfo.CashDiscount <= 0 && !noToQDStoreIds.Exists(x => x == newOrder.StoreId) && returnProduct.ProductPV != 0)
                {
                    OrderUtils.CreateQDOrder(orderInfo, partUserInfo, 1);
                }

                #endregion
                AdminOrders.ReturnApply(newOrder, newOrder.Uid, DateTime.Now, "后台申请部分退货");

                return PromptView("创建退款订单成功");
            }
            else
            {
                return PromptView("创建退款订单失败");
            }


            //OrderDetailForRefundModel model = new OrderDetailForRefundModel();
            //model.OrderInfo = orderInfo;
            //model.OrderProductList = orderProductList;
            //model.OrderActionList = OrderActions.GetOrderActionList(oid);
            //model.UserInfo = Users.GetUserById(orderInfo.Uid);
            //return View(model);
        }


        public static class TransExpV2<TIn, TOut>
        {

            private static readonly Func<TIn, TOut> cache = GetFunc();
            private static Func<TIn, TOut> GetFunc()
            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
                List<MemberBinding> memberBindingList = new List<MemberBinding>();

                foreach (var item in typeof(TOut).GetProperties())
                {
                    if (!item.CanWrite)
                        continue;

                    MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                    MemberBinding memberBinding = Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }

                MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
                Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

                return lambda.Compile();
            }

            public static TOut Trans(TIn tIn)
            {
                return cache(tIn);
            }

        }
        #endregion
    }
}
