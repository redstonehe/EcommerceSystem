using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using VMall.Core;
using System.Collections.Generic;

namespace VMall.Services
{
    /// <summary>
    /// 数据访问类:CashCouponDetail
    /// </summary>
    public partial class CashCouponDetail
    {
        public CashCouponDetail()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public static bool Exists(int DetailId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from hlh_CashCouponDetail");
            strSql.Append(" where DetailId=@DetailId");
            SqlParameter[] parameters = {
					new SqlParameter("@DetailId", SqlDbType.Int,4)
			};
            parameters[0].Value = DetailId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static int Add(CashCouponDetailInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into hlh_CashCouponDetail(");
            strSql.Append("CreationDate,CashId,Uid,DetailType,InAmount,OutAmount,Oid,OSN,DetailDes,Status,DirSaleUid)");
            strSql.Append(" values (");
            strSql.Append("@CreationDate,@CashId,@Uid,@DetailType,@InAmount,@OutAmount,@Oid,@OSN,@DetailDes,@Status,@DirSaleUid)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@CreationDate", SqlDbType.DateTime),
					new SqlParameter("@CashId", SqlDbType.Int,4),
					new SqlParameter("@Uid", SqlDbType.Int,4),
					new SqlParameter("@DetailType", SqlDbType.TinyInt,1),
					new SqlParameter("@InAmount", SqlDbType.Decimal,9),
					new SqlParameter("@OutAmount", SqlDbType.Decimal,9),
					new SqlParameter("@Oid", SqlDbType.Int,4),
					new SqlParameter("@OSN", SqlDbType.VarChar,30),
					new SqlParameter("@DetailDes", SqlDbType.NVarChar,300),
					new SqlParameter("@Status", SqlDbType.TinyInt,1),
                    new SqlParameter("@DirSaleUid", SqlDbType.Int,4)};
            parameters[0].Value = model.CreationDate;
            parameters[1].Value = model.CashId;
            parameters[2].Value = model.Uid;
            parameters[3].Value = model.DetailType;
            parameters[4].Value = model.InAmount;
            parameters[5].Value = model.OutAmount;
            parameters[6].Value = model.Oid;
            parameters[7].Value = model.OSN;
            parameters[8].Value = model.DetailDes;
            parameters[9].Value = model.Status;
            parameters[10].Value = model.DirSaleUid;
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters));

        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool Update(CashCouponDetailInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update hlh_CashCouponDetail set ");
            strSql.Append("CreationDate=@CreationDate,");
            strSql.Append("CashId=@CashId,");
            strSql.Append("Uid=@Uid,");
            strSql.Append("DetailType=@DetailType,");
            strSql.Append("InAmount=@InAmount,");
            strSql.Append("OutAmount=@OutAmount,");
            strSql.Append("Oid=@Oid,");
            strSql.Append("OSN=@OSN,");
            strSql.Append("DetailDes=@DetailDes,");
            strSql.Append("Status=@Status");
            strSql.Append(" where DetailId=@DetailId");
            SqlParameter[] parameters = {
					new SqlParameter("@CreationDate", SqlDbType.DateTime),
					new SqlParameter("@CashId", SqlDbType.Int,4),
					new SqlParameter("@Uid", SqlDbType.Int,4),
					new SqlParameter("@DetailType", SqlDbType.TinyInt,1),
					new SqlParameter("@InAmount", SqlDbType.Decimal,9),
					new SqlParameter("@OutAmount", SqlDbType.Decimal,9),
					new SqlParameter("@Oid", SqlDbType.Int,4),
					new SqlParameter("@OSN", SqlDbType.VarChar,30),
					new SqlParameter("@DetailDes", SqlDbType.NVarChar,300),
					new SqlParameter("@Status", SqlDbType.TinyInt,1),
					new SqlParameter("@DetailId", SqlDbType.Int,4)};
            parameters[0].Value = model.CreationDate;
            parameters[1].Value = model.CashId;
            parameters[2].Value = model.Uid;
            parameters[3].Value = model.DetailType;
            parameters[4].Value = model.InAmount;
            parameters[5].Value = model.OutAmount;
            parameters[6].Value = model.Oid;
            parameters[7].Value = model.OSN;
            parameters[8].Value = model.DetailDes;
            parameters[9].Value = model.Status;
            parameters[10].Value = model.DetailId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;

        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static bool Delete(int DetailId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_CashCouponDetail ");
            strSql.Append(" where DetailId=@DetailId");
            SqlParameter[] parameters = {
					new SqlParameter("@DetailId", SqlDbType.Int,4)
			};
            parameters[0].Value = DetailId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;

        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public static bool DeleteList(string DetailIdlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_CashCouponDetail ");
            strSql.Append(" where DetailId in (" + DetailIdlist + ")  ");
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static CashCouponDetailInfo GetModel(int DetailId)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_CashCouponDetail ");
            strSql.Append(" where DetailId=@DetailId");
            SqlParameter[] parameters = {
					new SqlParameter("@DetailId", SqlDbType.Int,4)
			};
            parameters[0].Value = DetailId;

            CashCouponDetailInfo model = new CashCouponDetailInfo();
            DataSet ds = RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static CashCouponDetailInfo GetModel(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_CashCouponDetail ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }

            CashCouponDetailInfo model = new CashCouponDetailInfo();
            DataSet ds = RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static CashCouponDetailInfo DataRowToModel(DataRow row)
        {
            CashCouponDetailInfo model = new CashCouponDetailInfo();
            if (row != null)
            {
                if (row["DetailId"] != null && row["DetailId"].ToString() != "")
                {
                    model.DetailId = int.Parse(row["DetailId"].ToString());
                }
                if (row["CreationDate"] != null && row["CreationDate"].ToString() != "")
                {
                    model.CreationDate = DateTime.Parse(row["CreationDate"].ToString());
                }
                if (row["CashId"] != null && row["CashId"].ToString() != "")
                {
                    model.CashId = int.Parse(row["CashId"].ToString());
                }
                if (row["Uid"] != null && row["Uid"].ToString() != "")
                {
                    model.Uid = int.Parse(row["Uid"].ToString());
                }
                if (row["DetailType"] != null && row["DetailType"].ToString() != "")
                {
                    model.DetailType = int.Parse(row["DetailType"].ToString());
                }
                if (row["InAmount"] != null && row["InAmount"].ToString() != "")
                {
                    model.InAmount = decimal.Parse(row["InAmount"].ToString());
                }
                if (row["OutAmount"] != null && row["OutAmount"].ToString() != "")
                {
                    model.OutAmount = decimal.Parse(row["OutAmount"].ToString());
                }
                if (row["Oid"] != null && row["Oid"].ToString() != "")
                {
                    model.Oid = int.Parse(row["Oid"].ToString());
                }
                if (row["OSN"] != null)
                {
                    model.OSN = row["OSN"].ToString();
                }
                if (row["DetailDes"] != null)
                {
                    model.DetailDes = row["DetailDes"].ToString();
                }
                if (row["Status"] != null && row["Status"].ToString() != "")
                {
                    model.Status = int.Parse(row["Status"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public static List<CashCouponDetailInfo> GetList(string strWhere)
        {
            List<CashCouponDetailInfo> list = new List<CashCouponDetailInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM hlh_CashCouponDetail ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    CashCouponDetailInfo info = BuildCashCouponDetailInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
            //return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString());
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public static List<CashCouponDetailInfo> GetList(int Top, string strWhere, string filedOrder)
        {
            List<CashCouponDetailInfo> list = new List<CashCouponDetailInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM hlh_CashCouponDetail ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    CashCouponDetailInfo info = BuildCashCouponDetailInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
            // return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public static int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_CashCouponDetail ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));

        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public static List<CashCouponDetailInfo> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            List<CashCouponDetailInfo> list = new List<CashCouponDetailInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.DetailId desc");
            }
            strSql.Append(")AS Row, T.*  from hlh_CashCouponDetail T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    CashCouponDetailInfo info = BuildCashCouponDetailInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
            //return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString());
        }


        /// <summary>
        /// 从IDataReader创建ProductInfo
        /// </summary>
        public static CashCouponDetailInfo BuildCashCouponDetailInfoReader(IDataReader row)
        {
            CashCouponDetailInfo model = new CashCouponDetailInfo();
            if (row != null)
            {
                if (row["DetailId"] != null && row["DetailId"].ToString() != "")
                {
                    model.DetailId = int.Parse(row["DetailId"].ToString());
                }
                if (row["CreationDate"] != null && row["CreationDate"].ToString() != "")
                {
                    model.CreationDate = DateTime.Parse(row["CreationDate"].ToString());
                }
                if (row["CashId"] != null && row["CashId"].ToString() != "")
                {
                    model.CashId = int.Parse(row["CashId"].ToString());
                }
                if (row["Uid"] != null && row["Uid"].ToString() != "")
                {
                    model.Uid = int.Parse(row["Uid"].ToString());
                }
                if (row["DetailType"] != null && row["DetailType"].ToString() != "")
                {
                    model.DetailType = int.Parse(row["DetailType"].ToString());
                }
                if (row["InAmount"] != null && row["InAmount"].ToString() != "")
                {
                    model.InAmount = decimal.Parse(row["InAmount"].ToString());
                }
                if (row["OutAmount"] != null && row["OutAmount"].ToString() != "")
                {
                    model.OutAmount = decimal.Parse(row["OutAmount"].ToString());
                }
                if (row["Oid"] != null && row["Oid"].ToString() != "")
                {
                    model.Oid = int.Parse(row["Oid"].ToString());
                }
                if (row["OSN"] != null)
                {
                    model.OSN = row["OSN"].ToString();
                }
                if (row["DetailDes"] != null)
                {
                    model.DetailDes = row["DetailDes"].ToString();
                }
                if (row["Status"] != null && row["Status"].ToString() != "")
                {
                    model.Status = int.Parse(row["Status"].ToString());
                }
                if (row["DirSaleUid"] != null && row["DirSaleUid"].ToString() != "")
                {
                    model.DirSaleUid = int.Parse(row["DirSaleUid"].ToString());
                }
            }
            return model;
        }
        /*
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public static DataSet GetList(int PageSize,int PageIndex,string strWhere)
        {
            SqlParameter[] parameters = {
                    new SqlParameter("@tblName", SqlDbType.VarChar, 255),
                    new SqlParameter("@fldName", SqlDbType.VarChar, 255),
                    new SqlParameter("@PageSize", SqlDbType.Int),
                    new SqlParameter("@PageIndex", SqlDbType.Int),
                    new SqlParameter("@IsReCount", SqlDbType.Bit),
                    new SqlParameter("@OrderType", SqlDbType.Bit),
                    new SqlParameter("@strWhere", SqlDbType.VarChar,1000),
                    };
            parameters[0].Value = "hlh_CashCouponDetail";
            parameters[1].Value = "DetailId";
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = 0;
            parameters[5].Value = 0;
            parameters[6].Value = strWhere;	
            return DbHelperSQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
        }*/

        #endregion  BasicMethod
        #region  ExtensionMethod

        #endregion  ExtensionMethod
    }
}

