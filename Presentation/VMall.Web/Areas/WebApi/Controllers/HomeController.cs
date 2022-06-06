using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

using VMall.Core;
using VMall.Services;
using VMall.Web.Areas.WebApi.Models;
using VMall.Web.Areas.WebApi.Secure;

namespace VMall.Web.Areas.WebApi.Controllers
{
    
    public class HomeController : Controller
    {
        //public ActionResult Index()
        //{
        //    return AjaxResult("success", "登录成功");
        //}
        public ActionResult Index()
        {
            //return AjaxResult("success", "访问成功");
            return Json("success.", JsonRequestBehavior.AllowGet);
            //return View();
        }

        [MyAuthorize]
        public ActionResult About()
        {
            return Json("Your application description page.", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Contact()
        {
            return Json("Contact", JsonRequestBehavior.AllowGet);
        }


        public ActionResult Error()
        {
            return Json("权限不足", JsonRequestBehavior.AllowGet);
        }
    }
}
