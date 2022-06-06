using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Models;

namespace VMall.Web.Controllers
{
    /// <summary>
    /// 购物车控制器类
    /// </summary>
    public partial class CartController : BaseWebController
    {
        #region 基本购物车操作模块
        /// <summary>
        /// 获得购物车模型通用方法
        /// </summary>
        /// <param name="selectedCartItemKeyList"></param>
        /// <param name="type">1.修改产品数量 2删除单个3批量删除4取消或选中购物车项</param>
        /// <returns></returns>
        public CartModel GetCartModel(int type = 0, string selectedCartItemKeyList = "", int pid = 0, int buyCount = 0, string pids = "")
        {
            //购物车商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            orderProductList = Carts.UpdateCartForPromotionActive(orderProductList, WorkContext.Uid, WorkContext.Sid);
            if (orderProductList.Exists(x => x.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId")) && WorkContext.PartUserInfo.AgentType <= 0)
                orderProductList = Carts.UpdateCartForAgent(orderProductList, WorkContext.Uid, WorkContext.Sid);

           //公司员工内部购买
            if (WorkContext.UserRankInfo.UserRid == 10)
               orderProductList=Carts.UpdateCartForCompanyUser(orderProductList, WorkContext.Uid, WorkContext.Sid);
            orderProductList.ForEach(x =>
            {
                PartProductInfo pro = Products.GetPartProductById(x.Pid);

                if (pro != null)
                {
                    x.PV = x.ProductPV;
                    x.HaiMi = x.ProductHaiMi;
                    x.TaxRate = pro.TaxRate;
                    x.SaleType = pro.SaleType;
                    x.HongBaoCut = x.ProductHBCut;
                    x.MinBuyCount = pro.MinBuyCount;
                    if (WorkContext.IsDirSaleUser && x.Pid == 3235)
                        x.MinBuyCount = 200;
                    x.ProductState = pro.State;
                }

                //SinglePromotionInfo single = Promotions.GetSinglePromotionByPidAndTime(x.Pid, DateTime.Now);
                //if (single != null)
                //{
                //    x.HongBaoCut = single.PromoHongBaoCut;
                //    x.PV = x.ProductPV;
                //    x.HaiMi = x.ProductHaiMi;
                //}
            });

            if (type == 1)
            {
                //对应商品
                OrderProductInfo orderProductInfo = Carts.GetCommonOrderProductByPid(pid, orderProductList);
                if (orderProductInfo != null)//当商品已经存在
                {
                    if (buyCount < 1)//当购买数量小于1时，删除此商品
                    {
                        Carts.DeleteCartProduct(ref orderProductList, orderProductInfo);
                    }
                    else if (buyCount != orderProductInfo.BuyCount)
                    {
                        Carts.AddExistProductToCart(ref orderProductList, buyCount, orderProductInfo, DateTime.Now);
                    }
                    orderProductList = Carts.UpdateCartForPromotionActive(orderProductList, WorkContext.Uid, WorkContext.Sid);
                    if (orderProductList.Exists(x => x.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId")) && WorkContext.PartUserInfo.AgentType <= 0)
                        orderProductList = Carts.UpdateCartForAgent(orderProductList, WorkContext.Uid, WorkContext.Sid);
                    orderProductList.ForEach(x =>
                    {
                        PartProductInfo pro = Products.GetPartProductById(x.Pid);

                        if (pro != null)
                        {
                            x.PV = x.ProductPV;
                            x.HaiMi = x.ProductHaiMi;
                            x.TaxRate = pro.TaxRate;
                            x.SaleType = pro.SaleType;
                            x.HongBaoCut = x.ProductHBCut;
                            x.MinBuyCount = pro.MinBuyCount;
                            if (WorkContext.IsDirSaleUser && x.Pid == 3235)
                                x.MinBuyCount = 200;
                            x.ProductState = pro.State;
                        }
                    });
                }
            }
            else if (type == 2)
            {
                //对应商品
                OrderProductInfo orderProductInfo = Carts.GetCommonOrderProductByPid(pid, orderProductList);
                if (orderProductInfo != null)
                    Carts.DeleteCartProduct(ref orderProductList, orderProductInfo);
            }
            else if (type == 3)
            {
                //对应商品
                foreach (string item in pids.Split(','))
                {
                    OrderProductInfo orderProductInfo = Carts.GetCommonOrderProductByPid(TypeHelper.StringToInt(item), orderProductList);
                    if (orderProductInfo != null)
                        Carts.DeleteCartProduct(ref orderProductList, orderProductInfo);
                }
            }
            //商品数量
            int pCount = Carts.SumOrderProductCount(orderProductList);
            //店铺购物车列表
            List<StoreCartInfo> storeCartList = new List<StoreCartInfo>();
            if (string.IsNullOrEmpty(selectedCartItemKeyList))
                storeCartList = Carts.TidyMallOrderProductList(orderProductList);
            else
                storeCartList = Carts.TidyMallOrderProductList(StringHelper.SplitString(selectedCartItemKeyList), orderProductList);


            //商品总数量
            int totalCount = Carts.SumMallCartOrderProductCount(storeCartList);
            //商品合计
            decimal productAmount = Carts.SumMallCartOrderProductAmount(storeCartList);
            //满减折扣
            int fullCut = Carts.SumMallCartFullCut(storeCartList);
            //关税合计
            decimal taxAmount = Carts.SumMallCartTaxAmount(storeCartList);
            //红包减免合计
            decimal HongBaoCutAmount = Carts.SumMallCartHongBaoCutAmount(storeCartList);
            decimal PVAmount = Carts.SumMallPVAmount(storeCartList);
            //订单合计
            decimal orderAmount = productAmount - fullCut + (taxAmount > 50 ? taxAmount : 0);

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCut = fullCut,
                taxAmount = taxAmount,
                HongBaoCutAmount = HongBaoCutAmount,
                PVAmount = PVAmount,
                OrderAmount = orderAmount,
                StoreCartList = storeCartList
            };
            //将购物车中商品数量写入cookie
            Carts.SetCartProductCountCookie(pCount);
            return model;
        }
        /// <summary>
        /// 购物车
        /// </summary>
        public ActionResult Index()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return Redirect(Url.Action("login", "account", new RouteValueDictionary { { "returnUrl", Url.Action("index") } }));

            CartModel model = GetCartModel();

            return View(model);
        }

        /// <summary>
        /// 顶部购物车
        /// </summary>
        /// <returns></returns>
        public ActionResult TopCart()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            CartModel model = GetCartModel();

            return View(model);
        }

        /// <summary>
        /// 购物车快照
        /// </summary>
        public ActionResult Snap()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            CartModel model = GetCartModel();

            return View(model);
        }

        /// <summary>
        /// 添加商品到购物车
        /// </summary>
        public ActionResult AddProduct()
        {
            int pid = WebHelper.GetQueryInt("pid");//商品id
            int buyCount = WebHelper.GetQueryInt("buyCount");//购买数量
            int scSubmitType = WorkContext.MallConfig.SCSubmitType;//购物车的提交方式
            int suitpid = WebHelper.GetQueryInt("suitpid");//套餐体验包pid
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return Redirect(Url.Action("login", "account", new RouteValueDictionary { { "returnUrl", Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }) } }));
                    case 1:
                        return Redirect(Url.Action("login", "account", new RouteValueDictionary { { "returnUrl", Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }) } }));
                    case 2:
                        return AjaxResult("nologin", "请先登录");
                    default:
                        return Redirect(Url.Action("login", "account", new RouteValueDictionary { { "returnUrl", Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }) } }));
                }
            }

            //判断商品是否存在
            PartProductInfo partProductInfo = Products.GetPartProductById(pid);

            if (partProductInfo == null)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView("/", "商品不存在");
                    case 1:
                        return PromptView("/", "商品不存在");
                    case 2:
                        return AjaxResult("noproduct", "请选择商品");
                    default:
                        return PromptView("/", "商品不存在");
                }
            }
            if (WorkContext.IsDirSaleUser && partProductInfo.Pid == 3235)
                partProductInfo.MinBuyCount = 200;

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(partProductInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView("/", "商品不存在");
                    case 1:
                        return PromptView("/", "商品不存在");
                    case 2:
                        return AjaxResult("noproduct", "请选择商品");
                    default:
                        return PromptView("/", "商品不存在");
                }
            }

            // 限制非直销会员购买咖啡券
            if (StringHelper.StrContainsForNum( WebHelper.GetConfigSettings("CoffeeQuanPid"),pid.ToString()) && !WorkContext.IsDirSaleUser)
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView("/", "您的会员等级还不能购买此商品");
                    case 1:
                        return PromptView("/", "您的会员等级还不能购买此商品");
                    case 2:
                        return AjaxResult("noproduct", "您的会员等级还不能购买此商品");
                    default:
                        return PromptView("/", "您的会员等级还不能购买此商品");
                }

            //购买数量不能小于1
            if (buyCount < 1)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView(Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }), "购买数量不能小于1");
                    case 1:
                        return PromptView(Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }), "购买数量不能小于1");
                    case 2:
                        return AjaxResult("buycountmin", "请填写购买数量");
                    default:
                        return PromptView(Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }), "购买数量不能小于1");
                }
            }

            //商品库存
            int stockNumber = Products.GetProductStockNumberByPid(pid);
            if (stockNumber < buyCount)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView(Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }), "商品已售空");
                    case 1:
                        return PromptView(Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }), "商品已售空");
                    case 2:
                        return AjaxResult("stockout", "商品库存不足");
                    default:
                        return PromptView(Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }), "商品已售空");
                }
            }

            //购买数量不能小于最低起购数量
            if (buyCount < partProductInfo.MinBuyCount)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView(Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }), "购买数量不能小于最低起购数量" + partProductInfo.MinBuyCount);
                    case 1:
                        return PromptView(Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }), "购买数量不能小于最低起购数量" + partProductInfo.MinBuyCount);
                    case 2:
                        return AjaxResult("buycountmin", "请填写正确的购买数量");
                    default:
                        return PromptView(Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }), "购买数量不能小于最低起购数量" + partProductInfo.MinBuyCount);
                }
            }

            //单品秒杀不能购买
            SinglePromotionInfo flashSale = Promotions.GetFlashSaleInfoByPidAndStartTime(pid, DateTime.Now);
            if (flashSale != null)
            {
                if (flashSale.StartTime1 > DateTime.Now)
                    switch (scSubmitType)
                    {
                        case 0:
                            return PromptView("/", "秒杀还未开始，不能进行购买");
                        case 1:
                            return PromptView("/", "秒杀还未开始，不能进行购买");
                        case 2:
                            return AjaxResult("noproduct", "秒杀还未开始，不能进行购买");
                        default:
                            return PromptView("/", "秒杀还未开始，不能进行购买");
                    }
            }

            //购物车中已经存在的商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            OrderProductInfo orderProductInfo = Carts.GetCommonOrderProductByPid(pid, orderProductList);
            if (orderProductInfo == null)
            {
                if ((WorkContext.Uid > 0 && orderProductList.Count >= WorkContext.MallConfig.MemberSCCount) || (WorkContext.Uid < 1 && orderProductList.Count >= WorkContext.MallConfig.GuestSCCount))
                {
                    switch (scSubmitType)
                    {
                        case 2:
                            return AjaxResult("full", "购物车已满");
                        default:
                            return PromptView(Url.Action("index", "cart"), "购物车已满,请先结账");
                    }
                }
            }

            buyCount = orderProductInfo == null ? buyCount : orderProductInfo.BuyCount + buyCount;
            //将商品添加到购物车
            Carts.AddProductToCart(ref orderProductList, buyCount, partProductInfo, WorkContext.Sid, WorkContext.Uid, DateTime.Now);
            //微商套餐包处理,非代理会员必选套餐包
            //if (partProductInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId") && WorkContext.PartUserInfo.AgentType <= 0)
            //{
            //    PartProductInfo agentProductInfo = Products.GetPartProductById(suitpid);
            //    if (agentProductInfo == null)
            //    {
            //        switch (scSubmitType)
            //        {
            //            case 0:
            //                return PromptView(Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }), "优选代理套餐产品不存在");
            //            case 1:
            //                return PromptView(Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }), "优选代理套餐产品不存在");
            //            case 2:
            //                return AjaxResult("buycountmin", "优选代理套餐产品不存在");
            //            default:
            //                return PromptView(Url.Action("product", "catalog", new RouteValueDictionary { { "pid", pid } }), "优选代理套餐产品不存在");
            //        }
            //    }
            //    Carts.AddProductToCart(ref orderProductList, 1, agentProductInfo, WorkContext.Sid, WorkContext.Uid, DateTime.Now);
            //}
            //将购物车中商品数量写入cookie
            Carts.SetCartProductCountCookie(Carts.SumOrderProductCount(orderProductList));

            switch (scSubmitType)
            {
                case 0:
                    return RedirectToAction("addsuccess", new RouteValueDictionary { { "type", 0 }, { "id", pid } });
                case 1:
                    return RedirectToAction("index");
                case 2:
                    return AjaxResult("success", "添加成功");
                default:
                    return RedirectToAction("addsuccess", new RouteValueDictionary { { "type", 0 }, { "id", pid } });
            }
        }

        /// <summary>
        /// 直接购买商品
        /// </summary>
        /// <returns></returns>
        public ActionResult DirectBuyProduct()
        {
            int pid = WebHelper.GetQueryInt("pid");//商品id
            int buyCount = WebHelper.GetQueryInt("buyCount");//购买数量
            string proid = WebHelper.GetQueryString("proid");//商品id
            int suitpid = WebHelper.GetQueryInt("suitpid");//套餐体验包pid
            if (!string.IsNullOrEmpty(proid))
            {
                pid = TypeHelper.StringToInt(StringHelper.SplitString(proid, "_")[0]);
                buyCount = TypeHelper.StringToInt(StringHelper.SplitString(proid, "_")[1]);
            }
            //当商城不允许游客使用购物车时
            //if ( WorkContext.Uid < 1)
            //return AjaxResult("success", Url.Action("login", "account", new RouteValueDictionary { { "returnUrl", WorkContext.Url } })); 
            //return AjaxResult("nologin", "请先登录");

            //判断商品是否存在
            PartProductInfo partProductInfo = Products.GetPartProductById(pid);

            if (partProductInfo == null)
                return AjaxResult("noproduct", "请选择商品");
            if (WorkContext.IsDirSaleUser && partProductInfo.Pid == 3235)
                partProductInfo.MinBuyCount = 200;

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(partProductInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return AjaxResult("noproduct", "请选择商品");

            // 限制非直销会员购买咖啡券
            if (StringHelper.StrContainsForNum(WebHelper.GetConfigSettings("CoffeeQuanPid"), pid.ToString()) && !WorkContext.IsDirSaleUser)
                return AjaxResult("noproduct", "您的会员等级还不能购买此商品");

            //购买数量不能小于1
            if (buyCount < 1)
                return AjaxResult("buycountmin", "请填写购买数量");

            //商品库存
            int stockNumber = Products.GetProductStockNumberByPid(pid);
            if (stockNumber < buyCount)
                return AjaxResult("stockout", "商品库存不足");

            //购买数量不能小于最低起购数量
            if (buyCount < partProductInfo.MinBuyCount)
                return AjaxResult("buycountmin", "请填写正确的购买数量");

            //单品秒杀不能购买
            SinglePromotionInfo flashSale = Promotions.GetFlashSaleInfoByPidAndStartTime(pid, DateTime.Now);
            if (flashSale != null)
            {
                if (flashSale.StartTime1 > DateTime.Now)
                    return AjaxResult("noproduct", "秒杀还未开始，不能进行购买");
            }
            if (new ExCodeProducts().GetList(" codetypeid>=3 ").Exists(x => x.Pid == pid))
            {
                List<ExChangeCouponsInfo> vaildEexCodeList = ExChangeCoupons.GetVaildListByPid(string.Format(" pid IN (" + pid + ") and uid={0} and state=1 and oid=0 and validtime>getdate()  ", WorkContext.Uid));
                if (vaildEexCodeList.Count <= 0)
                    return AjaxResult("noproduct", "尊敬的会员，您好。您现在所持有的愿望券与你所选购的愿望等级不符。\\r\\n 例：持有【优】等级愿望券，便只能选择【优】等级的愿望。如有不明，请及时联系客服人员。");
            }
            //购物车中已经存在的商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            OrderProductInfo orderProductInfo = Carts.GetCommonOrderProductByPid(pid, orderProductList);
            if (orderProductInfo == null)
            {
                if ((WorkContext.Uid > 0 && orderProductList.Count >= WorkContext.MallConfig.MemberSCCount) || (WorkContext.Uid < 1 && orderProductList.Count >= WorkContext.MallConfig.GuestSCCount))
                    return AjaxResult("full", "购物车已满");
            }

            //buyCount = orderProductInfo == null ? buyCount : orderProductInfo.BuyCount + buyCount;
            if (orderProductInfo != null)
                Carts.DeleteCartProduct(ref orderProductList, orderProductInfo);
            //将商品添加到购物车
            Carts.AddProductToCart(ref orderProductList, buyCount, partProductInfo, WorkContext.Sid, WorkContext.Uid, DateTime.Now);
            //微商套餐包处理,非代理会员必选套餐包
            int suitPid = 0;
            //if (partProductInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId") && WorkContext.PartUserInfo.AgentType <= 0)
            //{
            //    PartProductInfo agentProductInfo = Products.GetPartProductById(suitpid);
            //    if (agentProductInfo == null)
            //    {
            //        return AjaxResult("noproduct", "优选代理套餐产品不存在");
            //    }
            //    Carts.AddProductToCart(ref orderProductList, 1, agentProductInfo, WorkContext.Sid, WorkContext.Uid, DateTime.Now);
            //    suitPid = suitpid;
            //}
            //将购物车中商品数量写入cookie
            Carts.SetCartProductCountCookie(Carts.SumOrderProductCount(orderProductList));
            if (!string.IsNullOrEmpty(proid))
            {
                //RedirectToAction("confirmorder", "order", new RouteValueDictionary { { "selectedCartItemKeyList", "0_" + pid }, { "selectedStoreKeyList", partProductInfo.StoreId } });
                return Content("<script type='text/javascript'>window.location='" + Url.Action("confirmorder", "order", new RouteValueDictionary { { "selectedCartItemKeyList", "0_" + pid + (suitPid > 0 ? ",0_" + suitPid : "") }, { "selectedStoreKeyList", partProductInfo.StoreId + (suitPid > 0 ? "," + WebHelper.GetConfigSettingsInt("AgentSuitStoreId") : "") } }) + "';</script>");
            }
            else
            {
                return AjaxResult("success", Url.Action("confirmorder", "order", new RouteValueDictionary { { "selectedCartItemKeyList", "0_" + pid + (suitPid > 0 ? ",0_" + suitPid : "") }, { "selectedStoreKeyList", partProductInfo.StoreId + (suitPid > 0 ? "," + WebHelper.GetConfigSettingsInt("AgentSuitStoreId") : "") } }));
            }

        }

        /// <summary>
        /// 修改购物车中商品数量
        /// </summary>
        public ActionResult ChangePruductCount()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pid = WebHelper.GetQueryInt("pid");//商品id
            int buyCount = WebHelper.GetQueryInt("buyCount");//购买数量
            string selectedCartItemKeyList = WebHelper.GetQueryString("selectedCartItemKeyList");//选中的购物车项键列表

            PartProductInfo partProductInfo = Products.GetPartProductById(pid);
            if (WorkContext.IsDirSaleUser && partProductInfo.Pid == 3235)
                partProductInfo.MinBuyCount = 200;
            if (buyCount < partProductInfo.MinBuyCount)
                return AjaxResult("limitcount", "购买数量不能小于最低起购数量");

            CartModel model = GetCartModel(1, selectedCartItemKeyList, pid, buyCount);

            return View("ajaxindex", model);
        }

        /// <summary>
        /// 删除购物车中商品
        /// </summary>
        public ActionResult DelPruduct()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pid = WebHelper.GetQueryInt("pid");//商品id
            int pos = WebHelper.GetQueryInt("pos");//位置
            string selectedCartItemKeyList = WebHelper.GetQueryString("selectedCartItemKeyList");//选中的购物车项键列表

            CartModel model = GetCartModel(2, selectedCartItemKeyList, pid);

            if (pos == 0)
                return View("ajaxindex", model);
            else
                return View("topcart", model);
        }

        /// <summary>
        /// 批量删除选中的购物车商品
        /// </summary>
        public ActionResult BatchDelPruduct()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            string pids = WebHelper.GetQueryString("pids");//商品id
            int pos = WebHelper.GetQueryInt("pos");//位置
            string selectedCartItemKeyList = WebHelper.GetQueryString("selectedCartItemKeyList");//选中的购物车项键列表

            CartModel model = GetCartModel(3, selectedCartItemKeyList, 0, 0, pids);

            if (pos == 0)
                return View("ajaxindex", model);
            else
                return View("topcart", model);
        }

        /// <summary>
        /// 清空购物车
        /// </summary>
        public ActionResult Clear()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            //位置
            int pos = WebHelper.GetQueryInt("pos");

            Carts.ClearCart(WorkContext.Uid, WorkContext.Sid);
            //将购物车中商品数量写入cookie
            Carts.SetCartProductCountCookie(0);

            if (pos == 0)
                return View("ajaxindex");
            else
                return View("snap");
        }

        /// <summary>
        /// 购物车添加成功提示
        /// </summary>
        public ActionResult AddSuccess()
        {
            int id = WebHelper.GetQueryInt("id");//项id
            int type = WebHelper.GetQueryInt("type");//项类型

            CartAddSuccessModel model = new CartAddSuccessModel
            {
                ItemId = id,
                ItemType = type
            };
            return View(model);
        }

        /// <summary>
        /// 取消或选中购物车项
        /// </summary>
        /// <returns></returns>
        public ActionResult CancelOrSelectCartItem()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            string selectedCartItemKeyList = WebHelper.GetQueryString("selectedCartItemKeyList");//选中的购物车项键列表

            CartModel model = GetCartModel(4, selectedCartItemKeyList);

            return View("ajaxindex", model);
        }

        /// <summary>
        /// 全选购物车项
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectAllCartItem()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            CartModel model = GetCartModel();

            return View("ajaxindex", model);
        }
        #endregion


        #region 活动产品及套装购物车模块
        /// <summary>
        /// 添加套装到购物车
        /// </summary>
        public ActionResult AddSuit()
        {
            int pmId = WebHelper.GetQueryInt("pmId");//套装id
            int buyCount = WebHelper.GetQueryInt("buyCount");//购买数量
            int scSubmitType = WorkContext.MallConfig.SCSubmitType;//购物车的提交方式

            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return Redirect(Url.Action("login", "account", new RouteValueDictionary { { "returnUrl", Url.Action("suit", "catalog", new RouteValueDictionary { { "pmId", pmId } }) } }));
                    case 1:
                        return Redirect(Url.Action("login", "account", new RouteValueDictionary { { "returnUrl", Url.Action("suit", "catalog", new RouteValueDictionary { { "pmId", pmId } }) } }));
                    case 2:
                        return AjaxResult("nologin", "请先登录");
                    default:
                        return Redirect(Url.Action("login", "account", new RouteValueDictionary { { "returnUrl", Url.Action("suit", "catalog", new RouteValueDictionary { { "pmId", pmId } }) } }));
                }
            }

            //购买数量不能小于1
            if (buyCount < 1)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView(Url.Action("suit", "catalog", new RouteValueDictionary { { "pmId", pmId } }), "购买数量不能小于1");
                    case 1:
                        return PromptView(Url.Action("suit", "catalog", new RouteValueDictionary { { "pmId", pmId } }), "购买数量不能小于1");
                    case 2:
                        return AjaxResult("buycountmin", "请填写购买数量");
                    default:
                        return PromptView(Url.Action("suit", "catalog", new RouteValueDictionary { { "pmId", pmId } }), "购买数量不能小于1");
                }
            }

            //获得套装促销活动
            SuitPromotionInfo suitPromotionInfo = Promotions.GetSuitPromotionByPmIdAndTime(pmId, DateTime.Now);
            //套装促销活动不存在时
            if (suitPromotionInfo == null)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView("/", "套装不存在");
                    case 1:
                        return PromptView("/", "套装不存在");
                    case 2:
                        return AjaxResult("nosuit", "请选择套装");
                    default:
                        return PromptView("/", "套装不存在");
                }
            }

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(suitPromotionInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView("/", "套装不存在");
                    case 1:
                        return PromptView("/", "套装不存在");
                    case 2:
                        return AjaxResult("nosuit", "请选择套装");
                    default:
                        return PromptView("/", "套装不存在");
                }
            }

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            List<OrderProductInfo> suitOrderProductList = Carts.GetSuitOrderProductList(pmId, orderProductList, false);
            if (suitOrderProductList.Count == 0)
            {
                if ((WorkContext.Uid > 0 && orderProductList.Count >= WorkContext.MallConfig.MemberSCCount) || (WorkContext.Uid < 1 && orderProductList.Count >= WorkContext.MallConfig.GuestSCCount))
                {
                    switch (scSubmitType)
                    {
                        case 2:
                            return AjaxResult("full", "购物车已满");
                        default:
                            return PromptView(Url.Action("index", "cart"), "购物车已满,请先结账");
                    }
                }
            }

            //扩展套装商品列表
            List<ExtSuitProductInfo> extSuitProductList = Promotions.GetExtSuitProductList(suitPromotionInfo.PmId);
            if (extSuitProductList.Count < 1)
            {
                switch (scSubmitType)
                {
                    case 0:
                        return PromptView(Url.Action("suit", "catalog", new RouteValueDictionary { { "pmId", pmId } }), "套装商品为空");
                    case 1:
                        return PromptView(Url.Action("suit", "catalog", new RouteValueDictionary { { "pmId", pmId } }), "套装商品为空");
                    case 2:
                        return AjaxResult("noproduct", "套装中没有商品");
                    default:
                        return PromptView(Url.Action("suit", "catalog", new RouteValueDictionary { { "pmId", pmId } }), "套装商品为空");
                }
            }

            //套装商品id列表
            StringBuilder pidList = new StringBuilder();
            foreach (ExtSuitProductInfo extSuitProductInfo in extSuitProductList)
                pidList.AppendFormat("{0},", extSuitProductInfo.Pid);
            pidList.Remove(pidList.Length - 1, 1);

            //套装商品库存列表
            List<ProductStockInfo> productStockList = Products.GetProductStockList(pidList.ToString());
            foreach (ProductStockInfo item in productStockList)
            {
                if (item.Number < buyCount)
                {
                    switch (scSubmitType)
                    {
                        case 0:
                            return PromptView(Url.Action("suit", "catalog", new RouteValueDictionary { { "pmId", pmId } }), "库存不足");
                        case 1:
                            return PromptView(Url.Action("suit", "catalog", new RouteValueDictionary { { "pmId", pmId } }), "库存不足");
                        case 2:
                            return AjaxResult("stockout", item.Pid.ToString());
                        default:
                            return PromptView(Url.Action("suit", "catalog", new RouteValueDictionary { { "pmId", pmId } }), "库存不足");
                    }
                }
            }

            buyCount = suitOrderProductList.Count == 0 ? buyCount : suitOrderProductList[0].BuyCount / suitOrderProductList[0].ExtCode2 + buyCount;
            Carts.AddSuitToCart(ref orderProductList, extSuitProductList, suitPromotionInfo, buyCount, WorkContext.Sid, WorkContext.Uid, DateTime.Now);
            //将购物车中商品数量写入cookie
            Carts.SetCartProductCountCookie(Carts.SumOrderProductCount(orderProductList));

            switch (scSubmitType)
            {
                case 0:
                    return RedirectToAction("addsuccess", new RouteValueDictionary { { "type", 1 }, { "id", pmId } });
                case 1:
                    return RedirectToAction("index");
                case 2:
                    return AjaxResult("success", "添加成功");
                default:
                    return RedirectToAction("addsuccess", new RouteValueDictionary { { "type", 1 }, { "id", pmId } });
            }
        }

        /// <summary>
        /// 直接购买套装
        /// </summary>
        /// <returns></returns>
        public ActionResult DirectBuySuit()
        {
            int pmId = WebHelper.GetQueryInt("pmId");//套装id
            int buyCount = WebHelper.GetQueryInt("buyCount");//购买数量

            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            //购买数量不能小于1
            if (buyCount < 1)
                return AjaxResult("buycountmin", "请填写购买数量");

            //获得套装促销活动
            SuitPromotionInfo suitPromotionInfo = Promotions.GetSuitPromotionByPmIdAndTime(pmId, DateTime.Now);
            //套装促销活动不存在时
            if (suitPromotionInfo == null)
                return AjaxResult("nosuit", "请选择套装");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(suitPromotionInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return AjaxResult("nosuit", "请选择套装");

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            List<OrderProductInfo> suitOrderProductList = Carts.GetSuitOrderProductList(pmId, orderProductList, false);
            if (suitOrderProductList.Count == 0)
            {
                if ((WorkContext.Uid > 0 && orderProductList.Count >= WorkContext.MallConfig.MemberSCCount) || (WorkContext.Uid < 1 && orderProductList.Count >= WorkContext.MallConfig.GuestSCCount))
                    return AjaxResult("full", "购物车已满");
            }

            //扩展套装商品列表
            List<ExtSuitProductInfo> extSuitProductList = Promotions.GetExtSuitProductList(suitPromotionInfo.PmId);
            if (extSuitProductList.Count < 1)
                return AjaxResult("noproduct", "套装中没有商品");

            //套装商品id列表
            StringBuilder pidList = new StringBuilder();
            foreach (ExtSuitProductInfo extSuitProductInfo in extSuitProductList)
                pidList.AppendFormat("{0},", extSuitProductInfo.Pid);
            pidList.Remove(pidList.Length - 1, 1);

            //套装商品库存列表
            List<ProductStockInfo> productStockList = Products.GetProductStockList(pidList.ToString());
            foreach (ProductStockInfo item in productStockList)
            {
                if (item.Number < buyCount)
                    return AjaxResult("stockout", item.Pid.ToString());
            }

            buyCount = suitOrderProductList.Count == 0 ? buyCount : suitOrderProductList[0].BuyCount / suitOrderProductList[0].ExtCode3 + buyCount;
            Carts.AddSuitToCart(ref orderProductList, extSuitProductList, suitPromotionInfo, buyCount, WorkContext.Sid, WorkContext.Uid, DateTime.Now);
            //将购物车中商品数量写入cookie
            Carts.SetCartProductCountCookie(Carts.SumOrderProductCount(orderProductList));

            return AjaxResult("success", Url.Action("confirmorder", "order", new RouteValueDictionary { { "selectedCartItemKeyList", "1_" + pmId } }));
        }

        /// <summary>
        /// 修改购物车中套装数量
        /// </summary>
        public ActionResult ChangeSuitCount()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pmId = WebHelper.GetQueryInt("pmId");//套装id
            int buyCount = WebHelper.GetQueryInt("buyCount");//购买数量
            string selectedCartItemKeyList = WebHelper.GetQueryString("selectedCartItemKeyList");//选中的购物车项键列表

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            orderProductList.ForEach(x =>
            {
                PartProductInfo pro = Products.GetPartProductById(x.Pid);

                if (pro != null)
                {
                    x.PV = x.ProductPV;
                    x.HaiMi = x.ProductHaiMi;
                    x.TaxRate = pro.TaxRate;
                    x.SaleType = pro.SaleType;
                    x.HongBaoCut = x.ProductHBCut;
                    x.MinBuyCount = pro.MinBuyCount;
                    x.ProductState = pro.State;
                }

            });
            //套装商品列表
            List<OrderProductInfo> suitOrderProductList = Carts.GetSuitOrderProductList(pmId, orderProductList, true);
            if (suitOrderProductList.Count > 0)//当套装已经存在
            {
                if (buyCount < 1)//当购买数量小于1时，删除此套装
                {
                    Carts.DeleteCartSuit(ref orderProductList, pmId);
                }
                else
                {
                    OrderProductInfo orderProductInfo = suitOrderProductList.Find(x => x.Type == 3);
                    int oldBuyCount = orderProductInfo.RealCount / orderProductInfo.ExtCode2;
                    if (buyCount != oldBuyCount)
                        Carts.AddExistSuitToCart(ref orderProductList, suitOrderProductList, buyCount);
                }
            }

            //商品数量
            int pCount = Carts.SumOrderProductCount(orderProductList);
            //店铺购物车列表
            List<StoreCartInfo> storeCartList = Carts.TidyMallOrderProductList(StringHelper.SplitString(selectedCartItemKeyList), orderProductList);

            //商品总数量
            int totalCount = Carts.SumMallCartOrderProductCount(storeCartList);
            //商品合计
            decimal productAmount = Carts.SumMallCartOrderProductAmount(storeCartList);
            //满减折扣
            int fullCut = Carts.SumMallCartFullCut(storeCartList);
            decimal PVAmount = Carts.SumMallPVAmount(storeCartList);
            //订单合计
            decimal orderAmount = productAmount - fullCut;

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCut = fullCut,
                PVAmount = PVAmount,
                OrderAmount = orderAmount,
                StoreCartList = storeCartList
            };

            //将购物车中商品数量写入cookie
            Carts.SetCartProductCountCookie(pCount);

            return View("ajaxindex", model);
        }

        /// <summary>
        /// 删除购物车中套装
        /// </summary>
        public ActionResult DelSuit()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pmId = WebHelper.GetQueryInt("pmId");//套装id
            int pos = WebHelper.GetQueryInt("pos");//位置
            string selectedCartItemKeyList = WebHelper.GetQueryString("selectedCartItemKeyList");//选中的购物车项键列表

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            orderProductList.ForEach(x =>
            {
                PartProductInfo pro = Products.GetPartProductById(x.Pid);

                if (pro != null)
                {
                    x.PV = x.ProductPV;
                    x.HaiMi = x.ProductHaiMi;
                    x.TaxRate = pro.TaxRate;
                    x.SaleType = pro.SaleType;
                    x.HongBaoCut = x.ProductHBCut;
                    x.MinBuyCount = pro.MinBuyCount;
                    x.ProductState = pro.State;
                }

            });
            Carts.DeleteCartSuit(ref orderProductList, pmId);

            //商品数量
            int pCount = Carts.SumOrderProductCount(orderProductList);
            //店铺购物车列表
            List<StoreCartInfo> storeCartList = Carts.TidyMallOrderProductList(StringHelper.SplitString(selectedCartItemKeyList), orderProductList);

            //商品总数量
            int totalCount = Carts.SumMallCartOrderProductCount(storeCartList);
            //商品合计
            decimal productAmount = Carts.SumMallCartOrderProductAmount(storeCartList);
            //满减折扣
            int fullCut = Carts.SumMallCartFullCut(storeCartList);
            //订单合计
            decimal orderAmount = productAmount - fullCut;

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCut = fullCut,
                OrderAmount = orderAmount,
                StoreCartList = storeCartList
            };

            //将购物车中商品数量写入cookie
            Carts.SetCartProductCountCookie(pCount);

            if (pos == 0)
                return View("ajaxindex", model);
            else
                return View("snap", model);
        }

        /// <summary>
        /// 获得满赠赠品
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFullSend()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pmId = WebHelper.GetQueryInt("pmId");//满赠id
            //获得满赠赠品列表
            List<PartProductInfo> fullSendPresentList = Promotions.GetFullSendPresentList(pmId);

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (PartProductInfo partProductInfo in fullSendPresentList)
            {
                sb.AppendFormat("{0}\"pid\":\"{1}\",\"name\":\"{2}\",\"storeId\":\"{3}\",\"shopPrice\":\"{4}\",\"showImg\":\"{5}\",\"url\":\"{6}\"{7},", "{", partProductInfo.Pid, partProductInfo.Name, partProductInfo.StoreId, partProductInfo.ShopPrice, partProductInfo.ShowImg, Url.Action("product", "catalog", new RouteValueDictionary { { "pid", partProductInfo.Pid } }), "}");
            }
            if (fullSendPresentList.Count > 0)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            return AjaxResult("success", sb.ToString(), true);
        }

        /// <summary>
        /// 添加满赠到购物车
        /// </summary>
        public ActionResult AddFullSend()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pmId = WebHelper.GetQueryInt("pmId");//满赠id
            int pid = WebHelper.GetQueryInt("pid");//商品id
            string selectedCartItemKeyList = WebHelper.GetQueryString("selectedCartItemKeyList");//选中的购物车项键列表

            //添加的商品
            PartProductInfo partProductInfo = Products.GetPartProductById(pid);
            if (partProductInfo == null)
                return AjaxResult("noproduct", "请选择商品");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(partProductInfo.StoreId);
            if (storeInfo.State != (int)StoreState.Open)
                return AjaxResult("noproduct", "请选择商品");

            //商品库存
            int stockNumber = Products.GetProductStockNumberByPid(pid);
            if (stockNumber < 1)
                return AjaxResult("stockout", "商品库存不足");

            //满赠促销活动
            FullSendPromotionInfo fullSendPromotionInfo = Promotions.GetFullSendPromotionByPmIdAndTime(pmId, DateTime.Now);
            if (partProductInfo == null)
                return AjaxResult("nopromotion", "满赠促销活动不存在或已经结束");

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid, WorkContext.Sid);

            //满赠主商品列表
            List<OrderProductInfo> fullSendMainOrderProductList = Carts.GetFullSendMainOrderProductList(pmId, orderProductList);
            if (fullSendMainOrderProductList.Count < 1)
                return AjaxResult("nolimit", "不符合活动条件");
            decimal amount = Carts.SumOrderProductAmount(fullSendMainOrderProductList);
            if (fullSendPromotionInfo.LimitMoney > amount)
                return AjaxResult("nolimit", "不符合活动条件");

            if (!Promotions.IsExistFullSendProduct(pmId, pid, 1))
                return AjaxResult("nofullsendproduct", "此商品不是满赠商品");

            //赠送商品
            OrderProductInfo fullSendMinorOrderProductInfo = Carts.GetFullSendMinorOrderProduct(pmId, orderProductList);
            if (fullSendMinorOrderProductInfo != null)
            {
                if (fullSendMinorOrderProductInfo.Pid != pid)
                    Carts.DeleteCartFullSend(ref orderProductList, fullSendMinorOrderProductInfo);
                else
                    return AjaxResult("exist", "此商品已经添加");
            }

            //添加满赠商品
            Carts.AddFullSendToCart(ref orderProductList, partProductInfo, fullSendPromotionInfo, WorkContext.Sid, WorkContext.Uid, DateTime.Now);

            //商品数量
            int pCount = Carts.SumOrderProductCount(orderProductList);
            //店铺购物车列表
            List<StoreCartInfo> storeCartList = Carts.TidyMallOrderProductList(StringHelper.SplitString(selectedCartItemKeyList), orderProductList);

            //商品总数量
            int totalCount = Carts.SumMallCartOrderProductCount(storeCartList);
            //商品合计
            decimal productAmount = Carts.SumMallCartOrderProductAmount(storeCartList);
            //满减折扣
            int fullCut = Carts.SumMallCartFullCut(storeCartList);
            //订单合计
            decimal orderAmount = productAmount - fullCut;

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCut = fullCut,
                OrderAmount = orderAmount,
                StoreCartList = storeCartList
            };

            //将购物车中商品数量写入cookie
            Carts.SetCartProductCountCookie(pCount);

            return View("ajaxindex", model);
        }

        /// <summary>
        /// 删除购物车中满赠
        /// </summary>
        public ActionResult DelFullSend()
        {
            //当商城不允许游客使用购物车时
            if (WorkContext.MallConfig.IsGuestSC == 0 && WorkContext.Uid < 1)
                return AjaxResult("nologin", "请先登录");

            int pmId = WebHelper.GetQueryInt("pmId");//满赠id
            int pos = WebHelper.GetQueryInt("pos");//位置
            string selectedCartItemKeyList = WebHelper.GetQueryString("selectedCartItemKeyList");//选中的购物车项键列表

            //购物车商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid, WorkContext.Sid);
            //删除满赠
            Carts.DeleteCartFullSend(ref orderProductList, pmId);

            //商品数量
            int pCount = Carts.SumOrderProductCount(orderProductList);
            //店铺购物车列表
            List<StoreCartInfo> storeCartList = Carts.TidyMallOrderProductList(StringHelper.SplitString(selectedCartItemKeyList), orderProductList);

            //商品总数量
            int totalCount = Carts.SumMallCartOrderProductCount(storeCartList);
            //商品合计
            decimal productAmount = Carts.SumMallCartOrderProductAmount(storeCartList);
            //满减折扣
            int fullCut = Carts.SumMallCartFullCut(storeCartList);
            //订单合计
            decimal orderAmount = productAmount - fullCut;

            CartModel model = new CartModel
            {
                TotalCount = totalCount,
                ProductAmount = productAmount,
                FullCut = fullCut,
                OrderAmount = orderAmount,
                StoreCartList = storeCartList
            };

            //将购物车中商品数量写入cookie
            Carts.SetCartProductCountCookie(pCount);

            if (pos == 0)
                return View("ajaxindex", model);
            else
                return View("snap", model);
        }

        /// <summary>
        /// 满赠主商品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult FullSendMainProductList()
        {
            //满赠促销活动id
            int pmId = GetRouteInt("pmId");
            if (pmId == 0)
                pmId = WebHelper.GetQueryInt("pmId");
            //开始价格
            int startPrice = GetRouteInt("startPrice");
            if (startPrice == 0)
                startPrice = WebHelper.GetQueryInt("startPrice");
            //结束价格
            int endPrice = GetRouteInt("endPrice");
            if (endPrice == 0)
                endPrice = WebHelper.GetQueryInt("endPrice");
            //排序列
            int sortColumn = GetRouteInt("sortColumn");
            if (sortColumn == 0)
                sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = GetRouteInt("sortDirection");
            if (sortDirection == 0)
                sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = GetRouteInt("page");
            if (page == 0)
                page = WebHelper.GetQueryInt("page");

            //满赠促销活动
            FullSendPromotionInfo fullSendPromotionInfo = Promotions.GetFullSendPromotionByPmIdAndTime(pmId, DateTime.Now);
            if (fullSendPromotionInfo == null)
                return PromptView(Url.Action("index"), "促销活动不存在");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(fullSendPromotionInfo.StoreId);
            if (storeInfo == null || storeInfo.State != (int)StoreState.Open)
                return PromptView(Url.Action("index"), "店铺不存在");

            PageModel pageModel = new PageModel(20, page, Promotions.GetFullSendMainProductCount(pmId, startPrice, endPrice));

            FullSendMainProductListModel model = new FullSendMainProductListModel()
            {
                PmId = pmId,
                StartPrice = startPrice,
                EndPrice = endPrice,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                PageModel = pageModel,
                ProductList = Promotions.GetFullSendMainProductList(pageModel.PageSize, pageModel.PageNumber, pmId, startPrice, endPrice, sortColumn, sortDirection),
                OrderProductList = Carts.GetCartProductList(WorkContext.Uid, WorkContext.Sid)
            };

            return View(model);
        }

        /// <summary>
        /// 满减商品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult FullCutProductList()
        {
            //店铺id
            int storeId = GetRouteInt("storeId");
            if (storeId == 0)
                storeId = WebHelper.GetQueryInt("storeId");
            //满减促销活动id
            int pmId = GetRouteInt("pmId");
            if (pmId == 0)
                pmId = WebHelper.GetQueryInt("pmId");
            //开始价格
            int startPrice = GetRouteInt("startPrice");
            if (startPrice == 0)
                startPrice = WebHelper.GetQueryInt("startPrice");
            //结束价格
            int endPrice = GetRouteInt("endPrice");
            if (endPrice == 0)
                endPrice = WebHelper.GetQueryInt("endPrice");
            //排序列
            int sortColumn = GetRouteInt("sortColumn");
            if (sortColumn == 0)
                sortColumn = WebHelper.GetQueryInt("sortColumn");
            //排序方向
            int sortDirection = GetRouteInt("sortDirection");
            if (sortDirection == 0)
                sortDirection = WebHelper.GetQueryInt("sortDirection");
            //当前页数
            int page = GetRouteInt("page");
            if (page == 0)
                page = WebHelper.GetQueryInt("page");

            //满减促销活动
            FullCutPromotionInfo fullCutPromotionInfo = Promotions.GetFullCutPromotionByStoreIdAndPmIdAndTime(storeId, pmId, DateTime.Now);
            if (fullCutPromotionInfo == null)
                return PromptView(Url.Action("index"), "促销活动不存在");

            //店铺信息
            StoreInfo storeInfo = Stores.GetStoreById(fullCutPromotionInfo.StoreId);
            if (storeInfo == null || storeInfo.State != (int)StoreState.Open)
                return PromptView(Url.Action("index"), "店铺不存在");

            PageModel pageModel = new PageModel(20, page, Promotions.GetFullCutProductCount(fullCutPromotionInfo, startPrice, endPrice));

            FullCutProductListModel model = new FullCutProductListModel()
            {
                StoreId = storeId,
                PmId = pmId,
                StartPrice = startPrice,
                EndPrice = endPrice,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                PageModel = pageModel,
                ProductList = Promotions.GetFullCutProductList(pageModel.PageSize, pageModel.PageNumber, fullCutPromotionInfo, startPrice, endPrice, sortColumn, sortDirection),
                OrderProductList = Carts.GetCartProductList(WorkContext.Uid, WorkContext.Sid)
            };

            return View(model);
        }

        #endregion
    }
}
