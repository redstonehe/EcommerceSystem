using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using VMall.Core;

namespace VMall.Services
{
	/// <summary>
	/// 数据访问类:AgentSendOrder
	/// </summary>
	public partial class AgentSendOrder
	{
		public AgentSendOrder()
		{}
		#region  BasicMethod
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from hlh_agentsendorder");
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
		public int Add(AgentSendOrderInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into hlh_agentsendorder(");
			strSql.Append("creationdate,Pid,uid,sendosn,sendstate,sendcount,shipsn,shipcoid,shipconame,shiptime,shipfee,regionid,address,consignee,mobile,buyerremark,receivingtime,isextendreceive)");
			strSql.Append(" values (");
			strSql.Append("@creationdate,@Pid,@uid,@sendosn,@sendstate,@sendcount,@shipsn,@shipcoid,@shipconame,@shiptime,@shipfee,@regionid,@address,@consignee,@mobile,@buyerremark,@receivingtime,@isextendreceive)");
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = {
					new SqlParameter("@creationdate", SqlDbType.DateTime),
					new SqlParameter("@Pid", SqlDbType.Int,4),
					new SqlParameter("@uid", SqlDbType.Int,4),
					new SqlParameter("@sendosn", SqlDbType.NChar,30),
					new SqlParameter("@sendstate", SqlDbType.Int,4),
					new SqlParameter("@sendcount", SqlDbType.Int,4),
					new SqlParameter("@shipsn", SqlDbType.NChar,30),
					new SqlParameter("@shipcoid", SqlDbType.SmallInt,2),
					new SqlParameter("@shipconame", SqlDbType.NVarChar,30),
					new SqlParameter("@shiptime", SqlDbType.DateTime),
					new SqlParameter("@shipfee", SqlDbType.Decimal,9),
					new SqlParameter("@regionid", SqlDbType.Int,4),
					new SqlParameter("@address", SqlDbType.NVarChar,150),
					new SqlParameter("@consignee", SqlDbType.NVarChar,20),
					new SqlParameter("@mobile", SqlDbType.VarChar,15),
					new SqlParameter("@buyerremark", SqlDbType.NVarChar,250),
					new SqlParameter("@receivingtime", SqlDbType.DateTime),
					new SqlParameter("@isextendreceive", SqlDbType.TinyInt,1)};
			parameters[0].Value = model.CreationDate;
			parameters[1].Value = model.Pid;
			parameters[2].Value = model.Uid;
			parameters[3].Value = model.SendOSN;
			parameters[4].Value = model.SendState;
			parameters[5].Value = model.SendCount;
			parameters[6].Value = model.ShipSN;
			parameters[7].Value = model.ShipCoid;
			parameters[8].Value = model.ShipCoName;
			parameters[9].Value = model.ShipTime;
			parameters[10].Value = model.ShipFee;
			parameters[11].Value = model.RegionId;
			parameters[12].Value = model.Address;
			parameters[13].Value = model.Consignee;
			parameters[14].Value = model.Mobile;
			parameters[15].Value = model.BuyerRemark;
			parameters[16].Value = model.ReceivingTime;
			parameters[17].Value = model.IsExtendReceive;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters));
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(AgentSendOrderInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update hlh_agentsendorder set ");
			strSql.Append("creationdate=@creationdate,");
			strSql.Append("Pid=@Pid,");
			strSql.Append("uid=@uid,");
			strSql.Append("sendosn=@sendosn,");
			strSql.Append("sendstate=@sendstate,");
			strSql.Append("sendcount=@sendcount,");
			strSql.Append("shipsn=@shipsn,");
			strSql.Append("shipcoid=@shipcoid,");
			strSql.Append("shipconame=@shipconame,");
			strSql.Append("shiptime=@shiptime,");
			strSql.Append("shipfee=@shipfee,");
			strSql.Append("regionid=@regionid,");
			strSql.Append("address=@address,");
			strSql.Append("consignee=@consignee,");
			strSql.Append("mobile=@mobile,");
			strSql.Append("buyerremark=@buyerremark,");
			strSql.Append("receivingtime=@receivingtime,");
			strSql.Append("isextendreceive=@isextendreceive");
			strSql.Append(" where id=@id");
			SqlParameter[] parameters = {
					new SqlParameter("@creationdate", SqlDbType.DateTime),
					new SqlParameter("@Pid", SqlDbType.Int,4),
					new SqlParameter("@uid", SqlDbType.Int,4),
					new SqlParameter("@sendosn", SqlDbType.NChar,30),
					new SqlParameter("@sendstate", SqlDbType.Int,4),
					new SqlParameter("@sendcount", SqlDbType.Int,4),
					new SqlParameter("@shipsn", SqlDbType.NChar,30),
					new SqlParameter("@shipcoid", SqlDbType.SmallInt,2),
					new SqlParameter("@shipconame", SqlDbType.NVarChar,30),
					new SqlParameter("@shiptime", SqlDbType.DateTime),
					new SqlParameter("@shipfee", SqlDbType.Decimal,9),
					new SqlParameter("@regionid", SqlDbType.Int,4),
					new SqlParameter("@address", SqlDbType.NVarChar,150),
					new SqlParameter("@consignee", SqlDbType.NVarChar,20),
					new SqlParameter("@mobile", SqlDbType.VarChar,15),
					new SqlParameter("@buyerremark", SqlDbType.NVarChar,250),
					new SqlParameter("@receivingtime", SqlDbType.DateTime),
					new SqlParameter("@isextendreceive", SqlDbType.TinyInt,1),
					new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = model.CreationDate;
            parameters[1].Value = model.Pid;
            parameters[2].Value = model.Uid;
            parameters[3].Value = model.SendOSN;
            parameters[4].Value = model.SendState;
            parameters[5].Value = model.SendCount;
            parameters[6].Value = model.ShipSN;
            parameters[7].Value = model.ShipCoid;
            parameters[8].Value = model.ShipCoName;
            parameters[9].Value = model.ShipTime;
            parameters[10].Value = model.ShipFee;
            parameters[11].Value = model.RegionId;
            parameters[12].Value = model.Address;
            parameters[13].Value = model.Consignee;
            parameters[14].Value = model.Mobile;
            parameters[15].Value = model.BuyerRemark;
            parameters[16].Value = model.ReceivingTime;
            parameters[17].Value = model.IsExtendReceive;
			parameters[18].Value = model.Id;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int id)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from hlh_agentsendorder ");
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
			strSql.Append("delete from hlh_agentsendorder ");
			strSql.Append(" where id in ("+idlist + ")  ");

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public AgentSendOrderInfo GetModel(int id)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select  top 1 * from hlh_agentsendorder ");
			strSql.Append(" where id=@id");
			SqlParameter[] parameters = {
					new SqlParameter("@id", SqlDbType.Int,4)
			};
			parameters[0].Value = id;

			AgentSendOrderInfo model=new AgentSendOrderInfo();
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
        public AgentSendOrderInfo GetModel(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_agentsendorder ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }

            AgentSendOrderInfo model = new AgentSendOrderInfo();
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
		public AgentSendOrderInfo DataRowToModel(DataRow row)
		{
			AgentSendOrderInfo model=new AgentSendOrderInfo();
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
				if(row["Pid"]!=null && row["Pid"].ToString()!="")
				{
					model.Pid=int.Parse(row["Pid"].ToString());
				}
				if(row["uid"]!=null && row["uid"].ToString()!="")
				{
					model.Uid=int.Parse(row["uid"].ToString());
				}
				if(row["sendosn"]!=null)
				{
					model.SendOSN=row["sendosn"].ToString();
				}
				if(row["sendstate"]!=null && row["sendstate"].ToString()!="")
				{
					model.SendState=int.Parse(row["sendstate"].ToString());
				}
				if(row["sendcount"]!=null && row["sendcount"].ToString()!="")
				{
					model.SendCount=int.Parse(row["sendcount"].ToString());
				}
				if(row["shipsn"]!=null)
				{
					model.ShipSN=row["shipsn"].ToString();
				}
				if(row["shipcoid"]!=null && row["shipcoid"].ToString()!="")
				{
					model.ShipCoid=int.Parse(row["shipcoid"].ToString());
				}
				if(row["shipconame"]!=null)
				{
					model.ShipCoName=row["shipconame"].ToString();
				}
				if(row["shiptime"]!=null && row["shiptime"].ToString()!="")
				{
					model.ShipTime=DateTime.Parse(row["shiptime"].ToString());
				}
				if(row["shipfee"]!=null && row["shipfee"].ToString()!="")
				{
					model.ShipFee=decimal.Parse(row["shipfee"].ToString());
				}
				if(row["regionid"]!=null && row["regionid"].ToString()!="")
				{
					model.RegionId=int.Parse(row["regionid"].ToString());
				}
				if(row["address"]!=null)
				{
					model.Address=row["address"].ToString();
				}
				if(row["consignee"]!=null)
				{
					model.Consignee=row["consignee"].ToString();
				}
				if(row["mobile"]!=null)
				{
					model.Mobile=row["mobile"].ToString();
				}
				if(row["buyerremark"]!=null)
				{
					model.BuyerRemark=row["buyerremark"].ToString();
				}
				if(row["receivingtime"]!=null && row["receivingtime"].ToString()!="")
				{
					model.ReceivingTime=DateTime.Parse(row["receivingtime"].ToString());
				}
				if(row["isextendreceive"]!=null && row["isextendreceive"].ToString()!="")
				{
					model.IsExtendReceive=int.Parse(row["isextendreceive"].ToString());
				}
			}
			return model;
		}

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AgentSendOrderInfo BuildAgentSendOrderReader(IDataReader row)
        {
            AgentSendOrderInfo model = new AgentSendOrderInfo();
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
                if (row["Pid"] != null && row["Pid"].ToString() != "")
                {
                    model.Pid = int.Parse(row["Pid"].ToString());
                }
                if (row["uid"] != null && row["uid"].ToString() != "")
                {
                    model.Uid = int.Parse(row["uid"].ToString());
                }
                if (row["sendosn"] != null)
                {
                    model.SendOSN = row["sendosn"].ToString();
                }
                if (row["sendstate"] != null && row["sendstate"].ToString() != "")
                {
                    model.SendState = int.Parse(row["sendstate"].ToString());
                }
                if (row["sendcount"] != null && row["sendcount"].ToString() != "")
                {
                    model.SendCount = int.Parse(row["sendcount"].ToString());
                }
                if (row["shipsn"] != null)
                {
                    model.ShipSN = row["shipsn"].ToString();
                }
                if (row["shipcoid"] != null && row["shipcoid"].ToString() != "")
                {
                    model.ShipCoid = int.Parse(row["shipcoid"].ToString());
                }
                if (row["shipconame"] != null)
                {
                    model.ShipCoName = row["shipconame"].ToString();
                }
                if (row["shiptime"] != null && row["shiptime"].ToString() != "")
                {
                    model.ShipTime = DateTime.Parse(row["shiptime"].ToString());
                }
                if (row["shipfee"] != null && row["shipfee"].ToString() != "")
                {
                    model.ShipFee = decimal.Parse(row["shipfee"].ToString());
                }
                if (row["regionid"] != null && row["regionid"].ToString() != "")
                {
                    model.RegionId = int.Parse(row["regionid"].ToString());
                }
                if (row["address"] != null)
                {
                    model.Address = row["address"].ToString();
                }
                if (row["consignee"] != null)
                {
                    model.Consignee = row["consignee"].ToString();
                }
                if (row["mobile"] != null)
                {
                    model.Mobile = row["mobile"].ToString();
                }
                if (row["buyerremark"] != null)
                {
                    model.BuyerRemark = row["buyerremark"].ToString();
                }
                if (row["receivingtime"] != null && row["receivingtime"].ToString() != "")
                {
                    model.ReceivingTime = DateTime.Parse(row["receivingtime"].ToString());
                }
                if (row["isextendreceive"] != null && row["isextendreceive"].ToString() != "")
                {
                    model.IsExtendReceive = int.Parse(row["isextendreceive"].ToString());
                }
            }
            return model;
        }

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<AgentSendOrderInfo> GetList(string strWhere)
		{
            List<AgentSendOrderInfo> list = new List<AgentSendOrderInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select id,creationdate,Pid,uid,sendosn,sendstate,sendcount,shipsn,shipcoid,shipconame,shiptime,shipfee,regionid,address,consignee,mobile,buyerremark,receivingtime,isextendreceive ");
			strSql.Append(" FROM hlh_agentsendorder ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AgentSendOrderInfo info = BuildAgentSendOrderReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
		}

		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public List<AgentSendOrderInfo> GetList(int Top,string strWhere,string filedOrder)
		{
            List<AgentSendOrderInfo> list = new List<AgentSendOrderInfo>();
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ");
			if(Top>0)
			{
				strSql.Append(" top "+Top.ToString());
			}
			strSql.Append(" * ");
			strSql.Append(" FROM hlh_agentsendorder ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AgentSendOrderInfo info = BuildAgentSendOrderReader(reader);
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
			strSql.Append("select count(1) FROM hlh_agentsendorder T");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public List<AgentSendOrderInfo> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
            List<AgentSendOrderInfo> list = new List<AgentSendOrderInfo>();
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
			strSql.Append(")AS Row, T.*  from hlh_agentsendorder T ");
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
                    AgentSendOrderInfo info = BuildAgentSendOrderReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
		}
        /// <summary>
        /// 后台分页获取数据列表
        /// </summary>
        public DataTable AdminGetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            List<AgentSendOrderInfo> list = new List<AgentSendOrderInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.id desc");
            }
            strSql.Append(")AS Row, T.*,b.pid as ppid,b.name,b.storeid,b.showimg,c.uid as uuid,c.username,c.mobile as usermobile,c.agenttype  from hlh_agentsendorder T left join hlh_products b on T.pid=b.pid left join hlh_users c on T.uid=c.uid ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString()).Tables[0];
        }

		#endregion  BasicMethod
		#region  ExtensionMethod

		#endregion  ExtensionMethod
	}
}

