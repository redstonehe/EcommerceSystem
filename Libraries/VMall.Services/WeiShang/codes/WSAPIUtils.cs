using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;


namespace VMall.Services
{
    /// <summary>
    /// 系统工具类。
    /// </summary>
    public abstract class WSAPIUtils
    {
        /// <summary>
        /// Sign 请求签名。
        /// </summary>
        /// <param name="parameters">所有字符型的TOP请求参数</param>
        /// <param name="secret">签名密钥</param>
        /// <param name="qhs">是否前后都加密钥进行签名</param>
        /// <returns>签名</returns>
        public static string SignRequest(IDictionary<string, string> Sys_parameters,IDictionary<string, string> Bus_parameters, string appSecret)
        {
            // 第一步：把字典按Key的字母顺序排序
            //IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            //IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            //// 第二步：把所有参数名和参数值串在一起
            //StringBuilder query = new StringBuilder();
            ////query.Append(appSecret);
           
            //while (dem.MoveNext())
            //{
            //    string key = dem.Current.Key;
            //    string value = dem.Current.Value;
            //    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && !key.Equals("sign"))
            //    {
            //        query.Append(key).Append("=").Append(value).Append("&");
            //    }
            //}
            //query.Remove(query.Length-1,1);

            //query.Append(appSecret);

            StringBuilder query = new StringBuilder();
            //query.Append(parameters["Host"] + parameters["ExtUid"] + parameters["IP"] + appSecret);
            query.Append(Sys_parameters["Host"] + Bus_parameters["ExtUid"] + appSecret);
            // 第三步：使用SHA1加密
            string result = EncryptToSHA1(query.ToString());

            return result.ToLower();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncryptToSHA1(string str)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] str1 = Encoding.UTF8.GetBytes(str);
            byte[] str2 = sha1.ComputeHash(str1);
            sha1.Clear();
            (sha1 as IDisposable).Dispose();
            return BitConverter.ToString(str2).Replace("-", "").ToLower();
        }
        /// <summary>
        /// SHA1加密方法
        /// </summary>
        /// <param name="old_string">原来的字符串作为参数传进来</param>
        /// <returns>new_string返回加密后的新字符串（长度是40位）</returns>
        public static string SHA1_Hash(string old_string)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_old_string = UTF8Encoding.Default.GetBytes(old_string);
            byte[] bytes_new_string = sha1.ComputeHash(bytes_old_string);
            string new_string = BitConverter.ToString(bytes_new_string);
            new_string = new_string.Replace("-", "").ToLower();
            return new_string;
        }
        /// <summary>
        /// MD5加密方法
        /// </summary>
        /// <param name="old_string">原来的字符串作为参数传进来</param>
        /// <returns>new_string返回加密后的新字符串（长度是32位）</returns>
        public static string MD5_Hash(string old_string)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes_old_string = UTF8Encoding.Default.GetBytes(old_string);
            byte[] bytes_new_string = md5.ComputeHash(bytes_old_string);
            string new_string = BitConverter.ToString(bytes_new_string);
            new_string = new_string.Replace("-", "").ToUpper();
            return new_string;
        }
        /// <summary>
        /// 将应用级输入参数转化为json格式的字符串
        /// </summary>
        /// <returns></returns>
        public static string MethodParameterToJsonString(IDictionary<string, string> parameters)
        {
            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            StringBuilder query = new StringBuilder();
            query.Append("{");
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.AppendFormat("\"{0}\":\"{1}\",", key, value);
                }
            }
            if (query.Length > 1)
            {
                query.Remove(query.Length - 1, 1);
            }
            query.Append("}");
            return query.ToString();
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }  

    }
}
