using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;
using System.Web.Routing;
using System.Text;
using System.Web.Configuration;
using System.Data;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台会员报单控制器类
    /// </summary>
    public partial class DSOrderController : BaseMallAdminController
    {
        private static object _locker = new object();//锁对象
        private OrderApply OrderBLL = new OrderApply();


        #region 1980代报单

        /// <summary>
        /// 获得列表搜索条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="osn">订单编号</param>
        /// <param name="uid">用户id</param>
        /// <param name="consignee">收货人</param>
        /// <param name="orderState">订单状态</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public string GetCondition(int operateuid, string operusername, string usercode, string realname, string consignee, string consigneeMobile, int State, int pid, string productName)
        {
            StringBuilder condition = new StringBuilder();
            if (operateuid > 0)
                condition.AppendFormat(" AND T.[operateuid] = {0} ", operateuid);
            if (!string.IsNullOrWhiteSpace(operusername))
                condition.AppendFormat(" AND T.[operateuid] IN (select uid from hlh_users where [username] like '%{0}%' OR [mobile] like '%{0}%' ) ", operusername.Trim());
            if (!string.IsNullOrWhiteSpace(usercode))
                condition.AppendFormat(" AND T.[usercode] like '{0}%' ", usercode.Trim());
            if (!string.IsNullOrWhiteSpace(realname))
                condition.AppendFormat(" AND T.[realname] like '{0}%' ", realname.Trim());
            if (!string.IsNullOrWhiteSpace(consignee))
                condition.AppendFormat(" AND T.[consignee] like '{0}%' ", consignee.Trim());
            if (!string.IsNullOrWhiteSpace(consigneeMobile))
                condition.AppendFormat(" AND T.[consignmobile] = '{0}' ", consigneeMobile.Trim());
            if (State >= 0)
                condition.AppendFormat(" AND T.[state] = {0} ", State);

            if (pid > 0)
                condition.AppendFormat(" AND T.[pid] ={0}  ", pid);
            if (!string.IsNullOrEmpty(productName))
                condition.AppendFormat(" AND T.[pid] IN (SELECT pid FROM hlh_products WHERE name LIKE '%{0}%' ) ", productName.Trim());

            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }
        /// <summary>
        /// 订单列表
        /// </summary>
        /// <param name="OperateUid"></param>
        /// <param name="OperUserName"></param>
        /// <param name="UserCode"></param>
        /// <param name="RealName"></param>
        /// <param name="Consignee"></param>
        /// <param name="ConsigneeMobile"></param>
        /// <param name="State"></param>
        /// <param name="Pid"></param>
        /// <param name="productName"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public ActionResult OrderApplyList(string OperUserName, string UserCode, string RealName, string Consignee = "", string ConsigneeMobile = "", int OperateUid = 0, int Pid = 0, string productName = "", int State = -1, int pageSize = 15, int pageNumber = 1)
        {
            string condition = GetCondition(OperateUid, OperUserName, UserCode, RealName, Consignee, ConsigneeMobile, State, Pid, productName);
            PageModel pageModel = new PageModel(pageSize, pageNumber, OrderBLL.GetRecordCount(condition));
            OrderApplyListModel model = new OrderApplyListModel()
            {
                OrderApplyList = OrderBLL.AdminGetListByPage(condition, "", (pageModel.PageNumber - 1) * pageModel.PageSize + 1, pageModel.PageNumber * pageModel.PageSize),
                PageModel = pageModel,
                OperateUid = OperateUid,
                OperUserName = OperUserName,
                UserCode = UserCode,
                RealName = RealName,
                Consignee = Consignee,
                ConsigneeMobile = ConsigneeMobile,
                State = State,
                Pid = Pid,
                ProductName = productName,
            };
            MallUtils.SetAdminRefererCookie(string.Format("{0}?pageNumber={1}&pageSize={2}&operateuid={3}&operusername={4}&usercode={5}&realname={6}&consignee={7}&consigneeMobile={8}&State={9}&pid={10}&productName={11}",
                                                          Url.Action("OrderApplyList"),
                                                          pageModel.PageNumber, pageModel.PageSize, OperateUid, OperUserName, UserCode, RealName, Consignee, ConsigneeMobile, State, Pid, productName));

            List<SelectListItem> itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem() { Text = "全部", Value = "-1" });
            itemList.Add(new SelectListItem() { Text = "审核未通过", Value = "0" });
            itemList.Add(new SelectListItem() { Text = "等待审核", Value = "1" });
            itemList.Add(new SelectListItem() { Text = "审核成功", Value = "1" });
            ViewData["StateList"] = itemList;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public ActionResult ApplyDetail(int id = -1)
        {
            OrderApplyInfo Info = OrderBLL.GetModel(id);
            if (Info == null)
                return PromptView("申请不存在");

            OrderApplyModel model = new OrderApplyModel();
            model.OrderApply = Info;
            UserInfo userInfo = Users.GetUserById(Info.operateuid);
            model.RegionInfo = Regions.GetRegionById(Info.regionid);
            model.UserInfo = userInfo;
            model.OrderProduct = Products.GetPartProductById(Info.pid);

            string[] sizeList = StringHelper.SplitString(WorkContext.MallConfig.ProductShowThumbSize);

            ViewData["size"] = sizeList[sizeList.Length / 2];
            ViewData["referer"] = MallUtils.GetMallAdminRefererCookie();
            return View(model);
        }

        /// <summary>
        /// 审核通过1980报单，生成会员，生成订单并安置直销点位
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmApply(int id = -1)
        {
            OrderApplyInfo info = OrderBLL.GetModel(id);
            if (info == null)
                return PromptView("申请不存在");

            string username = info.usercode;//用户名
            string realname = info.realname;
            string idcard = info.idcard;
            string pname = info.parentcode;
            string managerCode = info.managercode;
            int placeSide = info.placeside;
            int pid = info.pid;
            int regionid = info.regionid;
            string address = info.address;

            string accountName = "";//手机号
            if (ValidateHelper.IsMobile(username))
                accountName = username;

            StringBuilder sb = new StringBuilder();
            //账号验证
            if (string.IsNullOrWhiteSpace(accountName) && string.IsNullOrWhiteSpace(username))
            {
                sb.Append("账户名不能为空");
                ApplyStateForError(info, sb.ToString());
                return PromptView(sb.ToString());
            }

            if (!string.IsNullOrEmpty(accountName) && !ValidateHelper.IsMobile(accountName))
            {
                sb.Append("手机号格式不正确");
                ApplyStateForError(info, sb.ToString());
                return PromptView(sb.ToString());
            }

            UserInfo userInfo = null;

            if (string.IsNullOrEmpty(accountName))
            {
                if (Users.IsExistUserName(username) || AccountUtils.IsUserExistForDirSale(username))
                {
                    sb.Append("用户名已经存在");
                    ApplyStateForError(info, sb.ToString());
                    return PromptView(sb.ToString());
                }

                else
                    userInfo = new UserInfo() { UserName = username, Email = string.Empty, Mobile = string.Empty };
            }
            if (!string.IsNullOrEmpty(accountName) && ValidateHelper.IsMobile(accountName))
            {
                if (Users.IsExistMobile(accountName) || AccountUtils.IsUserExistForDirSale(accountName))
                {
                    sb.Append("手机号已经存在");
                    ApplyStateForError(info, sb.ToString());
                    return PromptView(sb.ToString());
                }
                else
                    userInfo = new UserInfo() { UserName = username, Email = string.Empty, Mobile = accountName };
            }

            #region 初始化用户信息
            string nickName = string.Empty;
            //生成随机初始密码并发送短信
            string password = "123456";//初始密码123456
            userInfo.Salt = Randoms.CreateRandomValue(6);
            userInfo.Password = Users.CreateUserPassword(password, userInfo.Salt);
            userInfo.UserRid = UserRanks.GetLowestUserRank().UserRid;
            userInfo.StoreId = 0;
            userInfo.MallAGid = 1;//非管理员组

            if (!string.IsNullOrEmpty(accountName))
                userInfo.NickName = accountName.Substring(0, 3) + "***" + accountName.Substring(7, 4);
            else
                userInfo.NickName = username;

            userInfo.Avatar = "";
            userInfo.PayCredits = 0;
            userInfo.RankCredits = 0;
            userInfo.VerifyEmail = 0;
            userInfo.VerifyMobile = 1;
            userInfo.DirSalePwd = SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);
            userInfo.LastVisitIP = "";
            userInfo.LastVisitRgId = -1;
            userInfo.LastVisitTime = DateTime.Now;
            userInfo.RegisterIP = "";
            userInfo.RegisterRgId = -1;
            userInfo.RegisterTime = DateTime.Now;
            userInfo.Gender = 0;
            userInfo.RealName = realname;
            userInfo.Bday = new DateTime(1900, 1, 1);
            userInfo.IdCard = idcard;
            userInfo.RegionId = -1;
            userInfo.Address = string.Empty;
            userInfo.Bio = string.Empty;

            #endregion

            #region 处理用户注册推荐人关系
            string parentname = pname;
            if (string.IsNullOrEmpty(parentname.Trim()))
            {
                sb.Append("推荐人为空，请输入推荐人!");
                ApplyStateForError(info, sb.ToString());
                return PromptView(sb.ToString());
            }
            int pType;
            int uIdByPname = AccountUtils.GetUidByPname(parentname.Trim(), out pType);
            if (string.IsNullOrEmpty(parentname.Trim()) || uIdByPname < 1)//推荐人为空或推荐人在后台不存在
            {
                sb.Append("推荐人不正确，请输入正确的推荐人");
                ApplyStateForError(info, sb.ToString());
                return PromptView(sb.ToString());
            }
            else //推荐人正确
            {
                userInfo.Pid = uIdByPname;
                userInfo.Ptype = pType;//推荐人类型
            }
            #endregion

            //创建用户
            userInfo.Uid = Users.CreateUser(userInfo);
            //添加用户失败
            if (userInfo.Uid < 1)
            {
                sb.Append("创建用户失败,请联系管理员");
                ApplyStateForError(info, sb.ToString());
                return PromptView(sb.ToString());
            }

            MemberInfo member = new MemberInfo();
            if (!userInfo.IsDirSaleUser)
                member = AccountUtils.CreateMember(userInfo, realname, managerCode, placeSide, idcard, accountName);
            if (member.userId <= 0)
            {
                sb.Append("创建点位失败,请联系管理员");
                ApplyStateForError(info, sb.ToString());
                return PromptView(sb.ToString());
            }

            #region 订单

            #region 创建订单

            PartProductInfo partProductInfo = Products.GetPartProductById(pid);
            //将商品添加到购物车

            //需要添加的商品列表
            List<OrderProductInfo> addOrderProductList = new List<OrderProductInfo>();
            //初始化订单商品
            OrderProductInfo mainOrderProductInfo = Carts.BuildOrderProduct(partProductInfo);
            mainOrderProductInfo.Sid = "";
            mainOrderProductInfo.Uid = userInfo.Uid;

            mainOrderProductInfo.AddTime = DateTime.Now;
            mainOrderProductInfo.ProductPV = partProductInfo.PV;
            mainOrderProductInfo.ProductHaiMi = partProductInfo.HaiMi;
            mainOrderProductInfo.ProductHBCut = partProductInfo.HongBaoCut;
            int buyCount = 1;// (int)(amount / mainOrderProductInfo.DiscountPrice);
            mainOrderProductInfo.RealCount = buyCount;
            mainOrderProductInfo.BuyCount = buyCount;
            //将商品添加到"需要添加的商品列表"中
            addOrderProductList.Add(mainOrderProductInfo);
            Carts.ClearCart(userInfo.Uid, "");

            //将需要添加的商品持久化
            Carts.AddOrderProductList(addOrderProductList);
            PartUserInfo orderUser = Users.GetPartUserById(userInfo.Uid);
            StoreInfo storeInfo = Stores.GetStoreById(WebHelper.GetConfigSettingsInt("AgentSuitStoreId"));

            FullShipAddressInfo shipAddressInfo = new FullShipAddressInfo();
            shipAddressInfo.Uid = userInfo.Uid;
            shipAddressInfo.RegionId = regionid;
            shipAddressInfo.IsDefault = 1;
            shipAddressInfo.Alias = "地址1";
            shipAddressInfo.Consignee = info.consignee;
            shipAddressInfo.Mobile = info.consignmobile;
            shipAddressInfo.Phone = "";
            shipAddressInfo.Email = "";
            shipAddressInfo.ZipCode = "";
            shipAddressInfo.Address = WebHelper.HtmlEncode(address);

            string payName = "emsremit";
            PluginInfo payPluginInfo = Plugins.GetPayPluginBySystemName(payName);

            int payCreditCount = 0;
            decimal haiMiCount = 0;
            decimal hongBaoCount = 0;
            decimal cashCount = 0;
            decimal couponMoeny = 0;
            decimal YongJinCount = 0;
            decimal daiLiCount = 0;
            int storeFullCut = 0;
            string buyerRemark = "";
            string invoice = "";
            DateTime bestTime = DateTime.Now;
            string ip = "";
            string cashId = "";

            List<OrderProductInfo> odProductList = Carts.GetCartProductList(userInfo.Uid).FindAll(x => x.Pid == pid);
            OrderInfo orderInfo = Orders.CreateOrder(orderUser, storeInfo, odProductList, new List<SinglePromotionInfo>(), shipAddressInfo, payPluginInfo, ref payCreditCount, ref haiMiCount, ref hongBaoCount, ref cashCount, new List<CouponInfo>(), ref couponMoeny, ref daiLiCount, ref YongJinCount, storeFullCut, buyerRemark, invoice, bestTime, ip, cashId, new List<CashCouponInfo>(), new List<ExChangeCouponsInfo>());
            #endregion

            Orders.PayOrder(orderInfo.Oid, OrderState.Confirmed, "线下刷卡支付", DateTime.Now);
            OrderActions.CreateOrderAction(new OrderActionInfo()
            {
                Oid = orderInfo.Oid,
                Uid = orderInfo.Uid,
                RealName = "系统",
                ActionType = (int)OrderActionType.Pay,
                ActionTime = DateTime.Now,//交易时间,
                ActionDes = "您使用银行汇款支付订单成功，交易号为:线下刷卡支付"
            });
            Orders.UpdateOrderState(orderInfo.Oid, OrderState.Confirmed);

            Orders.ConfirmReceiving(orderInfo.Oid, OrderState.Completed, DateTime.Now);//将订单状态设为完成状态,并更新收货时间
            OrderActions.CreateOrderAction(new OrderActionInfo()
            {
                Oid = orderInfo.Oid,
                Uid = orderInfo.Uid,
                RealName = "系统",
                ActionType = (int)OrderActionType.Complete,
                ActionTime = DateTime.Now,//交易时间,
                ActionDes = "您的订单已经完成。发货模式为后发货模式，发货产品已最终发货产品为准。"
            });
            #endregion
            
            AddMallAdminLog("报单审核成功", "报单审核,报单会员为:" + username + ",订单号：" + orderInfo.OSN + ",推荐人：" + member.ParentCode + ",安置人：" + member.ManagerCode + ",直销会员ID：" + member.userId);

            info.state = 2;
            info.operateuid = WorkContext.Uid;
            info.detaildesc = "审核成功，报单成功";
            info.resultoid = orderInfo.Oid;
            info.resultosn = orderInfo.OSN;
            info.lastmodified = DateTime.Now;
            OrderBLL.Update(info);
            return PromptView("报单审核成功");
        }

        public ActionResult CancelApply(int id = -1, string detaildesc = "")
        {
            OrderApplyInfo info = OrderBLL.GetModel(id);
            if (info == null)
                return PromptView("申请不存在");
            ApplyStateForError(info, detaildesc);
            //info.state = 0;
            //info.operateuid = WorkContext.Uid;
            //info.detaildesc = "审核不通过，报单失败，原因:" + detaildesc;
            //info.lastmodified = DateTime.Now;
            //OrderBLL.Update(info);
            return PromptView("审核不通过");
        }

        public void ApplyStateForError(OrderApplyInfo info, string detaildesc)
        {
            info.state = 0;
            info.operateuid = WorkContext.Uid;
            info.detaildesc = "审核不通过，报单失败，原因:" + detaildesc;
            info.lastmodified = DateTime.Now;
            OrderBLL.Update(info);
        }
        #endregion

        #region 有机胚芽后台报单
        #endregion

        private void CreateOrderAction(int oid, OrderActionType orderActionType, string actionDes)
        {

        }
    }
}
