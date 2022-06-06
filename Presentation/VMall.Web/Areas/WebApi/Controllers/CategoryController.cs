using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Areas.WebApi.Models;
using System.Text;

namespace VMall.Web.Areas.WebApi.Controllers
{
    public class CategoryController : BaseApiController
    {
        /// <summary>
        /// 分类列表
        /// </summary>
        /// <returns></returns>
        public ActionResult CategoryList(int cateId)
        {
            CategoryInfo categoryInfo = null;
            List<CategoryInfo> categoryList = Categories.GetCategoryList();
            if (cateId > 0)
            {
                categoryInfo = Categories.GetCategoryById(cateId, categoryList);
                if (categoryInfo != null)
                    categoryList = Categories.GetChildCategoryList(cateId, categoryInfo.Layer, categoryList);
                else
                    return AppResult(false, "分类不存在");
            }

            CategoryListModel model = new CategoryListModel();
            //model.CategoryInfo = categoryInfo;
            model.CategoryList = categoryList;

            return AppResult(true,"成功",model);
        }

        /// <summary>
        /// 分类产品列表
        /// </summary>
        public ActionResult CateProductList()
        {
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
            //每页显示记录数
            int pageSize = WebHelper.GetQueryInt("pageSize");
            if (pageSize <= 0) pageSize = 20;
            //当前页数
            int pageNumber = WebHelper.GetQueryInt("pageNumber");
            if (pageNumber <= 0) pageNumber = 1;

            CategoryModel categoryModel = new CategoryModel();

            //分类信息
            CategoryInfo categoryInfo = Categories.GetCategoryById(cateId);
            if (categoryInfo == null)
            {
                //categoryModel.State = "The product category does not exist";
                return AppResult(false,"分类不存在");
            }

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

            int productTotalCount = Products.GetCategoryProductCount(cateId, brandId, filterPrice, catePriceRangeList, attrValueIdList, onlyStock);
            List<StoreProductInfo> productList = Products.GetCategoryProductList(pageSize, pageNumber, cateId, brandId, filterPrice, catePriceRangeList, attrValueIdList, onlyStock, sortColumn, sortDirection);
            if (productList != null && productList.Count > 0)
            {
                productList.ForEach(p => p.ShowImg = @"http://www.hhwtop.com/upload/store/" + p.StoreId.ToString() + @"/product/show/thumb100_100/" + p.ShowImg);
            }

            categoryModel.State = "Success";
            categoryModel.ProductTotalCount = productTotalCount;
            categoryModel.ProductList = productList;
            return AppResult(true, "成功", new { ProductTotalCount = productTotalCount, ProductList = productList });
            //return ConvertObject2Json(categoryModel);
        }
    }
}
