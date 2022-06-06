using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Mobile.Models;

namespace VMall.Web.Mobile.Controllers
{
    /// <summary>
    /// 商城目录控制器类
    /// </summary>
    public partial class CatalogController : BaseMobileController
    {
        private static string dirSaleApiUrl = WebSiteConfig.DirsaleApiUrl;

        /// <summary>
        /// 商品
        /// </summary>
        public ActionResult List()
        {
            //分类id
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
            //筛选属性
            string filterAttr = GetRouteString("filterAttr");
            if (filterAttr.Length == 0)
                filterAttr = WebHelper.GetQueryString("filterAttr");
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
            CategoryInfo categoryInfo = Categories.GetCategoryById(cateId);
            if (categoryInfo == null)
                return PromptView("/", "此分类不存在");

            //子分类列表
            List<CategoryInfo> ChildrenCateory = Categories.GetCategoryList().FindAll(x => x.ParentId == cateId);

            //分类关联品牌列表
            List<BrandInfo> brandList = Categories.GetCategoryBrandList(cateId);
            //分类筛选属性及其值列表
            List<KeyValuePair<AttributeInfo, List<AttributeValueInfo>>> cateAAndVList = Categories.GetCategoryFilterAAndVList(cateId);
            //分类价格范围列表
            string[] catePriceRangeList = StringHelper.SplitString(categoryInfo.PriceRange, "\r\n");

            //筛选属性处理
            List<int> attrValueIdList = new List<int>();
            string[] filterAttrValueIdList = StringHelper.SplitString(filterAttr, "-");
            if (filterAttrValueIdList.Length != cateAAndVList.Count)//当筛选属性和分类的筛选属性数目不对应时，重置筛选属性
            {
                if (cateAAndVList.Count == 0)
                {
                    filterAttr = "0";
                }
                else
                {
                    int count = cateAAndVList.Count;
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < count; i++)
                        sb.Append("0-");
                    filterAttr = sb.Remove(sb.Length - 1, 1).ToString();
                }
            }
            else
            {
                foreach (string attrValueId in filterAttrValueIdList)
                {
                    int temp = TypeHelper.StringToInt(attrValueId);
                    if (temp > 0) attrValueIdList.Add(temp);
                }
            }

            //分页对象
            PageModel pageModel = new PageModel(20, page, Products.GetCategoryProductCount(cateId, brandId, filterPrice, catePriceRangeList, attrValueIdList, onlyStock));
            //视图对象
            CategoryModel model = new CategoryModel()
            {
                ChildrenCateory = ChildrenCateory,
                CateId = cateId,
                BrandId = brandId,
                FilterPrice = filterPrice,
                FilterAttr = filterAttr,
                OnlyStock = onlyStock,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                CategoryInfo = categoryInfo,
                BrandList = brandList,
                CatePriceRangeList = catePriceRangeList,
                AAndVList = cateAAndVList,
                PageModel = pageModel,
                ProductList = Products.GetCategoryProductList(pageModel.PageSize, pageModel.PageNumber, cateId, brandId, filterPrice, catePriceRangeList, attrValueIdList, onlyStock, sortColumn, sortDirection)
            };
            if (page >= 2)
                return PartialView("listAjax", model);
            return View(model);
            //return View("list");
        }
        /// <summary>
        /// 商品
        /// </summary>
        public ActionResult List2()
        {

            return View("list2");
        }
        
        /// <summary>
        /// 商品
        /// </summary>
        //public ActionResult Product()
        //{
        //    //商品id
        //    int pid = GetRouteInt("pid");
        //    if (pid == 0)
        //        pid = WebHelper.GetQueryInt("pid");
        //    return RedirectToAction("item", "catalog", new { itemid = pid, uid = WorkContext.Uid });
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Product()
        {
            //商品id
            int pid = GetRouteInt("pid");
            if (pid == 0)
                pid = WebHelper.GetQueryInt("pid");
            //获取推荐Uid
            int uid = GetRouteInt("from_uid");
            if (uid == 0)
                uid = WebHelper.GetQueryInt("from_uid");
            int cookpuid = TypeHelper.StringToInt(WebHelper.GetCookie("puid"));
            if (uid <= 0)
            {
                uid = cookpuid;
            }

            //不登录情况下保存url的uid来锁定推荐关系
            if (WorkContext.Uid < 1)
            {
                WebHelper.SetCookie("puid", uid.ToString(), 60 * 24 * 30);
            }

            //判断商品是否存在
            ProductInfo productInfo = Products.GetProductById(pid);

            //PartProductInfo productInfo = Products.GetPartProductById(pid);
            if (productInfo == null)
                return RedirectToAction("OutProduct", "Catalog", new RouteValueDictionary { { "pid", pid } });
            //return PromptView(Url.Action("index", "home"), "您访问的商品不存在");
            if (WorkContext.IsDirSaleUser && productInfo.Pid == 3235)
                productInfo.MinBuyCount = 200;
            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(productInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return PromptView(Url.Action("index", "home"), "您访问的商品不存在");
            if (storeInfo.MallSource != 0)
                return PromptView(Url.Action("index", "home"), "您访问的商品不在此平台销售");
            //商品存在时
            ProductModel model = new ProductModel();
            //商品id
            model.Pid = pid;
            //商品信息
            productInfo.ShowImg = productInfo.ShowImg;
            model.ProductInfo = productInfo;
            //商品分类
            model.CategoryInfo = Categories.GetCategoryById(productInfo.CateId);
            //商品品牌
            model.BrandInfo = Brands.GetBrandById(productInfo.BrandId);
            //店铺信息
            model.StoreInfo = storeInfo;
            //商品图片列表
            model.ProductImageList = Products.GetProductImageList(pid);
            //model.ProductImageList.ForEach(p => p.ShowImg =  p.ShowImg);
            //商品SKU列表
            model.ProductSKUList = Products.GetProductSKUListBySKUGid(productInfo.SKUGid);
            //商品库存数量
            model.StockNumber = Products.GetProductStockNumberByPid(pid);

            //单品促销
            model.SinglePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(pid, DateTime.Now);
            SinglePromotionInfo singleInfo = Promotions.GetVailiSingleByPidAndendTime(pid, DateTime.Now);
            if (singleInfo != null)
            {
                model.ProductInfo.HaiMi = singleInfo.PromoHaiMi;
                model.ProductInfo.PV = singleInfo.PromoPV;
                model.ProductInfo.HongBaoCut = singleInfo.PromoHongBaoCut;
                //model.ProductInfo.ShopPrice = Promotions.ComputeDiscountPrice(model.ProductInfo.ShopPrice, model.SinglePromotionInfo, model.ProductInfo.MarketPrice);
            }
            //单品秒杀
            model.FlashSaleInfo = Promotions.GetFlashSaleInfoByPidAndEndTime(pid, DateTime.Now);

            //买送促销活动列表
            model.BuySendPromotionList = Promotions.GetBuySendPromotionList(productInfo.StoreId, pid, DateTime.Now);
            //赠品促销活动
            model.GiftPromotionInfo = Promotions.GetGiftPromotionByPidAndTime(pid, DateTime.Now);
            //赠品列表
            if (model.GiftPromotionInfo != null)
                model.ExtGiftList = Promotions.GetExtGiftList(model.GiftPromotionInfo.PmId);
            //套装商品列表
            model.SuitProductList = Promotions.GetProductAllSuitPromotion(pid, DateTime.Now);
            //满赠促销活动
            model.FullSendPromotionInfo = Promotions.GetFullSendPromotionByStoreIdAndPidAndTime(productInfo.StoreId, pid, DateTime.Now);
            //满减促销活动
            model.FullCutPromotionInfo = Promotions.GetFullCutPromotionByStoreIdAndPidAndTime(productInfo.StoreId, pid, DateTime.Now);

            //广告语
            model.Slogan = model.SinglePromotionInfo == null ? "" : (model.FlashSaleInfo == null ? model.SinglePromotionInfo.Slogan : model.FlashSaleInfo.Slogan);
            //商品促销信息
            model.PromotionMsg = Promotions.GeneratePromotionMsg(model.SinglePromotionInfo, model.BuySendPromotionList, model.FullSendPromotionInfo, model.FullCutPromotionInfo, WorkContext.PartUserInfo);
            //商品折扣价格
            model.DiscountPrice = Promotions.ComputeDiscountPrice(model.ProductInfo.ShopPrice, model.SinglePromotionInfo, model.ProductInfo.MarketPrice, 1);
            //是否收藏
            model.IsFav = FavoriteProducts.IsExistFavoriteProduct(WorkContext.Uid,pid);

            int reviewCount = ProductReviews.GetProductReviewCount(pid, 0);
            ViewData["reviewCount"] = reviewCount;

            //关联商品列表
            model.RelateProductList = Products.GetRelateProductList(pid);

            //更新浏览历史
            if (WorkContext.Uid > 0)
                Asyn.UpdateBrowseHistory(WorkContext.Uid, pid);
            //更新商品统计
            Asyn.UpdateProductStat(pid, WorkContext.RegionId);

            return View("Product", model);
        }

        /// <summary>
        /// 下架商品
        /// </summary>
        public ActionResult OutProduct()
        {
            //if (WebHelper.IsMobile() || WebHelper.IsWeChatBrowser())
            //    return RedirectToAction("Product", "Catalog", new RouteValueDictionary { { "area", "mob" } });
            //商品id
            int pid = GetRouteInt("pid");
            if (pid == 0)
                pid = WebHelper.GetQueryInt("pid");

            //判断下架商品是否存在
            ProductInfo productInfo = Products.GetProductModelByWhere(string.Format(" state=1 and pid={0}", pid));
            if (productInfo == null)
                return PromptView("/", "您访问的商品不存在");
            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(productInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return PromptView("/", "您访问的商品不存在");
            if (storeInfo.MallSource != 0)
                return PromptView("/", "您访问的商品不在此平台销售");
            //商品存在时
            ProductModel model = new ProductModel();
            //商品id
            model.Pid = pid;
            //商品信息
            model.ProductInfo = productInfo;
            //商品分类
            model.CategoryInfo = Categories.GetCategoryById(productInfo.CateId);
            //商品品牌
            model.BrandInfo = Brands.GetBrandById(productInfo.BrandId);

            //关联商品列表
            model.RelateProductList = Products.GetStoreSaleProductList(10, storeInfo.StoreId, 0);

            return View(model);
        }

        /// <summary>
        /// 商品细节
        /// </summary>
        public ActionResult ProductDetails()
        {
            //商品id
            int pid = GetRouteInt("pid");
            if (pid == 0)
                pid = WebHelper.GetQueryInt("pid");

            //判断商品是否存在
            ProductInfo productInfo = Products.GetProductById(pid);
            if (productInfo == null)
                return PromptView(Url.Action("index", "home"), "您访问的商品不存在");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(productInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return PromptView(Url.Action("index", "home"), "您访问的商品不存在");

            //商品存在时
            ProductDetailsModel model = new ProductDetailsModel();
            //商品id
            model.Pid = pid;
            //商品信息
            model.ProductInfo = productInfo;
            //扩展商品属性列表
            model.ExtProductAttributeList = Products.GetExtProductAttributeList(pid);
            return View(model);
        }

        /// <summary>
        /// 商品套装列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductSuitList()
        {
            //商品id
            int pid = GetRouteInt("pid");
            if (pid == 0)
                pid = WebHelper.GetQueryInt("pid");

            //判断商品是否存在
            PartProductInfo productInfo = Products.GetPartProductById(pid);
            if (productInfo == null)
                return PromptView(Url.Action("index", "home"), "您访问的商品不存在");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(productInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return PromptView(Url.Action("index", "home"), "您访问的商品不存在");


            List<KeyValuePair<SuitPromotionInfo, List<ExtSuitProductInfo>>> suitProductList = Promotions.GetProductAllSuitPromotion(pid, DateTime.Now);
            if (suitProductList.Count == 0)
                return PromptView(Url.Action("product", new RouteValueDictionary { { "pid", pid } }), "此商品没有套装");

            ProductSuitListModel model = new ProductSuitListModel();
            //商品id
            model.Pid = pid;
            //商品信息
            model.ProductInfo = productInfo;
            //套装商品列表
            model.SuitProductList = suitProductList;

            return View(model);
        }

        /// <summary>
        /// 分类
        /// </summary>
        public ActionResult Category()
        {
            //分类id
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
            //筛选属性
            string filterAttr = GetRouteString("filterAttr");
            if (filterAttr.Length == 0)
                filterAttr = WebHelper.GetQueryString("filterAttr");
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
            CategoryInfo categoryInfo = Categories.GetCategoryById(cateId);
            if (categoryInfo == null)
                return PromptView("/", "此分类不存在");

            //子分类列表
            List<CategoryInfo> ChildrenCateory = Categories.GetCategoryList().FindAll(x => x.ParentId == cateId);

            //分类关联品牌列表
            List<BrandInfo> brandList = Categories.GetCategoryBrandList(cateId);
            //分类筛选属性及其值列表
            List<KeyValuePair<AttributeInfo, List<AttributeValueInfo>>> cateAAndVList = Categories.GetCategoryFilterAAndVList(cateId);
            //分类价格范围列表
            string[] catePriceRangeList = StringHelper.SplitString(categoryInfo.PriceRange, "\r\n");

            //筛选属性处理
            List<int> attrValueIdList = new List<int>();
            string[] filterAttrValueIdList = StringHelper.SplitString(filterAttr, "-");
            if (filterAttrValueIdList.Length != cateAAndVList.Count)//当筛选属性和分类的筛选属性数目不对应时，重置筛选属性
            {
                if (cateAAndVList.Count == 0)
                {
                    filterAttr = "0";
                }
                else
                {
                    int count = cateAAndVList.Count;
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < count; i++)
                        sb.Append("0-");
                    filterAttr = sb.Remove(sb.Length - 1, 1).ToString();
                }
            }
            else
            {
                foreach (string attrValueId in filterAttrValueIdList)
                {
                    int temp = TypeHelper.StringToInt(attrValueId);
                    if (temp > 0) attrValueIdList.Add(temp);
                }
            }

            //分页对象
            PageModel pageModel = new PageModel(20, page, Products.GetCategoryProductCount(cateId, brandId, filterPrice, catePriceRangeList, attrValueIdList, onlyStock));
            //视图对象
            CategoryModel model = new CategoryModel()
            {
                ChildrenCateory = ChildrenCateory,
                CateId = cateId,
                BrandId = brandId,
                FilterPrice = filterPrice,
                FilterAttr = filterAttr,
                OnlyStock = onlyStock,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                CategoryInfo = categoryInfo,
                BrandList = brandList,
                CatePriceRangeList = catePriceRangeList,
                AAndVList = cateAAndVList,
                PageModel = pageModel,
                ProductList = Products.GetCategoryProductList(pageModel.PageSize, pageModel.PageNumber, cateId, brandId, filterPrice, catePriceRangeList, attrValueIdList, onlyStock, sortColumn, sortDirection)
            };
            if (page >= 2)
                return PartialView("categoryAjax", model);
            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchIndex()
        {
            MallSearchModel model = new MallSearchModel();
            model.HotSaleProductList = Products.GetMallSaleProductList(5);
            return View(model);
        }


        /// <summary>
        /// 搜索
        /// </summary>
        public ActionResult Search()
        {
            //搜索词
            string keyword = WebHelper.GetQueryString("keyword");
            WorkContext.SearchWord = WebHelper.HtmlEncode(keyword);
            if (keyword.Length == 0)
                return PromptView(WorkContext.UrlReferrer, "请输入搜索词");
            if (!SecureHelper.IsSafeSqlString(keyword))
                return PromptView(WorkContext.UrlReferrer, "您搜索的商品不存在");

            //异步保存搜索历史
            Asyn.UpdateSearchHistory(WorkContext.Uid, keyword);

            //判断搜索词是否为分类名称，如果是则重定向到分类页面
            //int cateId = Categories.GetCateIdByName(keyword);
            //if (cateId > 0)
            //{
            //    return Redirect(Url.Action("category", new RouteValueDictionary { { "cateId", cateId } }));
            //}
            //else
            //{
            int cateId = WebHelper.GetQueryInt("cateId");
            //}

            //分类列表
            List<CategoryInfo> categoryList = null;
            //分类信息
            CategoryInfo categoryInfo = null;
            //品牌列表
            List<BrandInfo> brandList = null;

            //品牌id
            //int brandId = Brands.GetBrandIdByName(keyword);
            //if (brandId > 0)//当搜索词为品牌名称时
            //{
            //    //获取品牌相关的分类
            //    categoryList = Brands.GetBrandCategoryList(brandId);

            //    //由于搜索结果的展示是以分类为基础的，所以当分类不存在时直接将搜索结果设为“搜索的商品不存在”
            //    if (categoryList.Count == 0)
            //        return PromptView(WorkContext.UrlReferrer, "您搜索的商品不存在");

            //    if (cateId > 0)
            //    {
            //        categoryInfo = Categories.GetCategoryById(cateId);
            //    }
            //    else
            //    {
            //        //当没有进行分类的筛选时，将分类列表中的首项设为当前选中的分类
            //        categoryInfo = categoryList[0];
            //        cateId = categoryInfo.CateId;
            //    }
            //    brandList = new List<BrandInfo>();
            //    brandList.Add(Brands.GetBrandById(brandId));
            //}
            //else//当搜索词为商品关键词时
            //{
            //获取商品关键词相关的分类
            categoryList = Searches.GetCategoryListByKeyword(keyword);

            //由于搜索结果的展示是以分类为基础的，所以当分类不存在时直接将搜索结果设为“搜索的商品不存在”
            if (categoryList.Count == 0)
                return PromptView(WorkContext.UrlReferrer, "您搜索的商品不存在");

            if (cateId > 0)
            {
                categoryInfo = Categories.GetCategoryById(cateId);
            }
            else
            {
                categoryInfo = categoryList[0];
                cateId = categoryInfo.CateId;
            }
            //根据商品关键词获取分类相关的品牌
            brandList = Searches.GetCategoryBrandListByKeyword(cateId, keyword);
            if (brandList.Count == 0)
                return PromptView(WorkContext.UrlReferrer, "您搜索的商品不存在");
            int brandId = WebHelper.GetQueryInt("brandId");
            //}
            //最后再检查一遍分类是否存在
            if (categoryInfo == null)
                return PromptView(WorkContext.UrlReferrer, "您搜索的商品不存在");

            //筛选价格
            int filterPrice = WebHelper.GetQueryInt("filterPrice");
            //筛选属性
            string filterAttr = WebHelper.GetQueryString("filterAttr");
            //是否只显示有货
            int onlyStock = WebHelper.GetQueryInt("onlyStock");
            //排序列
            int sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = WebHelper.GetQueryInt("page");

            //分类筛选属性及其值列表
            List<KeyValuePair<AttributeInfo, List<AttributeValueInfo>>> cateAAndVList = Categories.GetCategoryFilterAAndVList(cateId);
            //分类价格范围列表
            string[] catePriceRangeList = StringHelper.SplitString(categoryInfo.PriceRange, "\r\n");

            //筛选属性处理
            List<int> attrValueIdList = new List<int>();
            string[] filterAttrValueIdList = StringHelper.SplitString(filterAttr, "-");
            if (filterAttrValueIdList.Length != cateAAndVList.Count)//当筛选属性和分类的筛选属性数目不对应时，重置筛选属性
            {
                if (cateAAndVList.Count == 0)
                {
                    filterAttr = "0";
                }
                else
                {
                    int count = cateAAndVList.Count;
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < count; i++)
                        sb.Append("0-");
                    filterAttr = sb.Remove(sb.Length - 1, 1).ToString();
                }
            }
            else
            {
                foreach (string attrValueId in filterAttrValueIdList)
                {
                    int temp = TypeHelper.StringToInt(attrValueId);
                    if (temp > 0) attrValueIdList.Add(temp);
                }
            }


            //分页对象
            PageModel pageModel = new PageModel(20, page, Searches.GetSearchMallProductCount(keyword, cateId, brandId, filterPrice, catePriceRangeList, attrValueIdList, onlyStock));
            //视图对象
            MallSearchModel model = new MallSearchModel()
            {
                Word = keyword,
                CateId = cateId,
                BrandId = brandId,
                FilterPrice = filterPrice,
                FilterAttr = filterAttr,
                OnlyStock = onlyStock,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                CategoryList = categoryList,
                CategoryInfo = categoryInfo,
                BrandList = brandList,
                CatePriceRangeList = catePriceRangeList,
                AAndVList = cateAAndVList,
                PageModel = pageModel,
                ProductList = Searches.SearchMallProducts(pageModel.PageSize, pageModel.PageNumber, keyword, cateId, brandId, filterPrice, catePriceRangeList, attrValueIdList, onlyStock, sortColumn, sortDirection)
            };

            return View(model);
        }

        /// <summary>
        /// 搜索
        /// </summary>
        public ActionResult AjaxSearch()
        {
            //搜索词
            string keyword = WebHelper.GetQueryString("keyword");
            if (keyword.Length == 0)
                return Content("");
            if (!SecureHelper.IsSafeSqlString(keyword))
                return Content("");

            //分类id
            int cateId = WebHelper.GetQueryInt("cateId");
            //品牌id
            int brandId = WebHelper.GetQueryInt("brandId");
            //筛选价格
            int filterPrice = WebHelper.GetQueryInt("filterPrice");
            //筛选属性
            string filterAttr = WebHelper.GetQueryString("filterAttr");
            //是否只显示有货
            int onlyStock = WebHelper.GetQueryInt("onlyStock");
            //排序列
            int sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = WebHelper.GetQueryInt("page");

            //分类信息
            CategoryInfo categoryInfo = Categories.GetCategoryById(cateId);
            //分类价格范围列表
            string[] catePriceRangeList = StringHelper.SplitString(categoryInfo.PriceRange, "\r\n");
            //筛选属性处理
            List<int> attrValueIdList = new List<int>();
            string[] filterAttrValueIdList = StringHelper.SplitString(filterAttr, "-");
            foreach (string attrValueId in filterAttrValueIdList)
            {
                int temp = TypeHelper.StringToInt(attrValueId);
                if (temp > 0) attrValueIdList.Add(temp);
            }

            //分页对象
            PageModel pageModel = new PageModel(20, page, Searches.GetSearchMallProductCount(keyword, cateId, brandId, filterPrice, catePriceRangeList, attrValueIdList, onlyStock));
            //视图对象
            AjaxMallSearchModel model = new AjaxMallSearchModel()
            {
                PageModel = pageModel,
                ProductList = Searches.SearchMallProducts(pageModel.PageSize, pageModel.PageNumber, keyword, cateId, brandId, filterPrice, catePriceRangeList, attrValueIdList, onlyStock, sortColumn, sortDirection)
            };
            return PartialView("ajaxsearch", model);
            //return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 商品评价列表
        /// </summary>
        public ActionResult ProductReviewList()
        {
            int pid = WebHelper.GetQueryInt("pid");
            int type = WebHelper.GetQueryInt("type");
            int reviewType = WebHelper.GetQueryInt("reviewType");
            int page = WebHelper.GetQueryInt("page");

            //判断商品是否存在
            PartProductInfo productInfo = Products.GetPartProductById(pid);
            if (productInfo == null)
                return PromptView(Url.Action("index", "home"), "您访问的商品不存在");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(productInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return PromptView(Url.Action("index", "home"), "您访问的商品不存在");

            if (reviewType < 0 || reviewType > 3) reviewType = 0;

            PageModel pageModel = new PageModel(10, page, ProductReviews.GetProductReviewCount(pid, reviewType));
            ProductReviewListModel model = new ProductReviewListModel()
            {
                ProductInfo = productInfo,
                StoreInfo = storeInfo,
                ReviewType = reviewType,
                PageModel = pageModel,
                ProductReviewList = ProductReviews.GetProductReviewList(pid, reviewType, pageModel.PageSize, pageModel.PageNumber)
            };

            if (type == 2)
            {
                return PartialView("ajaxproductreviewlist", model);
            }
            return View(model);
        }

        /// <summary>
        /// 商品咨询列表
        /// </summary>
        public ActionResult ProductConsultList()
        {
            int pid = WebHelper.GetQueryInt("pid");
            int consultTypeId = WebHelper.GetQueryInt("consultTypeId");
            string consultMessage = WebHelper.GetQueryString("consultMessage");
            int page = WebHelper.GetQueryInt("page");

            //判断商品是否存在
            PartProductInfo productInfo = Products.GetPartProductById(pid);
            if (productInfo == null)
                return PromptView(Url.Action("index", "home"), "您访问的商品不存在");

            if (!SecureHelper.IsSafeSqlString(consultMessage))
                return PromptView(WorkContext.UrlReferrer, "您搜索的内容不存在");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(productInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return PromptView(Url.Action("index", "home"), "您访问的商品不存在");

            PageModel pageModel = new PageModel(10, page, ProductConsults.GetProductConsultCount(pid, consultTypeId, consultMessage));
            ProductConsultListModel model = new ProductConsultListModel()
            {
                ProductInfo = productInfo,
                StoreInfo = storeInfo,
                ConsultTypeId = consultTypeId,
                ConsultMessage = consultMessage,
                PageModel = pageModel,
                ProductConsultList = ProductConsults.GetProductConsultList(pageModel.PageSize, pageModel.PageNumber, pid, consultTypeId, consultMessage),
                ProductConsultTypeList = ProductConsults.GetProductConsultTypeList(),
                IsVerifyCode = CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages)
            };

            return View(model);
        }

       
    }
}
