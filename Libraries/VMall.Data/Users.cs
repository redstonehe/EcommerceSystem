﻿using System;
using System.Data;
using System.Collections.Generic;

using VMall.Core;

namespace VMall.Data
{
    /// <summary>
    /// 用户数据访问类
    /// </summary>
    public partial class Users
    {
        private static IUserNOSQLStrategy _usernosql = BMAData.UserNOSQL;//用户非关系型数据库

        #region 辅助方法

        /// <summary>
        /// 从IDataReader创建PartUserInfo
        /// </summary>
        public static PartUserInfo BuildPartUserFromReader(IDataReader reader)
        {
            PartUserInfo partUserInfo = new PartUserInfo();

            partUserInfo.Uid = TypeHelper.ObjectToInt(reader["uid"]);
            partUserInfo.UserName = reader["username"].ToString();
            partUserInfo.Email = reader["email"].ToString();
            partUserInfo.Mobile = reader["mobile"].ToString();
            partUserInfo.Password = reader["password"].ToString();
            partUserInfo.UserRid = TypeHelper.ObjectToInt(reader["userrid"]);
            partUserInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            partUserInfo.MallAGid = TypeHelper.ObjectToInt(reader["mallagid"]);
            partUserInfo.NickName = reader["nickname"].ToString();
            partUserInfo.Avatar = reader["avatar"].ToString();
            partUserInfo.PayCredits = TypeHelper.ObjectToInt(reader["paycredits"]);
            partUserInfo.RankCredits = TypeHelper.ObjectToInt(reader["rankcredits"]);
            partUserInfo.VerifyEmail = TypeHelper.ObjectToInt(reader["verifyemail"]);
            partUserInfo.VerifyMobile = TypeHelper.ObjectToInt(reader["verifymobile"]);
            partUserInfo.LiftBanTime = TypeHelper.ObjectToDateTime(reader["liftbantime"]);
            partUserInfo.Salt = reader["salt"].ToString();
            partUserInfo.Pid = TypeHelper.ObjectToInt(reader["pid"]);
            partUserInfo.Ptype = TypeHelper.ObjectToInt(reader["ptype"]);
            partUserInfo.IsDirSaleUser = Convert.ToBoolean(reader["isdirsaleuser"]);
            partUserInfo.DirSaleUid = TypeHelper.ObjectToInt(reader["dirsaleuid"]);
            partUserInfo.IsFXUser = TypeHelper.ObjectToInt(reader["isfxuser"]);
            partUserInfo.DirSalePwd = reader["dirsalepwd"].ToString();
            partUserInfo.OtherLoginId = reader["otherloginid"].ToString();
            partUserInfo.DirSalePwd = reader["dirsalepwd"].ToString();
            partUserInfo.PayPassword = reader["paypassword"].ToString();
            partUserInfo.MaxCashCount = TypeHelper.ObjectToInt(reader["maxcashcount"]);
            partUserInfo.AgentType = TypeHelper.ObjectToInt(reader["agenttype"]);
            partUserInfo.UserSource = TypeHelper.ObjectToInt(reader["usersource"]);
            partUserInfo.Ds2AgentRank = TypeHelper.ObjectToInt(reader["ds2agentrank"]);
            partUserInfo.AgentPid = TypeHelper.ObjectToInt(reader["agentpid"]);
            partUserInfo.AgentPType = TypeHelper.ObjectToInt(reader["agentptype"]);
            partUserInfo.BusiCentType = TypeHelper.ObjectToInt(reader["busicenttype"]);
            partUserInfo.IsActive = TypeHelper.ObjectToInt(reader["isactive"]);
            partUserInfo.ActiveTime = TypeHelper.ObjectToDateTime(reader["activetime"]);
            partUserInfo.MallSource = TypeHelper.ObjectToInt(reader["mallsource"]);
            return partUserInfo;
        }

        /// <summary>
        /// 从IDataReader创建UserInfo
        /// </summary>
        public static UserInfo BuildUserFromReader(IDataReader reader)
        {
            UserInfo userInfo = new UserInfo();

            userInfo.Uid = TypeHelper.ObjectToInt(reader["uid"]);
            userInfo.UserName = reader["username"].ToString();
            userInfo.Email = reader["email"].ToString();
            userInfo.Mobile = reader["mobile"].ToString();
            userInfo.Password = reader["password"].ToString();
            userInfo.UserRid = TypeHelper.ObjectToInt(reader["userrid"]);
            userInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            userInfo.MallAGid = TypeHelper.ObjectToInt(reader["mallagid"]);
            userInfo.NickName = reader["nickname"].ToString();
            userInfo.Avatar = reader["avatar"].ToString();
            userInfo.PayCredits = TypeHelper.ObjectToInt(reader["paycredits"]);
            userInfo.RankCredits = TypeHelper.ObjectToInt(reader["rankcredits"]);
            userInfo.VerifyEmail = TypeHelper.ObjectToInt(reader["verifyemail"]);
            userInfo.VerifyMobile = TypeHelper.ObjectToInt(reader["verifymobile"]);
            userInfo.LiftBanTime = TypeHelper.ObjectToDateTime(reader["liftbantime"]);
            userInfo.Salt = reader["salt"].ToString();
            userInfo.Pid = TypeHelper.ObjectToInt(reader["pid"]);
            userInfo.Ptype = TypeHelper.ObjectToInt(reader["ptype"]);
            userInfo.IsDirSaleUser = Convert.ToBoolean(reader["isdirsaleuser"]);
            userInfo.DirSaleUid = TypeHelper.ObjectToInt(reader["dirsaleuid"]);
            userInfo.IsFXUser = TypeHelper.ObjectToInt(reader["isfxuser"]);
            userInfo.DirSalePwd = reader["dirsalepwd"].ToString();
            userInfo.PayPassword = reader["paypassword"].ToString();
            userInfo.MaxCashCount = TypeHelper.ObjectToInt(reader["maxcashcount"]);
            userInfo.AgentType = TypeHelper.ObjectToInt(reader["agenttype"]);
            userInfo.UserSource = TypeHelper.ObjectToInt(reader["usersource"]);
            userInfo.Ds2AgentRank = TypeHelper.ObjectToInt(reader["ds2agentrank"]);
            userInfo.AgentPid = TypeHelper.ObjectToInt(reader["agentpid"]);
            userInfo.AgentPType = TypeHelper.ObjectToInt(reader["agentptype"]);
            userInfo.MallSource = TypeHelper.ObjectToInt(reader["mallsource"]);

            userInfo.LastVisitTime = TypeHelper.ObjectToDateTime(reader["lastvisittime"]);
            userInfo.LastVisitIP = reader["lastvisitip"].ToString();
            userInfo.LastVisitRgId = TypeHelper.ObjectToInt(reader["lastvisitrgid"]);
            userInfo.RegisterTime = TypeHelper.ObjectToDateTime(reader["registertime"]);
            userInfo.RegisterIP = reader["registerip"].ToString();
            userInfo.RegisterRgId = TypeHelper.ObjectToInt(reader["registerrgid"]);
            userInfo.Gender = TypeHelper.ObjectToInt(reader["gender"]);
            userInfo.RealName = reader["realname"].ToString();
            userInfo.Bday = TypeHelper.ObjectToDateTime(reader["bday"]);
            userInfo.IdCard = reader["idcard"].ToString();
            userInfo.RegionId = TypeHelper.ObjectToInt(reader["regionid"]);
            userInfo.Address = reader["address"].ToString();
            userInfo.Bio = reader["bio"].ToString();
            userInfo.BankName = reader["bankname"].ToString();
            userInfo.BankCardCode = reader["bankcardcode"].ToString();
            userInfo.BankUserName = reader["bankusername"].ToString();

            userInfo.BusiCentType = TypeHelper.ObjectToInt(reader["busicenttype"]);
            userInfo.IsActive = TypeHelper.ObjectToInt(reader["isactive"]);
            userInfo.ActiveTime = TypeHelper.ObjectToDateTime(reader["activetime"]);
            return userInfo;
        }

        /// <summary>
        /// 从IDataReader创建UserDetailInfo
        /// </summary>
        public static UserDetailInfo BuildUserDetailFromReader(IDataReader reader)
        {
            UserDetailInfo userDetailInfo = new UserDetailInfo();

            userDetailInfo.Uid = TypeHelper.ObjectToInt(reader["uid"]);
            userDetailInfo.LastVisitTime = TypeHelper.ObjectToDateTime(reader["lastvisittime"]);
            userDetailInfo.LastVisitIP = reader["lastvisitip"].ToString();
            userDetailInfo.LastVisitRgId = TypeHelper.ObjectToInt(reader["lastvisitrgid"]);
            userDetailInfo.RegisterTime = TypeHelper.ObjectToDateTime(reader["registertime"]);
            userDetailInfo.RegisterIP = reader["registerip"].ToString();
            userDetailInfo.RegisterRgId = TypeHelper.ObjectToInt(reader["registerrgid"]);
            userDetailInfo.Gender = TypeHelper.ObjectToInt(reader["gender"]);
            userDetailInfo.RealName = reader["realname"].ToString();
            userDetailInfo.Bday = TypeHelper.ObjectToDateTime(reader["bday"]);
            userDetailInfo.IdCard = reader["idcard"].ToString();
            userDetailInfo.RegionId = TypeHelper.ObjectToInt(reader["regionid"]);
            userDetailInfo.Address = reader["address"].ToString();
            userDetailInfo.Bio = reader["bio"].ToString();
            userDetailInfo.BankName = reader["bankname"].ToString();
            userDetailInfo.BankCardCode = reader["bankcardcode"].ToString();
            userDetailInfo.BankUserName = reader["bankusername"].ToString();
            return userDetailInfo;
        }

        #endregion

        /// <summary>
        /// 获得用户
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static UserInfo GetUserById(int uid)
        {
            UserInfo userInfo = null;

            IDataReader reader = VMall.Core.BMAData.RDBS.GetUserById(uid);
            if (reader.Read())
            {
                userInfo = BuildUserFromReader(reader);
            }
            reader.Close();
            return userInfo;
        }

        /// <summary>
        /// 获得部分用户
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static PartUserInfo GetPartUserById(int uid)
        {
            PartUserInfo partUserInfo = null;

            if (_usernosql != null)
            {
                partUserInfo = _usernosql.GetPartUserById(uid);
                if (partUserInfo == null)
                {
                    IDataReader reader = VMall.Core.BMAData.RDBS.GetPartUserById(uid);
                    if (reader.Read())
                    {
                        partUserInfo = BuildPartUserFromReader(reader);
                    }
                    reader.Close();
                    if (partUserInfo != null)
                        _usernosql.CreatePartUser(partUserInfo);
                }
            }
            else
            {
                IDataReader reader = VMall.Core.BMAData.RDBS.GetPartUserById(uid);
                if (reader.Read())
                {
                    partUserInfo = BuildPartUserFromReader(reader);
                }
                reader.Close();
            }

            return partUserInfo;
        }

        /// <summary>
        /// 获得用户细节
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static UserDetailInfo GetUserDetailById(int uid)
        {
            UserDetailInfo userDetailInfo = null;

            if (_usernosql != null)
            {
                userDetailInfo = _usernosql.GetUserDetailById(uid);
                if (userDetailInfo == null)
                {
                    IDataReader reader = VMall.Core.BMAData.RDBS.GetUserDetailById(uid);
                    if (reader.Read())
                    {
                        userDetailInfo = BuildUserDetailFromReader(reader);
                    }
                    reader.Close();
                    if (userDetailInfo != null)
                        _usernosql.CreateUserDetail(userDetailInfo);
                }
            }
            else
            {
                IDataReader reader = VMall.Core.BMAData.RDBS.GetUserDetailById(uid);
                if (reader.Read())
                {
                    userDetailInfo = BuildUserDetailFromReader(reader);
                }
                reader.Close();
            }

            return userDetailInfo;
        }

        /// <summary>
        /// 获得部分用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public static PartUserInfo GetPartUserByName(string userName)
        {
            PartUserInfo partUserInfo = null;

            IDataReader reader = VMall.Core.BMAData.RDBS.GetPartUserByName(userName);
            if (reader.Read())
            {
                partUserInfo = BuildPartUserFromReader(reader);
            }
            reader.Close();
            return partUserInfo;
        }

        /// <summary>
        /// 获得部分用户
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <returns></returns>
        public static PartUserInfo GetPartUserByEmail(string email)
        {
            PartUserInfo partUserInfo = null;

            IDataReader reader = VMall.Core.BMAData.RDBS.GetPartUserByEmail(email);
            if (reader.Read())
            {
                partUserInfo = BuildPartUserFromReader(reader);
            }
            reader.Close();
            return partUserInfo;
        }

        /// <summary>
        /// 获得部分用户
        /// </summary>
        /// <param name="mobile">用户手机</param>
        /// <returns></returns>
        public static PartUserInfo GetPartUserByMobile(string mobile)
        {
            PartUserInfo partUserInfo = null;

            IDataReader reader = VMall.Core.BMAData.RDBS.GetPartUserByMobile(mobile);
            if (reader.Read())
            {
                partUserInfo = BuildPartUserFromReader(reader);
            }
            reader.Close();
            return partUserInfo;
        }

        /// <summary>
        /// 获得用户id
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public static int GetUidByUserName(string userName)
        {
            return VMall.Core.BMAData.RDBS.GetUidByUserName(userName);
        }

        /// <summary>
        /// 获得用户id
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <returns></returns>
        public static int GetUidByEmail(string email)
        {
            return VMall.Core.BMAData.RDBS.GetUidByEmail(email);
        }

        /// <summary>
        /// 获得用户id
        /// </summary>
        /// <param name="mobile">用户手机</param>
        /// <returns></returns>
        public static int GetUidByMobile(string mobile)
        {
            return VMall.Core.BMAData.RDBS.GetUidByMobile(mobile);
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        public static int CreateUser(UserInfo userInfo)
        {
            return VMall.Core.BMAData.RDBS.CreateUser(userInfo);
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <returns></returns>
        public static void UpdateUser(UserInfo userInfo)
        {
            VMall.Core.BMAData.RDBS.UpdateUser(userInfo);
            if (_usernosql != null)
                _usernosql.UpdateUser(userInfo);
        }

        /// <summary>
        /// 更新部分用户
        /// </summary>
        /// <returns></returns>
        public static void UpdatePartUser(PartUserInfo partUserInfo)
        {
            VMall.Core.BMAData.RDBS.UpdatePartUser(partUserInfo);
            if (_usernosql != null)
                _usernosql.UpdatePartUser(partUserInfo);
        }

        /// <summary>
        /// 更新用户细节
        /// </summary>
        /// <returns></returns>
        public static void UpdateUserDetail(UserDetailInfo userDetailInfo)
        {
            VMall.Core.BMAData.RDBS.UpdateUserDetail(userDetailInfo);
            if (_usernosql != null)
                _usernosql.UpdateUserDetail(userDetailInfo);
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
            VMall.Core.BMAData.RDBS.UpdateUserLastVisit(uid, visitTime, ip, regionId);
            if (_usernosql != null)
                _usernosql.UpdateUserLastVisit(uid, visitTime, ip, regionId);
        }

        /// <summary>
        /// 后台获得用户列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public static DataTable AdminGetUserList(int pageSize, int pageNumber, string condition, string sort, string realName = "")
        {
            return VMall.Core.BMAData.RDBS.AdminGetUserList(pageSize, pageNumber, condition, sort, realName);
        }

        /// <summary>
        /// 后台获得用户列表条件
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="email">邮箱</param>
        /// <param name="mobile">手机</param>
        /// <param name="userRid">用户等级</param>
        /// <param name="mallAGid">商城管理员组</param>
        /// <returns></returns>
        public static string AdminGetUserListCondition(string userName, string email, string mobile, int userRid, int mallAGid, int userType = -1)
        {
            return VMall.Core.BMAData.RDBS.AdminGetUserListCondition(userName, email, mobile, userRid, mallAGid, userType);
        }

        /// <summary>
        /// 后台获得用户列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static string AdminGetUserListSort(string sortColumn, string sortDirection)
        {
            return VMall.Core.BMAData.RDBS.AdminGetUserListSort(sortColumn, sortDirection);
        }

        /// <summary>
        /// 后台获得用户列表数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetUserCount(string condition, string realName = "")
        {
            return VMall.Core.BMAData.RDBS.AdminGetUserCount(condition, realName);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="uidList">用户id</param>
        public static void DeleteUserById(string uidList)
        {
            VMall.Core.BMAData.RDBS.DeleteUserById(uidList);
            if (_usernosql != null)
            {
                List<OAuthInfo> oauthList = OAuths.GetOAuthUserList(uidList);
                foreach (OAuthInfo oauthInfo in oauthList)
                    _usernosql.DeleteUidByOpenIdAndServer(oauthInfo.OpenId, oauthInfo.Server);
                _usernosql.DeleteUserById(uidList);
            }
        }

        /// <summary>
        /// 获得用户等级下用户的数量
        /// </summary>
        /// <param name="userRid">用户等级id</param>
        /// <returns></returns>
        public static int GetUserCountByUserRid(int userRid)
        {
            return VMall.Core.BMAData.RDBS.GetUserCountByUserRid(userRid);
        }

        /// <summary>
        /// 获得商城管理员组下用户的数量
        /// </summary>
        /// <param name="mallAGid">商城管理员组id</param>
        /// <returns></returns>
        public static int GetUserCountByMallAGid(int mallAGid)
        {
            return VMall.Core.BMAData.RDBS.GetUserCountByMallAGid(mallAGid);
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
            bool result = VMall.Core.BMAData.RDBS.UpdateUser(uid, userName, nickName, avatar, gender, realName, bday, idCard, regionId, address, bio, bankName, bankCardCode, bankUserName);
            if (_usernosql != null)
                _usernosql.UpdateUser(uid, userName, nickName, avatar, gender, realName, bday, idCard, regionId, address, bio, bankName, bankCardCode, bankUserName);
            return result;
        }

        /// <summary>
        /// 更新用户邮箱
        /// </summary>
        /// <param name="uid">用户id.</param>
        /// <param name="email">邮箱</param>
        public static void UpdateUserEmailByUid(int uid, string email)
        {
            VMall.Core.BMAData.RDBS.UpdateUserEmailByUid(uid, email);
            if (_usernosql != null)
                _usernosql.UpdateUserEmailByUid(uid, email);
        }

        /// <summary>
        /// 更新用户手机
        /// </summary>
        /// <param name="uid">用户id.</param>
        /// <param name="mobile">手机</param>
        public static void UpdateUserMobileByUid(int uid, string mobile)
        {
            VMall.Core.BMAData.RDBS.UpdateUserMobileByUid(uid, mobile);
            if (_usernosql != null)
                _usernosql.UpdateUserMobileByUid(uid, mobile);
        }

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="uid">用户id.</param>
        /// <param name="password">密码</param>
        public static void UpdateUserPasswordByUid(int uid, string password)
        {
            VMall.Core.BMAData.RDBS.UpdateUserPasswordByUid(uid, password);
            if (_usernosql != null)
                _usernosql.UpdateUserPasswordByUid(uid, password);
        }

        /// <summary>
        /// 更新用户解禁时间
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="liftBanTime">解禁时间</param>
        public static void UpdateUserLiftBanTimeByUid(int uid, DateTime liftBanTime)
        {
            VMall.Core.BMAData.RDBS.UpdateUserLiftBanTimeByUid(uid, liftBanTime);
            if (_usernosql != null)
                _usernosql.UpdateUserLiftBanTimeByUid(uid, liftBanTime);
        }

        /// <summary>
        /// 更新用户等级
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="userRid">用户等级id</param>
        public static void UpdateUserRankByUid(int uid, int userRid)
        {
            VMall.Core.BMAData.RDBS.UpdateUserRankByUid(uid, userRid);
            if (_usernosql != null)
                _usernosql.UpdateUserRankByUid(uid, userRid);
        }

        /// <summary>
        /// 更新用户在线时间
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="onlineTime">在线时间</param>
        /// <param name="updateTime">更新时间</param>
        public static void UpdateUserOnlineTime(int uid, int onlineTime, DateTime updateTime)
        {
            VMall.Core.BMAData.RDBS.UpdateUserOnlineTime(uid, onlineTime, updateTime);
        }

        /// <summary>
        /// 通过注册ip获得注册时间
        /// </summary>
        /// <param name="registerIP">注册ip</param>
        /// <returns></returns>
        public static DateTime GetRegisterTimeByRegisterIP(string registerIP)
        {
            return VMall.Core.BMAData.RDBS.GetRegisterTimeByRegisterIP(registerIP);
        }

        /// <summary>
        /// 获得用户最后访问时间
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static DateTime GetUserLastVisitTimeByUid(int uid)
        {
            if (_usernosql != null)
            {
                UserDetailInfo userDetailInfo = GetUserDetailById(uid);
                if (userDetailInfo != null)
                    return userDetailInfo.LastVisitTime;
                else
                    return DateTime.Now;
            }
            else
            {
                return VMall.Core.BMAData.RDBS.GetUserLastVisitTimeByUid(uid);
            }
        }

        /// <summary>
        /// 设置店铺管理员
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="storeId">店铺id</param>
        public static void SetStoreAdminer(int uid, int storeId)
        {
            if (_usernosql != null)
            {
                int oldStoreAdminerId = GetStoreAdminerIdByStoreId(storeId);
                if (oldStoreAdminerId > 0)
                    _usernosql.SetStoreAdminer(oldStoreAdminerId, 0);
                _usernosql.SetStoreAdminer(uid, storeId);
            }
            VMall.Core.BMAData.RDBS.SetStoreAdminer(uid, storeId);
        }

        /// <summary>
        /// 获得店铺管理员id
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static int GetStoreAdminerIdByStoreId(int storeId)
        {
            return VMall.Core.BMAData.RDBS.GetStoreAdminerIdByStoreId(storeId);
        }

        #region 直销系统会员

        /// <summary>
        /// 直销会员登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sessionId"></param>
        /// <param name="loginIp"></param>
        /// <param name="loginAddress"></param>
        /// <param name="passwordType"></param>
        /// <param name="userCodeType"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static int CheckLogin(string userName, string password, string sessionId, string loginIp, string loginAddress, int passwordType, int userCodeType, out string errMsg)
        {
            DataTable dt = VMall.Core.BMAData.RDBS.CheckLogin(userName, password, sessionId, loginIp, loginAddress, passwordType, userCodeType);

            errMsg = "";
            int status = dt.Rows[0]["Status"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["Status"]);
            if (status > 0)
            {
                return dt.Rows[0]["UserId"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["UserId"]);
            }
            else
            {
                switch (status)
                {
                    case -4:
                        errMsg = "当前用户已经被锁定";
                        break;
                    case -3:
                        errMsg = "当前用户未激活";
                        break;
                    case -2:
                        errMsg = "密码错误次数过多被锁定,请联系管理员开通";
                        break;
                    case -1:
                        errMsg = "用户不存在";
                        break;
                    case -0:
                        int lockCount = dt.Rows[0]["LockPasswordAttemptCount"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["LockPasswordAttemptCount"]);
                        int attemptCount = dt.Rows[0]["PasswordAttemptCount"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["PasswordAttemptCount"]);
                        errMsg = string.Format("密码输入错误，您还有{0}次机会", lockCount - attemptCount - 1);
                        break;
                }
            }
            return 0;
        }

        /// <summary>
        /// 获得直销系统用户信息
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static PartUserInfo GetDirSaleUserById(int uid)
        {
            PartUserInfo partUserInfo = null;

            IDataReader reader = VMall.Core.BMAData.RDBS.GetDirSaleUserById(uid);
            if (reader.Read())
            {
                partUserInfo = BuildDirSaleUserFromReader(reader);
            }
            reader.Close();

            return partUserInfo;
        }

        /// <summary>
        /// 从IDataReader创建PartUserInfo
        /// </summary>
        public static PartUserInfo BuildDirSaleUserFromReader(IDataReader reader)
        {
            PartUserInfo partUserInfo = new PartUserInfo();

            partUserInfo.Uid = TypeHelper.ObjectToInt(reader["UserId"]);
            partUserInfo.UserName = reader["UserCode"].ToString();
            partUserInfo.Email = reader["UserEmail"].ToString();
            partUserInfo.Mobile = reader["UserPhone"].ToString();
            partUserInfo.Password = reader["Password1"].ToString();
            partUserInfo.UserRid = 7;
            partUserInfo.StoreId = 0;
            partUserInfo.MallAGid = 1;
            partUserInfo.NickName = reader["NickName"].ToString();
            //partUserInfo.Avatar = reader["avatar"].ToString();
            //partUserInfo.PayCredits = TypeHelper.ObjectToInt(reader["paycredits"]);
            //partUserInfo.RankCredits = TypeHelper.ObjectToInt(reader["rankcredits"]);
            //partUserInfo.VerifyEmail = TypeHelper.ObjectToInt(reader["verifyemail"]);
            //partUserInfo.VerifyMobile = TypeHelper.ObjectToInt(reader["verifymobile"]);
            //partUserInfo.LiftBanTime = TypeHelper.ObjectToDateTime(reader["liftbantime"]);
            //partUserInfo.Salt = reader["salt"].ToString();
            partUserInfo.IsDirSaleUser = true;

            return partUserInfo;
        }

        #endregion
    }
}
