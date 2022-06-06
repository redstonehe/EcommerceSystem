using System;
using System.Web.Mvc;
using System.Web.Routing;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using System.Collections.Generic;

using System.Linq;

namespace VMall.Web.Controllers
{
    /// <summary>
    /// 首页控制器类
    /// </summary>
    public partial class HomeController : BaseWebController
    {
        public static string AppKey = "22dbd1b598b311e59d7e08606ed9d972";
        /// <summary>
        /// 首页
        /// </summary>
        public ActionResult Index()
        {
            //判断请求是否来自移动设备，如果是则重定向到移动主题
            if (WebHelper.IsMobile() || WebHelper.IsWechat())
                return RedirectToAction("index", "home", new RouteValueDictionary { { "area", "mob" } });
            //if ((!Convert.ToBoolean(WebHelper.GetQueryInt("type"))) && WorkContext.MallAGid < 2)
            //    return View("prepare");
            //首页的数据需要在其视图文件中直接调用，所以此处不再需要视图模型

            return View("index2016");
        }

        public ActionResult SearchWordTips(string word) {
            List<ProductKeywordInfo> wordlist = Searches.GetSearchWordTips(word);
            return Json(wordlist, JsonRequestBehavior.AllowGet);
        }
    }
}
