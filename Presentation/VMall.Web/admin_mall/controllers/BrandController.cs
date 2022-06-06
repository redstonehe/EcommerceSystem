using System;
using System.Web;
using System.Data;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台品牌控制器类
    /// </summary>
    public partial class BrandController : BaseMallAdminController
    {
        /// <summary>
        /// 品牌列表
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ActionResult List(string brandName, string sortColumn, string sortDirection, int pageSize = 15, int pageNumber = 1, int hasPro = -1)
        {
            string strWhere = "1=1";
            if (!string.IsNullOrWhiteSpace(brandName))
                strWhere += string.Format("AND [name] like '%{0}%' ", brandName);
            if (hasPro == 0)
                strWhere += @" AND (SELECT COUNT(1) FROM dbo.hlh_products b WHERE b.[brandid]=a.brandid)=0";
            else if (hasPro == 1)
                strWhere += @" AND (SELECT COUNT(1) FROM dbo.hlh_products b WHERE b.[brandid]=a.brandid)>0";
            string condition = strWhere;
            string sort = AdminBrands.AdminGetBrandListSort(sortColumn, sortDirection);

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminBrands.AdminGetBrandCount(condition));

            BrandListModel model = new BrandListModel()
            {
                BrandList = AdminBrands.AdminGetBrandList(pageModel.PageSize, pageModel.PageNumber, condition, sort),
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                HasPro = hasPro,
                BrandName = brandName
            };
            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemList.Add(new SelectListItem() { Text = "有产品品牌", Value = "1" });
            itemList.Add(new SelectListItem() { Text = "无产品品牌", Value = "0" });
            ViewData["typeList"] = itemList;
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&brandName={5}&hasPro={6}",
                                                          Url.Action("list"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          sortColumn,
                                                          sortDirection,
                                                          brandName, hasPro));
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
        public ActionResult OutPutList(string brandName, int hasPro = -1)
        {
            string strWhere = "1=1 ";
            if (!string.IsNullOrWhiteSpace(brandName))
                strWhere += string.Format(" AND [name] like '%{0}%' ", brandName);
            if (hasPro == 0)
                strWhere += @" AND (SELECT COUNT(1) FROM dbo.hlh_products b WHERE b.[brandid]=a.brandid)=0";
            else if (hasPro == 1)
                strWhere += @" AND (SELECT COUNT(1) FROM dbo.hlh_products b WHERE b.[brandid]=a.brandid)>0";
            string sqlText = @"SELECT 
	                        brandid 品牌ID,
	                        name 品牌名称,
	                        'http://www.xxxx.com/upload/brand/thumb100_100/'+logo 品牌logo
	                        FROM dbo.hlh_brands a WHERE " + strWhere + " ORDER BY brandid DESC";
            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "品牌-" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 品牌选择列表
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ContentResult SelectList(string brandName, int pageNumber = 1, int pageSize = 24)
        {
            string condition = AdminBrands.AdminGetBrandListCondition(brandName);
            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminBrands.AdminGetBrandCount(condition));

            DataTable brandSelectList = AdminBrands.AdminGetBrandSelectList(pageModel.PageSize, pageModel.PageNumber, condition);

            StringBuilder result = new StringBuilder("{");
            result.AppendFormat("\"totalPages\":\"{0}\",\"pageNumber\":\"{1}\",\"items\":[", pageModel.TotalPages, pageModel.PageNumber);
            foreach (DataRow row in brandSelectList.Rows)
                result.AppendFormat("{0}\"id\":\"{1}\",\"name\":\"{2}\"{3},", "{", row["brandid"], row["name"].ToString().Trim(), "}");

            if (brandSelectList.Rows.Count > 0)
                result.Remove(result.Length - 1, 1);

            result.Append("]}");
            return Content(result.ToString());
        }

        /// <summary>
        /// 添加品牌
        /// </summary>
        [HttpGet]
        public ActionResult Add()
        {
            BrandModel model = new BrandModel();
            Load();
            return View(model);
        }

        /// <summary>
        /// 添加品牌
        /// </summary>
        [HttpPost]
        public ActionResult Add(BrandModel model)
        {
            if (AdminBrands.GetBrandIdByName(model.BrandName.Trim()) > 0)
                ModelState.AddModelError("BrandName", "名称已经存在");

            if (ModelState.IsValid)
            {
                BrandInfo brandInfo = new BrandInfo()
                {
                    DisplayOrder = model.DisplayOrder,
                    Name = model.BrandName.Trim(),
                    Logo = model.Logo
                };

                AdminBrands.CreateBrand(brandInfo);
                AddMallAdminLog("添加品牌", "添加品牌,品牌为:" + model.BrandName);
                return PromptView("品牌添加成功");
            }
            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑品牌
        /// </summary>
        [HttpGet]
        public ActionResult Edit(int brandId = -1)
        {
            BrandInfo brandInfo = AdminBrands.GetBrandById(brandId);
            if (brandInfo == null)
                return PromptView("品牌不存在");

            BrandModel model = new BrandModel();
            model.DisplayOrder = brandInfo.DisplayOrder;
            model.BrandName = brandInfo.Name;
            model.Logo = brandInfo.Logo;
            Load();

            return View(model);
        }

        /// <summary>
        /// 编辑品牌
        /// </summary>
        [HttpPost]
        public ActionResult Edit(BrandModel model, int brandId = -1)
        {
            BrandInfo brandInfo = AdminBrands.GetBrandById(brandId);
            if (brandInfo == null)
                return PromptView("品牌不存在");

            int brandId2 = AdminBrands.GetBrandIdByName(model.BrandName.Trim());
            if (brandId2 > 0 && brandId2 != brandId)
                ModelState.AddModelError("BrandName", "名称已经存在");

            if (ModelState.IsValid)
            {
                brandInfo.DisplayOrder = model.DisplayOrder;
                brandInfo.Name = model.BrandName.Trim();
                brandInfo.Logo = model.Logo;

                AdminBrands.UpdateBrand(brandInfo);
                AddMallAdminLog("修改品牌", "修改品牌,品牌ID为:" + brandId);
                return PromptView("品牌修改成功");
            }

            Load();
            return View(model);
        }

        /// <summary>
        /// 删除品牌
        /// </summary>
        public ActionResult Del(int brandId = -1)
        {
            int result = AdminBrands.DeleteBrandById(brandId);
            if (result == 0)
                return PromptView("删除失败,请先删除此品牌下的商品");
            AddMallAdminLog("删除品牌", "删除品牌,品牌ID为:" + brandId);
            return PromptView("品牌删除成功");
        }

        private void Load()
        {
            string allowImgType = string.Empty;
            string[] imgTypeList = StringHelper.SplitString(BMAConfig.MallConfig.UploadImgType, ",");
            foreach (string imgType in imgTypeList)
                allowImgType += string.Format("*{0};", imgType.ToLower());

            string[] sizeList = StringHelper.SplitString(WorkContext.MallConfig.BrandThumbSize);

            ViewData["size"] = sizeList[sizeList.Length / 2];
            ViewData["allowImgType"] = allowImgType;
            ViewData["maxImgSize"] = BMAConfig.MallConfig.UploadImgSize;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
        }
    }
}
