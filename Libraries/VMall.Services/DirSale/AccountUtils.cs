using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VMall.Core;
using VMall.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using VMall.Services.DirSale;
using System.Web.Script.Serialization;

namespace VMall.Services
{
    public class AccountUtils
    {
        private static object ctx = new object();//锁对象

        private static string dirSaleApiUrl = WebConfigurationManager.AppSettings["DirsaleApiUrl"];
        public static APIClient client = new APIClient(WebHelper.GetConfigSettings("DirsaleApiUrl"), "666888", "89365A9BA9AE4788A69FCF10A82BD8AB");

        /// <summary>
        /// 注册时判断用户名是否存在于直销系统中
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public static bool IsUserExistForDirSale(string accountName)
        {
            try
            {
                APIDictionary Params = new APIDictionary();
                Params.Add("userName", accountName);
                string FromDirSale = client.Execute(Params, "/api/User/IsUserExist");

                //string url = dirSaleApiUrl + "/api/User/IsUserExist?userName=" + accountName;
                //string FromDirSale = WebHelper.DoGet(url);
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public static string UserLogin(string accountName, string password)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("userName", accountName);
            Params.Add("password", password);
            string FromDirSale = client.Execute(Params, "/api/User/UserLogin");

            //string url = dirSaleApiUrl + "/api/User/UserLogin?userName=" + accountName + "&password=" + password;
            //string FromDirSale = WebHelper.DoGet(url);

            return FromDirSale;

        }

        /// <summary>
        /// 获取直销会员姓名
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static string GetUserNameByCodeForDir(string pName)
        {
            try
            {
                if (string.IsNullOrEmpty(pName))
                    return "";
                string showname = "";
                APIDictionary Params = new APIDictionary();
                Params.Add("userCode", pName);
                string FromDirSale = client.Execute(Params, "/api/User/GetUserNameByCode");

                //string url = dirSaleApiUrl + "/api/User/GetUserNameByCode?userCode=" + pName;
                //string FromDirSale = WebHelper.DoGet(url);
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    showname = token["Info"].ToString();
                }
                return showname;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// 根据会员 ID 返回会员编号
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static string GetUserCode(int userId)
        {
            try
            {
                string UserCode = "";
                APIDictionary Params = new APIDictionary();
                Params.Add("userId", userId);
                string FromDirSale = client.Execute(Params, "/api/User/GetUserCode");

                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    UserCode = token["Info"].ToString();
                }
                return UserCode;
            }
            catch (Exception ex)
            {
                return "";
            }
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
                APIDictionary Params = new APIDictionary();
                Params.Add("userName", pName);
                string FromDirSale = client.Execute(Params, "/api/User/GetUserIdByName");

                //string url = dirSaleApiUrl + "/api/User/GetUserIdByName?userName=" + pName;
                //string FromDirSale = WebHelper.DoGet(url);
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
        /// 根据会员ID获取用户信息
        /// </summary>
        /// <returns></returns>
        public static DSDetailInfo GetUserDetailMsg(int dirUid)
        {
            try
            {
                APIDictionary Params = new APIDictionary();
                Params.Add("userId", dirUid);
                string FromDirSale = client.Execute(Params, "/api/User/GetUserDetails");
                DSDetailInfo details = new DSDetailInfo();
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    details.UserId = TypeHelper.ObjectToInt(token["Info"].SelectToken("UserId"));
                    details.UserCode = token["Info"].SelectToken("UserCode").ToString();
                    details.UserName = token["Info"].SelectToken("UserName").ToString();
                    details.NickName = token["Info"].SelectToken("NickName").ToString();
                    details.UserCard = token["Info"].SelectToken("UserCard").ToString();
                    details.UserPhone = token["Info"].SelectToken("UserPhone").ToString();
                    details.Rank = TypeHelper.ObjectToInt(token["Info"].SelectToken("Rank"));
                    details.RankName = token["Info"].SelectToken("RankName").ToString();
                    details.ParentCode = token["Info"].SelectToken("ParentCode").ToString();
                    details.ManagerCode = token["Info"].SelectToken("ManagerCode").ToString();
                    details.RegisterDate = TypeHelper.ObjectToDateTime(token["Info"].SelectToken("RegisterDate"));
                }
                return details;
            }
            catch (Exception ex)
            {
                return new DSDetailInfo();
            }
        }
        /// <summary>
        /// 根据会员ID获取用户会员编号、是否新用户及用户级别
        /// </summary>
        /// <returns></returns>
        public static string[] GetUserDetailsForOrder(int dirUid)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("userId", dirUid);
            string FromDirSale = client.Execute(Params, "/api/User/GetUserDetails");

            string[] details = new string[3];
            JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            JToken token = (JToken)jsonObject;
            if (token["Result"].ToString() == "0")
            {
                string UserCode = token["Info"].SelectToken("UserCode").ToString();
                string IsNew = token["Info"].SelectToken("IsNew").ToString();
                string rank = token["Info"].SelectToken("Rank").ToString();
                details[0] = UserCode;
                details[1] = IsNew.ToString();
                details[2] = rank;
            }
            return details;
        }
        /// <summary>
        /// 根据会员 ID 获取用户手机
        /// </summary>
        /// <returns></returns>
        public static string GetUserMobile(int dirUid)
        {
            try
            {
                APIDictionary Params = new APIDictionary();
                Params.Add("userId", dirUid);
                string FromDirSale = client.Execute(Params, "/api/User/GetUserDetails");

                string UserMobile = string.Empty;
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    UserMobile = token["Info"].SelectToken("UserPhone").ToString();
                }
                return UserMobile;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #region 账户操作
        /// <summary>
        /// 根据会员 ID 获取用户直销级别
        /// </summary>
        /// <returns></returns>
        public static string GetUserRank(int dirUid)
        {
            try
            {
                APIDictionary Params = new APIDictionary();
                Params.Add("userId", dirUid);
                string FromDirSale = client.Execute(Params, "/api/User/GetUserDetails");

                string UserRank = string.Empty;
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    UserRank = token["Info"].SelectToken("Rank").ToString();
                }
                return UserRank;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// 获取账户列表
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="isDirUser"></param>
        /// <param name="dirUid"></param>
        /// <returns></returns>
        public static List<AccountInfo> GetAccountList(int uid, bool isDirUser, int dirUid)
        {
            List<AccountInfo> accountInfoList = new List<AccountInfo>();
            if (!isDirUser)
            {//海汇会员获取余额信息
                accountInfoList = Account.GetAccountInfoListByUid(uid);
            }
            else
            {//直销会员获取余额信息
                try
                {
                    APIDictionary Params = new APIDictionary();
                    Params.Add("userId", dirUid);
                    string FromDirSale = client.Execute(Params, "/api/User/GetUserDetails");

                    //string url = dirSaleApiUrl + "/api/User/GetUserDetails?userId=" + dirUid;

                    //string FromDirSale = WebHelper.DoGet(url);

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
                            info.LockBanlance = Convert.ToDecimal(item.SelectToken("LockBalance"));
                            accountInfoList.Add(info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteOperateLog("GetAccountListError", "获取直销账户列表api接口记录", "接口调用返回错误：" + ex.Message + "|DirUid：" + dirUid + "|uid：" + uid);
                }
                //直销会员的代理账户佣金账户保存在汇购系统中，两部分帐号需要拼接起来
                //List<int> agentaccountids = new List<int>();
                //agentaccountids.Add((int)AccountType.代理账户);
                //agentaccountids.Add((int)AccountType.佣金账户);
                //List<AccountInfo> SecondaccountInfoList = Account.GetAccountInfoListByUid(uid).FindAll(x => agentaccountids.Exists(p => p == x.AccountId));
                //accountInfoList.AddRange(SecondaccountInfoList);
            }
            return accountInfoList;
        }

        /// <summary>
        /// 活动赠送海米
        /// </summary>
        /// <param name="partUserInfo"></param>
        /// <param name="orderInfo"></param>
        /// <param name="operatorId"></param>
        /// <param name="returnTime"></param>
        public static void SendActiveHaimi(PartUserInfo partUserInfo, OrderInfo orderInfo, decimal amount, int operatorId, DateTime returnTime)
        {

            if (partUserInfo.IsDirSaleUser)
                UpdateAccountForDir(partUserInfo.DirSaleUid, (int)AccountType.海米账户, amount, 0, orderInfo.OSN, "活动赠送海米：订单号：" + orderInfo.OSN + ",赠送金额：" + amount);
            else
            {
                Account.UpdateAccountForIn(new AccountInfo() { UserId = orderInfo.Uid, AccountId = (int)AccountType.海米账户, TotalIn = amount });
                Account.CreateAccountDetail(new AccountDetailInfo()
                {
                    AccountId = (int)AccountType.海米账户,
                    UserId = orderInfo.Uid,
                    CreateTime = DateTime.Now,
                    DetailType = (int)DetailType.活动赠送,
                    InAmount = amount,
                    OutAmount = 0,
                    OrderCode = orderInfo.OSN,
                    AdminUid = operatorId,
                    Status = 1,
                    DetailDes = "活动赠送海米：订单号：" + orderInfo.OSN + ",赠送金额：" + amount
                });
            }

        }
        /// <summary>
        ///活动 赠送红包
        /// </summary>
        /// <param name="partUserInfo"></param>
        /// <param name="orderInfo"></param>
        /// <param name="operatorId"></param>
        /// <param name="returnTime"></param>
        public static void SendActiveHongBao(PartUserInfo partUserInfo, OrderInfo orderInfo, decimal amount, int operatorId, DateTime returnTime)
        {

            if (partUserInfo.IsDirSaleUser)
                UpdateAccountForDir(partUserInfo.DirSaleUid, (int)AccountType.红包账户, amount, 0, orderInfo.OSN, "活动赠送红包：订单号：" + orderInfo.OSN + ",赠送金额：" + amount);
            else
            {
                Account.UpdateAccountForIn(new AccountInfo() { UserId = orderInfo.Uid, AccountId = (int)AccountType.红包账户, TotalIn = amount });
                Account.CreateAccountDetail(new AccountDetailInfo()
                {
                    AccountId = (int)AccountType.红包账户,
                    UserId = orderInfo.Uid,
                    CreateTime = DateTime.Now,
                    DetailType = (int)DetailType.活动赠送,
                    InAmount = amount,
                    OutAmount = 0,
                    OrderCode = orderInfo.OSN,
                    AdminUid = operatorId,
                    Status = 1,
                    DetailDes = "活动赠送红包：订单号：" + orderInfo.OSN + ",赠送金额：" + amount
                });

            }
        }

        /// <summary>
        /// 活动赠送海米退回
        /// </summary>
        /// <param name="partUserInfo"></param>
        /// <param name="orderInfo"></param>
        /// <param name="operatorId"></param>
        /// <param name="returnTime"></param>
        public static void ReturnActiveHaimi(PartUserInfo partUserInfo, OrderInfo orderInfo, decimal amount, int operatorId, DateTime returnTime)
        {

            if (partUserInfo.IsDirSaleUser)
                UpdateAccountForDir(partUserInfo.DirSaleUid, (int)AccountType.海米账户, 0, amount, orderInfo.OSN, "活动赠送海米取消：订单号：" + orderInfo.OSN + ",取消金额：" + amount);
            else
            {
                Account.UpdateAccountForOut(new AccountInfo() { UserId = orderInfo.Uid, AccountId = (int)AccountType.海米账户, TotalOut = amount });
                Account.CreateAccountDetail(new AccountDetailInfo()
                {
                    AccountId = (int)AccountType.海米账户,
                    UserId = orderInfo.Uid,
                    CreateTime = DateTime.Now,
                    DetailType = (int)DetailType.活动赠送取消,
                    InAmount = 0,
                    OutAmount = amount,
                    OrderCode = orderInfo.OSN,
                    AdminUid = operatorId,
                    Status = 1,
                    DetailDes = "活动赠送海米取消：订单号：" + orderInfo.OSN + ",取消金额：" + amount
                });
            }

        }
        /// <summary>
        ///活动 赠送红包退回
        /// </summary>
        /// <param name="partUserInfo"></param>
        /// <param name="orderInfo"></param>
        /// <param name="operatorId"></param>
        /// <param name="returnTime"></param>
        public static void ReturnActiveHongBao(PartUserInfo partUserInfo, OrderInfo orderInfo, decimal amount, int operatorId, DateTime returnTime)
        {
            if (partUserInfo.IsDirSaleUser)
                UpdateAccountForDir(partUserInfo.DirSaleUid, (int)AccountType.红包账户, 0, amount, orderInfo.OSN, "活动赠送红包取消：订单号：" + orderInfo.OSN + ",取消金额：" + amount);
            else
            {
                Account.UpdateAccountForOut(new AccountInfo() { UserId = orderInfo.Uid, AccountId = (int)AccountType.红包账户, TotalOut = amount });
                Account.CreateAccountDetail(new AccountDetailInfo()
                {
                    AccountId = (int)AccountType.红包账户,
                    UserId = orderInfo.Uid,
                    CreateTime = DateTime.Now,
                    DetailType = (int)DetailType.活动赠送取消,
                    InAmount = 0,
                    OutAmount = amount,
                    OrderCode = orderInfo.OSN,
                    AdminUid = operatorId,
                    Status = 1,
                    DetailDes = "活动赠送取消红包：订单号：" + orderInfo.OSN + ",取消金额：" + amount
                });

            }
        }

        /// <summary>
        /// 返回海米
        /// </summary>
        /// <param name="partUserInfo"></param>
        /// <param name="orderInfo"></param>
        /// <param name="operatorId"></param>
        /// <param name="returnTime"></param>
        public static void ReturnUserHaimi(PartUserInfo partUserInfo, OrderInfo orderInfo, int operatorId, DateTime returnTime)
        {
            // int completeCount = Orders.GetUserOrderCount(WorkContext.Uid, "", "", (int)OrderState.Completed);
            if (partUserInfo.IsDirSaleUser)
                UpdateAccountForDir(partUserInfo.DirSaleUid, (int)AccountType.商城钱包, orderInfo.HaiMiDiscount, 0, orderInfo.OSN, "订单取消" + MallKey.MallDiscountName_JiangJin + "返回：订单号：" + orderInfo.OSN + ",返回金额：" + orderInfo.HaiMiDiscount);
            else
            {
                Account.UpdateAccountForIn(new AccountInfo() { UserId = orderInfo.Uid, AccountId = (int)AccountType.商城钱包, TotalIn = orderInfo.HaiMiDiscount });
                Account.CreateAccountDetail(new AccountDetailInfo()
                {
                    AccountId = (int)AccountType.商城钱包,
                    UserId = orderInfo.Uid,
                    CreateTime = DateTime.Now,
                    DetailType = (int)DetailType.订单取消返回,
                    InAmount = orderInfo.HaiMiDiscount,
                    OutAmount = 0,
                    OrderCode = orderInfo.OSN,
                    AdminUid = operatorId,
                    Status = 1,
                    DetailDes = "订单取消" + MallKey.MallDiscountName_JiangJin + "返回：订单号：" + orderInfo.OSN + ",返回金额：" + orderInfo.HaiMiDiscount
                });
            }

        }
        /// <summary>
        /// 返回红包
        /// </summary>
        /// <param name="partUserInfo"></param>
        /// <param name="orderInfo"></param>
        /// <param name="operatorId"></param>
        /// <param name="returnTime"></param>
        public static void ReturnUserHongBao(PartUserInfo partUserInfo, OrderInfo orderInfo, int operatorId, DateTime returnTime)
        {
            // int completeCount = Orders.GetUserOrderCount(WorkContext.Uid, "", "", (int)OrderState.Completed);
            if (partUserInfo.IsDirSaleUser)
                UpdateAccountForDir(partUserInfo.DirSaleUid, (int)AccountType.积分账户, orderInfo.HongBaoDiscount, 0, orderInfo.OSN, "订单取消" + MallKey.MallDiscountName_JiFen + "返回：订单号：" + orderInfo.OSN + ",返回金额：" + orderInfo.HongBaoDiscount);
            else
            {
                Account.UpdateAccountForIn(new AccountInfo() { UserId = orderInfo.Uid, AccountId = (int)AccountType.积分账户, TotalIn = orderInfo.HongBaoDiscount });
                Account.CreateAccountDetail(new AccountDetailInfo()
                {
                    AccountId = (int)AccountType.积分账户,
                    UserId = orderInfo.Uid,
                    CreateTime = DateTime.Now,
                    DetailType = (int)DetailType.订单取消返回,
                    InAmount = orderInfo.HongBaoDiscount,
                    OutAmount = 0,
                    OrderCode = orderInfo.OSN,
                    AdminUid = operatorId,
                    Status = 1,
                    DetailDes = "订单取消" + MallKey.MallDiscountName_JiFen + "返回：订单号：" + orderInfo.OSN + ",返回金额：" + orderInfo.HongBaoDiscount
                });

            }
        }

        /// <summary>
        /// 更新账户信息
        /// </summary>
        /// <param name="dirUid"></param>
        /// <param name="accountId"></param>
        /// <param name="inAmount"></param>
        /// <param name="outAmount"></param>
        /// <param name="orderCode"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static bool UpdateAccountForDir(int dirUid, int accountId, decimal inAmount, decimal outAmount, string orderCode, string remark)
        {
            //http://www.xxx.com/api/User/UpdateAccount?userId=xxx&accountId=x&inAmount=xxx&outAmount=xxx&orderCode=xxx&remark=xxx

            APIDictionary Params = new APIDictionary();
            Params.Add("userId", dirUid);
            Params.Add("accountId", accountId);
            Params.Add("inAmount", inAmount);
            Params.Add("outAmount", outAmount);
            Params.Add("cashflowType", 81);
            Params.Add("orderCode", orderCode);
            Params.Add("remark", remark);
            string FromDirSale = client.Execute(Params, "/api/User/UpdateAccount");

            //string url = dirSaleApiUrl + "/api/User/UpdateAccount?userId=" + dirUid + "&accountId=" + accountId + "&inAmount=" + inAmount + "&outAmount=" + outAmount + "&orderCode=" + orderCode + "&remark=" + remark;
            //string FromDirSale = WebHelper.DoGet(url);
            try
            {
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    return true;
                }
            }
            catch (Exception ex) { LogHelper.WriteOperateLog("UpdateAccountForDirError", "更新直销账户信息api接口记录", "接口调用返回：" + FromDirSale + "错误信息：" + ex.Message + "|更新DirUid:" + dirUid + "|更新信息：" + remark + "| url:"); }
            return false;
        }
        /// <summary>
        /// 获得账户收支详情
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="accountId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<AccountDetailInfo> GetDetail(int userId, int accountId, int pageIndex, int pageSize, ref int totalCount)
        {
            //  http://www.xxx.com/api/User/GetUserCashFlow?userId=xxx&accountId=x&pageIndex=x&pageSize=xx
            //accountId：账户对应的Id，如海米账户为10,预结算海米账户为11
            List<AccountDetailInfo> detailInfoList = new List<AccountDetailInfo>(); ;
            APIDictionary Params = new APIDictionary();
            Params.Add("userId", userId);
            Params.Add("accountId", accountId);
            Params.Add("pageIndex", pageIndex);
            Params.Add("pageSize", pageSize);
            string FromDirSale = client.Execute(Params, "/api/User/GetUserCashFlow");

            //string url = dirSaleApiUrl + "/api/User/GetUserCashFlow?userId=" + userId + "&accountId=" + accountId + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize;
            //string FromDirSale = WebHelper.DoGet(url);

            //JavaScriptSerializer js = new JavaScriptSerializer();
            //ResultModelUtils result = js.Deserialize<ResultModelUtils>(FromDirSale);
            try
            {
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;

                if (token["Result"].ToString() == "0")
                {
                    JToken counts = token["Info"].SelectToken("TotalRowCount");
                    totalCount = TypeHelper.ObjectToInt(counts);
                    JToken details = token["Info"].SelectToken("CashFlowList");
                    foreach (JObject item in details)
                    {
                        AccountDetailInfo detail = new AccountDetailInfo();
                        detail.CreateTime = TypeHelper.ObjectToDateTime(item.SelectToken("InsertDt"));
                        detail.InAmount = TypeHelper.ObjectToDecimal(item.SelectToken("InAmout"));
                        detail.OutAmount = TypeHelper.ObjectToDecimal(item.SelectToken("OutAmount"));
                        detail.CurBanlance = TypeHelper.ObjectToDecimal(item.SelectToken("CurBanlance"));
                        detail.DetailDes = item.SelectToken("Remark").ToString();
                        detailInfoList.Add(detail);
                    }
                }
            }
            catch (Exception ex) { LogHelper.WriteOperateLog("getDetailError", "获取直销账户详情记录", "接口调用返回错误：" + ex.Message + "|userId:" + userId + "|accountId:" + accountId); }
            return detailInfoList;
        }

        #endregion

        /// <summary>
        /// 获得会员在直销系统中的重消PV数
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public static decimal GetUserReorderPV(int DSUid, string curDate)
        {
            try
            {
                //http://www.xxx.com/api/User/GetUserReorderPV?userId=xxx&curDate=xxx

                APIDictionary Params = new APIDictionary();
                Params.Add("userId", DSUid);
                Params.Add("curDate", curDate);
                string FromDirSale = client.Execute(Params, "/api/User/GetUserReorderPV");
                //string url = dirSaleApiUrl + "/api/User/GetUserReorderPV?userId=" + DSUid + "&curDate=" + curDate;

                //string FromDirSale = WebHelper.DoGet(url);
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                decimal recorderPV = 0M;
                if (token["Result"].ToString() == "0")
                {
                    recorderPV = TypeHelper.ObjectToDecimal(token["Info"]);

                }
                return recorderPV;
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("GetUserReorderPVError", "获得会员在直销系统中的重消PV数", "接口调用返回错误：" + ex.Message + "|DirUid：" + DSUid + "|curDate：" + curDate);
                return 0M;
            }
        }


        /// <summary>
        /// 直销会员申请体验产品接口
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public static string applyTryFree(string pname, string name, string mobile, string provice, string city, string area, string address)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("pname", pname);
            Params.Add("name", name);
            Params.Add("mobile", mobile);
            Params.Add("provice", provice);
            Params.Add("city", city);
            Params.Add("area", area);
            Params.Add("address", pname);
            string FromDirSale = client.Execute(Params, "/api/product/applyTryFree");

            return FromDirSale;
        }


        /// <summary>
        /// 直销会员海米三级结算接口
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public static string HaimiDistribute(int userId, decimal haimiAmount, int orderState, string orderCode, int settleState, int settleLevel)
        {
            try
            {
                APIDictionary Params = new APIDictionary();
                Params.Add("userId", userId);
                Params.Add("haimiAmount", haimiAmount);
                Params.Add("orderState", orderState);
                Params.Add("orderCode", orderCode);
                Params.Add("settleState", settleState);
                Params.Add("settleLevel", settleLevel);

                string FromDirSale = client.Execute(Params, "/api/user/HaimiDistribute");
                return FromDirSale;
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("FailHaimiDistribute", "直销会员海米三级结算（直销系统）", "错误信息为：会员名：" + userId + "|失败原因,请求异常：" + ex.Message);
                return "{\"Result\":-1,\"Msg\":\"三级分销结算异常\",\"Info\":\"-1\"}";
            }
        }

        /// <summary>
        /// 创建直销会员
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static MemberInfo CreateMember(PartUserInfo userInfo, string realName, string managerCode, int placeSide, string idCard, string userPhone)
        {
            string userCode = string.IsNullOrEmpty(userInfo.UserName) ? (string.IsNullOrEmpty(userInfo.Mobile) ? "" : userInfo.Mobile) : userInfo.UserName; //userCode：登录名
            string userName = userCode;//userName：用户名
            string password = System.Web.HttpUtility.UrlEncode(userInfo.DirSalePwd);
            int rank = 0;
            if (userInfo.AgentType == 1 || userInfo.AgentType == 2)
                rank = 1;
            if (userInfo.AgentType == 3)
                rank = 2;
            if (userInfo.AgentType == 4)
                rank = 4;
            PartUserInfo parentuser = GetParentUserForDirSale(userInfo);
            if (parentuser.Uid <= 0)
                return new MemberInfo();
            string pcode = GetUserCode(parentuser.DirSaleUid);
            //string pcode =OrderUtils.GetParentCode(userInfo.Uid);
            //if (string.IsNullOrEmpty(pcode))
            //    return new MemberInfo();
            string parentCode = pcode;//parentCode：推荐人编号

            APIDictionary Params = new APIDictionary();
            Params.Add("userCode", userCode);
            Params.Add("userName", realName);
            Params.Add("nickName", userInfo.NickName);
            Params.Add("pwd", password);
            Params.Add("rank", rank);
            //Params.Add("station", "");
            Params.Add("parentCode", pcode);
            Params.Add("managerCode", managerCode);
            Params.Add("placeSide", placeSide);
            Params.Add("idCard", idCard);
            Params.Add("userPhone", userPhone);
            //Params.Add("userBank", "");
            //Params.Add("userCardName", "");
            //Params.Add("userCardNo", "");
            //Params.Add("province", "");
            //Params.Add("city", "");
            //Params.Add("area", "");
            //Params.Add("address", "");
            //Params.Add("phaseId", "");
            //Params.Add("activeTime", "");

            StringBuilder sb = new StringBuilder();
            foreach (var item in Params)
            {
                sb.Append(item.Key + ":" + item.Value + "\r\n");
            }
            LogHelper.WriteOperateLog("CreateMemberlog", "创建直销会员api参数记录", "参数列表:\r\n" + sb.ToString());

            try
            {
                string FromDirSale = client.Execute(Params, "/api/user/createMember");
                LogHelper.WriteOperateLog("CreateMemberlog", "创建直销会员api接口记录", "接口调用返回：" + FromDirSale);
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
                JToken token = (JToken)jsonObject;
                if (token["Result"].ToString() == "0")
                {
                    //{"Result":0,"Msg":"用户注册成功","Info":{"ParentCode":"company","ManagerCode":"zhangjunsheng10"}}
                    string resultParentCode = token["Info"].SelectToken("ParentCode").ToString();
                    string resultManagerCode = token["Info"].SelectToken("ManagerCode").ToString();
                    int resultUserId = TypeHelper.ObjectToInt(token["Info"].SelectToken("UserId"));//新增返回userId
                    PartUserInfo pUser = new PartUserInfo();
                    if (userInfo.Ptype == 1)
                        pUser = Users.GetPartUserByName(resultParentCode);
                    //更新会员的直销标识 isdirsaleuser 和dirsaleuid 
                    OrderUtils.UpdateUserOutSates(resultUserId, userInfo.Uid, userInfo.Ptype, pUser);

                    //成功加入直销后下一级的推荐类型、推荐id改成直销推荐类型、直销uid
                    Users.UpdateAgentUserForChildren(userInfo, resultUserId);

                    MemberInfo member = new MemberInfo();
                    member.userId = resultUserId;
                    member.ParentCode = parentCode;
                    member.ManagerCode = resultManagerCode;
                    return member;
                }
                else
                {
                    LogHelper.WriteOperateLog("CreateMemberError", "创建直销会员api接口错误", "错误信息为：会员ID：" + userInfo.Uid + "，会员名称" + userCode);
                    return new MemberInfo();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("CreateMemberError", "创建直销会员api接口错误", "错误信息为：会员ID：" + userInfo.Uid + "，会员名称" + userCode + "，错误信息:" + ex.Message);
                return new MemberInfo();
            }
        }

        /// <summary>
        /// /// <summary>
        /// 16.更改会员级别
        /// http://www.xxxx.com/api/User/changeRank
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool ChangeRank(PartUserInfo userInfo, int rank)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("userId", userInfo.DirSaleUid);
            Params.Add("rank", rank);
            string FromDirSale = client.Execute(Params, "/api/User/changeRank");

            JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            JToken token = (JToken)jsonObject;

            if (token["Result"].ToString() == "0")
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// /// <summary>
        /// 17.注销会员
        /// http://www.xxxx.com/api/User/exitUser
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool ExitUser(PartUserInfo userInfo)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("userId", userInfo.DirSaleUid);
            string FromDirSale = client.Execute(Params, "/api/User/exitUser");

            JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
            JToken token = (JToken)jsonObject;

            if (token["Result"].ToString() == "0")
            {
                //更新会员的直销标识 isdirsaleuser 和dirsaleuid 
                //UpdateFXUserOutSates(userInfo.DirSaleUid, userInfo.Uid, 2);
                return true;
            }
            return false;
        }


        /// <summary>
        /// 获得用户的有效的最近直销上级会员
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static PartUserInfo GetParentUserForDirSale(PartUserInfo user)
        {
            int pId = user.Pid;
            int ptype = user.Ptype;
            PartUserInfo parent = new PartUserInfo();
            if (user.Ptype == 1)
            {
                do
                {
                    if (ptype == 1)
                        parent = Users.GetUserById(pId);
                    if (ptype == 2)
                    {
                        string strWhere = string.Format("  isdirsaleuser=1 and dirsaleuid={0} ", pId);
                        parent = Users.GetPartUserModelByStrWhere(strWhere);
                    }
                    if (parent == null)
                    {
                        parent = new PartUserInfo();
                        break;
                    }

                    pId = parent.Pid;
                    ptype = parent.Ptype;
                    if (pId <= 0)
                        break;
                } while (!parent.IsDirSaleUser);

            }
            else
            {
                string strWhere = string.Format("  isdirsaleuser=1 and dirsaleuid={0} ", user.Pid);
                parent = Users.GetPartUserModelByStrWhere(strWhere);
            }
            return parent;
        }
    }

    public class MemberInfo
    {
        public MemberInfo()
        {
            userId = 0;
            ParentCode = "";
            ManagerCode = "";
        }
        public int userId { get; set; }
        public string ParentCode { get; set; }
        public string ManagerCode { get; set; }
    }

    public class DSDetailInfo
    {
        public int UserId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string UserCard { get; set; }
        public string UserPhone { get; set; }
        public int Rank { get; set; }
        public string RankName { get; set; }
        public string ParentCode { get; set; }
        public string ManagerCode { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
