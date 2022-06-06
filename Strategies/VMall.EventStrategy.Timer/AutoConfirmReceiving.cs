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
    ///自动确认收货事件
    /// </summary>
    public class AutoConfirmReceiving : IEvent
    {
        public void Execute(object eventInfo)
        {
            EventInfo e = (EventInfo)eventInfo;

            //发货时间7天后自动确认

            Orders.AutoConfirmRevecing(e.TimeValue);

            //EventLogs.CreateEventLog(e.Key, e.Title, Environment.MachineName, DateTime.Now);
        }

    }
}
