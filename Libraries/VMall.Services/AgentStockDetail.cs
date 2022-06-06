using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using VMall.Core;

namespace VMall.Services
{
	/// <summary>
	/// 数据访问类:hlh_agentstockdetail
	/// </summary>
	public partial class AgentStockDetail
	{
		public AgentStockDetail()
		{}
		#region  BasicMethod
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from hlh_agentstockdetail");
			strSql.Append(" where id=@id");
			SqlParameter[] parameters = {
					new SqlParameter("@id", SqlDbType.Int,4)
			};
			parameters[0].Value = id;

			return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public int Add(AgentStockDetailInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into hlh_agentstockdetail(");
			strSql.Append("creationdate,uid,pid,detailtype,inamount,outamount,currentbalance,ordercode,detaildesc,fromuser,touser)");
			strSql.Append(" values (");
			strSql.Append("@creationdate,@uid,@pid,@detailtype,@inamount,@outamount,@currentbalance,@ordercode,@detaildesc,@fromuser,@touser)");
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = {
					new SqlParameter("@creationdate", SqlDbType.DateTime),
					new SqlParameter("@uid", SqlDbType.Int,4),
					new SqlParameter("@pid", SqlDbType.Int,4),
					new SqlParameter("@detailtype", SqlDbType.TinyInt,1),
					new SqlParameter("@inamount", SqlDbType.Decimal,10),
					new SqlParameter("@outamount", SqlDbType.Decimal,10),
					new SqlParameter("@currentbalance", SqlDbType.Decimal,10),
					new SqlParameter("@ordercode", SqlDbType.Char,32),
					new SqlParameter("@detaildesc", SqlDbType.NVarChar,200),
					new SqlParameter("@fromuser", SqlDbType.Int,4),
					new SqlParameter("@touser", SqlDbType.Int,4)};
			parameters[0].Value = model.CreationDate;
			parameters[1].Value = model.Uid;
			parameters[2].Value = model.Pid;
			parameters[3].Value = model.DetailType;
			parameters[4].Value = model.InAmount;
			parameters[5].Value = model.OutAmount;
			parameters[6].Value = model.CurrentBalance;
			parameters[7].Value = model.OrderCode;
			parameters[8].Value = model.DetailDesc;
			parameters[9].Value = model.FromUser;
			parameters[10].Value = model.ToUser;

			 return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters));
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(AgentStockDetailInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update hlh_agentstockdetail set ");
			strSql.Append("creationdate=@creationdate,");
			strSql.Append("uid=@uid,");
			strSql.Append("pid=@pid,");
			strSql.Append("detailtype=@detailtype,");
			strSql.Append("inamount=@inamount,");
			strSql.Append("outamount=@outamount,");
			strSql.Append("currentbalance=@currentbalance,");
			strSql.Append("ordercode=@ordercode,");
			strSql.Append("detaildesc=@detaildesc,");
			strSql.Append("fromuser=@fromuser,");
			strSql.Append("touser=@touser");
			strSql.Append(" where id=@id");
			SqlParameter[] parameters = {
					new SqlParameter("@creationdate", SqlDbType.DateTime),
					new SqlParameter("@uid", SqlDbType.Int,4),
					new SqlParameter("@pid", SqlDbType.Int,4),
					new SqlParameter("@detailtype", SqlDbType.TinyInt,1),
					new SqlParameter("@inamount", SqlDbType.Decimal,10),
					new SqlParameter("@outamount", SqlDbType.Decimal,10),
					new SqlParameter("@currentbalance", SqlDbType.Decimal,10),
					new SqlParameter("@ordercode", SqlDbType.Char,32),
					new SqlParameter("@detaildesc", SqlDbType.NVarChar,200),
					new SqlParameter("@fromuser", SqlDbType.Int,4),
					new SqlParameter("@touser", SqlDbType.Int,4),
					new SqlParameter("@id", SqlDbType.Int,4)};
			parameters[0].Value = model.CreationDate;
			parameters[1].Value = model.Uid;
			parameters[2].Value = model.Pid;
			parameters[3].Value = model.DetailType;
			parameters[4].Value = model.InAmount;
			parameters[5].Value = model.OutAmount;
			parameters[6].Value = model.CurrentBalance;
			parameters[7].Value = model.OrderCode;
			parameters[8].Value = model.DetailDesc;
			parameters[9].Value = model.FromUser;
			parameters[10].Value = model.ToUser;
			parameters[11].Value = model.Id;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int id)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from hlh_agentstockdetail ");
			strSql.Append(" where id=@id");
			SqlParameter[] parameters = {
					new SqlParameter("@id", SqlDbType.Int,4)
			};
			parameters[0].Value = id;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string idlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from hlh_agentstockdetail ");
			strSql.Append(" where id in ("+idlist + ")  ");

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public AgentStockDetailInfo GetModel(int id)
		{
	
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select  top 1 * from hlh_agentstockdetail ");
			strSql.Append(" where id=@id");
			SqlParameter[] parameters = {
					new SqlParameter("@id", SqlDbType.Int,4)
			};
			parameters[0].Value = id;

			AgentStockDetailInfo model=new AgentStockDetailInfo();
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
        public AgentStockDetailInfo GetModel(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_agentstockdetail ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }

            AgentStockDetailInfo model = new AgentStockDetailInfo();
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
		public AgentStockDetailInfo DataRowToModel(DataRow row)
		{
			AgentStockDetailInfo model=new AgentStockDetailInfo();
			if (row != null)
			{
				if(row["id"]!=null && row["id"].ToString()!="")
				{
					model.Id=int.Parse(row["id"].ToString());
				}
				if(row["creationdate"]!=null && row["creationdate"].ToString()!="")
				{
					model.CreationDate=DateTime.Parse(row["creationdate"].ToString());
				}
				if(row["uid"]!=null && row["uid"].ToString()!="")
				{
					model.Uid=int.Parse(row["uid"].ToString());
				}
				if(row["pid"]!=null && row["pid"].ToString()!="")
				{
					model.Pid=int.Parse(row["pid"].ToString());
				}
				if(row["detailtype"]!=null && row["detailtype"].ToString()!="")
				{
					model.DetailType=int.Parse(row["detailtype"].ToString());
				}
				if(row["inamount"]!=null && row["inamount"].ToString()!="")
				{
                    model.InAmount = decimal.Parse(row["inamount"].ToString());
				}
				if(row["outamount"]!=null && row["outamount"].ToString()!="")
				{
                    model.OutAmount = decimal.Parse(row["outamount"].ToString());
				}
				if(row["currentbalance"]!=null && row["currentbalance"].ToString()!="")
				{
                    model.CurrentBalance = decimal.Parse(row["currentbalance"].ToString());
				}
				if(row["ordercode"]!=null)
				{
					model.OrderCode=row["ordercode"].ToString();
				}
				if(row["detaildesc"]!=null)
				{
					model.DetailDesc=row["detaildesc"].ToString();
				}
				if(row["fromuser"]!=null && row["fromuser"].ToString()!="")
				{
					model.FromUser=int.Parse(row["fromuser"].ToString());
				}
				if(row["touser"]!=null && row["touser"].ToString()!="")
				{
					model.ToUser=int.Parse(row["touser"].ToString());
				}
			}
			return model;
		}
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AgentStockDetailInfo BuildAgentStockDetailReader(IDataReader row)
        {
            AgentStockDetailInfo model = new AgentStockDetailInfo();
            if (row != null)
            {
                if (row["id"] != null && row["id"].ToString() != "")
                {
                    model.Id = int.Parse(row["id"].ToString());
                }
                if (row["creationdate"] != null && row["creationdate"].ToString() != "")
                {
                    model.CreationDate = DateTime.Parse(row["creationdate"].ToString());
                }
                if (row["uid"] != null && row["uid"].ToString() != "")
                {
                    model.Uid = int.Parse(row["uid"].ToString());
                }
                if (row["pid"] != null && row["pid"].ToString() != "")
                {
                    model.Pid = int.Parse(row["pid"].ToString());
                }
                if (row["detailtype"] != null && row["detailtype"].ToString() != "")
                {
                    model.DetailType = int.Parse(row["detailtype"].ToString());
                }
                if (row["inamount"] != null && row["inamount"].ToString() != "")
                {
                    model.InAmount = decimal.Parse(row["inamount"].ToString());
                }
                if (row["outamount"] != null && row["outamount"].ToString() != "")
                {
                    model.OutAmount = decimal.Parse(row["outamount"].ToString());
                }
                if (row["currentbalance"] != null && row["currentbalance"].ToString() != "")
                {
                    model.CurrentBalance = decimal.Parse(row["currentbalance"].ToString());
                }
                if (row["ordercode"] != null)
                {
                    model.OrderCode = row["ordercode"].ToString();
                }
                if (row["detaildesc"] != null)
                {
                    model.DetailDesc = row["detaildesc"].ToString();
                }
                if (row["fromuser"] != null && row["fromuser"].ToString() != "")
                {
                    model.FromUser = int.Parse(row["fromuser"].ToString());
                }
                if (row["touser"] != null && row["touser"].ToString() != "")
                {
                    model.ToUser = int.Parse(row["touser"].ToString());
                }
            }
            return model;
        }

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<AgentStockDetailInfo> GetList(string strWhere)
		{
            List<AgentStockDetailInfo> list = new List<AgentStockDetailInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select *  ");
			strSql.Append(" FROM hlh_agentstockdetail ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AgentStockDetailInfo info = BuildAgentStockDetailReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
		}

		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public List<AgentStockDetailInfo> GetList(int Top,string strWhere,string filedOrder)
		{
            List<AgentStockDetailInfo> list = new List<AgentStockDetailInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ");
			if(Top>0)
			{
				strSql.Append(" top "+Top.ToString());
			}
			strSql.Append(" * ");
			strSql.Append(" FROM hlh_agentstockdetail ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AgentStockDetailInfo info = BuildAgentStockDetailReader(reader);
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
			strSql.Append("select count(1) FROM hlh_agentstockdetail ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public List<AgentStockDetailInfo> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
            List<AgentStockDetailInfo> list = new List<AgentStockDetailInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			if (!string.IsNullOrEmpty(orderby.Trim()))
			{
				strSql.Append("order by T." + orderby );
			}
			else
			{
				strSql.Append("order by T.id desc");
			}
			strSql.Append(")AS Row, T.*  from hlh_agentstockdetail T ");
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
                    AgentStockDetailInfo info = BuildAgentStockDetailReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
		}

		

		#endregion  BasicMethod
		#region  ExtensionMethod
        /// <summary>
        /// 增加库存详情记录
        /// </summary>
        /// <param name="PUid">详情关联的用户id</param>
        /// <param name="Pid">产品id</param>
        /// <param name="Type">详情类型 1为增加，2为减少</param>
        /// <param name="InAmount">增加数量</param>
        /// <param name="OutAmount">减少数量</param>
        /// <param name="CurrentBalance">当前余额</param>
        /// <param name="OSN">订单号</param>
        /// <param name="DetailDesc">详情描述</param>
        /// <param name="FromUser">来源会员id</param>
        /// <param name="ToUser">去向会员id</param>
        public void AddDetail(int PUid, int Pid, int Type, decimal InAmount, decimal OutAmount, decimal CurrentBalance, string OSN, string DetailDesc, int FromUser, int ToUser)
        {
            PartProductInfo pro = Products.GetPartProductById(Pid);
            new AgentStockDetail().Add(new AgentStockDetailInfo()
            {
                CreationDate = DateTime.Now,
                Uid = PUid,
                Pid = Pid,
                DetailType = Type,
                InAmount=InAmount,
                OutAmount = OutAmount,
                CurrentBalance = CurrentBalance,
                OrderCode = OSN,
                DetailDesc =DetailDesc,
                FromUser = FromUser,
                ToUser = ToUser
            });
        }

		#endregion  ExtensionMethod
	}
}

