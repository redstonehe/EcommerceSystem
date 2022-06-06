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
    public abstract class SDDAPIUtils
    {
        /// <summary>
        /// Sign 请求签名。
        /// </summary>
        /// <param name="parameters">所有字符型的TOP请求参数</param>
        /// <param name="secret">签名密钥</param>
        /// <param name="qhs">是否前后都加密钥进行签名</param>
        /// <returns>签名</returns>
        public static string SignRequest(IDictionary<string, string> parameters, string appSecret,string subUrl)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder();
            //query.Append(appSecret);
            query.Append(subUrl+"?");
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && !key.Equals("sign"))
                {
                    query.Append(key).Append("=").Append(value).Append("&");
                }
            }
            query.Remove(query.Length-1,1);
            //query.Append(appSecret);
            // 第三步：使用MD5加密
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(Uri.EscapeDataString(query.ToString())));

            // 第四步：把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                string hex = bytes[i].ToString("X");
                if (hex.Length == 1)
                {
                    result.Append("0");
                }
                result.Append(hex);
            }

            return result.ToString().ToLower();
        }

        /// <summary>
        /// SignKey 请求签名。
        /// </summary>
        /// <param name="parameters">所有字符型的TOP请求参数</param>
        /// <param name="secret">签名密钥</param>
        /// <param name="qhs">是否前后都加密钥进行签名</param>
        /// <returns>签名</returns>
        public static string SignKeyRequest(IDictionary<string, string> parameters, string appSecret, string subUrl)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder();
            query.Append(subUrl + "?");
            //query.Append(appSecret);
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && !key.Equals("sign"))
                {
                    query.Append(key).Append("=").Append(value).Append("&");
                }
            }
            query.Append( appSecret);
            //Uri.EscapeDataString(query.ToString());
            
            //query.Append(appSecret);
            // 第三步：使用MD5加密
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(Uri.EscapeDataString(query.ToString())));

            // 第四步：把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                string hex = bytes[i].ToString("X");
                if (hex.Length == 1)
                {
                    result.Append("0");
                }
                result.Append(hex);
            }

            return result.ToString().ToLower();
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

        /// <summary>
        /// 将list集合转换为json格式
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        //public static string getOrder(List<EntityOrders> orders)
        //{
        //    //订单属性num_iid-商品数字编号:Number  sku_id-Sku的ID:Number   num-商品购买数量:Number  outer_sku_id-商家编码（商家为Sku设置的外部编号):String
        //    //title-商品标题:String  price-商品价格。精确到2位小数；单位：元:Price  sku_properties_name-SKU的值，即：商品的规格。如：黑色、39:String；
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("{\"orders\":[");
        //    for (int i = 0; i < orders.Count; i++)
        //    {
        //        sb.Append("{\"num_iid\":\"" + orders[i].num_iid.ToString() + "\",\"sku_id\":\"" + orders[i].sku_id.ToString() + "\",\"num\":\"" + orders[i].num.ToString() + "\",\"outer_sku_id\":\"" + orders[i].outer_sku_id.ToString() + "\",\"title\":\"" + orders[i].title.ToString() + "\",\"price\":\"" + orders[i].price.ToString() + "\",\"sku_properties_name\":\"" + orders[i].sku_properties_name.ToString() + "\"}");
        //        if (i < orders.Count - 1)
        //        {
        //            sb.Append(",");
        //        }
        //    }
        //    sb.Append("]}");
        //    return sb.ToString();
        //}

    }

    public class ProductEntity {
        /// <summary>
        /// 产品ID 充值时传给服务器
        /// </summary>
        public int pid { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string pname{ get; set; }
        /// <summary>
        /// 面值
        /// </summary>
        public float pvalue { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string punit { get; set; }
        /// <summary>
        /// 小分类，移动是100,联通是101,电信是102，固话是103
        /// </summary>
        public int psubcate { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int pchannel { get; set; }
        /// <summary>
        /// 省份ID
        /// </summary>
        public int pareaid { get; set; }
        /// <summary>
        /// 商品大类 话费业务是1，QQ业务是2，游戏业务是3
        /// </summary>
        public int pcate { get; set; }
        /// <summary>
        /// 库存 大于0均表示库存充足
        /// </summary>
        public int pstock { get; set; }
        /// <summary>
        /// 排序，值越小越靠前 
        /// </summary>
        public int porder { get; set; }
        /// <summary>
        /// 限购数量
        /// </summary>
        public string plimit { get; set; }
        /// <summary>
        /// 豪华版进价
        /// </summary>
        public decimal pmarkprice { get; set; }
    }
}
