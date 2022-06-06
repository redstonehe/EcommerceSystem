using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.Areas.WebApi.OuterSystem
{
    public class DirSaleSystemController : BaseApiController
    {
        //
        // GET: /WebApi/DirSaleSystem/

        public ActionResult Index()
        {
            return Content("11");
        }
        /// <summary>
        /// 更新发货信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOrderSend()
        {//orderstate 订单状态 shipsn 配送单号  shipcoid 配送公司id shipconame 配送公司名称 shiptime 配送时间 yyyymmdd hh:mm:ss:fff
            //发货通知
            int oid = WebHelper.GetRequestInt("oid");
            int orderstate = WebHelper.GetRequestInt("state");//发货状态 110
            string shipsn = WebHelper.GetRequestString("shipsn");
            int shipcoid = WebHelper.GetRequestInt("shipcoid");
            string shipconame = WebHelper.GetRequestString("shipconame");
            DateTime shiptime = TypeHelper.StringToDateTime(WebHelper.GetRequestString("shiptime"));

            AdminOrders.SendOrder(oid, OrderState.Sended, shipsn, shipcoid, shipconame, shiptime);
            OrderActions.CreateOrderAction(new OrderActionInfo()
            {
                Oid = oid,
                Uid = 2,
                RealName = "系统",
                ActionType = (int)OrderActionType.Send,
                ActionTime = DateTime.Now,
                ActionDes = "您订单的已经发货,发货方式为:" + shipconame + "。发货单号：" + shipsn
            });
            MallAdminLogs.CreateMallAdminLog(2, "system", 2, "系统管理员", "127.0.0.1", "发货", "发货,订单ID为:" + oid + "发货方式为:" + shipconame + "。发货单号：" + shipsn);

            return AppResult(true, "");
        }
        /// <summary>
        /// 更新库存信息
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateStock()
        {
            string stockListStr = WebHelper.GetRequestString("stock");
            //string tmp = "[{\"psn\":\"H20501001a\",\"number\":\"1\"},{\"psn\":\"H20302003\",\"number\":\"1\"}]";

            List<updateStockModel> list = JsonToObject<List<updateStockModel>>(stockListStr);

            foreach (var item in list)
            {
                Products.UpdateProductStockNumber(item.psn, item.number);
            }

            return AppResult(true, "", list);

            //return Content(ObjectToJson<List<updateStockModel>>(list));

        }


    }

    public class updateStockModel
    {
        public string psn { get; set; }
        public int number { get; set; }
    }
}
