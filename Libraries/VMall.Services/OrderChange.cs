using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMall.Core;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace VMall.Services
{
    public class OrderChange
    {
        /// <summary>
        /// 创建换货订单
        /// </summary>
        /// <param name="orderChange">换货model</param>
        public static void CreateOrderChange(OrderChangeInfo orderChange)
        {
            DbParameter[] parms = new DbParameter[] {
                                            SqlHelper.CreateInParam("@creationdate", SqlDbType.DateTime, 8, orderChange.CreationDate),
                                            SqlHelper.CreateInParam("@lastmodifity", SqlDbType.DateTime, 8, orderChange.LastModifity),
                                            SqlHelper.CreateInParam("@storeid", SqlDbType.Int, 4, orderChange.StoreId),
                                            SqlHelper.CreateInParam("@storename", SqlDbType.NVarChar, 60, orderChange.StoreName),
                                            SqlHelper.CreateInParam("@oid", SqlDbType.Int, 4, orderChange.Oid),
                                            SqlHelper.CreateInParam("@osn", SqlDbType.VarChar, 30, orderChange.OSN),
                                            SqlHelper.CreateInParam("@uid", SqlDbType.Int, 4, orderChange.Uid),
                                            SqlHelper.CreateInParam("@state", SqlDbType.TinyInt, 1, orderChange.State),
                                            SqlHelper.CreateInParam("@changetype", SqlDbType.TinyInt, 1, orderChange.ChangeType),
                                            SqlHelper.CreateInParam("@said", SqlDbType.Int, 4, orderChange.SAId),
                                            SqlHelper.CreateInParam("@changeoid", SqlDbType.Int, 4, orderChange.ChangeOid),
                                            SqlHelper.CreateInParam("@changeosn", SqlDbType.VarChar, 30, orderChange.ChangeOSN),
                                            SqlHelper.CreateInParam("@changedesc", SqlDbType.NVarChar, 300, orderChange.ChangeDesc)
            };
            string sqlStr = @"INSERT INTO [hlh_orderchange] ([creationdate],[lastmodifity],[storeid],[storename],[oid],[osn],[uid],[state],[changetype],[said],[changeoid],[changeosn],[changedesc])  VALUES (@creationdate,@lastmodifity,@storeid,@storename,@oid,@osn,@uid,@state,@changetype,@said,@changeoid,@changeosn,@changedesc) ";
            RDBSHelper.ExecuteScalar(CommandType.Text, sqlStr,parms);
        }

        /// <summary>
        /// 获取换货订单列表
        /// </summary>
        /// <param name="state">处理状态</param>
        /// <returns></returns>
        public static List<OrderChangeInfo> GetOrderChangeList(int pageSize, int pageNumber, string condition) 
        {
            List<OrderChangeInfo> changeInfoList = new List<OrderChangeInfo>();
            bool noCondition = string.IsNullOrWhiteSpace(condition);
            string commandText ;
            if (pageNumber == 1)
            {
                if (noCondition)
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}orderchange] ORDER BY [changeid] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre);

                else
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}orderchange] WHERE {2} ORDER BY [changeid] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                condition);
            }
            else
            {
                if (noCondition)
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}orderchange] WHERE [changeid] < (SELECT MIN([changeid]) FROM (SELECT TOP {2} [changeid] FROM [{1}orderchange] ORDER BY [changeid] DESC) AS [temp]) ORDER BY [changeid] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNumber - 1) * pageSize);
                else
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}orderchange] WHERE [changeid] < (SELECT MIN([changeid]) FROM (SELECT TOP {2} [changeid] FROM [{1}orderchange] WHERE {3} ORDER BY [changeid] DESC) AS [temp]) AND {3} ORDER BY [changeid] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNumber - 1) * pageSize,
                                                condition);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    OrderChangeInfo changeInfo = BuildOrderChangeInfoFromReader(reader);
                    changeInfoList.Add(changeInfo);
                }
                reader.Close();
            }
            return changeInfoList;
        }


        /// <summary>
        /// 根据oid取得换货记录
        /// </summary>
        /// <param name="changeid">记录id</param>
        /// <returns></returns>
        public static OrderChangeInfo GetChangeOrderByOid(int oid)
        {
            OrderChangeInfo changeInfo = null;
            DbParameter[] parms =  { 
                                         SqlHelper.CreateInParam("@oid", SqlDbType.Int, 4, oid)
                                    };
            string commandText = string.Format("SELECT * FROM [{0}orderchange] WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);

            IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms);
            if (reader.Read())
            {
                changeInfo = BuildOrderChangeInfoFromReader(reader);
            }
            reader.Close();
            return changeInfo;
        }

        /// <summary>
        /// 根据changeid取得换货记录
        /// </summary>
        /// <param name="changeid">记录id</param>
        /// <returns></returns>
        public static OrderChangeInfo GetChangeOrderByid(int changeid)
        {
            OrderChangeInfo changeInfo = null;
            DbParameter[] parms =  { 
                                         SqlHelper.CreateInParam("@changeid", SqlDbType.Int, 4, changeid)
                                    };
            string commandText = string.Format("SELECT * FROM [{0}orderchange] WHERE [changeid]=@changeid", RDBSHelper.RDBSTablePre);

            IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms);
            if (reader.Read())
            {
                changeInfo = BuildOrderChangeInfoFromReader(reader);
            }
            reader.Close();
            return changeInfo;
        }


        /// <summary>
        /// 更新换货记录--审核不通过，
        /// </summary>
        /// <param name="returnid">记录id</param>
        /// <param name="state">状态</param>

        /// <param name="lastModifity">最后修改时间</param>
        public static void UpdateOrderChange(int changeid, int state, DateTime lastModifity)
        {
            DbParameter[] parms = {
                                    SqlHelper.CreateInParam("@changeid", SqlDbType.Int, 4, changeid),
                                    SqlHelper.CreateInParam("@state", SqlDbType.TinyInt, 1, state), 
                                    
                                    SqlHelper.CreateInParam("@lastmodifity", SqlDbType.DateTime, 8, lastModifity)
                                   };
            string commandText = string.Format("UPDATE [{0}orderchange] SET [state]=@state,[lastmodifity]=@lastmodifity WHERE [changeid]=@changeid",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 更新换货记录--审核不通过，
        /// </summary>
        /// <param name="returnid">记录id</param>
        /// <param name="state">状态</param>

        /// <param name="lastModifity">最后修改时间</param>
        public static void UpdateOrderChange(int changeid, int state, DateTime lastModifity,string changeDesc="")
        {
            DbParameter[] parms = {
                                    SqlHelper.CreateInParam("@changeid", SqlDbType.Int, 4, changeid),
                                    SqlHelper.CreateInParam("@state", SqlDbType.TinyInt, 1, state), 
                                    
                                    SqlHelper.CreateInParam("@lastmodifity", SqlDbType.DateTime, 8, lastModifity),
                                    SqlHelper.CreateInParam("@changedesc", SqlDbType.NVarChar, 300, changeDesc)
                                   };
            string commandText = string.Format("UPDATE [{0}orderchange] SET [state]=@state,[lastmodifity]=@lastmodifity,[changedesc]=@changedesc WHERE [changeid]=@changeid",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 更新换货记录--发货
        /// </summary>
        /// <param name="changeid"></param>
        /// <param name="state"></param>
        /// <param name="changeOid"></param>
        /// <param name="changeOSN"></param>
        /// <param name="lastModifity"></param>
        public static void UpdateOrderChangeForSend(int changeid, int state, DateTime lastModifity, string changeshipsn, int changeshipcoid, string changeshipconame)
        {
            DbParameter[] parms = {
                                    SqlHelper.CreateInParam("@changeid", SqlDbType.Int, 4, changeid),
                                    SqlHelper.CreateInParam("@state", SqlDbType.TinyInt, 1, state), 
                                  
                                    SqlHelper.CreateInParam("@lastmodifity", SqlDbType.DateTime, 8, lastModifity),
                                    SqlHelper.CreateInParam("@changeshipsn", SqlDbType.Char,30, changeshipsn),
                                    SqlHelper.CreateInParam("@changeshipcoid", SqlDbType.SmallInt, 2, changeshipcoid),
                                    SqlHelper.CreateInParam("@changeshipconame", SqlDbType.NChar, 30, changeshipconame),
                                    SqlHelper.CreateInParam("changeshiptime", SqlDbType.DateTime, 8, lastModifity)

                                   };
            string commandText = string.Format("UPDATE [{0}orderchange] SET [state]=@state,[lastmodifity]=@lastmodifity,[changeshipsn]=@changeshipsn,[changeshipcoid]=@changeshipcoid,[changeshipconame]=@changeshipconame,[changeshiptime]=@changeshiptime WHERE [changeid]=@changeid",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 更新换货记录--由未处理更新为已处理
        /// </summary>
        /// <param name="changeid">记录id</param>
        /// <param name="state">状态</param>
        /// <param name="changeOid">产生的换货订单id</param>
        /// <param name="changeOSN">产生的换货订单编号</param>
        /// <param name="lastModifity">最后修改时间</param>
        public static void UpdateOrderChange(int changeid,int state,int changeOid,string changeOSN,DateTime lastModifity) {
            DbParameter[] parms = {
                                    SqlHelper.CreateInParam("@changeid", SqlDbType.Int, 4, changeid),
                                    SqlHelper.CreateInParam("@state", SqlDbType.TinyInt, 1, state), 
                                    SqlHelper.CreateInParam("@changeOid", SqlDbType.Int, 4, changeOid),
                                    SqlHelper.CreateInParam("@changeOSN", SqlDbType.VarChar, 30, changeOSN),
                                     SqlHelper.CreateInParam("@lastmodifity", SqlDbType.DateTime, 8, lastModifity)
                                   };
            string commandText = string.Format("UPDATE [{0}orderchange] SET [state]=@state,[changeOid]=@changeOid,[changeOSN]=@changeOSN,[lastmodifity]=@lastmodifity WHERE [oid]=@oid",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 获得订单换货列表条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="osn">订单编号</param>
        /// <returns></returns>
        public static string GetOrderChangeListCondition(int storeId, string osn, int state)
        {
            StringBuilder condition = new StringBuilder();

            if (storeId > 0)
                condition.AppendFormat(" AND [storeid] = {0} ", storeId);
            if (!string.IsNullOrWhiteSpace(osn))
                condition.AppendFormat(" AND [osn] like '{0}%' ", osn);
            if(state>=0)
                condition.AppendFormat(" AND [state] = {0} ", state);
            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }

        /// <summary>
        /// 获得订单换货数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int GetOrderChangeCount(string condition)
        {
            string commandText;
            if (string.IsNullOrWhiteSpace(condition))
                commandText = string.Format("SELECT COUNT(changeid) FROM [{0}orderchange]", RDBSHelper.RDBSTablePre);
            else
                commandText = string.Format("SELECT COUNT(changeid) FROM [{0}orderchange] WHERE {1}", RDBSHelper.RDBSTablePre, condition);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText), 0);
        }

        #region 辅助方法
        /// <summary>
        /// 从IDataReader创建ProductInfo
        /// </summary>
        public static OrderChangeInfo BuildOrderChangeInfoFromReader(IDataReader reader)
        {
            OrderChangeInfo changeInfo = new OrderChangeInfo();
            changeInfo.ChangeId = TypeHelper.ObjectToInt(reader["changeid"]);
            changeInfo.CreationDate = TypeHelper.ObjectToDateTime(reader["creationdate"]);
            changeInfo.LastModifity = TypeHelper.ObjectToDateTime(reader["lastmodifity"]);
            changeInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            changeInfo.StoreName = reader["storename"].ToString();
            changeInfo.Oid = TypeHelper.ObjectToInt(reader["oid"]);
            changeInfo.OSN = reader["osn"].ToString();
            changeInfo.Uid = TypeHelper.ObjectToInt(reader["uid"]);
            changeInfo.State = TypeHelper.ObjectToInt(reader["state"]);
            changeInfo.ChangeType = TypeHelper.ObjectToInt(reader["changetype"]);
            changeInfo.SAId = TypeHelper.ObjectToInt(reader["said"]);
            changeInfo.ChangeOid = TypeHelper.ObjectToInt(reader["changeoid"]);
            changeInfo.ChangeOSN = reader["changeosn"].ToString();
            changeInfo.ChangeDesc = reader["changedesc"].ToString();
            return changeInfo;
        }

        #endregion

        
    }
}
