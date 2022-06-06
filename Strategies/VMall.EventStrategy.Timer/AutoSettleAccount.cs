using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMall.Core;
using VMall.Services;
using System.Web.Configuration;

namespace VMall.EventStrategy.Timer
{
    /// <summary>
    ///自动结算事件
    /// </summary>
    public class AutoSettleAccount : IEvent
    {
        private bool runFlag = false;

        public void Execute(object eventInfo)
        {
            if (runFlag) return;

            try
            {
                runFlag = true;

                Orders.OrderSettle();
            }
            catch (Exception ex)
            {
                //记录执行异常
                LogHelper.WriteOperateLog("AutoSettleAccountError", "订单操作服务异常", "错误信息：" + ex.StackTrace);
            }
            finally
            {
                runFlag = false;
            }
        }
    }
}
