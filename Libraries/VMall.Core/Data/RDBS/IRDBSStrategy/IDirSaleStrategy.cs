using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace VMall.Core
{
    /// <summary>
    /// VMall关系数据库策略之直销分部接口
    /// </summary>
    public partial interface IRDBSStrategy
    {
        /// <summary>
        /// 直销会员登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sessionId"></param>
        /// <param name="loginIp"></param>
        /// <param name="loginAddress"></param>
        /// <param name="passwordType"></param>
        /// <param name="userCodeType"></param>
        /// <returns></returns>
        DataTable CheckLogin(string userName, string password, string sessionId, string loginIp, string loginAddress, int passwordType, int userCodeType);

        /// <summary>
        /// 获得直销会员信息
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        IDataReader GetDirSaleUserById(int uid);
    }
}
