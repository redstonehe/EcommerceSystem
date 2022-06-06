using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMall.Core;
using VMall.Services;
using System.Configuration;

namespace VMall.EventStrategy.Timer
{
    /// <summary>
    /// 自动取消过期未支付订单
    /// </summary>
    public class AutoCancelNoPayOrder : IEvent
    {
        public void Execute(object eventInfo)
        {
            //LogHelper.WriteOperateLog("自动取消进入", "自动取消进入服务", "=================");
            EventInfo e = (EventInfo)eventInfo;

            //未支付24小时后自动取消订单

            Orders.AutoCancelNoPayOrder(e.TimeValue);

            //EventLogs.CreateEventLog(e.Key, e.Title, Environment.MachineName, DateTime.Now);
        }
    }
}
