using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;
using System.Text;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台消息控制器类
    /// </summary>
    public partial class InformController : BaseMallAdminController
    {
        Informtype informTypeBLL = new Informtype();
        Inform informBLL = new Inform();
        /// <summary>
        ///  类型列表
        /// </summary>
        public ActionResult TypeList()
        {
            List<InformtypeInfo> TypeList = informTypeBLL.GetList();
            MallUtils.SetAdminRefererCookie(Url.Action("TypeList"));
            return View(TypeList);
        }

        /// <summary>
        /// 添加 类型
        /// </summary>
        [HttpGet]
        public ActionResult AddInformType()
        {
            InformTypeModel model = new InformTypeModel();
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 添加 类型
        /// </summary>
        [HttpPost]
        public ActionResult AddInformType(InformTypeModel model)
        {
            if (informTypeBLL.GetModel(string.Format(" name='{0}' ", model.InformTypeName.Trim())) != null)
                ModelState.AddModelError("TypeName", "名称已经存在");

            if (ModelState.IsValid)
            {
                InformtypeInfo InformtypeInfo = new InformtypeInfo()
                {
                    name = model.InformTypeName,
                    displayorder = model.DisplayOrder,
                    mallsource = 0
                };
                informTypeBLL.Add(InformtypeInfo);
                AddMallAdminLog("添加消息类型", "添加消息类型, 类型为:" + model.InformTypeName);
                return PromptView("消息类型添加成功");
            }
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 编辑类型
        /// </summary>
        [HttpGet]
        public ActionResult EditInformType(int TypeId = -1)
        {
            InformtypeInfo InformtypeInfo = informTypeBLL.GetModel(TypeId);
            if (InformtypeInfo == null)
                return PromptView("消息类型不存在");

            InformTypeModel model = new InformTypeModel();
            model.InformTypeName = InformtypeInfo.name;
            model.DisplayOrder = InformtypeInfo.displayorder;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 编辑消息类型
        /// </summary>
        [HttpPost]
        public ActionResult EditInformType(InformTypeModel model, int TypeId = -1)
        {
            InformtypeInfo InformtypeInfo = informTypeBLL.GetModel(TypeId);
            if (InformtypeInfo == null)
                return PromptView("消息类型不存在");

            InformtypeInfo InformtypeInfo2 = informTypeBLL.GetModel(string.Format(" name='{0}' ", model.InformTypeName));
            if (InformtypeInfo2 != null && InformtypeInfo2.typeid != TypeId)
                ModelState.AddModelError("InformTypeName", "消息类型名称已经存在");

            if (ModelState.IsValid)
            {
                InformtypeInfo.name = model.InformTypeName;
                InformtypeInfo.displayorder = model.DisplayOrder;
                informTypeBLL.Update(InformtypeInfo, "name,displayorder");
                AddMallAdminLog("修改消息类型", "修改消息类型, 类型ID为:" + TypeId);
                return PromptView(" 类型修改成功");
            }

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }


        /// <summary>
        /// 获得列表搜索条件
        /// </summary>
        /// <returns></returns>
        public string GetCondition(int typeid, string title, string content, DateTime? startDate, DateTime? endDate, int uid = 0)
        {
            StringBuilder condition = new StringBuilder();
            if (uid > 0)
                condition.AppendFormat(" AND T.[uid] = {0} ", uid);
            if (!string.IsNullOrWhiteSpace(title))
                condition.AppendFormat(" AND T.[title] like '%{0}%' ", title.Trim());
            if (!string.IsNullOrWhiteSpace(content))
                condition.AppendFormat(" AND T.[content] like '%{0}%' ", content.Trim());

            if (typeid > 0)
                condition.AppendFormat(" AND T.[typeid] = {0} ", typeid);
            if (startDate.HasValue)
                condition.AppendFormat(" AND T.[addtime] >= '{0}' ", startDate.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            if (endDate.HasValue)
                condition.AppendFormat(" AND T.[addtime] <= '{0}' ", endDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));

            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }
        /// <summary>
        /// 后台获得列表排序
        /// </summary>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public string AdminGetListSort(string sortColumn, string sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
                sortColumn = "[id]";
            if (string.IsNullOrWhiteSpace(sortDirection))
                sortDirection = "DESC";

            return string.Format("{0} {1} ", sortColumn, sortDirection);
        }


        /// <summary>
        ///  列表
        /// </summary>
        public ActionResult InformList(string Title, DateTime? StartTime, DateTime? EndTime, string sortColumn, string sortDirection, int InformTypeId = 0, int pageSize = 15, int pageNumber = 1, string Content = "")
        {
            string condition = GetCondition(InformTypeId, Title, Content, StartTime, EndTime);
            string sort = AdminGetListSort(sortColumn, sortDirection);

            PageModel pageModel = new PageModel(pageSize, pageNumber, informBLL.AdminGetRecordCount(condition));

            InformListModel model = new InformListModel()
            {
                InformList = informBLL.AdminGetListByPage(condition, sort, (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                InformTypeId = InformTypeId,
                InformTitle = Title
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&sortColumn={3}&sortDirection={4}&InformTypeId={5}&Title={6}&Content={7}&StartTime={8}&EndTime={9}",
                                                          Url.Action("InformList"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          sortColumn,
                                                          sortDirection,
                                                          InformTypeId, Title, Content, StartTime, EndTime));
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "全部类型", Value = "0" });
            foreach (InformtypeInfo InformtypeInfo in informTypeBLL.GetList())
            {
                list.Add(new SelectListItem() { Text = InformtypeInfo.name, Value = InformtypeInfo.typeid.ToString() });
            }
            ViewData["InformTypeList"] = list;
            return View(model);
        }

        /// <summary>
        /// 添加 
        /// </summary>
        [HttpGet]
        public ActionResult AddInform()
        {
            InformModel model = new InformModel();
            Load();
            return View(model);
        }

        /// <summary>
        /// 添加 
        /// </summary>
        [HttpPost]
        public ActionResult AddInform(InformModel model)
        {
            //if (informBLL.GetModel(string.Format(" title='{0}' ", model.Title)).id > 0)
            //    ModelState.AddModelError("Title", "标题已经存在");

            if (ModelState.IsValid)
            {
                if (model.IsBatchSend == 1)
                {
                    InformInfo Info = new InformInfo()
                    {
                        typeid = model.InformTypeId,
                        uid = 0,
                        addtime = DateTime.Now,
                        title = model.Title,
                        content = model.Body ?? "",
                        readtime = DateTime.Now
                    };
                    informBLL.AdminBatchSend(Info);
                    AddMallAdminLog("群发消息", "群发消息, 为:" + model.Title);
                }
                else
                {
                    InformInfo Info = new InformInfo()
                    {
                        typeid = model.InformTypeId,
                        isshow = model.IsShow,
                        istop = model.IsTop,
                        uid = model.Uid,
                        displayorder = model.DisplayOrder,
                        addtime = DateTime.Now,
                        title = model.Title,
                        content = model.Body ?? "",
                        readtime = DateTime.Now
                    };
                    informBLL.Add(Info);
                    AddMallAdminLog("添加消息", "添加消息, 为:" + model.Title);
                }

                return PromptView("消息添加成功");
            }
            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑 
        /// </summary>
        [HttpGet]
        public ActionResult EditInfrom(int Id = -1)
        {
            InformInfo Info = informBLL.GetModel(Id);
            if (Info == null)
                return PromptView(" 不存在");

            InformModel model = new InformModel();
            model.InformTypeId = Info.typeid;
            model.IsShow = Info.isshow;
            model.IsTop = Info.istop;
            model.Uid = Info.uid;
            model.DisplayOrder = Info.displayorder;
            model.Title = Info.title;
            model.Body = Info.content;

            Load();
            return View(model);
        }

        /// <summary>
        /// 编辑 
        /// </summary>
        [HttpPost]
        public ActionResult EditInfrom(InformModel model, int Id = -1)
        {
            InformInfo Info = informBLL.GetModel(Id);
            if (Info == null)
                return PromptView("消息不存在");

            if (ModelState.IsValid)
            {
                Info.typeid = model.InformTypeId;
                Info.isshow = model.IsShow;
                Info.istop = model.IsTop;
                //Info.uid = model.Uid;
                Info.displayorder = model.DisplayOrder;
                Info.title = model.Title;
                Info.content = model.Body ?? "" ;
                informBLL.Update(Info);
                AddMallAdminLog("修改消息", "修改消息, ID为:" + Id);
                return PromptView("消息修改成功");
            }

            Load();
            return View(model);
        }

        /// <summary>
        /// 删除 
        /// </summary>
        public ActionResult DelInform(int[] IdList)
        {
            informBLL.DeleteList(string.Join(",", IdList));
            AddMallAdminLog("删除 ", "删除 , ID为:" + CommonHelper.IntArrayToString(IdList));
            return PromptView(" 删除成功");
        }

        private void Load()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "请选择类型", Value = "0" });
            foreach (InformtypeInfo InformtypeInfo in informTypeBLL.GetList())
            {
                list.Add(new SelectListItem() { Text = InformtypeInfo.name, Value = InformtypeInfo.typeid.ToString() });
            }
            ViewData["InformTypeList"] = list;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
        }
    }
}
