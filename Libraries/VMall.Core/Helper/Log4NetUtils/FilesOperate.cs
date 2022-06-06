using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace VMall.Core.Helper.Log4NetUtils
{
    /// <summary>
    /// 文件 操作
    /// </summary>
    public static class FilesOperate
    {
        /// <summary>
        /// 获取App的当前路径 \\结束
        /// </summary>
        /// <returns></returns>
        public static string getAppPath()
        {
            return GetAssemblyPath();
        }

        /// <summary>
        /// 获取Assembly的运行路径 \\结束
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyPath()
        {
            string sCodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

            sCodeBase = sCodeBase.Substring(8, sCodeBase.Length - 8);    // 8是 file:// 的长度

            string[] arrSection = sCodeBase.Split(new char[] { '/' });

            string sDirPath = "";
            for (int i = 0; i < arrSection.Length - 1; i++)
            {
                sDirPath += arrSection[i] + Path.DirectorySeparatorChar;
            }

            return sDirPath;
        }

        /// <summary>
        /// 文件夹复制
        /// </summary>
        /// <param name="sSourceDirName">原始路径</param>
        /// <param name="sDestDirName">目标路径</param>
        /// <returns></returns>
        public static bool CopyDirectory(string sSourceDirName, string sDestDirName)
        {
            if (string.IsNullOrEmpty(sSourceDirName) || string.IsNullOrEmpty(sDestDirName))
            {
                return false;
            }

            //不复制.svn文件夹
            if (sSourceDirName.EndsWith("svn"))
            {
                return true;
            }

            if (sSourceDirName.Substring(sSourceDirName.Length - 1) != Path.DirectorySeparatorChar.ToString())
            {
                sSourceDirName = sSourceDirName + Path.DirectorySeparatorChar;
            }
            if (sDestDirName.Substring(sDestDirName.Length - 1) != Path.DirectorySeparatorChar.ToString())
            {
                sDestDirName = sDestDirName + Path.DirectorySeparatorChar;
            }

            #region 复制函数
            if (Directory.Exists(sSourceDirName))
            {
                if (!Directory.Exists(sDestDirName))
                {
                    Directory.CreateDirectory(sDestDirName);
                }
                foreach (string item in Directory.GetFiles(sSourceDirName))
                {
                    File.Copy(item, sDestDirName + System.IO.Path.GetFileName(item), true);
                }
                foreach (string item in Directory.GetDirectories(sSourceDirName))
                {
                    CopyDirectory(item, sDestDirName + item.Substring(item.LastIndexOf(Path.DirectorySeparatorChar) + 1));
                }
            }
            return true;
            #endregion
        }


        /// <summary> 
        /// 启动其他的应用程序 
        /// </summary> 
        /// <param name="file">应用程序名称</param> 
        /// <param name="workdirectory">应用程序工作目录</param> 
        /// <param name="args">命令行参数</param> 
        /// <param name="style">窗口风格</param> 
        public static bool StartProcess(string file, string workdirectory, string args, ProcessWindowStyle style)
        {
            try
            {
                Process pMyProcess = new Process();
                ProcessStartInfo pStartInfo = new ProcessStartInfo(file, args);
                pStartInfo.WindowStyle = style;
                pStartInfo.WorkingDirectory = workdirectory;
                pMyProcess.StartInfo = pStartInfo;
                pMyProcess.StartInfo.UseShellExecute = false;
                pMyProcess.Start();
                return true;
            }
            catch (Exception ex)
            {
                //LogAPI.debug(ex);
                return false;
            }
        }

        /// <summary>
        /// 获得本地计算机名
        /// </summary>
        /// <returns></returns>
        public static string GetComputerName()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        /// 获得计算机IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIPAddress()
        {
            try
            {
                string sComputerName;
                sComputerName = GetComputerName();
                string sIpAddress = "";
                IPAddress[] addr = Dns.GetHostAddresses(sComputerName);
                //for (int i = 0; i < addr.Length; i++)
                //{
                //    sIpAddress += addr[i].ToString() + " ";
                //}
                sIpAddress = addr[0].ToString();
                return sIpAddress;
            }
            catch (Exception ep)
            {
                //LogAPI.debug(ep);
                return "127.0.0.1";
            }
        }

        /// <summary>
        /// 描述:创建目录
        /// </summary>
        /// <returns></returns>
        public static bool CreateFolder(string sFolder)
        {
            //如果临时文件夹不存在，则创建该文件夹
            if (!Directory.Exists(sFolder))
            {
                Directory.CreateDirectory(sFolder);
            }
            return true;
        }
    }

}
