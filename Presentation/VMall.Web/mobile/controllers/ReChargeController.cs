using System;
using System.Web.Mvc;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Mobile.Models;
using System.Collections.Generic;

namespace VMall.Web.Mobile.Controllers
{
    /// <summary>
    /// 充值控制器类
    /// </summary>
    public partial class ReChargeController : BaseMobileController
    {
        public ActionResult index()
        {
            List<PartProductInfo> list = Products.GetProductListByWhere(string.Format(" [storeid] ={0}  AND [state]=0 ", TypeHelper.StringToInt(WebHelper.GetConfigSettings("ChongZhiStore"))));

            return View(list);

        }
    }
}
