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
    /// 商城后台汇购卡控制器类
    /// </summary>
    public partial class CashCouponController : BaseMallAdminController
    {
        CashCoupon bllCashCoupon = new CashCoupon();
        public string AdminGetcashCouponListCondition(string csn, string username, int type)
        {
            StringBuilder condition = new StringBuilder();

            if (type == 1)
                condition.AppendFormat(" AND T.[ValidTime] >= GETDATE() ", type);
            else if (type == 0)
                condition.AppendFormat(" AND T.[ValidTime] < GETDATE() ", type);
            if (!string.IsNullOrWhiteSpace(csn))
                condition.AppendFormat(" AND T.[CashCouponSN] ='{0}' ", csn.Trim());
            if (!string.IsNullOrEmpty(username))
                condition.AppendFormat(" AND (b.[username] = '{0}' or b.[email] = '{0}' or b.[mobile] = '{0}') ", username.Trim());
            condition.Append(" AND T.[CouponType] = 1 ");
            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }
        /// <summary>
        /// 汇购卡券列表
        /// </summary>

        /// <param name="couponTypeName">卡号</param>
        /// <param name="storeId">会员标识</param>
        /// <param name="type">类型0代表无效，1代表有效，-1代表全部</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult CashCouponList(string csn, string userName="", int type = 1, int pageNumber = 1, int pageSize = 15)
        {
            string condition = AdminGetcashCouponListCondition(csn, userName, type);

            PageModel pageModel = new PageModel(pageSize, pageNumber, CashCoupon.AdminGetRecordCount(condition));

            CashCouponListModel model = new CashCouponListModel()
            {
                CashCouponList = CashCoupon.AdminGetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                CSN = csn,
                UserName = userName,
                Type = type,
            };
            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemList.Add(new SelectListItem() { Text = "有效", Value = "1" });
            itemList.Add(new SelectListItem() { Text = "无效", Value = "0" });
            ViewData["typeList"] = itemList;

            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&csn={3}&userName={4}&type={5}",
                                                          Url.Action("cashcouponlist"),
                                                          pageModel.PageNumber, pageModel.PageSize,
                                                          csn, userName, type));
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
        public ActionResult OutPutList(string csn, string userName = "", int type = 1)
        {
            string condition = AdminGetcashCouponListCondition(csn, userName, type);

            string sqlText = @"SELECT  
T.CashCouponSN 汇购卡号,
T.CreationDate 创建时间,
b.username 用户名,
b.mobile 手机号,
T.CreateOSN 购买汇购卡订单号,
T.CashAmount 面值,
T.Banlance 	可用余额,
T.ValidTime 有效期
FROM hlh_CashCoupon T LEFT JOIN hlh_users b on T.uid=b.uid WHERE " + condition + "   order by T.CashId DESC";
            DataTable dt = AdminOrders.GetOrderByCondition(sqlText);

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "GB2312";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", "汇购卡-" + DateTime.Now.ToString("yyyyMMdd")));
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
        /// 增加卡券
        /// </summary>
        /// <param name="couponTypeId">优惠劵类型id</param>
        [HttpGet]
        public ActionResult AddCashCoupon(int couponTypeId = -1)
        {

            CashCouponModel model = new CashCouponModel();

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 增加卡券
        /// </summary>
        /// <param name="model">优惠劵发放模型</param>
        /// <param name="couponTypeId">优惠劵类型id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddCashCoupon(CashCouponModel model)
        {
            PartUserInfo user = Users.GetPartUserById(CashCoupon.GetUidByName(model.UserName));
            if(user==null)
                return PromptView("会员不存在或未登陆过！");
            if(!user.IsDirSaleUser)
                return PromptView("非直销会员不能添加汇购卡！");
            List<CashCouponInfo> list = CashCoupon.GetList(" uid= "+user.Uid);
            //if (user.Uid == 15078 || user.Uid == 24108)
            //{
            //    if (list.Count >= 6)
            //        return PromptView("会员最多拥有6张汇购卡");
            //}
            //else
            //{
                if (list.Count >= user.MaxCashCount)
                    return PromptView("该会员最多拥有" + user.MaxCashCount + "张汇购卡");
            //}
            if (ModelState.IsValid)
            {
                string cardNum = "Q" + DateTime.Now.ToString("yyyyMMddHHmmss") + Randoms.CreateRandomValue(5);
                int cashId = CashCoupon.Add(new CashCouponInfo()
                {
                    CashCouponSN = cardNum,
                    CouponType = 1,
                    ChannelId = 4,
                    Uid = user.Uid,
                    CreateOid = 0,
                    CreateOSN = "",
                    CashAmount =model.CashAmount,
                    TotalIn = model.CashAmount,
                    Banlance = model.CashAmount,
                    ValidTime = model.CreationDate.AddYears(1),
                    DirSaleUid = user.DirSaleUid
                });
                CashCouponDetail.Add(new CashCouponDetailInfo()
                {
                    CashId = cashId,

                    Uid = user.Uid,
                    DetailType = (int)CashDetailType.线下刷卡生成,
                    InAmount = model.CashAmount,
                    DetailDes = "线下刷卡获得尚睿·淳 消费合伙人资格万元代金券",
                    Status = 1,
                    DirSaleUid = user.DirSaleUid
                });

                AddMallAdminLog("后台增加汇购卡券", "卡号：" + cardNum);

                string backUrl = Url.Action("cashcouponlist");

                return PromptView(backUrl,"卡券增加成功");
            }

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CashId"></param>
        /// <returns></returns>
        public ActionResult BanCash(int CashId) {
            CashCouponInfo info = CashCoupon.GetModel(CashId);
            if(info==null)
                return PromptView("汇购卡不存在");
            info.ValidTime=info.CreationDate;
            info.Banlance = 0;
            CashCoupon.Update(info);
            CashCouponDetail.Add(new CashCouponDetailInfo()
            {
                CreationDate = DateTime.Now,
                CashId = info.CashId,
                Uid = info.Uid,
                DetailType = (int)CashDetailType.订单使用抵现,
                OutAmount = 0,
                DetailDes = "汇购卡券撤销",
                Status = 1,
                DirSaleUid = info.DirSaleUid
            });
            return PromptView(Url.Action("CashCouponList"), "禁用成功");
        }

        

        /// <summary>
        /// 申请汇购卡退款
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ApplyCashRefund(int CashId = -1)
        {
            CashCouponInfo info = CashCoupon.GetModel(CashId);
            if (info == null)
                return PromptView("汇购卡不存在");
            if (info.CreateOid<=0)
                return PromptView("线下支付汇购卡不支持线上申请退款");
            if(info.ValidTime<DateTime.Now)
                return PromptView("汇购卡已过期，不能申请退款");
            if(info.Banlance<=0)
                return PromptView("汇购卡可用余额不足，不能申请退款");

            CashRefundApplyReturnModel model = new CashRefundApplyReturnModel();

            return View(model);
          
        }
        /// <summary>
        /// 申请汇购卡退款
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ApplyCashRefund(CashRefundApplyReturnModel model, int CashId = -1)
        {
            CashCouponInfo info = CashCoupon.GetModel(CashId);
            if (info == null)
                return PromptView("汇购卡不存在");
            if (info.CreateOid <= 0)
                return PromptView("线下支付汇购卡不支持线上申请退款");
            if (info.ValidTime < DateTime.Now)
                return PromptView("汇购卡已过期，不能申请退款");
            if (info.Banlance <= 0)
                return PromptView("汇购卡可用余额不足，不能申请退款");
            OrderInfo orderInfo = AdminOrders.GetOrderByOid(info.CreateOid);
            if (orderInfo == null)
                return PromptView("订单不存在");
            if (ModelState.IsValid)
            {
                PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
                decimal refundAmount = info.Banlance;
                info.ValidTime = info.CreationDate;
                info.Banlance = 0;
                //将汇购卡设为不可用
                CashCoupon.Update(info);
                CashCouponDetail.Add(new CashCouponDetailInfo()
                {
                    CreationDate = DateTime.Now,
                    CashId = info.CashId,
                    Uid = info.Uid,
                    DetailType = (int)CashDetailType.汇购卡退款余额清零,
                    OutAmount = 0,
                    DetailDes = "汇购卡退款,余额清零",
                    Status = 1,
                    DirSaleUid = info.DirSaleUid
                });
                //更改该汇购卡关联订单的退货状态，并写入订单跟踪记录
                Orders.ReturnOrder(ref partUserInfo, orderInfo, WorkContext.Uid, DateTime.Now);
                //Orders.UpdateOrderState(orderInfo.Oid,OrderState.Returned);

                Orders.UpdateOrderReturnType(orderInfo.Oid, 3);//将订单退货类型refundtype设为3

                //增加退款记录
                OrderRefunds.ApplyRefund(new OrderRefundInfo()
                {
                    StoreId = orderInfo.StoreId,
                    StoreName = orderInfo.StoreName,
                    Oid = orderInfo.Oid,
                    OSN = orderInfo.OSN,
                    Uid = orderInfo.Uid,
                    State = 0,
                    ApplyTime = DateTime.Now,
                    PayMoney = orderInfo.SurplusMoney,
                    RefundMoney = refundAmount - (model.RefundPayFee),
                    RefundSN = "",
                    RefundFriendName = orderInfo.PayFriendName,
                    RefundSystemName = orderInfo.PaySystemName,
                    PayFriendName = orderInfo.PayFriendName,
                    PaySystemName = orderInfo.PaySystemName,
                    RefundTime = DateTime.Now,
                    PaySN = orderInfo.PaySN,
                    RefundTranSN = "",//记录退款流水号 
                    ReMark = Enum.GetName(typeof(OrderSource), orderInfo.OrderSource) + ",汇购卡退款，卡余额：" + refundAmount + ",承担手续费" + model.RefundPayFee + "，退款说明:" + model.RefundDesc
                });

                AddMallAdminLog("汇购卡退款申请", "退款申请，汇购卡号" + info.CashCouponSN + "：订单ID为:" + info.CreateOid + "承担手续费：" + model.RefundPayFee + ",退款说明" + model.RefundDesc == null ? "" : model.RefundDesc + ",操作人：" + WorkContext.Uid);
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = orderInfo.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.Return,
                    ActionTime = DateTime.Now,
                    ActionDes = "订单退款申请成功，请等待系统处理退款"
                });
                return PromptView(Url.Action("CashCouponList"), "退款申请成功");
            }

            return View(model);
        }

        /// <summary>
        /// 汇购卡详情列表
        /// </summary>
        public ActionResult CashDetailList( int CashId , int pageNumber = 1, int pageSize = 15)
        {
           
            string condition =" CashId="+CashId;

            PageModel pageModel = new PageModel(pageSize, pageNumber, CashCouponDetail.GetRecordCount(condition));

            CashCouponDetailListModel model = new CashCouponDetailListModel()
            {
                CashCouponDetailList = CashCouponDetail.GetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                CashId = CashId
               
            };

            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&CashId={3}",
                                                          Url.Action("cashcouponlist"),
                                                          pageModel.PageNumber, pageModel.PageSize,
                                                         CashId));
            ViewData["referer"] = Url.Action("CashCouponList");
            return View(model);
        }


        public ActionResult CashExtend(int CashId)
        {

            CashCouponInfo info = CashCoupon.GetModel(CashId);
            if (info == null)
                return Content("403");
            if (info.ValidTime > DateTime.Now)
                return Content("300");
            info.ValidTime = info.ValidTime.AddYears(1);
            if (bllCashCoupon.Update(info, "ValidTime"))
            {
                AddMallAdminLog("汇购卡延期", "汇购卡延期,卡号为:" + info.CashCouponSN);
                return Content("200");
            }
            return Content("400");
            
        }

    }
}
