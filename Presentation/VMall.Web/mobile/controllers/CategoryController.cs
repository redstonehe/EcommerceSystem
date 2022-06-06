using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Mobile.Models;

namespace VMall.Web.Mobile.Controllers
{
    /// <summary>
    /// 分类控制器类
    /// </summary>
    public partial class CategoryController : BaseMobileController
    {
        /// <summary>
        /// 分类列表
        /// </summary>
        /// <returns></returns>
        public ActionResult List(int chId = 1)
        {

            int cateId = GetRouteInt("cateId");
            if (cateId == 0)
                cateId = WebHelper.GetQueryInt("cateId");

            List<ChannelInfo> info = Channel.GetChannelList().FindAll(x=>x.State==1).ToList();

            List<CategoryInfo> frist = Channel.GetChannelFristCateoryList(chId);
            List<CategoryInfo> AllCateory = Categories.GetCategoryList();
            CategoryListModel model = new CategoryListModel();
            model.ChId = chId; 
            model.CategoryList = frist;
            model.ChannelList = info;
            model.AllCateory = AllCateory;
            return View(model);




            //CategoryInfo categoryInfo = null;
            //List<CategoryInfo> categoryList = Categories.GetCategoryList();
            //if (cateId > 0)
            //{
            //    categoryInfo = Categories.GetCategoryById(cateId, categoryList);
            //    if (categoryInfo != null)
            //        categoryList = Categories.GetChildCategoryList(cateId, categoryInfo.Layer, categoryList);
            //}

            //CategoryListModel model = new CategoryListModel();
            //model.CategoryInfo = categoryInfo;
            //model.CategoryList = categoryList;
            //model.ChannelList = info;
            //return View(model);
        }
    }
}
