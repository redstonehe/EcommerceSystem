using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMall.Core
{
    public class AccountDetailInfo
    {
        private int _detailid = 0;//记录id
        private int _accountid = 0;//帐号id
        private int _userid = 0;//会员id
        private DateTime _createtime = DateTime.Now;//创建时间
        private int _detailtype = 0;//交易类型
        private decimal _inamount = 0M;//收入详细
        private decimal _outamount = 0M;//支出详细
        private string _ordercode = string.Empty;//订单号
        private int _adminuid = 0;//操作人id
        private int _status = 1;//当前状态 1有效
        private string _detaildes = string.Empty;//交易描述
        private string _adminname = string.Empty;//操作人名称

        private decimal _curbanlance = 0M;//账户当前余额
        /// <summary>
        /// 账户当前余额
        /// </summary>
        public decimal CurBanlance
        {
            set { _curbanlance = value; }
            get { return _curbanlance; }
        }
        /// <summary>
        /// 记录id
        /// </summary>
        public int DetailId
        {
            set { _detailid = value; }
            get { return _detailid; }
        }
        /// <summary>
        /// 帐号id
        /// </summary>
        public int AccountId
        {
            set { _accountid = value; }
            get { return _accountid; }
        }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId
        {
            set { _userid = value; }
            get { return _userid; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 交易类型
        /// </summary>
        public int DetailType
        {
            set { _detailtype = value; }
            get { return _detailtype; }
        }
        /// <summary>
        /// 收入详细
        /// </summary>
        public decimal InAmount
        {
            set { _inamount = value; }
            get { return _inamount; }
        }
        /// <summary>
        /// 支出详细
        /// </summary>
        public decimal OutAmount
        {
            set { _outamount = value; }
            get { return _outamount; }
        }
        /// <summary>
        /// 操作人id
        /// </summary>
        public int AdminUid
        {
            set { _adminuid = value; }
            get { return _adminuid; }
        }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderCode
        {
            set { _ordercode = value.Trim(); }
            get { return _ordercode; }
        }
        /// <summary>
        /// 当前状态 1为已处理
        /// </summary>
        public int Status
        {
            set { _status = value; }
            get { return _status; }
        }
        /// <summary>
        /// 交易描述
        /// </summary>
        public string DetailDes
        {
            set { _detaildes = value.Trim(); }
            get { return _detaildes; }
        }
        /// <summary>
        /// 操作人名称--显示用
        /// </summary>
        public string AdminName
        {
            set { _adminname = value.Trim(); }
            get { return _adminname; }
        }
    }
    /// <summary>
    ///  DetailType规则已MallSource开头，后两位表示具体操作类型，
    ///  MallSource 类型如下
    ///  自营商城 = 0,
    ///  尚睿淳商城 = 1,
    ///  有机胚芽微商订货系统 = 2,
    ///  汇购优选系统 = 3,
    ///  施惠葆商城 = 4,
    ///  如施惠葆操作类型格式为 401
    ///  35之后采用新规则
    /// </summary>
    public enum DetailType
    {
        /// <summary>
        /// 转账支出
        /// </summary>
        账户间转账支出 = 101,
        /// <summary>
        /// 转账收入
        /// </summary>
        账户间转账收入 = 102,
        /// <summary>
        /// 订单抵现支出
        /// </summary>
        订单抵现支出 = 1,
        /// <summary>
        /// 订单取消返回
        /// </summary>
        订单取消返回 = 2,
        /// <summary>
        /// 海米分销结算收入
        /// </summary>
        三级分销结算收入 = 3,
        /// <summary>
        /// 海米分销结算支出
        /// </summary>
        三级分销结算支出 = 4,
        /// <summary>
        /// 活动赠送
        /// </summary>
        活动赠送 = 5,
        /// <summary>
        /// 活动赠送
        /// </summary>
        活动赠送取消 = 6,
        /// <summary>
        /// 注册送红包
        /// </summary>
        注册送红包 = 7,
        /// <summary>
        /// 提现支出
        /// </summary>
        提现支出 = 8,
         /// <summary>
        /// 提现取消返回
        /// </summary>
        提现取消返回 = 9,
        /// <summary>
        /// 微商代理结算收入
        /// </summary>
        微商代理结算收入 = 10,
        /// <summary>
        /// 微商代理结算支出
        /// </summary>
        微商代理结算支出 = 11,
        /// <summary>
        /// 导入代理账户余额
        /// </summary>
        导入代理账户余额 = 12,
        /// <summary>
        /// 扣除代理账户
        /// </summary>
        扣除代理账户 = 13,
        /// <summary>
        /// 咖啡消费券推荐奖励
        /// </summary>
        咖啡消费券推荐奖励 = 14,
        /// <summary>
        /// 咖啡消费券差价返还奖励
        /// </summary>
        咖啡消费券差价返还奖励 = 15,
        /// <summary>
        /// 代理体验包预结算收入
        /// </summary>
        代理体验包预结算收入 = 16,
        /// <summary>
        /// 代理体验包预结算支出
        /// </summary>
        代理体验包预结算支出 = 17,
        /// <summary>
        /// 微商订货预结算收入
        /// </summary>
        微商订货预结算收入 = 18,
        /// <summary>
        /// 微商订货预结算支出
        /// </summary>
        微商订货预结算支出 = 19,
        /// <summary>
        /// 微商订货结算收入
        /// </summary>
        微商订货结算收入 = 20,
        /// <summary>
        /// 微商订货结算支出
        /// </summary>
        微商订货结算支出 = 21,
        /// <summary>
        /// 中核积分佣金预结算收入
        /// </summary>
        中核积分佣金预结算收入 = 22,
        /// <summary>
        /// 中核积分佣金预结算支出
        /// </summary>
        中核积分佣金预结算支出 = 23,
        /// <summary>
        /// 中核积分佣金结算收入
        /// </summary>
        中核积分佣金结算收入 = 24,
        /// <summary>
        /// 中核积分佣金结算支出
        /// </summary>
        中核积分佣金结算支出 = 25,


        /// <summary>
        /// 预结算收入
        /// </summary>
        预结算收入 = 302,
        /// <summary>
        /// 预结算支出
        /// </summary>
        预结算支出 = 303,
        /// <summary>
        /// 结算收入
        /// </summary>
        结算收入 = 304,
        /// <summary>
        /// 结算支出
        /// </summary>
        结算支出 = 305
    }
}
