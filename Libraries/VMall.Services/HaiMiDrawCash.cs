
using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using VMall.Core;
using System.Collections.Generic;

namespace VMall.Services
{
    /// <summary>
    /// 数据访问类:HaiMiDrawCash
    /// </summary>
    public partial class HaiMiDrawCash
    {
        public HaiMiDrawCash()
        { }
        #region  BasicMethod


        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from hlh_HaiMiDrawCash");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.Int,4)
			};
            parameters[0].Value = Id;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(HaiMiDrawCashInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into hlh_HaiMiDrawCash(");
            strSql.Append("CreateTime,Uid,AccountId,DrawCashSN,AdminUid,Amount,Poundage,ActualAmount,State,PayTime,Remark,BankId,BankName,BankProvice,BankCity,BankAddress,BankCardCode,BankUserName,Result,TaxAmount)");
            strSql.Append(" values (");
            strSql.Append("@CreateTime,@Uid,@AccountId,@DrawCashSN,@AdminUid,@Amount,@Poundage,@ActualAmount,@State,@PayTime,@Remark,@BankId,@BankName,@BankProvice,@BankCity,@BankAddress,@BankCardCode,@BankUserName,@Result,@TaxAmount)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@CreateTime", SqlDbType.DateTime),
					new SqlParameter("@Uid", SqlDbType.Int,4),
					new SqlParameter("@AccountId", SqlDbType.Int,4),
					new SqlParameter("@DrawCashSN", SqlDbType.NVarChar,50),
					new SqlParameter("@AdminUid", SqlDbType.Int,4),
					new SqlParameter("@Amount", SqlDbType.Decimal,9),
					new SqlParameter("@Poundage", SqlDbType.Decimal,9),
					new SqlParameter("@ActualAmount", SqlDbType.Decimal,9),
					new SqlParameter("@State", SqlDbType.Int,4),
					new SqlParameter("@PayTime", SqlDbType.DateTime),
					new SqlParameter("@Remark", SqlDbType.NVarChar,500),
					new SqlParameter("@BankId", SqlDbType.Int,4),
					new SqlParameter("@BankName", SqlDbType.NVarChar,100),
					new SqlParameter("@BankProvice", SqlDbType.NVarChar,50),
					new SqlParameter("@BankCity", SqlDbType.NVarChar,50),
					new SqlParameter("@BankAddress", SqlDbType.NVarChar,100),
					new SqlParameter("@BankCardCode", SqlDbType.NVarChar,50),
					new SqlParameter("@BankUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@Result", SqlDbType.NVarChar,200),
                    new SqlParameter("@TaxAmount", SqlDbType.Decimal,9)};
            parameters[0].Value = model.CreateTime;
            parameters[1].Value = model.Uid;
            parameters[2].Value = model.AccountId;
            parameters[3].Value = model.DrawCashSN;
            parameters[4].Value = model.AdminUid;
            parameters[5].Value = model.Amount;
            parameters[6].Value = model.Poundage;
            parameters[7].Value = model.ActualAmount;
            parameters[8].Value = model.State;
            parameters[9].Value = model.PayTime;
            parameters[10].Value = model.Remark;
            parameters[11].Value = model.BankId;
            parameters[12].Value = model.BankName;
            parameters[13].Value = model.BankProvice;
            parameters[14].Value = model.BankCity;
            parameters[15].Value = model.BankAddress;
            parameters[16].Value = model.BankCardCode;
            parameters[17].Value = model.BankUserName;
            parameters[18].Value = model.Result;
            parameters[19].Value = model.TaxAmount;
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters));
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(HaiMiDrawCashInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update hlh_HaiMiDrawCash set ");
            strSql.Append("CreateTime=@CreateTime,");
            strSql.Append("Uid=@Uid,");
            strSql.Append("AccountId=@AccountId,");
            strSql.Append("DrawCashSN=@DrawCashSN,");
            strSql.Append("AdminUid=@AdminUid,");
            strSql.Append("Amount=@Amount,");
            strSql.Append("Poundage=@Poundage,");
            strSql.Append("ActualAmount=@ActualAmount,");
            strSql.Append("State=@State,");
            strSql.Append("PayTime=@PayTime,");
            strSql.Append("Remark=@Remark,");
            strSql.Append("BankId=@BankId,");
            strSql.Append("BankName=@BankName,");
            strSql.Append("BankProvice=@BankProvice,");
            strSql.Append("BankCity=@BankCity,");
            strSql.Append("BankAddress=@BankAddress,");
            strSql.Append("BankCardCode=@BankCardCode,");
            strSql.Append("BankUserName=@BankUserName,");
            strSql.Append("Result=@Result");
            strSql.Append("TaxAmount=@TaxAmount");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@CreateTime", SqlDbType.DateTime),
					new SqlParameter("@Uid", SqlDbType.Int,4),
					new SqlParameter("@AccountId", SqlDbType.Int,4),
					new SqlParameter("@DrawCashSN", SqlDbType.NVarChar,50),
					new SqlParameter("@AdminUid", SqlDbType.Int,4),
					new SqlParameter("@Amount", SqlDbType.Decimal,9),
					new SqlParameter("@Poundage", SqlDbType.Decimal,9),
					new SqlParameter("@ActualAmount", SqlDbType.Decimal,9),
					new SqlParameter("@State", SqlDbType.Int,4),
					new SqlParameter("@PayTime", SqlDbType.DateTime),
					new SqlParameter("@Remark", SqlDbType.NVarChar,500),
					new SqlParameter("@BankId", SqlDbType.Int,4),
					new SqlParameter("@BankName", SqlDbType.NVarChar,100),
					new SqlParameter("@BankProvice", SqlDbType.NVarChar,50),
					new SqlParameter("@BankCity", SqlDbType.NVarChar,50),
					new SqlParameter("@BankAddress", SqlDbType.NVarChar,100),
					new SqlParameter("@BankCardCode", SqlDbType.NVarChar,50),
					new SqlParameter("@BankUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@Result", SqlDbType.NVarChar,200),
                    new SqlParameter("@TaxAmount", SqlDbType.Decimal,9),
					new SqlParameter("@Id", SqlDbType.Int,4)};
            parameters[0].Value = model.CreateTime;
            parameters[1].Value = model.Uid;
            parameters[2].Value = model.AccountId;
            parameters[3].Value = model.DrawCashSN;
            parameters[4].Value = model.AdminUid;
            parameters[5].Value = model.Amount;
            parameters[6].Value = model.Poundage;
            parameters[7].Value = model.ActualAmount;
            parameters[8].Value = model.State;
            parameters[9].Value = model.PayTime;
            parameters[10].Value = model.Remark;
            parameters[11].Value = model.BankId;
            parameters[12].Value = model.BankName;
            parameters[13].Value = model.BankProvice;
            parameters[14].Value = model.BankCity;
            parameters[15].Value = model.BankAddress;
            parameters[16].Value = model.BankCardCode;
            parameters[17].Value = model.BankUserName;
            parameters[18].Value = model.Result;
            parameters[19].Value = model.TaxAmount;
            parameters[20].Value = model.Id;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int Id)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_HaiMiDrawCash ");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.Int,4)
			};
            parameters[0].Value = Id;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string Idlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_HaiMiDrawCash ");
            strSql.Append(" where Id in (" + Idlist + ")  ");
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public HaiMiDrawCashInfo GetModel(int Id)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_HaiMiDrawCash ");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.Int,4)
			};
            parameters[0].Value = Id;

            HaiMiDrawCashInfo model = new HaiMiDrawCashInfo();
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
        public HaiMiDrawCashInfo GetModel(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_HaiMiDrawCash ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }

            HaiMiDrawCashInfo model = new HaiMiDrawCashInfo();
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
        public static HaiMiDrawCashInfo DataRowToModel(DataRow row)
        {
            HaiMiDrawCashInfo model = new HaiMiDrawCashInfo();
            if (row != null)
            {
                if (row["Id"] != null && row["Id"].ToString() != "")
                {
                    model.Id = int.Parse(row["Id"].ToString());
                }
                if (row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                {
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                }
                if (row["Uid"] != null && row["Uid"].ToString() != "")
                {
                    model.Uid = int.Parse(row["Uid"].ToString());
                }
                if (row["AccountId"] != null && row["AccountId"].ToString() != "")
                {
                    model.AccountId = int.Parse(row["AccountId"].ToString());
                }
                if (row["DrawCashSN"] != null)
                {
                    model.DrawCashSN = row["DrawCashSN"].ToString();
                }
                if (row["AdminUid"] != null && row["AdminUid"].ToString() != "")
                {
                    model.AdminUid = int.Parse(row["AdminUid"].ToString());
                }
                if (row["Amount"] != null && row["Amount"].ToString() != "")
                {
                    model.Amount = decimal.Parse(row["Amount"].ToString());
                }
                if (row["Poundage"] != null && row["Poundage"].ToString() != "")
                {
                    model.Poundage = decimal.Parse(row["Poundage"].ToString());
                }
                if (row["ActualAmount"] != null && row["ActualAmount"].ToString() != "")
                {
                    model.ActualAmount = decimal.Parse(row["ActualAmount"].ToString());
                }
                if (row["State"] != null && row["State"].ToString() != "")
                {
                    model.State = int.Parse(row["State"].ToString());
                }
                if (row["PayTime"] != null && row["PayTime"].ToString() != "")
                {
                    model.PayTime = DateTime.Parse(row["PayTime"].ToString());
                }
                if (row["Remark"] != null)
                {
                    model.Remark = row["Remark"].ToString();
                }
                if (row["BankId"] != null && row["BankId"].ToString() != "")
                {
                    model.BankId = int.Parse(row["BankId"].ToString());
                }
                if (row["BankName"] != null)
                {
                    model.BankName = row["BankName"].ToString();
                }
                if (row["BankProvice"] != null)
                {
                    model.BankProvice = row["BankProvice"].ToString();
                }
                if (row["BankCity"] != null)
                {
                    model.BankCity = row["BankCity"].ToString();
                }
                if (row["BankAddress"] != null)
                {
                    model.BankAddress = row["BankAddress"].ToString();
                }
                if (row["BankCardCode"] != null)
                {
                    model.BankCardCode = row["BankCardCode"].ToString();
                }
                if (row["BankUserName"] != null)
                {
                    model.BankUserName = row["BankUserName"].ToString();
                }
                if (row["Result"] != null)
                {
                    model.Result = row["Result"].ToString();
                }
                if (row["TaxAmount"] != null && row["TaxAmount"].ToString() != "")
                {
                    model.TaxAmount = decimal.Parse(row["TaxAmount"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static HaiMiDrawCashInfo BuildHaiMiDrawCashInfoReader(IDataReader row)
        {
            HaiMiDrawCashInfo model = new HaiMiDrawCashInfo();
            if (row != null)
            {
                if (row["Id"] != null && row["Id"].ToString() != "")
                {
                    model.Id = int.Parse(row["Id"].ToString());
                }
                if (row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                {
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                }
                if (row["Uid"] != null && row["Uid"].ToString() != "")
                {
                    model.Uid = int.Parse(row["Uid"].ToString());
                }
                if (row["AccountId"] != null && row["AccountId"].ToString() != "")
                {
                    model.AccountId = int.Parse(row["AccountId"].ToString());
                }
                if (row["DrawCashSN"] != null)
                {
                    model.DrawCashSN = row["DrawCashSN"].ToString();
                }
                if (row["AdminUid"] != null && row["AdminUid"].ToString() != "")
                {
                    model.AdminUid = int.Parse(row["AdminUid"].ToString());
                }
                if (row["Amount"] != null && row["Amount"].ToString() != "")
                {
                    model.Amount = decimal.Parse(row["Amount"].ToString());
                }
                if (row["Poundage"] != null && row["Poundage"].ToString() != "")
                {
                    model.Poundage = decimal.Parse(row["Poundage"].ToString());
                }
                if (row["ActualAmount"] != null && row["ActualAmount"].ToString() != "")
                {
                    model.ActualAmount = decimal.Parse(row["ActualAmount"].ToString());
                }
                if (row["State"] != null && row["State"].ToString() != "")
                {
                    model.State = int.Parse(row["State"].ToString());
                }
                if (row["PayTime"] != null && row["PayTime"].ToString() != "")
                {
                    model.PayTime = DateTime.Parse(row["PayTime"].ToString());
                }
                if (row["Remark"] != null)
                {
                    model.Remark = row["Remark"].ToString();
                }
                if (row["BankId"] != null && row["BankId"].ToString() != "")
                {
                    model.BankId = int.Parse(row["BankId"].ToString());
                }
                if (row["BankName"] != null)
                {
                    model.BankName = row["BankName"].ToString();
                }
                if (row["BankProvice"] != null)
                {
                    model.BankProvice = row["BankProvice"].ToString();
                }
                if (row["BankCity"] != null)
                {
                    model.BankCity = row["BankCity"].ToString();
                }
                if (row["BankAddress"] != null)
                {
                    model.BankAddress = row["BankAddress"].ToString();
                }
                if (row["BankCardCode"] != null)
                {
                    model.BankCardCode = row["BankCardCode"].ToString();
                }
                if (row["BankUserName"] != null)
                {
                    model.BankUserName = row["BankUserName"].ToString();
                }
                if (row["Result"] != null)
                {
                    model.Result = row["Result"].ToString();
                }
                if (row["TaxAmount"] != null && row["TaxAmount"].ToString() != "")
                {
                    model.TaxAmount = decimal.Parse(row["TaxAmount"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_HaiMiDrawCash ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int AdminGetRecordCount(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_HaiMiDrawCash T LEFT JOIN hlh_users b on T.uid=b.uid inner join hlh_userdetails ud on ud.uid=T.uid ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<HaiMiDrawCashInfo> GetList(string strWhere)
        {
            List<HaiMiDrawCashInfo> list = new List<HaiMiDrawCashInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM hlh_HaiMiDrawCash ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    HaiMiDrawCashInfo info = BuildHaiMiDrawCashInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public List<HaiMiDrawCashInfo> GetList(int Top, string strWhere, string filedOrder)
        {
            List<HaiMiDrawCashInfo> list = new List<HaiMiDrawCashInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM hlh_HaiMiDrawCash ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    HaiMiDrawCashInfo info = BuildHaiMiDrawCashInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public List<HaiMiDrawCashInfo> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            List<HaiMiDrawCashInfo> list = new List<HaiMiDrawCashInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.Id desc");
            }
            strSql.Append(")AS Row, T.*  from hlh_HaiMiDrawCash T ");
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
                    HaiMiDrawCashInfo info = BuildHaiMiDrawCashInfoReader(reader);
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
                strSql.Append("order by T.Id desc");
            }
            strSql.Append(")AS Row, T.*,b.username,b.email,b.mobile,a.AccountName,ud.realname  from hlh_HaiMiDrawCash T LEFT JOIN hlh_users b on T.uid=b.uid left join hlh_accounttype a on T.AccountId=a.AccountId inner join hlh_userdetails ud on ud.uid=t.uid");
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
            parameters[0].Value = "hlh_HaiMiDrawCash";
            parameters[1].Value = "Id";
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = 0;
            parameters[5].Value = 0;
            parameters[6].Value = strWhere;	
            return RDBSHelper.RunProcedure("UP_GetRecordByPage",parameters,"ds");
        }*/

        #endregion  BasicMethod
        #region  ExtensionMethod
        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public bool SetState(int Id, int State, string result, HaiMiDrawCashInfo info)
        {
            PartUserInfo user = Users.GetPartUserById(info.Uid);
            SqlParameter[] parms =  {
                                       new SqlParameter("@Id",Id),
                                       new SqlParameter("@State",State),
                                       new SqlParameter("@Result",result)
                                   };
            string commandText = string.Format("UPDATE [{0}HaiMiDrawCash] SET [State]=@State,[Result]=@Result WHERE [Id]=@Id",
                                               RDBSHelper.RDBSTablePre);
            //提现撤销要退回
            if (State == 0)
            {
                //更新直销的账户--不包含代理账户和佣金账户
                if (user.IsDirSaleUser && (info.AccountId != (int)AccountType.代理账户 && info.AccountId != (int)AccountType.佣金账户))
                {
                    AccountUtils.UpdateAccountForDir(user.DirSaleUid, info.AccountId, info.Amount, 0, info.DrawCashSN, string.Format("提现退回,金额:{0},退回原因：{1}", info.Amount, result));
                }
                else
                {
                    //更新账户
                    Account.UpdateAccountForIn(new AccountInfo()
                    {
                        AccountId = info.AccountId,
                        UserId = info.Uid,
                        TotalIn = info.Amount
                    });
                    Account.CreateAccountDetail(new AccountDetailInfo()
                    {
                        AccountId = info.AccountId,
                        UserId = info.Uid,
                        CreateTime = DateTime.Now,
                        DetailType = (int)DetailType.提现取消返回,
                        InAmount = info.Amount,
                        OrderCode = info.DrawCashSN,
                        AdminUid = 0,//system
                        Status = 1,
                        DetailDes = string.Format("提现退回,金额:{0},退回原因：{1}", info.Amount, result)
                    });
                }
            }
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 批量设置状态
        /// </summary>
        /// <param name="adId">广告id</param>
        public bool SetStateByIds(int[] idList, int State = 2, string result = "银行转账已成功")
        {
            if (idList != null && idList.Length > 0)
            {
                string commandText = String.Format("UPDATE [{0}HaiMiDrawCash] SET [State]={2},[Result]='{3}' WHERE [Id] IN ({1})", RDBSHelper.RDBSTablePre, CommonHelper.IntArrayToString(idList), State, result);
               return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText)>0;
            }
            return false;
        }
        /// <summary>
        /// 批量设置状态
        /// </summary>
        /// <param name="adId">广告id</param>
        public bool SetStateBySNs(string snList, int State = 2, string result = "")
        {
            if (State == 2)
                result = "银行转账已成功";
            if (!string.IsNullOrEmpty(snList))
            {
                string[] sns = StringHelper.SplitString(snList, "\n");
                StringBuilder sb = new StringBuilder();
                foreach (string i in sns)
                {
                    sb.Append("'" + i + "'");
                    sb.Append(",");
                }

                string commandText = String.Format("UPDATE [{0}HaiMiDrawCash] SET [State]={2},[Result]='{3}' WHERE [DrawCashSN] IN ({1})", RDBSHelper.RDBSTablePre, sb.ToString().TrimEnd(','), State, result);
                return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText) > 0;
            }
            return false;

        }
        #endregion  ExtensionMethod
    }
}

