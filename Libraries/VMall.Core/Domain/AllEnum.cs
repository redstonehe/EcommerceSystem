using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMall.Core
{
    /// <summary>
    /// 
    /// </summary>
    public enum Menu2Icon
    {
        //商品管理 = "fa-archive",
        //促销活动 = "fa-expeditedssl",
        //订单管理 = "fa-shopping-cart",
        //用户管理 = "fa-user",
        //报单管理 = "fa-list-alt",
        //商家店铺管理 = "fa-cubes",
        //广告评价管理 = "fa-laptop",
        //商城内容 = "fa-comments-o",
        //报表统计 = "fa-bar-chart",
        //系统设置 = "fa-cogs",
        //日志管理 = "fa-database",
        //开发管理 = "fa-asterisk",
        //权限管理 = "fa-group",
        //系统帮助 = "fa-cutlery",
        //商品管理 = "fa-archive",
        //商品管理 = "fa-archive",
        //商品管理 = "fa-archive",
        //商品管理 = "fa-archive",
    }
    /// <summary>
    /// 平台来源
    /// </summary>
    public enum MallSourceType
    {
        /// <summary>
        /// 自营商城
        /// </summary>
        自营商城 = 0,

    }
    /// <summary>
    /// 平台来源
    /// </summary>
    public enum UserSourceType
    {
        /// <summary>
        /// 散客会员
        /// </summary>
        散客会员 = 0,
        /// <summary>
        /// 引流会员
        /// </summary>
        引流会员 = 1

    }
    /// <summary>
    /// 等级类型
    /// </summary>
    public enum AgentTypeEnum
    {
        /// <summary>
        /// 消费者
        /// </summary>
        消费者 = 0,
        /// <summary>
        /// 普卡
        /// </summary>
        普卡 = 1,
        /// <summary>
        /// 金卡
        /// </summary>
        金卡 = 2,
        /// <summary>
        /// 白金卡
        /// </summary>
        白金卡 = 3,
        

    }


    /// <summary>
    /// 消息类型
    /// </summary>
    public enum InformTypeEnum
    {
        /// <summary>
        /// 系统消息
        /// </summary>
        系统消息 = 1,
        /// <summary>
        /// 会员消息
        /// </summary>
        会员消息 = 1,
        /// <summary>
        /// 总代
        /// </summary>
        订单消息 = 2,
      
    }

    /// <summary>
    /// log类型
    /// FATAL > ERROR > WARN > INFO > DEBUG
    /// </summary>
    public enum LogLevelEnum
    {
        /// <summary>
        /// DEBUG
        /// </summary>
        DEBUG = 1,
        /// <summary>
        /// INFO
        /// </summary>
        INFO = 2,
        /// <summary>
        /// WARN
        /// </summary>
        WARN = 3,
        /// <summary>
        /// ERROR
        /// </summary>
        ERROR = 4,
        /// <summary>
        /// FATAL
        /// </summary>
        FATAL = 5,
    }





    public enum TestType
    {
        red = 1,
        green = 2,
        blue = 3
    }














    public class ValueDictionary
    {
        public ValueDictionary() { }
        Dictionary<string, string> menuDict = new Dictionary<string, string>()
        { 
            { "商品管理", "fa-archive" },
            { "促销活动", "fa-expeditedssl" },
            { "订单管理", "fa-shopping-cart" },
            { "用户管理", "fa-user" },
            { "报单管理", "fa-list-alt" },
            { "商家店铺管理", "fa-cubes" },
            { "广告评价管理", "fa-laptop" },
            { "商城内容", "fa-comments-o" },
            { "报表统计", "fa-bar-chart" },
            { "系统设置", "fa-cogs" },
            { "日志管理", "fa-database" },
            { "开发管理", "fa-asterisk" },
            { "权限管理", "fa-group"},
            { "系统帮助", "fa-cutlery"}
        };
    }
}
