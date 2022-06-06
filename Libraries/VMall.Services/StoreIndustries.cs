using System;
using System.Collections.Generic;

using VMall.Core;

namespace VMall.Services
{
    /// <summary>
    /// 店铺行业操作管理类
    /// </summary>
    public partial class StoreIndustries
    {
        /// <summary>
        /// 获得店铺行业列表
        /// </summary>
        public static List<StoreIndustryInfo> GetStoreIndustryList()
        {
            List<StoreIndustryInfo> storeIndustryList = VMall.Core.BMACache.Get(CacheKeys.MALL_STORE_INDUSTRYLIST) as List<StoreIndustryInfo>;
            if (storeIndustryList == null)
            {
                storeIndustryList = VMall.Data.StoreIndustries.GetStoreIndustryList();
                VMall.Core.BMACache.Insert(CacheKeys.MALL_STORE_INDUSTRYLIST, storeIndustryList);
            }
            return storeIndustryList;
        }

        /// <summary>
        /// 获得店铺行业
        /// </summary>
        /// <param name="storeIid">店铺行业id</param>
        /// <returns></returns>
        public static StoreIndustryInfo GetStoreIndustryById(int storeIid)
        {
            foreach (StoreIndustryInfo storeIndustryInfo in GetStoreIndustryList())
            {
                if (storeIndustryInfo.StoreIid == storeIid)
                    return storeIndustryInfo;
            }
            return null;
        }

        /// <summary>
        /// 获得店铺行业id
        /// </summary>
        /// <param name="title">店铺行业标题</param>
        /// <returns></returns>
        public static int GetStoreIidByTitle(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                foreach (StoreIndustryInfo storeIndustryInfo in GetStoreIndustryList())
                {
                    if (storeIndustryInfo.Title == title)
                        return storeIndustryInfo.StoreIid;
                }
            }
            return -1;
        }
    }
}
