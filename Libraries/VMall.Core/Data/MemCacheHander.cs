using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

using BeIT.MemCached;
using Utility;

namespace DAL.Base
{
    public class MemCacheHander
    {
        private static MemcachedClient baseCache = MemcachedClient.GetInstance("BaseMemCache");

        #region 基本操作

        public static MemcachedClient GetClient()
        {
            return baseCache;
        }

        /// <summary>
        /// 从缓存中得到对象
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static object GetValue(string strKey)
        {
            return baseCache.Get(strKey);
        }
         
        public static T  GetValue<T>(string strKey) where T : class  ,new()
        {
            T model = default(T);
            byte[] bt = baseCache.Get(strKey) as byte[];
            if (bt != null)
            {
                model = Utility.Serialize.SerializeHelper.DesFromBin<T>(bt);
            }
            return model; 
        }


        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static bool SetValue(string strKey, object objValue)
        {
            return baseCache.Set(strKey, Utility.Serialize.SerializeHelper.SerToBin(objValue));
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="objValue"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public static bool SetValue(string strKey, object objValue, DateTime expiry)
        {
            return baseCache.Set(strKey, Utility.Serialize.SerializeHelper.SerToBin(objValue), expiry);
        }

        public static bool SetValue(string strKey, object objValue, TimeSpan expiry)
        {
            return baseCache.Set(strKey, Utility.Serialize.SerializeHelper.SerToBin(objValue), expiry);
        }
 

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static bool SetValueNotSerial(string strKey, object objValue)
        {
            return baseCache.Set(strKey, objValue);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="objValue"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public static bool SetValueNotSerial(string strKey, object objValue, DateTime expiry)
        {
            return baseCache.Set(strKey, objValue, expiry);
        }

        public static bool SetValueNotSerial(string strKey, object objValue, TimeSpan expiry)
        {
            return baseCache.Set(strKey, objValue, expiry);
        }

        /// <summary>
        ///  删除缓存项
        /// </summary>
        /// <param name="strkey"></param>
        /// <returns></returns>
        public static bool Delete(string strkey)
        {
            return baseCache.Delete(strkey);
        }

        public static bool FlushAll()
        {
            return baseCache.FlushAll();
        }

        /// <summary>
        /// 获取版本号。
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public static int GetVersion(string key)
        {
            int version = 1;
            object o = baseCache.Get(key); // 从远程缓存取
            if (o == null) // 如果远程缓存不存在
            {
                baseCache.Set(key, version); // 设置远程缓存
            }
            else
            {
                version = int.Parse(o.ToString());
            }
            return version;
        }
        /// <summary>
        /// 更新版本号。为防止冲突，采取将版本号加1的方式控制缓存键值的改变来达到更新缓存项的目的　
        /// </summary>
        /// <param name="key">缓存键值</param>
        public static void SetVersion(string key)
        {
            int newVersion = GetVersion(key) + 1;
            baseCache.Set(key, newVersion);
        }
        #endregion 基本操作

        #region 实体类缓存key
        /// <summary>
        /// 全局版本号
        /// </summary>
        public static int GetValue_Model_All_Vision(long clsGuid)
        {
            return GetVersion(string.Format(BaseMemCacheKey.Vision_Model_All, clsGuid));
        }

        /// <summary>
        /// 根据Id取memcache版本号的值
        /// </summary>
        public static int GetValue_Model_Vision(long clsGuid, int Id)
        {
            return GetVersion(string.Format(BaseMemCacheKey.Vision_Model_Id, GetValue_Model_All_Vision(clsGuid), Id));
        }
        /// <summary>
        /// 根据字符串id取memcache版本号
        /// </summary>
        public static int GetValue_Model_Vision(long clsGuid, string strId)
        {
            return GetVersion(string.Format(BaseMemCacheKey.Vision_Model_Id, GetValue_Model_All_Vision(clsGuid), strId));
        }
        /// <summary>
        /// 根据id取memcache版本号的key
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>缓存项键值</returns>
        public static string GetKey_Model_Value(long clsGuid, int Id)
        {
            return string.Format(BaseMemCacheKey.Key_Model_Id, GetValue_Model_All_Vision(clsGuid), GetValue_Model_Vision(clsGuid, Id), Id);
        }
        /// <summary>
        /// 根据字符串id取memcache版本号的key
        /// </summary>
        /// <param name="Id">字符串Id值</param>
        /// <returns>缓存项键值</returns>
        public static string GetKey_Model_Value(long clsGuid, string Id)
        {
            return string.Format(BaseMemCacheKey.Key_Model_Id, GetValue_Model_All_Vision(clsGuid), GetValue_Model_Vision(clsGuid, Id), Id);
        }

        /// <summary>
        /// 根据where查询条件取memcache版本号的key
        /// </summary>
        /// <param name="clsGuid"></param>
        /// <param name="checksumWhere">查询条件的checksum</param>
        /// <returns></returns>
        public static string GetKey_Model_Where(long clsGuid, long checksumWhere)
        {
            return string.Format(BaseMemCacheKey.Key_Model_Where, GetValue_Model_All_Vision(clsGuid), checksumWhere);
        }
        #endregion 实体类缓存key
    }
}