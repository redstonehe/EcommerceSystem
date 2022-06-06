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
    public class AccountController : BaseApiController
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
        /// 2、	查询海米账户余额
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBalance()
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
                    if(timestmap<DateTime.Now.AddHours(-1))
                        return WebApiResult(-2, "时间戳失效", new object());
                    int uid = TypeHelper.StringToInt(SecureHelper.AESDecrypt(txtParams["sourceId"], DaMeiLoginKey));
                    UserInfo userinfo = Users.GetUserById(uid);
                    if (userinfo == null)
                        return WebApiResult(0, "未找到用户", new object());
                    List<AccountInfo> accountlist = AccountUtils.GetAccountList(userinfo.Uid, userinfo.IsDirSaleUser, userinfo.DirSaleUid);
                    AccountInfo haimiAccount = accountlist.Find(x => x.AccountId == (int)AccountType.海米账户);
                    if (haimiAccount == null)
                        return WebApiResult(0, "未找到用户", new object());
                    return WebApiResult(1, "", new
                    {
                        HaiMiBalance = haimiAccount.Banlance
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

        /// <summary>
        /// 3、	扣除海米账户余额：
        /// </summary>
        /// <returns></returns>
        public ActionResult Deduct()
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
                    List<AccountInfo> accountlist = AccountUtils.GetAccountList(userinfo.Uid, userinfo.IsDirSaleUser, userinfo.DirSaleUid);
                    AccountInfo haimiAccount = accountlist.Find(x => x.AccountId == (int)AccountType.海米账户);
                    if (haimiAccount == null)
                        return WebApiResult(0, "未找到用户", new object());

                    decimal amount = TypeHelper.StringToDecimal(txtParams["amount"]);
                    if (haimiAccount.Banlance < amount)
                        return WebApiResult(-5, "余额不足", new object());
                    bool isok = false;

                    string remark = txtParams.ContainsKey("remark")  ?  txtParams["remark"]:"";
                    //更新直销的账户
                    if (userinfo.IsDirSaleUser)
                    {
                        isok = AccountUtils.UpdateAccountForDir(userinfo.DirSaleUid, (int)AccountType.海米账户, 0, amount, "", remark);
                    }
                    //更新海汇账户
                    else
                    {
                        isok = Account.UpdateAccountForOut2(new AccountInfo()
                        {
                            AccountId = (int)AccountType.海米账户,
                            UserId = userinfo.Uid,
                            TotalOut = amount
                        });
                        Account.CreateAccountDetail(new AccountDetailInfo()
                        {
                            AccountId = (int)AccountType.海米账户,
                            UserId = userinfo.Uid,
                            CreateTime = DateTime.Now,
                            DetailType = (int)DetailType.订单抵现支出,
                            OutAmount = amount,
                            OrderCode = "",
                            AdminUid = userinfo.Uid,//system
                            Status = 1,
                            DetailDes = remark
                        });
                    }

                    return WebApiResult(1, "", new
                    {
                        updatestate = isok?"ok":"fail"
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
