using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.Api.Controllers
{
    public partial class HomeController : BaseApiController
    {
        public ActionResult Index()
        {
            return AjaxResult("success", "登录成功");
        }

    }
}
