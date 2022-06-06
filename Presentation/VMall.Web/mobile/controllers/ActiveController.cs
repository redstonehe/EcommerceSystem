using System;
using System.Text;
using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Mobile.Models;

namespace VMall.Web.Mobile.Controllers
{
    public class ActiveController : BaseMobileController
    {
        //
        // GET: /Active/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Item()
        {
            return View();
        }
    }
}
