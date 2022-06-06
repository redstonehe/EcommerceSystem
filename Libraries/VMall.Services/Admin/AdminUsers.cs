using System;
using System.Data;

using VMall.Core;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace VMall.Services
{
    /// <summary>
    /// 后台用户操作管理类
    /// </summary>
    public partial class AdminUsers : Users
    {
        /// <summary>
        /// 后台获得用户列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetUserList(int pageSize, int pageNumber, string condition, string sort,string realName="")
        {
            return VMall.Data.Users.AdminGetUserList(pageSize, pageNumber, condition, sort, realName);
        }

        /// <summary>
        /// 后台获得用户列表条件
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="email">邮箱</param>
        /// <param name="mobile">手机</param>
        /// <param name="userRid">用户等级</param>
        /// <param name="mallAGid">商城管理员组</param>
        /// <returns></returns>
        public static string AdminGetUserListCondition(string userName, string email, string mobile, int userRid, int mallAGid, int userType = -1)
        {
            return VMall.Data.Users.AdminGetUserListCondition(userName, email, mobile, userRid, mallAGid, userType);
        }

        /// <summary>
        /// 后台获得用户列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetUserListSort(string sortColumn, string sortDirection)
        {
            return VMall.Data.Users.AdminGetUserListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得用户列表数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetUserCount(string condition,string realName="")
        {
            return VMall.Data.Users.AdminGetUserCount(condition, realName);
        }

        /// <summary>
        /// 设置店铺管理员
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="storeId">店铺id</param>
        public static void SetStoreAdminer(int uid, int storeId)
        {
            VMall.Data.Users.SetStoreAdminer(uid, storeId);
        }

        /// <summary>
        /// 撤销店铺管理员
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="storeId">店铺id</param>
        public static bool RemoveStoreAdminer(int uid)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid",uid)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [storeid]=0 WHERE [uid]=@uid;",
                                                RDBSHelper.RDBSTablePre);
           return  RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms)>0;
        }
        /// <summary>
        /// 获得管理员组的用户列表
        /// </summary>
        /// <returns></returns>
        public static List<PartUserInfo> GetAdminGroupUserList(int mallAGid)
        {
            List<PartUserInfo> userlist = new List<PartUserInfo>();
            string strSql =string.Format( "SELECT * FROM dbo.hlh_users WHERE mallagid={0}",mallAGid);

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    PartUserInfo info = VMall.Data.Users.BuildPartUserFromReader(reader);
                    userlist.Add(info);
                }
                reader.Close();
            }
            return userlist;
        }
        /// <summary>
        /// 后台获得店铺的管理员列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable GetStoreAdminUserList(int pageSize, int pageNumber, string condition, string sort, string realName = "")
        {
            bool noCondition = string.IsNullOrWhiteSpace(condition);
            string strWhere = " 1=1 ";
            if (!string.IsNullOrEmpty(realName))
                strWhere += string.Format(" AND b.[realname] like '%{1}%'", RDBSHelper.RDBSTablePre, realName);
            string commandText;
            commandText = string.Format(@"with tb as(
 select a.*,b.uid AS userid,b.lastvisittime,b.lastvisitip,b.lastvisitrgid,b.registerip,b.registerrgid,b.gender,realname,b.bday,b.idcard,b.regionid,b.address,b.bio,b.registertime ,c.title,ROW_NUMBER() OVER (ORDER BY a.[uid] DESC) AS RowNumber from [hlh_users] as a left join 
 hlh_userdetails as b on a.uid=b.uid left join hlh_malladmingroups c ON c.[mallagid]=a.[mallagid]
 WHERE 1=1 {0} AND {1} )
 select * from tb where RowNumber between (" + pageNumber + @"-1)*" + pageSize + @"+1 and (" + pageNumber + @")*" + pageSize + " order by tb.[uid] desc", noCondition ? "" : " AND " + condition, strWhere);
            
            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText).Tables[0];
        }
    }
}
