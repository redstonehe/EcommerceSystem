using System;
using System.Data;
using System.Web.Mvc;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;
using System.Collections.Generic;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台商品评价控制器类
    /// </summary>
    public partial class ProductReviewController : BaseMallAdminController
    {
        /// <summary>
        /// 商品评价列表
        /// </summary>
        public ActionResult ProductReviewList(string storeName, string message, string rateStartTime, string rateEndTime, string sortColumn, string sortDirection, int storeId = -1, int pid = 0, string pname = "", int type = 0, int pageNumber = 1, int pageSize = 15)
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            string condition = AdminProductReviews.AdminGetProductReviewListCondition(storeId, pid, pname, type, message, rateStartTime, rateEndTime);
            string sort = AdminProductReviews.AdminGetProductReviewListSort(sortColumn, sortDirection);

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminProductReviews.AdminGetProductReviewCount(condition));
            ProductReviewListModel model = new ProductReviewListModel()
            {
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                ProductReviewList = AdminProductReviews.AdminGetProductReviewList(pageModel.PageSize, pageModel.PageNumber, condition, sort),
                StoreId = storeId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "全部店铺" : storeName,
                Pid = pid,
                PName = pname,
                Type = type,
                Message = message,
                StartTime = rateStartTime,
                EndTime = rateEndTime
            };
            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部", Value = "0" });
            itemList.Add(new SelectListItem() { Text = "好评", Value = "1" });
            itemList.Add(new SelectListItem() { Text = "中评", Value = "2" });
            itemList.Add(new SelectListItem() { Text = "差评", Value = "3" });
            ViewData["typeList"] = itemList;
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&storeId={5}&storeName={6}&pid={7}&message={8}&startTime={9}&endTime={10}&pname={11}&type={12}",
                                                            Url.Action("productreviewlist"),
                                                            pageModel.PageNumber, pageModel.PageSize,
                                                            sortColumn, sortDirection,
                                                            storeId, storeName, pid,
                                                            message, rateStartTime, rateEndTime, pname, type));
            return View(model);
        }

        /// <summary>
        /// 商品评价回复列表
        /// </summary>
        public ActionResult ProductReviewReplyList(int reviewId = -1)
        {
            ProductReviewInfo productReviewInfo = AdminProductReviews.AdminGetProductReviewById(reviewId);
            if (productReviewInfo == null)
                return PromptView("商品评价不存在");

            ProductReviewReplyListModel model = new ProductReviewReplyListModel()
            {
                ProductReviewReplyList = AdminProductReviews.AdminGetProductReviewReplyList(reviewId),
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?reviewId={1}", Url.Action("productreviewreplylist"), reviewId));
            return View(model);
        }

        /// <summary>
        /// 改变商品评价的状态
        /// </summary>
        public ActionResult ChangeProductReviewState(int reviewId = -1, int state = -1)
        {
            bool result = AdminProductReviews.ChangeProductReviewState(reviewId, state);
            if (result)
            {
                AddMallAdminLog("修改商品评价状态", "修改商品评价状态,商品评价ID和状态为:" + reviewId + "_" + state);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 删除商品评价
        /// </summary>
        public ActionResult DelProductReview(int reviewId)
        {
            AdminProductReviews.DeleteProductReviewById(reviewId);
            AddMallAdminLog("删除商品评价", "删除商品评价,商品评价ID为:" + reviewId);
            return PromptView("商品评价删除成功");
        }

        /// <summary>
        /// 添加商品评价
        /// </summary>
        [HttpGet]
        public ActionResult AddProductReview()
        {
            AddProductReviewModel model = new AddProductReviewModel();

            string backUrl = MallUtils.GetMallAdminRefererCookie();
            if (backUrl.Length == 0 || backUrl == "/malladmin/home/mallruninfo")
            {
                backUrl = Url.Action("productreviewlist");
                MallUtils.SetAdminRefererCookie(backUrl);
            }
            ViewData["referer"] = backUrl;
            return View(model);
        }

        /// <summary>
        /// 添加商品评价
        /// </summary>
        [HttpPost]
        public ActionResult AddProductReview(AddProductReviewModel model)
        {

            if (ModelState.IsValid)
            {
                ProductReviewInfo reviewInfo = new ProductReviewInfo()
                {
                    Pid = model.Pid,
                    Uid = 0,
                    OPRId = 0,
                    Oid = 0,
                    ParentId = 0,
                    State = 0,
                    Star = model.Star,
                    Quality = 0,
                    Message = model.Message,
                    BuyTime = model.BuyTime,
                    ReviewTime = model.ReviewTime,
                    PayCredits = 0,
                    IP = WebHelper.GetIP(),
                    ShowNickName = model.ShowNickName

                };

                PartProductInfo info = Products.GetPartProductById(model.Pid);
                if (info == null)
                {
                    return PromptView("商品不存在");
                }
                reviewInfo.StoreId = info.StoreId;
                reviewInfo.PName = info.Name;
                reviewInfo.PShowImg = info.ShowImg;

                ProductReviews.ReviewProduct(reviewInfo);
                string backUrl = Url.Action("productreviewlist");

                return PromptView(backUrl, "商品评价添加成功");
            }
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }
    }
}
