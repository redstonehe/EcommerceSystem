﻿using System;
using System.Data;

using VMall.Core;

namespace VMall.Services
{
    /// <summary>
    /// 后台活动专题操作管理类
    /// </summary>
    public partial class AdminTopic : Topics
    {
        /// <summary>
        /// 创建活动专题
        /// </summary>
        /// <param name="topicInfo">活动专题信息</param>
        public static void CreateTopic(TopicInfo topicInfo)
        {
            VMall.Data.Topics.CreateTopic(topicInfo);
        }

        /// <summary>
        /// 更新活动专题
        /// </summary>
        /// <param name="topicInfo">活动专题信息</param>
        public static void UpdateTopic(TopicInfo topicInfo)
        {
            VMall.Data.Topics.UpdateTopic(topicInfo);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_TOPIC_INFO + topicInfo.TopicId);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_TOPIC_INFO + topicInfo.SN);
        }

        /// <summary>
        /// 删除活动专题
        /// </summary>
        /// <param name="topicId">活动专题id</param>
        public static void DeleteTopicById(int topicId)
        {
            TopicInfo topicInfo = AdminGetTopicById(topicId);
            if (topicInfo != null)
            {
                VMall.Data.Topics.DeleteTopicById(topicId);
                VMall.Core.BMACache.Remove(CacheKeys.MALL_TOPIC_INFO + topicInfo.TopicId);
                VMall.Core.BMACache.Remove(CacheKeys.MALL_TOPIC_INFO + topicInfo.SN);
            }
        }

        /// <summary>
        /// 后台获得活动专题
        /// </summary>
        /// <param name="topicId">活动专题id</param>
        /// <returns></returns>
        public static TopicInfo AdminGetTopicById(int topicId)
        {
            if (topicId > 0)
                return VMall.Data.Topics.AdminGetTopicById(topicId);
            return null;
        }

        /// <summary>
        /// 后台获得活动专题列表条件
        /// </summary>
        /// <param name="sn">编号</param>
        /// <param name="title">标题</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static string AdminGetTopicListCondition(string sn, string title, string startTime, string endTime)
        {
            return VMall.Data.Topics.AdminGetTopicListCondition(sn, title, startTime, endTime);
        }

        /// <summary>
        /// 后台获得活动专题列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetTopicListSort(string sortColumn, string sortDirection)
        {
            return VMall.Data.Topics.AdminGetTopicListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得活动专题列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetTopicList(int pageSize, int pageNumber, string condition, string sort)
        {
            return VMall.Data.Topics.AdminGetTopicList(pageSize, pageNumber, condition, sort);
        }

        /// <summary>
        /// 后台获得活动专题数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetTopicCount(string condition)
        {
            return VMall.Data.Topics.AdminGetTopicCount(condition);
        }
    }
}
