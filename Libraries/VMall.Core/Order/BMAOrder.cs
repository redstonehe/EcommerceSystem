using System;
using System.IO;

namespace VMall.Core
{
    /// <summary>
    /// VMall订单管理类
    /// </summary>
    public class BMAOrder
    {
        private static IOrderStrategy _iorderstrategy = null;//订单策略

        static BMAOrder()
        {
            try
            {
                string[] fileNameList = Directory.GetFiles(System.Web.HttpRuntime.BinDirectory, "VMall.OrderStrategy.*.dll", SearchOption.TopDirectoryOnly);
                _iorderstrategy = (IOrderStrategy)Activator.CreateInstance(Type.GetType(string.Format("VMall.OrderStrategy.{0}.OrderStrategy, VMall.OrderStrategy.{0}", fileNameList[0].Substring(fileNameList[0].IndexOf("OrderStrategy.") + 14).Replace(".dll", "")),
                                                                                        false,
                                                                                        true));
            }
            catch
            {
                throw new BMAException("创建'订单策略对象'失败,可能存在的原因:未将'订单策略程序集'添加到bin目录中;'订单策略程序集'文件名不符合'VMall.OrderStrategy.{策略名称}.dll'格式");
            }
        }

        /// <summary>
        /// 订单策略实例
        /// </summary>
        public static IOrderStrategy Instance
        {
            get { return _iorderstrategy; }
        }
    }
}
