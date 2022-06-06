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

namespace VMall.Web.controllers
{
    public class ActiveController : BaseWebController
    {
        //
        // GET: /Active/

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult FSList()
        {
            

            return View();
        }


    }
}
