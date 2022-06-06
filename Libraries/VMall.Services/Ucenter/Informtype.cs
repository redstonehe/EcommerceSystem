using System;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using Maticsoft.DBUtility;
using DAL.Base;
using VMall.Core;
namespace VMall.Services
{
    //消息类型表
    public partial class Informtype : BaseDAL<InformtypeInfo>
    {
        /// <summary>
        /// 批量删除一批数据
        /// </summary>
        public bool DeleteList(string typeidlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_informtype ");
            strSql.Append(" where typeid in (" + typeidlist + ")  ");
            int rows = DbHelperSQL.ExecuteSql(strSql.ToString());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}