using System;
using System.Web;
using System.Data;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台报表统计控制器类
    /// </summary>
    public partial class StatController : BaseMallAdminController
    {
        /// <summary>
        /// 在线用户列表
        /// </summary>
        /// <param name="provinceId">省id</param>
        /// <param name="cityId">市id</param>
        /// <param name="regionId">区/县id</param>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult OnlineUserList(string sortColumn, string sortDirection, int provinceId = -1, int cityId = -1, int regionId = -1, int pageNumber = 1, int pageSize = 15)
        {
            int locationType = 0, locationId = 0;
            if (regionId > 0)
            {
                locationType = 2;
                locationId = regionId;
            }
            else if (cityId > 0)
            {
                locationType = 1;
                locationId = cityId;
            }
            else if (provinceId > 0)
            {
                locationType = 0;
                locationId = provinceId;
            }

            string sort = OnlineUsers.GetOnlineUserListSort(sortColumn, sortDirection);
            PageModel pageModel = new PageModel(pageSize, pageNumber, OnlineUsers.GetOnlineUserCount(locationType, locationId));

            OnlineUserListModel model = new OnlineUserListModel()
            {
                OnlineUserList = OnlineUsers.GetOnlineUserList(pageModel.PageSize, pageModel.PageNumber, locationType, locationId, sort),
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                ProvinceId = provinceId,
                CityId = cityId,
                RegionId = regionId
            };

            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&provinceId={5}&cityId={6}&regionId={7}",
                                                          Url.Action("onlineuserlist"),
                                                          pageModel.PageNumber, pageModel.PageSize,
                                                          sortColumn, sortDirection,
                                                          provinceId, cityId, regionId));
            return View(model);
        }

        /// <summary>
        /// 在线用户趋势
        /// </summary>
        /// <returns></returns>
        public ActionResult OnlineUserTrend()
        {
            OnlineUserTrendModel model = new OnlineUserTrendModel();

            model.PVStatList = PVStats.GetTodayHourPVStatList();
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 搜索词统计列表
        /// </summary>
        /// <param name="word">搜索词</param>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult SearchWordStatList(string word, string sortColumn, string sortDirection, int pageNumber = 1, int pageSize = 15)
        {
            string sort = AdminSearchHistories.GetSearchWordStatListSort(sortColumn, sortDirection);
            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminSearchHistories.GetSearchWordStatCount(word));

            SearchWordStatListModel model = new SearchWordStatListModel()
            {
                SearchWordStatList = AdminSearchHistories.GetSearchWordStatList(pageModel.PageSize, pageModel.PageNumber, word, sort),
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                Word = word
            };
            return View(model);
        }

        /// <summary>
        /// 商品统计-导出报表
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="consignee"></param>
        /// <param name="orderState"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult OutPutSearchList(string word)
        {
            StringBuilder condition = new StringBuilder();
            condition.Append(" where  1=1  ");
            if (!string.IsNullOrWhiteSpace(word))
                condition.AppendFormat(" AND [word] like '{0}%' ", word);

            string sqlText = @"SELECT  [word] 搜索词,SUM([times]) AS 搜索次数 ,MAX([updatetime]) AS 最后搜索时间 FROM [hlh_searchhistories] 
" + condition.ToString() + "GROUP BY [word] ORDER BY 搜索次数 desc";
            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "商品统计-" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 客户端统计
        /// </summary>
        /// <returns></returns>
        public ActionResult ClientStat()
        {
            ClientStatModel model = new ClientStatModel();

            model.BrowserStat = PVStats.GetBrowserStat();
            model.OSStat = PVStats.GetOSStat();

            return View(model);
        }

        /// <summary>
        /// 地区统计
        /// </summary>
        /// <returns></returns>
        public ActionResult RegionStat()
        {
            RegionStatModel model = new RegionStatModel();

            model.RegionStat = PVStats.GetProvinceRegionStat();

            return View(model);
        }

        /// <summary>
        /// 商品统计
        /// </summary>
        /// <param name="productName">商品名称</param>
        /// <param name="categoryName">分类名称</param>
        /// <param name="brandName">品牌名称</param>
        /// <param name="sortOptions">排序</param>
        /// <param name="cateId">分类id</param>
        /// <param name="brandId">品牌id</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult ProductStat(string storeName, string productName, string categoryName, string brandName, string sortColumn, string sortDirection, int storeId = -1, int cateId = -1, int brandId = -1, int pageNumber = 1, int pageSize = 15)
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            string condition = AdminProducts.AdminGetProductListCondition(storeId, 0, productName, cateId, brandId, -1);
            string sort = AdminProducts.AdminGetProductListSort(sortColumn, sortDirection);

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminProducts.AdminGetProductCount(condition));

            DataTable productList = AdminProducts.AdminGetProductList(pageModel.PageSize, pageModel.PageNumber, condition, sort);
            StringBuilder pidList = new StringBuilder();
            foreach (DataRow row in productList.Rows)
            {
                pidList.AppendFormat("{0},", row["pid"]);
            }

            ProductStatModel model = new ProductStatModel()
            {
                ProductList = pidList.Length > 0 ? AdminProducts.GetProductSummaryList(pidList.Remove(pidList.Length - 1, 1).ToString()) : new DataTable(),
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                ProductName = productName,
                StoreId = storeId,
                CateId = cateId,
                BrandId = brandId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "全部店铺" : storeName,
                CategoryName = string.IsNullOrWhiteSpace(categoryName) ? "全部分类" : categoryName,
                BrandName = string.IsNullOrWhiteSpace(brandName) ? "全部品牌" : brandName
            };
            return View(model);
        }

        /// <summary>
        /// 商品统计-导出报表
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="consignee"></param>
        /// <param name="orderState"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult OutPutList(string productName, int storeId = -1, int cateId = -1, int brandId = -1, int pid = 0, int state = -1)
        {
            StringBuilder condition = new StringBuilder();
            condition.Append(" 1=1  ");
            if (state > -1)
                condition.AppendFormat(" AND a.[state]={0} ", (int)state);
            //else
            //    condition.AppendFormat(" AND (a.[state]=0 OR a.[state]=1) ");
            if (!string.IsNullOrWhiteSpace(productName))
                condition.AppendFormat(" AND a.[name] like '%{0}%' ", productName);

            if (cateId > 0)
                condition.AppendFormat(" AND a.[cateid] = {0} ", cateId);

            if (brandId > 0)
                condition.AppendFormat(" AND a.[brandid] = {0} ", brandId);
            if (storeId == -1)
            {
                storeId = WorkContext.PartUserInfo.StoreId;
            }
            if (storeId > 0)
                condition.AppendFormat(" AND a.[storeid] = {0} ", storeId);

            if (pid > 0)
                condition.AppendFormat(" AND a.[pid] = {0} ", pid);

            string sqlText = @"SELECT 
      [pid] AS '商品编号'
	  ,a.[name] AS '商品名称'
	  ,[psn] AS '商品货号'
      ,b.name AS '商品店铺'
      ,[shopprice] AS '商城价'
	  
      ,(SELECT COUNT([reviewid]) FROM [hlh_productreviews] WHERE [pid]=a.pid AND [parentid]=0 AND [state]=0 ) AS '评价数量'
      ,visitcount AS '访问数量'
      ,salecount AS '购买数量'
      ,CASE a.visitcount WHEN 0 THEN  '0%' ELSE CONVERT(VARCHAR,convert(DECIMAL(18,2),(convert(DECIMAL(18,2),salecount)/convert(DECIMAL(18,2),visitcount))*100))+'%' END AS '访问购买率'
	FROM dbo.hlh_products a 
	 LEFT JOIN dbo.hlh_stores b ON a.storeid = b.storeid LEFT JOIN dbo.hlh_brands c ON a.brandid=c.brandid  
	WHERE   " + condition.ToString() + " ORDER BY a.addtime DESC  ";
            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "商品统计-" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 销售明细
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult SaleList(string startTime, string endTime, int pageNumber = 1, int pageSize = 15)
        {
            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminOrders.GetSaleProductCount(startTime, endTime));

            SaleListModel model = new SaleListModel()
            {
                SaleProductList = AdminOrders.GetSaleProductList(pageModel.PageSize, pageModel.PageNumber, startTime, endTime),
                PageModel = pageModel,
                StartTime = startTime,
                EndTime = endTime
            };
            return View(model);
        }
        /// <summary>
        /// 销售明细-导出报表
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="consignee"></param>
        /// <param name="orderState"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult OutPutSaleList(DateTime? startDate, DateTime? endDate, string productName = "", int storeId = -1, int cateId = -1, int brandId = -1, int pid = 0, int state = -1)
        {
            StringBuilder condition = new StringBuilder();
            condition.AppendFormat(" where [orderstate]={0} AND [mallsource]=0 ", (int)OrderState.Completed);

            if (state > -1)
                condition.AppendFormat(" AND a.[state]={0} ", (int)state);
            //else
            //    condition.AppendFormat(" AND (a.[state]=0 OR a.[state]=1) ");
            if (startDate.HasValue)
                condition.AppendFormat(" AND o.[addtime] >= '{0}' ", startDate.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            if (endDate.HasValue)
                condition.AppendFormat(" AND o.[addtime] <= '{0}' ", endDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));
            if (!string.IsNullOrWhiteSpace(productName))
                condition.AppendFormat(" AND a.[name] like '%{0}%' ", productName);

            if (cateId > 0)
                condition.AppendFormat(" AND a.[cateid] = {0} ", cateId);

            if (brandId > 0)
                condition.AppendFormat(" AND a.[brandid] = {0} ", brandId);
            if (storeId == -1)
            {
                storeId = WorkContext.PartUserInfo.StoreId;
            }
            if (storeId > 0)
                condition.AppendFormat(" AND a.[storeid] = {0} ", storeId);

            if (pid > 0)
                condition.AppendFormat(" AND a.[pid] = {0} ", pid);

            //            SELECT  o.[osn] 订单编号,a.[name] 商品名称,a.[psn] 商品货号,a.[realcount] 商品数量,a.[shopprice] 商品价格,o.[addtime] 售出日期 
            //FROM (
            //SELECT [oid],[osn],[addtime] FROM [hlh_orders] WHERE orderstate=140
            //    ) 
            //AS o LEFT JOIN [hlh_orderproducts] AS a ON o.[oid]=a.[oid]   ORDER BY [recordid] DESC

            string sqlText = @"SELECT  o.[osn] 订单编号,a.[name] 商品名称,a.[psn] 商品货号,a.[shopprice] 商品价格,a.[realcount] 商品数量,o.                                                [addtime] 售出日期 
                                        FROM (SELECT [oid],[osn],[addtime] FROM [hlh_orders] o 
                                        " + condition.ToString() + " ) AS o LEFT JOIN [hlh_orderproducts] AS a ON o.[oid]=a.[oid]   ORDER BY                                                [recordid] DESC  ";
            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "销售明细-" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 退款统计
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult RefundStatList(string startTime, string endTime, int pageNumber = 1, int pageSize = 15)
        {
            if (!string.IsNullOrEmpty(HttpContext.Request.Form["IsOutPut"]) && HttpContext.Request.Form["IsOutPut"].Trim() == "1")
            {
                //ViewData["IsOutPut"] = "0";
                return OutPutRefundStatList(startTime, endTime);
            }
            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminOrders.GetRefundStatCount(startTime, endTime));

            SaleListModel model = new SaleListModel()
            {
                SaleProductList = AdminOrders.GetRefundStatList(pageModel.PageSize, pageModel.PageNumber, startTime, endTime),
                PageModel = pageModel,
                StartTime = startTime,
                EndTime = endTime
            };
            return View(model);
        }
        /// <summary>
        /// 退款统计-导出报表
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="consignee"></param>
        /// <param name="orderState"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult OutPutRefundStatList(string startDate, string endDate)
        {
            string condition = AdminOrders.GetRefundStatCondition(startDate, endDate);

            // SELECT  o.[osn] 订单编号,a.[name] 商品名称,a.[psn] 商品货号,a.[realcount] 商品数量,a.[shopprice] 商品价格,o.[addtime] 售出日期 
            //FROM (
            //SELECT [oid],[osn],[addtime] FROM [hlh_orders] WHERE orderstate=140
            //    ) 
            //AS o LEFT JOIN [hlh_orderproducts] AS a ON o.[oid]=a.[oid]   ORDER BY [recordid] DESC

            string sqlText = @"select o.storename 店铺名称,o.addtime 订单时间,RTRIM(o.osn) 订单编号,
ISNULL((SELECT RTRIM(username) username FROM dbo.hlh_users WHERE uid=o.uid),'') 会员编号,
ISNULL((SELECT RTRIM(mobile) FROM dbo.hlh_users WHERE uid=o.uid),'') 会员手机号,
ISNULL((SELECT  RTRIM(realname) FROM dbo.hlh_userdetails WHERE uid=o.uid),'') 会员姓名,o.productamount 产品金额,o.fullcut 满减,o.shipfee 物流费,o.orderamount 订单金额,o.commisiondiscount 佣金账户抵扣,o.couponmoney 优惠券抵扣,o.discount 订单折扣,o.surplusmoney 应付金额,
o.payfriendname 支付方式,o.paytime 支付时间,o.paysn 支付单号,
(SELECT TOP 1 refundmoney FROM dbo.hlh_orderrefunds r WHERE r.oid=o.oid) AS 退款金额,
(SELECT TOP 1 refundfriendname FROM dbo.hlh_orderrefunds WHERE oid=o.oid) 退款方式,
(SELECT TOP 1 refundsn FROM dbo.hlh_orderrefunds WHERE oid=o.oid) 退款单号,
(SELECT TOP 1 refundtransn FROM dbo.hlh_orderrefunds WHERE oid=o.oid) 退款流水号,
(SELECT TOP 1 refundtime FROM dbo.hlh_orderrefunds WHERE oid=o.oid) 退款时间,
(SELECT TOP 1 remark FROM dbo.hlh_orderrefunds WHERE oid=o.oid) 退款备注,

CASE o.orderstate WHEN 10 THEN '已提交' WHEN 30 THEN '等待付款' WHEN 50 THEN '确认中' WHEN 70 THEN '已确认' WHEN 90 THEN '备货中' WHEN 110 THEN '已发货' WHEN 140 THEN '已完成' WHEN 180 THEN '锁定' WHEN 200 THEN '取消' WHEN 160 THEN '退货' ELSE '未知' END 订单状态 from hlh_orders o where
                                        " + condition + " ORDER BY [oid] DESC  ";
            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "退款统计-" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 销售趋势
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="trendType">趋势类型(0代表订单数，1代表订单合计)</param>
        /// <param name="timeType">时间类型(0代表小时，1代表天，2代表月，3代表年)</param>
        /// <returns></returns>
        public ActionResult SaleTrend(string startTime = "0", string endTime = "23", int trendType = 0, int timeType = 0)
        {
            if (string.IsNullOrWhiteSpace(startTime) || string.IsNullOrWhiteSpace(endTime))
                return PromptView(Url.Action("saletrend"), "请输入筛选时间");

            SaleTrendModel model = new SaleTrendModel();

            model.StartTime = startTime;
            model.EndTime = endTime;

            trendType = trendType == 0 ? 0 : 1;
            model.TrendType = trendType;

            if (timeType == 3)//按年筛选
            {
                string startYear = new DateTime(TypeHelper.StringToInt(startTime, DateTime.Now.Year), 1, 1).ToString();
                string endYear = new DateTime((TypeHelper.StringToInt(endTime, DateTime.Now.Year) + 1), 1, 1).ToString();
                model.TrendItemList = AdminOrders.GetSaleTrend(trendType, 3, startYear, endYear);
                model.TimeType = 3;
            }
            else if (timeType == 2)//按月筛选
            {
                string startMonth = TypeHelper.StringToDateTime(startTime).ToString();
                string endMonth = (TypeHelper.StringToDateTime(endTime).AddMonths(1)).ToString();
                model.TrendItemList = AdminOrders.GetSaleTrend(trendType, 2, startMonth, endMonth);
                model.TimeType = 2;
            }
            else if (timeType == 1)//按天筛选
            {
                string startDay = TypeHelper.StringToDateTime(startTime).ToString();
                string endDay = (TypeHelper.StringToDateTime(endTime).AddDays(1)).ToString();
                model.TrendItemList = AdminOrders.GetSaleTrend(trendType, 1, startDay, endDay);
                model.TimeType = 1;
            }
            else//按小时筛选
            {
                int startHour = TypeHelper.StringToInt(startTime, -1);
                int endHour = TypeHelper.StringToInt(endTime, -1);

                if (startHour < 0 || startHour > 23)
                    startHour = 0;
                if (endHour < 0 || endHour > 23)
                    endHour = 23;

                startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startHour, 0, 0).ToString();
                endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, endHour, 59, 59).ToString();
                model.TrendItemList = AdminOrders.GetSaleTrend(trendType, 0, startTime, endTime);
                model.TimeType = 0;
            }

            return View(model);
        }

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
        public string GetSaleResultCondition(int storeId, string osn, int uid, string consignee, string orderState, DateTime? startDate, DateTime? endDate, int isKFXH = 0, int IsBuyHGCard = 0, string payName = "")
        {
            StringBuilder condition = new StringBuilder();
            condition.AppendFormat(" AND o.[mallsource] in (0,2) ");
            if (storeId > 0)
                condition.AppendFormat(" AND o.[storeid] = {0} ", storeId);
            if (!string.IsNullOrWhiteSpace(osn))
                condition.AppendFormat(" AND [osn] like '{0}%' ", osn);
            if (uid > 0)
                condition.AppendFormat(" AND [uid] = {0} ", uid);
            if (!string.IsNullOrWhiteSpace(consignee))
                condition.AppendFormat(" AND [consignee] like '{0}%' ", consignee);
            //if (orderState > 0)
            //    condition.AppendFormat(" AND [orderstate] = {0} ", orderState);
            if (!orderState.Trim().StartsWith("0"))
                condition.AppendFormat(" AND [orderstate] IN ({0}) ", orderState);
            if (startDate.HasValue)
                condition.AppendFormat(" AND [addtime] >= '{0}' ", startDate.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            if (endDate.HasValue)
                condition.AppendFormat(" AND [addtime] <= '{0}' ", endDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));
            if (isKFXH == 0)
                condition.AppendFormat(" AND [cashdiscount]=0 ");
            if (isKFXH == 1)
                condition.AppendFormat(" AND [cashdiscount]>0 ");
            if (IsBuyHGCard == 1)
                condition.AppendFormat(" AND [oid] IN (SELECT oid FROM hlh_orderproducts WHERE pid IN ({0}) ) ", WebHelper.GetConfigSettings("CoffeeQuanPid"));
            if (IsBuyHGCard == 0)
                condition.AppendFormat(" AND [oid] IN (SELECT oid FROM hlh_orderproducts WHERE pid NOT IN ({0}) ) ", WebHelper.GetConfigSettings("CoffeeQuanPid"));
            if (!string.IsNullOrEmpty(payName))
                condition.AppendFormat(" AND [paysystemname] ='{0}' ", payName);
            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }

        /// <summary>
        /// 销售业绩报表
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="osn">订单编号</param>
        /// <param name="accountName">账户名</param>
        /// <param name="consignee">收货人</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ActionResult SaleResult(string storeName, string osn, DateTime? startDate, DateTime? endDate, string accountName, string consignee,
            string sortColumn, string sortDirection, int storeId = -1, string orderState = "0", int pageSize = 15, int pageNumber = 1, int isKFXH = -1, int IsBuyHGCard = -1, string payName = "")
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            //获取用户id
            int uid = Users.GetUidByAccountName(accountName);
            string condition = GetSaleResultCondition(storeId, osn, uid, consignee, orderState, startDate, endDate, isKFXH, IsBuyHGCard, payName);
            string sort = AdminOrders.GetOrderListSort(sortColumn, sortDirection);

            if (!string.IsNullOrEmpty(HttpContext.Request.Form["IsOutPut"]) && HttpContext.Request.Form["IsOutPut"].Trim() == "1")
            {
                return OutPutOrder(condition, sort);
            }

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminOrders.GetOrderCount(condition));

            SaleResultModel model = new SaleResultModel()
            {
                OrderList = AdminOrders.GetSaleResult(pageModel.PageSize, pageModel.PageNumber, condition, sort),
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                StoreId = storeId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "全部店铺" : storeName,
                OSN = osn,
                AccountName = accountName,
                Consignee = consignee,
                OrderState = orderState,
                StartDate = startDate,
                EndDate = endDate,
                IsKFXH = isKFXH,
                IsBuyHGCard = IsBuyHGCard,
                PayName = payName
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&storeId={5}&storeName={6}&OSN={7}&AccountName={8}&Consignee={9}&OrderState={10}&&isKFXH={11}&IsBuyHGCard={12}&payName={13}",
                                                          Url.Action("SaleResult"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          sortColumn, sortDirection,
                                                          storeId, storeName,
                                                          osn, accountName, consignee, orderState, isKFXH, IsBuyHGCard, payName));

            List<SelectListItem> itemList = new List<SelectListItem>();
            //itemList.Add(new SelectListItem() { Text = "全部", Value = "0" });
            //foreach (OrderState source in Enum.GetValues(typeof(OrderState)))
            //{
            //    itemList.Add(new SelectListItem() { Text = source.ToString(), Value = ((int)source).ToString() });
            //}
            itemList.Add(new SelectListItem() { Text = "已提交", Value = ((int)OrderState.Submitted).ToString() });
            itemList.Add(new SelectListItem() { Text = "等待付款", Value = ((int)OrderState.WaitPaying).ToString() });
            itemList.Add(new SelectListItem() { Text = "待确认", Value = ((int)OrderState.Confirming).ToString() });
            itemList.Add(new SelectListItem() { Text = "已确认", Value = ((int)OrderState.Confirmed).ToString() });
            itemList.Add(new SelectListItem() { Text = "备货中", Value = ((int)OrderState.PreProducting).ToString() });
            itemList.Add(new SelectListItem() { Text = "已发货", Value = ((int)OrderState.Sended).ToString() });
            itemList.Add(new SelectListItem() { Text = "已完成", Value = ((int)OrderState.Completed).ToString() });
            itemList.Add(new SelectListItem() { Text = "已锁定", Value = ((int)OrderState.Locked).ToString() });
            itemList.Add(new SelectListItem() { Text = "已取消", Value = ((int)OrderState.Cancelled).ToString() });
            itemList.Add(new SelectListItem() { Text = "已退货", Value = ((int)OrderState.Returned).ToString() });
            ViewData["orderStateList"] = itemList;
            List<SelectListItem> itemListKFXF = new List<SelectListItem>();
            itemListKFXF.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemListKFXF.Add(new SelectListItem() { Text = "是", Value = "1" });
            itemListKFXF.Add(new SelectListItem() { Text = "否", Value = "0" });
            ViewData["KFXFList"] = itemListKFXF;

            List<SelectListItem> itemListCashCard = new List<SelectListItem>();
            itemListCashCard.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemListCashCard.Add(new SelectListItem() { Text = "是", Value = "1" });
            itemListCashCard.Add(new SelectListItem() { Text = "否", Value = "0" });
            ViewData["CashCardList"] = itemListCashCard;

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

            //ViewData["IsOutPut"] = "0";

            return View(model);
        }

        /// <summary>
        /// 下载销售业绩报表
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="osn">订单编号</param>
        /// <param name="accountName">账户名</param>
        /// <param name="consignee">收货人</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        //public ActionResult DownloadSaleResult(string storeName, string osn, DateTime? startDate, DateTime? endDate, string accountName, string consignee,
        //    string sortColumn, string sortDirection, int storeId = -1, int orderState = 0)
        //{
        //    //获取用户id
        //    int uid = Users.GetUidByAccountName(accountName);

        //    string condition = AdminOrders.GetOrderListCondition(storeId, osn, uid, consignee, orderState, startDate, endDate);
        //    string sort = AdminOrders.GetOrderListSort(sortColumn, sortDirection);

        //    return OutPutOrder(condition, sort);
        //}

        /// <summary>
        /// 导出销售业绩报表
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="consignee"></param>
        /// <param name="orderState"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult OutPutOrder(string condition, string sort)
        {
            if (string.IsNullOrEmpty(condition))
                condition = "1=1";

            string sqlText = string.Format(@"select o.storename 店铺名称,o.addtime 订单时间,RTRIM(o.osn) 订单编号,
ISNULL((SELECT RTRIM(username) username FROM dbo.hlh_users WHERE uid=o.uid),'') 会员编号,
ISNULL((SELECT RTRIM(mobile) FROM dbo.hlh_users WHERE uid=o.uid),'') 会员手机号,
ISNULL((SELECT  RTRIM(realname) FROM dbo.hlh_userdetails WHERE uid=o.uid),'') 会员姓名,o.productamount 产品金额,o.fullcut 满减,o.shipfee 物流费,o.orderamount 订单金额,o.commisiondiscount 佣金账户抵扣,o.couponmoney 优惠券抵扣,o.discount 订单折扣,o.surplusmoney 应付金额,o.payfriendname 支付方式,o.paytime 支付时间,
CASE o.orderstate WHEN 10 THEN '已提交' WHEN 30 THEN '等待付款' WHEN 50 THEN '确认中' WHEN 70 THEN '已确认' WHEN 90 THEN '备货中' WHEN 110 THEN '已发货' WHEN 140 THEN '已完成' WHEN 180 THEN '锁定' WHEN 200 THEN '取消' WHEN 160 THEN '退货' ELSE '未知' END 订单状态,o.buyerremark 备注 from hlh_orders o where {0} order by {1}", condition, sort);
            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "销售业绩报表-" + DateTime.Now.ToString("yyyyMMdd")));
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
    }
}
