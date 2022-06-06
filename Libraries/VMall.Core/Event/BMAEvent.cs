using System;
using System.Threading;
using System.Web;

namespace VMall.Core
{
    /// <summary>
    /// VMall事件管理类
    /// </summary>
    public class BMAEvent
    {
        private static Timer _timer;//定时器
        private static Timer _timer2=null;
        private static Timer _timer3 = null;
        private static Timer _timer4 = null;
        static BMAEvent()
        {
            EventConfigInfo eventConfigInfo = BMAConfig.EventConfig;
            if (eventConfigInfo.BMAEventState == 1)
                _timer = new Timer(new TimerCallback(Processor), null, 60000, eventConfigInfo.BMAEventPeriod * 60000);
            if (eventConfigInfo.BMAEventList.Exists(x => x.TimeType == 2 && x.Key == "autoconfirmreceiving"))//自动执行定时器
                _timer2 = new Timer(new TimerCallback(AutoProcessor), null, 60000, 1 * 30000);
            if (eventConfigInfo.BMAEventList.Exists(x => x.TimeType == 2 && x.Key == "autosettleaccount"))//自动执行定时器
                _timer3 = new Timer(new TimerCallback(AutoProcessor2), null, 60000, 1 * 30000);
            if (eventConfigInfo.BMAEventList.Exists(x => x.TimeType == 2 && x.Key == "autocancelnopayorder"))//自动执行定时器
                _timer4 = new Timer(new TimerCallback(AutoProcessor3), null, 60000, 1 * 30000);
        }

        /// <summary>
        /// 此方法为空，只是起到激活VMall事件处理机制的作用
        /// </summary>
        public static void Start() {
            System.Web.HttpRuntime.Cache.Insert("AutoServiceKey", "true", null, DateTime.UtcNow.AddMinutes(1), System.Web.Caching.Cache.NoSlidingExpiration);
            System.Web.HttpRuntime.Cache.Insert("AutoSettleServiceKey", "true", null, DateTime.UtcNow.AddMinutes(1), System.Web.Caching.Cache.NoSlidingExpiration);
            System.Web.HttpRuntime.Cache.Insert("AutoCancelOrderKey", "true", null, DateTime.UtcNow.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// 执行指定事件
        /// </summary>
        public static void Execute(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            EventConfigInfo eventConfigInfo = BMAConfig.EventConfig;
            if (eventConfigInfo.BMAEventState == 0 || eventConfigInfo.BMAEventList.Count == 0)
                return;

            EventInfo eventInfo = eventConfigInfo.BMAEventList.Find(x => x.Key == key);
            if (eventInfo != null && eventInfo.Instance != null)
            {
                eventInfo.LastExecuteTime = DateTime.Now;
                ThreadPool.QueueUserWorkItem(eventInfo.Instance.Execute, eventInfo);
            }
        }

        /// <summary>
        /// 事件处理程序
        /// </summary>
        /// <param name="state">参数对象</param>
        private static void Processor(object state)
        {
            EventConfigInfo eventConfigInfo = BMAConfig.EventConfig;
            if (eventConfigInfo.BMAEventState == 0 || eventConfigInfo.BMAEventList.Count == 0)
                return;

            //循环执行每个事件
            foreach (EventInfo eventInfo in eventConfigInfo.BMAEventList)
            {
                //如果事件未开启则跳过
                if (eventInfo.Enabled == 0)
                    continue;

                //如果事件实例为空则跳过
                if (eventInfo.Instance == null)
                    continue;

                //当前时间
                DateTime nowTime = DateTime.Now;

                if (eventInfo.TimeType == 0)//特定时间执行
                {
                    //事件今天应该执行的时间
                    DateTime executeTime = nowTime.Date.AddMinutes(eventInfo.TimeValue);
                    //当事件今天已经执行或者还未达到今天的执行时间则跳出
                    if (!(eventInfo.LastExecuteTime.Value <= executeTime && nowTime >= executeTime))
                        continue;
                }
                else if (eventInfo.TimeType == 1)//时间间隔执行
                {
                    //当前时间还未达到下次执行时间时跳出
                    if ((nowTime - eventInfo.LastExecuteTime.Value).Minutes < eventInfo.TimeValue)
                        continue;
                }
                else
                {
                    continue;
                    //throw new BMAException("事件：" + eventInfo.Key + "的时间类型只能是0或1");
                }

                eventInfo.LastExecuteTime = nowTime;
                ThreadPool.QueueUserWorkItem(eventInfo.Instance.Execute, eventInfo);
            }
        }
        /// <summary>
        /// 事件处理程序
        /// </summary>
        /// <param name="state">参数对象</param>
        private static void AutoProcessor(object state)
        {
            EventConfigInfo eventConfigInfo = BMAConfig.EventConfig;
            if (eventConfigInfo.BMAEventState == 0 || eventConfigInfo.BMAEventList.Count == 0)
                return;

            //循环执行每个事件
            foreach (EventInfo eventInfo in eventConfigInfo.BMAEventList.FindAll(x=>x.TimeType==2))
            {
                //如果事件未开启则跳过
                if (eventInfo.Enabled == 0)
                    continue;

                //如果事件实例为空则跳过
                if (eventInfo.Instance == null)
                    continue;

                //当前时间
                DateTime nowTime = DateTime.Now;

                if (eventInfo.TimeType == 0)//特定时间执行
                {
                    //事件今天应该执行的时间
                    DateTime executeTime = nowTime.Date.AddMinutes(eventInfo.TimeValue);
                    //当事件今天已经执行或者还未达到今天的执行时间则跳出
                    if (!(eventInfo.LastExecuteTime.Value <= executeTime && nowTime >= executeTime))
                        continue;
                }
                else if (eventInfo.TimeType == 1)//时间间隔执行
                {
                    //当前时间还未达到下次执行时间时跳出
                    if ((nowTime - eventInfo.LastExecuteTime.Value).Minutes < eventInfo.TimeValue)
                        continue;
                }
                else if (eventInfo.TimeType == 2 && eventInfo.Key == "autoconfirmreceiving")//自动执行事件
                {
                    if (!string.IsNullOrEmpty(System.Web.HttpRuntime.Cache.Get("AutoServiceKey") as string))
                        continue;
                }
                else
                {
                    continue;
                    //throw new BMAException("事件：" + eventInfo.Key + "的时间类型只能是0或1");
                }

                eventInfo.LastExecuteTime = nowTime;
                
                ThreadPool.QueueUserWorkItem(eventInfo.Instance.Execute, eventInfo);
                if (eventInfo.TimeType == 2)
                {
                    System.Web.HttpRuntime.Cache.Insert("AutoServiceKey", "true", null, DateTime.UtcNow.AddMinutes(1), System.Web.Caching.Cache.NoSlidingExpiration);
                }
            }
        }
        /// <summary>
        /// 事件处理程序
        /// </summary>
        /// <param name="state">参数对象</param>
        private static void AutoProcessor2(object state)
        {
            EventConfigInfo eventConfigInfo = BMAConfig.EventConfig;
            if (eventConfigInfo.BMAEventState == 0 || eventConfigInfo.BMAEventList.Count == 0)
                return;

            //循环执行每个事件
            foreach (EventInfo eventInfo in eventConfigInfo.BMAEventList.FindAll(x => x.TimeType == 2))
            {
                //如果事件未开启则跳过
                if (eventInfo.Enabled == 0)
                    continue;

                //如果事件实例为空则跳过
                if (eventInfo.Instance == null)
                    continue;

                //当前时间
                DateTime nowTime = DateTime.Now;

                if (eventInfo.TimeType == 2 && eventInfo.Key == "autosettleaccount")//自动执行事件
                {
                    if (!string.IsNullOrEmpty(System.Web.HttpRuntime.Cache.Get("AutoSettleServiceKey") as string))
                        continue;
                }
                else
                {
                    continue;
                    //throw new BMAException("事件：" + eventInfo.Key + "的时间类型只能是0或1");
                }

                eventInfo.LastExecuteTime = nowTime;

                ThreadPool.QueueUserWorkItem(eventInfo.Instance.Execute, eventInfo);
                if (eventInfo.TimeType == 2)
                {
                    System.Web.HttpRuntime.Cache.Insert("AutoSettleServiceKey", "true", null, DateTime.UtcNow.AddMinutes(1), System.Web.Caching.Cache.NoSlidingExpiration);
                }
            }
        }

        /// <summary>
        /// 事件处理程序
        /// </summary>
        /// <param name="state">参数对象</param>
        private static void AutoProcessor3(object state)
        {
            EventConfigInfo eventConfigInfo = BMAConfig.EventConfig;
            if (eventConfigInfo.BMAEventState == 0 || eventConfigInfo.BMAEventList.Count == 0)
                return;

            //循环执行每个事件
            foreach (EventInfo eventInfo in eventConfigInfo.BMAEventList.FindAll(x => x.TimeType == 2))
            {
                //如果事件未开启则跳过
                if (eventInfo.Enabled == 0)
                    continue;

                //如果事件实例为空则跳过
                if (eventInfo.Instance == null)
                    continue;

                //当前时间
                DateTime nowTime = DateTime.Now;

                if (eventInfo.TimeType == 2 && eventInfo.Key == "autocancelnopayorder")//自动执行事件
                {
                    if (!string.IsNullOrEmpty(System.Web.HttpRuntime.Cache.Get("AutoCancelOrderKey") as string))
                        continue;
                }
                else
                {
                    continue;
                    //throw new BMAException("事件：" + eventInfo.Key + "的时间类型只能是0或1");
                }

                eventInfo.LastExecuteTime = nowTime;

                ThreadPool.QueueUserWorkItem(eventInfo.Instance.Execute, eventInfo);
                if (eventInfo.TimeType == 2)
                {
                    System.Web.HttpRuntime.Cache.Insert("AutoCancelOrderKey", "true", null, DateTime.UtcNow.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                }
            }
        }
    }
}
