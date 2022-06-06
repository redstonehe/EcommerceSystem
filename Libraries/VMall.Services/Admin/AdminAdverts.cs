using System;
using System.Data;
using System.Collections.Generic;

using VMall.Core;
using System.Data.SqlClient;

namespace VMall.Services
{
    /// <summary>
    /// 后台广告操作管理类
    /// </summary>
    public partial class AdminAdverts : Adverts
    {
        /// <summary>
        /// 创建广告位置
        /// </summary>
        public static void CreateAdvertPosition(AdvertPositionInfo advertPositionInfo)
        {
            VMall.Data.Adverts.CreateAdvertPosition(advertPositionInfo);
        }

        /// <summary>
        /// 更新广告位置
        /// </summary>
        public static void UpdateAdvertPosition(AdvertPositionInfo advertPositionInfo)
        {
            VMall.Data.Adverts.UpdateAdvertPosition(advertPositionInfo);
        }

        /// <summary>
        /// 删除广告位置
        /// </summary>
        /// <param name="adPosId">广告位置id</param>
        public static void DeleteAdvertPositionById(int adPosId)
        {
            VMall.Data.Adverts.DeleteAdvertPositionById(adPosId);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_ADVERT_LIST + adPosId);

        }




        /// <summary>
        /// 创建广告
        /// </summary>
        public static void CreateAdvert(AdvertInfo advertInfo)
        {
            VMall.Data.Adverts.CreateAdvert(advertInfo);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_ADVERT_LIST + advertInfo.AdPosId);
        }

        /// <summary>
        /// 更新广告
        /// </summary>
        public static void UpdateAdvert(int oldAdPosId, AdvertInfo advertInfo)
        {
            VMall.Data.Adverts.UpdateAdvert(advertInfo);
            if (oldAdPosId == advertInfo.AdPosId)
            {
                VMall.Core.BMACache.Remove(CacheKeys.MALL_ADVERT_LIST + advertInfo.AdPosId);
            }
            else
            {
                VMall.Core.BMACache.Remove(CacheKeys.MALL_ADVERT_LIST + oldAdPosId);
                VMall.Core.BMACache.Remove(CacheKeys.MALL_ADVERT_LIST + advertInfo.AdPosId);
            }
        }

        /// <summary>
        /// 删除广告
        /// </summary>
        /// <param name="adId">广告id</param>
        public static void DeleteAdvertById(int adId)
        {
            AdvertInfo advertInfo = AdminGetAdvertById(adId);
            if (advertInfo != null)
            {
                VMall.Data.Adverts.DeleteAdvertById(adId);
                VMall.Core.BMACache.Remove(CacheKeys.MALL_ADVERT_LIST + advertInfo.AdPosId);
            }
        }
        /// <summary>
        /// 批量删除广告
        /// </summary>
        /// <param name="adId">广告id</param>
        public static void DeleteAdvertsByIds(int[] idList)
        {
            if (idList != null && idList.Length > 0)
            {
                string commandText = String.Format("DELETE FROM [{0}adverts] WHERE [adid] IN ({1})",
                                                RDBSHelper.RDBSTablePre,
                                                CommonHelper.IntArrayToString(idList));
                RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText);
                 VMall.Core.BMACache.Remove(CacheKeys.MALL_ADVERT_LIST );
            }
               
        }
        
        /// <summary>
        /// 后台获得广告
        /// </summary>
        /// <param name="adId">广告id</param>
        /// <returns></returns>
        public static AdvertInfo AdminGetAdvertById(int adId)
        {
            return VMall.Data.Adverts.AdminGetAdvertById(adId);
        }

        /// <summary>
        /// 后台获得广告列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="adPosId">广告位置id</param>
        /// <returns></returns>
        public static DataTable AdminGetAdvertList(int pageSize, int pageNumber, int adPosId, string title)
        {
            return VMall.Data.Adverts.AdminGetAdvertList(pageSize, pageNumber, adPosId,title);
        }

        /// <summary>
        /// 后台获得广告数量
        /// </summary>
        /// <param name="adPosId">广告位置id</param>
        /// <returns></returns>
        public static int AdminGetAdvertCount(int adPosId,string title)
        {
            return VMall.Data.Adverts.AdminGetAdvertCount(adPosId,title);
        }

        /// <summary>
        /// 设置广告显示隐藏
        /// </summary>
        public static bool SetAdvState(int adid,int state)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@adid",adid),
                                       new SqlParameter("@state",state)
                                   };
            string commandText = string.Format("UPDATE [{0}adverts] SET [state]=@state WHERE [adid]=@adid",
                                               RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
    }
}
