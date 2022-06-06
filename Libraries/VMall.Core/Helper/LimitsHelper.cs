using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Configuration;


namespace VMall.Core
{
    public class LimitHelper
    {
        /// <summary> 
        ///判断是否请求的地址是否本域
        /// </summary> 
        public static bool GetUrlReferrer()
        {
            System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;
            string ReUrl = string.Empty;
            string HostUrl = string.Empty;
            if (request.Url.Host.ToString() != "")
            {
                HostUrl = GetDomiancs.GetDomain(request.Url.Host.ToString());
            }
            if (request.UrlReferrer != null)
            {
                if (request.UrlReferrer.Host.ToString() != "")
                {
                    ReUrl = GetDomiancs.GetDomain(request.UrlReferrer.Host.ToString());
                }
            }
            if (ReUrl != HostUrl)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 一段时间内限制执行的任务
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="limitType">存储主键值</param>
        /// <param name="SubmitTime">间隔时间</param>
        /// <param name="SubmitNumber">提交次数</param>
        /// <returns></returns>
        public static bool Execution(string keyValue, int limitType, int SubmitTime, int SubmitNumber)
        {
            return Execution(keyValue, limitType, SubmitTime, SubmitNumber, true);
        }

        /// <summary>
        /// 一段时间内限制执行的任务
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="limitType">存储主键值</param>
        /// <param name="SubmitTime">间隔时间</param>
        /// <param name="SubmitNumber">提交次数</param>
        /// <param name="IsMinutes">时间间隔单位是否为分钟，否则为秒</param>
        /// <returns></returns>
        public static bool Execution(string keyValue, int limitType, int SubmitTime, int SubmitNumber, bool IsMinutes)
        {
            string cacheKey = "Limit(" + limitType + ")";
            List<Limit> limitList = HttpRuntime.Cache[cacheKey] as List<Limit>;
            //建立缓存
            if (limitList == null)
            {
                limitList = new List<Limit>();
                HttpRuntime.Cache.Insert(cacheKey, limitList, null, DateTime.UtcNow.AddDays(1), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
            //找出当前用户的缓存
            DateTime thenTime = DateTime.Now;
            Limit limit = limitList.Find(delegate(Limit item)
            {
                TimeSpan TS = thenTime.Subtract(item.CreateDate);
                //超过10分钟后移除该用户缓存
                if (IsMinutes)
                {
                    if (TS.TotalMinutes >= SubmitTime)
                    {
                        limitList.Remove(item);
                    }
                }
                else
                {
                    if (TS.TotalSeconds >= SubmitTime)
                    {
                        limitList.Remove(item);
                    }
                }

                if (item.KeyValue == keyValue)
                    return true;
                else
                    return false;
            });
            //当前用户不存在
            if (limit == null)
            {
                limit = new Limit();
                limit.KeyValue = keyValue;
                limit.Count = 1;
                limit.CreateDate = DateTime.Now;
                limitList.Add(limit);
                return true;
            }
            else
            {
                //操作到达3次
                if (limit.Count == SubmitNumber)
                {
                    return false;
                }
                limit.Count++;
            }
            return true;
        }

        /// <summary>
        /// 一段时间内限制执行的任务
        /// </summary>
        /// <param name="ip">客户端IP</param>
        /// <param name="iimitsEnum">限制类型</param>
        /// <returns></returns>
        public static bool Execution(string keyValue, int limitType)
        {
            return Execution(keyValue, limitType, 10, 10, true);
        }
    }
    public class Limit
    {
        public string KeyValue { get; set; }
        public int Count { get; set; }
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// 操作的枚举
    /// </summary>
    public enum LimitEnum
    {
        Order = 1, //订单
        Register = 2,//注册
        UpdateMember = 3,//修改资料
        PNothing = 4,//缺货登记
        RegisterForIP=5,//
        RegisterForMobile=6//
        
    }

    public class GetDomiancs
    {
        /// <summary>
        /// 获取根域名（前面没有.  例：baidu.com）
        /// </summary>
        /// <param name="Domain"></param>
        /// <returns></returns>
        public static string GetDomain(string Domain)
        {
            if (Domain == "localhost")
            {
                return string.Empty;
            }
            string[] strArr = Domain.Split('.');
            string lastStr = strArr.GetValue(strArr.Length - 1).ToString();

            string[] domainRules = ".com.cn|.net.cn|.org.cn|.gov.cn|.com|.net|.cn|.org|.cc|.me|.tel|.mobi|.asia|.biz|.info|.name|.tv|.hk|.公司|.中国|.网络".Split('|');
            string findStr = string.Empty;
            string replaceStr = string.Empty;
            string returnStr = string.Empty;
            for (int i = 0; i < domainRules.Length; i++)
            {
                if (Domain.EndsWith(domainRules[i].ToLower())) //如果最后有找到匹配项
                {
                    findStr = domainRules[i].ToString();
                    replaceStr = Domain.Replace(findStr, ""); //将匹配项替换为空，便于再次判断
                    if (replaceStr.IndexOf('.') > 0) //存在二级域名或者三级，比如：www.px915
                    {
                        string[] replaceArr = replaceStr.Split('.');
                        returnStr = replaceArr.GetValue(replaceArr.Length - 1).ToString() + findStr;
                        return returnStr;
                    }
                    else
                    {
                        returnStr = replaceStr + findStr;
                        return returnStr;
                    };
                }
                else
                { returnStr = Domain; }
            }
            return returnStr;
        }
    }

}
