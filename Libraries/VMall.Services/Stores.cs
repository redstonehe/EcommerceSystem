using System;
using System.Data;
using System.Collections.Generic;

using VMall.Core;
using System.Data.SqlClient;

namespace VMall.Services
{
    /// <summary>
    /// 店铺操作管理类
    /// </summary>
    public partial class Stores
    {
        /// <summary>
        /// 创建店铺评价
        /// </summary>
        /// <param name="storeReviewInfo">店铺评价信息</param>
        public static void CreateStoreReview(StoreReviewInfo storeReviewInfo)
        {
            VMall.Data.Stores.CreateStoreReview(storeReviewInfo);
        }

        /// <summary>
        /// 获得店铺评价
        /// </summary>
        /// <param name="oid">订单id</param>
        public static StoreReviewInfo GetStoreReviewByOid(int oid)
        {
            return VMall.Data.Stores.GetStoreReviewByOid(oid);
        }

        /// <summary>
        /// 汇总店铺评价
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static DataTable SumStoreReview(int storeId, DateTime startTime, DateTime endTime)
        {
            return VMall.Data.Stores.SumStoreReview(storeId, startTime, endTime);
        }





        /// <summary>
        /// 获得店铺
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static StoreInfo GetStoreById(int storeId)
        {
            if (storeId < 1) return null;
            return VMall.Data.Stores.GetStoreById(storeId);
        }

        /// <summary>
        /// 根据店铺名称得到店铺id
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <returns></returns>
        public static int GetStoreIdByName(string storeName)
        {
            if (string.IsNullOrWhiteSpace(storeName))
                return 0;
            return VMall.Data.Stores.GetStoreIdByName(storeName);
        }

        /// <summary>
        /// 更新店铺积分
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="dePoint">商品描述评分</param>
        /// <param name="sePoint">商家服务评分</param>
        /// <param name="shPoint">商家配送评分</param>
        public static void UpdateStorePoint(int storeId, decimal dePoint, decimal sePoint, decimal shPoint)
        {
            VMall.Data.Stores.UpdateStorePoint(storeId, dePoint, sePoint, shPoint);
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetStoreIdList()
        {
            return VMall.Data.Stores.GetStoreIdList();
        }





        /// <summary>
        /// 获得店长
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static StoreKeeperInfo GetStoreKeeperById(int storeId)
        {
            return VMall.Data.Stores.GetStoreKeeperById(storeId);
        }





        /// <summary>
        /// 获得店铺分类列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static List<StoreClassInfo> GetStoreClassList(int storeId)
        {
            List<StoreClassInfo> storeClassList = VMall.Core.BMACache.Get(CacheKeys.MALL_STORE_CLASSLIST + storeId) as List<StoreClassInfo>;
            if (storeClassList == null)
            {
                storeClassList = new List<StoreClassInfo>();
                List<StoreClassInfo> sourceStoreClassList = VMall.Data.Stores.GetStoreClassList(storeId);
                CreateStoreClassTree(sourceStoreClassList, storeClassList, 0);
                VMall.Core.BMACache.Insert(CacheKeys.MALL_STORE_CLASSLIST + storeId, storeClassList);
            }
            return storeClassList;
        }

        /// <summary>
        /// 递归创建店铺分类列表树
        /// </summary>
        private static void CreateStoreClassTree(List<StoreClassInfo> sourceStoreClassList, List<StoreClassInfo> resultStoreClassList, int parentId)
        {
            foreach (StoreClassInfo storeClassInfo in sourceStoreClassList)
            {
                if (storeClassInfo.ParentId == parentId)
                {
                    resultStoreClassList.Add(storeClassInfo);
                    CreateStoreClassTree(sourceStoreClassList, resultStoreClassList, storeClassInfo.StoreCid);
                }
            }
        }

        /// <summary>
        /// 获得店铺分类id
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="name">店铺分类名称</param>
        /// <returns></returns>
        public static int GetStoreCidByStoreIdAndName(int storeId, string name)
        {
            if (storeId < 1 || string.IsNullOrWhiteSpace(name))
                return 0;
            foreach (StoreClassInfo storeClassInfo in GetStoreClassList(storeId))
            {
                if (storeClassInfo.Name == name)
                    return storeClassInfo.StoreCid;
            }
            return 0;
        }

        /// <summary>
        /// 获得店铺分类
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="name">店铺分类名称</param>
        /// <returns></returns>
        public static StoreClassInfo GetStoreClassByStoreIdAndName(int storeId, string name)
        {
            foreach (StoreClassInfo storeClassInfo in GetStoreClassList(storeId))
            {
                if (storeClassInfo.Name == name)
                    return storeClassInfo;
            }
            return null;
        }

        /// <summary>
        /// 获得店铺分类
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="storeCid">店铺分类id</param>
        /// <returns></returns>
        public static StoreClassInfo GetStoreClassByStoreIdAndStoreCid(int storeId, int storeCid)
        {
            if (storeCid < 1) return null;
            foreach (StoreClassInfo storeClassInfo in GetStoreClassList(storeId))
            {
                if (storeClassInfo.StoreCid == storeCid)
                    return storeClassInfo;
            }

            return null;
        }

        /// <summary>
        /// 判断是否有子店铺分类
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="storeCid">店铺分类id</param>
        /// <returns></returns>
        public static bool IsHasChildStoreClass(int storeId, int storeCid)
        {
            foreach (StoreClassInfo storeClassInfo in GetStoreClassList(storeId))
            {
                if (storeClassInfo.ParentId == storeCid)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获得子店铺分类列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="storeCid">店铺分类id</param>
        /// <returns></returns>
        public static List<StoreClassInfo> GetChildStoreClassList(int storeId, int storeCid)
        {
            return GetChildStoreClassList(storeId, storeCid, false);
        }

        /// <summary>
        /// 获得子店铺分类列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="storeCid">店铺分类id</param>
        /// <param name="isAllChildren">是否包括全部子节点</param>
        /// <returns></returns>
        public static List<StoreClassInfo> GetChildStoreClassList(int storeId, int storeCid, bool isAllChildren)
        {
            List<StoreClassInfo> storeClassList = new List<StoreClassInfo>();

            if (storeId > 0 && storeCid > 0)
            {
                int flag = 0;
                if (isAllChildren)
                {
                    foreach (StoreClassInfo storeClassInfo in GetStoreClassList(storeId))
                    {
                        if (storeClassInfo.ParentId == storeCid || storeClassInfo.Layer > 2)
                        {
                            flag = 1;
                            storeClassList.Add(storeClassInfo);
                        }
                        else if (flag == 1 && storeClassInfo.Layer == 1)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    foreach (StoreClassInfo storeClassInfo in GetStoreClassList(storeId))
                    {
                        if (storeClassInfo.ParentId == storeCid && storeClassInfo.Layer == 2)
                        {
                            flag = 1;
                            storeClassList.Add(storeClassInfo);
                        }
                        else if (flag == 1 && storeClassInfo.Layer == 1)
                        {
                            break;
                        }
                    }
                }
            }
            return storeClassList;
        }





        /// <summary>
        /// 获得店铺配送模板
        /// </summary>
        /// <param name="storeSTid">店铺配送模板id</param>
        /// <returns></returns>
        public static StoreShipTemplateInfo GetStoreShipTemplateById(int storeSTid)
        {
            StoreShipTemplateInfo storeShipTemplateInfo = VMall.Core.BMACache.Get(CacheKeys.MALL_STORE_SHIPTEMPLATEINFO + storeSTid) as StoreShipTemplateInfo;
            if (storeShipTemplateInfo == null)
            {
                storeShipTemplateInfo = VMall.Data.Stores.GetStoreShipTemplateById(storeSTid);
                VMall.Core.BMACache.Insert(CacheKeys.MALL_STORE_SHIPTEMPLATEINFO + storeSTid, storeShipTemplateInfo);
            }

            return storeShipTemplateInfo;
        }

        /// <summary>
        /// 获得店铺配送模板列表
        /// </summary>
        /// <param name="storeSTid">店铺配送模板id</param>
        /// <returns></returns>
        public static List<StoreShipTemplateInfo> GetStoreShipTemplateListByIds(string storeSTids)
        {
            List<StoreShipTemplateInfo> list = new List<StoreShipTemplateInfo>();
            string commandText = string.Format("SELECT * FROM [{0}storeshiptemplates]  ", RDBSHelper.RDBSTablePre);
            if (!string.IsNullOrEmpty(storeSTids))
                commandText += " where storestid IN (" + storeSTids + ")  ";
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    StoreShipTemplateInfo Info = VMall.Data.Stores.BuildStoreShipTemplateFromReader(reader);
                    list.Add(Info);
                }
                reader.Close();
            }
            return list;

        }



        /// <summary>
        /// 获得店铺配送费用
        /// </summary>
        /// <param name="storeSTId">店铺模板id</param>
        /// <param name="provinceId">省id</param>
        /// <param name="cityId">市id</param>
        /// <returns></returns>
        public static StoreShipFeeInfo GetStoreShipFeeByStoreSTidAndRegion(int storeSTId, int provinceId, int cityId)
        {
            return VMall.Data.Stores.GetStoreShipFeeByStoreSTidAndRegion(storeSTId, provinceId, cityId);
        }

        /// <summary>
        /// 获得店铺配送费用
        /// </summary>
        /// <param name="storeSTid">店铺模板id</param>
        /// <param name="provinceId">省id</param>
        /// <param name="cityId">市id</param>
        /// <returns></returns>
        public static StoreShipFeeInfo GetStoreShipFeeByStoreSTidAndRegionAndType(int storeSTid, int provinceId, int cityId, int shipType)
        {
            //SELECT  * FROM [hlh_storeshipfees] WHERE storestid=3 AND ( CHARINDEX(','+[regionid]+',',',8,')>0   OR [regionid]='' OR [regionid]='-1')
            List<StoreShipFeeInfo> list = new List<StoreShipFeeInfo>();
            StoreShipFeeInfo info = null;
            SqlParameter[] parms =  {
                                       new SqlParameter("@storestid",storeSTid),
                                        new SqlParameter("@shiptype",shipType),
                                       new SqlParameter("@provinceid",","+provinceId.ToString()+",")
                                   };
            string commandText = "SELECT  * FROM [hlh_storeshipfees] WHERE storestid=@storestid AND shiptype=@shiptype AND ( CHARINDEX('," + provinceId.ToString() + ",' ,','+[regionid]+',')>0  OR [regionid]='' OR [regionid]='-1')";
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    StoreShipFeeInfo orderInfo = VMall.Data.Stores.BuildStoreShipFeeFromReader(reader);
                    list.Add(orderInfo);
                }
                reader.Close();
            }
            if (list.Exists(x => StringHelper.StrContainsForNum(x.RegionId, provinceId.ToString())))
            {
                List<StoreShipFeeInfo> ProvinceList = list.FindAll(x => StringHelper.StrContainsForNum(x.RegionId, provinceId.ToString()));
                StoreShipFeeInfo cityInfo = ProvinceList.Find(x => StringHelper.StrContainsForNum(x.CityId, cityId.ToString()));
                if (ProvinceList.Exists(x => StringHelper.StrContainsForNum(x.CityId, cityId.ToString())))
                {
                    info = ProvinceList.Find(x => StringHelper.StrContainsForNum(x.RegionId, provinceId.ToString()) && StringHelper.StrContainsForNum(x.CityId, cityId.ToString()));
                }
                else
                {
                    if (cityInfo == null)
                    {
                        info = ProvinceList.Find(x => StringHelper.StrContainsForNum(x.RegionId, provinceId.ToString()) && string.IsNullOrEmpty(x.CityId));
                        if (info == null)
                            info = list.Find(x => x.RegionId == "-1");
                    }
                }
            }
            else
            {
                info = list.Find(x => x.RegionId == "-1");
            }
            return info;


        }
        /// <summary>
        /// 后台获得店铺配送费用列表
        /// </summary>
        /// <param name="storeSTid">店铺配送模板id</param>
        /// <returns></returns>
        public static List<StoreShipFeeInfo> GetStoreShipFeeList(int storeSTid)
        {
            List<StoreShipFeeInfo> list = new List<StoreShipFeeInfo>();
            SqlParameter[] parms = {
                                    new SqlParameter("@storestid",storeSTid)
                                    
                                   };
            string commandText = string.Format("SELECT * FROM [{0}storeshipfees] WHERE [storestid]=@storestid ", RDBSHelper.RDBSTablePre);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    StoreShipFeeInfo orderInfo = VMall.Data.Stores.BuildStoreShipFeeFromReader(reader);
                    list.Add(orderInfo);
                }
                reader.Close();
            }
            return list;
        }
    }
}
