using System;
using System.Text;
using System.Drawing;
using System.Web.Mvc;
using System.Collections.Generic;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using System.Web;

namespace VMall.Web.Mobile.Controllers
{
    /// <summary>
    /// 工具控制器类
    /// </summary>
    public partial class CodeImgController : BaseMobileController
    {

        /// <summary>
        /// 输出分享二维码图片
        /// </summary>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <returns></returns>
        public ActionResult Index()
        {
            //if (WorkContext.PartUserInfo.AgentType > 0 || WorkContext.Uid == 1)
            //{
                //获得用户唯一标示符sid
                int uid = WebHelper.GetRequestInt("shareid");
                PartUserInfo partUserInfo = Users.GetPartUserById(uid);
                //二维码
                string parentName = string.IsNullOrEmpty(partUserInfo.UserName) ? (string.IsNullOrEmpty(partUserInfo.Email) ? (string.IsNullOrEmpty(partUserInfo.Mobile) ? "" : partUserInfo.Mobile) : partUserInfo.Email) : partUserInfo.UserName;
                bool isMobile = true;//WebHelper.IsMobile();
                string shareUrl = "http://" + Request.Url.Host + ":" + Request.Url.Port + (isMobile ? "/mob" : "") + "/account/register?pname=" + parentName.Trim() + "&returnUrl=http://" + Request.Url.Host + ":" + Request.Url.Port + (isMobile ? "/mob" : "");
                //string shareUrl = "http://" + Request.Url.Host + ":" + Request.Url.Port + (isMobile ? "/mob" : "") + "/home/ShareIndex?uid=" + WorkContext.Uid;
                //string codeImgPath = CreateCode_Simple(shareUrl, WorkContext.PartUserInfo.Uid, WorkContext.PartUserInfo.Salt, isMobile);
                UserInfo user = Users.GetUserById(WorkContext.Uid);
                string bgQRcode = IOHelper.CreatQRCodeWithLogoNew(shareUrl,partUserInfo.Uid);

                ViewData["codeImg"] = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/upload/bgqrcode/" + bgQRcode;
                //ViewData["sharelink"] = shareUrl;
            //}
            //else
            //{

            //}
            return base.View();

            //输出验证图片
            //return new ImageResult(verifyImage.Image, verifyImage.ContentType);
        }



    }
}
