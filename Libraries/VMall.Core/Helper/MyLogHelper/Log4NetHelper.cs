using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VMall.Core.Helper
{
    /// <summary>  
    /// LogHelper的摘要说明。   
    /// </summary>   
    public class Log4NetHelper
    {
        /// <summary>
        /// 静态只读实体对象info信息
        /// </summary>
        public static readonly log4net.ILog Loginfo = log4net.LogManager.GetLogger("AppLogger");
        /// <summary>
        ///  静态只读实体对象error信息
        /// </summary>
        //public static readonly log4net.ILog Logerror = log4net.LogManager.GetLogger("logerror");

        /// <summary>
        ///  动态修改log4net组件的日志文件名
        /// </summary>
        /// <param name="iLog"></param>
        /// <param name="fileName"></param>
        private static void ChangeLog4netLogFileName(log4net.ILog iLog, string fileName,string filePath)
        {
            log4net.Core.LogImpl logImpl = iLog as log4net.Core.LogImpl;
            if (logImpl != null)
            {
                log4net.Appender.AppenderCollection ac = ((log4net.Repository.Hierarchy.Logger)logImpl.Logger).Appenders;
                for (int i = 0; i < ac.Count; i++)
                {     // 这里我只对RollingFileAppender类型做修改 
                    log4net.Appender.RollingFileAppender rfa = ac[i] as log4net.Appender.RollingFileAppender;
                    if (rfa != null)
                    {
                        rfa.File = fileName;
                        //检测日志目录是否存在
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        if (!System.IO.File.Exists(fileName))
                        {
                            //FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            //StreamWriter sr = new StreamWriter(fs, System.Text.Encoding.Default);

                            System.IO.File.Create(fileName).Dispose();//.Close();//.Dispose();
                            //or using
                            //using (System.IO.File.Create(fileName)) {  }
                            
                        }
                        
                        // 更新Writer属性 
                        rfa.Writer = new System.IO.StreamWriter(rfa.File, rfa.AppendToFile, rfa.Encoding);
                    }
                }
            }
        }
        /// <summary>
        ///  添加info信息
        /// </summary>
        /// <param name="info">自定义日志内容说明</param>
        public static void WriteLogWithName(string fileName)
        {
            try
            {
                if (Loginfo.IsInfoEnabled)
                {
                    //Loginfo.Info(info);
                   // string fileName = @" c:/chang.log ";
                    log4net.ILog iLog = log4net.LogManager.GetLogger("AppLogger");
                    DateTime date = DateTime.Now;
                    string foder = date.Year.ToString() + "-" + date.Month.ToString();
                    string LogsRootPath = System.AppDomain.CurrentDomain.BaseDirectory + "/Logs";
                   
                    string logName = fileName +date.ToString("dd") + ".log";
                    string fName = LogsRootPath + "/" + foder + "/" + logName;
                    string filePath = LogsRootPath + "/" + foder;
                    ChangeLog4netLogFileName(iLog, fName, filePath);
                    iLog.Info(" Test:info ");
                }
            }
            catch { }
        }


        /// <summary>
        /// 添加异常信息
        /// </summary>
        /// <param name="info">自定义日志内容说明</param>
        /// <param name="ex">异常信息</param>
        //public static void WriteLog(string info, Exception ex)
        //{
        //    try
        //    {
        //        if (Logerror.IsErrorEnabled)
        //        {
        //            Logerror.Error(info, ex);
        //        }
        //    }
        //    catch { }
        //}
    }
}
