using System;
using System.Data;
using VMall.Core;
using DirSale;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using DAL.Base;

namespace VMall.Services
{
    /// <summary>
    /// 用户操作管理类
    /// </summary>
    public partial class Users : BaseDAL<PartUserInfo>
    {
        /// <summary>
        /// 获得部分用户
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static PartUserInfo GetPartUserById(int uid)
        {
            if (uid > 0)
                return VMall.Data.Users.GetPartUserById(uid);

            return null;
        }

        /// <summary>
        /// 获得用户
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static UserInfo GetUserById(int uid)
        {
            if (uid > 0)
                return VMall.Data.Users.GetUserById(uid);

            return null;
        }

        /// <summary>
        /// 获得用户细节
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static UserDetailInfo GetUserDetailById(int uid)
        {
            if (uid > 0)
                return VMall.Data.Users.GetUserDetailById(uid);

            return null;
        }

        /// <summary>
        /// 获得部分用户
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static PartUserInfo GetPartUserByUidAndPwd(int uid, string password)
        {
            PartUserInfo partUserInfo = null;

            string isDirSaleUser = MallUtils.GetIsDirSaleUserCookie();
            //if (!string.IsNullOrEmpty(isDirSaleUser) && isDirSaleUser == "1")
            //{
            //    partUserInfo = GetDirSaleUser(uid);
            //}
            //else
            //{
            //    partUserInfo = GetPartUserById(uid);
            //}
            partUserInfo = GetPartUserById(uid);
            if (partUserInfo != null && partUserInfo.Password == password)
                return partUserInfo;
            return null;
        }

        /// <summary>
        /// 获得部分用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public static PartUserInfo GetPartUserByName(string userName)
        {
            return VMall.Data.Users.GetPartUserByName(userName);
        }

        /// <summary>
        /// 获得部分用户
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <returns></returns>
        public static PartUserInfo GetPartUserByEmail(string email)
        {
            return VMall.Data.Users.GetPartUserByEmail(email);
        }

        /// <summary>
        /// 获得部分用户
        /// </summary>
        /// <param name="mobile">用户手机</param>
        /// <returns></returns>
        public static PartUserInfo GetPartUserByMobile(string mobile)
        {
            return VMall.Data.Users.GetPartUserByMobile(mobile);
        }

        /// <summary>
        /// 获得用户id
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public static int GetUidByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return -1;

            return VMall.Data.Users.GetUidByUserName(userName);
        }

        /// <summary>
        /// 获得用户id
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <returns></returns>
        public static int GetUidByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return -1;

            return VMall.Data.Users.GetUidByEmail(email);
        }

        /// <summary>
        /// 获得用户id
        /// </summary>
        /// <param name="mobile">用户手机</param>
        /// <returns></returns>
        public static int GetUidByMobile(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return -1;

            return VMall.Data.Users.GetUidByMobile(mobile);
        }

        /// <summary>
        /// 获得用户id
        /// </summary>
        /// <param name="accountName">账户名</param>
        /// <returns></returns>
        public static int GetUidByAccountName(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName))
                return -1;

            if (ValidateHelper.IsEmail(accountName))//邮箱
            {
                return GetUidByEmail(accountName);
            }
            else if (ValidateHelper.IsMobile(accountName))//手机
            {
                return GetUidByMobile(accountName);
            }
            else//用户名
            {
                return GetUidByUserName(accountName);
            }
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        public static int CreateUser(UserInfo userInfo)
        {
            return VMall.Data.Users.CreateUser(userInfo);
        }

        /// <summary>
        /// 创建部分用户
        /// </summary>
        /// <returns></returns>
        public static PartUserInfo CreatePartGuest()
        {
            return new PartUserInfo
            {
                Uid = -1,
                UserName = "guest",
                Email = "",
                Mobile = "",
                Password = "",
                UserRid = 6,
                StoreId = 0,
                MallAGid = 1,
                NickName = "游客",
                Avatar = "",
                PayCredits = 0,
                RankCredits = 0,
                VerifyEmail = 0,
                VerifyMobile = 0,
                LiftBanTime = new DateTime(1900, 1, 1),
                Salt = ""
            };
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <returns></returns>
        public static void UpdateUser(UserInfo userInfo)
        {
            VMall.Data.Users.UpdateUser(userInfo);
        }

        /// <summary>
        /// 更新部分用户
        /// </summary>
        /// <returns></returns>
        public static void UpdatePartUser(PartUserInfo partUserInfo)
        {
            VMall.Data.Users.UpdatePartUser(partUserInfo);
        }

        /// <summary>
        /// 更新用户细节
        /// </summary>
        /// <returns></returns>
        public static void UpdateUserDetail(UserDetailInfo userDetailInfo)
        {
            VMall.Data.Users.UpdateUserDetail(userDetailInfo);
        }

        /// <summary>
        /// 更新用户最后访问
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="visitTime">访问时间</param>
        /// <param name="ip">ip</param>
        /// <param name="regionId">区域id</param>
        public static void UpdateUserLastVisit(int uid, DateTime visitTime, string ip, int regionId)
        {
            VMall.Data.Users.UpdateUserLastVisit(uid, visitTime, ip, regionId);
        }

        /// <summary>
        /// 用户名是否存在
        /// </summary>
        /// <param name="userName">用户名</param>
        public static bool IsExistUserName(string userName)
        {
            return GetUidByUserName(userName) > 0 ? true : false;
        }

        /// <summary>
        /// 邮箱是否存在
        /// </summary>
        /// <param name="userName">邮箱</param>
        public static bool IsExistEmail(string email)
        {
            return GetUidByEmail(email) > 0 ? true : false;
        }

        /// <summary>
        /// 手机是否存在
        /// </summary>
        /// <param name="userName">手机</param>
        public static bool IsExistMobile(string mobile)
        {
            return GetUidByMobile(mobile) > 0 ? true : false;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="uidList">用户id</param>
        public static void DeleteUserById(int[] uidList)
        {
            //此方法需要补充
            if (uidList != null && uidList.Length > 0)
                VMall.Data.Users.DeleteUserById(CommonHelper.IntArrayToString(uidList));
        }

        /// <summary>
        /// 创建用户密码
        /// </summary>
        /// <param name="password">真实密码</param>
        /// <param name="salt">散列盐值</param>
        /// <returns></returns>
        public static string CreateUserPassword(string password, string salt)
        {
            return SecureHelper.MD5(password + salt);
        }

        /// <summary>
        /// 获得用户等级下用户的数量
        /// </summary>
        /// <param name="userRid">用户等级id</param>
        /// <returns></returns>
        public static int GetUserCountByUserRid(int userRid)
        {
            return VMall.Data.Users.GetUserCountByUserRid(userRid);
        }

        /// <summary>
        /// 获得商城管理员组下用户的数量
        /// </summary>
        /// <param name="mallAGid">商城管理员组id</param>
        /// <returns></returns>
        public static int GetUserCountByMallAGid(int mallAGid)
        {
            return VMall.Data.Users.GetUserCountByMallAGid(mallAGid);
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="userName">用户名</param>
        /// <param name="nickName">昵称</param>
        /// <param name="avatar">头像</param>
        /// <param name="gender">性别</param>
        /// <param name="realName">真实名称</param>
        /// <param name="bday">出生日期</param>
        /// <param name="idCard">The id card.</param>
        /// <param name="regionId">区域id</param>
        /// <param name="address">所在地</param>
        /// <param name="bio">简介</param>
        /// <returns></returns>
        public static bool UpdateUser(int uid, string userName, string nickName, string avatar, int gender, string realName, DateTime bday, string idCard, int regionId, string address, string bio, string bankName, string bankCardCode, string bankUserName)
        {
            return VMall.Data.Users.UpdateUser(uid, userName, nickName, avatar, gender, realName, bday, idCard, regionId, address, bio, bankName, bankCardCode, bankUserName);
        }

        ///// <summary>
        ///// 更新用户
        ///// </summary>
        ///// <returns></returns>
        //public void UpdateUser(UserInfo userInfo)
        //{
        //    DbParameter[] parms = {
        //                               GenerateInParam("@username",SqlDbType.NChar,20,userInfo.UserName),
        //                               GenerateInParam("@email",SqlDbType.Char,50,userInfo.Email),
        //                               GenerateInParam("@mobile",SqlDbType.Char,15,userInfo.Mobile),
        //                               GenerateInParam("@password",SqlDbType.Char,32,userInfo.Password),
        //                               GenerateInParam("@userrid",SqlDbType.SmallInt,2,userInfo.UserRid),
        //                               GenerateInParam("@storeid",SqlDbType.Int,4,userInfo.StoreId),
        //                               GenerateInParam("@mallagid",SqlDbType.SmallInt,2,userInfo.MallAGid),
        //                               GenerateInParam("@nickname",SqlDbType.NChar,20,userInfo.NickName),
        //                               GenerateInParam("@avatar",SqlDbType.Char,40,userInfo.Avatar),
        //                               GenerateInParam("@paycredits",SqlDbType.Int,4,userInfo.PayCredits),
        //                               GenerateInParam("@rankcredits",SqlDbType.Int,4,userInfo.RankCredits),
        //                               GenerateInParam("@verifyemail",SqlDbType.TinyInt,1,userInfo.VerifyEmail),
        //                               GenerateInParam("@verifymobile",SqlDbType.TinyInt,1,userInfo.VerifyMobile),
        //                               GenerateInParam("@liftbantime",SqlDbType.DateTime,8,userInfo.LiftBanTime),
        //                               GenerateInParam("@salt",SqlDbType.NChar,6,userInfo.Salt),
        //                               GenerateInParam("@lastvisittime",SqlDbType.DateTime,8,userInfo.LastVisitTime),
        //                               GenerateInParam("@lastvisitip",SqlDbType.Char,15,userInfo.LastVisitIP),
        //                               GenerateInParam("@lastvisitrgid",SqlDbType.SmallInt,2,userInfo.LastVisitRgId),
        //                               GenerateInParam("@registertime",SqlDbType.DateTime,8,userInfo.RegisterTime),
        //                               GenerateInParam("@registerip",SqlDbType.Char,15,userInfo.RegisterIP),
        //                               GenerateInParam("@registerrgid",SqlDbType.SmallInt,2,userInfo.RegisterRgId),
        //                               GenerateInParam("@gender",SqlDbType.TinyInt,1,userInfo.Gender),
        //                               GenerateInParam("@realname",SqlDbType.NVarChar,10,userInfo.RealName),
        //                               GenerateInParam("@bday",SqlDbType.DateTime,8,userInfo.Bday),
        //                               GenerateInParam("@idcard",SqlDbType.VarChar,18,userInfo.IdCard),
        //                               GenerateInParam("@regionid",SqlDbType.SmallInt,2,userInfo.RegionId),
        //                               GenerateInParam("@address",SqlDbType.NVarChar,150,userInfo.Address),
        //                               GenerateInParam("@bio",SqlDbType.NVarChar,300,userInfo.Bio),
        //                               GenerateInParam("@uid",SqlDbType.Int,4,userInfo.Uid)
        //                           }
        //    SqlParameter[] parms =  {
        //                               new SqlParameter("@uid", userInfo.Uid),
        //                               new SqlParameter("@avatar",userInfo.Avatar)
        //                           };
        //    string commandText = string.Format("UPDATE [{0}users] SET [avatar]=@avatar WHERE  [uid]=@uid",
        //                                        RDBSHelper.RDBSTablePre);
        //    return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;

        //    RDBSHelper.ExecuteScalar(CommandType.StoredProcedure,
        //                             string.Format("{0}updateuser", RDBSHelper.RDBSTablePre),
        //                             parms);
        //}

        /// <summary>
        /// 更新用户邮箱
        /// </summary>
        /// <param name="uid">用户id.</param>
        /// <param name="email">邮箱</param>
        public static void UpdateUserEmailByUid(int uid, string email)
        {
            VMall.Data.Users.UpdateUserEmailByUid(uid, email);
        }

        /// <summary>
        /// 更新用户手机
        /// </summary>
        /// <param name="uid">用户id.</param>
        /// <param name="mobile">手机</param>
        public static void UpdateUserMobileByUid(int uid, string mobile)
        {
            VMall.Data.Users.UpdateUserMobileByUid(uid, mobile);
        }

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="uid">用户id.</param>
        /// <param name="password">密码</param>
        public static void UpdateUserPasswordByUid(int uid, string password)
        {
            VMall.Data.Users.UpdateUserPasswordByUid(uid, password);

        }

        /// <summary>
        /// 生成用户盐值
        /// </summary>
        /// <returns></returns>
        public static string GenerateUserSalt()
        {
            return Randoms.CreateRandomValue(6);
        }

        /// <summary>
        /// 更新用户解禁时间
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="liftBanTime">解禁时间</param>
        public static void UpdateUserLiftBanTimeByUid(int uid, DateTime liftBanTime)
        {
            VMall.Data.Users.UpdateUserLiftBanTimeByUid(uid, liftBanTime);
        }

        /// <summary>
        /// 更新用户解禁时间
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="userRankInfo">用户等级</param>
        public static void UpdateUserLiftBanTimeByUid(int uid, UserRankInfo userRankInfo)
        {
            UpdateUserLiftBanTimeByUid(uid, DateTime.Now.AddDays(userRankInfo.LimitDays));
        }

        /// <summary>
        /// 更新用户等级
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="userRid">用户等级id</param>
        public static void UpdateUserRankByUid(int uid, int userRid)
        {
            VMall.Data.Users.UpdateUserRankByUid(uid, userRid);
        }

        /// <summary>
        /// 更新用户在线时间
        /// </summary>
        /// <param name="uid">用户id</param>
        public static void UpdateUserOnlineTime(int uid)
        {
            int updateOnlineTimeSpan = BMAConfig.MallConfig.UpdateOnlineTimeSpan;
            if (updateOnlineTimeSpan == 0)
                return;

            int lastUpdateTime = TypeHelper.StringToInt(WebHelper.GetCookie("onlinetime"));
            if (lastUpdateTime > 0 && lastUpdateTime < (Environment.TickCount - updateOnlineTimeSpan * 60 * 1000))
            {
                VMall.Data.Users.UpdateUserOnlineTime(uid, updateOnlineTimeSpan, DateTime.Now);
                WebHelper.SetCookie("onlinetime", Environment.TickCount.ToString());
            }
            else if (lastUpdateTime == 0)
            {
                WebHelper.SetCookie("onlinetime", Environment.TickCount.ToString());
            }
        }

        /// <summary>
        /// 通过注册ip获得注册时间
        /// </summary>
        /// <param name="registerIP">注册ip</param>
        /// <returns></returns>
        public static DateTime GetRegisterTimeByRegisterIP(string registerIP)
        {
            return VMall.Data.Users.GetRegisterTimeByRegisterIP(registerIP);
        }

        /// <summary>
        /// 获得用户最后访问时间
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static DateTime GetUserLastVisitTimeByUid(int uid)
        {
            return VMall.Data.Users.GetUserLastVisitTimeByUid(uid);
        }

        /// <summary>
        /// 获得店铺管理员id
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static int GetStoreAdminerIdByStoreId(int storeId)
        {
            return VMall.Data.Users.GetStoreAdminerIdByStoreId(storeId);
        }

        #region 分享会员

        /// <summary>
        /// 获得分享用户列表数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        public static int GetUserCount(PartUserInfo user)
        {
            SqlParameter[] parms = {
                                        new SqlParameter("@pid", user.Uid),  
                                        new SqlParameter("@diruid", user.DirSaleUid)
                                    };
            string commandText = string.Empty;
            if (user.IsDirSaleUser)
            {
                commandText = string.Format(@"SELECT count(*) FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@diruid AND ptype=2  ", RDBSHelper.RDBSTablePre);
            }
            else
            {
                commandText = string.Format(@"SELECT count(*)  FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@pid AND ptype=1 ", RDBSHelper.RDBSTablePre);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText,
                                                                   parms));
        }

        /// <summary>
        /// 获得下一级代理用户列表数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        public static int GetAgentUserCount(PartUserInfo user)
        {
            SqlParameter[] parms = {
                                        new SqlParameter("@pid", user.Uid),  
                                        new SqlParameter("@diruid", user.DirSaleUid)
                                    };
            string commandText = string.Empty;
            if (user.IsDirSaleUser)
            {
                commandText = string.Format(@"SELECT count(*) FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@diruid AND ptype=2 and agenttype>0 ", RDBSHelper.RDBSTablePre);
            }
            else
            {
                commandText = string.Format(@"SELECT count(*)  FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@pid AND ptype=1 and agenttype>0", RDBSHelper.RDBSTablePre);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText,
                                                                   parms));
        }
        /// <summary>
        /// 获得下一级具体代理用户列表数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        public static int GetAgentUserCount(PartUserInfo user, int agentType)
        {
            SqlParameter[] parms = {
                                        new SqlParameter("@pid", user.Uid),  
                                        new SqlParameter("@diruid", user.DirSaleUid),
                                        new SqlParameter("@agenttype", agentType)
                                    };
            string commandText = string.Empty;
            if (user.IsDirSaleUser)
            {
                commandText = string.Format(@"SELECT count(*) FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@diruid AND ptype=2 and agenttype=@agenttype ", RDBSHelper.RDBSTablePre);
            }
            else
            {
                commandText = string.Format(@"SELECT count(*)  FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@pid AND ptype=1 and agenttype=@agenttype ", RDBSHelper.RDBSTablePre);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms));
        }
        /// <summary>
        /// 获得下一级具体代理用户列表数量再某段时间内
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        public static int GetAgentUserCountInSomeTime(PartUserInfo user, int agentType)
        {
            SqlParameter[] parms = {
                                        new SqlParameter("@pid", user.Uid),  
                                        new SqlParameter("@diruid", user.DirSaleUid),
                                        new SqlParameter("@agenttype", agentType),
                                    };
            DateTime intime= DateTime.Now.AddMonths(-3);
            string commandText = string.Empty;
            if (user.IsDirSaleUser)
            {
                commandText = string.Format(@"SELECT count(*) FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@diruid AND ptype=2 and agenttype=@agenttype  and activetime>'{1}'", RDBSHelper.RDBSTablePre, intime);
            }
            else
            {
                commandText = string.Format(@"SELECT count(*)  FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@pid AND ptype=1 and agenttype=@agenttype and activetime>'{1}'", RDBSHelper.RDBSTablePre, intime);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms));
        }
        /// <summary>
        /// 根据id获取被分享会员列表
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<UserInfo> GetSubRecommendListByPid(PartUserInfo user, int pageNo = 1, int pageSize = 8)
        {
            List<UserInfo> subUserList = new List<UserInfo>();

            SqlParameter[] parms = {
                                        new SqlParameter("@pid", user.Uid),  
                                        new SqlParameter("@diruid", user.DirSaleUid)
                                    };
            string commandText = string.Empty;
            if (pageNo == 1)
            {
                if (user.IsDirSaleUser)
                {
                    commandText = string.Format(@"SELECT TOP {1} * FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@diruid AND ptype=2  ORDER BY a.[uid] DESC", RDBSHelper.RDBSTablePre, pageSize);
                }
                else
                {
                    commandText = string.Format(@"SELECT TOP {1} * FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@pid AND ptype=1  ORDER BY a.[uid] DESC", RDBSHelper.RDBSTablePre, pageSize);
                }
            }
            else
            {
                if (user.IsDirSaleUser)
                {
                    commandText = string.Format(@"with tb as(
 select a.*,b.uid AS userid,b.lastvisittime,b.lastvisitip,b.lastvisitrgid,b.registerip,b.registerrgid,b.gender,realname,b.bday,b.idcard,b.regionid,b.address,b.bio,b.registertime ,b.bankname,b.bankcardcode,b.bankusername ,ROW_NUMBER() OVER (ORDER BY a.[uid] DESC)  AS RowNumber from [hlh_users] as a left join 
 hlh_userdetails as b on a.uid=b.uid
 WHERE a.[pid]=@diruid AND a.ptype=2 )
 select * from tb where RowNumber between (" + pageNo + @"-1)*" + pageSize + @"+1 and (" + pageNo + @")*" + pageSize + " order by tb.[uid] desc",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNo - 1) * pageSize
                                                );
                }
                else
                {
                    commandText = string.Format(@"with tb as(
 select a.*,b.uid AS userid,b.lastvisittime,b.lastvisitip,b.lastvisitrgid,b.registerip,b.registerrgid,b.gender,realname,b.bday,b.idcard,b.regionid,b.address,b.bio,b.registertime ,b.bankname,b.bankcardcode,b.bankusername ,ROW_NUMBER() OVER (ORDER BY a.[uid] DESC) AS RowNumber from [hlh_users] as a left join 
 hlh_userdetails as b on a.uid=b.uid
 WHERE a.[pid]=@pid AND a.ptype=1 )
 select * from tb where RowNumber between (" + pageNo + @"-1)*" + pageSize + @"+1 and (" + pageNo + @")*" + pageSize + " order by tb.[uid] desc",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNo - 1) * pageSize
                                                );
                }
            }

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    UserInfo partUserInfo = new UserInfo();
                    partUserInfo = VMall.Data.Users.BuildUserFromReader(reader);
                    partUserInfo.ParentLevel = 1;
                    subUserList.Add(partUserInfo);
                }
                reader.Close();
            }
            return subUserList;
        }


        /// <summary>
        /// 获得分享用户列表数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        public static int GetUserCountForAgentSystem(PartUserInfo user)
        {
            SqlParameter[] parms = {
                                        new SqlParameter("@agentpid", user.Uid),  
                                        new SqlParameter("@diruid", user.DirSaleUid)
                                    };
            string commandText = string.Format(@"SELECT count(*)  FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [agentpid]=@agentpid AND agentptype=1 ", RDBSHelper.RDBSTablePre);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText,
                                                                   parms));
        }
        /// <summary>
        /// 根据id获取被分享会员列表
        /// </summary>
        /// <param name="agentpid"></param>
        /// <returns></returns>
        public static List<UserInfo> GetSubRecommendListByPidForAgentSystem(PartUserInfo user, int pageNo = 1, int pageSize = 8)
        {
            List<UserInfo> subUserList = new List<UserInfo>();
            SqlParameter[] parms = {
                                        new SqlParameter("@agentpid", user.Uid),  
                                        new SqlParameter("@diruid", user.DirSaleUid)
                                    };
            string commandText = string.Empty;
            if (pageNo == 1)
            {

                commandText = string.Format(@"SELECT TOP {1} * FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [agentpid]=@agentpid AND agentptype=1  ORDER BY a.[uid] DESC", RDBSHelper.RDBSTablePre, pageSize);

            }
            else
            {
                commandText = string.Format(@"with tb as(
 select a.*,b.uid AS userid,b.lastvisittime,b.lastvisitip,b.lastvisitrgid,b.registerip,b.registerrgid,b.gender,realname,b.bday,b.idcard,b.regionid,b.address,b.bio,b.registertime ,b.bankname,b.bankcardcode,b.bankusername ,ROW_NUMBER() OVER (ORDER BY a.[uid] DESC) AS RowNumber from [hlh_users] as a left join 
 hlh_userdetails as b on a.uid=b.uid
 WHERE a.[agentpid]=@agentpid AND a.agentptype=1 )
 select * from tb where RowNumber between (" + pageNo + @"-1)*" + pageSize + @"+1 and (" + pageNo + @")*" + pageSize + " order by tb.[uid] desc",
                                            pageSize,
                                            RDBSHelper.RDBSTablePre,
                                            (pageNo - 1) * pageSize
                                            );

            }

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    UserInfo partUserInfo = new UserInfo();
                    partUserInfo = VMall.Data.Users.BuildUserFromReader(reader);
                    partUserInfo.ParentLevel = 1;
                    subUserList.Add(partUserInfo);
                }
                reader.Close();
            }
            return subUserList;
        }

        #endregion


        /// <summary>
        /// 更新会员头像
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdatePartUserAvatar(PartUserInfo userInfo)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", userInfo.Uid),
                                       new SqlParameter("@avatar",userInfo.Avatar)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [avatar]=@avatar WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        /// <summary>
        /// 更新会员密码--new
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdatePartUserPwd(PartUserInfo userInfo, string password)
        {
            userInfo.Password = CreateUserPassword(password, userInfo.Salt);
            userInfo.DirSalePwd = SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", userInfo.Uid),
                                       new SqlParameter("@password", userInfo.Password),
                                       new SqlParameter("@dirsalepwd",userInfo.DirSalePwd)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [password]=@password,[dirsalepwd]=@dirsalepwd WHERE  [uid]=@uid",
                                                RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 更新会员支付密码
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdatePartUserPayPwd(PartUserInfo userInfo, string password)
        {
            userInfo.PayPassword = CreateUserPassword(password, userInfo.Salt);

            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", userInfo.Uid),
                                       new SqlParameter("@paypassword", userInfo.PayPassword)
                                      
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [paypassword]=@paypassword WHERE  [uid]=@uid",
                                                RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 更新会员直销密码
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdatePartUserDirSalePwd(PartUserInfo userInfo)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", userInfo.Uid),
                                       new SqlParameter("@dirsalepwd",userInfo.DirSalePwd)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [dirsalepwd]=@dirsalepwd WHERE  [uid]=@uid",
                                                RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        /// <summary>
        /// 根据外部id获取会员
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static PartUserInfo GetPartUserInfoByOtherLoginId(string otherLoginId)
        {
            PartUserInfo partUserInfo = null; ;
            SqlParameter[] parms = {
                                        new SqlParameter("@otherloginid", otherLoginId)
                                    };
            string commandText = string.Format(@"SELECT * FROM [{0}users]  WHERE [otherloginid]=@otherloginid", RDBSHelper.RDBSTablePre);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                if (reader.Read())
                {
                    partUserInfo = VMall.Data.Users.BuildPartUserFromReader(reader);
                }
                reader.Close();
            }
            return partUserInfo;
        }
        /// <summary>
        /// 更新会员微信绑定资料
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdatePartUserForWeiXin(PartUserInfo userInfo)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", userInfo.Uid),
                                       new SqlParameter("@nickname",userInfo.NickName),
                                       new SqlParameter("@avatar",userInfo.Avatar),
                                       new SqlParameter("@otherloginid",userInfo.OtherLoginId)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [nickname]=@nickname,[avatar]=@avatar,[otherloginid]=@otherloginid WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 微信绑定接触
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool WeiXinUnBind(int uid)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", uid)                                       
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [otherloginid]='' WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        /// <summary>
        /// 根据直销id获取会员
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static PartUserInfo GetPartUserInfoByDirSaleUid(int dirSaleuid)
        {
            PartUserInfo partUserInfo = null; ;
            SqlParameter[] parms = {
                                        new SqlParameter("@dirsaleuid", dirSaleuid)
                                    };
            string commandText = string.Format(@"SELECT TOP 1 * FROM [{0}users]  WHERE [isdirsaleuser]=1 and [dirsaleuid]=@dirsaleuid", RDBSHelper.RDBSTablePre);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                if (reader.Read())
                {
                    partUserInfo = VMall.Data.Users.BuildPartUserFromReader(reader);
                }
                reader.Close();
            }
            return partUserInfo;
        }

        /// <summary>
        /// 根据条件获取会员
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<PartUserInfo> GetPartUserInfoByStrWhere(string strWhere)
        {
            List<PartUserInfo> list = new List<PartUserInfo>();
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(strWhere))
            {
                sb.Append(" where ");
                sb.Append(strWhere);
            }
            string commandText = "select * from hlh_users " + sb.ToString();
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    PartUserInfo info = VMall.Data.Users.BuildPartUserFromReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 根据条件获取会员，包括详情
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public static List<UserInfo> GetUserInfoByStrWhere(string strWhere)
        {
            List<UserInfo> list = new List<UserInfo>();
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(strWhere))
            {
                sb.Append(" where ");
                sb.Append(strWhere);
            }
            string commandText = "select a.*,b.lastvisittime,b.lastvisitip,b.lastvisitrgid,b.registertime,b.registerip,b.registerrgid,b.gender,b.realname,b.bday,b.idcard,b.regionid,b.address,b.bio,b.bankname,b.bankcardcode,b.bankusername from hlh_users as a inner join hlh_userdetails as b on a.uid=b.uid" + sb.ToString();
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    UserInfo info = VMall.Data.Users.BuildUserFromReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }
        /// <summary>
        /// 根据条件获取会员
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static PartUserInfo GetPartUserModelByStrWhere(string strWhere)
        {
            PartUserInfo info = new PartUserInfo();
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(strWhere))
            {
                sb.Append(" where ");
                sb.Append(strWhere);
            }
            string commandText = "select top 1 * from hlh_users " + sb.ToString();
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                if (reader.Read())
                {
                    info = VMall.Data.Users.BuildPartUserFromReader(reader);
                }
                reader.Close();
            }
            return info;
        }
        /// <summary>
        /// 获得用户的有效的最近代理上级会员
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static PartUserInfo GetParentUserForAgent(PartUserInfo user)
        {
            int pId = user.Pid;
            int ptype = user.Ptype;
            PartUserInfo parent = new PartUserInfo();
            if (user.Ptype == 1)
            {
                do
                {
                    if (ptype == 1)
                        parent = GetUserById(pId);
                    if (ptype == 2)
                    {
                        string strWhere = string.Format("  isdirsaleuser=1 and dirsaleuid={0} ", pId);
                        parent = GetPartUserModelByStrWhere(strWhere);
                    }
                    if (parent == null)
                    {
                        parent = new PartUserInfo();
                        break;
                    }
                    if (parent.IsDirSaleUser)
                        break;
                    pId = parent.Pid;
                    ptype = parent.Ptype;
                    if (pId <= 0)
                        break;
                } while (parent.AgentType <= 0);
            }
            else
            {
                //string strWhere = string.Format("  isdirsaleuser=1 and dirsaleuid={0} ", pId);
                //parent = GetPartUserModelByStrWhere(strWhere);
                do
                {
                    string strWhere = string.Format("  isdirsaleuser=1 and dirsaleuid={0} ", pId);
                    parent = GetPartUserModelByStrWhere(strWhere);
                    if (parent == null)
                    {
                        parent = new PartUserInfo();
                        break;
                    }
                    if (parent.IsDirSaleUser && parent.AgentType > user.AgentType)
                        break;
                    pId = parent.Pid;
                    ptype = parent.Ptype;
                    if (pId <= 0)
                        break;
                } while (parent.AgentType <= 0);

            }
            return parent;
        }

        /// <summary>
        /// 获得用户的有效的最近代理库存上级会员
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static PartUserInfo GetParentUserForAgentStock(PartUserInfo user)
        {
            int pId = user.Pid;
            int ptype = user.Ptype;
            PartUserInfo parent = new PartUserInfo();
            if (user.Ptype == 1)
            {
                //while (parent.AgentType > 0) {
                //    parent = GetUserById(pId);
                //    if (parent == null) break;
                //    pId = parent.Pid;
                //}
                do
                {
                    if (ptype == 1)
                        parent = GetUserById(pId);
                    if (ptype == 2)
                    {
                        string strWhere = string.Format("  isdirsaleuser=1 and dirsaleuid={0} ", pId);
                        parent = GetPartUserModelByStrWhere(strWhere);
                    }
                    if (parent == null)
                    {
                        parent = new PartUserInfo();
                        break;
                    }
                    if (parent.IsDirSaleUser && parent.AgentType > user.AgentType)
                        break;
                    pId = parent.Pid;
                    ptype = parent.Ptype;
                    if (pId <= 0)
                        break;
                } while (parent.AgentType <= 0 || parent.AgentType <= user.AgentType);
            }
            else
            {
                do
                {
                    string strWhere = string.Format("  isdirsaleuser=1 and dirsaleuid={0} ", pId);
                    parent = GetPartUserModelByStrWhere(strWhere);
                    if (parent == null)
                    {
                        parent = new PartUserInfo();
                        break;
                    }
                    if (parent.IsDirSaleUser && parent.AgentType > user.AgentType)
                        break;
                    pId = parent.Pid;
                    ptype = parent.Ptype;
                    if (pId <= 0)
                        break;
                } while (parent.AgentType <= 0 || parent.AgentType <= user.AgentType);
            }
            return parent;
        }

        /// <summary>
        /// 更新会员名
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdatePartUserNameByUid(int uid, string userName)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", uid),
                                       new SqlParameter("@username",userName)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [userName]=@userName WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 更新会员姓名
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdatePartRealNameByUid(int uid, string realName)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", uid),
                                       new SqlParameter("@realname",realName)
                                   };
            string commandText = string.Format("UPDATE [{0}userdetails] SET [realname]=@realName WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 更新会员手机
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdatePartUserMobileByUid(int uid, string mobile)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", uid),
                                       new SqlParameter("@mobile",mobile)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [mobile]=@mobile,[verifymobile]=1 WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        #region 直销系统会员

        /// <summary>
        /// 直销会员登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public static int CheckLogin(string userName, string password, string sessionId, int passwordType, int userCodeType, out string errMsg)
        {
            string encryptPwd = SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);
            string loginIp = Common.GetIP();
            string loginAddress = Common.GetAddressByIp(null);

            return VMall.Data.Users.CheckLogin(userName, encryptPwd, sessionId, loginIp, loginAddress, passwordType, userCodeType, out errMsg);
        }

        /// <summary>
        /// 根据用户ID得到用户详细信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static PartUserInfo GetDirSaleUser(int uid)
        {
            return VMall.Data.Users.GetDirSaleUserById(uid);
        }

        #endregion


        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <returns></returns>
        public static string VerifyCodeForRegister()
        {
            //获得用户唯一标示符sid
            string sid = MallUtils.GetSidCookie();
            //当sid为空时
            if (sid == null)
            {
                //生成sid
                sid = Sessions.GenerateSid();
                //将sid保存到cookie中
                MallUtils.SetSidCookie(sid);
            }

            //生成验证值
            string verifyValue = Randoms.CreateRandomValue(4, false).ToLower();
            //将验证值保存到session中
            Sessions.SetItem(sid, "VerifyCodeForRegister", verifyValue);

            return verifyValue;
        }
        /// <summary>
        /// 语音验证码
        /// </summary>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <returns></returns>
        public static string VerifyCodeForRegisterCall()
        {
            //获得用户唯一标示符sid
            string sid = MallUtils.GetSidCookie();
            //当sid为空时
            if (sid == null)
            {
                //生成sid
                sid = Sessions.GenerateSid();
                //将sid保存到cookie中
                MallUtils.SetSidCookie(sid);
            }

            //生成验证值
            string verifyValue = Randoms.CreateRandomValue(4, false).ToLower();
            //将验证值保存到session中
            Sessions.SetItem(sid, "VerifyCodeForRegisterCall", verifyValue);

            return verifyValue;
        }

        #region 推荐网络
        /// <summary>
        /// 获得分享用户列表数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="startAddTime">添加开始时间</param>
        /// <param name="endAddTime">添加结束时间</param>
        /// <param name="orderState">订单状态(0代表全部状态)</param>
        /// <returns></returns>
        public static int GetUserNetCount(PartUserInfo user)
        {
            SqlParameter[] parms = {
                                        new SqlParameter("@pid", user.Uid),  
                                        new SqlParameter("@diruid", user.DirSaleUid)
                                    };
            string commandText = string.Empty;
            if (user.IsDirSaleUser)
            {
                commandText = string.Format(@"SELECT count(*) FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@diruid AND ptype=2  ", RDBSHelper.RDBSTablePre);
            }
            else
            {
                commandText = string.Format(@"SELECT count(*)  FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@pid AND ptype=1 ", RDBSHelper.RDBSTablePre);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText,
                                                                   parms));
        }
        /// <summary>
        /// 根据uid获取推荐网络
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<UserInfo> GetRecommendNetByPid(PartUserInfo user)
        {
            List<UserInfo> subUserList = new List<UserInfo>();

            SqlParameter[] parms = {
                                        new SqlParameter("@pid", user.Uid),  
                                        new SqlParameter("@diruid", user.DirSaleUid)
                                    };
            string commandText = string.Empty;
            //commandText = string.Format(@"SELECT  * FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]={1} AND ptype=1  ORDER BY a.[uid] DESC", RDBSHelper.RDBSTablePre, ParentId);
            if (user.IsDirSaleUser)
                commandText = string.Format(@"SELECT  * FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@diruid AND ptype=2  ORDER BY a.[uid] DESC", RDBSHelper.RDBSTablePre);
            else
                commandText = string.Format(@"SELECT  * FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@pid AND ptype=1  ORDER BY a.[uid] DESC", RDBSHelper.RDBSTablePre);

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    UserInfo partUserInfo = new UserInfo();
                    partUserInfo = VMall.Data.Users.BuildUserFromReader(reader);
                    partUserInfo.ParentLevel = 1;
                    subUserList.Add(partUserInfo);
                }
                reader.Close();
            }
            return subUserList;
        }

        /// <summary>
        /// 根据uid获取代理推荐网络
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<UserInfo> GetAgentRecommendNetByPid(PartUserInfo user)
        {
            List<UserInfo> subUserList = new List<UserInfo>();

            SqlParameter[] parms = {
                                        new SqlParameter("@agentpid", user.Uid),  
                                        new SqlParameter("@diruid", user.DirSaleUid)
                                    };
            string commandText = string.Empty;
            //commandText = string.Format(@"SELECT  * FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]={1} AND ptype=1  ORDER BY a.[uid] DESC", RDBSHelper.RDBSTablePre, ParentId);
            //if (user.IsDirSaleUser)
            //    commandText = string.Format(@"SELECT  * FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [pid]=@diruid AND ptype=2  ORDER BY a.[uid] DESC", RDBSHelper.RDBSTablePre);
            //else
            commandText = string.Format(@"SELECT  * FROM [{0}users] a LEFT JOIN [{0}userdetails] b ON a.uid=b.uid  WHERE [agentpid]=@agentpid AND agentptype=1  ORDER BY a.[uid] DESC", RDBSHelper.RDBSTablePre);

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    UserInfo partUserInfo = new UserInfo();
                    partUserInfo = VMall.Data.Users.BuildUserFromReader(reader);
                    partUserInfo.ParentLevel = 1;
                    subUserList.Add(partUserInfo);
                }
                reader.Close();
            }
            return subUserList;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public static List<UserParentNet> GetChildren(int ParentId)
        {
            List<UserInfo> list = null;
            List<UserParentNet> newList = new List<UserParentNet>();
            PartUserInfo userInfo = Users.GetPartUserById(ParentId);
            list = Users.GetRecommendNetByPid(userInfo);
            foreach (var user in list)
            {
                UserParentNet userNet = new UserParentNet();
                userNet.Uid = user.Uid;
                userNet.UserName = string.IsNullOrEmpty(user.UserName) ? (string.IsNullOrEmpty(user.Email) ? (string.IsNullOrEmpty(user.Mobile) ? "" : user.Mobile) : user.Email) : user.UserName;
                userNet.UserMobile = user.Mobile;
                userNet.NickName = user.NickName;
                userNet.RealName = user.RealName;
                userNet.UserRank = "";
                userNet.AgentRank = Enum.GetName(typeof(AgentTypeEnum), user.AgentType); ;
                userNet.AgentSource = Enum.GetName(typeof(MallSourceType), user.MallSource); 
                userNet.IsDirSaleUser = user.IsDirSaleUser;
                userNet.AgentType = user.AgentType;
                
                userNet.RegisterTime = user.RegisterTime.ToString("yyyy-MM-dd HH:mm");
                userNet.ChildrenCount = Users.GetUserNetCount(user);
                //obj.UserName = (!string.IsNullOrEmpty(obj.UserName) && obj.UserName.Length > 1) ? ("*" + obj.UserName.Substring(obj.UserName.Length - 1)) : "**";
                newList.Add(userNet);
            }

            return newList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public static List<UserParentNet> GetChildrenAgent(int ParentId)
        {
            List<UserInfo> list = null;
            List<UserParentNet> newList = new List<UserParentNet>();
            PartUserInfo userInfo = Users.GetPartUserById(ParentId);
            list = Users.GetAgentRecommendNetByPid(userInfo);
            foreach (var user in list)
            {
                UserParentNet userNet = new UserParentNet();
                userNet.Uid = user.Uid;
                userNet.UserName = string.IsNullOrEmpty(user.UserName) ? (string.IsNullOrEmpty(user.Email) ? (string.IsNullOrEmpty(user.Mobile) ? "" : user.Mobile) : user.Email) : user.UserName;
                userNet.UserMobile = user.Mobile;
                userNet.NickName = user.NickName;
                userNet.RealName = user.RealName;
                userNet.UserRank = user.IsDirSaleUser ? "直销会员" : (user.IsFXUser > 0 ? (user.IsFXUser == 1 ? "分销会员" : "高级分销会员") : "普通会员");
                string agentTitle = string.Empty;
                if (user.AgentType == 5)
                    agentTitle = "汇购优选股东";
                if (user.AgentType == 4)
                    agentTitle = "大区(战略合伙人)";
                if (user.AgentType == 3)
                    agentTitle = "董事(VIP、H3会员)";
                if (user.AgentType == 2)
                    agentTitle = "合伙人(星级、H2会员)";
                if (user.AgentType == 1)
                    agentTitle = "代言人(事业伙伴、H1会员)";
                if (user.AgentType == 0)
                    agentTitle = "无";
                userNet.AgentRank = agentTitle;
                string AgentSource = string.Empty;
                if (user.AgentType > 0)
                {
                        AgentSource = "自营商城";
                    
                }
                userNet.IsDirSaleUser = user.IsDirSaleUser;
                userNet.AgentType = user.AgentType;
                userNet.AgentSource = AgentSource;
                userNet.RegisterTime = user.IsActive == 1 ? user.ActiveTime.ToString("yyyy-MM-dd HH:mm") : "未激活";
                userNet.ChildrenCount = Users.GetUserCountForAgentSystem(user);
                //obj.UserName = (!string.IsNullOrEmpty(obj.UserName) && obj.UserName.Length > 1) ? ("*" + obj.UserName.Substring(obj.UserName.Length - 1)) : "**";
                newList.Add(userNet);
            }

            return newList;
        }
        /// <summary>
        /// 根据pid、ptype获取上级会员uid
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static int GetUidByPidAndPtype(int pid, int pType)
        {
            SqlParameter[] parms = {
                                        new SqlParameter("@pid", pid),  
                                        new SqlParameter("@ptype", pType)
                                    };
            string commandText = string.Empty;

            if (pType == 1)
                commandText = string.Format(@"SELECT  uid FROM [{0}users]  WHERE [uid]=@pid  ", RDBSHelper.RDBSTablePre);
            else
                commandText = string.Format(@"SELECT  uid FROM [{0}users]  WHERE [isdirsaleuser]=1 AND [dirsaleuid]=@pid  ", RDBSHelper.RDBSTablePre);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms));
        }
        /// <summary>
        /// 根据pid、ptype获取代理上级会员agentuid
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static int GetAgentidByPidAndPtype(int agentpid, int pType)
        {
            SqlParameter[] parms = {
                                        new SqlParameter("@agentpid", agentpid),  
                                        new SqlParameter("@ptype", pType)
                                    };
            string commandText = string.Empty;

            if (pType == 1)
                commandText = string.Format(@"SELECT  uid FROM [{0}users]  WHERE [uid]=@agentpid  ", RDBSHelper.RDBSTablePre);
            else
                return 0;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms));
        }

        /// <summary>
        /// 根据pid、ptype获取上级会员
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static PartUserInfo GetParentUserByPidAndPtype(int pid, int pType)
        {
            PartUserInfo info = null;
            SqlParameter[] parms = {
                                        new SqlParameter("@pid", pid),  
                                        new SqlParameter("@ptype", pType)
                                    };
            string commandText = string.Empty;

            if (pType == 1)
                commandText = string.Format(@"SELECT  * FROM [{0}users]  WHERE [uid]=@pid  ", RDBSHelper.RDBSTablePre);
            else
                commandText = string.Format(@"SELECT  * FROM [{0}users]  WHERE [isdirsaleuser]=1 AND [dirsaleuid]=@pid  ", RDBSHelper.RDBSTablePre);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                if (reader.Read())
                {
                    info = VMall.Data.Users.BuildPartUserFromReader(reader);
                }
                reader.Close();
            }
            return info;
        }

        #endregion


        /// <summary>
        /// 更新会员最大汇购卡数
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdateMaxCashCount(PartUserInfo userInfo)
        {

            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", userInfo.Uid),
                                       new SqlParameter("@maxcashcount", userInfo.MaxCashCount)
                                      
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [maxcashcount]=@maxcashcount WHERE  [uid]=@uid",
                                                RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        /// <summary>
        /// 更新会员为微商代理会员标识
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdateAgentUserMark(PartUserInfo userInfo, PartUserInfo parentUser)
        {

            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", userInfo.Uid),
                                       new SqlParameter("@agenttype", userInfo.AgentType),
                                       new SqlParameter("@pid", parentUser.DirSaleUid),
                                      
                                   };
            string commandText = string.Empty;
            if (!userInfo.IsDirSaleUser)//汇购会员升级代理需要往上找最近的代理并挂靠
            {
                if (parentUser.IsDirSaleUser)
                    commandText = string.Format("UPDATE [{0}users] SET [agenttype]=@agenttype,ptype=2,pid=@pid,isnewds=1 WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
                else
                    commandText = string.Format("UPDATE [{0}users] SET [agenttype]=@agenttype,ptype=1,pid={1},isnewds=1 WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre, parentUser.Uid);
            }

            else//直销会员升级代理会员不用变推荐关系，只改代理等级
                if (userInfo.AgentType > 0)
                    commandText = string.Format("UPDATE [{0}users] SET [agenttype]=@agenttype,[ds2agentrank]=0 WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);

            if (!userInfo.IsDirSaleUser)
            {
                if (parentUser.Uid > 0)
                {
                    return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
                }
                return false;
            }
            else
            {
                if (userInfo.AgentType > 0)
                    return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
                return false;
            }

        }
        /// <summary>
        /// 更新微商代理会员下级会员推荐人类型和推荐id
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdateAgentUserForChildren(PartUserInfo userInfo, int dsUid)
        {

            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", userInfo.Uid),
                                       new SqlParameter("@pid", dsUid)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET ptype=2,pid=@pid WHERE  ptype=1 and pid=@uid",
                                                RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 撤销微商代理会员下级会员推荐人类型和推荐id返回为汇购推荐类型
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool BackAgentUserForChildren(PartUserInfo userInfo)
        {

            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", userInfo.Uid),
                                       new SqlParameter("@pid", userInfo.DirSaleUid)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET ptype=1,pid=@uid WHERE  ptype=2 and pid=@pid",
                                                RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        /// <summary>
        /// 更新原直销会员的代理级别，规则//4白金卡=VIP3,3金卡=星级经销商2，2银卡=事业伙伴1
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdateDSUserAGTypeByUid(int uid, int agenttype)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", uid),
                                       new SqlParameter("@agenttype",agenttype)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [agenttype]=@agenttype WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 更新会员的代理级别或代理折扣资格
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdateUserAGTypeOrDs2AgentRank(int uid, int agenttype, int ds2agentrank)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", uid),
                                       new SqlParameter("@agenttype",agenttype),
                                       new SqlParameter("@ds2agentrank",ds2agentrank)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [agenttype]=@agenttype,[ds2agentrank]=@ds2agentrank WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 更新会员身份
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdateUserRank(int uid, int UserRid, int busicenttype)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", uid),
                                       new SqlParameter("@userrid",UserRid),
                                       new SqlParameter("@busicenttype",busicenttype)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [userrid]=@userrid,[busicenttype]=@busicenttype WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 推荐4位合伙人升级董事（星级=>VIP）
        /// </summary>
        public static void UpgradeAgentToVIP(OrderInfo orderInfo)
        {
            try
            {
                PartUserInfo orderUserInfo = Users.GetPartUserById(orderInfo.ActualUid);
                int parentuid = Users.GetUidByPidAndPtype(orderUserInfo.Pid, orderUserInfo.Ptype);//获得父id
                PartUserInfo parentUser = Users.GetPartUserById(parentuid);
                //判断该父id下其他推荐的星级
                int validCount = Users.GetAgentUserCount(parentUser, 2);
                if (validCount >= 4 && parentUser.AgentType == 2)
                {
                    SqlParameter[] parms =  {
                                       new SqlParameter("@uid", parentuid),
                                       new SqlParameter("@agenttype",3)
                                   };
                    string commandText = string.Format("UPDATE [{0}users] SET [agenttype]=@agenttype WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
                    bool flag = RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
                    if (flag)
                        LogHelper.WriteOperateLog("UpgradeAgentToVIPLogs", "推荐4位合伙人升级董事", "成功信息为：订单会员ID：" + orderInfo.ActualUid + ",升级会员ID：" + parentuid + ",订单号：" + orderInfo.OSN + ",升级时间：" + DateTime.Now + "-升级为推荐4位合伙人升级董事（VIP）");
                    else
                        LogHelper.WriteOperateLog("UpgradeAgentToVIPLogs", "推荐4位合伙人升级董事", "失败信息为：订单会员ID：" + orderInfo.ActualUid + ",升级会员ID：" + parentuid + ",订单号：" + orderInfo.OSN + ",升级时间：" + DateTime.Now + "-升级为推荐4位合伙人升级董事（VIP）");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("UpgradeAgentToVIPLogs", "推荐4位合伙人升级董事", "异常信息为：订单会员ID：" + orderInfo.ActualUid + ",订单号：" + orderInfo.OSN + ",异常：" + ex.Message);
            }
        }

        /// <summary>
        /// H1 3个月内推荐20位H1升级H2（H1=>H2）
        /// </summary>
        public static void UpgradeAgentToH2(OrderInfo orderInfo)
        {
            try
            {
                PartUserInfo orderUserInfo = Users.GetPartUserById(orderInfo.ActualUid);
                int parentuid = Users.GetUidByPidAndPtype(orderUserInfo.Pid, orderUserInfo.Ptype);//获得父id
                PartUserInfo parentUser = Users.GetPartUserById(parentuid);
                //判断该父id下其他推荐的星级
                int validCount = Users.GetAgentUserCountInSomeTime(parentUser, 1);
                if (validCount >= 20 && parentUser.AgentType == 1)
                {
                    SqlParameter[] parms =  {
                                       new SqlParameter("@uid", parentuid),
                                       new SqlParameter("@agenttype",2)
                                   };
                    string commandText = string.Format("UPDATE [{0}users] SET [agenttype]=@agenttype WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
                    bool flag = RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
                    if (flag)
                        LogHelper.WriteOperateLog("UpgradeAgentToH2Logs", "推荐20位H1升级H2", "成功信息为：订单会员ID：" + orderInfo.ActualUid + ",升级会员ID：" + parentuid + ",订单号：" + orderInfo.OSN + ",升级时间：" + DateTime.Now + "-升级为推荐20位H1升级H2");
                    else
                        LogHelper.WriteOperateLog("UpgradeAgentToH2Logs", "推荐20位H1升级H2", "失败信息为：订单会员ID：" + orderInfo.ActualUid + ",升级会员ID：" + parentuid + ",订单号：" + orderInfo.OSN + ",升级时间：" + DateTime.Now + "-升级为推荐20位H1升级H2");
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("UpgradeAgentToH2Logs", "推荐20位H1升级H2", "异常信息为：订单会员ID：" + orderInfo.ActualUid + ",订单号：" + orderInfo.OSN + ",异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 更新会员的代理网络
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdateUserAgentRecommer(int uid, int agentpid, int agentptype)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", uid),
                                       new SqlParameter("@agentpid",agentpid),
                                       new SqlParameter("@agentptype",agentptype)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [pid]=@agentpid WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        /// <summary>
        /// 更新会员的推荐网络
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdateUserForPid(int uid, int pid, int ptype)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", uid),
                                       new SqlParameter("@pid",pid),
                                       new SqlParameter("@ptype",ptype)
                                   };
            string commandText = string.Format("UPDATE [{0}users] SET [pid]=@pid,[ptype]=@ptype WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        /// <summary>
        /// 更新会员的银行卡信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool UpdateBankInfo(int uid, string bankName, string bankcardcode, string bankUserName)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid", uid),
                                       new SqlParameter("@bankname",bankName),
                                       new SqlParameter("@bankcardcode",bankcardcode),
                                       new SqlParameter("@bankusername",bankUserName)
                                   };
            string commandText = string.Format("UPDATE [{0}userdetails] SET [bankname]=@bankname,[bankcardcode]=@bankcardcode,[bankusername]=@bankusername WHERE  [uid]=@uid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueUserCode()
        {
            SqlParameter[] parms = {
					new SqlParameter("@UserCode", SqlDbType.NVarChar,20)
            };
            string UserCode = string.Empty;
            parms[0].Value = UserCode;
            parms[0].Direction = ParameterDirection.Output;

            RDBSHelper.ExecuteScalar(CommandType.StoredProcedure, string.Format("GetUniqueUserCode"), parms);
            UserCode = parms[0].Value.ToString();
            if (Users.IsExistUserName(UserCode) || AccountUtils.IsUserExistForDirSale(UserCode))
            {
                UserCode = GetUniqueUserCode();
            }
            return UserCode;
        }

        #region 查找代理会员所有伞下会员
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<UserInfo> GetAllChildrenForAgent(int uid, int LevelCount = 100)
        {
            UserInfo parent = Users.GetUserById(uid);
            PartUserInfo partUser = Users.GetPartUserById(uid);
            List<UserInfo> children = Users.GetAgentRecommendNetByPid(partUser);
            children.Add(parent);//将父级加入到list中 

            GetTreeOrgChart(ref children, LevelCount, 2);//数据拼接成组织层级
            return children;
            
        }

        //参数为整个表树形集合
        public static void GetTreeOrgChart(ref List<UserInfo> list, int LevelCount, int NetType)
        {
            List<UserInfo> itemNode = list.FindAll(t => t.ParentLevel == 0);
            foreach (UserInfo entity in itemNode)
            {
                if (!list.Exists(x => x.Uid == entity.Uid))
                    list.Add(entity);
                //创建子节点
                GetTreeNodeOrgChart(entity.Uid, ref list, entity.Ptype, entity.DirSaleUid, LevelCount, NetType);
            }

            //return list;
        }

        /// <summary>
        /// 创建子节点
        /// </summary>
        /// <param name="parentID">父节点主键</param>
        /// <param name="list">菜单集合</param>
        /// <returns></returns>
        public static void GetTreeNodeOrgChart(int ParentId, ref List<UserInfo> list, int Ptype, int DsParentId, int LevelCount, int NetType)
        {
            List<UserInfo> itemNode = new List<UserInfo>();
            LevelCount--;
            if (NetType == 1)
            {
                if (Ptype == 1)
                    itemNode = list.FindAll(t => t.Pid == ParentId && t.Ptype == 1 && t.ParentLevel == 1);
                if (Ptype == 2)
                    itemNode = list.FindAll(t => t.Pid == DsParentId && t.Ptype == 2 && t.ParentLevel == 1);
            }
            if (NetType == 2)
            {
                itemNode = list.FindAll(t => t.AgentPid == ParentId && t.ParentLevel == 1 && (t.IsDirSaleUser || t.AgentType > 0));
            }
            if (itemNode.Count > 0)
            {
                if (LevelCount + 1 > 0)
                {
                    foreach (UserInfo entity in itemNode)
                    {
                        //创建子节点
                        List<UserInfo> children = new List<UserInfo>();
                        if (NetType == 1)
                            children = Users.GetSubRecommendListByPid(entity, 1, 2000);
                        if (NetType == 2)
                            children = Users.GetAgentRecommendNetByPid(entity);
                        list.AddRange(children);
                        GetTreeNodeOrgChart(entity.Uid, ref children, entity.Ptype, entity.DirSaleUid, LevelCount, NetType);
                    }
                }
            }

        }


        #endregion
    }
    public class UserParentNet
    {
        public int Uid { get; set; }
        public string UserName { get; set; }
        public string UserMobile { get; set; }
        public string NickName { get; set; }
        public string RealName { get; set; }
        public string UserRank { get; set; }
        public string RegisterTime { get; set; }
        public int ChildrenCount { get; set; }
        public int pType { get; set; }
        public int pid { get; set; }
        public int AgentType { get; set; }
        public string AgentRank { get; set; }
        public string AgentSource { get; set; }
        public bool IsDirSaleUser { get; set; }
        public int AgentPid { get; set; }
        public int AgentPType { get; set; }
    }
}
