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
using VMall.Services.WeiShang;

namespace VMall.Web.Areas.WebApi
{
    public class Agent_UserController : BaseApiController
    {
        //
        // GET: /api/agent_user/aduit
        private static string appKey = "9849C088EAE44B39BF2FE2DF04680854";// WebHelper.GetConfigSettings("DaMeiAppKey");
        //public static string DaMeiLoginKey = "9849C088EAE44B39BF2FE2DF04680854";

        public ActionResult Index()
        {
            return Content("微商代理接口");
        }
        /// <summary>
        /// 1、	根据审核信息更新会员微商状态
        /// </summary>
        /// <returns></returns>
        public ActionResult Aduit()
        {
            StringBuilder sb = new StringBuilder();
            APIDictionary txtParams = new APIDictionary();
            try
            {
                if (Request.HttpMethod.ToUpper() == "POST")
                {
                    NameValueCollection nvc = Request.Form;
                    if (nvc.Count != 0)
                    {
                        for (int i = 0; i < nvc.Count; i++)
                        {
                            txtParams.Add(nvc.GetKey(i), nvc.GetValues(i)[0]);
                        }
                    }
                    else
                        return WebApiResult(-3, "请求传递参数列表为空", new object());
                }
                else
                    return WebApiResult(-3, "错误的请求方式，系统当前只支持Post请求", new object());


                string sign = txtParams["sign"];
                string ourSign = APIUtils.SignRequest(txtParams, appKey).ToUpper();
                if (sign == ourSign)//MD5验证 通过 执行 数据写入操作
                {
                    DateTime timestmap = TypeHelper.ObjectToDateTime(txtParams["timestamp"]);
                    if (timestmap < DateTime.Now.AddMinutes(-10))
                        return WebApiResult(-2, "时间戳失效", new object());
                    int extid = TypeHelper.StringToInt(txtParams["extid"]);
                    UserInfo userinfo = Users.GetUserById(extid);
                    if (userinfo == null)
                        return WebApiResult(0, "用户不存在", new object());
                    //接收审核通过状态，更新微商会员标识，同时查询微商订单并推送未推送的订单并更新订单已推送状态
                    LogHelper.WriteOperateLog("Agent_User_Aduit_success", "审核代理会员成功：", "会员id：" + userinfo.Uid);
                    //Users.UpdateAgentUserMark(userinfo);
                    //微商订单需要区分开独立店铺，订单流程不同 需推送和审核
                    //int WS_Storeid = 0;
                    //List<OrderInfo> orderList = Orders.GetOrderListByWhere(string.Format(" storeid={0} and ordersource<>{1} and orderstate>=70 and orderstate<=140 and uid={2} ", WS_Storeid, (int)OrderSource.微商系统, extid));
                    //foreach (var item in orderList)
                    //{
                    //    WS_Order.Submit(item.Oid, userinfo);
                    //}
                    return WebApiResult(1, "成功", new object());
                }
                else  // MD5验证不正确 
                {
                    return WebApiResult(-1, "签名失效", new object());
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteOperateLog("Agent_User_Aduit_error", "审核代理会员通知异常：", "异常信息：" + ex.Message);
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
