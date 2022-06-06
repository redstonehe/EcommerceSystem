using System;
using System.Web.Mvc;
using System.Linq;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Models;
using System.Collections.Generic;

using System.Security.Cryptography;
using System.Web.Security;
using System.Text;

using System.Drawing;
using System.Data;
using System.Web;
namespace VMall.Web.Controllers
{
    /// <summary>
    /// 测试控制器类
    /// </summary>
    public partial class TestController : BaseWebController
    {
        public static APIClient client = new APIClient(WebHelper.GetConfigSettings("DirsaleApiUrl"), "666999", "87365A9BA9AE4808A69FCF10A82BD8EE");

        public ActionResult Index()
        {
            return View();
        }

        #region 启德直销系统测试
        /// <summary>
        /// 订单推送1
        /// </summary>
        /// <returns></returns>
        public ActionResult AddOrder_QD()
        {
            int oid = WebHelper.GetFormInt("oid");
            int uid = WebHelper.GetFormInt("uid");
            int PhasedType = WebHelper.GetFormInt("PhasedType");

            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            PartUserInfo userInfo = Users.GetPartUserById(uid);
            bool flag = OrderUtils.CreateQDOrder(orderInfo, userInfo, PhasedType);
            return Content(flag.ToString());

        }
        /// <summary>
        /// 订单取消
        /// </summary>
        /// <returns></returns>
        public ActionResult QXOrder_QD()
        {
            int oid = WebHelper.GetFormInt("oid2");
            int uid = WebHelper.GetFormInt("uid2");

            OrderInfo orderInfo = Orders.GetOrderByOid(oid);
            PartUserInfo userInfo = Users.GetPartUserById(uid);
            bool flag = OrderUtils.CancelQDOrder(orderInfo, userInfo, "商城系统取消订单");
            return Content(flag.ToString());

        }
        /// <summary>
        /// 订单推送2
        /// </summary>
        /// <returns></returns>
        //public ActionResult AddOrder_QD_Code()
        //{
        //    int oid = WebHelper.GetFormInt("oid");
        //    int dirsaleuid = WebHelper.GetFormInt("dirsaleuid");
        //    int PhasedType = WebHelper.GetFormInt("PhasedType");

        //    OrderInfo orderInfo = Orders.GetOrderByOid(oid);
        //    //PartUserInfo userInfo = Users.GetPartUserById(uid);
        //    try
        //    {
        //        //isPhased，如果直销会员是新会员，传3，如果直销会员是老会员且没有选择分期，传1，如果直销会员是老会员且选择分期，传2
        //        int isPhased = 0;
        //        string[] details = AccountUtils.GetUserDetailsForOrder(dirsaleuid);
        //        if (details.Length < 1)
        //            return Content("");
        //        if (details[1] == "1")
        //            isPhased = 3;
        //        else
        //            isPhased = PhasedType;
        //        int orderType = 1;

        //        RegionInfo regionInfo = Regions.GetRegionById(orderInfo.RegionId);
        //        string DirsaleUid = dirsaleuid.ToString();

        //        List<OrderProductInfo> orderProductList = Orders.GetOrderProductList(orderInfo.Oid);
        //        string proData = OrderUtils.getOrderProductStr(orderProductList);
        //        decimal orderPV = orderInfo.CashDiscount > 0 ? 0 : orderProductList.Sum(x => x.BuyCount * x.ProductPV);
        //        APIDictionary Params = new APIDictionary();
        //        Params.Add("userCode", dirsaleuid);
        //        Params.Add("station", "");
        //        Params.Add("orderType", orderType);
        //        Params.Add("isPhased", isPhased);
        //        Params.Add("payTime", orderInfo.PayTime);
        //        Params.Add("payUser", dirsaleuid);
        //        Params.Add("amount", orderInfo.OrderAmount);
        //        Params.Add("score", orderPV);
        //        Params.Add("cash", orderInfo.SurplusMoney);
        //        Params.Add("emoney", orderInfo.OrderAmount - orderInfo.SurplusMoney);
        //        Params.Add("province", regionInfo.ProvinceName);
        //        Params.Add("city", regionInfo.CityName);
        //        Params.Add("area", regionInfo.Name);
        //        Params.Add("address", orderInfo.Address);
        //        Params.Add("postCode", orderInfo.ZipCode);
        //        Params.Add("consignee", orderInfo.Consignee);
        //        Params.Add("phone", orderInfo.Mobile);
        //        Params.Add("proData", proData);
        //        Params.Add("orderCode", orderInfo.OSN);
        //        string FromDirSale = client.Execute(Params, "/api/order/createOrder");
        //        JObject jsonObject2 = (JObject)JsonConvert.DeserializeObject(FromDirSale);
        //        JToken token2 = (JToken)jsonObject2;

        //        //传递成功后需要在汇购中标识已经传递成功的状态
        //        if (token2["Result"].ToString() == "0")
        //        {
        //            LogHelper.WriteOperateLog("SuccessCreateQDOrder", "添加已支付订单（直销系统）", "成功信息为：返回订单ID：" + token2["Info"].SelectToken("OrderId").ToString() + "|订单号：" + token2["Info"].SelectToken("OrderNo").ToString() + ",汇购订单ID：" + orderInfo.Oid + ",直销会员Id：" + dirsaleuid);

        //            return Content("成功信息为：返回订单ID：" + token2["Info"].SelectToken("OrderId").ToString() + "|订单号：" + token2["Info"].SelectToken("OrderNo").ToString() + ",汇购订单ID：" + orderInfo.Oid + ",直销会员Id：" + dirsaleuid);
        //        }
        //        else
        //        {
        //            LogHelper.WriteOperateLog("FailCreateQDOrder", "添加已支付订单（直销系统）", "错误信息为：订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|失败原因：" + token2["Msg"].ToString() + ",直销会员Id：" + dirsaleuid);
        //            return Content("错误信息为：订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|失败原因：" + token2["Msg"].ToString() + ",直销会员Id：" + dirsaleuid);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.WriteOperateLog("FailCreateQDOrder", "添加已支付订单（直销系统）", "错误信息为：订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|失败原因,请求异常：" + ex.Message);
        //        return Content("错误信息为：订单id：" + orderInfo.Oid + "|订单号：" + orderInfo.OSN + "|失败原因,请求异常：" + ex.Message);
        //    }
        //    //return Content("");

        //}
        /// <summary>
        /// API测试
        /// </summary>
        /// <returns></returns>
        public ActionResult testlogin(string userName, string password)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("userName", userName);
            Params.Add("password", password);
            string FromDirSale = client.Execute(Params, "/api/User/UserLogin");
            return Content(FromDirSale);
        }
        /// <summary>
        /// API测试-用户是否存在
        /// </summary>
        /// <returns></returns>
        public ActionResult testUserExist(string username)
        {
            APIDictionary Params = new APIDictionary();
            //Params.Add("subUrl", "/api/User/IsUserExist");
            Params.Add("userName", username);
            
            //LogHelper.WriteOperateLog("API参数", "获得会员在直销系统中的重消PV数", "接口调用返回错误：" + ex.Message + "|DirUid：" + DSUid + "|curDate：" + curDate);
            
            string FromDirSale = client.Execute(Params, "/api/User/IsUserExist");
            return Content(FromDirSale);
        }

        /// <summary>
        /// API测试-通过名称获取用户id
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserIdByName(string username)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("userName", username);
            string FromDirSale = client.Execute(Params, "/api/User/GetUserIdByName");
            return Content(FromDirSale);
        }

        /// <summary>
        /// API测试-获取用户名
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserCode(int userId)
        {
            string FromDirSale = AccountUtils.GetUserCode(userId);
            return Content(FromDirSale);
        }
        /// <summary>
        /// API测试-获取用户名
        /// </summary>
        /// <returns></returns>
        public ActionResult t_login(string accountName, string password)
        {
            string FromDirSale = AccountUtils.UserLogin(accountName, password);
            return Content(FromDirSale);
        }
        /// <summary>
        /// API测试-pv查询
        /// </summary>
        /// <returns></returns>
        public ActionResult testPV(int DSUid, string curDate)
        {
            APIDictionary Params = new APIDictionary();
            Params.Add("userId", DSUid);
            Params.Add("curDate", curDate);
            string FromDirSale = client.Execute(Params, "/api/User/GetUserReorderPV");
            return Content(FromDirSale);
        }

        /// <summary>
        /// API测试-更新帐号
        /// </summary>
        /// <returns></returns>
        //public ActionResult testUpdateAccount(int DSUid)
        //{
        //    //decimal amount=
        //    // bool FromDirSale = false;// AccountUtils.UpdateAccountForDir(DSUid, (int)AccountType.海米账户, 173.4M, 0, "1120170226192929767582", "结算订单:1120170226192929767582,支出/收入金额:173.4");
        //    //bool FromDirSale = AccountUtils.UpdateAccountForDir(DSUid, (int)AccountType.海米账户, 5000, 0, "1111111111111111111", "增加测试金额,支出/收入金额:5000");
        //    //bool FromDirSale1 = AccountUtils.UpdateAccountForDir(DSUid, (int)AccountType.红包账户, 8000, 0, "2222222222222222222", "增加测试金额,支出/收入金额:8000");
        //    bool FromDirSale = AccountUtils.UpdateAccountForDir(DSUid, (int)AccountType.海米账户, 35272.29M, 0, "", "海米账户转入锁定余额，金额：35272.29");
        //    return Content(FromDirSale.ToString());
        //}

        /// <summary>
        /// API测试--海米结算
        /// </summary>
        /// <returns></returns>
        //public ActionResult HMJS(int userId)
        //{
        //    APIDictionary Params = new APIDictionary();
        //    Params.Add("userId", userId);
        //    Params.Add("haimiAmount", 10);
        //    Params.Add("orderState", 140);
        //    Params.Add("orderCode", "1120170320142920575483");
        //    Params.Add("settleState", 0);
        //    Params.Add("settleLevel", 3);

        //    string FromDirSale = client.Execute(Params, "/api/user/HaimiDistribute");
        //    return Content(FromDirSale);
        //}

        /// <summary>
        /// API测试-注销帐号
        /// </summary>
        /// <returns></returns>
        //public ActionResult extuser(int userid)
        //{
        //    APIDictionary Params = new APIDictionary();
        //    Params.Add("userId", userid);
        //    string FromDirSale = client.Execute(Params, "/api/User/exitUser");
        //    return Content(FromDirSale);
        //}
        /// <summary>
        /// API测试-支付密码
        /// </summary>
        /// <returns></returns>
        public ActionResult testpaywsd(int dsuid)
        {
            string FromDirSale = OrderUtils.GetPayPassword("", dsuid);
            string psd = SecureHelper.DecryptString(FromDirSale, DirSaleUserInfo.EncryptKey);
            return Content(psd);
        }

        #endregion


        #region 商城测试


        /// <summary>
        /// 测试获得有效代理上级
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult GetParent(int uid, int atype = 1)
        {
            PartUserInfo user = Users.GetPartUserById(uid);
            //user.AgentType = atype;
            PartUserInfo availParentUser = Users.GetParentUserForAgent(user);

            return Content("有效上级id：" + availParentUser.Uid.ToString());
        }
        /// <summary>
        /// 测试获得有效代理库存上级
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult GetStockParent(int uid, int atype = 1)
        {
            PartUserInfo user = Users.GetPartUserById(uid);
            //user.AgentType = atype;
            PartUserInfo availParentUser = Users.GetParentUserForAgentStock(user);

            return Content("有效代理库存上级id：" + availParentUser.Uid.ToString());
        }
        /// <summary>
        /// 测试获得有效代理上级
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ActionResult GetDSParent(int uid)
        {
            PartUserInfo user = Users.GetPartUserById(uid);
            PartUserInfo availParentUser = AccountUtils.GetParentUserForDirSale(user);

            return Content("有效上级id：" + availParentUser.Uid.ToString());
        }
        #endregion


        #region 新微商制度
        public ActionResult T_agent()
        {
            MemberInfo member = new MemberInfo()
            {
                ParentCode = "CHN88888",
                ManagerCode = "CHN100000",
                userId = 100000
            };
            return View("/Views/ucenter/agentresult.cshtml", member);

        }

        public ActionResult TT_agent()
        {
            int joinuid = 437;
            MemberInfo member = new MemberInfo()
            {
                ParentCode = "CHN88888",
                ManagerCode = "CHN100000",
                userId = 100000
            };
            return RedirectToAction("agentresult", "UCenter", new { ParentCode = member.ParentCode, ManagerCode = member.ManagerCode, joinuid = joinuid });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public ActionResult batUpDSAgentType()
        //{
        //    int i = 0;
        //    string sqlStr = string.Format(@"  isdirsaleuser=1 and  dirsaleuid>0  ");//直销帐号对应微商级别，//4白金卡=VIP3,3金卡=星级经销商2，2银卡=事业伙伴1
        //    List<PartUserInfo> list = Users.GetPartUserInfoByStrWhere(sqlStr);

        //    foreach (var item in list)
        //    {
        //        if (item.AgentType <= 0)
        //        {
        //            string[] result = AccountUtils.GetUserDetailsForOrder(item.DirSaleUid);
        //            int Rank = TypeHelper.StringToInt(result[2]);
        //            int AgentType = 0;
        //            if (Rank == 4)//4白金卡=VIP3,3金卡=星级经销商2，2银卡=事业伙伴1
        //                AgentType = 3;
        //            else if (Rank == 3)
        //                AgentType = 2;
        //            else if (Rank == 2)
        //                AgentType = 1;
        //            else
        //                AgentType = 0;
        //            if (result != null && result.Length > 0 && Rank > 0)
        //            {
        //                if (Users.UpdateDSUserAGTypeByUid(item.Uid, AgentType))
        //                {
        //                    LogHelper.WriteOperateLog("BatchUpDSAgentType", "批量更新直销会员代理等级", "成功信息为：会员ID：" + item.Uid + ",直销会员ID：" + item.DirSaleUid + ",级别代码：" + Rank + ",代理等级:" + AgentType);
        //                    i++;
        //                }
        //            }
        //        }
        //    }

        //    return Content("更新数据成功，记录数：" + i.ToString());

        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public ActionResult batAddExistAgentUser()
        //{
        //    //33367,34298,35458,35459,35460
        //    int i = 0;
        //    string sqlStr = string.Format(@"  a.uid in (33367,34298,35458,35459,35460) ");
        //    //string sqlStr = string.Format(@"  a.uid in (436) ");
        //    List<UserInfo> list = Users.GetUserInfoByStrWhere(sqlStr);

        //    foreach (var item in list)
        //    {
        //        PartUserInfo user = Users.GetPartUserById(item.Uid);
        //        //item.Password
        //        if (!user.IsDirSaleUser)
        //        {
        //            string parentCode = WebHelper.GetFormString("parentCode");
        //            string managerCode = "";
        //            int placeSide = 1;
        //            string userPhone = item.Mobile;
        //            string realName = item.RealName;
        //            string userCard = item.IdCard;
        //            MemberInfo member = AccountUtils.CreateMember(user, realName, managerCode, placeSide, userCard, userPhone);

        //            LogHelper.WriteOperateLog("batAddExistAgentUser", "批量更新已存在代理会员", "成功信息为：会员ID：" + item.Uid + ",直销会员ID：" + member.userId + ",推荐人：" + member.ParentCode + ",安置人:" + member.ManagerCode);
        //            i++;
        //        }
        //    }

        //    return Content("更新数据成功，记录数：" + i.ToString());


        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public ActionResult singleAddExistAgentUser(int uid, string managerName)
        //{
        //    PartUserInfo partuser = Users.GetPartUserById(uid);
        //    UserInfo user = Users.GetUserById(uid);
        //    user.DirSalePwd = partuser.DirSalePwd;
        //    MemberInfo member = new MemberInfo();
        //    if (!user.IsDirSaleUser)
        //    {
        //        string parentCode = WebHelper.GetFormString("parentCode");
        //        string managerCode = managerName;
        //        int placeSide = 1;
        //        string userPhone = user.Mobile;
        //        string realName = user.RealName;
        //        string userCard = user.IdCard;
        //        member = AccountUtils.CreateMember(user, realName, managerCode, placeSide, userCard, userPhone);

        //        LogHelper.WriteOperateLog("singleAddExistAgentUser", "单个更新已存在代理会员", "成功信息为：会员ID：" + user.Uid + ",直销会员ID：" + member.userId + ",推荐人：" + member.ParentCode + ",安置人:" + member.ManagerCode);

        //    }
        //    return Content("更新数据成功，成功信息为：会员ID：" + user.Uid + ",直销会员ID：" + member.userId + ",推荐人：" + member.ParentCode + ",安置人:" + member.ManagerCode);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        //public ActionResult updateParentIds(int oid)
        //{
        //    int fromparentid1 = 0; //int 事业伙伴出货uid
        //    int fromparentid2 = 0; //int 星级出货uid
        //    int fromparentid3 = 0; // int VIP出货id
        //    int fromparentid4 = 0;  //大区出货id 

        //    PartUserInfo Parent1 = new PartUserInfo();//事业伙伴
        //    PartUserInfo Parent2 = new PartUserInfo();//星级
        //    PartUserInfo Parent3 = new PartUserInfo();//VIP
        //    PartUserInfo Parent4 = new PartUserInfo();//大区

        //    OrderInfo orderInfo = Orders.GetOrderByOid(oid);
        //    PartUserInfo currentuser = Users.GetPartUserById(orderInfo.Uid);
        //    PartUserInfo parentUser = Users.GetParentUserForAgentStock(currentuser);
        //    if (currentuser.AgentType == 4)
        //    {
        //    }
        //    if (currentuser.AgentType == 3) //VIP往上找大区，找不到大区或大区没货在公司拿库存
        //    {
        //        if (parentUser.AgentType == 4)
        //        {
        //            Parent4 = Users.GetParentUserForAgentStock(currentuser);
        //            fromparentid4 = Parent4.Uid;
        //        }
        //    }
        //    if (currentuser.AgentType == 2) //星级往上找VIP，再找大区，找不到大区或大区没货在公司拿库存
        //    {
        //        if (parentUser.AgentType == 4)
        //        {
        //            Parent4 = Users.GetParentUserForAgentStock(currentuser);
        //            fromparentid4 = Parent4.Uid;
        //        }
        //        if (parentUser.AgentType == 3)
        //        {
        //            Parent3 = Users.GetParentUserForAgentStock(currentuser);
        //            fromparentid3 = Parent3.Uid;
        //            if (Parent3.Uid > 0)
        //            {
        //                Parent4 = Users.GetParentUserForAgentStock(Parent3);
        //                fromparentid4 = Parent4.Uid;
        //            }
        //        }
        //    }
        //    if (currentuser.AgentType == 1) //事业伙伴往上找星级，再找VIP，再找大区，找不到大区或大区没货在公司拿库存
        //    {
        //        if (parentUser.AgentType == 4)
        //        {
        //            Parent4 = Users.GetParentUserForAgentStock(currentuser);
        //            fromparentid4 = Parent4.Uid;
        //        }
        //        if (parentUser.AgentType == 3)
        //        {
        //            Parent3 = Users.GetParentUserForAgentStock(currentuser);
        //            fromparentid3 = Parent3.Uid;
        //            if (Parent3.Uid > 0)
        //            {
        //                Parent4 = Users.GetParentUserForAgentStock(Parent3);
        //                fromparentid4 = Parent4.Uid;
        //            }
        //        }
        //        if (parentUser.AgentType == 2)
        //        {
        //            Parent2 = Users.GetParentUserForAgentStock(currentuser);
        //            fromparentid2 = Parent2.Uid;
        //            if (Parent2.Uid > 0)
        //            {
        //                Parent3 = Users.GetParentUserForAgentStock(Parent2);
        //                fromparentid3 = Parent3.Uid;
        //                if (Parent3.Uid > 0)
        //                {
        //                    Parent4 = Users.GetParentUserForAgentStock(Parent3);
        //                    fromparentid4 = Parent4.Uid;
        //                }
        //            }
        //        }
        //    }
        //    if (currentuser.AgentType == 0)//个人零售 
        //    {
        //        if (parentUser.AgentType == 4)
        //        {
        //            Parent4 = Users.GetParentUserForAgentStock(currentuser);
        //            fromparentid4 = Parent4.Uid;
        //        }
        //        if (parentUser.AgentType == 3)
        //        {
        //            Parent3 = Users.GetParentUserForAgentStock(currentuser);
        //            fromparentid3 = Parent3.Uid;
        //            if (Parent3.Uid > 0)
        //            {
        //                Parent4 = Users.GetParentUserForAgentStock(Parent3);
        //                fromparentid4 = Parent4.Uid;
        //            }
        //        }
        //        if (parentUser.AgentType == 2)
        //        {
        //            Parent2 = Users.GetParentUserForAgentStock(currentuser);
        //            fromparentid2 = Parent2.Uid;
        //            if (Parent2.Uid > 0)
        //            {
        //                Parent3 = Users.GetParentUserForAgentStock(Parent2);
        //                fromparentid3 = Parent3.Uid;
        //                if (Parent3.Uid > 0)
        //                {
        //                    Parent4 = Users.GetParentUserForAgentStock(Parent3);
        //                    fromparentid4 = Parent4.Uid;
        //                }
        //            }
        //        }
        //        if (parentUser.AgentType == 1)
        //        {
        //            Parent1 = Users.GetParentUserForAgentStock(currentuser);
        //            fromparentid1 = Parent1.Uid;
        //            if (Parent1.Uid > 0)
        //            {
        //                Parent2 = Users.GetParentUserForAgentStock(Parent1);
        //                fromparentid2 = Parent2.Uid;
        //                if (Parent2.Uid > 0)
        //                {
        //                    Parent3 = Users.GetParentUserForAgentStock(Parent2);
        //                    fromparentid3 = Parent3.Uid;
        //                    if (Parent3.Uid > 0)
        //                    {
        //                        Parent4 = Users.GetParentUserForAgentStock(Parent3);
        //                        fromparentid4 = Parent4.Uid;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Content("更新数据成功，成功信息为：<br/>会员ID：" + currentuser.Uid + ",会员名:" + currentuser.UserName + ",会员手机:" + currentuser.Mobile + ",会员代理等级:" + currentuser.AgentType + ",订单ID:" + orderInfo.Oid + ",订单号:" + orderInfo.OSN + "<br/>事业伙伴-fromparentid1:" + fromparentid1 + ",星级-fromparentid2:" + fromparentid2 + ",VIP-fromparentid3:" + fromparentid3 + ",大区-fromparentid4:" + fromparentid4);
        //}
        #endregion

        #region excel

        public ActionResult BrowserT()
        {
            if (WebHelper.IsMobile() || WebHelper.IsWeChatBrowser())
                return Content("True");
            return Content("False");
        }

        public ActionResult excelT()
        {
            StringBuilder sb = new StringBuilder();
            HttpPostedFileBase file = Request.Files[0];
            string path = MallUtils.SaveOrderExcel(file);
            if (path == "-1" || path == "-2" || path == "-3")
                return Content(path);
            //ExcelHelper_NPOI excel_helper = new ExcelHelper_NPOI(AppDomain.CurrentDomain.BaseDirectory + "test.xlsx");
            ExcelHelper_NPOI excel_helper = new ExcelHelper_NPOI(path);
            DataTable dt = excel_helper.ExcelToDataTable("", true, 13);

            //List<string> tableList = GetColumnsByDataTable(dt);

            foreach (DataRow item in dt.Rows)
            {
                try
                {
                    sb.Append(item[2].ToString() + "\r\t");
                }
                catch (Exception ex)
                { }
            }
            return Content(sb.ToString());
        }

        /// <summary>
        /// 根据datatable获得列名
        /// </summary>
        /// <param name="dt">表对象</param>
        /// <returns>返回结果的数据列数组</returns>
        public static List<string> GetColumnsByDataTable(DataTable dt)
        {
            List<string> strColumns = new List<string>();

            if (dt.Columns.Count > 0)
            {
                int columnNum = 0;
                columnNum = dt.Columns.Count; ;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    strColumns.Add(dt.Columns[i].ColumnName);
                }
            }
            return strColumns;
        }
        #endregion

        //public ActionResult test9()
        //{
        //    OrderApply applyBll = new OrderApply();
        //    OrderApplyInfo info = new OrderApplyInfo();
        //    int i = applyBll.Add(info);
        //    return Content(i.ToString());
        //}

        public ActionResult testAdo()
        {
            Channel bll = new Channel();
            ChannelInfo info = bll.GetModel(1);
            return Content(info.Name + "-" + info.Description + "-" + info.CreationDate);
        }
        public ActionResult testAdo2()
        {
            SuitPromotions bll = new SuitPromotions();
            SuitPromotionInfo info = bll.GetModel(6);
            return Content(info.Name + "-" + info.StartTime1 + "-" + info.EndTime1);
        }
        public ActionResult testAdo3()
        {
            Channel bll = new Channel();
            ChannelInfo info = new ChannelInfo()
            {
                DisplayOrder = 0,
                Name ="测试添加专区666",
                State = 0,
                Description =  "",
                LinkType = 0,
                LinkUrl =  ""
            };

            new Channel().Add(info);
            return Content(info.Name + "-" +info.State);
        }


        /// <summary>
        /// 批量拉取直销会员 未登录的 默认密码 123456
        /// 20180611 已批量拉取到直销Userid 26236 之前的直销会员 
        /// </summary>
        /// <returns></returns>
        //public ActionResult GetDirSaleUserOnce()
        //{
        //    int i = 0;
        //    int j = 0;
        //    //int x = 0;//起始
        //    int y = 26236;//终止

        //    for (int x = 20001; x <= y; x++)
        //    {
        //        try
        //        {
        //            DSDetailInfo dsInfo = AccountUtils.GetUserDetailMsg(x);
        //            if (dsInfo.UserId >= 1) //有直销id返回才操作
        //            {
        //                PartUserInfo user = Users.GetPartUserInfoByDirSaleUid(dsInfo.UserId);
        //                if (user == null)//不存在该直销id才操作
        //                {
        //                    PartUserInfo usertme = Users.GetPartUserByName(dsInfo.UserCode);
        //                    if (usertme == null)//不存在相同用户名才操作(手机号可更改，不唯一）
        //                    {
        //                        #region 绑定用户信息

        //                        UserInfo userInfo = new UserInfo();

        //                        userInfo.UserName = dsInfo.UserCode;
        //                        userInfo.Email = string.Empty;
        //                        userInfo.Mobile = dsInfo.UserPhone;
        //                        string password = "123456";
        //                        userInfo.Salt = Randoms.CreateRandomValue(6);
        //                        userInfo.Password = Users.CreateUserPassword(password, userInfo.Salt);
        //                        userInfo.UserRid = UserRanks.GetLowestUserRank().UserRid;
        //                        userInfo.StoreId = 0;
        //                        userInfo.MallAGid = 1;//非管理员组
        //                        userInfo.NickName = dsInfo.NickName;
        //                        userInfo.Avatar = "";
        //                        userInfo.PayCredits = 0;
        //                        userInfo.RankCredits = 0;
        //                        userInfo.VerifyEmail = 0;
        //                        userInfo.VerifyMobile = 0;
        //                        userInfo.LastVisitIP = WorkContext.IP;
        //                        userInfo.LastVisitRgId = WorkContext.RegionId;
        //                        userInfo.LastVisitTime = DateTime.Now;
        //                        userInfo.RegisterIP = WorkContext.IP;
        //                        userInfo.RegisterRgId = WorkContext.RegionId;
        //                        userInfo.RegisterTime = dsInfo.RegisterDate;

        //                        userInfo.Gender = WebHelper.GetFormInt("gender");
        //                        userInfo.RealName = dsInfo.UserName;
        //                        userInfo.Bday = new DateTime(1900, 1, 1);
        //                        userInfo.IdCard = dsInfo.UserCard;
        //                        userInfo.RegionId = WebHelper.GetFormInt("regionId");
        //                        userInfo.Address = WebHelper.HtmlEncode(WebHelper.GetFormString("address"));
        //                        userInfo.Bio = WebHelper.HtmlEncode(WebHelper.GetFormString("bio"));

        //                        #endregion
        //                        PartUserInfo parentUser = Users.GetPartUserModelByStrWhere(string.Format(" isdirsaleuser=1 and username='{0}'  ", dsInfo.ParentCode));

        //                        userInfo.Pid = parentUser.Uid > 0 ? parentUser.DirSaleUid : 0;
        //                        userInfo.Ptype = (int)UserPanertType.DirSaleUser;

        //                        userInfo.IsDirSaleUser = true;
        //                        userInfo.DirSaleUid = dsInfo.UserId;
        //                        userInfo.MaxCashCount = 5;
        //                        int Rank = dsInfo.Rank;
        //                        if (Rank == 4)//4白金卡=VIP3,3金卡=星级经销商2，2银卡=事业伙伴1
        //                            userInfo.Ds2AgentRank = 3;
        //                        else if (Rank == 3)
        //                            userInfo.Ds2AgentRank = 2;
        //                        else if (Rank == 2)
        //                            userInfo.Ds2AgentRank = 1;
        //                        else
        //                            userInfo.Ds2AgentRank = 0;
        //                        //创建用户
        //                        userInfo.Uid = Users.CreateUser(userInfo);

        //                        if (string.IsNullOrEmpty(userInfo.DirSalePwd))
        //                        {
        //                            userInfo.DirSalePwd = SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);
        //                            Users.UpdatePartUserDirSalePwd(userInfo);
        //                        }

        //                        //添加用户失败
        //                        if (userInfo.Uid < 1)
        //                        {
        //                            LogHelper.WriteOperateLog("AddDirSaleUserFail", "拉取直销会员数据错误", "错误信息为：直销会员ID：" + dsInfo.UserId + ",手机号：" + dsInfo.UserPhone + ",用户名：" + dsInfo.UserCode);
        //                            j++;
        //                            continue;
        //                        }
        //                        else
        //                        {
        //                            LogHelper.WriteOperateLog("AddDirSaleUserFor1", "拉取直销会员数据", "成功信息为：会员ID：" + userInfo.Uid + ",直销会员ID：" + userInfo.DirSaleUid + ",手机号：" + userInfo.Mobile + ",用户名：" + userInfo.UserName);
        //                            i++;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LogHelper.WriteOperateLog("AddDirSaleUserError", "拉取直销会员数据异常", "错误信息为：" + ex.Message);
        //            continue;
        //        }

        //    }

        //    return Content("更新数据，成功记录数：" + i.ToString() + ",失败记录数" + j.ToString());
        //}

        /// <summary>
        /// 批量拉取直销会员 未登录的 默认密码 123456
        /// 20181106 批量拉取到直销Userid 26236 之后到 26896 的直销会员 
        /// 20181106 批量拉取到直销Userid 200001 之后到 200009 的直销会员 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDirSaleUserTwice()
        {
            int i = 0;
            int j = 0;
            //int x = 0;//起始
            int y = 200009;//终止

            for (int x = 200001; x <= y; x++)
            {
                try
                {
                    DSDetailInfo dsInfo = AccountUtils.GetUserDetailMsg(x);
                    if (dsInfo.UserId >= 1) //有直销id返回才操作
                    {
                        PartUserInfo user = Users.GetPartUserInfoByDirSaleUid(dsInfo.UserId);
                        if (user == null)//不存在该直销id才操作
                        {
                            PartUserInfo usertme = Users.GetPartUserByName(dsInfo.UserCode);
                            if (usertme == null)//不存在相同用户名才操作(手机号可更改，不唯一）
                            {
                                #region 绑定用户信息

                                UserInfo userInfo = new UserInfo();

                                userInfo.UserName = dsInfo.UserCode;
                                userInfo.Email = string.Empty;
                                userInfo.Mobile = dsInfo.UserPhone;
                                string password = "123456";
                                userInfo.Salt = Randoms.CreateRandomValue(6);
                                userInfo.Password = Users.CreateUserPassword(password, userInfo.Salt);
                                userInfo.UserRid = UserRanks.GetLowestUserRank().UserRid;
                                userInfo.StoreId = 0;
                                userInfo.MallAGid = 1;//非管理员组
                                userInfo.NickName = dsInfo.NickName;
                                userInfo.Avatar = "";
                                userInfo.PayCredits = 0;
                                userInfo.RankCredits = 0;
                                userInfo.VerifyEmail = 0;
                                userInfo.VerifyMobile = 0;
                                userInfo.LastVisitIP = WorkContext.IP;
                                userInfo.LastVisitRgId = WorkContext.RegionId;
                                userInfo.LastVisitTime = DateTime.Now;
                                userInfo.RegisterIP = WorkContext.IP;
                                userInfo.RegisterRgId = WorkContext.RegionId;
                                userInfo.RegisterTime = dsInfo.RegisterDate;

                                userInfo.Gender = WebHelper.GetFormInt("gender");
                                userInfo.RealName = dsInfo.UserName;
                                userInfo.Bday = new DateTime(1900, 1, 1);
                                userInfo.IdCard = dsInfo.UserCard;
                                userInfo.RegionId = WebHelper.GetFormInt("regionId");
                                userInfo.Address = WebHelper.HtmlEncode(WebHelper.GetFormString("address"));
                                userInfo.Bio = WebHelper.HtmlEncode(WebHelper.GetFormString("bio"));

                                #endregion
                                PartUserInfo parentUser = Users.GetPartUserModelByStrWhere(string.Format(" isdirsaleuser=1 and username='{0}'  ", dsInfo.ParentCode));

                                userInfo.Pid = parentUser.Uid > 0 ? parentUser.DirSaleUid : 0;
                                userInfo.Ptype = (int)UserPanertType.DirSaleUser;

                                userInfo.IsDirSaleUser = true;
                                userInfo.DirSaleUid = dsInfo.UserId;
                                userInfo.MaxCashCount = 5;
                                int Rank = dsInfo.Rank;
                                if (Rank == 4)//4白金卡=VIP3,3金卡=星级经销商2，2银卡=事业伙伴1
                                    userInfo.Ds2AgentRank = 3;
                                else if (Rank == 3)
                                    userInfo.Ds2AgentRank = 2;
                                else if (Rank == 2)
                                    userInfo.Ds2AgentRank = 1;
                                else
                                    userInfo.Ds2AgentRank = 0;
                                //创建用户
                                userInfo.Uid = Users.CreateUser(userInfo);

                                if (string.IsNullOrEmpty(userInfo.DirSalePwd))
                                {
                                    userInfo.DirSalePwd = SecureHelper.EncryptString(password, DirSaleUserInfo.EncryptKey);
                                    Users.UpdatePartUserDirSalePwd(userInfo);
                                }

                                //添加用户失败
                                if (userInfo.Uid < 1)
                                {
                                    LogHelper.WriteOperateLog("AddDirSaleUserFail", "拉取直销会员数据错误", "错误信息为：直销会员ID：" + dsInfo.UserId + ",手机号：" + dsInfo.UserPhone + ",用户名：" + dsInfo.UserCode);
                                    j++;
                                    continue;
                                }
                                else
                                {
                                    LogHelper.WriteOperateLog("AddDirSaleUserFor1", "拉取直销会员数据", "成功信息为：会员ID：" + userInfo.Uid + ",直销会员ID：" + userInfo.DirSaleUid + ",手机号：" + userInfo.Mobile + ",用户名：" + userInfo.UserName);
                                    i++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteOperateLog("AddDirSaleUserError", "拉取直销会员数据异常", "错误信息为：" + ex.Message);
                    continue;
                }

            }

            return Content("更新数据，成功记录数：" + i.ToString() + ",失败记录数" + j.ToString());
        }

        /// <summary>
        /// 
        /// 汇购uid 42441-50161的会员批量获取pid 20180611号同步
        /// </summary>
        /// <returns></returns>
        //public ActionResult UpdateDirSalePid()
        //{
        //    int i = 0;
        //    //int x = 0;//起始汇购uid
        //    int y = 50161;//终止汇购uid

        //    for (int x = 42441; x <= y; x++)
        //    {
        //        try
        //        {
        //            PartUserInfo tmpInfo = Users.GetPartUserById(x);
        //            if (tmpInfo != null)
        //            {
        //                DSDetailInfo dsInfo = AccountUtils.GetUserDetailMsg(tmpInfo.DirSaleUid);
        //                if (dsInfo.UserId >= 1) //有直销id返回才操作
        //                {
        //                    PartUserInfo parentuser = Users.GetPartUserByName(dsInfo.ParentCode);
        //                    if (parentuser != null)//
        //                    {
        //                        if (tmpInfo.Pid == 0)
        //                        {
        //                            int Pid = parentuser != null ? parentuser.DirSaleUid : 0;
        //                            Users.UpdateUserForPid(tmpInfo.Uid, Pid, 2);
        //                            LogHelper.WriteOperateLog("UpdateDirSaleUserFor1", "更新直销会员pid", "成功信息为：会员ID：" + tmpInfo.Uid + ",直销会员ID：" + tmpInfo.DirSaleUid + ",手机号：" + tmpInfo.Mobile + ",用户名：" + tmpInfo.UserName + ",parentcode:" + dsInfo.ParentCode + ",pid:" + Pid);

        //                            i++;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LogHelper.WriteOperateLog("UpdateDirSaleUserError", "更新直销会员pid异常", "错误信息为：" + ex.Message);
        //            continue;
        //        }

        //    }

        //    return Content("更新数据，成功记录数：" + i.ToString() );
        //}

        
        public ActionResult ee(int userId)
        {

            APIDictionary Params = new APIDictionary();
            Params.Add("userId", userId);
            string FromDirSale = client.Execute(Params, "/api/User/GetUserDetails");
            return Content(FromDirSale);
        }

    }
}
