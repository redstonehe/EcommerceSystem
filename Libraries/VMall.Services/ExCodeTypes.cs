using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using VMall.Core;
using System.Collections.Generic;

namespace VMall.Services
{
	/// <summary>
    /// 数据访问类:ExCodeTypes
	/// </summary>
	public partial class ExCodeTypes
	{
        public ExCodeTypes()
		{}
		#region  BasicMethod
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int codetypeid)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from hlh_excodetypes");
			strSql.Append(" where codetypeid=@codetypeid");
			SqlParameter[] parameters = {
					new SqlParameter("@codetypeid", SqlDbType.Int,4)
			};
			parameters[0].Value = codetypeid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public int Add(ExCodeTypesInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into hlh_excodetypes(");
			strSql.Append("storeid,state,name,money,count,sendmode,getmode,usemode,userranklower,orderamountlower,limitstorecid,limitproduct,sendstarttime,sendendtime,useexpiretime,usestarttime,useendtime)");
			strSql.Append(" values (");
			strSql.Append("@storeid,@state,@name,@money,@count,@sendmode,@getmode,@usemode,@userranklower,@orderamountlower,@limitstorecid,@limitproduct,@sendstarttime,@sendendtime,@useexpiretime,@usestarttime,@useendtime)");
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = {
					new SqlParameter("@storeid", SqlDbType.Int,4),
					new SqlParameter("@state", SqlDbType.TinyInt,1),
					new SqlParameter("@name", SqlDbType.NVarChar,50),
					new SqlParameter("@money", SqlDbType.Int,4),
					new SqlParameter("@count", SqlDbType.Int,4),
					new SqlParameter("@sendmode", SqlDbType.TinyInt,1),
					new SqlParameter("@getmode", SqlDbType.TinyInt,1),
					new SqlParameter("@usemode", SqlDbType.TinyInt,1),
					new SqlParameter("@userranklower", SqlDbType.SmallInt,2),
					new SqlParameter("@orderamountlower", SqlDbType.Int,4),
					new SqlParameter("@limitstorecid", SqlDbType.Int,4),
					new SqlParameter("@limitproduct", SqlDbType.TinyInt,1),
					new SqlParameter("@sendstarttime", SqlDbType.DateTime),
					new SqlParameter("@sendendtime", SqlDbType.DateTime),
					new SqlParameter("@useexpiretime", SqlDbType.Int,4),
					new SqlParameter("@usestarttime", SqlDbType.DateTime),
					new SqlParameter("@useendtime", SqlDbType.DateTime)};
            parameters[0].Value = model.StoreId;
            parameters[1].Value = model.State;
			parameters[2].Value = model.Name;
			parameters[3].Value = model.Money;
			parameters[4].Value = model.Count;
			parameters[5].Value = model.SendMode;
			parameters[6].Value = model.GetMode;
			parameters[7].Value = model.UseMode;
			parameters[8].Value = model.UserRankLower;
            parameters[9].Value = model.OrderAmountLower;
			parameters[10].Value = model.LimitStoreCid;
			parameters[11].Value = model.LimitProduct;
			parameters[12].Value = model.SendStartTime;
			parameters[13].Value = model.SendEndTime;
			parameters[14].Value = model.UseExpireTime;
			parameters[15].Value = model.UseStartTime;
			parameters[16].Value = model.UseEndTime;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters));
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(ExCodeTypesInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update hlh_excodetypes set ");
			strSql.Append("storeid=@storeid,");
			strSql.Append("state=@state,");
			strSql.Append("name=@name,");
			strSql.Append("money=@money,");
			strSql.Append("count=@count,");
			strSql.Append("sendmode=@sendmode,");
			strSql.Append("getmode=@getmode,");
			strSql.Append("usemode=@usemode,");
			strSql.Append("userranklower=@userranklower,");
			strSql.Append("orderamountlower=@orderamountlower,");
			strSql.Append("limitstorecid=@limitstorecid,");
			strSql.Append("limitproduct=@limitproduct,");
			strSql.Append("sendstarttime=@sendstarttime,");
			strSql.Append("sendendtime=@sendendtime,");
			strSql.Append("useexpiretime=@useexpiretime,");
			strSql.Append("usestarttime=@usestarttime,");
			strSql.Append("useendtime=@useendtime");
			strSql.Append(" where codetypeid=@codetypeid");
			SqlParameter[] parameters = {
					new SqlParameter("@storeid", SqlDbType.Int,4),
					new SqlParameter("@state", SqlDbType.TinyInt,1),
					new SqlParameter("@name", SqlDbType.NVarChar,50),
					new SqlParameter("@money", SqlDbType.Int,4),
					new SqlParameter("@count", SqlDbType.Int,4),
					new SqlParameter("@sendmode", SqlDbType.TinyInt,1),
					new SqlParameter("@getmode", SqlDbType.TinyInt,1),
					new SqlParameter("@usemode", SqlDbType.TinyInt,1),
					new SqlParameter("@userranklower", SqlDbType.SmallInt,2),
					new SqlParameter("@orderamountlower", SqlDbType.Int,4),
					new SqlParameter("@limitstorecid", SqlDbType.Int,4),
					new SqlParameter("@limitproduct", SqlDbType.TinyInt,1),
					new SqlParameter("@sendstarttime", SqlDbType.DateTime),
					new SqlParameter("@sendendtime", SqlDbType.DateTime),
					new SqlParameter("@useexpiretime", SqlDbType.Int,4),
					new SqlParameter("@usestarttime", SqlDbType.DateTime),
					new SqlParameter("@useendtime", SqlDbType.DateTime),
					new SqlParameter("@codetypeid", SqlDbType.Int,4)};
			parameters[0].Value = model.StoreId;
			parameters[1].Value = model.State;
			parameters[2].Value = model.Name;
			parameters[3].Value = model.Money;
			parameters[4].Value = model.Count;
			parameters[5].Value = model.SendMode;
			parameters[6].Value = model.GetMode;
			parameters[7].Value = model.UseMode;
			parameters[8].Value = model.UserRankLower;
			parameters[9].Value = model.OrderAmountLower;
			parameters[10].Value = model.LimitStoreCid;
			parameters[11].Value = model.LimitProduct;
			parameters[12].Value = model.SendStartTime;
			parameters[13].Value = model.SendEndTime;
			parameters[14].Value = model.UseExpireTime;
			parameters[15].Value = model.UseStartTime;
			parameters[16].Value = model.UseEndTime;
			parameters[17].Value = model.CodeTypeId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int codetypeid)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from hlh_excodetypes ");
			strSql.Append(" where codetypeid=@codetypeid");
			SqlParameter[] parameters = {
					new SqlParameter("@codetypeid", SqlDbType.Int,4)
			};
			parameters[0].Value = codetypeid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string codetypeidlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from hlh_excodetypes ");
			strSql.Append(" where codetypeid in ("+codetypeidlist + ")  ");

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public ExCodeTypesInfo GetModel(int codetypeid)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select  top 1 * from hlh_excodetypes ");
			strSql.Append(" where codetypeid=@codetypeid");
			SqlParameter[] parameters = {
					new SqlParameter("@codetypeid", SqlDbType.Int,4)
			};
			parameters[0].Value = codetypeid;

			ExCodeTypesInfo model=new ExCodeTypesInfo();
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
        public ExCodeTypesInfo GetModel(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_excodetypes ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
           

            ExCodeTypesInfo model = new ExCodeTypesInfo();
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
		public ExCodeTypesInfo DataRowToModel(DataRow row)
		{
			ExCodeTypesInfo model=new ExCodeTypesInfo();
			if (row != null)
			{
				if(row["codetypeid"]!=null && row["codetypeid"].ToString()!="")
				{
					model.CodeTypeId=int.Parse(row["codetypeid"].ToString());
				}
				if(row["storeid"]!=null && row["storeid"].ToString()!="")
				{
					model.StoreId=int.Parse(row["storeid"].ToString());
				}
				if(row["state"]!=null && row["state"].ToString()!="")
				{
					model.State=int.Parse(row["state"].ToString());
				}
				if(row["name"]!=null)
				{
					model.Name=row["name"].ToString();
				}
				if(row["money"]!=null && row["money"].ToString()!="")
				{
					model.Money=int.Parse(row["money"].ToString());
				}
				if(row["count"]!=null && row["count"].ToString()!="")
				{
					model.Count=int.Parse(row["count"].ToString());
				}
				if(row["sendmode"]!=null && row["sendmode"].ToString()!="")
				{
					model.SendMode=int.Parse(row["sendmode"].ToString());
				}
				if(row["getmode"]!=null && row["getmode"].ToString()!="")
				{
					model.GetMode=int.Parse(row["getmode"].ToString());
				}
				if(row["usemode"]!=null && row["usemode"].ToString()!="")
				{
					model.UseMode=int.Parse(row["usemode"].ToString());
				}
				if(row["userranklower"]!=null && row["userranklower"].ToString()!="")
				{
					model.UserRankLower=int.Parse(row["userranklower"].ToString());
				}
				if(row["orderamountlower"]!=null && row["orderamountlower"].ToString()!="")
				{
					model.OrderAmountLower=int.Parse(row["orderamountlower"].ToString());
				}
				if(row["limitstorecid"]!=null && row["limitstorecid"].ToString()!="")
				{
					model.LimitStoreCid=int.Parse(row["limitstorecid"].ToString());
				}
				if(row["limitproduct"]!=null && row["limitproduct"].ToString()!="")
				{
					model.LimitProduct=int.Parse(row["limitproduct"].ToString());
				}
				if(row["sendstarttime"]!=null && row["sendstarttime"].ToString()!="")
				{
					model.SendStartTime=DateTime.Parse(row["sendstarttime"].ToString());
				}
				if(row["sendendtime"]!=null && row["sendendtime"].ToString()!="")
				{
					model.SendEndTime=DateTime.Parse(row["sendendtime"].ToString());
				}
				if(row["useexpiretime"]!=null && row["useexpiretime"].ToString()!="")
				{
					model.UseExpireTime=int.Parse(row["useexpiretime"].ToString());
				}
				if(row["usestarttime"]!=null && row["usestarttime"].ToString()!="")
				{
					model.UseStartTime=DateTime.Parse(row["usestarttime"].ToString());
				}
				if(row["useendtime"]!=null && row["useendtime"].ToString()!="")
				{
					model.UseEndTime=DateTime.Parse(row["useendtime"].ToString());
				}
			}
			return model;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<ExCodeTypesInfo> GetList(string strWhere)
		{
            List<ExCodeTypesInfo> list =new List<ExCodeTypesInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select * ");
			strSql.Append(" FROM hlh_excodetypes ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    ExCodeTypesInfo info = BuildExCodeTypesInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
		}

		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public List<ExCodeTypesInfo> GetList(int Top,string strWhere,string filedOrder)
		{
            List<ExCodeTypesInfo> list = new List<ExCodeTypesInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ");
			if(Top>0)
			{
				strSql.Append(" top "+Top.ToString());
			}
			strSql.Append(" * ");
			strSql.Append(" FROM hlh_excodetypes ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    ExCodeTypesInfo info = BuildExCodeTypesInfoReader(reader);
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
			strSql.Append("select count(1) FROM hlh_excodetypes ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public List<ExCodeTypesInfo> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
            List<ExCodeTypesInfo> list = new List<ExCodeTypesInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			if (!string.IsNullOrEmpty(orderby.Trim()))
			{
				strSql.Append("order by T." + orderby );
			}
			else
			{
				strSql.Append("order by T.codetypeid desc");
			}
			strSql.Append(")AS Row, T.*  from hlh_excodetypes T ");
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
                    ExCodeTypesInfo info = BuildExCodeTypesInfoReader(reader);
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
            List<ExCodeTypesInfo> list = new List<ExCodeTypesInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.codetypeid desc");
            }
            strSql.Append(")AS Row, T.*  from hlh_excodetypes T ");
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
        public ExCodeTypesInfo BuildExCodeTypesInfoReader(IDataReader row)
        {
            ExCodeTypesInfo model = new ExCodeTypesInfo();
            if (row != null)
            {
                if (row["codetypeid"] != null && row["codetypeid"].ToString() != "")
                {
                    model.CodeTypeId = int.Parse(row["codetypeid"].ToString());
                }
                if (row["storeid"] != null && row["storeid"].ToString() != "")
                {
                    model.StoreId = int.Parse(row["storeid"].ToString());
                }
                if (row["state"] != null && row["state"].ToString() != "")
                {
                    model.State = int.Parse(row["state"].ToString());
                }
                if (row["name"] != null)
                {
                    model.Name = row["name"].ToString();
                }
                if (row["money"] != null && row["money"].ToString() != "")
                {
                    model.Money = int.Parse(row["money"].ToString());
                }
                if (row["count"] != null && row["count"].ToString() != "")
                {
                    model.Count = int.Parse(row["count"].ToString());
                }
                if (row["sendmode"] != null && row["sendmode"].ToString() != "")
                {
                    model.SendMode = int.Parse(row["sendmode"].ToString());
                }
                if (row["getmode"] != null && row["getmode"].ToString() != "")
                {
                    model.GetMode = int.Parse(row["getmode"].ToString());
                }
                if (row["usemode"] != null && row["usemode"].ToString() != "")
                {
                    model.UseMode = int.Parse(row["usemode"].ToString());
                }
                if (row["userranklower"] != null && row["userranklower"].ToString() != "")
                {
                    model.UserRankLower = int.Parse(row["userranklower"].ToString());
                }
                if (row["orderamountlower"] != null && row["orderamountlower"].ToString() != "")
                {
                    model.OrderAmountLower = int.Parse(row["orderamountlower"].ToString());
                }
                if (row["limitstorecid"] != null && row["limitstorecid"].ToString() != "")
                {
                    model.LimitStoreCid = int.Parse(row["limitstorecid"].ToString());
                }
                if (row["limitproduct"] != null && row["limitproduct"].ToString() != "")
                {
                    model.LimitProduct = int.Parse(row["limitproduct"].ToString());
                }
                if (row["sendstarttime"] != null && row["sendstarttime"].ToString() != "")
                {
                    model.SendStartTime = DateTime.Parse(row["sendstarttime"].ToString());
                }
                if (row["sendendtime"] != null && row["sendendtime"].ToString() != "")
                {
                    model.SendEndTime = DateTime.Parse(row["sendendtime"].ToString());
                }
                if (row["useexpiretime"] != null && row["useexpiretime"].ToString() != "")
                {
                    model.UseExpireTime = int.Parse(row["useexpiretime"].ToString());
                }
                if (row["usestarttime"] != null && row["usestarttime"].ToString() != "")
                {
                    model.UseStartTime = DateTime.Parse(row["usestarttime"].ToString());
                }
                if (row["useendtime"] != null && row["useendtime"].ToString() != "")
                {
                    model.UseEndTime = DateTime.Parse(row["useendtime"].ToString());
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
			parameters[0].Value = "hlh_excodetypes";
			parameters[1].Value = "codetypeid";
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

