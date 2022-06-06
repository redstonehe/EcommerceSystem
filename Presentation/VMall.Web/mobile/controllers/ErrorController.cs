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
    /// 404控制器类
    /// </summary>
    public partial class ErrorController : BaseMobileController
    {
        public ActionResult index()
        {
            return View();

        }
    }
}
