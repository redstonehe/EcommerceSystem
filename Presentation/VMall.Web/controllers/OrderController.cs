using System;
using System.Text;
using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Models;

using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace VMall.Web.Controllers
{
    /// <summary>
    /// 订单控制器类
    /// </summary>
    public partial class OrderController : BaseWebController
    {
        private static object _locker = new object();//锁对象

        private static string dirSaleApiUrl = WebSiteConfig.DirsaleApiUrl;

        /// <summary>
        /// 确认订单
        /// </summary>
        public ActionResult ConfirmOrder()
        {
            //选中的购物车项键列表
            string selectedCartItemKeyList = WebHelper.GetRequestString("selectedCartItemKeyList");

            //选中的店铺项键列表
            string selectedStoreKeyList = WebHelper.GetRequestString("selectedStoreKeyList");
            //配送地址id
            int saId = WebHelper.GetFormInt("saId");
            //支付插件名称
            string payName = WebHelper.GetFormString("payName");
            //是否选择汇购卡
            int selectCashPay = WebHelper.GetFormInt("selectCashPay");


            //订单商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid);
            //更新购物车价格
            orderProductList = Carts.UpdateCartForPromotionActive(orderProductList, WorkContext.Uid, WorkContext.Sid);
            //更新不参与汇购卡的活动产品
            orderProductList = Carts.UpdateCartForSpecialPrice(orderProductList, WorkContext.Uid, WorkContext.Sid, selectCashPay == 1 ? true : false);
            //代理
            if (orderProductList.Exists(x => x.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId")) && WorkContext.PartUserInfo.AgentType <= 0)
                orderProductList = Carts.UpdateCartForAgent(orderProductList, WorkContext.Uid, WorkContext.Sid);
            //公司员工内部购买
            if (WorkContext.UserRankInfo.UserRid == 10)
                orderProductList = Carts.UpdateCartForCompanyUser(orderProductList, WorkContext.Uid, WorkContext.Sid);
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
                    x.StoreSTid = pro.StoreSTid;
                    x.Weight = pro.Weight;
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
            //是否使用汇购卡券--判断标准选中购物车存在旗舰店专区的产品  旗舰店专区id为1 并且用户可用汇购卡张数大于0

            bool isUserCashCoupon = WorkContext.IsDirSaleUser ? Channel.GetCartProductChanId(WorkContext.Uid, selectedCartItemKeyList).Distinct<int>().ToList().Exists(x => x == 1) : false;
            int AvailCash = WorkContext.IsDirSaleUser ? CashCoupon.GetRecordCount(string.Format(" Uid={0} AND CouponType=1 AND Banlance>0 AND ValidTime>='{1}'", WorkContext.Uid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))) : 0;
            if (orderProductList.Count < 1)
                return PromptView("购物车中没有商品，请先添加商品");

            //店铺购物车列表
            List<StoreCartInfo> storeCartList = Carts.TidyMallOrderProductList(StringHelper.SplitString(selectedCartItemKeyList), orderProductList, StringHelper.SplitString(selectedStoreKeyList));
            if (Carts.SumMallCartOrderProductCount(storeCartList) < 1)
                return PromptView("请先选择购物车商品");
            if (WorkContext.PartUserInfo.AgentType <= 0)
            {
                StoreCartInfo scInfo = storeCartList.Find(x => x.StoreInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"));
                if (scInfo != null)
                {
                    //if (Carts.SumOrderProductAgentOrginAmount(scInfo.SelectedOrderProductList) > 10000)
                    //{
                    //    if (storeCartList.Exists(x => x.StoreInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId")) && !storeCartList.Exists(x => x.StoreInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId")) && storeCartList.Exists(x => !x.SelectedOrderProductList.Exists(p => StringHelper.StrContainsForNum(WebHelper.GetConfigSettings("AgentSuitPid"), p.Pid.ToString()))))
                    //        return PromptView("代理套餐必选");
                    //    if (storeCartList.Exists(x => x.StoreInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId")) && !storeCartList.Exists(x => x.SelectedOrderProductList.Exists(p => StringHelper.StrContainsForNum(WebHelper.GetConfigSettings("AgentSuitPid"), p.Pid.ToString()))))
                    //        return PromptView("代理套餐为必选");
                    //}
                    if (storeCartList.Exists(x => x.StoreInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId")) && WorkContext.PartUserInfo.AgentType <= 0)
                    {
                        List<OrderProductInfo> selectAgentOrderProduct = storeCartList.Find(x => x.StoreInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId")).SelectedOrderProductList;
                        orderProductList = Carts.UpdateCartForAgent(selectAgentOrderProduct, WorkContext.Uid, WorkContext.Sid);
                        //公司员工内部购买
                        if (WorkContext.UserRankInfo.UserRid == 10)
                            orderProductList = Carts.UpdateCartForCompanyUser(orderProductList, WorkContext.Uid, WorkContext.Sid);
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
                                x.StoreSTid = pro.StoreSTid;
                                x.Weight = pro.Weight;
                                x.ProductState = pro.State;
                            }
                        });
                        storeCartList = Carts.TidyMallOrderProductList(StringHelper.SplitString(selectedCartItemKeyList), orderProductList, StringHelper.SplitString(selectedStoreKeyList));
                    }
                }

            }


            ConfirmOrderModel model = new ConfirmOrderModel();
            model.SelectCashPay = selectCashPay == 0 ? false : true;

            model.isUserCsahCoupon = isUserCashCoupon && AvailCash > 0 && WorkContext.IsDirSaleUser;
            model.SelectedCartItemKeyList = selectedCartItemKeyList;
            if (string.IsNullOrEmpty(selectedStoreKeyList))
                model.SelectedStoreKeyList = Carts.GetSelectStoreIdList(storeCartList);
            else
                model.SelectedStoreKeyList = selectedStoreKeyList;
            if (saId > 0)
                model.DefaultFullShipAddressInfo = ShipAddresses.GetFullShipAddressBySAId(saId, WorkContext.Uid);
            if (model.DefaultFullShipAddressInfo == null)
                model.DefaultFullShipAddressInfo = ShipAddresses.GetDefaultFullShipAddress(WorkContext.Uid);

            if (payName.Length > 0)
                model.DefaultPayPluginInfo = Plugins.GetPayPluginBySystemName(payName);
            if (model.DefaultPayPluginInfo == null)
                model.DefaultPayPluginInfo = Plugins.GetDefaultPayPlugin();
            model.PayPluginList = Plugins.GetPayPluginList();

            model.PayCreditName = Credits.PayCreditName;
            model.UserPayCredits = WorkContext.PartUserInfo.PayCredits;
            model.MaxUsePayCredits = Credits.GetOrderMaxUsePayCredits(WorkContext.PartUserInfo.PayCredits);

            model.UserAccountInfo = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid);
            model.CashCouponList = CashCoupon.GetList(string.Format("  uid={0} AND CouponType=1 AND Banlance>0 AND ValidTime>='{1}' ", WorkContext.Uid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));

            GroupProductInfo gpInfo = new GroupProducts().GetModel(3);
            List<StoreOrder> storeOrderList = new List<StoreOrder>();
            List<OrderProductInfo> exorderproductlist = new List<OrderProductInfo>();
            bool isSend = true;
            foreach (StoreCartInfo item in storeCartList)
            {
                StoreOrder storeOrder = new StoreOrder();
                storeOrder.StoreCartInfo = item;
                storeOrder.ProductAmount = Carts.SumOrderProductAmount(item.SelectedOrderProductList);
                storeOrder.FullCut = Carts.SumFullCut(item.CartItemList);

                storeOrder.ShipFee = model.DefaultFullShipAddressInfo != null ? Orders.GetShipFee(model.DefaultFullShipAddressInfo.ProvinceId, model.DefaultFullShipAddressInfo.CityId, item.SelectedOrderProductList, ref isSend) : 0;// model.DefaultFullShipAddressInfo != null ? Orders.GetShipFee(item.StoreInfo.StoreId, storeOrder.ProductAmount) : 0;// 
                decimal oldShipFee = storeOrder.ShipFee;
                storeOrder.ShipFee = Orders.GetDoublee11ShipFee(item.SelectedOrderProductList, oldShipFee, gpInfo);

                storeOrder.ShipTempList = Stores.GetStoreShipTemplateListByIds(string.Join(",", item.SelectedOrderProductList.Select(x => x.StoreSTid)));
                storeOrder.isSend = isSend;
                storeOrder.TotalCount = Carts.SumOrderProductCount(item.SelectedOrderProductList);
                storeOrder.TotalWeight = Carts.SumOrderProductWeight(item.SelectedOrderProductList);
                storeOrder.TaxFee = Carts.SumOrderProductTaxAmount(item.SelectedOrderProductList);
                storeOrder.HongBaoCutFee = Carts.SumOrderProductHongBaoCutAmount(item.SelectedOrderProductList);
                storeOrderList.Add(storeOrder);

                model.AllHonBaoCutFee += storeOrder.HongBaoCutFee;
                model.TaxFee += storeOrder.TaxFee;
                model.AllShipFee += storeOrder.ShipFee;
                model.AllFullCut += storeOrder.FullCut;
                model.AllProductAmount += storeOrder.ProductAmount;
                model.AllTotalCount += storeOrder.TotalCount;
                model.AllTotalWeight += storeOrder.TotalWeight;
                exorderproductlist.AddRange(item.SelectedOrderProductList);
            }

            model.AllSelectOrderProductList = exorderproductlist;
            bool useExCode = false;
            //可用兑换码列表
            List<ExChangeCouponsInfo> vaildEexCodeList = new List<ExChangeCouponsInfo>();
            //所有可用兑换码列表 该会员的有效的未使用兑换码
            List<ExChangeCouponsInfo> allExCodeList = ExChangeCoupons.GetList(string.Format(" uid={0} and state=1 and oid=0 and validtime>getdate()", WorkContext.Uid));
            exorderproductlist = exorderproductlist.FindAll(x => x.Type == 0 && x.ExtCode1 > 0);
            if (exorderproductlist.Count > 0)
            {
                List<SinglePromotionInfo> exsinglelist = Promotions.GetSingleByStrWhere(string.Format(" pid IN (" + string.Join(",", exorderproductlist.Select(x => x.Pid)) + ") AND state=1 AND (( [endtime1]>@nowtime AND [starttime1]<=@nowtime ) OR ( [endtime2]>@nowtime AND [starttime2]<=@nowtime ) OR ( [endtime3]>@nowtime AND [starttime3]<=@nowtime )) AND  discounttype=6 "));
                if (exsinglelist.Count > 0)
                {
                    useExCode = true;
                    vaildEexCodeList = ExChangeCoupons.GetVaildListByPid(string.Format(" pid IN (" + string.Join(",", exsinglelist.Select(x => x.Pid)) + ") and uid={0} and state=1 and oid=0 and validtime>getdate()  ", WorkContext.Uid));
                }
            }
            model.VaildEexCodeList = vaildEexCodeList;
            model.AllExCodeList = allExCodeList;
            model.UseExCode = useExCode;
            model.StoreOrderList = storeOrderList;
            model.AvailCashCouponAmount = Channel.GetChannelProductAmount(WorkContext.Uid, 1, selectedCartItemKeyList);
            //代理部分
            //model.AvailAgentAmount=storeOrderList.FindAll(x=>x.StoreCartInfo.StoreInfo.StoreId==WebHelper.GetConfigSettingsInt("AgentStoreId")||x.StoreCartInfo.StoreInfo.StoreId==WebHelper.GetConfigSettingsInt("AgentSuitStoreId")).Sum(x=>x.ProductAmount);
            model.isUserAgentDiscount = model.UserAccountInfo.FindAll(x => x.AccountId == (int)AccountType.代理账户).Exists(x => x.Banlance > 0);
            model.isUserCommisionDiscount = model.UserAccountInfo.FindAll(x => x.AccountId == (int)AccountType.佣金账户).Exists(x => x.Banlance > 0);

            IPayPlugin payPlugin = (IPayPlugin)model.DefaultPayPluginInfo.Instance;
            model.PayFee = payPlugin.GetPayFee(model.AllProductAmount, DateTime.Now, WorkContext.PartUserInfo);
            model.AllOrderAmount = model.AllProductAmount - model.AllFullCut + model.AllShipFee + model.PayFee + (model.TaxFee > 50 ? model.TaxFee : 0);




            model.IsVerifyCode = CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages);

            return View(model);
        }

        #region 不登陆直接购买
        /// <summary>
        /// 确认订单--直接购买
        /// </summary>
        public ActionResult ConfirmOrder_DirectBuy()
        {
            //选中的购物车项键列表
            int pid = WebHelper.GetRequestInt("pid");
            if (pid == 0)
                pid = WebHelper.GetFormInt("pid");
            int pcount = WebHelper.GetFormInt("pcount");
            string payName = WebHelper.GetFormString("payName");//支付插件名称 "custompay";// 

            PartProductInfo product = Products.GetPartProductById(pid);
            if (product == null)
                return PromptView("商品已下架或商品不存在，请选择正确商品");

            ConfirmOrder_AgentModel model = new ConfirmOrder_AgentModel();

            if (payName.Length > 0)
                model.DefaultPayPluginInfo = Plugins.GetPayPluginBySystemName(payName);
            if (model.DefaultPayPluginInfo == null)
                model.DefaultPayPluginInfo = Plugins.GetDefaultPayPlugin();
            model.PayPluginList = Plugins.GetPayPluginList();// new List<PluginInfo>() { Plugins.GetPayPluginBySystemName(payName) };// 

            IPayPlugin payPlugin = (IPayPlugin)model.DefaultPayPluginInfo.Instance;
            model.AllProductAmount = product.ShopPrice * 1;
            model.AllOrderAmount = model.AllProductAmount;
            model.Product = product;
            model.Pid = pid.ToString();

            return View(model);
        }
        /// <summary>
        /// 提交直接购买订单
        /// </summary>
        /// <returns></returns>
        public ActionResult SubmitOrder_DirectBuy()
        {
            lock (_locker)
            {
                #region 获取表单参数
                int pid = WebHelper.GetRequestInt("pid");
                if (pid == 0)
                    pid = WebHelper.GetFormInt("pid");
                int pcount = WebHelper.GetFormInt("pcount");
                string payName = WebHelper.GetFormString("payName");//支付插件名称
                string consignee = WebHelper.GetFormString("consignee");//
                string mobile = WebHelper.GetFormString("mobile");//
                string idcard = WebHelper.GetFormString("idcard");
                int regionid = WebHelper.GetFormInt("regionid");//
                string address = WebHelper.GetFormString("address");//
                string buyerRemark1 = WebHelper.GetFormString("buyerRemark");//
                string buyerRemark = buyerRemark1 + "-身份证:" + idcard;
                #endregion

                #region 提交参数验证
                PartProductInfo product = Products.GetPartProductById(pid);
                if (product == null)
                    return AjaxResult("errorparouct", "商品已下架或商品不存在，请选择正确商品");

                //验证支付方式是否为空
                PluginInfo payPluginInfo = Plugins.GetPayPluginBySystemName(payName);
                if (payPluginInfo == null)
                    return AjaxResult("empaypay", "请选择支付方式");

                #endregion

                //验证商品库存
                ProductStockInfo productStockInfo = Products.GetProductStockByPid(pid);
                if (productStockInfo.Number <= 0)
                    return AjaxResult("outstock", "商品库存不足");

                ConfirmOrder_AgentModel model = new ConfirmOrder_AgentModel();
                model.AllProductAmount = product.ShopPrice;
                model.AllOrderAmount = model.AllProductAmount;
                model.Product = product;
                model.Pid = pid.ToString();

                //验证支付金额必须大于0，即优惠金额必须小于订单总金额
                if (model.AllOrderAmount < 0)
                    return AjaxResult("overamount", "支付金额必须大于0");

                //验证已经通过,进行订单保存
                int pCount = pcount;
                string oidList = "";
                decimal AllMoney = 0;

                PartUserInfo orderUser = WorkContext.Uid < 1 ? new PartUserInfo() : WorkContext.PartUserInfo;

                StoreInfo storeInfo = Stores.GetStoreById(product.StoreId);
                OrderInfo orderInfo = Orders.CreateOrder_DirectBuy(orderUser, storeInfo, product, payPluginInfo, consignee, mobile, regionid, address, WorkContext.IP, pCount, pid, buyerRemark, 1);

                if (orderInfo != null)
                {
                    oidList += orderInfo.Oid + ",";
                    AllMoney += orderInfo.SurplusMoney;
                }
                else
                    return AjaxResult("error", "提交失败!请重新提交");

                if (AllMoney > 0)
                    return AjaxResult("success", Url.Action("payshow", new RouteValueDictionary { { "oidList", oidList.TrimEnd(',') } }));
                else
                    return AjaxResult("success", Url.Action("submitresult", new RouteValueDictionary { { "oidList", oidList.TrimEnd(',') } }));
            }
        }
        #endregion


        #region 优惠券验证
        /// <summary>
        /// 获得有效的优惠劵列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetValidCouponList()
        {
            //选中的购物车项键列表
            string selectedCartItemKeyList = WebHelper.GetQueryString("selectedCartItemKeyList");
            //选中的店铺项键列表
            string selectedStoreKeyList = WebHelper.GetRequestString("selectedStoreKeyList");
            //是否选择汇购卡
            int selectCashPay = WebHelper.GetFormInt("selectCashPay");
            //购物车商品列表
            List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid);
            //更新购物车价格
            orderProductList = Carts.UpdateCartForPromotionActive(orderProductList, WorkContext.Uid, WorkContext.Sid);
            //更新不参与汇购卡的活动产品
            orderProductList = Carts.UpdateCartForSpecialPrice(orderProductList, WorkContext.Uid, WorkContext.Sid, selectCashPay == 1 ? true : false);
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
                    x.StoreSTid = pro.StoreSTid;
                    x.Weight = pro.Weight;
                    x.ProductState = pro.State;
                }
            });
            //店铺购物车列表
            List<StoreCartInfo> storeCartList = Carts.TidyMallOrderProductList(StringHelper.SplitString(selectedCartItemKeyList), orderProductList, StringHelper.SplitString(selectedStoreKeyList));

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            DataTable dt = Coupons.GetUnUsedCouponList(WorkContext.Uid);
            foreach (DataRow row in dt.Rows)
            {
                if (TypeHelper.ObjectToInt(row["state"]) == 0)
                {
                    continue;
                }
                else if (TypeHelper.ObjectToInt(row["useexpiretime"]) == 0 && TypeHelper.ObjectToDateTime(row["usestarttime"]) > DateTime.Now)
                {
                    continue;
                }
                else if (TypeHelper.ObjectToInt(row["useexpiretime"]) == 0 && TypeHelper.ObjectToDateTime(row["useendtime"]) <= DateTime.Now)
                {
                    continue;
                }
                //else if (TypeHelper.ObjectToInt(row["useexpiretime"]) > 0 && TypeHelper.ObjectToDateTime(row["activatetime"]) <= DateTime.Now.AddDays(-1 * TypeHelper.ObjectToInt(row["useexpiretime"])))
                //{
                //    continue;
                //}
                else if (TypeHelper.ObjectToDateTime(row["activatetime"]) > DateTime.Now)
                {
                    continue;
                }
                else if (TypeHelper.ObjectToDateTime(row["validtime"]) <= DateTime.Now)
                {
                    continue;
                }
                else if (TypeHelper.ObjectToInt(row["userranklower"]) > WorkContext.UserRid)
                {
                    continue;
                }
                else
                {
                    int storeId = TypeHelper.ObjectToInt(row["storeid"]);
                    if (storeId != -1)
                    {
                        StoreCartInfo storeCartInfo = storeCartList.Find(x => x.StoreInfo.StoreId == storeId);
                        if (storeCartInfo == null)
                        {
                            continue;
                        }
                        if (TypeHelper.ObjectToInt(row["orderamountlower"]) > Carts.SumOrderProductAmount(storeCartInfo.SelectedOrderProductList))
                        {
                            continue;
                        }
                        else
                        {
                            int limitStoreCid = TypeHelper.ObjectToInt(row["limitstorecid"]);
                            if (limitStoreCid > 0)
                            {
                                foreach (OrderProductInfo orderProductInfo in storeCartInfo.SelectedOrderProductList)
                                {
                                    if (orderProductInfo.Type == 0 && orderProductInfo.StoreCid != limitStoreCid)
                                    {
                                        continue;
                                    }
                                }
                            }

                            if (TypeHelper.ObjectToInt(row["limitproduct"]) == 1)
                            {
                                List<OrderProductInfo> commonOrderProductList = Carts.GetCommonOrderProductList(storeCartInfo.SelectedOrderProductList);
                                if (!Coupons.IsSameCouponType(TypeHelper.ObjectToInt(row["coupontypeid"]), Carts.GetPidList(commonOrderProductList)))
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(row["channelid"].ToString()))
                        {
                            List<OrderProductInfo> allProduct = new List<OrderProductInfo>();
                            foreach (var item in storeCartList)
                            {
                                foreach (var detail in item.SelectedOrderProductList)
                                {
                                    List<int> channelIdList = Channel.GetProductChannels(detail.Pid);
                                    List<int> active_10_1 = new List<int>() { 6, 9 };
                                    if (channelIdList.Exists(x => active_10_1.Exists(p => p == x)))
                                    {
                                        allProduct.Add(detail);
                                    }
                                }
                            }
                            if (TypeHelper.ObjectToInt(row["orderamountlower"]) > Carts.SumOrderProductAmount(allProduct))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            List<OrderProductInfo> allProduct = new List<OrderProductInfo>();
                            foreach (var item in storeCartList)
                            {
                                allProduct.AddRange(item.SelectedOrderProductList);
                            }
                            if (TypeHelper.ObjectToInt(row["orderamountlower"]) > Carts.SumOrderProductAmount(allProduct))
                            {
                                continue;
                            }
                        }
                        //if (TypeHelper.ObjectToInt(row["limitproduct"]) == 1)
                        //{
                        //    List<OrderProductInfo> commonOrderProductList = Carts.GetCommonOrderProductList(allProduct);
                        //    if (!Coupons.IsSameCouponType(TypeHelper.ObjectToInt(row["coupontypeid"]), Carts.GetPidList(commonOrderProductList)))
                        //    {
                        //        continue;
                        //    }
                        //}
                    }
                }
                sb.AppendFormat("{0}\"couponId\":\"{1}\",\"name\":\"{2}\",\"money\":\"{3}\",\"useMode\":\"{4}\"{5},", "{", row["couponid"], row["name"], row["money"], row["usemode"], "}");
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            return AjaxResult("success", sb.ToString(), true);
        }

        /// <summary>
        /// 验证优惠劵编号
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifyCouponSN()
        {
            string couponSN = WebHelper.GetQueryString("couponSN");//优惠劵编号
            if (couponSN.Length == 0)
                return AjaxResult("emptycouponsn", "请输入优惠劵编号");
            if (couponSN.Length != 16)
                return AjaxResult("errorcouponsn", "优惠劵编号不正确");

            CouponInfo couponInfo = Coupons.GetCouponByCouponSN(couponSN);
            if (couponInfo == null)//不存在
                return AjaxResult("noexist", "优惠劵不存在");
            return AjaxResult("success", "此优惠劵可以正常使用");
        }

        #endregion

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <returns></returns>
        public ActionResult SubmitOrder()
        {
            lock (_locker)
            {
                #region 获取表单参数
                string selectedCartItemKeyList = WebHelper.GetFormString("selectedCartItemKeyList");//选中的购物车项键列表
                //选中的店铺项键列表
                string selectedStoreKeyList = WebHelper.GetRequestString("selectedStoreKeyList");
                int saId = WebHelper.GetFormInt("saId");//配送地址id
                string payName = WebHelper.GetFormString("payName");//支付方式名称
                int payCreditCount = WebHelper.GetFormInt("payCreditCount");//支付积分
                decimal haiMiCount = TypeHelper.StringToDecimal(WebHelper.GetFormString("haiMiCount"));//支付海米
                decimal hongBaoCount = TypeHelper.StringToDecimal(WebHelper.GetFormString("hongBaoCount"));//支付红包
                decimal cashCount = TypeHelper.StringToDecimal(WebHelper.GetFormString("cashCount"));//使用汇购卡券金额
                string cashId = WebHelper.GetFormString("cashId");//使用汇购卡id
                string payPswd = WebHelper.GetFormString("payPswd");//支付密码

                string[] couponIdList = StringHelper.SplitString(WebHelper.GetFormString("couponIdList"));//客户已经激活的优惠劵
                string[] couponSNList = StringHelper.SplitString(WebHelper.GetFormString("couponSNList"));//客户还未激活的优惠劵
                int fullCut = WebHelper.GetFormInt("fullCut");//满减金额
                DateTime bestTime = TypeHelper.StringToDateTime(WebHelper.GetFormString("bestTime"), new DateTime(1970, 1, 1));//最佳配送时间
                string buyerRemark = WebHelper.GetFormString("buyerRemark");//买家备注
                string invoice = WebHelper.GetFormString("invoice");//发票抬头
                string verifyCode = WebHelper.GetFormString("verifyCode");//验证码

                string isuseridcard = WebHelper.GetFormString("isuseridcard");//是否需要身份证
                string idcard = WebHelper.GetFormString("idcard");//身份证
                int selectCashPay = WebHelper.GetFormInt("selectCashPay");//汇购卡支付选择

                //string exchangecode = WebHelper.GetFormString("exchangecode");
                string selectexIds = WebHelper.GetFormString("selectexIds");
                string exselectname = WebHelper.GetFormString("exselectname");

                decimal daiLiCount = TypeHelper.StringToDecimal(WebHelper.GetFormString("daiLiCount"));//代理账户支付
                decimal YongJinCount = TypeHelper.StringToDecimal(WebHelper.GetFormString("YongJinCount"));//佣金账户支付


                string invoice_title = WebHelper.GetFormString("invoice_title");
                string invoice_id = WebHelper.GetFormString("invoice_id");
                string invoice_regaddr = WebHelper.GetFormString("invoice_regaddr");
                string invoice_regmobile = WebHelper.GetFormString("invoice_regmobile");
                string invoice_bank = WebHelper.GetFormString("invoice_bank");
                string invoice_bankno = WebHelper.GetFormString("invoice_bankno");
                int invoicetype = WebHelper.GetFormInt("invoicetype", -1);
                int invoicebodyvalue = WebHelper.GetFormInt("invoicebodyvalue", -1);
                #endregion

                #region 提交参数验证
                if (StringHelper.ArrayContainsForNum(WebSiteConfig.QuanQiuGouStoreId, selectedStoreKeyList) && string.IsNullOrEmpty(idcard))
                    return AjaxResult("wrongidcard", "身份证不能为空");
                if (isuseridcard == "1")
                    buyerRemark = buyerRemark + "-身份证:" + idcard;

                if (haiMiCount < 0)
                    return AjaxResult("errorcount", MallKey.MallDiscountName_JiangJin+"抵扣不能为负数");
                if (hongBaoCount < 0)
                    return AjaxResult("errorcount", MallKey.MallDiscountName_JiFen+"减免不能为负数");
                if (cashCount < 0)
                    return AjaxResult("errorcount", "汇购卡券不能为负数");
                //验证验证码
                if (CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.MallConfig.VerifyPages))
                {
                    if (string.IsNullOrWhiteSpace(verifyCode))
                    {
                        return AjaxResult("emptyverifycode", "验证码不能为空");
                    }
                    else if (verifyCode.ToLower() != Sessions.GetValueString(WorkContext.Sid, "verifyCode"))
                    {
                        return AjaxResult("wrongverifycode", "验证码不正确");
                    }
                }

                //验证发票抬头长度
                if (StringHelper.GetStringLength(invoice) > 60)
                    return AjaxResult("muchinvoice", "发票抬头最多填写30个字");
                //验证发票
                string incoicemore = "";
                if (invoicetype == 0 || invoicetype == 1)
                {
                    if (invoicebodyvalue < 0)
                        return AjaxResult("errorinvoice", "选择开票主体");
                    if (invoicebodyvalue == 0)
                    {
                        if (invoice_title == "")
                            return AjaxResult("muchinvoice", "填写抬头");
                    }
                    if (invoicebodyvalue == 1)
                    {
                        if (invoice_title == "" || invoice_id == "" || invoice_regaddr == "" || invoice_regmobile == "" || invoice_bank == "" || invoice_bankno == "")
                            return AjaxResult("muchinvoice", "填写完整的发票信息");
                    }
                    if (invoicetype == 0 && invoicebodyvalue == 0)
                        incoicemore = "个人普通发票，发票抬头：" + invoice_title;
                    if(invoicetype == 0 && invoicebodyvalue == 1)
                        incoicemore = "单位普通发票，发票抬头：" + invoice_title + "，纳税人识别号：" + invoice_id + "，注册地址：" + invoice_regaddr + "，注册电话：" + invoice_regmobile + "，开户行：" + invoice_bank + "，银行帐号：" + invoice_bankno;
                    if(invoicetype == 1 && invoicebodyvalue == 1)
                        incoicemore = "增值税专用发票，发票抬头：" + invoice_title + "，纳税人识别号：" + invoice_id + "，注册地址：" + invoice_regaddr + "，注册电话：" + invoice_regmobile + "，开户行：" + invoice_bank + "，银行帐号：" + invoice_bankno;
                }
                
                

                //验证买家备注的内容长度
                if (StringHelper.GetStringLength(buyerRemark) > 120)
                    return AjaxResult("muchbuyerremark", "备注最多填写60个字");

                //验证支付方式是否为空
                PluginInfo payPluginInfo = Plugins.GetPayPluginBySystemName(payName);
                if (payPluginInfo == null)
                    return AjaxResult("empaypay", "请选择支付方式");

                //验证配送地址是否为空
                FullShipAddressInfo fullShipAddressInfo = ShipAddresses.GetFullShipAddressBySAId(saId, WorkContext.Uid);
                if (fullShipAddressInfo == null)
                    return AjaxResult("emptysaid", "请选择配送地址");

                #endregion

                //
                //if(Orders.GetOrdertCountByToday(WorkContext.Uid)>=8)
                //    return AjaxResult("emptycart", "提交订单频繁，请稍后再试");
                //购物车商品列表
                List<OrderProductInfo> orderProductList = Carts.GetCartProductList(WorkContext.Uid);
                orderProductList = Carts.UpdateCartForPromotionActive(orderProductList, WorkContext.Uid, WorkContext.Sid);
                //更新不参与汇购卡的活动产品
                orderProductList = Carts.UpdateCartForSpecialPrice(orderProductList, WorkContext.Uid, WorkContext.Sid, selectCashPay == 1 ? true : false);

                //公司员工内部购买
                if (WorkContext.UserRankInfo.UserRid == 10)
                    orderProductList = Carts.UpdateCartForCompanyUser(orderProductList, WorkContext.Uid, WorkContext.Sid);
                orderProductList.ForEach(x =>
                {
                    PartProductInfo pro = Products.GetPartProductById(x.Pid);
                    if (pro != null)
                    {
                        x.PV = x.ProductPV;
                        x.HaiMi = x.ProductHaiMi;
                        x.HongBaoCut = x.ProductHBCut;
                        x.TaxRate = pro.TaxRate;
                        x.StoreSTid = pro.StoreSTid;
                        x.Weight = pro.Weight;
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
                if (orderProductList.Count < 1)
                    return AjaxResult("emptycart", "购物车中没有商品");
                //验证内部员工购买（和治友德旗舰店专区产品（注：每月每种产品限购一件），享受专属折扣 4 折）
                bool hasLimitbuyforcompany = false;
                if (WorkContext.UserRankInfo.UserRid == 10)
                {
                    foreach (var orderProductInfo in orderProductList)
                    {
                        //验证商品信息
                        PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                        if (partProductInfo != null)
                        {
                            List<int> channels = Channel.GetProductChannels(orderProductInfo.Pid);
                            if (orderProductInfo.Type == 0 && channels.Exists(x => x == 1))
                            {
                                //判断本月是否购买过此产品
                                int monthbuy = Orders.GetOrderProductCountByPidAndUidForMonth(orderProductInfo.Pid, orderProductInfo.Uid, DateTime.Now);
                                if (monthbuy <= 0)
                                {
                                    if (orderProductInfo.BuyCount > 1)
                                    {
                                        hasLimitbuyforcompany = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (hasLimitbuyforcompany)
                    return AjaxResult("limitcount", "和治友德旗舰店专区产品员工购买每月每种产品限购一件");

                //店铺购物车列表
                List<StoreCartInfo> storeCartList = Carts.TidyMallOrderProductList(StringHelper.SplitString(selectedCartItemKeyList), orderProductList, StringHelper.SplitString(selectedStoreKeyList));
                if (Carts.SumMallCartOrderProductCount(storeCartList) < 1)
                    return AjaxResult("nonselected", "购物车中没有选中的商品");

                //存在汇购卡券产品
                if (storeCartList.Exists(x => x.SelectedOrderProductList.Exists(p => StringHelper.StrContainsForNum(WebHelper.GetConfigSettings("CoffeeQuanPid"), p.Pid.ToString()))) && !WorkContext.IsDirSaleUser)
                    return AjaxResult("errorproduct", "您的会员等级还不能购买汇购卡，请返回购物车修改");
                //存在下架产品
                if (storeCartList.Exists(x => x.SelectedOrderProductList.Exists(p => p.ProductState == -1 || p.ProductState >= 1)))
                    return AjaxResult("errorproduct", "订单中存在下架或库存不足的失效商品，请返回购物车修改");

                //验证支付积分
                if (payCreditCount > 0)
                {
                    if (payCreditCount > WorkContext.PartUserInfo.PayCredits)
                        return AjaxResult("noenoughpaycredit", "你使用的" + Credits.PayCreditName + "数超过你所拥有的" + WorkContext.PartUserInfo.PayCredits + "数");
                    if (payCreditCount > Credits.OrderMaxUsePayCredits * storeCartList.Count)
                        return AjaxResult("maxusepaycredit", "此笔订单最多使用" + Credits.OrderMaxUsePayCredits + "个" + Credits.PayCreditName);
                }
                //验证支付海米
                List<AccountInfo> accountInfoList = AccountUtils.GetAccountList(WorkContext.Uid, WorkContext.PartUserInfo.IsDirSaleUser, WorkContext.PartUserInfo.DirSaleUid);
                if (haiMiCount > 0)
                {
                    //if (WorkContext.Uid == 445)
                    //{
                    //    return AjaxResult("nohaimiaccount", "您的海米账号余额已被锁定。");
                    //}
                    AccountInfo accontInfo = accountInfoList.Find(x => x.AccountId == (int)AccountType.商城钱包);
                    if (accontInfo == null)
                        return AjaxResult("nohaimiaccount", "您还没有拥有" + MallKey.MallDiscountName_JiangJin + "账号，请联系客服");

                    if (haiMiCount > accontInfo.Banlance)
                        return AjaxResult("noenoughhaimi", "使用的" + MallKey.MallDiscountName_JiangJin + "数超过你所拥有的" + accontInfo.Banlance + "数");
                }
                //验证红包
                //decimal orderHongBaoCount = Carts.SumMallCartHongBaoCutAmount(storeCartList);
                if (hongBaoCount > 0)
                {
                    AccountInfo accontInfo = accountInfoList.Find(x => x.AccountId == (int)AccountType.积分账户);
                    if (accontInfo == null)
                        return AjaxResult("nohongbaoaccount", "您还没有拥有" + MallKey.MallDiscountName_JiFen + "账号，请联系客服");

                    if (hongBaoCount > accontInfo.Banlance)
                        return AjaxResult("noenoughhongbao", "使用的" + MallKey.MallDiscountName_JiFen + "超过你所拥有的" + accontInfo.Banlance + "数");
                }
                //验证汇购卡券
                List<CashCouponInfo> selectCashList = new List<CashCouponInfo>();
                if (cashCount > 0 && !string.IsNullOrEmpty(cashId))
                {
                    if (string.IsNullOrEmpty(payPswd))
                        return AjaxResult("errorcash", "支付密码不能为空");

                    selectCashList = CashCoupon.GetList(string.Format(" CashId IN ({0})", cashId));
                    if (selectCashList.Count != StringHelper.SplitString(cashId).Length)
                        return AjaxResult("errorcash", "存在错误的汇购卡编号");
                    if (selectCashList.Exists(x => x.Uid != WorkContext.Uid))
                        return AjaxResult("errorcash", "存在错误的汇购卡编号");
                    if (cashCount > selectCashList.Sum(x => x.Banlance))
                        return AjaxResult("errorcash", "使用的汇购卡券超过可用余额");
                    if (selectCashList.Exists(x => x.ValidTime < DateTime.Now))
                        return AjaxResult("errorcash", "使用的汇购卡已过期");

                    //CashCouponInfo cash = CashCoupon.GetModel(cashId);
                    //if (cash == null)
                    //    return AjaxResult("errorcash", "错误的汇购卡编号");
                    //if (cash.Uid != WorkContext.Uid)
                    //    return AjaxResult("errorcash", "错误的汇购卡编号");
                    //if (cashCount > cash.Banlance)
                    //    return AjaxResult("errorcash", "使用的汇购卡券超过可用余额");
                    //if (cash.ValidTime < DateTime.Now)
                    //    return AjaxResult("errorcash", "使用的汇购卡已过期");
                    if (OrderUtils.GetPayPassword(payPswd, WorkContext.PartUserInfo.DirSaleUid) != SecureHelper.EncryptString(payPswd, DirSaleUserInfo.EncryptKey))
                        return AjaxResult("errorcash", "支付密码不正确");
                }


                //验证支付金额
                GroupProductInfo gpInfo = new GroupProducts().GetModel(3);
                ConfirmOrderModel model = new ConfirmOrderModel();
                List<StoreOrder> storeOrderList = new List<StoreOrder>();
                List<OrderProductInfo> exorderproductlist = new List<OrderProductInfo>();
                bool isSend = true;
                foreach (StoreCartInfo item in storeCartList)
                {
                    StoreOrder storeOrder = new StoreOrder();
                    storeOrder.StoreCartInfo = item;
                    storeOrder.ProductAmount = Carts.SumOrderProductAmount(item.SelectedOrderProductList);
                    storeOrder.FullCut = Carts.SumFullCut(item.CartItemList);

                    storeOrder.ShipFee = Orders.GetShipFee(fullShipAddressInfo.ProvinceId, fullShipAddressInfo.CityId, item.SelectedOrderProductList, ref isSend);// Orders.GetShipFee(item.StoreInfo.StoreId, storeOrder.ProductAmount);
                    decimal oldShipFee = storeOrder.ShipFee;
                    storeOrder.ShipFee = Orders.GetDoublee11ShipFee(item.SelectedOrderProductList, oldShipFee, gpInfo);

                    storeOrder.TotalCount = Carts.SumOrderProductCount(item.SelectedOrderProductList);
                    storeOrder.TotalWeight = Carts.SumOrderProductWeight(item.SelectedOrderProductList);
                    storeOrder.TaxFee = Carts.SumOrderProductTaxAmount(item.SelectedOrderProductList);
                    storeOrder.HongBaoCutFee = Carts.SumOrderProductHongBaoCutAmount(item.SelectedOrderProductList);
                    storeOrderList.Add(storeOrder);

                    model.AllHonBaoCutFee += storeOrder.HongBaoCutFee;
                    model.TaxFee += storeOrder.TaxFee;
                    model.AllShipFee += storeOrder.ShipFee;
                    model.AllFullCut += storeOrder.FullCut;
                    model.AllProductAmount += storeOrder.ProductAmount;
                    model.AllTotalCount += storeOrder.TotalCount;
                    model.AllTotalWeight += storeOrder.TotalWeight;
                    exorderproductlist.AddRange(item.SelectedOrderProductList);
                }
                model.AllSelectOrderProductList = exorderproductlist;

                //验证代理、佣金账户支付
                // model.AvailAgentAmount = storeOrderList.FindAll(x => x.StoreCartInfo.StoreInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId") || x.StoreCartInfo.StoreInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId")).Sum(x => x.ProductAmount);

                if (daiLiCount > 0)
                {
                    AccountInfo accontInfo = accountInfoList.Find(x => x.AccountId == (int)AccountType.代理账户);
                    if (accontInfo == null)
                        return AjaxResult("noaccount", "您还没有拥有代理账户，请联系客服");

                    if (daiLiCount > accontInfo.Banlance)
                        return AjaxResult("errorcount", "使用数超过你所拥有的余额数" + accontInfo.Banlance);
                    //if(daiLiCount>model.AvailAgentAmount)
                    //    return AjaxResult("errorcount", "使用数超过可用数" + model.AvailAgentAmount);
                }
                if (YongJinCount > 0)
                {
                    AccountInfo accontInfo = accountInfoList.Find(x => x.AccountId == (int)AccountType.佣金账户);
                    if (accontInfo == null)
                        return AjaxResult("noaccount", "您还没有拥有佣金账户，请联系客服");

                    if (YongJinCount > accontInfo.Banlance)
                        return AjaxResult("errorcount", "使用数超过你所拥有的余额数" + accontInfo.Banlance);
                    //if (YongJinCount > model.AvailAgentAmount)
                    //    return AjaxResult("errorcount", "使用数超过可用数" + model.AvailAgentAmount);
                }


                model.AllOrderAmount = model.AllProductAmount - model.AllFullCut + model.AllShipFee + model.PayFee + (model.TaxFee > 50 ? model.TaxFee : 0);

                if (model.AllOrderAmount - haiMiCount - hongBaoCount - cashCount - daiLiCount - YongJinCount < 0)
                {
                    return AjaxResult("overamount", "你使用的抵现总金额不能超过订单总金额");
                }
                if (!isSend)
                {
                    return AjaxResult("oversend", "订单中有不支持配送产品，请返回购物车修改");
                }

                #region 验证优惠劵
                List<CouponInfo> couponList = new List<CouponInfo>();
                if (couponIdList.Length > 0)
                {
                    foreach (string couponId in couponIdList)
                    {
                        int tempCouponId = TypeHelper.StringToInt(couponId);
                        if (tempCouponId > 0)
                        {
                            CouponInfo couponInfo = Coupons.GetCouponByCouponId(TypeHelper.StringToInt(couponId));
                            if (couponInfo == null)
                                return AjaxResult("nocoupon", "优惠劵不存在");
                            else
                                couponList.Add(couponInfo);
                        }
                    }
                }
                if (couponSNList.Length > 0)
                {
                    foreach (string couponSN in couponSNList)
                    {
                        if (!string.IsNullOrWhiteSpace(couponSN))
                        {
                            CouponInfo couponInfo = Coupons.GetCouponByCouponSN(couponSN);
                            if (couponInfo == null)
                                return AjaxResult("nocoupon", "优惠劵" + couponSN + "不存在");
                            else
                                couponList.Add(couponInfo);
                        }
                    }
                }
                foreach (CouponInfo couponInfo in couponList)
                {
                    #region  验证

                    CouponTypeInfo couponTypeInfo = Coupons.GetCouponTypeById(couponInfo.CouponTypeId);
                    if (couponTypeInfo == null)
                    {
                        return AjaxResult("nocoupontype", "编号为" + couponInfo.CouponSN + "优惠劵类型不存在");
                    }
                    else if (couponTypeInfo.State == 0)
                    {
                        return AjaxResult("closecoupontype", "编号为" + couponInfo.CouponSN + "优惠劵类型已关闭");
                    }
                    else if (couponTypeInfo.UseExpireTime == 0 && couponTypeInfo.UseStartTime > DateTime.Now)
                    {
                        return AjaxResult("unstartcoupon", "编号为" + couponInfo.CouponSN + "优惠劵还未到使用时间");
                    }
                    else if (couponTypeInfo.UseExpireTime == 0 && couponTypeInfo.UseEndTime <= DateTime.Now)
                    {
                        return AjaxResult("expiredcoupon", "编号为" + couponInfo.CouponSN + "优惠劵已过期");
                    }
                    //else if (couponTypeInfo.UseExpireTime > 0 && couponInfo.ActivateTime <= DateTime.Now.AddDays(-1 * couponTypeInfo.UseExpireTime))
                    //{
                    //    return AjaxResult("expiredcoupon", "编号为" + couponInfo.CouponSN + "优惠劵已过期");
                    //}
                    else if (couponInfo.ActivateTime > DateTime.Now)
                    {
                        return AjaxResult("expiredcoupon", "编号为" + couponInfo.CouponSN + "优惠劵还未到使用时间");
                    }
                    else if (couponInfo.ValidTime <= DateTime.Now)
                    {
                        return AjaxResult("expiredcoupon", "编号为" + couponInfo.CouponSN + "优惠劵已过期");
                    }
                    else if (couponTypeInfo.UserRankLower > WorkContext.PartUserInfo.UserRid)
                    {
                        return AjaxResult("userranklowercoupon", "你的用户等级太低，不能使用编号为" + couponInfo.CouponSN + "优惠劵");
                    }
                    else if (couponList.Count > 1 && couponTypeInfo.UseMode == 1)
                    {
                        return AjaxResult("nomutcoupon", "编号为" + couponInfo.CouponSN + "优惠劵不能叠加使用");
                    }
                    else
                    {
                        if (couponTypeInfo.StoreId != -1)
                        {
                            StoreCartInfo storeCartInfo = storeCartList.Find(x => x.StoreInfo.StoreId == couponTypeInfo.StoreId);

                            if (storeCartInfo == null)
                            {
                                return AjaxResult("wrongstorecoupon", "编号为" + couponInfo.CouponSN + "优惠劵只能购买对应店铺的商品");
                            }

                            if (couponTypeInfo.OrderAmountLower > Carts.SumOrderProductAmount(storeCartInfo.SelectedOrderProductList))
                            {
                                return AjaxResult("orderamountlowercoupon", "订单金额太低，不能使用编号为" + couponInfo.CouponSN + "优惠劵");
                            }

                            if (couponTypeInfo.LimitStoreCid > 0)
                            {
                                foreach (OrderProductInfo orderProductInfo in storeCartInfo.SelectedOrderProductList)
                                {
                                    if (orderProductInfo.Type == 0 && orderProductInfo.StoreCid != couponTypeInfo.LimitStoreCid)
                                    {
                                        return AjaxResult("limitstoreclasscoupon", "编号为" + couponInfo.CouponSN + "优惠劵只能在购买" + Stores.GetStoreClassByStoreIdAndStoreCid(couponTypeInfo.StoreId, couponTypeInfo.LimitStoreCid).Name + "类的商品时使用");
                                    }
                                }
                            }

                            if (couponTypeInfo.LimitProduct == 1)
                            {
                                List<OrderProductInfo> commonOrderProductList = Carts.GetCommonOrderProductList(storeCartInfo.SelectedOrderProductList);
                                if (!Coupons.IsSameCouponType(couponTypeInfo.CouponTypeId, Carts.GetPidList(commonOrderProductList)))
                                    return AjaxResult("limitproductcoupon", "编号为" + couponInfo.CouponSN + "优惠劵只能在购买指定商品时使用");
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(couponInfo.ChannelId))
                            {
                                List<OrderProductInfo> allProduct = new List<OrderProductInfo>();
                                foreach (var item in storeCartList)
                                {
                                    foreach (var detail in item.SelectedOrderProductList)
                                    {
                                        List<int> channelIdList = Channel.GetProductChannels(detail.Pid);
                                        List<int> active_10_1 = new List<int>() { 6, 9 };
                                        if (channelIdList.Exists(x => active_10_1.Exists(p => p == x)))
                                        {
                                            allProduct.Add(detail);
                                        }
                                    }
                                }
                                if (couponTypeInfo.OrderAmountLower > Carts.SumOrderProductAmount(allProduct))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                List<OrderProductInfo> allProduct = new List<OrderProductInfo>();
                                foreach (var item in storeCartList)
                                {
                                    allProduct.AddRange(item.SelectedOrderProductList);
                                }
                                if (couponTypeInfo.OrderAmountLower > Carts.SumOrderProductAmount(allProduct))
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    #endregion
                }
                #endregion

                //验证兑换码
                List<int> otherSelectPids = new List<int>() { 3757, 3756, 3755, 3730, 3729, 3728, 3745, 3744 };
                if (model.AllSelectOrderProductList.Exists(x => otherSelectPids.Exists(p => p == x.Pid)) && string.IsNullOrEmpty(exselectname.Trim()))
                    return AjaxResult("wrongexcode", "请选择兑换码所需具体信息，如城市或书籍种类");
                if (!string.IsNullOrEmpty(exselectname.Trim()))
                    buyerRemark += "+类型：" + exselectname;

                bool useExCode = false;
                exorderproductlist = exorderproductlist.FindAll(x => x.Type == 0 && x.ExtCode1 > 0);
                List<ExCodeProductsInfo> exProducts = new List<ExCodeProductsInfo>();
                List<SinglePromotionInfo> exsinglelist = new List<SinglePromotionInfo>();
                if (exorderproductlist.Count > 0)
                {
                    exsinglelist = Promotions.GetSingleByStrWhere(string.Format(" pid IN (" + string.Join(",", exorderproductlist.Select(x => x.Pid)) + ") AND state=1 AND (( [endtime1]>@nowtime AND [starttime1]<=@nowtime ) OR ( [endtime2]>@nowtime AND [starttime2]<=@nowtime ) OR ( [endtime3]>@nowtime AND [starttime3]<=@nowtime )) AND  discounttype=6 "));
                    if (exsinglelist.Count > 0)
                    {
                        useExCode = true;
                        exProducts = new ExCodeProducts().GetList(string.Format(" pid IN (" + string.Join(",", exsinglelist.Select(x => x.Pid)) + ") "));
                    }
                }
                model.UseExCode = useExCode;

                if (useExCode && string.IsNullOrEmpty(selectexIds))
                    return AjaxResult("wrongexcode", "请选择兑换码");
                List<ExChangeCouponsInfo> selectExCodeList = new List<ExChangeCouponsInfo>();
                if (useExCode && !string.IsNullOrEmpty(selectexIds))
                {

                    selectExCodeList = ExChangeCoupons.GetList(string.Format(" exid IN ({0})", selectexIds));
                    List<int> typeids = exProducts.Select(x => x.CodeTypeId).Distinct().ToList();
                    if (typeids.Count > 1)
                        return AjaxResult("errorexcode", "每次只能使用一个等级的兑换码");

                    //List<int> allpids = exorderproductlist.Select(x => x.Pid).ToList();

                    int exprocount = exorderproductlist.FindAll(x => exsinglelist.Exists(p => p.Pid == x.Pid)).Sum(x => x.BuyCount);
                    if (selectExCodeList.Count != exprocount)
                        return AjaxResult("errorexcode", "商品数与兑换码数不相等");
                    if (selectExCodeList.Count != StringHelper.SplitString(selectexIds).Length)
                        return AjaxResult("errorexcode", "存在错误的兑换码");
                    if (selectExCodeList.Exists(x => x.uid != WorkContext.Uid))
                        return AjaxResult("errorexcode", "存在错误的兑换码");

                    if (selectExCodeList.Exists(x => x.validtime < DateTime.Now))
                        return AjaxResult("errorexcode", "存在失效的兑换码");
                    if (selectExCodeList.Exists(x => x.oid > 0))
                        return AjaxResult("errorexcode", "存在已使用的兑换码");
                    if (selectExCodeList.Exists(x => x.state == 0))
                        return AjaxResult("errorexcode", "存在未激活的兑换码");
                }

                //验证支付金额必须大于0，即优惠金额必须小于订单总金额
                if (model.AllOrderAmount - haiMiCount - hongBaoCount - cashCount - couponList.Sum(x => x.Money) - daiLiCount - YongJinCount < 0)
                {
                    return AjaxResult("overamount", "你使用的优惠总金额不能超过订单总金额");
                }
                string pidList = Carts.GetMallCartPidList(storeCartList);//商品id列表
                List<ProductStockInfo> productStockList = Products.GetProductStockList(pidList);//商品库存列表
                List<SinglePromotionInfo> singlePromotionList = new List<SinglePromotionInfo>();//单品促销活动列表
                //循环店铺购物车列表
                foreach (StoreCartInfo storeCartInfo in storeCartList)
                {
                    //循环购物车项列表，依次验证
                    foreach (CartItemInfo cartItemInfo in storeCartInfo.CartItemList)
                    {

                        if (cartItemInfo.Type == 0)//购物车商品
                        {
                            CartProductInfo cartProductInfo = (CartProductInfo)cartItemInfo.Item;
                            if (cartProductInfo.Selected)
                            {

                                #region 验证

                                OrderProductInfo orderProductInfo = cartProductInfo.OrderProductInfo;
                                // 验证最大咖啡券购买数
                                if (StringHelper.StrContainsForNum(WebHelper.GetConfigSettings("CoffeeQuanPid"), orderProductInfo.Pid.ToString()))
                                {
                                    int currentCount = CashCoupon.GetRecordCount(string.Format("  Uid={0} AND CouponType=1", WorkContext.Uid));
                                    int noPayCashCount = Orders.GetOrderProductCountByPidAndUid(orderProductInfo.Pid, WorkContext.Uid);
                                    //if (WorkContext.Uid == 15078 || WorkContext.Uid==24108)
                                    //{
                                    //    if (currentCount + noPayCashCount + orderProductInfo.BuyCount > 6)
                                    //    {
                                    //        return AjaxResult("outmaxproduct", "您的帐号限购6张汇购卡，请返回购物车修改或支付未付款的汇购卡订单");
                                    //    }
                                    //}
                                    //else
                                    //{
                                    if (currentCount + noPayCashCount + orderProductInfo.BuyCount > WorkContext.PartUserInfo.MaxCashCount)
                                    {
                                        return AjaxResult("outmaxproduct", "您的帐号限购" + WorkContext.PartUserInfo.MaxCashCount + "张汇购卡，请返回购物车修改或支付未付款的汇购卡订单");
                                    }
                                    //}
                                    //if(orderProductInfo.BuyCount>1)
                                    //    return AjaxResult("outmaxproduct", "汇购卡单次购买仅限1张，请返回购物车修改");
                                }
                                //验证商品信息
                                PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                                if (partProductInfo == null)
                                {
                                    return AjaxResult("outsaleproduct", "商品" + partProductInfo.Name + "已经下架，请删除此商品");
                                }
                                if (orderProductInfo.Name != partProductInfo.Name || orderProductInfo.ShopPrice != partProductInfo.ShopPrice || orderProductInfo.MarketPrice != partProductInfo.MarketPrice || orderProductInfo.CostPrice != partProductInfo.CostPrice || orderProductInfo.Weight != partProductInfo.Weight || orderProductInfo.PSN != partProductInfo.PSN)
                                {
                                    return AjaxResult("changeproduct", "商品" + partProductInfo.Name + "信息有变化，请删除后重新添加");
                                }

                                //验证商品库存
                                ProductStockInfo productStockInfo = Products.GetProductStock(orderProductInfo.Pid, productStockList);
                                if (productStockInfo.Number < orderProductInfo.RealCount)
                                {
                                    return AjaxResult("outstock", "商品" + partProductInfo.Name + "库存不足");
                                }
                                else
                                {
                                    productStockInfo.Number -= orderProductInfo.RealCount;
                                }

                                //验证买送促销活动
                                if (orderProductInfo.ExtCode2 > 0)
                                {
                                    BuySendPromotionInfo buySendPromotionInfo = Promotions.GetBuySendPromotion(orderProductInfo.BuyCount, orderProductInfo.StoreId, orderProductInfo.Pid, DateTime.Now);
                                    if (buySendPromotionInfo == null)
                                    {
                                        return AjaxResult("stopbuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经停止,请删除此商品后重新添加");
                                    }
                                    else if (buySendPromotionInfo.PmId != orderProductInfo.ExtCode2)
                                    {
                                        return AjaxResult("replacebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经替换，请删除此商品后重新添加");
                                    }
                                    else if (buySendPromotionInfo.UserRankLower > WorkContext.UserRid)
                                    {
                                        orderProductInfo.RealCount = orderProductInfo.BuyCount;
                                        orderProductInfo.ExtCode2 = -1 * orderProductInfo.ExtCode2;
                                        Carts.UpdateOrderProductBuySend(new List<OrderProductInfo>() { orderProductInfo });
                                        if (cartProductInfo.GiftList.Count > 0)
                                        {
                                            foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                            {
                                                gift.RealCount = orderProductInfo.BuyCount * gift.ExtCode2;
                                            }
                                            Carts.UpdateOrderProductCount(cartProductInfo.GiftList);
                                        }
                                        return AjaxResult("userranklowerbuysend", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的买送促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                    }
                                    else if (orderProductInfo.RealCount != (orderProductInfo.BuyCount + (orderProductInfo.BuyCount / buySendPromotionInfo.BuyCount) * buySendPromotionInfo.SendCount))
                                    {
                                        return AjaxResult("changebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经改变，请删除此商品后重新添加");
                                    }
                                }

                                //验证单品促销活动
                                if (orderProductInfo.ExtCode1 > 0)
                                {
                                    SinglePromotionInfo singlePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                                    if (singlePromotionInfo == null)
                                    {
                                        return AjaxResult("stopsingle", "商品" + orderProductInfo.Name + "的单品促销活动已经停止，请删除此商品后重新添加");
                                    }
                                    if (singlePromotionInfo.PayCredits != orderProductInfo.PayCredits || singlePromotionInfo.CouponTypeId != orderProductInfo.CouponTypeId)
                                    {
                                        return AjaxResult("changesingle", "商品" + orderProductInfo.Name + "的单品促销活动已经改变，请删除此商品后重新添加");
                                    }
                                    if (singlePromotionInfo.UserRankLower > WorkContext.PartUserInfo.UserRid)
                                    {
                                        orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                        orderProductInfo.PayCredits = 0;
                                        orderProductInfo.CouponTypeId = 0;
                                        orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                        Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                        return AjaxResult("userranklowersingle", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的单品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                    }
                                    if (singlePromotionInfo.QuotaLower > 0 && orderProductInfo.BuyCount < singlePromotionInfo.QuotaLower)
                                    {
                                        orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                        orderProductInfo.PayCredits = 0;
                                        orderProductInfo.CouponTypeId = 0;
                                        orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                        Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                        return AjaxResult("orderminsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最少购买" + singlePromotionInfo.QuotaLower + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                    }
                                    if (singlePromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > singlePromotionInfo.QuotaUpper)
                                    {
                                        orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                        orderProductInfo.PayCredits = 0;
                                        orderProductInfo.CouponTypeId = 0;
                                        orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                        Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                        return AjaxResult("ordermuchsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最多购买" + singlePromotionInfo.QuotaUpper + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                    }
                                    if (singlePromotionInfo.AllowBuyCount > 0 && Promotions.GetSinglePromotionProductBuyCount(WorkContext.Uid, singlePromotionInfo.PmId) > singlePromotionInfo.AllowBuyCount)
                                    {
                                        orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                        orderProductInfo.PayCredits = 0;
                                        orderProductInfo.CouponTypeId = 0;
                                        orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                        Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                        return AjaxResult("userminsingle", "商品" + orderProductInfo.Name + "的单品促销活动每个人最多购买" + singlePromotionInfo.AllowBuyCount + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                    }
                                    if (singlePromotionInfo.IsStock == 1)
                                    {
                                        SinglePromotionInfo temp = singlePromotionList.Find(x => x.PmId == singlePromotionInfo.PmId);
                                        if (temp == null)
                                        {
                                            temp = singlePromotionInfo;
                                            singlePromotionList.Add(temp);
                                        }

                                        if (temp.Stock < orderProductInfo.RealCount)
                                        {
                                            orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                            orderProductInfo.PayCredits = 0;
                                            orderProductInfo.CouponTypeId = 0;
                                            orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                            Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                            return AjaxResult("stockoutsingle", "商品" + orderProductInfo.Name + "的单品促销活动库存不足,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        else
                                        {
                                            temp.Stock -= orderProductInfo.RealCount;
                                        }
                                    }
                                }

                                //验证赠品促销活动
                                if (orderProductInfo.ExtCode3 > 0)
                                {
                                    GiftPromotionInfo giftPromotionInfo = Promotions.GetGiftPromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                                    if (giftPromotionInfo == null)
                                    {
                                        return AjaxResult("stopgift", "商品" + orderProductInfo.Name + "的赠品促销活动已经停止,请删除此商品后重新添加");
                                    }
                                    else if (giftPromotionInfo.PmId != orderProductInfo.ExtCode3)
                                    {
                                        return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                    }
                                    else if (giftPromotionInfo.UserRankLower > WorkContext.UserRid)
                                    {
                                        orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                        Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                        Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                                        return AjaxResult("userranklowergift", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的赠品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                    }
                                    else if (giftPromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > giftPromotionInfo.QuotaUpper)
                                    {
                                        orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                        Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                        Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                                        return AjaxResult("ordermuchgift", "商品" + orderProductInfo.Name + "的赠品要求每单最多购买" + giftPromotionInfo.QuotaUpper + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                    }
                                    else if (cartProductInfo.GiftList.Count > 0)
                                    {
                                        List<ExtGiftInfo> extGiftList = Promotions.GetExtGiftList(orderProductInfo.ExtCode3);
                                        if (extGiftList.Count != cartProductInfo.GiftList.Count)
                                        {
                                            return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                        }
                                        List<OrderProductInfo> newGiftOrderProductList = new List<OrderProductInfo>(extGiftList.Count);
                                        foreach (ExtGiftInfo extGiftInfo in extGiftList)
                                        {
                                            OrderProductInfo giftOrderProduct = Carts.BuildOrderProduct(extGiftInfo);
                                            Carts.SetGiftOrderProduct(giftOrderProduct, 1, orderProductInfo.RealCount, extGiftInfo.Number, giftPromotionInfo.PmId);
                                            newGiftOrderProductList.Add(giftOrderProduct);
                                        }

                                        //验证赠品信息是否改变
                                        for (int i = 0; i < newGiftOrderProductList.Count; i++)
                                        {
                                            OrderProductInfo newSuitOrderProductInfo = newGiftOrderProductList[i];
                                            OrderProductInfo oldSuitOrderProductInfo = cartProductInfo.GiftList[i];
                                            if (newSuitOrderProductInfo.Pid != oldSuitOrderProductInfo.Pid ||
                                                newSuitOrderProductInfo.Name != oldSuitOrderProductInfo.Name ||
                                                newSuitOrderProductInfo.ShopPrice != oldSuitOrderProductInfo.ShopPrice ||
                                                newSuitOrderProductInfo.MarketPrice != oldSuitOrderProductInfo.MarketPrice ||
                                                newSuitOrderProductInfo.CostPrice != oldSuitOrderProductInfo.CostPrice ||
                                                newSuitOrderProductInfo.Type != oldSuitOrderProductInfo.Type ||
                                                newSuitOrderProductInfo.RealCount != oldSuitOrderProductInfo.RealCount ||
                                                newSuitOrderProductInfo.BuyCount != oldSuitOrderProductInfo.BuyCount ||
                                                newSuitOrderProductInfo.ExtCode1 != oldSuitOrderProductInfo.ExtCode1 ||
                                                newSuitOrderProductInfo.ExtCode2 != oldSuitOrderProductInfo.ExtCode2 ||
                                                newSuitOrderProductInfo.ExtCode3 != oldSuitOrderProductInfo.ExtCode3 ||
                                                newSuitOrderProductInfo.ExtCode4 != oldSuitOrderProductInfo.ExtCode4 ||
                                                newSuitOrderProductInfo.ExtCode5 != oldSuitOrderProductInfo.ExtCode5)
                                            {
                                                return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                            }
                                        }

                                        //验证赠品库存
                                        foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                        {
                                            ProductStockInfo stockInfo = Products.GetProductStock(gift.Pid, productStockList);
                                            if (stockInfo.Number < gift.RealCount)
                                            {
                                                if (stockInfo.Number == 0)
                                                {
                                                    Carts.DeleteOrderProductList(new List<OrderProductInfo>() { gift });
                                                    if (cartProductInfo.GiftList.Count == 1)
                                                    {
                                                        orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                                        Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                                    }
                                                }
                                                else
                                                {
                                                    gift.RealCount = stockInfo.Number;
                                                    Carts.UpdateOrderProductCount(new List<OrderProductInfo>() { gift });
                                                }
                                                return AjaxResult("outstock", "商品" + orderProductInfo.Name + "的赠品" + gift.Name + "库存不足,请返回购物车重新确认");
                                            }
                                            else
                                            {
                                                stockInfo.Number -= gift.RealCount;
                                            }
                                        }
                                    }
                                }

                                #endregion
                            }
                        }
                        else if (cartItemInfo.Type == 1)//购物车套装
                        {
                            CartSuitInfo cartSuitInfo = (CartSuitInfo)cartItemInfo.Item;
                            if (cartSuitInfo.Checked)
                            {
                                #region 验证

                                SuitPromotionInfo suitPromotionInfo = Promotions.GetSuitPromotionByPmIdAndTime(cartSuitInfo.PmId, DateTime.Now);
                                if (suitPromotionInfo == null)
                                {
                                    return AjaxResult("stopsuit", "套装" + suitPromotionInfo.Name + "已经停止,请删除此套装");
                                }
                                else if (suitPromotionInfo.UserRankLower > WorkContext.UserRid)
                                {
                                    return AjaxResult("userranklowersuit", "你的用户等级太低，无法购买套装" + suitPromotionInfo.Name + "请删除此套装");
                                }
                                else if (suitPromotionInfo.QuotaUpper > 0 && cartSuitInfo.BuyCount > suitPromotionInfo.QuotaUpper)
                                {
                                    return AjaxResult("ordermuchsuit", "套装" + suitPromotionInfo.Name + "要求每单最多购买" + suitPromotionInfo.QuotaUpper + "个");
                                }
                                else if (suitPromotionInfo.OnlyOnce == 1 && Promotions.IsJoinSuitPromotion(WorkContext.Uid, suitPromotionInfo.PmId))
                                {
                                    return AjaxResult("usermuchsuit", "套装" + suitPromotionInfo.Name + "要求每人最多购买1个");
                                }

                                List<OrderProductInfo> newSuitOrderProductList = new List<OrderProductInfo>();
                                int k = 1;
                                foreach (ExtSuitProductInfo extSuitProductInfo in Promotions.GetExtSuitProductList(cartSuitInfo.PmId))
                                {
                                    OrderProductInfo suitOrderProductInfo = Carts.BuildOrderProduct(extSuitProductInfo);
                                    Carts.SetSuitOrderProduct(suitOrderProductInfo, cartSuitInfo.BuyCount, extSuitProductInfo.Number, extSuitProductInfo.Discount, suitPromotionInfo, k);
                                    k++;
                                    newSuitOrderProductList.Add(suitOrderProductInfo);
                                }
                                List<OrderProductInfo> oldSuitOrderProductList = new List<OrderProductInfo>();
                                foreach (CartProductInfo cartProductInfo in cartSuitInfo.CartProductList)
                                {
                                    oldSuitOrderProductList.Add(cartProductInfo.OrderProductInfo);
                                }
                                if (newSuitOrderProductList.Count != oldSuitOrderProductList.Count)
                                {
                                    return AjaxResult("changesuit", "套装" + suitPromotionInfo.Name + "已经改变，请删除此套装后重新下单");
                                }
                                else
                                {
                                    for (int i = 0; i < newSuitOrderProductList.Count; i++)
                                    {
                                        OrderProductInfo newSuitOrderProductInfo = newSuitOrderProductList[i];
                                        OrderProductInfo oldSuitOrderProductInfo = oldSuitOrderProductList[i];
                                        if (newSuitOrderProductInfo.Pid != oldSuitOrderProductInfo.Pid ||
                                            newSuitOrderProductInfo.Name != oldSuitOrderProductInfo.Name ||
                                            newSuitOrderProductInfo.ShopPrice != oldSuitOrderProductInfo.ShopPrice ||
                                            newSuitOrderProductInfo.MarketPrice != oldSuitOrderProductInfo.MarketPrice ||
                                            newSuitOrderProductInfo.CostPrice != oldSuitOrderProductInfo.CostPrice ||
                                            newSuitOrderProductInfo.Type != oldSuitOrderProductInfo.Type ||
                                            newSuitOrderProductInfo.RealCount != oldSuitOrderProductInfo.RealCount ||
                                            newSuitOrderProductInfo.BuyCount != oldSuitOrderProductInfo.BuyCount ||
                                            newSuitOrderProductInfo.ExtCode1 != oldSuitOrderProductInfo.ExtCode1 ||
                                            newSuitOrderProductInfo.ExtCode2 != oldSuitOrderProductInfo.ExtCode2 ||
                                            newSuitOrderProductInfo.ExtCode3 != oldSuitOrderProductInfo.ExtCode3 ||
                                            newSuitOrderProductInfo.ExtCode4 != oldSuitOrderProductInfo.ExtCode4 ||
                                            newSuitOrderProductInfo.ExtCode5 != oldSuitOrderProductInfo.ExtCode5)
                                        {
                                            return AjaxResult("changesuit", "套装" + suitPromotionInfo.Name + "已经改变，请删除此套装后重新下单");
                                        }
                                    }

                                    foreach (CartProductInfo cartProductInfo in cartSuitInfo.CartProductList)
                                    {
                                        OrderProductInfo orderProductInfo = cartProductInfo.OrderProductInfo;

                                        //验证商品信息
                                        PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                                        if (partProductInfo == null)
                                        {
                                            return AjaxResult("outsaleproduct", "套装中的商品" + partProductInfo.Name + "已经下架，请删除此套装");
                                        }
                                        if (orderProductInfo.Name != partProductInfo.Name || orderProductInfo.ShopPrice != partProductInfo.ShopPrice || orderProductInfo.MarketPrice != partProductInfo.MarketPrice || orderProductInfo.CostPrice != partProductInfo.CostPrice || orderProductInfo.Weight != partProductInfo.Weight || orderProductInfo.PSN != partProductInfo.PSN)
                                        {
                                            return AjaxResult("changeproduct", "套装中的商品" + partProductInfo.Name + "信息有变化，请删除套装后重新添加");
                                        }

                                        //验证商品库存
                                        ProductStockInfo productStockInfo = Products.GetProductStock(orderProductInfo.Pid, productStockList);
                                        if (productStockInfo.Number < orderProductInfo.RealCount)
                                        {
                                            return AjaxResult("outstock", "套装中的商品" + partProductInfo.Name + "库存不足");
                                        }
                                        else
                                        {
                                            productStockInfo.Number -= orderProductInfo.RealCount;
                                        }

                                        //验证赠品促销活动
                                        if (orderProductInfo.ExtCode3 > 0)
                                        {
                                            GiftPromotionInfo giftPromotionInfo = Promotions.GetGiftPromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                                            if (giftPromotionInfo == null)
                                            {
                                                return AjaxResult("stopgift", "套装中的商品" + orderProductInfo.Name + "的赠品促销活动已经停止,请删除此套装后重新添加");
                                            }
                                            else if (giftPromotionInfo.PmId != orderProductInfo.ExtCode3)
                                            {
                                                return AjaxResult("changegift", "套装中的商品" + orderProductInfo.Name + "的赠品已经改变，请删除此套装后重新添加");
                                            }
                                            else if (cartProductInfo.GiftList.Count > 0)
                                            {
                                                List<ExtGiftInfo> extGiftList = Promotions.GetExtGiftList(orderProductInfo.ExtCode3);
                                                if (extGiftList.Count != cartProductInfo.GiftList.Count)
                                                {
                                                    return AjaxResult("changegift", "套装中的商品" + orderProductInfo.Name + "的赠品已经改变，请删除此套装后重新添加");
                                                }
                                                List<OrderProductInfo> newGiftOrderProductList = new List<OrderProductInfo>(extGiftList.Count);
                                                foreach (ExtGiftInfo extGiftInfo in extGiftList)
                                                {
                                                    OrderProductInfo giftOrderProduct = Carts.BuildOrderProduct(extGiftInfo);
                                                    Carts.SetGiftOrderProduct(giftOrderProduct, 1, orderProductInfo.RealCount, extGiftInfo.Number, giftPromotionInfo.PmId);
                                                    newGiftOrderProductList.Add(giftOrderProduct);
                                                }

                                                //验证赠品信息是否改变
                                                for (int i = 0; i < newGiftOrderProductList.Count; i++)
                                                {
                                                    OrderProductInfo newSuitOrderProductInfo = newGiftOrderProductList[i];
                                                    OrderProductInfo oldSuitOrderProductInfo = cartProductInfo.GiftList[i];
                                                    if (newSuitOrderProductInfo.Pid != oldSuitOrderProductInfo.Pid ||
                                                        newSuitOrderProductInfo.Name != oldSuitOrderProductInfo.Name ||
                                                        newSuitOrderProductInfo.ShopPrice != oldSuitOrderProductInfo.ShopPrice ||
                                                        newSuitOrderProductInfo.MarketPrice != oldSuitOrderProductInfo.MarketPrice ||
                                                        newSuitOrderProductInfo.CostPrice != oldSuitOrderProductInfo.CostPrice ||
                                                        newSuitOrderProductInfo.Type != oldSuitOrderProductInfo.Type ||
                                                        newSuitOrderProductInfo.RealCount != oldSuitOrderProductInfo.RealCount ||
                                                        newSuitOrderProductInfo.BuyCount != oldSuitOrderProductInfo.BuyCount ||
                                                        newSuitOrderProductInfo.ExtCode1 != oldSuitOrderProductInfo.ExtCode1 ||
                                                        newSuitOrderProductInfo.ExtCode2 != oldSuitOrderProductInfo.ExtCode2 ||
                                                        newSuitOrderProductInfo.ExtCode3 != oldSuitOrderProductInfo.ExtCode3 ||
                                                        newSuitOrderProductInfo.ExtCode4 != oldSuitOrderProductInfo.ExtCode4 ||
                                                        newSuitOrderProductInfo.ExtCode5 != oldSuitOrderProductInfo.ExtCode5)
                                                    {
                                                        return AjaxResult("changegift", "套装中的商品" + orderProductInfo.Name + "的赠品已经改变，请删除此套装后重新添加");
                                                    }
                                                }

                                                //验证赠品库存
                                                foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                                {
                                                    ProductStockInfo stockInfo = Products.GetProductStock(gift.Pid, productStockList);
                                                    if (stockInfo.Number < gift.RealCount)
                                                    {
                                                        if (stockInfo.Number == 0)
                                                        {
                                                            Carts.DeleteOrderProductList(new List<OrderProductInfo>() { gift });
                                                            if (cartProductInfo.GiftList.Count == 1)
                                                            {
                                                                orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                                                Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                                            }
                                                        }
                                                        else
                                                        {
                                                            gift.RealCount = stockInfo.Number;
                                                            Carts.UpdateOrderProductCount(new List<OrderProductInfo>() { gift });
                                                        }
                                                        return AjaxResult("outstock", "套装中的商品" + orderProductInfo.Name + "的赠品" + gift.Name + "库存不足,请返回购物车重新确认");
                                                    }
                                                    else
                                                    {
                                                        stockInfo.Number -= gift.RealCount;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }

                                #endregion
                            }
                        }
                        else if (cartItemInfo.Type == 2)//购物车满赠
                        {
                            CartFullSendInfo cartFullSendInfo = (CartFullSendInfo)cartItemInfo.Item;

                            #region 验证满赠促销

                            if (cartFullSendInfo.IsEnough && cartFullSendInfo.FullSendMinorOrderProductInfo != null)
                            {
                                if (cartFullSendInfo.FullSendPromotionInfo.UserRankLower > WorkContext.UserRid)
                                {
                                    return AjaxResult("userranklowerfullsend", "你的用户等级太低，无法参加商品" + cartFullSendInfo.FullSendMinorOrderProductInfo.Name + "的满赠促销活动,请先删除此赠品");
                                }
                                if (cartFullSendInfo.FullSendMinorOrderProductInfo.DiscountPrice != cartFullSendInfo.FullSendPromotionInfo.AddMoney)
                                {
                                    return AjaxResult("fullsendadd", "商品" + cartFullSendInfo.FullSendMinorOrderProductInfo.Name + "的满赠促销活动金额错误,请删除后重新添加");
                                }
                                if (!Promotions.IsExistFullSendProduct(cartFullSendInfo.FullSendPromotionInfo.PmId, cartFullSendInfo.FullSendMinorOrderProductInfo.Pid, 1))
                                {
                                    return AjaxResult("nonfullsendminor", "商品" + cartFullSendInfo.FullSendMinorOrderProductInfo.Name + "不是满赠赠品,请删除后再结账");
                                }

                                ProductStockInfo productStockInfo = Products.GetProductStock(cartFullSendInfo.FullSendMinorOrderProductInfo.Pid, productStockList);
                                if (productStockInfo.Number < cartFullSendInfo.FullSendMinorOrderProductInfo.RealCount)
                                {
                                    return AjaxResult("outstock", "满赠赠品" + cartFullSendInfo.FullSendMinorOrderProductInfo.Name + "库存不足,请删除此满赠赠品");
                                }
                                else
                                {
                                    productStockInfo.Number -= cartFullSendInfo.FullSendMinorOrderProductInfo.RealCount;
                                }
                            }

                            #endregion
                            foreach (CartProductInfo cartProductInfo in cartFullSendInfo.FullSendMainCartProductList)
                            {
                                if (cartProductInfo.Selected)
                                {
                                    #region 验证

                                    OrderProductInfo orderProductInfo = cartProductInfo.OrderProductInfo;

                                    //验证商品信息
                                    PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                                    if (partProductInfo == null)
                                    {
                                        return AjaxResult("outsaleproduct", "商品" + partProductInfo.Name + "已经下架，请删除此商品");
                                    }
                                    if (orderProductInfo.Name != partProductInfo.Name || orderProductInfo.ShopPrice != partProductInfo.ShopPrice || orderProductInfo.MarketPrice != partProductInfo.MarketPrice || orderProductInfo.CostPrice != partProductInfo.CostPrice || orderProductInfo.Weight != partProductInfo.Weight || orderProductInfo.PSN != partProductInfo.PSN)
                                    {
                                        return AjaxResult("changeproduct", "商品" + partProductInfo.Name + "信息有变化，请删除后重新添加");
                                    }

                                    //验证商品库存
                                    ProductStockInfo productStockInfo = Products.GetProductStock(orderProductInfo.Pid, productStockList);
                                    if (productStockInfo.Number < orderProductInfo.RealCount)
                                    {
                                        return AjaxResult("outstock", "商品" + partProductInfo.Name + "库存不足");
                                    }
                                    else
                                    {
                                        productStockInfo.Number -= orderProductInfo.RealCount;
                                    }

                                    //验证买送促销活动
                                    if (orderProductInfo.ExtCode2 > 0)
                                    {
                                        BuySendPromotionInfo buySendPromotionInfo = Promotions.GetBuySendPromotion(orderProductInfo.BuyCount, orderProductInfo.StoreId, orderProductInfo.Pid, DateTime.Now);
                                        if (buySendPromotionInfo == null)
                                        {
                                            return AjaxResult("stopbuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经停止,请删除此商品后重新添加");
                                        }
                                        else if (buySendPromotionInfo.PmId != orderProductInfo.ExtCode2)
                                        {
                                            return AjaxResult("replacebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经替换，请删除此商品后重新添加");
                                        }
                                        else if (buySendPromotionInfo.UserRankLower > WorkContext.UserRid)
                                        {
                                            orderProductInfo.RealCount = orderProductInfo.BuyCount;
                                            orderProductInfo.ExtCode2 = -1 * orderProductInfo.ExtCode2;
                                            Carts.UpdateOrderProductBuySend(new List<OrderProductInfo>() { orderProductInfo });
                                            if (cartProductInfo.GiftList.Count > 0)
                                            {
                                                foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                                {
                                                    gift.RealCount = orderProductInfo.BuyCount * gift.ExtCode2;
                                                }
                                                Carts.UpdateOrderProductCount(cartProductInfo.GiftList);
                                            }
                                            return AjaxResult("userranklowerbuysend", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的买送促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        else if (orderProductInfo.RealCount != (orderProductInfo.BuyCount + (orderProductInfo.BuyCount / buySendPromotionInfo.BuyCount) * buySendPromotionInfo.SendCount))
                                        {
                                            return AjaxResult("changebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经改变，请删除此商品后重新添加");
                                        }
                                    }

                                    //验证单品促销活动
                                    if (orderProductInfo.ExtCode1 > 0)
                                    {
                                        SinglePromotionInfo singlePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                                        if (singlePromotionInfo == null)
                                        {
                                            return AjaxResult("stopsingle", "商品" + orderProductInfo.Name + "的单品促销活动已经停止，请删除此商品后重新添加");
                                        }
                                        if (singlePromotionInfo.PayCredits != orderProductInfo.PayCredits || singlePromotionInfo.CouponTypeId != orderProductInfo.CouponTypeId)
                                        {
                                            return AjaxResult("changesingle", "商品" + orderProductInfo.Name + "的单品促销活动已经改变，请删除此商品后重新添加");
                                        }
                                        if (singlePromotionInfo.UserRankLower > WorkContext.PartUserInfo.UserRid)
                                        {
                                            orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                            orderProductInfo.PayCredits = 0;
                                            orderProductInfo.CouponTypeId = 0;
                                            orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                            Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                            return AjaxResult("userranklowersingle", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的单品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        if (singlePromotionInfo.QuotaLower > 0 && orderProductInfo.BuyCount < singlePromotionInfo.QuotaLower)
                                        {
                                            orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                            orderProductInfo.PayCredits = 0;
                                            orderProductInfo.CouponTypeId = 0;
                                            orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                            Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                            return AjaxResult("orderminsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最少购买" + singlePromotionInfo.QuotaLower + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        if (singlePromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > singlePromotionInfo.QuotaUpper)
                                        {
                                            orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                            orderProductInfo.PayCredits = 0;
                                            orderProductInfo.CouponTypeId = 0;
                                            orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                            Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                            return AjaxResult("ordermuchsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最多购买" + singlePromotionInfo.QuotaLower + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        if (singlePromotionInfo.AllowBuyCount > 0 && Promotions.GetSinglePromotionProductBuyCount(WorkContext.Uid, singlePromotionInfo.PmId) > singlePromotionInfo.AllowBuyCount)
                                        {
                                            orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                            orderProductInfo.PayCredits = 0;
                                            orderProductInfo.CouponTypeId = 0;
                                            orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                            Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                            return AjaxResult("userminsingle", "商品" + orderProductInfo.Name + "的单品促销活动每个人最多购买" + singlePromotionInfo.AllowBuyCount + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        if (singlePromotionInfo.IsStock == 1)
                                        {
                                            SinglePromotionInfo temp = singlePromotionList.Find(x => x.PmId == singlePromotionInfo.PmId);
                                            if (temp == null)
                                            {
                                                temp = singlePromotionInfo;
                                                singlePromotionList.Add(temp);
                                            }

                                            if (temp.Stock < orderProductInfo.RealCount)
                                            {
                                                orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                                orderProductInfo.PayCredits = 0;
                                                orderProductInfo.CouponTypeId = 0;
                                                orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                                Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                                return AjaxResult("stockoutsingle", "商品" + orderProductInfo.Name + "的单品促销活动库存不足,所以您当前只能享受普通购买，请返回购物车重新确认");
                                            }
                                            else
                                            {
                                                temp.Stock -= orderProductInfo.RealCount;
                                            }
                                        }
                                    }

                                    //验证赠品促销活动
                                    if (orderProductInfo.ExtCode3 > 0)
                                    {
                                        GiftPromotionInfo giftPromotionInfo = Promotions.GetGiftPromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                                        if (giftPromotionInfo == null)
                                        {
                                            return AjaxResult("stopgift", "商品" + orderProductInfo.Name + "的赠品促销活动已经停止,请删除此商品后重新添加");
                                        }
                                        else if (giftPromotionInfo.PmId != orderProductInfo.ExtCode3)
                                        {
                                            return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                        }
                                        else if (giftPromotionInfo.UserRankLower > WorkContext.UserRid)
                                        {
                                            orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                            Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                            Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                                            return AjaxResult("userranklowergift", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的赠品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        else if (giftPromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > giftPromotionInfo.QuotaUpper)
                                        {
                                            orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                            Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                            Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                                            return AjaxResult("ordermuchgift", "商品" + orderProductInfo.Name + "的赠品要求每单最多购买" + giftPromotionInfo.QuotaUpper + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        else if (cartProductInfo.GiftList.Count > 0)
                                        {
                                            List<ExtGiftInfo> extGiftList = Promotions.GetExtGiftList(orderProductInfo.ExtCode3);
                                            if (extGiftList.Count != cartProductInfo.GiftList.Count)
                                            {
                                                return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                            }
                                            List<OrderProductInfo> newGiftOrderProductList = new List<OrderProductInfo>(extGiftList.Count);
                                            foreach (ExtGiftInfo extGiftInfo in extGiftList)
                                            {
                                                OrderProductInfo giftOrderProduct = Carts.BuildOrderProduct(extGiftInfo);
                                                Carts.SetGiftOrderProduct(giftOrderProduct, 1, orderProductInfo.RealCount, extGiftInfo.Number, giftPromotionInfo.PmId);
                                                newGiftOrderProductList.Add(giftOrderProduct);
                                            }

                                            //验证赠品信息是否改变
                                            for (int i = 0; i < newGiftOrderProductList.Count; i++)
                                            {
                                                OrderProductInfo newSuitOrderProductInfo = newGiftOrderProductList[i];
                                                OrderProductInfo oldSuitOrderProductInfo = cartProductInfo.GiftList[i];
                                                if (newSuitOrderProductInfo.Pid != oldSuitOrderProductInfo.Pid ||
                                                    newSuitOrderProductInfo.Name != oldSuitOrderProductInfo.Name ||
                                                    newSuitOrderProductInfo.ShopPrice != oldSuitOrderProductInfo.ShopPrice ||
                                                    newSuitOrderProductInfo.MarketPrice != oldSuitOrderProductInfo.MarketPrice ||
                                                    newSuitOrderProductInfo.CostPrice != oldSuitOrderProductInfo.CostPrice ||
                                                    newSuitOrderProductInfo.Type != oldSuitOrderProductInfo.Type ||
                                                    newSuitOrderProductInfo.RealCount != oldSuitOrderProductInfo.RealCount ||
                                                    newSuitOrderProductInfo.BuyCount != oldSuitOrderProductInfo.BuyCount ||
                                                    newSuitOrderProductInfo.ExtCode1 != oldSuitOrderProductInfo.ExtCode1 ||
                                                    newSuitOrderProductInfo.ExtCode2 != oldSuitOrderProductInfo.ExtCode2 ||
                                                    newSuitOrderProductInfo.ExtCode3 != oldSuitOrderProductInfo.ExtCode3 ||
                                                    newSuitOrderProductInfo.ExtCode4 != oldSuitOrderProductInfo.ExtCode4 ||
                                                    newSuitOrderProductInfo.ExtCode5 != oldSuitOrderProductInfo.ExtCode5)
                                                {
                                                    return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                                }
                                            }

                                            //验证赠品库存
                                            foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                            {
                                                ProductStockInfo stockInfo = Products.GetProductStock(gift.Pid, productStockList);
                                                if (stockInfo.Number < gift.RealCount)
                                                {
                                                    if (stockInfo.Number == 0)
                                                    {
                                                        Carts.DeleteOrderProductList(new List<OrderProductInfo>() { gift });
                                                        if (cartProductInfo.GiftList.Count == 1)
                                                        {
                                                            orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                                            Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        gift.RealCount = stockInfo.Number;
                                                        Carts.UpdateOrderProductCount(new List<OrderProductInfo>() { gift });
                                                    }
                                                    return AjaxResult("outstock", "商品" + orderProductInfo.Name + "的赠品" + gift.Name + "库存不足,请返回购物车重新确认");
                                                }
                                                else
                                                {
                                                    stockInfo.Number -= gift.RealCount;
                                                }
                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }
                        }
                        else if (cartItemInfo.Type == 3)//购物车满减
                        {
                            CartFullCutInfo cartFullCutInfo = (CartFullCutInfo)cartItemInfo.Item;

                            #region 验证满减促销

                            if (cartFullCutInfo.FullCutPromotionInfo.UserRankLower > WorkContext.UserRid)
                            {
                                List<OrderProductInfo> updateFullCutOrderProductList = new List<OrderProductInfo>();
                                foreach (CartProductInfo cartProductInfo in cartFullCutInfo.FullCutCartProductList)
                                {
                                    cartProductInfo.OrderProductInfo.ExtCode5 = -1 * cartProductInfo.OrderProductInfo.ExtCode5;
                                    updateFullCutOrderProductList.Add(cartProductInfo.OrderProductInfo);
                                }
                                Carts.UpdateOrderProductFullCut(updateFullCutOrderProductList);
                                return AjaxResult("userranklowerfullcut", "你的用户等级太低，无法参加" + cartFullCutInfo.FullCutPromotionInfo.Name + "满减促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                            }

                            #endregion
                            foreach (CartProductInfo cartProductInfo in cartFullCutInfo.FullCutCartProductList)
                            {
                                if (cartProductInfo.Selected)
                                {
                                    #region 验证

                                    OrderProductInfo orderProductInfo = cartProductInfo.OrderProductInfo;

                                    //验证商品信息
                                    PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                                    if (partProductInfo == null)
                                    {
                                        return AjaxResult("outsaleproduct", "商品" + partProductInfo.Name + "已经下架，请删除此商品");
                                    }
                                    if (orderProductInfo.Name != partProductInfo.Name || orderProductInfo.ShopPrice != partProductInfo.ShopPrice || orderProductInfo.MarketPrice != partProductInfo.MarketPrice || orderProductInfo.CostPrice != partProductInfo.CostPrice || orderProductInfo.Weight != partProductInfo.Weight || orderProductInfo.PSN != partProductInfo.PSN)
                                    {
                                        return AjaxResult("changeproduct", "商品" + partProductInfo.Name + "信息有变化，请删除后重新添加");
                                    }

                                    //验证商品库存
                                    ProductStockInfo productStockInfo = Products.GetProductStock(orderProductInfo.Pid, productStockList);
                                    if (productStockInfo.Number < orderProductInfo.RealCount)
                                    {
                                        return AjaxResult("outstock", "商品" + partProductInfo.Name + "库存不足");
                                    }
                                    else
                                    {
                                        productStockInfo.Number -= orderProductInfo.RealCount;
                                    }

                                    //验证买送促销活动
                                    if (orderProductInfo.ExtCode2 > 0)
                                    {
                                        BuySendPromotionInfo buySendPromotionInfo = Promotions.GetBuySendPromotion(orderProductInfo.BuyCount, orderProductInfo.StoreId, orderProductInfo.Pid, DateTime.Now);
                                        if (buySendPromotionInfo == null)
                                        {
                                            return AjaxResult("stopbuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经停止,请删除此商品后重新添加");
                                        }
                                        else if (buySendPromotionInfo.PmId != orderProductInfo.ExtCode2)
                                        {
                                            return AjaxResult("replacebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经替换，请删除此商品后重新添加");
                                        }
                                        else if (buySendPromotionInfo.UserRankLower > WorkContext.UserRid)
                                        {
                                            orderProductInfo.RealCount = orderProductInfo.BuyCount;
                                            orderProductInfo.ExtCode2 = -1 * orderProductInfo.ExtCode2;
                                            Carts.UpdateOrderProductBuySend(new List<OrderProductInfo>() { orderProductInfo });
                                            if (cartProductInfo.GiftList.Count > 0)
                                            {
                                                foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                                {
                                                    gift.RealCount = orderProductInfo.BuyCount * gift.ExtCode2;
                                                }
                                                Carts.UpdateOrderProductCount(cartProductInfo.GiftList);
                                            }
                                            return AjaxResult("userranklowerbuysend", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的买送促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        else if (orderProductInfo.RealCount != (orderProductInfo.BuyCount + (orderProductInfo.BuyCount / buySendPromotionInfo.BuyCount) * buySendPromotionInfo.SendCount))
                                        {
                                            return AjaxResult("changebuysend", "商品" + orderProductInfo.Name + "的买送促销活动已经改变，请删除此商品后重新添加");
                                        }
                                    }

                                    //验证单品促销活动
                                    if (orderProductInfo.ExtCode1 > 0)
                                    {
                                        SinglePromotionInfo singlePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                                        if (singlePromotionInfo == null)
                                        {
                                            return AjaxResult("stopsingle", "商品" + orderProductInfo.Name + "的单品促销活动已经停止，请删除此商品后重新添加");
                                        }
                                        if (singlePromotionInfo.PayCredits != orderProductInfo.PayCredits || singlePromotionInfo.CouponTypeId != orderProductInfo.CouponTypeId)
                                        {
                                            return AjaxResult("changesingle", "商品" + orderProductInfo.Name + "的单品促销活动已经改变，请删除此商品后重新添加");
                                        }
                                        if (singlePromotionInfo.UserRankLower > WorkContext.PartUserInfo.UserRid)
                                        {
                                            orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                            orderProductInfo.PayCredits = 0;
                                            orderProductInfo.CouponTypeId = 0;
                                            orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                            Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                            return AjaxResult("userranklowersingle", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的单品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        if (singlePromotionInfo.QuotaLower > 0 && orderProductInfo.BuyCount < singlePromotionInfo.QuotaLower)
                                        {
                                            orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                            orderProductInfo.PayCredits = 0;
                                            orderProductInfo.CouponTypeId = 0;
                                            orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                            Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                            return AjaxResult("orderminsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最少购买" + singlePromotionInfo.QuotaLower + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        if (singlePromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > singlePromotionInfo.QuotaUpper)
                                        {
                                            orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                            orderProductInfo.PayCredits = 0;
                                            orderProductInfo.CouponTypeId = 0;
                                            orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                            Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                            return AjaxResult("ordermuchsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最多购买" + singlePromotionInfo.QuotaLower + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        if (singlePromotionInfo.AllowBuyCount > 0 && Promotions.GetSinglePromotionProductBuyCount(WorkContext.Uid, singlePromotionInfo.PmId) > singlePromotionInfo.AllowBuyCount)
                                        {
                                            orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                            orderProductInfo.PayCredits = 0;
                                            orderProductInfo.CouponTypeId = 0;
                                            orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                            Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                            return AjaxResult("userminsingle", "商品" + orderProductInfo.Name + "的单品促销活动每个人最多购买" + singlePromotionInfo.AllowBuyCount + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        if (singlePromotionInfo.IsStock == 1)
                                        {
                                            SinglePromotionInfo temp = singlePromotionList.Find(x => x.PmId == singlePromotionInfo.PmId);
                                            if (temp == null)
                                            {
                                                temp = singlePromotionInfo;
                                                singlePromotionList.Add(temp);
                                            }

                                            if (temp.Stock < orderProductInfo.RealCount)
                                            {
                                                orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                                                orderProductInfo.PayCredits = 0;
                                                orderProductInfo.CouponTypeId = 0;
                                                orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                                                Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                                                return AjaxResult("stockoutsingle", "商品" + orderProductInfo.Name + "的单品促销活动库存不足,所以您当前只能享受普通购买，请返回购物车重新确认");
                                            }
                                            else
                                            {
                                                temp.Stock -= orderProductInfo.RealCount;
                                            }
                                        }
                                    }

                                    //验证赠品促销活动
                                    if (orderProductInfo.ExtCode3 > 0)
                                    {
                                        GiftPromotionInfo giftPromotionInfo = Promotions.GetGiftPromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                                        if (giftPromotionInfo == null)
                                        {
                                            return AjaxResult("stopgift", "商品" + orderProductInfo.Name + "的赠品促销活动已经停止,请删除此商品后重新添加");
                                        }
                                        else if (giftPromotionInfo.PmId != orderProductInfo.ExtCode3)
                                        {
                                            return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                        }
                                        else if (giftPromotionInfo.UserRankLower > WorkContext.UserRid)
                                        {
                                            orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                            Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                            Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                                            return AjaxResult("userranklowergift", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的赠品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        else if (giftPromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > giftPromotionInfo.QuotaUpper)
                                        {
                                            orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                            Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                            Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                                            return AjaxResult("ordermuchgift", "商品" + orderProductInfo.Name + "的赠品要求每单最多购买" + giftPromotionInfo.QuotaUpper + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                                        }
                                        else if (cartProductInfo.GiftList.Count > 0)
                                        {
                                            List<ExtGiftInfo> extGiftList = Promotions.GetExtGiftList(orderProductInfo.ExtCode3);
                                            if (extGiftList.Count != cartProductInfo.GiftList.Count)
                                            {
                                                return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                            }
                                            List<OrderProductInfo> newGiftOrderProductList = new List<OrderProductInfo>(extGiftList.Count);
                                            foreach (ExtGiftInfo extGiftInfo in extGiftList)
                                            {
                                                OrderProductInfo giftOrderProduct = Carts.BuildOrderProduct(extGiftInfo);
                                                Carts.SetGiftOrderProduct(giftOrderProduct, 1, orderProductInfo.RealCount, extGiftInfo.Number, giftPromotionInfo.PmId);
                                                newGiftOrderProductList.Add(giftOrderProduct);
                                            }

                                            //验证赠品信息是否改变
                                            for (int i = 0; i < newGiftOrderProductList.Count; i++)
                                            {
                                                OrderProductInfo newSuitOrderProductInfo = newGiftOrderProductList[i];
                                                OrderProductInfo oldSuitOrderProductInfo = cartProductInfo.GiftList[i];
                                                if (newSuitOrderProductInfo.Pid != oldSuitOrderProductInfo.Pid ||
                                                    newSuitOrderProductInfo.Name != oldSuitOrderProductInfo.Name ||
                                                    newSuitOrderProductInfo.ShopPrice != oldSuitOrderProductInfo.ShopPrice ||
                                                    newSuitOrderProductInfo.MarketPrice != oldSuitOrderProductInfo.MarketPrice ||
                                                    newSuitOrderProductInfo.CostPrice != oldSuitOrderProductInfo.CostPrice ||
                                                    newSuitOrderProductInfo.Type != oldSuitOrderProductInfo.Type ||
                                                    newSuitOrderProductInfo.RealCount != oldSuitOrderProductInfo.RealCount ||
                                                    newSuitOrderProductInfo.BuyCount != oldSuitOrderProductInfo.BuyCount ||
                                                    newSuitOrderProductInfo.ExtCode1 != oldSuitOrderProductInfo.ExtCode1 ||
                                                    newSuitOrderProductInfo.ExtCode2 != oldSuitOrderProductInfo.ExtCode2 ||
                                                    newSuitOrderProductInfo.ExtCode3 != oldSuitOrderProductInfo.ExtCode3 ||
                                                    newSuitOrderProductInfo.ExtCode4 != oldSuitOrderProductInfo.ExtCode4 ||
                                                    newSuitOrderProductInfo.ExtCode5 != oldSuitOrderProductInfo.ExtCode5)
                                                {
                                                    return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                                                }
                                            }

                                            //验证赠品库存
                                            foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                                            {
                                                ProductStockInfo stockInfo = Products.GetProductStock(gift.Pid, productStockList);
                                                if (stockInfo.Number < gift.RealCount)
                                                {
                                                    if (stockInfo.Number == 0)
                                                    {
                                                        Carts.DeleteOrderProductList(new List<OrderProductInfo>() { gift });
                                                        if (cartProductInfo.GiftList.Count == 1)
                                                        {
                                                            orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                                                            Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        gift.RealCount = stockInfo.Number;
                                                        Carts.UpdateOrderProductCount(new List<OrderProductInfo>() { gift });
                                                    }
                                                    return AjaxResult("outstock", "商品" + orderProductInfo.Name + "的赠品" + gift.Name + "库存不足,请返回购物车重新确认");
                                                }
                                                else
                                                {
                                                    stockInfo.Number -= gift.RealCount;
                                                }
                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }
                        }
                    }
                }
                if (Carts.SumMallCartFullCut(storeCartList) != fullCut)
                    return AjaxResult("wrongfullcutmoney", "满减金额不正确,请刷新页面重新提交");

                //验证已经通过,进行订单保存
                int pCount = 0;
                string oidList = "";
                decimal AllMoney = 0;

                decimal couponMoeny = Coupons.SumCouponMoney(couponList);
                int agentMainOid = 0;
                int agentSubOid = 0;
                List<OrderInfo> allOrderList = new List<OrderInfo>();
                foreach (StoreCartInfo storeCartInfo in storeCartList)
                {
                    List<SinglePromotionInfo> storeSinglePromotionList = singlePromotionList.FindAll(x => x.StoreId == storeCartInfo.StoreInfo.StoreId);
                    List<CouponInfo> storeCouponList = couponList;
                    if (!storeCouponList.Exists(x => x.StoreId == -1))
                        couponList.FindAll(x => x.StoreId == storeCartInfo.StoreInfo.StoreId);
                    int storeFullCut = Carts.SumFullCut(storeCartInfo.CartItemList);

                    OrderInfo orderInfo = Orders.CreateOrder(WorkContext.PartUserInfo, storeCartInfo.StoreInfo, storeCartInfo.SelectedOrderProductList, storeSinglePromotionList, fullShipAddressInfo, payPluginInfo, ref payCreditCount, ref haiMiCount, ref hongBaoCount, ref cashCount, storeCouponList, ref couponMoeny, ref daiLiCount, ref YongJinCount, storeFullCut, buyerRemark, invoice, bestTime, WorkContext.IP, cashId, selectCashList, selectExCodeList,incoicemore);

                    #region 货到付款处理 已删除 不支持货到付款方式
                    //如果是旗舰店并且为在货到付款方式 提交订单后自动确认 确认后发送订单信息到直销系统
                    //if (storeCartInfo.StoreInfo.StoreId.ToString() == WebSiteConfig.HealthenStoreId && orderInfo.PayMode == 0)
                    //{
                    //    //SendOutOrder(orderInfo);
                    //    Orders.UpdateOrderState(orderInfo.Oid, OrderState.Confirmed);
                    //    OrderActions.CreateOrderAction(new OrderActionInfo()
                    //    {
                    //        Oid = orderInfo.Oid,
                    //        Uid = orderInfo.Uid,
                    //        RealName = "系统",
                    //        ActionType = (int)OrderActionType.Confirm,
                    //        ActionTime = DateTime.Now,//交易时间,
                    //        ActionDes = "您的订单已经确认,正在备货中"
                    //    });
                    //    PayUtils.SendOutOrder(orderInfo, WorkContext.PartUserInfo);
                    //}
                    #endregion

                    if (orderInfo != null)
                    {
                        oidList += orderInfo.Oid + ",";
                        allOrderList.Add(orderInfo);
                        AllMoney += orderInfo.SurplusMoney;
                        //删除剩余的满赠赠品
                        if (storeCartInfo.RemainedOrderProductList.Count > 0)
                        {
                            List<OrderProductInfo> delOrderProductList = Carts.GetFullSendMinorOrderProductList(storeCartInfo.RemainedOrderProductList);
                            if (delOrderProductList.Count > 0)
                            {
                                Carts.DeleteOrderProductList(delOrderProductList);
                                pCount += Carts.SumOrderProductCount(storeCartInfo.RemainedOrderProductList) - delOrderProductList.Count;
                            }
                        }
                    }
                    else
                        return AjaxResult("error", "提交失败!请返回购物车重新提交");
                    //找到微商订单的主订单id并关联到套餐id
                    if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"))
                    {
                        agentMainOid = orderInfo.Oid;
                    }
                    if (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId"))
                    {
                        agentSubOid = orderInfo.Oid;
                    }

                }
                if (agentMainOid > 0)
                {
                    OrderInfo subOrder = allOrderList.Find(x => x.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId"));
                    if (subOrder != null)
                        Orders.UpdateMainOid(subOrder.Oid, agentMainOid, agentSubOid, 1);
                }
                if (agentSubOid > 0)
                {
                    OrderInfo mainOrder = allOrderList.Find(x => x.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"));
                    if (mainOrder != null)
                        Orders.UpdateMainOid(mainOrder.Oid, agentMainOid, agentSubOid, 2);
                }
                Carts.SetCartProductCountCookie(pCount);
                if (AllMoney > 0)
                    return AjaxResult("success", Url.Action("payshow", new RouteValueDictionary { { "oidList", oidList.TrimEnd(',') } }));
                else
                    return AjaxResult("success", Url.Action("submitresult", new RouteValueDictionary { { "oidList", oidList.TrimEnd(',') } }));
            }
        }

        /// <summary>
        /// 提交结果
        /// </summary>
        public ActionResult SubmitResult()
        {
            //订单id列表
            string oidList = WebHelper.GetQueryString("oidList");

            string paySystemName = "";
            decimal allPayMoney = 0M;
            string onlinePayOidList = "";
            List<OrderInfo> orderList = new List<OrderInfo>();
            foreach (string oid in StringHelper.SplitString(oidList))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                if (orderInfo != null && (orderInfo.Uid == WorkContext.Uid || orderInfo.Uid == 0) && (orderInfo.OrderState == (int)OrderState.WaitPaying || orderInfo.OrderState == (int)OrderState.Confirmed) && (paySystemName.Length == 0 || paySystemName == orderInfo.PaySystemName))
                    orderList.Add(orderInfo);
                else
                    return Redirect("/ucenter/orderlist");

                paySystemName = orderInfo.PaySystemName;
                if (orderInfo.OrderState == (int)OrderState.WaitPaying)
                {
                    allPayMoney += orderInfo.SurplusMoney;
                    if (orderInfo.PayMode == 1)
                        onlinePayOidList += orderInfo.Oid + ",";
                }
            }

            if (orderList.Count < 1)
                return Redirect("/");

            SubmitResultModel model = new SubmitResultModel();
            model.OidList = oidList;
            model.OrderList = orderList;
            model.PayPlugin = Plugins.GetPayPluginBySystemName(paySystemName);
            model.AllPayMoney = allPayMoney;
            model.OnlinePayOidList = onlinePayOidList.TrimEnd(',');
            return View(model);
        }

        /// <summary>
        /// 支付展示
        /// </summary>
        public ActionResult PayShow()
        {
            //订单id列表
            string oidList = WebHelper.GetQueryString("oidList");

            string paySystemName = "";
            decimal allSurplusMoney = 0M;
            List<OrderInfo> orderList = new List<OrderInfo>();
            foreach (string oid in StringHelper.SplitString(oidList))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                if (orderInfo != null && (orderInfo.Uid == WorkContext.Uid || orderInfo.Uid == 0) && orderInfo.OrderState == (int)OrderState.WaitPaying && (paySystemName.Length == 0 || paySystemName == orderInfo.PaySystemName))
                    orderList.Add(orderInfo);
                else
                    return Redirect("/ucenter/orderlist");

                paySystemName = orderInfo.PaySystemName;
                allSurplusMoney += orderInfo.SurplusMoney;
            }

            if (orderList.Count < 1 || allSurplusMoney == 0M)
                return Redirect("/");

            PayShowModel model = new PayShowModel();
            model.OidList = oidList;
            model.OrderList = orderList;
            model.PayPlugin = Plugins.GetPayPluginBySystemName(paySystemName);
            model.ShowView = "~/plugins/" + model.PayPlugin.Folder + "/views/show.cshtml";
            model.AllSurplusMoney = allSurplusMoney;
            return View(model);
        }

        /// <summary>
        /// 支付结果
        /// </summary>
        public ActionResult PayResult()
        {
            //订单id列表
            string oidList = WebHelper.GetQueryString("oidList");

            PayResultModel model = new PayResultModel();
            List<OrderInfo> successOrderList = new List<OrderInfo>();
            List<OrderInfo> failOrderList = new List<OrderInfo>();

            foreach (string oid in StringHelper.SplitString(oidList))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                if (orderInfo != null && (orderInfo.Uid == WorkContext.Uid || orderInfo.Uid == 0) && orderInfo.OrderState == (int)OrderState.Confirmed)
                    successOrderList.Add(orderInfo);
                else
                    failOrderList.Add(orderInfo);
            }

            model.SuccessOrderList = successOrderList;
            model.FailOrderList = failOrderList;
            model.State = failOrderList.Count > 0 ? 0 : 1;
            return View(model);
        }
        /// <summary>
        /// 支付结果
        /// </summary>
        /// <param name="oids"></param>
        /// <param name="paystatus"></param>
        /// <returns></returns>
        public ActionResult ResultPay(string oids, string paystatus)
        {
            PayResultModel payResuleModel = new PayResultModel();
            List<OrderInfo> successOrderList = new List<OrderInfo>();
            List<OrderInfo> failOrderList = new List<OrderInfo>();
            foreach (string oid in StringHelper.SplitString(oids, ","))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                //if (orderInfo != null && orderInfo.Uid == WorkContext.Uid && orderInfo.OrderState == (int)OrderState.Confirmed)
                //    successOrderList.Add(orderInfo);
                if (orderInfo != null && (orderInfo.Uid == WorkContext.Uid || orderInfo.Uid == 0) && orderInfo.OrderState == (int)OrderState.Confirmed && orderInfo.OrderState <= (int)OrderState.Completed && (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId") || orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId")))
                {
                    successOrderList.Add(orderInfo);
                }
                else if (orderInfo != null && (orderInfo.Uid == WorkContext.Uid || orderInfo.Uid == 0) && (orderInfo.OrderState == (int)OrderState.Confirmed || orderInfo.OrderState == (int)OrderState.Completed) && orderInfo.StoreId != WebHelper.GetConfigSettingsInt("AgentStoreId") && orderInfo.StoreId != WebHelper.GetConfigSettingsInt("AgentSuitStoreId"))
                {
                    successOrderList.Add(orderInfo);
                }
                else
                    failOrderList.Add(orderInfo);
            }
            if (paystatus == "1")
            {
                List<OrderInfo> orderForExCode = new List<OrderInfo>();
                //List<OrderProductInfo> orderProductForExCode = new List<OrderProductInfo>();
                foreach (var item in successOrderList)
                {
                    if (item.StoreId.ToString() != WebHelper.GetConfigSettings("SRCStoreId") && item.CashDiscount <= 0)
                    {
                        orderForExCode.Add(item);
                        //orderProductForExCode.AddRange(Orders.GetOrderProductList(item.Oid));
                    }
                }
                decimal AmountForExCode = orderForExCode.Sum(x => x.OrderAmount);
                //订单赠送兑换码
                List<int> testuid = new List<int>() { 1, 8081, 18782, 23073, 27460, 31506, 32138, 32861, 32977, 32979, 32986, 33492, 33497, 33499, 33500, 15171, 29361 };
                //不使用汇购卡支付，不属于尚睿淳店铺，时间在5月16之前，订单金额大于299
                if (((DateTime.Now < new DateTime(2017, 5, 16) && DateTime.Now >= new DateTime(2017, 4, 1)) || testuid.Exists(x => x == successOrderList.First().Uid)) && AmountForExCode >= 299)
                {
                    ExChangeCouponsInfo codeInfo = new ExChangeCouponsInfo();
                    codeInfo.createoid = successOrderList.First().Oid;
                    codeInfo.createtime = DateTime.Now;
                    codeInfo.createuid = successOrderList.First().Uid;
                    codeInfo.state = 1;
                    codeInfo.validtime = new DateTime(2017, 6, 1);
                    codeInfo.uid = successOrderList.First().Uid;
                    if (AmountForExCode >= 299 && AmountForExCode < 1999)
                    {
                        codeInfo.codetypeid = 3;
                        codeInfo.type = 3;
                        codeInfo.exsn = "YDH" + Randoms.CreateRandomValue(8, true).ToLower();
                    }
                    else if (AmountForExCode >= 1999 && AmountForExCode < 4599)
                    {
                        codeInfo.codetypeid = 4;
                        codeInfo.type = 4;
                        codeInfo.exsn = "JDH" + Randoms.CreateRandomValue(8, true).ToLower();
                    }
                    else if (AmountForExCode >= 4599)
                    {
                        codeInfo.codetypeid = 5;
                        codeInfo.type = 5;
                        codeInfo.exsn = "ZDH" + Randoms.CreateRandomValue(8, true).ToLower();
                    }
                    ExChangeCoupons.Add(codeInfo);
                }

            }
            payResuleModel.SuccessOrderList = successOrderList;
            payResuleModel.FailOrderList = failOrderList;
            payResuleModel.State = paystatus == "0" ? -1 : (paystatus == "1" ? 1 : 0);
            return View("PayResult", payResuleModel);
        }

        #region 加入蓝钻
        /// <summary>
        /// 加入蓝钻计划 
        /// </summary>
        /// <param name="oids">订单id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult JoinPlan(string oids)
        {
            OrderInfo info = Orders.GetOrderByOid(TypeHelper.StringToInt(oids));
            if (string.IsNullOrEmpty(oids) || WorkContext.Uid < 1 || info == null || (info != null && info.Uid != WorkContext.Uid) || info.ProductAmount < 5800)
            {
                return PromptView(WorkContext.UrlReferrer, "您不符合成为蓝钻会员的的条件！");
            }
            if (WorkContext.IsDirSaleUser)
            {
                return PromptView(WorkContext.UrlReferrer, "您已经是蓝钻会员，无需再加入！");
            }
            string parentCode = OrderUtils.GetParentCode(WorkContext.Uid);
            ViewData["parentCode"] = parentCode;
            ViewData["oids"] = oids;

            return View();
        }

        /// <summary>
        /// 加入蓝钻计划
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //public ActionResult JoinPlan()
        //{
        //    string oids = WebHelper.GetFormString("oids");
        //    OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oids));
        //    if (orderInfo != null)
        //    {
        //        RegionInfo reinfo = Regions.GetRegionById(orderInfo.RegionId);//区域信息
        //    }
        //    else
        //    {
        //        return AjaxResult("error", "[{\"key\":\"orderinfo\",\"msg\":\"订单信息错误\"}]", true);
        //    }
        //    string userCode = string.IsNullOrEmpty(WorkContext.UserName) ? (string.IsNullOrEmpty(WorkContext.UserEmail) ? (string.IsNullOrEmpty(WorkContext.UserMobile) ? "" : WorkContext.UserMobile) : WorkContext.UserEmail) : WorkContext.UserName; //userCode：登录名
        //    if (string.IsNullOrEmpty(userCode))
        //    {
        //        return AjaxResult("error", "[{\"key\":\"username\",\"msg\":\"登录名不能为空\"}]", true);
        //    }
        //    string userName = userCode;//userName：用户名

        //    string nickName = string.IsNullOrEmpty(WorkContext.NickName) ? userCode : WorkContext.NickName;//nickName：昵称
        //    string password = System.Web.HttpUtility.UrlEncode(WorkContext.PartUserInfo.DirSalePwd);//System.Web.HttpUtility.UrlEncode(SecureHelper.EncryptString(OrderUtils.getCookiePassword(), DirSaleUserInfo.EncryptKey));//加密后的登录密码  SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);

        //    string pcode = OrderUtils.GetParentCode(WorkContext.Uid);
        //    string parentCode = string.IsNullOrEmpty(pcode) ? WebHelper.GetFormString("parentCode") : pcode;//WebHelper.GetFormString("parentCode"); ;//parentCode：推荐人编号
        //    string managerCode = WebHelper.GetFormString("managerCode"); //managerCode：安置人编号
        //    if (WebHelper.GetFormString("managertype") == "1" && string.IsNullOrEmpty(WebHelper.GetFormString("managerCode")))
        //    {
        //        return AjaxResult("error", "[{\"key\":\"managerCode\",\"msg\":\"自主填写安置人编号不能为空\"}]", true);
        //    }
        //    string managerArea = WebHelper.GetFormString("managerArea"); ;//managerArea：安置分区
        //    string userCard = WebHelper.GetFormString("userCard"); ;//userCard：用户身份证号
        //    string userPhone = WebHelper.GetFormString("userPhone"); ;//userPhone：电话
        //    string provice = "";//    provice：省
        //    string city = "";//city：市
        //    string area = "";//：区或者县
        //    string address = "";//address:具体地址
        //    if (orderInfo != null)
        //    {
        //        RegionInfo reinfo = Regions.GetRegionById(orderInfo.RegionId);//区域信息
        //        provice = reinfo.ProvinceName;
        //        city = reinfo.CityName;
        //        area = reinfo.Name;
        //        address = orderInfo.Address;
        //    }


        //    string hongbao = string.Empty; ;
        //    string haimi = string.Empty;
        //    string[] account = OrderUtils.GetAccountDetail(WorkContext.Uid);
        //    hongbao = account[0];
        //    haimi = account[1];
        //    try
        //    {
        //        //string ApiUrlHost = "http://192.168.88.99:8088";
        //        //http://www.hzs6666.com/api/User/CreateMember?userCode=111&userName=111&nickName=111&password=12345&parentCode=999&managerCode=999&managerArea=1&userCard=888888888888888888&userPhone=88888888888&provice=**&city=**&area=**
        //        string url = string.Format("{0}/api/User/CreateMember?userCode={1}&userName={2}&nickName={3}&password={4}&parentCode={5}&managerCode={6}&managerArea={7}&userCard={8}&userPhone={9}&provice={10}&city={11}&area={12}&address={13}&hongbao={14}&haimi={15}", dirSaleApiUrl, userCode, userName, nickName, password, parentCode, managerCode, managerArea, userCard, userPhone, provice, city, area, address, hongbao, haimi);

        //        string FromDirSale = WebHelper.DoGet(url);
        //        JObject jsonObject = (JObject)JsonConvert.DeserializeObject(FromDirSale);
        //        JToken token = (JToken)jsonObject;
        //        if (token["Result"].ToString() == "0")
        //        {
        //            //{"Result":0,"Msg":"用户注册成功","Info":{"ParentCode":"company","ManagerCode":"zhangjunsheng10"}}
        //            string resultParentCode = token["Info"].SelectToken("ParentCode").ToString();
        //            string resultManagerCode = token["Info"].SelectToken("ManagerCode").ToString();
        //            int resultUserId = TypeHelper.ObjectToInt(token["Info"].SelectToken("UserId"));//新增返回userId
        //            //创建会员成功 创建订单
        //            //http://www.xxxx.com/api/product/CreateUserOrder?userId=xxx&userCode=xxxs&password=xxx&consignee=xxx&phone=xxx&orderCode=xxx&provice=xxx&city=xxx&area=xxx&address=xxx&postCode=xxx&proData=xxx&orderType=xxx 
        //            //orderType：1-海客购物，10-直销会员购物

        //            string orderUrl = string.Format("{0}/api/product/CreateUserOrder?userCode={1}&password={2}&consignee={3}&phone={4}&orderCode={5}&provice={6}&city={7}&area={8}&address={9}&postCode={10}&proData={11}&userId={12}&orderType={13}", dirSaleApiUrl, userCode, password, orderInfo.Consignee, orderInfo.Mobile, orderInfo.OSN, provice, city, area, address, orderInfo.ZipCode, WebUtils.getOrderProductStr(Orders.GetOrderProductList(TypeHelper.StringToInt(oids))), resultUserId, (int)OrderSource.直销后台);
        //            string FromDirSale2 = WebHelper.DoGet(orderUrl);
        //            try //请求不成功会报错
        //            {
        //                JObject jsonObject2 = (JObject)JsonConvert.DeserializeObject(FromDirSale2);
        //                JToken token2 = (JToken)jsonObject2;

        //                //加入成功后要重新用直销账号登陆，注销当前账号，同时写入该账号同时存在直销后台的数据库字段标识
        //                if (token2["Result"].ToString() == "0" && OrderUtils.UpdateUserOutSates(resultUserId, WorkContext.Uid) > 0)
        //                {
        //                    return AjaxResult("success", "[{\"key\":\"showsuccessinfo\",\"msg\":{\"ParentCode\":\"" + resultParentCode + "\",\"ManagerCode\":\"" + resultManagerCode + "\"}}]", true);
        //                }
        //                else
        //                {
        //                    LogHelper.WriteOperateLog("UpdateUserOutSates", "加入蓝钻会员api接口请求错误", "错误信息为：会员ID：" + WorkContext.Uid);
        //                    return AjaxResult("error", "[{\"key\":\"syserror\",\"msg\":\"加入失败,可能存在以下原因：\r\n" + token["Msg"].ToString() + "\"}]", true);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                LogHelper.WriteOperateLog("CreateUserOrder", "请求创建订单api接口错误", "错误信息为：会员ID：" + WorkContext.Uid + "|直销会员ID" + resultUserId + "|错误信息" + ex.Message);
        //                return AjaxResult("error", "[{\"key\":\"syserror\",\"msg\":\"请求错误\"}]", true);
        //            }
        //        }
        //        else
        //        {
        //            return AjaxResult("error", "[{\"key\":\"Requesterror\",\"msg\":\"" + token["Msg"].ToString() + "\"}]", true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.WriteOperateLog("CreateMemberError", "请求会员api接口错误", "错误信息为：会员ID：" + WorkContext.Uid + "错误信息" + ex.Message);
        //        return AjaxResult("error", "[{\"key\":\"syserror\",\"msg\":\"请求错误\"}]", true);
        //    }
        //}

        public ActionResult JoinResult(string parentCode, string managerCode)
        {
            ViewData["parentCode"] = parentCode;
            ViewData["managerCode"] = managerCode;
            return View();
        }

        #endregion

        /// <summary>
        /// 扫码支付页面刷新验证支付状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NativePayCheck()
        {
            string oidList = WebHelper.GetFormString("oidList");
            List<OrderInfo> orderList = new List<OrderInfo>();
            List<OrderInfo> failorderList = new List<OrderInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (string oid in StringHelper.SplitString(oidList))
            {
                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));
                if (orderInfo != null)
                {
                    if (orderInfo.PayMode == 1 && !string.IsNullOrEmpty(orderInfo.PaySN) && orderInfo.PaySystemName == "wechatpay")
                        orderList.Add(orderInfo);
                    else
                        failorderList.Add(orderInfo);
                }
                else
                {

                    return Content("{\"state\":\"fail\"}");
                }
            }
            if (failorderList.Count > 0)
            {
                return Content("{\"state\":\"fail\"}");
            }
            sb.Append("}");
            return Content("{\"state\":\"success\"}");
        }

        /// <summary>
        /// 重销分期
        /// </summary>
        /// <param name="oids"></param>
        /// <returns></returns>
        public ActionResult ChongXiaoFenQi(string oids, int PhasedType)
        {
            FenQiResultModel model = new FenQiResultModel();
            string SuccessOidList = "";
            string FailOidList = "";
            foreach (string oid in StringHelper.SplitString(oids, ","))
            {
                bool result = Orders.SetFenQi(TypeHelper.StringToInt(oid), PhasedType);
                if (result)
                {
                    SuccessOidList += oid + ",";
                }
                else
                {
                    FailOidList += oid + ",";
                }
            }

            model.SuccessOidList = SuccessOidList.TrimEnd(',');
            model.FailOidList = FailOidList.TrimEnd(',');
            return View(model);
        }

        /// <summary>
        /// 支付测试--跳过支付直接将订单置为支付成功 以便后续功能测试
        /// </summary>
        /// <param name="oids"></param>
        /// <param name="paystatus"></param>
        /// <returns></returns>
        public ActionResult TestPay(string oids, string paystatus)
        {
            PayResultModel payResuleModel = new PayResultModel();
            List<OrderInfo> successOrderList = new List<OrderInfo>();
            List<OrderInfo> failOrderList = new List<OrderInfo>();
            foreach (string oid in StringHelper.SplitString(oids, ","))
            {
                //伪付款成功
                Orders.PayOrder(TypeHelper.StringToInt(oid), OrderState.Confirmed, "11111111111111", DateTime.Now);
                //LogHelper.WriteOperateLog("TestPay", "测试支付后台修改状态记录", "修改信息为：订单id：" + oid + "交易号:11111111111111" + "交易状态" + OrderState.Confirmed);
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = TypeHelper.StringToInt(oid),
                    Uid = WorkContext.Uid,
                    RealName = "系统测试操作",
                    ActionType = (int)OrderActionType.Pay,
                    ActionTime = DateTime.Now,//交易时间,
                    ActionDes = "系统测试支付订单成功，交易流水号为:11111111111111"
                });

                //订单信息
                OrderInfo orderInfo = Orders.GetOrderByOid(TypeHelper.StringToInt(oid));

                //支付成功后自动确认
                Orders.ConfirmOrder(orderInfo);
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = orderInfo.Uid,
                    RealName = "系统",
                    ActionType = (int)OrderActionType.Confirm,
                    ActionTime = DateTime.Now,//交易时间,
                    ActionDes = "您的订单已经确认,正在备货中"
                });
                //发送外部订单

                //OrderUtils.SendOutOrder(orderInfo, WorkContext.PartUserInfo);


                if (orderInfo != null && (orderInfo.Uid == WorkContext.Uid || orderInfo.Uid == 0) && orderInfo.OrderState == (int)OrderState.Confirmed && orderInfo.OrderState <= (int)OrderState.Completed && (orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId") || orderInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId")))
                {
                    successOrderList.Add(orderInfo);
                }
                else if (orderInfo != null && orderInfo.Uid == WorkContext.Uid && orderInfo.OrderState == (int)OrderState.Confirmed && orderInfo.StoreId != WebHelper.GetConfigSettingsInt("AgentStoreId") && orderInfo.StoreId != WebHelper.GetConfigSettingsInt("AgentSuitStoreId"))
                {
                    successOrderList.Add(orderInfo);
                }
                else
                    failOrderList.Add(orderInfo);
            }
            payResuleModel.SuccessOrderList = successOrderList;
            payResuleModel.FailOrderList = failOrderList;
            payResuleModel.State = paystatus == "0" ? -1 : (paystatus == "1" ? 1 : 0);
            return View("PayResult", payResuleModel);
        }

        protected sealed override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            //不允许游客访问
            //if (WorkContext.Uid < 1 && WorkContext.Action != "confirmorder_directbuy" && WorkContext.Action != "submitorder_directbuy" && WorkContext.Action != "submitresult" && WorkContext.Action != "payshow" && WorkContext.Action != "payresult" && WorkContext.Action != "resultpay" && WorkContext.Action != "testpay" && WorkContext.Action != "nativepaycheck")
            if (WorkContext.Uid < 1  )
            {
                if (WorkContext.IsHttpAjax)//如果为ajax请求
                    filterContext.Result = AjaxResult("nologin", "请先登录");
                else//如果为普通请求
                    filterContext.Result = RedirectToAction("login", "account", new RouteValueDictionary { { "returnUrl", WorkContext.Url } });
            }
        }
    }
}
