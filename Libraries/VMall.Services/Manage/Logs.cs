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
    //hlh_Logs
    public partial class Logs : BaseDAL<LogsInfo>
    {
        /// <summary>
        /// 批量删除一批数据
        /// </summary>
        public bool DeleteList(string Idlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_Logs ");
            strSql.Append(" where Id in (" + Idlist + ")  ");
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
        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int AdminGetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_Logs T LEFT JOIN hlh_users b on T.Operator=b.uid inner join hlh_userdetails ud on ud.uid=T.Operator ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
        }

        /// <summary>
        /// 后台分页获取数据列表
        /// </summary>
        public DataTable AdminGetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            //List<CashCouponInfo> list = new List<CashCouponInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.Id desc");
            }
            strSql.Append(")AS Row, T.*,b.username,b.email,b.mobile,ud.realname from hlh_Logs T LEFT JOIN hlh_users b on T.Operator=b.uid  inner join hlh_userdetails ud on ud.uid=T.Operator");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);

            return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString()).Tables[0];

        }

    }
}