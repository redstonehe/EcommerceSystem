using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VMall.Core;
using VMall.Services;
using System.Drawing;

using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using VMall.Services.DirSale;
using System.Web.Script.Serialization;

namespace VMall.Services.WeiShang
{
    public class BaseRequest
    {
        private static object ctx = new object();//锁对象

        public static WSAPIClient WSclient = new WSAPIClient("http://hgw.51shop.mobi/v2", "hgw.51shop.mobi","127.0.0.1", "Xquark");
    }
}
