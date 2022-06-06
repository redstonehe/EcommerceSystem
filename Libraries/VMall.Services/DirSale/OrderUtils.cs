using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VMall.Core;
using VMall.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;
namespace VMall.Services
{
    public class OrderUtils
    {
        private static string dirSaleApiUrl = WebConfigurationManager.AppSettings["DirsaleApiUrl"];
        public static APIClient client = new APIClient(WebHelper.GetConfigSettings("DirsaleApiUrl"), "666888", "89365A9BA9AE4788A69FCF10A82BD8AB");

        /// <summary>
        /// 判断直销会员支付密码是否正确
        /// </summary>
        /// <param name="inputPswd"></param>
        /// <returns></returns>
        public static string GetPayPassword(string inputPswd, int dirSaleUid)
        {
            try
            {
                //http://www.xxx.com/api/User/GetUserPayPwd?userId=xxx
                APIDictionary Params = new APIDictionary();
                Params.Add("userId", dirSaleUid);
                string FromDirSale = client.Execute(Params, "/api/User/GetUserPayPwd");

                //string url = dirSaleApiUrl + "/api/User/GetUserPayPwd?userId=" + dirSaleUid;

                //string FromDirSale = WebHelper.DoGet(url);
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    return token["Info"].ToString();

                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("ERRORGetPayPswd", "判断直销会员支付密码是否正确", "错误信息为：直销会员ID：" + dirSaleUid + "|失败原因：" + ex.Message);
                return string.Empty;
            }
        }

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
                string password = System.Web.HttpUtility.UrlEncode(userInfo.DirSalePwd);// System.Web.HttpUtility.UrlEncode(SecureHelper.EncryptString(getCookiePassword(), DirSaleUserInfo.EncryptKey));
                string orderUrl = string.Format("{0}/api/product/CreateUserOrder?userCode={1}&password={2}&consignee={3}&phone={4}&orderCode={5}&provice={6}&city={7}&area={8}&address={9}&postCode={10}&proData={11}&userId={12}&orderType={13}", dirSaleApiUrl, userCode, password, orderInfo.Consignee, orderInfo.Mobile, orderInfo.OSN, regionInfo.ProvinceName, regionInfo.CityName, regionInfo.Name, orderInfo.Address, orderInfo.ZipCode, getOrderProductStr(Orders.GetOrderProductList(orderInfo.Oid)), userInfo.IsDirSaleUser ? userInfo.DirSaleUid : userInfo.Uid, userInfo.IsDirSaleUser ? (int)OrderSource.直销后台 : (int)OrderSource.自营商城);
                string FromDirSale2 = WebHelper.DoGet(orderUrl);
                JObject jsonObject2 = (JObject)JsonConvert.DeserializeObject(FromDirSale2);
                JToken token2 = (JToken)jsonObject2;

                //传递成功后需要在汇购中标识已经传递成功的状态
                if (token2["Result"].ToString() == "0")
                {
                    return true;
                }
                else
                {
                    LogHelper.WriteOperateLog("FailSendOutOrder", "发送外部订单失败（直销系统）", "错误信息为：会员ID：" + userInfo.Uid + "|订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|失败原因：" + token2["Msg"].ToString());
                    return false;

                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("ERRORSendOutOrder", "发送外部订单异常（直销系统）", "错误信息为：会员ID：" + userInfo.Uid + "|订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|失败原因：" + ex.Message);
                return false;
            }

        }

        /// <summary>
        /// 创建分销会员资格
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool CreateFXMember(OrderInfo orderInfo, PartUserInfo userInfo)
        {

            LogHelper.WriteOperateLog("CreateActivateMemberlog", "创建分销会员api接口记录", "===========开始进入============");
            string userCode = string.IsNullOrEmpty(userInfo.UserName) ? (string.IsNullOrEmpty(userInfo.Email) ? (string.IsNullOrEmpty(userInfo.Mobile) ? "" : userInfo.Mobile) : userInfo.Email) : userInfo.UserName; //userCode：登录名

            string userName = userCode;//userName：用户名

            string nickName = string.IsNullOrEmpty(userInfo.NickName) ? userCode : userInfo.NickName;//nickName：昵称

            string password = System.Web.HttpUtility.UrlEncode(userInfo.DirSalePwd);// System.Web.HttpUtility.UrlEncode(SecureHelper.EncryptString(getSessionPassword(userInfo.Uid), DirSaleUserInfo.EncryptKey));//加密后的登录密码  SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);
            LogHelper.WriteOperateLog("CreateActivateMemberlog", "创建分销会员api接口记录", "=====密码====" + password + "=====");
            string pcode = GetParentCode(userInfo.Uid);
            if (string.IsNullOrEmpty(pcode))
                return false;
            string parentCode = pcode;//parentCode：推荐人编号

            //string userPhone = WebHelper.GetFormString("userPhone"); ;//userPhone：电话
            //string provice = "";//    provice：省
            //string city = "";//city：市
            //string area = "";//：区或者县
            //string address = "";//address:具体地址
            //if (orderInfo != null)
            //{
            //    RegionInfo reinfo = Regions.GetRegionById(orderInfo.RegionId);//区域信息
            //    provice = reinfo.ProvinceName;
            //    city = reinfo.CityName;
            //    area = reinfo.Name;
            //    address = orderInfo.Address;
            //}

            string hongbao = string.Empty; ;
            string haimi = string.Empty;
            string[] account = GetAccountDetail(userInfo.Uid);
            hongbao = account[0];
            haimi = account[1];
            try
            {
                LogHelper.WriteOperateLog("CreateActivateMemberlog", "创建分销会员api接口记录", "===========开始接口调用============");
                //http://www.xxx.com/api/User/CreateActivateMember?userCode=xxx&userName=xxx&password=xxx&parentCode=xxx&userCard=xxx&userPhone=xxx&provice=xxx&city=xxx&area=xxx&addres s=xxx&hongbao=xxx&haimi=xxx
                string url = string.Format("{0}/api/User/CreateActivateMember?userCode={1}&userName={2}&password={3}&parentCode={4}&userCard={5}&userPhone={6}&provice={7}&city={8}&area={9}&address={10}&hongbao={11}&haimi={12}", dirSaleApiUrl, userCode, userName, password, parentCode, "", "", "", "", "", "", hongbao, haimi);

                string FromDirSale = WebHelper.DoGet(url);
                LogHelper.WriteOperateLog("CreateActivateMemberlog", "创建分销会员api接口记录", "接口调用返回：" + FromDirSale);
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    //{"Result":0,"Msg":"用户注册成功","Info":{"ParentCode":"company","ManagerCode":"zhangjunsheng10"}}
                    string resultParentCode = token["Info"].SelectToken("ParentCode").ToString();
                    string resultManagerCode = token["Info"].SelectToken("ManagerCode").ToString();
                    int resultUserId = TypeHelper.ObjectToInt(token["Info"].SelectToken("UserId"));//新增返回userId
                    LogHelper.WriteOperateLog("CreateActivateMemberlog", "创建分销会员api接口记录", "接口调用成功，直销uid:" + resultUserId);
                    return UpdateFXUserOutSates(resultUserId, userInfo.Uid, 1) > 0;
                }
                else
                {
                    LogHelper.WriteOperateLog("CreateActivateMemberError", "创建分销会员api接口错误", "错误信息为：会员ID：" + userInfo.Uid + "|会员名称" + userCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("CreateActivateMemberError", "创建分销会员api接口错误", "错误信息为：会员ID：" + userInfo.Uid + "|会员名称" + userCode + "错误信息" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 当用户取消订单或者退货时，将直销网中的点位废弃
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool DiscartFXUser(PartUserInfo userInfo)
        {
            LogHelper.WriteOperateLog("DiscartFXUserlog", "废弃分销会员api接口记录", "===========开始进入============");
            string userCode = string.IsNullOrEmpty(userInfo.UserName) ? (string.IsNullOrEmpty(userInfo.Email) ? (string.IsNullOrEmpty(userInfo.Mobile) ? "" : userInfo.Mobile) : userInfo.Email) : userInfo.UserName;
            if (string.IsNullOrEmpty(userCode))
                return false;
            //http://www.xxx.com/api/User/DiscardUser?userCode=xxx
            string url = dirSaleApiUrl + "/api/User/DiscardUser?userCode=" + userCode;

            string FromDirSale = WebHelper.DoGet(url);
            LogHelper.WriteOperateLog("DiscartFXUserlog", "废弃分销会员api接口记录", "接口调用返回：" + FromDirSale);
            JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            JToken token = (JToken)jsonObject;
            if (token["Result"].ToString() == "0")
            {
                UpdateFXUserOutSates(userInfo.DirSaleUid, userInfo.Uid, 2);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 对于废弃的点位，调用该接口，重新激活
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool ActivateHuigouUser(PartUserInfo userInfo)
        {
            LogHelper.WriteOperateLog("ActivateHuigouUserlog", "重新激活分销会员api接口记录", "===========开始进入============");
            string userCode = string.IsNullOrEmpty(userInfo.UserName) ? (string.IsNullOrEmpty(userInfo.Email) ? (string.IsNullOrEmpty(userInfo.Mobile) ? "" : userInfo.Mobile) : userInfo.Email) : userInfo.UserName;
            if (string.IsNullOrEmpty(userCode))
                return false;
            //http://www.xxx.com/api/User/ActivateHuigouUser?userCode=xxx
            string url = dirSaleApiUrl + "/api/User/ActivateHuigouUser?userCode=" + userCode;

            string FromDirSale = WebHelper.DoGet(url);
            LogHelper.WriteOperateLog("ActivateHuigouUserlog", "重新激活分销会员api接口记录", "接口调用返回：" + FromDirSale);
            JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            JToken token = (JToken)jsonObject;
            if (token["Result"].ToString() == "0")
            {
                UpdateFXUserOutSates(userInfo.DirSaleUid, userInfo.Uid, 1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将list集合转换为json格式
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static string getOrderProductStr(List<OrderProductInfo> orders)
        {
            //[{"ProSN":"HZ50203001","ProCount":2},{"ProSN":"H10102004c","ProCount":1}]
            //ProSN表示产品编号，ProCount表示产品数量
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < orders.Count; i++)
            {
                sb.Append("{\"ProSN\":\"" + orders[i].PSN.ToString() + "\",\"ProCount\":\"" + orders[i].BuyCount.ToString() + "\"}");
                if (i < orders.Count - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// 获取cookie中的密码
        /// </summary>
        /// <returns></returns>
        public static string getTmpPassword()
        {
            LogHelper.WriteOperateLog("Cookielog", "创建分销会员api接口获取密码记录", "=========开始=======");
            string entryStr = MallUtils.GetBMACookie("random");
            // string entryStr = Session[partUserInfo.Uid + "_enpw"];
            LogHelper.WriteOperateLog("Cookielog", "创建分销会员api接口获取密码记录", "获取到的密钥：===|" + entryStr + "|====uusercookie" + MallUtils.GetBMACookie("uid") + "|域数" + HttpContext.Current.Request.Cookies.Count);
            string decryptpwd = MallUtils.AESDecrypt(entryStr);

            string pwsStr = decryptpwd.Substring(18, (decryptpwd.Length - 38));
            LogHelper.WriteOperateLog("Cookielog", "创建分销会员api接口获取密码记录", "cookie密码：" + entryStr + "|解密后密钥：" + decryptpwd + "|密钥长度：" + decryptpwd.Length + "|psw:==" + pwsStr + "==");
            return pwsStr;

        }
        /// <summary>
        /// 获取session中的密码
        /// </summary>
        /// <returns></returns>
        public static string getSessionPassword(int uid)
        {
            LogHelper.WriteOperateLog("Sessionlog", "创建分销会员api接口获取密码记录", "=========开始=======");
            //string entryStr = MallUtils.GetBMACookie("random");
            string entryStr = (string)System.Web.HttpContext.Current.Session[uid + "_enpw"];
            LogHelper.WriteOperateLog("Sessionlog", "创建分销会员api接口获取密码记录", "获取到的密钥：===|" + entryStr);
            string decryptpwd = MallUtils.AESDecrypt(entryStr);

            string pwsStr = decryptpwd.Substring(18, (decryptpwd.Length - 38));
            LogHelper.WriteOperateLog("Sessionlog", "创建分销会员api接口获取密码记录", "cookie密码：" + entryStr + "|解密后密钥：" + decryptpwd + "|密钥长度：" + decryptpwd.Length + "|psw:==" + pwsStr + "==");
            return pwsStr;

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
                        //http://192.168.88.99:8088/api/User/GetUserCode?userId=235

                        APIDictionary Params = new APIDictionary();
                        Params.Add("userId", partUserInfo.Pid);
                        string FromDirSale = client.Execute(Params, "/api/User/GetUserCode");

                        //string url = string.Format("{0}/api/User/GetUserCode?userId={1}", dirSaleApiUrl, partUserInfo.Pid);

                        //string FromDirSale = WebHelper.DoGet(url);
                        JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                        JToken token = (JToken)jsonObject;
                        if (token["Result"].ToString() == "0")
                        {
                            pCode = token["Info"].ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteOperateLog("GetUserCode", "请求api接口错误", "错误信息为：会员ID：" + uid + "推荐人id" + partUserInfo.Pid + "错误信息" + ex.Message);
                    }
                }
                else if (partUserInfo.Ptype == 1 && partUserInfo.Pid > 0)
                {
                    PartUserInfo parentUserInfo = Users.GetPartUserById(partUserInfo.Pid);
                    if (parentUserInfo != null)
                    {
                        try
                        {
                            APIDictionary Params = new APIDictionary();
                            Params.Add("userId", partUserInfo.DirSaleUid);
                            string FromDirSale = client.Execute(Params, "/api/User/GetUserCode");

                            //string url = string.Format("{0}/api/User/GetUserCode?userId={1}", dirSaleApiUrl, partUserInfo.DirSaleUid);

                            //string FromDirSale = WebHelper.DoGet(url);
                            JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                            JToken token = (JToken)jsonObject;
                            if (token["Result"].ToString() == "0")
                            {
                                pCode = token["Info"].ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteOperateLog("GetUserCode", "请求api接口错误", "错误信息为：会员ID：" + uid + "推荐人id" + partUserInfo.Pid + "错误信息" + ex.Message);
                        }
                    }
                }
            }
            return pCode;
        }

        /// <summary>
        /// 获取红包和海米账户余额
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
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
        public static int UpdateUserOutSates(int dirSaleUid, int uid, int ptype, PartUserInfo parentUser)
        {

            SqlParameter[] parameters = new SqlParameter[] { 

                new SqlParameter("@uid", uid),
                new SqlParameter("@dirsaleuid", dirSaleUid),
                new SqlParameter("@pid", parentUser.DirSaleUid)
                
            };
            string SqlStr = string.Empty;
            if (ptype == 1)
                SqlStr = "UPDATE dbo.hlh_users SET isdirsaleuser=1,dirsaleuid=@dirsaleuid,isnewds=1 WHERE uid=@uid";
            else if (ptype == 2)
                SqlStr = "UPDATE dbo.hlh_users SET isdirsaleuser=1,dirsaleuid=@dirsaleuid,isnewds=1 WHERE uid=@uid";
            else if (ptype == 3)
                SqlStr = "UPDATE dbo.hlh_users SET isdirsaleuser=0,dirsaleuid=0,agenttype=0,isnewds=0 WHERE uid=@uid";
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, SqlStr, parameters));
        }

        /// <summary>
        /// 更新改账号存在于直销后台的数据库字段标识,更新用户外部状态并保存直销会员id
        /// </summary>
        /// <param name="dirSaleUid"></param>
        /// <param name="uid"></param>
        /// <param name="userType">类型，1为创建 2为废弃</param>
        /// <returns></returns>
        public static int UpdateFXUserOutSates(int dirSaleUid, int uid, int userType)
        {

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@uid", uid), 
                                                             new SqlParameter("@dirsaleuid", dirSaleUid),
                                                             new SqlParameter("@isfxuser", userType)
                                                            };
            string SqlStr = "UPDATE dbo.hlh_users SET isfxuser=@isfxuser,dirsaleuid=@dirsaleuid WHERE uid=@uid";

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, SqlStr, parameters));
        }
        /// <summary>
        /// 更新改账号拥有三级分销资格
        /// </summary>
        /// <param name="dirSaleUid"></param>
        /// <param name="uid"></param>
        /// <param name="userType">类型，0为普通会员 ，1为普通分销 2为高级分销 </param>
        /// <returns></returns>
        public static int UpdateFXUserSates(int uid, int userType)
        {

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@uid", uid), 
                                                             new SqlParameter("@isfxuser", userType)
                                                            };
            string SqlStr = "UPDATE dbo.hlh_users SET isfxuser=@isfxuser WHERE uid=@uid";

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, SqlStr, parameters));
        }

        /// <summary>
        /// 添加已支付订单到直销系统中
        /// 直销会员支付成功后的有效订单推送到直销系统中
        /// 分为确认订单后自动推送和选择分期后推送
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public static bool CreateQDOrder(OrderInfo orderInfo, PartUserInfo userInfo, int PhasedType)
        {
            orderInfo = Orders.GetOrderByOid(orderInfo.Oid);
            try
            {
                //isPhased，如果直销会员是新会员，传3，如果直销会员是老会员且没有选择分期，传1，如果直销会员是老会员且选择分期，传2
                int isPhased = 0;
                string[] details = AccountUtils.GetUserDetailsForOrder(userInfo.DirSaleUid);
                if (details.Length < 1)
                    return false;
                if (details[1] == "1")
                {
                    isPhased = 3;
                }
                else
                {
                    isPhased = PhasedType;
                }
                int orderType = 1;

                RegionInfo regionInfo = Regions.GetRegionById(orderInfo.RegionId);
                string userCode = details[0]; //string.IsNullOrEmpty(userInfo.UserName) ? (string.IsNullOrEmpty(userInfo.Email) ? (string.IsNullOrEmpty(userInfo.Mobile) ? "" : userInfo.Mobile) : userInfo.Email) : userInfo.UserName; //userCode：登录名


                List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
                string proData = getOrderProductStr(orderProductList);

                decimal orderPV = orderInfo.CashDiscount > 0 ? 0 : orderProductList.Sum(x => x.BuyCount * x.ProductPV);

                APIDictionary Params = new APIDictionary();
                Params.Add("userCode", userCode);
                Params.Add("userId", userInfo.DirSaleUid);

                Params.Add("station", "");
                Params.Add("orderType", orderType);
                Params.Add("isPhased", isPhased);
                Params.Add("payTime", orderInfo.PayTime);
                Params.Add("payUser", userCode);
                Params.Add("payUserId", userInfo.DirSaleUid);
                Params.Add("amount", orderInfo.OrderAmount);
                Params.Add("score", orderPV);
                Params.Add("cash", orderInfo.SurplusMoney);
                Params.Add("emoney", orderInfo.OrderAmount - orderInfo.SurplusMoney);
                Params.Add("province", regionInfo.ProvinceName);
                Params.Add("city", regionInfo.CityName);
                Params.Add("area", regionInfo.Name);
                Params.Add("address", orderInfo.Address);
                Params.Add("postCode", orderInfo.ZipCode);
                Params.Add("consignee", orderInfo.Consignee);
                Params.Add("phone", orderInfo.Mobile);
                Params.Add("proData", proData);
                Params.Add("orderCode", orderInfo.OSN);
                string FromDirSale = client.Execute(Params, "/api/order/createOrder");
                JObject jsonObject2 = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token2 = (JToken)jsonObject2;

                //传递成功后需要在汇购中标识已经传递成功的状态
                if (token2["Result"].ToString() == "0")
                {
                    Orders.SetSendSusscess(orderInfo.Oid);
                    LogHelper.WriteOperateLog("SuccessCreateQDOrder", "添加已支付订单（直销系统）", "成功信息为：返回订单ID：" + token2["Info"].SelectToken("OrderId").ToString() + "|订单号：" + token2["Info"].SelectToken("OrderNo").ToString() + ",汇购订单ID：" + orderInfo.Oid + ",会员名：" + userCode);

                    return true;
                }
                else
                {
                    LogHelper.WriteOperateLog("FailCreateQDOrder", "添加已支付订单（直销系统）", "错误信息为：会员ID：" + userInfo.Uid + "|订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|失败原因：" + token2["Msg"].ToString() + ",会员名：" + userCode);
                    return false;

                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("FailCreateQDOrder", "添加已支付订单（直销系统）", "错误信息为：会员ID：" + userInfo.Uid + "|订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|失败原因,请求异常：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 取消已支付订单
        /// 直销会员取消订单,直销会员退货
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public static bool CancelQDOrder(OrderInfo orderInfo, PartUserInfo userInfo, string remark)
        {
            try
            {
                if (!userInfo.IsDirSaleUser)

                    return false;

                string userCode = string.IsNullOrEmpty(userInfo.UserName) ? (string.IsNullOrEmpty(userInfo.Email) ? (string.IsNullOrEmpty(userInfo.Mobile) ? "" : userInfo.Mobile) : userInfo.Email) : userInfo.UserName; //userCode：登录名
                APIDictionary Params = new APIDictionary();
                Params.Add("orderCode", orderInfo.OSN);
                Params.Add("cancelCode", userCode);
                Params.Add("remark", remark);
                string FromDirSale = client.Execute(Params, "/api/order/cancelOrder");

                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;

                LogHelper.WriteOperateLog("SuccessCancelQDOrder", "取消已支付订单（直销系统）", "信息为：会员ID：" + userInfo.Uid + "|订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|接口返回：" + FromDirSale);
                if (token["Result"].ToString() == "0")
                {
                   
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("FailCancelQDOrder", "取消已支付订单（直销系统）", "错误信息为：会员ID：" + userInfo.Uid + "|订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|失败原因,请求异常：" + ex.Message);
                return false;
            }
        }

    }
}
