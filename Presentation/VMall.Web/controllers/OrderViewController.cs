using System;
using System.Text;
using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Models;

namespace VMall.Web.Controllers
{
    /// <summary>
    /// 订单查询
    /// </summary>
    public class OrderViewController : BaseWebController
    {

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 订单列表
        /// </summary>
        public ActionResult OrderViewList()
        {
            string oinfo = WebHelper.GetFormString("oinfo");
            string viewCode = WebHelper.GetFormString("viewCode");
            string startAddTime = DateTime.Now.AddMonths(-1).ToString();
            string endAddTime = DateTime.Now.ToString();
            OrderViewModel model = new OrderViewModel();
          
            if (viewCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "viewCode"))
            {
                model.State = false;
                model.Message = "验证码不正确";
                model.OrderInfoList = new List<OrderInfo>();
                return View(model);
            } List<OrderInfo> list = Orders.GetOrderListByWhere(string.Format(" (osn='{0}' or mobile='{1}') and addtime>='{2}' and addtime<='{3}' and mallsource={4} and orderstate>=70 and orderstate<=140 ", oinfo, oinfo, startAddTime, endAddTime, (int)MallSource.自营商城));
            model.State = true;
            model.Message = "";
            model.OrderInfoList = list;
            return View(model);
        }


    }
}
