using VMall.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMall.Web.payment
{
    public partial class NativePayPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(Request.QueryString["data"]))
            //{
            //    string str = Request.QueryString["data"];
            //    //将字符串生成二维码图片
            //    Bitmap image = IOHelper.CreateCodeForImg(str);

            //    //保存为PNG到内存流  
            //    MemoryStream ms = new MemoryStream();
            //    image.Save(ms, ImageFormat.Png);

            //    //输出二维码图片
            //    Response.BinaryWrite(ms.GetBuffer());
            //    Response.End();
            //}
        }
    }
}