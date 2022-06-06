using System;

namespace VMall.Core
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// 已提交
        /// </summary>
        Submitted = 10,
        /// <summary>
        /// 等待付款
        /// </summary>
        WaitPaying = 30,
        /// <summary>
        /// 确认中
        /// </summary>
        Confirming = 50,
        /// <summary>
        /// 已确认
        /// </summary>
        Confirmed = 70,
        /// <summary>
        /// 预结算
        /// </summary>
        //PreSettle = 80,
        /// <summary>
        /// 备货中
        /// </summary>
        PreProducting = 90,
        /// <summary>
        /// 已发货
        /// </summary>
        Sended = 110,
        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 140,
        /// <summary>
        /// 已结算
        /// </summary>
        //Settled = 150,
        /// <summary>
        /// 退货
        /// </summary>
        Returned = 160,
        /// <summary>
        /// 换货
        /// </summary>
        Changed = 170,
        /// <summary>
        /// 锁定
        /// </summary>
        Locked = 180,
        /// <summary>
        /// 删除（非物理删除，仅不显示）
        /// </summary>
        Deleted = 190,
        /// <summary>
        /// 取消
        /// </summary>
        Cancelled = 200
    }

    /// <summary>
    /// 订单结算状态
    /// </summary>
    public enum OrderSettleState
    {
        /// <summary>
        /// 订单未参与过结算
        /// </summary>
        NotSettled = 0,
        /// <summary>
        /// 预结算(预收入)
        /// </summary>
        PreSettle = 1,
        /// <summary>
        /// 已结算(确定收入)
        /// </summary>
        Settled = 2,
        /// <summary>
        /// 预结算取消(预收入撤销)
        /// </summary>
        PreSettleCancelled = 3
    }

    public enum OrderSource
    {
        自营商城 = 1,
        天鹰网 = 2,
        全球购 = 3,
        直销后台 = 10,
        手机端 = 11,
        app端 = 21,
        启德系统 = 31,
        微商系统 = 41
    }

    public enum MallSource
    {
        自营商城 = 0,
        咖啡商城 = 1,
        微商订货系统 = 2,
        汇购优选系统 = 3
    }
}
