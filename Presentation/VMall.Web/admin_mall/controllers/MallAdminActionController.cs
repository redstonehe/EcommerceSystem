using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台商城管理员组控制器类
    /// </summary>
    public partial class MallAdminActionController : BaseMallAdminController
    {
        /// <summary>
        /// 商城一级菜单列表
        /// </summary>
        public ActionResult List()
        {

            MallAdminActionListModel model = new MallAdminActionListModel()
            {
                MallAdminActionList = MallAdminActions.GetMallAdminActionList().FindAll(x => x.ParentId == 0).OrderBy(a => a.Aid).ToList()
            };
            MallUtils.SetAdminRefererCookie(Url.Action("list"));
            return View(model);
        }
        /// <summary>
        /// 商城二级菜单列表
        /// </summary>
        public ActionResult GetSubMenu(int parentId)
        {
            return this.Json(MallAdminActions.GetMallAdminActionList().FindAll(x => x.ParentId == parentId).OrderByDescending(a => a.Aid).ToList(), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 添加菜单
        /// </summary>
        [HttpGet]
        public ActionResult Add()
        {
            MallAdminActionModel model = new MallAdminActionModel();
            model.ParentId = 0;
            model.DisplayOrder = 0;
            LoadSelect();
            return View(model);
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        [HttpPost]
        public ActionResult Add(MallAdminActionModel model)
        {
            if (MallAdminActions.GetMallAdminActionIdByTitle(model.Title) > 0)
                ModelState.AddModelError("AdminActionTitle", "名称已经存在");

            if (ModelState.IsValid)
            {
                MallAdminActionInfo mallAdminActionInfo = new MallAdminActionInfo()
                {
                    Title = model.Title,
                    Action = model.Action,
                    ParentId = model.ParentId,
                    DisplayOrder = model.DisplayOrder

                };

                MallAdminActions.CreateMallAdminAction(mallAdminActionInfo);
                AddMallAdminLog("添加商城菜单", "添加商城菜单,菜单为:" + model.Title);
                return PromptView("添加商城菜单");
            }
            LoadSelect();
            return View(model);
        }

        /// <summary>
        /// 编辑菜单
        /// </summary>
        [HttpGet]
        public ActionResult Edit(int aid = -1)
        {
            if (aid < 0)
                return PromptView("菜单不存在");
            MallAdminActionInfo actionInfo = MallAdminActions.GetMallAdminActionById(aid);
            if (actionInfo == null)
                return PromptView("菜单不存在");

            MallAdminActionModel model = new MallAdminActionModel();
            model.Title = actionInfo.Title;
            model.Action = actionInfo.Action;
            model.ParentId = actionInfo.ParentId;
            model.DisplayOrder = actionInfo.DisplayOrder;

            LoadSelect();
            return View(model);
        }

        /// <summary>
        /// 编辑菜单
        /// </summary>
        [HttpPost]
        public ActionResult Edit(MallAdminActionModel model, int aid = -1)
        {
            if (aid < 0)
                return PromptView("菜单不存在");

            MallAdminActionInfo actionInfo = MallAdminActions.GetMallAdminActionById(aid);
            if (actionInfo == null)
                return PromptView("菜单不存在");

            int aid2 = MallAdminActions.GetMallAdminActionIdByTitle(model.Title);
            if (aid2 > 0 && aid2 != aid)
                ModelState.AddModelError("AdminActionTitle", "名称已经存在");

            if (ModelState.IsValid)
            {

                actionInfo.Title = model.Title;
                actionInfo.Action = model.Action == null ? "" : model.Action;
                actionInfo.ParentId = model.ParentId;
                actionInfo.DisplayOrder = model.DisplayOrder;

                MallAdminActions.UpdateMallAdminAction(actionInfo);
                AddMallAdminLog("修改商城菜单", "修改菜单,商城菜单修改为:" + model.Title);
                return PromptView("商城菜单修改成功");
            }

            LoadSelect();
            return View(model);
        }

        

        private void Load()
        {
            //ViewData["mallAdminActionTree"] = MallAdminActions.GetMallAdminActionTree();
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
        }

        private void LoadSelect()
        {
            List<SelectListItem> menuLevelList = new List<SelectListItem>();
            menuLevelList.Add(new SelectListItem() { Text = "顶级菜单", Value = "0" });
            foreach (var info in MallAdminActions.GetMallAdminActionList().FindAll(x => x.ParentId == 0))
            {
                menuLevelList.Add(new SelectListItem() { Text = "--" + info.Title, Value = info.Aid.ToString() });
            }
            ViewData["menuLevelList"] = menuLevelList;

            //0表示对普通管理员显示，1表示仅对开发管理员显示
            List<SelectListItem> displayOrderList = new List<SelectListItem>();
            displayOrderList.Add(new SelectListItem() { Text = "显示", Value = "0" });
            displayOrderList.Add(new SelectListItem() { Text = "隐藏(开发管理员可见)", Value = "1" });
            ViewData["displayOrderList"] = displayOrderList;

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="mallAGid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OperateRights(int aid = -1, int mallAGid = -1)
        {
            if (aid < 0)
                return PromptView("菜单不存在");
            if (mallAGid < 0)
                return PromptView("管理员组不存在");

            MallAdminActionInfo actionInfo = MallAdminActions.GetMallAdminActionById(aid);

            if (actionInfo == null)
                return PromptView("菜单不存在");
            MallAdminActionInfo ParentActionInfo = MallAdminActions.GetMallAdminActionById(actionInfo.ParentId);

            MallAdminGroupInfo adminGroup = null;
            foreach (MallAdminGroupInfo info in MallAdminGroups.GetMallAdminGroupList())
            {
                if (info.MallAGid == mallAGid)
                {
                    adminGroup = info;
                    break;
                }
            }
            if (adminGroup == null)
                return PromptView("理员组不存在");

            List<AdminOperateRightsInfo> rights = new AdminOperateRights().GetList(string.Format(" Aid={0} AND MallAGid={1} ", aid, mallAGid));
            OperateRightsModel model = new OperateRightsModel();
            model.Aid = actionInfo.Aid;
            model.mallAGid = adminGroup.MallAGid;
            model.ActionInfo = actionInfo;
            model.ParentActionInfo = ParentActionInfo;
            model.adminGroup = adminGroup;
            model.HasRightsList = rights.FindAll(x => x.State == 1).Select(x => x.Operate).ToArray();
            model.ALLRightsList = rights;
            //List<SelectListItem> mallAdminGroupList = new List<SelectListItem>();
            //mallAdminGroupList.Add(new SelectListItem() { Text = "全部组", Value = "0" });
            //foreach (MallAdminGroupInfo info in MallAdminGroups.GetMallAdminGroupList())
            //{
            //    if(info.MallAGid==mallAGid)
            //        mallAdminGroupList.Add(new SelectListItem() { Text = info.Title, Value = info.MallAGid.ToString(),Selected=true });
            //    else
            //        mallAdminGroupList.Add(new SelectListItem() { Text = info.Title, Value = info.MallAGid.ToString() });

            //}
            //ViewData["mallAdminGroupList"] = mallAdminGroupList;
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="aid"></param>
        /// <param name="mallAGid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OperateRights(OperateRightsModel model, int aid = -1, int mallAGid = -1)
        {
            if (aid < 0)
                return PromptView("菜单不存在");
            if (mallAGid < 0)
                return PromptView("管理员组不存在");

            MallAdminActionInfo actionInfo = MallAdminActions.GetMallAdminActionById(aid);

            if (actionInfo == null)
                return PromptView("菜单不存在");
            MallAdminActionInfo ParentActionInfo = MallAdminActions.GetMallAdminActionById(actionInfo.ParentId);

            MallAdminGroupInfo adminGroup = null;
            foreach (MallAdminGroupInfo info in MallAdminGroups.GetMallAdminGroupList())
            {
                if (info.MallAGid == mallAGid)
                {
                    adminGroup = info;
                    break;
                }
            }
            if (adminGroup == null)
                return PromptView("理员组不存在");
            if (ModelState.IsValid)
            {
                List<AdminOperateRightsInfo> AllRights = new AdminOperateRights().GetList(string.Format(" [Aid]={0} AND [MallAGid]={1} ", aid, mallAGid));
                List<AdminOperateRightsInfo> selectRights = new List<AdminOperateRightsInfo>();
                if (model.SelectRightsList != null)
                {
                    foreach (string item in model.SelectRightsList)
                    {
                        AdminOperateRightsInfo info = new AdminOperateRightsInfo();
                        info.RightsId = TypeHelper.StringToInt(item);
                        selectRights.Add(info);
                    }
                }
                    //List<AdminOperateRightsInfo> noSelectRights = AllRights.Select(x => x.RightsId).ToList().Except(selectRights.Select(x => x.RightsId)).ToList();
                    List<int> selectRightsId = selectRights.Select(x => x.RightsId).ToList();
                    List<int> noSelectRightsId = AllRights.Select(x => x.RightsId).ToList().Except(selectRights.Select(x => x.RightsId)).ToList();
                    AdminOperateRights.EditRights(selectRightsId, noSelectRightsId);

                    AddMallAdminLog("修改商城管理员组操作权限", "修改商城管理员组操作权限,商城管理员组ID为:" + mallAGid);
                    return PromptView(Url.Action("edit", "malladmingroup", new { mallAGid = mallAGid }), "商城管理员组操作权限修改成功");
                
            }

            Load();
            return View();
        }


        public ActionResult RightsList(int aid)
        {
            if (aid < 0)
                return PromptView("菜单不存在");

            MallAdminActionInfo actionInfo = MallAdminActions.GetMallAdminActionById(aid);

            if (actionInfo == null)
                return PromptView("菜单不存在");
            MallAdminActionInfo ParentActionInfo = MallAdminActions.GetMallAdminActionById(actionInfo.ParentId);

            RightsListModel model = new RightsListModel();
            List<AdminOperateInfo> list = new AdminOperate().GetList(string.Format(" [Aid]={0} or [Aid]=0", aid));
            model.Aid = aid;
            model.ActionInfo = actionInfo;
            model.ParentActionInfo = ParentActionInfo;
            model.list = list;
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddRights(int aid = -1)
        {
            if (aid < 0)
                return PromptView("菜单不存在");
            MallAdminActionInfo actionInfo = MallAdminActions.GetMallAdminActionById(aid);
            if (actionInfo == null)
                return PromptView("菜单不存在");
            MallAdminActionInfo ParentActionInfo = MallAdminActions.GetMallAdminActionById(actionInfo.ParentId);
            ViewData["actionname"] = actionInfo.Title;
            ViewData["parentactionname"] = ParentActionInfo.Title;
            OperateModel model = new OperateModel();
            model.aid = aid;
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="aid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddRights(OperateModel model)
        {
            if (model.aid < 0)
                return PromptView("菜单不存在");
            MallAdminActionInfo actionInfo = MallAdminActions.GetMallAdminActionById(model.aid);
            if (actionInfo == null)
                return PromptView("菜单不存在");

            if (new AdminOperate().GetModel(string.Format(" [Operate]='{0}' AND [Aid]={1}", model.Operate, model.aid)) != null)
                ModelState.AddModelError("AdminActionTitle", "操作已经存在");

            if (ModelState.IsValid)
            {
                AdminOperateInfo info = new AdminOperateInfo()
                {
                    OperateName=model.OperateName,
                    Operate=model.Operate,
                    Aid = model.aid
                };
                new AdminOperate().Add(info);
                new AdminOperateRights().BatchInsert(model.Operate, model.aid);
                AddMallAdminLog("添加商城操作值", "添加商城操作值:" + model.OperateName);
                return PromptView(Url.Action("RightsList", new { aid =model.aid}), "添加商城操作值");
            }
            
            return View(model);
        }
    }
}
