using System;
using System.IO;

namespace VMall.Core
{
    /// <summary>
    /// VMall短信管理类
    /// </summary>
    public class BMASMS
    {
        private static ISMSStrategy _ismsstrategy = null;//短信策略

        static BMASMS()
        {
            try
            {
                string[] fileNameList = Directory.GetFiles(System.Web.HttpRuntime.BinDirectory, "VMall.SMSStrategy.*.dll", SearchOption.TopDirectoryOnly);
                _ismsstrategy = (ISMSStrategy)Activator.CreateInstance(Type.GetType(string.Format("VMall.SMSStrategy.{0}.SMSStrategy, VMall.SMSStrategy.{0}", fileNameList[0].Substring(fileNameList[0].IndexOf("SMSStrategy.") + 12).Replace(".dll", "")),
                                                                                   false,
                                                                                   true));
            }
            catch
            {
                throw new BMAException("创建'短信策略对象'失败,可能存在的原因:未将'短信策略程序集'添加到bin目录中;'短信策略程序集'文件名不符合'VMall.SMSStrategy.{策略名称}.dll'格式");
            }
        }

        /// <summary>
        /// 短信策略实例
        /// </summary>
        public static ISMSStrategy Instance
        {
            get { return _ismsstrategy; }
        }
    }
}
