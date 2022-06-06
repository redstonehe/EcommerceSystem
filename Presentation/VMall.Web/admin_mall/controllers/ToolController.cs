using System;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台工具控制器类
    /// </summary>
    public partial class ToolController : Controller
    {
        private string ip = "";//ip地址
        private MallConfigInfo mallConfigInfo = BMAConfig.MallConfig;//商城配置信息
        private PartUserInfo partUserInfo = null;//用户信息

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            ip = WebHelper.GetIP();
            //当用户ip不在允许的后台访问ip列表时
            if (!string.IsNullOrEmpty(mallConfigInfo.AdminAllowAccessIP) && !ValidateHelper.InIPList(ip, mallConfigInfo.AdminAllowAccessIP))
            {
                filterContext.Result = HttpNotFound();
                return;
            }
            //当用户IP被禁止时
            if (BannedIPs.CheckIP(ip))
            {
                filterContext.Result = HttpNotFound();
                return;
            }

            //获得用户id
            int uid = MallUtils.GetUidCookie();
            if (uid < 1)
                uid = WebHelper.GetRequestInt("uid");

            if (uid < 1)//当用户为游客时
            {
                //创建游客
                partUserInfo = Users.CreatePartGuest();
            }
            else//当用户为会员时
            {
                //获得保存在cookie中的密码
                string encryptPwd = MallUtils.GetCookiePassword();
                if (string.IsNullOrWhiteSpace(encryptPwd))
                    encryptPwd = WebHelper.GetRequestString("password");
                //防止用户密码被篡改为危险字符
                if (encryptPwd.Length == 0 || !SecureHelper.IsBase64String(encryptPwd))
                {
                    //创建游客
                    partUserInfo = Users.CreatePartGuest();
                    MallUtils.SetUidCookie(-1);
                    MallUtils.SetCookiePassword("");
                }
                else
                {
                    partUserInfo = Users.GetPartUserByUidAndPwd(uid, MallUtils.DecryptCookiePassword(encryptPwd));
                    if (partUserInfo == null)
                    {
                        partUserInfo = Users.CreatePartGuest();
                        MallUtils.SetUidCookie(-1);
                        MallUtils.SetCookiePassword("");
                    }
                }
            }

            //当用户等级是禁止访问等级时
            if (partUserInfo.UserRid == 1)
            {
                filterContext.Result = HttpNotFound();
                return;
            }

            //如果当前用户没有登录
            if (partUserInfo.Uid < 1)
            {
                filterContext.Result = HttpNotFound();
                return;
            }

            //如果当前用户不是管理员
            if (partUserInfo.MallAGid == 1)
            {
                filterContext.Result = HttpNotFound();
                return;
            }
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <returns></returns>
        public ActionResult Upload()
        {
            string operation = WebHelper.GetQueryString("operation");

            if (operation == "ueconfig")
            {
                StringBuilder imageAllowFiles = new StringBuilder("[");
                foreach (string imgType in StringHelper.SplitString(mallConfigInfo.UploadImgType))
                {
                    imageAllowFiles.AppendFormat("\"{0}\",", imgType);
                }
                if (imageAllowFiles.Length > 1)
                    imageAllowFiles.Remove(imageAllowFiles.Length - 1, 1);
                imageAllowFiles.Append("]");

                string imageUrlPrefix = string.IsNullOrEmpty(mallConfigInfo.UploadServer) ? "/" : mallConfigInfo.UploadServer;

                return Content(string.Format("{0}\"imageActionName\": \"uploadimage\", \"imageFieldName\": \"upfile\", \"imageMaxSize\": {1},\"imageAllowFiles\": {2}, \"imageCompressEnable\": true, \"imageCompressBorder\": 1600, \"imageInsertAlign\": \"none\", \"imageUrlPrefix\": \"{3}\", \"imagePathFormat\": \"\", \"imageManagerActionName\": \"listimage\",\"imageManagerListPath\": \"upload/image\",\"imageManagerListSize\": 20, \"imageManagerUrlPrefix\": \"/ueditor/net/\",\"imageManagerInsertAlign\": \"none\", \"imageManagerAllowFiles\": [\".png\", \".jpg\", \".jpeg\", \".gif\", \".bmp\"]{4}", "{", mallConfigInfo.UploadImgSize, imageAllowFiles, imageUrlPrefix, "}"));
            }
            if (operation == "uploadproductimage")//上传商品图片
            {
                int storeId = WebHelper.GetQueryInt("storeId");
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveUplaodProductImage(storeId, file);
                return Content(result);
            }
            if (operation == "uploadproducteditorimage")//上传商品编辑器中图片
            {
                int storeId = WebHelper.GetQueryInt("storeId");
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveProductEditorImage(storeId, file);
                return Content(string.Format("{3}'url':'upload/store/{0}/product/editor/{1}','state':'{2}'{4}", storeId, result, GetUEState(result), "{", "}"));
            }
            if (operation == "uploadstorebanner")//上传店铺banner
            {
                int storeId = WebHelper.GetQueryInt("storeId");
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveUploadStoreBanner(storeId, file);
                return Content(result);
            }
            if (operation == "uploadstorelogo")//上传店铺logo
            {
                int storeId = WebHelper.GetQueryInt("storeId");
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveUploadStoreLogo(storeId, file);
                return Content(result);
            }
            if (operation == "uploadadvertbody")//上传广告主体
            {
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveUploadAdvertBody(file);
                return Content(result);
            }
            if (operation == "uploadbannerimg")//上传banner图片
            {
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveUploadBannerImg(file);
                return Content(result);
            }
            if (operation == "uploadnewseditorimage")//上传新闻编辑器中的图片
            {
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveNewsEditorImage(file);
                return Content(string.Format("{2}'url':'upload/news/{0}','state':'{1}'{3}", result, GetUEState(result), "{", "}"));
            }
            if (operation == "uploadinformseditorimage")//上传消息编辑器中的图片
            {
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveInformsEditorImage(file);
                return Content(string.Format("{2}'url':'upload/informs/{0}','state':'{1}'{3}", result, GetUEState(result), "{", "}"));
            }
            if (operation == "uploadbrandlogo")//上传品牌logo
            {
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveUploadBrandLogo(file);
                return Content(result);
            }
            if (operation == "uploadhelpeditorimage")//上传帮助编辑器中的图片
            {
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveHelpEditorImage(file);
                return Content(string.Format("{2}'url':'upload/help/{0}','state':'{1}'{3}", result, GetUEState(result), "{", "}"));
            }
            if (operation == "uploadfriendlinklogo")//上传友情链接logo
            {
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveUploadFriendLinkLogo(file);
                return Content(result);
            }
            if (operation == "uploaduserrankavatar")//上传用户等级头像
            {
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveUploadUserRankAvatar(file);
                return Content(result);
            }
            if (operation == "uploadstorerankavatar")//上传店铺等级头像
            {
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveUploadStoreRankAvatar(file);
                return Content(result);
            }
            if (operation == "uploadgroupproductimage")//上传分区系列
            {
                HttpPostedFileBase file = Request.Files[0];
                string result = MallUtils.SaveUploadGroupProductImage(file);
                return Content(result);
            }
            return HttpNotFound();
        }

        /// <summary>
        /// 省列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProvinceList()
        {
            List<RegionInfo> regionList = Regions.GetProvinceList();

            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            foreach (RegionInfo info in regionList)
            {
                sb.AppendFormat("{0}\"id\":\"{1}\",\"name\":\"{2}\"{3},", "{", info.RegionId, info.Name, "}");
            }

            if (regionList.Count > 0)
                sb.Remove(sb.Length - 1, 1);

            sb.Append("]");

            return Content(sb.ToString());
        }

        /// <summary>
        /// 市列表
        /// </summary>
        /// <param name="provinceId">省id</param>
        /// <returns></returns>
        public ActionResult CityList(int provinceId = -1)
        {
            List<RegionInfo> regionList = Regions.GetCityList(provinceId);

            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            foreach (RegionInfo info in regionList)
            {
                sb.AppendFormat("{0}\"id\":\"{1}\",\"name\":\"{2}\"{3},", "{", info.RegionId, info.Name, "}");
            }

            if (regionList.Count > 0)
                sb.Remove(sb.Length - 1, 1);

            sb.Append("]");

            return Content(sb.ToString());
        }

        /// <summary>
        /// 县或区列表
        /// </summary>
        /// <param name="cityId">市id</param>
        /// <returns></returns>
        public ActionResult CountyList(int cityId = -1)
        {
            List<RegionInfo> regionList = Regions.GetCountyList(cityId);

            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            foreach (RegionInfo info in regionList)
            {
                sb.AppendFormat("{0}\"id\":\"{1}\",\"name\":\"{2}\"{3},", "{", info.RegionId, info.Name, "}");
            }

            if (regionList.Count > 0)
                sb.Remove(sb.Length - 1, 1);

            sb.Append("]");

            return Content(sb.ToString());
        }

        /// <summary>
        /// 获得ueditor状态
        /// </summary>
        /// <param name="result">上传结果</param>
        /// <returns></returns>
        private string GetUEState(string result)
        {
            if (result == "-1")
            {
                return "上传图片不能为空";
            }
            else if (result == "-2")
            {
                return "不允许的图片类型";
            }
            else if (result == "-3")
            {
                return "图片大小超出网站限制";
            }
            else
            {
                return "SUCCESS";
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 增加线下代理会员，生成订单生成库存 并减少上级库存
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAgentAndOrder()
        {
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


            int regionid = TypeHelper.StringToInt(WebHelper.GetFormString("regionid"));
            string address = WebHelper.GetFormString("address");
            string accountName = mobile;

            StringBuilder errorList = new StringBuilder("[");
            //账号验证
            if (string.IsNullOrWhiteSpace(accountName))
                return Content("账户名不能为空");
            if (!ValidateHelper.IsMobile(accountName))
                return Content("手机号格式不正确");

            UserInfo userInfo = null;

            if (errorList.Length == 1)
            {
                if (Users.IsExistMobile(accountName) || AccountUtils.IsUserExistForDirSale(accountName))
                    return Content("手机号已经存在");
                else
                    userInfo = new UserInfo() { UserName = string.Empty, Email = string.Empty, Mobile = accountName };
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
                userInfo.NickName = accountName.Substring(0, 3) + "***" + accountName.Substring(7, 4);
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
            OrderInfo orderInfo = Orders.CreateOrder(orderUser, storeInfo, addOrderProductList, new List<SinglePromotionInfo>(), shipAddressInfo, payPluginInfo, ref payCreditCount, ref haiMiCount, ref hongBaoCount, ref cashCount, new List<CouponInfo>(), ref couponMoeny, ref daiLiCount, ref YongJinCount, storeFullCut, buyerRemark, invoice, bestTime, ip, cashId, new List<CashCouponInfo>(), null);

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

            return View();
        }

    }
}
