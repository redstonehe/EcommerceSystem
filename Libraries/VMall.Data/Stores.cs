﻿using System;
using System.Data;
using System.Collections.Generic;

using VMall.Core;

namespace VMall.Data
{
    /// <summary>
    /// 店铺数据访问类
    /// </summary>
    public partial class Stores
    {
        private static IStoreNOSQLStrategy _storenosql = BMAData.StoreNOSQL;//店铺非关系型数据库

        #region 辅助方法

        /// <summary>
        /// 从IDataReader创建StoreReviewInfo
        /// </summary>
        public static StoreReviewInfo BuildStoreReviewFromReader(IDataReader reader)
        {
            StoreReviewInfo storeReviewInfo = new StoreReviewInfo();

            storeReviewInfo.ReviewId = TypeHelper.ObjectToInt(reader["reviewid"]);
            storeReviewInfo.Oid = TypeHelper.ObjectToInt(reader["oid"]);
            storeReviewInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            storeReviewInfo.DescriptionStar = TypeHelper.ObjectToInt(reader["descriptionstar"]);
            storeReviewInfo.ServiceStar = TypeHelper.ObjectToInt(reader["servicestar"]);
            storeReviewInfo.ShipStar = TypeHelper.ObjectToInt(reader["shipstar"]);
            storeReviewInfo.Uid = TypeHelper.ObjectToInt(reader["uid"]);
            storeReviewInfo.ReviewTime = TypeHelper.ObjectToDateTime(reader["reviewtime"]);
            storeReviewInfo.IP = reader["ip"].ToString();

            return storeReviewInfo;
        }

        /// <summary>
        /// 从IDataReader创建StoreInfo
        /// </summary>
        public static StoreInfo BuildStoreFromReader(IDataReader reader)
        {
            StoreInfo storeInfo = new StoreInfo();

            storeInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            storeInfo.State = TypeHelper.ObjectToInt(reader["state"]);
            storeInfo.Name = reader["name"].ToString();
            storeInfo.RegionId = TypeHelper.ObjectToInt(reader["regionid"]);
            storeInfo.StoreRid = TypeHelper.ObjectToInt(reader["storerid"]);
            storeInfo.StoreIid = TypeHelper.ObjectToInt(reader["storeiid"]);
            storeInfo.Logo = reader["logo"].ToString();
            storeInfo.CreateTime = TypeHelper.ObjectToDateTime(reader["createtime"]);
            storeInfo.Mobile = reader["mobile"].ToString();
            storeInfo.Phone = reader["phone"].ToString();
            storeInfo.QQ = reader["qq"].ToString();
            storeInfo.WW = reader["ww"].ToString();
            storeInfo.DePoint = TypeHelper.ObjectToDecimal(reader["depoint"]);
            storeInfo.SePoint = TypeHelper.ObjectToDecimal(reader["sepoint"]);
            storeInfo.ShPoint = TypeHelper.ObjectToDecimal(reader["shpoint"]);
            storeInfo.Honesties = TypeHelper.ObjectToInt(reader["honesties"]);
            storeInfo.StateEndTime = TypeHelper.ObjectToDateTime(reader["stateendtime"]);
            storeInfo.Theme = reader["theme"].ToString();
            storeInfo.Banner = reader["banner"].ToString();
            storeInfo.Announcement = reader["announcement"].ToString();
            storeInfo.Description = reader["description"].ToString();
            storeInfo.Amount1 = TypeHelper.ObjectToDecimal(reader["amount1"]);
            storeInfo.Discount1 = TypeHelper.ObjectToDecimal(reader["discount1"]);
            storeInfo.Amount2 = TypeHelper.ObjectToDecimal(reader["amount2"]);
            storeInfo.Discount2 = TypeHelper.ObjectToDecimal(reader["discount2"]);
            storeInfo.Amount3 = TypeHelper.ObjectToDecimal(reader["amount3"]);
            storeInfo.Discount3 = TypeHelper.ObjectToDecimal(reader["discount3"]);
            storeInfo.Amount4 = TypeHelper.ObjectToDecimal(reader["amount4"]);
            storeInfo.Discount4 = TypeHelper.ObjectToDecimal(reader["discount4"]);
            storeInfo.Amount5 = TypeHelper.ObjectToDecimal(reader["amount5"]);
            storeInfo.Discount5 = TypeHelper.ObjectToDecimal(reader["discount5"]);
            storeInfo.MallSource = TypeHelper.ObjectToInt(reader["mallsource"]);
            return storeInfo;
        }

        /// <summary>
        /// 从IDataReader创建StoreKeeperInfo
        /// </summary>
        public static StoreKeeperInfo BuildStoreKeeperFromReader(IDataReader reader)
        {
            StoreKeeperInfo storeKeeperInfo = new StoreKeeperInfo();

            storeKeeperInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            storeKeeperInfo.Type = TypeHelper.ObjectToInt(reader["type"]);
            storeKeeperInfo.Name = reader["name"].ToString();
            storeKeeperInfo.IdCard = reader["idcard"].ToString();
            storeKeeperInfo.Address = reader["address"].ToString();

            return storeKeeperInfo;
        }

        /// <summary>
        /// 从IDataReader创建StoreShipTemplateInfo
        /// </summary>
        public static StoreShipTemplateInfo BuildStoreShipTemplateFromReader(IDataReader reader)
        {
            StoreShipTemplateInfo storeShipTemplateInfo = new StoreShipTemplateInfo();

            storeShipTemplateInfo.StoreSTid = TypeHelper.ObjectToInt(reader["storestid"]);
            storeShipTemplateInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            storeShipTemplateInfo.Free = TypeHelper.ObjectToInt(reader["free"]);
            storeShipTemplateInfo.Type = TypeHelper.ObjectToInt(reader["type"]);
            storeShipTemplateInfo.Title = reader["title"].ToString();
            storeShipTemplateInfo.FreeStartPrice = TypeHelper.ObjectToDecimal(reader["freestartprice"]);
            storeShipTemplateInfo.TemplateRemark = reader["templateremark"].ToString();
            storeShipTemplateInfo.NoSendArea = reader["nosendarea"].ToString();
            storeShipTemplateInfo.NoSendCity = reader["nosendcity"].ToString();
            return storeShipTemplateInfo;
        }

        /// <summary>
        /// 从IDataReader创建StoreShipFeeInfo
        /// </summary>
        public static StoreShipFeeInfo BuildStoreShipFeeFromReader(IDataReader reader)
        {
            StoreShipFeeInfo storeShipFeeInfo = new StoreShipFeeInfo();

            storeShipFeeInfo.RecordId = TypeHelper.ObjectToInt(reader["recordid"]);
            storeShipFeeInfo.StoreSTid = TypeHelper.ObjectToInt(reader["storestid"]);
            storeShipFeeInfo.RegionId = reader["regionid"].ToString();
            storeShipFeeInfo.StartValue = TypeHelper.ObjectToFloat(reader["startvalue"]);
            storeShipFeeInfo.StartFee = TypeHelper.ObjectToDecimal(reader["startfee"]);
            storeShipFeeInfo.AddValue = TypeHelper.ObjectToFloat(reader["addvalue"]);
            storeShipFeeInfo.AddFee = TypeHelper.ObjectToDecimal(reader["addfee"]);
            storeShipFeeInfo.ShipType = TypeHelper.ObjectToInt(reader["shiptype"]);
            storeShipFeeInfo.CityId = reader["cityid"].ToString();
            return storeShipFeeInfo;
        }

        #endregion

        /// <summary>
        /// 创建店铺评价
        /// </summary>
        /// <param name="storeReviewInfo">店铺评价信息</param>
        public static void CreateStoreReview(StoreReviewInfo storeReviewInfo)
        {
            VMall.Core.BMAData.RDBS.CreateStoreReview(storeReviewInfo);
        }

        /// <summary>
        /// 获得店铺评价
        /// </summary>
        /// <param name="oid">订单id</param>
        public static StoreReviewInfo GetStoreReviewByOid(int oid)
        {
            StoreReviewInfo storeReviewInfo = null;
            IDataReader reader = VMall.Core.BMAData.RDBS.GetStoreReviewByOid(oid);
            if (reader.Read())
            {
                storeReviewInfo = BuildStoreReviewFromReader(reader);
            }
            reader.Close();
            return storeReviewInfo;
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
            return VMall.Core.BMAData.RDBS.SumStoreReview(storeId, startTime, endTime);
        }







        /// <summary>
        /// 创建店铺
        /// </summary>
        /// <param name="storeInfo">店铺信息</param>
        /// <returns>店铺id</returns>
        public static int CreateStore(StoreInfo storeInfo)
        {
            return VMall.Core.BMAData.RDBS.CreateStore(storeInfo);
        }

        /// <summary>
        /// 更新店铺
        /// </summary>
        /// <param name="storeInfo">店铺信息</param>
        public static void UpdateStore(StoreInfo storeInfo)
        {
            VMall.Core.BMAData.RDBS.UpdateStore(storeInfo);
            if (_storenosql != null)
                _storenosql.UpdateStore(storeInfo);
        }

        /// <summary>
        /// 获得店铺
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static StoreInfo GetStoreById(int storeId)
        {
            StoreInfo storeInfo = null;

            if (_storenosql != null)
            {
                storeInfo = _storenosql.GetStoreById(storeId);
                if (storeInfo == null)
                {
                    IDataReader reader = VMall.Core.BMAData.RDBS.GetStoreById(storeId);
                    if (reader.Read())
                    {
                        storeInfo = BuildStoreFromReader(reader);
                    }
                    reader.Close();
                    if (storeInfo != null)
                        _storenosql.CreateStore(storeInfo);
                }
            }
            else
            {
                IDataReader reader = VMall.Core.BMAData.RDBS.GetStoreById(storeId);
                if (reader.Read())
                {
                    storeInfo = BuildStoreFromReader(reader);
                }
                reader.Close();
            }
            return storeInfo;
        }

        /// <summary>
        /// 后台获得店铺列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetStoreList(int pageSize, int pageNumber, string condition, string sort)
        {
            return VMall.Core.BMAData.RDBS.AdminGetStoreList(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 后台获得店铺选择列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static DataTable AdminGetStoreSelectList(int pageSize, int pageNumber, string condition)
        {
            return VMall.Core.BMAData.RDBS.AdminGetStoreSelectList(pageSize, pageNumber, condition);
        }

        /// <summary>
        /// 后台获得店铺列表条件
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="storeRid">店铺等级id</param>
        /// <param name="storeIid">店铺行业id</param>
        /// <param name="state">店铺状态</param>
        /// <returns></returns>
        public static string AdminGetStoreListCondition(string storeName, int storeRid, int storeIid, int state, int storeId = 0)
        {
            return VMall.Core.BMAData.RDBS.AdminGetStoreListCondition(storeName, storeRid, storeIid, state, storeId);
        }

        /// <summary>
        /// 后台获得店铺列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetStoreListSort(string sortColumn, string sortDirection)
        {
            return VMall.Core.BMAData.RDBS.AdminGetStoreListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得店铺数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetStoreCount(string condition)
        {
            return VMall.Core.BMAData.RDBS.AdminGetStoreCount(condition);
        }

        /// <summary>
        /// 根据店铺名称得到店铺id
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <returns></returns>
        public static int GetStoreIdByName(string storeName)
        {
            return VMall.Core.BMAData.RDBS.GetStoreIdByName(storeName);
        }

        /// <summary>
        /// 后台根据店铺名称得到店铺id
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <returns></returns>
        public static int AdminGetStoreIdByName(string storeName)
        {
            return VMall.Core.BMAData.RDBS.AdminGetStoreIdByName(storeName);
        }

        /// <summary>
        /// 更新店铺状态
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="state">状态</param>
        /// <param name="stateEndTime">状态结束时间</param>
        public static void UpdateStoreState(int storeId, StoreState state, DateTime stateEndTime)
        {
            VMall.Core.BMAData.RDBS.UpdateStoreState(storeId, state, stateEndTime);
            if (_storenosql != null)
                _storenosql.UpdateStoreState(storeId, state, stateEndTime);
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
            VMall.Core.BMAData.RDBS.UpdateStorePoint(storeId, dePoint, sePoint, shPoint);
            if (_storenosql != null)
                _storenosql.UpdateStorePoint(storeId, dePoint, sePoint, shPoint);
        }

        /// <summary>
        /// 删除店铺
        /// </summary>
        /// <param name="storeId">店铺id</param>
        public static void DeleteStoreById(int storeId)
        {
            VMall.Core.BMAData.RDBS.DeleteStoreById(storeId);
            if (_storenosql != null)
                _storenosql.DeleteStoreById(storeId);
        }

        /// <summary>
        /// 获得店铺数量通过店铺行业id
        /// </summary>
        /// <param name="storeIid">店铺行业id</param>
        /// <returns></returns>
        public static int GetStoreCountByStoreIid(int storeIid)
        {
            return VMall.Core.BMAData.RDBS.GetStoreCountByStoreIid(storeIid);
        }

        /// <summary>
        /// 获得店铺数量通过店铺等级id
        /// </summary>
        /// <param name="storeRid">店铺等级id</param>
        /// <returns></returns>
        public static int GetStoreCountByStoreRid(int storeRid)
        {
            return VMall.Core.BMAData.RDBS.GetStoreCountByStoreRid(storeRid);
        }

        /// <summary>
        /// 获得店铺id列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetStoreIdList()
        {
            return VMall.Core.BMAData.RDBS.GetStoreIdList();
        }






        /// <summary>
        /// 创建店长
        /// </summary>
        /// <param name="storeKeeperInfo">店长信息</param>
        public static void CreateStoreKeeper(StoreKeeperInfo storeKeeperInfo)
        {
            VMall.Core.BMAData.RDBS.CreateStoreKeeper(storeKeeperInfo);
        }

        /// <summary>
        /// 更新店长
        /// </summary>
        /// <param name="storeKeeperInfo">店长信息</param>
        public static void UpdateStoreKeeper(StoreKeeperInfo storeKeeperInfo)
        {
            VMall.Core.BMAData.RDBS.UpdateStoreKeeper(storeKeeperInfo);
            if (_storenosql != null)
                _storenosql.UpdateStoreKeeper(storeKeeperInfo);
        }

        /// <summary>
        /// 获得店长
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static StoreKeeperInfo GetStoreKeeperById(int storeId)
        {
            StoreKeeperInfo storeKeeperInfo = null;

            if (_storenosql != null)
            {
                storeKeeperInfo = _storenosql.GetStoreKeeperById(storeId);
                if (storeKeeperInfo == null)
                {
                    IDataReader reader = VMall.Core.BMAData.RDBS.GetStoreKeeperById(storeId);
                    if (reader.Read())
                    {
                        storeKeeperInfo = BuildStoreKeeperFromReader(reader);
                    }
                    reader.Close();
                    if (storeKeeperInfo != null)
                        _storenosql.CreateStoreKeeper(storeKeeperInfo);
                }
            }
            else
            {
                IDataReader reader = VMall.Core.BMAData.RDBS.GetStoreKeeperById(storeId);
                if (reader.Read())
                {
                    storeKeeperInfo = BuildStoreKeeperFromReader(reader);
                }
                reader.Close();
            }

            return storeKeeperInfo;
        }




        /// <summary>
        /// 获得店铺分类列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static List<StoreClassInfo> GetStoreClassList(int storeId)
        {
            List<StoreClassInfo> storeClassList = new List<StoreClassInfo>();

            IDataReader reader = VMall.Core.BMAData.RDBS.GetStoreClassList(storeId);
            while (reader.Read())
            {
                StoreClassInfo storeClassInfo = new StoreClassInfo();
                storeClassInfo.StoreCid = TypeHelper.ObjectToInt(reader["storecid"]);
                storeClassInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
                storeClassInfo.DisplayOrder = TypeHelper.ObjectToInt(reader["displayorder"]);
                storeClassInfo.Name = reader["name"].ToString();
                storeClassInfo.ParentId = TypeHelper.ObjectToInt(reader["parentid"]);
                storeClassInfo.Layer = TypeHelper.ObjectToInt(reader["layer"]);
                storeClassInfo.HasChild = TypeHelper.ObjectToInt(reader["haschild"]);
                storeClassInfo.Path = reader["path"].ToString();
                storeClassList.Add(storeClassInfo);
            }
            reader.Close();

            return storeClassList;
        }

        /// <summary>
        /// 创建店铺分类
        /// </summary>
        public static int CreateStoreClass(StoreClassInfo storeClassInfo)
        {
            return VMall.Core.BMAData.RDBS.CreateStoreClass(storeClassInfo);
        }

        /// <summary>
        /// 更新店铺分类
        /// </summary>
        public static void UpdateStoreClass(StoreClassInfo storeClassInfo)
        {
            VMall.Core.BMAData.RDBS.UpdateStoreClass(storeClassInfo);
        }

        /// <summary>
        /// 删除店铺分类
        /// </summary>
        /// <param name="storeCid">店铺分类id</param>
        public static void DeleteStoreClassById(int storeCid)
        {
            VMall.Core.BMAData.RDBS.DeleteStoreClassById(storeCid);
        }




        /// <summary>
        /// 创建店铺配送模板
        /// </summary>
        /// <param name="storeShipTemplateInfo">店铺配送模板信息</param>
        public static int CreateStoreShipTemplate(StoreShipTemplateInfo storeShipTemplateInfo)
        {
            return VMall.Core.BMAData.RDBS.CreateStoreShipTemplate(storeShipTemplateInfo);
        }

        /// <summary>
        /// 更新店铺配送模板
        /// </summary>
        /// <param name="storeShipTemplateInfo">店铺配送模板信息</param>
        public static void UpdateStoreShipTemplate(StoreShipTemplateInfo storeShipTemplateInfo)
        {
            VMall.Core.BMAData.RDBS.UpdateStoreShipTemplate(storeShipTemplateInfo);
        }

        /// <summary>
        /// 删除店铺配送模板
        /// </summary>
        /// <param name="storeSTId">店铺配送模板id</param>
        public static void DeleteStoreShipTemplateById(int storeSTId)
        {
            VMall.Core.BMAData.RDBS.DeleteStoreShipTemplateById(storeSTId);
        }

        /// <summary>
        /// 获得店铺配送模板
        /// </summary>
        /// <param name="storeSTId">店铺配送模板id</param>
        /// <returns></returns>
        public static StoreShipTemplateInfo GetStoreShipTemplateById(int storeSTId)
        {
            StoreShipTemplateInfo storeShipTemplateInfo = null;
            IDataReader reader = VMall.Core.BMAData.RDBS.GetStoreShipTemplateById(storeSTId);
            if (reader.Read())
            {
                storeShipTemplateInfo = BuildStoreShipTemplateFromReader(reader);
            }

            reader.Close();
            return storeShipTemplateInfo;
        }

        /// <summary>
        /// 获得店铺配送模板列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static List<StoreShipTemplateInfo> GetStoreShipTemplateList(int storeId)
        {
            List<StoreShipTemplateInfo> storeShipTemplateList = new List<StoreShipTemplateInfo>();

            IDataReader reader = VMall.Core.BMAData.RDBS.GetStoreShipTemplateList(storeId);
            while (reader.Read())
            {
                StoreShipTemplateInfo storeShipTemplateInfo = BuildStoreShipTemplateFromReader(reader);
                storeShipTemplateList.Add(storeShipTemplateInfo);
            }
            reader.Close();

            return storeShipTemplateList;
        }




        /// <summary>
        /// 创建店铺配送费用
        /// </summary>
        /// <param name="storeShipFeeInfo">店铺配送费用信息</param>
        public static void CreateStoreShipFee(StoreShipFeeInfo storeShipFeeInfo)
        {
            VMall.Core.BMAData.RDBS.CreateStoreShipFee(storeShipFeeInfo);
        }

        /// <summary>
        /// 更新店铺配送费用
        /// </summary>
        /// <param name="storeShipFeeInfo">店铺配送费用信息</param>
        public static void UpdateStoreShipFee(StoreShipFeeInfo storeShipFeeInfo)
        {
            VMall.Core.BMAData.RDBS.UpdateStoreShipFee(storeShipFeeInfo);
        }

        /// <summary>
        /// 删除店铺配送费用
        /// </summary>
        /// <param name="recordId">记录id</param>
        public static void DeleteStoreShipFeeById(int recordId)
        {
            VMall.Core.BMAData.RDBS.DeleteStoreShipFeeById(recordId);
        }

        /// <summary>
        /// 获得店铺配送费用
        /// </summary>
        /// <param name="recordId">记录id</param>
        /// <returns></returns>
        public static StoreShipFeeInfo GetStoreShipFeeById(int recordId)
        {
            StoreShipFeeInfo storeShipFeeInfo = null;
            IDataReader reader = VMall.Core.BMAData.RDBS.GetStoreShipFeeById(recordId);
            if (reader.Read())
            {
                storeShipFeeInfo = BuildStoreShipFeeFromReader(reader);
            }

            reader.Close();
            return storeShipFeeInfo;
        }

        /// <summary>
        /// 后台获得店铺配送费用列表
        /// </summary>
        /// <param name="storeSTid">店铺配送模板id</param>
        /// <returns></returns>
        public static DataTable AdminGetStoreShipFeeList(int storeSTid)
        {
            return VMall.Core.BMAData.RDBS.AdminGetStoreShipFeeList(storeSTid);
        }

        /// <summary>
        /// 获得店铺配送费用
        /// </summary>
        /// <param name="storeSTid">店铺模板id</param>
        /// <param name="provinceId">省id</param>
        /// <param name="cityId">市id</param>
        /// <returns></returns>
        public static StoreShipFeeInfo GetStoreShipFeeByStoreSTidAndRegion(int storeSTid, int provinceId, int cityId)
        {
            StoreShipFeeInfo storeShipFeeInfo = null;
            IDataReader reader = VMall.Core.BMAData.RDBS.GetStoreShipFeeByStoreSTidAndRegion(storeSTid, provinceId, cityId);
            if (reader.Read())
            {
                storeShipFeeInfo = BuildStoreShipFeeFromReader(reader);
            }

            reader.Close();
            return storeShipFeeInfo;
        }
    }
}
