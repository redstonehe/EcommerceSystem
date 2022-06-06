using System;
using System.Web.Mvc;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Models;
using System.Collections.Generic;

namespace VMall.Web.Controllers
{
    /// <summary>
    /// 充值控制器类
    /// </summary>
    public partial class ReChargeController : BaseWebController
    {
        public ActionResult index()
        {
            List<PartProductInfo> list = Products.GetProductListByWhere(string.Format(" [storeid] ={0}  AND [state]=0 ", TypeHelper.StringToInt(WebHelper.GetConfigSettings("ChongZhiStore"))));

            return View(list);

        }
        /// <summary>
        /// 详细信息
        /// </summary>
        public ActionResult Details()
        {
            //新闻id
            int newsId = GetRouteInt("newsId");
            if (newsId == 0)
                newsId = WebHelper.GetQueryInt("newsId");

            NewsInfo newsInfo = News.GetNewsById(newsId);
            if (newsInfo == null)
                return PromptView("/", "您访问的页面不存在");

            NewsModel model = new NewsModel();
            model.NewsInfo = newsInfo;
            model.NewsTypeList = News.GetNewsTypeList();

            return View(model);
        }

        /// <summary>
        /// 新闻列表
        /// </summary>
        public ActionResult List()
        {
            string newsTitle = WebHelper.GetQueryString("newsTitle");
            int newsTypeId = WebHelper.GetQueryInt("newsTypeId");
            int page = WebHelper.GetQueryInt("page");

            if (!SecureHelper.IsSafeSqlString(newsTitle))
                return PromptView(WorkContext.UrlReferrer, "您搜索的新闻不存在");

            string condition = News.GetNewsListCondition(newsTypeId, newsTitle);
            PageModel pageModel = new PageModel(10, page, News.GetNewsCount(condition));
            NewsListModel model = new NewsListModel()
            {
                PageModel = pageModel,
                NewsList = News.GetNewsList(pageModel.PageSize, pageModel.PageNumber, condition),
                NewsTitle = newsTitle,
                NewsTypeId = newsTypeId,
                NewsTypeList = News.GetNewsTypeList()
            };

            return View(model);
        }
    }
}
