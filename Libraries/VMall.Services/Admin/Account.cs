using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMall.Core;
using System.Data.SqlClient;
using System.Data;

namespace VMall.Services
{
    public class Account
    {
        #region 账户

        /// <summary>
        /// 根据id获取账户名称
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string GetAccountName(int accountId)
        {
            SqlParameter[] parms = {
                                        new SqlParameter("@accountid",accountId)
                                    };
            string commandText = string.Format(@"SELECT AccountName FROM [{0}accounttype]  WHERE  [accountid]=@accountid ", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms).ToString();
        }
        /// <summary>
        /// 根据会员id获取帐号余额信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<AccountInfo> GetAccountInfoListByUid(int uid)
        {
            List<AccountInfo> accountInfoList = new List<AccountInfo>();
            SqlParameter[] parms = {
                                        new SqlParameter("@uid", uid)    
                                    };
            string commandText = string.Format(@"SELECT * FROM [{0}account] a LEFT JOIN [{0}accounttype] b ON a.accountid=b.accountid WHERE [UserId]=@uid", RDBSHelper.RDBSTablePre);

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    AccountInfo accountInfo = new AccountInfo();
                    accountInfo.AccountId = TypeHelper.ObjectToInt(reader["AccountId"]);
                    accountInfo.UserId = TypeHelper.ObjectToInt(reader["UserId"]);
                    accountInfo.TotalIn = Convert.ToDecimal(reader["TotalIn"]);
                    accountInfo.TotalOut = Convert.ToDecimal(reader["TotalOut"]);
                    accountInfo.Banlance = Convert.ToDecimal(reader["Banlance"]);
                    accountInfo.LockBanlance = Convert.ToDecimal(reader["LockBanlance"]);
                    accountInfo.IsInternalTransfer = TypeHelper.ObjectToBool(reader["IsInternalTransfer"]);
                    accountInfo.IsMemberTransfer = TypeHelper.ObjectToBool(reader["IsMemberTransfer"]);
                    accountInfo.AccountName = reader["AccountName"].ToString();
                    accountInfoList.Add(accountInfo);
                }
                reader.Close();
            }
            return accountInfoList;
        }

        /// <summary>
        /// 获取所有用户的某一帐号余额信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static DataTable AdminGetAllAccountInfoListByAid(string strWhere, string orderby, int startIndex, int endIndex)
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
                strSql.Append("order by T.UserId desc");
            }
            strSql.Append(")AS Row, T.*,b.AccountName,c.username,c.email,c.mobile,c.nickname,ud.realname from hlh_account T left join hlh_accounttype b on T.AccountId=b.AccountId LEFT JOIN hlh_users c  on T.UserId=c.uid inner join hlh_userdetails ud on c.uid=ud.uid ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);

            return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString()).Tables[0];
        }
        /// <summary>
        /// 获取所有用户的某一帐号余额信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<AccountInfo> GetAccountListByAid(string strWhere)
        {
            List<AccountInfo> accountInfoList = new List<AccountInfo>();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * from hlh_account  ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AccountInfo accountInfo = new AccountInfo();
                    accountInfo.AccountId = TypeHelper.ObjectToInt(reader["AccountId"]);
                    accountInfo.UserId = TypeHelper.ObjectToInt(reader["UserId"]);
                    accountInfo.TotalIn = Convert.ToDecimal(reader["TotalIn"]);
                    accountInfo.TotalOut = Convert.ToDecimal(reader["TotalOut"]);
                    accountInfo.Banlance = Convert.ToDecimal(reader["Banlance"]);
                    accountInfo.LockBanlance = Convert.ToDecimal(reader["LockBanlance"]);
                    accountInfo.IsInternalTransfer = TypeHelper.ObjectToBool(reader["IsInternalTransfer"]);
                    accountInfo.IsMemberTransfer = TypeHelper.ObjectToBool(reader["IsMemberTransfer"]);
                    accountInfoList.Add(accountInfo);
                }
                reader.Close();
            }
            return accountInfoList;
        }

        /// <summary>
        /// 获取用户帐号余额信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static decimal GetAccountBanlanceByAid(string strWhere)
        {
            decimal banlance = 0M;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT top 1 * from hlh_account  ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                if (reader.Read())
                {
                    banlance = Convert.ToDecimal(reader["Banlance"]);
                }
                reader.Close();
            }
            return banlance;
        }

        /// <summary>
        /// 获取所有用户的某一帐号数量
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static int GetAllAccountInfoListCount(string condition)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT count(1) FROM [hlh_account] T left join hlh_users c on T.UserId=c.uid inner join hlh_userdetails ud on c.uid=ud.uid ");
            if (condition.Trim() != "")
            {
                strSql.Append(" where " + condition);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));

        }

        #endregion

        #region 结算

        /// <summary>
        /// 结算时更新账户信息
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateAccountBySettleAccount(SettleModel model)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@AccountId", model.accountid),
                                       new SqlParameter("@UserId",model.uid),
                                       new SqlParameter("@AccountIn", model.accountin),
                                       new SqlParameter("@AccountOut", model.accountout)
                                   };
            string commandText = string.Format("UPDATE [{0}account] SET [TotalIn]=[TotalIn]+@AccountIn,[TotalOut]=[TotalOut]+@AccountOut,[Banlance]=[Banlance]+@AccountIn-@AccountOut WHERE [AccountId]=@AccountId and [UserId]=@UserId",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 结算时更新冻结账户信息
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateAccountLockBanlance(int userId, int opUserId, int accountId, int detailType, decimal inMoney, decimal outMoney, string remark, string orderCode)
        {
            SqlParameter[] parms = {
                                       new SqlParameter("@UserId", userId),
                                       new SqlParameter("@OpUserId", opUserId),
                                       new SqlParameter("@AccountId", accountId),
                                       new SqlParameter("@DetailType", detailType),
                                       new SqlParameter("@InMoney", inMoney),
                                       new SqlParameter("@OutMoney", outMoney),
                                       new SqlParameter("@Remark", remark),
                                       new SqlParameter("@OrderCode", orderCode),
                                   };
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure, string.Format("{0}updatelockaccount", RDBSHelper.RDBSTablePre), parms);
        }

        /// <summary>
        /// 微商预结算
        /// </summary>
        /// <param name="model"></param>
        public static void WeishangPresettle()
        {
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure, string.Format("{0}weishangpresettle", RDBSHelper.RDBSTablePre));
        }

        /// <summary>
        /// 汇购优选预结算
        /// </summary>
        /// <param name="model"></param>
        public static void YouxuanPresettle()
        {
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure, string.Format("{0}youxuanpresettle", RDBSHelper.RDBSTablePre));
        }

        /// <summary>
        /// 微商正式结算
        /// </summary>
        /// <param name="model"></param>
        public static void WeishangSettle()
        {
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure, string.Format("{0}weishangsettle", RDBSHelper.RDBSTablePre));
        }

        /// <summary>
        /// 汇购优选正式结算
        /// </summary>
        /// <param name="model"></param>
        public static void YouxuanSettle()
        {
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure, string.Format("{0}youxuansettle", RDBSHelper.RDBSTablePre));
        }

        /// <summary>
        /// 代理体验包预结算取消
        /// </summary>
        /// <param name="model"></param>
        public static void CancelAgentPackPresettle(int orderId)
        {
            SqlParameter[] parms = {
                                       new SqlParameter("@OrderId", orderId)
                                   };
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure, string.Format("{0}cancelagentpackpresettle", RDBSHelper.RDBSTablePre), parms);
        }

        /// <summary>
        /// 代理体验包正式结算时，将冻结金额转移到正式金额
        /// </summary>
        /// <param name="model"></param>
        public static void AgentPackSettleTransfer(int orderId)
        {
            SqlParameter[] parms = {
                                       new SqlParameter("@OrderId", orderId)
                                   };
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure, string.Format("{0}agentpacksettletransfer", RDBSHelper.RDBSTablePre), parms);
        }

        /// <summary>
        /// 订单结算
        /// </summary>
        /// <param name="model"></param>
        public static void OrderSettle()
        {
            RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure, string.Format("{0}ordersettle", RDBSHelper.RDBSTablePre));
        }

        #endregion

        #region 更新账户

        /// <summary>
        /// 更新账户收入
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateAccountForIn(AccountInfo model)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@AccountId", model.AccountId),
                                       new SqlParameter("@UserId",model.UserId),
                                       new SqlParameter("@AccountIn", model.TotalIn)
                                   };
            string commandText = string.Format("UPDATE [{0}account] SET [TotalIn]=[TotalIn]+@AccountIn,[Banlance]=([TotalIn]+@AccountIn)-[TotalOut] WHERE [AccountId]=@AccountId and [UserId]=@UserId",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 更新账户支出
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateAccountForOut(AccountInfo model)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@AccountId", model.AccountId),
                                       new SqlParameter("@UserId",model.UserId),
                                       new SqlParameter("@AccountOut", model.TotalOut)
                                   };
            string commandText = string.Format("UPDATE [{0}account] SET [TotalOut]=[TotalOut]+@AccountOut,[Banlance]=[TotalIn]-([TotalOut]+@AccountOut) WHERE [AccountId]=@AccountId and [UserId]=@UserId",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 更新账户支出
        /// </summary>
        /// <param name="model"></param>
        public static bool UpdateAccountForOut2(AccountInfo model)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@AccountId", model.AccountId),
                                       new SqlParameter("@UserId",model.UserId),
                                       new SqlParameter("@AccountOut", model.TotalOut)
                                   };
            string commandText = string.Format("UPDATE [{0}account] SET [TotalOut]=[TotalOut]+@AccountOut,[Banlance]=[TotalIn]-([TotalOut]+@AccountOut) WHERE [AccountId]=@AccountId and [UserId]=@UserId",
                                                RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        #endregion

        #region 账户详情

        /// <summary>
        /// 插入账户交易详情
        /// </summary>
        public static void CreateAccountDetail(AccountDetailInfo detail)
        {
            decimal banlance = GetAccountBanlanceByAid(string.Format(" AccountId={0} and UserId={1} ", detail.AccountId, detail.UserId));
            SqlParameter[] parms = {
	                                    new SqlParameter("@accountid",detail.AccountId),
	                                    new SqlParameter("@userid",detail.UserId),
	                                    new SqlParameter("@creationdate",detail.CreateTime),
	                                    new SqlParameter("@detailtype",detail.DetailType),
                                        new SqlParameter("@inamount", detail.InAmount),
                                        new SqlParameter("@outamount", detail.OutAmount),
                                        new SqlParameter("@curbanlance", banlance),
                                        new SqlParameter("@ordercode", detail.OrderCode),
                                        new SqlParameter("@adminuid", detail.AdminUid),
                                        new SqlParameter("@status", detail.Status),
                                        new SqlParameter("@detaildes", detail.DetailDes)
                                    };
            string sqlStr = string.Format(@"INSERT INTO [{0}accountdetail] ([accountid],[userid],[creationdate],[detailtype],[inamount],[outamount],[curbanlance],[ordercode],[adminuid],[status],[detaildes]) VALUES(@accountid ,@userid ,@creationdate,@detailtype,@inamount,@outamount,@curbanlance,@ordercode,@adminuid,@status,@detaildes)",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, sqlStr, parms);
        }

        /// <summary>
        /// 根据会员id获取帐号余额信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<AccountDetailInfo> GetAccountDetailList(int uid, int accountId)
        {
            List<AccountDetailInfo> accountDetailList = new List<AccountDetailInfo>();
            SqlParameter[] parms = {
                                        new SqlParameter("@uid", uid),
                                        new SqlParameter("@accountid", accountId) 
                                    };
            string commandText = string.Format(@"SELECT * FROM [{0}accountdetail] WHERE [userid]=@uid and [accountid]=@accountid order by creationdate desc", RDBSHelper.RDBSTablePre);

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    AccountDetailInfo accountDetail = BuildAccountDetailFromReader(reader);
                    accountDetailList.Add(accountDetail);
                }
                reader.Close();
            }
            return accountDetailList;
        }
        /// <summary>
        /// 根据会员id获取账户流水信息--分页
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static List<AccountDetailInfo> GetAccountDetailList(int uid, int accountId, int pageNo, int pageSize, string orderCode = "")
        {
            List<AccountDetailInfo> detailList = new List<AccountDetailInfo>();
            SqlParameter[] parms = {
                                        new SqlParameter("@uid", uid),  
                                        new SqlParameter("@accountid",accountId)
                                    };
            string commandText = string.Empty;
            if (pageNo == 1)
            {
                if (string.IsNullOrEmpty(orderCode))
                    commandText = string.Format(@"SELECT TOP {1} * FROM [{0}accountdetail]   WHERE [userid]=@uid and [accountid]=@accountid  ORDER BY [creationdate] DESC", RDBSHelper.RDBSTablePre, pageSize);
                else
                    commandText = string.Format(@"SELECT TOP {1} * FROM [{0}accountdetail]   WHERE [userid]=@uid and [accountid]=@accountid AND ordercode LIKE '%" + orderCode + "%'  ORDER BY [creationdate] DESC", RDBSHelper.RDBSTablePre, pageSize);
            }
            else
            {
                if (string.IsNullOrEmpty(orderCode))
                    commandText = string.Format(@"with tb as(select a.*,ROW_NUMBER() OVER (ORDER BY a.[creationdate] DESC)  AS RowNumber from [hlh_accountdetail] as a WHERE a.[userid]=@uid and a.[accountid]=@accountid   )   
select * from tb where RowNumber between (" + pageNo + @"-1)*" + pageSize + @"+1 and (" + pageNo + @")*" + pageSize + " order by tb.[creationdate] desc",
                                                                    pageSize, RDBSHelper.RDBSTablePre, (pageNo - 1) * pageSize);
                else 
                commandText = string.Format(@"with tb as(select a.*,ROW_NUMBER() OVER (ORDER BY a.[creationdate] DESC)  AS RowNumber from [hlh_accountdetail] as a WHERE a.[userid]=@uid and a.[accountid]=@accountid AND ordercode LIKE '%" + orderCode + "%'  )                             select * from tb where RowNumber between (" + pageNo + @"-1)*" + pageSize + @"+1 and (" + pageNo + @")*" + pageSize + " order by tb.[creationdate] desc",
                                                pageSize, RDBSHelper.RDBSTablePre, (pageNo - 1) * pageSize);
            }

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    AccountDetailInfo detailInfo = BuildAccountDetailFromReader(reader);
                    detailList.Add(detailInfo);
                }
                reader.Close();
            }
            return detailList;
        }
        /// <summary>
        /// 根据会员id获取账户流水总数
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static int GetAccountDetailCount(int uid, int accountId, string orderCode = "")
        {
            SqlParameter[] parms = {
                                        new SqlParameter("@uid", uid),  
                                        new SqlParameter("@accountid",accountId)
                                    };
            string commandText = string.Empty;
            if (string.IsNullOrEmpty(orderCode))
                commandText = string.Format(@"SELECT count(*) FROM [{0}accountdetail]  WHERE [userid]=@uid AND [accountid]=@accountid ", RDBSHelper.RDBSTablePre);
            else
                commandText = string.Format(@"SELECT count(*) FROM [{0}accountdetail]  WHERE [userid]=@uid AND [accountid]=@accountid AND ordercode LIKE '%" + orderCode + "%'  ", RDBSHelper.RDBSTablePre);
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText,
                                                                   parms));
        }

        /// <summary>
        /// 根据条件获取帐号账户详情信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<AccountDetailInfo> GetAccountDetailListByWhere(string strWhere)
        {
            List<AccountDetailInfo> accountDetailList = new List<AccountDetailInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(@"SELECT * FROM [{0}accountdetail] ", RDBSHelper.RDBSTablePre));
            if (strWhere.Trim() != "")
                sb.Append(" where " + strWhere);
            sb.Append(" order by creationdate desc ");
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, sb.ToString()))
            {
                while (reader.Read())
                {
                    AccountDetailInfo accountDetail = BuildAccountDetailFromReader(reader);
                    accountDetailList.Add(accountDetail);
                }
                reader.Close();
            }
            return accountDetailList;
        }

        #endregion

        #region 辅助方法
        /// <summary>
        /// 从IDataReader创建 AccountInfo
        /// </summary>
        public static AccountInfo BuildAccountInfoFromReader(IDataReader reader)
        {
            AccountInfo accountInfo = new AccountInfo();
            accountInfo.AccountId = TypeHelper.ObjectToInt(reader["AccountId"]);
            accountInfo.UserId = TypeHelper.ObjectToInt(reader["UserId"]);
            accountInfo.TotalIn = Convert.ToDecimal(reader["TotalIn"]);
            accountInfo.TotalOut = Convert.ToDecimal(reader["TotalOut"]);
            accountInfo.Banlance = Convert.ToDecimal(reader["Banlance"]);
            accountInfo.LockBanlance = Convert.ToDecimal(reader["LockBanlance"]);
            accountInfo.IsInternalTransfer = TypeHelper.ObjectToBool(reader["IsInternalTransfer"]);
            accountInfo.IsMemberTransfer = TypeHelper.ObjectToBool(reader["IsMemberTransfer"]);
            return accountInfo;
        }

        /// <summary>
        /// 从IDataReader创建 AccountDetailInfo
        /// </summary>
        public static AccountDetailInfo BuildAccountDetailFromReader(IDataReader reader)
        {
            AccountDetailInfo detailInfo = new AccountDetailInfo();
            detailInfo.AccountId = TypeHelper.ObjectToInt(reader["accountid"]);
            detailInfo.UserId = TypeHelper.ObjectToInt(reader["userid"]);
            detailInfo.CreateTime = TypeHelper.ObjectToDateTime(reader["creationdate"]);
            detailInfo.DetailType = TypeHelper.ObjectToInt(reader["detailtype"]);
            detailInfo.InAmount = Convert.ToDecimal(reader["inamount"]);
            detailInfo.OutAmount = Convert.ToDecimal(reader["outamount"]);
            detailInfo.CurBanlance = Convert.ToDecimal(reader["curbanlance"]);
            detailInfo.OrderCode = reader["ordercode"].ToString();
            detailInfo.AdminUid = TypeHelper.ObjectToInt(reader["adminuid"]);
            detailInfo.Status = TypeHelper.ObjectToInt(reader["status"]);
            detailInfo.DetailDes = reader["detaildes"].ToString();
            return detailInfo;
        }
        #endregion
    }
}
