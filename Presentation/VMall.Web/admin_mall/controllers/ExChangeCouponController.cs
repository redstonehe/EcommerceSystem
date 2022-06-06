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
    /// 商城后台兑换码控制器类
    /// </summary>
    public partial class ExChangeCouponController : BaseMallAdminController
    {
        ExCodeTypes ExCodeTypesBLL = new ExCodeTypes();
        ExCodeProducts ExCodeProductsBLL = new ExCodeProducts();
        #region 兑换码类型
        /// <summary>
        /// 后台获得兑换码类型列表条件
        /// </summary>
        /// <param name="csn"></param>
        /// <param name="username"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string AdminListCondition(int storeId, int type, string codeTypeName)
        {
            StringBuilder condition = new StringBuilder();

            if (storeId > 0)
                condition.AppendFormat(" AND [storeid] = {0}", storeId);

            //if (type == 0)
            //    condition.AppendFormat(" AND ([sendmode]=1 OR [sendmode]=2 OR ([sendmode]=0 AND [sendstarttime]<='{0}' AND [sendendtime]>'{0}'))", DateTime.Now);
            //else if (type == 1)
            //    condition.AppendFormat(" AND ([useexpiretime]>0 OR ([useexpiretime]=0 AND [usestarttime]<='{0}' AND [useendtime]>'{0}'))", DateTime.Now);

            if (!string.IsNullOrWhiteSpace(codeTypeName))
                condition.AppendFormat(" AND [name] like '%{0}%' ", codeTypeName.Trim());

            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }
        /// <summary>
        /// 兑换码类型列表
        /// </summary>
        /// <param name="storeName">店铺名称</param>
        /// <param name="codeTypeName">兑换码类型名称</param>
        /// <param name="storeId">店铺id</param>
        /// <param name="type">类型0代表正在发放，1代表正在使用，-1代表全部</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult CodeTypeList(string storeName, string codeTypeName, int storeId = -1, int type = -1, int pageNumber = 1, int pageSize = 15)
        {
            if (storeId == -1)
                storeId = WorkContext.PartUserInfo.StoreId;
            string condition = AdminListCondition(storeId, type, codeTypeName);

            PageModel pageModel = new PageModel(pageSize, pageNumber, ExCodeTypesBLL.GetRecordCount(condition));

            CodeTypeListModel model = new CodeTypeListModel()
            {
                CodeTypeList = ExCodeTypesBLL.AdminGetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                StoreId = storeId,
                StoreName = string.IsNullOrWhiteSpace(storeName) ? "全部店铺" : storeName,
                Type = type,
                CodeTypeName = codeTypeName
            };

            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemList.Add(new SelectListItem() { Text = "正在发放", Value = "0" });
            itemList.Add(new SelectListItem() { Text = "正在使用", Value = "1" });
            ViewData["typeList"] = itemList;

            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&storeId={3}&storeName={4}&codeTypeName={5}&type={6}",
                                                          Url.Action("codetypelist"),
                                                          pageModel.PageNumber, pageModel.PageSize,
                                                          storeId, storeName, codeTypeName, type));
            return View(model);
        }


        /// <summary>
        /// 添加兑换码类型
        /// </summary>
        [HttpGet]
        public ActionResult AddCodeType()
        {
            CodeTypeModel model = new CodeTypeModel();
            LoadcodeType();
            return View(model);
        }

        /// <summary>
        /// 添加兑换码类型
        /// </summary>
        [HttpPost]
        public ActionResult AddCodeType(CodeTypeModel model)
        {
            if (ModelState.IsValid)
            {
                DateTime nullTime = new DateTime(1970, 1, 1);

                if (model.SendModel == 1 || model.SendModel == 2)
                {
                    model.GetModel = 0;
                    model.SendStartTime = nullTime;
                    model.SendEndTime = nullTime;
                }

                if (model.UseTimeType == 1)
                {
                    model.UseStartTime = nullTime;
                    model.UseEndTime = nullTime;
                }
                else
                {
                    model.UseExpireTime = 0;
                }

                ExCodeTypesInfo info = new ExCodeTypesInfo()
                {
                    StoreId = model.StoreId,
                    Name = model.CodeTypeName,
                    Money = model.Money,
                    Count = model.Count,
                    SendMode = model.SendModel,
                    GetMode = model.GetModel,
                    UseMode = model.UseModel,
                    UserRankLower = model.UserRankLower,
                    OrderAmountLower = model.OrderAmountLower,
                    LimitStoreCid = -1,// model.LimitStoreCid,
                    LimitProduct = model.LimitProduct,
                    SendStartTime = model.SendStartTime.Value,
                    SendEndTime = model.SendEndTime.Value,
                    UseExpireTime = model.UseExpireTime,
                    UseStartTime = model.UseStartTime.Value,
                    UseEndTime = model.UseEndTime.Value,
                    State = model.State
                };
                ExCodeTypesBLL.Add(info);
                AddMallAdminLog("添加兑换码类型", "添加兑换码类型,兑换码类型为:" + model.CodeTypeName);
                return PromptView("兑换码类型添加成功");
            }
            LoadcodeType();
            return View(model);
        }

        /// <summary>
        /// 展示兑换码类型
        /// </summary>
        /// <param name="codeTypeId">兑换码类型id</param>
        public ActionResult ShowCodeType(int codeTypeId = -1)
        {
            ExCodeTypesInfo codeTypeInfo = ExCodeTypesBLL.GetModel(codeTypeId);
            if (codeTypeInfo == null)
                return PromptView("兑换码类型不存在");

            CodeTypeModel model = new CodeTypeModel();
            model.CodeTypeName = codeTypeInfo.Name;
            model.Money = codeTypeInfo.Money;
            model.Count = codeTypeInfo.Count;
            model.SendModel = codeTypeInfo.SendMode;
            model.GetModel = codeTypeInfo.GetMode;
            model.UseModel = codeTypeInfo.UseMode;
            model.UserRankLower = codeTypeInfo.UserRankLower;
            model.OrderAmountLower = codeTypeInfo.OrderAmountLower;
            model.LimitStoreCid = codeTypeInfo.LimitStoreCid;
            model.LimitProduct = codeTypeInfo.LimitProduct;
            model.UseExpireTime = codeTypeInfo.UseExpireTime;
            model.SendStartTime = codeTypeInfo.SendStartTime;
            model.SendEndTime = codeTypeInfo.SendEndTime;
            model.UseStartTime = codeTypeInfo.UseStartTime;
            model.UseEndTime = codeTypeInfo.UseEndTime;
            model.State = codeTypeInfo.State;

            model.StoreId = codeTypeInfo.StoreId;
            //model.StoreName = AdminStores.GetStoreById(codeTypeInfo.StoreId).Name;

            LoadcodeType();
            return View(model);
        }

        /// <summary>
        /// 改变兑换码类型状态
        /// </summary>
        /// <param name="codeTypeId">兑换码类型id</param>
        /// <param name="state">状态</param>
        public ActionResult ChangeCodeTypeState(int codeTypeId = -1, int state = 0)
        {
            ExCodeTypesInfo info = ExCodeTypesBLL.GetModel(codeTypeId);
            if (info == null)
                return PromptView("兑换码类型不存在");
            info.State = state;
            if (ExCodeTypesBLL.Update(info))
                return PromptView("更改兑换码类型状态成功");
            else
                return PromptView("更改兑换码类型状态失败");
        }

        /// <summary>
        /// 删除兑换码类型
        /// </summary>
        /// <param name="codeTypeIdList">兑换码类型id列表</param>
        /// <returns></returns>
        public ActionResult DelcodeType(int[] codeTypeIdList)
        {
            string ids = CommonHelper.IntArrayToString(codeTypeIdList);
            ExCodeTypesBLL.DeleteList(ids);
            AddMallAdminLog("删除兑换码类型", "删除兑换码类型,兑换码类型ID为:" + ids);
            return PromptView("兑换码类型删除成功");
        }

        private void LoadcodeType()
        {
            List<SelectListItem> itemList = new List<SelectListItem>();
            foreach (UserRankInfo userRankInfo in AdminUserRanks.GetCustomerUserRankList())
            {
                itemList.Add(new SelectListItem() { Text = userRankInfo.Title, Value = userRankInfo.UserRid.ToString() });
            }
            ViewData["userRankList"] = itemList;

            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
        }

        #endregion

        #region 兑换码商品操作
        /// <summary>
        /// 兑换码商品列表
        /// </summary>
        /// <param name="codeTypeId">兑换码类型id</param>
        /// <returns></returns>
        public ActionResult CodeProductList(int codeTypeId = -1, int pid = -1, int pageSize = 15, int pageNumber = 1)
        {
            ExCodeTypesInfo codeTypeInfo = ExCodeTypesBLL.GetModel(codeTypeId);
            if (codeTypeInfo == null)
                return PromptView("兑换码类型不存在");
            if (codeTypeInfo.LimitProduct == 0)
                return PromptView("此兑换码类型没有限制商品");

            string condition = string.Format(" codetypeid ={0} ", codeTypeId);
            PageModel pageModel = new PageModel(pageSize, pageNumber, ExCodeProductsBLL.GetRecordCount(condition));

            CodeProductListModel model = new CodeProductListModel()
            {
                CodeProductList = ExCodeProductsBLL.AdminGetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                CodeTypeId = codeTypeId,
                StoreId = codeTypeInfo.StoreId
            };

            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&codeTypeId={3}",
                                                          Url.Action("codeproductlist"),
                                                          pageModel.PageNumber,
                                                          pageModel.PageSize,
                                                          codeTypeId));
            return View(model);
        }

        /// <summary>
        /// 添加兑换码商品
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCodeProduct(int codeTypeId = -1, int pid = 1)
        {
            ExCodeTypesInfo codeTypeInfo = ExCodeTypesBLL.GetModel(codeTypeId);
            if (codeTypeInfo == null)
                return PromptView("兑换码类型不存在");
            if (codeTypeInfo.LimitProduct == 0)
                return PromptView("此兑换码类型没有限制商品");

            PartProductInfo partProductInfo = AdminProducts.AdminGetPartProductById(pid);
            if (partProductInfo == null)
                return PromptView("此商品不存在");

            //if (codeTypeInfo.StoreId != partProductInfo.StoreId)
            //    return PromptView(Url.Action("codeproductlist", new { codeTypeId = codeTypeId }), "只能关联同一店铺的商品");

            if (ExCodeProductsBLL.IsExistCodeProduct(codeTypeId, pid))
                return PromptView("此商品已经存在");

            ExCodeProductsBLL.Add(new ExCodeProductsInfo
            {
                CodeTypeId = codeTypeId,
                Pid = pid

            });
            AddMallAdminLog("添加兑换码商品", "添加兑换码商品,商品为:" + partProductInfo.Name);
            return PromptView("兑换码商品添加成功");
        }

        /// <summary>
        /// 删除兑换码商品
        /// </summary>
        /// <param name="recordId">记录id</param>
        /// <returns></returns>
        [HttpAJAX]
        public ActionResult DelCodeProduct(int recordId)
        {
            bool result = ExCodeProductsBLL.Delete(recordId);
            if (result)
            {
                AddMallAdminLog("删除兑换码商品", "删除兑换码商品,商品ID:" + recordId);
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }

        /// <summary>
        /// 删除兑换码商品
        /// </summary>
        /// <param name="recordIdList">记录id</param>
        /// <returns></returns>
        public ActionResult DelCodeProduct(int[] recordIdList)
        {
            ExCodeProductsBLL.DeleteList(CommonHelper.IntArrayToString(recordIdList));
            AddMallAdminLog("删除兑换码商品", "删除兑换码商品,商品ID:" + CommonHelper.IntArrayToString(recordIdList));
            return PromptView("兑换码商品删除成功");
        }

        #endregion

        #region 兑换码操作

        public string AdminGetExChangeCouponListCondition(string exsn, string username, int type, int codetypeid)
        {
            StringBuilder condition = new StringBuilder();

            if (type == 1)
                condition.AppendFormat(" AND T.[validtime] >= GETDATE() ", type);
            else if (type == 0)
                condition.AppendFormat(" AND T.[validtime] < GETDATE() ", type);
            if (!string.IsNullOrWhiteSpace(exsn))
                condition.AppendFormat(" AND T.[exsn] ='{0}' ", exsn);
            if (!string.IsNullOrEmpty(username))
                condition.AppendFormat(" AND (b.[username] = '{0}' or b.[email] = '{0}' or b.[mobile] = '{0}') ", username);
            if (codetypeid > 0)
                condition.AppendFormat(" AND T.[codetypeid] = {0} ", codetypeid);
            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }
        /// <summary>
        /// 兑换码列表
        /// </summary>

        /// <param name="codeTypeName">卡号</param>
        /// <param name="storeId">会员标识</param>
        /// <param name="type">类型0代表无效，1代表有效，-1代表全部</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="pageSize">每页数</param>
        /// <returns></returns>
        public ActionResult ExChangeCouponList(string exsn, string userName = "", int type = -1, int pageNumber = 1, int pageSize = 15, int codeTypeId = -1)
        {
            string condition = AdminGetExChangeCouponListCondition(exsn, userName, type, codeTypeId);

            PageModel pageModel = new PageModel(pageSize, pageNumber, ExChangeCoupons.AdminGetRecordCount(condition));

            ExChangeCouponListModel model = new ExChangeCouponListModel()
            {
                ExChangeCouponList = ExChangeCoupons.AdminGetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                EXSN = exsn,
                UserName = userName,
                Type = type,
                CodeTypeId = codeTypeId,
            };
            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemList.Add(new SelectListItem() { Text = "有效", Value = "1" });
            itemList.Add(new SelectListItem() { Text = "无效", Value = "0" });
            ViewData["typeList"] = itemList;

            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&csn={3}&userName={4}&type={5}&codeTypeId={6}",
                                                          Url.Action("exchangecouponlist"),
                                                          pageModel.PageNumber, pageModel.PageSize,
                                                          exsn, userName, type, codeTypeId));
            return View(model);
        }


        /// <summary>
        /// 增加兑换码
        /// </summary>
        /// <param name="">兑换码id</param>
        [HttpGet]
        public ActionResult AddExChangeCoupon()
        {
            ExChangeCouponModel model = new ExChangeCouponModel();
            List<SelectListItem> CodeTypeList = new List<SelectListItem>();
            CodeTypeList.Add(new SelectListItem() { Text = "3月面膜兑换码", Value = "2" });
            CodeTypeList.Add(new SelectListItem() { Text = "优兑换码", Value = "3" });
            CodeTypeList.Add(new SelectListItem() { Text = "精兑换码", Value = "4" });
            CodeTypeList.Add(new SelectListItem() { Text = "臻兑换码", Value = "5" });

            ViewData["CodeTypeList"] = CodeTypeList;
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();

            return View(model);
        }

        /// <summary>
        /// 增加卡券
        /// </summary>
        /// <param name="model">兑换码发放模型</param>
        /// <param name="codeTypeId">兑换码类型id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddExChangeCoupon(ExChangeCouponModel model)
        {
            PartUserInfo user = Users.GetPartUserById(CashCoupon.GetUidByName(model.UserName));
            if (user == null)
                return PromptView("会员不存在！");
            if (model.CodeTypeId == 2)
            {
                List<ExChangeCouponsInfo> list = ExChangeCoupons.GetList(string.Format(" uid={0} and type=2", user.Uid));
                if (list.Count >= 1)
                    return PromptView("该会员已经存在后台赠送类型兑换码");
            }
            if (ModelState.IsValid)
            {
                string cardNum = string.Empty;
                if (model.CodeTypeId == 2)
                    cardNum = "DH" + Randoms.CreateRandomValue(8, true).ToLower();//Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                if (model.CodeTypeId == 3)
                    cardNum = "YDH" + Randoms.CreateRandomValue(8, true).ToLower();
                if (model.CodeTypeId == 4)
                    cardNum = "JDH" + Randoms.CreateRandomValue(8, true).ToLower();
                if (model.CodeTypeId == 5)
                    cardNum = "ZDH" + Randoms.CreateRandomValue(8, true).ToLower();
                int cashId = ExChangeCoupons.Add(new ExChangeCouponsInfo()
                {
                    exsn = cardNum,
                    uid = user.Uid,
                    type = model.CodeTypeId,
                    state = 1,
                    createuid = 1,
                    validtime = model.ValidTime,
                    createtime = DateTime.Now,
                    codetypeid=model.CodeTypeId
                });

                AddMallAdminLog("后台增加兑换码", "兑换码：" + cardNum);
                string backUrl = Url.Action("exchangecouponlist", new  {codeTypeId=model.CodeTypeId });
                return PromptView(backUrl, "兑换码增加成功");
            }
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 兑换码禁用
        /// </summary>
        /// <param name="CashId"></param>
        /// <returns></returns>
        public ActionResult BanExchange(int ExId)
        {
            ExChangeCouponsInfo info = ExChangeCoupons.GetModel(ExId);
            if (info == null)
                return PromptView("兑换码不存在");
            info.validtime = info.createtime;
            info.state = 0;
            ExChangeCoupons.Update(info);
            return PromptView(Url.Action("ExChangeCouponList"), "禁用成功");
        }

        #endregion

    }
}
