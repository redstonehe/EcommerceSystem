using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.IO;
namespace Com.LaKaLa
{
    public class Tools
    {
        // 随机生成字符串(自定义长度)
        public static string getRandomString(int length)
        {
            String radStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder generateRandStr = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                int randNum = rand.Next(36);
                generateRandStr.Append(radStr.Substring(randNum, 1));
            }
            return generateRandStr + "";
        }

        public static string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }

        public static byte[] ParseHexString(string text)
        {
            if ((text.Length % 2) != 0)
            {
                return null;
            }

            if (text.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                text = text.Substring(2);
            }

            int arrayLength = text.Length / 2;
            byte[] byteArray = new byte[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                byteArray[i] = byte.Parse(text.Substring(i * 2, 2), NumberStyles.HexNumber);
            }

            return byteArray;
        }

        public static string getSHA1(string str) {
            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();

            //将mystr转换成byte[]
            byte[] dataToHash = Encoding.GetEncoding("GBK").GetBytes(str);

            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);

            //将运算结果转换成string
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");

            return hash.ToLower();
        }



    }

    //public class Logs
    //{
    //    private static object ctx = new object();

    //    /// <summary>
    //    /// 记录日志
    //    /// </summary>
    //    /// <param name="filePrefix">文件前缀</param>
    //    /// <param name="title">日志标题</param>
    //    /// <param name="content">日志内容</param>
    //    //public static void WriteOperateLog(string filePrefix, string title, string content)
    //    //{
    //    //    StreamWriter sw = null;
    //    //    DateTime date = DateTime.Now;
    //    //    string foder = date.Year.ToString() + "/" + date.Month.ToString();
    //    //    string FileName = string.Empty;
    //    //    try
    //    //    {
    //    //        // string LogsRootPath = Environment.CurrentDirectory;
    //    //        string LogsRootPath = System.AppDomain.CurrentDomain.BaseDirectory + "/Logs";

    //    //        string logName = filePrefix + date.ToString("dd") + ".log";
    //    //        FileName = LogsRootPath + "/" + foder + "/" + logName;

    //    //        #region 检测日志目录是否存在
    //    //        if (!Directory.Exists(LogsRootPath + "/" + foder + ""))
    //    //        {
    //    //            Directory.CreateDirectory(LogsRootPath + "/" + foder + "");
    //    //        }
    //    //        lock (ctx)
    //    //        {
    //    //            if (!File.Exists(FileName))
    //    //                sw = File.CreateText(FileName);
    //    //            else
    //    //                sw = File.AppendText(FileName);
    //    //        }
    //    //        #endregion

    //    //        sw.WriteLine("Title：  " + title);
    //    //        sw.WriteLine("Content：" + content + "\r");
    //    //        sw.WriteLine("Time：   " + date.ToString());
    //    //        sw.WriteLine("---------------------------------------------------\r");
    //    //        sw.Flush();
    //    //        sw.Close();
    //    //    }
    //    //    finally
    //    //    {
    //    //        if (sw != null)
    //    //            sw.Close();
    //    //    }
    //    //}
    //    //public static string GetCustomerService(string sellerMemo)
    //    //{
    //    //    if (string.IsNullOrEmpty(sellerMemo) || !Utility.Text.CString.IsNumber(sellerMemo))
    //    //    {
    //    //        return string.Empty;
    //    //    }
    //    //    return sellerMemo;
    //    //}
    //}
}
