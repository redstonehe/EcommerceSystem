﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台广告控制器类
    /// </summary>
    public partial class AdvertController : BaseMallAdminController
    {
        #region 广告位置

        /// <summary>
        /// 广告位置列表
        /// </summary>
        public ActionResult AdvertPositionList(int pageSize = 15, int pageNumber = 1, string title = "")
        {
            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminAdverts.GetAdvertPositionCount(title));

            AdvertPositionListModel model = new AdvertPositionListModel()
            {
                Title = title,
                PageModel = pageModel,
                AdvertPositionList = AdminAdverts.GetAdvertPositionList(pageSize, pageNumber, title)
            };

            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&title={3}",
                                                          Url.Action("advertpositionlist"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize, title));
            return View(model);
        }

        /// <summary>
        /// 添加广告位置
        /// </summary>
        [HttpGet]
        public ActionResult AddAdvertPosition()
        {
            AdvertPositionModel model = new AdvertPositionModel();
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 添加广告位置
        /// </summary>
        [HttpPost]
        public ActionResult AddAdvertPosition(AdvertPositionModel model)
        {
            if (ModelState.IsValid)
            {
                AdvertPositionInfo advertPositionInfo = new AdvertPositionInfo()
                {
                    Title = model.Title,
                    Description = model.Description ?? ""
                };

                AdminAdverts.CreateAdvertPosition(advertPositionInfo);
                AddMallAdminLog("添加广告位置", "添加广告位置,广告位置为:" + model.Title);
                return PromptView("广告位置添加成功");
            }
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 编辑广告位置
        /// </summary>
        [HttpGet]
        public ActionResult EditAdvertPosition(int adPosId = -1)
        {
            AdvertPositionInfo advertPositionInfo = AdminAdverts.GetAdvertPositionById(adPosId);
            if (advertPositionInfo == null)
                return PromptView("广告位置不存在");

            AdvertPositionModel model = new AdvertPositionModel();
            model.Title = advertPositionInfo.Title;
            model.Description = advertPositionInfo.Description;

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 编辑广告位置
        /// </summary>
        [HttpPost]
        public ActionResult EditAdvertPosition(AdvertPositionModel model, int adPosId = -1)
        {
            AdvertPositionInfo advertPositionInfo = AdminAdverts.GetAdvertPositionById(adPosId);
            if (advertPositionInfo == null)
                return PromptView("广告位置不存在");

            if (ModelState.IsValid)
            {
                advertPositionInfo.Title = model.Title;
                advertPositionInfo.Description = model.Description ?? "";

                AdminAdverts.UpdateAdvertPosition(advertPositionInfo);
                AddMallAdminLog("修改广告位置", "修改广告位置,广告位置ID为:" + adPosId);
                return PromptView("广告位置修改成功");
            }

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 删除广告位置
        /// </summary>
        public ActionResult DelAdvertPosition(int adPosId = -1)
        {
            AdminAdverts.DeleteAdvertPositionById(adPosId);
            AddMallAdminLog("删除广告位置", "删除广告位置,广告位置ID为:" + adPosId);
            return PromptView("广告位置删除成功");
        }

        #endregion

        #region 广告列表

        /// <summary>
        /// 广告列表
        /// </summary>
        public ActionResult AdvertList(int adPosId = 0, string title = "", int pageSize = 15, int pageNumber = 1)
        {
            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminAdverts.AdminGetAdvertCount(adPosId, title));

            AdvertListModel model = new AdvertListModel()
            {
                PageModel = pageModel,
                Title = title,
                AdPosId = adPosId,
                AdvertList = AdminAdverts.AdminGetAdvertList(pageModel.PageSize, pageModel.PageNumber, adPosId, title)
            };

            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部广告位置", Value = "0" });
            foreach (AdvertPositionInfo advertPositionInfo in AdminAdverts.GetAllAdvertPosition())
            {
                itemList.Add(new SelectListItem() { Text = advertPositionInfo.Title, Value = advertPositionInfo.AdPosId.ToString() });
            }
            ViewData["advertPositionList"] = itemList;

            MallUtils.SetAdminRefererCookie(string.Format("{0}?adPosId={1}&pageNumber={2}&pageSize={3}&title={4}", Url.Action("advertlist"), adPosId, pageModel.PageNumber, pageModel.PageSize, title));
            return View(model);
        }

        /// <summary>
        /// 添加广告
        /// </summary>
        [HttpGet]
        public ActionResult AddAdvert()
        {
            AdvertModel model = new AdvertModel();
            Load();
            return View(model);
        }

        /// <summary>
        /// 添加广告
        /// </summary>
        [HttpPost]
        public ActionResult AddAdvert(AdvertModel model)
        {
            if (AdminAdverts.GetAdvertPositionById(model.AdPosId) == null)
                ModelState.AddModelError("AdPosId", "广告位置不存在");

            if (ModelState.IsValid)
            {
                if (model.Url.IndexOf("/mob/catalog/item?itemid=") >= 0)
                {
                    string pid = model.Url.Substring(model.Url.IndexOf("?itemid=") + 8, model.Url.IndexOf("&uid=") - model.Url.IndexOf("?itemid=") - 8);
                    model.Url = "/mob/product/" + pid + ".html";
                }
                AdvertInfo advertInfo = new AdvertInfo()
                {
                    ClickCount = 0,
                    AdPosId = model.AdPosId,
                    Type = model.Type,
                    Title = model.Title,
                    Body = model.Body,

                    Url = model.Url ?? "",
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    ExtField1 = model.ExtField1 ?? "",
                    ExtField2 = model.ExtField2 ?? "",
                    ExtField3 = model.ExtField3 ?? "",
                    ExtField4 = model.ExtField4 ?? "",
                    ExtField5 = model.ExtField5 ?? "",
                    State = model.State,
                    DisplayOrder = model.DisplayOrder
                };

                AdminAdverts.CreateAdvert(advertInfo);
                AddMallAdminLog("添加广告", "添加广告,广告为:" + model.Title);
                return PromptView("广告添加成功");
            }
            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑广告
        /// </summary>
        [HttpGet]
        public ActionResult EditAdvert(int adId = -1)
        {
            AdvertInfo advertInfo = AdminAdverts.AdminGetAdvertById(adId);
            if (advertInfo == null)
                return PromptView("广告不存在");

            AdvertModel model = new AdvertModel();
            model.AdPosId = advertInfo.AdPosId;
            model.Type = advertInfo.Type;
            model.Title = advertInfo.Title;
            model.Body = advertInfo.Body;
            model.Url = advertInfo.Url;
            model.StartTime = advertInfo.StartTime;
            model.EndTime = advertInfo.EndTime;
            model.ExtField1 = advertInfo.ExtField1;
            model.ExtField2 = advertInfo.ExtField2;
            model.ExtField3 = advertInfo.ExtField3;
            model.ExtField4 = advertInfo.ExtField4;
            model.ExtField5 = advertInfo.ExtField5;
            model.State = advertInfo.State;
            model.DisplayOrder = advertInfo.DisplayOrder;

            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑广告
        /// </summary>
        [HttpPost]
        public ActionResult EditAdvert(AdvertModel model, int adId = -1)
        {
            AdvertInfo advertInfo = AdminAdverts.AdminGetAdvertById(adId);
            if (advertInfo == null)
                return PromptView("广告不存在");

            int oldAdPosId = advertInfo.AdPosId;
            if (ModelState.IsValid)
            {
                if (model.Url.IndexOf("/mob/catalog/item?itemid=") >= 0)
                {
                    string pid = model.Url.Substring(model.Url.IndexOf("?itemid=") + 8, model.Url.IndexOf("&uid=") - model.Url.IndexOf("?itemid=") - 8);
                    model.Url = "/mob/product/" + pid + ".html";
                }
                advertInfo.AdPosId = model.AdPosId;
                advertInfo.Type = model.Type;
                advertInfo.Title = model.Title;
                advertInfo.Body = model.Body;
                advertInfo.Url = model.Url ?? "";
                advertInfo.StartTime = model.StartTime;
                advertInfo.EndTime = model.EndTime;
                advertInfo.ExtField1 = model.ExtField1 ?? "";
                advertInfo.ExtField2 = model.ExtField2 ?? "";
                advertInfo.ExtField3 = model.ExtField3 ?? "";
                advertInfo.ExtField4 = model.ExtField4 ?? "";
                advertInfo.ExtField5 = model.ExtField5 ?? "";
                advertInfo.State = model.State;
                advertInfo.DisplayOrder = model.DisplayOrder;

                AdminAdverts.UpdateAdvert(oldAdPosId, advertInfo);
                AddMallAdminLog("修改广告", "修改广告,广告ID为:" + adId);
                return PromptView("广告修改成功");
            }

            Load();
            return View(model);
        }

        /// <summary>
        /// 设置广告显示隐藏
        /// </summary>
        public ActionResult SetAdvState(int adid = -1, int state = 0)
        {
            AdvertInfo advertInfo = AdminAdverts.AdminGetAdvertById(adid);
            if (advertInfo == null)
                return PromptView("广告不存在");
            bool result = AdminAdverts.SetAdvState(adid, state);
            if (result)
                return Content("1");

            else
                return Content("0");

        }

        /// <summary>
        /// 删除广告
        /// </summary>
        public ActionResult DelAdvert(int adId = -1)
        {
            AdminAdverts.DeleteAdvertById(adId);
            AddMallAdminLog("删除广告", "删除广告,广告ID为:" + adId);
            return PromptView("广告删除成功");
        }
        /// <summary>
        /// 批量删除广告
        /// </summary>
        public ActionResult BatchDel(int[] idList)
        {
            AdminAdverts.DeleteAdvertsByIds(idList);
            AddMallAdminLog("批量删除广告", "批量删除广告,广告ID为:" + CommonHelper.IntArrayToString(idList));
            return PromptView("广告批量删除成功");
        }

        private void Load()
        {
            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "请选择广告位置", Value = "0" });
            foreach (AdvertPositionInfo advertPositionInfo in AdminAdverts.GetAllAdvertPosition())
            {
                itemList.Add(new SelectListItem() { Text = advertPositionInfo.Title, Value = advertPositionInfo.AdPosId.ToString() });
            }
            ViewData["advertPositionList"] = itemList;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
        }
        #endregion
    }
}
