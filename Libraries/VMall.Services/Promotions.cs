using System;
using System.Text;
using System.Collections.Generic;

using VMall.Core;
using System.Data.SqlClient;
using System.Data;

namespace VMall.Services
{
    /// <summary>
    /// 促销活动操作管理类
    /// </summary>
    public partial class Promotions
    {
        /// <summary>
        /// 获得单品促销活动
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static SinglePromotionInfo GetSinglePromotionByPidAndTime(int pid, DateTime nowTime)
        {
            return VMall.Data.Promotions.GetSinglePromotionByPidAndTime(pid, nowTime);
        }

        /// <summary>
        /// 获得有效单品秒杀-未过期
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static SinglePromotionInfo GetFlashSaleInfoByPidAndEndTime(int pid, DateTime nowTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_singlepromotions  ");
            strSql.Append(" where pid=@pid AND state=1 AND (( [endtime1]>@nowtime ) OR ( [endtime2]>@nowtime) OR ( [endtime3]>@nowtime)) AND discounttype=3");
            SqlParameter[] parameters = {
					new SqlParameter("@pid", SqlDbType.Int,4),
                    new SqlParameter("@nowtime", SqlDbType.DateTime)
			};
            parameters[0].Value = pid;
            parameters[1].Value = nowTime;

            SinglePromotionInfo model = null;
            IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString(), parameters);
            if (reader.Read())
            {
                model = VMall.Data.Promotions.BuildSinglePromotionFromReader(reader);
            }
            return model;
        }
        /// <summary>
        /// 获得有效单品秒杀-未开始
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static SinglePromotionInfo GetFlashSaleInfoByPidAndStartTime(int pid, DateTime nowTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_singlepromotions  ");
            strSql.Append(" where pid=@pid AND state=1 AND (( [starttime1]>@nowtime ) OR ( [starttime2]>@nowtime) OR ( [starttime3]>@nowtime)) AND discounttype=3");
            SqlParameter[] parameters = {
					new SqlParameter("@pid", SqlDbType.Int,4),
                    new SqlParameter("@nowtime", SqlDbType.DateTime)
			};
            parameters[0].Value = pid;
            parameters[1].Value = nowTime;

            SinglePromotionInfo model = null;
            IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString(), parameters);
            if (reader.Read())
            {
                model = VMall.Data.Promotions.BuildSinglePromotionFromReader(reader);
            }
            return model;
        }
        /// <summary>
        /// 获得有效单品促销-未开始和正在进行中
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static SinglePromotionInfo GetVailiSingleByPidAndendTime(int pid, DateTime nowTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_singlepromotions  ");
            //strSql.Append(" where pid=@pid AND state=1 AND (( [endtime1]>@nowtime ) OR ( [endtime2]>@nowtime) OR ( [endtime3]>@nowtime)) ");
            strSql.Append(" where pid=@pid AND state=1 AND (( [endtime1]>@nowtime AND [starttime1]<=@nowtime ) OR ( [endtime2]>@nowtime AND [starttime2]<=@nowtime ) OR ( [endtime3]>@nowtime AND [starttime3]<=@nowtime )) ");
            SqlParameter[] parameters = {
					new SqlParameter("@pid", SqlDbType.Int,4),
                    new SqlParameter("@nowtime", SqlDbType.DateTime)
			};
            parameters[0].Value = pid;
            parameters[1].Value = nowTime;

            SinglePromotionInfo model = null;
            IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString(), parameters);
            if (reader.Read())
            {
                model = VMall.Data.Promotions.BuildSinglePromotionFromReader(reader);
            }
            return model;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pids"></param>
        /// <param name="nowTime"></param>
        /// <returns></returns>
        public static List<SinglePromotionInfo> GetVailiSingleListByPidsAndendTime(string pids, DateTime nowTime)
        {
            List<SinglePromotionInfo> list = new List<SinglePromotionInfo>();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  * from hlh_singlepromotions  ");
            if (!string.IsNullOrEmpty(pids))
                strSql.Append(" where pid IN (" + pids + ") AND state=1 AND (( [endtime1]>@nowtime AND [starttime1]<=@nowtime ) OR ( [endtime2]>@nowtime AND [starttime2]<=@nowtime ) OR ( [endtime3]>@nowtime AND [starttime3]<=@nowtime )) ");
            SqlParameter[] parameters = {
					//new SqlParameter("@pids", SqlDbType.NChar,500),
                    new SqlParameter("@nowtime", SqlDbType.DateTime)
			};
            //parameters[0].Value = pids;
            parameters[0].Value = nowTime;

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString(), parameters))
            {
                while (reader.Read())
                {
                    SinglePromotionInfo model = VMall.Data.Promotions.BuildSinglePromotionFromReader(reader);
                    list.Add(model);
                }
                reader.Close();
            }
            return list;

        }
        /// <summary>
        /// 获得单品促销
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static List<SinglePromotionInfo> GetSingleByStrWhere(string strWhere)
        {
            List<SinglePromotionInfo> list = new List<SinglePromotionInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_singlepromotions  ");
            if (strWhere.Trim() != "")
                strSql.Append(" where " + strWhere);
            SqlParameter[] parameters = {
					//new SqlParameter("@pids", SqlDbType.NChar,500),
                    new SqlParameter("@nowtime", SqlDbType.DateTime)
			};
            parameters[0].Value = DateTime.Now;
            SinglePromotionInfo model = null;
            IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString(), parameters);
            if (reader.Read())
            {
                model = VMall.Data.Promotions.BuildSinglePromotionFromReader(reader);
                list.Add(model);
            }
            return list;
        }


        /// <summary>
        /// 获得单品促销活动商品的购买数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="pid">商品id</param>
        /// <param name="pmId">单品促销活动id</param>
        /// <returns></returns>
        public static int GetSinglePromotionProductBuyCount(int uid, int pmId)
        {
            return VMall.Data.Promotions.GetSinglePromotionProductBuyCount(uid, pmId);
        }

        /// <summary>
        /// 更新单品促销活动的库存
        /// </summary>
        /// <param name="singlePromotionList">单品促销活动列表</param>
        public static void UpdateSinglePromotionStock(List<SinglePromotionInfo> singlePromotionList)
        {
            VMall.Data.Promotions.UpdateSinglePromotionStock(singlePromotionList);
        }




        /// <summary>
        /// 获得买送促销活动列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static List<BuySendPromotionInfo> GetBuySendPromotionList(int storeId, int pid, DateTime nowTime)
        {
            return VMall.Data.Promotions.GetBuySendPromotionList(storeId, pid, nowTime);
        }

        /// <summary>
        /// 获得买送促销活动
        /// </summary>
        /// <param name="buyCount">购买数量</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="pid">商品id</param>
        /// <param name="buyTime">购买时间</param>
        /// <returns></returns>
        public static BuySendPromotionInfo GetBuySendPromotion(int buyCount, int storeId, int pid, DateTime buyTime)
        {
            BuySendPromotionInfo buySendPromotionInfo = null;
            //获得买送促销活动列表
            List<BuySendPromotionInfo> buySendPromotionList = GetBuySendPromotionList(storeId, pid, buyTime);
            //买送促销活动存在时
            if (buySendPromotionList.Count > 0)
            {
                foreach (BuySendPromotionInfo item in buySendPromotionList)
                {
                    if (item.BuyCount <= buyCount)
                        buySendPromotionInfo = item;
                }
            }
            return buySendPromotionInfo;
        }

        /// <summary>
        /// 买送商品是否已经存在
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <param name="pid">商品id</param>
        public static bool IsExistBuySendProduct(int pmId, int pid)
        {
            return VMall.Data.Promotions.IsExistBuySendProduct(pmId, pid);
        }

        /// <summary>
        /// 获得全部买送促销活动
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static List<BuySendPromotionInfo> GetAllBuySendPromotion(int storeId, DateTime nowTime)
        {
            return VMall.Data.Promotions.GetAllBuySendPromotion(storeId, nowTime);
        }





        /// <summary>
        /// 获得赠品促销活动
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static GiftPromotionInfo GetGiftPromotionByPidAndTime(int pid, DateTime buyTime)
        {
            return VMall.Data.Promotions.GetGiftPromotionByPidAndTime(pid, buyTime);
        }




        /// <summary>
        /// 赠品是否已经存在
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="giftId">赠品id</param>
        public static bool IsExistGift(int pmId, int giftId)
        {
            return VMall.Data.Promotions.IsExistGift(pmId, giftId);
        }

        /// <summary>
        /// 获得扩展赠品列表
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <returns></returns>
        public static List<ExtGiftInfo> GetExtGiftList(int pmId)
        {
            return VMall.Data.Promotions.GetExtGiftList(pmId);
        }




        /// <summary>
        /// 获得套装促销活动
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static SuitPromotionInfo GetSuitPromotionByPmIdAndTime(int pmId, DateTime nowTime)
        {
            if (pmId > 0)
                return VMall.Data.Promotions.GetSuitPromotionByPmIdAndTime(pmId, nowTime);
            return null;
        }

        /// <summary>
        /// 判断用户是否参加过指定套装促销活动
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="pmId">促销活动id</param>
        /// <returns></returns>
        public static bool IsJoinSuitPromotion(int uid, int pmId)
        {
            return VMall.Data.Promotions.IsJoinSuitPromotion(uid, pmId);
        }

        /// <summary>
        /// 获得套装促销活动列表
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static List<SuitPromotionInfo> GetSuitPromotionList(int pid, DateTime nowTime)
        {
            return VMall.Data.Promotions.GetSuitPromotionList(pid, nowTime);
        }

        /// <summary>
        /// 获得套装促销活动列表
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static List<KeyValuePair<SuitPromotionInfo, List<ExtSuitProductInfo>>> GetProductAllSuitPromotion(int pid, DateTime nowTime)
        {
            List<KeyValuePair<SuitPromotionInfo, List<ExtSuitProductInfo>>> result = new List<KeyValuePair<SuitPromotionInfo, List<ExtSuitProductInfo>>>();

            List<SuitPromotionInfo> suitPromotionList = GetSuitPromotionList(pid, nowTime);
            if (suitPromotionList.Count > 0)
            {
                StringBuilder pmIdList = new StringBuilder();
                foreach (SuitPromotionInfo suitPromotionInfo in suitPromotionList)
                {
                    pmIdList.AppendFormat("{0},", suitPromotionInfo.PmId);
                }
                List<ExtSuitProductInfo> allExtSuitProduct = GetAllExtSuitProductList(pmIdList.Remove(pmIdList.Length - 1, 1).ToString());

                foreach (SuitPromotionInfo suitPromotionInfo in suitPromotionList)
                {
                    result.Add(new KeyValuePair<SuitPromotionInfo, List<ExtSuitProductInfo>>(suitPromotionInfo, allExtSuitProduct.FindAll(x => x.PmId == suitPromotionInfo.PmId)));
                }
            }

            return result;
        }

        /// <summary>
        /// 获得所有有效套装促销活动列表
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static List<KeyValuePair<SuitPromotionInfo, List<ExtSuitProductInfo>>> GetAllSuitPromotion(List<SuitPromotionInfo> suitPromotionList)
        {
            List<KeyValuePair<SuitPromotionInfo, List<ExtSuitProductInfo>>> result = new List<KeyValuePair<SuitPromotionInfo, List<ExtSuitProductInfo>>>();
           
            if (suitPromotionList.Count > 0)
            {
                StringBuilder pmIdList = new StringBuilder();
                foreach (SuitPromotionInfo suitPromotionInfo in suitPromotionList)
                {
                    pmIdList.AppendFormat("{0},", suitPromotionInfo.PmId);
                }
                List<ExtSuitProductInfo> allExtSuitProduct = GetAllExtSuitProductList(pmIdList.Remove(pmIdList.Length - 1, 1).ToString());

                foreach (SuitPromotionInfo suitPromotionInfo in suitPromotionList)
                {
                    result.Add(new KeyValuePair<SuitPromotionInfo, List<ExtSuitProductInfo>>(suitPromotionInfo, allExtSuitProduct.FindAll(x => x.PmId == suitPromotionInfo.PmId)));
                }
            }

            return result;
        }


        /// <summary>
        /// 套装商品是否存在
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="pid">商品id</param>
        public static bool IsExistSuitProduct(int pmId, int pid)
        {
            return VMall.Data.Promotions.IsExistSuitProduct(pmId, pid);
        }

        /// <summary>
        /// 获得扩展套装商品列表
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <returns></returns>
        public static List<ExtSuitProductInfo> GetExtSuitProductList(int pmId)
        {
            return VMall.Data.Promotions.GetExtSuitProductList(pmId);
        }

        /// <summary>
        /// 获得全部扩展套装商品列表
        /// </summary>
        /// <param name="pmIdList">促销活动id列表</param>
        /// <returns></returns>
        public static List<ExtSuitProductInfo> GetAllExtSuitProductList(string pmIdList)
        {
            return VMall.Data.Promotions.GetAllExtSuitProductList(pmIdList);
        }





        /// <summary>
        /// 获得满赠促销活动
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static FullSendPromotionInfo GetFullSendPromotionByStoreIdAndPidAndTime(int storeId, int pid, DateTime nowTime)
        {
            return VMall.Data.Promotions.GetFullSendPromotionByStoreIdAndPidAndTime(storeId, pid, nowTime);
        }

        /// <summary>
        /// 获得满赠促销活动
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static FullSendPromotionInfo GetFullSendPromotionByPmIdAndTime(int pmId, DateTime nowTime)
        {
            return VMall.Data.Promotions.GetFullSendPromotionByPmIdAndTime(pmId, nowTime);
        }




        /// <summary>
        /// 满赠商品是否存在
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <param name="pid">商品id</param>
        public static bool IsExistFullSendProduct(int pmId, int pid)
        {
            return VMall.Data.Promotions.IsExistFullSendProduct(pmId, pid);
        }

        /// <summary>
        /// 判断满赠商品是否存在
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <param name="pid">商品id</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsExistFullSendProduct(int pmId, int pid, int type)
        {
            return VMall.Data.Promotions.IsExistFullSendProduct(pmId, pid, type);
        }

        /// <summary>
        /// 获得满赠主商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pmId">满赠促销活动id</param>
        /// <param name="startPrice">开始价格</param>
        /// <param name="endPrice">结束价格</param>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static List<StoreProductInfo> GetFullSendMainProductList(int pageSize, int pageNumber, int pmId, int startPrice, int endPrice, int sortColumn, int sortDirection)
        {
            return VMall.Data.Promotions.GetFullSendMainProductList(pageSize, pageNumber, pmId, startPrice, endPrice, sortColumn, sortDirection);
        }

        /// <summary>
        /// 获得满赠主商品数量
        /// </summary>
        /// <param name="pmId">满赠促销活动id</param>
        /// <param name="startPrice">开始价格</param>
        /// <param name="endPrice">结束价格</param>
        /// <returns></returns>
        public static int GetFullSendMainProductCount(int pmId, int startPrice, int endPrice)
        {
            return VMall.Data.Promotions.GetFullSendMainProductCount(pmId, startPrice, endPrice);
        }

        /// <summary>
        /// 获得满赠赠品列表
        /// </summary>
        /// <param name="pmId">促销活动id</param>
        /// <returns></returns>
        public static List<PartProductInfo> GetFullSendPresentList(int pmId)
        {
            return VMall.Data.Promotions.GetFullSendPresentList(pmId);
        }



        /// <summary>
        /// 获得满减促销产品列表
        /// </summary>
        /// <param name="pids"></param>
        /// <param name="nowTime"></param>
        /// <returns></returns>
        public static List<StoreProductInfo> GetVailiFullCutProductListByPidsAndendTime(string pids, DateTime nowTime)
        {
            List<StoreProductInfo> list = new List<StoreProductInfo>();
            // SELECT * FROM dbo.hlh_fullcutproducts a LEFT JOIN dbo.hlh_fullcutpromotions b ON b.pmid = a.pmid WHERE pid IN (536,579,546) AND state=1 AND type<2 AND  [endtime]>GETDATE() AND [starttime]<=GETDATE()  
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM dbo.hlh_fullcutproducts a LEFT JOIN dbo.hlh_fullcutpromotions b ON b.pmid = a.pmid WHERE  state=1 AND type<2 AND  [endtime]>@nowtime AND [starttime]<=@nowtime ");
            if (!string.IsNullOrEmpty(pids))
                strSql.Append(" AND pid IN (" + pids + ")  ");
            SqlParameter[] parameters = {
					//new SqlParameter("@pids", SqlDbType.NChar,500),
                    new SqlParameter("@nowtime", SqlDbType.DateTime)
			};
            //parameters[0].Value = pids;
            parameters[0].Value = nowTime;

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString(), parameters))
            {
                while (reader.Read())
                {
                    StoreProductInfo model = new StoreProductInfo();
                    model.Pid = TypeHelper.ObjectToInt(reader["pid"]);
                    model.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
                    list.Add(model);
                }
                reader.Close();
            }
            return list;

        }

        /// <summary>
        /// 获得满减促销活动
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="pid">商品id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static FullCutPromotionInfo GetFullCutPromotionByStoreIdAndPidAndTime(int storeId, int pid, DateTime nowTime)
        {
            return VMall.Data.Promotions.GetFullCutPromotionByStoreIdAndPidAndTime(storeId, pid, nowTime);
        }

        /// <summary>
        /// 获得满减促销活动
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="pmId">促销活动id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static FullCutPromotionInfo GetFullCutPromotionByStoreIdAndPmIdAndTime(int storeId, int pmId, DateTime nowTime)
        {
            return VMall.Data.Promotions.GetFullCutPromotionByStoreIdAndPmIdAndTime(storeId, pmId, nowTime);
        }

        /// <summary>
        /// 满减商品是否已经存在
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <param name="pid">商品id</param>
        public static bool IsExistFullCutProduct(int pmId, int pid)
        {
            if (pmId > 0 && pid > 0)
                return VMall.Data.Promotions.IsExistFullCutProduct(pmId, pid);
            return false;
        }

        /// <summary>
        /// 获得全部满减促销活动
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static List<FullCutPromotionInfo> GetAllFullCutPromotion(int storeId, DateTime nowTime)
        {
            return VMall.Data.Promotions.GetAllFullCutPromotion(storeId, nowTime);
        }




        /// <summary>
        /// 获得满减商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="fullCutPromotionInfo">满减促销活动</param>
        /// <param name="startPrice">开始价格</param>
        /// <param name="endPrice">结束价格</param>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static List<StoreProductInfo> GetFullCutProductList(int pageSize, int pageNumber, FullCutPromotionInfo fullCutPromotionInfo, int startPrice, int endPrice, int sortColumn, int sortDirection)
        {
            return VMall.Data.Promotions.GetFullCutProductList(pageSize, pageNumber, fullCutPromotionInfo, startPrice, endPrice, sortColumn, sortDirection);
        }

        /// <summary>
        /// 获得满减商品数量
        /// </summary>
        /// <param name="fullCutPromotionInfo">满减促销活动</param>
        /// <param name="startPrice">开始价格</param>
        /// <param name="endPrice">结束价格</param>
        /// <returns></returns>
        public static int GetFullCutProductCount(FullCutPromotionInfo fullCutPromotionInfo, int startPrice, int endPrice)
        {
            return VMall.Data.Promotions.GetFullCutProductCount(fullCutPromotionInfo, startPrice, endPrice);
        }





        /// <summary>
        /// 生成商品的促销信息
        /// </summary>
        /// <param name="singlePromotionInfo">单品促销活动</param>
        /// <param name="buySendPromotionList">买送促销活动列表</param>
        /// <param name="fullSendPromotionInfo">满赠促销活动</param>
        /// <param name="fullCutPromotionInfo">满减促销活动</param>
        /// <returns></returns>
        public static string GeneratePromotionMsg(SinglePromotionInfo singlePromotionInfo, List<BuySendPromotionInfo> buySendPromotionList, FullSendPromotionInfo fullSendPromotionInfo, FullCutPromotionInfo fullCutPromotionInfo, PartUserInfo user)
        {
            StringBuilder promotionMsg = new StringBuilder();
            //单品促销
            if (singlePromotionInfo != null)
            {
                if (singlePromotionInfo.UserRankLower == Convert.ToInt32(user.IsDirSaleUser) || singlePromotionInfo.UserRankLower == 0)
                {
                    //折扣类别
                    switch (singlePromotionInfo.DiscountType)
                    {
                        case 0://商城价折扣
                            promotionMsg.AppendFormat("折扣：{0}折<br/>", singlePromotionInfo.DiscountValue);
                            break;
                        case 1://直降
                            promotionMsg.AppendFormat("直降：{0}元<br/>", singlePromotionInfo.DiscountValue);
                            break;
                        case 2://折后价
                            promotionMsg.AppendFormat("折后价：{0}元<br/>", singlePromotionInfo.DiscountValue);
                            break;
                        case 3://秒杀价
                            promotionMsg.AppendFormat("限时秒杀价：{0}元<br/>", singlePromotionInfo.DiscountValue);
                            break;
                        case 4://市场价折扣
                            promotionMsg.AppendFormat("折扣：{0}折<br/>", singlePromotionInfo.DiscountValue);
                            break;
                        case 5://特价
                            promotionMsg.AppendFormat("特价：{0}元<br/>", singlePromotionInfo.DiscountValue);
                            break;
                        case 6://兑换价
                            promotionMsg.AppendFormat("兑换价：{0}元<br/>", singlePromotionInfo.DiscountValue);
                            break;
                        case 7://梯度促销

                            promotionMsg.AppendFormat("促销价：{0}<br/>", singlePromotionInfo.Slogan);
                            break;
                        case 8://双数促销
                            promotionMsg.AppendFormat("促销价：{0}<br/>", singlePromotionInfo.Slogan);
                            break;
                        case 9://新品促销
                            promotionMsg.AppendFormat("新品尝鲜价：{0}<br/>", singlePromotionInfo.DiscountValue);
                            break;
                        default://默认
                            promotionMsg.AppendFormat("促销价：{0}元<br/>", singlePromotionInfo.DiscountValue);
                            break;
                    }
                }
                //积分
                if (singlePromotionInfo.PayCredits > 0)
                    promotionMsg.AppendFormat("赠送{0}：{1}<br/>", Credits.PayCreditName, singlePromotionInfo.PayCredits);

                //优惠劵
                if (singlePromotionInfo.CouponTypeId > 0)
                {
                    CouponTypeInfo couponTypeInfo = Coupons.GetCouponTypeById(singlePromotionInfo.CouponTypeId);
                    if (couponTypeInfo != null)
                        promotionMsg.AppendFormat("赠送优惠劵：{0}<br/>", couponTypeInfo.Name);
                }
            }
            //买送促销
            if (buySendPromotionList != null && buySendPromotionList.Count > 0)
            {
                promotionMsg.Append("买送促销：");
                foreach (BuySendPromotionInfo buySendPromotionInfo in buySendPromotionList)
                    promotionMsg.AppendFormat("买{0}送{1},", buySendPromotionInfo.BuyCount, buySendPromotionInfo.SendCount);
                promotionMsg.Remove(promotionMsg.Length - 1, 1);
                promotionMsg.Append("<br/>");
            }
            //满赠促销
            if (fullSendPromotionInfo != null)
            {
                promotionMsg.Append("满赠促销：");
                promotionMsg.AppendFormat("满{0}元加{1}元<br/>", fullSendPromotionInfo.LimitMoney, fullSendPromotionInfo.AddMoney);
            }
            //满减促销
            if (fullCutPromotionInfo != null)
            {
                promotionMsg.Append("满减促销：");
                promotionMsg.AppendFormat("满{0}元减{1}元,", fullCutPromotionInfo.LimitMoney1, fullCutPromotionInfo.CutMoney1);
                if (fullCutPromotionInfo.LimitMoney2 > 0 && fullCutPromotionInfo.CutMoney2 > 0)
                    promotionMsg.AppendFormat("满{0}元减{1}元,", fullCutPromotionInfo.LimitMoney2, fullCutPromotionInfo.CutMoney2);
                if (fullCutPromotionInfo.LimitMoney3 > 0 && fullCutPromotionInfo.CutMoney3 > 0)
                    promotionMsg.AppendFormat("满{0}元减{1}元,", fullCutPromotionInfo.LimitMoney3, fullCutPromotionInfo.CutMoney3);
                promotionMsg.Remove(promotionMsg.Length - 1, 1);
                promotionMsg.Append("<br/>");
            }

            return promotionMsg.Length > 0 ? promotionMsg.Remove(promotionMsg.Length - 5, 5).ToString() : "";
        }

        /// <summary>
        /// 计算商品的折扣价
        /// </summary>
        /// <param name="shopPrice">商城价格</param>
        /// <param name="singlePromotionInfo">单品促销活动</param>
        /// <returns></returns>
        public static decimal ComputeDiscountPrice(decimal shopPrice, SinglePromotionInfo singlePromotionInfo, decimal markPrice, int buyCount)
        {
            decimal discountPrice = shopPrice;
            if (singlePromotionInfo != null)
            {
                switch (singlePromotionInfo.DiscountType)
                {
                    case 0://商城价折扣
                        discountPrice = Math.Round(shopPrice * singlePromotionInfo.DiscountValue / 10, 2);
                        break;
                    case 1://直降
                        discountPrice = shopPrice - singlePromotionInfo.DiscountValue;
                        break;
                    case 2://折后价
                        discountPrice = singlePromotionInfo.DiscountValue;
                        break;
                    case 3://秒杀价
                        discountPrice = singlePromotionInfo.DiscountValue;
                        break;
                    case 4://市场价折扣
                        discountPrice = Math.Round(markPrice * singlePromotionInfo.DiscountValue / 10, 2);
                        break;
                    case 5://特价
                        discountPrice = singlePromotionInfo.DiscountValue;
                        break;
                    case 6://兑换价
                        discountPrice = singlePromotionInfo.DiscountValue;
                        break;
                    case 7://梯度优惠
                        if (singlePromotionInfo.Amount1 > 0 && buyCount > singlePromotionInfo.Amount1)
                            discountPrice = singlePromotionInfo.Discount1;
                        if (singlePromotionInfo.Amount2 > 0 && buyCount > singlePromotionInfo.Amount2)
                            discountPrice = singlePromotionInfo.Discount2;
                        if (singlePromotionInfo.Amount3 > 0 && buyCount > singlePromotionInfo.Amount3)
                            discountPrice = singlePromotionInfo.Discount3;
                        if (singlePromotionInfo.Amount4 > 0 && buyCount > singlePromotionInfo.Amount4)
                            discountPrice = singlePromotionInfo.Discount4;
                        if (singlePromotionInfo.Amount5 > 0 && buyCount > singlePromotionInfo.Amount5)
                            discountPrice = singlePromotionInfo.Discount5;
                        break;
                    case 8:
                        if (buyCount % 2 == 0)
                            discountPrice = singlePromotionInfo.DiscountValue;
                        else
                            discountPrice = shopPrice;
                        break;
                    case 9://新品价
                        discountPrice = singlePromotionInfo.DiscountValue;
                        break;
                    default:
                        discountPrice = singlePromotionInfo.DiscountValue;
                        break;

                }
            }
            if (discountPrice < 0 || discountPrice > shopPrice)
                discountPrice = shopPrice;

            return discountPrice;
        }
    }
}
