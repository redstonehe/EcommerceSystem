using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using VMall.Core;
using System.Linq;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.MallAdmin.Models;
using System.Text;
using System.Data;
using Newtonsoft.Json;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台首页控制器类
    /// </summary>
    public partial class HomeController : BaseMallAdminController
    {
        /// <summary>
        /// 首页
        /// </summary>
        public ActionResult Index()
        {
            return View("indexNew");
        }
        /// <summary>
        /// 首页
        /// </summary>
        public ActionResult IndexNew()
        {
            return View();
        }
        /// <summary>
        /// 导航栏
        /// </summary>
        public ActionResult NavBar()
        {
            return View();
        }

        /// <summary>
        /// 菜单栏
        /// </summary>
        public ActionResult Menu()
        {
            return View();
        }

        /// <summary>
        /// 菜单栏
        /// </summary>
        public ActionResult RightView()
        {
            return View();
        }

        /// <summary>
        /// 商城运行信息
        /// </summary>
        public ActionResult MallRunInfo()
        {
            MallRunInfoModel model = new MallRunInfoModel();

            //model.WaitConfirmCount = AdminOrders.GetOrderCountByCondition(0, (int)OrderState.Confirming, "", "");
            //model.WaitPreProductCount = AdminOrders.GetOrderCountByCondition(0, (int)OrderState.Confirmed, "", "");
            //model.WaitSendCount = AdminOrders.GetOrderCountByCondition(0, (int)OrderState.PreProducting, "", "");
            //model.WaitPayCount = AdminOrders.GetOrderCountByCondition(0, (int)OrderState.WaitPaying, "", "");
            model.AllOrderToday = AdminOrders.GetOrderListCoutByWhere(string.Format(" addtime>='{0}' and addtime<'{1}' and mallsource={2} ", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day), (int)MallSourceType.自营商城));

            model.AllUsersCount = AdminUsers.AdminGetUserCount("", "");
            model.AllProductCount = AdminProducts.AdminGetProductCount(" state=0 ");
            model.AllInformCount = new Complain().GetCount(" state=0 ");

            //model.ValidOrderToday = AdminOrders.GetOrderListCoutByWhere(string.Format(" addtime>='{0}' and addtime<'{1}' and orderstate>=70 and orderstate<=140 and mallsource={2} ", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day),(int)MallSourceType.自营商城));
            //model.ValidOrderAmountToday = AdminOrders.GetOrderListByWhere(string.Format(" addtime>='{0}' and addtime<'{1}' and orderstate>=70 and orderstate<=140 and mallsource={2} ", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day), (int)MallSourceType.自营商城)).Sum(x => x.OrderAmount);
            //model.ValidOrderSurplusMoneyToday = AdminOrders.GetOrderListByWhere(string.Format(" addtime>='{0}' and addtime<'{1}' and orderstate>=70 and orderstate<=140 and mallsource={2} ", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day), (int)MallSourceType.自营商城)).Sum(x => x.SurplusMoney);

            //string condition = AdminOrderRefunds.GetOrderRefundListCondition(0, "", 0, "");
            //string condition2 = OrderChange.GetOrderChangeListCondition(0, "", 0);
            //int c2 = AdminOrderRefunds.GetOrderRefundCount(condition);
            //int c1 = OrderChange.GetOrderChangeCount(condition2);

            //model.OnlineUserCount = OnlineUsers.GetOnlineUserCount();
            //model.OnlineGuestCount = OnlineUsers.GetOnlineGuestCount();
            //model.OnlineMemberCount = model.OnlineUserCount - model.OnlineGuestCount;

            //model.MallVersion = BMAVersion.MALL_VERSION;
            //model.NetVersion = Environment.Version.ToString();
            //model.OSVersion = Environment.OSVersion.ToString();
            //model.TickCount = (Environment.TickCount / 1000 / 60).ToString();
            //model.ProcessorCount = Environment.ProcessorCount.ToString();
            //model.WorkingSet = (Environment.WorkingSet / 1024 / 1024).ToString();

            MallUtils.SetAdminRefererCookie(Url.Action("mallruninfo"));
            return View(model);
        }

        public ActionResult OrderChart()
        {
            //string s = "[{ \"period\": \"2012-10-01\", \"licensed\": 20 },  { \"period\": \"2012-09-30\", \"licensed\": 3351 },  { \"period\": \"2012-09-29\", \"licensed\": 3269 },  { \"period\": \"2012-09-20\", \"licensed\": 324 },  {\"period\": \"2012-09-19\", \"licensed\": 3257 },  { \"period\": \"2012-09-18\", \"licensed\": 3248 },  { \"period\": \"2012-09-17\", \"licensed\": 3171 },  { \"period\": \"2012-09-16\", \"licensed\": 3171 }]";
            int period = 15;
            DateTime starttime = DateTime.Now.AddDays(-period);
            DateTime endtime = DateTime.Now;
            string timeStr = string.Empty;
            DataTable model = Orders.GetOrderCountStat(starttime, endtime);
            // 把DataTable转换为IList<>  
            List<HomeOrderStatModel> list = ModelConvertHelper<HomeOrderStatModel>.ConvertToModel(model);
            for (int i = 1; i <= period; i++)
            {
                timeStr = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd");
                if (!list.Exists(x => x.time == timeStr))
                    list.Add(new HomeOrderStatModel() { time = timeStr, count = 0 });
            }
            string JsonString = string.Empty;
            JsonString = JsonConvert.SerializeObject(list);
            return Content(JsonString);
        }
        public ActionResult OrderAmountChart()
        {
            int period = 15;
            DateTime starttime = DateTime.Now.AddDays(-period);
            DateTime endtime = DateTime.Now;
            string timeStr = string.Empty;
            DataTable model = Orders.GetOrderAmountStat(starttime, endtime);
            // 把DataTable转换为IList<>  
            List<HomeOrderAmountModel> list = ModelConvertHelper<HomeOrderAmountModel>.ConvertToModel(model);
            for (int i = 1; i <= period; i++)
            {
                timeStr = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd");
                if (!list.Exists(x => x.time == timeStr))
                    list.Add(new HomeOrderAmountModel() { time = timeStr, amount = 0.00M });
            }
            string JsonString = string.Empty;
            JsonString = JsonConvert.SerializeObject(list);
            return Content(JsonString);
        }
        public ActionResult AllAction()
        {
            List<MallAdminActionInfo> list = MallAdminActions.GetMallAdminActionList().FindAll(x => x.ParentId != 0);
            string s = string.Join(",", list.Select(x => x.Action).ToArray());
            return Content(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Tools()
        {
            return View();
        }
        /// <summary>
        ///  解密密码字符串
        /// </summary>
        /// <returns></returns>
        public ActionResult JieMi(string password)
        {
            string DecryptPWD = SecureHelper.DecryptString(password, DirSaleUserInfo.EncryptKey);
            return Content(DecryptPWD);
        }

        /// <summary>
        ///  后台获取密码根据UID
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminGetPWDByUid(int uid)
        {
            PartUserInfo user = Users.GetPartUserById(uid);
            if (user == null)
                return Content("");
            string DecryptPWD = SecureHelper.DecryptString(user.DirSalePwd, DirSaleUserInfo.EncryptKey);
            return Content(DecryptPWD);
        }
        /// <summary>
        ///  后台获取密码根据UID
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminGetPWDByMobile(string mobile)
        {
            if (!ValidateHelper.IsMobile(mobile))
            {
                return Content("");
            }
            PartUserInfo user = Users.GetPartUserByMobile(mobile);
            if (user == null)
                return Content("");
            string DecryptPWD = SecureHelper.DecryptString(user.DirSalePwd, DirSaleUserInfo.EncryptKey);
            return Content(DecryptPWD);
        }
        /// <summary>
        ///  后台获取支付密码根据UID
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminGetPaypwdByUid(int uid)
        {
            PartUserInfo user = Users.GetPartUserById(uid);
            if (user == null)
                return Content("");
            string FromDirSale = "";
            if (user.IsDirSaleUser)
                FromDirSale = OrderUtils.GetPayPassword("", user.DirSaleUid);
            else
                FromDirSale = user.PayPassword;
            if (string.IsNullOrEmpty(FromDirSale))
                return Content("");
            string psd = SecureHelper.DecryptString(FromDirSale, DirSaleUserInfo.EncryptKey);
            return Content(psd);
        }

        //CreateEncryptPwd
        /// <summary>
        ///  后台生成密码
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateEncryptPwd(string pwd)
        {
            if (string.IsNullOrEmpty(pwd))
                return Content("");
            string psd = SecureHelper.EncryptString(pwd, DirSaleUserInfo.EncryptKey);
            return Content(psd);
        }

        /// <summary>
        ///  清楚指定缓存
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearCache(string key)
        {
            VMall.Core.BMACache.Remove(key);

            return Content("清除缓存：" + key + "  成功");
        }

        public ActionResult AgentUser()
        {
            return View();
        }

        /// <summary>
        /// 增加线下代理会员，生成订单生成库存 并减少上级库存
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAgentAndOrder()
        {
            string username = WebHelper.GetFormString("username");
            string mobile = WebHelper.GetFormString("mobile");
            string realname = WebHelper.GetFormString("realname");
            string idcard = WebHelper.GetFormString("idcard");
            int rank = TypeHelper.StringToInt(WebHelper.GetFormString("rank"));
            string pname = WebHelper.GetFormString("pname");
            string bankname = WebHelper.GetFormString("bankname");
            string bankno = WebHelper.GetFormString("bankno");
            string bankusername = WebHelper.GetFormString("bankusername");
            int pid = TypeHelper.StringToInt(WebHelper.GetFormString("pid"));
            decimal amount = TypeHelper.StringToDecimal(WebHelper.GetFormString("amount"));

            int regionid = TypeHelper.StringToInt(WebHelper.GetFormString("regionId"));
            string address = WebHelper.GetFormString("address");

            int issend = TypeHelper.StringToInt(WebHelper.GetFormString("issend"));
            string accountName = mobile;

            StringBuilder errorList = new StringBuilder("[");
            //账号验证

            if (string.IsNullOrWhiteSpace(accountName) && string.IsNullOrWhiteSpace(username))
                return Content("账户名不能为空");
            if (!string.IsNullOrEmpty(accountName) && !ValidateHelper.IsMobile(accountName))
                return Content("手机号格式不正确");

            UserInfo userInfo = null;

            if (errorList.Length == 1)
            {
                if (string.IsNullOrEmpty(accountName))
                {
                    if (Users.IsExistUserName(username) || AccountUtils.IsUserExistForDirSale(username))
                        return Content("用户名已经存在");
                    else
                        userInfo = new UserInfo() { UserName = username, Email = string.Empty, Mobile = string.Empty };
                }
                if (!string.IsNullOrEmpty(accountName) && ValidateHelper.IsMobile(accountName))
                {
                    if (Users.IsExistMobile(accountName) || AccountUtils.IsUserExistForDirSale(accountName))
                        return Content("手机号已经存在");
                    else
                        userInfo = new UserInfo() { UserName = username, Email = string.Empty, Mobile = accountName };
                }
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
            if (nickName.Length > 0)
                userInfo.NickName = WebHelper.HtmlEncode(nickName);
            else
            {
                if (!string.IsNullOrEmpty(mobile))
                {
                    userInfo.NickName = accountName.Substring(0, 3) + "***" + accountName.Substring(7, 4);
                }
                else { userInfo.NickName = username; }
            }

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
            userInfo.BankName = bankname;
            userInfo.BankCardCode = bankno;
            userInfo.BankUserName = bankusername;

            userInfo.UserSource = 1;
            userInfo.AgentType = rank;
            //userInfo.Ptype = 2;
            //PartUserInfo parentUser = Users.GetPartUserByName(mobile);
            //if (parentUser==null)
            //    parentUser = Users.GetPartUserByMobile(mobile);
            //if (parentUser==null)
            //    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "推荐人不存在", "}");
            //userInfo.Pid = 0;

            #endregion

            #region 处理用户注册推荐人关系
            string parentname = WebHelper.GetQueryString("pname");
            if (string.IsNullOrEmpty(parentname.Trim()))
                parentname = WebHelper.GetFormString("pname");
            if (string.IsNullOrEmpty(parentname.Trim()))
                return Content("推荐人为空，请输入推荐人!");
            int pType;
            int uIdByPname = AccountUtils.GetUidByPname(parentname.Trim(), out pType);
            if (string.IsNullOrEmpty(parentname.Trim()) || uIdByPname < 1)//推荐人为空或推荐人在后台不存在
                return Content("推荐人不正确，请输入正确的推荐人");
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
                return Content("创建用户失败,请联系管理员");


            //创建订单
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

            decimal productPiscount = 0;
            if (rank == 1)
                productPiscount = 0.65M;
            else if (rank == 2)
                productPiscount = 0.55M;
            else if (rank == 3)
                productPiscount = 0.47M;
            else if (rank == 4)
                productPiscount = 0.42M;
            else
                productPiscount = 1;

            mainOrderProductInfo.DiscountPrice = mainOrderProductInfo.DiscountPrice * productPiscount;

            int buyCount = (int)(amount / mainOrderProductInfo.DiscountPrice);

            mainOrderProductInfo.RealCount = buyCount;
            mainOrderProductInfo.BuyCount = buyCount;
            //将商品添加到"需要添加的商品列表"中
            addOrderProductList.Add(mainOrderProductInfo);

            Carts.ClearCart(userInfo.Uid, "");

            //将需要添加的商品持久化
            Carts.AddOrderProductList(addOrderProductList);

            PartUserInfo orderUser = Users.GetPartUserById(userInfo.Uid);
            StoreInfo storeInfo = Stores.GetStoreById(WebHelper.GetConfigSettingsInt("AgentStoreId"));

            FullShipAddressInfo shipAddressInfo = new FullShipAddressInfo();
            //ShipAddressInfo shipAddressInfo = new ShipAddressInfo();
            shipAddressInfo.Uid = userInfo.Uid;
            shipAddressInfo.RegionId = regionid;
            shipAddressInfo.IsDefault = 1;
            shipAddressInfo.Alias = "地址1";
            shipAddressInfo.Consignee = realname;
            shipAddressInfo.Mobile = mobile;
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
                ActionDes = "您的订单已经完成。"
            });

            List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);

            bool isChangeSelfStock = true;
            //获得有效代理上级用户
            PartUserInfo parentUser = Users.GetParentUserForAgent(orderUser);
            //该订单的代理库存处理
            foreach (var item in orderProductList)
            {
                //找上级的对应产品的代理库存，找不到说明库存为0  公司拿货，库存小于购买，不足部分从公司补，并记录库存加减详情
                new AgentStock().UpdateAgentStockForOrder(parentUser, orderUser, orderInfo, item, isChangeSelfStock);
            }
            AgentStockInfo Stock = new AgentStock().GetModel(string.Format(" uid={0} and pid={1} ", userInfo.Uid, pid));
            if (Stock != null)
            {
                Stock.Balance = AgentStock.CutDecimalWithN(amount / mainOrderProductInfo.DiscountPrice, 4);
                Stock.AgentAmount = amount;
                new AgentStock().Update(Stock);
            }
            Orders.UpdateOrderForAgentAdmin(orderInfo.Oid, amount);

            AgentStockDetailInfo detail = new AgentStockDetail().GetModel(string.Format(" uid={0} AND pid={1} AND ordercode='{2}' AND detailtype=1  ", userInfo.Uid, pid, orderInfo.OSN));
            if (detail != null)
            {
                detail.InAmount = amount;
                detail.CurrentBalance = amount;
                new AgentStockDetail().Update(detail);
            }
            if (issend == 1)
            {
                if (Stock != null)
                {
                    Stock.Balance = 0;
                    Stock.AgentAmount = 0;
                    new AgentStock().Update(Stock);
                }
                new AgentStockDetail().AddDetail(orderInfo.Uid, pid, 2, 0, amount, 0, orderInfo.OSN, string.Format("公司发货，产品：{0},金额：{1}", partProductInfo.Name, amount), 0, orderInfo.Uid);
            }
            return Content("增加会员ID：" + userInfo.Uid + ",订单ID：" + orderInfo.Oid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AgentOrder()
        {
            return View();
        }
        /// <summary>
        /// 增加代理生成订单生成库存 并减少上级库存
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateAgentOrder()
        {
            string username = WebHelper.GetFormString("username");
            string mobile = WebHelper.GetFormString("mobile");
            string realname = WebHelper.GetFormString("realname");
            string idcard = WebHelper.GetFormString("idcard");
            int rank = TypeHelper.StringToInt(WebHelper.GetFormString("rank"));
            string pname = WebHelper.GetFormString("pname");
            string bankname = WebHelper.GetFormString("bankname");
            string bankno = WebHelper.GetFormString("bankno");
            string bankusername = WebHelper.GetFormString("bankusername");
            int pid = TypeHelper.StringToInt(WebHelper.GetFormString("pid"));
            decimal amount = TypeHelper.StringToDecimal(WebHelper.GetFormString("amount"));


            int regionid = TypeHelper.StringToInt(WebHelper.GetFormString("regionId"));
            string address = WebHelper.GetFormString("address");

            int issend = TypeHelper.StringToInt(WebHelper.GetFormString("issend"));
            string accountName = mobile;

            //账号验证
            //if (string.IsNullOrWhiteSpace(accountName))
            //    return Content("账户名不能为空");


            PartUserInfo userInfo = null;
            if (username != "")
                userInfo = Users.GetPartUserByName(username);
            if (mobile != "" && ValidateHelper.IsMobile(mobile))
                userInfo = Users.GetPartUserByMobile(mobile);

            if (userInfo == null)
                return Content("账户不正确");


            //创建订单
            PartProductInfo partProductInfo = Products.GetPartProductById(pid);

            //将商品添加到购物车
            Carts.ClearCart(userInfo.Uid, "");

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

            decimal productPiscount = 0;
            if (rank == 1)
                productPiscount = 0.65M;
            else if (rank == 2)
                productPiscount = 0.55M;
            else if (rank == 3)
                productPiscount = 0.47M;
            else if (rank == 4)
                productPiscount = 0.42M;
            else
                productPiscount = 1;

            mainOrderProductInfo.DiscountPrice = mainOrderProductInfo.DiscountPrice * productPiscount;

            int buyCount = (int)(amount / mainOrderProductInfo.DiscountPrice);

            mainOrderProductInfo.RealCount = buyCount;
            mainOrderProductInfo.BuyCount = buyCount;
            //将商品添加到"需要添加的商品列表"中
            addOrderProductList.Add(mainOrderProductInfo);



            //将需要添加的商品持久化
            Carts.AddOrderProductList(addOrderProductList);

            PartUserInfo orderUser = Users.GetPartUserById(userInfo.Uid);
            StoreInfo storeInfo = Stores.GetStoreById(WebHelper.GetConfigSettingsInt("AgentStoreId"));

            FullShipAddressInfo shipAddressInfo = new FullShipAddressInfo();
            //ShipAddressInfo shipAddressInfo = new ShipAddressInfo();
            shipAddressInfo.Uid = userInfo.Uid;
            shipAddressInfo.RegionId = regionid;
            shipAddressInfo.IsDefault = 1;
            shipAddressInfo.Alias = "地址1";
            shipAddressInfo.Consignee = realname;
            shipAddressInfo.Mobile = mobile;
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
                ActionDes = "您的订单已经完成。"
            });

            List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);

            bool isChangeSelfStock = true;
            //获得有效代理上级用户
            PartUserInfo parentUser = Users.GetParentUserForAgentStock(orderUser);
            //该订单的代理库存处理
            foreach (var item in orderProductList)
            {
                //找上级的对应产品的代理库存，找不到说明库存为0  公司拿货，库存小于购买，不足部分从公司补，并记录库存加减详情
                new AgentStock().UpdateAgentStockForOrder(parentUser, orderUser, orderInfo, item, isChangeSelfStock);
            }
            AgentStockInfo Stock = new AgentStock().GetModel(string.Format(" uid={0} and pid={1} ", userInfo.Uid, pid));
            if (Stock != null)
            {
                Stock.Balance = AgentStock.CutDecimalWithN(amount / mainOrderProductInfo.DiscountPrice, 4);
                Stock.AgentAmount = amount;
                new AgentStock().Update(Stock);
            }
            Orders.UpdateOrderForAgentAdmin(orderInfo.Oid, amount);

            AgentStockDetailInfo detail = new AgentStockDetail().GetModel(string.Format(" uid={0} AND pid={1} AND ordercode='{2}' AND detailtype=1  ", userInfo.Uid, pid, orderInfo.OSN));
            if (detail != null)
            {
                detail.InAmount = amount;
                detail.CurrentBalance = amount;
                new AgentStockDetail().Update(detail);
            }
            if (issend == 1)
            {
                if (Stock != null)
                {
                    Stock.Balance = 0;
                    Stock.AgentAmount = 0;
                    new AgentStock().Update(Stock);
                }
                new AgentStockDetail().AddDetail(orderInfo.Uid, pid, 2, 0, amount, 0, orderInfo.OSN, string.Format("公司发货，产品：{0},金额：{1}", partProductInfo.Name, amount), 0, orderInfo.Uid);
            }

            return Content("增加会员ID：" + userInfo.Uid + ",订单ID：" + orderInfo.Oid);
        }

        /// <summary>
        /// 增加1980代理生成订单并安置直销点位
        /// </summary>
        /// <returns></returns>
        public ActionResult AgentUserFor1980()
        {
            return View();
        }

        /// <summary>
        /// 增加1980代理生成订单并安置直销点位
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAgentAndOrderFor1980()
        {
            string username = WebHelper.GetFormString("username");
            string mobile = WebHelper.GetFormString("mobile");
            string realname = WebHelper.GetFormString("realname");
            string idcard = WebHelper.GetFormString("idcard");
            int rank = TypeHelper.StringToInt(WebHelper.GetFormString("rank"));
            string pname = WebHelper.GetFormString("pname");
            string managerCode = WebHelper.GetFormString("managerCode");
            int placeSide = WebHelper.GetFormInt("placeSide");
            string bankname = WebHelper.GetFormString("bankname");
            string bankno = WebHelper.GetFormString("bankno");
            string bankusername = WebHelper.GetFormString("bankusername");
            int pid = TypeHelper.StringToInt(WebHelper.GetFormString("pid"));
            decimal amount = TypeHelper.StringToDecimal(WebHelper.GetFormString("amount"));

            int regionid = TypeHelper.StringToInt(WebHelper.GetFormString("regionId"));
            string address = WebHelper.GetFormString("address");

            //int issend = TypeHelper.StringToInt(WebHelper.GetFormString("issend"));
            string accountName = mobile;

            StringBuilder errorList = new StringBuilder("[");
            //账号验证

            if (string.IsNullOrWhiteSpace(accountName) && string.IsNullOrWhiteSpace(username))
                return Content("账户名不能为空");
            if (!string.IsNullOrEmpty(accountName) && !ValidateHelper.IsMobile(accountName))
                return Content("手机号格式不正确");

            UserInfo userInfo = null;
            int uid = 0;
            if (!string.IsNullOrEmpty(mobile))
                uid = Users.GetUidByMobile(mobile);
            if (uid <= 0)
            {
                if (errorList.Length == 1)
                {
                    if (string.IsNullOrEmpty(accountName))
                    {
                        if (Users.IsExistUserName(username) || AccountUtils.IsUserExistForDirSale(username))
                            return Content("用户名已经存在");
                        else
                            userInfo = new UserInfo() { UserName = username, Email = string.Empty, Mobile = string.Empty };
                    }
                    if (!string.IsNullOrEmpty(accountName) && ValidateHelper.IsMobile(accountName))
                    {
                        if (Users.IsExistMobile(accountName) || AccountUtils.IsUserExistForDirSale(accountName))
                            return Content("手机号已经存在");
                        else
                            userInfo = new UserInfo() { UserName = username, Email = string.Empty, Mobile = accountName };
                    }
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
                if (nickName.Length > 0)
                    userInfo.NickName = WebHelper.HtmlEncode(nickName);
                else
                {
                    if (!string.IsNullOrEmpty(mobile))
                    {
                        userInfo.NickName = accountName.Substring(0, 3) + "***" + accountName.Substring(7, 4);
                    }
                    else { userInfo.NickName = username; }
                }

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
                userInfo.BankName = bankname;
                userInfo.BankCardCode = bankno;
                userInfo.BankUserName = bankusername;

                userInfo.UserSource = 1;
                userInfo.AgentType = rank;

                //userInfo.Ptype = 2;
                //PartUserInfo parentUser = Users.GetPartUserByName(mobile);
                //if (parentUser==null)
                //    parentUser = Users.GetPartUserByMobile(mobile);
                //if (parentUser==null)
                //    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "推荐人不存在", "}");
                //userInfo.Pid = 0;

                #endregion

                #region 处理用户注册推荐人关系
                string parentname = WebHelper.GetQueryString("pname");
                if (string.IsNullOrEmpty(parentname.Trim()))
                    parentname = WebHelper.GetFormString("pname");
                if (string.IsNullOrEmpty(parentname.Trim()))
                    return Content("推荐人为空，请输入推荐人!");
                int pType;
                int uIdByPname = AccountUtils.GetUidByPname(parentname.Trim(), out pType);
                if (string.IsNullOrEmpty(parentname.Trim()) || uIdByPname < 1)//推荐人为空或推荐人在后台不存在
                    return Content("推荐人不正确，请输入正确的推荐人");
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
                    return Content("创建用户失败,请联系管理员");
            }
            else
            {
                userInfo = Users.GetUserById(uid);
                userInfo.DirSalePwd = Users.GetPartUserById(uid).DirSalePwd;
            }

            #region

            //创建订单
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

            decimal productPiscount = 1;
            mainOrderProductInfo.DiscountPrice = mainOrderProductInfo.DiscountPrice * productPiscount;

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
            //ShipAddressInfo shipAddressInfo = new ShipAddressInfo();
            shipAddressInfo.Uid = userInfo.Uid;
            shipAddressInfo.RegionId = regionid;
            shipAddressInfo.IsDefault = 1;
            shipAddressInfo.Alias = "地址1";
            shipAddressInfo.Consignee = realname;
            shipAddressInfo.Mobile = mobile;
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

            MemberInfo member = new MemberInfo();
            if (!userInfo.IsDirSaleUser)
                member = AccountUtils.CreateMember(userInfo, realname, managerCode, placeSide, idcard, mobile);
            Users.UpdateDSUserAGTypeByUid(uid, rank);

            return Content("增加会员ID：" + userInfo.Uid + ",订单ID：" + orderInfo.Oid + ",推荐人：" + member.ParentCode + ",安置人：" + member.ManagerCode + ",直销会员ID：" + member.userId);
        }

        /// <summary>
        /// 增加线下代理会员，系统只有会员关系，没有订单、库存以及结算关系s
        /// </summary>
        /// <returns></returns>
        public ActionResult OutLineAgent()
        {
            return View();
        }

        /// <summary>
        /// 增加线下代理会员，系统只有会员关系，没有订单、库存以及结算关系
        /// </summary>
        /// <returns></returns>
        public ActionResult AddOutLineAgent()
        {
            string username = WebHelper.GetFormString("username");
            string mobile = WebHelper.GetFormString("mobile");
            string realname = WebHelper.GetFormString("realname");
            string idcard = WebHelper.GetFormString("idcard");
            int rank = TypeHelper.StringToInt(WebHelper.GetFormString("rank"));
            string pname = WebHelper.GetFormString("pname");
            string bankname = WebHelper.GetFormString("bankname");
            string bankno = WebHelper.GetFormString("bankno");
            string bankusername = WebHelper.GetFormString("bankusername");

            string accountName = mobile;

            StringBuilder errorList = new StringBuilder("[");
            //账号验证

            if (string.IsNullOrWhiteSpace(accountName) && string.IsNullOrWhiteSpace(username))
                return Content("账户名不能为空");
            if (!string.IsNullOrEmpty(accountName) && !ValidateHelper.IsMobile(accountName))
                return Content("手机号格式不正确");

            UserInfo userInfo = null;

            if (errorList.Length == 1)
            {
                if (string.IsNullOrEmpty(accountName))
                {
                    if (Users.IsExistUserName(username) || AccountUtils.IsUserExistForDirSale(username))
                        return Content("用户名已经存在");
                    else
                        userInfo = new UserInfo() { UserName = username, Email = string.Empty, Mobile = string.Empty };
                }
                if (!string.IsNullOrEmpty(accountName) && ValidateHelper.IsMobile(accountName))
                {
                    if (Users.IsExistMobile(accountName) || AccountUtils.IsUserExistForDirSale(accountName))
                        return Content("手机号已经存在");
                    else
                        userInfo = new UserInfo() { UserName = username, Email = string.Empty, Mobile = accountName };
                }
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
            if (nickName.Length > 0)
                userInfo.NickName = WebHelper.HtmlEncode(nickName);
            else
            {
                if (!string.IsNullOrEmpty(mobile))
                {
                    userInfo.NickName = accountName.Substring(0, 3) + "***" + accountName.Substring(7, 4);
                }
                else { userInfo.NickName = username; }
            }

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
            userInfo.BankName = bankname;
            userInfo.BankCardCode = bankno;
            userInfo.BankUserName = bankusername;

            userInfo.UserSource = 1;
            userInfo.AgentType = rank;
            //userInfo.Ptype = 2;
            //PartUserInfo parentUser = Users.GetPartUserByName(mobile);
            //if (parentUser==null)
            //    parentUser = Users.GetPartUserByMobile(mobile);
            //if (parentUser==null)
            //    errorList.AppendFormat("{0}\"key\":\"{1}\",\"msg\":\"{2}\"{3},", "{", "accountName", "推荐人不存在", "}");
            //userInfo.Pid = 0;

            #endregion

            #region 处理用户注册推荐人关系
            string parentname = WebHelper.GetQueryString("pname");
            if (string.IsNullOrEmpty(parentname.Trim()))
                parentname = WebHelper.GetFormString("pname");
            if (string.IsNullOrEmpty(parentname.Trim()))
                return Content("推荐人为空，请输入推荐人!");
            int pType;
            int uIdByPname = AccountUtils.GetUidByPname(parentname.Trim(), out pType);
            if (string.IsNullOrEmpty(parentname.Trim()) || uIdByPname < 1)//推荐人为空或推荐人在后台不存在
                return Content("推荐人不正确，请输入正确的推荐人");
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
                return Content("创建用户失败,请联系管理员");

            return Content("增加会员ID：" + userInfo.Uid);
        }


        /// <summary>
        /// 订货系统报单选择
        /// </summary>
        /// <returns></returns>
        public ActionResult AgentOrderSelect()
        {
            return View();
        }

        /// <summary>
        /// 增加微商订货系统报单订单(已存在会员)
        /// </summary>
        /// <returns></returns>
        public ActionResult AgentOrderForAgentSystem()
        {
            return View();
        }
        /// <summary>
        /// 增加微商订货系统报单订单(已存在会员)
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateAgentOrderForAgentSystem()
        {
            string usercode = WebHelper.GetFormString("usercode");
            int pid = TypeHelper.StringToInt(WebHelper.GetFormString("pid"));
            int price = TypeHelper.StringToInt(WebHelper.GetFormString("price"));
            int pCount = TypeHelper.StringToInt(WebHelper.GetFormString("pCount"));
            int regionid = TypeHelper.StringToInt(WebHelper.GetFormString("regionId"));
            string address = WebHelper.GetFormString("address");
            string consignmobile = WebHelper.GetFormString("consignmobile");
            string consignee = WebHelper.GetFormString("consignee");
            int ordertype = 1;
            decimal amount = price * pCount;

            if (usercode == "" || pid <= 0 || pCount <= 0 || regionid <= 0 || address == "" || consignmobile == "" || consignee == "")
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"缺少必填参数\"}]", true);
            PartUserInfo userInfo = null;
            if (usercode != "")
                userInfo = Users.GetPartUserByName(usercode);
            if (usercode != "" && ValidateHelper.IsMobile(usercode))
                userInfo = Users.GetPartUserByMobile(usercode);

            if (userInfo == null)
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"会员无效\"}]", true);

            //创建订单
            PartProductInfo partProductInfo = Products.GetPartProductById(pid);

            //将商品添加到购物车
            Carts.ClearCart(userInfo.Uid, "");

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

            if (price == 3)
            {
                mainOrderProductInfo.DiscountPrice = 70;
                partProductInfo.ShopPrice = 70;
            }
            else if (price == 2)
            {
                mainOrderProductInfo.DiscountPrice = 90;
                partProductInfo.ShopPrice = 90;
            }
            if (userInfo.AgentType <= 0)
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"该会员还不是代理会员，请先修改代理级别\"}]", true);
            if (userInfo.AgentType == 2 && price != 2)
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"会员级别与价格不符合\"}]", true);
            if ((userInfo.AgentType == 3 || userInfo.AgentType == 4) && price != 3)
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"会员级别与价格不符合\"}]", true);

            int buyCount = pCount;
            mainOrderProductInfo.RealCount = buyCount;
            mainOrderProductInfo.BuyCount = buyCount;
            //将商品添加到"需要添加的商品列表"中
            addOrderProductList.Add(mainOrderProductInfo);

            //将需要添加的商品持久化
            Carts.AddOrderProductList(addOrderProductList);

            PartUserInfo orderUser = Users.GetPartUserById(userInfo.Uid);
            StoreInfo storeInfo = Stores.GetStoreById(WebHelper.GetConfigSettingsInt("AgentSystemStoreId"));

            FullShipAddressInfo shipAddressInfo = new FullShipAddressInfo();
            //ShipAddressInfo shipAddressInfo = new ShipAddressInfo();
            shipAddressInfo.Uid = userInfo.Uid;
            shipAddressInfo.RegionId = regionid;
            shipAddressInfo.IsDefault = 1;
            shipAddressInfo.Alias = "地址1";
            shipAddressInfo.Consignee = consignee;
            shipAddressInfo.Mobile = consignmobile;
            shipAddressInfo.Phone = "";
            shipAddressInfo.Email = "";
            shipAddressInfo.ZipCode = "";
            shipAddressInfo.Address = WebHelper.HtmlEncode(address);

            string payName = "emsremit";
            PluginInfo payPluginInfo = Plugins.GetPayPluginBySystemName(payName);
            string buyerRemark = "";
            DateTime bestTime = DateTime.Now;

            PartUserInfo operateUserInfo = null;
            if (ordertype == 1)
                operateUserInfo = userInfo;
            else if (ordertype == 2)
                operateUserInfo = Users.GetParentUserByPidAndPtype(userInfo.AgentPid, userInfo.AgentPType);
            OrderInfo orderInfo = Orders.CreateOrder_Agent_ForAdmin(orderUser, storeInfo, partProductInfo, payPluginInfo, consignee, consignmobile, regionid, address, WorkContext.IP, pCount, pid, buyerRemark, "", ordertype, operateUserInfo);

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

            Orders.ConfirmOrder(orderInfo);

            //Orders.UpdateOrderState(orderInfo.Oid, OrderState.Confirmed);
            //Orders.ConfirmReceiving(orderInfo.Oid, OrderState.Completed, DateTime.Now);//将订单状态设为完成状态,并更新收货时间
            //OrderActions.CreateOrderAction(new OrderActionInfo()
            //{
            //    Oid = orderInfo.Oid,
            //    Uid = orderInfo.Uid,
            //    RealName = "系统",
            //    ActionType = (int)OrderActionType.Complete,
            //    ActionTime = DateTime.Now,//交易时间,
            //    ActionDes = "您的订单已经完成。"
            //});

            return AjaxResult("success", "提交成功");
        }

        /// <summary>
        /// 增加微商订货系统报单订单(不存在会员)
        /// </summary>
        /// <returns></returns>
        public ActionResult AgentOrderForAgentSystem_Nouser()
        {
            return View();
        }
        /// <summary>
        /// 增加微商订货系统报单订单(不存在会员)
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateAgentOrderForAgentSystem_Nouser()
        {
            string usercode = WebHelper.GetFormString("usercode");
            string realname = WebHelper.GetFormString("realname");
            string idcard = WebHelper.GetFormString("idcard");
            string parentusercode = WebHelper.GetFormString("parentusercode");
            int rank = TypeHelper.StringToInt(WebHelper.GetFormString("rank"));
            int pid = TypeHelper.StringToInt(WebHelper.GetFormString("pid"));
            int pCount = TypeHelper.StringToInt(WebHelper.GetFormString("pCount"));
            int regionid = TypeHelper.StringToInt(WebHelper.GetFormString("regionId"));
            string address = WebHelper.GetFormString("address");
            string consignmobile = WebHelper.GetFormString("consignmobile");
            string consignee = WebHelper.GetFormString("consignee");
            int ordertype = 1;
            decimal amount = rank * pCount;

            if (usercode == "" || realname == "" || idcard == "" || parentusercode == "" || pid <= 0 || pCount <= 0 || regionid <= 0 || address == "" || consignmobile == "" || consignee == "")
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"缺少必填参数\"}]", true);
            //PartUserInfo user = null;
            //if (usercode != "")
            //    user = Users.GetPartUserByName(usercode);
            //if (usercode != "" && ValidateHelper.IsMobile(usercode))
            //    user = Users.GetPartUserByMobile(usercode);
            //if (user != null)
            //    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"用户名或手机已存在，请使用已有会员报单功能\"}]", true);

            UserInfo userInfo = null;
            if (ValidateHelper.IsMobile(usercode))
            {
                if (Users.IsExistMobile(usercode) || AccountUtils.IsUserExistForDirSale(usercode))
                    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"手机号已存在，请使用已有会员报单功能\"}]", true);
                else
                    userInfo = new UserInfo() { Mobile = usercode };
            }
            else
            {
                if (Users.IsExistUserName(usercode) || AccountUtils.IsUserExistForDirSale(usercode))
                    return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"会员编号已存在，请使用已有会员报单功能或者生成新的会员编号\"}]", true);
                else
                    userInfo = new UserInfo() { UserName = usercode };
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

            if (ValidateHelper.IsMobile(usercode))
                userInfo.NickName = usercode.Substring(0, 3) + "***" + usercode.Substring(7, 4);
            else
                userInfo.NickName = usercode;

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

            userInfo.UserSource = 1;
            userInfo.AgentType = rank;

            #endregion

            #region 处理用户注册推荐人关系
            string parentname = parentusercode;
            if (string.IsNullOrEmpty(parentname.Trim()))
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"推荐人为空，请输入推荐人\"}]", true);
            int pType;
            int uIdByPname = AccountUtils.GetUidByPname(parentname.Trim(), out pType);
            if (string.IsNullOrEmpty(parentname.Trim()) || uIdByPname < 1)//推荐人为空或推荐人在后台不存在
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"推荐人有误或不存在，请输入正确的推荐人\"}]", true);

            else //推荐人正确
            {
                userInfo.Pid = uIdByPname;
                userInfo.Ptype = pType;//推荐人类型
            }
            #endregion

            //验证推荐人必须是代理会员
            PartUserInfo parentUser = Users.GetParentUserByPidAndPtype(userInfo.Pid, userInfo.Ptype);
            if (parentUser.AgentType <= 0)
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"推荐人还不是代理会员，不能推荐代理会员\"}]", true);
            //创建用户
            userInfo.Uid = Users.CreateUser(userInfo);
            //添加用户失败
            if (userInfo.Uid < 1)
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"创建用户失败,请联系管理员\"}]", true);

            //修改代理网络
            Users.UpdateUserAgentRecommer(userInfo.Uid, parentUser.Uid, 1);

            //创建订单
            PartProductInfo partProductInfo = Products.GetPartProductById(pid);

            //将商品添加到购物车
            Carts.ClearCart(userInfo.Uid, "");

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

            if (rank == 3 || rank == 4)
            {
                mainOrderProductInfo.DiscountPrice = 70;
                partProductInfo.ShopPrice = 70;
            }
            else if (rank == 2)
            {
                mainOrderProductInfo.DiscountPrice = 90;
                partProductInfo.ShopPrice = 90;
            }

            int buyCount = pCount;
            mainOrderProductInfo.RealCount = buyCount;
            mainOrderProductInfo.BuyCount = buyCount;
            //将商品添加到"需要添加的商品列表"中
            addOrderProductList.Add(mainOrderProductInfo);

            //将需要添加的商品持久化
            Carts.AddOrderProductList(addOrderProductList);

            PartUserInfo orderUser = Users.GetPartUserById(userInfo.Uid);
            StoreInfo storeInfo = Stores.GetStoreById(WebHelper.GetConfigSettingsInt("AgentSystemStoreId"));

            FullShipAddressInfo shipAddressInfo = new FullShipAddressInfo();
            //ShipAddressInfo shipAddressInfo = new ShipAddressInfo();
            shipAddressInfo.Uid = userInfo.Uid;
            shipAddressInfo.RegionId = regionid;
            shipAddressInfo.IsDefault = 1;
            shipAddressInfo.Alias = "地址1";
            shipAddressInfo.Consignee = consignee;
            shipAddressInfo.Mobile = consignmobile;
            shipAddressInfo.Phone = "";
            shipAddressInfo.Email = "";
            shipAddressInfo.ZipCode = "";
            shipAddressInfo.Address = WebHelper.HtmlEncode(address);

            string payName = "emsremit";
            PluginInfo payPluginInfo = Plugins.GetPayPluginBySystemName(payName);
            string buyerRemark = "";
            DateTime bestTime = DateTime.Now;

            PartUserInfo operateUserInfo = null;
            if (ordertype == 1)
                operateUserInfo = userInfo;
            else if (ordertype == 2)
                operateUserInfo = Users.GetParentUserByPidAndPtype(userInfo.AgentPid, userInfo.AgentPType);
            OrderInfo orderInfo = Orders.CreateOrder_Agent_ForAdmin(orderUser, storeInfo, partProductInfo, payPluginInfo, consignee, consignmobile, regionid, address, WorkContext.IP, pCount, pid, buyerRemark, "", ordertype, operateUserInfo);

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

            //Orders.UpdateOrderState(orderInfo.Oid, OrderState.Confirmed);
            Orders.ConfirmOrder(orderInfo);

            return AjaxResult("success", "提交成功");
        }


        /// <summary>
        /// 后台增加订单
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminAddOrderView()
        {
            return View();
        }
        /// <summary>
        /// 后台增加订单-捕手订单
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateAdminAddOrder()
        {
            string usercode = WebHelper.GetFormString("usercode");
            int pid = TypeHelper.StringToInt(WebHelper.GetFormString("pid"));
            //int price = TypeHelper.StringToInt(WebHelper.GetFormString("price"));
            int pCount = TypeHelper.StringToInt(WebHelper.GetFormString("pCount"));
            int regionid = TypeHelper.StringToInt(WebHelper.GetFormString("regionId"));
            string address = WebHelper.GetFormString("address");
            string consignmobile = WebHelper.GetFormString("consignmobile");
            string consignee = WebHelper.GetFormString("consignee");

            //decimal amount = price * pCount;

            if (usercode == "" || pid <= 0 || pCount <= 0 || regionid <= 0 || address == "" || consignmobile == "" || consignee == "")
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"缺少必填参数\"}]", true);
            PartUserInfo userInfo = null;
            if (usercode != "")
                userInfo = Users.GetPartUserByName(usercode);
            if (usercode != "" && ValidateHelper.IsMobile(usercode))
                userInfo = Users.GetPartUserByMobile(usercode);

            if (userInfo == null)
                return AjaxResult("error", "[{\"key\":\"error\",\"msg\":\"会员无效\"}]", true);

            //创建订单
            PartProductInfo partProductInfo = Products.GetPartProductById(pid);

            //将商品添加到购物车
            Carts.ClearCart(userInfo.Uid, "");

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
            mainOrderProductInfo.DiscountPrice = partProductInfo.ShopPrice;

            int buyCount = pCount;
            mainOrderProductInfo.RealCount = buyCount;
            mainOrderProductInfo.BuyCount = buyCount;
            //将商品添加到"需要添加的商品列表"中
            addOrderProductList.Add(mainOrderProductInfo);

            //将需要添加的商品持久化
            Carts.AddOrderProductList(addOrderProductList);

            PartUserInfo orderUser = Users.GetPartUserById(userInfo.Uid);
            StoreInfo storeInfo = Stores.GetStoreById(partProductInfo.StoreId);

            FullShipAddressInfo shipAddressInfo = new FullShipAddressInfo();
            //ShipAddressInfo shipAddressInfo = new ShipAddressInfo();
            shipAddressInfo.Uid = userInfo.Uid;
            shipAddressInfo.RegionId = regionid;
            shipAddressInfo.IsDefault = 1;
            shipAddressInfo.Alias = "地址1";
            shipAddressInfo.Consignee = consignee;
            shipAddressInfo.Mobile = consignmobile;
            shipAddressInfo.Phone = "";
            shipAddressInfo.Email = "";
            shipAddressInfo.ZipCode = "";
            shipAddressInfo.Address = WebHelper.HtmlEncode(address);

            string payName = "emsremit";
            PluginInfo payPluginInfo = Plugins.GetPayPluginBySystemName(payName);
            string buyerRemark = "";
            DateTime bestTime = DateTime.Now;

            PartUserInfo operateUserInfo = null;

            operateUserInfo = userInfo;

            OrderInfo orderInfo = Orders.CreateOrder_ForAdmin(orderUser, storeInfo, partProductInfo, payPluginInfo, consignee, consignmobile, regionid, address, WorkContext.IP, pCount, pid, buyerRemark, "");

            Orders.PayOrder(orderInfo.Oid, OrderState.Confirmed, "线下转账支付", DateTime.Now);
            OrderActions.CreateOrderAction(new OrderActionInfo()
            {
                Oid = orderInfo.Oid,
                Uid = orderInfo.Uid,
                RealName = "系统",
                ActionType = (int)OrderActionType.Pay,
                ActionTime = DateTime.Now,//交易时间,
                ActionDes = "您使用银行汇款支付订单成功，交易号为:线下转账支付"
            });

            Orders.ConfirmOrder(orderInfo);

            //Orders.UpdateOrderState(orderInfo.Oid, OrderState.Confirmed);
            //Orders.ConfirmReceiving(orderInfo.Oid, OrderState.Completed, DateTime.Now);//将订单状态设为完成状态,并更新收货时间
            //OrderActions.CreateOrderAction(new OrderActionInfo()
            //{
            //    Oid = orderInfo.Oid,
            //    Uid = orderInfo.Uid,
            //    RealName = "系统",
            //    ActionType = (int)OrderActionType.Complete,
            //    ActionTime = DateTime.Now,//交易时间,
            //    ActionDes = "您的订单已经完成。"
            //});

            return AjaxResult("success", "提交成功");
        }

    }
}
