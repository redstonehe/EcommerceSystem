using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using VMall.Core;
using System.Data;

namespace VMall.Services
{
    /// <summary>
    /// 购物车操作管理类
    /// </summary>
    public partial class Carts
    {
        private static ICartStrategy _icartstrategy = BMACart.Instance;//购物车策略

        /// <summary>
        /// 是否持久化订单商品
        /// </summary>
        public static bool IsPersistOrderProduct
        {
            get { return _icartstrategy.IsPersistOrderProduct; }
        }

        /// <summary>
        /// 获得购物车中商品数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static int GetCartProductCount(int uid)
        {
            if (uid < 1)
                return 0;
            return _icartstrategy.GetCartProductCount(uid);
        }

        /// <summary>
        /// 获得购物车中商品数量
        /// </summary>
        /// <param name="sid">用户sid</param>
        /// <returns></returns>
        public static int GetCartProductCount(string sid)
        {
            if (string.IsNullOrWhiteSpace(sid))
                return 0;
            return _icartstrategy.GetCartProductCount(sid);
        }

        /// <summary>
        /// 获得购物车中商品数量
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="sid">用户sid</param>
        /// <returns></returns>
        public static int GetCartProductCount(int uid, string sid)
        {
            if (uid > 0)
                return GetCartProductCount(uid);
            else
                return GetCartProductCount(sid);
        }

        /// <summary>
        /// 添加订单商品列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        public static void AddOrderProductList(List<OrderProductInfo> orderProductList)
        {
            _icartstrategy.AddOrderProductList(orderProductList);
        }

        /// <summary>
        /// 删除订单商品列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        public static void DeleteOrderProductList(List<OrderProductInfo> orderProductList)
        {
            _icartstrategy.DeleteOrderProductList(orderProductList);
        }

        /// <summary>
        /// 更新订单商品的数量
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        public static void UpdateOrderProductCount(List<OrderProductInfo> orderProductList)
        {
            _icartstrategy.UpdateOrderProductCount(orderProductList);
        }

        /// <summary>
        /// 更新购物车的用户id
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="sid">用户sid</param>
        public static void UpdateCartUidBySid(int uid, string sid)
        {
            _icartstrategy.UpdateCartUidBySid(uid, sid);
        }

        /// <summary>
        /// 更新订单商品的买送促销活动
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        public static void UpdateOrderProductBuySend(List<OrderProductInfo> orderProductList)
        {
            _icartstrategy.UpdateOrderProductBuySend(orderProductList);
        }

        /// <summary>
        /// 更新订单商品的单品促销活动
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        public static void UpdateOrderProductSingle(List<OrderProductInfo> orderProductList)
        {
            _icartstrategy.UpdateOrderProductSingle(orderProductList);
        }

        /// <summary>
        /// 更新订单商品的赠品促销活动
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        public static void UpdateOrderProductGift(List<OrderProductInfo> orderProductList)
        {
            _icartstrategy.UpdateOrderProductGift(orderProductList);
        }

        /// <summary>
        /// 更新订单商品的满赠促销活动
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        public static void UpdateOrderProductFullSend(List<OrderProductInfo> orderProductList)
        {
            _icartstrategy.UpdateOrderProductFullSend(orderProductList);
        }

        /// <summary>
        /// 更新订单商品的满减促销活动
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        public static void UpdateOrderProductFullCut(List<OrderProductInfo> orderProductList)
        {
            _icartstrategy.UpdateOrderProductFullCut(orderProductList);
        }

        /// <summary>
        /// 获得购物车商品列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetCartProductList(int uid, int mallsource = 0)
        {
            return _icartstrategy.GetCartProductList(uid, mallsource);
        }

        /// <summary>
        /// 获得购物车商品列表
        /// </summary>
        /// <param name="sid">用户sid</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetCartProductList(string sid, int mallsource = 0)
        {
            return _icartstrategy.GetCartProductList(sid, mallsource);
        }

        /// <summary>
        /// 获得购物车商品列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="sid">用户sid</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetCartProductList(int uid, string sid, int mallsource = 0)
        {
            if (uid > 0)
                return GetCartProductList(uid, mallsource);
            else
                return GetCartProductList(sid, mallsource);
        }

        /// <summary>
        /// 清空购物车
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <returns></returns>
        public static int ClearCart(int uid)
        {
            return _icartstrategy.ClearCart(uid);
        }

        /// <summary>
        /// 清空购物车
        /// </summary>
        /// <param name="sid">sid</param>
        /// <returns></returns>
        public static int ClearCart(string sid)
        {
            return _icartstrategy.ClearCart(sid);
        }

        /// <summary>
        /// 清空购物车
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="sid">sid</param>
        /// <returns></returns>
        public static int ClearCart(int uid, string sid)
        {
            if (uid > 0)
                return ClearCart(uid);
            else
                return ClearCart(sid);
        }

        /// <summary>
        /// 清空过期购物车
        /// </summary>
        /// <param name="expireTime">过期时间</param>
        public static void ClearExpiredCart(DateTime expireTime)
        {
            _icartstrategy.ClearExpiredCart(expireTime);
        }


        #region 更新购物车--与促销活动同步
        /// <summary>
        /// 更新单品促销产品价格
        /// </summary>
        /// <param name="orderProductList"></param>
        /// <param name="uid"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static List<OrderProductInfo> UpdateCartForPromotionActive(List<OrderProductInfo> orderProductList, int uid, string sid)
        {
            foreach (var orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0)
                {
                    PartUserInfo user = Users.GetPartUserById(orderProductInfo.Uid);
                    //验证商品信息
                    PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                    if (partProductInfo != null)
                    {
                        //当购物车中产品原本属于单品促促销类型
                        if (orderProductInfo.ExtCode1 > 0)
                        {
                            SinglePromotionInfo singlePromotionInfo2 = Promotions.GetSinglePromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                            //当单品促销活动不存在或者无效的时候
                            if (singlePromotionInfo2 == null)
                            {
                                //更新为普通产品类型
                                UpdateCartProductFromSingleToProduct(orderProductInfo, partProductInfo);
                                //return AjaxResult("stopsingle", "商品" + orderProductInfo.Name + "的单品促销活动已经停止，请删除此商品后重新添加");
                            }
                            //单品促销存在变动
                            else
                            {
                                //更新单品促销
                                decimal discountprice = Promotions.ComputeDiscountPrice(partProductInfo.ShopPrice, singlePromotionInfo2, partProductInfo.MarketPrice, orderProductInfo.BuyCount);
                                if (singlePromotionInfo2.PromoHaiMi != orderProductInfo.ProductHaiMi || singlePromotionInfo2.PromoHongBaoCut != orderProductInfo.ProductHBCut || singlePromotionInfo2.PromoPV != orderProductInfo.ProductPV || discountprice != orderProductInfo.DiscountPrice)
                                    if (orderProductInfo.BuyCount >= singlePromotionInfo2.QuotaLower && singlePromotionInfo2.IsStock == 0)
                                        if (singlePromotionInfo2.UserRankLower == Convert.ToInt32(user.IsDirSaleUser) || singlePromotionInfo2.UserRankLower == 0)
                                        {
                                            if ((singlePromotionInfo2.DiscountType == 8 && orderProductInfo.BuyCount % 2 == 0) || singlePromotionInfo2.DiscountType != 8)
                                            {
                                                UpdateCartProductFromSinglePromotion(singlePromotionInfo2, orderProductInfo, discountprice);
                                            }
                                            if (singlePromotionInfo2.DiscountType == 8 && orderProductInfo.BuyCount % 2 == 1)
                                                UpdateCartProductFromSingleToProduct(orderProductInfo, partProductInfo);

                                        }
                                if (singlePromotionInfo2.IsStock == 1 && singlePromotionInfo2.Stock < orderProductInfo.RealCount)
                                {
                                    UpdateCartProductFromSingleToProduct(orderProductInfo, partProductInfo);
                                }
                            }
                        }
                        else //当购物车中产品不为单品促促销类型
                        {
                            //同步单品促销
                            SinglePromotionInfo singlePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(partProductInfo.Pid, DateTime.Now);
                            //当单品促销活动存在时则设置订单商品信息
                            if (singlePromotionInfo != null)
                            {
                                decimal discountprice = Promotions.ComputeDiscountPrice(partProductInfo.ShopPrice, singlePromotionInfo, partProductInfo.MarketPrice, orderProductInfo.BuyCount);
                                if (singlePromotionInfo.PromoHaiMi != orderProductInfo.ProductHaiMi || singlePromotionInfo.PromoHongBaoCut != orderProductInfo.ProductHBCut || singlePromotionInfo.PromoPV != orderProductInfo.ProductPV || discountprice != orderProductInfo.DiscountPrice)
                                    if ((singlePromotionInfo.UserRankLower == Convert.ToInt32(user.IsDirSaleUser) || singlePromotionInfo.UserRankLower == 0) && (singlePromotionInfo.IsStock == 0 || (singlePromotionInfo.IsStock == 1 && singlePromotionInfo.Stock >= orderProductInfo.RealCount)))
                                        UpdateCartProductFromSinglePromotion(singlePromotionInfo, orderProductInfo, discountprice);
                            }
                            else
                            {
                                if (orderProductInfo.DiscountPrice != partProductInfo.ShopPrice || orderProductInfo.ShopPrice != partProductInfo.ShopPrice || orderProductInfo.MarketPrice != partProductInfo.MarketPrice || orderProductInfo.StoreSTid != partProductInfo.StoreSTid || partProductInfo.HaiMi != orderProductInfo.ProductHaiMi || partProductInfo.HongBaoCut != orderProductInfo.ProductHBCut || partProductInfo.PV != orderProductInfo.ProductPV)
                                {
                                    //同步商品信息至订单产品表
                                    //UpdateCartProductFromProductModify(orderProductInfo,partProductInfo);
                                }
                            }
                        }

                        //验证买送促销活动

                        //BuySendPromotionInfo buySendPromotionInfo = Promotions.GetBuySendPromotion(orderProductInfo.BuyCount, orderProductInfo.StoreId, orderProductInfo.Pid, DateTime.Now);
                        //if (buySendPromotionInfo != null) { }
                    }
                    #region
                    //    SinglePromotionInfo singlePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                    //    if (singlePromotionInfo == null)
                    //    {
                    //        return AjaxResult("stopsingle", "商品" + orderProductInfo.Name + "的单品促销活动已经停止，请删除此商品后重新添加");
                    //    }
                    //    if (singlePromotionInfo.PayCredits != orderProductInfo.PayCredits || singlePromotionInfo.CouponTypeId != orderProductInfo.CouponTypeId)
                    //    {
                    //        return AjaxResult("changesingle", "商品" + orderProductInfo.Name + "的单品促销活动已经改变，请删除此商品后重新添加");
                    //    }
                    //    if (singlePromotionInfo.UserRankLower > WorkContext.PartUserInfo.UserRid)
                    //    {
                    //        orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                    //        orderProductInfo.PayCredits = 0;
                    //        orderProductInfo.CouponTypeId = 0;
                    //        orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                    //        Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                    //        return AjaxResult("userranklowersingle", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的单品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                    //    }
                    //    if (singlePromotionInfo.QuotaLower > 0 && orderProductInfo.BuyCount < singlePromotionInfo.QuotaLower)
                    //    {
                    //        orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                    //        orderProductInfo.PayCredits = 0;
                    //        orderProductInfo.CouponTypeId = 0;
                    //        orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                    //        Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                    //        return AjaxResult("orderminsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最少购买" + singlePromotionInfo.QuotaLower + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                    //    }
                    //    if (singlePromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > singlePromotionInfo.QuotaUpper)
                    //    {
                    //        orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                    //        orderProductInfo.PayCredits = 0;
                    //        orderProductInfo.CouponTypeId = 0;
                    //        orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                    //        Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                    //        return AjaxResult("ordermuchsingle", "商品" + orderProductInfo.Name + "的单品促销活动要求每单最多购买" + singlePromotionInfo.QuotaUpper + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                    //    }
                    //    if (singlePromotionInfo.AllowBuyCount > 0 && Promotions.GetSinglePromotionProductBuyCount(WorkContext.Uid, singlePromotionInfo.PmId) > singlePromotionInfo.AllowBuyCount)
                    //    {
                    //        orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                    //        orderProductInfo.PayCredits = 0;
                    //        orderProductInfo.CouponTypeId = 0;
                    //        orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                    //        Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                    //        return AjaxResult("userminsingle", "商品" + orderProductInfo.Name + "的单品促销活动每个人最多购买" + singlePromotionInfo.AllowBuyCount + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                    //    }
                    //    if (singlePromotionInfo.IsStock == 1)
                    //    {
                    //        SinglePromotionInfo temp = singlePromotionList.Find(x => x.PmId == singlePromotionInfo.PmId);
                    //        if (temp == null)
                    //        {
                    //            temp = singlePromotionInfo;
                    //            singlePromotionList.Add(temp);
                    //        }

                    //        if (temp.Stock < orderProductInfo.RealCount)
                    //        {
                    //            orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice;
                    //            orderProductInfo.PayCredits = 0;
                    //            orderProductInfo.CouponTypeId = 0;
                    //            orderProductInfo.ExtCode1 = -1 * orderProductInfo.ExtCode1;
                    //            Carts.UpdateOrderProductSingle(new List<OrderProductInfo>() { orderProductInfo });
                    //            return AjaxResult("stockoutsingle", "商品" + orderProductInfo.Name + "的单品促销活动库存不足,所以您当前只能享受普通购买，请返回购物车重新确认");
                    //        }
                    //        else
                    //        {
                    //            temp.Stock -= orderProductInfo.RealCount;
                    //        }
                    //    }


                    ////验证赠品促销活动
                    //if (orderProductInfo.ExtCode3 > 0)
                    //{
                    //    GiftPromotionInfo giftPromotionInfo = Promotions.GetGiftPromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                    //    if (giftPromotionInfo == null)
                    //    {
                    //        return AjaxResult("stopgift", "商品" + orderProductInfo.Name + "的赠品促销活动已经停止,请删除此商品后重新添加");
                    //    }
                    //    else if (giftPromotionInfo.PmId != orderProductInfo.ExtCode3)
                    //    {
                    //        return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                    //    }
                    //    else if (giftPromotionInfo.UserRankLower > WorkContext.UserRid)
                    //    {
                    //        orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                    //        Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                    //        Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                    //        return AjaxResult("userranklowergift", "你的用户等级太低，无法参加商品" + orderProductInfo.Name + "的赠品促销活动,所以您当前只能享受普通购买，请返回购物车重新确认");
                    //    }
                    //    else if (giftPromotionInfo.QuotaUpper > 0 && orderProductInfo.BuyCount > giftPromotionInfo.QuotaUpper)
                    //    {
                    //        orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                    //        Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                    //        Carts.DeleteOrderProductList(cartProductInfo.GiftList);
                    //        return AjaxResult("ordermuchgift", "商品" + orderProductInfo.Name + "的赠品要求每单最多购买" + giftPromotionInfo.QuotaUpper + "个,所以您当前只能享受普通购买，请返回购物车重新确认");
                    //    }
                    //    else if (cartProductInfo.GiftList.Count > 0)
                    //    {
                    //        List<ExtGiftInfo> extGiftList = Promotions.GetExtGiftList(orderProductInfo.ExtCode3);
                    //        if (extGiftList.Count != cartProductInfo.GiftList.Count)
                    //        {
                    //            return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                    //        }
                    //        List<OrderProductInfo> newGiftOrderProductList = new List<OrderProductInfo>(extGiftList.Count);
                    //        foreach (ExtGiftInfo extGiftInfo in extGiftList)
                    //        {
                    //            OrderProductInfo giftOrderProduct = Carts.BuildOrderProduct(extGiftInfo);
                    //            Carts.SetGiftOrderProduct(giftOrderProduct, 1, orderProductInfo.RealCount, extGiftInfo.Number, giftPromotionInfo.PmId);
                    //            newGiftOrderProductList.Add(giftOrderProduct);
                    //        }

                    //        //验证赠品信息是否改变
                    //        for (int i = 0; i < newGiftOrderProductList.Count; i++)
                    //        {
                    //            OrderProductInfo newSuitOrderProductInfo = newGiftOrderProductList[i];
                    //            OrderProductInfo oldSuitOrderProductInfo = cartProductInfo.GiftList[i];
                    //            if (newSuitOrderProductInfo.Pid != oldSuitOrderProductInfo.Pid ||
                    //                newSuitOrderProductInfo.Name != oldSuitOrderProductInfo.Name ||
                    //                newSuitOrderProductInfo.ShopPrice != oldSuitOrderProductInfo.ShopPrice ||
                    //                newSuitOrderProductInfo.MarketPrice != oldSuitOrderProductInfo.MarketPrice ||
                    //                newSuitOrderProductInfo.CostPrice != oldSuitOrderProductInfo.CostPrice ||
                    //                newSuitOrderProductInfo.Type != oldSuitOrderProductInfo.Type ||
                    //                newSuitOrderProductInfo.RealCount != oldSuitOrderProductInfo.RealCount ||
                    //                newSuitOrderProductInfo.BuyCount != oldSuitOrderProductInfo.BuyCount ||
                    //                newSuitOrderProductInfo.ExtCode1 != oldSuitOrderProductInfo.ExtCode1 ||
                    //                newSuitOrderProductInfo.ExtCode2 != oldSuitOrderProductInfo.ExtCode2 ||
                    //                newSuitOrderProductInfo.ExtCode3 != oldSuitOrderProductInfo.ExtCode3 ||
                    //                newSuitOrderProductInfo.ExtCode4 != oldSuitOrderProductInfo.ExtCode4 ||
                    //                newSuitOrderProductInfo.ExtCode5 != oldSuitOrderProductInfo.ExtCode5)
                    //            {
                    //                return AjaxResult("changegift", "商品" + orderProductInfo.Name + "的赠品已经改变，请删除此商品后重新添加");
                    //            }
                    //        }

                    //        //验证赠品库存
                    //        foreach (OrderProductInfo gift in cartProductInfo.GiftList)
                    //        {
                    //            ProductStockInfo stockInfo = Products.GetProductStock(gift.Pid, productStockList);
                    //            if (stockInfo.Number < gift.RealCount)
                    //            {
                    //                if (stockInfo.Number == 0)
                    //                {
                    //                    Carts.DeleteOrderProductList(new List<OrderProductInfo>() { gift });
                    //                    if (cartProductInfo.GiftList.Count == 1)
                    //                    {
                    //                        orderProductInfo.ExtCode3 = -1 * orderProductInfo.ExtCode3;
                    //                        Carts.UpdateOrderProductGift(new List<OrderProductInfo>() { orderProductInfo });
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    gift.RealCount = stockInfo.Number;
                    //                    Carts.UpdateOrderProductCount(new List<OrderProductInfo>() { gift });
                    //                }
                    //                return AjaxResult("outstock", "商品" + orderProductInfo.Name + "的赠品" + gift.Name + "库存不足,请返回购物车重新确认");
                    //            }
                    //            else
                    //            {
                    //                stockInfo.Number -= gift.RealCount;
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                }
            }
            List<OrderProductInfo> newOrderProductList = Carts.GetCartProductList(uid, sid);
            return newOrderProductList;
        }
        /// <summary>
        /// 从单品促销中更新购物车
        /// </summary>
        /// <param name="singlePromotionInfo"></param>
        /// <param name="orderProductInfo"></param>
        public static void UpdateCartProductFromSinglePromotion(SinglePromotionInfo singlePromotionInfo, OrderProductInfo orderProductInfo, decimal NewDiscountPrice)
        {
            string sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]={1},[producthaimi]={2},[productpv]={3},[producthbcut]={4},[extcode1]={5} WHERE [recordid]={6};", RDBSHelper.RDBSTablePre, NewDiscountPrice, singlePromotionInfo.PromoHaiMi, singlePromotionInfo.PromoPV, singlePromotionInfo.PromoHongBaoCut, singlePromotionInfo.PmId, orderProductInfo.RecordId);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, sql);
        }
        /// <summary>
        /// 购物车中单品促销改为正常商品
        /// </summary>
        /// <param name="singlePromotionInfo"></param>
        /// <param name="orderProductInfo"></param>
        public static void UpdateCartProductFromSingleToProduct(OrderProductInfo orderProductInfo, PartProductInfo partProductInfo)
        {
            string sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]={1},[producthaimi]={2},[productpv]={3},[producthbcut]={4},[extcode1]={5} WHERE [recordid]={6};", RDBSHelper.RDBSTablePre, partProductInfo.ShopPrice, partProductInfo.HaiMi, partProductInfo.PV, partProductInfo.HongBaoCut, 0, orderProductInfo.RecordId);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, sql);
        }
        /// <summary>
        /// 购物车中正常商品信息变更同步
        /// </summary>
        /// <param name="singlePromotionInfo"></param>
        /// <param name="orderProductInfo"></param>
        public static void UpdateCartProductFromProductModify(OrderProductInfo orderProductInfo, PartProductInfo partProductInfo)
        {
            //orderProductInfo.DiscountPrice != partProductInfo.ShopPrice || orderProductInfo.ShopPrice != partProductInfo.ShopPrice || orderProductInfo.MarketPrice != partProductInfo.MarketPrice || orderProductInfo.StoreSTid != partProductInfo.StoreSTid || partProductInfo.HaiMi != orderProductInfo.ProductHaiMi || partProductInfo.HongBaoCut != orderProductInfo.ProductHBCut || partProductInfo.PV != orderProductInfo.ProductPV

            string sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]={1},[shopprice]={2},[marketprice]={3},[storestid]={4},[producthaimi]={5},[productpv]={6},[producthbcut]={7} WHERE [recordid]={8};",
                RDBSHelper.RDBSTablePre,
                partProductInfo.ShopPrice,
                partProductInfo.ShopPrice,
                partProductInfo.MarketPrice,
                partProductInfo.StoreSTid,
                partProductInfo.HaiMi,
                partProductInfo.PV,
                partProductInfo.HongBaoCut,
                orderProductInfo.RecordId);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, sql);
        }
        /// <summary>
        /// 更新部分产品海米pv-活动期间
        /// </summary>
        /// <param name="singlePromotionInfo"></param>
        /// <param name="orderProductInfo"></param>
        public static void UpdateCartProductHPFor11(decimal newHaimi, decimal newPV, OrderProductInfo orderProductInfo)
        {
            string sql = string.Format("UPDATE [{0}orderproducts] SET [producthaimi]={1},[productpv]={2} WHERE [recordid]={3};", RDBSHelper.RDBSTablePre, newHaimi, newPV, orderProductInfo.RecordId);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, sql);
        }

        /// <summary>
        /// 更新价格For特价产品不参与汇购卡支付
        /// </summary>
        /// <param name="orderProductList"></param>
        /// <param name="uid"></param>
        /// <param name="sid"></param>
        /// <param name="selectCash"></param>
        /// <returns></returns>
        public static List<OrderProductInfo> UpdateCartForSpecialPrice(List<OrderProductInfo> orderProductList, int uid, string sid, bool selectCash)
        {
            foreach (var orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0)
                {
                    //验证商品信息
                    PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                    if (partProductInfo != null)
                    {
                        //当购物车中产品原本属于单品促促销类型
                        if (orderProductInfo.ExtCode1 > 0)
                        {
                            //原本为单品促销特价类型 选中汇购卡支付时需要更新为普通商品
                            SinglePromotionInfo singlePromotionInfo2 = Promotions.GetSinglePromotionByPidAndTime(orderProductInfo.Pid, DateTime.Now);
                            //当单品促销活动存在并且为特价类型
                            if (singlePromotionInfo2 != null)
                                if (singlePromotionInfo2.DiscountType == 5)
                                    if (selectCash)
                                        //更新为普通产品类型
                                        UpdateCartProductFromSingleToProduct(orderProductInfo, partProductInfo);
                        }
                        else //当购物车中产品不为单品促促销类型--为普通类型
                        {
                            //原本为普通商品类型 不选中汇购卡支付时需要更新为单品特价促销商品

                            //同步单品促销
                            SinglePromotionInfo singlePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(partProductInfo.Pid, DateTime.Now);
                            //当单品促销活动存在时则设置订单商品信息
                            if (singlePromotionInfo != null)
                            {
                                if (singlePromotionInfo.DiscountType == 5)
                                    if (!selectCash)
                                    {
                                        decimal discountprice = Promotions.ComputeDiscountPrice(partProductInfo.ShopPrice, singlePromotionInfo, partProductInfo.MarketPrice, orderProductInfo.BuyCount);
                                        if (singlePromotionInfo.PromoHaiMi != orderProductInfo.ProductHaiMi || singlePromotionInfo.PromoHongBaoCut != orderProductInfo.ProductHBCut || singlePromotionInfo.PromoPV != orderProductInfo.ProductPV || discountprice != orderProductInfo.DiscountPrice)
                                            UpdateCartProductFromSinglePromotion(singlePromotionInfo, orderProductInfo, discountprice);
                                    }
                            }
                        }
                    }
                }
            }
            List<OrderProductInfo> newOrderProductList = Carts.GetCartProductList(uid, sid);
            return newOrderProductList;
        }

        /// <summary>
        /// 更新双11部分产品海米pv
        /// </summary>
        /// <param name="orderProductList"></param>
        /// <param name="uid"></param>
        /// <param name="sid"></param>
        /// <param name="selectCash"></param>
        /// <returns></returns>
        public static List<OrderProductInfo> UpdateCartForDouble11(List<OrderProductInfo> orderProductList, int uid, string sid, bool selectCash)
        {
            foreach (var orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0)
                {
                    //验证商品信息
                    PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                    if (partProductInfo != null)
                    {
                        //更新部分产品海米pv

                        // UpdateCartProductHPFor11(singlePromotionInfo, orderProductInfo, discountprice);

                    }
                }
            }
            List<OrderProductInfo> newOrderProductList = Carts.GetCartProductList(uid, sid);
            return newOrderProductList;
        }

        /// <summary>
        /// 更新产品-和治友德品牌买一送一PV减半政策
        /// 更新产品-和治友德旗舰店专区买一送一PV减半政策 20180808
        /// </summary>
        /// <param name="orderProductList"></param>
        /// <param name="selectCash"></param>
        /// <returns></returns>
        public static void UpdateCartForHZYDSend(List<OrderProductInfo> orderProductList)
        {
            foreach (var orderProductInfo in orderProductList)
            {
                //产品为和治友德旗舰店专区 并且pv大于0 才执行政策
                if (orderProductInfo.Type == 0 && Channel.GetProductChannels(orderProductInfo.Pid).Distinct<int>().ToList().Exists(x => x == 1) && orderProductInfo.PV > 0)
                {
                    //验证商品信息
                    PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                    if (partProductInfo != null)
                        //更新买一送一pv减半,PV必须大于0
                        if (partProductInfo.PV > 0)
                            UpdateCartForHZYDSendDetail(orderProductInfo);
                }
            }
        }

        /// <summary>
        /// 更新和治友德买一送一PV减半政策
        /// </summary>
        /// <param name="singlePromotionInfo"></param>
        /// <param name="orderProductInfo"></param>
        public static void UpdateCartForHZYDSendDetail(OrderProductInfo orderProductInfo)
        {
            string sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]=[discountprice]/2,[realcount]=[realcount]*2,[buycount]=[buycount]*2,[productpv]=[productpv]/4 WHERE [recordid]={1};", RDBSHelper.RDBSTablePre, orderProductInfo.RecordId);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, sql);
        }
        /// <summary>
        /// 更新代理产品折扣
        /// </summary>
        /// <param name="orderProductList"></param>
        /// <param name="uid"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static List<OrderProductInfo> UpdateCartForAgent(List<OrderProductInfo> orderProductList, int uid, string sid)
        {
            PartUserInfo user = Users.GetPartUserById(uid);
            List<OrderProductInfo> agentOPlist = orderProductList.FindAll(x => x.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"));
            StoreInfo agentStore = Stores.GetStoreById(WebHelper.GetConfigSettingsInt("AgentStoreId"));
            if (agentStore.Amount1 > 0)
            {
                //List<OrderProductInfo> agentSuitOPlist = orderProductList.FindAll(x => x.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId"));
                decimal agentAmount = agentOPlist.Sum(x => x.BuyCount * x.ShopPrice);// +agentSuitOPlist.Sum(x => x.BuyCount * x.ShopPrice);
                List<string> agentPids_298 = WebHelper.GetConfigSettings("298_AgentPid").Split(',').ToList();
                foreach (var item in agentOPlist)
                {
                    if (agentAmount < agentStore.Amount1 && item.ShopPrice != item.DiscountPrice && !agentPids_298.Exists(x => TypeHelper.StringToInt(x) == item.Pid) && user.Ds2AgentRank <= 0)
                        UpdateCartForAgentDetail(item, 0, 1);
                    if (agentAmount < agentStore.Amount1 && agentPids_298.Exists(x => TypeHelper.StringToInt(x) == item.Pid) && user.Ds2AgentRank <= 0)
                        if (item.BuyCount % 2 == 0)//双数，8折298
                            UpdateCartForAgentDetail(item, 149, 2);
                        else//单数
                            UpdateCartForAgentDetail(item, 0, 1);
                    if (agentStore.Amount1 > 0 && agentAmount >= agentStore.Amount1 && agentAmount < agentStore.Amount2 && item.ShopPrice * (agentStore.Discount1 / 10) != item.DiscountPrice)
                        UpdateCartForAgentDetail(item, agentStore.Discount1);
                    else if (agentStore.Amount2 > 0 && agentAmount >= agentStore.Amount2 && agentAmount < agentStore.Amount3 && item.ShopPrice * (agentStore.Discount2 / 10) != item.DiscountPrice)
                        UpdateCartForAgentDetail(item, agentStore.Discount2);
                    else if (agentStore.Amount3 > 0 && agentAmount >= agentStore.Amount3 && agentAmount < agentStore.Amount4 && item.ShopPrice * (agentStore.Discount3 / 10) != item.DiscountPrice)
                        UpdateCartForAgentDetail(item, agentStore.Discount3);
                    else if (agentStore.Amount4 > 0 && agentAmount >= agentStore.Amount4 && agentAmount < agentStore.Amount5 && item.ShopPrice * (agentStore.Discount4 / 10) != item.DiscountPrice)
                        UpdateCartForAgentDetail(item, agentStore.Discount4);
                    else if (agentStore.Amount5 > 0 && agentAmount >= agentStore.Amount5 && item.ShopPrice * (agentStore.Discount5 / 10) != item.DiscountPrice)
                        UpdateCartForAgentDetail(item, agentStore.Discount5);

                }
            }
            List<OrderProductInfo> newOrderProductList = Carts.GetCartProductList(uid, sid);
            return newOrderProductList;
        }
        /// <summary>
        /// 更新代理产品折扣价
        /// </summary>
        /// <param name="singlePromotionInfo"></param>
        /// <param name="orderProductInfo"></param>
        public static void UpdateCartForAgentDetail(OrderProductInfo orderProductInfo, decimal discount, int type = 0)
        {
            string sql = string.Empty;
            if (type == 0)
            {
                sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]=[shopprice]*{1} WHERE [recordid]={2};", RDBSHelper.RDBSTablePre, discount / 10, orderProductInfo.RecordId);
            }
            else if (type == 1)
            {
                sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]=[shopprice] WHERE [recordid]={1};", RDBSHelper.RDBSTablePre, orderProductInfo.RecordId);
            }
            else if (type == 2)
            {
                sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]={1} WHERE [recordid]={2};", RDBSHelper.RDBSTablePre, discount, orderProductInfo.RecordId);
            }
            RDBSHelper.ExecuteNonQuery(CommandType.Text, sql);
        }

        /// <summary>
        /// 更新引流政策产品价格
        /// </summary>
        /// <param name="orderProductList"></param>
        /// <param name="uid"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static List<OrderProductInfo> UpdateCartForMCAgent(List<OrderProductInfo> orderProductList, int uid, string sid)
        {
            PartUserInfo user = Users.GetPartUserById(uid);
            List<OrderProductInfo> agentOPlist = orderProductList.FindAll(x => x.StoreId == WebHelper.GetConfigSettingsInt("MCAgentStoreId"));

            foreach (var item in agentOPlist)
            {
                //if (user.UserSource == (int)UserSourceType.散客会员)
                //{
                //    if (item.ShopPrice == item.DiscountPrice)
                //        UpdateCartForMCAgentDetail(item, 598, 2);
                //}
                if (item.BuyCount < 4)
                {
                    if (item.DiscountPrice <= item.ShopPrice)
                        UpdateCartForMCAgentDetail(item, 598, 2);
                }
                if (item.BuyCount >= 4)
                {
                    if (item.ShopPrice != item.DiscountPrice)
                        UpdateCartForMCAgentDetail(item, 0, 1);
                }
            }

            List<OrderProductInfo> newOrderProductList = Carts.GetCartProductList(uid, sid);
            return newOrderProductList;
        }

        /// <summary>
        /// 更新代理产品折扣价
        /// </summary>
        /// <param name="singlePromotionInfo"></param>
        /// <param name="orderProductInfo"></param>
        public static void UpdateCartForMCAgentDetail(OrderProductInfo orderProductInfo, decimal discount, int type = 0)
        {
            string sql = string.Empty;
            if (type == 0)
            {
                sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]=[shopprice]*{1} WHERE [recordid]={2};", RDBSHelper.RDBSTablePre, discount / 10, orderProductInfo.RecordId);
            }
            else if (type == 1)
            {
                sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]=[shopprice] WHERE [recordid]={1};", RDBSHelper.RDBSTablePre, orderProductInfo.RecordId);
            }
            else if (type == 2)
            {
                sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]={1} WHERE [recordid]={2};", RDBSHelper.RDBSTablePre, discount, orderProductInfo.RecordId);
            }
            RDBSHelper.ExecuteNonQuery(CommandType.Text, sql);
        }

        /// <summary>
        /// 更新引流政策产品价格
        /// </summary>
        /// <param name="orderProductList"></param>
        /// <param name="uid"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static List<OrderProductInfo> UpdateCartForMCAgentBuyAgain(List<OrderProductInfo> orderProductList, int uid, string sid)
        {
            PartUserInfo user = Users.GetPartUserById(uid);
            List<OrderProductInfo> agentOPlist = orderProductList.FindAll(x => x.StoreId == WebHelper.GetConfigSettingsInt("MCAgentStoreId"));
            //foreach (var item in agentOPlist)
            //{
            //    if (user.AgentType == (int)AgentTypeEnum.VIP会员)
            //    {
            //        if (item.DiscountPrice != 299)
            //            UpdateCartForMCAgentBuyAgainDetail(item, 299, 2);
            //    }
            //    if (user.AgentType == (int)AgentTypeEnum.推广主管)
            //    {
            //        if (item.DiscountPrice != 284)
            //            UpdateCartForMCAgentBuyAgainDetail(item, 284, 2);
            //    }
            //    if (user.AgentType == (int)AgentTypeEnum.推广经理)
            //    {
            //        if (item.DiscountPrice != 267)
            //            UpdateCartForMCAgentBuyAgainDetail(item, 267, 2);
            //    }
            //    if (user.AgentType == (int)AgentTypeEnum.销售总监)
            //    {
            //        if (item.DiscountPrice != 247)
            //            UpdateCartForMCAgentBuyAgainDetail(item, 247, 2);
            //    }
            //    if (user.AgentType == (int)AgentTypeEnum.区域总监)
            //    {
            //        if (item.DiscountPrice != 221)
            //            UpdateCartForMCAgentBuyAgainDetail(item, 221, 2);
            //    }
            //    if (user.AgentType >= (int)AgentTypeEnum.产品代理商)
            //    {
            //        if (item.DiscountPrice != 399)
            //            UpdateCartForMCAgentBuyAgainDetail(item, 399, 2);
            //    }
            //}
            List<OrderProductInfo> newOrderProductList = Carts.GetCartProductList(uid, sid);
            return newOrderProductList;
        }

        /// <summary>
        /// 更新重消直接打折扣
        /// vip重消的时候价格是：399-100=299元，推广主管重消价格是：399-115=284，推广经理重消价格是：399-132=267，销售总监重消价格是：399-152=247，区域总监重消价格是：399-178=221
        /// </summary>
        /// <param name="singlePromotionInfo"></param>
        /// <param name="orderProductInfo"></param>
        public static void UpdateCartForMCAgentBuyAgainDetail(OrderProductInfo orderProductInfo, decimal discount, int type = 0)
        {
            string sql = string.Empty;
            if (type == 1)
            {
                sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]=[shopprice] WHERE [recordid]={1};", RDBSHelper.RDBSTablePre, orderProductInfo.RecordId);
            }
            else if (type == 2)
            {
                sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]={1} WHERE [recordid]={2};", RDBSHelper.RDBSTablePre, discount, orderProductInfo.RecordId);
            }
            RDBSHelper.ExecuteNonQuery(CommandType.Text, sql);
        }


        /// <summary>
        /// 更新公司内部员工购买
        /// </summary>
        /// <param name="orderProductList"></param>
        /// <param name="uid"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static List<OrderProductInfo> UpdateCartForCompanyUser(List<OrderProductInfo> orderProductList, int uid, string sid)
        {
            foreach (var orderProductInfo in orderProductList)
            {
                //汇购优选代理产品,内部员工享受VIP折扣价
                if (orderProductInfo.Type == 0 && orderProductInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"))
                {
                    //验证商品信息
                    PartProductInfo partProductInfo = Products.GetPartProductById(orderProductInfo.Pid);
                    if (partProductInfo != null)
                    {
                        //更新内部员工享受VIP折扣价
                        UpdateCartForCompanyUser_Agent(orderProductInfo);
                    }
                }
                //和治友德旗舰店专区产品（注：每月每种产品限购一件），享受专属折扣 4 折
                List<int> channels = Channel.GetProductChannels(orderProductInfo.Pid);
                if (orderProductInfo.Type == 0 && channels.Exists(x => x == 1))
                {
                    //判断本月是否购买过此产品
                    int monthbuy = Orders.GetOrderProductCountByPidAndUidForMonth(orderProductInfo.Pid, uid, DateTime.Now);
                    if (monthbuy > 0)
                    {
                        UpdateCartForCompanyUser_HZYDChannel(orderProductInfo, 1);
                    }
                    else
                    {
                        UpdateCartForCompanyUser_HZYDChannel(orderProductInfo, 0);
                    }

                }
            }
            List<OrderProductInfo> newOrderProductList = Carts.GetCartProductList(uid, sid);
            return newOrderProductList;
        }
        /// <summary>
        /// 更新公司内部员工购买汇购优选代理产品,内部员工享受VIP折扣价4.7折
        /// </summary>
        /// <param name="singlePromotionInfo"></param>
        /// <param name="orderProductInfo"></param>
        public static void UpdateCartForCompanyUser_Agent(OrderProductInfo orderProductInfo)
        {
            string sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]=[shopprice]*{1} WHERE [recordid]={2};", RDBSHelper.RDBSTablePre, 0.47, orderProductInfo.RecordId);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, sql);
        }
        /// <summary>
        /// 更新公司内部员工购买和治友德旗舰店专区产品（注：每月每种产品限购一件），享受专属折扣 4 折
        /// </summary>
        /// <param name="orderProductInfo"></param>
        /// <param name="type">type 0表示没买过，享受4折， 1 表示之前买过，不享受4折，</param>
        public static void UpdateCartForCompanyUser_HZYDChannel(OrderProductInfo orderProductInfo, int type = 0)
        {
            string sql = string.Empty;
            if (type == 0)
                sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]=[shopprice]*{1} WHERE [recordid]={2};", RDBSHelper.RDBSTablePre, 0.4, orderProductInfo.RecordId);
            else if (type == 1)
                sql = string.Format("UPDATE [{0}orderproducts] SET [discountprice]=[shopprice] WHERE [recordid]={1};", RDBSHelper.RDBSTablePre, orderProductInfo.RecordId);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, sql);
        }
        #endregion



        /// <summary>
        /// 判断购物车项是否被选中
        /// </summary>
        /// <param name="type">类型(0代表单一项,1代表组合项)</param>
        /// <param name="id">id</param>
        /// <param name="selectedCartItemKeyList">选中的购物车项键列表</param>
        /// <returns></returns>
        public static bool IsSelectCartItem(int type, int id, string[] selectedCartItemKeyList)
        {
            if (selectedCartItemKeyList == null || selectedCartItemKeyList.Length == 0)
                return true;
            string cartItemKey = string.Format("{0}_{1}", type, id);
            foreach (string selectedCartItemKey in selectedCartItemKeyList)
            {
                if (cartItemKey == selectedCartItemKey)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 整理店铺订单商品列表
        /// </summary>
        /// <param name="selectedCartItemKeyList">选中的购物车项键列表</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="selectedOrderProductList">选中的订单商品列表</param>
        /// <param name="remainedOrderProductList">剩余的订单商品列表</param>
        /// <returns></returns>
        public static List<CartItemInfo> TidyStoreOrderProductList(string[] selectedCartItemKeyList, List<OrderProductInfo> orderProductList, out List<OrderProductInfo> selectedOrderProductList, out List<OrderProductInfo> remainedOrderProductList)
        {
            //声明一个购物车项列表
            List<CartItemInfo> cartItemList = new List<CartItemInfo>();
            //初始化选中的订单商品列表
            selectedOrderProductList = new List<OrderProductInfo>();
            //初始化剩余的订单商品列表
            remainedOrderProductList = new List<OrderProductInfo>();

            //订单商品商品数量
            int count = orderProductList.Count;
            for (int i = 0; i < count; i++)
            {
                OrderProductInfo orderProductInfo = orderProductList[i];
                if (orderProductInfo == null)//如果此订单商品已经被置为null则跳过
                    continue;

                if (orderProductInfo.Type == 0)//当商品是普通订单商品时
                {
                    if (orderProductInfo.ExtCode4 > 0)//满赠订单商品处理
                    {
                        #region 满赠订单商品处理

                        FullSendPromotionInfo fullSendPromotionInfo = Promotions.GetFullSendPromotionByPmIdAndTime(orderProductInfo.ExtCode4, DateTime.Now);
                        if (fullSendPromotionInfo != null)
                        {
                            CartFullSendInfo cartFullSendInfo = new CartFullSendInfo();

                            cartFullSendInfo.FullSendPromotionInfo = fullSendPromotionInfo;

                            List<CartProductInfo> fullSendMainCartProductList = new List<CartProductInfo>();
                            CartProductInfo cartProductInfo1 = new CartProductInfo();
                            cartProductInfo1.Selected = IsSelectCartItem(0, orderProductInfo.Pid, selectedCartItemKeyList);
                            cartProductInfo1.OrderProductInfo = orderProductInfo;
                            orderProductList[i] = null;
                            List<OrderProductInfo> giftList1 = new List<OrderProductInfo>();
                            //获取商品的赠品
                            for (int j = 0; j < count; j++)
                            {
                                OrderProductInfo item = orderProductList[j];
                                if (item != null && item.Type == 1 && item.ExtCode1 == orderProductInfo.ExtCode3)
                                {
                                    giftList1.Add(item);
                                    orderProductList[j] = null;
                                }
                            }
                            cartProductInfo1.GiftList = giftList1;
                            fullSendMainCartProductList.Add(cartProductInfo1);
                            //获取同一满赠商品
                            for (int j = 0; j < count; j++)
                            {
                                OrderProductInfo item = orderProductList[j];
                                if (item != null && item.Type == 0 && item.ExtCode4 == orderProductInfo.ExtCode4)
                                {
                                    CartProductInfo cartProductInfo2 = new CartProductInfo();
                                    cartProductInfo2.Selected = IsSelectCartItem(0, item.Pid, selectedCartItemKeyList);
                                    cartProductInfo2.OrderProductInfo = item;
                                    orderProductList[j] = null;
                                    List<OrderProductInfo> giftList2 = new List<OrderProductInfo>();
                                    for (int k = 0; k < count; k++)
                                    {
                                        OrderProductInfo item2 = orderProductList[k];
                                        if (item2 != null && item2.Type == 1 && item2.ExtCode1 == item.ExtCode3)
                                        {
                                            giftList2.Add(item2);
                                            orderProductList[k] = null;
                                        }
                                    }
                                    cartProductInfo2.GiftList = giftList2;
                                    fullSendMainCartProductList.Add(cartProductInfo2);
                                }
                            }
                            cartFullSendInfo.FullSendMainCartProductList = fullSendMainCartProductList;

                            decimal selectedFullSendMainCartProductAmount = 0M;
                            foreach (CartProductInfo fullSendMainCartProductInfo in fullSendMainCartProductList)
                            {
                                if (fullSendMainCartProductInfo.Selected)
                                    selectedFullSendMainCartProductAmount += fullSendMainCartProductInfo.OrderProductInfo.DiscountPrice * fullSendMainCartProductInfo.OrderProductInfo.BuyCount;
                            }
                            cartFullSendInfo.IsEnough = selectedFullSendMainCartProductAmount >= fullSendPromotionInfo.LimitMoney;

                            //获取商品的满赠商品
                            for (int j = 0; j < count; j++)
                            {
                                OrderProductInfo item = orderProductList[j];
                                if (item != null && item.Type == 4 && item.ExtCode1 == orderProductInfo.ExtCode4)
                                {
                                    cartFullSendInfo.FullSendMinorOrderProductInfo = item;
                                    orderProductList[j] = null;
                                    break;
                                }
                            }

                            CartItemInfo cartItemInfo = new CartItemInfo();
                            cartItemInfo.Type = 2;
                            cartItemInfo.Item = cartFullSendInfo;
                            cartItemList.Add(cartItemInfo);
                        }
                        else//当满赠促销活动不存在时，按照没有满赠促销商品处理
                        {
                            List<OrderProductInfo> updateFullSendOrderProductList = new List<OrderProductInfo>();

                            orderProductInfo.ExtCode4 = 0;
                            updateFullSendOrderProductList.Add(orderProductInfo);

                            CartProductInfo cartProductInfo1 = new CartProductInfo();
                            cartProductInfo1.Selected = IsSelectCartItem(0, orderProductInfo.Pid, selectedCartItemKeyList);
                            cartProductInfo1.OrderProductInfo = orderProductInfo;
                            orderProductList[i] = null;
                            List<OrderProductInfo> giftList1 = new List<OrderProductInfo>();
                            //获取商品的赠品
                            for (int j = 0; j < count; j++)
                            {
                                OrderProductInfo item = orderProductList[j];
                                if (item != null && item.Type == 1 && item.ExtCode1 == orderProductInfo.ExtCode3)
                                {
                                    giftList1.Add(item);
                                    orderProductList[j] = null;
                                }
                            }
                            cartProductInfo1.GiftList = giftList1;

                            CartItemInfo cartItemInfo1 = new CartItemInfo();
                            cartItemInfo1.Type = 0;
                            cartItemInfo1.Item = cartProductInfo1;
                            cartItemList.Add(cartItemInfo1);

                            //获取同一满赠商品
                            for (int j = 0; j < count; j++)
                            {
                                OrderProductInfo item = orderProductList[j];
                                if (item != null && item.Type == 0 && item.ExtCode4 == orderProductInfo.ExtCode4)
                                {
                                    item.ExtCode4 = 0;
                                    updateFullSendOrderProductList.Add(item);

                                    CartProductInfo cartProductInfo2 = new CartProductInfo();
                                    cartProductInfo2.Selected = IsSelectCartItem(0, item.Pid, selectedCartItemKeyList);
                                    cartProductInfo2.OrderProductInfo = item;
                                    orderProductList[j] = null;
                                    List<OrderProductInfo> giftList2 = new List<OrderProductInfo>();
                                    for (int k = 0; k < count; k++)
                                    {
                                        OrderProductInfo item2 = orderProductList[k];
                                        if (item2 != null && item2.Type == 1 && item2.ExtCode1 == item.ExtCode3)
                                        {
                                            giftList2.Add(item2);
                                            orderProductList[k] = null;
                                        }
                                    }
                                    cartProductInfo2.GiftList = giftList2;

                                    CartItemInfo cartItemInfo2 = new CartItemInfo();
                                    cartItemInfo2.Type = 0;
                                    cartItemInfo2.Item = cartProductInfo2;
                                    cartItemList.Add(cartItemInfo2);
                                }
                            }

                            //更新商品的满赠促销活动
                            UpdateOrderProductFullSend(updateFullSendOrderProductList);

                            //获取商品的满赠商品
                            for (int j = 0; j < count; j++)
                            {
                                OrderProductInfo item = orderProductList[j];
                                //当满赠赠品存在时删除满赠赠品
                                if (item != null && item.Type == 4 && item.ExtCode1 == orderProductInfo.ExtCode4)
                                {
                                    DeleteOrderProductList(new List<OrderProductInfo>() { item });
                                    break;
                                }
                            }
                        }

                        #endregion
                    }
                    else if (orderProductInfo.ExtCode5 > 0)//满减订单商品处理
                    {
                        #region 满减订单商品处理

                        FullCutPromotionInfo fullCutPromotionInfo = Promotions.GetFullCutPromotionByStoreIdAndPmIdAndTime(orderProductInfo.StoreId, orderProductInfo.ExtCode5, DateTime.Now);
                        if (fullCutPromotionInfo != null)
                        {
                            CartFullCutInfo cartFullCutInfo = new CartFullCutInfo();

                            cartFullCutInfo.FullCutPromotionInfo = fullCutPromotionInfo;

                            List<CartProductInfo> fullCutCartProductList = new List<CartProductInfo>();
                            CartProductInfo cartProductInfo1 = new CartProductInfo();
                            cartProductInfo1.Selected = IsSelectCartItem(0, orderProductInfo.Pid, selectedCartItemKeyList);
                            cartProductInfo1.OrderProductInfo = orderProductInfo;
                            orderProductList[i] = null;
                            List<OrderProductInfo> giftList1 = new List<OrderProductInfo>();
                            //获取商品的赠品
                            for (int j = 0; j < count; j++)
                            {
                                OrderProductInfo item = orderProductList[j];
                                if (item != null && item.Type == 1 && item.ExtCode1 == orderProductInfo.ExtCode3)
                                {
                                    giftList1.Add(item);
                                    orderProductList[j] = null;
                                }
                            }
                            cartProductInfo1.GiftList = giftList1;
                            fullCutCartProductList.Add(cartProductInfo1);
                            //获取同一满减商品
                            for (int j = 0; j < count; j++)
                            {
                                OrderProductInfo item = orderProductList[j];
                                if (item != null && item.Type == 0 && item.ExtCode5 == orderProductInfo.ExtCode5)
                                {
                                    CartProductInfo cartProductInfo2 = new CartProductInfo();
                                    cartProductInfo2.Selected = IsSelectCartItem(0, item.Pid, selectedCartItemKeyList);
                                    cartProductInfo2.OrderProductInfo = item;
                                    orderProductList[j] = null;
                                    List<OrderProductInfo> giftList2 = new List<OrderProductInfo>();
                                    for (int k = 0; k < count; k++)
                                    {
                                        OrderProductInfo item2 = orderProductList[k];
                                        if (item2 != null && item2.Type == 1 && item2.ExtCode1 == item.ExtCode3)
                                        {
                                            giftList2.Add(item2);
                                            orderProductList[k] = null;
                                        }
                                    }
                                    cartProductInfo2.GiftList = giftList2;
                                    fullCutCartProductList.Add(cartProductInfo2);
                                }
                            }
                            cartFullCutInfo.FullCutCartProductList = fullCutCartProductList;

                            decimal selectedFullCutCartProductAmount = 0M;
                            foreach (CartProductInfo fullCutCartProductInfo in fullCutCartProductList)
                            {
                                if (fullCutCartProductInfo.Selected)
                                    selectedFullCutCartProductAmount += fullCutCartProductInfo.OrderProductInfo.DiscountPrice * fullCutCartProductInfo.OrderProductInfo.BuyCount;
                            }
                            if (fullCutPromotionInfo.LimitMoney3 > 0 && selectedFullCutCartProductAmount >= fullCutPromotionInfo.LimitMoney3)
                            {
                                cartFullCutInfo.IsEnough = true;
                                cartFullCutInfo.LimitMoney = fullCutPromotionInfo.LimitMoney3;
                                cartFullCutInfo.CutMoney = fullCutPromotionInfo.CutMoney3;
                            }
                            else if (fullCutPromotionInfo.LimitMoney2 > 0 && selectedFullCutCartProductAmount >= fullCutPromotionInfo.LimitMoney2)
                            {
                                cartFullCutInfo.IsEnough = true;
                                cartFullCutInfo.LimitMoney = fullCutPromotionInfo.LimitMoney2;
                                cartFullCutInfo.CutMoney = fullCutPromotionInfo.CutMoney2;
                            }
                            else if (selectedFullCutCartProductAmount >= fullCutPromotionInfo.LimitMoney1)
                            {
                                cartFullCutInfo.IsEnough = true;
                                cartFullCutInfo.LimitMoney = fullCutPromotionInfo.LimitMoney1;
                                cartFullCutInfo.CutMoney = fullCutPromotionInfo.CutMoney1;
                            }
                            else
                            {
                                cartFullCutInfo.IsEnough = false;
                                cartFullCutInfo.LimitMoney = fullCutPromotionInfo.LimitMoney1;
                                cartFullCutInfo.CutMoney = fullCutPromotionInfo.CutMoney1;
                            }

                            CartItemInfo cartItemInfo = new CartItemInfo();
                            cartItemInfo.Type = 3;
                            cartItemInfo.Item = cartFullCutInfo;
                            cartItemList.Add(cartItemInfo);
                        }
                        else//当满减促销活动不存在时，按照没有满减促销商品处理
                        {
                            List<OrderProductInfo> updateFullCutOrderProductList = new List<OrderProductInfo>();

                            orderProductInfo.ExtCode5 = 0;
                            updateFullCutOrderProductList.Add(orderProductInfo);

                            CartProductInfo cartProductInfo1 = new CartProductInfo();
                            cartProductInfo1.Selected = IsSelectCartItem(0, orderProductInfo.Pid, selectedCartItemKeyList);
                            cartProductInfo1.OrderProductInfo = orderProductInfo;
                            orderProductList[i] = null;
                            List<OrderProductInfo> giftList1 = new List<OrderProductInfo>();
                            //获取商品的赠品
                            for (int j = 0; j < count; j++)
                            {
                                OrderProductInfo item = orderProductList[j];
                                if (item != null && item.Type == 1 && item.ExtCode1 == orderProductInfo.ExtCode3)
                                {
                                    giftList1.Add(item);
                                    orderProductList[j] = null;
                                }
                            }
                            cartProductInfo1.GiftList = giftList1;

                            CartItemInfo cartItemInfo1 = new CartItemInfo();
                            cartItemInfo1.Type = 0;
                            cartItemInfo1.Item = cartProductInfo1;
                            cartItemList.Add(cartItemInfo1);

                            //获取同一满减商品
                            for (int j = 0; j < count; j++)
                            {
                                OrderProductInfo item = orderProductList[j];
                                if (item != null && item.Type == 0 && item.ExtCode5 == orderProductInfo.ExtCode5)
                                {
                                    item.ExtCode5 = 0;
                                    updateFullCutOrderProductList.Add(item);

                                    CartProductInfo cartProductInfo2 = new CartProductInfo();
                                    cartProductInfo2.Selected = IsSelectCartItem(0, item.Pid, selectedCartItemKeyList);
                                    cartProductInfo2.OrderProductInfo = item;
                                    orderProductList[j] = null;
                                    List<OrderProductInfo> giftList2 = new List<OrderProductInfo>();
                                    for (int k = 0; k < count; k++)
                                    {
                                        OrderProductInfo item2 = orderProductList[k];
                                        if (item2 != null && item2.Type == 1 && item2.ExtCode1 == item.ExtCode3)
                                        {
                                            giftList2.Add(item2);
                                            orderProductList[k] = null;
                                        }
                                    }
                                    cartProductInfo2.GiftList = giftList2;

                                    CartItemInfo cartItemInfo2 = new CartItemInfo();
                                    cartItemInfo2.Type = 0;
                                    cartItemInfo2.Item = cartProductInfo2;
                                    cartItemList.Add(cartItemInfo2);
                                }
                            }

                            //更新商品的满减促销活动
                            UpdateOrderProductFullCut(updateFullCutOrderProductList);
                        }

                        #endregion
                    }
                    else//非满赠和满减订单商品处理
                    {
                        #region 非满赠和满减订单商品处理

                        CartProductInfo cartProductInfo = new CartProductInfo();
                        cartProductInfo.Selected = IsSelectCartItem(0, orderProductInfo.Pid, selectedCartItemKeyList);
                        cartProductInfo.OrderProductInfo = orderProductInfo;
                        orderProductList[i] = null;
                        List<OrderProductInfo> giftList = new List<OrderProductInfo>();
                        //获取商品的赠品
                        for (int j = 0; j < count; j++)
                        {
                            OrderProductInfo item = orderProductList[j];
                            if (item != null && item.Type == 1 && item.ExtCode1 == orderProductInfo.ExtCode3)
                            {
                                giftList.Add(item);
                                orderProductList[j] = null;
                            }
                        }
                        cartProductInfo.GiftList = giftList;

                        CartItemInfo cartItemInfo = new CartItemInfo();
                        cartItemInfo.Type = 0;
                        cartItemInfo.Item = cartProductInfo;
                        cartItemList.Add(cartItemInfo);

                        #endregion
                    }
                }
                else if (orderProductInfo.Type == 3)//当商品是套装商品时
                {
                    #region 套装商品处理

                    CartSuitInfo cartSuitInfo = new CartSuitInfo();
                    cartSuitInfo.Checked = IsSelectCartItem(1, orderProductInfo.ExtCode1, selectedCartItemKeyList);
                    cartSuitInfo.PmId = orderProductInfo.ExtCode1;
                    cartSuitInfo.BuyCount = orderProductInfo.RealCount / orderProductInfo.ExtCode2;

                    decimal suitAmount = 0M;
                    List<CartProductInfo> cartProductList = new List<CartProductInfo>();
                    for (int j = 0; j < count; j++)
                    {
                        OrderProductInfo item = orderProductList[j];
                        //获取同一套装商品
                        if (item != null && item.Type == 3 && item.ExtCode1 == orderProductInfo.ExtCode1)
                        {
                            suitAmount += item.DiscountPrice * item.RealCount;

                            CartProductInfo cartProductInfo = new CartProductInfo();
                            cartProductInfo.Selected = cartSuitInfo.Checked;
                            cartProductInfo.OrderProductInfo = item;
                            orderProductList[j] = null;
                            List<OrderProductInfo> giftList = new List<OrderProductInfo>();
                            //获取商品的赠品
                            for (int k = 0; k < count; k++)
                            {
                                OrderProductInfo item2 = orderProductList[k];
                                if (item2 != null && item2.Type == 2 && item2.ExtCode1 == item.ExtCode2)
                                {
                                    giftList.Add(item2);
                                    orderProductList[k] = null;
                                }
                            }
                            cartProductInfo.GiftList = giftList;

                            cartProductList.Add(cartProductInfo);
                        }
                    }
                    cartSuitInfo.SuitPrice = suitAmount / cartSuitInfo.BuyCount;
                    cartSuitInfo.SuitAmount = suitAmount;
                    cartSuitInfo.CartProductList = cartProductList;

                    CartItemInfo cartItemInfo = new CartItemInfo();
                    cartItemInfo.Type = 1;
                    cartItemInfo.Item = cartSuitInfo;
                    cartItemList.Add(cartItemInfo);

                    #endregion
                }
            }
            cartItemList.Sort();

            foreach (CartItemInfo cartItemInfo in cartItemList)
            {
                if (cartItemInfo.Type == 0)
                {
                    CartProductInfo cartProductInfo = (CartProductInfo)cartItemInfo.Item;
                    if (cartProductInfo.Selected)
                    {
                        selectedOrderProductList.Add(cartProductInfo.OrderProductInfo);
                        selectedOrderProductList.AddRange(cartProductInfo.GiftList);
                    }
                    else
                    {
                        remainedOrderProductList.Add(cartProductInfo.OrderProductInfo);
                        remainedOrderProductList.AddRange(cartProductInfo.GiftList);
                    }
                }
                else if (cartItemInfo.Type == 1)
                {
                    CartSuitInfo cartSuitInfo = (CartSuitInfo)cartItemInfo.Item;
                    if (cartSuitInfo.Checked)
                    {
                        foreach (CartProductInfo cartProductInfo in cartSuitInfo.CartProductList)
                        {
                            selectedOrderProductList.Add(cartProductInfo.OrderProductInfo);
                            selectedOrderProductList.AddRange(cartProductInfo.GiftList);
                        }
                    }
                    else
                    {
                        foreach (CartProductInfo cartProductInfo in cartSuitInfo.CartProductList)
                        {
                            remainedOrderProductList.Add(cartProductInfo.OrderProductInfo);
                            remainedOrderProductList.AddRange(cartProductInfo.GiftList);
                        }
                    }
                }
                else if (cartItemInfo.Type == 2)
                {
                    CartFullSendInfo cartFullSendInfo = (CartFullSendInfo)cartItemInfo.Item;
                    if (cartFullSendInfo.FullSendMinorOrderProductInfo != null)
                    {
                        if (cartFullSendInfo.IsEnough)//当金额足够时才添加
                        {
                            selectedOrderProductList.Add(cartFullSendInfo.FullSendMinorOrderProductInfo);
                        }
                        else
                        {
                            remainedOrderProductList.Add(cartFullSendInfo.FullSendMinorOrderProductInfo);
                        }
                    }
                    foreach (CartProductInfo cartProductInfo in cartFullSendInfo.FullSendMainCartProductList)
                    {
                        if (cartProductInfo.Selected)
                        {
                            selectedOrderProductList.Add(cartProductInfo.OrderProductInfo);
                            selectedOrderProductList.AddRange(cartProductInfo.GiftList);
                        }
                        else
                        {
                            remainedOrderProductList.Add(cartProductInfo.OrderProductInfo);
                            remainedOrderProductList.AddRange(cartProductInfo.GiftList);
                        }
                    }
                }
                else if (cartItemInfo.Type == 3)
                {
                    CartFullCutInfo cartFullCutInfo = (CartFullCutInfo)cartItemInfo.Item;
                    foreach (CartProductInfo cartProductInfo in cartFullCutInfo.FullCutCartProductList)
                    {
                        if (cartProductInfo.Selected)
                        {
                            selectedOrderProductList.Add(cartProductInfo.OrderProductInfo);
                            selectedOrderProductList.AddRange(cartProductInfo.GiftList);
                        }
                        else
                        {
                            remainedOrderProductList.Add(cartProductInfo.OrderProductInfo);
                            remainedOrderProductList.AddRange(cartProductInfo.GiftList);
                        }
                    }
                }
            }

            return cartItemList;
        }

        /// <summary>
        /// 获得购物车商品数量
        /// </summary>
        /// <returns></returns>
        public static int GetCartProductCountCookie()
        {
            return TypeHelper.StringToInt(WebHelper.GetCookie("cart", "pcount"));
        }

        /// <summary>
        /// 设置购物车商品数量cookie
        /// </summary>
        /// <param name="count">购物车商品数量</param>
        /// <returns></returns>
        public static void SetCartProductCountCookie(int count)
        {
            if (count < 0) count = 0;
            WebHelper.SetCookie("cart", "pcount", count.ToString(), BMAConfig.MallConfig.SCExpire * 24 * 60);
        }





        /// <summary>
        /// 整理商城订单商品列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<StoreCartInfo> TidyMallOrderProductList(List<OrderProductInfo> orderProductList)
        {
            return TidyMallOrderProductList(null, orderProductList);
        }

        /// <summary>
        /// 整理商城订单商品列表
        /// </summary>
        /// <param name="selectedCartItemKeyList">选中的购物车项键列表</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<StoreCartInfo> TidyMallOrderProductList(string[] selectedCartItemKeyList, List<OrderProductInfo> orderProductList, string[] selectStoreKeyList = null)
        {
            List<StoreCartInfo> list = new List<StoreCartInfo>();
            List<int> showStoreId = new List<int>();
            List<int> storeIdList = new List<int>(orderProductList.Count);
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                storeIdList.Add(orderProductInfo.StoreId);
            }
            List<int> selectStore = new List<int>();
            if (selectStoreKeyList != null && selectStoreKeyList.Count() > 0)
            {
                foreach (var i in selectStoreKeyList)
                {
                    selectStore.Add(TypeHelper.StringToInt(i));
                }
                showStoreId = storeIdList.Distinct<int>().Intersect(selectStore).ToList();
            }
            else
            {
                showStoreId = storeIdList.Distinct<int>().ToList();
            }

            foreach (int storeId in showStoreId)
            {
                List<OrderProductInfo> selectedOrderProductList;
                List<OrderProductInfo> remainedOrderProductList;
                StoreInfo storeInfo = Stores.GetStoreById(storeId);
                List<OrderProductInfo> storeOrderProductList = GetStoreOrderProductList(storeId, orderProductList);
                //if (storeOrderProductList.Any())
                //{
                //    PartUserInfo user = Users.GetPartUserById(storeOrderProductList.First().Uid);
                //    if (storeOrderProductList.Exists(x => x.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId")) && user.AgentType <= 0)
                //        storeOrderProductList = Carts.UpdateCartForAgent(storeOrderProductList, storeOrderProductList.First().Uid, storeOrderProductList.First().Sid);
                //    storeOrderProductList.ForEach(x =>
                //    {
                //        PartProductInfo pro = Products.GetPartProductById(x.Pid);

                //        if (pro != null)
                //        {
                //            x.PV = x.ProductPV;
                //            x.HaiMi = x.ProductHaiMi;
                //            x.TaxRate = pro.TaxRate;
                //            x.SaleType = pro.SaleType;
                //            x.HongBaoCut = x.ProductHBCut;
                //            x.MinBuyCount = pro.MinBuyCount;

                //            x.ProductState = pro.State;
                //        }
                //    });
                //}
                List<CartItemInfo> cartItemList = TidyStoreOrderProductList(selectedCartItemKeyList, storeOrderProductList, out selectedOrderProductList, out remainedOrderProductList);

                StoreCartInfo storeCartInfo = new StoreCartInfo();
                storeCartInfo.StoreInfo = storeInfo;
                storeCartInfo.CartItemList = cartItemList;
                storeCartInfo.SelectedOrderProductList = selectedOrderProductList;
                storeCartInfo.RemainedOrderProductList = remainedOrderProductList;
                list.Add(storeCartInfo);
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetSelectStoreIdList(List<StoreCartInfo> storeCartList)
        {
            List<int> SelectStoreId = new List<int>();
            List<int> showStoreId = new List<int>();
            foreach (StoreCartInfo item in storeCartList)
            {
                SelectStoreId.Add(item.StoreInfo.StoreId);
            }
            if (SelectStoreId != null && SelectStoreId.Count() > 0)
            {
                showStoreId = SelectStoreId.Distinct<int>().ToList();
            }
            return string.Join(",", showStoreId);
        }




        /// <summary>
        /// 汇总订单商品数量
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static int SumOrderProductCount(List<OrderProductInfo> orderProductList)
        {
            int count = 0;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
                count = count + orderProductInfo.RealCount;
            return count;
        }

        /// <summary>
        /// 获得商品总计
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static decimal SumOrderProductAmount(List<OrderProductInfo> orderProductList)
        {
            decimal productAmount = 0M;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
                if (orderProductInfo.Type == 3)
                    productAmount = productAmount + orderProductInfo.RealCount * orderProductInfo.DiscountPrice;
                else
                    productAmount = productAmount + orderProductInfo.BuyCount * orderProductInfo.DiscountPrice;
            return productAmount;
        }

        /// <summary>
        /// 获得代理商品折扣前总计
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static decimal SumOrderProductAgentOrginAmount(List<OrderProductInfo> orderProductList)
        {
            decimal productAmount = 0M;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                productAmount = productAmount + orderProductInfo.BuyCount * orderProductInfo.ShopPrice;
            }
            return productAmount;
        }

        /// <summary>
        /// 汇总订单商品重量
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static int SumOrderProductWeight(List<OrderProductInfo> orderProductList)
        {
            int totalWeight = 0;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
                totalWeight = totalWeight + orderProductInfo.RealCount * orderProductInfo.Weight;
            return totalWeight;
        }

        /// <summary>
        /// 汇总满减
        /// </summary>
        /// <param name="cartItemList">购物车项列表</param>
        /// <returns></returns>
        public static int SumFullCut(List<CartItemInfo> cartItemList)
        {
            //满减
            int fullCut = 0;
            foreach (CartItemInfo cartItemInfo in cartItemList)
            {
                if (cartItemInfo.Type == 3)
                {
                    CartFullCutInfo cartFullCutInfo = (CartFullCutInfo)cartItemInfo.Item;
                    if (cartFullCutInfo.IsEnough)
                        fullCut += cartFullCutInfo.CutMoney;
                }
            }
            return fullCut;
        }

        /// <summary>
        /// 汇总支付积分
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static int SumPayCredits(List<OrderProductInfo> orderProductList)
        {
            int payCredits = 0;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
                payCredits = payCredits + orderProductInfo.RealCount * orderProductInfo.PayCredits;
            return payCredits;
        }

        /// <summary>
        /// 汇总商品折扣
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static decimal SumOrderProductDiscount(List<OrderProductInfo> orderProductList)
        {
            decimal discount = 0M;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
                discount += (orderProductInfo.RealCount * orderProductInfo.ShopPrice - orderProductInfo.BuyCount * orderProductInfo.ShopPrice);
            return discount;
        }

        /// <summary>
        /// 汇总优惠劵
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static Dictionary<int, CouponTypeInfo> SumCouponType(List<OrderProductInfo> orderProductList)
        {
            Dictionary<int, CouponTypeInfo> couponTypeList = new Dictionary<int, CouponTypeInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.CouponTypeId > 0)
                {
                    CouponTypeInfo couponTypeInfo = Coupons.GetCouponTypeById(orderProductInfo.CouponTypeId);
                    if (couponTypeInfo != null && !couponTypeList.ContainsKey(couponTypeInfo.CouponTypeId))
                    {
                        couponTypeList.Add(couponTypeInfo.CouponTypeId, couponTypeInfo);
                    }
                }
            }
            return couponTypeList;
        }






        /// <summary>
        /// 汇总商城购物车订单商品数量
        /// </summary>
        /// <param name="storeCartList">店铺购物车列表</param>
        /// <returns></returns>
        public static int SumMallCartOrderProductCount(List<StoreCartInfo> storeCartList)
        {
            int count = 0;
            foreach (StoreCartInfo storeCartInfo in storeCartList)
                count += SumOrderProductCount(storeCartInfo.SelectedOrderProductList);
            return count;
        }

        /// <summary>
        /// 汇总商城购物车商品总计
        /// </summary>
        /// <param name="storeCartList">店铺购物车列表</param>
        /// <returns></returns>
        public static decimal SumMallCartOrderProductAmount(List<StoreCartInfo> storeCartList)
        {
            decimal productAmount = 0M;
            foreach (StoreCartInfo storeCartInfo in storeCartList)
                productAmount += SumOrderProductAmount(storeCartInfo.SelectedOrderProductList);
            return productAmount;
        }

        /// <summary>
        /// 汇总商城购物车满减
        /// </summary>
        /// <param name="storeCartList">店铺购物车列表</param>
        /// <returns></returns>
        public static int SumMallCartFullCut(List<StoreCartInfo> storeCartList)
        {
            int fullCut = 0;
            foreach (StoreCartInfo storeCartInfo in storeCartList)
                fullCut += SumFullCut(storeCartInfo.CartItemList);
            return fullCut;
        }

        /// <summary>
        /// 同级购物车产品关税总计
        /// </summary>
        /// <param name="storeCartList"></param>
        /// <returns></returns>
        public static decimal SumMallCartTaxAmount(List<StoreCartInfo> storeCartList)
        {
            decimal taxAmount = 0M;
            foreach (StoreCartInfo storeCartInfo in storeCartList)
                taxAmount += SumOrderProductTaxAmount(storeCartInfo.SelectedOrderProductList);
            return taxAmount;
        }

        /// <summary>
        /// 获得商品关税
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static decimal SumOrderProductTaxAmount(List<OrderProductInfo> orderProductList)
        {
            decimal productTaxAmount = 0M;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {

                productTaxAmount = productTaxAmount + orderProductInfo.BuyCount * orderProductInfo.DiscountPrice * (orderProductInfo.TaxRate / 100);
            }
            return productTaxAmount;
        }

        /// <summary>
        /// 同级购物车产品红包减免
        /// </summary>
        /// <param name="storeCartList"></param>
        /// <returns></returns>
        public static decimal SumMallCartHongBaoCutAmount(List<StoreCartInfo> storeCartList)
        {
            decimal hongBaoAmount = 0M;
            foreach (StoreCartInfo storeCartInfo in storeCartList)
                hongBaoAmount += SumOrderProductHongBaoCutAmount(storeCartInfo.SelectedOrderProductList);
            return hongBaoAmount;
        }
        /// <summary>
        /// 同级购物车产品PV
        /// </summary>
        /// <param name="storeCartList"></param>
        /// <returns></returns>
        public static decimal SumMallPVAmount(List<StoreCartInfo> storeCartList)
        {
            decimal PVAmount = 0M;
            foreach (StoreCartInfo storeCartInfo in storeCartList)
                PVAmount += SumOrderProductPVAmount(storeCartInfo.SelectedOrderProductList);
            return PVAmount;
        }
        /// <summary>
        /// 获得商品红包减免
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static decimal SumOrderProductPVAmount(List<OrderProductInfo> orderProductList)
        {
            decimal productPVAmount = 0M;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                productPVAmount += orderProductInfo.BuyCount * orderProductInfo.PV;
            }
            return productPVAmount;
        }

        /// <summary>
        /// 获得商品红包减免
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static decimal SumOrderProductHongBaoCutAmount(List<OrderProductInfo> orderProductList)
        {
            decimal productHongBaoCutAmount = 0M;
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                productHongBaoCutAmount += orderProductInfo.BuyCount * orderProductInfo.HongBaoCut;
            }
            return productHongBaoCutAmount;
        }
        /// <summary>
        /// 获得店铺商品可用汇购卡金额
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static decimal SumOrderProductCashCutAmount(List<OrderProductInfo> orderProductList, int uid, int chId, int type)
        {
            string selectedCartItemKeyList = string.Join(",", orderProductList.Select(x => x.Pid).ToArray());
            decimal productCashAmount = Channel.GetChannelProductAmount(uid, chId, selectedCartItemKeyList, type);
            return productCashAmount;
        }
        /// <summary>
        /// 获得代理店铺商品可用代理、佣金金额
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static decimal SumOrderProductAgentCutAmount(List<OrderProductInfo> orderProductList, StoreInfo storeInfo)
        {
            decimal productAgentAmount = 0;
            if (storeInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId") || storeInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentSuitStoreId"))
            {
                productAgentAmount = orderProductList.Sum(x => x.BuyCount * x.DiscountPrice);
            }
            return productAgentAmount;
        }
        /// <summary>
        /// 获得普通订单商品列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetCommonOrderProductList(List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> commonOrderProductList = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0)
                    commonOrderProductList.Add(orderProductInfo);
            }
            return commonOrderProductList;
        }

        /// <summary>
        /// 从订单商品列表中获得普通商品
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static OrderProductInfo GetCommonOrderProductByPid(int pid, List<OrderProductInfo> orderProductList)
        {
            foreach (OrderProductInfo info in orderProductList)
            {
                if (info.Type == 0 && info.Pid == pid)
                    return info;
            }
            return null;
        }

        /// <summary>
        /// 从订单商品列表中获得赠品列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="pmId">赠品促销活动id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetGiftOrderProductList(int type, int pmId, List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> giftOrderProductList = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == type && orderProductInfo.ExtCode1 == pmId)
                    giftOrderProductList.Add(orderProductInfo);
            }
            return giftOrderProductList;
        }

        /// <summary>
        /// 从订单商品列表中获得指定套装商品列表
        /// </summary>
        /// <param name="pmId">套装促销活动id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="isContainGift">是否包含赠品</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetSuitOrderProductList(int pmId, List<OrderProductInfo> orderProductList, bool isContainGift)
        {
            List<OrderProductInfo> suitOrderProductList = new List<OrderProductInfo>();
            if (isContainGift)
            {
                foreach (OrderProductInfo orderProductInfo in orderProductList)
                {
                    if (orderProductInfo.Type == 3 && orderProductInfo.ExtCode1 == pmId)
                    {
                        suitOrderProductList.Add(orderProductInfo);
                        foreach (OrderProductInfo item in orderProductList)
                        {
                            if (item.Type == 2 && item.ExtCode1 == orderProductInfo.ExtCode2)
                                suitOrderProductList.Add(item);
                        }
                    }
                }
            }
            else
            {
                foreach (OrderProductInfo orderProductInfo in orderProductList)
                {
                    if (orderProductInfo.Type == 3 && orderProductInfo.ExtCode1 == pmId)
                        suitOrderProductList.Add(orderProductInfo);
                }
            }
            return suitOrderProductList;
        }

        /// <summary>
        /// 从订单商品列表中获得指定的满赠主商品列表
        /// </summary>
        /// <param name="pmId">满赠促销活动id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetFullSendMainOrderProductList(int pmId, List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> list = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0 && orderProductInfo.ExtCode4 == pmId)
                    list.Add(orderProductInfo);
            }
            return list;
        }

        /// <summary>
        /// 从订单商品列表中获的满赠次商品列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetFullSendMinorOrderProductList(List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> list = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 4)
                    list.Add(orderProductInfo);
            }
            return list;
        }

        /// <summary>
        /// 从订单商品列表中获得指定的满赠次商品
        /// </summary>
        /// <param name="pmId">满赠促销活动id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static OrderProductInfo GetFullSendMinorOrderProduct(int pmId, List<OrderProductInfo> orderProductList)
        {
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 4 && orderProductInfo.ExtCode1 == pmId)
                    return orderProductInfo;
            }
            return null;
        }

        /// <summary>
        /// 从订单商品列表中获得指定满减商品列表
        /// </summary>
        /// <param name="pmId">满减促销活动id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetFullCutOrderProductList(int pmId, List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> fullCutOrderProductList = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 0 && orderProductInfo.ExtCode5 == pmId)
                    fullCutOrderProductList.Add(orderProductInfo);
            }
            return fullCutOrderProductList;
        }

        /// <summary>
        /// 从订单商品列表中获得指定商品
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static OrderProductInfo GetOrderProductByPid(int pid, List<OrderProductInfo> orderProductList)
        {
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Pid == pid)
                    return orderProductInfo;
            }
            return null;
        }

        /// <summary>
        /// 获得商品id列表
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static string GetPidList(List<OrderProductInfo> orderProductList)
        {
            if (orderProductList.Count == 0)
                return string.Empty;

            if (orderProductList.Count == 1)
                return orderProductList[0].Pid.ToString();

            List<int> pidList = new List<int>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                pidList.Add(orderProductInfo.Pid);
            }

            StringBuilder sb = new StringBuilder();
            foreach (int pid in pidList.Distinct<int>())
                sb.AppendFormat("{0},", pid);
            return CommonHelper.GetUniqueString(sb.Remove(sb.Length - 1, 1).ToString());
        }





        /// <summary>
        /// 获得商城购物车商品id列表
        /// </summary>
        /// <param name="storeCartList">店铺购物车列表/param>
        /// <returns></returns>
        public static string GetMallCartPidList(List<StoreCartInfo> storeCartList)
        {
            if (storeCartList.Count == 0)
                return string.Empty;

            List<int> pidList = new List<int>();
            foreach (StoreCartInfo storeCartInfo in storeCartList)
            {
                foreach (OrderProductInfo orderProductInfo in storeCartInfo.SelectedOrderProductList)
                {
                    pidList.Add(orderProductInfo.Pid);
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (int pid in pidList.Distinct<int>())
                sb.AppendFormat("{0},", pid);
            return CommonHelper.GetUniqueString(sb.Remove(sb.Length - 1, 1).ToString());
        }

        /// <summary>
        /// 从订单商品列表中获得同一配送方式的商品列表
        /// </summary>
        /// <param name="storeSTId">店铺配送模板id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetSameShipOrderProductList(int storeSTId, List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> list = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.StoreSTid == storeSTId)
                    list.Add(orderProductInfo);
            }
            return list;
        }

        /// <summary>
        /// 获得指定店铺的订单商品列表
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns></returns>
        public static List<OrderProductInfo> GetStoreOrderProductList(int storeId, List<OrderProductInfo> orderProductList)
        {
            List<OrderProductInfo> storeOrderProductList = new List<OrderProductInfo>();
            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.StoreId == storeId)
                    storeOrderProductList.Add(orderProductInfo);
            }
            return storeOrderProductList;
        }









        /// <summary>
        /// 设置订单商品的买送促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="buySendPromotionInfo">买送优惠活动</param>
        public static void SetBuySendPromotionOfOrderProduct(OrderProductInfo orderProductInfo, int buyCount, BuySendPromotionInfo buySendPromotionInfo)
        {
            orderProductInfo.RealCount = buyCount + (buyCount / buySendPromotionInfo.BuyCount) * buySendPromotionInfo.SendCount;
            orderProductInfo.BuyCount = buyCount;
            orderProductInfo.ExtCode2 = buySendPromotionInfo.PmId;
            orderProductInfo.PV = 0;
            orderProductInfo.HaiMi = 0;
            orderProductInfo.HongBaoCut = 0;
            orderProductInfo.ProductHaiMi = 0;
            orderProductInfo.ProductHBCut = 0;
            orderProductInfo.ProductPV = 0;
        }

        /// <summary>
        /// 设置订单商品的单品促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="singlePromotionInfo">单品促销活动</param>
        public static void SetSinglePromotionOfOrderProduct(OrderProductInfo orderProductInfo, SinglePromotionInfo singlePromotionInfo)
        {
            //if (orderProductInfo.BuyCount >= singlePromotionInfo.QuotaLower)
            //{
            //    orderProductInfo.ProductHaiMi = singlePromotionInfo.PromoHaiMi;
            //    orderProductInfo.ProductPV = singlePromotionInfo.PromoPV;
            //    orderProductInfo.ProductHBCut = singlePromotionInfo.PromoHongBaoCut;
            //}
            orderProductInfo.ExtCode1 = singlePromotionInfo.PmId;
            PartUserInfo user = Users.GetPartUserById(orderProductInfo.Uid);
            if (singlePromotionInfo.UserRankLower == Convert.ToInt32(user.IsDirSaleUser) || singlePromotionInfo.UserRankLower == 0)
            {
                orderProductInfo.ProductHaiMi = singlePromotionInfo.PromoHaiMi;
                orderProductInfo.ProductPV = singlePromotionInfo.PromoPV;
                orderProductInfo.ProductHBCut = singlePromotionInfo.PromoHongBaoCut;
                switch (singlePromotionInfo.DiscountType)
                {
                    case 0://商城价折扣
                        {
                            decimal temp = Math.Round((orderProductInfo.ShopPrice * singlePromotionInfo.DiscountValue) / 10, 2);
                            orderProductInfo.DiscountPrice = temp < 0 ? orderProductInfo.ShopPrice : temp;
                            break;
                        }
                    case 1://直降
                        {
                            decimal temp = orderProductInfo.ShopPrice - singlePromotionInfo.DiscountValue;
                            orderProductInfo.DiscountPrice = temp < 0 ? orderProductInfo.ShopPrice : temp;
                            break;
                        }
                    case 2://折后价
                        {
                            orderProductInfo.DiscountPrice = singlePromotionInfo.DiscountValue;
                            break;
                        }
                    case 3://秒杀价
                        {
                            orderProductInfo.DiscountPrice = singlePromotionInfo.DiscountValue;
                            break;
                        }
                    case 4://市场价折扣
                        {
                            decimal temp = Math.Round((orderProductInfo.MarketPrice * singlePromotionInfo.DiscountValue) / 10, 2);
                            orderProductInfo.DiscountPrice = temp < 0 ? orderProductInfo.MarketPrice : temp;
                            break;
                        }
                    case 5://特价
                        {
                            orderProductInfo.DiscountPrice = singlePromotionInfo.DiscountValue;
                            break;
                        }
                    case 6://兑换价
                        {
                            orderProductInfo.DiscountPrice = singlePromotionInfo.DiscountValue;
                            break;
                        }
                    case 7:
                        {
                            if (singlePromotionInfo.Amount1 > 0 && orderProductInfo.BuyCount > singlePromotionInfo.Amount1)
                                orderProductInfo.DiscountPrice = singlePromotionInfo.Discount1;
                            if (singlePromotionInfo.Amount2 > 0 && orderProductInfo.BuyCount > singlePromotionInfo.Amount2)
                                orderProductInfo.DiscountPrice = singlePromotionInfo.Discount2;
                            if (singlePromotionInfo.Amount3 > 0 && orderProductInfo.BuyCount > singlePromotionInfo.Amount3)
                                orderProductInfo.DiscountPrice = singlePromotionInfo.Discount3;
                            if (singlePromotionInfo.Amount4 > 0 && orderProductInfo.BuyCount > singlePromotionInfo.Amount4)
                                orderProductInfo.DiscountPrice = singlePromotionInfo.Discount4;
                            if (singlePromotionInfo.Amount5 > 0 && orderProductInfo.BuyCount > singlePromotionInfo.Amount5)
                                orderProductInfo.DiscountPrice = singlePromotionInfo.Discount5;
                            break;
                        }
                    case 8:
                        {
                            if (orderProductInfo.BuyCount % 2 == 0)
                                orderProductInfo.DiscountPrice = singlePromotionInfo.DiscountValue;
                            break;
                        }
                    case 9:
                        {
                            orderProductInfo.DiscountPrice = singlePromotionInfo.DiscountValue;
                            break;
                        }
                    default://默认
                        {
                            orderProductInfo.DiscountPrice = singlePromotionInfo.DiscountValue;
                            break;
                        }
                }
            }
            //设置赠送积分
            if (singlePromotionInfo.PayCredits > 0)
                orderProductInfo.PayCredits = singlePromotionInfo.PayCredits;

            //设置赠送优惠劵
            if (singlePromotionInfo.CouponTypeId > 0)
                orderProductInfo.CouponTypeId = singlePromotionInfo.CouponTypeId;
        }

        /// <summary>
        /// 设置赠品订单商品
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="type">类型</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="number">赠送数量</param>
        /// <param name="pmId">赠品促销活动id</param>
        public static void SetGiftOrderProduct(OrderProductInfo orderProductInfo, int type, int buyCount, int number, int pmId)
        {
            orderProductInfo.DiscountPrice = 0M;
            orderProductInfo.RealCount = buyCount * number;
            orderProductInfo.BuyCount = 0;
            orderProductInfo.Type = type;
            orderProductInfo.ExtCode1 = pmId;
            orderProductInfo.ExtCode2 = number;
        }

        /// <summary>
        /// 设置订单商品的满赠促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="fullSendPromotionInfo">满赠促销活动</param>
        public static void SetFullSendPromotionOfOrderProduct(OrderProductInfo orderProductInfo, FullSendPromotionInfo fullSendPromotionInfo)
        {
            orderProductInfo.ExtCode4 = fullSendPromotionInfo.PmId;
        }

        /// <summary>
        /// 设置订单商品的满减促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="fullCutPromotionInfo">满减促销活动</param>
        public static void SetFullCutPromotionOfOrderProduct(OrderProductInfo orderProductInfo, FullCutPromotionInfo fullCutPromotionInfo)
        {
            orderProductInfo.ExtCode5 = fullCutPromotionInfo.PmId;
        }

        /// <summary>
        /// 更新订单商品的买送促销活动
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="buyTime">购买时间</param>
        public static void UpdateBuySendPromotionOfOrderProduct(OrderProductInfo orderProductInfo, int buyCount, DateTime buyTime)
        {
            //获得商品的买送促销活动
            BuySendPromotionInfo buySendPromotionInfo = Promotions.GetBuySendPromotion(buyCount, orderProductInfo.StoreId, orderProductInfo.Pid, buyTime);

            if (buySendPromotionInfo == null && orderProductInfo.ExtCode2 > 0)//当商品存在买送促销活动但添加后不存在买送促销活动时
            {
                orderProductInfo.RealCount = buyCount;
                orderProductInfo.BuyCount = buyCount;
                orderProductInfo.ExtCode2 = 0;
            }
            else if (buySendPromotionInfo != null && orderProductInfo.ExtCode2 <= 0)//当商品不存在买送促销活动但添加后存在买送促销活动时
            {
                SetBuySendPromotionOfOrderProduct(orderProductInfo, buyCount, buySendPromotionInfo);
            }
            else if (buySendPromotionInfo != null && orderProductInfo.ExtCode2 > 0)//当商品存在买送促销活动但添加后仍然满足买送促销活动时
            {
                SetBuySendPromotionOfOrderProduct(orderProductInfo, buyCount, buySendPromotionInfo);
            }
            else
            {
                orderProductInfo.RealCount = buyCount;
                orderProductInfo.BuyCount = buyCount;
            }
        }

        /// <summary>
        /// 更新赠品订单商品
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="buyCount">购买数量</param>
        /// <returns></returns>
        public static List<OrderProductInfo> UpdateGiftOrderProduct(List<OrderProductInfo> orderProductList, OrderProductInfo orderProductInfo, int buyCount)
        {
            if (orderProductInfo.ExtCode3 < 1)
                return new List<OrderProductInfo>();

            //获得赠品订单商品列表
            List<OrderProductInfo> giftOrderProductList = GetGiftOrderProductList(1, orderProductInfo.ExtCode3, orderProductList);
            foreach (OrderProductInfo item in giftOrderProductList)
                item.RealCount = buyCount * item.ExtCode2;
            return giftOrderProductList;
        }

        /// <summary>
        /// 设置套装订单商品
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="number">数量</param>
        /// <param name="discount">折扣值</param>
        /// <param name="suitPromotionInfo">套装促销活动</param>
        public static void SetSuitOrderProduct(OrderProductInfo orderProductInfo, int buyCount, int number, decimal discount, SuitPromotionInfo suitPromotionInfo, int i)
        {
            if (i == 1)
            {
                orderProductInfo.PV = suitPromotionInfo.SuitPV;
                orderProductInfo.ProductPV = suitPromotionInfo.SuitPV;
                orderProductInfo.HaiMi = suitPromotionInfo.SuitHaiMi;
                orderProductInfo.ProductHaiMi = suitPromotionInfo.SuitHaiMi;
                orderProductInfo.HongBaoCut = suitPromotionInfo.SuitHongBaoCut;
                orderProductInfo.ProductHBCut = suitPromotionInfo.SuitHongBaoCut;
            }
            orderProductInfo.Type = 3;
            orderProductInfo.ExtCode1 = suitPromotionInfo.PmId;
            orderProductInfo.ExtCode2 = number;
            orderProductInfo.RealCount = number * buyCount;
            orderProductInfo.BuyCount = buyCount;
            orderProductInfo.DiscountPrice = orderProductInfo.ShopPrice - discount;
        }

        /// <summary>
        /// 设置满赠订单商品
        /// </summary>
        /// <param name="orderProductInfo">订单商品</param>
        /// <param name="fullSendPromotionInfo">满赠促销活动</param>
        public static void SetFullSendOrderProduct(OrderProductInfo orderProductInfo, FullSendPromotionInfo fullSendPromotionInfo)
        {
            orderProductInfo.RealCount = 1;
            orderProductInfo.BuyCount = 1;
            orderProductInfo.DiscountPrice = fullSendPromotionInfo.AddMoney;
            orderProductInfo.Type = 4;
            orderProductInfo.ExtCode1 = fullSendPromotionInfo.PmId;
        }






        /// <summary>
        /// 删除购物车中的商品
        /// </summary>
        /// <param name="orderProductList">购物车中商品列表</param>
        /// <param name="orderProductInfo">删除商品</param>
        public static void DeleteCartProduct(ref List<OrderProductInfo> orderProductList, OrderProductInfo orderProductInfo)
        {
            //需要删除的商品列表
            List<OrderProductInfo> delOrderProductList = new List<OrderProductInfo>();

            //将主商品添加到需要删除的商品列表中
            delOrderProductList.Add(orderProductInfo);

            if (orderProductInfo.ExtCode3 > 0)
            {
                //赠品商品列表
                List<OrderProductInfo> giftOrderProductList = GetGiftOrderProductList(1, orderProductInfo.ExtCode3, orderProductList);
                //将赠品添加到需要删除的商品列表中
                delOrderProductList.AddRange(giftOrderProductList);
            }

            if (orderProductInfo.ExtCode4 > 0)
            {
                //满赠赠品
                OrderProductInfo fullSendMinorOrderProductInfo = GetFullSendMinorOrderProduct(orderProductInfo.ExtCode4, orderProductList);
                if (fullSendMinorOrderProductInfo != null)
                {
                    FullSendPromotionInfo fullSendPromotionInfo = Promotions.GetFullSendPromotionByPmIdAndTime(orderProductInfo.ExtCode4, DateTime.Now);
                    if (fullSendPromotionInfo != null)
                    {
                        List<OrderProductInfo> fullSendMainOrderProductList = GetFullSendMainOrderProductList(orderProductInfo.ExtCode4, orderProductList);
                        decimal amount = SumOrderProductAmount(fullSendMainOrderProductList) - orderProductInfo.DiscountPrice * orderProductInfo.BuyCount;
                        if (amount < fullSendPromotionInfo.LimitMoney || fullSendMinorOrderProductInfo.DiscountPrice != fullSendPromotionInfo.AddMoney)
                            delOrderProductList.Add(fullSendMinorOrderProductInfo);
                    }
                    else
                    {
                        delOrderProductList.Add(fullSendMinorOrderProductInfo);
                    }
                }
            }

            //删除商品
            DeleteOrderProductList(delOrderProductList);
            foreach (OrderProductInfo item in delOrderProductList)
                orderProductList.Remove(item);
        }

        /// <summary>
        /// 添加已经存在的商品到购物车中
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="orderProductInfo">订单商品信息</param>
        /// <param name="buyTime">购买时间</param>
        public static void AddExistProductToCart(ref List<OrderProductInfo> orderProductList, int buyCount, OrderProductInfo orderProductInfo, DateTime buyTime)
        {
            List<OrderProductInfo> updateOrderProductList = new List<OrderProductInfo>();

            //更新买送促销活动
            UpdateBuySendPromotionOfOrderProduct(orderProductInfo, buyCount, buyTime);
            //更新订单商品的赠品促销活动
            List<OrderProductInfo> giftOrderProductList = UpdateGiftOrderProduct(orderProductList, orderProductInfo, buyCount);

            updateOrderProductList.Add(orderProductInfo);
            updateOrderProductList.AddRange(giftOrderProductList);
            //PartUserInfo user = Users.GetPartUserById(orderProductInfo.Uid);
            //if (user != null)
            //{
            //    if (orderProductInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"))
            //    {
            //        decimal discountPrice=ord
            //        if (user.AgentType == 1)
            //        {
            //            mainOrderProductInfo.DiscountPrice = (decimal)((double)mainOrderProductInfo.DiscountPrice * 0.65);
            //        }
            //        else if (user.AgentType == 2)
            //        {
            //            mainOrderProductInfo.DiscountPrice = (decimal)((double)mainOrderProductInfo.DiscountPrice * 0.55);
            //        }
            //        else if (user.AgentType == 3)
            //        {
            //            mainOrderProductInfo.DiscountPrice = (decimal)((double)mainOrderProductInfo.DiscountPrice * 0.47);
            //        }
            //        else if (user.AgentType == 4)
            //        {
            //            mainOrderProductInfo.DiscountPrice = (decimal)((double)mainOrderProductInfo.DiscountPrice * 0.42);
            //        }
            //        else if (user.AgentType == 5)
            //        {
            //            mainOrderProductInfo.DiscountPrice = (decimal)((double)mainOrderProductInfo.DiscountPrice * 0.4);
            //        }
            //        UpdateOrderProductCount(updateOrderProductList);
            //    }
            //}
            UpdateOrderProductCount(updateOrderProductList);
        }

        /// <summary>
        /// 添加新商品到购物车
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="partProductInfo">购买商品</param>
        /// <param name="sid">用户sessionId</param>
        /// <param name="uid">用户id</param>
        /// <param name="buyTime">购买时间</param>
        public static void AddNewProductToCart(ref List<OrderProductInfo> orderProductList, int buyCount, PartProductInfo partProductInfo, string sid, int uid, DateTime buyTime)
        {
            //需要添加的商品列表
            List<OrderProductInfo> addOrderProductList = new List<OrderProductInfo>();

            //初始化订单商品
            OrderProductInfo mainOrderProductInfo = BuildOrderProduct(partProductInfo);
            InitOrderProduct(mainOrderProductInfo, buyCount, sid, uid, buyTime);
            mainOrderProductInfo.ProductPV = partProductInfo.PV;
            mainOrderProductInfo.ProductHaiMi = partProductInfo.HaiMi;
            mainOrderProductInfo.ProductHBCut = partProductInfo.HongBaoCut;
            StoreInfo storeInfo = Stores.GetStoreById(mainOrderProductInfo.StoreId);
            if (storeInfo != null)
                mainOrderProductInfo.MallSource = storeInfo.MallSource;
            PartUserInfo user = Users.GetPartUserById(uid);
            if (user != null)
            {
                if (partProductInfo.StoreId == WebHelper.GetConfigSettingsInt("AgentStoreId"))
                {
                    if (user.AgentType == 1 || user.Ds2AgentRank == 1)
                    {
                        mainOrderProductInfo.DiscountPrice = (decimal)((double)mainOrderProductInfo.DiscountPrice * 0.65);
                    }
                    else if (user.AgentType == 2 || user.Ds2AgentRank == 2)
                    {
                        mainOrderProductInfo.DiscountPrice = (decimal)((double)mainOrderProductInfo.DiscountPrice * 0.55);
                    }
                    else if (user.AgentType == 3 || user.Ds2AgentRank == 3)
                    {
                        mainOrderProductInfo.DiscountPrice = (decimal)((double)mainOrderProductInfo.DiscountPrice * 0.47);
                    }
                    else if (user.AgentType == 4 || user.Ds2AgentRank == 4)
                    {
                        if (user.Uid == WebHelper.GetConfigSettingsInt("SpecialAgentUid"))
                            mainOrderProductInfo.DiscountPrice = (decimal)((double)mainOrderProductInfo.DiscountPrice * 0.4);
                        else
                            mainOrderProductInfo.DiscountPrice = (decimal)((double)mainOrderProductInfo.DiscountPrice * 0.42);
                    }
                    else if (user.AgentType == 5 || user.Ds2AgentRank == 5)
                    {
                        mainOrderProductInfo.DiscountPrice = (decimal)((double)mainOrderProductInfo.DiscountPrice * 0.4);
                    }
                }
            }

            //所有会员购买安即可4800套餐 价格为30w 单价为30w/4800=62.5
            //PartUserInfo userInfo = Users.GetPartUserById(uid);
            //if (userInfo != null) {
            //if (partProductInfo.Pid == 3236)
            //{
            //    mainOrderProductInfo.DiscountPrice = 62.5M;
            //}
            // }

            //获得买送促销活动
            BuySendPromotionInfo buySendPromotionInfo = Promotions.GetBuySendPromotion(buyCount, partProductInfo.StoreId, partProductInfo.Pid, buyTime);
            //当买送促销活动存在时设置订单商品信息
            if (buySendPromotionInfo != null)
                SetBuySendPromotionOfOrderProduct(mainOrderProductInfo, buyCount, buySendPromotionInfo);

            //获得单品促销活动
            SinglePromotionInfo singlePromotionInfo = Promotions.GetSinglePromotionByPidAndTime(partProductInfo.Pid, buyTime);
            //当单品促销活动存在时则设置订单商品信息
            if (singlePromotionInfo != null)
                SetSinglePromotionOfOrderProduct(mainOrderProductInfo, singlePromotionInfo);

            //获得满赠促销活动
            FullSendPromotionInfo fullSendPromotionInfo = Promotions.GetFullSendPromotionByStoreIdAndPidAndTime(partProductInfo.StoreId, partProductInfo.Pid, buyTime);
            if (fullSendPromotionInfo != null)
                SetFullSendPromotionOfOrderProduct(mainOrderProductInfo, fullSendPromotionInfo);

            //获得满减促销活动
            FullCutPromotionInfo fullCutPromotionInfo = Promotions.GetFullCutPromotionByStoreIdAndPidAndTime(partProductInfo.StoreId, partProductInfo.Pid, buyTime);
            if (fullCutPromotionInfo != null)
                SetFullCutPromotionOfOrderProduct(mainOrderProductInfo, fullCutPromotionInfo);


            //将商品添加到"需要添加的商品列表"中
            addOrderProductList.Add(mainOrderProductInfo);

            //获得赠品列表
            GiftPromotionInfo giftPromotionInfo = Promotions.GetGiftPromotionByPidAndTime(partProductInfo.Pid, buyTime);
            if (giftPromotionInfo != null)
            {
                List<ExtGiftInfo> extGiftList = Promotions.GetExtGiftList(giftPromotionInfo.PmId);
                if (extGiftList.Count > 0)
                {
                    mainOrderProductInfo.ExtCode3 = giftPromotionInfo.PmId;
                    foreach (ExtGiftInfo extGiftInfo in extGiftList)
                    {
                        OrderProductInfo giftOrderProduct = BuildOrderProduct(extGiftInfo);
                        InitOrderProduct(giftOrderProduct, 0, sid, uid, buyTime);
                        SetGiftOrderProduct(giftOrderProduct, 1, mainOrderProductInfo.RealCount, extGiftInfo.Number, giftPromotionInfo.PmId);
                        //将赠品添加到"需要添加的商品列表"中
                        addOrderProductList.Add(giftOrderProduct);
                    }
                }
            }

            //将需要添加的商品持久化
            AddOrderProductList(addOrderProductList);

            orderProductList.AddRange(addOrderProductList);
        }

        /// <summary>
        /// 将商品添加到购物车
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="partProductInfo">购买商品</param>
        /// <param name="sid">用户sessionId</param>
        /// <param name="uid">用户id</param>
        /// <param name="buyTime">购买时间</param>
        public static void AddProductToCart(ref List<OrderProductInfo> orderProductList, int buyCount, PartProductInfo partProductInfo, string sid, int uid, DateTime buyTime)
        {
            if (orderProductList.Count == 0)
            {
                AddNewProductToCart(ref orderProductList, buyCount, partProductInfo, sid, uid, buyTime);
            }
            else
            {
                OrderProductInfo orderProductInfo = GetCommonOrderProductByPid(partProductInfo.Pid, orderProductList);
                if (orderProductInfo == null)//此商品作为普通商品不存在于购物车中时
                    AddNewProductToCart(ref orderProductList, buyCount, partProductInfo, sid, uid, buyTime);
                else//此商品作为普通商品存在于购物车中时
                    AddExistProductToCart(ref orderProductList, buyCount, orderProductInfo, buyTime);
            }
        }

        /// <summary>
        /// 删除购物车中的套装
        /// </summary>
        /// <param name="orderProductList">购物车商品列表</param>
        /// <param name="pmId">套装促销活动id</param>
        public static void DeleteCartSuit(ref List<OrderProductInfo> orderProductList, int pmId)
        {
            //需要删除的商品列表
            List<OrderProductInfo> delOrderProductList = new List<OrderProductInfo>();

            foreach (OrderProductInfo orderProductInfo in orderProductList)
            {
                if (orderProductInfo.Type == 3 && orderProductInfo.ExtCode1 == pmId)
                {
                    delOrderProductList.Add(orderProductInfo);
                    if (orderProductInfo.ExtCode3 > 0)
                    {
                        delOrderProductList.AddRange(GetGiftOrderProductList(2, orderProductInfo.ExtCode3, orderProductList));
                    }
                }
            }

            //删除商品
            DeleteOrderProductList(delOrderProductList);
            foreach (OrderProductInfo orderProductInfo in delOrderProductList)
                orderProductList.Remove(orderProductInfo);
        }

        /// <summary>
        /// 添加已经存在的套装到购物车中
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="suitOrderProductList">套装商品列表</param>
        /// <param name="buyCount">购买数量</param>
        public static void AddExistSuitToCart(ref List<OrderProductInfo> orderProductList, List<OrderProductInfo> suitOrderProductList, int buyCount)
        {
            List<OrderProductInfo> list = new List<OrderProductInfo>();

            foreach (OrderProductInfo orderProductInfo in suitOrderProductList)
            {
                if (orderProductInfo.Type == 3)
                {
                    orderProductInfo.RealCount = buyCount * orderProductInfo.ExtCode2;
                    orderProductInfo.BuyCount = buyCount;
                    list.Add(orderProductInfo);
                    if (orderProductInfo.ExtCode3 > 0)
                    {
                        foreach (OrderProductInfo item in suitOrderProductList)
                        {
                            if (item.Type == 2)
                            {
                                item.RealCount = orderProductInfo.RealCount * item.ExtCode2;
                                list.Add(item);
                            }
                        }
                    }
                }
            }

            UpdateOrderProductCount(list);
        }

        /// <summary>
        /// 添加新套装到购物车中
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="extSuitProductList">扩展套装商品列表</param>
        /// <param name="suitPromotionInfo">套装促销活动信息</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="sid">用户sessionId</param>
        /// <param name="uid">用户id</param>
        /// <param name="buyTime">购买时间</param>
        public static void AddNewSuitToCart(ref List<OrderProductInfo> orderProductList, List<ExtSuitProductInfo> extSuitProductList, SuitPromotionInfo suitPromotionInfo, int buyCount, string sid, int uid, DateTime buyTime)
        {
            List<OrderProductInfo> addOrderProductList = new List<OrderProductInfo>();
            int i = 1;
            foreach (ExtSuitProductInfo extSuitProductInfo in extSuitProductList)
            {
                OrderProductInfo suitOrderProductInfo = BuildOrderProduct(extSuitProductInfo);
                InitOrderProduct(suitOrderProductInfo, 0, sid, uid, buyTime);
                SetSuitOrderProduct(suitOrderProductInfo, buyCount, extSuitProductInfo.Number, extSuitProductInfo.Discount, suitPromotionInfo, i);
                i++;
                addOrderProductList.Add(suitOrderProductInfo);

                //获得赠品列表
                GiftPromotionInfo giftPromotionInfo = Promotions.GetGiftPromotionByPidAndTime(suitOrderProductInfo.Pid, buyTime);
                if (giftPromotionInfo != null)
                {
                    List<ExtGiftInfo> extGiftList = Promotions.GetExtGiftList(giftPromotionInfo.PmId);
                    if (extGiftList.Count > 0)
                    {
                        suitOrderProductInfo.ExtCode3 = giftPromotionInfo.PmId;
                        foreach (ExtGiftInfo extGiftInfo in extGiftList)
                        {
                            OrderProductInfo giftOrderProduct = BuildOrderProduct(extGiftInfo);
                            InitOrderProduct(giftOrderProduct, 0, sid, uid, buyTime);
                            SetGiftOrderProduct(giftOrderProduct, 2, suitOrderProductInfo.RealCount, extGiftInfo.Number, giftPromotionInfo.PmId);
                            //将赠品添加到"需要添加的商品列表"中
                            addOrderProductList.Add(giftOrderProduct);
                        }
                    }
                }
            }

            //将需要添加的商品持久化
            AddOrderProductList(addOrderProductList);

            orderProductList.AddRange(addOrderProductList);
        }

        /// <summary>
        /// 添加套装到购物车
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="extSuitProductList">扩展套装商品列表</param>
        /// <param name="suitPromotionInfo">套装促销活动</param>
        /// <param name="buyCount">购买数量</param>
        /// <param name="sid">用户sessionId</param>
        /// <param name="uid">用户id</param>
        /// <param name="buyTime">购买时间</param>
        public static void AddSuitToCart(ref List<OrderProductInfo> orderProductList, List<ExtSuitProductInfo> extSuitProductList, SuitPromotionInfo suitPromotionInfo, int buyCount, string sid, int uid, DateTime buyTime)
        {
            //套装商品列表
            List<OrderProductInfo> suitOrderProductList = GetSuitOrderProductList(suitPromotionInfo.PmId, orderProductList, true);
            if (suitOrderProductList.Count < 1)
                AddNewSuitToCart(ref orderProductList, extSuitProductList, suitPromotionInfo, buyCount, sid, uid, buyTime);
            else
                AddExistSuitToCart(ref orderProductList, suitOrderProductList, buyCount);
        }

        /// <summary>
        /// 删除购物车中的满赠
        /// </summary>
        /// <param name="orderProductList">购物车商品列表</param>
        /// <param name="pmId">满赠促销活动id</param>
        public static void DeleteCartFullSend(ref List<OrderProductInfo> orderProductList, int pmId)
        {
            //赠送商品
            OrderProductInfo fullSendMinorOrderProductInfo = GetFullSendMinorOrderProduct(pmId, orderProductList);
            if (fullSendMinorOrderProductInfo != null)
            {
                orderProductList.Remove(fullSendMinorOrderProductInfo);
                DeleteOrderProductList(new List<OrderProductInfo>() { fullSendMinorOrderProductInfo });
            }
        }

        /// <summary>
        /// 删除购物车中的满赠
        /// </summary>
        /// <param name="orderProductList">购物车商品列表</param>
        /// <param name="fullSendMinorOrderProductInfo">赠送商品</param>
        public static void DeleteCartFullSend(ref List<OrderProductInfo> orderProductList, OrderProductInfo fullSendMinorOrderProductInfo)
        {
            orderProductList.Remove(fullSendMinorOrderProductInfo);
            DeleteOrderProductList(new List<OrderProductInfo>() { fullSendMinorOrderProductInfo });
        }

        /// <summary>
        /// 添加满赠到购物车中
        /// </summary>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="partProductInfo">购买商品</param>
        /// <param name="fullSendPromotionInfo">满赠促销活动</param>
        /// <param name="sid">用户sessionId</param>
        /// <param name="uid">用户id</param>
        /// <param name="buyTime">购买时间</param>
        public static void AddFullSendToCart(ref List<OrderProductInfo> orderProductList, PartProductInfo partProductInfo, FullSendPromotionInfo fullSendPromotionInfo, string sid, int uid, DateTime buyTime)
        {
            OrderProductInfo orderProductInfo = BuildOrderProduct(partProductInfo);
            InitOrderProduct(orderProductInfo, 0, sid, uid, buyTime);
            SetFullSendOrderProduct(orderProductInfo, fullSendPromotionInfo);
            AddOrderProductList(new List<OrderProductInfo>() { orderProductInfo });
            orderProductList.Add(orderProductInfo);
        }






        /// <summary>
        /// 创建OrderProductInfo
        /// </summary>
        public static OrderProductInfo BuildOrderProduct(PartProductInfo partProuctInfo)
        {
            OrderProductInfo orderProductInfo = new OrderProductInfo();
            orderProductInfo.Pid = partProuctInfo.Pid;
            orderProductInfo.PSN = partProuctInfo.PSN;
            orderProductInfo.CateId = partProuctInfo.CateId;
            orderProductInfo.BrandId = partProuctInfo.BrandId;
            orderProductInfo.StoreId = partProuctInfo.StoreId;
            orderProductInfo.StoreCid = partProuctInfo.StoreCid;
            orderProductInfo.StoreSTid = partProuctInfo.StoreSTid;
            orderProductInfo.Name = partProuctInfo.Name;
            orderProductInfo.DiscountPrice = partProuctInfo.ShopPrice;
            orderProductInfo.ShopPrice = partProuctInfo.ShopPrice;
            orderProductInfo.MarketPrice = partProuctInfo.MarketPrice;
            orderProductInfo.CostPrice = partProuctInfo.CostPrice;
            orderProductInfo.Weight = partProuctInfo.Weight;
            orderProductInfo.ShowImg = partProuctInfo.ShowImg;
            orderProductInfo.SaleType = partProuctInfo.SaleType;
            return orderProductInfo;
        }

        /// <summary>
        /// 创建OrderProductInfo
        /// </summary>
        public static OrderProductInfo BuildOrderProduct(ExtGiftInfo extGiftInfo)
        {
            OrderProductInfo orderProductInfo = new OrderProductInfo();
            orderProductInfo.Pid = extGiftInfo.Pid;
            orderProductInfo.PSN = extGiftInfo.PSN;
            orderProductInfo.CateId = extGiftInfo.CateId;
            orderProductInfo.BrandId = extGiftInfo.BrandId;
            orderProductInfo.StoreId = extGiftInfo.StoreId;
            orderProductInfo.StoreCid = extGiftInfo.StoreCid;
            orderProductInfo.StoreSTid = extGiftInfo.StoreSTid;
            orderProductInfo.Name = extGiftInfo.Name;
            orderProductInfo.DiscountPrice = extGiftInfo.ShopPrice;
            orderProductInfo.ShopPrice = extGiftInfo.ShopPrice;
            orderProductInfo.MarketPrice = extGiftInfo.MarketPrice;
            orderProductInfo.CostPrice = extGiftInfo.CostPrice;
            orderProductInfo.Weight = extGiftInfo.Weight;
            orderProductInfo.ShowImg = extGiftInfo.ShowImg;
            return orderProductInfo;
        }

        /// <summary>
        /// 创建OrderProductInfo
        /// </summary>
        public static OrderProductInfo BuildOrderProduct(ExtSuitProductInfo extSuitProductInfo)
        {
            OrderProductInfo orderProductInfo = new OrderProductInfo();
            orderProductInfo.Pid = extSuitProductInfo.Pid;
            orderProductInfo.PSN = extSuitProductInfo.PSN;
            orderProductInfo.CateId = extSuitProductInfo.CateId;
            orderProductInfo.BrandId = extSuitProductInfo.BrandId;
            orderProductInfo.StoreId = extSuitProductInfo.StoreId;
            orderProductInfo.StoreCid = extSuitProductInfo.StoreCid;
            orderProductInfo.StoreSTid = extSuitProductInfo.StoreSTid;
            orderProductInfo.Name = extSuitProductInfo.Name;
            orderProductInfo.DiscountPrice = extSuitProductInfo.ShopPrice;
            orderProductInfo.ShopPrice = extSuitProductInfo.ShopPrice;
            orderProductInfo.MarketPrice = extSuitProductInfo.MarketPrice;
            orderProductInfo.CostPrice = extSuitProductInfo.CostPrice;
            orderProductInfo.Weight = extSuitProductInfo.Weight;
            orderProductInfo.ShowImg = extSuitProductInfo.ShowImg;
            return orderProductInfo;
        }

        /// <summary>
        /// 初始化订单商品
        /// </summary>
        private static void InitOrderProduct(OrderProductInfo orderProductInfo, int buyCount, string sid, int uid, DateTime buyTime)
        {
            if (uid > 0)
                orderProductInfo.Sid = "";
            else
                orderProductInfo.Sid = sid;
            orderProductInfo.Uid = uid;
            orderProductInfo.RealCount = buyCount;
            orderProductInfo.BuyCount = buyCount;
            orderProductInfo.AddTime = buyTime;
        }
    }
}
