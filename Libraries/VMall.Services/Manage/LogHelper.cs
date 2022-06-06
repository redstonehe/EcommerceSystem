using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VMall.Core;


namespace VMall.Services
{
    public class LogHelper
    {
        private static object ctx = new object();

        private static Logs LogBLL = new Logs();
        /// 记录日志
        /// </summary>
        /// <param name="filePrefix">文件前缀</param>
        /// <param name="title">日志标题</param>
        /// <param name="content">日志内容</param>
        /// <param name="logLevel"5.>FATAL > 4.ERROR >3. WARN > 2.INFO >1. DEBUG,Error 类型写入</param>
        public static void WriteOperateLog(string filePrefix, string title, string content, int logLevel = (int)LogLevelEnum.INFO)
        {
            StreamWriter sw = null;
            DateTime date = DateTime.Now;
            string foder = date.Year.ToString() + "-" + date.Month.ToString();
            string FileName = string.Empty;
            try
            {
                // string LogsRootPath = Environment.CurrentDirectory;
                string LogsRootPath = System.AppDomain.CurrentDomain.BaseDirectory + "/Logs";

                string logName = filePrefix + date.ToString("dd") + ".log";
                FileName = LogsRootPath + "/" + foder + "/" + logName;

                #region 检测日志目录是否存在
                if (!Directory.Exists(LogsRootPath + "/" + foder + ""))
                {
                    Directory.CreateDirectory(LogsRootPath + "/" + foder + "");
                }
                lock (ctx)
                {
                    if (!File.Exists(FileName))
                        sw = File.CreateText(FileName);
                    else
                        sw = File.AppendText(FileName);
                }
                #endregion

                sw.WriteLine("Title：  " + title);
                sw.WriteLine("Content：" + content + "\r");
                sw.WriteLine("Time：   " + date.ToString());
                sw.WriteLine("---------------------------------------------------\r");
                sw.Flush();
                sw.Close();
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }

            if (logLevel == (int)LogLevelEnum.ERROR)
            {
                try
                {
                    //错误日志写入数据库
                    LogsInfo log = new LogsInfo();
                    log.LogDate = DateTime.Now;
                    log.Level = LogLevelEnum.ERROR.ToString();
                    log.Title = title;
                    log.Message = content;
                    log.IP = WebHelper.GetIP();
                    LogBLL.Add(log);
                }
                catch (Exception ex) { }
            }

        }


        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="UserName"></param>
        /// <param name="Title"></param>
        /// <param name="Content"></param>
        /// <param name="IP"></param>
        public static void WriteOperateLog(string Title, string Content)
        {
            StreamWriter sw = null;
            DateTime date = DateTime.Now;
            string foder = date.Year.ToString() + "/" + date.Month.ToString();
            string FileName = string.Empty;
            try
            {
                string LogsRootPath = String.Empty;
                if (HttpContext.Current != null)
                {
                    LogsRootPath = HttpContext.Current.Server.MapPath("/Logs"); //ConfigurationManager.AppSettings["LogsRootPath"].ToString();
                }
                else
                {
                    LogsRootPath = HttpRuntime.AppDomainAppPath + "Logs";
                }
                string logName = date.ToString("dd") + ".log";
                FileName = LogsRootPath + "/" + foder + "/" + logName;

                #region 检测日志目录是否存在
                if (!Directory.Exists(LogsRootPath + "/" + foder + ""))
                {
                    Directory.CreateDirectory(LogsRootPath + "/" + foder + "");
                }

                if (!File.Exists(FileName))
                    sw = File.CreateText(FileName);
                else
                    sw = File.AppendText(FileName);
                #endregion

                sw.WriteLine("Title：  " + Title);
                sw.WriteLine("Content：" + Content + "\r");
                sw.WriteLine("Time：   " + date.ToString());
                sw.WriteLine("---------------------------------------------------\r");
                sw.Flush();
                sw.Close();
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LogsRootPath">存放日志路径</param>
        /// <param name="filePrefix">文件名前缀</param>
        /// <param name="title">日志标题</param>
        /// <param name="content">类容</param>
        public static void WriteOperateLog(string LogsRootPath, string filePrefix, string title, string content)
        {
            object ctx = new object();
            StreamWriter sw = null;
            DateTime date = DateTime.Now;
            string foder = date.Year.ToString() + "/" + date.Month.ToString();
            string FileName = string.Empty;
            try
            {
                string logName = filePrefix + date.ToString("dd") + ".log";
                FileName = LogsRootPath + "/" + foder + "/" + logName;

                #region 检测日志目录是否存在
                if (!Directory.Exists(LogsRootPath + "/" + foder + ""))
                {
                    Directory.CreateDirectory(LogsRootPath + "/" + foder + "");
                }
                lock (ctx)
                {
                    if (!File.Exists(FileName))
                        sw = File.CreateText(FileName);
                    else
                        sw = File.AppendText(FileName);
                }
                #endregion

                sw.WriteLine("Title：  " + title);
                sw.WriteLine("Content：" + content + "\r");
                sw.WriteLine("Time：   " + date.ToString());
                sw.WriteLine("---------------------------------------------------\r");
                sw.Flush();
                sw.Close();
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }

        }


        //public static string GetCustomerService(string sellerMemo)
        //{
        //    if (string.IsNullOrEmpty(sellerMemo) || !Utility.Text.CString.IsNumber(sellerMemo))
        //    {
        //        return string.Empty;
        //    }
        //    return sellerMemo;
        //}
    }
}
