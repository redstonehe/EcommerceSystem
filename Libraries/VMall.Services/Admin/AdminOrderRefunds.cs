using System;
using System.Collections.Generic;

using VMall.Core;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace VMall.Services
{
    /// <summary>
    /// 后台订单退款操作管理类
    /// </summary>
    public partial class AdminOrderRefunds : OrderRefunds
    {


        /// <summary>
        /// 获得订单退货数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int GetOrderReturnCount(string condition)
        {
            string commandText;
            if (string.IsNullOrWhiteSpace(condition))
                commandText = string.Format("SELECT COUNT(*) FROM [{0}orderreturn]", RDBSHelper.RDBSTablePre);
            else
                commandText = string.Format("SELECT COUNT(*) FROM [{0}orderreturn] WHERE {1}", RDBSHelper.RDBSTablePre, condition);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText), 0);
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="refundId">退款id</param>
        /// <param name="refundSN">退款单号</param>
        /// <param name="refundSystemName">退款方式系统名</param>
        /// <param name="refundFriendName">退款方式昵称</param>
        /// <param name="refundTime">退款时间</param>
        public static void RefundOrder(int refundId, string refundSN, string refundSystemName, string refundFriendName, DateTime refundTime)
        {
            VMall.Data.OrderRefunds.RefundOrder(refundId, refundSN, refundSystemName, refundFriendName, refundTime);
        }

        /// <summary>
        /// 获得订单退款列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static List<OrderRefundInfo> GetOrderRefundList(int pageSize, int pageNumber, string condition)
        {
            return VMall.Data.OrderRefunds.GetOrderRefundList(pageSize, pageNumber, condition);
        }

        /// <summary>
        /// 获得订单退款列表条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="osn">订单编号</param>
        /// <returns></returns>
        public static string GetOrderRefundListCondition(int storeId, string osn, int state, string paySystemName)
        {
            StringBuilder condition = new StringBuilder();

            if (storeId > 0)
                condition.AppendFormat(" AND [storeid] = {0} ", storeId);
            if (!string.IsNullOrWhiteSpace(osn))
                condition.AppendFormat(" AND [osn] like '{0}%' ", osn);
            if (!string.IsNullOrEmpty(paySystemName))
                condition.AppendFormat(" AND [paysystemname] = '{0}' ", paySystemName);
            condition.AppendFormat(" AND [state] = {0} ", state);
            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
            //return VMall.Data.OrderRefunds.GetOrderRefundListCondition(storeId, osn,state);
        }

        /// <summary>
        /// 获得订单退款数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int GetOrderRefundCount(string condition)
        {
            return VMall.Data.OrderRefunds.GetOrderRefundCount(condition);
        }

        /// <summary>
        ///确认退款
        /// </summary>
        /// <param name="refundId">退款id</param>
        /// <param name="refundSN">退款单号</param>
        /// <param name="refundSystemName">退款方式系统名</param>
        /// <param name="refundFriendName">退款方式昵称</param>
        /// <param name="refundTime">退款时间</param>
        public static void RefundOrder(int refundId, string refundSN, string refundSystemName, string refundFriendName, DateTime refundTime, string refundTranSN, string reMark)
        {
            SqlParameter[] parms = {
	                                new SqlParameter("@refundid", refundId),
                                    new SqlParameter("@refundsn", refundSN),
                                    new SqlParameter("@refundsystemname",refundSystemName),
                                    new SqlParameter("@refundfriendname", refundFriendName),
                                    new SqlParameter("@refundtime", refundTime),
                                    new SqlParameter("@refundtransn", refundTranSN),
                                    new SqlParameter("@remark", reMark)
                                   };
            string commandText = string.Format("UPDATE [{0}orderrefunds] SET [state]=1,[refundsn]=@refundsn,[refundsystemname]=@refundsystemname,[refundfriendname]=@refundfriendname,[refundtime]=@refundtime,[refundtransn]=@refundtransn,[remark]=[remark]+@remark WHERE [refundid]=@refundid", RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        ///更新退款单号
        /// </summary>
        /// <param name="refundId">退款id</param>
        /// <param name="refundSN">退款单号</param>
        /// <param name="refundSystemName">退款方式系统名</param>
        /// <param name="refundFriendName">退款方式昵称</param>
        /// <param name="refundTime">退款时间</param>
        public static void UpdateRefundSN(int refundId, string refundSN)
        {
            SqlParameter[] parms = {
	                                new SqlParameter("@refundid", refundId),
                                    new SqlParameter("@refundsn", refundSN)
                                   };
            string commandText = string.Format("UPDATE [{0}orderrefunds] SET [refundsn]=@refundsn WHERE [refundid]=@refundid", RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        ///确认退款--更新退款交易号
        /// </summary>
        /// <param name="refundId">退款id</param>
        /// <param name="refundSN">退款单号</param>
        /// <param name="refundSystemName">退款方式系统名</param>
        /// <param name="refundFriendName">退款方式昵称</param>
        /// <param name="refundTime">退款时间</param>
        public static void UpdateRefundTranSN(int refundId, string refundTranSN)
        {
            SqlParameter[] parms = {
	                                new SqlParameter("@refundid", refundId),
                                    new SqlParameter("@refundtransn", refundTranSN)
                                   };
            string commandText = string.Format("UPDATE [{0}orderrefunds] SET [state]=1,[refundtransn]=@refundtransn WHERE [refundid]=@refundid", RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 根据订单号查找退款记录
        /// </summary>
        /// <param name="osn"></param>
        /// <returns></returns>
        public static OrderRefundInfo GetRefundInfoByOSN(string osn)
        {
            OrderRefundInfo orderRefundInfo = null;
            SqlParameter[] parms = {
	                                new SqlParameter("@osn", osn)
                                   };
            string commandText = string.Format("SELECT * FROM [{0}orderrefunds]  WHERE [osn]=@osn", RDBSHelper.RDBSTablePre);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                if (reader.Read())
                {
                    orderRefundInfo = new OrderRefundInfo();
                    orderRefundInfo.RefundId = TypeHelper.ObjectToInt(reader["refundid"].ToString());
                    orderRefundInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
                    orderRefundInfo.StoreName = reader["storename"].ToString();
                    orderRefundInfo.Oid = TypeHelper.ObjectToInt(reader["oid"]);
                    orderRefundInfo.OSN = reader["osn"].ToString();
                    orderRefundInfo.Uid = TypeHelper.ObjectToInt(reader["uid"]);
                    orderRefundInfo.State = TypeHelper.ObjectToInt(reader["state"]);
                    orderRefundInfo.ApplyTime = TypeHelper.ObjectToDateTime(reader["applytime"]);
                    orderRefundInfo.PayMoney = TypeHelper.ObjectToDecimal(reader["paymoney"]);
                    orderRefundInfo.RefundMoney = TypeHelper.ObjectToDecimal(reader["refundmoney"]);
                    orderRefundInfo.RefundSN = reader["refundsn"].ToString();
                    orderRefundInfo.RefundSystemName = reader["refundsystemname"].ToString();
                    orderRefundInfo.RefundFriendName = reader["refundfriendname"].ToString();
                    orderRefundInfo.RefundTime = TypeHelper.ObjectToDateTime(reader["refundtime"]);
                    orderRefundInfo.PaySN = reader["paysn"].ToString();
                    orderRefundInfo.PaySystemName = reader["paysystemname"].ToString();
                    orderRefundInfo.PayFriendName = reader["payfriendname"].ToString();
                    orderRefundInfo.RefundTranSN = reader["refundtransn"].ToString();
                    orderRefundInfo.ReMark = reader["remark"].ToString();
                }
                reader.Close();
            }
            return orderRefundInfo;
        }
        /// <summary>
        /// 根据退款单号查找退款记录
        /// </summary>
        /// <param name="osn"></param>
        /// <returns></returns>
        public static OrderRefundInfo GetRefundInfoByRefundSN(string refundsn)
        {
            OrderRefundInfo orderRefundInfo = null;
            SqlParameter[] parms = {
	                                new SqlParameter("@refundsn", refundsn)
                                   };
            string commandText = string.Format("SELECT * FROM [{0}orderrefunds]  WHERE [refundsn]=@refundsn", RDBSHelper.RDBSTablePre);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                if (reader.Read())
                {
                    orderRefundInfo = new OrderRefundInfo();
                    orderRefundInfo.RefundId = TypeHelper.ObjectToInt(reader["refundid"].ToString());
                    orderRefundInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
                    orderRefundInfo.StoreName = reader["storename"].ToString();
                    orderRefundInfo.Oid = TypeHelper.ObjectToInt(reader["oid"]);
                    orderRefundInfo.OSN = reader["osn"].ToString();
                    orderRefundInfo.Uid = TypeHelper.ObjectToInt(reader["uid"]);
                    orderRefundInfo.State = TypeHelper.ObjectToInt(reader["state"]);
                    orderRefundInfo.ApplyTime = TypeHelper.ObjectToDateTime(reader["applytime"]);
                    orderRefundInfo.PayMoney = TypeHelper.ObjectToDecimal(reader["paymoney"]);
                    orderRefundInfo.RefundMoney = TypeHelper.ObjectToDecimal(reader["refundmoney"]);
                    orderRefundInfo.RefundSN = reader["refundsn"].ToString();
                    orderRefundInfo.RefundSystemName = reader["refundsystemname"].ToString();
                    orderRefundInfo.RefundFriendName = reader["refundfriendname"].ToString();
                    orderRefundInfo.RefundTime = TypeHelper.ObjectToDateTime(reader["refundtime"]);
                    orderRefundInfo.PaySN = reader["paysn"].ToString();
                    orderRefundInfo.PaySystemName = reader["paysystemname"].ToString();
                    orderRefundInfo.PayFriendName = reader["payfriendname"].ToString();
                    orderRefundInfo.RefundTranSN = reader["refundtransn"].ToString();
                    orderRefundInfo.ReMark = reader["remark"].ToString();
                }
                reader.Close();
            }
            return orderRefundInfo;
        }
    }
}
