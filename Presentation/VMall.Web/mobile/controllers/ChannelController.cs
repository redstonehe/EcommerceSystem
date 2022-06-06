using System;
using System.Text;
using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Mobile.Models;

namespace VMall.Web.Mobile.Controllers
{
    public class ChannelController : BaseMobileController
    {
        GroupProducts bllGroupProducts = new GroupProducts();
        //
        // GET: /Channel/

        public ActionResult Index()
        {
            //频道id
            int chId = GetRouteInt("chId");
            if (chId == 0)
                chId = WebHelper.GetQueryInt("chId");
            //组id
            int gid = GetRouteInt("gid");
            if (gid == 0)
                gid = WebHelper.GetQueryInt("gid");
            int cateId = GetRouteInt("cateId");
            if (cateId == 0)
                cateId = WebHelper.GetQueryInt("cateId");
            //品牌id
            int brandId = GetRouteInt("brandId");
            if (brandId == 0)
                brandId = WebHelper.GetQueryInt("brandId");
            //筛选价格
            int filterPrice = GetRouteInt("filterPrice");
            if (filterPrice == 0)
                filterPrice = WebHelper.GetQueryInt("filterPrice");

            //是否只显示有货
            int onlyStock = GetRouteInt("onlyStock");
            if (onlyStock == 0)
                onlyStock = WebHelper.GetQueryInt("onlyStock");
            //排序列
            int sortColumn = GetRouteInt("sortColumn");
            if (sortColumn == 0)
                sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = GetRouteInt("sortDirection");
            if (sortDirection == 0)
                sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = GetRouteInt("page");
            if (page == 0)
                page = WebHelper.GetQueryInt("page");

            //分类信息
            ChannelInfo channelInfo = Channel.GetChannelById(chId);
            if (channelInfo == null)
                return PromptView("/", "此频道不存在");

            GroupProductInfo groupInfo = bllGroupProducts.GetModel(gid);

            //分类关联品牌列表
            List<BrandInfo> brandList = Channel.GetChannelBrandList(chId);

            //分类导航
            List<CategoryInfo> AllCateory = Categories.GetCategoryList();
            List<CategoryInfo> frist = Channel.GetChannelFristCateoryList(chId);
            List<CategoryInfo> second = new List<CategoryInfo>();
            List<CategoryInfo> third = new List<CategoryInfo>();
            List<CategoryInfo> checkCateory = new List<CategoryInfo>();
            CategoryInfo currentCateory = new CategoryInfo();
            StringBuilder CatePath = new StringBuilder();
            if (cateId > 0)
            {
                currentCateory = Categories.GetCategoryById(cateId);

                if (currentCateory != null)
                {
                    string[] cateNav = StringHelper.SplitString(currentCateory.Path);
                    foreach (var item in cateNav)
                    {
                        CategoryInfo itemInfo = AllCateory.Find(x => x.CateId == TypeHelper.StringToInt(item));

                        checkCateory.Add(itemInfo);
                    }
                    if (currentCateory.ParentId == 0 && currentCateory.Layer == 1)
                    {
                        second = AllCateory.FindAll(x => x.ParentId == cateId);
                        List<CategoryInfo> tmpCate = new List<CategoryInfo>();
                        second.ForEach(x =>
                        {
                            tmpCate.AddRange(AllCateory.FindAll(k => k.ParentId == x.CateId));
                        });
                        CatePath.Append(currentCateory.Path);
                        if (second.Count > 0)
                        {
                            CatePath.Append(",");
                            CatePath.Append(string.Join(",", second.Select(x => x.Path)));
                        }
                        if (tmpCate.Count > 0)
                        {
                            CatePath.Append(",");
                            CatePath.Append(string.Join(",", tmpCate.Select(x => x.Path)));
                        }
                    }
                    if (currentCateory.Layer == 2)
                    {
                        CategoryInfo pCateory = Categories.GetCategoryById(currentCateory.ParentId);
                        second = AllCateory.FindAll(x => x.ParentId == pCateory.CateId);
                        third = AllCateory.FindAll(x => x.ParentId == currentCateory.CateId);
                        CatePath.Append(currentCateory.CateId);
                        if (third.Count > 0)
                        {
                            CatePath.Append(",");
                            CatePath.Append(string.Join(",", third.Select(x => x.Path)));
                        }
                    }
                    if (currentCateory.Layer == 3)
                    {
                        string[] path = currentCateory.Path.Split(',');
                        second = AllCateory.FindAll(x => x.ParentId == TypeHelper.StringToInt(path[0]));
                        third = third = AllCateory.FindAll(x => x.ParentId == currentCateory.ParentId);
                        CatePath.Append(currentCateory.CateId);
                    }
                }
            }

            #region MyRegion
            
          
            ////分类筛选属性及其值列表
            //List<KeyValuePair<AttributeInfo, List<AttributeValueInfo>>> cateAAndVList = Categories.GetCategoryFilterAAndVList(chId);

            ////分类价格范围列表
            //string[] catePriceRangeList = StringHelper.SplitString(categoryInfo.PriceRange, "\r\n");

            ////筛选属性处理
            //List<int> attrValueIdList = new List<int>();
            //string[] filterAttrValueIdList = StringHelper.SplitString(filterAttr, "-");
            //if (filterAttrValueIdList.Length != cateAAndVList.Count)//当筛选属性和分类的筛选属性数目不对应时，重置筛选属性
            //{
            //    if (cateAAndVList.Count == 0)
            //    {
            //        filterAttr = "0";
            //    }
            //    else
            //    {
            //        int count = cateAAndVList.Count;
            //        StringBuilder sb = new StringBuilder();
            //        for (int i = 0; i < count; i++)
            //            sb.Append("0-");
            //        filterAttr = sb.Remove(sb.Length - 1, 1).ToString();
            //    }
            //}
            //else
            //{
            //    foreach (string attrValueId in filterAttrValueIdList)
            //    {
            //        int temp = TypeHelper.StringToInt(attrValueId);
            //        if (temp > 0) attrValueIdList.Add(temp);
            //    }
            //}
            #endregion

            string[] catePriceRangeList = StringHelper.SplitString(currentCateory.PriceRange, "\r\n");
            //int filterPrice = 0;
            //分页对象
            PageModel pageModel = new PageModel(20, page, Channel.GetChannelProductCount(chId, CatePath.ToString(), brandId, catePriceRangeList, onlyStock, filterPrice,gid));
            //视图对象
            ChannelModel model = new ChannelModel()
            {
                ChId = chId,
                GId = gid,
                CateId = cateId,
                BrandId = brandId,
                FilterPrice = filterPrice,
                OnlyStock = onlyStock,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                ChannelInfo = channelInfo,
                GroupProductInfo = groupInfo,
                BrandList = brandList,
                CatePriceRangeList = catePriceRangeList,
                PageModel = pageModel,
                ProductList = Channel.GetChannelProductList(pageModel.PageSize, pageModel.PageNumber, chId, CatePath.ToString(), brandId, filterPrice, catePriceRangeList, onlyStock, sortColumn, sortDirection,"",gid)
            };
            if (page >= 2)
                return PartialView("ajaxchannel", model);
            return View(model);
        }


        /// <summary>
        /// 频道异步
        /// </summary>
        public ActionResult AjaxChannel()
        {
            //频道id
            int chId = GetRouteInt("chId");
            if (chId == 0)
                chId = WebHelper.GetQueryInt("chId");
            //品牌id
            int brandId = WebHelper.GetQueryInt("brandId");
            //筛选价格
            int filterPrice = WebHelper.GetQueryInt("filterPrice");
            //筛选属性
            //string filterAttr = WebHelper.GetQueryString("filterAttr");
            //是否只显示有货
            int onlyStock = WebHelper.GetQueryInt("onlyStock");
            //排序列
            int sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = WebHelper.GetQueryInt("page");

            //分类信息
            //CategoryInfo categoryInfo = Categories.GetCategoryById(cateId);

            //频道信息
            ChannelInfo channelInfo = Channel.GetChannelById(chId);

            //分类关联品牌列表
            List<BrandInfo> brandList = Channel.GetChannelBrandList(chId);

            if (channelInfo == null)
                return PromptView("/", "此频道不存在");
            //分类价格范围列表
            string[] catePriceRangeList = new string[0]; //StringHelper.SplitString(categoryInfo.PriceRange, "\r\n");
            //筛选属性处理
            //List<int> attrValueIdList = new List<int>();
            //string[] filterAttrValueIdList = StringHelper.SplitString(filterAttr, "-");
            //foreach (string attrValueId in filterAttrValueIdList)
            //{
            //    int temp = TypeHelper.StringToInt(attrValueId);
            //    if (temp > 0) attrValueIdList.Add(temp);
            //}

            //分页对象
            PageModel pageModel = new PageModel(20, page, Channel.GetChannelProductCount(chId, "", brandId, catePriceRangeList, onlyStock, filterPrice));
            //视图对象
            ChannelModel model = new ChannelModel()
            {
                ChId = chId,
                BrandId = brandId,
                FilterPrice = filterPrice,
                OnlyStock = onlyStock,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                ChannelInfo = channelInfo,
                BrandList = brandList,
                CatePriceRangeList = catePriceRangeList,
                PageModel = pageModel,
                ProductList = Channel.GetChannelProductList(pageModel.PageSize, pageModel.PageNumber, chId, "", brandId, filterPrice, catePriceRangeList, onlyStock, sortColumn, sortDirection)
            };

            return View(model);
        }

        public ActionResult FSList()
        {
            //排序列
            int sortColumn = GetRouteInt("sortColumn");
            if (sortColumn == 0)
                sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = GetRouteInt("sortDirection");
            if (sortDirection == 0)
                sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = GetRouteInt("page");
            if (page == 0)
                page = WebHelper.GetQueryInt("page");



            //分页对象
            PageModel pageModel = new PageModel(20, page, Channel.GetFlashSaleProductCount());
            //视图对象
            FlashSaleModel model = new FlashSaleModel()
            {
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                PageModel = pageModel,
                ProductList = Channel.GetFlashSaleProductList(pageModel.PageSize, pageModel.PageNumber, sortColumn, sortDirection)
            };

            return View(model);
        }
        public ActionResult AjaxFSList()
        {
            //排序列
            int sortColumn = GetRouteInt("sortColumn");
            if (sortColumn == 0)
                sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = GetRouteInt("sortDirection");
            if (sortDirection == 0)
                sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = GetRouteInt("page");
            if (page == 0)
                page = WebHelper.GetQueryInt("page");



            //分页对象
            PageModel pageModel = new PageModel(20, page, Channel.GetFlashSaleProductCount());
            //视图对象
            FlashSaleModel model = new FlashSaleModel()
            {
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                PageModel = pageModel,
                ProductList = Channel.GetFlashSaleProductList(pageModel.PageSize, pageModel.PageNumber, sortColumn, sortDirection)
            };

            return View("ajaxfs", model);
        }

        /// <summary>
        /// 套餐促销列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SuitList()
        {
            //排序列
            int sortColumn = GetRouteInt("sortColumn");
            if (sortColumn == 0)
                sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = GetRouteInt("sortDirection");
            if (sortDirection == 0)
                sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = GetRouteInt("page");
            if (page == 0)
                page = WebHelper.GetQueryInt("page");
            string orderFiled = string.Empty;
            string orderDisply = string.Empty;
            switch (sortColumn)
            {
                case 0:
                    orderFiled = " starttime1 ";
                    break;
                case 1:
                    orderFiled = " starttime1 ";
                    break;
                default:
                    orderFiled = " starttime1 ";
                    break;
            }
            switch (sortDirection)
            {
                case 0:
                    orderDisply = " ASC ";
                    break;
                case 1:
                    orderDisply = " DESC ";
                    break;
                default:
                    orderDisply = " ASC ";
                    break;
            }

            string condition = " state =1 and starttime1<=getdate() and endtime1>=getdate()";

            //分页对象
            PageModel pageModel = new PageModel(100, page, new SuitPromotions().GetCount(condition));
            List<SuitPromotionInfo> suitList = new SuitPromotions().GetList(condition, pageModel.PageSize, pageModel.PageIndex, "", orderFiled + orderDisply);
            //视图对象
            SuitListModel model = new SuitListModel()
            {
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                PageModel = pageModel,
                ProductList = suitList,
                SuitProductList = Promotions.GetAllSuitPromotion(suitList)

            };

            return View(model);
        }

        public ActionResult SubIndex(int chId)
        {
            List<GroupProductInfo> GroupList = bllGroupProducts.GetList(string.Format(" [ChannelId]={0} AND [State]=1 AND [StartTime]<='{1}' AND [EndTime]>='{2}'", chId, DateTime.Now, DateTime.Now));
            //频道信息
            ChannelInfo channelInfo = Channel.GetChannelById(chId);
            if (channelInfo == null)
                return PromptView("/", "此频道不存在");
            ChannelIndexModel model = new ChannelIndexModel();
            model.GroupProductList = GroupList;
            model.ChannelInfo = channelInfo;
            model.ChId = chId;

            return View(model);
        }
        /// <summary>
        /// 和治友德主页
        /// </summary>
        /// <returns></returns>
        public ActionResult FoHow()
        {
            return View("fohow");
        }
        /// <summary>
        /// 全球购主页
        /// </summary>
        /// <returns></returns>
        public ActionResult qqg() {
            return View("qqg");
        }
        /// <summary>
        /// 汇生鲜主页
        /// </summary>
        /// <returns></returns>
        public ActionResult Fresh()
        {
            return View("fresh");
        }
        /// <summary>
        /// 汇生鲜主页
        /// </summary>
        /// <returns></returns>
        public ActionResult HBArea()
        {
            return View("hbarea");
        }
        public ActionResult YouXuan(int chId = 2)
        {
            List<GroupProductInfo> GroupList = bllGroupProducts.GetList(string.Format(" [ChannelId]={0} AND [State]=1 AND [StartTime]<='{1}' AND [EndTime]>='{2}'", chId, DateTime.Now, DateTime.Now));
            //频道信息
            ChannelInfo channelInfo = Channel.GetChannelById(chId);
            if (channelInfo == null)
                return PromptView("/", "此频道不存在");
            ChannelIndexModel model = new ChannelIndexModel();
            model.GroupProductList = GroupList;
            model.ChannelInfo = channelInfo;
            model.ChId = chId;

            return View(model);
        }
    }
}
