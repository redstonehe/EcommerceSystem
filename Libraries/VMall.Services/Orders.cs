using System;
using System.Text;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using VMall.Core;
using System.Web.Configuration;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data.Common;

namespace VMall.Services
{
    /// <summary>
    /// 订单操作管理类
    /// </summary>
    public partial class Orders
    {
        private static IOrderStrategy _iorderstrategy = BMAOrder.Instance;//订单策略

        private static object _locker = new object();//锁对象

        private static string _osnformat;//订单编号格式

        private static OrderGift orderGiftBLL = new OrderGift();

        static Orders()
        {
            _osnformat = BMAConfig.MallConfig.OSNFormat;
        }


        #region 创建订单相关方法
        /// <summary>
        /// 重置订单编号格式
        /// </summary>
        public static void ResetOSNFormat()
        {
            lock (_locker)
            {
                _osnformat = BMAConfig.MallConfig.OSNFormat;
            }
        }

        /// <summary>
        /// 生成订单编号--18位 前缀1位+支付方式1位+订单日期14位+2位随机数后缀
        /// 20181101 之后采用新的订单编号规则，已MO开头+前缀1位+支付方式1位+订单日期14位+4位随机数后缀，总计22位订单编号
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="uid">用户id</param>
        /// <param name="shipRegionId">配送区域id</param>
        /// <param name="addTime">下单时间</param>
        /// <returns>订单编号</returns>
        public static string GenerateOSN(int storeId, int uid, int shipRegionId, DateTime addTime, string payModel, string orderSource, decimal payMoney)
        {
            //StringBuilder osn = new StringBuilder(_osnformat);
            //osn.Replace("{storeid}", storeId.ToString());
            //osn.Replace("{uid}", uid.ToString());
            //osn.Replace("{srid}", shipRegionId.ToString());
            //osn.Replace("{addtime}", addTime.ToString("yyyyMMddHHmmss"));
            //return osn.ToString();
            decimal limitAmount = 399;
            DateTime curr = TypeHelper.StringToDateTime(WebHelper.GetConfigSettings("LastOrderNumTime"));
            DateTime start = new DateTime(curr.Year, curr.Month, curr.Day);
            //DateTime start = new DateTime(2016, 7, 8);
            DateTime end = start.AddDays(1);
            StringBuilder osn = new StringBuilder();
            osn.Append("1");//前缀
            osn.Append(payModel);//支付方式
            osn.Append(addTime.ToString("yyyyMMddHHmmss"));//订单日期
            Random _random = new Random();//随机发生器
            if (DateTime.Now > new DateTime(2018, 11, 1))
            {
                osn.Insert(0, "MO");
                osn.Append(Randoms.CreateRandomValue(4));//4位随机数后缀
            }
            else
            {
                if (DateTime.Now > start && DateTime.Now < end)//5-9顺序排列
                {
                    osn.Append(Randoms.CreateRandomValue(5));//5位随机数后缀+1位特定尾数
                    if (payMoney > limitAmount)
                    {
                        //OrderInfo info = Orders.GetOrdertByWhere(string.Format(" orderamount >{0} and addtime>='{1}' and addtime<'{2}' order by  oid desc", limitAmount, start.ToString("yyyy-MM-dd HH:mm:ss.fff"), end.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                        //int lastNum = 4;
                        //if (info != null)
                        //{
                        //    string infoosn = info.OSN;
                        //    lastNum = TypeHelper.StringToInt(infoosn.Substring(infoosn.Length - 1, 1));

                        //}
                        //if ((lastNum >= 0 && lastNum < 5) || lastNum == 9)
                        //    lastNum = 4;
                        //osn.Append((lastNum + 1).ToString());
                        osn.Append((_random.Next(5, 9)).ToString());
                    }
                    else //0-9顺序排列
                    {
                        //OrderInfo info = Orders.GetOrdertByWhere(string.Format(" orderamount <={0} and addtime>='{1}' and addtime<'{2}' order by  oid desc", limitAmount, start.ToString("yyyy-MM-dd HH:mm:ss.fff"), end.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                        //int lastNum = -1;
                        //if (info != null)
                        //{
                        //    string infoosn = info.OSN;
                        //    lastNum = TypeHelper.StringToInt(infoosn.Substring(infoosn.Length - 1, 1));

                        //}
                        //if (lastNum == 9)
                        //    lastNum = -1;
                        //osn.Append((lastNum + 1).ToString());
                        osn.Append((_random.Next(0, 9)).ToString());
                    }
                }
                else
                {
                    osn.Append(Randoms.CreateRandomValue(6));//6位随机数后缀
                }
            }
            //osn.Append(Randoms.CreateRandomValue(6));//2位随机数后缀

            return osn.ToString();
        }

        /// <summary>
        /// 生成订货系统订单编号--18位 前缀1位+支付方式1位+订单日期14位+2位随机数后缀
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="uid">用户id</param>
        /// <param name="shipRegionId">配送区域id</param>
        /// <param name="addTime">下单时间</param>
        /// <returns>订单编号</returns>
        private static string GenerateOSN_Agent(DateTime addTime, string payModel, string orderSource)
        {
            StringBuilder osn = new StringBuilder();
            osn.Append("7");//前缀
            osn.Append(payModel);//支付方式
            osn.Append(addTime.ToString("yyyyMMddHHmmss"));//订单日期
            Random _random = new Random();//随机发生器
            if (DateTime.Now > new DateTime(2018, 11, 1))
            {
                osn.Insert(0, "AO");
                osn.Append(Randoms.CreateRandomValue(4));//4位随机数后缀
            }
            else
            {
                osn.Append(Randoms.CreateRandomValue(6));//6位随机数后缀
            }
            return osn.ToString();
        }
        /// <summary>
        /// 根据条件获取订单
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static OrderInfo GetOrdertByWhere(string condition)
        {
            OrderInfo orderInfo = null;
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" where ");
                sb.Append(condition);
            }
            string commandText = "select top 1 * from hlh_orders " + sb.ToString();
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    orderInfo = VMall.Data.Orders.BuildOrderFromReader(reader);
                }
                reader.Close();
            }
            return orderInfo;
        }
        /// <summary>
        /// 获取用户当日订单数
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static int GetOrdertCountByToday(int uid)
        {
            DateTime today = TypeHelper.StringToDateTime(WebHelper.GetConfigSettings("LastOrderNumTime"));
            DateTime start = new DateTime(today.Year, today.Month, today.Day);

            DateTime end = start.AddDays(1);
            StringBuilder sb = new StringBuilder();

            sb.Append(" where ");
            sb.AppendFormat(" uid={0} and addtime>='{1}' and addtime<'{2}' ", uid, start.ToString("yyyy-MM-dd HH:mm:ss.fff"), end.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            string commandText = "select count(*) from hlh_orders " + sb.ToString();

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText));
        }

        /// <summary>
        /// 获取用户引流包订单数，每个会员限购一单
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static int GetYXOrdertCountByUid(int uid, int yxStoreId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" where ");
            sb.AppendFormat(" uid={0} and storeid={1} and orderstate<=140 ", uid, yxStoreId);
            string commandText = "select count(*) from hlh_orders " + sb.ToString();

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText));
        }
        /// <summary>
        /// 从订单商品列表中获得指定订单的商品列表
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetOrderProductList(int oid, List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> list = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Oid == oid)
                    list.Add(orderProductInfo);
            }
            return list;
        }

        /// <summary>
        /// 获得配送费用
        /// </summary>
        /// <param name="provinceId">省id</param>
        /// <param name="cityId">市id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static decimal GetShipFee(int provinceId, int cityId, List<OrderProductInfo> orderProductList, ref bool isSend)
        {
            List<int> storeSTidList = new List<int>(orderProductList.Count);
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                storeSTidList.Add(orderProductInfo.StoreSTid);
            }
            storeSTidList = storeSTidList.Distinct<int>().ToList<int>();

            decimal shipFee = 0;
            foreach (int storeSTId in storeSTidList)
            {
                StoreShipTemplateInfo storeShipTemplateInfo = Stores.GetStoreShipTemplateById(storeSTId);

                //不发货地区处理
                if (!string.IsNullOrEmpty(storeShipTemplateInfo.NoSendArea))
                {
                    if (StringHelper.StrContainsForNum(storeShipTemplateInfo.NoSendArea, provinceId.ToString()) && string.IsNullOrEmpty(storeShipTemplateInfo.NoSendCity))
                    {
                        isSend = false;
                    }
                    if (StringHelper.StrContainsForNum(storeShipTemplateInfo.NoSendArea, provinceId.ToString()) && !string.IsNullOrEmpty(storeShipTemplateInfo.NoSendCity))
                    {
                        if (StringHelper.StrContainsForNum(storeShipTemplateInfo.NoSendCity, cityId.ToString()))
                        {
                            isSend = false;
                        }
                    }
                }

                if (storeShipTemplateInfo.Free != 1)//不免运费以及部分免运费
                {
                    decimal totalProductAmount = 0;
                    StoreShipFeeInfo storeShipFeeInfo = null;
                    if (storeShipTemplateInfo.Free != 4)
                    {
                        totalProductAmount = Carts.SumOrderProductAmount(orderProductList.FindAll(x => x.StoreSTid == storeSTId));
                        if ((totalProductAmount >= storeShipTemplateInfo.FreeStartPrice && storeShipTemplateInfo.Free == 3) || storeShipTemplateInfo.Free == 2 || storeShipTemplateInfo.Free == 0)
                            storeShipFeeInfo = Stores.GetStoreShipFeeByStoreSTidAndRegionAndType(storeSTId, provinceId, cityId, 0);
                        else
                            storeShipFeeInfo = Stores.GetStoreShipFeeByStoreSTidAndRegionAndType(storeSTId, provinceId, cityId, 1);
                    }
                    else//快递模版根据特定条件切换快递或物流
                    {
                        decimal totalProductCount = Carts.SumOrderProductCount(orderProductList.FindAll(x => x.StoreSTid == storeSTId));
                        decimal totalProductWeight = Carts.SumOrderProductWeight(orderProductList.FindAll(x => x.StoreSTid == storeSTId));
                        if (storeShipTemplateInfo.Type == 0)
                            totalProductAmount = totalProductCount;
                        if (storeShipTemplateInfo.Type == 1)
                            totalProductAmount = totalProductWeight;
                        if (totalProductAmount <= storeShipTemplateInfo.FreeStartPrice)
                            storeShipFeeInfo = Stores.GetStoreShipFeeByStoreSTidAndRegionAndType(storeSTId, provinceId, cityId, 0);
                        else
                            storeShipFeeInfo = Stores.GetStoreShipFeeByStoreSTidAndRegionAndType(storeSTId, provinceId, cityId, 1);

                    }
                    List<OrderProductInfo> list = Carts.GetSameShipOrderProductList(storeSTId, orderProductList);

                    if (storeShipTemplateInfo.Type == 0)//按件数计算运费
                    {
                        int totalCount = Carts.SumOrderProductCount(orderProductList.FindAll(x => x.StoreSTid == storeSTId));
                        if (storeShipTemplateInfo.Free == 2 && totalProductAmount >= storeShipTemplateInfo.FreeStartPrice)//达到部分价格包邮条件
                        {
                            shipFee += 0;
                        }
                        else if (storeShipTemplateInfo.Free == 3 && totalProductAmount >= storeShipTemplateInfo.FreeStartPrice && !StringHelper.StrContainsForNum(storeShipFeeInfo.RegionId, provinceId.ToString()))//达到部分价格部分地区包邮条件
                        {
                            shipFee += 0;
                        }

                        else
                        {
                            if (totalCount <= storeShipFeeInfo.StartValue)//没有超过起步价时
                            {
                                shipFee += storeShipFeeInfo.StartFee;
                            }
                            else//超过起步价时
                            {
                                double temp = 0;
                                if ((totalCount - storeShipFeeInfo.StartValue) % storeShipFeeInfo.AddValue == 0)
                                    temp = (totalCount - storeShipFeeInfo.StartValue) / storeShipFeeInfo.AddValue;
                                else
                                    temp = Math.Ceiling((totalCount - storeShipFeeInfo.StartValue) / storeShipFeeInfo.AddValue);
                                shipFee += storeShipFeeInfo.StartFee + Convert.ToDecimal(temp) * storeShipFeeInfo.AddFee;
                            }
                        }
                    }
                    else//按重量计算运费
                    {
                        int totalWeight = Carts.SumOrderProductWeight(orderProductList.FindAll(x => x.StoreSTid == storeSTId));
                        if (storeShipTemplateInfo.Free == 2 && totalProductAmount >= storeShipTemplateInfo.FreeStartPrice)//达到部分价格包邮条件
                        {
                            shipFee += 0;
                        }
                        else if (storeShipTemplateInfo.Free == 3 && totalProductAmount >= storeShipTemplateInfo.FreeStartPrice && !StringHelper.StrContainsForNum(storeShipFeeInfo.RegionId, provinceId.ToString()))//达到部分价格部分地区包邮条件
                        {
                            shipFee += 0;
                        }
                        else
                        {
                            if (totalWeight <= storeShipFeeInfo.StartValue * 1000)//没有超过起步价时
                            {
                                shipFee += storeShipFeeInfo.StartFee;
                            }
                            else//超过起步价时
                            {
                                double temp = 0;
                                if ((totalWeight - storeShipFeeInfo.StartValue * 1000) % (storeShipFeeInfo.AddValue * 1000) == 0)
                                    temp = (totalWeight - storeShipFeeInfo.StartValue * 1000) / (storeShipFeeInfo.AddValue * 1000);
                                else
                                    temp = Math.Ceiling((totalWeight - storeShipFeeInfo.StartValue * 1000) / (storeShipFeeInfo.AddValue * 1000));
                                shipFee += storeShipFeeInfo.StartFee + Convert.ToDecimal(temp) * storeShipFeeInfo.AddFee;
                            }
                        }
                    }
                }
            }

            return shipFee;
        }

        /// <summary>
        /// 获得运费（根据店铺）海之圣旗舰店满99包邮 不满收10运费
        /// </summary>
        /// <param name="provinceId">省id</param>
        /// <param name="cityId">市id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static int GetShipFee(int storeId, decimal storeOrderamount)
        {
            int shipFee = 0;
            if ((storeId.ToString() == WebConfigurationManager.AppSettings["HealthenStoreId"] && storeOrderamount < 99) || (storeId.ToString() == WebConfigurationManager.AppSettings["QuanQiuGouStoreId"] && storeOrderamount < 100))
            {
                shipFee = 10;
            }

            return shipFee;
        }

        /// <summary>
        /// 获得双11活动产品配送费用
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static decimal GetDoublee11ShipFee(List<OrderProductInfo> orderProductList, decimal oldShipFee, GroupProductInfo gpInfo)
        {
            //不满足双11时间，返回原价运费
            DateTime startTime = TypeHelper.StringToDateTime(WebHelper.GetConfigSettings("Double11StartTime"));
            if (DateTime.Now < startTime && DateTime.Now > new DateTime(2016, 11, 12))
                return oldShipFee;
            if (gpInfo == null)
                return oldShipFee;
            decimal totalProductAmount = 0;
            List<string> gPidtmp = gpInfo.Products.Split(',').ToList();
            List<int> gPid = new List<int>();
            gPidtmp.ForEach(x =>
            {
                gPid.Add(TypeHelper.StringToInt(x));
            });
            //List<int> exceptPid = (List<int>)orderProductList.Select(x => x.Pid).Except(gPid);
            if (!orderProductList.Exists(x => gPid.Contains(x.Pid)))
                return oldShipFee;
            if (gpInfo != null)
                totalProductAmount = Carts.SumOrderProductAmount(orderProductList.FindAll(x => !gPid.Contains(x.Pid)));
            else
                return oldShipFee;
            if (oldShipFee >= 18 && totalProductAmount >= 199 && totalProductAmount < 299)
                oldShipFee = oldShipFee - 18;
            if (totalProductAmount > 299)
            {
                decimal amountLevel = (totalProductAmount - 199) / 100;
                decimal shipLevel = (oldShipFee - 18) / 10;
                if (amountLevel >= shipLevel)
                    oldShipFee = 0;
                else
                    oldShipFee = (shipLevel - Math.Floor(amountLevel)) * 10;
            }

            return oldShipFee;
        }

        #endregion

        #region 获得订单。以及订单产品相关
        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns>订单信息</returns>
        public static OrderInfo GetOrderByOid(int oid)
        {
            if (oid > 0)
                return VMall.Data.Orders.GetOrderByOid(oid);
            else
                return null;
        }

        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="osn">订单编号</param>
        /// <returns>订单信息</returns>
        public static OrderInfo GetOrderByOSN(string osn)
        {
            if (!string.IsNullOrWhiteSpace(osn))
                return VMall.Data.Orders.GetOrderByOSN(osn);
            return null;
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
            return VMall.Data.Orders.GetOrderCountByCondition(storeId, orderState, startTime, endTime);
        }

        /// <summary>
        /// 获得订单商品列表
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetOrderProductList(int oid)
        {
            return VMall.Data.Orders.GetOrderProductList(oid);
        }

        /// <summary>
        /// 获得订单商品列表
        /// </summary>
        /// <param name="oidList">订单id列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetOrderProductList(string oidList)
        {
            if (!string.IsNullOrEmpty(oidList))
                return VMall.Data.Orders.GetOrderProductList(oidList);
            return new List<OrderProductInfo>();
        }
        /// <summary>
        /// 获得订单产品数
        /// </summary>
        /// <param name="Pid"></param>
        /// <param name="Uid"></param>
        /// <returns></returns>
        public static int GetOrderProductCountByPidAndUid(int Pid, int Uid)
        {

            string commandText = string.Format(@"SELECT SUM(buycount) FROM dbo.hlh_orderproducts WHERE pid={0} AND uid={1} AND oid IN (
	SELECT oid FROM dbo.hlh_orders WHERE orderstate<70 AND uid={2})", Pid, Uid, Uid);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText), 0);
        }

        /// <summary>
        /// 获得当月订单产品数，订单状态为等待付款-已完成
        /// </summary>
        /// <param name="Pid"></param>
        /// <param name="Uid"></param>
        /// <returns></returns>
        public static int GetOrderProductCountByPidAndUidForMonth(int Pid, int Uid, DateTime time)
        {
            DateTime start = new DateTime(time.Year, time.Month, 1);
            DateTime end = start.AddMonths(1);
            StringBuilder sb = new StringBuilder();
            string commandText = string.Format(@"SELECT SUM(buycount) FROM dbo.hlh_orderproducts WHERE pid={0} AND uid={1} AND oid IN (
	SELECT oid FROM dbo.hlh_orders WHERE addtime>'{3}' AND addtime<'{4}' AND orderstate>=30 AND orderstate<=140 AND uid={2})", Pid, Uid, Uid, start, end);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText), 0);
        }
        #endregion

        #region 订单操作

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="storeInfo">店铺信息</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="singlePromotionList">单品促销活动列表</param>
        /// <param name="fullShipAddressInfo">配送地址</param>
        /// <param name="payPluginInfo">支付方式</param>
        /// <param name="payCreditCount">支付积分数</param>
        /// <param name="couponList">优惠劵列表</param>
        /// <param name="fullCut">满减</param>
        /// <param name="buyerRemark">买家备注</param>
        /// <param name="bestTime">最佳配送时间</param>
        /// <param name="ip">ip地址</param>
        /// <returns>订单信息</returns>
        public static OrderInfo CreateOrder(PartUserInfo partUserInfo, StoreInfo storeInfo, List<OrderProductInfo> orderProductList, List<SinglePromotionInfo> singlePromotionList, FullShipAddressInfo fullShipAddressInfo, PluginInfo payPluginInfo, ref int payCreditCount, ref decimal haiMiCount, ref decimal hongBaoCount, ref decimal cashCount, List<CouponInfo> couponList, ref decimal couponMoeny, ref decimal daiLiCount, ref  decimal YongJinCount, int fullCut, string buyerRemark, string invoice, DateTime bestTime, string ip, string cashId, List<CashCouponInfo> selectCashList, List<ExChangeCouponsInfo> selectExCodeList = null, string invoicemore = "")
        {
            if (orderProductList.Count() <= 0)
                return null;
            DateTime nowTime = DateTime.Now;
            IPayPlugin payPlugin = (IPayPlugin)payPluginInfo.Instance;

            OrderInfo orderInfo = new OrderInfo();


            orderInfo.Uid = partUserInfo.Uid;

            orderInfo.Weight = Carts.SumOrderProductWeight(orderProductList);
            orderInfo.ProductAmount = Carts.SumOrderProductAmount(orderProductList);
            orderInfo.FullCut = fullCut;
            GroupProductInfo gpInfo = new GroupProducts().GetModel(3);
            bool isSend = true;
            orderInfo.ShipFee = GetShipFee(fullShipAddressInfo.ProvinceId, fullShipAddressInfo.CityId, orderProductList, ref isSend);// GetShipFee(storeInfo.StoreId, orderInfo.ProductAmount);// 
            decimal oldShipFee = orderInfo.ShipFee;
            orderInfo.ShipFee = Orders.GetDoublee11ShipFee(orderProductList, oldShipFee, gpInfo);
            orderInfo.PayFee = payPlugin.GetPayFee(orderInfo.ProductAmount - orderInfo.FullCut, nowTime, partUserInfo);
            orderInfo.TaxAmount = Carts.SumOrderProductTaxAmount(orderProductList);
            orderInfo.OrderAmount = orderInfo.ProductAmount - orderInfo.FullCut + orderInfo.ShipFee + orderInfo.PayFee + (orderInfo.TaxAmount > 50 ? orderInfo.TaxAmount : 0);

            #region 订单使用优惠及抵扣
            decimal remainStoreAmount = orderInfo.OrderAmount;

            decimal payCreditMoney = Credits.PayCreditsToMoney(payCreditCount);
            if (orderInfo.OrderAmount >= payCreditMoney)
            {
                orderInfo.PayCreditCount = payCreditCount;
                orderInfo.PayCreditMoney = payCreditMoney;
                remainStoreAmount = orderInfo.OrderAmount - payCreditMoney;
                payCreditCount = 0;

            }
            else
            {
                int orderPayCredits = Credits.MoneyToPayCredits(orderInfo.OrderAmount);
                orderInfo.PayCreditCount = orderPayCredits;
                orderInfo.PayCreditMoney = orderInfo.OrderAmount;
                payCreditCount = payCreditCount - orderPayCredits;
                remainStoreAmount = 0;
            }


            //红包减免
            decimal storeHongbao = Carts.SumOrderProductHongBaoCutAmount(orderProductList);
            if (remainStoreAmount > 0)
            {
                if (storeHongbao > 0 && hongBaoCount > 0)
                {
                    //if (storeHongbao >= hongBaoCount)
                    //{
                    //    orderInfo.HongBaoDiscount = hongBaoCount;
                    //    hongBaoCount = 0;
                    //}
                    // else
                    // {
                    if (hongBaoCount < storeHongbao)
                    {
                        orderInfo.HongBaoDiscount = hongBaoCount;
                        remainStoreAmount = remainStoreAmount - hongBaoCount;
                        hongBaoCount = 0;
                    }
                    else
                    {
                        orderInfo.HongBaoDiscount = storeHongbao;
                        hongBaoCount = hongBaoCount - storeHongbao;
                        remainStoreAmount = remainStoreAmount - storeHongbao;
                        //remainStoreAmount = 0;
                    }
                    // }
                }
            }
            // orderInfo.HongBaoDiscount = hongBaoCount;
            //汇购卡券--只能旗舰店专区产品使用
            decimal storeCash = Carts.SumOrderProductCashCutAmount(orderProductList, orderInfo.Uid, 1, 2);
            if (remainStoreAmount > 0)
            {
                if (storeCash > 0 && cashCount > 0)
                {
                    if (cashCount < storeCash)
                    {
                        orderInfo.CashDiscount = cashCount;
                        remainStoreAmount = remainStoreAmount - cashCount;
                        cashCount = 0;
                    }
                    else
                    {
                        orderInfo.CashDiscount = storeCash;
                        cashCount = cashCount - storeCash;
                        remainStoreAmount = remainStoreAmount - storeCash;
                        //remainStoreAmount = 0;
                    }
                    //orderInfo.CashDiscount = storeCash;
                    //cashCount = cashCount - storeCash;
                }
            }
            //海米抵现
            if (remainStoreAmount > 0)
            {
                if (remainStoreAmount >= haiMiCount)
                {
                    orderInfo.HaiMiDiscount = haiMiCount;
                    remainStoreAmount = remainStoreAmount - haiMiCount;
                    haiMiCount = 0;
                }
                else
                {
                    orderInfo.HaiMiDiscount = remainStoreAmount;
                    haiMiCount = haiMiCount - remainStoreAmount;
                    remainStoreAmount = 0;
                }
            }
            //优惠券抵现
            if (remainStoreAmount > 0)
            {
                if (remainStoreAmount >= couponMoeny)
                {
                    orderInfo.CouponMoney = couponMoeny;
                    remainStoreAmount = remainStoreAmount - couponMoeny;
                    couponMoeny = 0;
                }
                else
                {
                    orderInfo.CouponMoney = remainStoreAmount;
                    couponMoeny = couponMoeny - remainStoreAmount;
                    remainStoreAmount = 0;
                }
            }
            //代理、佣金账户抵现
            //代理抵现
            if (remainStoreAmount > 0)
            {
                if (remainStoreAmount >= daiLiCount)
                {
                    orderInfo.AgentDiscount = daiLiCount;
                    remainStoreAmount = remainStoreAmount - daiLiCount;
                    daiLiCount = 0;
                }
                else
                {
                    orderInfo.AgentDiscount = remainStoreAmount;
                    daiLiCount = daiLiCount - remainStoreAmount;
                    remainStoreAmount = 0;
                }
            }
            //佣金抵现
            if (remainStoreAmount > 0)
            {
                if (remainStoreAmount >= YongJinCount)
                {
                    orderInfo.CommisionDiscount = YongJinCount;
                    remainStoreAmount = remainStoreAmount - YongJinCount;
                    YongJinCount = 0;
                }
                else
                {
                    orderInfo.CommisionDiscount = remainStoreAmount;
                    YongJinCount = YongJinCount - remainStoreAmount;
                    remainStoreAmount = 0;
                }
            }
            #endregion

            #region
            //decimal storeAgent = Carts.SumOrderProductAgentCutAmount(orderProductList, storeInfo);//当前代理店铺可用抵现金额
            //decimal remainStoreAgent = 0;
            ////代理账户
            //if (storeAgent > 0 && daiLiCount > 0)
            //{
            //    if (daiLiCount < storeAgent)
            //    {
            //        orderInfo.AgentDiscount = daiLiCount;
            //        daiLiCount = 0;
            //        remainStoreAgent = storeAgent - daiLiCount;
            //    }
            //    else
            //    {
            //        orderInfo.AgentDiscount = storeAgent;
            //        daiLiCount = daiLiCount - storeAgent;
            //        remainStoreAgent = 0;
            //    }
            //}
            ////佣金账户
            //if (remainStoreAgent > 0 && YongJinCount > 0)
            //{
            //    if (YongJinCount < remainStoreAgent)
            //    {
            //        orderInfo.CommisionDiscount = YongJinCount;
            //        YongJinCount = 0;

            //    }
            //    else
            //    {
            //        orderInfo.CommisionDiscount = remainStoreAgent;
            //        YongJinCount = YongJinCount - remainStoreAgent;

            //    }
            //}
            #endregion

            //orderInfo.CouponMoney = Coupons.SumCouponMoney(couponList);
            orderInfo.SurplusMoney = orderInfo.OrderAmount - orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount - orderInfo.CashDiscount - orderInfo.AgentDiscount - orderInfo.CommisionDiscount;
            if (orderInfo.SurplusMoney < 0)
                return null;
            orderInfo.OrderState = (orderInfo.SurplusMoney <= 0 || payPlugin.PayMode == 0) ? (int)OrderState.Confirming : (int)OrderState.WaitPaying;

            orderInfo.ParentId = 0;
            orderInfo.IsReview = 0;
            orderInfo.AddTime = nowTime;
            orderInfo.StoreId = storeInfo.StoreId;
            orderInfo.StoreName = storeInfo.Name;
            orderInfo.PaySystemName = payPluginInfo.SystemName;
            orderInfo.PayFriendName = payPluginInfo.FriendlyName;
            orderInfo.PayMode = payPlugin.PayMode;

            orderInfo.RegionId = fullShipAddressInfo.RegionId;
            orderInfo.Consignee = fullShipAddressInfo.Consignee;
            orderInfo.Mobile = fullShipAddressInfo.Mobile;
            orderInfo.Phone = fullShipAddressInfo.Phone;
            orderInfo.Email = fullShipAddressInfo.Email;
            orderInfo.ZipCode = fullShipAddressInfo.ZipCode;
            orderInfo.Address = fullShipAddressInfo.Address;
            orderInfo.BestTime = bestTime;
            if (StringHelper.StrContainsForNum(WebConfigurationManager.AppSettings["QuanQiuGouStoreId"], storeInfo.StoreId.ToString()))//全球购订单 
                orderInfo.BuyerRemark = buyerRemark;
            else
                orderInfo.BuyerRemark = buyerRemark.Split('-')[0];
            //orderInfo.BuyerRemark = buyerRemark;
            orderInfo.Invoice = invoice;
            orderInfo.IP = ip;

            if (StringHelper.StrContainsForNum(WebConfigurationManager.AppSettings["QuanQiuGouStoreId"], storeInfo.StoreId.ToString()))//全球购订单 
            {
                orderInfo.OrderSource = (int)OrderSource.全球购;
            }
            //else if (storeInfo.StoreId.ToString() == WebConfigurationManager.AppSettings["HealthenStoreId"])
            //{
            //    orderInfo.OrderSource = (int)OrderSource.直销后台;
            //}
            else
            {
                orderInfo.OrderSource = (int)OrderSource.自营商城;
            }
            orderInfo.MallSource = storeInfo.MallSource;

            //直销会员购买和治友品牌产品，买一送一 pv减半,不使用汇购卡支付
            //直销会员购买和治友德旗舰店专区产品，买一送一 pv减半,不使用汇购卡支付 ---20180808 新政策
            if (partUserInfo.IsDirSaleUser && orderInfo.CashDiscount <= 0)
            {
                Carts.UpdateCartForHZYDSend(orderProductList);
            }
            orderInfo.OSN = GenerateOSN(storeInfo.StoreId, partUserInfo.Uid, fullShipAddressInfo.RegionId, nowTime, payPlugin.PayMode.ToString(), orderInfo.OrderSource.ToString(), orderInfo.OrderAmount);
            try
            {
                //添加订单
                int oid = _iorderstrategy.CreateOrder(orderInfo, Carts.IsPersistOrderProduct, orderProductList);
                if (oid > 0)
                {

                    orderInfo.Oid = oid;

                    if (!string.IsNullOrEmpty(invoicemore))
                    {
                        UpdateOrderInvoiceMore(orderInfo.Oid, invoicemore);
                    }
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = orderInfo.Oid,
                        Uid = orderInfo.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Submit,
                        ActionTime = DateTime.Now,
                        ActionDes = orderInfo.OrderState == (int)OrderState.WaitPaying ? "您提交了订单，等待您付款" : "您提交了订单，请等待系统确认"
                    });
                    //减少商品库存数量
                    Products.DecreaseProductStockNumber(orderProductList);
                    //更新限购库存
                    if (singlePromotionList.Count > 0)
                        Promotions.UpdateSinglePromotionStock(singlePromotionList);
                    //使用支付积分
                    Credits.PayOrder(ref partUserInfo, orderInfo, orderInfo.PayCreditCount, nowTime);


                    //使用海米记录
                    if (orderInfo.HaiMiDiscount > 0)
                    {
                        //更新直销的账户
                        if (partUserInfo.IsDirSaleUser)
                        {
                            AccountUtils.UpdateAccountForDir(partUserInfo.DirSaleUid, (int)AccountType.商城钱包, 0, orderInfo.HaiMiDiscount, orderInfo.OSN, "订单" + MallKey.MallDiscountName_JiangJin + "抵现：订单号:" + orderInfo.OSN + ",抵现金额:" + orderInfo.HaiMiDiscount);
                        }
                        //更新海汇账户
                        else
                        {
                            Account.UpdateAccountForOut(new AccountInfo()
                            {
                                AccountId = (int)AccountType.商城钱包,
                                UserId = partUserInfo.Uid,
                                TotalOut = orderInfo.HaiMiDiscount
                            });
                            Account.CreateAccountDetail(new AccountDetailInfo()
                            {
                                AccountId = (int)AccountType.商城钱包,
                                UserId = partUserInfo.Uid,
                                CreateTime = DateTime.Now,
                                DetailType = (int)DetailType.订单抵现支出,
                                OutAmount = orderInfo.HaiMiDiscount,
                                OrderCode = orderInfo.OSN,
                                AdminUid = partUserInfo.Uid,//system
                                Status = 1,
                                DetailDes = "订单" + MallKey.MallDiscountName_JiangJin + "抵现：订单号:" + orderInfo.OSN + ",抵现金额:" + orderInfo.HaiMiDiscount
                            });
                        }
                    }
                    //红包记录
                    if (orderInfo.HongBaoDiscount > 0)
                    {
                        //更新直销的账户
                        if (partUserInfo.IsDirSaleUser)
                        {
                            AccountUtils.UpdateAccountForDir(partUserInfo.DirSaleUid, (int)AccountType.积分账户, 0, orderInfo.HongBaoDiscount, orderInfo.OSN, "订单" + MallKey.MallDiscountName_JiFen + "减免抵现：订单号:" + orderInfo.OSN + ",抵现金额:" + orderInfo.HongBaoDiscount);
                        }
                        //更新海汇账户
                        else
                        {
                            Account.UpdateAccountForOut(new AccountInfo()
                            {
                                AccountId = (int)AccountType.积分账户,
                                UserId = partUserInfo.Uid,
                                TotalOut = orderInfo.HongBaoDiscount
                            });
                            Account.CreateAccountDetail(new AccountDetailInfo()
                            {
                                AccountId = (int)AccountType.积分账户,
                                UserId = partUserInfo.Uid,
                                CreateTime = DateTime.Now,
                                DetailType = (int)DetailType.订单抵现支出,
                                OutAmount = orderInfo.HongBaoDiscount,
                                OrderCode = orderInfo.OSN,
                                AdminUid = partUserInfo.Uid,//system
                                Status = 1,
                                DetailDes = "订单" + MallKey.MallDiscountName_JiFen + "减免抵现：订单号:" + orderInfo.OSN + ",抵现金额:" + orderInfo.HongBaoDiscount
                            });
                        }
                    }
                    //汇购卡券使用
                    if (orderInfo.CashDiscount > 0 && !string.IsNullOrEmpty(cashId))
                    {
                        selectCashList.ForEach(x =>
                        {
                            CashCouponInfo cashInfo = CashCoupon.GetModel(x.CashId);
                            if (cashInfo != null)
                            {
                                x.Banlance = cashInfo.Banlance;
                            }

                        });
                        if (selectCashList.Count > 0)
                        {
                            ///小于第一个勾选，使用第一个
                            if (orderInfo.CashDiscount <= selectCashList.FirstOrDefault().Banlance)
                            {
                                CashCoupon.UpdateCashForOut(new CashCouponInfo()
                                {
                                    CashId = selectCashList.FirstOrDefault().CashId,
                                    Uid = partUserInfo.Uid,
                                    TotalOut = orderInfo.CashDiscount
                                });
                                CashCouponDetail.Add(new CashCouponDetailInfo()
                                {
                                    CreationDate = DateTime.Now,
                                    CashId = selectCashList.FirstOrDefault().CashId,
                                    Uid = partUserInfo.Uid,
                                    DetailType = (int)CashDetailType.订单使用抵现,
                                    OutAmount = orderInfo.CashDiscount,
                                    Oid = orderInfo.Oid,
                                    OSN = orderInfo.OSN,
                                    DetailDes = "订单汇购卡券抵现：订单号:" + orderInfo.OSN + ",抵现金额:" + orderInfo.CashDiscount,
                                    Status = 1,
                                    DirSaleUid = partUserInfo.DirSaleUid
                                });
                            }
                            else //使用多个合并
                            {
                                decimal allUsetCashCount = 0;
                                decimal leftCashCount = orderInfo.CashDiscount; ;
                                foreach (var item in selectCashList)
                                {
                                    if (leftCashCount > 0)
                                    {
                                        decimal outCash = 0M;
                                        CashCouponInfo cashInfo = CashCoupon.GetModel(item.CashId);
                                        if (cashInfo.Banlance <= 0)
                                            continue;
                                        if (leftCashCount >= item.Banlance)
                                            outCash = item.Banlance;
                                        else
                                            outCash = leftCashCount;
                                        CashCoupon.UpdateCashForOut(new CashCouponInfo()
                                        {
                                            CashId = item.CashId,
                                            Uid = partUserInfo.Uid,
                                            TotalOut = outCash
                                        });
                                        CashCouponDetail.Add(new CashCouponDetailInfo()
                                        {
                                            CreationDate = DateTime.Now,
                                            CashId = item.CashId,
                                            Uid = partUserInfo.Uid,
                                            DetailType = (int)CashDetailType.订单使用抵现,
                                            OutAmount = outCash,
                                            Oid = orderInfo.Oid,
                                            OSN = orderInfo.OSN,
                                            DetailDes = "订单汇购卡券抵现(合并汇购卡支付)：订单号:" + orderInfo.OSN + ",抵现金额:" + outCash,
                                            Status = 1,
                                            DirSaleUid = partUserInfo.DirSaleUid
                                        });
                                        allUsetCashCount += outCash;
                                        leftCashCount = orderInfo.CashDiscount - allUsetCashCount;
                                    }
                                }
                            }
                        }
                        //使用汇购卡支付的产品pv海米清零
                        UpdateCashOrderPVAndHMTo0(oid);
                    }
                    //使用优惠劵
                    foreach (CouponInfo couponInfo in couponList)
                    {
                        if (couponInfo.Uid > 0)
                            Coupons.UseCoupon(couponInfo.CouponId, oid, nowTime, ip);
                        else
                            Coupons.ActivateAndUseCoupon(couponInfo.CouponId, partUserInfo.Uid, oid, nowTime, ip);
                    }
                    //使用兑换码
                    foreach (var item in selectExCodeList)
                    {
                        ExChangeCouponsInfo exInfo = item;
                        exInfo.oid = orderInfo.Oid;
                        exInfo.usetime = DateTime.Now;
                        exInfo.useip = ip;
                        ExChangeCoupons.Update(exInfo);
                    }
                    //使用代理账户
                    if (orderInfo.AgentDiscount > 0)
                    {
                        //更新汇购账户，代理账户信息存放在汇购系统中
                        Account.UpdateAccountForOut(new AccountInfo()
                        {
                            AccountId = (int)AccountType.代理账户,
                            UserId = partUserInfo.Uid,
                            TotalOut = orderInfo.AgentDiscount
                        });
                        Account.CreateAccountDetail(new AccountDetailInfo()
                        {
                            AccountId = (int)AccountType.代理账户,
                            UserId = partUserInfo.Uid,
                            CreateTime = DateTime.Now,
                            DetailType = (int)DetailType.订单抵现支出,
                            OutAmount = orderInfo.AgentDiscount,
                            OrderCode = orderInfo.OSN,
                            AdminUid = partUserInfo.Uid,//system
                            Status = 1,
                            DetailDes = "订单抵现：订单号:" + orderInfo.OSN + ",抵现金额:" + orderInfo.AgentDiscount
                        });
                    }
                    //使用佣金账户
                    if (orderInfo.CommisionDiscount > 0)
                    {
                        //更新汇购账户，佣金账户信息存放在汇购系统中
                        Account.UpdateAccountForOut(new AccountInfo()
                        {
                            AccountId = (int)AccountType.佣金账户,
                            UserId = partUserInfo.Uid,
                            TotalOut = orderInfo.CommisionDiscount
                        });
                        Account.CreateAccountDetail(new AccountDetailInfo()
                        {
                            AccountId = (int)AccountType.佣金账户,
                            UserId = partUserInfo.Uid,
                            CreateTime = DateTime.Now,
                            DetailType = (int)DetailType.订单抵现支出,
                            OutAmount = orderInfo.CommisionDiscount,
                            OrderCode = orderInfo.OSN,
                            AdminUid = partUserInfo.Uid,//system
                            Status = 1,
                            DetailDes = "订单抵现：订单号:" + orderInfo.OSN + ",抵现金额:" + orderInfo.CommisionDiscount
                        });
                    }

                    //全额抵扣自动确认
                    if (orderInfo.SurplusMoney <= 0 && orderInfo.OrderState == (int)OrderState.Confirming)
                    {
                        Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, string.Empty, DateTime.Now);
                        Orders.ConfirmOrder(orderInfo);
                        if (orderInfo.StoreId != WebHelper.GetConfigSettingsInt("AgentStoreId"))
                        {
                            //orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount;
                            String showStr = "您使用" + (orderInfo.CouponMoney > 0 ? orderInfo.CouponMoney.ToString("0.00") + "优惠券、" : "") + (orderInfo.HaiMiDiscount > 0 ? orderInfo.HaiMiDiscount.ToString("0.00") + MallKey.MallDiscountName_JiangJin : "") + (orderInfo.HongBaoDiscount > 0 ? "、" + orderInfo.HongBaoDiscount.ToString("0.00") + MallKey.MallDiscountName_JiFen : "") + (orderInfo.CashDiscount > 0 ? "、" + orderInfo.CashDiscount.ToString("0.00") + "汇购卡" : "") + (orderInfo.AgentDiscount > 0 ? "、" + orderInfo.AgentDiscount.ToString("0.00") + "代理账户" : "") + (orderInfo.CommisionDiscount > 0 ? "、" + orderInfo.CommisionDiscount.ToString("0.00") + "佣金账户" : "") + "抵扣订单金额";
                            OrderActions.CreateOrderAction(new OrderActionInfo()
                            {
                                Oid = orderInfo.Oid,
                                Uid = orderInfo.Uid,
                                RealName = "系统",
                                ActionType = (int)OrderActionType.Confirm,
                                ActionTime = DateTime.Now,//交易时间,
                                ActionDes = showStr + "，您的订单已经确认,正在备货中"
                            });
                        }
                    }

                    return orderInfo;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return null;
        }

        /// <summary>
        /// 创建订单-APP
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="storeInfo">店铺信息</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="singlePromotionList">单品促销活动列表</param>
        /// <param name="fullShipAddressInfo">配送地址</param>
        /// <param name="payPluginInfo">支付方式</param>
        /// <param name="payCreditCount">支付积分数</param>
        /// <param name="couponList">优惠劵列表</param>
        /// <param name="fullCut">满减</param>
        /// <param name="buyerRemark">买家备注</param>
        /// <param name="bestTime">最佳配送时间</param>
        /// <param name="ip">ip地址</param>
        /// <returns>订单信息</returns>
        public static OrderInfo CreateAPPOrder(PartUserInfo partUserInfo, StoreInfo storeInfo, List<OrderProductInfo> orderProductList, List<SinglePromotionInfo> singlePromotionList, FullShipAddressInfo fullShipAddressInfo, PluginInfo payPluginInfo, List<CouponInfo> couponList, ref decimal couponMoeny, ref  decimal YongJinCount, int fullCut, string buyerRemark, string invoice, DateTime bestTime, string ip = "")
        {
            if (orderProductList.Count() <= 0)
                return null;
            DateTime nowTime = DateTime.Now;
            //IPayPlugin payPlugin = (IPayPlugin)payPluginInfo.Instance;

            OrderInfo orderInfo = new OrderInfo();

            orderInfo.Uid = partUserInfo.Uid;

            orderInfo.Weight = Carts.SumOrderProductWeight(orderProductList);
            orderInfo.ProductAmount = Carts.SumOrderProductAmount(orderProductList);
            orderInfo.FullCut = fullCut;
            GroupProductInfo gpInfo = new GroupProducts().GetModel(3);
            bool isSend = true;
            orderInfo.ShipFee = GetShipFee(fullShipAddressInfo.ProvinceId, fullShipAddressInfo.CityId, orderProductList, ref isSend);// GetShipFee(storeInfo.StoreId, orderInfo.ProductAmount);// 
            orderInfo.PayFee = 0;// payPlugin.GetPayFee(orderInfo.ProductAmount - orderInfo.FullCut, nowTime, partUserInfo);
            orderInfo.TaxAmount = Carts.SumOrderProductTaxAmount(orderProductList);
            orderInfo.OrderAmount = orderInfo.ProductAmount - orderInfo.FullCut + orderInfo.ShipFee + orderInfo.PayFee + (orderInfo.TaxAmount > 50 ? orderInfo.TaxAmount : 0);

            #region 订单使用优惠及抵扣
            decimal remainStoreAmount = orderInfo.OrderAmount;

            //优惠券抵现
            if (remainStoreAmount > 0)
            {
                if (remainStoreAmount >= couponMoeny)
                {
                    orderInfo.CouponMoney = couponMoeny;
                    remainStoreAmount = remainStoreAmount - couponMoeny;
                    couponMoeny = 0;
                }
                else
                {
                    orderInfo.CouponMoney = remainStoreAmount;
                    couponMoeny = couponMoeny - remainStoreAmount;
                    remainStoreAmount = 0;
                }
            }

            //佣金抵现
            if (remainStoreAmount > 0)
            {
                if (remainStoreAmount >= YongJinCount)
                {
                    orderInfo.CommisionDiscount = YongJinCount;
                    remainStoreAmount = remainStoreAmount - YongJinCount;
                    YongJinCount = 0;
                }
                else
                {
                    orderInfo.CommisionDiscount = remainStoreAmount;
                    YongJinCount = YongJinCount - remainStoreAmount;
                    remainStoreAmount = 0;
                }
            }
            #endregion

            orderInfo.SurplusMoney = orderInfo.OrderAmount - orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount - orderInfo.CashDiscount - orderInfo.AgentDiscount - orderInfo.CommisionDiscount;
            if (orderInfo.SurplusMoney < 0)
                return null;
            // orderInfo.OrderState = (orderInfo.SurplusMoney <= 0 || payPlugin.PayMode == 0) ? (int)OrderState.Confirming : (int)OrderState.WaitPaying;
            orderInfo.OrderState = (orderInfo.SurplusMoney <= 0) ? (int)OrderState.Confirming : (int)OrderState.WaitPaying;

            orderInfo.ParentId = 0;
            orderInfo.IsReview = 0;
            orderInfo.AddTime = nowTime;
            orderInfo.StoreId = storeInfo.StoreId;
            orderInfo.StoreName = storeInfo.Name;
            orderInfo.PaySystemName = payPluginInfo.SystemName;
            orderInfo.PayFriendName = payPluginInfo.FriendlyName;
            orderInfo.PayMode = 1;

            orderInfo.RegionId = fullShipAddressInfo.RegionId;
            orderInfo.Consignee = fullShipAddressInfo.Consignee;
            orderInfo.Mobile = fullShipAddressInfo.Mobile;
            orderInfo.Phone = fullShipAddressInfo.Phone;
            orderInfo.Email = fullShipAddressInfo.Email;
            orderInfo.ZipCode = fullShipAddressInfo.ZipCode;
            orderInfo.Address = fullShipAddressInfo.Address;
            orderInfo.BestTime = bestTime;
            orderInfo.BuyerRemark = buyerRemark;
            orderInfo.Invoice = invoice;
            orderInfo.IP = WebHelper.GetIP();
            orderInfo.OrderSource = (int)OrderSource.app端;
            orderInfo.MallSource = storeInfo.MallSource;

            orderInfo.OSN = GenerateOSN(storeInfo.StoreId, partUserInfo.Uid, fullShipAddressInfo.RegionId, nowTime, "1", orderInfo.OrderSource.ToString(), orderInfo.OrderAmount);
            try
            {
                //添加订单
                int oid = _iorderstrategy.CreateOrder(orderInfo, Carts.IsPersistOrderProduct, orderProductList);
                if (oid > 0)
                {
                    orderInfo.Oid = oid;

                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = orderInfo.Oid,
                        Uid = orderInfo.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Submit,
                        ActionTime = DateTime.Now,
                        ActionDes = orderInfo.OrderState == (int)OrderState.WaitPaying ? "您提交了订单，等待您付款" : "您提交了订单，请等待系统确认"
                    });
                    //减少商品库存数量
                    Products.DecreaseProductStockNumber(orderProductList);
                    //更新限购库存
                    if (singlePromotionList.Count > 0)
                        Promotions.UpdateSinglePromotionStock(singlePromotionList);
                    //使用支付积分
                    //Credits.PayOrder(ref partUserInfo, orderInfo, orderInfo.PayCreditCount, nowTime);

                    //使用优惠劵
                    foreach (CouponInfo couponInfo in couponList)
                    {
                        if (couponInfo.Uid > 0)
                            Coupons.UseCoupon(couponInfo.CouponId, oid, nowTime, ip);
                        else
                            Coupons.ActivateAndUseCoupon(couponInfo.CouponId, partUserInfo.Uid, oid, nowTime, ip);
                    }
                    //使用佣金账户
                    if (orderInfo.CommisionDiscount > 0)
                    {
                        //更新汇购账户，佣金账户信息存放在汇购系统中
                        Account.UpdateAccountForOut(new AccountInfo()
                        {
                            AccountId = (int)AccountType.佣金账户,
                            UserId = partUserInfo.Uid,
                            TotalOut = orderInfo.CommisionDiscount
                        });
                        Account.CreateAccountDetail(new AccountDetailInfo()
                        {
                            AccountId = (int)AccountType.佣金账户,
                            UserId = partUserInfo.Uid,
                            CreateTime = DateTime.Now,
                            DetailType = (int)DetailType.订单抵现支出,
                            OutAmount = orderInfo.CommisionDiscount,
                            OrderCode = orderInfo.OSN,
                            AdminUid = partUserInfo.Uid,//system
                            Status = 1,
                            DetailDes = "订单抵现：订单号:" + orderInfo.OSN + ",抵现金额:" + orderInfo.CommisionDiscount
                        });
                    }

                    //全额抵扣自动确认
                    if (orderInfo.SurplusMoney <= 0 && orderInfo.OrderState == (int)OrderState.Confirming)
                    {
                        Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, string.Empty, DateTime.Now);
                        Orders.ConfirmOrder(orderInfo);
                        if (orderInfo.StoreId != WebHelper.GetConfigSettingsInt("AgentStoreId"))
                        {
                            String showStr = "您使用" + (orderInfo.CouponMoney > 0 ? orderInfo.CouponMoney.ToString("0.00") + "优惠券、" : "") + (orderInfo.HaiMiDiscount > 0 ? orderInfo.HaiMiDiscount.ToString("0.00") + MallKey.MallDiscountName_JiangJin : "") + (orderInfo.HongBaoDiscount > 0 ? "、" + orderInfo.HongBaoDiscount.ToString("0.00") + MallKey.MallDiscountName_JiFen : "") + (orderInfo.CashDiscount > 0 ? "、" + orderInfo.CashDiscount.ToString("0.00") + "汇购卡" : "") + (orderInfo.AgentDiscount > 0 ? "、" + orderInfo.AgentDiscount.ToString("0.00") + "代理账户" : "") + (orderInfo.CommisionDiscount > 0 ? "、" + orderInfo.CommisionDiscount.ToString("0.00") + "佣金账户" : "") + "抵扣订单金额";
                            OrderActions.CreateOrderAction(new OrderActionInfo()
                            {
                                Oid = orderInfo.Oid,
                                Uid = orderInfo.Uid,
                                RealName = "系统",
                                ActionType = (int)OrderActionType.Confirm,
                                ActionTime = DateTime.Now,//交易时间,
                                ActionDes = showStr + "，您的订单已经确认,正在备货中"
                            });
                        }
                    }

                    return orderInfo;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return null;
        }

        /// <summary>
        /// 创建订货系统订单
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="storeInfo">店铺信息</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="singlePromotionList">单品促销活动列表</param>
        /// <param name="payPluginInfo">支付方式</param>
        /// <param name="couponList">优惠劵列表</param>
        /// <param name="ip">ip地址</param>
        /// <returns>订单信息</returns>
        public static OrderInfo CreateOrder_Agent(PartUserInfo partUserInfo, StoreInfo storeInfo, PartProductInfo productInfo, PluginInfo payPluginInfo, string consignee, string mobile, int regionid, string address, string ip, int buyCount, int pid, string buyerRemark, int againmobile, string parentmobile, string wechat, int ordertype, PartUserInfo operateUserInfo)
        {
            if (productInfo == null)
                return null;
            UserInfo userInfo = null;
            if (ordertype == 1)
            {
                if (!string.IsNullOrEmpty(mobile) && ValidateHelper.IsMobile(mobile))
                {
                    if (Users.IsExistMobile(mobile))
                        partUserInfo = Users.GetPartUserByMobile(mobile);
                    else
                    {
                        userInfo = new UserInfo() { UserName = "", Email = string.Empty, Mobile = mobile };

                        #region 初始化用户信息
                        string nickName = string.Empty;
                        //生成随机初始密码并发送短信
                        string password = "123456";//初始密码123456
                        userInfo.Salt = Randoms.CreateRandomValue(6);
                        userInfo.Password = Users.CreateUserPassword(password, userInfo.Salt);
                        userInfo.UserRid = UserRanks.GetLowestUserRank().UserRid;
                        userInfo.StoreId = 0;
                        userInfo.MallAGid = 1;//非管理员组
                        if (nickName.Length > 0)
                            userInfo.NickName = WebHelper.HtmlEncode(nickName);
                        else
                            userInfo.NickName = mobile.Substring(0, 3) + "***" + mobile.Substring(7, 4);

                        userInfo.Avatar = "";
                        userInfo.PayCredits = 0;
                        userInfo.RankCredits = 0;
                        userInfo.VerifyEmail = 0;
                        userInfo.VerifyMobile = 1;
                        userInfo.DirSalePwd = SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);
                        userInfo.LastVisitIP = "";
                        userInfo.LastVisitRgId = -1;
                        userInfo.LastVisitTime = DateTime.Now;
                        userInfo.RegisterIP = "";
                        userInfo.RegisterRgId = -1;
                        userInfo.RegisterTime = DateTime.Now;
                        userInfo.Gender = 0;
                        userInfo.RealName = consignee;
                        userInfo.Bday = new DateTime(1900, 1, 1);
                        userInfo.IdCard = "";
                        userInfo.RegionId = -1;
                        userInfo.Address = string.Empty;
                        userInfo.Bio = string.Empty;
                        userInfo.BankName = "";
                        userInfo.BankCardCode = "";
                        userInfo.BankUserName = "";

                        userInfo.UserSource = 0;
                        userInfo.AgentType = 0;

                        #endregion

                        #region 处理用户注册推荐人关系
                        string parentname = parentmobile;
                        if (string.IsNullOrEmpty(parentname.Trim()))
                            return null;
                        int pType;
                        int uIdByPname = AccountUtils.GetUidByPname(parentname.Trim(), out pType);
                        if (string.IsNullOrEmpty(parentname.Trim()) || uIdByPname < 1)//推荐人为空或推荐人在后台不存在
                            return null;
                        else //推荐人正确
                        {
                            userInfo.Pid = uIdByPname;
                            userInfo.Ptype = pType;//推荐人类型
                        }
                        #endregion

                        //创建用户
                        userInfo.Uid = Users.CreateUser(userInfo);
                        int agentpid = Users.GetUidByAccountName(parentname);// 
                        int agentptype = 1;
                        Users.UpdateUserAgentRecommer(userInfo.Uid, agentpid, agentptype);

                        //添加用户失败
                        if (userInfo.Uid < 1)
                            return null;
                    }
                }
                partUserInfo = userInfo;
            }


            DateTime nowTime = DateTime.Now;
            IPayPlugin payPlugin = (IPayPlugin)payPluginInfo.Instance;

            OrderInfo orderInfo = new OrderInfo();
            orderInfo.Uid = operateUserInfo.Uid;
            orderInfo.Weight = 0;
            orderInfo.ProductAmount = productInfo.ShopPrice * buyCount;
            orderInfo.FullCut = 0;
            orderInfo.ShipFee = 0;
            orderInfo.PayFee = 0;
            orderInfo.TaxAmount = 0;
            orderInfo.OrderAmount = orderInfo.ProductAmount - orderInfo.FullCut + orderInfo.ShipFee + orderInfo.PayFee + (orderInfo.TaxAmount > 50 ? orderInfo.TaxAmount : 0);
            orderInfo.SurplusMoney = orderInfo.OrderAmount - orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount - orderInfo.CashDiscount;
            if (orderInfo.SurplusMoney < 0)
                return null;
            orderInfo.OrderState = (orderInfo.SurplusMoney <= 0 || payPlugin.PayMode == 0) ? (int)OrderState.Confirming : (int)OrderState.WaitPaying;

            orderInfo.ParentId = 0;
            orderInfo.IsReview = 0;
            orderInfo.AddTime = nowTime;
            orderInfo.StoreId = storeInfo.StoreId;
            orderInfo.StoreName = storeInfo.Name;
            orderInfo.PaySystemName = payPluginInfo.SystemName;
            orderInfo.PayFriendName = payPluginInfo.FriendlyName;
            orderInfo.PayMode = payPlugin.PayMode;

            orderInfo.RegionId = regionid;
            orderInfo.Consignee = consignee;
            orderInfo.Mobile = mobile;
            orderInfo.Phone = "";
            orderInfo.Email = wechat;
            orderInfo.ZipCode = ordertype.ToString();
            orderInfo.Address = address;
            //orderInfo.BestTime = bestTime;
            orderInfo.BuyerRemark = buyerRemark;
            orderInfo.Invoice = "";
            orderInfo.IP = ip;
            orderInfo.MallSource = (int)MallSource.微商订货系统;
            if (ordertype == 1)
                orderInfo.ActualUid = partUserInfo.Uid;
            else if (ordertype == 2)
                orderInfo.ActualUid = operateUserInfo.Uid;
            if (StringHelper.StrContainsForNum(WebConfigurationManager.AppSettings["QuanQiuGouStoreId"], storeInfo.StoreId.ToString()))//全球购订单 
                orderInfo.OrderSource = (int)OrderSource.全球购;
            else
                orderInfo.OrderSource = (int)OrderSource.自营商城;
            orderInfo.OSN = GenerateOSN_Agent(nowTime, payPlugin.PayMode.ToString(), orderInfo.OrderSource.ToString());
            try
            {
                OrderProductInfo mainOrderProductInfo = Carts.BuildOrderProduct(productInfo);
                InitOrderProduct(mainOrderProductInfo, buyCount, "", operateUserInfo.Uid, DateTime.Now);
                mainOrderProductInfo.ProductPV = productInfo.PV;
                mainOrderProductInfo.ProductHaiMi = productInfo.HaiMi;
                mainOrderProductInfo.ProductHBCut = productInfo.HongBaoCut;
                List<OrderProductInfo> orderProductList = new List<OrderProductInfo>();
                orderProductList.Add(mainOrderProductInfo);
                //添加订单
                int oid = _iorderstrategy.CreateOrder(orderInfo, false, orderProductList);
                if (oid > 0)
                {
                    orderInfo.Oid = oid;
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = orderInfo.Oid,
                        Uid = orderInfo.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Submit,
                        ActionTime = DateTime.Now,
                        ActionDes = orderInfo.OrderState == (int)OrderState.WaitPaying ? "您提交了订单，等待您付款" : "您提交了订单，请等待系统确认"
                    });

                    //全额抵扣自动确认
                    if (orderInfo.SurplusMoney <= 0 && orderInfo.OrderState == (int)OrderState.Confirming)
                    {
                        Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, string.Empty, DateTime.Now);
                        Orders.ConfirmOrder(orderInfo);
                        //orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount;
                        String showStr = "您使用" + (orderInfo.CouponMoney > 0 ? orderInfo.CouponMoney.ToString("0.00") + "优惠券、" : "") + (orderInfo.HaiMiDiscount > 0 ? orderInfo.HaiMiDiscount.ToString("0.00") + MallKey.MallDiscountName_JiangJin : "") + (orderInfo.HongBaoDiscount > 0 ? "、" + orderInfo.HongBaoDiscount.ToString("0.00") + MallKey.MallDiscountName_JiFen : "") + (orderInfo.CashDiscount > 0 ? "、" + orderInfo.CashDiscount.ToString("0.00") + "汇购卡" : "") + "抵扣订单金额";
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = orderInfo.Oid,
                            Uid = orderInfo.Uid,
                            RealName = "系统",
                            ActionType = (int)OrderActionType.Confirm,
                            ActionTime = DateTime.Now,//交易时间,
                            ActionDes = showStr + "，您的订单已经确认,正在备货中"
                        });
                    }
                    return orderInfo;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return null;
        }

        /// <summary>
        /// 创建订货系统订单-后台
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="storeInfo">店铺信息</param>
        /// <param name="ip">ip地址</param>
        /// <returns>订单信息</returns>
        public static OrderInfo CreateOrder_Agent_ForAdmin(PartUserInfo partUserInfo, StoreInfo storeInfo, PartProductInfo productInfo, PluginInfo payPluginInfo, string consignee, string mobile, int regionid, string address, string ip, int buyCount, int pid, string buyerRemark, string wechat, int ordertype, PartUserInfo operateUserInfo)
        {
            if (productInfo == null)
                return null;

            DateTime nowTime = DateTime.Now;
            IPayPlugin payPlugin = (IPayPlugin)payPluginInfo.Instance;

            OrderInfo orderInfo = new OrderInfo();
            orderInfo.Uid = operateUserInfo.Uid;
            orderInfo.Weight = 0;
            orderInfo.ProductAmount = productInfo.ShopPrice * buyCount;
            orderInfo.FullCut = 0;
            orderInfo.ShipFee = 0;
            orderInfo.PayFee = 0;
            orderInfo.TaxAmount = 0;
            orderInfo.OrderAmount = orderInfo.ProductAmount - orderInfo.FullCut + orderInfo.ShipFee + orderInfo.PayFee + (orderInfo.TaxAmount > 50 ? orderInfo.TaxAmount : 0);
            orderInfo.SurplusMoney = orderInfo.OrderAmount - orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount - orderInfo.CashDiscount;
            if (orderInfo.SurplusMoney < 0)
                return null;
            orderInfo.OrderState = (orderInfo.SurplusMoney <= 0 || payPlugin.PayMode == 0) ? (int)OrderState.Confirming : (int)OrderState.WaitPaying;

            orderInfo.ParentId = 0;
            orderInfo.IsReview = 0;
            orderInfo.AddTime = nowTime;
            orderInfo.StoreId = storeInfo.StoreId;
            orderInfo.StoreName = storeInfo.Name;
            orderInfo.PaySystemName = payPluginInfo.SystemName;
            orderInfo.PayFriendName = payPluginInfo.FriendlyName;
            orderInfo.PayMode = payPlugin.PayMode;

            orderInfo.RegionId = regionid;
            orderInfo.Consignee = consignee;
            orderInfo.Mobile = mobile;
            orderInfo.Phone = "";
            orderInfo.Email = wechat;
            orderInfo.ZipCode = ordertype.ToString();
            orderInfo.Address = address;
            //orderInfo.BestTime = bestTime;
            orderInfo.BuyerRemark = buyerRemark;
            orderInfo.Invoice = "";
            orderInfo.IP = ip;
            orderInfo.MallSource = (int)MallSource.微商订货系统;
            if (ordertype == 1)
                orderInfo.ActualUid = partUserInfo.Uid;
            else if (ordertype == 2)
                orderInfo.ActualUid = operateUserInfo.Uid;
            if (StringHelper.StrContainsForNum(WebConfigurationManager.AppSettings["QuanQiuGouStoreId"], storeInfo.StoreId.ToString()))//全球购订单 
                orderInfo.OrderSource = (int)OrderSource.全球购;
            else
                orderInfo.OrderSource = (int)OrderSource.自营商城;
            orderInfo.OSN = GenerateOSN_Agent(nowTime, payPlugin.PayMode.ToString(), orderInfo.OrderSource.ToString());
            try
            {
                OrderProductInfo mainOrderProductInfo = Carts.BuildOrderProduct(productInfo);
                InitOrderProduct(mainOrderProductInfo, buyCount, "", operateUserInfo.Uid, DateTime.Now);
                mainOrderProductInfo.ProductPV = productInfo.PV;
                mainOrderProductInfo.ProductHaiMi = productInfo.HaiMi;
                mainOrderProductInfo.ProductHBCut = productInfo.HongBaoCut;
                List<OrderProductInfo> orderProductList = new List<OrderProductInfo>();
                orderProductList.Add(mainOrderProductInfo);
                //添加订单
                int oid = _iorderstrategy.CreateOrder(orderInfo, false, orderProductList);
                if (oid > 0)
                {
                    orderInfo.Oid = oid;
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = orderInfo.Oid,
                        Uid = orderInfo.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Submit,
                        ActionTime = DateTime.Now,
                        ActionDes = orderInfo.OrderState == (int)OrderState.WaitPaying ? "您提交了订单，等待您付款" : "您提交了订单，请等待系统确认"
                    });

                    //全额抵扣自动确认
                    if (orderInfo.SurplusMoney <= 0 && orderInfo.OrderState == (int)OrderState.Confirming)
                    {
                        Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, string.Empty, DateTime.Now);
                        Orders.ConfirmOrder(orderInfo);
                        //orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount;
                        String showStr = "您使用" + (orderInfo.CouponMoney > 0 ? orderInfo.CouponMoney.ToString("0.00") + "优惠券、" : "") + (orderInfo.HaiMiDiscount > 0 ? orderInfo.HaiMiDiscount.ToString("0.00") + MallKey.MallDiscountName_JiangJin : "") + (orderInfo.HongBaoDiscount > 0 ? "、" + orderInfo.HongBaoDiscount.ToString("0.00") + MallKey.MallDiscountName_JiFen: "") + (orderInfo.CashDiscount > 0 ? "、" + orderInfo.CashDiscount.ToString("0.00") + "汇购卡" : "") + "抵扣订单金额";
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = orderInfo.Oid,
                            Uid = orderInfo.Uid,
                            RealName = "系统",
                            ActionType = (int)OrderActionType.Confirm,
                            ActionTime = DateTime.Now,//交易时间,
                            ActionDes = showStr + "，您的订单已经确认,正在备货中"
                        });
                    }
                    return orderInfo;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return null;
        }

        /// <summary>
        /// 创建订货系统订单-后台
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="storeInfo">店铺信息</param>
        /// <param name="ip">ip地址</param>
        /// <returns>订单信息</returns>
        public static OrderInfo CreateOrder_ForAdmin(PartUserInfo partUserInfo, StoreInfo storeInfo, PartProductInfo productInfo, PluginInfo payPluginInfo, string consignee, string mobile, int regionid, string address, string ip, int buyCount, int pid, string buyerRemark, string wechat)
        {
            if (productInfo == null)
                return null;

            DateTime nowTime = DateTime.Now;
            IPayPlugin payPlugin = (IPayPlugin)payPluginInfo.Instance;

            OrderInfo orderInfo = new OrderInfo();
            orderInfo.Uid = partUserInfo.Uid;
            orderInfo.Weight = 0;
            orderInfo.ProductAmount = productInfo.ShopPrice * buyCount;
            orderInfo.FullCut = 0;
            orderInfo.ShipFee = 0;
            orderInfo.PayFee = 0;
            orderInfo.TaxAmount = 0;
            orderInfo.OrderAmount = orderInfo.ProductAmount - orderInfo.FullCut + orderInfo.ShipFee + orderInfo.PayFee + (orderInfo.TaxAmount > 50 ? orderInfo.TaxAmount : 0);
            orderInfo.SurplusMoney = orderInfo.OrderAmount - orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount - orderInfo.CashDiscount;
            if (orderInfo.SurplusMoney < 0)
                return null;
            orderInfo.OrderState = (orderInfo.SurplusMoney <= 0 || payPlugin.PayMode == 0) ? (int)OrderState.Confirming : (int)OrderState.WaitPaying;

            orderInfo.ParentId = 0;
            orderInfo.IsReview = 0;
            orderInfo.AddTime = nowTime;
            orderInfo.StoreId = storeInfo.StoreId;
            orderInfo.StoreName = storeInfo.Name;
            orderInfo.PaySystemName = payPluginInfo.SystemName;
            orderInfo.PayFriendName = payPluginInfo.FriendlyName;
            orderInfo.PayMode = payPlugin.PayMode;

            orderInfo.RegionId = regionid;
            orderInfo.Consignee = consignee;
            orderInfo.Mobile = mobile;
            orderInfo.Phone = "";
            orderInfo.Email = wechat;
            orderInfo.ZipCode = "";
            orderInfo.Address = address;
            //orderInfo.BestTime = bestTime;
            orderInfo.BuyerRemark = buyerRemark;
            orderInfo.Invoice = "";
            orderInfo.IP = ip;
            orderInfo.MallSource = (int)MallSource.自营商城;
            orderInfo.ActualUid = partUserInfo.Uid;
            if (StringHelper.StrContainsForNum(WebConfigurationManager.AppSettings["QuanQiuGouStoreId"], storeInfo.StoreId.ToString()))//全球购订单 
                orderInfo.OrderSource = (int)OrderSource.全球购;
            else
                orderInfo.OrderSource = (int)OrderSource.自营商城;
            orderInfo.OSN = GenerateOSN(storeInfo.StoreId, partUserInfo.Uid, 0, nowTime, payPlugin.PayMode.ToString(), orderInfo.OrderSource.ToString(), orderInfo.OrderAmount);
            //GenerateOSN(storeInfo.StoreId ,partUserInfo.Uid,0,nowTime, payPlugin.PayMode.ToString(), orderInfo.OrderSource.ToString(),0);
            try
            {
                OrderProductInfo mainOrderProductInfo = Carts.BuildOrderProduct(productInfo);
                InitOrderProduct(mainOrderProductInfo, buyCount, "", partUserInfo.Uid, DateTime.Now);
                mainOrderProductInfo.ProductPV = productInfo.PV;
                mainOrderProductInfo.ProductHaiMi = productInfo.HaiMi;
                mainOrderProductInfo.ProductHBCut = productInfo.HongBaoCut;
                List<OrderProductInfo> orderProductList = new List<OrderProductInfo>();
                orderProductList.Add(mainOrderProductInfo);
                //添加订单
                int oid = _iorderstrategy.CreateOrder(orderInfo, false, orderProductList);
                if (oid > 0)
                {
                    orderInfo.Oid = oid;
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = orderInfo.Oid,
                        Uid = orderInfo.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Submit,
                        ActionTime = DateTime.Now,
                        ActionDes = orderInfo.OrderState == (int)OrderState.WaitPaying ? "您提交了订单，等待您付款" : "您提交了订单，请等待系统确认"
                    });

                    //全额抵扣自动确认
                    if (orderInfo.SurplusMoney <= 0 && orderInfo.OrderState == (int)OrderState.Confirming)
                    {
                        Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, string.Empty, DateTime.Now);
                        Orders.ConfirmOrder(orderInfo);
                        //orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount;
                        String showStr = "您使用" + (orderInfo.CouponMoney > 0 ? orderInfo.CouponMoney.ToString("0.00") + "优惠券、" : "") + (orderInfo.HaiMiDiscount > 0 ? orderInfo.HaiMiDiscount.ToString("0.00") + MallKey.MallDiscountName_JiangJin : "") + (orderInfo.HongBaoDiscount > 0 ? "、" + orderInfo.HongBaoDiscount.ToString("0.00") + MallKey.MallDiscountName_JiFen : "") + (orderInfo.CashDiscount > 0 ? "、" + orderInfo.CashDiscount.ToString("0.00") + "汇购卡" : "") + "抵扣订单金额";
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = orderInfo.Oid,
                            Uid = orderInfo.Uid,
                            RealName = "系统",
                            ActionType = (int)OrderActionType.Confirm,
                            ActionTime = DateTime.Now,//交易时间,
                            ActionDes = showStr + "，您的订单已经确认,正在备货中"
                        });
                    }
                    return orderInfo;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return null;
        }

        /// <summary>
        /// 创建不登陆直接购买订单
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="storeInfo">店铺信息</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="singlePromotionList">单品促销活动列表</param>
        /// <param name="payPluginInfo">支付方式</param>
        /// <param name="couponList">优惠劵列表</param>
        /// <param name="ip">ip地址</param>
        /// <returns>订单信息</returns>
        public static OrderInfo CreateOrder_DirectBuy(PartUserInfo partUserInfo, StoreInfo storeInfo, PartProductInfo productInfo, PluginInfo payPluginInfo, string consignee, string mobile, int regionid, string address, string ip, int buyCount, int pid, string buyerRemark, int osnPre = 7)
        {
            if (productInfo == null)
                return null;
            PartUserInfo userInfo = partUserInfo;

            DateTime nowTime = DateTime.Now;
            IPayPlugin payPlugin = (IPayPlugin)payPluginInfo.Instance;

            OrderInfo orderInfo = new OrderInfo();
            orderInfo.Uid = userInfo.Uid;
            orderInfo.Weight = 0;
            orderInfo.ProductAmount = productInfo.ShopPrice * buyCount;
            orderInfo.FullCut = 0;
            orderInfo.ShipFee = 0;
            orderInfo.PayFee = 0;
            orderInfo.TaxAmount = 0;
            orderInfo.OrderAmount = orderInfo.ProductAmount - orderInfo.FullCut + orderInfo.ShipFee + orderInfo.PayFee + (orderInfo.TaxAmount > 50 ? orderInfo.TaxAmount : 0);
            orderInfo.SurplusMoney = orderInfo.OrderAmount - orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount - orderInfo.CashDiscount;
            if (orderInfo.SurplusMoney < 0)
                return null;
            orderInfo.OrderState = (orderInfo.SurplusMoney <= 0 || payPlugin.PayMode == 0) ? (int)OrderState.Confirming : (int)OrderState.WaitPaying;

            orderInfo.ParentId = 0;
            orderInfo.IsReview = 0;
            orderInfo.AddTime = nowTime;
            orderInfo.StoreId = storeInfo.StoreId;
            orderInfo.StoreName = storeInfo.Name;
            orderInfo.PaySystemName = payPluginInfo.SystemName;
            orderInfo.PayFriendName = payPluginInfo.FriendlyName;
            orderInfo.PayMode = payPlugin.PayMode;

            orderInfo.RegionId = regionid;
            orderInfo.Consignee = consignee;
            orderInfo.Mobile = mobile;
            orderInfo.Phone = "";
            orderInfo.Email = "";
            orderInfo.ZipCode = "";
            orderInfo.Address = address;
            //orderInfo.BestTime = bestTime;
            if (buyerRemark.Contains("身份证"))//汇购订单 
                orderInfo.BuyerRemark = buyerRemark;
            else
                orderInfo.BuyerRemark = buyerRemark.Split('-')[0];
            //orderInfo.BuyerRemark = buyerRemark;
            orderInfo.Invoice = "";
            orderInfo.IP = ip;
            orderInfo.MallSource = buyerRemark.Contains("身份证") ? (int)MallSource.自营商城 : (int)MallSource.微商订货系统;

            orderInfo.ActualUid = userInfo.Uid;
            //if (StringHelper.StrContainsForNum(WebConfigurationManager.AppSettings["QuanQiuGouStoreId"], storeInfo.StoreId.ToString()))//全球购订单 
            //    orderInfo.OrderSource = (int)OrderSource.全球购;
            //else
            orderInfo.OrderSource = (int)OrderSource.自营商城;
            orderInfo.OSN = osnPre == 1 ? GenerateOSN(storeInfo.StoreId, partUserInfo.Uid, 0, nowTime, payPlugin.PayMode.ToString(), orderInfo.OrderSource.ToString(), orderInfo.OrderAmount) : GenerateOSN_Agent(nowTime, payPlugin.PayMode.ToString(), orderInfo.OrderSource.ToString());
            try
            {
                OrderProductInfo mainOrderProductInfo = Carts.BuildOrderProduct(productInfo);
                InitOrderProduct(mainOrderProductInfo, buyCount, "", userInfo.Uid, DateTime.Now);
                mainOrderProductInfo.ProductPV = productInfo.PV;
                mainOrderProductInfo.ProductHaiMi = productInfo.HaiMi;
                mainOrderProductInfo.ProductHBCut = productInfo.HongBaoCut;
                List<OrderProductInfo> orderProductList = new List<OrderProductInfo>();
                orderProductList.Add(mainOrderProductInfo);
                //添加订单
                int oid = _iorderstrategy.CreateOrder(orderInfo, false, orderProductList);
                if (oid > 0)
                {
                    orderInfo.Oid = oid;
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = orderInfo.Oid,
                        Uid = orderInfo.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Submit,
                        ActionTime = DateTime.Now,
                        ActionDes = orderInfo.OrderState == (int)OrderState.WaitPaying ? "您提交了订单，等待您付款" : "您提交了订单，请等待系统确认"
                    });

                    //全额抵扣自动确认
                    if (orderInfo.SurplusMoney <= 0 && orderInfo.OrderState == (int)OrderState.Confirming)
                    {
                        Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, string.Empty, DateTime.Now);
                        Orders.ConfirmOrder(orderInfo);
                        //orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount;
                        String showStr = "您使用" + (orderInfo.CouponMoney > 0 ? orderInfo.CouponMoney.ToString("0.00") + "优惠券、" : "") + (orderInfo.HaiMiDiscount > 0 ? orderInfo.HaiMiDiscount.ToString("0.00") + MallKey.MallDiscountName_JiangJin : "") + (orderInfo.HongBaoDiscount > 0 ? "、" + orderInfo.HongBaoDiscount.ToString("0.00") + MallKey.MallDiscountName_JiFen : "") + (orderInfo.CashDiscount > 0 ? "、" + orderInfo.CashDiscount.ToString("0.00") + "汇购卡" : "") + "抵扣订单金额";
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = orderInfo.Oid,
                            Uid = orderInfo.Uid,
                            RealName = "系统",
                            ActionType = (int)OrderActionType.Confirm,
                            ActionTime = DateTime.Now,//交易时间,
                            ActionDes = showStr + "，您的订单已经确认,正在备货中"
                        });
                    }
                    return orderInfo;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return null;
        }

        /// <summary>
        /// 创建充值订单
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="storeInfo">店铺信息</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="singlePromotionList">单品促销活动列表</param>
        /// <param name="payPluginInfo">支付方式</param>
        /// <param name="couponList">优惠劵列表</param>
        /// <param name="ip">ip地址</param>
        /// <returns>订单信息</returns>
        public static OrderInfo CreateOrder_ChongZhi(PartUserInfo partUserInfo, StoreInfo storeInfo, PartProductInfo productInfo, PluginInfo payPluginInfo, ref decimal haiMiCount, List<CouponInfo> couponList, string ip, string CZMobile, int buyCount, int pid, int areaid, string mobiletips)
        {
            if (productInfo == null)
                return null;
            DateTime nowTime = DateTime.Now;
            IPayPlugin payPlugin = (IPayPlugin)payPluginInfo.Instance;

            OrderInfo orderInfo = new OrderInfo();
            orderInfo.Uid = partUserInfo.Uid;

            orderInfo.Weight = 0;
            orderInfo.ProductAmount = productInfo.ShopPrice;
            orderInfo.FullCut = 0;
            GroupProductInfo gpInfo = new GroupProducts().GetModel(3);
            orderInfo.ShipFee = 0;

            orderInfo.PayFee = 0;
            orderInfo.TaxAmount = 0;
            orderInfo.OrderAmount = orderInfo.ProductAmount - orderInfo.FullCut + orderInfo.ShipFee + orderInfo.PayFee + (orderInfo.TaxAmount > 50 ? orderInfo.TaxAmount : 0);

            #region 订单使用优惠及抵扣
            //海米抵现
            if (orderInfo.OrderAmount >= haiMiCount)
            {
                orderInfo.HaiMiDiscount = haiMiCount;
                haiMiCount = 0;
            }
            else
            {
                orderInfo.HaiMiDiscount = orderInfo.OrderAmount;
                haiMiCount = haiMiCount - orderInfo.OrderAmount;
            }
            #endregion

            orderInfo.CouponMoney = Coupons.SumCouponMoney(couponList);
            orderInfo.SurplusMoney = orderInfo.OrderAmount - orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount - orderInfo.CashDiscount;
            if (orderInfo.SurplusMoney < 0)
                return null;
            orderInfo.OrderState = (orderInfo.SurplusMoney <= 0 || payPlugin.PayMode == 0) ? (int)OrderState.Confirming : (int)OrderState.WaitPaying;

            orderInfo.ParentId = 0;
            orderInfo.IsReview = 0;
            orderInfo.AddTime = nowTime;
            orderInfo.StoreId = storeInfo.StoreId;
            orderInfo.StoreName = storeInfo.Name;
            orderInfo.PaySystemName = payPluginInfo.SystemName;
            orderInfo.PayFriendName = payPluginInfo.FriendlyName;
            orderInfo.PayMode = payPlugin.PayMode;

            orderInfo.RegionId = 0;
            orderInfo.Consignee = mobiletips;
            orderInfo.Mobile = CZMobile;
            orderInfo.Phone = pid.ToString();
            orderInfo.Email = "";
            orderInfo.ZipCode = areaid.ToString();
            orderInfo.Address = CZMobile + "充值" + productInfo.MarketPrice + "元";
            //orderInfo.BestTime = bestTime;

            orderInfo.BuyerRemark = "";

            orderInfo.BuyerRemark = "";
            orderInfo.Invoice = "";
            orderInfo.IP = ip;

            if (StringHelper.StrContainsForNum(WebConfigurationManager.AppSettings["QuanQiuGouStoreId"], storeInfo.StoreId.ToString()))//全球购订单 
            {
                orderInfo.OrderSource = (int)OrderSource.全球购;
            }
            //else if (storeInfo.StoreId.ToString() == WebConfigurationManager.AppSettings["HealthenStoreId"])
            //{
            //    orderInfo.OrderSource = (int)OrderSource.直销后台;
            //}
            else
            {
                orderInfo.OrderSource = (int)OrderSource.自营商城;
            }

            orderInfo.OSN = GenerateOSN(storeInfo.StoreId, partUserInfo.Uid, 0, nowTime, payPlugin.PayMode.ToString(), orderInfo.OrderSource.ToString(), orderInfo.OrderAmount);
            try
            {
                OrderProductInfo mainOrderProductInfo = Carts.BuildOrderProduct(productInfo);
                InitOrderProduct(mainOrderProductInfo, buyCount, "", partUserInfo.Uid, DateTime.Now);
                mainOrderProductInfo.ProductPV = productInfo.PV;
                mainOrderProductInfo.ProductHaiMi = productInfo.HaiMi;
                mainOrderProductInfo.ProductHBCut = productInfo.HongBaoCut;
                List<OrderProductInfo> orderProductList = new List<OrderProductInfo>();
                orderProductList.Add(mainOrderProductInfo);
                //添加订单
                int oid = _iorderstrategy.CreateOrder(orderInfo, false, orderProductList);
                if (oid > 0)
                {

                    orderInfo.Oid = oid;

                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = orderInfo.Oid,
                        Uid = orderInfo.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Submit,
                        ActionTime = DateTime.Now,
                        ActionDes = orderInfo.OrderState == (int)OrderState.WaitPaying ? "您提交了订单，等待您付款" : "您提交了订单，请等待系统确认"
                    });


                    //使用海米记录
                    if (orderInfo.HaiMiDiscount > 0)
                    {
                        //更新直销的账户
                        if (partUserInfo.IsDirSaleUser)
                        {
                            AccountUtils.UpdateAccountForDir(partUserInfo.DirSaleUid, (int)AccountType.商城钱包, 0, orderInfo.HaiMiDiscount, orderInfo.OSN, "订单" + MallKey.MallDiscountName_JiangJin + "抵现：订单号:" + orderInfo.OSN + ",抵现金额:" + orderInfo.HaiMiDiscount);
                        }
                        //更新海汇账户
                        else
                        {
                            Account.UpdateAccountForOut(new AccountInfo()
                            {
                                AccountId = (int)AccountType.商城钱包,
                                UserId = partUserInfo.Uid,
                                TotalOut = orderInfo.HaiMiDiscount
                            });
                            Account.CreateAccountDetail(new AccountDetailInfo()
                            {
                                AccountId = (int)AccountType.商城钱包,
                                UserId = partUserInfo.Uid,
                                CreateTime = DateTime.Now,
                                DetailType = (int)DetailType.订单抵现支出,
                                OutAmount = orderInfo.HaiMiDiscount,
                                OrderCode = orderInfo.OSN,
                                AdminUid = partUserInfo.Uid,//system
                                Status = 1,
                                DetailDes = "订单" + MallKey.MallDiscountName_JiangJin + "抵现：订单号:" + orderInfo.OSN + ",抵现金额:" + orderInfo.HaiMiDiscount
                            });
                        }
                    }

                    //使用优惠劵
                    foreach (CouponInfo couponInfo in couponList)
                    {
                        if (couponInfo.Uid > 0)
                            Coupons.UseCoupon(couponInfo.CouponId, oid, nowTime, ip);
                        else
                            Coupons.ActivateAndUseCoupon(couponInfo.CouponId, partUserInfo.Uid, oid, nowTime, ip);
                    }

                    //全额抵扣自动确认
                    if (orderInfo.SurplusMoney <= 0 && orderInfo.OrderState == (int)OrderState.Confirming)
                    {
                        Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, string.Empty, DateTime.Now);
                        Orders.ConfirmOrder(orderInfo);
                        //orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount;
                        String showStr = "您使用" + (orderInfo.CouponMoney > 0 ? orderInfo.CouponMoney.ToString("0.00") + "优惠券、" : "") + (orderInfo.HaiMiDiscount > 0 ? orderInfo.HaiMiDiscount.ToString("0.00") + MallKey.MallDiscountName_JiangJin : "") + (orderInfo.HongBaoDiscount > 0 ? "、" + orderInfo.HongBaoDiscount.ToString("0.00") + MallKey.MallDiscountName_JiFen : "") + (orderInfo.CashDiscount > 0 ? "、" + orderInfo.CashDiscount.ToString("0.00") + "汇购卡" : "") + "抵扣订单金额";
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = orderInfo.Oid,
                            Uid = orderInfo.Uid,
                            RealName = "系统",
                            ActionType = (int)OrderActionType.Confirm,
                            ActionTime = DateTime.Now,//交易时间,
                            ActionDes = showStr + "，您的订单已经确认,正在备货中"
                        });
                    }

                    return orderInfo;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return null;
        }

        /// <summary>
        /// 创建重销旅游订单 -5900PV
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="storeInfo">店铺信息</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="singlePromotionList">单品促销活动列表</param>
        /// <param name="payPluginInfo">支付方式</param>
        /// <param name="couponList">优惠劵列表</param>
        /// <param name="ip">ip地址</param>
        /// <returns>订单信息</returns>
        public static OrderInfo CreateOrder_Trip(PartUserInfo partUserInfo, StoreInfo storeInfo, PartProductInfo productInfo, PluginInfo payPluginInfo, string ip, int buyCount, OrderInfo oldorder)
        {
            if (productInfo == null)
                return null;
            DateTime nowTime = DateTime.Now;
            IPayPlugin payPlugin = (IPayPlugin)payPluginInfo.Instance;

            OrderInfo orderInfo = new OrderInfo();
            orderInfo.Uid = partUserInfo.Uid;
            orderInfo.Weight = 0;
            orderInfo.ProductAmount = productInfo.ShopPrice;
            orderInfo.FullCut = 0;
            orderInfo.ShipFee = 0;
            orderInfo.PayFee = 0;
            orderInfo.TaxAmount = 0;
            orderInfo.OrderAmount = 0;
            orderInfo.HaiMiDiscount = 0;
            orderInfo.CouponMoney = 0;
            orderInfo.SurplusMoney = 0;
            orderInfo.OrderState = (orderInfo.SurplusMoney <= 0 || payPlugin.PayMode == 0) ? (int)OrderState.Confirming : (int)OrderState.WaitPaying;
            orderInfo.ParentId = 0;
            orderInfo.IsReview = 0;
            orderInfo.AddTime = nowTime;
            orderInfo.StoreId = storeInfo.StoreId;
            orderInfo.StoreName = storeInfo.Name;
            orderInfo.PaySystemName = payPluginInfo.SystemName;
            orderInfo.PayFriendName = payPluginInfo.FriendlyName;
            orderInfo.PayMode = payPlugin.PayMode;

            orderInfo.RegionId = oldorder.RegionId;
            orderInfo.Consignee = oldorder.Consignee;
            orderInfo.Mobile = oldorder.Mobile;
            orderInfo.Phone = oldorder.Phone;
            orderInfo.Email = oldorder.Email;
            orderInfo.ZipCode = oldorder.ZipCode;
            orderInfo.Address = oldorder.Address;
            //orderInfo.BestTime = bestTime;
            orderInfo.BuyerRemark = "旅游产品";
            orderInfo.Invoice = "";
            orderInfo.IP = ip;
            orderInfo.OrderSource = (int)OrderSource.自营商城;

            orderInfo.OSN = GenerateOSN(storeInfo.StoreId, partUserInfo.Uid, 0, nowTime, payPlugin.PayMode.ToString(), orderInfo.OrderSource.ToString(), orderInfo.OrderAmount);
            try
            {
                OrderProductInfo mainOrderProductInfo = Carts.BuildOrderProduct(productInfo);
                InitOrderProduct(mainOrderProductInfo, buyCount, "", partUserInfo.Uid, DateTime.Now);
                mainOrderProductInfo.ProductPV = productInfo.PV;
                mainOrderProductInfo.ProductHaiMi = productInfo.HaiMi;
                mainOrderProductInfo.ProductHBCut = productInfo.HongBaoCut;
                List<OrderProductInfo> orderProductList = new List<OrderProductInfo>();
                orderProductList.Add(mainOrderProductInfo);
                //添加订单
                int oid = _iorderstrategy.CreateOrder(orderInfo, false, orderProductList);
                if (oid > 0)
                {
                    orderInfo.Oid = oid;

                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = orderInfo.Oid,
                        Uid = orderInfo.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Submit,
                        ActionTime = DateTime.Now,
                        ActionDes = orderInfo.OrderState == (int)OrderState.WaitPaying ? "您提交了订单，等待您付款" : "您提交了订单，请等待系统确认"
                    });

                    //全额抵扣自动确认
                    if (orderInfo.SurplusMoney <= 0 && orderInfo.OrderState == (int)OrderState.Confirming)
                    {
                        Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, string.Empty, DateTime.Now);

                        ConfirmReceiving(orderInfo.Oid, OrderState.Completed, DateTime.Now);//将订单状态设为完成状态,并更新收货时间

                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = orderInfo.Oid,
                            Uid = orderInfo.Uid,
                            RealName = "系统",
                            ActionType = (int)OrderActionType.Complete,
                            ActionTime = DateTime.Now,//交易时间,
                            ActionDes = "您的订单已经完成。"
                        });
                        OrderUtils.CreateQDOrder(orderInfo, partUserInfo, 1);
                    }
                    return orderInfo;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return null;
        }

        /// <summary>
        /// 创建MC每月领用赠品订单-
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="storeInfo">店铺信息</param>
        /// <param name="ip">ip地址</param>
        /// <returns>订单信息</returns>
        public static OrderInfo CreateOrder_ForMC(PartUserInfo partUserInfo, StoreInfo storeInfo, PartProductInfo productInfo, PluginInfo payPluginInfo, string consignee, string mobile, int regionid, string address, string ip, int buyCount, int pid, string buyerRemark, string wechat, FullShipAddressInfo fullShipAddressInfo, List<OrderProductInfo> addOrderProductList, int orderGiftId)
        {
            if (productInfo == null)
                return null;

            DateTime nowTime = DateTime.Now;
            IPayPlugin payPlugin = (IPayPlugin)payPluginInfo.Instance;

            OrderInfo orderInfo = new OrderInfo();
            orderInfo.Uid = partUserInfo.Uid;
            orderInfo.Weight = 0;
            orderInfo.ProductAmount = productInfo.ShopPrice * buyCount;
            orderInfo.FullCut = 0;
            bool isSend = true;
            orderInfo.ShipFee = GetShipFee(fullShipAddressInfo.ProvinceId, fullShipAddressInfo.CityId, addOrderProductList, ref isSend);

            orderInfo.PayFee = 0;
            orderInfo.TaxAmount = 0;
            orderInfo.OrderAmount = orderInfo.ProductAmount - orderInfo.FullCut + orderInfo.ShipFee + orderInfo.PayFee + (orderInfo.TaxAmount > 50 ? orderInfo.TaxAmount : 0);
            orderInfo.SurplusMoney = orderInfo.OrderAmount - orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount - orderInfo.CashDiscount;
            if (orderInfo.SurplusMoney < 0)
                return null;
            orderInfo.OrderState = (orderInfo.SurplusMoney <= 0 || payPlugin.PayMode == 0) ? (int)OrderState.Confirming : (int)OrderState.WaitPaying;

            orderInfo.ParentId = 0;
            orderInfo.IsReview = 0;
            orderInfo.AddTime = nowTime;
            orderInfo.StoreId = storeInfo.StoreId;
            orderInfo.StoreName = storeInfo.Name;
            orderInfo.PaySystemName = payPluginInfo.SystemName;
            orderInfo.PayFriendName = payPluginInfo.FriendlyName;
            orderInfo.PayMode = payPlugin.PayMode;

            orderInfo.RegionId = regionid;
            orderInfo.Consignee = consignee;
            orderInfo.Mobile = mobile;
            orderInfo.Phone = "";
            orderInfo.Email = wechat;
            orderInfo.ZipCode = "";
            orderInfo.Address = address;
            //orderInfo.BestTime = bestTime;
            orderInfo.BuyerRemark = buyerRemark;
            orderInfo.Invoice = "";
            orderInfo.IP = ip;
            orderInfo.MallSource = (int)MallSource.自营商城;
            orderInfo.ExtOrderId = orderGiftId.ToString();
            orderInfo.ActualUid = partUserInfo.Uid;

            orderInfo.OrderSource = (int)OrderSource.自营商城;
            orderInfo.OSN = GenerateOSN(storeInfo.StoreId, partUserInfo.Uid, 0, nowTime, payPlugin.PayMode.ToString(), orderInfo.OrderSource.ToString(), orderInfo.OrderAmount);
            //GenerateOSN(storeInfo.StoreId ,partUserInfo.Uid,0,nowTime, payPlugin.PayMode.ToString(), orderInfo.OrderSource.ToString(),0);
            try
            {
                OrderProductInfo mainOrderProductInfo = Carts.BuildOrderProduct(productInfo);
                InitOrderProduct(mainOrderProductInfo, buyCount, "", partUserInfo.Uid, DateTime.Now);
                mainOrderProductInfo.ProductPV = productInfo.PV;
                mainOrderProductInfo.ProductHaiMi = productInfo.HaiMi;
                mainOrderProductInfo.ProductHBCut = productInfo.HongBaoCut;
                List<OrderProductInfo> orderProductList = new List<OrderProductInfo>();
                orderProductList.Add(mainOrderProductInfo);
                //添加订单
                int oid = _iorderstrategy.CreateOrder(orderInfo, false, orderProductList);
                if (oid > 0)
                {
                    orderInfo.Oid = oid;
                    UpdateOrderGift(oid, orderGiftId.ToString());
                    //创建订单处理
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = orderInfo.Oid,
                        Uid = orderInfo.Uid,
                        RealName = "本人",
                        ActionType = (int)OrderActionType.Submit,
                        ActionTime = DateTime.Now,
                        ActionDes = orderInfo.OrderState == (int)OrderState.WaitPaying ? "您提交了订单，等待您付款" : "您提交了订单，请等待系统确认"
                    });

                    //全额抵扣自动确认
                    if (orderInfo.SurplusMoney <= 0 && orderInfo.OrderState == (int)OrderState.Confirming)
                    {
                        Orders.PayOrder(orderInfo.Oid, OrderState.Confirming, string.Empty, DateTime.Now);
                        Orders.ConfirmOrder(orderInfo);
                        //orderInfo.PayCreditMoney - orderInfo.CouponMoney - orderInfo.HaiMiDiscount - orderInfo.HongBaoDiscount;
                        String showStr = "您使用" + (orderInfo.CouponMoney > 0 ? orderInfo.CouponMoney.ToString("0.00") + "优惠券、" : "") + (orderInfo.HaiMiDiscount > 0 ? orderInfo.HaiMiDiscount.ToString("0.00") + MallKey.MallDiscountName_JiangJin : "") + (orderInfo.HongBaoDiscount > 0 ? "、" + orderInfo.HongBaoDiscount.ToString("0.00") + MallKey.MallDiscountName_JiFen : "") + (orderInfo.CashDiscount > 0 ? "、" + orderInfo.CashDiscount.ToString("0.00") + "汇购卡" : "") + "抵扣订单金额";
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = orderInfo.Oid,
                            Uid = orderInfo.Uid,
                            RealName = "系统",
                            ActionType = (int)OrderActionType.Confirm,
                            ActionTime = DateTime.Now,//交易时间,
                            ActionDes = showStr + "，您的订单已经确认,正在备货中"
                        });
                    }
                    return orderInfo;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return null;
        }

        /// <summary>
        /// 初始化订单商品
        /// </summary>
        private static void InitOrderProduct(OrderProductInfo orderProductInfo, int buyCount, string sid, int uid, DateTime buyTime)
        {
            if (uid > 0)
                orderProductInfo.Sid = "";
            else
                orderProductInfo.Sid = sid;
            orderProductInfo.Uid = uid;
            orderProductInfo.RealCount = buyCount;
            orderProductInfo.BuyCount = buyCount;
            orderProductInfo.AddTime = buyTime;
        }

        #region 更新订单信息
        /// <summary>
        /// 更新订单折扣
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="discount">折扣</param>
        /// <param name="surplusMoney">剩余金额</param>
        public static void UpdateOrderDiscount(int oid, decimal discount, decimal surplusMoney)
        {
            VMall.Data.Orders.UpdateOrderDiscount(oid, discount, surplusMoney);
        }

        /// <summary>
        /// 更新订单快递单号
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="shipFee">快递单号</param>

        public static void UpdateOrderShipSN(int oid, string shipSn)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@shipsn",shipSn)
                                   };
            string commandText = string.Format("UPDATE [{0}orders] SET [shipsn]=@shipsn WHERE [oid]=@oid",
                                               RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 更新订单快递信息
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="shipFee">快递单号</param>

        public static void UpdateShipInfo(int oid, string shipSN, int shipCoId, string shipCoName)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@shipsn",shipSN),
                                       new SqlParameter("@shipcoid",shipCoId),
                                       new SqlParameter("@shipconame",shipCoName)
                                   };
            string commandText = string.Format("UPDATE [{0}orders] SET [shipsn]=@shipsn,[shipcoid]=@shipcoid,[shipconame]=@shipconame WHERE [oid]=@oid",
                                               RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
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
            VMall.Data.Orders.UpdateOrderShipFee(oid, shipFee, orderAmount, surplusMoney);
        }
        #endregion

        /// <summary>
        /// 付款
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="paySN">支付单号</param>
        /// <param name="payTime">支付时间</param>
        public static void PayOrder(int oid, OrderState orderState, string paySN, DateTime payTime, string payDevice = "")
        {
            payDevice = string.IsNullOrEmpty(payDevice) ? (WebHelper.IsMobile() ? "m" : "pc") : payDevice;
            VMall.Data.Orders.PayOrder(oid, orderState, paySN, payTime, payDevice);
        }

        /// <summary>
        /// 确认订单
        /// </summary>
        /// <param name="orderInfo">订单信息</param>
        public static void ConfirmOrder(OrderInfo orderInfo)
        {

            UpdateOrderState(orderInfo.Oid, OrderState.Confirmed);

            List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            //蜜采代理店铺订单
            if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("MCAgentStoreId"))
            {
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = orderInfo.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.Confirm,
                    ActionTime = DateTime.Now,//交易时间,
                    ActionDes = "您的订单已经确认,正在备货中"
                });
                AdminOrders.PreProduct(orderInfo);
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = orderInfo.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.PreProduct,
                    ActionTime = DateTime.Now.AddSeconds(50),//交易时间,
                    ActionDes = "您的订单已经备货完成，等待出库"
                });
                if (partUserInfo.AgentType <= 0)
                {
                    //更新是否激活、激活时间
                    partUserInfo.IsActive = 1;
                    partUserInfo.ActiveTime = DateTime.Now;
                    int oldAgenttype = partUserInfo.AgentType;
                    if (oldAgenttype <= 0) { }
                        //partUserInfo.AgentType = (int)AgentTypeEnum.VIP会员;
                    //大于4盒成为VIP
                    if (orderProductList.Sum(x => x.BuyCount) >= 4)
                    {
                        bool flag = new Users().Update(partUserInfo, "agenttype,isactive,activetime");
                    }
                    //存在引流产品
                    if (orderProductList.Exists(
                        x => StringHelper.StrContainsForNum(WebHelper.GetConfigSettings("GetSourcePid"), x.Pid.ToString())))
                    {
                        OrderProductInfo orderPro = orderProductList.Find(x => x.Pid == WebHelper.GetConfigSettingsInt("GetSourcePid"));
                        DateTime nextMonth = DateTime.Now.AddMonths(1);
                        DateTime next2Month = nextMonth.AddMonths(1);
                        DateTime nextYear = nextMonth.AddYears(1);
                        //插入订单赠品表记录
                        orderGiftBLL.Add(new OrderGiftInfo()
                        {
                            CreationDate = DateTime.Now,
                            CreateOid = orderInfo.Oid,
                            Uid = orderInfo.Uid,
                            State = 1,
                            GiftPid = WebHelper.GetConfigSettingsInt("GiftPid"),
                            GiftCount = orderPro.BuyCount >= 1 && orderPro.BuyCount < 4 ? 1 : 12,
                            UseCount = 0,
                            StartTime = new DateTime(nextMonth.Year, nextMonth.Month, 1),
                            EndTime = orderPro.BuyCount >= 1 && orderPro.BuyCount < 4 ? new DateTime(next2Month.Year, next2Month.Month, 1) : new DateTime(nextYear.Year, nextYear.Month, 1),
                            LastModify = DateTime.Now
                        });

                    }
                }
                if (orderInfo.ExtOrderId != "")
                {
                    OrderGiftInfo gift = orderGiftBLL.GetModel(TypeHelper.StringToInt(orderInfo.ExtOrderId));
                    gift.UseCount = gift.UseCount - 1;
                    gift.LastModify = DateTime.Now;
                    orderGiftBLL.Update(gift, "UseCount,LastModify");
                }
            }
            #region

            #region 代理订单
            //代理订单处理逻辑
            else if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"))
            {
                bool isChangeSelfStock = true;
                int currentAgentType = partUserInfo.AgentType;
                //获得有效代理上级用户
                PartUserInfo parentUser = Users.GetParentUserForAgent(partUserInfo);
                if (partUserInfo.AgentType < 1)
                {
                    int agentType = 0;
                    decimal AllAmount = orderInfo.OrderAmount;
                    if (AllAmount >= 2980 && AllAmount < 11000)
                        agentType = 1;
                    else if (AllAmount >= 11000 && AllAmount < 38000)
                        agentType = 2;
                    else if (AllAmount >= 38000 && AllAmount < 300000)
                        agentType = 3;
                    else if (AllAmount > 300000)
                        agentType = 4;
                    partUserInfo.AgentType = agentType;

                    Users.UpdateAgentUserMark(partUserInfo, parentUser);//更新代理等级

                    ConfirmReceiving(orderInfo.Oid, OrderState.Completed, DateTime.Now);//将订单状态设为完成状态,并更新收货时间
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = orderInfo.Oid,
                        Uid = orderInfo.Uid,
                        RealName = "系统",
                        ActionType = (int)OrderActionType.Complete,
                        ActionTime = DateTime.Now,//交易时间,
                        ActionDes = "您的订单已经完成。"
                    });

                    //OrderInfo suborder = Orders.GetOrdertByWhere(string.Format(" [mainoid]={0}", orderInfo.Oid));

                    #region 原1980套餐处理

                    //if (suborder != null)//购买代理资格订单，包含1980体验套餐的订单直接完成
                    //{
                    //if (suborder.OrderState == (int)OrderState.WaitPaying)
                    //{
                    //    UpdateOrderState(suborder.Oid, OrderState.Confirmed);
                    //    OrderActions.CreateOrderAction(new OrderActionInfo()
                    //    {
                    //        Oid = suborder.Oid,
                    //        Uid = suborder.Uid,
                    //        RealName = "系统",
                    //        ActionType = (int)OrderActionType.Confirm,
                    //        ActionTime = DateTime.Now,//交易时间,
                    //        ActionDes = "您的订单已经确认,正在备货中"
                    //    });
                    //    //代理套餐直接备货
                    //    AdminOrders.PreProduct(suborder);
                    //    OrderActions.CreateOrderAction(new OrderActionInfo()
                    //    {
                    //        Oid = suborder.Oid,
                    //        Uid = suborder.Uid,
                    //        RealName = "系统",
                    //        ActionType = (int)OrderActionType.PreProduct,
                    //        ActionTime = DateTime.Now,//交易时间,
                    //        ActionDes = "您的订单已经备货完成，等待出库"
                    //    });
                    //}
                    //OrderInfo suborder2 = Orders.GetOrderByOid(suborder.Oid);
                    //if (suborder2.OrderState >= (int)OrderState.Confirmed && suborder2.OrderState <= (int)OrderState.Completed)
                    //{
                    //    decimal AllAmount = orderInfo.OrderAmount;
                    //    if (AllAmount >= 2980 && AllAmount < 11000)
                    //        agentType = 1;
                    //    else if (AllAmount >= 11000 && AllAmount < 38000)
                    //        agentType = 2;
                    //    else if (AllAmount >= 38000 && AllAmount < 300000)
                    //        agentType = 3;
                    //    else if (AllAmount > 300000)
                    //        agentType = 4;
                    //    partUserInfo.AgentType = agentType;

                    //    Users.UpdateAgentUserMark(partUserInfo, parentUser);//更新代理等级
                    //}
                    //ConfirmReceiving(orderInfo.Oid, OrderState.Completed, DateTime.Now);//将订单状态设为完成状态,并更新收货时间
                    //OrderActions.CreateOrderAction(new OrderActionInfo()
                    //{
                    //    Oid = orderInfo.Oid,
                    //    Uid = orderInfo.Uid,
                    //    RealName = "系统",
                    //    ActionType = (int)OrderActionType.Complete,
                    //    ActionTime = DateTime.Now,//交易时间,
                    //    ActionDes = "您的订单已经完成。"
                    //});
                    //}
                    #endregion

                    if (AllAmount < 2980 && partUserInfo.AgentType == 0)//普通会员零售购买，要发货订单
                    {
                        isChangeSelfStock = false;
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = orderInfo.Oid,
                            Uid = orderInfo.Uid,
                            RealName = "系统",
                            ActionType = (int)OrderActionType.Confirm,
                            ActionTime = DateTime.Now,//交易时间,
                            ActionDes = "您的订单已经确认,正在备货中"
                        });
                        AdminOrders.PreProduct(orderInfo);
                        OrderActions.CreateOrderAction(new OrderActionInfo()
                        {
                            Oid = orderInfo.Oid,
                            Uid = orderInfo.Uid,
                            RealName = "系统",
                            ActionType = (int)OrderActionType.PreProduct,
                            ActionTime = DateTime.Now,//交易时间,
                            ActionDes = "您的订单已经备货完成，等待出库"
                        });
                    }
                }
                if (currentAgentType >= 1) //已经是代理会员二次补货订单需要发货
                {
                    isChangeSelfStock = false;
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = orderInfo.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.Confirm,
                    ActionTime = DateTime.Now,//交易时间,
                    ActionDes = "您的订单已经确认,正在备货中"
                });
                    AdminOrders.PreProduct(orderInfo);
                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = orderInfo.Oid,
                        Uid = orderInfo.Uid,
                        RealName = "系统",
                        ActionType = (int)OrderActionType.PreProduct,
                        ActionTime = DateTime.Now,//交易时间,
                        ActionDes = "您的订单已经备货完成，等待出库"
                    });

                }

                //该订单的代理库存处理
                foreach (var item in orderProductList)
                {
                    //找上级的对应产品的代理库存，找不到说明库存为0  公司拿货，库存小于购买，不足部分从公司补，并记录库存加减详情
                    new AgentStock().UpdateAgentStockForOrder(parentUser, partUserInfo, orderInfo, item, isChangeSelfStock);
                }
            }
            else if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId"))
            {
                //套餐代理资格更新
                //if (partUserInfo.AgentType <= 0)
                //{
                //    int agentType = 1;
                //OrderInfo mainorder = Orders.GetOrdertByWhere(string.Format(" [oid]={0}", orderInfo.MainOid));
                //获得有效代理上级用户

                //if (mainorder != null)
                //{
                //    if (mainorder.OrderState >= (int)OrderState.Confirmed && mainorder.OrderState <= (int)OrderState.Completed)
                //    {
                //        decimal AllAmount = mainorder.OrderAmount;// orderInfo.OrderAmount + mainorder.OrderAmount;
                //        if (AllAmount >= 2980 && AllAmount < 11000)
                //            agentType = 1;
                //        else if (AllAmount >= 11000 && AllAmount < 38000)
                //            agentType = 2;
                //        else if (AllAmount >= 38000 && AllAmount < 300000)
                //            agentType = 3;
                //        else if (AllAmount > 300000)
                //            agentType = 4;

                //    }
                //}
                //    partUserInfo.AgentType = agentType;
                //    PartUserInfo parentUser = Users.GetParentUserForAgent(partUserInfo);
                //    Users.UpdateAgentUserMark(partUserInfo, parentUser);
                //}
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = orderInfo.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.Confirm,
                    ActionTime = DateTime.Now,//交易时间,
                    ActionDes = "您的订单已经确认,正在备货中"
                });
                //代理套餐直接备货
                AdminOrders.PreProduct(orderInfo);
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = orderInfo.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.PreProduct,
                    ActionTime = DateTime.Now,//交易时间,
                    ActionDes = "您的订单已经备货完成，等待出库"
                });
            }
            #endregion

            //汇购优选引流包店铺
            else if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("YXYLBStoreId"))
            {
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = orderInfo.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.Confirm,
                    ActionTime = DateTime.Now,//交易时间,
                    ActionDes = "您的订单已经确认,正在备货中"
                });
                AdminOrders.PreProduct(orderInfo);
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = orderInfo.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.PreProduct,
                    ActionTime = DateTime.Now.AddSeconds(50),//交易时间,
                    ActionDes = "您的订单已经备货完成，等待出库"
                });
                //更新是否激活、激活时间
                partUserInfo.IsActive = 1;
                partUserInfo.ActiveTime = DateTime.Now;
                int oldAgenttype = partUserInfo.AgentType;
                if (oldAgenttype <= 0)
                    partUserInfo.AgentType = 1;
                if (oldAgenttype <= 0 && partUserInfo.IsDirSaleUser && partUserInfo.Ds2AgentRank > 0)
                    partUserInfo.AgentType = partUserInfo.Ds2AgentRank;
                //非代理会员需要重新锁定代理关系，代理会员已经存在代理关系
                if (oldAgenttype <= 0)
                {
                    PartUserInfo parentUser = Users.GetParentUserByPidAndPtype(partUserInfo.Pid, partUserInfo.Ptype);
                    //修改代理网络
                    Users.UpdateUserAgentRecommer(partUserInfo.Uid, parentUser.Uid, 1);

                }
                new Users().Update(partUserInfo, "agenttype,isactive,activetime");
                //推荐20升h2
                if (partUserInfo.AgentType == 1)
                    Users.UpgradeAgentToH2(orderInfo);
                //送优惠券

                int sendCount = 0;
                if (partUserInfo.AgentType == 2)
                    sendCount = 3;
                else if (partUserInfo.AgentType == 3)
                    sendCount = 10;
                else if (partUserInfo.AgentType == 4)
                    sendCount = 25;
                if (sendCount > 0)
                {
                    AdminCoupons.AdminSendCouponToUser(partUserInfo.Uid, WebHelper.GetConfigSettingsInt("YXCouponTid"), WebHelper.GetConfigSettingsInt("YX"), 20, sendCount, partUserInfo.Uid, DateTime.Now, "", 0, DateTime.Now, new DateTime(2100, 1, 1), "2");
                    MallAdminLogs.CreateMallAdminLog(1, "系统", 2, "系统管理员", "", "代理激活按用户id发放优惠劵", string.Format("用户id:{0},优惠劵类型id:{1},名称:{2},发放数量:{3}", partUserInfo.Uid, WebHelper.GetConfigSettingsInt("YXCouponTid"), "代理激活20元优惠券", sendCount));
                }

            }
            //汇购优选正价品店铺
            else if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("YX"))
            {
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = orderInfo.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.Confirm,
                    ActionTime = DateTime.Now,//交易时间,
                    ActionDes = "您的订单已经确认,正在备货中"
                });
                AdminOrders.PreProduct(orderInfo);
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = orderInfo.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.PreProduct,
                    ActionTime = DateTime.Now.AddSeconds(50),//交易时间,
                    ActionDes = "您的订单已经备货完成，等待出库"
                });
            }
            #endregion
            else
            {
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = orderInfo.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.Confirm,
                    ActionTime = DateTime.Now,//交易时间,
                    ActionDes = "您的订单已经确认,正在备货中"
                });
            }

        }

        /// <summary>
        /// 备货
        /// </summary>
        /// <param name="orderInfo">订单信息</param>
        public static void PreProduct(OrderInfo orderInfo)
        {
            UpdateOrderState(orderInfo.Oid, OrderState.PreProducting);
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
        public static void SendOrder(int oid, OrderState orderState, string shipSN, int shipCoId, string shipCoName, DateTime shipTime)
        {
            VMall.Data.Orders.SendOrderProduct(oid, orderState, shipSN, shipCoId, shipCoName, shipTime);
        }

        /// <summary>
        /// 完成订单
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="completeTime">完成时间</param>
        /// <param name="ip">ip</param>
        public static void CompleteOrder(ref PartUserInfo partUserInfo, OrderInfo orderInfo, DateTime completeTime, string ip)
        {
            UpdateOrderState(orderInfo.Oid, OrderState.Completed);//将订单状态设为完成状态

            //订单商品列表
            List<OrderProductInfo> orderProductList = GetOrderProductList(orderInfo.Oid);

            //发放完成订单积分
            Credits.SendCompleteOrderCredits(ref partUserInfo, orderInfo, orderProductList, completeTime);

            //发放单品促销活动支付积分和优惠劵
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0)
                {
                    if (orderProductInfo.PayCredits > 0)
                        Credits.SendSinglePromotionCredits(ref partUserInfo, orderInfo, orderProductInfo.PayCredits, completeTime);
                    if (orderProductInfo.CouponTypeId > 0)
                        Coupons.SendSinglePromotionCoupon(partUserInfo, orderProductInfo.CouponTypeId, orderInfo, ip);
                }
            }
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="completeTime">完成时间</param>
        /// <param name="ip">ip</param>
        public static void CompleteOrderNew(ref PartUserInfo partUserInfo, OrderInfo orderInfo, DateTime completeTime, string ip)
        {
            ConfirmReceiving(orderInfo.Oid, OrderState.Completed, DateTime.Now);//将订单状态设为完成状态,并更新收货时间

            //订单商品列表
            List<OrderProductInfo> orderProductList = GetOrderProductList(orderInfo.Oid);

            //发放完成订单积分
            Credits.SendCompleteOrderCredits(ref partUserInfo, orderInfo, orderProductList, completeTime);

            //发放单品促销活动支付积分和优惠劵
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0)
                {
                    if (orderProductInfo.PayCredits > 0)
                        Credits.SendSinglePromotionCredits(ref partUserInfo, orderInfo, orderProductInfo.PayCredits, completeTime);
                    //if (orderProductInfo.CouponTypeId > 0)
                    //    if (orderInfo.CouponMoney <= 0)
                    //        Coupons.SendSinglePromotionCoupon(partUserInfo, orderProductInfo.CouponTypeId, orderInfo, ip);
                }
            }
        }
        /// <summary>
        /// 退货申请
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="operatorId">操作人id</param>
        /// <param name="returnTime">退货时间</param>
        public static void ReturnApply(OrderInfo orderInfo, int operatorId, DateTime returnApplyTime, string returnDesc)
        {
            UpdateOrderReturnType(orderInfo.Oid, 1);//将订单returntype设为审核状态
            OrderReturnInfo info = OrderReturn.GetOrderReturnByOid(orderInfo.Oid);
            if (info == null)
            {
                OrderReturn.CreateOrderReturn(new OrderReturnInfo()
                {
                    CreationDate = DateTime.Now,
                    LastModifity = DateTime.Now,
                    StoreId = orderInfo.StoreId,
                    StoreName = orderInfo.StoreName,
                    Uid = orderInfo.Uid,
                    Oid = orderInfo.Oid,
                    OSN = orderInfo.OSN,
                    State = 1,
                    ReturnDesc = returnDesc,
                    ReturnShipDesc = "",
                    ReturnShipFee = 0M

                });//加入退货表记录
            }
            else
            {
                OrderReturn.UpdateOrderReturn(info.ReturnId, 1, DateTime.Now, returnDesc);
            }
        }

        /// <summary>
        /// 换货申请
        /// </summary>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="operatorId">操作人id</param>
        /// <param name="returnTime">换货申请时间</param>
        /// <param name="said">配送地址id</param>
        public static void ChangeApply(OrderInfo orderInfo, int operatorId, DateTime returnTime, int said, int changeType, string changeDesc)
        {
            // UpdateOrderState(orderInfo.Oid, OrderState.Changed);//将订单状态设为换货状态
            UpdateOrderChangeType(orderInfo.Oid, 1);//将订单changetype设为审核状态
            OrderChangeInfo info = OrderChange.GetChangeOrderByOid(orderInfo.Oid);
            if (orderInfo.OrderState == (int)OrderState.Completed)//用户收货后换货
            {
                if (info == null)
                    //此操作将换货记录保存到表'orderchange'中
                    OrderChange.CreateOrderChange(new OrderChangeInfo
               {
                   StoreId = orderInfo.StoreId,
                   StoreName = orderInfo.StoreName,
                   Oid = orderInfo.Oid,
                   OSN = orderInfo.OSN,
                   Uid = orderInfo.Uid,
                   State = 1,
                   SAId = said,
                   ChangeType = changeType,
                   ChangeDesc = changeDesc
               });
                else
                    OrderChange.UpdateOrderChange(info.ChangeId, 1, DateTime.Now, changeDesc);
            }
            //Products.IncreaseProductStockNumber(GetOrderProductList(orderInfo.Oid));//增加商品库存数量
        }
        /// <summary>
        /// 订单returntype状态
        /// </summary>
        public static void UpdateOrderReturnType(int oid, int returnType)
        {

            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@returntype",returnType)
                                   };
            string commandText = string.Format("UPDATE [{0}orders] SET [returntype]=@returntype WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);

            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 订单returntype状态修改并退货确认退账户金额
        /// </summary>
        public static void UpdateOrderReturnTypeTo2(ref PartUserInfo partUserInfo, OrderInfo orderInfo, int operatorId, DateTime returnTime, int returnType)
        {
            ReturnOrder(ref partUserInfo, orderInfo, operatorId, returnTime);
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",orderInfo.Oid),
                                       new SqlParameter("@returntype",returnType)
                                   };
            string commandText = string.Format("UPDATE [{0}orders] SET [returntype]=@returntype WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);

            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 订单changetype状态
        /// </summary>
        public static void UpdateOrderChangeType(int oid, int changetype)
        {

            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@changetype",changetype)
                                   };
            string commandText = string.Format("UPDATE [{0}orders] SET [changetype]=@changetype WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);

            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 退款更新--更新orders.returnedtype为4 orderreturn.state为4
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="returnTyepe"></param>
        public static void UpdateOrderForRefund(int oid)
        {
            UpdateOrderReturnType(oid, 4);

            OrderReturn.UpdateOrderReturnByOid(oid, 4, DateTime.Now);
        }

        /// <summary>
        /// 退货
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="operatorId">操作人id</param>
        /// <param name="returnTime">退货时间</param>
        public static void ReturnOrder(ref PartUserInfo partUserInfo, OrderInfo orderInfo, int operatorId, DateTime returnTime)
        {
            UpdateOrderState(orderInfo.Oid, OrderState.Returned);//将订单状态设为退货状态
            //存在推广产品配置退回99红包
            //List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
            //if (orderProductList.Exists(x => x.Pid == TypeHelper.StringToInt(WebConfigurationManager.AppSettings["ActiveProduct"], 0)))
            //{
            //    OrderProductInfo orderProductInfo = orderProductList.Find(x => x.Pid == TypeHelper.StringToInt(WebConfigurationManager.AppSettings["ActiveProduct"], 0));
            //    AccountUtils.ReturnActiveHongBao(partUserInfo, orderInfo, 99*orderProductInfo.BuyCount, partUserInfo.Uid, DateTime.Now);
            //}

            if (orderInfo.OrderState == (int)OrderState.Sended)//用户收货时退货
            {
                //if (orderInfo.CouponMoney > 0)//退回用户使用的优惠劵
                //    Coupons.ReturnUserOrderUseCoupons(orderInfo.Oid);

                //if (orderInfo.PayCreditCount > 0)//退回用户使用的积分
                //    Credits.ReturnUserOrderUseCredits(ref partUserInfo, orderInfo, operatorId, returnTime);
                if (orderInfo.HaiMiDiscount > 0)//退回使用海米
                    AccountUtils.ReturnUserHaimi(partUserInfo, orderInfo, operatorId, returnTime);
                if (orderInfo.HongBaoDiscount > 0)//退回使用红包
                    AccountUtils.ReturnUserHongBao(partUserInfo, orderInfo, operatorId, returnTime);
                //if (orderInfo.PaySN.Length > 0)//退回用户支付的金钱(此操作只是将退款记录保存到表'orderrefunds'中，实际退款还需要再次操作)
                //    OrderRefunds.ApplyRefund(new OrderRefundInfo
                //    {
                //        Oid = orderInfo.Oid,
                //        OSN = orderInfo.OSN,
                //        Uid = orderInfo.Uid,
                //        State = 0,
                //        ApplyTime = returnTime,
                //        PayMoney = orderInfo.SurplusMoney,
                //        RefundMoney = orderInfo.SurplusMoney,
                //        PaySN = orderInfo.PaySN,
                //        PaySystemName = orderInfo.PaySystemName,
                //        PayFriendName = orderInfo.PayFriendName
                //    });

            }
            else if (orderInfo.OrderState == (int)OrderState.Completed)//订单完成后退货
            {
                //if (orderInfo.CouponMoney > 0)//退回用户使用的优惠劵
                //    Coupons.ReturnUserOrderUseCoupons(orderInfo.Oid);

                //if (orderInfo.PayCreditCount > 0)//退回用户使用的积分
                //    Credits.ReturnUserOrderUseCredits(ref partUserInfo, orderInfo, operatorId, returnTime);
                if (orderInfo.HaiMiDiscount > 0)//退回使用海米
                    AccountUtils.ReturnUserHaimi(partUserInfo, orderInfo, operatorId, returnTime);
                if (orderInfo.HongBaoDiscount > 0)//退回使用红包
                    AccountUtils.ReturnUserHongBao(partUserInfo, orderInfo, operatorId, returnTime);
                if (orderInfo.AgentDiscount > 0)
                {
                    Account.UpdateAccountForIn(new AccountInfo() { UserId = orderInfo.Uid, AccountId = (int)AccountType.代理账户, TotalIn = orderInfo.AgentDiscount });
                    Account.CreateAccountDetail(new AccountDetailInfo()
                    {
                        AccountId = (int)AccountType.代理账户,
                        UserId = orderInfo.Uid,
                        CreateTime = DateTime.Now,
                        DetailType = (int)DetailType.订单取消返回,
                        InAmount = orderInfo.AgentDiscount,
                        OutAmount = 0,
                        OrderCode = orderInfo.OSN,
                        AdminUid = operatorId,
                        Status = 1,
                        DetailDes = "订单取消账户金额返回：订单号：" + orderInfo.OSN + ",返回金额：" + orderInfo.AgentDiscount
                    });
                }
                if (orderInfo.CommisionDiscount > 0)
                {
                    Account.UpdateAccountForIn(new AccountInfo() { UserId = orderInfo.Uid, AccountId = (int)AccountType.佣金账户, TotalIn = orderInfo.CommisionDiscount });
                    Account.CreateAccountDetail(new AccountDetailInfo()
                    {
                        AccountId = (int)AccountType.佣金账户,
                        UserId = orderInfo.Uid,
                        CreateTime = DateTime.Now,
                        DetailType = (int)DetailType.订单取消返回,
                        InAmount = orderInfo.CommisionDiscount,
                        OutAmount = 0,
                        OrderCode = orderInfo.OSN,
                        AdminUid = operatorId,
                        Status = 1,
                        DetailDes = "订单取消账户金额返回：订单号：" + orderInfo.OSN + ",返回金额：" + orderInfo.CommisionDiscount
                    });
                }
                //应退金钱
                decimal returnMoney = orderInfo.SurplusMoney;

                //订单发放的积分
                DataTable sendCredits = Credits.GetUserOrderSendCredits(orderInfo.Oid);
                int payCreditAmount = TypeHelper.ObjectToInt(sendCredits.Rows[0]["paycreditamount"]);
                int rankCreditAmount = TypeHelper.ObjectToInt(sendCredits.Rows[0]["rankcreditamount"]);
                //判断用户当前积分是否足够退回，如果不足够就将差额核算成金钱并在应退金钱中减去
                if (partUserInfo.PayCredits < payCreditAmount)
                {
                    returnMoney = returnMoney - Credits.PayCreditsToMoney(payCreditAmount - partUserInfo.PayCredits);
                    payCreditAmount = partUserInfo.PayCredits;
                }
                //收回订单发放的积分
                Credits.ReturnUserOrderSendCredits(ref partUserInfo, orderInfo, payCreditAmount, rankCreditAmount, operatorId, returnTime);

                StringBuilder couponIdList = new StringBuilder();
                //订单发放的优惠劵列表
                List<CouponInfo> couponList = Coupons.GetUserOrderSendCouponList(orderInfo.Oid);
                //判断优惠劵是否已经被使用，如果已经使用就在应退金钱中减去优惠劵金额
                foreach (CouponInfo couponInfo in couponList)
                {
                    if (couponInfo.Oid > 0)
                        returnMoney = returnMoney - couponInfo.Money;
                    else
                        couponIdList.AppendFormat("{0},", couponInfo.CouponId);
                }
                //收回订单发放的优惠劵
                if (couponIdList.Length > 0)
                {
                    Coupons.DeleteCouponById(couponIdList.Remove(couponIdList.Length - 1, 1).ToString());
                }

                //if (returnMoney > 0)//退回用户支付的金钱(此操作只是将退款记录保存到表'orderrefunds'中，实际退款还需要再次操作)
                //    OrderRefunds.ApplyRefund(new OrderRefundInfo
                //    {
                //        Oid = orderInfo.Oid,
                //        OSN = orderInfo.OSN,
                //        Uid = orderInfo.Uid,
                //        State = 0,
                //        ApplyTime = returnTime,
                //        PayMoney = orderInfo.SurplusMoney,
                //        RefundMoney = returnMoney,
                //        PaySN = orderInfo.PaySN,
                //        PaySystemName = orderInfo.PaySystemName,
                //        PayFriendName = orderInfo.PayFriendName
                //    });
            }

            Products.IncreaseProductStockNumber(GetOrderProductList(orderInfo.Oid));//增加商品库存数量

            //非汇购卡支付 直销用户 订单已经确认后 收获前 才可以取消
            if (orderInfo.CashDiscount <= 0 && partUserInfo.IsDirSaleUser && orderInfo.OrderState >= (int)OrderState.Completed)
            {
                OrderUtils.CancelQDOrder(orderInfo, partUserInfo, "7天无条件退货");
            }
            //直销用户注销(订单满足条件，会员满足新直销用户条件),库存退回，代理身份撤销
            if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId") || orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId"))
            {
                if (partUserInfo.AgentType > 0 && orderInfo.OrderState >= (int)OrderState.Confirmed && orderInfo.OrderState < (int)OrderState.Completed)
                {

                    string sqlStr = string.Format(" orderstate>={0} and orderstate<={1} and uid={2} and (storeid={3} or storeid={4}) ", (int)OrderState.Confirmed, (int)OrderState.Completed, orderInfo.Uid, WebHelper.GetConfigSettingsInt("AgentStoreId"), WebHelper.GetConfigSettingsInt("AgentSuitStoreId"));
                    List<OrderInfo> UserAvailAgentOrderList = Orders.GetOrderListByWhere(sqlStr);
                    if (UserAvailAgentOrderList.Count <= 0)
                    {


                    }
                }
                //取消代理订单后减相应库存
                if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"))
                {
                    //获得有效代理上级用户
                    PartUserInfo parentUser = Users.GetParentUserForAgent(partUserInfo);
                    List<OrderProductInfo> productList = Orders.GetOrderProductList(orderInfo.Oid);
                    foreach (var item in productList)
                    {
                        new AgentStock().UpdateAgentStockForCancel(parentUser, partUserInfo, orderInfo, item);
                    }
                }
            }
            //该订单使用的兑换码重新可用
            List<ExChangeCouponsInfo> exchangelist = ExChangeCoupons.GetList(string.Format(" oid={0} ", orderInfo.Oid));
            if (exchangelist.Count > 0)
            {
                foreach (var item in exchangelist)
                {
                    item.oid = 0;
                    item.usetime = item.createtime;
                    ExChangeCoupons.Update(item);
                }
            }
            //订单赠送兑换码取消
            ExChangeCouponsInfo exInfo = ExChangeCoupons.GetModel(string.Format(" [createoid]={0} ", orderInfo.Oid));
            if (exInfo != null)
            {
                exInfo.validtime = DateTime.Now;
                exInfo.state = 0;
                ExChangeCoupons.Update(exInfo);
            }

        }



        /// <summary>
        /// 锁定订单
        /// </summary>
        /// <param name="orderInfo">订单信息</param>
        public static void LockOrder(OrderInfo orderInfo)
        {
            UpdateOrderState(orderInfo.Oid, OrderState.Locked);
            Products.IncreaseProductStockNumber(GetOrderProductList(orderInfo.Oid));//增加商品库存数量
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="operatorId">操作人id</param>
        /// <param name="cancelTime">取消时间</param>
        public static void CancelOrder(ref PartUserInfo partUserInfo, OrderInfo orderInfo, int operatorId, DateTime cancelTime)
        {
            UpdateOrderState(orderInfo.Oid, OrderState.Cancelled);//将订单状态设为取消状态

            if (orderInfo.CouponMoney > 0)//退回用户使用的优惠劵
                Coupons.ReturnUserOrderUseCoupons(orderInfo.Oid);

            if (orderInfo.PayCreditCount > 0)//退回用户使用的积分
                Credits.ReturnUserOrderUseCredits(ref partUserInfo, orderInfo, operatorId, cancelTime);
            if (orderInfo.HaiMiDiscount > 0)//退回使用海米
                AccountUtils.ReturnUserHaimi(partUserInfo, orderInfo, operatorId, cancelTime);
            if (orderInfo.HongBaoDiscount > 0)//退回使用红包
                AccountUtils.ReturnUserHongBao(partUserInfo, orderInfo, operatorId, cancelTime);
            if (orderInfo.CashDiscount > 0)//退回使用汇购卡券
                CashCoupon.ReturnUserCash(partUserInfo, orderInfo);
            if (orderInfo.AgentDiscount > 0)
            {
                Account.UpdateAccountForIn(new AccountInfo() { UserId = orderInfo.Uid, AccountId = (int)AccountType.代理账户, TotalIn = orderInfo.AgentDiscount });
                Account.CreateAccountDetail(new AccountDetailInfo()
                {
                    AccountId = (int)AccountType.代理账户,
                    UserId = orderInfo.Uid,
                    CreateTime = DateTime.Now,
                    DetailType = (int)DetailType.订单取消返回,
                    InAmount = orderInfo.AgentDiscount,
                    OutAmount = 0,
                    OrderCode = orderInfo.OSN,
                    AdminUid = operatorId,
                    Status = 1,
                    DetailDes = "订单取消账户金额返回：订单号：" + orderInfo.OSN + ",返回金额：" + orderInfo.AgentDiscount
                });
            }
            if (orderInfo.CommisionDiscount > 0)
            {
                Account.UpdateAccountForIn(new AccountInfo() { UserId = orderInfo.Uid, AccountId = (int)AccountType.佣金账户, TotalIn = orderInfo.CommisionDiscount });
                Account.CreateAccountDetail(new AccountDetailInfo()
                {
                    AccountId = (int)AccountType.佣金账户,
                    UserId = orderInfo.Uid,
                    CreateTime = DateTime.Now,
                    DetailType = (int)DetailType.订单取消返回,
                    InAmount = orderInfo.CommisionDiscount,
                    OutAmount = 0,
                    OrderCode = orderInfo.OSN,
                    AdminUid = operatorId,
                    Status = 1,
                    DetailDes = "订单取消账户金额返回：订单号：" + orderInfo.OSN + ",返回金额：" + orderInfo.CommisionDiscount
                });
            }
            if (orderInfo.OrderState == (int)OrderState.Confirmed)
            {

            }
            Products.IncreaseProductStockNumber(GetOrderProductList(orderInfo.Oid));//增加商品库存数量
            //该订单使用的兑换码重新可用
            List<ExChangeCouponsInfo> exchangelist = ExChangeCoupons.GetList(string.Format(" oid={0} ", orderInfo.Oid));
            if (exchangelist.Count > 0)
            {
                foreach (var item in exchangelist)
                {
                    item.oid = 0;
                    item.usetime = item.createtime;
                    ExChangeCoupons.Update(item);
                }
            }

            if (partUserInfo != null)
            {
                //非汇购卡支付 直销用户 订单已经确认后 收获前 才可以取消  ,不属于尚睿淳店铺,不属于充值店（充值没有收货地址，推送异常）
                if (orderInfo.CashDiscount <= 0 && partUserInfo.IsDirSaleUser && orderInfo.OrderState >= (int)OrderState.Confirmed && orderInfo.OrderState < (int)OrderState.Completed && orderInfo.StoreId.ToString() != WebHelper.GetConfigSettings("ChongZhiStore") && orderInfo.StoreId.ToString() != WebHelper.GetConfigSettings("SRCStoreId") && orderInfo.StoreId.ToString() != WebHelper.GetConfigSettings("AgentStoreId") && orderInfo.StoreId.ToString() != WebHelper.GetConfigSettings("AgentSuitStoreId"))
                {
                    OrderUtils.CancelQDOrder(orderInfo, partUserInfo, "用户主动取消订单");
                }
                //直销用户注销(订单满足条件，会员满足新直销用户条件),库存退回，代理身份撤销
                if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId") || orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId"))
                {
                    if (orderInfo.OrderState >= (int)OrderState.Confirmed && orderInfo.OrderState <= (int)OrderState.Completed)
                    {
                        if (partUserInfo.AgentType > 0)
                        {
                            string sqlStr = string.Format(" orderstate>={0} and orderstate<={1} and uid={2} and (storeid={3} or storeid={4}) ", (int)OrderState.Confirmed, (int)OrderState.Completed, orderInfo.Uid, WebHelper.GetConfigSettingsInt("AgentStoreId"), WebHelper.GetConfigSettingsInt("AgentSuitStoreId"));
                            List<OrderInfo> UserAvailAgentOrderList = Orders.GetOrderListByWhere(sqlStr);
                            if (UserAvailAgentOrderList.Count <= 0)
                            {

                            }
                        }
                        //取消代理订单后减相应库存
                        if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"))
                        {
                            //获得有效代理上级用户
                            PartUserInfo parentUser = Users.GetParentUserForAgent(partUserInfo);
                            List<OrderProductInfo> productList = Orders.GetOrderProductList(orderInfo.Oid);
                            foreach (var item in productList)
                            {
                                new AgentStock().UpdateAgentStockForCancel(parentUser, partUserInfo, orderInfo, item);
                            }
                        }
                    }

                }
            }
            //订单赠送兑换码取消
            ExChangeCouponsInfo exInfo = ExChangeCoupons.GetModel(string.Format(" [createoid]={0} ", orderInfo.Oid));
            if (exInfo != null)
            {
                exInfo.validtime = DateTime.Now;
                exInfo.state = 0;
                ExChangeCoupons.Update(exInfo);
            }
        }

        /// <summary>
        /// 删除订单/取消删除
        /// </summary>
        /// <param name="oid">订单号</param>
        /// <param name="isDel">是否删除 true删除 false 取消删除（针对后台操作）</param>
        public static void DeleteOrder(int oid, bool isDel)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid)
                                   };
            string commandText = string.Empty;
            if (isDel)
                commandText = string.Format("UPDATE [{0}orders] SET [isdelete]=1 WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);
            else
                commandText = string.Format("UPDATE [{0}orders] SET [isdelete]=0 WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);

            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 延长收货期
        /// </summary>
        /// <param name="oid">订单号</param>
        public static void ExtendReceive(int oid)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid)
                                   };
            string commandText = string.Format("UPDATE [{0}orders] SET [isextendreceive]=1 WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);

            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        #endregion

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        public static void UpdateOrderState(int oid, OrderState orderState)
        {
            VMall.Data.Orders.UpdateOrderState(oid, orderState);
        }

        /// <summary>
        /// 更新订单结算状态
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单结算状态</param>
        public static void UpdateOrderSettleState(int oid, OrderSettleState orderSettleState)
        {
            VMall.Data.Orders.UpdateOrderSettleState(oid, orderSettleState);
        }

        /// <summary>
        /// 更新订单月结状态
        /// </summary>
        /// <param name="oid"></param>
        public static void UpdateOrderMonthSettled(int oid)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid)
                                   };
            string commandText = string.Format("UPDATE [{0}orders] SET [monthsettled]=1 WHERE [oid]=@oid",
                                               RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="orderState">订单状态</param>
        public static void ConfirmReceiving(int oid, OrderState orderState, DateTime receivingTime)
        {
            VMall.Data.Orders.ConfirmReceiving(oid, orderState, receivingTime);
        }

        /// <summary>
        /// 更新订单的评价
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="isReview">是否评价</param>
        public static void UpdateOrderIsReview(int oid, int isReview)
        {
            VMall.Data.Orders.UpdateOrderIsReview(oid, isReview);
        }

        #region 订单列表、根据条件查找订单
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
        public static DataTable GetUserOrderList(int uid, int pageSize, int pageNumber, string startAddTime, string endAddTime, int orderState, int mallSource = 0)
        {
            return VMall.Data.Orders.GetUserOrderList(uid, pageSize, pageNumber, startAddTime, endAddTime, orderState, mallSource);
        }

        /// <summary>
        /// 获得用户订单数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        public static int GetUserOrderCount(int uid, string startAddTime, string endAddTime, int orderState, int mallSource = 0)
        {
            return VMall.Data.Orders.GetUserOrderCount(uid, startAddTime, endAddTime, orderState, mallSource);
        }

        /// <summary>
        /// 是否评价了所有订单商品
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static bool IsReviewAllOrderProduct(List<OrderProductInfo> orderProductList)
        {
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.IsReview == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 根据条件获取订单列表数量
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static int GetOrderListCoutByWhere(string condition)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" where ");
                sb.Append(condition);
            }
            string commandText = "select Count(*) from hlh_orders " + sb.ToString();
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText));
            ;
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
        public static List<OrderInfo> GetOrderListByWhereForPage(int pageSize, int pageNo, string condition)
        {
            List<OrderInfo> orderInfoList = new List<OrderInfo>();
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(condition))
            {
                //sb.Append(" where ");
                sb.Append(condition);
            }
            //string commandText = "select * from hlh_orders " + sb.ToString();
            string commandText = string.Empty;
            if (pageNo == 1)
            {
                commandText = string.Format(@"SELECT TOP {1} * FROM [{0}orders] where " + condition + "  ORDER BY [addtime] DESC", RDBSHelper.RDBSTablePre, pageSize);
            }
            else
            {
                commandText = @"with tb as( 
select *,ROW_NUMBER() OVER (ORDER BY addtime DESC)  AS RowNumber from [hlh_orders] where " + condition + ") select * from tb where  RowNumber between (" + pageNo + @"-1)*" + pageSize + @"+1 and (" + pageNo + @")*" + pageSize + " order by tb.[addtime] desc)";

            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    OrderInfo orderInfo = VMall.Data.Orders.BuildOrderFromReader(reader);
                    orderInfoList.Add(orderInfo);
                }
                reader.Close();
            }
            return orderInfoList;
        }

        /// <summary>
        /// 根据条件获取订单列表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static List<OrderInfo> GetOrderListByWhere(string condition)
        {
            List<OrderInfo> orderInfoList = new List<OrderInfo>();
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" where ");
                sb.Append(condition);
            }
            string commandText = "select * from hlh_orders " + sb.ToString();
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    OrderInfo orderInfo = VMall.Data.Orders.BuildOrderFromReader(reader);
                    orderInfoList.Add(orderInfo);
                }
                reader.Close();
            }
            return orderInfoList;
        }

        #endregion

        #region 自动收货、自动取消过期订单
        /// <summary>
        /// 自动确认收货
        /// </summary>
        public static void AutoConfirmRevecing(int timeValue)
        {
            int extentReceiveDay = 5;
            try
            {
                //TODO 执行方法  读取订单表满足自动收获以及满足自动结算的订单  
                int atuoConfirmOrderTime = timeValue;//Convert.ToInt32(WebConfigurationManager.AppSettings["AutoConfirmOrderTime"]);
                //SELECT *FROM dbo.hlh_orders WHERE orderstate=110 AND ((shiptime<='2016-05-05 15:13:58.387' AND isextendreceive=0) or (shiptime<='2016-05-02 15:13:58.387' AND isextendreceive=1))
                string sqlStr = string.Format(" orderstate={0} and ( ( shiptime<='{1}' AND isextendreceive=0 ) or ( shiptime<='{2}' AND isextendreceive=1 ) )", (int)OrderState.Sended, DateTime.Now.AddDays(-atuoConfirmOrderTime).ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.AddDays(-atuoConfirmOrderTime - extentReceiveDay).ToString("yyyy-MM-dd HH:mm:ss.fff"));//已经发货并且发货时间在7天之前的订单、或已经发货申请了延期收货，发货时间在7+5天之前的订单
                List<OrderInfo> orderList = GetOrderListByWhere(sqlStr);
                foreach (OrderInfo item in orderList)
                {
                    if (item.OrderState != (int)OrderState.Sended)
                        return;
                    DateTime updateDate = DateTime.Now;

                    PartUserInfo partUserInfo = Users.GetPartUserById(item.Uid);
                    Orders.CompleteOrderNew(ref partUserInfo, item, updateDate, "");

                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = item.Oid,
                        Uid = item.Uid,
                        RealName = "系统",
                        ActionType = (int)OrderActionType.Complete,
                        ActionTime = updateDate,//交易时间,
                        ActionDes = "订单已完成，感谢您在" + BMAConfig.MallConfig.MallName + "购物，欢迎您再次光临"
                    });
                    MallAdminLogs.CreateMallAdminLog(2, "system", 2, "系统管理员", "127.0.0.1", "自动收货完成订单", "完成订单,订单ID为:" + item.Oid);
                    LogHelper.WriteOperateLog("AutoConfirmReceiving", "订单自动收货服务", "订单号：" + item.Oid + "|更新时间：" + updateDate);
                }
            }
            catch (Exception ex)
            {
                //记录执行异常
                LogHelper.WriteOperateLog("AutoConfirmReceivingError", "自动确认收货", "错误信息：" + ex.Message, (int)LogLevelEnum.ERROR);
            }
        }

        /// <summary>
        /// 自动取消未付款
        /// </summary>
        public static void AutoCancelNoPayOrder(int timeValue)
        {
            // LogHelper.WriteOperateLog("自动取消进入", "自动取消进入方法", "=================");
            try
            {
                //TODO 执行方法  读取订单表满足未支付超过24小时的订单  
                int atuoCancelOrderTime = timeValue;//单位分钟
                //SELECT *FROM dbo.hlh_orders WHERE addtime<=DATEADD(d,-7,GETDATE()) AND orderstate=30
                string sqlStr = string.Format(" orderstate={0} and  addtime<='{1}' ", (int)OrderState.WaitPaying, DateTime.Now.AddHours(-24).ToString("yyyy-MM-dd HH:mm:ss.fff"));//未支付超过24小时的订单
                List<OrderInfo> orderList = GetOrderListByWhere(sqlStr);
                foreach (OrderInfo item in orderList)
                {
                    if (item.OrderState != (int)OrderState.WaitPaying)
                        return;
                    DateTime updateDate = DateTime.Now;

                    PartUserInfo partUserInfo = Users.GetPartUserById(item.Uid);
                    //取消订单
                    Orders.CancelOrder(ref partUserInfo, item, partUserInfo.Uid, DateTime.Now);

                    OrderActions.CreateOrderAction(new OrderActionInfo()
                    {
                        Oid = item.Oid,
                        Uid = item.Uid,
                        RealName = "系统",
                        ActionType = (int)OrderActionType.Cancel,
                        ActionTime = updateDate,//交易时间,
                        ActionDes = "订单超过" + atuoCancelOrderTime + "小时未支付，系统自动取消订单"
                    });

                    LogHelper.WriteOperateLog("AutoCancelNoPayOrder", "未支付订单过期自动取消服务", "订单号：" + item.Oid + "|取消时间：" + updateDate);
                }
            }
            catch (Exception ex)
            {
                //记录执行异常
                LogHelper.WriteOperateLog("AutoCancelNoPayOrderError", "自动取消未付款操作服务异常", "错误信息：" + ex.Message, (int)LogLevelEnum.ERROR);
            }
        }

        #endregion

        #region 结算
        /// <summary>
        /// 获得订单用户的推荐关系
        /// </summary>
        /// <param name="partUserInfo"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static List<PartUserInfo> GetOrderUserLevel(PartUserInfo partUserInfo, int level, List<PartUserInfo> parentList = null)
        {
            List<PartUserInfo> userList = null;
            if (level > 0)
            {
                if (parentList == null)
                {
                    userList = new List<PartUserInfo>();
                }
                else
                {
                    userList = parentList;
                }
                if (partUserInfo == null) return userList;

                //先要判断时候存在直销帐号 存在直销则不由汇购结算
                if (partUserInfo.IsDirSaleUser)
                {
                    return userList;
                }
                //判断推荐人是否为直销推荐人 是则不由汇购结算
                else if (partUserInfo.Ptype == 2)
                {
                    return userList;
                }
                else
                {
                    PartUserInfo parentUser = Users.GetPartUserById(partUserInfo.Pid);
                    if (parentUser != null && parentUser.IsFXUser == 1)
                    {
                        userList.Add(parentUser);
                        level--;
                    }
                    GetOrderUserLevel(parentUser, level, userList);
                }
            }

            return userList;
        }

        private static string dirSaleApiUrl = WebConfigurationManager.AppSettings["DirsaleApiUrl"];
        /// <summary>
        /// 结算--计算每个帐号的结算额，并分配至每个帐号，同时将此订单状态改为已结算
        /// </summary>
        /// <param name="settleLevel"></param>
        /// <param name="settleUserList"></param>
        /// <param name="totalValue"></param>
        public static void SettleAccount(int[] settleLevel, List<PartUserInfo> settleUserList, decimal totalValue, OrderInfo orderinfo, PartUserInfo parentUser)
        {
            bool isOK = false;
            int noSettleCount = settleLevel.Length - settleUserList.Count;
            //未结算数大于0 说明直属上层存在直销会员推荐 此时计算提交给后台系统结算
            if (noSettleCount > 0)
            {
                int HGUid = noSettleCount >= 3 ? parentUser.Uid : settleUserList.Last().Uid;
                int DirUid = noSettleCount >= 3 ? parentUser.Pid : settleUserList.Last().Pid;
                //http://www.xxxx.com/api/user/HaimiDistribute?userId=xxx&haimiAmount=xxx&orderState=x&orderCode=xxx&settleState=x&settleLevel=x
                string url = dirSaleApiUrl + "/api/user/HaimiDistribute?userId=" + DirUid + "&haimiAmount=" + totalValue + "&orderState=" +
                    orderinfo.OrderState + "&orderCode=" + orderinfo.OSN + "&settleState=" + orderinfo.SettleState + "&settleLevel=" + (settleUserList.Count + 1);

                LogHelper.WriteOperateLog(" SettleAccountForDirErrLog", "后台结算海米异常记录1", (string.IsNullOrEmpty(url) ? "空url" : url));

                string FromDirSale = WebHelper.DoGet(url);
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    isOK = true;
                    LogHelper.WriteOperateLog(" SettleAccountForDirRightLog", "后台结算海米正常", "详情信息：商城截止会员id为：" + HGUid + ",由直销会员id为：" + DirUid + "的会员开始新结算，向上结算" + noSettleCount + "层");
                }
                else
                {
                    LogHelper.WriteOperateLog(" SettleAccountForDirErrLog", "后台结算海米异常记录1", (string.IsNullOrEmpty(url) ? "空url" : url));
                    LogHelper.WriteOperateLog(" SettleAccountForDirErrLog", "后台结算海米异常记录2", "错误信息：后台返回：" + token["Msg"].ToString() + ":" + url);
                    return;
                }
            }
            //LogHelper.WriteOperateLog("AutoSettleLog", "订单自动结算服务", "直销后台结算成功");

            try
            {
                decimal rate = 0m;
                for (int i = 0; i < settleUserList.Count; i++)
                {
                    if (i == 0)
                        rate = 0.3m;
                    else
                        rate = 0.2m;

                    //totalValue = 170;
                    SettleModel smodel = new SettleModel();
                    smodel.uid = settleUserList[i].Uid;
                    if ((orderinfo.OrderState == (int)OrderState.Confirmed && orderinfo.SettleState == (int)OrderSettleState.NotSettled)
                    || (orderinfo.OrderState == (int)OrderState.Returned && orderinfo.SettleState == (int)OrderSettleState.PreSettle)
                    || (orderinfo.OrderState == (int)OrderState.Cancelled && orderinfo.SettleState == (int)OrderSettleState.PreSettle))
                    {
                        smodel.accountid = (int)AccountType.预结算海米账户;
                        //smodel.accountin = orderinfo.OrderState == (int)OrderState.Confirmed ? totalValue * (settleLevel[i] / 100) : 0m;
                        //smodel.accountout = orderinfo.OrderState == (int)OrderState.Confirmed ? 0m : totalValue * (settleLevel[i] / 100);
                        smodel.accountin = orderinfo.OrderState == (int)OrderState.Confirmed ? 50m : 0m;
                        smodel.accountout = orderinfo.OrderState == (int)OrderState.Confirmed ? 0m : 50m;
                        UpdateAccount(smodel, orderinfo.OSN);
                    }
                    else if (orderinfo.OrderState == (int)OrderState.Completed)
                    {
                        if (orderinfo.SettleState == (int)OrderSettleState.PreSettle)
                        {
                            //如果订单状态是正式确认收货，则先将以前预结算海米账户中的海米转出
                            smodel.accountid = (int)AccountType.预结算海米账户;
                            smodel.accountin = 0m;
                            smodel.accountout = totalValue * (settleLevel[i] / 100);
                            UpdateAccount(smodel, orderinfo.OSN);
                        }
                        //然后将转出的海米加入到正式的海米账户
                        smodel = new SettleModel();
                        smodel.uid = settleUserList[i].Uid;
                        smodel.accountid = (int)AccountType.海米账户;
                        smodel.accountin = totalValue * (settleLevel[i] / 100);
                        smodel.accountout = 0m;
                        UpdateAccount(smodel, orderinfo.OSN);
                    }
                }
                isOK = true;
            }
            catch (Exception ex)
            {
                isOK = false;
                LogHelper.WriteOperateLog(" AutoSettleAccountError", "商城结算海米异常记录", "错误信息：" + ex.Message);
            }

            //Update Order State
            if (isOK)
            {
                if (orderinfo.OrderState == (int)OrderState.Confirmed)
                {
                    UpdateOrderSettleState(orderinfo.Oid, OrderSettleState.PreSettle);
                }
                else if (orderinfo.OrderState == (int)OrderState.Cancelled && orderinfo.SettleState == (int)OrderSettleState.PreSettle)
                {
                    UpdateOrderSettleState(orderinfo.Oid, OrderSettleState.PreSettleCancelled);
                }
                else if (orderinfo.OrderState == (int)OrderState.Returned && orderinfo.SettleState == (int)OrderSettleState.PreSettle)
                {
                    UpdateOrderSettleState(orderinfo.Oid, OrderSettleState.PreSettleCancelled);
                }
                else if (orderinfo.OrderState == (int)OrderState.Completed)
                {
                    UpdateOrderSettleState(orderinfo.Oid, OrderSettleState.Settled);
                }
            }
        }

        private static bool DirSaleSettle(decimal totalValue, OrderInfo orderinfo, PartUserInfo orderUser, int settleLevel)
        {
            //http://www.xxxx.com/api/user/HaimiDistribute?userId=xxx&haimiAmount=xxx&orderState=x&orderCode=xxx&settleState=x&settleLevel=x&highCount=x
            //string url = dirSaleApiUrl + "/api/user/HaimiDistribute?userId=" + orderUser.Pid + "&haimiAmount=" + totalValue + "&orderState=" +
            //    orderinfo.OrderState + "&orderCode=" + orderinfo.OSN + "&settleState=" + orderinfo.SettleState + "&settleLevel=" + settleLevel +
            //    "&highCount=" + highCount;

            bool settledOk = false;
            string FromDirSale = AccountUtils.HaimiDistribute(orderUser.Pid, totalValue, orderinfo.OrderState, orderinfo.OSN, orderinfo.SettleState, settleLevel); //WebHelper.DoGet(url);
            JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            JToken token = (JToken)jsonObject;
            if (token["Result"].ToString() == "0")
            {
                settledOk = true;
                LogHelper.WriteOperateLog(" SettleAccountForDirRightLog", "后台结算海米正常", "详情信息：商城截止会员id为：" + orderUser.Uid
                    + ",由直销会员id为：" + orderUser.Pid + "的会员开始新结算");
            }
            else
            {
                settledOk = false;
                LogHelper.WriteOperateLog(" SettleAccountForDirErrLog", "后台结算海米异常记录", "错误信息：后台返回：" + token["Msg"].ToString());
            }

            return settledOk;
        }

        /// <summary>
        /// 汇购三级分销结算（秒结）
        /// </summary>
        /// <param name="settleLevel"></param>
        /// <param name="settleUserList"></param>
        /// <param name="totalValue"></param>
        public static void SettleAccount(decimal totalValue, OrderInfo orderinfo, PartUserInfo orderUser)
        {
            if (totalValue <= 0 || orderUser == null || orderUser.Pid == 0) return;

            //购物所得海米先100%返回给购物会员
            SettleModel smodel = new SettleModel();
            smodel.uid = orderUser.Uid;
            if ((orderinfo.OrderState >= (int)OrderState.Confirmed && orderinfo.OrderState <= (int)OrderState.Sended
                && orderinfo.SettleState == (int)OrderSettleState.NotSettled)
                || (orderinfo.OrderState == (int)OrderState.Returned && orderinfo.SettleState == (int)OrderSettleState.PreSettle)
                || (orderinfo.OrderState == (int)OrderState.Cancelled && orderinfo.SettleState == (int)OrderSettleState.PreSettle))
            {
                smodel.accountid = (int)AccountType.预结算海米账户;
                smodel.accountin = (orderinfo.OrderState >= (int)OrderState.Confirmed && orderinfo.OrderState <= (int)OrderState.Sended) ? totalValue : 0m;
                smodel.accountout = (orderinfo.OrderState >= (int)OrderState.Confirmed && orderinfo.OrderState <= (int)OrderState.Sended) ? 0m : totalValue;
                UpdateAccount(smodel, orderinfo.OSN);
            }
            else if (orderinfo.OrderState == (int)OrderState.Completed)
            {
                if (orderinfo.SettleState == (int)OrderSettleState.PreSettle)
                {
                    //如果订单状态是正式确认收货，则先将以前预结算海米账户中的海米转出
                    smodel.accountid = (int)AccountType.预结算海米账户;
                    smodel.accountin = 0m;
                    smodel.accountout = totalValue;
                    UpdateAccount(smodel, orderinfo.OSN);
                }
                //然后将转出的海米加入到正式的海米账户
                smodel = new SettleModel();
                smodel.uid = orderUser.Uid;
                smodel.accountid = (int)AccountType.海米账户;
                smodel.accountin = totalValue;
                smodel.accountout = 0m;
                UpdateAccount(smodel, orderinfo.OSN);
            }

            if (orderUser.IsDirSaleUser || orderUser.Ptype == 2)
            {
                bool settledOk = DirSaleSettle(totalValue, orderinfo, orderUser, 0);
                if (settledOk) UpdateSettleState(orderinfo);
                return;
            }

            //SettleModel smodel = null;
            decimal rate = 0m;
            int settleLevel = 0;
            int pId = orderUser.Pid;
            PartUserInfo parentUser = null;
            while (pId > 0)
            {
                parentUser = Users.GetPartUserById(pId);
                if (parentUser != null && parentUser.IsFXUser >= 1)
                {
                    settleLevel++;
                    if (settleLevel == 1)
                        rate = 0.3m;
                    else if (settleLevel <= 3)
                        rate = 0.2m;
                    else
                        rate = 0m;

                    if (settleLevel <= 3)
                    {
                        smodel = new SettleModel();
                        smodel.uid = parentUser.Uid;
                        if ((orderinfo.OrderState >= (int)OrderState.Confirmed && orderinfo.OrderState <= (int)OrderState.Sended
                            && orderinfo.SettleState == (int)OrderSettleState.NotSettled)
                            || (orderinfo.OrderState == (int)OrderState.Returned && orderinfo.SettleState == (int)OrderSettleState.PreSettle)
                            || (orderinfo.OrderState == (int)OrderState.Cancelled && orderinfo.SettleState == (int)OrderSettleState.PreSettle))
                        {
                            smodel.accountid = (int)AccountType.预结算海米账户;
                            smodel.accountin = (orderinfo.OrderState >= (int)OrderState.Confirmed && orderinfo.OrderState <= (int)OrderState.Sended) ? totalValue * rate : 0m;
                            smodel.accountout = (orderinfo.OrderState >= (int)OrderState.Confirmed && orderinfo.OrderState <= (int)OrderState.Sended) ? 0m : totalValue * rate;
                            UpdateAccount(smodel, orderinfo.OSN);
                        }
                        else if (orderinfo.OrderState == (int)OrderState.Completed)
                        {
                            if (orderinfo.SettleState == (int)OrderSettleState.PreSettle)
                            {
                                //如果订单状态是正式确认收货，则先将以前预结算海米账户中的海米转出
                                smodel.accountid = (int)AccountType.预结算海米账户;
                                smodel.accountin = 0m;
                                smodel.accountout = totalValue * rate;
                                UpdateAccount(smodel, orderinfo.OSN);
                            }
                            //然后将转出的海米加入到正式的海米账户
                            smodel = new SettleModel();
                            smodel.uid = parentUser.Uid;
                            smodel.accountid = (int)AccountType.海米账户;
                            smodel.accountin = totalValue * rate;
                            smodel.accountout = 0m;
                            UpdateAccount(smodel, orderinfo.OSN);
                        }
                    }

                    if (parentUser.Ptype == 2)
                    {
                        DirSaleSettle(totalValue, orderinfo, parentUser, settleLevel > 4 ? 4 : settleLevel);
                        break;
                    }
                }
                else if (parentUser != null && parentUser.Ptype == 2)
                {
                    DirSaleSettle(totalValue, orderinfo, parentUser, settleLevel > 4 ? 4 : settleLevel);
                    break;
                }

                if (parentUser == null) break;
                pId = parentUser.Pid;
            }

            //Update Order State
            UpdateSettleState(orderinfo);
        }

        /// <summary>
        /// 汇购优选代理结算（订单确认收货后7天开始结算）
        /// </summary>
        /// <param name="orderinfo"></param>
        /// <param name="orderUser"></param>
        /// <param name="agentPackStoreId"></param>
        /// <param name="agentPackOrders"></param>
        public static void SettleAgentAccount(OrderInfo orderinfo, PartUserInfo orderUser, int agentPackStoreId, Dictionary<int, List<int>> agentPackOrders,
            List<OrderProductInfo> orderProductList)
        {
            if (orderUser.Pid <= 0 || orderinfo.OrderState != (int)OrderState.Completed) return;

            int parentId = -1;
            PartUserInfo parentUser = null;
            //每推荐2个汇购优选代理得1980元现金奖励
            if (orderinfo.StoreId == agentPackStoreId)
            {
                parentId = orderUser.Pid;
                if (orderUser.Ptype == 2)
                {
                    parentId = -1;
                    parentUser = Users.GetPartUserInfoByDirSaleUid(orderUser.Pid);
                    if (parentUser != null)
                    {
                        parentId = parentUser.Uid;
                    }
                }
                if (parentId <= 0) return;

                if (!agentPackOrders.ContainsKey(parentId))
                {
                    List<int> oList = new List<int>() { orderinfo.Oid };
                    agentPackOrders.Add(parentId, oList);
                }
                else
                {
                    agentPackOrders[parentId].Add(orderinfo.Oid);
                }
            }
            else
            {
                LogHelper.WriteOperateLog("AutoSettleAccount", "订单自动结算服务", "777777777777");
                //确认收货后7天后返本金给直接推荐人
                if (orderProductList == null || orderProductList.Count == 0) return;

                LogHelper.WriteOperateLog("AutoSettleAccount", "订单自动结算服务", "888888888888");

                string remark = string.Empty;
                foreach (OrderProductInfo op in orderProductList)
                {
                    LogHelper.WriteOperateLog("AutoSettleAccount", "订单自动结算服务", "9999999999");
                    if (op.FromParentId1 > 0 && op.FromParentAmount1 > 0)
                    {
                        remark = string.Format("返还产品购物本金，购物会员：{0}，订单号：{1}，产品名称：{2}，返回金额：{3}", orderUser.UserName,
                        orderinfo.OSN, op.Name, op.FromParentAmount1.ToString("f2"));
                        LogHelper.WriteOperateLog("AutoSettleAccount", "订单自动结算服务", remark);
                        UpdateAgentAccount(op.FromParentId1, (int)AccountType.代理账户, op.FromParentAmount1, 0m, orderinfo.OSN, remark);
                    }

                    if (op.FromParentId2 > 0 && op.FromParentAmount2 > 0)
                    {
                        remark = string.Format("返还产品购物本金，购物会员：{0}，订单号：{1}，产品名称：{2}，返回金额：{3}", orderUser.UserName,
                        orderinfo.OSN, op.Name, op.FromParentAmount2.ToString("f2"));
                        LogHelper.WriteOperateLog("AutoSettleAccount", "订单自动结算服务", remark);
                        UpdateAgentAccount(op.FromParentId2, (int)AccountType.代理账户, op.FromParentAmount2, 0m, orderinfo.OSN, remark);
                    }

                    LogHelper.WriteOperateLog("AutoSettleAccount", "订单自动结算服务", op.FromParentId3.ToString() + "|" + op.FromParentAmount3.ToString());
                    if (op.FromParentId3 > 0 && op.FromParentAmount3 > 0)
                    {
                        LogHelper.WriteOperateLog("AutoSettleAccount", "订单自动结算服务", "101010101010");
                        remark = string.Format("返还产品购物本金，购物会员：{0}，订单号：{1}，产品名称：{2}，返回金额：{3}", orderUser.UserName,
                        orderinfo.OSN, op.Name, op.FromParentAmount3.ToString("f2"));
                        LogHelper.WriteOperateLog("AutoSettleAccount", "订单自动结算服务", remark);
                        UpdateAgentAccount(op.FromParentId3, (int)AccountType.代理账户, op.FromParentAmount3, 0m, orderinfo.OSN, remark);
                    }

                    if (op.FromParentId4 > 0 && op.FromParentAmount4 > 0)
                    {
                        remark = string.Format("返还产品购物本金，购物会员：{0}，订单号：{1}，产品名称：{2}，返回金额：{3}", orderUser.UserName,
                        orderinfo.OSN, op.Name, op.FromParentAmount4.ToString("f2"));
                        LogHelper.WriteOperateLog("AutoSettleAccount", "订单自动结算服务", remark);
                        UpdateAgentAccount(op.FromParentId4, (int)AccountType.代理账户, op.FromParentAmount4, 0m, orderinfo.OSN, remark);
                    }
                }

                //修改订单结算状态
                Orders.UpdateOrderSettleState(orderinfo.Oid, OrderSettleState.Settled);
            }
        }

        /// <summary>
        /// 微商预结算（有机胚芽）
        /// </summary>
        public static void WeishangPresettle()
        {
            Account.WeishangPresettle();
        }

        /// <summary>
        /// 汇购优选预结算
        /// </summary>
        public static void YouxuanPresettle()
        {
            Account.YouxuanPresettle();
        }

        /// <summary>
        /// 微商正式结算（有机胚芽）
        /// </summary>
        public static void WeishangSettle()
        {
            Account.WeishangSettle();
        }

        /// <summary>
        /// 汇购优选正式结算
        /// </summary>
        public static void YouxuanSettle()
        {
            Account.YouxuanSettle();
        }

        /// <summary>
        /// 订单结算
        /// </summary>
        /// <param name="orderList"></param>
        public static void OrderSettle()
        {
            Account.OrderSettle();
        }

        /// <summary>
        /// 代理1980元产品体验包结算
        /// </summary>
        /// <param name="orderList"></param>
        public static void AgentPackPresettle(List<OrderInfo> orderList)
        {
            if (orderList == null || orderList.Count == 0) return;

            int parentId = -1;
            PartUserInfo parentUser = null;
            Dictionary<int, List<int>> agentPackOrders = new Dictionary<int, List<int>>();
            foreach (OrderInfo item in orderList)
            {
                if (item.OrderState >= (int)OrderState.Confirmed && item.OrderState <= (int)OrderState.Completed &&
                    item.SettleState == (int)OrderSettleState.NotSettled)
                {
                    PartUserInfo orderUser = Users.GetPartUserById(item.Uid);
                    if (orderUser == null || orderUser.Uid <= 0 || orderUser.Pid <= 0) continue;

                    parentId = orderUser.Pid;
                    if (orderUser.Ptype == 2)
                    {
                        parentId = -1;
                        parentUser = Users.GetPartUserInfoByDirSaleUid(orderUser.Pid);
                        if (parentUser != null)
                        {
                            parentId = parentUser.Uid;
                        }
                    }
                    if (parentId <= 0) continue;

                    if (!agentPackOrders.ContainsKey(parentId))
                    {
                        List<int> oList = new List<int>() { item.Oid };
                        agentPackOrders.Add(parentId, oList);
                    }
                    else
                    {
                        agentPackOrders[parentId].Add(item.Oid);
                    }
                }
                else if ((item.OrderState == (int)OrderState.Returned || item.OrderState == (int)OrderState.Cancelled) &&
                    item.SettleState == (int)OrderSettleState.PreSettle)
                {
                    //预结算取消
                    Account.CancelAgentPackPresettle(item.Oid);
                }
            }

            if (agentPackOrders.Count <= 0) return;

            string remark = string.Empty;
            string orderCode = string.Empty;
            foreach (var item in agentPackOrders)
            {
                if (item.Value.Count >= 2)
                {
                    parentUser = Users.GetPartUserById(item.Key);
                    if (parentUser == null || parentUser.Uid == 0) continue;

                    int oCount = item.Value.Count / 2;
                    for (int i = 0; i < oCount; i++)
                    {
                        orderCode = string.Format("OrderId={0}|OrderId={1}", item.Value[i * 2], item.Value[i * 2 + 1]);
                        remark = "代理体验包预结算：推荐两个汇购优选体验包奖励1980元，" + orderCode;
                        UpdateAccountLockBanlance(parentUser.Uid, 2, (int)AccountType.佣金账户, (int)DetailType.代理体验包预结算收入, 1980m, 0, remark, orderCode);
                        Orders.UpdateOrderSettleState(item.Value[i * 2], OrderSettleState.PreSettle);
                        Orders.UpdateOrderSettleState(item.Value[i * 2 + 1], OrderSettleState.PreSettle);
                    }
                }
            }
        }

        /// <summary>
        /// 汇购优选代理结算（收货7天后结算）
        /// </summary>
        /// <param name="orderList"></param>
        /// <param name="registerTime"></param>
        /// <param name="agentPackStoreId"></param>
        /// <param name="agentStoreId"></param>
        public static void AgentWeekSettle(List<OrderInfo> orderList, string receivingTime, int agentPackStoreId, int agentStoreId,
            int agent298Pid, int specialAgentUid)
        {
            if (orderList == null || orderList.Count == 0) return;

            string sqlStr = string.Empty;
            string remark = string.Empty;
            decimal totalPackAmount = 0m;//1980元汇购优选体验包月销售总额
            PartUserInfo parentUser = null;
            List<OrderInfo> agentOrderList = null;
            Dictionary<int, decimal> bigAreaScore = new Dictionary<int, decimal>();

            foreach (OrderInfo item in orderList)
            {
                PartUserInfo orderUser = Users.GetPartUserById(item.Uid);
                if (orderUser == null || orderUser.Uid == 0 || orderUser.Pid <= 0) continue;

                decimal origOrderUserDiscount = GetAgentDiscount(orderUser, specialAgentUid);
                List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(item.Oid);
                //总共可用于结算的PV和海米
                //decimal totalSettleHaiMiValue = orderProductList.Sum(x => x.ProductHaiMi * x.BuyCount);
                //decimal totalSettlePVValue = orderProductList.Sum(x => x.ProductPV * x.BuyCount);

                if (item.StoreId == agentPackStoreId || item.StoreId == agentStoreId)
                {
                    if (item.StoreId == agentPackStoreId)
                    {
                        totalPackAmount += item.SurplusMoney;
                    }
                    else
                    {
                        //返利润差额给直接推荐人
                        if (orderProductList == null || orderProductList.Count == 0) continue;

                        decimal agentProfit = 0m;
                        decimal totalAmount = 0m;
                        decimal discountDiff = 0m;
                        decimal parentDiscount = 0m;
                        decimal orderUserDiscount = 0m;
                        foreach (OrderProductInfo op in orderProductList)
                        {
                            totalAmount = 0m;
                            //orderUserDiscount = origOrderUserDiscount;
                            //if (op.ShopPrice == op.DiscountPrice)
                            //{
                            //    orderUserDiscount = 1m;
                            //}
                            //else if (op.Pid == agent298Pid && orderUserDiscount == 0m)
                            //{
                            //    if (op.BuyCount % 2 == 0)
                            //    {
                            //        orderUserDiscount = 0.792m;
                            //    }
                            //    else
                            //    {
                            //        orderUserDiscount = 1m;
                            //    }
                            //}
                            orderUserDiscount = op.DiscountPrice / op.ShopPrice;

                            if (op.FromCompanyAmount > 0)
                            {
                                totalAmount = op.FromCompanyAmount / orderUserDiscount;
                            }

                            if (op.FromParentId4 > 0 && orderUserDiscount >= 0.42m)
                            {
                                discountDiff = 0.05m;
                                parentDiscount = 0.42m;
                                //高卫那个大区按照4折进行结算
                                if (op.FromParentId4 == specialAgentUid)
                                {
                                    discountDiff = 0.07m;
                                    parentDiscount = 0.4m;
                                }

                                if (op.FromParentAmount4 > 0)
                                {
                                    totalAmount = totalAmount + (op.FromParentAmount4 / parentDiscount);
                                }
                                if (op.FromParentId3 > 0)
                                {
                                    agentProfit = totalAmount * discountDiff;
                                }
                                else
                                {
                                    agentProfit = totalAmount * (orderUserDiscount - parentDiscount);
                                }
                                remark = string.Format("返还产品利润差额，购物会员：{0}，订单号：{1}，产品名称：{2}，返回金额：{3}", orderUser.UserName,
                                    item.OSN, op.Name, agentProfit.ToString("f2"));
                                UpdateAgentAccount(op.FromParentId4, (int)AccountType.佣金账户, agentProfit, 0m, item.OSN, remark);
                            }

                            if (op.FromParentId3 > 0 && orderUserDiscount > 0.47m)
                            {
                                if (op.FromParentAmount3 > 0)
                                {
                                    totalAmount = totalAmount + (op.FromParentAmount3 / 0.47m);
                                }
                                if (op.FromParentId2 > 0)
                                {
                                    agentProfit = totalAmount * 0.08m;
                                }
                                else
                                {
                                    agentProfit = totalAmount * (orderUserDiscount - 0.47m);
                                }
                                remark = string.Format("返还产品利润差额，购物会员：{0}，订单号：{1}，产品名称：{2}，返回金额：{3}", orderUser.UserName,
                                    item.OSN, op.Name, agentProfit.ToString("f2"));
                                UpdateAgentAccount(op.FromParentId3, (int)AccountType.佣金账户, agentProfit, 0m, item.OSN, remark);
                            }

                            if (op.FromParentId2 > 0 && orderUserDiscount > 0.55m)
                            {
                                if (op.FromParentAmount2 > 0)
                                {
                                    totalAmount = totalAmount + (op.FromParentAmount2 / 0.55m);
                                }
                                if (op.FromParentId1 > 0)
                                {
                                    agentProfit = totalAmount * 0.1m;
                                }
                                else
                                {
                                    agentProfit = totalAmount * (orderUserDiscount - 0.55m);
                                }
                                remark = string.Format("返还产品利润差额，购物会员：{0}，订单号：{1}，产品名称：{2}，返回金额：{3}", orderUser.UserName,
                                    item.OSN, op.Name, agentProfit.ToString("f2"));
                                UpdateAgentAccount(op.FromParentId2, (int)AccountType.佣金账户, agentProfit, 0m, item.OSN, remark);
                            }

                            if (op.FromParentId1 > 0 && orderUserDiscount > 0.65m)
                            {
                                if (op.FromParentAmount1 > 0)
                                {
                                    totalAmount = totalAmount + (op.FromParentAmount1 / 0.65m);
                                }
                                agentProfit = totalAmount * (orderUserDiscount - 0.65m);
                                remark = string.Format("返还产品利润差额，购物会员：{0}，订单号：{1}，产品名称：{2}，返回金额：{3}", orderUser.UserName,
                                    item.OSN, op.Name, agentProfit.ToString("f2"));
                                UpdateAgentAccount(op.FromParentId1, (int)AccountType.佣金账户, agentProfit, 0m, item.OSN, remark);
                            }
                        }
                    }

                    //新奖金制度
                    parentUser = GetParentUser(orderUser);
                    if (parentUser != null)
                    {
                        //1、直接推荐大区，奖励付款金额的5%。
                        //2、直接推荐VIP经销商，奖励付款金额的5%。
                        bool isAgentScore = true;
                        if (item.SurplusMoney >= 19000m)
                        {
                            sqlStr = string.Format("orderstate={0} and receivingtime<'{1}' and storeid={2} and mallsource=0 and uid={3} and surplusmoney>=2980",
                                (int)OrderState.Completed, receivingTime, agentStoreId, item.Uid);
                            agentOrderList = Orders.GetOrderListByWhere(sqlStr);
                            if (agentOrderList != null && agentOrderList.Count > 0 && agentOrderList.Min(o => o.Oid) == item.Oid)
                            {
                                isAgentScore = false;
                                remark = string.Format("直接推荐大区或者VIP经销商，奖励付款金额的5%，订单号为：{0}，订单金额为：{1}", item.OSN, item.SurplusMoney.ToString("f2"));
                                UpdateAgentAccount(parentUser.Uid, (int)AccountType.佣金账户, (item.SurplusMoney * 0.05m), 0m, item.OSN, remark);
                            }
                        }

                        if (isAgentScore)
                        {
                            if (origOrderUserDiscount == 0m) origOrderUserDiscount = 1m;
                            while (parentUser != null)
                            {
                                if (parentUser.AgentType == 4)
                                {
                                    if (!bigAreaScore.ContainsKey(parentUser.Uid))
                                    {
                                        bigAreaScore.Add(parentUser.Uid, (item.SurplusMoney / origOrderUserDiscount));
                                    }
                                    else
                                    {
                                        bigAreaScore[parentUser.Uid] = bigAreaScore[parentUser.Uid] + (item.SurplusMoney / origOrderUserDiscount);
                                    }
                                    break;
                                }
                                parentUser = GetParentUser(parentUser);
                            }
                        }
                    }

                    //更新订单月结状态
                    Orders.UpdateOrderMonthSettled(item.Oid);

                    MallAdminLogs.CreateMallAdminLog(2, "system", 2, "系统管理员", "127.0.0.1", "汇购优选代理订单月结", "月结订单,订单ID为:" + item.Oid);
                    LogHelper.WriteOperateLog("AutoSettleAccount", "订单自动结算服务", "订单号：" + item.Oid + "|结算时间：" + DateTime.Now);
                }
            }

            //新奖金制度
            //大区推大区五代返利：大区拿直推大区未来累计业绩4%的返利，拿二代大区未来累计业绩2%的返利，拿三代大区到六代大区未来累计业绩1%的返利。
            //大区之间业绩不累计。以上返利均按大区折扣价计算。
            int depth = 0;
            int currBigAreaPid = 0;
            decimal awardRate = 0m;
            parentUser = null;
            PartUserInfo currUser = null;
            PartUserInfo tempParentUser = null;
            foreach (var item in bigAreaScore)
            {
                depth = 1;
                currUser = Users.GetPartUserById(item.Key);
                parentUser = GetParentUser(currUser);
                currBigAreaPid = parentUser.Uid;

                while (parentUser != null && depth <= 6)
                {
                    if (depth == 1)
                        awardRate = 0.04m;
                    else if (depth == 2)
                        awardRate = 0.02m;
                    else
                        awardRate = 0.01m;

                    if (parentUser.AgentType == 4)
                    {
                        if (currBigAreaPid == parentUser.Uid)
                        {
                            remark = string.Format("大区推大区五代返利，第{0}代返利，总业绩为：{1}", depth, (item.Value * 0.42m).ToString("f2"));
                            UpdateAgentAccount(parentUser.Uid, (int)AccountType.佣金账户, (item.Value * 0.42m * awardRate), 0m, "", remark);
                        }

                        depth = depth + 1;
                        currBigAreaPid = 0;
                        tempParentUser = null;
                        tempParentUser = GetParentUser(parentUser);
                        if (tempParentUser != null)
                        {
                            currBigAreaPid = tempParentUser.Uid;
                        }
                    }

                    parentUser = GetParentUser(parentUser);
                }
            }


            ////大区、VIP经销商及星级经销商每月均分全球事业伙伴（1980元）总额的1%、5%及2%
            //if (totalPackAmount > 0)
            //{
            //    decimal avgMoney = 0m;

            //    //大区
            //    string sqlStr = string.Format("a.isdirsaleuser=1 and a.dirsaleuid>0 and a.agenttype=4 and b.registertime<='{0}'", registerTime);
            //    List<UserInfo> userList = Users.GetUserInfoByStrWhere(sqlStr);
            //    if (userList != null && userList.Count > 0)
            //    {
            //        //每个人能分得
            //        avgMoney = (totalPackAmount * 0.01m) / userList.Count;
            //        remark = string.Format("大区每月均分全球事业伙伴销售总额的1%，每份为：{0}", avgMoney.ToString("f2"));
            //        foreach (UserInfo user in userList)
            //        {
            //            //AccountUtils.UpdateAccountForDir(user.DirSaleUid, 3, avgMoney, 0m, string.Empty, remark);
            //            UpdateAgentAccount(user.Uid, (int)AccountType.佣金账户, avgMoney, 0m, string.Empty, remark);
            //        }
            //    }

            //    //VIP经销商
            //    sqlStr = string.Format("a.isdirsaleuser=1 and a.dirsaleuid>0 and a.agenttype=3 and isnewds=1 and b.registertime<='{0}'", registerTime);
            //    userList = Users.GetUserInfoByStrWhere(sqlStr);
            //    if (userList != null && userList.Count > 0)
            //    {
            //        //每个人能分得
            //        avgMoney = (totalPackAmount * 0.05m) / userList.Count;
            //        remark = string.Format("VIP经销商每月均分全球事业伙伴销售总额的5%，每份为：{0}", avgMoney.ToString("f2"));
            //        foreach (UserInfo user in userList)
            //        {
            //            //AccountUtils.UpdateAccountForDir(user.DirSaleUid, 3, avgMoney, 0m, string.Empty, remark);
            //            UpdateAgentAccount(user.Uid, (int)AccountType.佣金账户, avgMoney, 0m, string.Empty, remark);
            //        }
            //    }

            //    //星级经销商
            //    sqlStr = string.Format("a.isdirsaleuser=1 and a.dirsaleuid>0 and a.agenttype=2 and isnewds=1 and b.registertime<='{0}'", registerTime);
            //    userList = Users.GetUserInfoByStrWhere(sqlStr);
            //    if (userList != null && userList.Count > 0)
            //    {
            //        //每个人能分得
            //        avgMoney = (totalPackAmount * 0.02m) / userList.Count;
            //        remark = string.Format("星级经销商每月均分全球事业伙伴销售总额的2%，每份为：{0}", avgMoney.ToString("f2"));
            //        foreach (UserInfo user in userList)
            //        {
            //            //AccountUtils.UpdateAccountForDir(user.DirSaleUid, 3, avgMoney, 0m, string.Empty, remark);
            //            UpdateAgentAccount(user.Uid, (int)AccountType.佣金账户, avgMoney, 0m, string.Empty, remark);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 返回订单预结算信息
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public static OrderSettleInfo GetOrderSettlePreview(int oid)
        {
            OrderSettleInfo orderSettleInfo = new OrderSettleInfo();

            OrderInfo orderInfo = GetOrderByOid(oid);
            if (orderInfo == null || orderInfo.StoreId.ToString() != WebHelper.GetConfigSettings("AgentStoreId") ||
                orderInfo.OrderState < (int)OrderState.Confirmed || orderInfo.OrderState > (int)OrderState.Completed)
            {
                return orderSettleInfo;
            }
            PartUserInfo orderUser = Users.GetPartUserById(orderInfo.Uid);
            if (orderUser == null || orderUser.Uid == 0 || orderUser.Pid <= 0 || orderUser.Ptype != 2)
            {
                return orderSettleInfo;
            }
            List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(oid);
            if (orderProductList == null || orderProductList.Count == 0)
            {
                return orderSettleInfo;
            }

            orderSettleInfo.Oid = oid;
            PartUserInfo parentUser = GetParentUser(orderUser);
            if (parentUser != null)
            {
                orderSettleInfo.OrderUserParentId = parentUser.Uid;
                orderSettleInfo.OrderUserParentName = parentUser.UserName;
                orderSettleInfo.OrderUserParentMobile = parentUser.Mobile;
                //1、直接推荐大区，奖励付款金额的5%。
                //2、直接推荐VIP经销商，奖励付款金额的5%。
                if (orderInfo.SurplusMoney >= 19000m)
                {
                    string sqlStr = string.Format("orderstate>={0} and orderstate<={1} and storeid={2} and mallsource=0 and uid={3} and surplusmoney>=2980",
                        (int)OrderState.Confirmed, (int)OrderState.Completed, WebHelper.GetConfigSettings("AgentStoreId"), orderInfo.Uid);
                    List<OrderInfo> agentOrderList = Orders.GetOrderListByWhere(sqlStr);
                    if (agentOrderList != null && agentOrderList.Count > 0 && agentOrderList.Min(o => o.Oid) == oid)
                    {
                        orderSettleInfo.RecommandAward = orderInfo.SurplusMoney * 0.05m;
                    }
                }
            }

            parentUser = null;
            decimal agentProfit = 0m;
            decimal totalAmount = 0m;
            decimal discountDiff = 0m;
            decimal parentDiscount = 0m;
            decimal orderUserDiscount = 0m;
            int specialAgentUid = 0;
            int.TryParse(WebHelper.GetConfigSettings("SpecialAgentUid"), out specialAgentUid);

            foreach (OrderProductInfo op in orderProductList)
            {
                if (op.FromParentId1 == 0 && op.FromParentId2 == 0 && op.FromParentId3 == 0 && op.FromParentId4 == 0) continue;

                OrderSettleDetail settleDetail = new OrderSettleDetail();
                //计算购物成本
                //FromParentId1
                settleDetail.FromParent1Id = op.FromParentId1;
                if (op.FromParentId1 > 0)
                {
                    parentUser = Users.GetPartUserById(op.FromParentId1);
                    if (parentUser != null)
                    {
                        settleDetail.FromParent1Name = parentUser.UserName;
                        settleDetail.FromParent1Mobile = parentUser.Mobile;
                    }
                }
                settleDetail.Parent1Principal = op.FromParentAmount1;
                //FromParentId2
                settleDetail.FromParent2Id = op.FromParentId2;
                if (op.FromParentId2 > 0)
                {
                    parentUser = Users.GetPartUserById(op.FromParentId2);
                    if (parentUser != null)
                    {
                        settleDetail.FromParent2Name = parentUser.UserName;
                        settleDetail.FromParent2Mobile = parentUser.Mobile;
                    }
                }
                settleDetail.Parent2Principal = op.FromParentAmount2;
                //FromParentId3
                settleDetail.FromParent3Id = op.FromParentId3;
                if (op.FromParentId3 > 0)
                {
                    parentUser = Users.GetPartUserById(op.FromParentId3);
                    if (parentUser != null)
                    {
                        settleDetail.FromParent3Name = parentUser.UserName;
                        settleDetail.FromParent3Mobile = parentUser.Mobile;
                    }
                }
                settleDetail.Parent3Principal = op.FromParentAmount3;
                //FromParentId4
                settleDetail.FromParent4Id = op.FromParentId4;
                if (op.FromParentId4 > 0)
                {
                    parentUser = Users.GetPartUserById(op.FromParentId4);
                    if (parentUser != null)
                    {
                        settleDetail.FromParent4Name = parentUser.UserName;
                        settleDetail.FromParent4Mobile = parentUser.Mobile;
                    }
                }
                settleDetail.Parent4Principal = op.FromParentAmount4;

                //计算返还利润
                totalAmount = 0m;
                orderUserDiscount = op.DiscountPrice / op.ShopPrice;
                if (op.FromCompanyAmount > 0)
                {
                    totalAmount = op.FromCompanyAmount / orderUserDiscount;
                }
                if (op.FromParentId4 > 0 && orderUserDiscount >= 0.42m)
                {
                    discountDiff = 0.05m;
                    parentDiscount = 0.42m;
                    //高卫那个大区按照4折进行结算
                    if (op.FromParentId4 == specialAgentUid)
                    {
                        discountDiff = 0.07m;
                        parentDiscount = 0.4m;
                    }

                    if (op.FromParentAmount4 > 0)
                    {
                        totalAmount = totalAmount + (op.FromParentAmount4 / parentDiscount);
                    }
                    if (op.FromParentId3 > 0)
                    {
                        agentProfit = totalAmount * discountDiff;
                    }
                    else
                    {
                        agentProfit = totalAmount * (orderUserDiscount - parentDiscount);
                    }
                    settleDetail.Parent4Profit = agentProfit;
                }

                if (op.FromParentId3 > 0 && orderUserDiscount > 0.47m)
                {
                    if (op.FromParentAmount3 > 0)
                    {
                        totalAmount = totalAmount + (op.FromParentAmount3 / 0.47m);
                    }
                    if (op.FromParentId2 > 0)
                    {
                        agentProfit = totalAmount * 0.08m;
                    }
                    else
                    {
                        agentProfit = totalAmount * (orderUserDiscount - 0.47m);
                    }
                    settleDetail.Parent3Profit = agentProfit;
                }

                if (op.FromParentId2 > 0 && orderUserDiscount > 0.55m)
                {
                    if (op.FromParentAmount2 > 0)
                    {
                        totalAmount = totalAmount + (op.FromParentAmount2 / 0.55m);
                    }
                    if (op.FromParentId1 > 0)
                    {
                        agentProfit = totalAmount * 0.1m;
                    }
                    else
                    {
                        agentProfit = totalAmount * (orderUserDiscount - 0.55m);
                    }
                    settleDetail.Parent2Profit = agentProfit;
                }

                if (op.FromParentId1 > 0 && orderUserDiscount > 0.65m)
                {
                    if (op.FromParentAmount1 > 0)
                    {
                        totalAmount = totalAmount + (op.FromParentAmount1 / 0.65m);
                    }
                    agentProfit = totalAmount * (orderUserDiscount - 0.65m);
                    settleDetail.Parent1Profit = agentProfit;
                }

                orderSettleInfo.SettleDetail.Add(settleDetail);
            }

            return orderSettleInfo;
        }

        /// <summary>
        /// 获取推荐人
        /// </summary>
        /// <param name="childUser"></param>
        /// <returns></returns>
        private static PartUserInfo GetParentUser(PartUserInfo childUser)
        {
            PartUserInfo parentUser = null;
            if (childUser.Ptype == 1)
            {
                parentUser = Users.GetPartUserById(childUser.Pid);
            }
            else if (childUser.Ptype == 2)
            {
                parentUser = Users.GetPartUserInfoByDirSaleUid(childUser.Pid);
            }

            return parentUser;
        }

        /// <summary>
        /// 每推荐2个汇购优选代理得1980元现金奖励
        /// </summary>
        public static void GenAgentPackAward(Dictionary<int, List<int>> userPackOrders)
        {
            if (userPackOrders.Count <= 0) return;

            PartUserInfo parentUser = null;
            string remark = string.Empty;
            foreach (var item in userPackOrders)
            {
                if (item.Value.Count >= 2)
                {
                    parentUser = Users.GetPartUserById(item.Key);
                    if (parentUser == null || parentUser.Uid == 0) continue;

                    int oCount = item.Value.Count / 2;
                    for (int i = 0; i < oCount; i++)
                    {
                        //先将冻结金额转出
                        Account.AgentPackSettleTransfer(item.Value[i * 2]);
                        Account.AgentPackSettleTransfer(item.Value[i * 2 + 1]);
                        //再添加正式金额
                        remark = string.Format("推荐两个汇购优选体验包公司奖励1980元，OrderId={0}和OrderId={1}", item.Value[i * 2], item.Value[i * 2 + 1]);
                        UpdateAgentAccount(parentUser.Uid, (int)AccountType.佣金账户, 1980m, 0m, string.Empty, remark);
                        Orders.UpdateOrderSettleState(item.Value[i * 2], OrderSettleState.Settled);
                        Orders.UpdateOrderSettleState(item.Value[i * 2 + 1], OrderSettleState.Settled);
                    }
                }
            }
        }

        /// <summary>
        /// vip代理会员升级到大区
        /// </summary>
        /// <param name="vipUserIds"></param>
        public static void UpgradeVip(List<int> vipUserIds, int agentStoreId, string receivedTime)
        {
            foreach (int userId in vipUserIds)
            {
                string commandText = string.Format("select isnull(sum(surplusmoney),0) from hlh_orders where [uid]={0} and orderstate=140 and receivingtime<'{1}' and storeid={2}", userId, receivedTime, agentStoreId);
                decimal userScore = TypeHelper.ObjectToDecimal(RDBSHelper.ExecuteScalar(CommandType.Text, commandText));

                if (userScore >= 800000)
                {
                    UpdateUserAgentType(userId);
                }
            }
        }

        /// <summary>
        /// 修改会员代理级别
        /// </summary>
        /// <param name="oid"></param>
        private static void UpdateUserAgentType(int uid)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid",uid)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [agenttype]=4 WHERE [uid]=@uid",
                                               RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 根据代理级别返回代理折扣
        /// </summary>
        /// <param name="agentType"></param>
        /// <returns></returns>
        private static decimal GetAgentDiscount(PartUserInfo user, int specialAgentUid)
        {
            decimal agentDiscount = 0m;
            int agentType = user.AgentType;
            if (user.Ds2AgentRank > agentType)
            {
                agentType = user.Ds2AgentRank;
            }

            switch (agentType)
            {
                case 1:
                    agentDiscount = 0.65m;
                    break;
                case 2:
                    agentDiscount = 0.55m;
                    break;
                case 3:
                    agentDiscount = 0.47m;
                    break;
                case 4:
                    agentDiscount = 0.42m;
                    if (user.Uid == specialAgentUid)
                    {
                        agentDiscount = 0.4m;
                    }
                    break;
                case 5:
                    agentDiscount = 0.4m;
                    break;
                default:
                    agentDiscount = 0m;
                    break;
            }

            return agentDiscount;
        }

        /// <summary>
        /// Update Order State
        /// </summary>
        /// <param name="orderinfo"></param>
        private static void UpdateSettleState(OrderInfo orderinfo)
        {
            if (orderinfo.OrderState >= (int)OrderState.Confirmed && orderinfo.OrderState <= (int)OrderState.Sended)
            {
                UpdateOrderSettleState(orderinfo.Oid, OrderSettleState.PreSettle);
            }
            else if (orderinfo.OrderState == (int)OrderState.Cancelled && orderinfo.SettleState == (int)OrderSettleState.PreSettle)
            {
                UpdateOrderSettleState(orderinfo.Oid, OrderSettleState.PreSettleCancelled);
            }
            else if (orderinfo.OrderState == (int)OrderState.Returned && orderinfo.SettleState == (int)OrderSettleState.PreSettle)
            {
                UpdateOrderSettleState(orderinfo.Oid, OrderSettleState.PreSettleCancelled);
            }
            else if (orderinfo.OrderState == (int)OrderState.Completed)
            {
                UpdateOrderSettleState(orderinfo.Oid, OrderSettleState.Settled);
            }
        }

        /// <summary>
        /// 修改三级分配账户金额
        /// </summary>
        /// <param name="smodel"></param>
        /// <param name="orderCode"></param>
        private static void UpdateAccount(SettleModel smodel, string orderCode)
        {
            //updateacctount
            Account.UpdateAccountBySettleAccount(smodel);
            //insert accountdetail
            Account.CreateAccountDetail(new AccountDetailInfo()
            {
                AccountId = smodel.accountid,
                UserId = smodel.uid,
                CreateTime = DateTime.Now,
                DetailType = smodel.accountin > 0 ? (int)DetailType.三级分销结算收入 : (int)DetailType.三级分销结算支出,
                InAmount = smodel.accountin,
                OutAmount = smodel.accountout,
                OrderCode = orderCode,
                AdminUid = 2,//system
                Status = 1,
                DetailDes = "结算订单:" + orderCode + ",支出/收入金额:" + (smodel.accountin > 0 ? smodel.accountin.ToString("f2") : smodel.accountout.ToString("f2"))
            });
        }

        /// <summary>
        /// 修改汇购优选代理账户金额
        /// </summary>
        /// <param name="smodel"></param>
        /// <param name="orderCode"></param>
        private static void UpdateAgentAccount(int uid, int accountId, decimal inAmount, decimal outAmount, string orderCode, string remark)
        {
            SettleModel smodel = new SettleModel();
            smodel.uid = uid;
            smodel.accountid = accountId;
            smodel.accountin = inAmount;
            smodel.accountout = outAmount;
            //updateacctount
            Account.UpdateAccountBySettleAccount(smodel);

            //insert accountdetail
            Account.CreateAccountDetail(new AccountDetailInfo()
            {
                AccountId = smodel.accountid,
                UserId = smodel.uid,
                CreateTime = DateTime.Now,
                DetailType = smodel.accountin > 0 ? (int)DetailType.微商代理结算收入 : (int)DetailType.微商代理结算支出,
                InAmount = smodel.accountin,
                OutAmount = smodel.accountout,
                OrderCode = orderCode,
                AdminUid = 2,//system
                Status = 1,
                DetailDes = remark
            });
        }

        /// <summary>
        /// 修改汇购优选代理账户冻结金额
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateAccountLockBanlance(int userId, int opUserId, int accountId, int detailType, decimal inMoney, decimal outMoney, string remark, string orderCode)
        {
            Account.UpdateAccountLockBanlance(userId, opUserId, accountId, detailType, inMoney, outMoney, remark, orderCode);
        }

        #endregion

        #region 直销有关的订单操作
        /// <summary>
        /// 是否满足重销条件 是直销会员 当月开始至今的有效订单 并且订购的产品pv>=600 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static decimal GetListForChongXiao(int uid)
        {
            DateTime now = DateTime.Now;
            DateTime d1 = new DateTime(now.Year, now.Month, 1);

            //SELECT SUM(b.pv) FROM dbo.hlh_orderproducts a LEFT JOIN dbo.hlh_products b ON a.pid=b.pid  WHERE  oid IN ( SELECT oid FROM dbo.hlh_orders WHERE addtime>='2016-05-01 00:00:00.000' AND addtime<=GETDATE() AND orderstate>=70 AND orderstate<=140 AND uid=1)  
            //SELECT SUM(a.productpv*a.buycount) FROM dbo.hlh_orderproducts a   WHERE  oid IN ( SELECT oid FROM dbo.hlh_orders WHERE addtime>='2016-05-01 00:00:00.000' AND addtime<=GETDATE() AND orderstate>=70 AND orderstate<=140 AND uid=1)
            string commandText = string.Format("SELECT SUM(a.productpv * a.buycount) FROM dbo.hlh_orderproducts a  WHERE  oid IN ( SELECT oid FROM dbo.hlh_orders WHERE addtime>='{0}' AND addtime<='{1}' AND orderstate>=70 AND orderstate<=140 AND uid={2} AND cashdiscount<=0)", d1.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), uid);

            return TypeHelper.ObjectToDecimal(RDBSHelper.ExecuteScalar(CommandType.Text, commandText));
        }

        /// <summary>
        /// 设置重销分期
        /// </summary>
        public static bool SetFenQi(int oid, int PhasedType)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid)
                                   };
            if (PhasedType == 2)
            {
                string commandText = string.Format("UPDATE [{0}orders] SET [IsPhased]=1 WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);
                RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
            }
            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            PartUserInfo partUserInfo = new PartUserInfo();
            if (orderInfo != null)
            {
                partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            }
            try
            {
                bool Resultstate = false;
                //汇购卡支付订单不推送 汇购卡订单不计算pv
                List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
                if (!orderProductList.Exists(x => StringHelper.StrContainsForNum(WebHelper.GetConfigSettings("CoffeeQuanPid"), x.Pid.ToString())))//存在汇购卡，该订单不推送
                {
                    //不使用汇购卡支付，并且是直销会员，订单没推送过，即来源不等于31,不属于尚睿淳店铺
                    if (orderInfo.CashDiscount <= 0 && partUserInfo.IsDirSaleUser && orderInfo.OrderSource != (int)OrderSource.启德系统 && orderInfo.StoreId.ToString() != WebHelper.GetConfigSettings("SRCStoreId"))
                    {
                        Resultstate = OrderUtils.CreateQDOrder(orderInfo, partUserInfo, PhasedType);
                    }
                }
                return Resultstate;
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("推送设置重销分期订单错误", "推送已支付订单", "错误信息为：会员ID：" + partUserInfo.Uid + "|订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|失败原因：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 设置直销订单推送标识 orderSource 标识为31-启德订单
        /// </summary>
        public static void SetSendSusscess(int oid)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid)
                                   };
            string commandText = string.Format("UPDATE [{0}orders] SET [ordersource]=31 WHERE [oid]=@oid",
                                               RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);


        }

        #endregion

        /// <summary>
        /// 更新微商订单号和来源
        /// </summary>
        public static bool UpdateWSOrder(int oid, int ordersource, string orderid)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@ordersource",ordersource)
                                   };
            string commandText = string.Format("UPDATE [{0}orders] SET [ordersource]=@ordersource,[extorderid]={1} WHERE [oid]=@oid",
                                               RDBSHelper.RDBSTablePre, orderid);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;


        }
        /// <summary>
        /// 更新赠品订单id到订单表，便于后续更新赠品订单状态
        /// </summary>
        public static bool UpdateOrderGift(int oid, string extorderid)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@extorderid",extorderid)
                                   };
            string commandText = string.Format("UPDATE [{0}orders] SET [extorderid]={1} WHERE [oid]=@oid",
                                               RDBSHelper.RDBSTablePre, extorderid);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;


        }
        /// <summary>
        /// 更新汇购卡支付订单海米pv清零
        /// </summary>
        public static bool UpdateCashOrderPVAndHMTo0(int oid)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid)
                                   };
            string commandText = string.Format("UPDATE [{0}orderproducts] SET [producthaimi]=0,[productpv]=0 WHERE [oid]=@oid",
                                               RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;


        }
        /// <summary>
        /// 更新旅游关联订单
        /// </summary>
        public static bool UpdateTripOrder(string oids, int extorderid)
        {
            if (!string.IsNullOrEmpty(oids))
            {
                SqlParameter[] parms =  {
                                       new SqlParameter("@extorderid",extorderid)
                                   };
                string commandText = string.Format("UPDATE [{0}orders] SET [extorderid]=@extorderid WHERE [oid] IN ({1})",
                                                   RDBSHelper.RDBSTablePre, oids);
                return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
            }
            return false;
        }
        /// <summary>
        /// 更新订单主订单id信息
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="mainoid">主订单id</param>
        /// <param name="suboid">子订单id</param>
        /// <param name="type">类型 1更新主订单id 2更新子订单id</param>
        /// <returns></returns>
        public static bool UpdateMainOid(int oid, int mainoid, int suboid, int type)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@mainoid",mainoid),
                                       new SqlParameter("@suboid",suboid),
                                    };
            string commandText = string.Empty;
            if (type == 1)
                commandText = string.Format("UPDATE [{0}orders] SET [mainoid]=@mainoid WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);
            if (type == 2)
                commandText = string.Format("UPDATE [{0}orders] SET [suboid]=@suboid WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        /// <summary>
        /// 更新订单产品的代理数量来源信息
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public static bool UpdateOrderProductAgentCount(int recordid, int fromparent, int fromcompany, int fromParentId, int fromParentId1, decimal fromParentAmount1, int fromParentId2, decimal fromParentAmount2, int fromParentId3, decimal fromParentAmount3, int fromParentId4, decimal fromParentAmount4, decimal fromCompanyAmount)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@fromparent",fromparent),
                                       new SqlParameter("@fromcompany",fromcompany),
                                       new SqlParameter("@fromparentid",fromParentId),
                                       new SqlParameter("@fromparentid1",fromParentId1),
                                       new SqlParameter("@fromparentamount1",fromParentAmount1),
                                       new SqlParameter("@fromparentid2",fromParentId2),
                                       new SqlParameter("@fromparentamount2",fromParentAmount2),
                                       new SqlParameter("@fromparentid3",fromParentId3),
                                       new SqlParameter("@fromparentamount3",fromParentAmount3),
                                       new SqlParameter("@fromparentid4",fromParentId4),
                                       new SqlParameter("@fromparentamount4",fromParentAmount4),
                                       new SqlParameter("@fromcompanyamount",fromCompanyAmount),
                                       new SqlParameter("@recordid",recordid)

                                   };
            string commandText = string.Format("UPDATE [{0}orderproducts] SET [fromparent]=@fromparent,[fromcompany]=@fromcompany,[fromparentid]=@fromparentid,[fromparentid1]=@fromparentid1,[fromparentamount1]=@fromparentamount1,[fromparentid2]=@fromparentid2,[fromparentamount2]=@fromparentamount2,[fromparentid3]=@fromparentid3,[fromparentamount3]=@fromparentamount3,[fromparentid4]=@fromparentid4,[fromparentamount4]=@fromparentamount4,[fromcompanyamount]=@fromcompanyamount WHERE [recordid]=@recordid",
                                               RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        /// <summary>
        /// 查找代理体验包销售总额
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public static decimal GetAgentSuitSum()
        {
            string commandText = string.Format("SELECT SUM(orderamount) FROM dbo.hlh_orders WHERE storeid={0} AND orderstate>=70 AND orderstate<=140 ", WebHelper.GetConfigSettingsInt("AgentSuitStoreId"));
            return TypeHelper.ObjectToDecimal(RDBSHelper.ExecuteScalar(CommandType.Text, commandText));
        }

        /// <summary>
        /// 更新订单金额（后台插入订单用）
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="mainoid">主订单id</param>
        /// <param name="suboid">子订单id</param>

        /// <returns></returns>
        public static bool UpdateOrderForAgentAdmin(int oid, decimal amount)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@productamount",amount),
                                       new SqlParameter("@orderamount",amount),
                                       new SqlParameter("@surplusmoney",amount)
                                    };
            string commandText = string.Format("UPDATE [{0}orders] SET [productamount]=@productamount,[orderamount]=@orderamount,[surplusmoney]=@surplusmoney WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 更新订单发票信息
        /// </summary>
        public static bool UpdateOrderInvoiceMore(int oid, string invoiceMore)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@invoicemore",invoiceMore)
                                    };
            string commandText = string.Format("UPDATE [{0}orders] SET [invoicemore]=@invoicemore WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 增加订单后台操作备注
        /// </summary>
        public static bool UpdateOrderAdminRemark(int oid, string adminRemark)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@adminremark",adminRemark)
                                    };
            string commandText = string.Format("UPDATE [{0}orders] SET [adminremark]=[adminremark]+@adminremark WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 修改订单收货时间
        /// </summary>
        public static bool UpdateOrderAdminReceivTime(int oid, DateTime time)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@receivingtime",time)
                                    };
            string commandText = string.Format("UPDATE [{0}orders] SET [receivingtime]=@receivingtime WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        #region 订单统计

        /// <summary>
        /// 获得订单统计
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="orderActionType">订单操作类型</param>
        /// <returns></returns>
        public static DataTable GetOrderCountStat(DateTime startTime, DateTime endTime)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@starttime",startTime),
                                       new SqlParameter("@endtime",endTime)
                                    };
            string commandText = string.Format("SELECT convert(char(10),addtime,120) time ,SUM(1) count FROM hlh_orders WHERE orderstate>=70 AND orderstate<=140 AND addtime>= @starttime AND addtime<@endtime GROUP BY convert(char(10),addtime,120)");
            // string commandText = string.Format("SELECT [oid] FROM [{0}orderactions] WHERE [actiontype]=@orderactiontype AND [actiontime]>=@starttime AND [actiontime]<@endtime", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText, parms).Tables[0];
        }
        /// <summary>
        /// 获得订单统计
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="orderActionType">订单操作类型</param>
        /// <returns></returns>
        public static DataTable GetOrderAmountStat(DateTime startTime, DateTime endTime)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@starttime",startTime),
                                       new SqlParameter("@endtime",endTime)
                                    };
            string commandText = string.Format("SELECT convert(char(10),addtime,120) time ,SUM(surplusmoney) amount FROM hlh_orders WHERE orderstate>=70 AND orderstate<=140 AND addtime>= @starttime AND addtime<@endtime GROUP BY convert(char(10),addtime,120)");
            // string commandText = string.Format("SELECT [oid] FROM [{0}orderactions] WHERE [actiontype]=@orderactiontype AND [actiontime]>=@starttime AND [actiontime]<@endtime", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText, parms).Tables[0];
        }
        #endregion


    }
    /// <summary>
    /// 订单金额统计类
    /// </summary>
    public class HomeOrderAmountModel
    {
        public string time { get; set; }
        public decimal amount { get; set; }
    }
    /// <summary>
    /// 订单数统计类
    /// </summary>
    public class HomeOrderStatModel
    {
        public string time { get; set; }
        public int count { get; set; }
    }
    /// <summary>
    /// 结算信息类
    /// </summary>
    public class SettleModel
    {
        public int uid { get; set; }
        public int accountid { get; set; }
        public decimal accountin { get; set; }
        public decimal accountout { get; set; }
    }
}
