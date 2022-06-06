using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace VMall.Web.Mobile
{
    public class UpfilePublic
    {
        public static string[] UploadPhoto(HttpPostedFileBase FileData, string extensionValue, string extensionText)
        {
            bool fileOk = false;
            string filename = "";
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/userheadpic/" ;
            string[] returnString = new string[2] { "头像修改成功", "" };
            if (string.IsNullOrEmpty(extensionValue))
                extensionValue = "255216,7173,6677,13780";//"说明255216是jpg;7173是gif;6677是BMP,13780是PNG;";
            if (string.IsNullOrEmpty(extensionText))
                extensionText = "jpg,jpeg,gif,bmp,png";//"说明255216是jpg;7173是gif;6677是BMP,13780是PNG;";

            //判断是否有选择文件
            if (string.IsNullOrEmpty(FileData.FileName))
            {
                returnString[0] = "没有选择文件";
            }
            else
            {
                byte[] file = new Byte[FileData.ContentLength];
                Stream fsRead = FileData.InputStream;
                fsRead.Read(file, 0, FileData.ContentLength);

                fileOk = IsAllowedExtension(file, extensionValue);
                fsRead.Flush();
                fsRead.Close();
                if (fileOk)
                {
                    int size = 1024 * 500;//默认1MB

                    //判断图片大小是否在要求范围内
                    if (FileData.ContentLength <= size)
                    {
                        try
                        {
                            //定义图片文件名
                            filename = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + Path.GetExtension(FileData.FileName);//以日期为新的文件名

                            if (path != "")
                            {
                                //创建存放图片文件夹
                                if (!Directory.Exists(path))
                                    Directory.CreateDirectory(path);
                                FileStream fsWrite = new FileStream(Path.Combine(path, filename), FileMode.Create, FileAccess.Write);
                                fsWrite.Write(file, 0, file.Length);
                                fsWrite.Flush();
                                fsWrite.Close();
                                returnString[1] = filename;
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        //图片大小超出要求
                        returnString[0] = "图片过大";
                    }
                }
                else
                {
                    returnString[0] = "图片格式不正确";
                }
            }
            return returnString;
        }

        //真正是否真的为图片
        public static bool IsAllowedExtension(byte[] file, string strExtensionValue)
        {
            // FileStream fs = new FileStream(FileData.FileName, FileMode.Open, FileAccess.Read);
            // BinaryReader r = new BinaryReader(fs);
            bool bReturn = false;
            string fileclass = "";
            byte buffer;
            try
            {
                buffer = file[0];
                fileclass = buffer.ToString();
                buffer = file[1];
                fileclass += buffer.ToString();
            }
            catch { }
            //r.Close();
            string[] allowedExtensions = strExtensionValue.Split(',');
            for (int i = 0; i < allowedExtensions.Length; i++)
            {
                if (fileclass == allowedExtensions[i])
                {
                    bReturn = true;
                    break;
                }
            }
            return bReturn;
        }
    }
}
