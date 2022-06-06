using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using System.Text;
using System.Collections.Specialized;
using System.Collections;

namespace VMall.Web.Areas.WebApi
{
    public class UserController : BaseApiController
    {
        //
        // GET: /WebApi/DirSaleSystem/
        private static string appKey = WebHelper.GetConfigSettings("DaMeiAppKey");
        public static string DaMeiLoginKey = "32C534A23AA0456FB78D32759D3FBB74";

        public ActionResult Index()
        {
            return Content("11");
        }
        /// <summary>
        /// 1、	根据用户唯一标识返回用户信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserInfo()
        {
            StringBuilder sb = new StringBuilder();
            APIDictionary txtParams = new APIDictionary();
            try
            {
                if (Request.HttpMethod.ToUpper() == "GET")
                {
                    NameValueCollection nvc = GETInput();
                    if (nvc.Count != 0)
                    {
                        for (int i = 0; i < nvc.Count; i++)
                        {
                            txtParams.Add(nvc.GetKey(i), nvc.GetValues(i)[0]);
                        }
                    }
                    else
                    {
                        return WebApiResult(-3, "请求传递参数列表为空", new object());
                    }
                }
                else
                {
                    return WebApiResult(-3, "错误的请求方式，系统当前只支持GET请求", new object());
                }

                string sign = txtParams["sign"];
                string ourSign = APIUtils.SignRequest(txtParams, appKey);
                if (sign == ourSign)//MD5验证 通过 执行 数据写入操作
                {
                    DateTime timestmap = TypeHelper.ObjectToDateTime(txtParams["time"]);
                    if (timestmap < DateTime.Now.AddHours(-1))
                        return WebApiResult(-2, "时间戳失效", new object());
                    int uid = TypeHelper.StringToInt(SecureHelper.AESDecrypt(txtParams["sourceId"], DaMeiLoginKey));
                    UserInfo userinfo = Users.GetUserById(uid);
                    if (userinfo == null)
                        return WebApiResult(0, "未找到用户", new object());
                    return WebApiResult(1, "", new
                    {
                        UId = userinfo.Uid,
                        Phone = userinfo.Mobile,
                        RealName = userinfo.RealName,
                        UserName = userinfo.UserName,
                        UnionId = userinfo.OtherLoginId,
                        NickName = userinfo.NickName
                    });
                }
                else  // MD5验证不正确 
                {
                    return WebApiResult(-1, "签名失效", new object());
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("DaMei_Getuserinfo", "获取会员信息", "获取会员信息请求异常");
                return WebApiResult(-4, "系统异常", new object());
            }
        }

        #region 获取POST请求和GET请求参数
        //获取GET返回来的数据
        private NameValueCollection GETInput()
        { return Request.QueryString; }

        // 获取POST返回来的数据
        private string PostInput()
        {
            try
            {
                System.IO.Stream s = Request.InputStream;
                int count = 0;
                byte[] buffer = new byte[1024];
                StringBuilder builder = new StringBuilder();
                while ((count = s.Read(buffer, 0, 1024)) > 0)
                {
                    builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
                }
                s.Flush();
                s.Close();
                s.Dispose();
                return builder.ToString();
            }
            catch (Exception ex)
            { throw ex; }
        }
        private SortedList Param()
        {
            string POSTStr = PostInput();
            SortedList SortList = new SortedList();
            int index = POSTStr.IndexOf("&");
            string[] Arr = { };
            if (index != -1) //参数传递不只一项
            {
                Arr = POSTStr.Split('&');
                for (int i = 0; i < Arr.Length; i++)
                {
                    int equalindex = Arr[i].IndexOf('=');
                    string paramN = Arr[i].Substring(0, equalindex);
                    string paramV = Arr[i].Substring(equalindex + 1);
                    if (!SortList.ContainsKey(paramN)) //避免用户传递相同参数
                    { SortList.Add(paramN, paramV); }
                    else //如果有相同的，一直删除取最后一个值为准
                    { SortList.Remove(paramN); SortList.Add(paramN, paramV); }
                }
            }
            else //参数少于或等于1项
            {
                int equalindex = POSTStr.IndexOf('=');
                if (equalindex != -1)
                { //参数是1项
                    string paramN = POSTStr.Substring(0, equalindex);
                    string paramV = POSTStr.Substring(equalindex + 1);
                    SortList.Add(paramN, paramV);

                }
                else //没有传递参数过来
                { SortList = null; }
            }
            return SortList;
        }
        #endregion

        


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


    }

}
