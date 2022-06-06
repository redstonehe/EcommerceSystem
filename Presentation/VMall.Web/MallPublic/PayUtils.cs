using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HuiGouMall.Core;
using HuiGouMall.Services;
using HuiGouMall.Web.Framework;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Data.SqlClient;
using HuiGouMall.Web.Models;
using System.Data;

namespace HuiGouMall.Web
{
    public class PayUtils
    {

        private static string dirSaleApiUrl = WebSiteConfig.DirsaleApiUrl;


        /// <summary>
        /// 发送外部订单
        /// </summary>
        /// <param name="partUserInfo">会员信息</param>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="fullShipAddressInfo">地址信息</param>
        public static bool SendOutOrder(OrderInfo orderInfo, PartUserInfo userInfo)
        {
            try
            {
                RegionInfo regionInfo = Regions.GetRegionById(orderInfo.RegionId);
                string userCode = string.IsNullOrEmpty(userInfo.UserName) ? (string.IsNullOrEmpty(userInfo.Email) ? (string.IsNullOrEmpty(userInfo.Mobile) ? "" : userInfo.Mobile) : userInfo.Email) : userInfo.UserName; //userCode：登录名
                string password = System.Web.HttpUtility.UrlEncode(SecureHelper.EncryptString(getCookiePassword(), DirSaleUserInfo.EncryptKey));
                string orderUrl = string.Format("{0}/api/product/CreateUserOrder?userCode={1}&password={2}&consignee={3}&phone={4}&orderCode={5}&provice={6}&city={7}&area={8}&address={9}&postCode={10}&proData={11}&userId={12}&orderType={13}", dirSaleApiUrl, userCode, password, orderInfo.Consignee, orderInfo.Mobile, orderInfo.OSN, regionInfo.ProvinceName, regionInfo.CityName, regionInfo.Name, orderInfo.Address, orderInfo.ZipCode, getOrderProductStr(Orders.GetOrderProductList(orderInfo.Oid)), userInfo.IsDirSaleUser ? userInfo.DirSaleUid : userInfo.Uid, userInfo.IsDirSaleUser ? (int)OrderSource.直销后台 : (int)OrderSource.汇购网);
                string FromDirSale2 = WebHelper.DoGet(orderUrl);
                JObject jsonObject2 = (JObject)JsonConvert.DeserializeObject(FromDirSale2);
                JToken token2 = (JToken)jsonObject2;

                //传递成功后需要在汇购网标识已经传递成功的状态
                if (token2["Result"].ToString() == "0")
                {
                    return true;
                }
                else
                {
                    Logs.WriteOperateLog("FailSendOutOrder", "发送外部订单失败（直销系统）", "错误信息为：会员ID：" + userInfo.Uid + "|订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|失败原因：" + token2["Msg"].ToString());
                    return false;

                }
            }
            catch (Exception ex)
            {
                Logs.WriteOperateLog("ERRORSendOutOrder", "发送外部订单异常（直销系统）", "错误信息为：会员ID：" + userInfo.Uid + "|订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|失败原因：" + ex.Message);
                return false;
            }

        }

        /// <summary>
        /// 将list集合转换为json格式
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static string getOrderProductStr(List<OrderProductInfo> orders)
        {
            //{"list":[{"ProSN":"xxx","ProCount":"xxx"},{"ProSN":"xxx","ProCount":"xxx"}]}
            //ProSN表示产品编号，ProCount表示产品数量
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"list\":[");
            for (int i = 0; i < orders.Count; i++)
            {
                sb.Append("{\"ProSN\":\"" + orders[i].PSN.ToString() + "\",\"ProCount\":\"" + orders[i].BuyCount.ToString() + "\"}");
                if (i < orders.Count - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("]}");
            return sb.ToString();
        }

        /// <summary>
        /// 获取cookie中的密码
        /// </summary>
        /// <returns></returns>
        public static string getCookiePassword()
        {
            string entryStr = MallUtils.GetBMACookie("random");
            return MallUtils.AESDecrypt(entryStr).Substring(18, MallUtils.AESDecrypt(entryStr).Length - 38);
        }


        /// <summary>
        /// 获取推荐人编号及验证
        /// </summary>
        /// <returns></returns>
        public static string GetParentCode(int uid)
        {
            PartUserInfo partUserInfo = Users.GetPartUserById(uid);

            string pCode = string.Empty;
            if (partUserInfo != null)
            {
                if (partUserInfo.Ptype == 2 && partUserInfo.Pid > 0)
                {
                    try
                    {
                        //string ApiUrlHost = "http://192.168.88.99:8088";
                        //http://192.168.88.99:8088/api/User/GetUserCode?userId=235
                        string url = string.Format("{0}/api/User/GetUserCode?userId={1}", dirSaleApiUrl, partUserInfo.Pid);

                        string FromDirSale = WebHelper.DoGet(url);
                        JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                        JToken token = (JToken)jsonObject;
                        if (token["Result"].ToString() == "0")
                        {
                            pCode = token["Info"].ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logs.WriteOperateLog("GetUserCode", "请求api接口错误", "错误信息为：会员ID：" + uid + "推荐人id" + partUserInfo.Pid + "错误信息" + ex.Message);
                    }
                }
            }
            return pCode;
        }

        public static string[] GetAccountDetail(int uid)
        {
            string[] account = new string[2];
            List<AccountInfo> accountInfoList = new List<AccountInfo>();
            SqlParameter[] parms = {
                                        new SqlParameter("@uid", uid)    
                                    };
            string commandText = string.Format(@"SELECT * FROM dbo.hlh_account WHERE [AccountId] IN (9,10) AND [UserId]=@uid ORDER BY [AccountId]", RDBSHelper.RDBSTablePre);

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    AccountInfo accountInfo = new AccountInfo();
                    accountInfo.AccountId = TypeHelper.ObjectToInt(reader["AccountId"]);
                    accountInfo.UserId = TypeHelper.ObjectToInt(reader["UserId"]);
                    accountInfo.Banlance = Convert.ToDecimal(reader["Banlance"]);
                    accountInfo.LockBanlance = Convert.ToDecimal(reader["LockBanlance"]);
                    accountInfoList.Add(accountInfo);
                }
                reader.Close();
            }
            account[0] = accountInfoList.Find(x => x.AccountId == 9) == null ? "0.0000" : accountInfoList.Find(x => x.AccountId == 9).Banlance.ToString();//红包账户
            account[1] = accountInfoList.Find(x => x.AccountId == 10) == null ? "0.0000" : accountInfoList.Find(x => x.AccountId == 10).Banlance.ToString();//海米账户
            return account;
        }

        /// <summary>
        /// 更新改账号存在于直销后台的数据库字段标识,更新用户外部状态并保存直销会员id
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static int UpdateUserOutSates(int dirSaleUid,int uid)
        {

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@uid", uid)
                                                            , new SqlParameter("@dirsaleuid", dirSaleUid) };
            string SqlStr = "UPDATE dbo.hlh_users SET isdirsaleuser=1,dirsaleuid=@dirsaleuid WHERE uid=@uid";

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, SqlStr, parameters));
        }
    }
}