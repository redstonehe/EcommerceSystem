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
    //hlh_kftransfer2severb
    public partial class Kftransfer2severb : BaseDAL<Kftransfer2severbInfo>
    {
        /// <summary>
        /// 批量删除一批数据
        /// </summary>
        public bool DeleteList(string Idlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_kftransfer2severb ");
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
        /// 增加一条数据
        /// </summary>
        public int AddModel(Kftransfer2severbInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into hlh_kftransfer2severb(");
            strSql.Append("ApplySN,CreateTime,Uid,Amount,CardNumber,CardType,CardUserName,CardMobile,Remark,AdminUid,HandleTime,State,HandleResult,LastModify)");
            strSql.Append(" values (");
            strSql.Append("@ApplySN,@CreateTime,@Uid,@Amount,@CardNumber,@CardType,@CardUserName,@CardMobile,@Remark,@AdminUid,@HandleTime,@State,@HandleResult,@LastModify)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@ApplySN", SqlDbType.NChar,30),
					new SqlParameter("@CreateTime", SqlDbType.DateTime),
					new SqlParameter("@Uid", SqlDbType.Int,4),
					new SqlParameter("@Amount", SqlDbType.Decimal,10),
                    new SqlParameter("@CardNumber", SqlDbType.NChar,200),
                    new SqlParameter("@CardType", SqlDbType.TinyInt,1),
                    new SqlParameter("@CardUserName", SqlDbType.NVarChar,20),
                    new SqlParameter("@CardMobile", SqlDbType.NChar,15),
                    new SqlParameter("@Remark", SqlDbType.NVarChar,500),
                    new SqlParameter("@AdminUid", SqlDbType.Int,4),
                    new SqlParameter("@HandleTime", SqlDbType.DateTime),
                    new SqlParameter("@State", SqlDbType.TinyInt,1),
                    new SqlParameter("@HandleResult", SqlDbType.NVarChar,500),
                    new SqlParameter("@LastModify", SqlDbType.DateTime)};
            parameters[0].Value = model.ApplySN;
            parameters[1].Value = model.CreateTime;
            parameters[2].Value = model.Uid;
            parameters[3].Value = model.Amount;
            parameters[4].Value = model.CardNumber;
            parameters[5].Value = model.CardType;
            parameters[6].Value = model.CardUserName;
            parameters[7].Value = model.CardMobile;
            parameters[8].Value = model.Remark;
            parameters[9].Value = model.AdminUid;
            parameters[10].Value = model.HandleTime;
            parameters[11].Value = model.State;
            parameters[12].Value = model.HandleResult;
            parameters[13].Value = model.LastModify; 
            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int AdminGetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_kftransfer2severb T LEFT JOIN hlh_users b on T.uid=b.uid inner join hlh_userdetails ud on ud.uid=T.uid ");
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
            strSql.Append(")AS Row, T.*,b.username,b.email,b.mobile,ud.realname from hlh_kftransfer2severb T LEFT JOIN hlh_users b on T.uid=b.uid  inner join hlh_userdetails ud on ud.uid=t.uid");
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