using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VMall.Core;
using VMall.Services;
using VMall.Web.Api.Secure;
using VMall.Web.Framework;

namespace VMall.Web.Api.Controllers
{

    public class HomeController : BaseApiController
    {
        public ActionResult ShareIndex()
        {
            //return AjaxResult("success","访问成功");
            return base.Json("success.11", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            //return AjaxResult("success", "访问成功");
            return base.Json("success.", JsonRequestBehavior.AllowGet);
            //return View();
        }

        [MyAuthorize]
        public ActionResult About()
        {
            return Json("Your application description page.", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return base.Json("权限不足", JsonRequestBehavior.AllowGet);
        }
    }
}
