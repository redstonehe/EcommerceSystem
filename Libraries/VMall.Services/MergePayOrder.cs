using System;
using System.Text;

using System.Collections.Generic;
using VMall.Core;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace VMall.Services
{
    /// <summary>
    /// 合并订单管理类
    /// </summary>
    public partial class MergePayOrder
    {
        /// <summary>
        /// 创建合并支付订单
        /// </summary>
        /// <param name="merOrderList">需要提交的合并订单</param>
        public static void CreateMergePayOrder(List<MergePayOrderInfo> merOrderList)
        {
            StringBuilder sbStr = new StringBuilder();
            sbStr.Append("INSERT INTO [hlh_orderpaymerge] ([creationdate],[mergeosn],[suboid],[subosn],[suborderamount],[uid],[storeid],[storename],[paysystemname],[payfriendname])                                VALUES ");

            foreach (MergePayOrderInfo mergePayOrderInfo in merOrderList)
            {

                // 合并记录id  mergeid (int )
                //创建时间 creationdate (datatime)
                //合并订单号 mergeosn (varchar(30))
                //子订单id suboid (int)
                //子订单号 subosn (varchar(30))
                //子订单金额 suborderamount (decimal)
                //用户id uid (int)
                //店铺id storeid (int)
                //店铺名称 storename (nvarchar(120))
                //支付方式系统名  paysystemname (varchar(20))
                //支付方式友好名  payfriendname (nvarchar(60))
                //DbParameter[] parms = new DbParameter[] {
                //                                //GenerateInParam("@mergeid", SqlDbType.Int, 4, mergePayOrderInfo.MergeId),
                //                                GenerateInParam("@creationdate", SqlDbType.DateTime, 8, mergePayOrderInfo.CreationDate),
                //                                GenerateInParam("@mergeosn", SqlDbType.VarChar, 30, mergePayOrderInfo.MergeOSN),
                //                                GenerateInParam("@suboid", SqlDbType.Int, 4, mergePayOrderInfo.SubOid),
                //                                GenerateInParam("@subosn", SqlDbType.VarChar, 30, mergePayOrderInfo.MergeOSN),
                //                                GenerateInParam("@suborderamount", SqlDbType.Decimal, 4, mergePayOrderInfo.SubOrderAmount),
                //                                GenerateInParam("@uid", SqlDbType.Int, 4, mergePayOrderInfo.Uid),
                //                                GenerateInParam("@storeid", SqlDbType.Int, 4, mergePayOrderInfo.StoreId),
                //                                GenerateInParam("@storename", SqlDbType.NVarChar, 120, mergePayOrderInfo.StoreName),
                //                                GenerateInParam("@paysystemname", SqlDbType.VarChar, 120, mergePayOrderInfo.PaySystemName),
                //                                GenerateInParam("@payfriendname", SqlDbType.NVarChar, 60, mergePayOrderInfo.PayFriendName)
                //                            };
                sbStr.AppendFormat("('{0}','{1}',{2},'{3}',{4},{5},{6},'{7}','{8}','{9}'),",
                    mergePayOrderInfo.CreationDate,
                    mergePayOrderInfo.MergeOSN,
                    mergePayOrderInfo.SubOid,
                    mergePayOrderInfo.SubOSN,
                    mergePayOrderInfo.SubOrderAmount,
                    mergePayOrderInfo.Uid,
                    mergePayOrderInfo.StoreId,
                    mergePayOrderInfo.StoreName,
                    mergePayOrderInfo.PaySystemName,
                    mergePayOrderInfo.PayFriendName);
                //string sqlStr = @"INSERT INTO [hlh_orderpaymerge] ([creationdate],[mergeosn],[suboid],[subosn],[suborderamount],[uid],[storeid],[storename],[paysystemname],[payfriendname])                                VALUES (@creationdate,@mergeosn,@suboid,@subosn,@suborderamount,@uid,@storeid,@storename,@paysystemname,@payfriendname)";
                ////RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure,string.Format("{0}addorderproduct", RDBSHelper.RDBSTablePre), parms);
                //RDBSHelper.ExecuteScalar(CommandType.Text, sqlStr, parms);
            }
            string sqlStr = sbStr.ToString().TrimEnd(',');
            //RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure,string.Format("{0}addorderproduct", RDBSHelper.RDBSTablePre), parms);
            RDBSHelper.ExecuteScalar(CommandType.Text, sqlStr);
            //return 0;
        }

        /// <summary>
        /// 根据子订单id取得合并支付记录
        /// </summary>
        /// <param name="suboid">子订单号</param>
        /// <returns></returns>
        public static MergePayOrderInfo GetMergeOrderBySubOid(int suboid)
        {

            MergePayOrderInfo merInfo = null;
            DbParameter[] parms =  { 
                                        GenerateInParam("@suboid", SqlDbType.Int, 4, suboid)
                                    };
            string commandText = string.Format("SELECT * FROM [{0}orderpaymerge] WHERE [suboid]=@suboid", RDBSHelper.RDBSTablePre);

            IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms);
            if (reader.Read())
            {
                merInfo = BuildMergePayOrderFromReader(reader);
            }
            reader.Close();
            return merInfo;
            // return null;   
        }

        /// <summary>
        /// 根据合并订单号获取所有子订单列表
        /// </summary>
        /// <returns></returns>
        public static List<MergePayOrderInfo> GetMergeOrderListByMergeOSN(string mergeosn)
        {
            List<MergePayOrderInfo> mergeList = new List<MergePayOrderInfo>();
            DbParameter[] parms =  { 
                                        GenerateInParam("@mergeosn", SqlDbType.VarChar, 32, mergeosn)
                                    };
            string commandText = string.Format("SELECT * FROM [{0}orderpaymerge] WHERE [mergeosn]=@mergeosn", RDBSHelper.RDBSTablePre);

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    MergePayOrderInfo changeInfo = BuildMergePayOrderFromReader(reader);
                    mergeList.Add(changeInfo);
                }
                reader.Close();
            }
            return mergeList;

        }

        /// <summary>
        /// 修改合并支付记录订单价格-单个产品退货
        /// </summary>
        public static bool UpdateMergeAmount_SingleReturn(int mergeid, decimal Amount)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@mergeid",mergeid),
                                       new SqlParameter("@Amount",Amount)
                                    };
            string commandText = string.Format("UPDATE [{0}orderpaymerge] SET [suborderamount]=@Amount WHERE [mergeid]=@mergeid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        #region  辅助方法
        /// <summary>
        /// 从IDataReader创建ProductInfo
        /// </summary>
        public static MergePayOrderInfo BuildMergePayOrderFromReader(IDataReader reader)
        {
            MergePayOrderInfo merOrderInfo = new MergePayOrderInfo();

            merOrderInfo.MergeId = TypeHelper.ObjectToInt(reader["mergeid"]);
            merOrderInfo.CreationDate = TypeHelper.ObjectToDateTime(reader["creationdate"]);
            merOrderInfo.MergeOSN = reader["mergeosn"].ToString();
            merOrderInfo.SubOid = TypeHelper.ObjectToInt(reader["suboid"]);
            merOrderInfo.SubOSN = reader["subosn"].ToString();
            merOrderInfo.SubOrderAmount = TypeHelper.ObjectToDecimal(reader["suborderamount"]);
            merOrderInfo.Uid = TypeHelper.ObjectToInt(reader["uid"]);
            merOrderInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            merOrderInfo.StoreName = reader["storename"].ToString();
            merOrderInfo.PaySystemName = reader["paysystemname"].ToString();
            merOrderInfo.PayFriendName = reader["payfriendname"].ToString();

            return merOrderInfo;
        }


        /// <summary>
        /// 生成输入参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="sqlDbType">参数类型</param>
        /// <param name="size">类型大小</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private static DbParameter GenerateInParam(string paramName, SqlDbType sqlDbType, int size, object value)
        {
            SqlParameter param = new SqlParameter(paramName, sqlDbType, size);
            param.Direction = ParameterDirection.Input;
            if (value != null)
                param.Value = value;
            return param;
        }

        #endregion
    }


}
