using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using VMall.Core;
using System.Collections.Generic;
using DAL.Base;

namespace VMall.Services
{
    /// <summary>
    /// 数据访问类:hlh_CashCoupon
    /// </summary>
    public partial class CashCoupon : BaseDAL<CashCouponInfo>
    {
        public CashCoupon()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public static bool Exists(int CashId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from hlh_CashCoupon");
            strSql.Append(" where CashId=@CashId");
            SqlParameter[] parameters = {
					new SqlParameter("@CashId", SqlDbType.Int,4)
			};
            parameters[0].Value = CashId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static int Add(CashCouponInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into hlh_CashCoupon(");
            strSql.Append("CashCouponSN,CreationDate,LastModifity,CouponType,StoreId,ChannelId,Uid,CreateOid,CreateOSN,CashAmount,TotalIn,TotalOut,Banlance,ValidTime,DirSaleUid)");
            strSql.Append(" values (");
            strSql.Append("@CashCouponSN,@CreationDate,@LastModifity,@CouponType,@StoreId,@ChannelId,@Uid,@CreateOid,@CreateOSN,@CashAmount,@TotalIn,@TotalOut,@Banlance,@ValidTime,@DirSaleUid)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@CashCouponSN", SqlDbType.VarChar,32),
					new SqlParameter("@CreationDate", SqlDbType.DateTime),
					new SqlParameter("@LastModifity", SqlDbType.DateTime),
					new SqlParameter("@CouponType", SqlDbType.TinyInt,1),
					new SqlParameter("@StoreId", SqlDbType.Int,4),
					new SqlParameter("@ChannelId", SqlDbType.Int,4),
					new SqlParameter("@Uid", SqlDbType.Int,4),
					new SqlParameter("@CreateOid", SqlDbType.Int,4),
					new SqlParameter("@CreateOSN", SqlDbType.VarChar,30),
					new SqlParameter("@CashAmount", SqlDbType.Decimal,9),
					new SqlParameter("@TotalIn", SqlDbType.Decimal,9),
					new SqlParameter("@TotalOut", SqlDbType.Decimal,9),
					new SqlParameter("@Banlance", SqlDbType.Decimal,9),
					new SqlParameter("@ValidTime", SqlDbType.DateTime),
                    new SqlParameter("@DirSaleUid", SqlDbType.Int,4)};
            parameters[0].Value = model.CashCouponSN;
            parameters[1].Value = model.CreationDate;
            parameters[2].Value = model.LastModifity;
            parameters[3].Value = model.CouponType;
            parameters[4].Value = model.StoreId;
            parameters[5].Value = model.ChannelId;
            parameters[6].Value = model.Uid;
            parameters[7].Value = model.CreateOid;
            parameters[8].Value = model.CreateOSN;
            parameters[9].Value = model.CashAmount;
            parameters[10].Value = model.TotalIn;
            parameters[11].Value = model.TotalOut;
            parameters[12].Value = model.Banlance;
            parameters[13].Value = model.ValidTime;
            parameters[14].Value = model.DirSaleUid;
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters));
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool Update(CashCouponInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update hlh_CashCoupon set ");
            strSql.Append("CashCouponSN=@CashCouponSN,");
            strSql.Append("CreationDate=@CreationDate,");
            strSql.Append("LastModifity=@LastModifity,");
            strSql.Append("CouponType=@CouponType,");
            strSql.Append("StoreId=@StoreId,");
            strSql.Append("ChannelId=@ChannelId,");
            strSql.Append("Uid=@Uid,");
            strSql.Append("CreateOid=@CreateOid,");
            strSql.Append("CreateOSN=@CreateOSN,");
            strSql.Append("CashAmount=@CashAmount,");
            strSql.Append("TotalIn=@TotalIn,");
            strSql.Append("TotalOut=@TotalOut,");
            strSql.Append("Banlance=@Banlance,");
            strSql.Append("ValidTime=@ValidTime");
            strSql.Append(" where CashId=@CashId");
            SqlParameter[] parameters = {
					new SqlParameter("@CashCouponSN", SqlDbType.VarChar,32),
					new SqlParameter("@CreationDate", SqlDbType.DateTime),
					new SqlParameter("@LastModifity", SqlDbType.DateTime),
					new SqlParameter("@CouponType", SqlDbType.TinyInt,1),
					new SqlParameter("@StoreId", SqlDbType.Int,4),
					new SqlParameter("@ChannelId", SqlDbType.Int,4),
					new SqlParameter("@Uid", SqlDbType.Int,4),
					new SqlParameter("@CreateOid", SqlDbType.Int,4),
					new SqlParameter("@CreateOSN", SqlDbType.VarChar,30),
					new SqlParameter("@CashAmount", SqlDbType.Decimal,9),
					new SqlParameter("@TotalIn", SqlDbType.Decimal,9),
					new SqlParameter("@TotalOut", SqlDbType.Decimal,9),
					new SqlParameter("@Banlance", SqlDbType.Decimal,9),
					new SqlParameter("@ValidTime", SqlDbType.DateTime),
					new SqlParameter("@CashId", SqlDbType.Int,4)};
            parameters[0].Value = model.CashCouponSN;
            parameters[1].Value = model.CreationDate;
            parameters[2].Value = model.LastModifity;
            parameters[3].Value = model.CouponType;
            parameters[4].Value = model.StoreId;
            parameters[5].Value = model.ChannelId;
            parameters[6].Value = model.Uid;
            parameters[7].Value = model.CreateOid;
            parameters[8].Value = model.CreateOSN;
            parameters[9].Value = model.CashAmount;
            parameters[10].Value = model.TotalIn;
            parameters[11].Value = model.TotalOut;
            parameters[12].Value = model.Banlance;
            parameters[13].Value = model.ValidTime;
            parameters[14].Value = model.CashId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static bool Delete(int CashId)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_CashCoupon ");
            strSql.Append(" where CashId=@CashId");
            SqlParameter[] parameters = {
					new SqlParameter("@CashId", SqlDbType.Int,4)
			};
            parameters[0].Value = CashId;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public static bool DeleteList(string CashIdlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_CashCoupon ");
            strSql.Append(" where CashId in (" + CashIdlist + ")  ");
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static CashCouponInfo GetModel(int CashId)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_CashCoupon ");
            strSql.Append(" where CashId=@CashId");
            SqlParameter[] parameters = {
					new SqlParameter("@CashId", SqlDbType.Int,4)
			};
            parameters[0].Value = CashId;

            CashCouponInfo model = new CashCouponInfo();
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
        public static CashCouponInfo GetModel(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_CashCoupon ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }

            CashCouponInfo model = new CashCouponInfo();
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
        public static CashCouponInfo DataRowToModel(DataRow row)
        {
            CashCouponInfo model = new CashCouponInfo();
            if (row != null)
            {
                if (row["CashId"] != null && row["CashId"].ToString() != "")
                {
                    model.CashId = int.Parse(row["CashId"].ToString());
                }
                if (row["CashCouponSN"] != null)
                {
                    model.CashCouponSN = row["CashCouponSN"].ToString();
                }
                if (row["CreationDate"] != null && row["CreationDate"].ToString() != "")
                {
                    model.CreationDate = DateTime.Parse(row["CreationDate"].ToString());
                }
                if (row["LastModifity"] != null && row["LastModifity"].ToString() != "")
                {
                    model.LastModifity = DateTime.Parse(row["LastModifity"].ToString());
                }
                if (row["CouponType"] != null && row["CouponType"].ToString() != "")
                {
                    model.CouponType = int.Parse(row["CouponType"].ToString());
                }
                if (row["StoreId"] != null && row["StoreId"].ToString() != "")
                {
                    model.StoreId = int.Parse(row["StoreId"].ToString());
                }
                if (row["ChannelId"] != null && row["ChannelId"].ToString() != "")
                {
                    model.ChannelId = int.Parse(row["ChannelId"].ToString());
                }
                if (row["Uid"] != null && row["Uid"].ToString() != "")
                {
                    model.Uid = int.Parse(row["Uid"].ToString());
                }
                if (row["CreateOid"] != null && row["CreateOid"].ToString() != "")
                {
                    model.CreateOid = int.Parse(row["CreateOid"].ToString());
                }
                if (row["CreateOSN"] != null)
                {
                    model.CreateOSN = row["CreateOSN"].ToString();
                }
                if (row["CashAmount"] != null && row["CashAmount"].ToString() != "")
                {
                    model.CashAmount = decimal.Parse(row["CashAmount"].ToString());
                }
                if (row["TotalIn"] != null && row["TotalIn"].ToString() != "")
                {
                    model.TotalIn = decimal.Parse(row["TotalIn"].ToString());
                }
                if (row["TotalOut"] != null && row["TotalOut"].ToString() != "")
                {
                    model.TotalOut = decimal.Parse(row["TotalOut"].ToString());
                }
                if (row["Banlance"] != null && row["Banlance"].ToString() != "")
                {
                    model.Banlance = decimal.Parse(row["Banlance"].ToString());
                }
                if (row["ValidTime"] != null && row["ValidTime"].ToString() != "")
                {
                    model.ValidTime = DateTime.Parse(row["ValidTime"].ToString());
                }
                if (row["DirSaleUid"] != null && row["DirSaleUid"].ToString() != "")
                {
                    model.DirSaleUid = int.Parse(row["DirSaleUid"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public static List<CashCouponInfo> GetList(string strWhere)
        {
            List<CashCouponInfo> list = new List<CashCouponInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM hlh_CashCoupon ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    CashCouponInfo info = BuildCashCouponInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
            //return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString());
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public static List<CashCouponInfo> GetList(int Top, string strWhere, string filedOrder)
        {
            List<CashCouponInfo> list = new List<CashCouponInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM hlh_CashCoupon ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    CashCouponInfo info = BuildCashCouponInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
            //return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public static int GetRecordCount(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_CashCoupon ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public static List<CashCouponInfo> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            List<CashCouponInfo> list = new List<CashCouponInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.CashId desc");
            }
            strSql.Append(")AS Row, T.*  from hlh_CashCoupon T ");
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
                    CashCouponInfo info = BuildCashCouponInfoReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
            //return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString());
        }


        /// <summary>
        /// 获取记录总数
        /// </summary>
        public static int AdminGetRecordCount(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_CashCoupon T LEFT JOIN hlh_users b on T.uid=b.uid");
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
                strSql.Append("order by T.CashId desc");
            }
            strSql.Append(")AS Row, T.*,b.username,b.email,b.mobile  from hlh_CashCoupon T LEFT JOIN hlh_users b on T.uid=b.uid");
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
        public static DataSet GetList(int PageSize,int PageIndex,string strWhere)
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
            parameters[0].Value = "hlh_CashCoupon";
            parameters[1].Value = "CashId";
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = 0;
            parameters[5].Value = 0;
            parameters[6].Value = strWhere;	
            return DbHelperSQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
        }*/

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public static CashCouponInfo BuildCashCouponInfoReader(IDataReader row)
        {
            CashCouponInfo model = new CashCouponInfo();
            if (row != null)
            {
                if (row["CashId"] != null && row["CashId"].ToString() != "")
                {
                    model.CashId = int.Parse(row["CashId"].ToString());
                }
                if (row["CashCouponSN"] != null)
                {
                    model.CashCouponSN = row["CashCouponSN"].ToString();
                }
                if (row["CreationDate"] != null && row["CreationDate"].ToString() != "")
                {
                    model.CreationDate = DateTime.Parse(row["CreationDate"].ToString());
                }
                if (row["LastModifity"] != null && row["LastModifity"].ToString() != "")
                {
                    model.LastModifity = DateTime.Parse(row["LastModifity"].ToString());
                }
                if (row["CouponType"] != null && row["CouponType"].ToString() != "")
                {
                    model.CouponType = int.Parse(row["CouponType"].ToString());
                }
                if (row["StoreId"] != null && row["StoreId"].ToString() != "")
                {
                    model.StoreId = int.Parse(row["StoreId"].ToString());
                }
                if (row["ChannelId"] != null && row["ChannelId"].ToString() != "")
                {
                    model.ChannelId = int.Parse(row["ChannelId"].ToString());
                }
                if (row["Uid"] != null && row["Uid"].ToString() != "")
                {
                    model.Uid = int.Parse(row["Uid"].ToString());
                }
                if (row["CreateOid"] != null && row["CreateOid"].ToString() != "")
                {
                    model.CreateOid = int.Parse(row["CreateOid"].ToString());
                }
                if (row["CreateOSN"] != null)
                {
                    model.CreateOSN = row["CreateOSN"].ToString();
                }
                if (row["CashAmount"] != null && row["CashAmount"].ToString() != "")
                {
                    model.CashAmount = decimal.Parse(row["CashAmount"].ToString());
                }
                if (row["TotalIn"] != null && row["TotalIn"].ToString() != "")
                {
                    model.TotalIn = decimal.Parse(row["TotalIn"].ToString());
                }
                if (row["TotalOut"] != null && row["TotalOut"].ToString() != "")
                {
                    model.TotalOut = decimal.Parse(row["TotalOut"].ToString());
                }
                if (row["Banlance"] != null && row["Banlance"].ToString() != "")
                {
                    model.Banlance = decimal.Parse(row["Banlance"].ToString());
                }
                if (row["ValidTime"] != null && row["ValidTime"].ToString() != "")
                {
                    model.ValidTime = DateTime.Parse(row["ValidTime"].ToString());
                }
                if (row["DirSaleUid"] != null && row["DirSaleUid"].ToString() != "")
                {
                    model.DirSaleUid = int.Parse(row["DirSaleUid"].ToString());
                }
            }
            return model;
        }

        #endregion  BasicMethod
        #region  ExtensionMethod

        /// <summary>
        /// 更新汇购卡支出
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateCashForOut(CashCouponInfo model)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@CashId", model.CashId),
                                       new SqlParameter("@Uid",model.Uid),
                                       new SqlParameter("@TotalOut", model.TotalOut),
                                       new SqlParameter("@LastModifity", model.LastModifity)
                                   };
            string commandText = string.Format("UPDATE [{0}CashCoupon] SET [TotalOut]=[TotalOut]+@TotalOut,[Banlance]=[TotalIn]-([TotalOut]+@TotalOut),[LastModifity]=@LastModifity  WHERE [Uid]=@Uid and [CashId]=@CashId",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 更新汇购卡收入
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateCashForIn(CashCouponInfo model)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@CashId", model.CashId),
                                       new SqlParameter("@Uid",model.Uid),
                                       new SqlParameter("@TotalIn", model.TotalIn),
                                       new SqlParameter("@LastModifity", model.LastModifity)
                                   };
            string commandText = string.Format("UPDATE [{0}CashCoupon] SET [TotalIn]=[TotalIn]+@TotalIn,[Banlance]=([TotalIn]+@TotalIn)-[TotalOut],[LastModifity]=@LastModifity  WHERE [Uid]=@Uid and [CashId]=@CashId",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }


        /// <summary>
        /// 返回汇购卡
        /// </summary>
        /// <param name="partUserInfo"></param>
        /// <param name="orderInfo"></param>
        /// <param name="operatorId"></param>
        /// <param name="returnTime"></param>
        public static void ReturnUserCash(PartUserInfo partUserInfo, OrderInfo orderInfo)
        {
            List<CashCouponDetailInfo> infoList = CashCouponDetail.GetList(string.Format("Oid={0} AND Uid={1} AND OutAmount>0 AND DetailType=2", orderInfo.Oid, partUserInfo.Uid));
            
            if (infoList.Count<=0)
                return;
            decimal allReturnCashAmount = 0M;
            foreach (var item in infoList)
            {
                if (orderInfo.CashDiscount >= allReturnCashAmount)
                {
                    int cashId = item.CashId;
                    CashCoupon.UpdateCashForIn(new CashCouponInfo()
                    {
                        CashId = cashId,
                        Uid = orderInfo.Uid,
                        TotalIn = item.OutAmount
                    });
                    CashCouponDetail.Add(new CashCouponDetailInfo()
                    {
                        CreationDate = DateTime.Now,
                        CashId = cashId,
                        Uid = partUserInfo.Uid,
                        DetailType = (int)CashDetailType.订单抵现退还,
                        InAmount = item.OutAmount,
                        Oid = orderInfo.Oid,
                        OSN = orderInfo.OSN,
                        DetailDes = "订单汇购卡券退还(合并支付退回)：订单号:" + orderInfo.OSN + ",金额:" + item.OutAmount,
                        Status = 1,
                        DirSaleUid = partUserInfo.DirSaleUid
                    });
                    allReturnCashAmount += item.OutAmount;
                }
            }
        }


        public static bool isDSUser(string name)
        {
            bool isDSUser =false;
            PartUserInfo user;
            if (ValidateHelper.IsEmail(name))
               user = Users.GetPartUserByEmail(name);
            else if (ValidateHelper.IsMobile(name))
                 user  = Users.GetPartUserByMobile(name);
            else
                 user  = Users.GetPartUserByName(name);
            if (user != null)
                isDSUser = user.IsDirSaleUser;
            return isDSUser;
        }

        public static int GetUidByName(string name)
        {
            
            int user=-1;
            if (ValidateHelper.IsEmail(name))
                user = Users.GetUidByEmail(name);
            else if (ValidateHelper.IsMobile(name))
                user = Users.GetUidByMobile(name);
            else
                user = Users.GetUidByUserName(name);

            return user;
        }
        #endregion  ExtensionMethod
    }
}

