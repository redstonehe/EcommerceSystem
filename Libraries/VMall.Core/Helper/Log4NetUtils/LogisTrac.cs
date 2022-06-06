using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace VMall.Core.Helper.Log4NetUtils
{
    /// <summary>
    /// 日志记录类(记录到文本文件中)
    /// </summary>
    public static class LogisTrac
    {
        private static readonly string LOG_DIR = "日志";
        private static readonly string LOG_FILE = LOG_DIR + "\\log" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
        private const string LOG4NET_CONFIG = "log4net_config.xml";
        private static readonly log4net.ILog m_log = log4net.LogManager.GetLogger(typeof(LogisTrac));


        static LogisTrac()
        {
            try
            {
                ConfigureLoad();
            }
            catch { }
        }

        /// <summary>
        /// 返回ILog接口
        /// </summary>
        private static log4net.ILog Log
        {
            get
            {
                return m_log;
            }
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
            WriteLog("--------------------------------------[本次异常开始]--------------------------------------");
            WriteLog("Message : " + e.Message);
            WriteLog("Source : " + e.Source);
            WriteLog("StackTrace : " + e.StackTrace);
            WriteLog("TargetSite : " + e.TargetSite);
            WriteLog("--------------------------------------[本次异常结束]--------------------------------------\r\n");
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

            //查看Log文件夹是否存在，如果不存在，则创建
            string sLogDir = sPath + LOG_DIR;
            if (!Directory.Exists(sLogDir))
            {
                Directory.CreateDirectory(sLogDir);
            }
            string sLogFile = sPath + LOG_FILE;
            sPath += LOG4NET_CONFIG;
            doc.Load(@sPath);
            XmlElement myElement = doc.DocumentElement;

            //修改log.txt的路径
            XmlNode pLogFileAppenderNode = myElement.SelectSingleNode("descendant::appender[@name='LogFileAppender']/file");
            // Create an attribute collection from the element.
            XmlAttributeCollection attrColl = pLogFileAppenderNode.Attributes;
            attrColl[0].Value = sLogFile;

            log4net.Config.XmlConfigurator.Configure(myElement);
        }

    }

}
