using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMall.Core
{
    //public enum AccountType
    //{
    //    /// <summary>
    //    /// 奖金账户
    //    /// </summary>
    //    奖金账户 = 1,
    //    /// <summary>
    //    /// 电子账户
    //    /// </summary>
    //    电子账户 = 2,
    //    /// <summary>
    //    /// 现金账户
    //    /// </summary>
    //    现金账户 = 3,
    //    /// <summary>
    //    /// 购物积分
    //    /// </summary>
    //    购物积分 = 4,
    //    /// <summary>
    //    /// 基金账户
    //    /// </summary>
    //    基金账户 = 5,
    //    /// <summary>
    //    /// 消费币
    //    /// </summary>
    //    消费币 = 6,
    //    /// <summary>
    //    /// 保证金账户
    //    /// </summary>
    //    保证金账户 = 7,
    //    /// <summary>
    //    /// 福利账户
    //    /// </summary>
    //    福利账户 = 8,
    //    /// <summary>
    //    /// 红包账户
    //    /// </summary>
    //    红包账户 = 9,
    //    /// <summary>
    //    /// 海米账户
    //    /// </summary>
    //    海米账户 = 10,
    //    /// <summary>
    //    /// 预结算海米账户
    //    /// </summary>
    //    预结算海米账户 = 11,
    //    /// <summary>
    //    /// 代理账户
    //    /// </summary>
    //    代理账户 = 30,
    //    /// <summary>
    //    /// 佣金账户
    //    /// </summary>
    //    佣金账户 = 31,
    //    /// <summary>
    //    /// 佣金账户
    //    /// </summary>
    //    有机胚芽佣金 = 33
    //}
    public enum AccountType
    {
        /// <summary>
        /// 奖金账户
        /// </summary>
        奖金账户 = 1,
        /// <summary>
        /// 积分账户
        /// </summary>
        积分账户 = 2,
        /// <summary>
        /// 报单账户
        /// </summary>
        报单账户 = 3,
        /// <summary>
        /// 综合服务帐户
        /// </summary>
        综合服务帐户 = 4,
        /// <summary>
        /// 福利账户（重消）
        /// </summary>
        重消福利账户 = 5,
        /// <summary>
        /// 电子货币账户
        /// </summary>
        电子货币账户 = 6,
        /// <summary>
        /// 商城钱包
        /// </summary>
        商城钱包= 7,
        /// <summary>
        /// 商城奖金账户
        /// </summary>
        商城奖金账户 = 8,




        代理账户 = 3000,
        佣金账户 = 3001,
        海米账户 = 3002,
        红包账户 = 3003,
        预结算海米账户 = 30004,
        ///// <summary>
        ///// 保证金账户
        ///// </summary>
        //保证金账户 = 7,
        ///// <summary>
        ///// 福利账户
        ///// </summary>
        //福利账户 = 8,
        ///// <summary>
        ///// 红包账户
        ///// </summary>
        //红包账户 = 9,
        ///// <summary>
        ///// 海米账户
        ///// </summary>
        //海米账户 = 10,
        ///// <summary>
        ///// 预结算海米账户
        ///// </summary>
        //预结算海米账户 = 11,
        ///// <summary>
        ///// 代理账户
        ///// </summary>
        //代理账户 = 30,
        ///// <summary>
        ///// 佣金账户
        ///// </summary>
        //佣金账户 = 31,
        ///// <summary>
        ///// 佣金账户
        ///// </summary>
        //有机胚芽佣金 = 33
    }
    /// <summary>
    /// 推荐类型
    /// </summary>
    public enum PanertType
    {
        /// <summary>
        /// 汇购会员
        /// </summary>
        海客 = 1,
        /// <summary>
        /// 直销系统会员
        /// </summary>
        直销会员 = 2
    }
}
