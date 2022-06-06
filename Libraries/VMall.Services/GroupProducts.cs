using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using VMall.Core;
using System.Collections.Generic;

namespace VMall.Services
{
    /// <summary>
    /// 数据访问类:GroupProducts
    /// </summary>
    public partial class GroupProducts
    {
        public GroupProducts()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int Groupid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from hlh_GroupProduct");
            strSql.Append(" where Groupid=@Groupid");
            SqlParameter[] parameters = {
					new SqlParameter("@Groupid", SqlDbType.Int,4)
			};
            parameters[0].Value = Groupid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(GroupProductInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into hlh_GroupProduct(");
            strSql.Append("CreationTime,GroupTitle,GroupLogo,ChannelId,State,StartTime,EndTime,DisplayOrder,Type,Link,Products,ExtField1,ExtField2,ExtField3,ExtField4,ExtField5)");
            strSql.Append(" values (");
            strSql.Append("@CreationTime,@GroupTitle,@GroupLogo,@ChannelId,@State,@StartTime,@EndTime,@DisplayOrder,@Type,@Link,@Products,@ExtField1,@ExtField2,@ExtField3,@ExtField4,@ExtField5)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@CreationTime", SqlDbType.DateTime),
					new SqlParameter("@GroupTitle", SqlDbType.NVarChar,50),
					new SqlParameter("@GroupLogo", SqlDbType.NVarChar,200),
					new SqlParameter("@ChannelId", SqlDbType.Int,4),
					new SqlParameter("@State", SqlDbType.TinyInt,1),
					new SqlParameter("@StartTime", SqlDbType.DateTime),
					new SqlParameter("@EndTime", SqlDbType.DateTime),
					new SqlParameter("@DisplayOrder", SqlDbType.Int,4),
					new SqlParameter("@Type", SqlDbType.TinyInt,1),
					new SqlParameter("@Link", SqlDbType.NVarChar,200),
					new SqlParameter("@Products", SqlDbType.NVarChar,-1),
					new SqlParameter("@ExtField1", SqlDbType.NVarChar,500),
					new SqlParameter("@ExtField2", SqlDbType.NVarChar,500),
					new SqlParameter("@ExtField3", SqlDbType.NVarChar,500),
					new SqlParameter("@ExtField4", SqlDbType.NVarChar,500),
					new SqlParameter("@ExtField5", SqlDbType.NVarChar,500)};
            parameters[0].Value = model.CreationTime;
            parameters[1].Value = model.GroupTitle;
            parameters[2].Value = model.GroupLogo;
            parameters[3].Value = model.ChannelId;
            parameters[4].Value = model.State;
            parameters[5].Value = model.StartTime;
            parameters[6].Value = model.EndTime;
            parameters[7].Value = model.DisplayOrder;
            parameters[8].Value = model.Type;
            parameters[9].Value = model.Link;
            parameters[10].Value = model.Products;
            parameters[11].Value = model.ExtField1;
            parameters[12].Value = model.ExtField2;
            parameters[13].Value = model.ExtField3;
            parameters[14].Value = model.ExtField4;
            parameters[15].Value = model.ExtField5;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters));
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(GroupProductInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update hlh_GroupProduct set ");
            strSql.Append("CreationTime=@CreationTime,");
            strSql.Append("GroupTitle=@GroupTitle,");
            strSql.Append("GroupLogo=@GroupLogo,");
            strSql.Append("ChannelId=@ChannelId,");
            strSql.Append("State=@State,");
            strSql.Append("StartTime=@StartTime,");
            strSql.Append("EndTime=@EndTime,");
            strSql.Append("DisplayOrder=@DisplayOrder,");
            strSql.Append("Type=@Type,");
            strSql.Append("Link=@Link,");
            strSql.Append("Products=@Products,");
            strSql.Append("ExtField1=@ExtField1,");
            strSql.Append("ExtField2=@ExtField2,");
            strSql.Append("ExtField3=@ExtField3,");
            strSql.Append("ExtField4=@ExtField4,");
            strSql.Append("ExtField5=@ExtField5");
            strSql.Append(" where Groupid=@Groupid");
            SqlParameter[] parameters = {
					new SqlParameter("@CreationTime", SqlDbType.DateTime),
					new SqlParameter("@GroupTitle", SqlDbType.NVarChar,50),
					new SqlParameter("@GroupLogo", SqlDbType.NVarChar,200),
					new SqlParameter("@ChannelId", SqlDbType.Int,4),
					new SqlParameter("@State", SqlDbType.TinyInt,1),
					new SqlParameter("@StartTime", SqlDbType.DateTime),
					new SqlParameter("@EndTime", SqlDbType.DateTime),
					new SqlParameter("@DisplayOrder", SqlDbType.Int,4),
					new SqlParameter("@Type", SqlDbType.TinyInt,1),
					new SqlParameter("@Link", SqlDbType.NVarChar,200),
					new SqlParameter("@Products", SqlDbType.NVarChar,-1),
					new SqlParameter("@ExtField1", SqlDbType.NVarChar,500),
					new SqlParameter("@ExtField2", SqlDbType.NVarChar,500),
					new SqlParameter("@ExtField3", SqlDbType.NVarChar,500),
					new SqlParameter("@ExtField4", SqlDbType.NVarChar,500),
					new SqlParameter("@ExtField5", SqlDbType.NVarChar,500),
					new SqlParameter("@Groupid", SqlDbType.Int,4)};
            parameters[0].Value = model.CreationTime;
            parameters[1].Value = model.GroupTitle;
            parameters[2].Value = model.GroupLogo;
            parameters[3].Value = model.ChannelId;
            parameters[4].Value = model.State;
            parameters[5].Value = model.StartTime;
            parameters[6].Value = model.EndTime;
            parameters[7].Value = model.DisplayOrder;
            parameters[8].Value = model.Type;
            parameters[9].Value = model.Link;
            parameters[10].Value = model.Products;
            parameters[11].Value = model.ExtField1;
            parameters[12].Value = model.ExtField2;
            parameters[13].Value = model.ExtField3;
            parameters[14].Value = model.ExtField4;
            parameters[15].Value = model.ExtField5;
            parameters[16].Value = model.Groupid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int Groupid)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_GroupProduct ");
            strSql.Append(" where Groupid=@Groupid");
            SqlParameter[] parameters = {
					new SqlParameter("@Groupid", SqlDbType.Int,4)
			};
            parameters[0].Value = Groupid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string Groupidlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_GroupProduct ");
            strSql.Append(" where Groupid in (" + Groupidlist + ")  ");
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public GroupProductInfo GetModel(int Groupid)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 Groupid,CreationTime,GroupTitle,GroupLogo,ChannelId,State,StartTime,EndTime,DisplayOrder,Type,Link,Products,ExtField1,ExtField2,ExtField3,ExtField4,ExtField5 from hlh_GroupProduct ");
            strSql.Append(" where Groupid=@Groupid");
            SqlParameter[] parameters = {
					new SqlParameter("@Groupid", SqlDbType.Int,4)
			};
            parameters[0].Value = Groupid;

            GroupProductInfo model = new GroupProductInfo();
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
        public GroupProductInfo GetModel(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_GroupProduct ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }

            GroupProductInfo model = new GroupProductInfo();
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
        public static GroupProductInfo DataRowToModel(DataRow row)
        {
            GroupProductInfo model = new GroupProductInfo();
            if (row != null)
            {
                if (row["Groupid"] != null && row["Groupid"].ToString() != "")
                {
                    model.Groupid = int.Parse(row["Groupid"].ToString());
                }
                if (row["CreationTime"] != null && row["CreationTime"].ToString() != "")
                {
                    model.CreationTime = DateTime.Parse(row["CreationTime"].ToString());
                }
                if (row["GroupTitle"] != null)
                {
                    model.GroupTitle = row["GroupTitle"].ToString();
                }
                if (row["GroupLogo"] != null)
                {
                    model.GroupLogo = row["GroupLogo"].ToString();
                }
                if (row["ChannelId"] != null && row["ChannelId"].ToString() != "")
                {
                    model.ChannelId = int.Parse(row["ChannelId"].ToString());
                }
                if (row["State"] != null && row["State"].ToString() != "")
                {
                    model.State = int.Parse(row["State"].ToString());
                }
                if (row["StartTime"] != null && row["StartTime"].ToString() != "")
                {
                    model.StartTime = DateTime.Parse(row["StartTime"].ToString());
                }
                if (row["EndTime"] != null && row["EndTime"].ToString() != "")
                {
                    model.EndTime = DateTime.Parse(row["EndTime"].ToString());
                }
                if (row["DisplayOrder"] != null && row["DisplayOrder"].ToString() != "")
                {
                    model.DisplayOrder = int.Parse(row["DisplayOrder"].ToString());
                }
                if (row["Type"] != null && row["Type"].ToString() != "")
                {
                    model.Type = int.Parse(row["Type"].ToString());
                }
                if (row["Link"] != null)
                {
                    model.Link = row["Link"].ToString();
                }
                if (row["Products"] != null)
                {
                    model.Products = row["Products"].ToString();
                }
                if (row["ExtField1"] != null)
                {
                    model.ExtField1 = row["ExtField1"].ToString();
                }
                if (row["ExtField2"] != null)
                {
                    model.ExtField2 = row["ExtField2"].ToString();
                }
                if (row["ExtField3"] != null)
                {
                    model.ExtField3 = row["ExtField3"].ToString();
                }
                if (row["ExtField4"] != null)
                {
                    model.ExtField4 = row["ExtField4"].ToString();
                }
                if (row["ExtField5"] != null)
                {
                    model.ExtField5 = row["ExtField5"].ToString();
                }
            }
            return model;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static GroupProductInfo BuildGroupProductInfoReader(IDataReader row)
        {
            GroupProductInfo model = new GroupProductInfo();
            if (row != null)
            {
                if (row["Groupid"] != null && row["Groupid"].ToString() != "")
                {
                    model.Groupid = int.Parse(row["Groupid"].ToString());
                }
                if (row["CreationTime"] != null && row["CreationTime"].ToString() != "")
                {
                    model.CreationTime = DateTime.Parse(row["CreationTime"].ToString());
                }
                if (row["GroupTitle"] != null)
                {
                    model.GroupTitle = row["GroupTitle"].ToString();
                }
                if (row["GroupLogo"] != null)
                {
                    model.GroupLogo = row["GroupLogo"].ToString();
                }
                if (row["ChannelId"] != null && row["ChannelId"].ToString() != "")
                {
                    model.ChannelId = int.Parse(row["ChannelId"].ToString());
                }
                if (row["State"] != null && row["State"].ToString() != "")
                {
                    model.State = int.Parse(row["State"].ToString());
                }
                if (row["StartTime"] != null && row["StartTime"].ToString() != "")
                {
                    model.StartTime = DateTime.Parse(row["StartTime"].ToString());
                }
                if (row["EndTime"] != null && row["EndTime"].ToString() != "")
                {
                    model.EndTime = DateTime.Parse(row["EndTime"].ToString());
                }
                if (row["DisplayOrder"] != null && row["DisplayOrder"].ToString() != "")
                {
                    model.DisplayOrder = int.Parse(row["DisplayOrder"].ToString());
                }
                if (row["Type"] != null && row["Type"].ToString() != "")
                {
                    model.Type = int.Parse(row["Type"].ToString());
                }
                if (row["Link"] != null)
                {
                    model.Link = row["Link"].ToString();
                }
                if (row["Products"] != null)
                {
                    model.Products = row["Products"].ToString();
                }
                if (row["ExtField1"] != null)
                {
                    model.ExtField1 = row["ExtField1"].ToString();
                }
                if (row["ExtField2"] != null)
                {
                    model.ExtField2 = row["ExtField2"].ToString();
                }
                if (row["ExtField3"] != null)
                {
                    model.ExtField3 = row["ExtField3"].ToString();
                }
                if (row["ExtField4"] != null)
                {
                    model.ExtField4 = row["ExtField4"].ToString();
                }
                if (row["ExtField5"] != null)
                {
                    model.ExtField5 = row["ExtField5"].ToString();
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<GroupProductInfo> GetList(string strWhere)
        {
            List<GroupProductInfo> list = new List<GroupProductInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Groupid,CreationTime,GroupTitle,GroupLogo,ChannelId,State,StartTime,EndTime,DisplayOrder,Type,Link,Products,ExtField1,ExtField2,ExtField3,ExtField4,ExtField5 ");
            strSql.Append(" FROM hlh_GroupProduct ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    GroupProductInfo info = BuildGroupProductInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public List<GroupProductInfo> GetList(int Top, string strWhere, string filedOrder)
        {
            List<GroupProductInfo> list = new List<GroupProductInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" Groupid,CreationTime,GroupTitle,GroupLogo,ChannelId,State,StartTime,EndTime,DisplayOrder,Type,Link,Products,ExtField1,ExtField2,ExtField3,ExtField4,ExtField5 ");
            strSql.Append(" FROM hlh_GroupProduct ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    GroupProductInfo info = BuildGroupProductInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public List<GroupProductInfo> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            List<GroupProductInfo> list = new List<GroupProductInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.Groupid desc");
            }
            strSql.Append(")AS Row, T.*  from hlh_GroupProduct T ");
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
                    GroupProductInfo info = BuildGroupProductInfoReader(reader);
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_GroupProduct ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
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
            parameters[0].Value = "hlh_GroupProduct";
            parameters[1].Value = "Groupid";
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = 0;
            parameters[5].Value = 0;
            parameters[6].Value = strWhere;	
            return DbHelperSQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
        }*/

        #endregion  BasicMethod
        #region  ExtensionMethod

        public List<PartProductInfo> getGroupProductList(string strWhere, string orderby, int startIndex, int endIndex)
        {
            List<PartProductInfo> list = new List<PartProductInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.pid desc");
            }
            strSql.Append(")AS Row, T.*  from hlh_Products T ");
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
                    PartProductInfo info = VMall.Data.Products.BuildPartProductFromReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }

        #endregion  ExtensionMethod
    }
}

