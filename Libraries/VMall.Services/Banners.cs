using System;

using VMall.Core;

namespace VMall.Services
{
    /// <summary>
    /// banner操作管理类
    /// </summary>
    public partial class Banners
    {
        /// <summary>
        /// 获得首页banner列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static BannerInfo[] GetHomeBannerList(int type)
        {
            BannerInfo[] bannerList = VMall.Core.BMACache.Get(CacheKeys.MALL_BANNER_HOMELIST + type) as BannerInfo[];
            if (bannerList == null)
            {
                bannerList = VMall.Data.Banners.GetHomeBannerList(type, DateTime.Now);
                VMall.Core.BMACache.Insert(CacheKeys.MALL_BANNER_HOMELIST + type, bannerList);
            }
            return bannerList;
        }
    }
}
