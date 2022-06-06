using System;
using System.Data.SqlClient;
using VMall.Core;
using System.Data.Common;
using System.Data;

namespace VMall.Services
{
    /// <summary>
    /// 订单退款操作管理类
    /// </summary>
    public partial class OrderRefunds
    {
        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="orderRefundInfo">订单退款信息</param>
        public static void ApplyRefund(OrderRefundInfo orderRefundInfo)
        {
            VMall.Data.OrderRefunds.ApplyRefund(orderRefundInfo);
        }

        /// <summary>
        /// 更新退款状态为未处理
        /// </summary>
        /// <param name="refundid">记录id</param>
        /// <param name="state">状态</param>
        public static void UpdateOrderReFundForNo(int refundid, int state,string remark = "")
        {
            DbParameter[] parms = {
                                    SqlHelper.CreateInParam("@refundid", SqlDbType.Int, 4, refundid),
                                    SqlHelper.CreateInParam("@state", SqlDbType.TinyInt, 1, state),
                                    SqlHelper.CreateInParam("@remark", SqlDbType.NVarChar, 300, remark)
                                   };
            string commandText = string.Format("UPDATE [{0}orderrefunds] SET [state]=@state,[remark]=@remark WHERE [refundid]=@refundid",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }
    }
}
