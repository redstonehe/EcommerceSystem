using System;
using System.Web;
using System.Web.Mvc;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;
using System.Collections.Generic;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台banner控制器类
    /// </summary>
    public partial class BannerController : BaseMallAdminController
    {
        /// <summary>
        /// banner列表
        /// </summary>
        public ActionResult List(int pageSize = 15, int pageNumber = 1,string title="",int type=1)
        {
            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminBanners.AdminGetBannerCount(title,type));

            BannerListModel model = new BannerListModel()
            {
                Title=title,
                Type=type,
                PageModel = pageModel,
                BannerList = AdminBanners.AdminGetBannerList(pageSize, pageNumber,title,type)
            };
            List<SelectListItem> itemList = new List<SelectListItem>();
            //itemList.Add(new SelectListItem() { Text = "全部类型", Value = "-1" });
            //itemList.Add(new SelectListItem() { Text = "PC", Value = "0" });
            itemList.Add(new SelectListItem() { Text = "手机", Value = "1" });
            //itemList.Add(new SelectListItem() { Text = "手机端全球购", Value = "2" });
            //itemList.Add(new SelectListItem() { Text = "手机端汇生鲜", Value = "3" });
            //itemList.Add(new SelectListItem() { Text = "手机端和治友德", Value = "4" });
            //itemList.Add(new SelectListItem() { Text = "手机端活动专区", Value = "5" });
            //itemList.Add(new SelectListItem() { Text = "微商订货系统", Value = "6" });
            //itemList.Add(new SelectListItem() { Text = "红包专区", Value = "7" });

            ViewData["bannerTypeList"] = itemList;
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&title={3}&type={4}",
                                                          Url.Action("list"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,title,type));
            return View(model);
        }

        /// <summary>
        /// 添加banner
        /// </summary>
        [HttpGet]
        public ActionResult Add()
        {
            BannerModel model = new BannerModel();
            Load();
            return View(model);
        }

        /// <summary>
        /// 添加banner
        /// </summary>
        [HttpPost]
        public ActionResult Add(BannerModel model)
        {
            if (ModelState.IsValid)
            {
                BannerInfo bannerInfo = new BannerInfo()
                {
                    Type = model.BannerType,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    IsShow = model.IsShow,
                    Title = model.BannerTitle == null ? "" : model.BannerTitle,
                    Img = model.Img,
                    Url = model.Url,
                    DisplayOrder = model.DisplayOrder,
                    ExtField1 = model.ExtField1 ?? "",
                    ExtField2 = model.ExtField2 ?? "",
                    ExtField3 = model.ExtField3 ?? ""
                };

                AdminBanners.CreateBanner(bannerInfo);
                AddMallAdminLog("添加banner", "添加banner,banner为:" + model.BannerTitle);
                return PromptView("banner添加成功");
            }
            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑banner
        /// </summary>
        [HttpGet]
        public ActionResult Edit(int id = -1)
        {
            BannerInfo bannerInfo = AdminBanners.AdminGetBannerById(id);
            if (bannerInfo == null)
                return PromptView("Banner不存在");

            BannerModel model = new BannerModel();
            model.BannerType = bannerInfo.Type;
            model.StartTime = bannerInfo.StartTime;
            model.EndTime = bannerInfo.EndTime;
            model.IsShow = bannerInfo.IsShow;
            model.BannerTitle = bannerInfo.Title;
            model.Img = bannerInfo.Img;
            model.Url = bannerInfo.Url;
            model.DisplayOrder = bannerInfo.DisplayOrder;
            model.ExtField1 = bannerInfo.ExtField1;
            model.ExtField2 = bannerInfo.ExtField2;
            model.ExtField3 = bannerInfo.ExtField3;
            Load();

            return View(model);
        }

        /// <summary>
        /// 编辑banner
        /// </summary>
        [HttpPost]
        public ActionResult Edit(BannerModel model, int id = -1)
        {
            BannerInfo bannerInfo = AdminBanners.AdminGetBannerById(id);
            if (bannerInfo == null)
                return PromptView("Banner不存在");

            if (ModelState.IsValid)
            {
                //bannerInfo.Type = model.BannerType;
                bannerInfo.StartTime = model.StartTime;
                bannerInfo.EndTime = model.EndTime;
                bannerInfo.IsShow = model.IsShow;
                bannerInfo.Title = model.BannerTitle == null ? "" : model.BannerTitle;
                bannerInfo.Img = model.Img;
                bannerInfo.Url = model.Url;
                bannerInfo.DisplayOrder = model.DisplayOrder;
                bannerInfo.ExtField1 = model.ExtField1 ?? "";
                bannerInfo.ExtField2 = model.ExtField2 ?? "";
                bannerInfo.ExtField3 = model.ExtField3 ?? "";
                AdminBanners.UpdateBanner(bannerInfo);
                AddMallAdminLog("修改banner", "修改banner,bannerID为:" + id);
                return PromptView("banner修改成功");
            }

            Load();
            return View(model);
        }

        /// <summary>
        /// 删除banner
        /// </summary>
        public ActionResult Del(int[] idList)
        {
            AdminBanners.DeleteBannerById(idList);
            AddMallAdminLog("删除banner", "删除banner,bannerID为:" + CommonHelper.IntArrayToString(idList));
            return PromptView("banner删除成功");
        }
        /// <summary>
        /// 批量显示banner
        /// </summary>
        public ActionResult ShowBatch(int[] idList)
        {
            AdminBanners.ShowBannerById(idList);
            return PromptView("banner批量显示成功");
        }

        private void Load()
        {
            string allowImgType = string.Empty;
            string[] imgTypeList = StringHelper.SplitString(BMAConfig.MallConfig.UploadImgType, ",");
            foreach (string imgType in imgTypeList)
                allowImgType += string.Format("*{0};", imgType.ToLower());

            ViewData["allowImgType"] = allowImgType;
            ViewData["maxImgSize"] = BMAConfig.MallConfig.UploadImgSize;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
        }
    }
}
