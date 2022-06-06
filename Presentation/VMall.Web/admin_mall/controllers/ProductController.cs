using System;
using System.Web;
using System.Data;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;

namespace VMall.Web.MallAdmin.Controllers
{
    using VMall.Web.MallAdmin.bll;
    /// <summary>
    /// 商城后台商品控制器类
    /// </summary>
    public partial class ProductController : BaseMallAdminController
    {
        #region 商品列表
        /// <summary>
        /// 在售商品列表
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="productName">商品名称</param>
        /// <param name="categoryName">分类名称</param>
        /// <param name="brandName">品牌名称</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="cateId">分类id</param>
        /// <param name="brandId">品牌id</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult OnSaleProductList(string storeName, string productName, string categoryName, string brandName, string sortColumn, string sortDirection, int storeId = -1, int cateId = -1, int brandId = -1, int pageNumber = 1, int pageSize = 20, int pid = 0)
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            string condition = AdminProducts.AdminGetProductListCondition(storeId, 0, productName, cateId, brandId, (int)ProductState.OnSale, pid);
            sortColumn = "addtime";
            sortDirection = "DESC";
            string sort = AdminProducts.AdminGetProductListSort(sortColumn, sortDirection);

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminProducts.AdminGetProductCount(condition));

            ProductListModel model = new ProductListModel()
            {
                ProductList = AdminProducts.AdminGetProductList(pageModel.PageSize, pageModel.PageNumber, condition, sort),
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                StoreId = storeId,
                ProductName = productName,
                CateId = cateId,
                BrandId = brandId,
                Pid = pid,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "全部店铺" : storeName,
                CategoryName = string.IsNullOrWhiteSpace(categoryName) ? "全部分类" : categoryName,
                BrandName = string.IsNullOrWhiteSpace(brandName) ? "全部品牌" : brandName
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&storeId={5}&ProductName={6}&cateId={7}&brandId={8}&storeName={9}&categoryName={10}&brandName={11}&pid={12}",
                                                          Url.Action("onsaleproductlist"),
                                                          pageModel.PageNumber, pageModel.PageSize,
                                                          sortColumn, sortDirection,
                                                          storeId, productName, cateId, brandId,
                                                          storeName, categoryName, brandName, pid));
            return View(model);
        }

        /// <summary>
        /// 下架商品列表
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="productName">商品名称</param>
        /// <param name="categoryName">分类名称</param>
        /// <param name="brandName">品牌名称</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="cateId">分类id</param>
        /// <param name="brandId">品牌id</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult OutSaleProductList(string storeName, string productName, string categoryName, string brandName, string sortColumn, string sortDirection, int storeId = -1, int cateId = -1, int brandId = -1, int pageNumber = 1, int pageSize = 15, int pid = 0)
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            string condition = AdminProducts.AdminGetProductListCondition(storeId, 0, productName, cateId, brandId, (int)ProductState.OutSale, pid);
            sortColumn = "addtime";
            sortDirection = "DESC";
            string sort = AdminProducts.AdminGetProductListSort(sortColumn, sortDirection);

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminProducts.AdminGetProductCount(condition));

            ProductListModel model = new ProductListModel()
            {
                ProductList = AdminProducts.AdminGetProductList(pageModel.PageSize, pageModel.PageNumber, condition, sort),
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                StoreId = storeId,
                ProductName = productName,
                CateId = cateId,
                BrandId = brandId,
                Pid = pid,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "全部店铺" : storeName,
                CategoryName = string.IsNullOrWhiteSpace(categoryName) ? "全部分类" : categoryName,
                BrandName = string.IsNullOrWhiteSpace(brandName) ? "全部品牌" : brandName
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&storeId={5}&ProductName={6}&cateId={7}&brandId={8}&storeName={9}&categoryName={10}&brandName={11}&pid={12}",
                                                          Url.Action("outsaleproductlist"),
                                                          pageModel.PageNumber, pageModel.PageSize,
                                                          sortColumn, sortDirection,
                                                          storeId, productName, cateId, brandId,
                                                          storeName, categoryName, brandName, pid));
            return View(model);
        }

        /// <summary>
        /// 导出报表
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="consignee"></param>
        /// <param name="orderState"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult OutPutList(string productName, int storeId = -1, int cateId = -1, int brandId = -1, int pid = 0, int state = 0)
        {
            StringBuilder condition = new StringBuilder();
            condition.Append(" 1=1  ");
            if (state > -1)
                condition.AppendFormat(" AND a.[state]={0} ", (int)state);

            if (!string.IsNullOrWhiteSpace(productName))
                condition.AppendFormat(" AND a.[name] like '%{0}%' ", productName);

            if (cateId > 0)
                condition.AppendFormat(" AND a.[cateid] = {0} ", cateId);

            if (brandId > 0)
                condition.AppendFormat(" AND a.[brandid] = {0} ", brandId);
            if (storeId == -1)
            {
                storeId = WorkContext.PartUserInfo.StoreId;
            }

            if (storeId > 0)
                condition.AppendFormat(" AND a.[storeid] = {0} ", storeId);

            if (pid > 0)
                condition.AppendFormat(" AND a.[pid] = {0} ", pid);
            condition.Append(" AND b.[mallsource]=0 ");
            string sqlText = @"SELECT 
      [pid] AS '商品编号'
	  ,a.[name] AS '商品名称'
	  ,[psn] AS '商品货号'
      ,b.name AS '商品店铺'
      ,c.name AS '商品品牌'
      ,(
        SELECT name FROM dbo.hlh_categories WHERE (cateid IN(
			SELECT parentid FROM dbo.hlh_categories WHERE cateid IN (SELECT parentid FROM dbo.hlh_categories WHERE cateid=a.cateid) ) )
			OR (cateid IN(SELECT parentid FROM dbo.hlh_categories WHERE cateid =a.cateid AND layer=2)  )
			OR (cateid IN(SELECT cateid FROM dbo.hlh_categories WHERE cateid =a.cateid AND layer=1) )
		) AS '商品一级分类'
      ,
      (
      SELECT name FROM dbo.hlh_categories WHERE (
      cateid IN(SELECT parentid FROM dbo.hlh_categories WHERE cateid=a.cateid AND layer=3) )
      OR (cateid IN(SELECT cateid FROM dbo.hlh_categories WHERE cateid =A.cateid AND layer=2) )
      ) AS '商品二级分类'
      ,(SELECT name FROM dbo.hlh_categories WHERE       cateid =a.cateid AND layer=3 ) AS '商品三级分类'
      ,(CASE a.saletype WHEN 1 THEN '一般贸易' WHEN 2 THEN '保税商品' WHEN 3 THEN '海外直邮' ELSE '普通商品' END) AS '商品类型'
      ,[costprice] AS '成本价'
      ,[marketprice] AS '市场价'
      ,[shopprice] AS '商城价'
	 
	FROM dbo.hlh_products a 
	 LEFT JOIN dbo.hlh_stores b ON a.storeid = b.storeid LEFT JOIN dbo.hlh_brands c ON a.brandid=c.brandid 
	WHERE   " + condition.ToString() + " ORDER BY a.addtime DESC  ";
            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", (state == 0 ? "在售产品-" : "下架产品-") + DateTime.Now.ToString("yyyyMMdd")));
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.ContentType = "application/vnd.xls";
            Response.Write("<meta http-equiv=Content-Type content=\"text/html; charset=GB2312\">");
            System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
            System.Web.UI.WebControls.DataGrid dg = new System.Web.UI.WebControls.DataGrid();
            dg.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(dg_ItemDataBound);
            dg.DataSource = dt;
            dg.DataBind();
            dg.RenderControl(oHtmlTextWriter);
            Response.Write(oStringWriter.ToString());
            Response.Flush();
            Response.End();
            return Content("下载完毕");
        }

        private void dg_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Header && e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Footer)
            {
                foreach (System.Web.UI.WebControls.TableCell item in e.Item.Cells)
                    item.Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            }
        }

        /// <summary>
        /// 回收站商品列表
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="productName">商品名称</param>
        /// <param name="categoryName">分类名称</param>
        /// <param name="brandName">品牌名称</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="cateId">分类id</param>
        /// <param name="brandId">品牌id</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult RecycleBinProductList(string storeName, string productName, string categoryName, string brandName, string sortColumn, string sortDirection, int storeId = -1, int cateId = -1, int brandId = -1, int pageNumber = 1, int pageSize = 15)
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            string condition = AdminProducts.AdminGetProductListCondition(storeId, 0, productName, cateId, brandId, (int)ProductState.RecycleBin);
            string sort = AdminProducts.AdminGetProductListSort(sortColumn, sortDirection);

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminProducts.AdminGetProductCount(condition));

            ProductListModel model = new ProductListModel()
            {
                ProductList = AdminProducts.AdminGetProductList(pageModel.PageSize, pageModel.PageNumber, condition, sort),
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                StoreId = storeId,
                ProductName = productName,
                CateId = cateId,
                BrandId = brandId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "全部店铺" : storeName,
                CategoryName = string.IsNullOrWhiteSpace(categoryName) ? "全部分类" : categoryName,
                BrandName = string.IsNullOrWhiteSpace(brandName) ? "全部品牌" : brandName
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&storeId={5}&ProductName={6}&cateId={7}&brandId={8}&storeName={9}&categoryName={10}&brandName={11}",
                                                          Url.Action("recyclebinproductlist"),
                                                          pageModel.PageNumber, pageModel.PageSize,
                                                          sortColumn, sortDirection,
                                                          storeId, productName, cateId, brandId,
                                                          storeName, categoryName, brandName));
            return View(model);
        }

        /// <summary>
        /// 商品选择列表
        /// </summary>
        /// <param name="productName">商品名称</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="cateId">分类id</param>
        /// <param name="brandId">品牌id</param>
        /// <returns></returns>
        public ActionResult ProductSelectList(string productName, int pageNumber = 1, int pageSize = 12, int storeId = -1, int cateId = -1, int brandId = -1)
        {
            string condition = AdminProducts.AdminGetProductListCondition(storeId, 0, productName, cateId, brandId, (int)ProductState.OnSale);
            string sort = AdminProducts.AdminGetProductListSort("pid", "desc");

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminProducts.AdminGetProductCount(condition));

            DataTable dt = AdminProducts.AdminGetProductList(pageModel.PageSize, pageModel.PageNumber, condition, sort);
            StringBuilder result = new System.Text.StringBuilder("{");
            result.AppendFormat("\"totalPages\":\"{0}\",\"pageNumber\":\"{1}\",\"items\":[", pageModel.TotalPages, pageModel.PageNumber);
            foreach (DataRow row in dt.Rows)
                result.AppendFormat("{0}\"id\":\"{1}\",\"name\":\"{2}\",\"shopprice\":\"{3}\"{4},", "{", row["pid"], row["pname"].ToString().Trim(), row["shopprice"], "}");

            if (dt.Rows.Count > 0)
                result.Remove(result.Length - 1, 1);

            result.Append("]}");
            return Content(result.ToString());
        }

        #endregion
        /// <summary>
        /// 合并SKU
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddProductToSKU(int pid = -1)
        {
            ProductInfo productInfo = AdminProducts.AdminGetProductById(pid);
            if (productInfo == null)
                return PromptView("商品不存在");
            //查找该分类下的属性--根据分类
            //ViewData["cateattr"] = AdminCategories.GetCategoryAAndVListJsonCache(productInfo.CateId);
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(productInfo);
        }
        /// <summary>
        /// 合并SKU
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddProductToSKU()
        {
            int oldpid = WebHelper.GetFormInt("OldPid");
            int targetPid = WebHelper.GetFormInt("targetpid");
            ProductInfo productInfo = AdminProducts.AdminGetProductById(oldpid);
            if (productInfo == null)
                return PromptView("商品不存在");
            ProductInfo targetProductInfo = AdminProducts.AdminGetProductById(targetPid);
            if (productInfo == null)
                return PromptView("SKU商品不存在");
            if (oldpid == targetPid)
                return PromptView("商品重复");
            int skugid = targetProductInfo.SKUGid;
            int attrid = WebHelper.GetFormInt("attrid");
            int attrvalueid = WebHelper.GetFormInt("attrvalueid");
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            if (AdminProducts.UpdateSKUGid(productInfo, skugid) && AdminProducts.AddProductSKU(productInfo, skugid, attrid, attrvalueid))
            {
                return PromptView("合并SKU成功");
            }

            return View(productInfo);
        }
        #region 商品添加、编辑

        /// <summary>
        /// 商品置顶
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductSortTop(int pid = -1, int orderType = -1000)
        {
            ProductInfo productInfo = AdminProducts.AdminGetProductById(pid);
            if (productInfo == null)
                return PromptView("商品不存在");
            bool result = AdminProducts.UpdateProductSort(productInfo, orderType);
            if (result)
                return Content("1");
            else
                return Content("0");
        }
        /// <summary>
        /// 商品显示排序
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductShowOrder(int pid = -1, int orderType = 1000)
        {
            ProductInfo productInfo = AdminProducts.AdminGetProductById(pid);
            if (productInfo == null)
                return PromptView("商品不存在");
            bool result = AdminProducts.UpdateProductSort(productInfo, orderType * -1);
            if (result)
                return PromptView("更新成功");
            else
                return PromptView("更新失败");
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        [HttpGet]
        public ActionResult AddProduct()
        {
            AddProductModel model = new AddProductModel();

            string backUrl = MallUtils.GetMallAdminRefererCookie();
            if (backUrl.Length == 0 || backUrl == "/malladmin/home/mallruninfo")
            {
                backUrl = Url.Action("onsaleproductlist");
                MallUtils.SetAdminRefererCookie(backUrl);
            }
            ViewData["referer"] = backUrl;
            return View(model);
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddProduct(AddProductModel model)
        {
            if (AdminProducts.AdminGetProductIdByName(model.ProductName) > 0)
                ModelState.AddModelError("ProductName", "名称已经存在");

            if (ModelState.IsValid)
            {
                ProductInfo productInfo = new ProductInfo()
                {
                    PSN = model.PSN ?? "",
                    CateId = model.CateId,
                    BrandId = model.BrandId,
                    SKUGid = 0,
                    StoreId = model.StoreId,
                    StoreCid = model.StoreCid,
                    StoreSTid = model.StoreSTid,
                    Name = model.ProductName,
                    ShopPrice = model.ShopPrice,
                    MarketPrice = model.MarketPrice,
                    CostPrice = model.CostPrice,
                    PV = model.PV,
                    HaiMi = model.HaiMi,
                    HongBaoCut = model.HongBaoCut,
                    State = model.State,
                    IsBest = model.IsBest == true ? 1 : 0,
                    IsHot = model.IsHot == true ? 1 : 0,
                    IsNew = model.IsNew == true ? 1 : 0,
                    SaleType = model.SaleType,
                    TaxRate = model.Taxrate,
                    DisplayOrder = model.DisplayOrder,
                    MinBuyCount = model.MinBuyCount,
                    Weight = model.Weight,
                    ShowImg = "",
                    Description = model.Description ?? "",
                    ProductParam = model.ProductParam ?? "",
                    MobileDescription = model.MobileDescription ?? "",
                    AddTime = DateTime.Now,
                    Unit = model.Unit ?? "",
                    SettlePercent = model.SettlePercent,
                    ReBuyCycle = model.ReBuyCycle,
                    RelateReBuyPid = model.RelateReBuyPid,
                    VideoUrl = model.VideoUrl ?? ""
                };

                //属性处理
                List<ProductAttributeInfo> productAttributeList = new List<ProductAttributeInfo>();
                if (model.AttrValueIdList != null && model.AttrValueIdList.Length > 0)
                {
                    for (int i = 0; i < model.AttrValueIdList.Length; i++)
                    {
                        int attrId = model.AttrIdList[i];//属性id
                        int attrValueId = model.AttrValueIdList[i];//属性值id
                        string inputValue = model.AttrInputValueList[i];//属性输入值
                        if (attrId > 0 && attrValueId > 0)
                        {
                            productAttributeList.Add(new ProductAttributeInfo
                            {
                                AttrId = attrId,
                                AttrValueId = attrValueId,
                                InputValue = inputValue ?? ""
                            });
                        }
                    }
                }

                int pid = AdminProducts.AddProduct(productInfo, model.StockNumber, model.StockLimit, productAttributeList);
                AddMallAdminLog("添加普通商品", "添加普通商品,商品为:" + model.ProductName);

                //频道产品添加
                if (!string.IsNullOrEmpty(model.ChannelId))
                {
                    foreach (var item in model.ChannelId.Split(','))
                    {
                        ChannelProductInfo channelProductInfo = new ChannelProductInfo()
                        {
                            CreationDate = DateTime.Now,
                            ChId = TypeHelper.StringToInt(item),
                            Pid = pid,
                            State = 0
                        };
                        if (channelProductInfo.ChId > 0)
                        {
                            new ChannelBLL().CreateChannelProduct(channelProductInfo);
                            AddMallAdminLog("添加频道商品", "添加频道商品,商品id为：" + pid + "商品名称为:" + model.ProductName + "添加频道id为：" + channelProductInfo.ChId);
                        }
                    }
                }

                string backUrl = null;
                if (productInfo.State == (int)ProductState.OnSale)
                    backUrl = Url.Action("onsaleproductlist");
                else
                    backUrl = Url.Action("outsaleproductlist");
                return PromptView(backUrl, "普通商品添加成功");
            }
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 编辑商品
        /// </summary>
        [HttpGet]
        public ActionResult EditProduct(int pid = -1)
        {
            ProductInfo productInfo = AdminProducts.AdminGetProductById(pid);
            if (productInfo == null)
                return PromptView("商品不存在");

            EditProductModel model = new EditProductModel();
            model.SubTitle = productInfo.SubTitle;
            model.PSN = productInfo.PSN;
            model.BrandId = productInfo.BrandId;
            model.StoreCid = productInfo.StoreCid;
            model.StoreSTid = productInfo.StoreSTid;
            model.ProductName = productInfo.Name;
            model.SKUGid = productInfo.SKUGid;
            model.ShopPrice = productInfo.ShopPrice;
            model.MarketPrice = productInfo.MarketPrice;
            model.CostPrice = productInfo.CostPrice;
            model.PV = System.Math.Round(productInfo.PV, 2);
            model.HaiMi = System.Math.Round(productInfo.HaiMi, 2);

            model.HongBaoCut = productInfo.HongBaoCut;
            model.State = productInfo.State;
            model.IsBest = productInfo.IsBest == 1 ? true : false;
            model.IsHot = productInfo.IsHot == 1 ? true : false;
            model.IsNew = productInfo.IsNew == 1 ? true : false;
            model.SaleType = productInfo.SaleType;
            model.Taxrate = productInfo.TaxRate;
            model.DisplayOrder = productInfo.DisplayOrder;
            model.MinBuyCount = productInfo.MinBuyCount;
            model.Weight = productInfo.Weight;
            model.Description = productInfo.Description;
            model.ProductParam = productInfo.ProductParam;
            model.MobileDescription = productInfo.MobileDescription;
            model.BrandName = AdminBrands.GetBrandById(productInfo.BrandId).Name;
            model.ShowOrder = productInfo.ShowOrder;
            model.Unit = productInfo.Unit;
            model.SettlePercent = productInfo.SettlePercent;
            model.ReBuyCycle = productInfo.ReBuyCycle;
            model.RelateReBuyPid = productInfo.RelateReBuyPid;
            model.VideoUrl = productInfo.VideoUrl;
            //库存信息
            ProductStockInfo productStockInfo = AdminProducts.GetProductStockByPid(pid);
            model.StockNumber = productStockInfo.Number;
            model.StockLimit = productStockInfo.Limit;

            //商品属性列表
            List<ProductAttributeInfo> productAttributeList = Products.GetProductAttributeList(pid);

            //商品sku项列表
            DataTable productSKUItemList = AdminProducts.GetProductSKUItemList(productInfo.Pid);

            //频道信息
            int chid = 0;
            string name = string.Empty;
            List<ChannelInfo> channelList = AdminChannel.GetChanneListlByPid(pid);
            //if (channelList)
            //{
            //    chid = channelInfo.ChId;
            //    name = channelInfo.Name;
            //}
            //ViewData["channelId"] = chid;
            //ViewData["channelName"] = name;
            ViewData["channelList"] = channelList;

            ViewData["pid"] = productInfo.Pid;
            ViewData["storeId"] = productInfo.StoreId;
            ViewData["storeName"] = AdminStores.GetStoreById(productInfo.StoreId).Name;
            ViewData["cateId"] = productInfo.CateId;
            ViewData["categoryName"] = AdminCategories.GetCategoryById(productInfo.CateId).Name;
            ViewData["productAttributeList"] = productAttributeList;
            ViewData["productSKUItemList"] = productSKUItemList;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 编辑商品
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditProduct(EditProductModel model, int pid = -1)
        {
            ProductInfo productInfo = AdminProducts.AdminGetProductById(pid);
            if (productInfo == null)
                return PromptView("商品不存在");

            int pid2 = AdminProducts.AdminGetProductIdByName(model.ProductName);
            if (pid2 > 0 && pid2 != pid)
                ModelState.AddModelError("ProductName", "名称已经存在");

            if (ModelState.IsValid)
            {
                productInfo.SubTitle = model.SubTitle ?? "";
                productInfo.PSN = model.PSN ?? "";
                productInfo.BrandId = model.BrandId;
                productInfo.StoreCid = model.StoreCid;
                productInfo.StoreSTid = model.StoreSTid;
                productInfo.Name = model.ProductName;
                productInfo.CateId = model.CateId;
                productInfo.ShopPrice = model.ShopPrice;
                productInfo.MarketPrice = model.MarketPrice;
                productInfo.CostPrice = model.CostPrice;
                productInfo.PV = model.PV;
                productInfo.HaiMi = model.HaiMi;
                productInfo.HongBaoCut = model.HongBaoCut;
                productInfo.State = model.State;
                productInfo.IsBest = model.IsBest == true ? 1 : 0;
                productInfo.IsHot = model.IsHot == true ? 1 : 0;
                productInfo.IsNew = model.IsNew == true ? 1 : 0;
                productInfo.SaleType = model.SaleType;
                productInfo.TaxRate = System.Math.Round(model.Taxrate, 2);
                productInfo.DisplayOrder = model.DisplayOrder;
                productInfo.Weight = model.Weight;
                productInfo.MinBuyCount = model.MinBuyCount;
                productInfo.Description = model.Description ?? "";
                productInfo.ProductParam = model.ProductParam ?? "";
                productInfo.MobileDescription = model.MobileDescription ?? "";
                productInfo.Unit = model.Unit ?? "";
                productInfo.SettlePercent = model.SettlePercent;
                productInfo.ReBuyCycle = model.ReBuyCycle;
                productInfo.RelateReBuyPid = model.RelateReBuyPid;
                productInfo.VideoUrl = model.VideoUrl ?? "";
                AdminProducts.UpdateProduct(productInfo, model.StockNumber, model.StockLimit);
                AddMallAdminLog("修改商品", "修改商品,商品ID为:" + pid);
                return PromptView("商品修改成功");
            }


            //商品属性列表
            List<ProductAttributeInfo> productAttributeList = Products.GetProductAttributeList(pid);

            //商品sku项列表
            DataTable productSKUItemList = AdminProducts.GetProductSKUItemList(productInfo.Pid);

            ViewData["pid"] = productInfo.Pid;
            ViewData["storeId"] = productInfo.StoreId;
            ViewData["storeName"] = AdminStores.GetStoreById(productInfo.StoreId).Name;
            ViewData["cateId"] = productInfo.CateId;
            ViewData["categoryName"] = AdminCategories.GetCategoryById(productInfo.CateId).Name;
            ViewData["productAttributeList"] = productAttributeList;
            ViewData["productSKUItemList"] = productSKUItemList;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 添加SKU
        /// </summary>
        [HttpGet]
        public ActionResult AddSKU()
        {
            AddSKUModel model = new AddSKUModel();

            string backUrl = MallUtils.GetMallAdminRefererCookie();
            if (backUrl.Length == 0 || backUrl == "/malladmin/home/mallruninfo")
            {
                backUrl = Url.Action("onsaleproductlist");
                MallUtils.SetAdminRefererCookie(backUrl);
            }
            ViewData["referer"] = backUrl;
            return View(model);
        }

        /// <summary>
        /// 添加SKU
        /// </summary>
        [HttpPost]
        public ActionResult AddSKU(AddSKUModel model)
        {
            if (AdminProducts.AdminGetProductIdByName(model.ProductName) > 0)
                ModelState.AddModelError("ProductName", "名称已经存在");
            if (model.AttrIdList == null || model.AttrIdList.Length < 1)
                ModelState.AddModelError("CateId", "请选择SKU");

            if (ModelState.IsValid)
            {
                //商品信息
                ProductInfo productInfo = new ProductInfo()
                {
                    PSN = "",
                    CateId = model.CateId,
                    BrandId = model.BrandId,
                    SKUGid = 0,
                    StoreId = model.StoreId,
                    StoreCid = model.StoreCid,
                    StoreSTid = model.StoreSTid,
                    Name = model.ProductName,
                    ShopPrice = model.ShopPrice,
                    MarketPrice = model.MarketPrice,
                    CostPrice = model.CostPrice,
                    PV = model.PV,
                    HaiMi = model.HaiMi,
                    HongBaoCut = model.HongBaoCut,
                    State = (int)ProductState.OutSale,//设为下架状态
                    IsBest = model.IsBest == true ? 1 : 0,
                    IsHot = model.IsHot == true ? 1 : 0,
                    IsNew = model.IsNew == true ? 1 : 0,
                    DisplayOrder = model.DisplayOrder,
                    Weight = model.Weight,
                    ShowImg = "",
                    Description = model.Description ?? "",
                    ProductParam = model.ProductParam ?? "",
                    MobileDescription = model.MobileDescription ?? "",
                    AddTime = DateTime.Now
                };

                //SKU处理
                List<ProductSKUItemInfo> productSKUItemList = new List<ProductSKUItemInfo>();
                for (int i = 0; i < model.AttrIdList.Length; i++)
                {
                    int attrId = model.AttrIdList[i];//属性id
                    int attrValueId = model.AttrValueIdList[i];//属性值id
                    string inputValue = model.AttrInputValueList[i];//属性输入值
                    if (attrId > 0 && attrValueId > 0)
                    {
                        productSKUItemList.Add(new ProductSKUItemInfo()
                        {
                            AttrId = attrId,
                            AttrValueId = attrValueId,
                            InputValue = inputValue ?? ""
                        });
                    }
                }
                //频道产品
                List<ChannelProductInfo> channelInfoList = new List<ChannelProductInfo>();
                if (!string.IsNullOrEmpty(model.ChannelId))
                {
                    int i = 0;
                    foreach (var item in model.ChannelId.Split(','))
                    {
                        ChannelProductInfo channelProductInfo = new ChannelProductInfo()
                        {
                            CreationDate = DateTime.Now,
                            ChId = TypeHelper.StringToInt(item),
                            State = 0
                        };
                        channelInfoList.Add(channelProductInfo);
                    }
                }

                //ChannelProductInfo channelProductInfo = new ChannelProductInfo()
                //{
                //    CreationDate = DateTime.Now,
                //    ChId = model.ChannelId,
                //    State = 0
                //};

                AdminProducts.AddSKU(productInfo, productSKUItemList, channelInfoList);
                AddMallAdminLog("添加SKU商品", "添加SKU商品,商品为:" + model.ProductName);

                return PromptView(Url.Action("outsaleproductlist"), "SKU商品添加成功");
            }
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 产品二维码
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult ProductQRCode(int pid)
        {
            ProductInfo productInfo = AdminProducts.AdminGetProductById(pid);
            if (productInfo == null)
                return PromptView("商品不存在");
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(productInfo);
        }

        /// <summary>
        /// 返回产品二维码
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public string CreatQRCode(int pid, int parentid, string parentname)
        {
            //string nr,  int parentid, string parentname, string pname
            //二维码

            //http://127.0.0.1:8111/mob/cart/AddProductForCode?uid=415&pid=541&buyCount=1

            ProductInfo productInfo = AdminProducts.AdminGetProductById(pid);
            if (productInfo == null)
                return string.Empty;
            int buyCount = productInfo.MinBuyCount > 0 ? productInfo.MinBuyCount : 1;
            string shareUrl = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/mob" + "/cart/AddProductForCode?uid=" + parentid + "&pid=" + pid + "&buyCount=" + buyCount;
            parentname = parentname.Replace("*", "");

            string bgQRcode = IOHelper.CreateCodeForProduct(shareUrl, "", parentid, parentname, productInfo.Name);
            string codeimg = "http://" + Request.Url.Host + ":" + Request.Url.Port + "/upload/productcode/" + bgQRcode;

            return codeimg;

        }

        #endregion

        #region 商品状态
        /// <summary>
        /// 上架商品
        /// </summary>
        public ActionResult OnSaleProduct(int[] pidList)
        {
            bool result = AdminProducts.OnSaleProductById(pidList);
            if (result)
            {
                AddMallAdminLog("上架商品", "上架商品,商品ID为:" + CommonHelper.IntArrayToString(pidList));
                if (WorkContext.IsHttpAjax)
                    return Content("1");
                else
                    return PromptView("商品上架成功");
            }
            else
            {
                if (WorkContext.IsHttpAjax)
                    return Content("0");
                else
                    return PromptView("商品上架失败");
            }
        }

        /// <summary>
        /// 下架商品
        /// </summary>
        public ActionResult OutSaleProduct(int[] pidList)
        {
            bool result = AdminProducts.OutSaleProductById(pidList);
            if (result)
            {
                AddMallAdminLog("下架商品", "下架商品,商品ID为:" + CommonHelper.IntArrayToString(pidList));
                if (WorkContext.IsHttpAjax)
                    return Content("1");
                else
                    return PromptView("商品下架成功");
            }
            else
            {
                if (WorkContext.IsHttpAjax)
                    return Content("0");
                else
                    return PromptView("商品下架失败");
            }
        }

        /// <summary>
        /// 回收商品
        /// </summary>
        public ActionResult RecycleProduct(int[] pidList)
        {
            AdminProducts.RecycleProductById(pidList);
            AddMallAdminLog("回收商品", "回收商品,商品ID为:" + CommonHelper.IntArrayToString(pidList));
            return PromptView("商品删除成功");
        }

        /// <summary>
        /// 恢复商品
        /// </summary>
        public ActionResult RestoreProduct(int[] pidList)
        {
            AdminProducts.RestoreProductById(pidList);
            AddMallAdminLog("还原商品", "还原商品,商品ID为:" + CommonHelper.IntArrayToString(pidList));
            return PromptView("商品还原成功");
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        public ActionResult DelProduct(int[] pidList)
        {
            AdminProducts.DeleteProductById(pidList);
            AddMallAdminLog("删除商品", "删除商品,商品ID为:" + CommonHelper.IntArrayToString(pidList));
            return PromptView("商品删除成功");
        }

        /// <summary>
        /// 改变商品新品状态
        /// </summary>
        public ActionResult ChangeProductIsNew(int[] pidList, int state = 0)
        {
            bool result = AdminProducts.ChangeProductIsNew(pidList, state);
            if (result)
            {
                AddMallAdminLog("修改商品新品状态", "修改商品新品状态,商品ID和状态为:" + CommonHelper.IntArrayToString(pidList) + "_" + state);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 改变商品热销状态
        /// </summary>
        public ActionResult ChangeProductIsHot(int[] pidList, int state = 0)
        {
            bool result = AdminProducts.ChangeProductIsHot(pidList, state);
            if (result)
            {
                AddMallAdminLog("修改商品热销状态", "修改商品热销状态,商品ID和状态为:" + CommonHelper.IntArrayToString(pidList) + "_" + state);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 改变商品精品状态
        /// </summary>
        public ActionResult ChangeProductIsBest(int[] pidList, int state = 0)
        {
            bool result = AdminProducts.ChangeProductIsBest(pidList, state);
            if (result)
            {
                AddMallAdminLog("修改商品精品状态", "修改商品精品状态,商品ID和状态为:" + CommonHelper.IntArrayToString(pidList) + "_" + state);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 修改商品排序
        /// </summary>
        public ActionResult UpdateProductDisplayOrder(int pid = -1, int displayOrder = 0)
        {
            bool result = AdminProducts.UpdateProductDisplayOrder(pid, displayOrder);
            if (result)
            {
                AddMallAdminLog("修改商品顺序", "修改商品顺序,商品ID:" + pid);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 修改商品商城价格
        /// </summary>
        public ActionResult UpdateProductShopPrice(int pid = -1, decimal shopPrice = 0.00M)
        {
            bool result = AdminProducts.UpdateProductShopPrice(pid, shopPrice);
            if (result)
            {
                AddMallAdminLog("修改商品本店价格", "修改商品本店价格,商品ID和价格为:" + pid + "_" + shopPrice);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 修改商品库存数量
        /// </summary>
        public ActionResult UpdateProductStockNumber(int pid = -1, int stockNumber = 0)
        {
            bool result = AdminProducts.UpdateProductStockNumber(pid, stockNumber);
            if (result)
            {
                AddMallAdminLog("更新商品库存数量", "更新商品库存数量,商品ID和库存数量为:" + pid + "_" + stockNumber);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }



        /// <summary>
        /// 更新商品属性
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="attrId">属性id</param>
        /// <param name="attrValueId">属性值id</param>
        /// <param name="inputValue">输入值</param>
        /// <param name="type">更新类型</param>
        /// <returns></returns>
        public ActionResult UpdateProductAttribute(int pid = -1, int attrId = -1, int attrValueId = -1, string inputValue = "", int type = -1)
        {
            bool result = false;
            ProductAttributeInfo productAttributeInfo = AdminProducts.GetProductAttributeByPidAndAttrId(pid, attrId);
            if (productAttributeInfo == null)
            {
                productAttributeInfo = new ProductAttributeInfo();

                productAttributeInfo.Pid = pid;
                productAttributeInfo.AttrId = attrId;
                productAttributeInfo.AttrValueId = attrValueId;
                if (AdminCategories.GetAttributeValueById(attrValueId).IsInput == 0 || string.IsNullOrWhiteSpace(inputValue))
                    productAttributeInfo.InputValue = "";
                else
                    productAttributeInfo.InputValue = inputValue;

                result = AdminProducts.CreateProductAttribute(productAttributeInfo);
            }
            else
            {
                if (type == 1)
                {
                    productAttributeInfo.AttrValueId = attrValueId;
                    productAttributeInfo.InputValue = inputValue;
                    result = AdminProducts.UpdateProductAttribute(productAttributeInfo);
                }
                else if (type == 0)
                {
                    productAttributeInfo.AttrValueId = attrValueId;
                    productAttributeInfo.InputValue = "";
                    result = AdminProducts.UpdateProductAttribute(productAttributeInfo);
                }
            }
            if (result)
            {
                AddMallAdminLog("修改商品属性", "修改商品属性,商品属性ID:" + pid + "_" + attrId);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 删除商品属性
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="attrId">属性id</param>
        /// <returns></returns>
        public ActionResult DelProductAttribute(int pid = -1, int attrId = -1)
        {
            bool result = AdminProducts.DeleteProductAttributeByPidAndAttrId(pid, attrId);
            if (result)
            {
                AddMallAdminLog("删除商品属性", "删除商品属性,商品属性ID:" + pid + "_" + attrId);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        #endregion

        #region 商品图片

        /// <summary>
        /// 商品图片列表
        /// </summary>
        public ActionResult ProductImageList(int pid = -1)
        {
            PartProductInfo partProductInfo = AdminProducts.AdminGetPartProductById(pid);
            if (partProductInfo == null)
                return PromptView("商品不存在");

            ProductImageListModel model = new ProductImageListModel()
            {
                ProductImageList = AdminProducts.GetProductImageList(pid),
                Pid = pid,
                StoreId = partProductInfo.StoreId
            };
            Load();
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult ImageCopy(int pid = -1)
        {
            PartProductInfo partProductInfo = AdminProducts.AdminGetPartProductById(pid);
            if (partProductInfo == null)
                return PromptView("商品不存在");
            Load();
            return View();
        }
        /// <summary>
        /// 商品图片复制
        /// </summary>
        /// <returns></returns>
        public ActionResult ADDProductImageCopy(int pid = -1, string copypid = "")
        {
            PartProductInfo partProductInfo = AdminProducts.AdminGetPartProductById(pid);
            if (partProductInfo == null)
                return PromptView("商品不存在");

            List<string> copyPid = StringHelper.SplitString(copypid).ToList();
            copyPid.Distinct().Where(x => x != pid.ToString());

            List<ProductImageInfo> ProductImageList = AdminProducts.GetProductImageList(pid);

            foreach (var item in copyPid)
            {
                foreach (var info in ProductImageList)
                {
                    ProductImageInfo productImageInfo = new ProductImageInfo
                    {
                        Pid = TypeHelper.StringToInt(item),
                        ShowImg = info.ShowImg,
                        IsMain = info.ShowImg == partProductInfo.ShowImg ? 1 : 0,
                        DisplayOrder = 0,
                        StoreId = partProductInfo.StoreId
                    };
                    AdminProducts.CreateProductImage(productImageInfo);
                    if (info.ShowImg == partProductInfo.ShowImg)
                    {
                        AdminProducts.UpdateProductShowImage(TypeHelper.StringToInt(item), partProductInfo.ShowImg);
                    }
                }
            }
            return RedirectToAction("OnSaleProductList");
        }

        /// <summary>
        /// 添加商品图片
        /// </summary>
        public ActionResult AddProductImage(string showImg, int displayOrder = 0, int pid = -1, int MainImage = 0)
        {
            PartProductInfo partProductInfo = AdminProducts.AdminGetPartProductById(pid);
            if (partProductInfo == null)
                return PromptView(Url.Action("productimagelist", new { pid = pid }), "商品不存在");

            if (string.IsNullOrWhiteSpace(showImg))
                return PromptView(Url.Action("productimagelist", new { pid = pid }), "图片不能为空");
            ProductImageInfo oldMainImage = null;
            if (MainImage == 1)
            {
                List<ProductImageInfo> ProductImageList = AdminProducts.GetProductImageList(pid);
                oldMainImage = ProductImageList.Find(x => x.IsMain == 1);
            }
            ProductImageInfo productImageInfo = new ProductImageInfo
            {
                Pid = pid,
                ShowImg = showImg,
                IsMain = MainImage,
                DisplayOrder = displayOrder,
                StoreId = partProductInfo.StoreId
            };
            bool flag = AdminProducts.CreateProductImage(productImageInfo);
            if (MainImage == 1 && flag)
            {
                if (oldMainImage != null)
                {
                    bool result = AdminProducts.CancelProductMainImage(pid, oldMainImage.PImgId);
                    if (result)
                        AdminProducts.UpdateProductShowImage(productImageInfo.Pid, productImageInfo.ShowImg);
                }
                else
                    AdminProducts.UpdateProductShowImage(productImageInfo.Pid, productImageInfo.ShowImg);

            }
            AddMallAdminLog("添加商品图片", "添加商品图片,商品ID为:" + pid);
            return PromptView(Url.Action("productimagelist", new { pid = pid }), "商品图片添加成功");
        }

        /// <summary>
        /// 删除商品图片
        /// </summary>
        public ActionResult DelProductImage(int pImgId = -1)
        {
            bool result = AdminProducts.DeleteProductImageById(pImgId);
            if (result)
            {
                AddMallAdminLog("删除商品图片", "删除商品图片,商品图片ID:" + pImgId);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 设置图片为商品主图
        /// </summary>
        public ActionResult SetProductMianImage(int pImgId = -1)
        {
            bool result = AdminProducts.SetProductMainImage(pImgId);
            if (result)
            {
                AddMallAdminLog("设置商品主图", "设置商品主图,商品图片ID:" + pImgId);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 改变商品图片的排序
        /// </summary>
        public ActionResult ChangeProductImageDisplayOrder(int pImgId = -1, int displayOrder = 0)
        {
            bool result = AdminProducts.ChangeProductImageDisplayOrder(pImgId, displayOrder);
            if (result)
            {
                AddMallAdminLog("修改商品图片顺序", "修改商品图片顺序,商品图片ID:" + pImgId);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        private void Load()
        {
            string allowImgType = string.Empty;
            string[] imgTypeList = StringHelper.SplitString(BMAConfig.MallConfig.UploadImgType, ",");
            foreach (string imgType in imgTypeList)
                allowImgType += string.Format("*{0};", imgType.ToLower());

            string[] sizeList = StringHelper.SplitString(WorkContext.MallConfig.ProductShowThumbSize);

            ViewData["size"] = sizeList[sizeList.Length / 2];
            ViewData["allowImgType"] = allowImgType;
            ViewData["maxImgSize"] = BMAConfig.MallConfig.UploadImgSize;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
        }


        #endregion

        #region 商品关键字

        /// <summary>
        /// 商品关键词列表
        /// </summary>
        public ActionResult ProductKeywordList(int pid = -1)
        {
            PartProductInfo partProductInfo = AdminProducts.AdminGetPartProductById(pid);
            if (partProductInfo == null)
                return PromptView("商品不存在");

            ProductKeywordListModel model = new ProductKeywordListModel()
            {
                ProductKeywordList = AdminProducts.GetProductKeywordList(pid),
                Pid = pid
            };
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 添加商品关键词
        /// </summary>
        public ActionResult AddProductKeyword(string keyword, int relevancy = 0, int pid = -1)
        {
            PartProductInfo partProductInfo = AdminProducts.AdminGetPartProductById(pid);
            if (partProductInfo == null)
                return PromptView(Url.Action("productkeywordlist", new { pid = pid }), "商品不存在");

            if (string.IsNullOrWhiteSpace(keyword))
                return PromptView(Url.Action("productkeywordlist", new { pid = pid }), "关键词不能为空");

            if (keyword.Length > 20)
                return PromptView(Url.Action("productkeywordlist", new { pid = pid }), "关键词最多只能输入20个字");

            if (AdminProducts.IsExistProductKeyWord(pid, keyword))
                return PromptView(Url.Action("productkeywordlist", new { pid = pid }), "关键词已经存在");

            ProductKeywordInfo productKeywordInfo = new ProductKeywordInfo
            {
                Keyword = keyword,
                Pid = pid,
                Relevancy = relevancy
            };
            AdminProducts.CreateProductKeyword(productKeywordInfo);
            AddMallAdminLog("添加商品关键词", "添加商品关键词,商品ID为:" + pid);
            return PromptView(Url.Action("productkeywordlist", new { pid = pid }), "商品关键词添加成功");
        }

        /// <summary>
        /// 更新商品关键词的相关性
        /// </summary>
        public ActionResult UpdateProductKeywordRelevancy(int keywordId = -1, int relevancy = 0)
        {
            bool result = AdminProducts.UpdateProductKeywordRelevancy(keywordId, relevancy);
            if (result)
            {
                AddMallAdminLog("修改商品关键词的相关性", "修改商品关键词的相关性,商品关键词ID:" + keywordId);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 删除商品关键词
        /// </summary>
        public ActionResult DelProductKeyword(int[] keywordIdList)
        {
            bool result = AdminProducts.DeleteProductKeyword(keywordIdList);
            if (result)
            {
                AddMallAdminLog("删除商品关键词", "删除商品关键词,商品关键词ID:" + CommonHelper.IntArrayToString(keywordIdList));
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        #endregion

        #region 关联商品

        /// <summary>
        /// 关联商品列表
        /// </summary>
        /// <param name="pid">主商品id</param>
        /// <returns></returns>
        public ActionResult RelateProductList(int pid = -1)
        {
            PartProductInfo partProductInfo = AdminProducts.AdminGetPartProductById(pid);
            if (partProductInfo == null)
                return PromptView("商品不存在");

            RelateProductListModel model = new RelateProductListModel()
            {
                RelateProductList = AdminProducts.AdminGetRelateProductList(pid),
                Pid = pid,
                StoreId = partProductInfo.StoreId
            };
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 添加关联商品
        /// </summary>
        /// <param name="pid">主商品id</param>
        /// <param name="relatePid">关联商品id</param>
        /// <returns></returns>
        public ActionResult AddRelateProduct(int pid = -1, int relatePid = -1)
        {
            PartProductInfo partProductInfo1 = AdminProducts.AdminGetPartProductById(pid);
            if (partProductInfo1 == null)
                return PromptView(Url.Action("relateproductlist", new { pid = pid }), "主商品不存在");

            PartProductInfo partProductInfo2 = AdminProducts.AdminGetPartProductById(relatePid);
            if (partProductInfo2 == null)
                return PromptView(Url.Action("relateproductlist", new { pid = pid }), "关联商品不存在");

            if (pid == relatePid)
                return PromptView(Url.Action("relateproductlist", new { pid = pid }), "不能关联自身");

            if (partProductInfo1.StoreId != partProductInfo2.StoreId)
                return PromptView(Url.Action("relateproductlist", new { pid = pid }), "只能关联同一店铺的商品");

            if (AdminProducts.IsExistRelateProduct(pid, relatePid))
                return PromptView(Url.Action("relateproductlist", new { pid = pid }), "此关联商品已经存在");

            AdminProducts.AddRelateProduct(pid, relatePid);
            AddMallAdminLog("添加关联商品", "添加关联商品,关联商品为:" + partProductInfo2.Name);
            return PromptView(Url.Action("relateproductlist", new { pid = pid }), "关联商品添加成功");
        }

        /// <summary>
        /// 删除关联商品
        /// </summary>
        /// <param name="pid">主商品id</param>
        /// <param name="relatePid">关联商品id</param>
        /// <returns></returns>
        public ActionResult DelRelateProduct(int pid = -1, int relatePid = -1)
        {
            bool result = AdminProducts.DeleteRelateProductByPidAndRelatePid(pid, relatePid);
            if (result)
            {
                AddMallAdminLog("删除关联商品", "删除关联商品品,商品ID为" + pid + "_" + relatePid);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        #endregion

        #region 定时商品

        /// <summary>
        /// 定时商品列表
        /// </summary>
        public ActionResult TimeProductList(string storeName, string productName, int storeId = -1, int pageSize = 15, int pageNumber = 1)
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminProducts.GetTimeProductCount(storeId, productName));
            TimeProductListModel model = new TimeProductListModel()
            {
                PageModel = pageModel,
                TimeProductList = AdminProducts.GetTimeProductList(pageSize, pageNumber, storeId, productName),
                StoreId = storeId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "全部店铺" : storeName,
                ProductName = productName
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&storeId={3}&storeName={4}&productName={5}",
                                                          Url.Action("timeproductlist"),
                                                          pageModel.PageNumber, pageModel.PageSize, storeId, storeName, productName));
            return View(model);
        }

        /// <summary>
        /// 添加定时商品
        /// </summary>
        [HttpGet]
        public ActionResult AddTimeProduct()
        {
            TimeProductModel model = new TimeProductModel();
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 添加定时商品
        /// </summary>
        [HttpPost]
        public ActionResult AddTimeProduct(TimeProductModel model)
        {
            PartProductInfo partProductInfo = AdminProducts.AdminGetPartProductById(model.Pid);
            if (partProductInfo == null)
                ModelState.AddModelError("Pid", "请选择商品");
            if (AdminProducts.IsExistTimeProduct(model.Pid))
                ModelState.AddModelError("Pid", "此商品已经存在");

            if (ModelState.IsValid)
            {
                DateTime noTime = new DateTime(1900, 1, 1);
                TimeProductInfo timeProductInfo = new TimeProductInfo()
                {
                    Pid = model.Pid,
                    StoreId = partProductInfo.StoreId,
                    OnSaleState = model.OnSaleTime == null ? 0 : 1,
                    OutSaleState = model.OutSaleTime == null ? 0 : 1,
                    OnSaleTime = model.OnSaleTime == null ? noTime : model.OnSaleTime.Value,
                    OutSaleTime = model.OutSaleTime == null ? noTime : model.OutSaleTime.Value
                };
                AdminProducts.AddTimeProduct(timeProductInfo);
                AddMallAdminLog("添加定时商品", "添加定时商品,定时商品为:" + partProductInfo.Name);
                return PromptView("定时商品添加成功");
            }
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 编辑定时商品
        /// </summary>
        [HttpGet]
        public ActionResult EditTimeProduct(int recordId = -1)
        {
            TimeProductInfo timeProductInfo = AdminProducts.GetTimeProductByRecordId(recordId);
            if (timeProductInfo == null)
                return PromptView("定时商品不存在");

            DateTime? nullTime = null;
            DateTime noTime = new DateTime(1900, 1, 1);

            TimeProductModel model = new TimeProductModel();
            model.Pid = timeProductInfo.Pid;
            model.OnSaleTime = timeProductInfo.OnSaleTime == noTime ? nullTime : timeProductInfo.OnSaleTime;
            model.OutSaleTime = timeProductInfo.OutSaleTime == noTime ? nullTime : timeProductInfo.OutSaleTime;

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 编辑定时商品
        /// </summary>
        [HttpPost]
        public ActionResult EditTimeProduct(TimeProductModel model, int recordId = -1)
        {
            TimeProductInfo timeProductInfo = AdminProducts.GetTimeProductByRecordId(recordId);
            if (timeProductInfo == null)
                return PromptView("定时商品不存在");

            if (ModelState.IsValid)
            {
                DateTime noTime = new DateTime(1900, 1, 1);
                timeProductInfo.OnSaleTime = model.OnSaleTime == null ? noTime : model.OnSaleTime.Value;
                timeProductInfo.OutSaleTime = model.OutSaleTime == null ? noTime : model.OutSaleTime.Value;

                if (model.OnSaleTime != timeProductInfo.OnSaleTime)
                    timeProductInfo.OnSaleState = model.OnSaleTime == null ? 0 : 1;
                if (model.OutSaleTime != timeProductInfo.OutSaleTime)
                    timeProductInfo.OutSaleState = model.OutSaleTime == null ? 0 : 1;

                AdminProducts.UpdateTimeProduct(timeProductInfo);
                AddMallAdminLog("修改定时商品", "修改定时商品,定时商品ID为:" + timeProductInfo.Pid);
                return PromptView("定时商品修改成功");
            }

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 删除定时商品
        /// </summary>
        public ActionResult DelTimeProduct(int recordId = -1)
        {
            AdminProducts.DeleteTimeProductByRecordId(recordId);
            AddMallAdminLog("删除定时商品", "删除定时商品,记录ID为" + recordId);
            return PromptView("定时商品修改成功");
        }

        #endregion

    }
}
