using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using VMall.Core;
using System.Collections.Generic;

namespace VMall.Services
{
    /// <summary>
    /// 数据访问类:RechargeProducts
    /// </summary>
    public partial class RechargeProducts
    {
        public RechargeProducts()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public static bool Exists(int PId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from hlh_rechargeproducts");
            strSql.Append(" where pid=@PId");
            SqlParameter[] parameters = {
					new SqlParameter("@PId", SqlDbType.Int,4)
			};
            parameters[0].Value = PId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static int Add(ProductEntity model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into hlh_rechargeproducts(");
            strSql.Append("[pid],[pname],[pvalue],[punit],[psubcate],[pchannel],[pareaid],[pcate],[pstock],[porder],[plimit],[pmarkprice])");
            strSql.Append(" values (");
            strSql.Append("@pid,@pname,@pvalue,@punit,@psubcate,@pchannel,@pareaid,@pcate,@pstock,@porder,@plimit,@pmarkprice)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@pid", SqlDbType.Int ,4),
					new SqlParameter("@pname", SqlDbType.NVarChar,100),
					new SqlParameter("@pvalue", SqlDbType.Float,4),
					new SqlParameter("@punit", SqlDbType.NVarChar,10),
					new SqlParameter("@psubcate", SqlDbType.Int,4),
					new SqlParameter("@pchannel", SqlDbType.Int,4),
					new SqlParameter("@pareaid", SqlDbType.Int,4),
					new SqlParameter("@pcate", SqlDbType.Int,4),
					new SqlParameter("@pstock", SqlDbType.Int,4),
					new SqlParameter("@porder", SqlDbType.Int,4),
					new SqlParameter("@plimit", SqlDbType.NVarChar,50),
					new SqlParameter("@pmarkprice", SqlDbType.Decimal,8)
            };
            parameters[0].Value = model.pid;
            parameters[1].Value = model.pname;
            parameters[2].Value = model.pvalue;
            parameters[3].Value = model.punit;
            parameters[4].Value = model.psubcate;
            parameters[5].Value = model.pchannel;
            parameters[6].Value = model.pareaid;
            parameters[7].Value = model.pcate;
            parameters[8].Value = model.pstock;
            parameters[9].Value = model.porder;
            parameters[10].Value = model.plimit;
            parameters[11].Value = model.pmarkprice;
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters));
        }
        ///// <summary>
        ///// 更新一条数据
        ///// </summary>
        //public static bool Update(ProductEntity model)
        //{
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("update hlh_rechargeproducts set ");
        //    strSql.Append("CashCouponSN=@CashCouponSN,");
        //    strSql.Append("CreationDate=@CreationDate,");
        //    strSql.Append("LastModifity=@LastModifity,");
        //    strSql.Append("CouponType=@CouponType,");
        //    strSql.Append("StoreId=@StoreId,");
        //    strSql.Append("ChannelId=@ChannelId,");
        //    strSql.Append("Uid=@Uid,");
        //    strSql.Append("CreateOid=@CreateOid,");
        //    strSql.Append("CreateOSN=@CreateOSN,");
        //    strSql.Append("CashAmount=@CashAmount,");
        //    strSql.Append("TotalIn=@TotalIn,");
        //    strSql.Append("TotalOut=@TotalOut,");
        //    strSql.Append("Banlance=@Banlance,");
        //    strSql.Append("ValidTime=@ValidTime");
        //    strSql.Append(" where CashId=@CashId");
        //    SqlParameter[] parameters = {
        //            new SqlParameter("@CashCouponSN", SqlDbType.VarChar,32),
        //            new SqlParameter("@CreationDate", SqlDbType.DateTime),
        //            new SqlParameter("@LastModifity", SqlDbType.DateTime),
        //            new SqlParameter("@CouponType", SqlDbType.TinyInt,1),
        //            new SqlParameter("@StoreId", SqlDbType.Int,4),
        //            new SqlParameter("@ChannelId", SqlDbType.Int,4),
        //            new SqlParameter("@Uid", SqlDbType.Int,4),
        //            new SqlParameter("@CreateOid", SqlDbType.Int,4),
        //            new SqlParameter("@CreateOSN", SqlDbType.VarChar,30),
        //            new SqlParameter("@CashAmount", SqlDbType.Decimal,9),
        //            new SqlParameter("@TotalIn", SqlDbType.Decimal,9),
        //            new SqlParameter("@TotalOut", SqlDbType.Decimal,9),
        //            new SqlParameter("@Banlance", SqlDbType.Decimal,9),
        //            new SqlParameter("@ValidTime", SqlDbType.DateTime),
        //            new SqlParameter("@CashId", SqlDbType.Int,4)};
        //    parameters[0].Value = model.CashCouponSN;
        //    parameters[1].Value = model.CreationDate;
        //    parameters[2].Value = model.LastModifity;
        //    parameters[3].Value = model.CouponType;
        //    parameters[4].Value = model.StoreId;
        //    parameters[5].Value = model.ChannelId;
        //    parameters[6].Value = model.Uid;
        //    parameters[7].Value = model.CreateOid;
        //    parameters[8].Value = model.CreateOSN;
        //    parameters[9].Value = model.CashAmount;
        //    parameters[10].Value = model.TotalIn;
        //    parameters[11].Value = model.TotalOut;
        //    parameters[12].Value = model.Banlance;
        //    parameters[13].Value = model.ValidTime;
        //    parameters[14].Value = model.CashId;

        //    return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        //}

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static bool Delete(int PId)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_rechargeproducts ");
            strSql.Append(" where pid=@PId");
            SqlParameter[] parameters = {
					new SqlParameter("@PId", SqlDbType.Int,4)
			};
            parameters[0].Value = PId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public static bool DeleteList(string PIdlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_rechargeproducts ");
            strSql.Append(" where pid in (" + PIdlist + ")  ");
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static ProductEntity GetModel(int PId)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_rechargeproducts ");
            strSql.Append(" where pid=@PId");
            SqlParameter[] parameters = {
					new SqlParameter("@PId", SqlDbType.Int,4)
			};
            parameters[0].Value = PId;

            ProductEntity model = new ProductEntity();
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
        public static ProductEntity GetModel(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_rechargeproducts ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }

            ProductEntity model = new ProductEntity();
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
        /// 获得数据列表
        /// </summary>
        public static List<ProductEntity> GetList(string strWhere)
        {
            List<ProductEntity> list = new List<ProductEntity>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM hlh_rechargeproducts ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    ProductEntity info = BuildProductEntityReader(reader);
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
        public static List<ProductEntity> GetList(int Top, string strWhere, string filedOrder)
        {
            List<ProductEntity> list = new List<ProductEntity>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM hlh_rechargeproducts ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    ProductEntity info = BuildProductEntityReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
            //return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public static int GetRecordCount(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_rechargeproducts ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public static List<ProductEntity> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            List<ProductEntity> list = new List<ProductEntity>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.CashId desc");
            }
            strSql.Append(")AS Row, T.*  from hlh_rechargeproducts T ");
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
                    ProductEntity info = BuildProductEntityReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
            //return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString());
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
            parameters[0].Value = "hlh_rechargeproducts";
            parameters[1].Value = "CashId";
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = 0;
            parameters[5].Value = 0;
            parameters[6].Value = strWhere;	
            return DbHelperSQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
        }*/

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static ProductEntity DataRowToModel(DataRow row)
        {
            ProductEntity model = new ProductEntity();
            if (row != null)
            {
                if (row["pid"] != null && row["pid"].ToString() != "")
                {
                    model.pid = int.Parse(row["pid"].ToString());
                }
                if (row["pname"] != null && row["pname"].ToString() != "")
                {
                    model.pname = row["pname"].ToString();
                }
                if (row["pvalue"] != null && row["pvalue"].ToString() != "")
                {
                    model.pvalue =float.Parse(row["pvalue"].ToString());
                }
                if (row["punit"] != null && row["punit"].ToString() != "")
                {
                    model.punit = row["punit"].ToString();
                }
                if (row["psubcate"] != null && row["psubcate"].ToString() != "")
                {
                    model.psubcate = int.Parse(row["psubcate"].ToString());
                }
                if (row["pchannel"] != null && row["pchannel"].ToString() != "")
                {
                    model.pchannel = int.Parse(row["pchannel"].ToString());
                }
                if (row["pareaid"] != null && row["pareaid"].ToString() != "")
                {
                    model.pareaid = int.Parse(row["pareaid"].ToString());
                }
                if (row["pcate"] != null && row["pcate"].ToString() != "")
                {
                    model.pcate = int.Parse(row["pcate"].ToString());
                }
                if (row["pstock"] != null && row["pstock"].ToString() != "")
                {
                    model.pstock = int.Parse(row["pstock"].ToString());
                }
                if (row["porder"] != null && row["porder"].ToString() != "")
                {
                    model.porder = int.Parse(row["porder"].ToString());
                }
                if (row["plimit"] != null && row["plimit"].ToString() != "")
                {
                    model.plimit = row["plimit"].ToString();
                }
                if (row["pmarkprice"] != null && row["pmarkprice"].ToString() != "")
                {
                    model.pmarkprice = decimal.Parse(row["pmarkprice"].ToString());
                }
            }
            return model;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static ProductEntity BuildProductEntityReader(IDataReader row)
        {
            ProductEntity model = new ProductEntity();
            if (row != null)
            {
                if (row["pid"] != null && row["pid"].ToString() != "")
                {
                    model.pid = int.Parse(row["pid"].ToString());
                }
                if (row["pname"] != null && row["pname"].ToString() != "")
                {
                    model.pname = row["pname"].ToString();
                }
                if (row["pvalue"] != null && row["pvalue"].ToString() != "")
                {
                    model.pvalue = float.Parse(row["pvalue"].ToString());
                }
                if (row["punit"] != null && row["punit"].ToString() != "")
                {
                    model.punit = row["punit"].ToString();
                }
                if (row["psubcate"] != null && row["psubcate"].ToString() != "")
                {
                    model.psubcate = int.Parse(row["psubcate"].ToString());
                }
                if (row["pchannel"] != null && row["pchannel"].ToString() != "")
                {
                    model.pchannel = int.Parse(row["pchannel"].ToString());
                }
                if (row["pareaid"] != null && row["pareaid"].ToString() != "")
                {
                    model.pareaid = int.Parse(row["pareaid"].ToString());
                }
                if (row["pcate"] != null && row["pcate"].ToString() != "")
                {
                    model.pcate = int.Parse(row["pcate"].ToString());
                }
                if (row["pstock"] != null && row["pstock"].ToString() != "")
                {
                    model.pstock = int.Parse(row["pstock"].ToString());
                }
                if (row["porder"] != null && row["porder"].ToString() != "")
                {
                    model.porder = int.Parse(row["porder"].ToString());
                }
                if (row["plimit"] != null && row["plimit"].ToString() != "")
                {
                    model.plimit = row["plimit"].ToString();
                }
                if (row["pmarkprice"] != null && row["pmarkprice"].ToString() != "")
                {
                    model.pmarkprice = decimal.Parse(row["pmarkprice"].ToString());
                }
            }
            return model;
        }

        #endregion  BasicMethod
        #region  ExtensionMethod

        #endregion  ExtensionMethod
    }
}

