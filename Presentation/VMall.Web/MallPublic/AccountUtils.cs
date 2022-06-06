using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HuiGouMall.Core;
using HuiGouMall.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HuiGouMall.Web.Models;
using System.Drawing;
using ThoughtWorks.QRCode.Codec;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace HuiGouMall.Web
{
    public class AccountUtils
    {
        private static object ctx = new object();//锁对象

        private static string dirSaleApiUrl = WebSiteConfig.DirsaleApiUrl;

        /// <summary>
        /// 注册时判断用户名是否存在于直销系统中
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public static bool IsUserExistForDirSale(string accountName)
        {

            string url = dirSaleApiUrl + "/api/User/IsUserExist?userName=" + accountName;

            string FromDirSale = WebHelper.DoGet(url);
            JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            JToken token = (JToken)jsonObject;
            if (token["Result"].ToString() == "0")
            {
                return true;

            }
            return false;
        }

        /// <summary>
        /// 根据推荐人名称获取会员id
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static int GetUidByPname(string pName, out int pType)
        {
            int uid = 0;
            pType = 1;
            PartUserInfo userinfo;
            if (ValidateHelper.IsEmail(pName))
            {
                userinfo = Users.GetPartUserByEmail(pName);
                if (userinfo != null)
                {
                    if (userinfo.IsDirSaleUser)
                    {
                        uid = userinfo.DirSaleUid;
                        pType = (int)UserPanertType.DirSaleUser;
                    }
                    else
                    {
                        uid = userinfo.Uid;
                        pType = (int)UserPanertType.HaiHuiUser;

                    }

                }
            }
            else if (ValidateHelper.IsMobile(pName))
            {

                userinfo = Users.GetPartUserByMobile(pName);
                if (userinfo != null)
                {
                    if (userinfo.IsDirSaleUser)
                    {
                        uid = userinfo.DirSaleUid;
                        pType = (int)UserPanertType.DirSaleUser;
                    }
                    else
                    {
                        uid = userinfo.Uid;
                        pType = (int)UserPanertType.HaiHuiUser;

                    }
                }
            }
            else
            {
                userinfo = Users.GetPartUserByName(pName);
                if (userinfo != null)
                {
                    if (userinfo.IsDirSaleUser)
                    {
                        uid = userinfo.DirSaleUid;
                        pType = (int)UserPanertType.DirSaleUser;
                    }
                    else
                    {
                        uid = userinfo.Uid;
                        pType = (int)UserPanertType.HaiHuiUser;

                    }

                }
            }
            //海汇会员中不存在
            if (uid < 1)
            {
                string url = dirSaleApiUrl + "/api/User/GetUserIdByName?userName=" + pName;
                string FromDirSale = WebHelper.DoGet(url);
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    uid = TypeHelper.ObjectToInt(token["Info"]);
                    pType = (int)UserPanertType.DirSaleUser;
                }
            }
            return uid;
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="nr"></param>
        public static string CreateCode_Simple(string nr, int uid, string salt)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 5;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            string filename = "sharecode-" + uid.ToString() + "-" + salt + ".jpg";
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "/upload/usersharecode/" + filename;

            string RootPath = System.AppDomain.CurrentDomain.BaseDirectory + "/upload/usersharecode";

            #region 检测日志目录是否存在
            if (!Directory.Exists(RootPath + "/" + ""))
            {
                Directory.CreateDirectory(RootPath + "/" + "");
            }
            lock (ctx)
            {
                if (!System.IO.File.Exists(filepath))
                {
                    Bitmap img = qrCodeEncoder.Encode(nr);
                    img.Save(filepath, System.Drawing.Imaging.ImageFormat.Png);
                    img.Dispose();
                }
                //else
                //    return filename;
            }
            #endregion

            return filename;
        }

        public static List<AccountInfo> GetAccountList(int uid, bool isDirUser, int dirUid)
        {
            List<AccountInfo> accountInfoList = new List<AccountInfo>();
            if (!isDirUser)
            {//海汇会员获取余额信息
                accountInfoList = Account.GetAccountInfoListByUid(uid);
            }
            else
            {//直销会员获取余额信息

                string url = dirSaleApiUrl + "/api/User/GetUserDetails?userId=" + dirUid;

                string FromDirSale = WebHelper.DoGet(url);
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    JToken accountInfo = token["Info"].SelectToken("Accounts");
                    foreach (JObject item in accountInfo)
                    {
                        AccountInfo info = new AccountInfo();
                        info.AccountId = TypeHelper.ObjectToInt(item.SelectToken("AccountId"));
                        info.AccountName = item.SelectToken("AccountName").ToString();
                        info.Banlance = Convert.ToDecimal(item.SelectToken("Balance"));
                        accountInfoList.Add(info);
                    }
                }
            }
            return accountInfoList;
        }
    }
}