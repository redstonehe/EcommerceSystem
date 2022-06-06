using System;
using System.Data;
using System.Collections.Generic;

using VMall.Core;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;

namespace VMall.Services
{
    /// <summary>
    /// 优惠劵操作管理类
    /// </summary>
    public partial class Coupons
    {
        /// <summary>
        /// 获得优惠劵类型
        /// </summary>
        /// <param name="couponTypeId">优惠劵类型id</param>
        /// <returns></returns>
        public static CouponTypeInfo GetCouponTypeById(int couponTypeId)
        {
            CouponTypeInfo couponTypeInfo = VMall.Core.BMACache.Get(CacheKeys.MALL_COUPONTYPE_INFO + couponTypeId) as CouponTypeInfo;
            if (couponTypeInfo == null)
            {
                couponTypeInfo = VMall.Data.Coupons.GetCouponTypeById(couponTypeId);
                VMall.Core.BMACache.Insert(CacheKeys.MALL_COUPONTYPE_INFO + couponTypeId, couponTypeInfo);
            }

            return couponTypeInfo;
        }

        /// <summary>
        /// 获得当前正在发放的活动优惠劵类型列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <returns></returns>
        public static List<CouponTypeInfo> GetSendingPromotionCouponTypeList(int storeId)
        {
            return VMall.Data.Coupons.GetSendingPromotionCouponTypeList(storeId);
        }

        /// <summary>
        /// 获得当前正在发放的优惠劵类型列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static List<CouponTypeInfo> GetSendingCouponTypeList(int storeId, DateTime nowTime)
        {
            return VMall.Data.Coupons.GetSendingCouponTypeList(storeId, nowTime);
        }
        /// <summary>
        /// 获得当前正在发放的优惠劵类型列表
        /// </summary>

        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static List<CouponTypeInfo> GetSendingCouponTypeList()
        {
            return GetCouponTypeListByWhere(" [sendmode]=0 AND [sendstarttime]<=GETDATE() AND [sendendtime]>GETDATE() AND [state]=1 ");
        }
        /// <summary>
        /// 获得当前正在使用的优惠劵类型列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="nowTime">当前时间</param>
        /// <returns></returns>
        public static List<CouponTypeInfo> GetUsingCouponTypeList(int storeId, DateTime nowTime)
        {
            return VMall.Data.Coupons.GetUsingCouponTypeList(storeId, nowTime);
        }


        /// <summary>
        /// 根据条件获取优惠券列表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static List<CouponTypeInfo> GetCouponTypeListByWhere(string condition)
        {
            List<CouponTypeInfo> couponTypeList = new List<CouponTypeInfo>();
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" where ");
                sb.Append(condition);
            }
            string commandText = "select * from hlh_coupontypes  " + sb.ToString();
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    CouponTypeInfo couponType = VMall.Data.Coupons.BuildCouponTypeFromReader(reader);
                    couponTypeList.Add(couponType);
                }
                reader.Close();
            }
            return couponTypeList;
        }


        /// <summary>
        /// 生成优惠劵编号
        /// </summary>
        /// <returns></returns>
        public static string GenerateCouponSN()
        {
            string sn = Randoms.CreateRandomValue(16);
            while (IsExistCouponSN(sn))
                sn = Randoms.CreateRandomValue(16);
            return sn;
        }

        /// <summary>
        /// 创建优惠劵
        /// </summary>
        /// <param name="couponInfo">优惠劵信息</param>
        public static void CreateCoupon(CouponInfo couponInfo)
        {
            VMall.Data.Coupons.CreateCoupon(couponInfo);
        }

        /// <summary>
        /// 删除优惠劵
        /// </summary>
        /// <param name="idList">id列表</param>
        public static void DeleteCouponById(int[] idList)
        {
            if (idList != null && idList.Length > 0)
                VMall.Data.Coupons.DeleteCouponById(CommonHelper.IntArrayToString(idList));
        }

        /// <summary>
        /// 删除优惠劵
        /// </summary>
        /// <param name="idList">id列表</param>
        public static void DeleteCouponById(string idList)
        {
            VMall.Data.Coupons.DeleteCouponById(idList);
        }

        /// <summary>
        /// 判断优惠劵编号是否存在
        /// </summary>
        /// <param name="couponSN">优惠劵编号</param>
        /// <returns></returns>
        public static bool IsExistCouponSN(string couponSN)
        {
            return VMall.Data.Coupons.IsExistCouponSN(couponSN);
        }

        /// <summary>
        /// 获得发放的优惠劵数量
        /// </summary>
        /// <param name="couponTypeId">优惠劵类型id</param>
        /// <returns></returns>
        public static int GetSendCouponCount(int couponTypeId)
        {
            int sendCount = TypeHelper.ObjectToInt(VMall.Core.BMACache.Get(CacheKeys.MALL_COUPONTYPE_SENDCOUNT + couponTypeId), -1);
            if (sendCount < 0)
            {
                sendCount = VMall.Data.Coupons.GetSendCouponCount(couponTypeId);
                VMall.Core.BMACache.Insert(CacheKeys.MALL_COUPONTYPE_SENDCOUNT + couponTypeId, sendCount);
            }

            return sendCount;
        }

        /// <summary>
        /// 获得发放给用户的优惠劵数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="couponTypeId">优惠劵类型id</param>
        /// <returns></returns>
        public static int GetSendUserCouponCount(int uid, int couponTypeId)
        {
            return VMall.Data.Coupons.GetSendUserCouponCount(uid, couponTypeId);
        }

        /// <summary>
        /// 获得今天用户发放的优惠劵数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="couponTypeId">优惠劵类型id</param>
        /// <param name="today">今天日期</param>
        /// <returns></returns>
        public static int GetTodaySendUserCouponCount(int uid, int couponTypeId, DateTime today)
        {
            return VMall.Data.Coupons.GetTodaySendUserCouponCount(uid, couponTypeId, today);
        }

        /// <summary>
        /// 获得优惠劵列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="type">类型(0代表全部，1代表未使用，2代表已使用，3代表已过期)</param>
        /// <returns></returns>
        public static DataTable GetCouponList(int uid, int type)
        {
            switch (type)
            {
                case 0:
                    return GetAllCouponList(uid);
                case 1:
                    return GetUnUsedCouponList(uid);
                case 2:
                    return GetUsedCouponList(uid);
                case 3:
                    return GetExpiredCouponList(uid);
                default:
                    return GetAllCouponList(uid);
            }
        }

        /// <summary>
        /// 获得全部优惠劵列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static DataTable GetAllCouponList(int uid)
        {
            return VMall.Data.Coupons.GetCouponList(uid, 0);
        }

        /// <summary>
        /// 获得未使用的优惠劵列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static DataTable GetUnUsedCouponList(int uid)
        {
            DataTable dt = VMall.Data.Coupons.GetCouponList(uid, 1);
            if (dt.Rows.Count < 1)
                return dt;

            DataTable couponList = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                //int useExpireTime = TypeHelper.ObjectToInt(row["useexpiretime"]);
                //if (useExpireTime > 0)
                //{
                //    if (TypeHelper.ObjectToDateTime(row["activatetime"]).AddDays(useExpireTime) > DateTime.Now)
                //        couponList.Rows.Add(row.ItemArray);
                //}
                //else
                //{
                couponList.Rows.Add(row.ItemArray);
                //}
            }
            return couponList;
        }

        /// <summary>
        /// 获得已使用的优惠劵列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static DataTable GetUsedCouponList(int uid)
        {
            return VMall.Data.Coupons.GetCouponList(uid, 2);
        }

        /// <summary>
        /// 获得已过期的优惠劵列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static DataTable GetExpiredCouponList(int uid)
        {
            DataTable dt = VMall.Data.Coupons.GetCouponList(uid, 3);
            if (dt.Rows.Count < 1)
                return dt;

            DataTable couponList = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                //int useExpireTime = TypeHelper.ObjectToInt(row["useexpiretime"]);
                //if (useExpireTime > 0)
                //{
                //    if (TypeHelper.ObjectToDateTime(row["activatetime"]).AddDays(useExpireTime) <= DateTime.Now)
                //        couponList.Rows.Add(row.ItemArray);
                //}
                //else
                //{
                couponList.Rows.Add(row.ItemArray);
                //}
            }
            return couponList;
        }

        /// <summary>
        /// 获得优惠劵
        /// </summary>
        /// <param name="couponId">优惠劵id</param>
        /// <returns></returns>
        public static CouponInfo GetCouponByCouponId(int couponId)
        {
            return VMall.Data.Coupons.GetCouponByCouponId(couponId);
        }

        /// <summary>
        /// 获得优惠劵
        /// </summary>
        /// <param name="couponSN">优惠劵编号</param>
        /// <returns></returns>
        public static CouponInfo GetCouponByCouponSN(string couponSN)
        {
            return VMall.Data.Coupons.GetCouponByCouponSN(couponSN);
        }


        /// <summary>
        /// 根据条件获取优惠券列表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static CouponInfo GetCouponBywhere(string condition)
        {
            string commandText = "select * from hlh_coupons  where " + condition;

            CouponInfo couponInfo = null;
            IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText);
            if (reader.Read())
            {
                couponInfo =  VMall.Data.Coupons.BuildCouponFromReader(reader);
            }
            reader.Close();
            return couponInfo;
            
        }
        /// <summary>
        /// 合计优惠劵金额
        /// </summary>
        /// <param name="couponList">优惠劵列表</param>
        /// <returns></returns>
        public static decimal SumCouponMoney(List<CouponInfo> couponList)
        {
            if (couponList.Count < 1)
                return 0;

            int money = 0;
            foreach (CouponInfo couponInfo in couponList)
            {
                money += couponInfo.Money;
            }
            return money;
        }

        /// <summary>
        /// 领取优惠劵
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="couponTypeInfo">优惠劵类型信息</param>
        /// <param name="pullTime">领取时间</param>
        /// <param name="pullIP">领取ip</param>
        /// <returns></returns>
        public static string PullCoupon(PartUserInfo partUserInfo, CouponTypeInfo couponTypeInfo, DateTime pullTime, string pullIP)
        {
            int sendCount = GetSendCouponCount(couponTypeInfo.CouponTypeId);
            VMall.Core.BMACache.Insert(CacheKeys.MALL_COUPONTYPE_SENDCOUNT + couponTypeInfo.CouponTypeId, sendCount + 1);

            string couponSN = GenerateCouponSN();

            CouponInfo couponInfo = new CouponInfo();

            couponInfo.CouponSN = couponSN;
            couponInfo.Uid = partUserInfo.Uid;
            couponInfo.CouponTypeId = couponTypeInfo.CouponTypeId;
            couponInfo.StoreId = couponTypeInfo.StoreId;
            couponInfo.Oid = 0;
            couponInfo.UseTime = new DateTime(1900, 1, 1);
            couponInfo.UseIP = "";
            couponInfo.Money = couponTypeInfo.Money;
            couponInfo.ActivateTime = couponTypeInfo.UseStartTime > DateTime.Now ? couponTypeInfo.UseStartTime : DateTime.Now;
            couponInfo.ActivateIP = pullIP;
            couponInfo.CreateUid = partUserInfo.Uid;
            couponInfo.CreateOid = 0;
            couponInfo.CreateTime = DateTime.Now;
            couponInfo.CreateIP = pullIP;
            couponInfo.ValidTime = couponTypeInfo.UseExpireTime > 0 ? DateTime.Now.AddDays(couponTypeInfo.UseExpireTime) : couponTypeInfo.UseEndTime;
            CreateCoupon(couponInfo);

            return couponSN;
        }

        /// <summary>
        /// 发放单品促销活动优惠劵
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="couponTypeId">优惠劵类型id</param>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="ip">ip</param>
        public static void SendSinglePromotionCoupon(PartUserInfo partUserInfo, int couponTypeId, OrderInfo orderInfo, string ip)
        {
            CouponTypeInfo couponTypeInfo = GetCouponTypeById(couponTypeId);
            if (couponTypeInfo == null || couponTypeInfo.State == 0 || couponTypeInfo.UserRankLower > partUserInfo.UserRid || couponTypeInfo.OrderAmountLower > orderInfo.OrderAmount)
                return;

            CouponInfo couponInfo = new CouponInfo();

            couponInfo.CouponSN = Coupons.GenerateCouponSN();
            couponInfo.Uid = partUserInfo.Uid;
            couponInfo.CouponTypeId = couponTypeId;
            couponInfo.StoreId = couponTypeInfo.StoreId;
            couponInfo.Oid = 0;
            couponInfo.UseTime = new DateTime(1900, 1, 1);
            couponInfo.UseIP = "";
            couponInfo.Money = couponTypeInfo.Money;
            couponInfo.ActivateTime = couponTypeInfo.UseStartTime > DateTime.Now ? couponTypeInfo.UseStartTime : DateTime.Now;
            couponInfo.CreateTime = DateTime.Now;
            couponInfo.ActivateIP = couponInfo.CreateIP = ip;
            couponInfo.CreateUid = 0;
            couponInfo.CreateOid = orderInfo.Oid;
            couponInfo.ValidTime = couponTypeInfo.UseExpireTime > 0 ? DateTime.Now.AddDays(couponTypeInfo.UseExpireTime) : couponTypeInfo.UseEndTime;
            CreateCoupon(couponInfo);
        }

        /// <summary>
        /// 使用优惠劵
        /// </summary>
        public static void UseCoupon(int couponId, int oid, DateTime time, string ip)
        {
            VMall.Data.Coupons.UseCoupon(couponId, oid, time, ip);
        }

        /// <summary>
        /// 激活和使用优惠劵
        /// </summary>
        public static void ActivateAndUseCoupon(int couponId, int uid, int oid, DateTime time, string ip)
        {
            VMall.Data.Coupons.ActivateAndUseCoupon(couponId, uid, oid, time, ip);
        }

        /// <summary>
        /// 激活优惠劵
        /// </summary>
        /// <param name="couponId">优惠劵id</param>
        /// <param name="uid">用户id</param>
        /// <param name="time">时间</param>
        /// <param name="ip">ip</param>
        public static void ActivateCoupon(int couponId, int uid, DateTime time, string ip)
        {
            VMall.Data.Coupons.ActivateCoupon(couponId, uid, time, ip);
        }

        /// <summary>
        /// 退还订单使用的优惠劵
        /// </summary>
        /// <param name="oid">订单id</param>
        public static void ReturnUserOrderUseCoupons(int oid)
        {
            VMall.Data.Coupons.ReturnUserOrderUseCoupons(oid);
        }

        /// <summary>
        /// 获得用户订单发放的优惠劵列表
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public static List<CouponInfo> GetUserOrderSendCouponList(int oid)
        {
            return VMall.Data.Coupons.GetUserOrderSendCouponList(oid);
        }




        /// <summary>
        /// 优惠劵商品是否已经存在
        /// </summary>
        /// <param name="couponTypeId">优惠劵类型id</param>
        /// <param name="pid">商品id</param>
        public static bool IsExistCouponProduct(int couponTypeId, int pid)
        {
            return VMall.Data.Coupons.IsExistCouponProduct(couponTypeId, pid);
        }

        /// <summary>
        /// 商品是否属于同一优惠劵类型
        /// </summary>
        /// <param name="couponTypeId">优惠劵类型id</param>
        /// <param name="pidList">商品id列表</param>
        /// <returns></returns>
        public static bool IsSameCouponType(int couponTypeId, string pidList)
        {
            return VMall.Data.Coupons.IsSameCouponType(couponTypeId, pidList);
        }


        /// <summary>
        /// 获得用户订单发放的优惠劵列表
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public static List<CouponInfo> GetUserCouponListByTypeId(int uid,int typeId)
        {
            List<CouponInfo> InfoList = new List<CouponInfo>();
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(uid.ToString()) || string.IsNullOrEmpty(typeId.ToString()))
            {
                return null;
            }
            sb.Append(string.Format(" where uid={0} and coupontypeid={1} and validtime>getdate() and oid=0 ",uid,typeId)); 

            string commandText = "select * from [hlh_coupons] " + sb.ToString();
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    CouponInfo info = VMall.Data.Coupons.BuildCouponFromReader(reader);
                    InfoList.Add(info);
                }
                reader.Close();
            }
            return InfoList;


            //DbParameter[] parms = {
            //                        new SqlParameter("@uid",uid),
            //                        new SqlParameter("@coupontypeid",typeId)
            //                       };
            //return RDBSHelper.ExecuteReader(CommandType.StoredProcedure,
            //                                string.Format("{0}getuserordersendcouponlist", RDBSHelper.RDBSTablePre),
            //                                parms);
        }

        /// <summary>
        /// 赠送优惠券给下级会员
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <returns></returns>
        public static bool SendCoupon2ChildrenUser(CouponInfo coupon, PartUserInfo user)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@uid",user.Uid),
                                       new SqlParameter("@couponid",coupon.CouponId)
                                   };
            string commandText = string.Format("UPDATE [{0}coupons] SET [uid]=@uid WHERE [couponid]=@couponid",
                                               RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
    }
}
