using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using VMall.Core;
using System.Collections.Generic;

namespace VMall.Services
{
	/// <summary>
	/// 数据访问类:ExChangeCoupons
	/// </summary>
	public partial class ExChangeCoupons
	{
		public ExChangeCoupons()
		{}
		#region  BasicMethod
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int exid)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from hlh_exchangecoupons");
			strSql.Append(" where exid=@exid");
			SqlParameter[] parameters = {
					new SqlParameter("@exid", SqlDbType.Int,4)
			};
			parameters[0].Value = exid;

			return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public static int Add(ExChangeCouponsInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into hlh_exchangecoupons(");
            strSql.Append("exsn,uid,type,state,storeid,oid,usetime,validtime,useip,createuid,createoid,createtime,codetypeid)");
			strSql.Append(" values (");
            strSql.Append("@exsn,@uid,@type,@state,@storeid,@oid,@usetime,@validtime,@useip,@createuid,@createoid,@createtime,@codetypeid)");
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = {
					new SqlParameter("@exsn", SqlDbType.NChar,32),
					new SqlParameter("@uid", SqlDbType.Int,4),
					new SqlParameter("@type", SqlDbType.TinyInt,1),
					new SqlParameter("@state", SqlDbType.TinyInt,1),
					new SqlParameter("@storeid", SqlDbType.Int,4),
					new SqlParameter("@oid", SqlDbType.Int,4),
					new SqlParameter("@usetime", SqlDbType.DateTime),
					new SqlParameter("@validtime", SqlDbType.DateTime),
					new SqlParameter("@useip", SqlDbType.NChar,15),
					new SqlParameter("@createuid", SqlDbType.Int,4),
					new SqlParameter("@createoid", SqlDbType.Int,4),
					new SqlParameter("@createtime", SqlDbType.DateTime),
                    new SqlParameter("@codetypeid", SqlDbType.Int,4) 
            };
			parameters[0].Value = model.exsn;
			parameters[1].Value = model.uid;
			parameters[2].Value = model.type;
			parameters[3].Value = model.state;
			parameters[4].Value = model.storeid;
			parameters[5].Value = model.oid;
			parameters[6].Value = model.usetime;
			parameters[7].Value = model.validtime;
			parameters[8].Value = model.useip;
			parameters[9].Value = model.createuid;
			parameters[10].Value = model.createoid;
			parameters[11].Value = model.createtime;
            parameters[12].Value = model.codetypeid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters));
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public static bool Update(ExChangeCouponsInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update hlh_exchangecoupons set ");
			strSql.Append("exsn=@exsn,");
			strSql.Append("uid=@uid,");
			strSql.Append("type=@type,");
			strSql.Append("state=@state,");
			strSql.Append("storeid=@storeid,");
			strSql.Append("oid=@oid,");
			strSql.Append("usetime=@usetime,");
			strSql.Append("validtime=@validtime,");
			strSql.Append("useip=@useip,");
			strSql.Append("createuid=@createuid,");
			strSql.Append("createoid=@createoid,");
			strSql.Append("createtime=@createtime,");
            strSql.Append("codetypeid=@codetypeid ");
			strSql.Append(" where exid=@exid");
			SqlParameter[] parameters = {
					new SqlParameter("@exsn", SqlDbType.NChar,32),
					new SqlParameter("@uid", SqlDbType.Int,4),
					new SqlParameter("@type", SqlDbType.TinyInt,1),
					new SqlParameter("@state", SqlDbType.TinyInt,1),
					new SqlParameter("@storeid", SqlDbType.Int,4),
					new SqlParameter("@oid", SqlDbType.Int,4),
					new SqlParameter("@usetime", SqlDbType.DateTime),
					new SqlParameter("@validtime", SqlDbType.DateTime),
					new SqlParameter("@useip", SqlDbType.NChar,15),
					new SqlParameter("@createuid", SqlDbType.Int,4),
					new SqlParameter("@createoid", SqlDbType.Int,4),
					new SqlParameter("@createtime", SqlDbType.DateTime),
                    new SqlParameter("@codetypeid", SqlDbType.Int,4),
					new SqlParameter("@exid", SqlDbType.Int,4)};
			parameters[0].Value = model.exsn;
			parameters[1].Value = model.uid;
			parameters[2].Value = model.type;
			parameters[3].Value = model.state;
			parameters[4].Value = model.storeid;
			parameters[5].Value = model.oid;
			parameters[6].Value = model.usetime;
			parameters[7].Value = model.validtime;
			parameters[8].Value = model.useip;
			parameters[9].Value = model.createuid;
			parameters[10].Value = model.createoid;
			parameters[11].Value = model.createtime;
            parameters[12].Value = model.codetypeid;
			parameters[13].Value = model.exid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
        public static bool Delete(int exid)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from hlh_exchangecoupons ");
			strSql.Append(" where exid=@exid");
			SqlParameter[] parameters = {
					new SqlParameter("@exid", SqlDbType.Int,4)
			};
			parameters[0].Value = exid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
        public static bool DeleteList(string exidlist)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from hlh_exchangecoupons ");
			strSql.Append(" where exid in ("+exidlist + ")  ");
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
        public static ExChangeCouponsInfo GetModel(int exid)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select  top 1 * from hlh_exchangecoupons ");
			strSql.Append(" where exid=@exid");
			SqlParameter[] parameters = {
					new SqlParameter("@exid", SqlDbType.Int,4)
			};
			parameters[0].Value = exid;

            ExChangeCouponsInfo model = new ExChangeCouponsInfo();
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
        public  static ExChangeCouponsInfo GetModel(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_exchangecoupons ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }

            ExChangeCouponsInfo model = new ExChangeCouponsInfo();
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
        public static ExChangeCouponsInfo DataRowToModel(DataRow row)
		{
			ExChangeCouponsInfo model=new ExChangeCouponsInfo();
			if (row != null)
			{
				if(row["exid"]!=null && row["exid"].ToString()!="")
				{
					model.exid=int.Parse(row["exid"].ToString());
				}
				if(row["exsn"]!=null)
				{
					model.exsn=row["exsn"].ToString();
				}
				if(row["uid"]!=null && row["uid"].ToString()!="")
				{
					model.uid=int.Parse(row["uid"].ToString());
				}
				if(row["type"]!=null && row["type"].ToString()!="")
				{
					model.type=int.Parse(row["type"].ToString());
				}
				if(row["state"]!=null && row["state"].ToString()!="")
				{
					model.state=int.Parse(row["state"].ToString());
				}
				if(row["storeid"]!=null && row["storeid"].ToString()!="")
				{
					model.storeid=int.Parse(row["storeid"].ToString());
				}
				if(row["oid"]!=null && row["oid"].ToString()!="")
				{
					model.oid=int.Parse(row["oid"].ToString());
				}
				if(row["usetime"]!=null && row["usetime"].ToString()!="")
				{
					model.usetime=DateTime.Parse(row["usetime"].ToString());
				}
				if(row["validtime"]!=null && row["validtime"].ToString()!="")
				{
					model.validtime=DateTime.Parse(row["validtime"].ToString());
				}
				if(row["useip"]!=null)
				{
					model.useip=row["useip"].ToString();
				}
				if(row["createuid"]!=null && row["createuid"].ToString()!="")
				{
					model.createuid=int.Parse(row["createuid"].ToString());
				}
				if(row["createoid"]!=null && row["createoid"].ToString()!="")
				{
					model.createoid=int.Parse(row["createoid"].ToString());
				}
				if(row["createtime"]!=null && row["createtime"].ToString()!="")
				{
					model.createtime=DateTime.Parse(row["createtime"].ToString());
				}
                if (row["codetypeid"] != null && row["codetypeid"].ToString() != "")
                {
                    model.codetypeid = int.Parse(row["codetypeid"].ToString());
                }
			}
			return model;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
        public static List<ExChangeCouponsInfo> GetList(string strWhere)
		{
            List<ExChangeCouponsInfo> list = new List<ExChangeCouponsInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select * ");
			strSql.Append(" FROM hlh_exchangecoupons ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    ExChangeCouponsInfo info = BuildExChangeCouponsInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
		}

		/// <summary>
		/// 获得前几行数据
		/// </summary>
        public static List<ExChangeCouponsInfo> GetList(int Top, string strWhere, string filedOrder)
		{
            List<ExChangeCouponsInfo> list = new List<ExChangeCouponsInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ");
			if(Top>0)
			{
				strSql.Append(" top "+Top.ToString());
			}
			strSql.Append(" * ");
			strSql.Append(" FROM hlh_exchangecoupons ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    ExChangeCouponsInfo info = BuildExChangeCouponsInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
		}

		/// <summary>
		/// 获取记录总数
		/// </summary>
        public static int GetRecordCount(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) FROM hlh_exchangecoupons ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
        public static List<ExChangeCouponsInfo> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
            List<ExChangeCouponsInfo> list = new List<ExChangeCouponsInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			if (!string.IsNullOrEmpty(orderby.Trim()))
			{
				strSql.Append("order by T." + orderby );
			}
			else
			{
				strSql.Append("order by T.exid desc");
			}
			strSql.Append(")AS Row, T.*  from hlh_exchangecoupons T ");
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
                    ExChangeCouponsInfo info = BuildExChangeCouponsInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
		}


        /// <summary>
        /// 获取记录总数
        /// </summary>
        public static int AdminGetRecordCount(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_exchangecoupons T LEFT JOIN hlh_users b on T.uid=b.uid");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public static DataTable AdminGetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
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
                strSql.Append("order by T.exid desc");
            }
            strSql.Append(")AS Row, T.*,b.username,b.email,b.mobile  from hlh_exchangecoupons T LEFT JOIN hlh_users b on T.uid=b.uid");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            //using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            //{
            //    while (reader.Read())
            //    {
            //        CashCouponInfo info = BuildCashCouponInfoReader(reader);
            //        list.Add(info);
            //    }
            //    reader.Close();
            //}
            return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString()).Tables[0];
            //return list;
            //return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString());
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static ExChangeCouponsInfo BuildExChangeCouponsInfoReader(IDataReader row)
        {
            ExChangeCouponsInfo model = new ExChangeCouponsInfo();
            if (row != null)
            {
                if (row["exid"] != null && row["exid"].ToString() != "")
                {
                    model.exid = int.Parse(row["exid"].ToString());
                }
                if (row["exsn"] != null)
                {
                    model.exsn = row["exsn"].ToString();
                }
                if (row["uid"] != null && row["uid"].ToString() != "")
                {
                    model.uid = int.Parse(row["uid"].ToString());
                }
                if (row["type"] != null && row["type"].ToString() != "")
                {
                    model.type = int.Parse(row["type"].ToString());
                }
                if (row["state"] != null && row["state"].ToString() != "")
                {
                    model.state = int.Parse(row["state"].ToString());
                }
                if (row["storeid"] != null && row["storeid"].ToString() != "")
                {
                    model.storeid = int.Parse(row["storeid"].ToString());
                }
                if (row["oid"] != null && row["oid"].ToString() != "")
                {
                    model.oid = int.Parse(row["oid"].ToString());
                }
                if (row["usetime"] != null && row["usetime"].ToString() != "")
                {
                    model.usetime = DateTime.Parse(row["usetime"].ToString());
                }
                if (row["validtime"] != null && row["validtime"].ToString() != "")
                {
                    model.validtime = DateTime.Parse(row["validtime"].ToString());
                }
                if (row["useip"] != null)
                {
                    model.useip = row["useip"].ToString();
                }
                if (row["createuid"] != null && row["createuid"].ToString() != "")
                {
                    model.createuid = int.Parse(row["createuid"].ToString());
                }
                if (row["createoid"] != null && row["createoid"].ToString() != "")
                {
                    model.createoid = int.Parse(row["createoid"].ToString());
                }
                if (row["createtime"] != null && row["createtime"].ToString() != "")
                {
                    model.createtime = DateTime.Parse(row["createtime"].ToString());
                }
                if (row["codetypeid"] != null && row["codetypeid"].ToString() != "")
                {
                    model.codetypeid = int.Parse(row["codetypeid"].ToString());
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
			parameters[0].Value = "hlh_exchangecoupons";
			parameters[1].Value = "exid";
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
        /// 获得数据列表
        /// </summary>
        public static List<ExChangeCouponsInfo> GetVaildListByPid(string strWhere)
        {
            List<ExChangeCouponsInfo> list = new List<ExChangeCouponsInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM hlh_exchangecoupons a LEFT JOIN dbo.hlh_excodeproducts b ON a.codetypeid=b.codetypeid ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    ExChangeCouponsInfo info = BuildExChangeCouponsInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }

        public static bool isExistUser(string name)
        {
            bool isUser = false;
            PartUserInfo user;
            if (ValidateHelper.IsEmail(name))
                user = Users.GetPartUserByEmail(name);
            else if (ValidateHelper.IsMobile(name))
                user = Users.GetPartUserByMobile(name);
            else
                user = Users.GetPartUserByName(name);
            if (user != null)
                isUser = true;
            return isUser;
        }
		#endregion  ExtensionMethod
	}
}

