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
    using VMall.Web.MallAdmin.bll;
    /// <summary>
    /// 商城后台品牌控制器类
    /// </summary>
    public partial class ChannelController : BaseMallAdminController
    {
        ChannelBLL bllChannel = new ChannelBLL();
        GroupProducts bllGroupProducts = new GroupProducts();
        #region 频道分区

        /// <summary>
        /// 频道列表
        /// </summary>
        /// <param name="channelName">频道名称</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ActionResult List(string channelName, int pageSize = 15, int pageNumber = 1)
        {
            string condition = "   mallsource=0  ";
            if (!string.IsNullOrWhiteSpace(channelName))
                condition += string.Format(" AND [name] like '%{0}%' ", channelName);

            PageModel pageModel = new PageModel(pageSize, pageNumber, bllChannel.AdminGetChannelCount(condition));

            ChannelListModel model = new ChannelListModel()
            {
                ChannelList = Channel.GetChannelListByWhere(condition, pageModel.PageNumber, pageModel.PageSize),
                PageModel = pageModel,
                ChannelName = channelName
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&brandName={3}",
                                                          Url.Action("list"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          channelName));
            return View(model);
        }

        /// <summary>
        /// 频道选择列表
        /// </summary>
        /// <param name="brandName">频道名称</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ContentResult SelectList(string channelName, int pageNumber = 1, int pageSize = 24)
        {
            string condition = " mallsource=0  ";
            if (!string.IsNullOrWhiteSpace(channelName))
                condition += string.Format(" AND [name] like '%{0}%' ", channelName);
            PageModel pageModel = new PageModel(pageSize, pageNumber, bllChannel.AdminGetChannelCount(condition));

            DataTable ChannelSelectList = bllChannel.AdminGetChannelSelectList(pageModel.PageSize, pageModel.PageNumber, condition);

            StringBuilder result = new StringBuilder("{");
            result.AppendFormat("\"totalPages\":\"{0}\",\"pageNumber\":\"{1}\",\"items\":[", pageModel.TotalPages, pageModel.PageNumber);
            foreach (DataRow row in ChannelSelectList.Rows)
                result.AppendFormat("{0}\"id\":\"{1}\",\"name\":\"{2}\"{3},", "{", row["chid"], row["name"].ToString().Trim(), "}");

            if (ChannelSelectList.Rows.Count > 0)
                result.Remove(result.Length - 1, 1);

            result.Append("]}");
            return Content(result.ToString());
        }

        /// <summary>
        /// 添加
        /// </summary>
        [HttpGet]
        public ActionResult Add()
        {
            ChannelModel model = new ChannelModel();
            Load();
            return View(model);
        }

        /// <summary>
        /// 添加
        /// </summary>
        [HttpPost]
        public ActionResult Add(ChannelModel model)
        {
            ChannelInfo info2 = Channel.GetChannelByName(model.Name);
            if (info2 != null && info2.ChId > 0)
                ModelState.AddModelError("Name", "名称已经存在");

            if (ModelState.IsValid)
            {
                ChannelInfo info = new ChannelInfo()
                {
                    DisplayOrder = model.DisplayOrder,
                    Name = model.Name,
                    State = model.State,
                    Description = model.Description ?? "",
                    LinkType = model.LinkType,
                    LinkUrl = model.LinkUrl ?? ""
                };

                new Channel().Add(info);
                VMall.Core.BMACache.Remove("/Mall/ChannelList");
                AddMallAdminLog("添加频道", "添加频道,频道为:" + model.Name);
                return PromptView("频道添加成功");
            }
            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        [HttpGet]
        public ActionResult Edit(int chId = -1)
        {
            ChannelInfo info = new Channel().GetModel(chId);// Channel.GetChannelById(chId); 
            if (info == null)
                return PromptView("频道不存在");

            ChannelModel model = new ChannelModel();
            model.DisplayOrder = info.DisplayOrder;
            model.Name = info.Name;
            model.Description = info.Description;
            model.State = info.State;
            model.LinkType = info.LinkType;
            model.LinkUrl = info.LinkUrl;
            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        [HttpPost]
        public ActionResult Edit(ChannelModel model, int chId = -1)
        {
            ChannelInfo info = Channel.GetChannelById(chId);
            if (info == null)
                return PromptView("频道不存在");

            ChannelInfo info2 = Channel.GetChannelByName(model.Name);
            if (info2 != null && info2.ChId > 0 && info2.ChId != info.ChId)
                ModelState.AddModelError("Name", "名称已经存在");

            if (ModelState.IsValid)
            {
                info.DisplayOrder = model.DisplayOrder;
                info.Name = model.Name;
                info.State = model.State;
                info.Description = model.Description ?? "";
                info.LinkType = model.LinkType;
                info.LinkUrl = model.LinkUrl ?? "";
                //Channel.UpdateChannel(info);
                new Channel().Update(info);
                VMall.Core.BMACache.Remove("/Mall/ChannelList");
                AddMallAdminLog("修改频道", "修改频道,ID为:" + chId);
                return PromptView("频道修改成功");
            }

            Load();
            return View(model);
        }

        /// <summary>
        /// 删除品牌
        /// </summary>
        //public ActionResult Del(int brandId = -1)
        //{
        //    int result = AdminBrands.DeleteBrandById(brandId);
        //    if (result == 0)
        //        return PromptView("删除失败,请先删除此品牌下的商品");
        //    AddMallAdminLog("删除品牌", "删除品牌,品牌ID为:" + brandId);
        //    return PromptView("品牌删除成功");
        //}

        #endregion

        #region 分区系列
        public ActionResult IndexList()
        {
            return View();
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
        /// <summary>
        /// 分区系列列表
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public ActionResult GroupList(int channelid)
        {
            GroupListModel model = new GroupListModel()
            {
                GroupList = bllGroupProducts.GetList(string.Format(" ChannelId={0} ", channelid)),
                ChannelId = channelid
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?channelid={1}", Url.Action("GroupList"), channelid));
            return View(model);
        }

        /// <summary>
        /// 添加分区系列
        /// </summary>
        [HttpGet]
        public ActionResult AddGroupProduct(int chid)
        {
            GroupProductModel model = new GroupProductModel();
            model.ChannelId = chid;
            Load();
            return View(model);
        }

        /// <summary>
        /// 添加分区系列
        /// </summary>
        [HttpPost]
        public ActionResult AddGroupProduct(GroupProductModel model, int chid)
        {
            if (ModelState.IsValid)
            {
                GroupProductInfo GPInfo = new GroupProductInfo()
                {
                    ChannelId = chid,
                    GroupTitle = model.GroupTitle,
                    Type = model.Type,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    State = model.State,
                    Link = model.Link ?? "",
                    GroupLogo = model.GroupLogo,
                    DisplayOrder = model.DisplayOrder,
                    Products = model.Products ?? "",
                    CreationTime = DateTime.Now,
                    ExtField1 = model.ExtField1 ?? "",
                    ExtField2 = model.ExtField2 ?? "",
                    ExtField3 = model.ExtField3 ?? "",
                    ExtField4 = model.ExtField4 ?? "",
                    ExtField5 = model.ExtField5 ?? ""
                };
                bllGroupProducts.Add(GPInfo);

                return PromptView("添加分区系列成功");
            }
            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑分区系列
        /// </summary>
        [HttpGet]
        public ActionResult EditGroupProduct(int groupid = -1)
        {
            GroupProductInfo groupInfo = bllGroupProducts.GetModel(groupid);
            if (groupInfo == null)
                return PromptView("分区系列不存在");

            GroupProductModel model = new GroupProductModel();

            model.ChannelId = groupInfo.ChannelId;
            model.GroupTitle = groupInfo.GroupTitle;
            groupInfo.Type = groupInfo.Type;
            model.StartTime = groupInfo.StartTime;
            model.EndTime = groupInfo.EndTime;
            model.State = groupInfo.State;
            model.Link = groupInfo.Link;
            model.GroupLogo = groupInfo.GroupLogo;
            model.DisplayOrder = groupInfo.DisplayOrder;
            model.Products = groupInfo.Products;
            model.ExtField1 = groupInfo.ExtField1;
            model.ExtField2 = groupInfo.ExtField2;
            model.ExtField3 = groupInfo.ExtField3;
            model.ExtField4 = groupInfo.ExtField4;
            model.ExtField5 = groupInfo.ExtField5;
            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑分区系列
        /// </summary>
        [HttpPost]
        public ActionResult EditGroupProduct(GroupProductModel model, int groupid = -1)
        {
            GroupProductInfo groupInfo = bllGroupProducts.GetModel(groupid);
            if (groupInfo == null)
                return PromptView("分区系列不存在");

            GroupProductInfo groupInfo2 = bllGroupProducts.GetModel(string.Format("GroupTitle='{0}'", model.GroupTitle));
            if (groupInfo2 != null)
            {
                if (groupInfo2.Groupid != groupid)
                    ModelState.AddModelError("GroupName", "名称已经存在");
            }

            if (ModelState.IsValid)
            {
                groupInfo.ChannelId = model.ChannelId;
                groupInfo.GroupTitle = model.GroupTitle;
                groupInfo.Type = model.Type;
                groupInfo.StartTime = model.StartTime;
                groupInfo.EndTime = model.EndTime;
                groupInfo.State = model.State;
                groupInfo.Link = model.Link ?? "";
                groupInfo.GroupLogo = model.GroupLogo;
                groupInfo.DisplayOrder = model.DisplayOrder;
                //groupInfo.Products = model.Products ?? "";
                groupInfo.CreationTime = DateTime.Now;
                groupInfo.ExtField1 = model.ExtField1 ?? "";
                groupInfo.ExtField2 = model.ExtField2 ?? "";
                groupInfo.ExtField3 = model.ExtField3 ?? "";
                groupInfo.ExtField4 = model.ExtField4 ?? "";
                groupInfo.ExtField5 = model.ExtField5 ?? "";

                bllGroupProducts.Update(groupInfo);
                Load();
                return PromptView("分区系列修改成功");
            }
            return View(model);
        }

        /// <summary>
        /// 删除分区系列
        /// </summary>
        public ActionResult DelGroupProduct(int groupid = -1)
        {
            bllGroupProducts.Delete(groupid);
            return PromptView("分区系列删除成功");
        }


        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pmId">活动id</param>
        /// <returns></returns>
        public ActionResult GroupProductList(int groupid, int channelid, int pageSize = 15, int pageNumber = 1)
        {
            GroupProductInfo groupInfo = bllGroupProducts.GetModel(groupid);
            if (groupInfo == null)
                return PromptView("分区系列不存在");
            string[] pidArray = StringHelper.SplitString(groupInfo.Products);

            PageModel pageModel = new PageModel(pageSize, pageNumber, pidArray.Length);

            GroupProductListModel model = new GroupProductListModel()
            {
                ProductList = string.IsNullOrEmpty(groupInfo.Products) ? new List<PartProductInfo>() : Products.GetPartProductList(groupInfo.Products),
                PageModel = pageModel,
                GroupId = groupid,
                ChannelId = channelid
            };

            MallUtils.SetAdminRefererCookie(string.Format("{0}?channelid={1}", Url.Action("GroupList"), channelid));
            return View(model);
        }

        /// <summary>
        /// 更改产品
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="PidGroups"></param>
        /// <returns></returns>
        public ActionResult UpdateProList(int groupId, string PidGroups)
        {
            GroupProductInfo groupInfo = bllGroupProducts.GetModel(groupId);
            if (groupInfo == null)
                return PromptView("分区系列不存在");
            groupInfo.Products = PidGroups;
            bllGroupProducts.Update(groupInfo);
            return PromptView(MallUtils.GetMallAdminRefererCookie(), "分区系列更改成功");
        }

        /// <summary>
        /// 分区商品选择列表
        /// </summary>
        /// <param name="productName">商品名称</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="cateId">分类id</param>
        /// <param name="brandId">品牌id</param>
        /// <returns></returns>
        public ActionResult ProductSelectList(string productName, int pageNumber = 1, int pageSize = 12, int channelId = -1, int cateId = -1, int brandId = -1)
        {
            PageModel pageModel = new PageModel(pageSize, pageNumber, Channel.GetChannelProductCount(channelId, "", 0, new string[0], 0));

            List<StoreProductInfo> dt = Channel.GetChannelProductList(pageModel.PageSize, pageModel.PageNumber, channelId, "", 0, 0, new string[0], 0, 0, 0, productName);
            StringBuilder result = new System.Text.StringBuilder("{");
            result.AppendFormat("\"totalPages\":\"{0}\",\"pageNumber\":\"{1}\",\"items\":[", pageModel.TotalPages, pageModel.PageNumber);
            foreach (var row in dt)
                result.AppendFormat("{0}\"id\":\"{1}\",\"name\":\"{2}\",\"shopprice\":\"{3}\",\"state\":\"{4}\"{5},", "{", row.Pid, row.Name.Trim(), row.ShopPrice, row.State, "}");

            if (dt.Count > 0)
                result.Remove(result.Length - 1, 1);

            result.Append("]}");
            return Content(result.ToString());
        }

        #endregion
    }
}
