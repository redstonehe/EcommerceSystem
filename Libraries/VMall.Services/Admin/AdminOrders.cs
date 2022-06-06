using System;
using System.Data;
using System.Text;
using VMall.Core;

namespace VMall.Services
{
    /// <summary>
    /// 后台订单操作管理类
    /// </summary>
    public partial class AdminOrders : Orders
    {
        /// <summary>
        /// 获得订单列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable GetOrderList(int pageSize, int pageNumber, string condition, string sort)
        {
            return VMall.Data.Orders.GetOrderList(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 获得销售业绩列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable GetSaleResult(int pageSize, int pageNumber, string condition, string sort)
        {
            return VMall.Data.Orders.GetSaleResult(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 根据条件获得订单列表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static DataTable GetOrderByCondition(string condition)
        {
            return VMall.Data.Orders.GetOrderByCondition(condition);
        }

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
        public static string GetOrderListCondition(int storeId, string osn, int uid, string consignee, int orderState, 
            DateTime? startDate, DateTime? endDate)
        {
            return VMall.Data.Orders.GetOrderListCondition(storeId, osn, uid, consignee, orderState, startDate, endDate);
        }

        /// <summary>
        /// 获得列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string GetOrderListSort(string sortColumn, string sortDirection)
        {
            return VMall.Data.Orders.GetOrderListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 获得订单数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int GetOrderCount(string condition)
        {
            return VMall.Data.Orders.GetOrderCount(condition);
        }

        /// <summary>
        /// 获得销售商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static DataTable GetSaleProductList(int pageSize, int pageNumber, string startTime, string endTime)
        {
            return VMall.Data.Orders.GetSaleProductList(pageSize, pageNumber, startTime, endTime);
        }

        /// <summary>
        /// 获得销售商品数量
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static int GetSaleProductCount(string startTime, string endTime)
        {
            return VMall.Data.Orders.GetSaleProductCount(startTime, endTime);
        }

        /// <summary>
        /// 获得销售趋势
        /// </summary>
        /// <param name="trendType">趋势类型(0代表订单数，1代表订单合计)</param>
        /// <param name="timeType">时间类型(0代表小时，1代表天，2代表月，3代表年)</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static DataTable GetSaleTrend(int trendType, int timeType, string startTime, string endTime)
        {
            return VMall.Data.Orders.GetSaleTrend(trendType, timeType, startTime, endTime);
        }

        /// <summary>
        /// 获得退款统计列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static DataTable GetRefundStatList(int pageSize, int pageNumber, string startTime, string endTime)
        {
            string condition = GetRefundStatCondition(startTime, endTime);
            bool noCondition = string.IsNullOrWhiteSpace(condition);
            string commandText;
//            commandText = string.Format(@"with tb as(
// select a.*,b.uid AS userid,b.lastvisittime,b.lastvisitip,b.lastvisitrgid,b.registerip,b.registerrgid,b.gender,realname,b.bday,b.idcard,b.regionid,b.address,b.bio,b.registertime,ROW_NUMBER() OVER (ORDER BY a.[uid] DESC) AS RowNumber from [hlh_users] as a left join 
// hlh_userdetails as b on a.uid=b.uid
// WHERE 1=1 {0}  )
// select * from tb where RowNumber between (" + pageNumber + @"-1)*" + pageSize + @"+1 and (" + pageNumber + @")*" + pageSize + " order by tb.[uid] desc", noCondition ? "" : " AND " + condition);
            commandText = string.Format(@"with tb as(
                                                    select o.*,u.username ,u.mobile AS usermobile,ud.realname,
                                                    (SELECT TOP 1 refundmoney FROM dbo.hlh_orderrefunds r WHERE r.oid=o.oid) as refundmoney,
                                                    (SELECT TOP 1 refundfriendname FROM dbo.hlh_orderrefunds WHERE oid=o.oid) as refundfriendname,
                                                    (SELECT TOP 1 refundsn FROM dbo.hlh_orderrefunds WHERE oid=o.oid) as refundsn,
                                                    (SELECT TOP 1 refundtransn FROM dbo.hlh_orderrefunds WHERE oid=o.oid) as refundtransn,
                                                    (SELECT TOP 1 refundtime FROM dbo.hlh_orderrefunds WHERE oid=o.oid) as refundtime,
                                                    (SELECT TOP 1 remark FROM dbo.hlh_orderrefunds WHERE oid=o.oid) as refundremark,
                                                    ROW_NUMBER() OVER (ORDER BY o.[oid] DESC) AS RowNumber from hlh_orders o left join hlh_users u                                                     ON u.uid = o.uid INNER JOIN dbo.hlh_userdetails ud ON ud.uid = u.uid
                                               WHERE 1=1 {0} )
                                        select * from tb where RowNumber between (" + pageNumber + @"-1)*" + pageSize + @"+1 and (" + pageNumber +                                          @")*" + pageSize + " order by tb.[oid] desc", noCondition ? "" : " AND " + condition);


            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText).Tables[0];
        }
        /// <summary>
        /// 获得退款统计列表条件
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static string GetRefundStatCondition(string startTime, string endTime)
        {
            StringBuilder condition = new StringBuilder();
            condition.Append(" paysn!='' and (orderstate=200 or orderstate=160) ");
            if (!string.IsNullOrEmpty(startTime))
                condition.AppendFormat(" AND [addtime]>='{0}' ", TypeHelper.StringToDateTime(startTime).ToString("yyyy-MM-dd HH:mm:ss"));
            if (!string.IsNullOrEmpty(endTime))
                condition.AppendFormat(" AND [addtime]<='{0}' ", TypeHelper.StringToDateTime(endTime).ToString("yyyy-MM-dd HH:mm:ss"));

            return condition.ToString();
        }

        /// <summary>
        /// 获得销售商品数量
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static int GetRefundStatCount(string startTime, string endTime)
        {
            string condition = GetRefundStatCondition(startTime, endTime);
            string commandText = string.Format("SELECT COUNT(oid) FROM hlh_orders o where {0}",
                                                condition);
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText));
        }
    }
}
