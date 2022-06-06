using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using VMall.Core;
using System.Linq;
using System.Collections.Generic;
//using Maticsoft.DBUtility;//Please add references
namespace VMall.Services
{
    /// <summary>
    /// 数据访问类:AdminOperateRights
    /// </summary>
    public partial class AdminOperateRights
    {
        public AdminOperateRights()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int RightsId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from hlh_AdminOperateRights");
            strSql.Append(" where RightsId=@RightsId");
            SqlParameter[] parameters = {
					new SqlParameter("@RightsId", SqlDbType.Int,4)
			};
            parameters[0].Value = RightsId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(AdminOperateRightsInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into hlh_AdminOperateRights(");
            strSql.Append("Operate,Aid,MallAGid,State)");
            strSql.Append(" values (");
            strSql.Append("@Operate,@Aid,@MallAGid,@State)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@Operate", SqlDbType.NVarChar,50),
					new SqlParameter("@Aid", SqlDbType.Int,4),
					new SqlParameter("@MallAGid", SqlDbType.Int,4),
                    new SqlParameter("@State", SqlDbType.Int,4)                    };
            parameters[0].Value = model.Operate;
            parameters[1].Value = model.Aid;
            parameters[2].Value = model.MallAGid;
            parameters[3].Value = model.State;
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters));
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(AdminOperateRightsInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update hlh_AdminOperateRights set ");
            strSql.Append("Operate=@Operate,");
            strSql.Append("Aid=@Aid,");
            strSql.Append("MallAGid=@MallAGid");
            strSql.Append("State=@State");
            strSql.Append(" where RightsId=@RightsId");
            SqlParameter[] parameters = {
					new SqlParameter("@Operate", SqlDbType.NVarChar,50),
					new SqlParameter("@Aid", SqlDbType.Int,4),
					new SqlParameter("@MallAGid", SqlDbType.Int,4),
                    new SqlParameter("@State", SqlDbType.Int,4),
					new SqlParameter("@RightsId", SqlDbType.Int,4)};
            parameters[0].Value = model.Operate;
            parameters[1].Value = model.Aid;
            parameters[2].Value = model.MallAGid;
            parameters[3].Value = model.State;
            parameters[4].Value = model.RightsId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }

        /// <summary>
        /// 批量更新状态
        /// </summary>
        public static bool UpdateState(int state, string RightsIdlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update hlh_AdminOperateRights set ");
            strSql.Append("State=@State");
            strSql.Append(" where RightsId in (" + RightsIdlist + ")  ");
            SqlParameter[] parameters = {
					new SqlParameter("@State", SqlDbType.Int,4)};
            parameters[0].Value = state;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }
        ///// <summary>
        ///// 批量更新数据
        ///// </summary>
        //public bool UpdateList(string RightsIdlist,string flied)
        //{
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("update  hlh_AdminOperateRights set flied");
        //    strSql.Append(" where RightsId in (" + RightsIdlist + ")  ");
        //    return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
        //}

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int RightsId)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_AdminOperateRights ");
            strSql.Append(" where RightsId=@RightsId");
            SqlParameter[] parameters = {
					new SqlParameter("@RightsId", SqlDbType.Int,4)
			};
            parameters[0].Value = RightsId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string RightsIdlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_AdminOperateRights ");
            strSql.Append(" where RightsId in (" + RightsIdlist + ")  ");
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AdminOperateRightsInfo GetModel(int RightsId)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 RightsId,Operate,Aid,MallAGid,State from hlh_AdminOperateRights ");
            strSql.Append(" where RightsId=@RightsId");
            SqlParameter[] parameters = {
					new SqlParameter("@RightsId", SqlDbType.Int,4)
			};
            parameters[0].Value = RightsId;

            AdminOperateRightsInfo model = new AdminOperateRightsInfo();
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
        public AdminOperateRightsInfo DataRowToModel(DataRow row)
        {
            AdminOperateRightsInfo model = new AdminOperateRightsInfo();
            if (row != null)
            {
                if (row["RightsId"] != null && row["RightsId"].ToString() != "")
                {
                    model.RightsId = int.Parse(row["RightsId"].ToString());
                }
                if (row["Operate"] != null)
                {
                    model.Operate = row["Operate"].ToString();
                }
                if (row["Aid"] != null && row["Aid"].ToString() != "")
                {
                    model.Aid = int.Parse(row["Aid"].ToString());
                }
                if (row["MallAGid"] != null && row["MallAGid"].ToString() != "")
                {
                    model.MallAGid = int.Parse(row["MallAGid"].ToString());
                }
                if (row["State"] != null && row["State"].ToString() != "")
                {
                    model.State = int.Parse(row["State"].ToString());
                }
            }
            return model;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AdminOperateRightsInfo BuildAdminOperateRightsInfoReader(IDataReader row)
        {
            AdminOperateRightsInfo model = new AdminOperateRightsInfo();
            if (row != null)
            {
                if (row["RightsId"] != null && row["RightsId"].ToString() != "")
                {
                    model.RightsId = int.Parse(row["RightsId"].ToString());
                }
                if (row["Operate"] != null)
                {
                    model.Operate = row["Operate"].ToString();
                }
                if (row["Aid"] != null && row["Aid"].ToString() != "")
                {
                    model.Aid = int.Parse(row["Aid"].ToString());
                }
                if (row["MallAGid"] != null && row["MallAGid"].ToString() != "")
                {
                    model.MallAGid = int.Parse(row["MallAGid"].ToString());
                }
                if (row["State"] != null && row["State"].ToString() != "")
                {
                    model.State = int.Parse(row["State"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<AdminOperateRightsInfo> GetList(string strWhere)
        {
            List<AdminOperateRightsInfo> list = new List<AdminOperateRightsInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select RightsId,Operate,Aid,MallAGid,State ");
            strSql.Append(" FROM hlh_AdminOperateRights ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AdminOperateRightsInfo info = BuildAdminOperateRightsInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public List<AdminOperateRightsInfo> GetList(int Top, string strWhere, string filedOrder)
        {
            List<AdminOperateRightsInfo> list = new List<AdminOperateRightsInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" RightsId,Operate,Aid,MallAGid,State ");
            strSql.Append(" FROM hlh_AdminOperateRights ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AdminOperateRightsInfo info = BuildAdminOperateRightsInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public List<AdminOperateRightsInfo> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            List<AdminOperateRightsInfo> list = new List<AdminOperateRightsInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.RightsId desc");
            }
            strSql.Append(")AS Row, T.*  from hlh_AdminOperateRights T ");
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
                    AdminOperateRightsInfo info = BuildAdminOperateRightsInfoReader(reader);
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
            strSql.Append("select count(1) FROM hlh_AdminOperateRights ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
        }

        #endregion  BasicMethod
        #region  ExtensionMethod
        /// <summary>
        /// 编辑权限
        /// </summary>
        /// <param name="selectRights"></param>
        /// <param name="noSelectRights"></param>
        public static void EditRights(List<int> selectRights, List<int> noSelectRights)
        {
            string selectStr = string.Join(",", selectRights.ToArray());
            string noSelectStr = string.Join(",", noSelectRights.ToArray());
            if (!string.IsNullOrEmpty(selectStr))
                UpdateState(1, selectStr);
            if (!string.IsNullOrEmpty(noSelectStr))
            UpdateState(0, noSelectStr);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="mallAGid"></param>
        /// <returns></returns>
        public List<AdminOperateRightsInfo> GetRightsByAction(string actionName, int mallAGid)
        {
            List<AdminOperateRightsInfo> list = new List<AdminOperateRightsInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT a.* FROM dbo.hlh_AdminOperateRights a LEFT JOIN dbo.hlh_malladminactions b ON a.Aid = b.aid WHERE b.[action]='" + actionName + "' AND a.[MallAGid]=" + mallAGid + " AND a.[State]=1");

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AdminOperateRightsInfo info = BuildAdminOperateRightsInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int BatchInsert(string operate, int aid)
        {
            string strSql = string.Format(@"INSERT INTO dbo.hlh_AdminOperateRights  ( Operate, Aid, MallAGid, State )
                               SELECT '{0}',{1},[mallagid],0 FROM dbo.hlh_malladmingroups WHERE mallagid>1", operate, aid);
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql));
        }

        #endregion  ExtensionMethod
    }
}

