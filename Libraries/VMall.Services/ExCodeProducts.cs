using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using VMall.Core;
using System.Collections.Generic;

namespace VMall.Services
{
	/// <summary>
	/// 数据访问类:ExCodeProducts
	/// </summary>
	public partial class ExCodeProducts
	{
        public ExCodeProducts()
		{}
		#region  BasicMethod
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int recordid)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from hlh_excodeproducts");
			strSql.Append(" where recordid=@recordid");
			SqlParameter[] parameters = {
					new SqlParameter("@recordid", SqlDbType.Int,4)
			};
			parameters[0].Value = recordid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public int Add(ExCodeProductsInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into hlh_excodeproducts(");
			strSql.Append("codetypeid,pid)");
			strSql.Append(" values (");
			strSql.Append("@codetypeid,@pid)");
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = {
					new SqlParameter("@codetypeid", SqlDbType.Int,4),
					new SqlParameter("@pid", SqlDbType.Int,4)};
			parameters[0].Value = model.CodeTypeId;
			parameters[1].Value = model.Pid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters));
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(ExCodeProductsInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update hlh_excodeproducts set ");
			strSql.Append("codetypeid=@codetypeid,");
			strSql.Append("pid=@pid");
			strSql.Append(" where recordid=@recordid");
			SqlParameter[] parameters = {
					new SqlParameter("@codetypeid", SqlDbType.Int,4),
					new SqlParameter("@pid", SqlDbType.Int,4),
					new SqlParameter("@recordid", SqlDbType.Int,4)};
			parameters[0].Value = model.CodeTypeId;
			parameters[1].Value = model.Pid;
			parameters[2].Value = model.RecordId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int recordid)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from hlh_excodeproducts ");
			strSql.Append(" where recordid=@recordid");
			SqlParameter[] parameters = {
					new SqlParameter("@recordid", SqlDbType.Int,4)
			};
			parameters[0].Value = recordid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string recordidlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from hlh_excodeproducts ");
			strSql.Append(" where recordid in ("+recordidlist + ")  ");

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public ExCodeProductsInfo GetModel(int recordid)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select  top 1 * from hlh_excodeproducts ");
			strSql.Append(" where recordid=@recordid");
			SqlParameter[] parameters = {
					new SqlParameter("@recordid", SqlDbType.Int,4)
			};
			parameters[0].Value = recordid;

			ExCodeProductsInfo model=new ExCodeProductsInfo();
            DataSet ds = RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString(), parameters);
			if(ds.Tables[0].Rows.Count>0)
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
        public static ExCodeProductsInfo GetModel(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_excodeproducts ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }

            ExCodeProductsInfo model = new ExCodeProductsInfo();
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
		public static ExCodeProductsInfo DataRowToModel(DataRow row)
		{
			ExCodeProductsInfo model=new ExCodeProductsInfo();
			if (row != null)
			{
				if(row["recordid"]!=null && row["recordid"].ToString()!="")
				{
					model.RecordId=int.Parse(row["recordid"].ToString());
				}
				if(row["codetypeid"]!=null && row["codetypeid"].ToString()!="")
				{
					model.CodeTypeId=int.Parse(row["codetypeid"].ToString());
				}
				if(row["pid"]!=null && row["pid"].ToString()!="")
				{
					model.Pid=int.Parse(row["pid"].ToString());
				}
			}
			return model;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<ExCodeProductsInfo> GetList(string strWhere)
		{
            List<ExCodeProductsInfo> list = new List<ExCodeProductsInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select * ");
			strSql.Append(" FROM hlh_excodeproducts ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    ExCodeProductsInfo info = BuildExCodeProductsInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
		}

		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public List<ExCodeProductsInfo> GetList(int Top,string strWhere,string filedOrder)
		{
            List<ExCodeProductsInfo> list = new List<ExCodeProductsInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ");
			if(Top>0)
			{
				strSql.Append(" top "+Top.ToString());
			}
			strSql.Append(" * ");
			strSql.Append(" FROM hlh_excodeproducts ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    ExCodeProductsInfo info = BuildExCodeProductsInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
		}

		/// <summary>
		/// 获取记录总数
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) FROM hlh_excodeproducts ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public List<ExCodeProductsInfo> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
            List<ExCodeProductsInfo> list = new List<ExCodeProductsInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			if (!string.IsNullOrEmpty(orderby.Trim()))
			{
				strSql.Append("order by T." + orderby );
			}
			else
			{
				strSql.Append("order by T.recordid desc");
			}
			strSql.Append(")AS Row, T.*  from hlh_excodeproducts T ");
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
                    ExCodeProductsInfo info = BuildExCodeProductsInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
		}

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataTable AdminGetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            List<ExCodeProductsInfo> list = new List<ExCodeProductsInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.recordid desc");
            }
            strSql.Append(")AS Row, T.recordid,T.codetypeid,B.*  from hlh_excodeproducts T left join hlh_products B on T.pid=B.pid ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString()).Tables[0];
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static ExCodeProductsInfo BuildExCodeProductsInfoReader(IDataReader row)
        {
            ExCodeProductsInfo model = new ExCodeProductsInfo();
            if (row != null)
            {
                if (row["recordid"] != null && row["recordid"].ToString() != "")
                {
                    model.RecordId = int.Parse(row["recordid"].ToString());
                }
                if (row["codetypeid"] != null && row["codetypeid"].ToString() != "")
                {
                    model.CodeTypeId = int.Parse(row["codetypeid"].ToString());
                }
                if (row["pid"] != null && row["pid"].ToString() != "")
                {
                    model.Pid = int.Parse(row["pid"].ToString());
                }
            }
            return model;
        }
		/*
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetList(int PageSize,int PageIndex,string strWhere)
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
			parameters[0].Value = "hlh_excodeproducts";
			parameters[1].Value = "recordid";
			parameters[2].Value = PageSize;
			parameters[3].Value = PageIndex;
			parameters[4].Value = 0;
			parameters[5].Value = 0;
			parameters[6].Value = strWhere;	
			return DbHelperSQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
		}*/

		#endregion  BasicMethod
		#region  ExtensionMethod

        /// <summary>
        /// 兑换码商品是否已经存在
        /// </summary>
        /// <param name="couponTypeId">优惠劵类型id</param>
        /// <param name="pid">商品id</param>
        public bool IsExistCodeProduct(int codeTypeId, int pid)
        {
            SqlParameter[] parameters = {
					new SqlParameter("@codetypeid", SqlDbType.Int,4),
                    new SqlParameter("@pid", SqlDbType.Int,4)
			};
            parameters[0].Value = codeTypeId;
            parameters[1].Value = pid;
            string commandText = string.Format("SELECT [recordid] FROM [{0}excodeproducts] WHERE  [pid]=@pid",
                                                RDBSHelper.RDBSTablePre);
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parameters), -1) > 0;
        }

		#endregion  ExtensionMethod
	}
}

