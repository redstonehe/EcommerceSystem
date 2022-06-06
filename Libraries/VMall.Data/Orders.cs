using System;
using System.Data;
using System.Collections.Generic;

using VMall.Core;

namespace VMall.Data
{
    /// <summary>
    /// 订单数据访问类
    /// </summary>
    public partial class Orders
    {
        private static IOrderNOSQLStrategy _ordernosql = BMAData.OrderNOSQL;//订单非关系型数据库

        #region 辅助方法

        /// <summary>
        /// 从IDataReader创建OrderProductInfo
        /// </summary>
        public static OrderProductInfo BuildOrderProductFromReader(IDataReader reader)
        {
            OrderProductInfo orderProductInfo = new OrderProductInfo();
            orderProductInfo.RecordId = TypeHelper.ObjectToInt(reader["recordid"]);
            orderProductInfo.Oid = TypeHelper.ObjectToInt(reader["oid"]);
            orderProductInfo.Uid = TypeHelper.ObjectToInt(reader["uid"]);
            orderProductInfo.Sid = reader["sid"].ToString();
            orderProductInfo.Pid = TypeHelper.ObjectToInt(reader["pid"]);
            orderProductInfo.PSN = reader["psn"].ToString();
            orderProductInfo.CateId = TypeHelper.ObjectToInt(reader["cateid"]);
            orderProductInfo.BrandId = TypeHelper.ObjectToInt(reader["brandid"]);
            orderProductInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            orderProductInfo.StoreCid = TypeHelper.ObjectToInt(reader["storecid"]);
            orderProductInfo.StoreSTid = TypeHelper.ObjectToInt(reader["storestid"]);
            orderProductInfo.Name = reader["name"].ToString();
            orderProductInfo.ShowImg = reader["showimg"].ToString();
            orderProductInfo.DiscountPrice = TypeHelper.ObjectToDecimal(reader["discountprice"]);
            orderProductInfo.ShopPrice = TypeHelper.ObjectToDecimal(reader["shopprice"]);
            orderProductInfo.CostPrice = TypeHelper.ObjectToDecimal(reader["costprice"]);
            orderProductInfo.MarketPrice = TypeHelper.ObjectToDecimal(reader["marketprice"]);
            orderProductInfo.Weight = TypeHelper.ObjectToInt(reader["weight"]);
            orderProductInfo.IsReview = TypeHelper.ObjectToInt(reader["isreview"]);
            orderProductInfo.RealCount = TypeHelper.ObjectToInt(reader["realcount"]);
            orderProductInfo.BuyCount = TypeHelper.ObjectToInt(reader["buycount"]);
            orderProductInfo.SendCount = TypeHelper.ObjectToInt(reader["sendcount"]);
            orderProductInfo.Type = TypeHelper.ObjectToInt(reader["type"]);
            orderProductInfo.PayCredits = TypeHelper.ObjectToInt(reader["paycredits"]);
            orderProductInfo.CouponTypeId = TypeHelper.ObjectToInt(reader["coupontypeid"]);
            orderProductInfo.ExtCode1 = TypeHelper.ObjectToInt(reader["extcode1"]);
            orderProductInfo.ExtCode2 = TypeHelper.ObjectToInt(reader["extcode2"]);
            orderProductInfo.ExtCode3 = TypeHelper.ObjectToInt(reader["extcode3"]);
            orderProductInfo.ExtCode4 = TypeHelper.ObjectToInt(reader["extcode4"]);
            orderProductInfo.ExtCode5 = TypeHelper.ObjectToInt(reader["extcode5"]);
            orderProductInfo.AddTime = TypeHelper.ObjectToDateTime(reader["addtime"]);
            orderProductInfo.ProductHaiMi = TypeHelper.ObjectToDecimal(reader["producthaimi"]);
            orderProductInfo.ProductPV = TypeHelper.ObjectToDecimal(reader["productpv"]);
            orderProductInfo.ProductHBCut = TypeHelper.ObjectToDecimal(reader["producthbcut"]);
            orderProductInfo.FromParent = TypeHelper.ObjectToInt(reader["fromparent"]);
            orderProductInfo.FromCompany = TypeHelper.ObjectToInt(reader["fromcompany"]);
            orderProductInfo.FromParentId = TypeHelper.ObjectToInt(reader["fromparentid"]);
            orderProductInfo.FromParentId1 = TypeHelper.ObjectToInt(reader["fromparentid1"]);
            orderProductInfo.FromParentAmount1 = TypeHelper.ObjectToDecimal(reader["fromparentamount1"]);
            orderProductInfo.FromParentId2 = TypeHelper.ObjectToInt(reader["fromparentid2"]);
            orderProductInfo.FromParentAmount2 = TypeHelper.ObjectToDecimal(reader["fromparentamount2"]);
            orderProductInfo.FromParentId3 = TypeHelper.ObjectToInt(reader["fromparentid3"]);
            orderProductInfo.FromParentAmount3 = TypeHelper.ObjectToDecimal(reader["fromparentamount3"]);
            orderProductInfo.FromParentId4 = TypeHelper.ObjectToInt(reader["fromparentid4"]);
            orderProductInfo.FromParentAmount4 = TypeHelper.ObjectToDecimal(reader["fromparentamount4"]);
            orderProductInfo.FromCompanyAmount = TypeHelper.ObjectToDecimal(reader["fromcompanyamount"]);
            return orderProductInfo;
        }

        /// <summary>
        /// 从IDataReader创建OrderInfo
        /// </summary>
        public static OrderInfo BuildOrderFromReader(IDataReader reader)
        {
            OrderInfo orderInfo = new OrderInfo();

            orderInfo.Oid = TypeHelper.ObjectToInt(reader["oid"]);
            orderInfo.OSN = reader["osn"].ToString();
            orderInfo.Uid = TypeHelper.ObjectToInt(reader["uid"]);

            orderInfo.OrderState = TypeHelper.ObjectToInt(reader["orderstate"]);

            orderInfo.ProductAmount = TypeHelper.ObjectToDecimal(reader["productamount"]);
            orderInfo.OrderAmount = TypeHelper.ObjectToDecimal(reader["orderamount"]);
            orderInfo.SurplusMoney = TypeHelper.ObjectToDecimal(reader["surplusmoney"]);

            orderInfo.ParentId = TypeHelper.ObjectToInt(reader["parentid"]);
            orderInfo.IsReview = TypeHelper.ObjectToInt(reader["isreview"]);
            orderInfo.AddTime = TypeHelper.ObjectToDateTime(reader["addtime"]);
            orderInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            orderInfo.StoreName = reader["storename"].ToString();
            orderInfo.ShipSN = reader["shipsn"].ToString();
            orderInfo.ShipCoId = TypeHelper.ObjectToInt(reader["shipcoid"]);
            orderInfo.ShipCoName = reader["shipconame"].ToString();
            orderInfo.ShipTime = TypeHelper.ObjectToDateTime(reader["shiptime"]);
            orderInfo.PaySN = reader["paysn"].ToString();
            orderInfo.PayFriendName = reader["payfriendname"].ToString();
            orderInfo.PaySystemName = reader["paysystemname"].ToString();
            orderInfo.PayMode = TypeHelper.ObjectToInt(reader["paymode"]);
            orderInfo.PayTime = TypeHelper.ObjectToDateTime(reader["paytime"]);

            orderInfo.RegionId = TypeHelper.ObjectToInt(reader["regionid"]);
            orderInfo.Consignee = reader["consignee"].ToString();
            orderInfo.Mobile = reader["mobile"].ToString();
            orderInfo.Phone = reader["phone"].ToString();
            orderInfo.Email = reader["email"].ToString();
            orderInfo.ZipCode = reader["zipcode"].ToString();
            orderInfo.Address = reader["address"].ToString();
            orderInfo.BestTime = TypeHelper.ObjectToDateTime(reader["besttime"]);

            orderInfo.ShipFee = TypeHelper.ObjectToDecimal(reader["shipfee"]);
            orderInfo.PayFee = TypeHelper.ObjectToDecimal(reader["payfee"]);
            orderInfo.FullCut = TypeHelper.ObjectToInt(reader["fullcut"]);
            orderInfo.Discount = TypeHelper.ObjectToDecimal(reader["discount"]);
            orderInfo.PayCreditCount = TypeHelper.ObjectToInt(reader["paycreditcount"]);
            orderInfo.PayCreditMoney = TypeHelper.ObjectToDecimal(reader["paycreditmoney"]);
            orderInfo.CouponMoney = TypeHelper.ObjectToDecimal(reader["couponmoney"]);
            orderInfo.Weight = TypeHelper.ObjectToInt(reader["weight"]);

            orderInfo.BuyerRemark = reader["buyerremark"].ToString();
            orderInfo.IP = reader["ip"].ToString();
            orderInfo.OrderSource = TypeHelper.ObjectToInt(reader["ordersource"]);
            orderInfo.TaxAmount = TypeHelper.ObjectToDecimal(reader["taxamount"]);
            orderInfo.ReceivingTime = TypeHelper.ObjectToDateTime(reader["receivingtime"]);
            orderInfo.PayDevice = reader["paydevice"].ToString();
            orderInfo.Invoice = reader["invoice"].ToString();
            orderInfo.HaiMiDiscount = TypeHelper.ObjectToDecimal(reader["haimidiscount"]);
            orderInfo.HongBaoDiscount = TypeHelper.ObjectToDecimal(reader["hongbaodiscount"]);
            orderInfo.SettleState = TypeHelper.ObjectToInt(reader["settlestate"]);
            orderInfo.IsExtendReceive=TypeHelper.ObjectToInt(reader["isextendreceive"]);
            orderInfo.IsDelete=TypeHelper.ObjectToInt(reader["isdelete"]);
            orderInfo.ReturnType = TypeHelper.ObjectToInt(reader["returntype"]);
            orderInfo.ChangeType = TypeHelper.ObjectToInt(reader["changetype"]);
            orderInfo.CashDiscount = TypeHelper.ObjectToDecimal(reader["cashdiscount"]);
            orderInfo.ExtOrderId = reader["extorderid"].ToString();
            orderInfo.MainOid = TypeHelper.ObjectToInt(reader["mainoid"]);
            orderInfo.SubOid = TypeHelper.ObjectToInt(reader["suboid"]);
            orderInfo.AgentDiscount = TypeHelper.ObjectToDecimal(reader["agentdiscount"]);
            orderInfo.CommisionDiscount = TypeHelper.ObjectToDecimal(reader["commisiondiscount"]);
            orderInfo.ActualUid = TypeHelper.ObjectToInt(reader["actualuid"]);
            orderInfo.InvoiceMore = reader["invoicemore"].ToString();
            orderInfo.AdminRemark = reader["adminremark"].ToString();
            return orderInfo;
        }

        #endregion

        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns>订单信息</returns>
        public static OrderInfo GetOrderByOid(int oid)
        {
            OrderInfo orderInfo = null;

            if (_ordernosql != null)
            {
                orderInfo = _ordernosql.GetOrderByOid(oid);
                if (orderInfo == null)
                {
                    IDataReader reader = VMall.Core.BMAData.RDBS.GetOrderByOid(oid);
                    if (reader.Read())
                    {
                        orderInfo = BuildOrderFromReader(reader);
                    }
                    reader.Close();
                    if (orderInfo != null)
                        _ordernosql.CreateOrder(orderInfo);
                }
            }
            else
            {
                IDataReader reader = VMall.Core.BMAData.RDBS.GetOrderByOid(oid);
                if (reader.Read())
                {
                    orderInfo = BuildOrderFromReader(reader);
                }
                reader.Close();
            }

            return orderInfo;
        }

        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="osn">订单编号</param>
        /// <returns>订单信息</returns>
        public static OrderInfo GetOrderByOSN(string osn)
        {
            OrderInfo orderInfo = null;
            IDataReader reader = VMall.Core.BMAData.RDBS.GetOrderByOSN(osn);
            if (reader.Read())
            {
                orderInfo = BuildOrderFromReader(reader);
            }
            reader.Close();
            return orderInfo;
        }

        /// <summary>
        /// 获得订单数量
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static int GetOrderCountByCondition(int storeId, int orderState, string startTime, string endTime)
        {
            return VMall.Core.BMAData.RDBS.GetOrderCountByCondition(storeId, orderState, startTime, endTime);
        }

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
            return VMall.Core.BMAData.RDBS.GetOrderList(pageSize, pageNumber, condition, sort);
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
            return VMall.Core.BMAData.RDBS.GetSaleResult(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 根据条件获得订单列表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static DataTable GetOrderByCondition(string condition)
        {
            return VMall.Core.BMAData.RDBS.GetOrderByCondition(condition);
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
            return VMall.Core.BMAData.RDBS.GetOrderListCondition(storeId, osn, uid, consignee, orderState, startDate, endDate);
        }

        /// <summary>
        /// 获得列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string GetOrderListSort(string sortColumn, string sortDirection)
        {
            return VMall.Core.BMAData.RDBS.GetOrderListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 获得订单数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int GetOrderCount(string condition)
        {
            return VMall.Core.BMAData.RDBS.GetOrderCount(condition);
        }

        /// <summary>
        /// 获得订单商品列表
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetOrderProductList(int oid)
        {
            List<OrderProductInfo> orderProductList = null;

            if (_ordernosql != null)
            {
                orderProductList = _ordernosql.GetOrderProductList(oid);
                if (orderProductList == null)
                {
                    orderProductList = new List<OrderProductInfo>();
                    IDataReader reader = VMall.Core.BMAData.RDBS.GetOrderProductList(oid);
                    while (reader.Read())
                    {
                        OrderProductInfo orderProductInfo = BuildOrderProductFromReader(reader);
                        orderProductList.Add(orderProductInfo);
                    }
                    reader.Close();
                    _ordernosql.CreateOrderProductList(oid, orderProductList);
                }
            }
            else
            {
                orderProductList = new List<OrderProductInfo>();
                IDataReader reader = VMall.Core.BMAData.RDBS.GetOrderProductList(oid);
                while (reader.Read())
                {
                    OrderProductInfo orderProductInfo = BuildOrderProductFromReader(reader);
                    orderProductList.Add(orderProductInfo);
                }
                reader.Close();
            }

            return orderProductList;
        }

        /// <summary>
        /// 获得订单商品列表
        /// </summary>
        /// <param name="oidList">订单id列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetOrderProductList(string oidList)
        {
            List<OrderProductInfo> orderProductList = new List<OrderProductInfo>();

            if (_ordernosql != null)
            {
                foreach (string oid in StringHelper.SplitString(oidList))
                    orderProductList.AddRange(GetOrderProductList(TypeHelper.StringToInt(oid)));
            }
            else
            {
                IDataReader reader = VMall.Core.BMAData.RDBS.GetOrderProductList(oidList);
                while (reader.Read())
                {
                    OrderProductInfo orderProductInfo = BuildOrderProductFromReader(reader);
                    orderProductList.Add(orderProductInfo);
                }
                reader.Close();
            }

            return orderProductList;
        }

        /// <summary>
        /// 更新订单折扣
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="discount">折扣</param>
        /// <param name="surplusMoney">剩余金额</param>
        public static void UpdateOrderDiscount(int oid, decimal discount, decimal surplusMoney)
        {
            VMall.Core.BMAData.RDBS.UpdateOrderDiscount(oid, discount, surplusMoney);
            if (_ordernosql != null)
                _ordernosql.UpdateOrderDiscount(oid, discount, surplusMoney);
        }

        /// <summary>
        /// 更新订单配送费用
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="shipFee">配送费用</param>
        /// <param name="orderAmount">订单合计</param>
        /// <param name="surplusMoney">剩余金额</param>
        public static void UpdateOrderShipFee(int oid, decimal shipFee, decimal orderAmount, decimal surplusMoney)
        {
            VMall.Core.BMAData.RDBS.UpdateOrderShipFee(oid, shipFee, orderAmount, surplusMoney);
            if (_ordernosql != null)
                _ordernosql.UpdateOrderShipFee(oid, shipFee, orderAmount, surplusMoney);
        }

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        public static void UpdateOrderState(int oid, OrderState orderState)
        {
            VMall.Core.BMAData.RDBS.UpdateOrderState(oid, orderState);
            if (_ordernosql != null)
                _ordernosql.UpdateOrderState(oid, orderState);
        }

        /// <summary>
        /// 更新订单结算状态
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单结算状态</param>
        public static void UpdateOrderSettleState(int oid, OrderSettleState orderSettleState)
        {
            VMall.Core.BMAData.RDBS.UpdateOrderSettleState(oid, orderSettleState);
            if (_ordernosql != null)
                _ordernosql.UpdateOrderSettleState(oid, orderSettleState);
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        public static void ConfirmReceiving(int oid, OrderState orderState, DateTime receivingTime)
        {
            VMall.Core.BMAData.RDBS.ConfirmReceiving(oid, orderState, receivingTime);
            if (_ordernosql != null)
                _ordernosql.ConfirmReceiving(oid, orderState, receivingTime);
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="shipSN">配送单号</param>
        /// <param name="shipCoId">配送公司id</param>
        /// <param name="shipCoName">配送公司名称</param>
        /// <param name="shipTime">配送时间</param>
        public static void SendOrderProduct(int oid, OrderState orderState, string shipSN, int shipCoId, string shipCoName, DateTime shipTime)
        {
            VMall.Core.BMAData.RDBS.SendOrderProduct(oid, orderState, shipSN, shipCoId, shipCoName, shipTime);
            if (_ordernosql != null)
                _ordernosql.SendOrderProduct(oid, orderState, shipSN, shipCoId, shipCoName, shipTime);
        }

        /// <summary>
        /// 付款
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="paySN">支付单号</param>
        /// <param name="payTime">支付时间</param>
        public static void PayOrder(int oid, OrderState orderState, string paySN, DateTime payTime, string payDevice)
        {
            VMall.Core.BMAData.RDBS.PayOrder(oid, orderState, paySN, payTime, payDevice);
            if (_ordernosql != null)
                _ordernosql.PayOrder(oid, orderState, paySN, payTime, payDevice);
        }

        /// <summary>
        /// 更新订单的评价
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="isReview">是否评价</param>
        public static void UpdateOrderIsReview(int oid, int isReview)
        {
            VMall.Core.BMAData.RDBS.UpdateOrderIsReview(oid, isReview);
            if (_ordernosql != null)
                _ordernosql.UpdateOrderIsReview(oid, isReview);
        }

        /// <summary>
        /// 获得用户订单列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        public static DataTable GetUserOrderList(int uid, int pageSize, int pageNumber, string startAddTime, string endAddTime, int orderState, int mallSource)
        {
            return VMall.Core.BMAData.RDBS.GetUserOrderList(uid, pageSize, pageNumber, startAddTime, endAddTime, orderState, mallSource);
        }

        /// <summary>
        /// 获得用户订单列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        public static int GetUserOrderCount(int uid, string startAddTime, string endAddTime, int orderState,int mallSource)
        {
            return VMall.Core.BMAData.RDBS.GetUserOrderCount(uid, startAddTime, endAddTime, orderState, mallSource);
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
            return VMall.Core.BMAData.RDBS.GetSaleProductList(pageSize, pageNumber, startTime, endTime);
        }

        /// <summary>
        /// 获得销售商品数量
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static int GetSaleProductCount(string startTime, string endTime)
        {
            return VMall.Core.BMAData.RDBS.GetSaleProductCount(startTime, endTime);
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
            return VMall.Core.BMAData.RDBS.GetSaleTrend(trendType, timeType, startTime, endTime);
        }
    }
}
