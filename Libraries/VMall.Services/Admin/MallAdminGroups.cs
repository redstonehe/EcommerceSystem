using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using VMall.Core;
using System.Data;

namespace VMall.Services
{
    /// <summary>
    /// 商城管理员组操作管理类
    /// </summary>
    public partial class MallAdminGroups
    {
        //商城后台导航菜单栏缓存文件夹
        private const string MallAdminNavMeunCacheFolder = "/admin_mall/cache/menu";

        /// <summary>
        /// 检查当前动作的授权
        /// </summary>
        /// <param name="mallAGid">商城管理员组id</param>
        /// <param name="controller">控制器名称</param>
        /// <param name="action">动作方法名称</param>
        /// <returns></returns>
        public static bool CheckAuthority(int mallAGid, string controller, string pageKey)
        {
            //非管理员
            if (mallAGid == 1)
                return false;

            //系统管理员具有一切权限
            if (mallAGid == 8 || mallAGid == 2)
                return true;
            if(pageKey.Contains("SelectList") ||pageKey.Contains("selectlist"))
                return true;
            HashSet<string> mallAdminActionHashSet = MallAdminActions.GetMallAdminActionHashSet();
            HashSet<string> mallAdminGroupActionHashSet = GetMallAdminGroupActionHashSet(mallAGid);
            string pageName = pageKey.Remove(0, 1).Replace("/", "_");
            //动作方法的优先级大于控制器的优先级
            if ((mallAdminActionHashSet.Contains(pageName) && mallAdminGroupActionHashSet.Contains(pageName))
                || (  mallAdminActionHashSet.ToList().Exists(x => x.Contains(controller)) && mallAdminGroupActionHashSet.ToList().Exists(x => x.Contains(controller)))
                )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获得商城管理员组操作HashSet
        /// </summary>
        /// <param name="mallAGid">商城管理员组id</param>
        /// <returns></returns>
        public static HashSet<string> GetMallAdminGroupActionHashSet(int mallAGid)
        {
            HashSet<string> actionHashSet = VMall.Core.BMACache.Get(CacheKeys.MALL_MALLADMINGROUP_ACTIONHASHSET + mallAGid) as HashSet<string>;
            if (actionHashSet == null)
            {
                MallAdminGroupInfo mallAdminGroupInfo = GetMallAdminGroupById(mallAGid);
                if (mallAdminGroupInfo != null)
                {
                    actionHashSet = new HashSet<string>();
                    string[] actionList = StringHelper.SplitString(mallAdminGroupInfo.ActionList);//将动作列表字符串分隔成动作列表
                    foreach (string action in actionList)
                    {
                        actionHashSet.Add(action);
                    }
                    VMall.Core.BMACache.Insert(CacheKeys.MALL_MALLADMINGROUP_ACTIONHASHSET + mallAGid, actionHashSet);
                }
            }
            return actionHashSet;
        }

        /// <summary>
        /// 获得商城管理员组列表
        /// </summary>
        /// <returns></returns>
        public static MallAdminGroupInfo[] GetMallAdminGroupList()
        {
            MallAdminGroupInfo[] mallAdminGroupList = VMall.Core.BMACache.Get(CacheKeys.MALL_MALLADMINGROUP_LIST) as MallAdminGroupInfo[];
            if (mallAdminGroupList == null)
            {
                mallAdminGroupList = VMall.Data.MallAdminGroups.GetMallAdminGroupList();
                VMall.Core.BMACache.Insert(CacheKeys.MALL_MALLADMINGROUP_LIST, mallAdminGroupList);
            }
            return mallAdminGroupList;
        }

        /// <summary>
        /// 获得用户级商城管理员组列表
        /// </summary>
        /// <returns></returns>
        public static MallAdminGroupInfo[] GetCustomerMallAdminGroupList()
        {
            MallAdminGroupInfo[] mallAdminGroupList = GetMallAdminGroupList();
            MallAdminGroupInfo[] customerMallAdminGroupList = new MallAdminGroupInfo[mallAdminGroupList.Length - 2];

            int i = 0;
            foreach (MallAdminGroupInfo mallAdminGroupInfo in mallAdminGroupList)
            {
                if (mallAdminGroupInfo.MallAGid != 8)
                {
                    customerMallAdminGroupList[i] = mallAdminGroupInfo;
                    i++;
                }
            }

            return customerMallAdminGroupList;
        }

        /// <summary>
        /// 获得商城管理员组
        /// </summary>
        /// <param name="mallAGid">商城管理员组id</param>
        /// <returns></returns>
        public static MallAdminGroupInfo GetMallAdminGroupById(int mallAGid)
        {
            foreach (MallAdminGroupInfo mallAdminGroupInfo in GetMallAdminGroupList())
            {
                if (mallAGid == mallAdminGroupInfo.MallAGid)
                    return mallAdminGroupInfo;
            }
            return null;
        }

        /// <summary>
        /// 获得商城管理员组id
        /// </summary>
        /// <param name="title">商城管理员组标题</param>
        /// <returns></returns>
        public static int GetMallAdminGroupIdByTitle(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                foreach (MallAdminGroupInfo mallAdminGroupInfo in GetMallAdminGroupList())
                {
                    if (mallAdminGroupInfo.Title == title)
                        return mallAdminGroupInfo.MallAGid;
                }
            }
            return -1;
        }

        /// <summary>
        /// 创建管理员组
        /// </summary>
        /// <param name="mallAdminGroupInfo">管理员组信息</param>
        public static void CreateMallAdminGroup(MallAdminGroupInfo mallAdminGroupInfo)
        {
            mallAdminGroupInfo.ActionList = mallAdminGroupInfo.ActionList.ToLower();
            int mallAGid = VMall.Data.MallAdminGroups.CreateMallAdminGroup(mallAdminGroupInfo);
            if (mallAGid > 0)
            {
                //创建组默认操作权限值列表

                BatchInsertDefaultRights(mallAGid);
                VMall.Core.BMACache.Remove(CacheKeys.MALL_MALLADMINGROUP_LIST);
                mallAdminGroupInfo.MallAGid = mallAGid;
                WriteMallAdminNavMenuCache(mallAdminGroupInfo);
            }
        }

        /// <summary>
        /// 删除商城管理员组
        /// </summary>
        /// <param name="mallAGid">商城管理员组id</param>
        /// <returns>-2代表内置管理员不能删除，-1代表此管理员组下还有会员未删除，0代表删除失败，1代表删除成功</returns>
        public static int DeleteMallAdminGroupById(int mallAGid)
        {
            if (mallAGid < 3)
                return -2;

            if (AdminUsers.GetUserCountByMallAGid(mallAGid) > 0)
                return -1;

            if (mallAGid > 0)
            {
                VMall.Data.MallAdminGroups.DeleteMallAdminGroupById(mallAGid);
                VMall.Core.BMACache.Remove(CacheKeys.MALL_MALLADMINGROUP_ACTIONHASHSET + mallAGid);
                VMall.Core.BMACache.Remove(CacheKeys.MALL_MALLADMINGROUP_LIST);
                File.Delete(IOHelper.GetMapPath(MallAdminNavMeunCacheFolder + "/" + mallAGid + ".js"));
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 更新商城管理员组
        /// </summary>
        public static void UpdateMallAdminGroup(MallAdminGroupInfo mallAdminGroupInfo)
        {
            mallAdminGroupInfo.ActionList = mallAdminGroupInfo.ActionList.ToLower();
            VMall.Data.MallAdminGroups.UpdateMallAdminGroup(mallAdminGroupInfo);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_MALLADMINGROUP_ACTIONHASHSET + mallAdminGroupInfo.MallAGid);
            VMall.Core.BMACache.Remove(CacheKeys.MALL_MALLADMINGROUP_LIST);
            WriteMallAdminNavMenuCache(mallAdminGroupInfo);
        }

        static Dictionary<string, string> menuDict = new Dictionary<string, string>()
        { 
            { "商品管理", "fa-archive" },
            { "促销活动", "fa-expeditedssl" },
            { "订单管理", "fa-shopping-cart" },
            { "用户管理", "fa-user" },
            { "报单管理", "fa-list-alt" },
            { "商家店铺管理", "fa-cubes" },
            { "广告评价管理", "fa-laptop" },
            { "商城内容", "fa-comments-o" },
            { "报表统计", "fa-bar-chart" },
            { "系统设置", "fa-cogs" },
            { "日志管理", "fa-database" },
            { "开发管理", "fa-asterisk" },
            { "权限管理", "fa-group"},
            { "系统帮助", "fa-cutlery"}
        };

        /// <summary>
        /// 将商城管理员组的导航菜单栏缓存写入到文件中
        /// </summary>
        private static void WriteMallAdminNavMenuCache(MallAdminGroupInfo mallAdminGroupInfo)
        {
            //HashSet<ActionHelper> mallAdminGroupActionHashSet = new HashSet<ActionHelper>();
            List<MallAdminActionInfo> list = new List<MallAdminActionInfo>();
            string[] actionList = StringHelper.SplitString(mallAdminGroupInfo.ActionList);//将后台操作列表字符串分隔成后台操作列表

            foreach (string action in actionList)
            {
                if (!string.IsNullOrEmpty(action))
                {
                    MallAdminActionInfo actionInfo = MallAdminActions.GetMallAdminActionByActionName(action.Trim());

                    if (actionInfo != null)
                    {
                        if (!list.Exists(x => x.ParentId == actionInfo.ParentId))
                        {
                            MallAdminActionInfo ParentActionInfo = MallAdminActions.GetMallAdminActionById(actionInfo.ParentId);
                            list.Add(ParentActionInfo);
                        }
                    }
                    list.Add(actionInfo);
                }
            }


            StringBuilder menu = new StringBuilder();
            StringBuilder menuList = new StringBuilder("var menuList = [");
            List<MallAdminActionInfo> pList = list.FindAll(x => x.ParentId == 0).OrderBy(x => x.Aid).ToList();
            foreach (var info in pList)
            {
                menu.AppendFormat("{0}\"title\":\"{1}\",\"subMenuList\":[", "{", info.Title);
                foreach (var item in list.FindAll(x => x.ParentId == info.Aid).OrderBy(x => x.Aid))
                {
                    if (item.Action.Trim() != "order_orderinfo")
                        menu.AppendFormat("{0}\"title\":\"{1}\",\"url\":\"{2}\"{3},", "{", item.Title, "/malladmin/" + item.Action.Replace("_", "/"), "}");
                }
                menu.Remove(menu.Length - 1, 1);
                menu.AppendFormat("]," + "\"icon\":\"{0}\"", menuDict[info.Title]);
                menu.Append("},");
                


            }
            menuList.Append(menu.ToString());
            //#region 商品管理

            //menu.AppendFormat("{0}\"title\":\"商品管理\",\"subMenuList\":[", "{");
            //if (mallAdminGroupActionHashSet.Contains("product"))
            //{
            //    menu.AppendFormat("{0}\"title\":\"添加商品\",\"url\":\"/malladmin/product/addproduct\"{1},", "{", "}");
            //    menu.AppendFormat("{0}\"title\":\"添加SKU\",\"url\":\"/malladmin/product/addsku\"{1},", "{", "}");
            //    menu.AppendFormat("{0}\"title\":\"在售商品\",\"url\":\"/malladmin/product/onsaleproductlist\"{1},", "{", "}");
            //    menu.AppendFormat("{0}\"title\":\"下架商品\",\"url\":\"/malladmin/product/outsaleproductlist\"{1},", "{", "}");
            //    menu.AppendFormat("{0}\"title\":\"定时商品\",\"url\":\"/malladmin/product/timeproductlist\"{1},", "{", "}");
            //    menu.AppendFormat("{0}\"title\":\"回收站\",\"url\":\"/malladmin/product/recyclebinproductlist\"{1},", "{", "}");
            //    flag = true;
            //}
            //if (flag)
            //{
            //    menu.Remove(menu.Length - 1, 1);
            //    menu.Append("]},");
            //    menuList.Append(menu.ToString());
            //}

            //#endregion


            if (menuList.Length > 16)
                menuList.Remove(menuList.Length - 1, 1);
            menuList.Append("]");

            try
            {
                string fileName = IOHelper.GetMapPath(MallAdminNavMeunCacheFolder + "/" + mallAdminGroupInfo.MallAGid + ".js");
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    Byte[] info = System.Text.Encoding.UTF8.GetBytes(menuList.ToString());
                    fs.Write(info, 0, info.Length);
                    fs.Flush();
                    fs.Close();
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int BatchInsertDefaultRights(int mallAGid)
        {
            //INSERT INTO dbo.hlh_AdminOperateRights( Operate, Aid, MallAGid,State ) SELECT b.Operate,a.aid,c.mallagid,0 FROM dbo.hlh_malladminactions a  CROSS JOIN dbo.hlh_AdminOperate b CROSS JOIN dbo.hlh_malladmingroups c  WHERE a.parentid<>0 AND c.mallagid>1 AND a.aid=87 AND (b.aid=0 OR b.aid=a.aid)

            string strSql = string.Format(@"INSERT INTO dbo.hlh_AdminOperateRights( Operate, Aid, MallAGid,State ) SELECT b.Operate,a.aid,c.mallagid,0 FROM dbo.hlh_malladminactions a  CROSS JOIN dbo.hlh_AdminOperate b CROSS JOIN dbo.hlh_malladmingroups c  WHERE a.parentid<>0 AND c.mallagid>1  AND (b.aid=0 OR b.aid=a.aid) AND c.mallagid={0} ", mallAGid);
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql));
        }
    }
}
