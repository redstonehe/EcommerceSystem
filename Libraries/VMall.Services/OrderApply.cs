using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using VMall.Core;
using Maticsoft.DBUtility;//Please add references
namespace VMall.Services
{
    /// <summary>
    /// 数据访问类:OrderApply
    /// </summary>
    public partial class OrderApply
    {
        public OrderApply()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from hlh_orderapply");
            strSql.Append(" where id=@id");
            SqlParameter[] parameters = {
					new SqlParameter("@id", SqlDbType.Int,4)
			};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(OrderApplyInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into hlh_orderapply(");
            strSql.Append("creationdate,lastmodified,operateuid,pid,usercode,realname,idcard,parentcode,managercode,placeside,consignee,consignmobile,regionid,address,payimg,adminoperuid,state,detaildesc,resultoid,resultosn)");
            strSql.Append(" values (");
            strSql.Append("@creationdate,@lastmodified,@operateuid,@pid,@usercode,@realname,@idcard,@parentcode,@managercode,@placeside,@consignee,@consignmobile,@regionid,@address,@payimg,@adminoperuid,@state,@detaildesc,@resultoid,@resultosn)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@creationdate", SqlDbType.DateTime),
					new SqlParameter("@lastmodified", SqlDbType.DateTime),
					new SqlParameter("@operateuid", SqlDbType.Int,4),
					new SqlParameter("@pid", SqlDbType.Int,4),
					new SqlParameter("@usercode", SqlDbType.VarChar,20),
					new SqlParameter("@realname", SqlDbType.NVarChar,10),
					new SqlParameter("@idcard", SqlDbType.VarChar,18),
					new SqlParameter("@parentcode", SqlDbType.VarChar,20),
					new SqlParameter("@managercode", SqlDbType.VarChar,20),
					new SqlParameter("@placeside", SqlDbType.TinyInt,1),
					new SqlParameter("@consignee", SqlDbType.NVarChar,20),
					new SqlParameter("@consignmobile", SqlDbType.VarChar,15),
					new SqlParameter("@regionid", SqlDbType.Int,4),
					new SqlParameter("@address", SqlDbType.NVarChar,150),
					new SqlParameter("@payimg", SqlDbType.VarChar,200),
					new SqlParameter("@adminoperuid", SqlDbType.Int,4),
					new SqlParameter("@state", SqlDbType.TinyInt,1),
					new SqlParameter("@detaildesc", SqlDbType.NVarChar,200),
					new SqlParameter("@resultoid", SqlDbType.Int,4),
					new SqlParameter("@resultosn", SqlDbType.NChar,32)};
            parameters[0].Value = model.creationdate;
            parameters[1].Value = model.lastmodified;
            parameters[2].Value = model.operateuid;
            parameters[3].Value = model.pid;
            parameters[4].Value = model.usercode;
            parameters[5].Value = model.realname;
            parameters[6].Value = model.idcard;
            parameters[7].Value = model.parentcode;
            parameters[8].Value = model.managercode;
            parameters[9].Value = model.placeside;
            parameters[10].Value = model.consignee;
            parameters[11].Value = model.consignmobile;
            parameters[12].Value = model.regionid;
            parameters[13].Value = model.address;
            parameters[14].Value = model.payimg;
            parameters[15].Value = model.adminoperuid;
            parameters[16].Value = model.state;
            parameters[17].Value = model.detaildesc;
            parameters[18].Value = model.resultoid;
            parameters[19].Value = model.resultosn;

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
        /// 更新一条数据
        /// </summary>
        public bool Update(OrderApplyInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update hlh_orderapply set ");
            strSql.Append("creationdate=@creationdate,");
            strSql.Append("lastmodified=@lastmodified,");
            strSql.Append("operateuid=@operateuid,");
            strSql.Append("pid=@pid,");
            strSql.Append("usercode=@usercode,");
            strSql.Append("realname=@realname,");
            strSql.Append("idcard=@idcard,");
            strSql.Append("parentcode=@parentcode,");
            strSql.Append("managercode=@managercode,");
            strSql.Append("placeside=@placeside,");
            strSql.Append("consignee=@consignee,");
            strSql.Append("consignmobile=@consignmobile,");
            strSql.Append("regionid=@regionid,");
            strSql.Append("address=@address,");
            strSql.Append("payimg=@payimg,");
            strSql.Append("adminoperuid=@adminoperuid,");
            strSql.Append("state=@state,");
            strSql.Append("detaildesc=@detaildesc,");
            strSql.Append("resultoid=@resultoid,");
            strSql.Append("resultosn=@resultosn");
            strSql.Append(" where id=@id");
            SqlParameter[] parameters = {
					new SqlParameter("@creationdate", SqlDbType.DateTime),
					new SqlParameter("@lastmodified", SqlDbType.DateTime),
					new SqlParameter("@operateuid", SqlDbType.Int,4),
					new SqlParameter("@pid", SqlDbType.Int,4),
					new SqlParameter("@usercode", SqlDbType.VarChar,20),
					new SqlParameter("@realname", SqlDbType.NVarChar,10),
					new SqlParameter("@idcard", SqlDbType.VarChar,18),
					new SqlParameter("@parentcode", SqlDbType.VarChar,20),
					new SqlParameter("@managercode", SqlDbType.VarChar,20),
					new SqlParameter("@placeside", SqlDbType.TinyInt,1),
					new SqlParameter("@consignee", SqlDbType.NVarChar,20),
					new SqlParameter("@consignmobile", SqlDbType.VarChar,15),
					new SqlParameter("@regionid", SqlDbType.Int,4),
					new SqlParameter("@address", SqlDbType.NVarChar,150),
					new SqlParameter("@payimg", SqlDbType.VarChar,200),
					new SqlParameter("@adminoperuid", SqlDbType.Int,4),
					new SqlParameter("@state", SqlDbType.TinyInt,1),
					new SqlParameter("@detaildesc", SqlDbType.NVarChar,200),
					new SqlParameter("@resultoid", SqlDbType.Int,4),
					new SqlParameter("@resultosn", SqlDbType.NChar,32),
					new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = model.creationdate;
            parameters[1].Value = model.lastmodified;
            parameters[2].Value = model.operateuid;
            parameters[3].Value = model.pid;
            parameters[4].Value = model.usercode;
            parameters[5].Value = model.realname;
            parameters[6].Value = model.idcard;
            parameters[7].Value = model.parentcode;
            parameters[8].Value = model.managercode;
            parameters[9].Value = model.placeside;
            parameters[10].Value = model.consignee;
            parameters[11].Value = model.consignmobile;
            parameters[12].Value = model.regionid;
            parameters[13].Value = model.address;
            parameters[14].Value = model.payimg;
            parameters[15].Value = model.adminoperuid;
            parameters[16].Value = model.state;
            parameters[17].Value = model.detaildesc;
            parameters[18].Value = model.resultoid;
            parameters[19].Value = model.resultosn;
            parameters[20].Value = model.id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
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
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_orderapply ");
            strSql.Append(" where id=@id");
            SqlParameter[] parameters = {
					new SqlParameter("@id", SqlDbType.Int,4)
			};
            parameters[0].Value = id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
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
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string idlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_orderapply ");
            strSql.Append(" where id in (" + idlist + ")  ");
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
        /// 得到一个对象实体
        /// </summary>
        public OrderApplyInfo GetModel(int id)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_orderapply ");
            strSql.Append(" where id=@id");
            SqlParameter[] parameters = {
					new SqlParameter("@id", SqlDbType.Int,4)
			};
            parameters[0].Value = id;

            OrderApplyInfo model = new OrderApplyInfo();
            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
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
        public OrderApplyInfo GetModel(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_orderapply ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }

            OrderApplyInfo model = new OrderApplyInfo();
            DataSet ds = DbHelperSQL.Query(strSql.ToString());
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
        public OrderApplyInfo DataRowToModel(DataRow row)
        {
            OrderApplyInfo model = new OrderApplyInfo();
            if (row != null)
            {
                if (row["id"] != null && row["id"].ToString() != "")
                {
                    model.id = int.Parse(row["id"].ToString());
                }
                if (row["creationdate"] != null && row["creationdate"].ToString() != "")
                {
                    model.creationdate = DateTime.Parse(row["creationdate"].ToString());
                }
                if (row["lastmodified"] != null && row["lastmodified"].ToString() != "")
                {
                    model.lastmodified = DateTime.Parse(row["lastmodified"].ToString());
                }
                if (row["operateuid"] != null && row["operateuid"].ToString() != "")
                {
                    model.operateuid = int.Parse(row["operateuid"].ToString());
                }
                if (row["pid"] != null && row["pid"].ToString() != "")
                {
                    model.pid = int.Parse(row["pid"].ToString());
                }
                if (row["usercode"] != null)
                {
                    model.usercode = row["usercode"].ToString();
                }
                if (row["realname"] != null)
                {
                    model.realname = row["realname"].ToString();
                }
                if (row["idcard"] != null)
                {
                    model.idcard = row["idcard"].ToString();
                }
                if (row["parentcode"] != null)
                {
                    model.parentcode = row["parentcode"].ToString();
                }
                if (row["managercode"] != null)
                {
                    model.managercode = row["managercode"].ToString();
                }
                if (row["placeside"] != null && row["placeside"].ToString() != "")
                {
                    model.placeside = int.Parse(row["placeside"].ToString());
                }
                if (row["consignee"] != null)
                {
                    model.consignee = row["consignee"].ToString();
                }
                if (row["consignmobile"] != null)
                {
                    model.consignmobile = row["consignmobile"].ToString();
                }
                if (row["regionid"] != null && row["regionid"].ToString() != "")
                {
                    model.regionid = int.Parse(row["regionid"].ToString());
                }
                if (row["address"] != null)
                {
                    model.address = row["address"].ToString();
                }
                if (row["payimg"] != null)
                {
                    model.payimg = row["payimg"].ToString();
                }
                if (row["adminoperuid"] != null && row["adminoperuid"].ToString() != "")
                {
                    model.adminoperuid = int.Parse(row["adminoperuid"].ToString());
                }
                if (row["state"] != null && row["state"].ToString() != "")
                {
                    model.state = int.Parse(row["state"].ToString());
                }
                if (row["detaildesc"] != null)
                {
                    model.detaildesc = row["detaildesc"].ToString();
                }
                if (row["resultoid"] != null && row["resultoid"].ToString() != "")
                {
                    model.resultoid = int.Parse(row["resultoid"].ToString());
                }
                if (row["resultosn"] != null)
                {
                    model.resultosn = row["resultosn"].ToString();
                }
            }
            return model;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public OrderApplyInfo BuildOrderApplyReader(IDataReader row)
        {
            OrderApplyInfo model = new OrderApplyInfo();
            if (row != null)
            {
                if (row["id"] != null && row["id"].ToString() != "")
                {
                    model.id = int.Parse(row["id"].ToString());
                }
                if (row["creationdate"] != null && row["creationdate"].ToString() != "")
                {
                    model.creationdate = DateTime.Parse(row["creationdate"].ToString());
                }
                if (row["lastmodified"] != null && row["lastmodified"].ToString() != "")
                {
                    model.lastmodified = DateTime.Parse(row["lastmodified"].ToString());
                }
                if (row["operateuid"] != null && row["operateuid"].ToString() != "")
                {
                    model.operateuid = int.Parse(row["operateuid"].ToString());
                }
                if (row["pid"] != null && row["pid"].ToString() != "")
                {
                    model.pid = int.Parse(row["pid"].ToString());
                }
                if (row["usercode"] != null)
                {
                    model.usercode = row["usercode"].ToString();
                }
                if (row["realname"] != null)
                {
                    model.realname = row["realname"].ToString();
                }
                if (row["idcard"] != null)
                {
                    model.idcard = row["idcard"].ToString();
                }
                if (row["parentcode"] != null)
                {
                    model.parentcode = row["parentcode"].ToString();
                }
                if (row["managercode"] != null)
                {
                    model.managercode = row["managercode"].ToString();
                }
                if (row["placeside"] != null && row["placeside"].ToString() != "")
                {
                    model.placeside = int.Parse(row["placeside"].ToString());
                }
                if (row["consignee"] != null)
                {
                    model.consignee = row["consignee"].ToString();
                }
                if (row["consignmobile"] != null)
                {
                    model.consignmobile = row["consignmobile"].ToString();
                }
                if (row["regionid"] != null && row["regionid"].ToString() != "")
                {
                    model.regionid = int.Parse(row["regionid"].ToString());
                }
                if (row["address"] != null)
                {
                    model.address = row["address"].ToString();
                }
                if (row["payimg"] != null)
                {
                    model.payimg = row["payimg"].ToString();
                }
                if (row["adminoperuid"] != null && row["adminoperuid"].ToString() != "")
                {
                    model.adminoperuid = int.Parse(row["adminoperuid"].ToString());
                }
                if (row["state"] != null && row["state"].ToString() != "")
                {
                    model.state = int.Parse(row["state"].ToString());
                }
                if (row["detaildesc"] != null)
                {
                    model.detaildesc = row["detaildesc"].ToString();
                }
                if (row["resultoid"] != null && row["resultoid"].ToString() != "")
                {
                    model.resultoid = int.Parse(row["resultoid"].ToString());
                }
                if (row["resultosn"] != null)
                {
                    model.resultosn = row["resultosn"].ToString();
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,creationdate,lastmodified,operateuid,pid,usercode,realname,idcard,parentcode,managercode,placeside,consignee,consignmobile,regionid,address,payimg,adminoperuid,state,detaildesc,resultoid,resultosn ");
            strSql.Append(" FROM hlh_orderapply ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" id,creationdate,lastmodified,operateuid,pid,usercode,realname,idcard,parentcode,managercode,placeside,consignee,consignmobile,regionid,address,payimg,adminoperuid,state,detaildesc,resultoid,resultosn ");
            strSql.Append(" FROM hlh_orderapply ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_orderapply T");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            object obj = DbHelperSQL.GetSingle(strSql.ToString());
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
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
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
            strSql.Append(")AS Row, T.*  from hlh_orderapply T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 后台分页获取数据列表
        /// </summary>
        public DataTable AdminGetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
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
            strSql.Append(")AS Row, T.*,b.pid as ppid,b.name,b.storeid,b.showimg,c.uid as uuid,c.username,c.mobile as usermobile,c.agenttype  from hlh_orderapply T  left join hlh_products b on T.pid=b.pid left join hlh_users c on T.operateuid=c.uid  ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            //DbHelperSQL.
            return DbHelperSQL.Query(strSql.ToString()).Tables[0];
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
            parameters[0].Value = "hlh_orderapply";
            parameters[1].Value = "id";
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

