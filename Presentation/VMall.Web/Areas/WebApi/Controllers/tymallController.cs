using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Areas.WebApi.Models;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Routing;


namespace VMall.Web.Areas.WebApi.Controllers
{
    public class tymallController : BaseApiController
    {

        private static IOrderStrategy _iorderstrategy = BMAOrder.Instance;//订单策略
        private static string appKey = WebSiteConfig.TYappkey;

        public ActionResult GetOrder()
        {
            //第一步进行签名验证 验证不通过不能进行后续操作
            //osn,username, orderstate,productamount,orderamount, surplusmoney，paytype, regionid, consignee, mobile, phone, email, address, weight, buyerremark orders

            StringBuilder sb = new StringBuilder();
            //            {
            //        "result": "0",
            //        "msg":"签名错误",
            //        "data":"{}"
            //}  
            //后台已经有此订单 则不用重新插入 直接跳转到支付接口

            sb.Append("{");
            PartUserInfo partUserInfo = new PartUserInfo();
            List<OrderProductInfo> orderProductList = new List<OrderProductInfo>();

            FullShipAddressInfo fullShipAddressInfo = new FullShipAddressInfo();
            PluginInfo payPluginInfo = new PluginInfo();

            string buyerRemark = string.Empty;

            string ip = string.Empty;
            int uid = 0;
            string osn = Request["osn"];
            string username = Request["username"];
            uid = Users.GetUidByUserName(username);
            string orderstate = Request["orderstate"];
            decimal productamount = Convert.ToDecimal(Request["productamount"]);
            string orderamount = Request["orderamount"];
            string surplusmoney = Request["surplusmoney"] == null ? "" : Request["surplusmoney"];
            string paytype = Request["paytype"];
            //string regionid = Request["regionid"];  provinc  city  region
            string provinc = Request["provinc"];
            string city = Request["city"];
            string region = Request["region"];
            string consignee = Request["consignee"];
            string mobile = Request["mobile"];
            string phone = Request["phone"] == null ? "" : Request["phone"];
            string email = Request["email"] == null ? "" : Request["email"];
            string address = Request["address"];
            string buyerremark = Request["buyerremark"] == null ? "" : Request["buyerremark"];
            string orders = Request["orders"];
            string isok = Request["isok"];

            //NameValueCollection nvc = Request.Form;
            //if (nvc.Count != 0)
            //{
            //    for (int i = 0; i < nvc.Count; i++)
            //    {
            //        txtParams.Add(nvc.GetKey(i), nvc.GetValues(i)[0]);
            //    }
            //}

            //osn,username, orderstate,productamount,orderamount, surplusmoney，paytype, provinc + city + region + address, consignee, mobile, phone, email, , buyerremark orders

            string md5Str = osn + username + appKey;
            string sign = SecureHelper.MD5(md5Str);

            //LogHelper.WriteOperateLog("MD5Log", "请求MD5log记录", "需要加密字符串：" + md5Str + "\r\n传输加密sign:" + isok + "\r\n服务器加密sign:" + sign + "\r\n url解码字符串" + test1 + "\r\nurlencodesing:" + test2 + "\r\n utf8字符串：" + utf8_string + "\r\n utf8sing:" + test3);

            //LogHelper.WriteOperateLog("MD5Log", "请求MD5log记录", "需要加密字符串：" + md5Str + "\r\n传输加密sign:" + isok + "\r\n服务器加密sign:" + sign + "\r\n订单产品" + orders);
            if (isok != sign)
            {
                return WebApiResult(false, "签名错误");
            }

            OrderInfo order = Orders.GetOrderByOSN(osn);
            if (order != null)
            {
                Server.Transfer("/payment/Error.aspx");

                return WebApiResult(false, "此订单号已经存在，请勿重复提交");
            }
            if (!string.IsNullOrEmpty(orders))
            {
                JObject jsonObject = (JObject)JsonConvert.DeserializeObject(orders);
                JToken token = (JToken)jsonObject.SelectToken("ordersproduct");
                foreach (JObject p in token)
                {
                    //{"pid":"123","pname":"山寨鞋子1","showimg":"3.jpg","buycount":"2","marketprice":"200.00","shopprice ":"199.00"}
                    OrderProductInfo opInfo = new OrderProductInfo();
                    opInfo.Pid = TypeHelper.ObjectToInt(p["pid"]);
                    opInfo.Name = p["pname"].ToString();
                    opInfo.ShowImg = p["showimg"].ToString();
                    opInfo.BuyCount = TypeHelper.ObjectToInt(p["buycount"]);
                    opInfo.MarketPrice = Convert.ToDecimal(p["marketprice"]);
                    opInfo.ShopPrice = Convert.ToDecimal(p["shopprice"]);
                    opInfo.Uid = uid;

                    orderProductList.Add(opInfo);
                }
                //jsonMap.Concat((Dictionary<String, String>)JsonConvert.DeserializeObject(reqData));
            }
            //orderProdList oprodList =JsonToObject(orders);

            //orderProductList = oprodList.orderProductList;
            ////验证支付方式是否为空
            //    PluginInfo payPluginInfo = Plugins.GetPayPluginBySystemName(payName);
            //    if (payPluginInfo == null)
            //        return AjaxResult("empaypay", "请选择支付方式");

            fullShipAddressInfo.Address = address;
            fullShipAddressInfo.Email = email;
            fullShipAddressInfo.Consignee = consignee;
            fullShipAddressInfo.Mobile = mobile;

            fullShipAddressInfo.Phone = phone;

            //获取区域id根据省市区
            RegionInfo regioninfo = Regions.GetRegionByNameAndLayer(region, 3);
            if (regioninfo == null)
            {
                return WebApiResult(false, "区域不存在");
            }
            fullShipAddressInfo.RegionId = regioninfo.RegionId;

            string oidList = "";
            OrderInfo orderInfo = CreateOrderFromOut(osn, uid, 0, productamount, orderProductList, fullShipAddressInfo, payPluginInfo, buyerremark, ip);
            if (orderInfo != null)
            {
                oidList += orderInfo.Oid;

                //创建订单处理
                OrderActions.CreateOrderAction(new OrderActionInfo()
                {
                    Oid = orderInfo.Oid,
                    Uid = uid,
                    RealName = "天鹰接口创建订单",
                    ActionType = (int)OrderActionType.Submit,
                    ActionTime = DateTime.Now,
                    ActionDes = orderInfo.OrderState == (int)OrderState.WaitPaying ? "天鹰商城系统提交了订单，等待付款" : "天鹰商城系统提交了订单，请等待系统确认"
                });
                //Server.Transfer("/payment/Error.aspx");
                //创建订单成功后自动跳转支付
                Server.Transfer("/payment/CreateWebOrder.aspx?oidList=" + oidList);
                return WebApiResult(true, "提交成功");
            }
            else
            {
                Server.Transfer("/payment/Error.aspx");
                return WebApiResult(false, "提交失败");

            }
        }

        /// <summary>
        /// 外部调用支付接口
        /// </summary>
        /// <param name="ordercode"></param>
        public void outpay(string ordercode)
        {
            OrderInfo order = Orders.GetOrderByOSN(ordercode);
            if (order == null)
            {
                Server.Transfer("/payment/Error.aspx");
            }
            Server.Transfer("/payment/CreateWebOrder.aspx?oidList=" + order.Oid);

        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="partUserInfo">用户信息</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <param name="fullShipAddressInfo">配送地址</param>
        /// <param name="payPluginInfo">支付方式</param>
        /// <param name="buyerRemark">买家备注</param>
        /// <param name="ip">ip地址</param>
        /// <returns>订单信息</returns>
        public OrderInfo CreateOrderFromOut(string osn, int uid, int weight, decimal productAmount, List<OrderProductInfo> orderProductList, FullShipAddressInfo fullShipAddressInfo, PluginInfo payPluginInfo, string buyerRemark, string ip)
        {
            DateTime nowTime = DateTime.Now;
            //IPayPlugin payPlugin = (IPayPlugin)payPluginInfo.Instance;

            OrderInfo orderInfo = new OrderInfo();

            orderInfo.OSN = osn;
            orderInfo.Uid = uid;
            orderInfo.Weight = weight;
            orderInfo.ProductAmount = productAmount;

            //orderInfo.ShipFee = GetShipFee(fullShipAddressInfo.ProvinceId, fullShipAddressInfo.CityId, orderProductList);
            orderInfo.PayFee = 0M;
            orderInfo.OrderAmount = orderInfo.ProductAmount - orderInfo.FullCut + orderInfo.ShipFee + orderInfo.PayFee;

            //decimal payCreditMoney = Credits.PayCreditsToMoney(payCreditCount);
            //if (orderInfo.OrderAmount >= payCreditMoney)
            //{
            //    orderInfo.PayCreditCount = payCreditCount;
            //    orderInfo.PayCreditMoney = payCreditMoney;
            //    payCreditCount = 0;
            //}
            //else
            //{
            //    int orderPayCredits = Credits.MoneyToPayCredits(orderInfo.OrderAmount);
            //    orderInfo.PayCreditCount = orderPayCredits;
            //    orderInfo.PayCreditMoney = orderInfo.OrderAmount;
            //    payCreditCount = payCreditCount - orderPayCredits;
            //}

            orderInfo.CouponMoney = 0;
            orderInfo.SurplusMoney = orderInfo.OrderAmount - orderInfo.PayCreditMoney - orderInfo.CouponMoney;

            orderInfo.OrderState = (int)OrderState.WaitPaying;

            orderInfo.ParentId = 0;
            orderInfo.IsReview = 0;
            orderInfo.AddTime = nowTime;
            orderInfo.StoreId = 0;
            orderInfo.StoreName = "";
            orderInfo.PaySystemName = "LaKaLa";//payPluginInfo.SystemName;
            orderInfo.PayFriendName = "拉卡拉支付";//payPluginInfo.FriendlyName;
            orderInfo.PayMode = 1; //payPlugin.PayMode;

            orderInfo.RegionId = fullShipAddressInfo.RegionId;
            orderInfo.Consignee = fullShipAddressInfo.Consignee;
            orderInfo.Mobile = fullShipAddressInfo.Mobile;
            orderInfo.Phone = fullShipAddressInfo.Phone;
            orderInfo.Email = fullShipAddressInfo.Email;
            //orderInfo.ZipCode = fullShipAddressInfo.ZipCode;
            orderInfo.Address = fullShipAddressInfo.Address;
            //orderInfo.BestTime = bestTime;

            orderInfo.BuyerRemark = buyerRemark;
            orderInfo.IP = ip;
            orderInfo.OrderSource = (int)OrderSource.天鹰网;
            try
            {
                //添加订单
                int oid = _iorderstrategy.CreateOrder(orderInfo, false, orderProductList);
                if (oid > 0)
                {
                    orderInfo.Oid = oid;
                    return orderInfo;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return null;
        }

        /// <summary>
        /// 订单退货申请
        /// 退货先申请退货   将退货记录插入退款表 状态为未操作 等收到退货后由管理员确认退货并退款
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderRetrunApply()
        {
            int uid = 0;
            string username = Request["username"];
            uid = Users.GetUidByUserName(username);
            if (uid < 1)
            {
                return WebApiResult(false, "用户不存在");
            }
            string osn = Request["osn"];
            string isok = Request["isok"];
            string md5Str = osn + username + appKey;
            string sign = SecureHelper.MD5(md5Str);
            if (isok != sign)
            {
                return WebApiResult(false, "签名错误");
            }
            OrderInfo orderInfo = AdminOrders.GetOrderByOSN(osn);
            if (orderInfo == null)
                return WebApiResult(false, "订单不存在");
            if (orderInfo.Uid != uid)
            {
                return WebApiResult(false, "订单不存在");
            }
            if (orderInfo.OrderState != (int)OrderState.Sended && orderInfo.OrderState != (int)OrderState.Completed)
                return WebApiResult(false, "订单当前不能退货");

            PartUserInfo partUserInfo = Users.GetPartUserById(orderInfo.Uid);
            AdminOrders.ReturnOrder(ref partUserInfo, orderInfo, uid, DateTime.Now);
            OrderActions.CreateOrderAction(new OrderActionInfo()
            {
                Oid = orderInfo.Oid,
                Uid = uid,
                RealName = AdminUsers.GetUserDetailById(uid).RealName,
                ActionType = (int)OrderActionType.Return,
                ActionTime = DateTime.Now,
                ActionDes = "订单已申请退货,请等待系统处理"
            });

            return WebApiResult(true, "退货申请成功,请等待系统处理");
        }

        /// <summary>
        /// 确认退货 在线支付原方式调用支付平台退款接口退款  线下支付人工退款后确定改变状态
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmReturn()
        {
            int uid = 0;
            string username = Request["username"];
            uid = Users.GetUidByUserName(username);
            if (uid < 1)
            {
                return WebApiResult(false, "用户不存在");
            }
            string osn = Request["osn"];
            string isok = Request["isok"];
            string md5Str = osn + username + appKey;
            string sign = SecureHelper.MD5(md5Str);
            if (isok != sign)
            {
                return WebApiResult(false, "签名错误");
            }
            OrderInfo orderInfo = AdminOrders.GetOrderByOSN(osn);
            if (orderInfo == null)
                return WebApiResult(false, "订单不存在");
            if (orderInfo.Uid != uid) {
                return WebApiResult(false, "订单不存在");
            }
            if (orderInfo.OrderState != (int)OrderState.Sended && orderInfo.OrderState != (int)OrderState.Completed && AdminOrderRefunds.GetRefundInfoByOSN(osn) == null)
                return WebApiResult(false, "订单当前不能退款");
            //ViewResult result=(ViewResult)lakalaReFund(orderInfo.Oid);new LaKaLaController().
            return RedirectToAction("ReFund", "LaKaLa", new RouteValueDictionary { { "oid", orderInfo.Oid } });
            //return WebApiResult(true, "退款成功", result);
        }

        /// <summary>
        /// 解析Json数据转换成List集合
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        //private orderProdList JsonToObject(string Content)
        //{
        //    if (string.IsNullOrEmpty(Content))
        //    {
        //        return new orderProdList();
        //    }
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    return serializer.Deserialize<orderProdList>(Content);
        //}

        /// <summary>
        /// ajax请求结果
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="content">内容</param>
        /// <param name="isObject">是否为对象</param>
        /// <returns></returns>
        //public ActionResult ApiResult(string result, string msg, string data, bool isObject)
        //{
        //    //return Content(string.Format("{\"result\":\"{0}\",\"msg\":\"{1}\",\"data\":\"{2}\"}", result, msg, data));
        //    return Content("{\"result\":\"'" + result + "'\",\"msg\":\"'" + msg + "'\",\"data\":\"'" + data + "'\"}");

        //}

        //public static string AppResult(this Controller controller, bool result, string msg, object data)
        //{
        //    return new JavaScriptSerializer().Serialize(new
        //    {
        //        result = result ? 0 : -1,
        //        msg = msg,
        //        info = data
        //    });
        //}

    }
    /// <summary>
    /// 
    /// </summary>
    public class orderProdList
    {
        public List<OrderProductInfo> orderProductList { get; set; }
    }
    /// <summary>
    /// 订单产品类
    /// {"pid":"123","pname":"山寨鞋子1","showimg":"3.jpg","buycount":"2","marketprice":"200.00","shopprice ":"199.00"}
    /// </summary>
    public class orderProduct
    {
        public int pid { get; set; }
        public string pname { get; set; }
        public string showmimg { get; set; }
        public int buycount { get; set; }
        public decimal marketprice { get; set; }
        public decimal shopprice { get; set; }

    }
}
