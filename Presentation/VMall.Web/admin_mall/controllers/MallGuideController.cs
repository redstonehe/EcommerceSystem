using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城手册控制器类
    /// </summary>
    public partial class MallGuideController : BaseMallAdminController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult UseGuide()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SystemNote()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        public ActionResult UpdateLog()
        {
            return View();

        }
        /// <summary>
        /// 
        /// </summary>
        public ActionResult AdminTools()
        {
            return View();

        }
    }
}