using System;
using System.Web;
using System.Data;
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
    /// 商城后台用户控制器类
    /// </summary>
    public partial class UserController : BaseMallAdminController
    {
        AgentStock AgentStockBLL = new AgentStock();
        AgentStockDetail AgentStockDetailBLL = new AgentStockDetail();

        #region 用户列表操作

        /// <summary>
        /// 后台获得用户列表条件
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="email">邮箱</param>
        /// <param name="mobile">手机</param>
        /// <param name="userRid">用户等级</param>
        /// <param name="mallAGid">商城管理员组</param>
        /// <returns></returns>
        public string GetUserListCondition(string userName, int uid, string email, string mobile, int userRid, int mallAGid, int userType = -1, int agentType = 0, int mallSource = -1)
        {
            StringBuilder condition = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(userName))
                condition.AppendFormat(" AND (a.[username] like '%{0}%' OR a.[email] like '%{0}%' OR a.[mobile] like '%{0}%' ) ", userName.Trim(), RDBSHelper.RDBSTablePre);
            if (uid > 0)
                condition.AppendFormat(" AND a.[uid] ={0} ", uid, RDBSHelper.RDBSTablePre);
            if (!string.IsNullOrWhiteSpace(email))
                condition.AppendFormat(" AND a.[email] like '%{0}%' ", email.Trim(), RDBSHelper.RDBSTablePre);

            if (!string.IsNullOrWhiteSpace(mobile))
                condition.AppendFormat(" AND a.[mobile] like '%{0}%' ", mobile.Trim(), RDBSHelper.RDBSTablePre);

            if (userRid > 0)
                condition.AppendFormat(" AND a.[userrid] = {0} ", userRid, RDBSHelper.RDBSTablePre);

            if (mallAGid > 0)
                condition.AppendFormat(" AND a.[mallagid] = {0} ", mallAGid, RDBSHelper.RDBSTablePre);
            if (userType == 0)
                condition.AppendFormat(" AND a.[isdirsaleuser] = 0  AND  a.[isfxuser] = 0 ", mallAGid, RDBSHelper.RDBSTablePre);
            else if (userType == 1)
                condition.AppendFormat(" AND a.[isdirsaleuser] = 1 ", mallAGid, RDBSHelper.RDBSTablePre);
            else if (userType == 2)
                condition.AppendFormat(" AND a.[isdirsaleuser] = 0 AND a.[isfxuser]>=1 ", mallAGid, RDBSHelper.RDBSTablePre);
            if (agentType > 0)
                condition.AppendFormat(" AND a.[agenttype] = {0}  ", agentType);
            if (mallSource > -1)
                condition.AppendFormat(" AND a.[mallsource] = {0}  ", mallSource);
            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        public ActionResult List(string userName, string email, string mobile, int userRid = 0, int mallAGid = 0, int pageNumber = 1, int pageSize = 10, string realName = "", int userType = -1, int agentType = -1, int mallSource = -1, int uid = 0)
        {
            string condition = GetUserListCondition(userName, uid, email, mobile, userRid, mallAGid, userType, agentType, mallSource);
            string sort = AdminUsers.AdminGetUserListSort("", "");

            if (!string.IsNullOrEmpty(HttpContext.Request.Form["IsOutPut"]) && HttpContext.Request.Form["IsOutPut"].Trim() == "1")
            {
                //ViewData["IsOutPut"] = "0";
                return OutPutUserList(condition, realName);
            }

            PageModel pageModel = new PageModel(pageSize, pageNumber, AdminUsers.AdminGetUserCount(condition, realName));

            UserListModel model = new UserListModel()
            {
                UserList = AdminUsers.AdminGetUserList(pageModel.PageSize, pageModel.PageNumber, condition, sort, realName),
                PageModel = pageModel,
                UserName = userName,
                Uid = uid,
                Email = email,
                Mobile = mobile,
                UserRid = userRid,
                MallAGid = mallAGid,
                RealName = realName,
                UserType = userType,
                AgentType = agentType,
                MallSource = mallSource
            };
            List<SelectListItem> userRankList = new List<SelectListItem>();
            userRankList.Add(new SelectListItem() { Text = "全部等级", Value = "0" });
            foreach (UserRankInfo info in AdminUserRanks.GetUserRankList())
            {
                userRankList.Add(new SelectListItem() { Text = info.Title, Value = info.UserRid.ToString() });
            }
            ViewData["userRankList"] = userRankList;

            List<SelectListItem> mallAdminGroupList = new List<SelectListItem>();
            mallAdminGroupList.Add(new SelectListItem() { Text = "全部组", Value = "0" });
            foreach (MallAdminGroupInfo info in MallAdminGroups.GetMallAdminGroupList())
            {
                if (info.MallAGid != 8)
                    mallAdminGroupList.Add(new SelectListItem() { Text = info.Title, Value = info.MallAGid.ToString() });
            }
            ViewData["mallAdminGroupList"] = mallAdminGroupList;

            List<SelectListItem> userTypeList = new List<SelectListItem>();
            userTypeList.Add(new SelectListItem() { Text = "全部类型", Value = "-1" });
            //userTypeList.Add(new SelectListItem() { Text = "直销会员", Value = "1" });
            //userTypeList.Add(new SelectListItem() { Text = "分销会员", Value = "2" });
            //userTypeList.Add(new SelectListItem() { Text = "普通会员", Value = "0" });
            ViewData["userTypeList"] = userTypeList;
            List<SelectListItem> agentTypeList = new List<SelectListItem>();
            agentTypeList.Add(new SelectListItem() { Text = "全部类型", Value = "-1" });
            foreach (AgentTypeEnum source in Enum.GetValues(typeof(AgentTypeEnum)))
            {
                agentTypeList.Add(new SelectListItem() { Text = source.ToString(), Value = ((int)source).ToString() });
            }
            ViewData["agentTypeList"] = agentTypeList;
            List<SelectListItem> mallSourceList = new List<SelectListItem>();
            mallSourceList.Add(new SelectListItem() { Text = "全部来源", Value = "-1" });
            foreach (MallSourceType source in Enum.GetValues(typeof(MallSourceType)))
            {
                mallSourceList.Add(new SelectListItem() { Text = source.ToString(), Value = ((int)source).ToString() });
            }

            ViewData["mallSourceList"] = mallSourceList;


            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&userName={3}&email={4}&mobile={5}&userRid={6}&mallAGid={7}&realName={8}&userType={9}&agentType={10}&mallSource={11}&uid={12}",
                                                          Url.Action("list"), pageModel.PageNumber, pageModel.PageSize,
                                                          userName, email, mobile, userRid, mallAGid, realName, userType, agentType, mallSource, uid));
            return View(model);
        }

        /// <summary>
        /// 用户列表-导出报表
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="consignee"></param>
        /// <param name="orderState"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult OutPutUserList(string condition, string realName = "")
        {
            bool noCondition = string.IsNullOrWhiteSpace(condition);
            string strWhere = "";
            if (!string.IsNullOrEmpty(realName))
                strWhere += string.Format(" AND b.[realname] like '%{1}%'", RDBSHelper.RDBSTablePre, realName);
            string commandText;
            commandText = string.Format(@"select a.uid 会员ID,a.username 用户名,a.mobile 手机,a.nickname 昵称,b.realname 姓名,CASE gender WHEN 1 THEN '男' WHEN 2 THEN '女' ELSE '保密' END 性别,CASE agenttype WHEN 0 THEN '消费者' WHEN 1 THEN 'VIP会员' WHEN 2 THEN '推广主管' WHEN 3 THEN '推广经理' WHEN 4 THEN '销售总监' WHEN 5 THEN '区域总监' WHEN 6 THEN '产品代理商' WHEN 7 THEN '合伙人' ELSE '未知' END  会员等级,b.idcard 身份证,b.registertime 注册时间, b.lastvisittime 最后访问时间, CASE  WHEN a.otherloginid<>'' THEN '已绑定' ELSE '未绑定' END 绑定微信,CASE usersource WHEN 0 THEN '散客注册' WHEN 1 THEN '推荐注册' ELSE '未知' END 注册方式,(SELECT username+''+mobile+''+nickname FROM hlh_users WHERE uid=a.pid) 推荐人  
 from [hlh_users]  a left join  hlh_userdetails b on a.uid=b.uid
 WHERE 1=1 {0} {1} order by a.[uid] desc", noCondition ? "" : " AND " + condition, string.IsNullOrEmpty(strWhere) ? "" : " AND " + strWhere);

            DataTable dt = AdminOrders.GetOrderByCondition(commandText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "会员列表-" + DateTime.Now.ToString("yyyyMMdd")));
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="DSuid"></param>
        /// <returns></returns>
        public ActionResult GetDSDetail(int uid, int DSuid)
        {
            DSDetailInfo model = new DSDetailInfo();
            model = AccountUtils.GetUserDetailMsg(DSuid);
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 账户列表
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="isDSUser"></param>
        /// <param name="DSuid"></param>
        /// <returns></returns>
        public ActionResult AccountList(int uid, bool isDSUser, int DSuid)
        {
            AccountInfoListModel model = new AccountInfoListModel();
            PartUserInfo userinfo = Users.GetPartUserById(uid);
            model.AccountInfoList = AccountUtils.GetAccountList(uid, isDSUser, DSuid);
            if (userinfo != null)
                model.Username = string.IsNullOrEmpty(userinfo.UserName) ? (string.IsNullOrEmpty(userinfo.Email) ? (string.IsNullOrEmpty(userinfo.Mobile) ? "" : userinfo.Mobile) : userinfo.Email) : userinfo.UserName;
            else
                model.Username = "";
            model.Uid = uid;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 账户详情
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public ActionResult accountDetail(int uid, int accountId, string accountName, int pageNumber = 1, int pageSize = 15, string orderCode = "")
        {
            AccountDetailModel model = new AccountDetailModel();
            PartUserInfo userInfo = Users.GetPartUserById(uid);
            model.AccountName = accountName;
            if (accountId == (int)AccountType.代理账户 || accountId == (int)AccountType.佣金账户)
            {
                model.PageModel = new PageModel(pageSize, pageNumber, Account.GetAccountDetailCount(uid, accountId, orderCode));
                model.AccountDetailList = Account.GetAccountDetailList(uid, accountId, pageNumber, pageSize, orderCode);
            }
            else
            {
                if (!userInfo.IsDirSaleUser)//汇购会员
                {
                    model.PageModel = new PageModel(pageSize, pageNumber, Account.GetAccountDetailCount(uid, accountId));
                    model.AccountDetailList = Account.GetAccountDetailList(uid, accountId, pageNumber, pageSize);
                }
                else//直销会员通过接口取流水
                {
                    int totalCount = 0;
                    model.AccountDetailList = AccountUtils.GetDetail(userInfo.DirSaleUid, accountId, pageNumber, pageSize, ref totalCount);
                    model.PageModel = new PageModel(pageSize, pageNumber, totalCount);
                }
            }
            model.Uid = uid;
            model.AccountId = accountId;
            model.OrderCode = orderCode;
            ViewData["isdirsale"] = userInfo.IsDirSaleUser;
            ViewData["dirsaleuid"] = userInfo.DirSaleUid;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 账户充值
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult AccountRecharge(int uid = -1, int accountId = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            if (accountId != 9)
                return PromptView("该帐号类型不能支持充值操作");

            AccountRechargeModel model = new AccountRechargeModel();
            model.AccountId = (int)AccountType.积分账户;
            model.UserId = uid;
            model.AccountName = Account.GetAccountName(accountId);

            List<SelectListItem> TypeList = new List<SelectListItem>();

            TypeList.Add(new SelectListItem() { Text = "活动赠送", Value = "5" });

            ViewData["TypeList"] = TypeList;
            ViewData["referer"] = Url.Action("accountlist", new { uid = uid, isDSUser = userInfo.IsDirSaleUser, DSuid = userInfo.DirSaleUid });
            return View(model);
        }
        /// <summary>
        /// 账户充值
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AccountRecharge(AccountRechargeModel model, int uid = -1, int accountId = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            if (accountId != 9)
                return PromptView("该帐号类型不能支持充值操作");

            if (ModelState.IsValid)
            {
                //更新汇购会员账户
                if (!userInfo.IsDirSaleUser)
                {
                    Account.UpdateAccountForIn(new AccountInfo()
                    {
                        AccountId = (int)AccountType.积分账户,
                        UserId = userInfo.Uid,
                        TotalIn = model.InAmount
                    });
                    Account.CreateAccountDetail(new AccountDetailInfo()
                    {
                        AccountId = (int)AccountType.积分账户,
                        UserId = userInfo.Uid,
                        CreateTime = DateTime.Now,
                        DetailType = (int)DetailType.活动赠送,
                        InAmount = model.InAmount,
                        OrderCode = "",
                        AdminUid = 1,//system
                        Status = 1,
                        DetailDes = "活动赠送" + MallKey.MallDiscountName_JiFen + "：赠送金额:" + model.InAmount
                    });
                }
                //更新直销的账户
                else if (userInfo.IsDirSaleUser)
                {
                    AccountUtils.UpdateAccountForDir(userInfo.DirSaleUid, (int)AccountType.积分账户, model.InAmount, 0, "", "活动赠送" + MallKey.MallDiscountName_JiFen + "：赠送金额:" + model.InAmount);
                }
                AddMallAdminLog("" + MallKey.MallDiscountName_JiFen + "帐号充值", "红包充值,会员ID为:" + uid + ",充值金额：" + model.InAmount);

                return PromptView(Url.Action("accountlist", new { uid = uid, isDSUser = userInfo.IsDirSaleUser, DSuid = userInfo.DirSaleUid }),  MallKey.MallDiscountName_JiFen + "帐号充值成功");
            }

            return View(model);
        }

        /// <summary>
        /// 账户修改
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult AccountModify(int uid = -1, int accountId = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            List<AccountInfo> accountList = Account.GetAccountListByAid(string.Format(" userid={0} and accountid={1} ", uid, accountId));
            if (accountList.Count <= 0)
                return PromptView("该帐户类型不存在");

            AccountModifyModel model = new AccountModifyModel();
            model.AccountId = accountId;
            model.UserId = uid;
            model.AccountName = Account.GetAccountName(accountId);

            List<SelectListItem> TypeList = new List<SelectListItem>();
            TypeList.Add(new SelectListItem() { Text = "收入", Value = "1" });
            TypeList.Add(new SelectListItem() { Text = "支出", Value = "2" });
            ViewData["TypeList"] = TypeList;
            ViewData["referer"] = Url.Action("accountlist", new { uid = uid, isDSUser = userInfo.IsDirSaleUser, DSuid = userInfo.DirSaleUid });
            return View(model);
        }
        /// <summary>
        /// 账户修改
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AccountModify(AccountModifyModel model, int uid = -1, int accountId = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            List<AccountInfo> accountList = Account.GetAccountListByAid(string.Format(" userid={0} and accountid={1} ", uid, accountId));
            if (accountList.Count <= 0)
                return PromptView("该帐户类型不存在");

            if (ModelState.IsValid)
            {
                //更新

                decimal InAmount = 0;
                decimal OutAmount = 0;
                if (model.Type == 1)//收入
                {
                    InAmount = model.Amount;
                    Account.UpdateAccountForIn(new AccountInfo()
                    {
                        AccountId = accountId,
                        UserId = userInfo.Uid,
                        TotalIn = InAmount
                    });
                }
                else
                { //支出
                    OutAmount = model.Amount;
                    Account.UpdateAccountForOut(new AccountInfo()
                    {
                        AccountId = accountId,
                        UserId = userInfo.Uid,
                        TotalOut = OutAmount
                    });
                }
                Account.CreateAccountDetail(new AccountDetailInfo()
                {
                    AccountId = accountId,
                    UserId = userInfo.Uid,
                    CreateTime = DateTime.Now,
                    DetailType = 20,
                    InAmount = InAmount,
                    OutAmount = OutAmount,
                    OrderCode = model.OrderCode ?? "",
                    AdminUid = 2,//system
                    Status = 1,
                    DetailDes = model.DetailDes
                });

                AddMallAdminLog("帐户金额修改", "帐户修改,会员ID为:" + uid + "账户ID：" + accountId + ",金额：" + model.Amount);

                return PromptView(Url.Action("accountlist", new { uid = uid, isDSUser = userInfo.IsDirSaleUser, DSuid = userInfo.DirSaleUid }), "帐户修改成功");
            }

            return View(model);
        }

        #endregion

        #region 推荐关系
        /// <summary>
        /// 推荐网络（全部）
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult RecommendNet(int uid)
        {
            UserInfo user = Users.GetUserById(uid);
            UserParentNet userNet = new UserParentNet();
            userNet.Uid = user.Uid;
            userNet.UserName = string.IsNullOrEmpty(user.UserName) ? (string.IsNullOrEmpty(user.Email) ? (string.IsNullOrEmpty(user.Mobile) ? "" : user.Mobile) : user.Email) : user.UserName;
            userNet.UserMobile = user.Mobile;
            userNet.NickName = user.NickName;
            userNet.RealName = user.RealName;
            userNet.UserRank = "";
            userNet.AgentRank = Enum.GetName(typeof(AgentTypeEnum), user.AgentType); ;
            userNet.AgentSource = Enum.GetName(typeof(MallSourceType), user.MallSource);
            userNet.RegisterTime = user.RegisterTime.ToString("yyyy-MM-dd HH:mm");
            userNet.ChildrenCount = Users.GetUserNetCount(user);
            userNet.pid = user.Pid;
            userNet.pType = user.Ptype;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(userNet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public ActionResult GetChildren(int ParentId)
        {
            return this.Json(Users.GetChildren(ParentId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult RecommendNetChart(int uid, int LevelCount = 3)
        {
            UserInfo parent = Users.GetUserById(uid);
            PartUserInfo partUser = Users.GetPartUserById(uid);
            List<UserInfo> children = Users.GetSubRecommendListByPid(partUser, 1, 100);
            children.Add(parent);//将父级加入到list中 
            ViewData["treeStr"] = GetTreeOrgChart(children, LevelCount, 1);//数据拼接成组织层级

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            NetChartModel model = new NetChartModel();
            model.Uid = uid;
            model.user = Users.GetPartUserById(uid);
            model.LevelCount = LevelCount;
            return View(model);
        }

        #region 会员列表辅助方法

        //参数为整个表树形集合
        public string GetTreeOrgChart(List<UserInfo> list, int LevelCount, int NetType)
        {
            var strHtml_OrgChart = new StringBuilder { };
            List<UserInfo> itemNode = list.FindAll(t => t.ParentLevel == 0);
            foreach (UserInfo entity in itemNode)
            {
                //string itemid = "uid" + entity.Uid;
                //strHtml_OrgChart.Append("var " + itemid + "={};");
                //strHtml_OrgChart.Append(" " + itemid + ".id='" + entity.Uid + "'; " + "" + itemid + ".name='" + entity.NickName + "'; " + itemid + ".data={}; " + itemid + ".children=[];");

                ////创建子节点
                //strHtml_OrgChart.Append(GetTreeNodeOrgChart(entity.Uid, list));

                //string userName = string.IsNullOrEmpty(entity.UserName) ? entity.Mobile : entity.UserName;
                string UserRank = "";
                string agentTitle = string.Empty;
                if (NetType == 1)
                {
                    agentTitle = Enum.GetName(typeof(AgentTypeEnum), entity.AgentType);
                }
                else if (NetType == 2)
                {
                    agentTitle = Enum.GetName(typeof(AgentTypeEnum), entity.AgentType);
                }
                string itemid = "uid" + entity.Uid;
                strHtml_OrgChart.Append("var " + itemid + " = new OrgNode();");
                strHtml_OrgChart.Append("" + itemid + ".Text = \"" + entity.UserName + "<br/>" + entity.Mobile + "<br/>" + entity.RealName + "<br/>" + agentTitle + "\";");
                strHtml_OrgChart.Append("" + itemid + ".Description = \"" + entity.NickName + "\";");
                //strHtml_OrgChart.Append("" + itemid + ".Link = \"#\";");
                //创建子节点
                strHtml_OrgChart.Append(GetTreeNodeOrgChart(entity.Uid, list, entity.Ptype, entity.DirSaleUid, LevelCount, NetType));
            }

            return strHtml_OrgChart.ToString();
        }

        /// <summary>
        /// 创建子节点
        /// </summary>
        /// <param name="parentID">父节点主键</param>
        /// <param name="list">菜单集合</param>
        /// <returns></returns>
        public string GetTreeNodeOrgChart(int ParentId, List<UserInfo> list, int Ptype, int DsParentId, int LevelCount, int NetType)
        {
            StringBuilder sb_TreeNode = new StringBuilder();
            List<UserInfo> itemNode = new List<UserInfo>();
            LevelCount--;
            if (NetType == 1)
            {
                if (Ptype == 1)
                    itemNode = list.FindAll(t => t.Pid == ParentId && t.Ptype == 1 && t.ParentLevel == 1);
                if (Ptype == 2)
                    itemNode = list.FindAll(t => t.Pid == DsParentId && t.Ptype == 2 && t.ParentLevel == 1);
            }
            if (NetType == 2)
            {
                //if (Ptype == 1)
                itemNode = list.FindAll(t => t.AgentPid == ParentId && t.ParentLevel == 1 && (t.IsDirSaleUser || t.AgentType > 0));
                //if (Ptype == 2)
                //    itemNode = list.FindAll(t => t.AgentPid == DsParentId && t.Ptype == 2 && t.ParentLevel == 1 && (t.IsDirSaleUser || t.AgentType > 0));
            }
            if (itemNode.Count > 0)
            {
                if (LevelCount + 1 > 0)
                {
                    foreach (UserInfo entity in itemNode)
                    {
                        //string itemid = "uid" + entity.Uid;
                        //string itemParentId = "uid" + ParentId;

                        //sb_TreeNode.Append("var " + itemid + "={};");
                        //sb_TreeNode.Append(" " + itemid + ".id='" + entity.Uid + "'; " + "" + itemid + ".name='" + entity.NickName + "'; " + itemid + ".data={}; " + itemid + ".children=[];");
                        //sb_TreeNode.Append(GetTreeNodeOrgChart(entity.Uid, list));
                        //sb_TreeNode.Append("" + itemParentId + ".children.push(" + itemid + ") ;");

                        //string userName = string.IsNullOrEmpty(entity.UserName) ? entity.Mobile : entity.UserName;
                        string UserRank = "";
                        string agentTitle = string.Empty;
                        if (NetType == 1)
                        {
                            agentTitle = Enum.GetName(typeof(AgentTypeEnum), entity.AgentType);

                        }
                        else if (NetType == 2)
                        {
                            agentTitle = Enum.GetName(typeof(AgentTypeEnum), entity.AgentType);
                        }
                        string itemid = "uid" + entity.Uid;
                        string itemParentId = "uid" + ParentId;
                        sb_TreeNode.Append("var " + itemid + " = new OrgNode();");
                        sb_TreeNode.Append("" + itemid + ".Text = \"" + entity.UserName + "<br/>" + entity.Mobile + "</span><br/>" + entity.RealName + "<br/>" + agentTitle + "\";");
                        if (NetType == 1)
                            sb_TreeNode.Append("" + itemid + ".Link = \"/malladmin/user/RecommendNetChart?uid=" + entity.Uid + "\";");
                        else if (NetType == 2)
                            sb_TreeNode.Append("" + itemid + ".Link = \"/malladmin/user/AgentNetChart?uid=" + entity.Uid + "\";");
                        sb_TreeNode.Append("" + itemid + ".Description = \"" + entity.NickName + "\";");
                        //sb_TreeNode.Append("" + itemid + ".Link = \"#\";");
                        sb_TreeNode.Append("" + itemParentId + ".Nodes.Add(" + itemid + ");");
                        //创建子节点
                        List<UserInfo> children = new List<UserInfo>();
                        if (NetType == 1)
                            children = Users.GetSubRecommendListByPid(entity, 1, 2000);
                        if (NetType == 2)
                            children = Users.GetAgentRecommendNetByPid(entity);
                        sb_TreeNode.Append(GetTreeNodeOrgChart(entity.Uid, children, entity.Ptype, entity.DirSaleUid, LevelCount, NetType));
                    }
                }

            }

            return sb_TreeNode.ToString();
        }

        #endregion

        #endregion

        #region 用户添加编辑
        /// <summary>
        /// 编辑用户身份
        /// </summary>
        [HttpGet]
        public ActionResult EditRank(int uid = -1)
        {

            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            UserRank2Model model = new UserRank2Model();
            model.UserName = userInfo.UserName;
            model.Mobile = userInfo.Mobile;
            model.UserRid = userInfo.UserRid;
            model.NickName = userInfo.NickName;
            model.BusiCentType = userInfo.BusiCentType;
            List<SelectListItem> userRankList = new List<SelectListItem>();
            userRankList.Add(new SelectListItem() { Text = "选择会员等级", Value = "0" });
            userRankList.Add(new SelectListItem() { Text = "公司员工", Value = "10" });
            userRankList.Add(new SelectListItem() { Text = "非公司员工", Value = "7" });
            ViewData["userRankList"] = userRankList;

            List<SelectListItem> BusiCentTypeList = new List<SelectListItem>();
            BusiCentTypeList.Add(new SelectListItem() { Text = "无", Value = "0" });
            BusiCentTypeList.Add(new SelectListItem() { Text = "商城商务中心（1980代报单）", Value = "1" });
            ViewData["BusiCentTypeList"] = BusiCentTypeList;

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 编辑用户身份
        /// </summary>
        [HttpPost]
        public ActionResult EditRank(UserRank2Model model, int uid = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");

            if (ModelState.IsValid)
            {
                userInfo.UserRid = model.UserRid;
                userInfo.BusiCentType = model.BusiCentType;
                AdminUsers.UpdateUserRank(userInfo.Uid, userInfo.UserRid, userInfo.BusiCentType);
                AddMallAdminLog("修改用户身份", "修改用户,用户ID为:" + uid);
                return PromptView("用户身份修改成功");
            }

            List<SelectListItem> userRankList = new List<SelectListItem>();
            userRankList.Add(new SelectListItem() { Text = "选择会员等级", Value = "0" });
            userRankList.Add(new SelectListItem() { Text = "公司员工", Value = "10" });
            userRankList.Add(new SelectListItem() { Text = "非公司员工", Value = "7" });
            ViewData["userRankList"] = userRankList;
            List<SelectListItem> BusiCentTypeList = new List<SelectListItem>();
            BusiCentTypeList.Add(new SelectListItem() { Text = "无", Value = "0" });
            BusiCentTypeList.Add(new SelectListItem() { Text = "商城商务中心（1980代报单）", Value = "1" });
            ViewData["BusiCentTypeList"] = BusiCentTypeList;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        [HttpGet]
        public ActionResult Add()
        {
            UserModel model = new UserModel();
            Load(model.RegionId);
            return View(model);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        [HttpPost]
        public ActionResult Add(UserModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError("Password", "密码不能为空");

            if (AdminUsers.IsExistUserName(model.UserName))
                ModelState.AddModelError("UserName", "名称已经存在");

            if (AdminUsers.IsExistEmail(model.Email))
                ModelState.AddModelError("Email", "email已经存在");

            if (AdminUsers.IsExistMobile(model.Mobile))
                ModelState.AddModelError("Mobile", "手机号已经存在");

            if (ModelState.IsValid)
            {
                string salt = Users.GenerateUserSalt();
                string nickName;
                if (string.IsNullOrWhiteSpace(model.NickName))
                    nickName = "hg" + Randoms.CreateRandomValue(7);
                else
                    nickName = model.NickName;

                UserInfo userInfo = new UserInfo()
                {
                    UserName = model.UserName == null ? "" : model.UserName.Trim(),
                    Email = model.Email == null ? "" : model.Email.Trim(),
                    Mobile = model.Mobile == null ? "" : model.Mobile.Trim(),
                    Salt = salt,
                    Password = Users.CreateUserPassword(model.Password, salt),
                    DirSalePwd = SecureHelper.EncryptString(model.Password, DirSaleUserInfo.EncryptKey),
                    UserRid = model.UserRid,
                    StoreId = 0,
                    MallAGid = model.MallAGid,
                    NickName = WebHelper.HtmlEncode(nickName),
                    Avatar = model.Avatar == null ? "" : WebHelper.HtmlEncode(model.Avatar),
                    PayCredits = model.PayCredits,
                    RankCredits = AdminUserRanks.GetUserRankById(model.UserRid).CreditsLower,
                    VerifyEmail = 1,
                    VerifyMobile = 1,
                    LiftBanTime = UserRanks.IsBanUserRank(model.UserRid) ? DateTime.Now.AddDays(WorkContext.UserRankInfo.LimitDays) : new DateTime(1900, 1, 1),
                    Ptype = 1,
                    Pid = model.Pid,

                    LastVisitTime = DateTime.Now,
                    LastVisitIP = WorkContext.IP,
                    LastVisitRgId = WorkContext.RegionId,
                    RegisterTime = DateTime.Now,
                    RegisterIP = WorkContext.IP,
                    RegisterRgId = WorkContext.RegionId,
                    Gender = model.Gender,
                    RealName = model.RealName == null ? "" : WebHelper.HtmlEncode(model.RealName),
                    Bday = model.Bday ?? new DateTime(1970, 1, 1),
                    IdCard = model.IdCard == null ? "" : model.IdCard,
                    RegionId = model.RegionId,
                    Address = model.Address == null ? "" : WebHelper.HtmlEncode(model.Address),
                    Bio = model.Bio == null ? "" : WebHelper.HtmlEncode(model.Bio)
                };

                AdminUsers.CreateUser(userInfo);
                AddMallAdminLog("添加用户", "添加用户,用户为:" + model.UserName);
                return PromptView("用户添加成功");
            }
            Load(model.RegionId);

            return View(model);
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        [HttpGet]
        public ActionResult Edit(int uid = -1)
        {

            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            if (userInfo.IsDirSaleUser)
                return PromptView("直销会员不支持编辑操作");

            UserModel model = new UserModel();
            model.UserName = userInfo.UserName;
            model.Email = userInfo.Email;
            model.Mobile = userInfo.Mobile;
            model.UserRid = userInfo.UserRid;
            model.MallAGid = userInfo.MallAGid;
            model.NickName = userInfo.NickName;
            model.Avatar = userInfo.Avatar;
            model.Pid = userInfo.Pid;
            model.PayCredits = userInfo.PayCredits;
            model.Gender = userInfo.Gender;
            model.RealName = userInfo.RealName;
            model.Bday = userInfo.Bday;
            model.IdCard = userInfo.IdCard;
            model.RegionId = userInfo.RegionId;
            model.Address = userInfo.Address;
            model.Bio = userInfo.Bio;
            model.BankName = userInfo.BankName;
            model.BankCardCode = userInfo.BankCardCode;
            model.BankUserName = userInfo.BankUserName;
            Load(model.RegionId);
            string[] configBanklist = StringHelper.SplitString(BMAConfig.MallConfig.BankList);
            List<SelectListItem> BankList = new List<SelectListItem>();
            BankList.Add(new SelectListItem() { Text = "选择银行", Value = "" });
            foreach (var info in configBanklist)
            {
                BankList.Add(new SelectListItem() { Text = info, Value = info });
            }
            ViewData["BankList"] = BankList;

            return View(model);
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        [HttpPost]
        public ActionResult Edit(UserModel model, int uid = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");

            int uid2 = AdminUsers.GetUidByUserName(model.UserName);
            if (uid2 > 0 && uid2 != uid)
                ModelState.AddModelError("UserName", "用户名已经存在");

            int uid3 = AdminUsers.GetUidByEmail(model.Email);
            if (uid3 > 0 && uid3 != uid)
                ModelState.AddModelError("Email", "邮箱已经存在");

            int uid4 = AdminUsers.GetUidByMobile(model.Mobile);
            if (uid4 > 0 && uid4 != uid)
                ModelState.AddModelError("Mobile", "手机号已经存在");

            if (ModelState.IsValid)
            {
                string nickName;
                if (string.IsNullOrWhiteSpace(model.NickName))
                    nickName = userInfo.NickName;
                else
                    nickName = model.NickName;

                userInfo.UserName = model.UserName == null ? "" : model.UserName.Trim();
                userInfo.Email = model.Email == null ? "" : model.Email.Trim();
                userInfo.Mobile = model.Mobile == null ? "" : model.Mobile.Trim();
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    userInfo.Password = Users.CreateUserPassword(model.Password, userInfo.Salt);
                    userInfo.DirSalePwd = SecureHelper.EncryptString(model.Password, DirSaleUserInfo.EncryptKey);
                }
                userInfo.UserRid = model.UserRid;
                userInfo.MallAGid = model.MallAGid;
                userInfo.NickName = WebHelper.HtmlEncode(nickName);
                userInfo.Avatar = model.Avatar == null ? "" : WebHelper.HtmlEncode(model.Avatar);
                userInfo.PayCredits = model.PayCredits;
                userInfo.RankCredits = userInfo.UserRid == model.UserRid ? userInfo.RankCredits : AdminUserRanks.GetUserRankById(model.UserRid).CreditsLower;
                userInfo.LiftBanTime = UserRanks.IsBanUserRank(model.UserRid) ? DateTime.Now.AddDays(WorkContext.UserRankInfo.LimitDays) : new DateTime(1900, 1, 1);
                userInfo.Gender = model.Gender;
                userInfo.RealName = model.RealName == null ? "" : WebHelper.HtmlEncode(model.RealName);
                userInfo.Bday = model.Bday ?? new DateTime(1970, 1, 1);
                userInfo.IdCard = model.IdCard == null ? "" : model.IdCard;
                userInfo.RegionId = model.RegionId;
                userInfo.Address = model.Address == null ? "" : WebHelper.HtmlEncode(model.Address);
                userInfo.Bio = model.Bio == null ? "" : WebHelper.HtmlEncode(model.Bio);
                userInfo.BankName = model.BankName == null ? "" : WebHelper.HtmlEncode(model.BankName);
                userInfo.BankCardCode = model.BankCardCode == null ? "" : WebHelper.HtmlEncode(model.BankCardCode);
                userInfo.BankUserName = model.BankUserName == null ? "" : WebHelper.HtmlEncode(model.BankUserName);

                AdminUsers.UpdateUser(userInfo);
                AddMallAdminLog("修改用户", "修改用户,用户ID为:" + uid);
                return PromptView("用户修改成功");
            }
            string[] configBanklist = StringHelper.SplitString(BMAConfig.MallConfig.BankList);
            List<SelectListItem> BankList = new List<SelectListItem>();
            BankList.Add(new SelectListItem() { Text = "选择银行", Value = "" });
            foreach (var info in configBanklist)
            {
                BankList.Add(new SelectListItem() { Text = info, Value = info });
            }
            ViewData["BankList"] = BankList;
            Load(model.RegionId);

            return View(model);
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        [HttpGet]
        public ActionResult EditDetail(int uid = -1)
        {

            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            //int uid, string userName, string nickName, string avatar, int gender, string realName, DateTime bday, string idCard, int regionId, string address, string bio, string bankName, string bankCardCode, string bankUserName
            EditUserDetailModel model = new EditUserDetailModel();
            model.UserName = userInfo.UserName;
            model.Email = userInfo.Email;
            model.Mobile = userInfo.Mobile;
            model.NickName = userInfo.NickName;
            model.Gender = userInfo.Gender;
            model.RealName = userInfo.RealName;
            model.Bday = userInfo.Bday;
            model.IdCard = userInfo.IdCard;
            model.RegionId = userInfo.RegionId;
            model.Address = userInfo.Address;
            model.Bio = userInfo.Bio;
            model.BankName = userInfo.BankName;
            model.BankCardCode = userInfo.BankCardCode;
            model.BankUserName = userInfo.BankUserName;
            Load(model.RegionId);
            string[] configBanklist = StringHelper.SplitString(BMAConfig.MallConfig.BankList);
            List<SelectListItem> BankList = new List<SelectListItem>();
            BankList.Add(new SelectListItem() { Text = "选择银行", Value = "" });
            foreach (var info in configBanklist)
            {
                BankList.Add(new SelectListItem() { Text = info, Value = info });
            }
            ViewData["BankList"] = BankList;

            return View(model);
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        [HttpPost]
        public ActionResult EditDetail(EditUserDetailModel model, int uid = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");

            if (ModelState.IsValid)
            {
                //int uid, string userName, string nickName, string avatar, int gender, string realName, DateTime bday, string idCard, int regionId, string address, string bio, string bankName, string bankCardCode, string bankUserName

                userInfo.Gender = model.Gender;
                userInfo.RealName = model.RealName == null ? "" : WebHelper.HtmlEncode(model.RealName);
                userInfo.Bday = model.Bday ?? new DateTime(1970, 1, 1);
                userInfo.IdCard = model.IdCard == null ? "" : model.IdCard;
                userInfo.RegionId = model.RegionId;
                userInfo.Address = model.Address == null ? "" : WebHelper.HtmlEncode(model.Address);
                userInfo.Bio = model.Bio == null ? "" : WebHelper.HtmlEncode(model.Bio);
                userInfo.BankName = model.BankName == null ? "" : WebHelper.HtmlEncode(model.BankName);
                userInfo.BankCardCode = model.BankCardCode == null ? "" : WebHelper.HtmlEncode(model.BankCardCode);
                userInfo.BankUserName = model.BankUserName == null ? "" : WebHelper.HtmlEncode(model.BankUserName);

                Users.UpdateUser(userInfo.Uid, userInfo.UserName, userInfo.NickName, userInfo.Avatar, userInfo.Gender, userInfo.RealName, userInfo.Bday, userInfo.IdCard, userInfo.RegionId, userInfo.Address, userInfo.Bio, userInfo.BankName, userInfo.BankCardCode, userInfo.BankUserName);

                AddMallAdminLog("修改用户基本资料", "修改用户基本资料,用户ID为:" + uid);
                return PromptView("用户基本资料修改成功");
            }

            Load(model.RegionId);

            return View(model);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        public ViewResult Del(int[] uidList)
        {
            AdminUsers.DeleteUserById(uidList);
            AddMallAdminLog("删除用户", "删除用户,用户ID为:" + CommonHelper.IntArrayToString(uidList));
            return PromptView("用户删除成功");
        }
        #endregion

        #region 汇购卡
        /// <summary>
        /// 编辑用户最大汇购卡数
        /// </summary>
        [HttpGet]
        public ActionResult SetMaxCash(int uid = -1)
        {

            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            if (!userInfo.IsDirSaleUser)
                return PromptView("非直销会员不支持该操作");
            UserCachModel model = new UserCachModel();
            //model.UserName = userInfo.UserName;
            //model.Email = userInfo.Email;
            //model.Mobile = userInfo.Mobile;
            model.MaxCashCount = userInfo.MaxCashCount;
            Load(0);
            return View(model);
        }

        /// <summary>
        ///编辑用户最大汇购卡数
        /// </summary>
        [HttpPost]
        public ActionResult SetMaxCash(UserCachModel model, int uid = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            if (!userInfo.IsDirSaleUser)
                return PromptView("非直销会员不支持该操作");

            if (ModelState.IsValid)
            {
                userInfo.MaxCashCount = model.MaxCashCount;
                AdminUsers.UpdateMaxCashCount(userInfo);
                AddMallAdminLog("修改用户最大汇购卡数", "修改用户最大汇购卡数,用户ID为:" + uid);
                return PromptView("修改成功");
            }

            Load(0);

            return View(model);
        }

        #endregion

        #region 微商代理

        #region

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStock(int uid)
        {
            AgentStockModel model = new AgentStockModel();
            model.AgentStockList = new AgentStock().GetAgentStockList(string.Format(" uid={0} ", uid), "", 1, 20);

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }
        /// <summary>
        /// 库存详情
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStockDetail(int uid, int pid, int pageSize = 15, int pageNumber = 1)
        {
            AgentStockDetailModel model = new AgentStockDetailModel();
            string condition = string.Format(" uid={0} and pid={1} ", uid, pid);
            PageModel pageModel = new PageModel(pageSize, pageNumber, new AgentStockDetail().GetRecordCount(condition));
            List<AgentStockDetailInfo> list = new AgentStockDetail().GetListByPage(condition, "creationdate desc", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize);
            model.DetailList = list;
            model.PageModel = pageModel;
            model.Uid = uid;
            model.Pid = pid;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 库存修改
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult StockModify(int uid = -1, int pid = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            AgentStockInfo info = new AgentStock().GetModel(string.Format(" uid={0} and pid={1} ", uid, pid));
            if (info == null)
                return PromptView("库存信息不存在");
            PartProductInfo product = AdminProducts.AdminGetProductById(pid);

            StockModifyModel model = new StockModifyModel();
            model.Pid = pid;
            model.Uid = uid;
            model.ProductName = product.Name;
            model.info = info;
            model.UserName = userInfo.UserName + "/" + userInfo.Mobile;
            List<SelectListItem> TypeList = new List<SelectListItem>();

            TypeList.Add(new SelectListItem() { Text = "收入", Value = "1" });
            TypeList.Add(new SelectListItem() { Text = "支出", Value = "2" });

            ViewData["TypeList"] = TypeList;
            ViewData["referer"] = Url.Action("GetStock", new { uid = uid });
            return View(model);
        }
        /// <summary>
        ///  库存修改
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StockModify(StockModifyModel model, int uid = -1, int pid = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            AgentStockInfo info = new AgentStock().GetModel(string.Format(" uid={0} and pid={1} ", uid, pid));
            if (info == null)
                return PromptView("库存信息不存在");
            PartProductInfo product = AdminProducts.AdminGetProductById(pid);

            if (ModelState.IsValid)
            {
                //更新库存
                decimal currentAmount = 0;
                decimal InAmount = 0;
                decimal OutAmount = 0;

                if (model.Type == 1)//收入
                {
                    InAmount = model.Amount;
                    info.AgentAmount = info.AgentAmount + InAmount;
                }
                else
                { //支出
                    OutAmount = model.Amount;
                    info.AgentAmount = info.AgentAmount - OutAmount;
                }
                decimal itemPrice = AgentStockBLL.SingleAgentPrice(userInfo, pid);
                info.Balance = AgentStock.CutDecimalWithN(info.AgentAmount / itemPrice, 4);
                AgentStockBLL.Update(info);
                currentAmount = info.AgentAmount;
                string orderCode = model.OrderCode ?? "";
                AgentStockDetailBLL.AddDetail(uid, pid, model.Type, InAmount, OutAmount, currentAmount, orderCode, model.DetailDes, 0, uid);

                AddMallAdminLog("库存更改", "库存更改,会员ID为:" + uid + ",金额：" + model.Amount + ",类型：" + model.Type);

                return PromptView(Url.Action("GetStock", new { uid = uid }), "库存更改成功");
            }

            return View(model);
        }

        /// <summary>
        /// 微商网络
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult AgentNet(int uid)
        {
            UserInfo user = Users.GetUserById(uid);
            UserParentNet userNet = new UserParentNet();
            userNet.Uid = user.Uid;
            userNet.UserName = string.IsNullOrEmpty(user.UserName) ? (string.IsNullOrEmpty(user.Email) ? (string.IsNullOrEmpty(user.Mobile) ? "" : user.Mobile) : user.Email) : user.UserName;
            userNet.UserMobile = user.Mobile;
            userNet.NickName = user.NickName;
            userNet.RealName = user.RealName;
            userNet.UserRank = user.IsDirSaleUser ? "直销会员" : (user.IsFXUser > 0 ? (user.IsFXUser == 1 ? "分销会员" : "高级分销会员") : "普通会员");
            string agentTitle = string.Empty;
            if (user.AgentType == 5)
                agentTitle = "汇购优选股东";
            if (user.AgentType == 4)
                agentTitle = "大区(战略合伙人)";
            if (user.AgentType == 3)
                agentTitle = "董事(VIP、H3会员)";
            if (user.AgentType == 2)
                agentTitle = "合伙人(星级、H2会员)";
            if (user.AgentType == 1)
                agentTitle = "代言人(事业伙伴、H1会员)";
            if (user.AgentType == 0)
                agentTitle = "无";
            userNet.AgentRank = agentTitle;
            string AgentSource = string.Empty;
            if (user.AgentType > 0)
            {
                AgentSource = "自营商城";
            }
            userNet.IsDirSaleUser = user.IsDirSaleUser;
            userNet.AgentType = user.AgentType;
            userNet.AgentSource = AgentSource;
            userNet.RegisterTime = user.IsActive == 1 ? user.ActiveTime.ToString("yyyy-MM-dd HH:mm") : "未激活";

            userNet.ChildrenCount = Users.GetUserCountForAgentSystem(user);
            userNet.pid = user.Pid;
            userNet.pType = user.Ptype;
            userNet.AgentPid = user.AgentPid;
            userNet.AgentPType = user.AgentPType;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(userNet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public ActionResult GetChildrenAgent(int ParentId)
        {
            return this.Json(Users.GetChildrenAgent(ParentId).FindAll(x => x.IsDirSaleUser || x.AgentType > 0), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 代理网络 图形
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult AgentNetChart(int uid, int LevelCount = 3)
        {
            UserInfo parent = Users.GetUserById(uid);
            PartUserInfo partUser = Users.GetPartUserById(uid);
            List<UserInfo> children = Users.GetAgentRecommendNetByPid(partUser);
            children.Add(parent);//将父级加入到list中 
            ViewData["treeStr"] = GetTreeOrgChart(children, LevelCount, 2);//数据拼接成组织层级

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            NetChartModel model = new NetChartModel();
            model.Uid = uid;
            model.user = Users.GetPartUserById(uid);
            model.LevelCount = LevelCount;
            return View(model);
        }
        #endregion

        /// <summary>
        /// 修改代理等级、代理折扣资格
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult RankModify(int uid = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");

            RankModifyModel model = new RankModifyModel();
            model.UserId = uid;
            model.UserName = userInfo.UserName;
            model.Moblie = userInfo.Mobile;
            model.RealName = userInfo.RealName;
            model.OldAgentType = userInfo.AgentType;
            //model.OldDS2AgentRank = userInfo.Ds2AgentRank;

            List<SelectListItem> AgentTypeList = new List<SelectListItem>();
            AgentTypeList.Add(new SelectListItem() { Text = "无", Value = "-1" });
            foreach (AgentTypeEnum source in Enum.GetValues(typeof(AgentTypeEnum)))
            {
                AgentTypeList.Add(new SelectListItem() { Text = source.ToString(), Value = ((int)source).ToString() });
            }

            ViewData["AgentTypeList"] = AgentTypeList;

            ViewData["referer"] = Url.Action("List", new { userName = userInfo.UserName, mobile = userInfo.Mobile });
            return View(model);
        }
        /// <summary>
        /// 修改代理等级、代理折扣资格
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RankModify(RankModifyModel model, int uid = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            if (ModelState.IsValid)
            {
                Users.UpdateUserAGTypeOrDs2AgentRank(uid, model.NewAgentType, model.NewDS2AgentRank);
                string oldAgentName = string.Empty;
                string newAgentName = string.Empty;
                oldAgentName = Enum.GetName(typeof(AgentTypeEnum), model.OldAgentType);
                newAgentName = Enum.GetName(typeof(AgentTypeEnum), model.NewAgentType);

                AddMallAdminLog("会员代理等级修改", "代理修改,会员ID:" + uid + "账号：" + model.UserName + "/" + model.Moblie + ",代理等级：【" + oldAgentName + "】修改为【" + newAgentName + "】");

                return PromptView(Url.Action("List", new { userName = userInfo.UserName, mobile = userInfo.Mobile }), "修改代理成功");
            }
            List<SelectListItem> AgentTypeList = new List<SelectListItem>();
            AgentTypeList.Add(new SelectListItem() { Text = "全部类型", Value = "-1" });
            foreach (AgentTypeEnum source in Enum.GetValues(typeof(AgentTypeEnum)))
            {
                AgentTypeList.Add(new SelectListItem() { Text = source.ToString(), Value = ((int)source).ToString() });
            }
            ViewData["AgentTypeList"] = AgentTypeList;
            ViewData["referer"] = Url.Action("List", new { userName = userInfo.UserName, mobile = userInfo.Mobile });
            return View(model);
        }

        /// <summary>
        /// 修改代理推荐关系
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult AgentNetModify(int uid = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");

            AgentNetModifyModel model = new AgentNetModifyModel();
            model.UserId = uid;
            model.UserName = userInfo.UserName;
            model.Moblie = userInfo.Mobile;
            model.RealName = userInfo.RealName;
            model.OldParentId = userInfo.Pid;
            model.OldParentType = userInfo.Ptype;
            PartUserInfo parentUser = Users.GetParentUserByPidAndPtype(userInfo.Pid, userInfo.Ptype);
            if (parentUser != null)
            {
                model.OldParentName = parentUser.Mobile;
                UserInfo user = Users.GetUserById(parentUser.Uid);
                model.OldRealName = user.RealName;
            }
            else
            {
                model.OldParentName = "";
                model.OldRealName = "";
            }

            ViewData["referer"] = Url.Action("List", new { userName = userInfo.UserName, mobile = userInfo.Mobile });
            return View(model);
        }
        /// <summary>
        /// 修改代理推荐关系
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AgentNetModify(AgentNetModifyModel model, int uid = -1)
        {
            UserInfo userInfo = AdminUsers.GetUserById(uid);
            if (userInfo == null)
                return PromptView("用户不存在");
            PartUserInfo newParentUser = new PartUserInfo();
            if (ValidateHelper.IsMobile(model.NewParentName))
                newParentUser = Users.GetPartUserByMobile(model.NewParentName);
            else
                newParentUser = Users.GetPartUserByName(model.NewParentName);
            if (newParentUser == null)
                return PromptView("推荐人不存在");
            if (ModelState.IsValid)
            {
                Users.UpdateUserAgentRecommer(uid, newParentUser.Uid, 1);

                AddMallAdminLog("代理推荐关系修改", "代理推荐关系修改,会员ID:" + uid + "账号：" + model.UserName + "/" + model.Moblie + ",推荐人：【" + model.OldParentName + "】修改为【" + model.NewParentName + "】");

                return PromptView(Url.Action("List", new { userName = userInfo.UserName, mobile = userInfo.Mobile }), "修改代理推荐关系成功");
            }

            ViewData["referer"] = Url.Action("List", new { userName = userInfo.UserName, mobile = userInfo.Mobile });
            return View(model);
        }

        #endregion


        public ActionResult GetUniqueUserCode()
        {
            string UserCode = Users.GetUniqueUserCode();

            return Content(UserCode);
        }

        private void Load(int regionId)
        {
            List<SelectListItem> userRankList = new List<SelectListItem>();
            userRankList.Add(new SelectListItem() { Text = "选择会员等级", Value = "0" });
            foreach (UserRankInfo info in AdminUserRanks.GetUserRankList())
            {
                userRankList.Add(new SelectListItem() { Text = info.Title, Value = info.UserRid.ToString() });
            }
            ViewData["userRankList"] = userRankList;


            List<SelectListItem> mallAdminGroupList = new List<SelectListItem>();
            mallAdminGroupList.Add(new SelectListItem() { Text = "选择管理员组", Value = "0" });
            foreach (MallAdminGroupInfo info in MallAdminGroups.GetMallAdminGroupList())
            {
                mallAdminGroupList.Add(new SelectListItem() { Text = info.Title, Value = info.MallAGid.ToString() });
            }
            ViewData["mallAdminGroupList"] = mallAdminGroupList;

            RegionInfo regionInfo = Regions.GetRegionById(regionId);
            if (regionInfo != null)
            {
                ViewData["provinceId"] = regionInfo.ProvinceId;
                ViewData["cityId"] = regionInfo.CityId;
                ViewData["countyId"] = regionInfo.RegionId;
            }
            else
            {
                ViewData["provinceId"] = -1;
                ViewData["cityId"] = -1;
                ViewData["countyId"] = -1;
            }

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
        }
    }
}
