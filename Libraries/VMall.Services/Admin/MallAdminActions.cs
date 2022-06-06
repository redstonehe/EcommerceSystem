using System;
using System.Collections.Generic;

using VMall.Core;
using System.Data.SqlClient;
using System.Data;

namespace VMall.Services
{
    public partial class MallAdminActions
    {
        /// <summary>
        /// 获得商城后台操作列表
        /// </summary>
        /// <returns></returns>
        public static List<MallAdminActionInfo> GetMallAdminActionList()
        {
            return VMall.Data.MallAdminActions.GetMallAdminActionList();
        }

        /// <summary>
        /// 获得商城后台操作树
        /// </summary>
        /// <returns></returns>
        public static List<MallAdminActionInfo> GetMallAdminActionTree()
        {
            List<MallAdminActionInfo> mallAdminActionTree = new List<MallAdminActionInfo>();
            List<MallAdminActionInfo> mallAdminActionList = GetMallAdminActionList();
            CreateMallAdminActionTree(mallAdminActionList, mallAdminActionTree, 0);
            return mallAdminActionTree;
        }

        /// <summary>
        /// 递归创建商城后台操作树
        /// </summary>
        private static void CreateMallAdminActionTree(List<MallAdminActionInfo> mallAdminActionList, List<MallAdminActionInfo> mallAdminActionTree, int parentId)
        {
            foreach (MallAdminActionInfo mallAdminActionInfo in mallAdminActionList)
            {
                if (mallAdminActionInfo.ParentId == parentId)
                {
                    mallAdminActionTree.Add(mallAdminActionInfo);
                    CreateMallAdminActionTree(mallAdminActionList, mallAdminActionTree, mallAdminActionInfo.Aid);
                }
            }
        }

        /// <summary>
        /// 获得商城后台操作HashSet
        /// </summary>
        /// <returns></returns>
        public static HashSet<string> GetMallAdminActionHashSet()
        {
            HashSet<string> actionHashSet = VMall.Core.BMACache.Get(CacheKeys.MALL_MALLADMINACTION_HASHSET) as HashSet<string>;
            if (actionHashSet == null)
            {
                actionHashSet = new HashSet<string>();
                List<MallAdminActionInfo> mallAdminActionList = GetMallAdminActionList();
                foreach (MallAdminActionInfo mallAdminActionInfo in mallAdminActionList)
                {
                    if (mallAdminActionInfo.ParentId != 0 && mallAdminActionInfo.Action != string.Empty)
                        actionHashSet.Add(mallAdminActionInfo.Action);
                }
                VMall.Core.BMACache.Insert(CacheKeys.MALL_MALLADMINACTION_HASHSET, actionHashSet);
            }
            return actionHashSet;
        }



        /// <summary>
        /// 获得商城菜单id
        /// </summary>
        /// <param name="title">商城菜单标题</param>
        /// <returns></returns>
        public static int GetMallAdminActionIdByTitle(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                foreach (MallAdminActionInfo mallAdminGroupInfo in GetMallAdminActionList())
                {
                    if (mallAdminGroupInfo.Title.Trim() == title.Trim())
                        return mallAdminGroupInfo.Aid;
                }
            }
            return -1;
        }

        /// <summary>
        /// 创建商城菜单
        /// </summary>
        /// <param name="mallAdminGroupInfo">商城菜单信息</param>
        /// <returns></returns>
        public static int CreateMallAdminAction(MallAdminActionInfo model)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@title",model.Title),
                                       new SqlParameter("@action",model.Action),
                                       new SqlParameter("@parentid",model.ParentId),
                                       new SqlParameter("@displayorder",model.DisplayOrder)
                                   };

            string commandText = string.Format("INSERT INTO [{0}malladminactions]([title],[action],[parentid],[displayorder]) VALUES(@title,@action,@parentid,@displayorder);SELECT SCOPE_IDENTITY();", RDBSHelper.RDBSTablePre);
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms));
        }
        /// <summary>
        /// 更新商城管理员组
        /// </summary>
        public static void UpdateMallAdminAction(MallAdminActionInfo model)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@title",model.Title),
                                       new SqlParameter("@action",model.Action),
                                       new SqlParameter("@parentid",model.ParentId),
                                       new SqlParameter("@displayorder",model.DisplayOrder),
                                       new SqlParameter("@aid",model.Aid),
                                   };

            string commandText = string.Format("UPDATE [{0}malladminactions] SET [title]=@title,[action]=@action,[parentid]=@parentid,[displayorder]=@displayorder WHERE [aid]=@aid",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 获得菜单
        /// </summary>
        /// <param name="mallAGid">商城菜单id</param>
        /// <returns></returns>
        public static MallAdminActionInfo GetMallAdminActionById(int aid)
        {
            foreach (MallAdminActionInfo info in GetMallAdminActionList())
            {
                if (aid == info.Aid)
                    return info;
            }
            return null;
        }
        /// <summary>
        /// 获得菜单
        /// </summary>
        /// <param name="mallAGid">商城菜单id</param>
        /// <returns></returns>
        public static MallAdminActionInfo GetMallAdminActionByActionName(string action)
        {
            MallAdminActionInfo mallAdminGroupInfo = null;
            if (!string.IsNullOrWhiteSpace(action))
            {
                mallAdminGroupInfo = GetMallAdminActionList().Find(x => x.Action.Trim() == action.Trim());

                return mallAdminGroupInfo;

            }
            return null;
        }
    }
}
