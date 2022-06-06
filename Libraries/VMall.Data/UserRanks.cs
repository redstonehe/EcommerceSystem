using System;
using System.Data;
using System.Collections.Generic;

using VMall.Core;

namespace VMall.Data
{
    /// <summary>
    /// 用户等级数据访问类
    /// </summary>
    public partial class UserRanks
    {
        /// <summary>
        /// 获得用户等级列表
        /// </summary>
        /// <returns></returns>
        public static List<UserRankInfo> GetUserRankList()
        {
            List<UserRankInfo> userRankList = new List<UserRankInfo>();
            IDataReader reader = VMall.Core.BMAData.RDBS.GetUserRankList();
            while(reader.Read())
            {
                UserRankInfo userRankInfo = new UserRankInfo();
                userRankInfo.UserRid = TypeHelper.ObjectToInt(reader["userrid"]);
                userRankInfo.System = TypeHelper.ObjectToInt(reader["system"]);
                userRankInfo.Title = reader["title"].ToString();
                userRankInfo.Avatar = reader["avatar"].ToString();
                userRankInfo.CreditsLower = TypeHelper.ObjectToInt(reader["creditslower"]);
                userRankInfo.CreditsUpper = TypeHelper.ObjectToInt(reader["creditsupper"]);
                userRankInfo.LimitDays = TypeHelper.ObjectToInt(reader["limitdays"]);
                userRankList.Add(userRankInfo);
            }
            reader.Close();
            return userRankList;
        }

        /// <summary>
        /// 创建用户等级
        /// </summary>
        public static void CreateUserRank(UserRankInfo userRankInfo)
        {
            VMall.Core.BMAData.RDBS.CreateUserRank(userRankInfo);
        }

        /// <summary>
        /// 删除用户等级
        /// </summary>
        /// <param name="userRid">用户等级id</param>
        public static void DeleteUserRankById(int userRid)
        {
            VMall.Core.BMAData.RDBS.DeleteUserRankById(userRid);
        }

        /// <summary>
        /// 更新用户等级
        /// </summary>
        public static void UpdateUserRank(UserRankInfo userRankInfo)
        {
            VMall.Core.BMAData.RDBS.UpdateUserRank(userRankInfo);
        }
    }
}
