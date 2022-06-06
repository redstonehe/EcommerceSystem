using System;
using System.Collections.Generic;

using VMall.Core;

namespace VMall.Services
{
    /// <summary>
    /// 广告操作管理类
    /// </summary>
    public partial class Adverts
    {
        /// <summary>
        /// 获得广告位置列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public static List<AdvertPositionInfo> GetAdvertPositionList(int pageSize, int pageNumber, string title)
        {
            return VMall.Data.Adverts.GetAdvertPositionList(pageSize, pageNumber,title);
        }

        /// <summary>
        /// 获得广告位置数量
        /// </summary>
        /// <returns></returns>
        public static int GetAdvertPositionCount(string title)
        {
            return VMall.Data.Adverts.GetAdvertPositionCount(title);
        }

        /// <summary>
        /// 获得全部广告位置
        /// </summary>
        /// <returns></returns>
        public static List<AdvertPositionInfo> GetAllAdvertPosition()
        {
            return VMall.Data.Adverts.GetAllAdvertPosition();
        }

        /// <summary>
        /// 获得广告位置
        /// </summary>
        /// <param name="adPosId">广告位置id</param>
        /// <returns></returns>
        public static AdvertPositionInfo GetAdvertPositionById(int adPosId)
        {
            if (adPosId > 0)
                return VMall.Data.Adverts.GetAdvertPositionById(adPosId);
            return null;
        }




        /// <summary>
        /// 获得广告列表
        /// </summary>
        /// <param name="adPosId">广告位置id</param>
        /// <returns></returns>
        public static List<AdvertInfo> GetAdvertList(int adPosId)
        {
            List<AdvertInfo> advertList = VMall.Core.BMACache.Get(CacheKeys.MALL_ADVERT_LIST + adPosId) as List<AdvertInfo>;
            if (advertList == null)
            {
                advertList = VMall.Data.Adverts.GetAdvertList(adPosId, DateTime.Now);
                VMall.Core.BMACache.Insert(CacheKeys.MALL_ADVERT_LIST + adPosId, advertList);
            }
            return advertList;
        }
    }
}
