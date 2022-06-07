﻿using System;
using System.Data;

using VMall.Core;

namespace VMall.Services
{
    /// <summary>
    /// 后台新闻操作管理类
    /// </summary>
    public partial class AdminNews : News
    {
        /// <summary>
        /// 创建新闻类型
        /// </summary>
        public static void CreateNewsType(NewsTypeInfo newsTypeInfo)
        {
            VMall.Data.News.CreateNewsType(newsTypeInfo);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_NEWSTYPE_LIST);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_NEWS_HOMELIST + "\\d+");
        }

        /// <summary>
        /// 删除新闻类型
        /// </summary>
        /// <param name="newsTypeId">新闻类型id</param>
        public static void DeleteNewsTypeById(int newsTypeId)
        {
            VMall.Data.News.DeleteNewsTypeById(newsTypeId);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_NEWSTYPE_LIST);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_NEWS_HOMELIST + "\\d+");
        }

        /// <summary>
        /// 更新新闻类型
        /// </summary>
        public static void UpdateNewsType(NewsTypeInfo newsTypeInfo)
        {
            VMall.Data.News.UpdateNewsType(newsTypeInfo);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_NEWSTYPE_LIST);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_NEWS_HOMELIST + "\\d+");
        }



        /// <summary>
        /// 创建新闻
        /// </summary>
        public static void CreateNews(NewsInfo newsInfo)
        {
            VMall.Data.News.CreateNews(newsInfo);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_NEWS_HOMELIST + newsInfo.NewsTypeId);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_NEWS_HOMELIST + "\\d+");
        }

        /// <summary>
        /// 删除新闻
        /// </summary>
        /// <param name="newsIdList">新闻id列表</param>
        public static void DeleteNewsById(int[] newsIdList)
        {
            if (newsIdList != null && newsIdList.Length > 0)
            {
                VMall.Data.News.DeleteNewsById(CommonHelper.IntArrayToString(newsIdList));
                VMall.Core.BMACache.Remove(CacheKeys.MALL_NEWS_HOMELIST + "\\d+");
            }
        }

        /// <summary>
        /// 更新新闻
        /// </summary>
        public static void UpdateNews(NewsInfo newsInfo)
        {
            VMall.Data.News.UpdateNews(newsInfo);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_NEWS_HOMELIST + newsInfo.NewsTypeId);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_NEWS_HOMELIST + "\\d+");
        }

        /// <summary>
        /// 后台获得新闻
        /// </summary>
        /// <param name="newsId">新闻id</param>
        /// <returns></returns>
        public static NewsInfo AdminGetNewsById(int newsId)
        {
            if (newsId > 0)
                return VMall.Data.News.AdminGetNewsById(newsId);
            return null;
        }

        /// <summary>
        /// 后台获得新闻列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetNewsList(int pageSize, int pageNumber, string condition, string sort)
        {
            return VMall.Data.News.AdminGetNewsList(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 后台获得新闻列表搜索条件
        /// </summary>
        /// <param name="newsTypeId">新闻类型id</param>
        /// <param name="title">新闻标题</param>
        /// <returns></returns>
        public static string AdminGetNewsListCondition(int newsTypeId, string title)
        {
            return VMall.Data.News.AdminGetNewsListCondition(newsTypeId, title);
        }

        /// <summary>
        /// 后台获得新闻列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetNewsListSort(string sortColumn, string sortDirection)
        {
            return VMall.Data.News.AdminGetNewsListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得新闻数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetNewsCount(string condition)
        {
            return VMall.Data.News.AdminGetNewsCount(condition);
        }

        /// <summary>
        /// 后台根据新闻标题得到新闻id
        /// </summary>
        /// <param name="title">新闻标题</param>
        /// <returns></returns>
        public static int AdminGetNewsIdByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return 0;
            return VMall.Data.News.AdminGetNewsIdByTitle(title);
        }
    }
}
