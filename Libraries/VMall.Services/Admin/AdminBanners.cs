using System;
using System.Collections.Generic;

using VMall.Core;
using System.Data;

namespace VMall.Services
{
    /// <summary>
    /// 后台banner操作管理类
    /// </summary>
    public partial class AdminBanners : Banners
    {
        /// <summary>
        /// 后台获得banner列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public static List<BannerInfo> AdminGetBannerList(int pageSize, int pageNumber, string title, int type)
        {
            return VMall.Data.Banners.AdminGetBannerList(pageSize, pageNumber,title,type);
        }

        /// <summary>
        /// 后台获得banner数量
        /// </summary>
        /// <returns></returns>
        public static int AdminGetBannerCount(string title, int type)
        {
            return VMall.Data.Banners.AdminGetBannerCount(title,type);
        }

        /// <summary>
        /// 后台获得banner
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public static BannerInfo AdminGetBannerById(int id)
        {
            if (id > 0)
                return VMall.Data.Banners.AdminGetBannerById(id);
            return null;
        }

        /// <summary>
        /// 创建banner
        /// </summary>
        public static void CreateBanner(BannerInfo bannerInfo)
        {
            VMall.Data.Banners.CreateBanner(bannerInfo);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_BANNER_HOMELIST + bannerInfo.Type);
        }

        /// <summary>
        /// 更新banner
        /// </summary>
        public static void UpdateBanner(BannerInfo bannerInfo)
        {
            VMall.Data.Banners.UpdateBanner(bannerInfo);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_BANNER_HOMELIST + bannerInfo.Type);
        }

        /// <summary>
        /// 删除banner
        /// </summary>
        /// <param name="idList">id列表</param>
        public static void DeleteBannerById(int[] idList)
        {
            if (idList != null && idList.Length > 0)
            {
                VMall.Data.Banners.DeleteBannerById(CommonHelper.IntArrayToString(idList));
                VMall.Core.BMACache.Remove(CacheKeys.MALL_BANNER_HOMELIST + "0");
                VMall.Core.BMACache.Remove(CacheKeys.MALL_BANNER_HOMELIST + "1");
            }
        }
        /// <summary>
        /// 批量显示banner
        /// </summary>
        /// <param name="idList">id列表</param>
        public static void ShowBannerById(int[] idList)
        {
            if (idList != null && idList.Length > 0)
            {
                string commandText = String.Format("UPDATE [{0}banners] SET [isshow]=1 WHERE [id] IN ({1})",
                                                RDBSHelper.RDBSTablePre,
                                                CommonHelper.IntArrayToString(idList));
                RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText);
                
                VMall.Core.BMACache.Remove(CacheKeys.MALL_BANNER_HOMELIST + "0");
                VMall.Core.BMACache.Remove(CacheKeys.MALL_BANNER_HOMELIST + "1");
            }
        }
    }
}
