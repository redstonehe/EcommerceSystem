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
    /// 商城后台提现控制器类
    /// </summary>
    public partial class HaimiDrawCashController : BaseMallAdminController
    {
        HaiMiDrawCash haiMiDrawCashBLL = new HaiMiDrawCash();
        public string AdminGetListCondition(string csn, string username, int type, int accountid, string realName)
        {
            StringBuilder condition = new StringBuilder();

            if (type >= 0)
                condition.AppendFormat(" AND T.[State] = {0} ", type);

            if (!string.IsNullOrWhiteSpace(csn))
                condition.AppendFormat(" AND T.[DrawCashSN] ='{0}' ", csn.Trim());
            if (!string.IsNullOrEmpty(username))
                condition.AppendFormat(" AND (b.[username] like '%{0}%' or b.[email] like '%{0}%' or b.[mobile] like '%{0}%') ", username.Trim());
            if (accountid == -1)
                condition.AppendFormat(" AND T.[AccountId] in ({0}) ", (int)AccountType.海米账户 + "," + (int)AccountType.代理账户);
            else
                condition.AppendFormat(" AND T.[AccountId] = {0} ", accountid);

            if (!string.IsNullOrWhiteSpace(realName))
                condition.AppendFormat(" AND (ud.[realname] like '%{0}%') ", realName.Trim());
            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }
        /// <summary>
        /// 提现申请列表
        /// </summary>
        /// <param name="csn">卡号</param>
        /// <param name="userName">会员标识</param>
        /// <param name="type">类型1代表提交申请等待处理，2代表处理成功，0代表处理失败，-1代表全部</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult List(string csn, int accountid = -1, string userName = "", string realName = "", int type = -1, int pageNumber = 1, int pageSize = 15)
        {
            string condition = AdminGetListCondition(csn, userName, type, accountid, realName);

            PageModel pageModel = new PageModel(pageSize, pageNumber, haiMiDrawCashBLL.AdminGetRecordCount(condition));

            HaiMiDrawCashListModel model = new HaiMiDrawCashListModel()
            {
                DrawCashList = haiMiDrawCashBLL.AdminGetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                CSN = csn,
                UserName = userName,
                Type = type,
                AccountId = accountid,
                RealName = realName
            };
            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemList.Add(new SelectListItem() { Text = "等待处理", Value = "1" });
            itemList.Add(new SelectListItem() { Text = "处理成功", Value = "2" });
            itemList.Add(new SelectListItem() { Text = "处理失败", Value = "0" });
            ViewData["typeList"] = itemList;
            List<SelectListItem> accountitemList = new List<SelectListItem>();
            accountitemList.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            accountitemList.Add(new SelectListItem() { Text = "代理账户", Value = ((int)AccountType.代理账户).ToString() });
            accountitemList.Add(new SelectListItem() { Text = "海米账户", Value = ((int)AccountType.海米账户).ToString() });
            ViewData["accountitemList"] = accountitemList;

            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&csn={3}&userName={4}&type={5}&accountid={6}&realName={7}",
                                                          Url.Action("list"),
                                                          pageModel.PageNumber, pageModel.PageSize,
                                                          csn, userName, type, accountid, realName));
            return View(model);
        }
        /// <summary>
        /// 佣金查询
        /// </summary>
        /// <returns></returns>
        public ActionResult IncomeAccount(string userName, string realName, int type = 1, int pageNumber = 1, int pageSize = 15)
        {
            StringBuilder condition = new StringBuilder();
            int accountId = (int)AccountType.佣金账户;
            condition.AppendFormat(string.Format(" T.AccountId={0}", accountId));
            if (!string.IsNullOrWhiteSpace(userName))
                condition.AppendFormat(" and  (c.[username] like '%{0}%' OR c.[email] like '%{0}%' OR c.[mobile] like '%{0}%') ", userName);
            if (type == 1)
            {
                condition.AppendFormat(" and T.Banlance>0");
            }
            if (!string.IsNullOrWhiteSpace(realName))
                condition.AppendFormat(" AND (ud.[realname] like '%{0}%') ", realName);

            if (!string.IsNullOrEmpty(HttpContext.Request.Form["IsOutPut"]) && HttpContext.Request.Form["IsOutPut"].Trim() == "1")
            {
                //ViewData["IsOutPut"] = "0";
                return OutPutIncomeAccount(condition.ToString());
            }

            PageModel pageModel = new PageModel(pageSize, pageNumber, Account.GetAllAccountInfoListCount(condition.ToString()));

            IncomeAccountModel model = new IncomeAccountModel()
            {
                IncomeAccountList = Account.AdminGetAllAccountInfoListByAid(condition.ToString(), "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                UserName = userName,
                Type = type

            };
            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部", Value = "0" });
            itemList.Add(new SelectListItem() { Text = "有效（余额大于0）", Value = "1" });


            ViewData["typeList"] = itemList;
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&userName={3}&type={4}&realName={5}",
                                                          Url.Action("IncomeAccount"),
                                                          pageModel.PageNumber, pageModel.PageSize,
                                                           userName, type, realName));
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);

        }

        /// <summary>
        /// 佣金查询-导出报表
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="consignee"></param>
        /// <param name="orderState"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult OutPutIncomeAccount(string condition)
        {
            bool noCondition = string.IsNullOrWhiteSpace(condition);

            string commandText = string.Format(@"SELECT  b.AccountName 账户类型,c.username 用户名,c.mobile 手机,c.nickname 昵称,ud.realname 姓名,T.Banlance 余额,T.LockBanlance 冻结余额 from hlh_account T left join hlh_accounttype b on T.AccountId=b.AccountId LEFT JOIN hlh_users c  on T.UserId=c.uid inner join hlh_userdetails ud on c.uid=ud.uid 
 WHERE 1=1 {0}  order by T.[Banlance] desc", noCondition ? "" : " AND " + condition);

            DataTable dt = AdminOrders.GetOrderByCondition(commandText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "佣金查询-" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 一键批量生成佣金提现申请
        /// </summary>
        /// <returns></returns>
        //public ActionResult BatchIncomeDraw()
        //{
        //    StringBuilder condition = new StringBuilder();
        //    int accountId = (int)AccountType.佣金账户;
        //    condition.AppendFormat(string.Format(" AccountId={0}", accountId));
        //    condition.AppendFormat(" and Banlance>0");
        //    List<AccountInfo> accountList = Account.GetAccountListByAid(condition.ToString());
        //    foreach (var item in accountList)
        //    {
        //        if (item.Banlance > 0)
        //        {
        //            UserDetailInfo user = Users.GetUserDetailById(item.UserId);
        //            if (user == null)
        //                continue;
        //            HaiMiDrawCashInfo info = new HaiMiDrawCashInfo();
        //            info.Uid = item.UserId;
        //            info.AccountId = accountId;
        //            info.DrawCashSN = "YJTX-" + DateTime.Now.ToString("yyyyMMddHHmmss") + Randoms.CreateRandomValue(5);
        //            info.Amount = item.Banlance;
        //            info.Poundage = item.Banlance * BMAConfig.MallConfig.YJFeeRate;
        //            //info.TaxAmount = TypeHelper.StringToDecimal(Amount) * BMAConfig.MallConfig.HMTaxRate;
        //            info.ActualAmount = info.Amount - info.Poundage - info.TaxAmount;
        //            info.State = 1;
        //            info.BankName = user.BankName;
        //            info.BankProvice = "";// regions.Find(x => x.Layer == 1) != null ? regions.Find(x => x.Layer == 1).Name : "";
        //            info.BankCity = "";// regions.Find(x => x.Layer == 2) != null ? regions.Find(x => x.Layer == 2).Name : "";
        //            info.BankAddress = "";//BankAddress;
        //            info.BankCardCode = user.BankCardCode;
        //            info.BankUserName = user.BankUserName;

        //            haiMiDrawCashBLL.Add(info);
        //            //更新佣金账户
        //            Account.UpdateAccountForOut(new AccountInfo()
        //            {
        //                AccountId = accountId,
        //                UserId = item.UserId,
        //                TotalOut = info.Amount
        //            });
        //            Account.CreateAccountDetail(new AccountDetailInfo()
        //            {
        //                AccountId = accountId,
        //                UserId = item.UserId,
        //                CreateTime = DateTime.Now,
        //                DetailType = (int)DetailType.提现支出,
        //                OutAmount = info.Amount,
        //                OrderCode = info.DrawCashSN,
        //                AdminUid = 0,//system
        //                Status = 1,
        //                DetailDes = string.Format("佣金提现：提现金额:{0}，综合税费:{1},实际提现金额:{2}", info.Amount.ToString("0.00"), info.Poundage.ToString("0.00"), info.ActualAmount.ToString("0.00"))
        //            });
        //        }
        //    }
        //    return Redirect("/malladmin/haimidrawcash/incomelist");
        //    //return RedirectToAction("incomelist");
        //}
        /// <summary>
        /// 佣金提现列表
        /// </summary>
        /// <param name="csn">卡号</param>
        /// <param name="userName">会员标识</param>
        /// <param name="type">类型1代表提交申请等待处理，2代表处理成功，0代表处理失败，-1代表全部</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult InComeList(string csn, int accountid = (int)AccountType.佣金账户, string userName = "", string realName = "", int type = -1, int pageNumber = 1, int pageSize = 15)
        {
            string condition = AdminGetListCondition(csn, userName, type, accountid, realName);

            PageModel pageModel = new PageModel(pageSize, pageNumber, haiMiDrawCashBLL.AdminGetRecordCount(condition));

            HaiMiDrawCashListModel model = new HaiMiDrawCashListModel()
            {
                DrawCashList = haiMiDrawCashBLL.AdminGetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                CSN = csn,
                UserName = userName,
                Type = type,
                AccountId = accountid
            };
            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemList.Add(new SelectListItem() { Text = "等待处理", Value = "1" });
            itemList.Add(new SelectListItem() { Text = "处理成功", Value = "2" });
            itemList.Add(new SelectListItem() { Text = "处理失败", Value = "0" });
            ViewData["typeList"] = itemList;
            List<SelectListItem> accountitemList = new List<SelectListItem>();

            accountitemList.Add(new SelectListItem() { Text = "佣金账户", Value = ((int)AccountType.佣金账户).ToString() });

            ViewData["accountitemList"] = accountitemList;

            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&csn={3}&userName={4}&type={5}&accountid={6}&realName={7}",
                                                          Url.Action("InComeList"),
                                                          pageModel.PageNumber, pageModel.PageSize,
                                                          csn, userName, type, accountid, realName));
            return View(model);
        }


        public ActionResult OutPutListFor2(string csn, string userName = "", string realName = "", int type = 1, int accountid = (int)AccountType.佣金账户)
        {
            string condition = AdminGetListCondition(csn, userName, type, accountid, realName);

            string sqlText = @"	 SELECT T.DrawCashSN 提现单号,
	  CONVERT(DATETIME,T.CreateTime,101)  提交时间,
	 b.username 会员编号,b.mobile 会员手机,
	 ud.realname 会员姓名,
	 T.Amount 提现金额,
	 T.TaxAmount 税费,
	 T.Poundage 手续费,
	 T.ActualAmount 实际金额,
	 T.BankName 银行名称,
	 T.BankProvice+T.BankCity+T.BankAddress 银行开户行,
	 T.BankCardCode 银行卡号,
	 T.BankUserName 开户人,
	 CASE T.State WHEN 0 THEN '处理失败' WHEN 1 THEN '等待处理' WHEN 2 THEN '处理成功'  ELSE '未知' END 状态
	
	  from hlh_HaiMiDrawCash T LEFT JOIN hlh_users b on T.uid=b.uid inner join hlh_userdetails ud on ud.uid=T.uid WHERE " + condition + "  ORDER BY T.CreateTime DESC";
            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "海米/代理提现-" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 导出报表
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="osn"></param>
        /// <param name="consignee"></param>
        /// <param name="orderState"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ActionResult OutPutList(string csn, string userName = "", string realName = "", int type = 1, int accountid = (int)AccountType.佣金账户)
        {
            string condition = AdminGetListCondition(csn, userName, type, accountid, realName);

            string sqlText = @"	 SELECT T.DrawCashSN 提现单号,
	  CONVERT(DATETIME,T.CreateTime,101)  提交时间,
	 b.username 会员编号,b.mobile 会员手机,
	 ud.realname 会员姓名,
	 T.Amount 提现金额,
	 T.TaxAmount 税费,
	 T.Poundage 手续费,
	 T.ActualAmount 实际金额,
	 T.BankName 银行名称,
	 T.BankProvice+T.BankCity+T.BankAddress 银行开户行,
	 T.BankCardCode 银行卡号,
	 T.BankUserName 开户人,
	 CASE T.State WHEN 0 THEN '处理失败' WHEN 1 THEN '等待处理' WHEN 2 THEN '处理成功'  ELSE '未知' END 状态
	
	  from hlh_HaiMiDrawCash T LEFT JOIN hlh_users b on T.uid=b.uid inner join hlh_userdetails ud on ud.uid=T.uid WHERE " + condition + "  ORDER BY T.CreateTime DESC";
            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "佣金提现-" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 更新处理状态
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult UpdateState(int Id = -1, int state = 2, string remark = "银行转账已成功")
        {
            HaiMiDrawCashInfo Info = haiMiDrawCashBLL.GetModel(Id);

            if (Info == null)
                return PromptView("记录不存在");
            bool result = haiMiDrawCashBLL.SetState(Id, state, remark, Info);
            if (result)
                return PromptView("更新成功");
            else
                return PromptView("更新失败");
        }
        public ActionResult BatchUpdateStateFail(string ids, string remark)
        {
            return RedirectToAction("BatchUpdateState", new { ids = ids, state = 0, remark = remark });
        }
        /// <summary>
        /// 批量处理状态
        /// </summary>
        public ActionResult BatchUpdateState(int[] idList = null, int state = 2, string remark = "银行转账已成功", string ids = "")
        {
            if (!string.IsNullOrEmpty(ids))
            {
                string[] idss = StringHelper.SplitString(ids);
                List<int> aa = new List<int>();
                foreach (var i in idss)
                {
                    aa.Add(TypeHelper.StringToInt(i));
                }
                idList = aa.ToArray();
            }
            bool flag = haiMiDrawCashBLL.SetStateByIds(idList, state, remark);
            string showMsg = "批量提现成功处理成功";
            //撤销退回
            if (state == 0)
            {
                foreach (int i in idList)
                {
                    HaiMiDrawCashInfo info = haiMiDrawCashBLL.GetModel(" [Id]='" + i + "'");
                    if (info != null)
                    {
                        if (info.State == 2)
                            continue;
                        PartUserInfo user = Users.GetPartUserById(info.Uid);
                        //string accountName = Enum.GetName(typeof(AccountType), info.AccountId);
                        //更新直销的账户--不包含代理账户和佣金账户
                        if (user.IsDirSaleUser && (info.AccountId != (int)AccountType.代理账户 && info.AccountId != (int)AccountType.佣金账户))
                        {
                            AccountUtils.UpdateAccountForDir(user.DirSaleUid, info.AccountId, info.Amount, 0, info.DrawCashSN, string.Format("提现退回,金额:{0},退回原因：{1}", info.Amount, remark));
                        }
                        else
                        {
                            //更新账户
                            Account.UpdateAccountForIn(new AccountInfo()
                            {
                                AccountId = info.AccountId,
                                UserId = info.Uid,
                                TotalIn = info.Amount
                            });
                            Account.CreateAccountDetail(new AccountDetailInfo()
                            {
                                AccountId = info.AccountId,
                                UserId = info.Uid,
                                CreateTime = DateTime.Now,
                                DetailType = (int)DetailType.提现取消返回,
                                InAmount = info.Amount,
                                OrderCode = info.DrawCashSN,
                                AdminUid = 0,//system
                                Status = 1,
                                DetailDes = string.Format("提现退回,金额:{0},退回原因：{1}", info.Amount, remark)
                            });
                        }
                    }
                }
                showMsg = "批量提现撤销处理成功";
            }

            return PromptView(showMsg);
        }
        /// <summary>
        /// 批量更新处理成功状态
        /// </summary>
        public ActionResult BatchUpdateStateBySN(string snList, int state = 2, string result = "")
        {

            bool flag = haiMiDrawCashBLL.SetStateBySNs(snList, state, result);
            //撤销提现要返回海米
            string[] sns = StringHelper.SplitString(snList, "\n");
            if (state == 0)
            {
                foreach (string i in sns)
                {
                    HaiMiDrawCashInfo info = haiMiDrawCashBLL.GetModel(" [DrawCashSN]='" + i + "'");
                    if (info != null)
                    {
                        string accountName = Enum.GetName(typeof(AccountType), info.AccountId);
                        if (info.AccountId == (int)AccountType.佣金账户 || info.AccountId == (int)AccountType.代理账户)
                        {
                            Account.UpdateAccountForIn(new AccountInfo()
                            {
                                AccountId = info.AccountId,
                                UserId = WorkContext.Uid,
                                TotalIn = info.Amount
                            });
                            Account.CreateAccountDetail(new AccountDetailInfo()
                            {
                                AccountId = info.AccountId,
                                UserId = WorkContext.Uid,
                                CreateTime = DateTime.Now,
                                DetailType = (int)DetailType.提现取消返回,
                                InAmount = info.Amount,
                                OrderCode = info.DrawCashSN,
                                AdminUid = 0,//system
                                Status = 1,
                                DetailDes = string.Format(accountName + "提现撤销：金额:{0}", info.Amount)
                            });
                        }
                        else
                        {
                            //更新直销的账户
                            if (WorkContext.PartUserInfo.IsDirSaleUser)
                            {
                                AccountUtils.UpdateAccountForDir(WorkContext.PartUserInfo.DirSaleUid, info.AccountId, info.Amount, 0, info.DrawCashSN, string.Format("海米提现撤销：金额:{0}", info.Amount));
                            }
                            else
                            {
                                Account.UpdateAccountForIn(new AccountInfo()
                                {
                                    AccountId = info.AccountId,
                                    UserId = WorkContext.Uid,
                                    TotalIn = info.Amount
                                });
                                Account.CreateAccountDetail(new AccountDetailInfo()
                                {
                                    AccountId = info.AccountId,
                                    UserId = WorkContext.Uid,
                                    CreateTime = DateTime.Now,
                                    DetailType = (int)DetailType.提现取消返回,
                                    InAmount = info.Amount,
                                    OrderCode = info.DrawCashSN,
                                    AdminUid = 0,//system
                                    Status = 1,
                                    DetailDes = string.Format(accountName + "提现撤销：金额:{0}", info.Amount)
                                });
                            }
                        }
                    }
                }
            }
            if (flag)
                return Content("1");
            return Content("0");
        }


        #region 用户帐号列表
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="accountid"></param>
        /// <param name="type"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult UserAccountList(string userName, string realName, int accountid = (int)AccountType.代理账户, int type = 1, int pageNumber = 1, int pageSize = 15)
        {
            StringBuilder condition = new StringBuilder();
            int accountId = accountid;
            condition.AppendFormat(string.Format(" T.AccountId={0}", accountId));
            if (!string.IsNullOrWhiteSpace(userName))
                condition.AppendFormat(" and  (c.[username] like '%{0}%' OR c.[email] like '%{0}%' OR c.[mobile] like '%{0}%') ", userName.Trim());
            if (type == 1)
            {
                condition.AppendFormat(" and T.Banlance>0");
            }
            if (accountid == -1)
                condition.AppendFormat(" AND T.[AccountId] in ({0}) ", (int)AccountType.海米账户 + "," + (int)AccountType.代理账户);
            else
                condition.AppendFormat(" AND T.[AccountId] = {0} ", accountid);

            if (!string.IsNullOrWhiteSpace(realName))
                condition.AppendFormat(" AND (ud.[realname] like '%{0}%') ", realName.Trim());
            PageModel pageModel = new PageModel(pageSize, pageNumber, Account.GetAllAccountInfoListCount(condition.ToString()));

            IncomeAccountModel model = new IncomeAccountModel()
            {
                IncomeAccountList = Account.AdminGetAllAccountInfoListByAid(condition.ToString(), "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                UserName = userName,
                Type = type,
                AccountId = accountid

            };
            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部", Value = "0" });
            itemList.Add(new SelectListItem() { Text = "有效（余额大于0）", Value = "1" });
            ViewData["typeList"] = itemList;
            List<SelectListItem> accountitemList = new List<SelectListItem>();
            accountitemList.Add(new SelectListItem() { Text = "代理账户", Value = ((int)AccountType.代理账户).ToString() });
            accountitemList.Add(new SelectListItem() { Text = "佣金账户", Value = ((int)AccountType.佣金账户).ToString() });
            ViewData["accountitemList"] = accountitemList;

            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&userName={3}&type={4}&accountid={5}&realName={6}",
                                                          Url.Action("UserAccountList"),
                                                          pageModel.PageNumber, pageModel.PageSize,
                                                           userName, type, accountid, realName));
            return View(model); ;

        }
        #endregion


    }
}
