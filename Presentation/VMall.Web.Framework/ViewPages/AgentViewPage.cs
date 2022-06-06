using System;

namespace VMall.Web.Framework
{
    /// <summary>
    /// 移动前台视图页面基类型
    /// </summary>
    public abstract class AgentViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        public AgentWorkContext WorkContext;

        public sealed override void InitHelpers()
        {
            base.InitHelpers();
            WorkContext = ((BaseAgentController)(this.ViewContext.Controller)).WorkContext;
        }

        public sealed override void Write(object value)
        {
            Output.Write(value);
        }
    }

    /// <summary>
    /// 移动前台视图页面基类型
    /// </summary>
    public abstract class AgentViewPage : AgentViewPage<dynamic>
    {
    }
}
