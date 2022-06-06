using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace VMall.Core.Helper.Log4NetUtils
{
    /// <summary>
    /// 日志记录类(记录到数据库) 
    /// </summary>
    public static class LogisTracToSqlDB
    {
        private static readonly log4net.ILog m_log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string LOG4NET_CONFIG = "log4net_config.xml";

        static LogisTracToSqlDB()
        {
            try
            {
                ConfigureLoad();
            }
            catch { }
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="sInfo"></param>
        public static void WriteLog(string sInfo)
        {
            m_log.Error(sInfo);
        }
        /// <summary>
        /// 记录debug信息
        /// </summary>
        /// <param name="e"></param>
        public static void WriteLog(Exception e)
        {
            WriteLog(e.ToString());
            //WriteLog("--------------------------------------[本次异常开始]--------------------------------------");
            //WriteLog("Message : " + e.Message);
            //WriteLog("Source : " + e.Source);
            //WriteLog("StackTrace : " + e.StackTrace);
            //WriteLog("TargetSite : " + e.TargetSite);
            //WriteLog("--------------------------------------[本次异常结束]--------------------------------------\r\n");
        }

        /// <summary>
        /// 配置log4net环境
        /// </summary>
        private static void ConfigureLoad()
        {
            XmlDocument doc = new XmlDocument();
            //使用当前dll路径
            string sPath = FilesOperate.GetAssemblyPath();
            if (!sPath.EndsWith("\\"))
            {
                sPath += "\\";
            }
            sPath += LOG4NET_CONFIG;
            doc.Load(@sPath);
            XmlElement myElement = doc.DocumentElement;
            log4net.Config.XmlConfigurator.Configure(myElement);
        }
    }

}
