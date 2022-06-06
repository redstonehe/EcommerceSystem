using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

using VMall.Core;

namespace VMall.RDBSStrategy.SqlServer
{
    /// <summary>
    /// SqlServer策略之直销分部类
    /// </summary>
    public partial class RDBSStrategy : IRDBSStrategy
    {
        /// <summary>
        /// 直销会员登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sessionId"></param>
        /// <param name="passwordType"></param>
        /// <param name="userCodeType"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public DataTable CheckLogin(string userName, string password, string sessionId, string loginIp, string loginAddress, int passwordType, int userCodeType)
        {
            DbParameter[] parms = {
                                      GenerateInParam("@LoginName", SqlDbType.NVarChar, 20, userName),
                                      GenerateInParam("@Password", SqlDbType.NVarChar, 50, password),
                                      GenerateInParam("@SessionId", SqlDbType.NVarChar, 25, sessionId),
                                      GenerateInParam("@LoginIp", SqlDbType.NVarChar, 20, loginIp),
                                      GenerateInParam("@LoginAddress", SqlDbType.NVarChar, 50, loginAddress),
                                      GenerateInParam("@PasswordType", SqlDbType.Int, 4, passwordType),
                                      GenerateInParam("@UserCodeType", SqlDbType.Int, 4, userCodeType)
                                  };

            return RDBSHelper.ExecuteDataset(RDBSHelper.DirSaleConnectionString, CommandType.StoredProcedure, "LoginCheck", parms).Tables[0];
        }

        /// <summary>
        /// 获取直销会员信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public IDataReader GetDirSaleUserById(int uid)
        {
            DbParameter[] parms = { GenerateInParam("@userId", SqlDbType.Int, 4, uid) };
            string commandText = "SELECT * FROM [UserInfo] WHERE [UserId]=@userId";

            return RDBSHelper.ExecuteReader(RDBSHelper.DirSaleConnectionString, CommandType.Text, commandText, parms);
        }
    }
}
