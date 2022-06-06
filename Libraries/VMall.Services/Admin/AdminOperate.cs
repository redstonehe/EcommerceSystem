using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using VMall.Core;
using System.Collections.Generic;
//using Maticsoft.DBUtility;//Please add references
namespace VMall.Services
{
    /// <summary>
    /// 数据访问类:AdminOperate
    /// </summary>
    public partial class AdminOperate
    {
        public AdminOperate()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int OperateId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from hlh_AdminOperate");
            strSql.Append(" where OperateId=@OperateId");
            SqlParameter[] parameters = {
					new SqlParameter("@OperateId", SqlDbType.Int,4)
			};
            parameters[0].Value = OperateId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(AdminOperateInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into hlh_AdminOperate(");
            strSql.Append("Operate,OperateName,Aid)");
            strSql.Append(" values (");
            strSql.Append("@Operate,@OperateName,@Aid)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@Operate", SqlDbType.NVarChar,50),
					new SqlParameter("@OperateName", SqlDbType.NVarChar,50),
					new SqlParameter("@Aid", SqlDbType.Int,4)};
            parameters[0].Value = model.Operate;
            parameters[1].Value = model.OperateName;
            parameters[2].Value = model.Aid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters));
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(AdminOperateInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update hlh_AdminOperate set ");
            strSql.Append("Operate=@Operate,");
            strSql.Append("OperateName=@OperateName,");
            strSql.Append("Aid=@Aid");
            strSql.Append(" where OperateId=@OperateId");
            SqlParameter[] parameters = {
					new SqlParameter("@Operate", SqlDbType.NVarChar,50),
					new SqlParameter("@OperateName", SqlDbType.NVarChar,50),
					new SqlParameter("@Aid", SqlDbType.Int,4),
					new SqlParameter("@OperateId", SqlDbType.Int,4)};
            parameters[0].Value = model.Operate;
            parameters[1].Value = model.OperateName;
            parameters[2].Value = model.Aid;
            parameters[3].Value = model.OperateId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int OperateId)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_AdminOperate ");
            strSql.Append(" where OperateId=@OperateId");
            SqlParameter[] parameters = {
					new SqlParameter("@OperateId", SqlDbType.Int,4)
			};
            parameters[0].Value = OperateId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string OperateIdlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_AdminOperate ");
            strSql.Append(" where OperateId in (" + OperateIdlist + ")  ");
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AdminOperateInfo GetModel(int OperateId)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 OperateId,Operate,OperateName,Aid from hlh_AdminOperate ");
            strSql.Append(" where OperateId=@OperateId");
            SqlParameter[] parameters = {
					new SqlParameter("@OperateId", SqlDbType.Int,4)
			};
            parameters[0].Value = OperateId;

            AdminOperateInfo model = new AdminOperateInfo();
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
        public AdminOperateInfo GetModel(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_AdminOperate ");
            if (strWhere.Trim() != "")

                strSql.Append(" where " + strWhere);

            AdminOperateInfo model = new AdminOperateInfo();
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
        public AdminOperateInfo DataRowToModel(DataRow row)
        {
            AdminOperateInfo model = new AdminOperateInfo();
            if (row != null)
            {
                if (row["OperateId"] != null && row["OperateId"].ToString() != "")
                {
                    model.OperateId = int.Parse(row["OperateId"].ToString());
                }
                if (row["Operate"] != null)
                {
                    model.Operate = row["Operate"].ToString();
                }
                if (row["OperateName"] != null)
                {
                    model.OperateName = row["OperateName"].ToString();
                }
                if (row["Aid"] != null && row["Aid"].ToString() != "")
                {
                    model.Aid = int.Parse(row["Aid"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AdminOperateInfo BuilAdminOperateInfoReader(IDataReader row)
        {
            AdminOperateInfo model = new AdminOperateInfo();
            if (row != null)
            {
                if (row["OperateId"] != null && row["OperateId"].ToString() != "")
                {
                    model.OperateId = int.Parse(row["OperateId"].ToString());
                }
                if (row["Operate"] != null)
                {
                    model.Operate = row["Operate"].ToString();
                }
                if (row["OperateName"] != null)
                {
                    model.OperateName = row["OperateName"].ToString();
                }
                if (row["Aid"] != null && row["Aid"].ToString() != "")
                {
                    model.Aid = int.Parse(row["Aid"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<AdminOperateInfo> GetList(string strWhere)
        {
            List<AdminOperateInfo> list = new List<AdminOperateInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select OperateId,Operate,OperateName,Aid ");
            strSql.Append(" FROM hlh_AdminOperate ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AdminOperateInfo info = BuilAdminOperateInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public List<AdminOperateInfo> GetList(int Top, string strWhere, string filedOrder)
        {
            List<AdminOperateInfo> list = new List<AdminOperateInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" OperateId,Operate,OperateName,Aid ");
            strSql.Append(" FROM hlh_AdminOperate ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AdminOperateInfo info = BuilAdminOperateInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public List<AdminOperateInfo> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            List<AdminOperateInfo> list = new List<AdminOperateInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.OperateId desc");
            }
            strSql.Append(")AS Row, T.*  from hlh_AdminOperate T ");
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
                    AdminOperateInfo info = BuilAdminOperateInfoReader(reader);
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
            strSql.Append("select count(1) FROM hlh_AdminOperate ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
        }

        #endregion  BasicMethod
        #region  ExtensionMethod

        #endregion  ExtensionMethod
    }
}

