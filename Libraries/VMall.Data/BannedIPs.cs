using System;
using System.Data;
using System.Collections.Generic;

using VMall.Core;

namespace VMall.Data
{
    /// <summary>
    /// 禁止IP数据访问类
    /// </summary>
    public partial class BannedIPs
    {
        /// <summary>
        /// 获得禁止的ip列表
        /// </summary>
        /// <returns></returns>
        public static HashSet<string> GetBannedIPList()
        {
            HashSet<string> ipList = new HashSet<string>();
            IDataReader reader = VMall.Core.BMAData.RDBS.GetBannedIPList();
            while (reader.Read())
            {
                ipList.Add(reader["ip"].ToString());
            }
            reader.Close();
            return ipList;
        }

        /// <summary>
        /// 获得禁止的ip
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public static BannedIPInfo GetBannedIPById(int id)
        {
            BannedIPInfo bannedIPInfo = null;
            IDataReader reader = VMall.Core.BMAData.RDBS.GetBannedIPById(id);
            if (reader.Read())
            {
                bannedIPInfo = new BannedIPInfo();
                bannedIPInfo.Id = TypeHelper.ObjectToInt(reader["id"]);
                bannedIPInfo.IP = reader["ip"].ToString();
                bannedIPInfo.LiftBanTime = TypeHelper.ObjectToDateTime(reader["liftbantime"]);
            }

            reader.Close();
            return bannedIPInfo;
        }

        /// <summary>
        /// 获得禁止IP的id
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public static int GetBannedIPIdByIP(string ip)
        {
            return VMall.Core.BMAData.RDBS.GetBannedIPIdByIP(ip);
        }

        /// <summary>
        /// 添加禁止的ip
        /// </summary>
        public static void AddBannedIP(BannedIPInfo bannedIPInfo)
        {
            VMall.Core.BMAData.RDBS.AddBannedIP(bannedIPInfo);
        }

        /// <summary>
        /// 更新禁止的ip
        /// </summary>
        public static void UpdateBannedIP(BannedIPInfo bannedIPInfo)
        {
            VMall.Core.BMAData.RDBS.UpdateBannedIP(bannedIPInfo);
        }

        /// <summary>
        /// 删除禁止的ip
        /// </summary>
        /// <param name="idList">id列表</param>
        public static void DeleteBannedIPById(string idList)
        {
            VMall.Core.BMAData.RDBS.DeleteBannedIPById(idList);
        }

        /// <summary>
        /// 后台获得禁止的ip列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="ip">ip</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetBannedIPList(int pageSize, int pageNumber, string ip, string sort)
        {
            return VMall.Core.BMAData.RDBS.AdminGetBannedIPList(pageSize, pageNumber, ip, sort);
        }

        /// <summary>
        /// 后台获得禁止的ip列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetBannedIPListSort(string sortColumn, string sortDirection)
        {
            return VMall.Core.BMAData.RDBS.AdminGetBannedIPListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得禁止的ip数量
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public static int AdminGetBannedIPCount(string ip)
        {
            return VMall.Core.BMAData.RDBS.AdminGetBannedIPCount(ip);
        }
    }
}
