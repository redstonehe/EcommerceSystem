using System;
using System.Web.Mvc;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;
using VMall.Web.Mobile.Models;
using System.Collections.Generic;
using log4net;
using VMall.Core.Helper;
using VMall.Log4net;

namespace VMall.Web.Mobile.Controllers
{
    /// <summary>
    /// 充值控制器类
    /// </summary>
    public partial class TestController : BaseMobileController
    {
        #region 全局变量
        /// <summary>
        /// 日志引用
        /// </summary>
        private static log4net.ILog Log = LogManager.GetLogger("AppLogger");
        #endregion


        public ActionResult T_agent()
        {
            MemberInfo member = new MemberInfo()
            {
                ParentCode = "CHN88888",
                ManagerCode = "CHN100000",
                userId = 100000
            };
            return View("/mobile/Views/ucenter/agentresult.cshtml", member);

        }

        public ActionResult TT_agent()
        {
            int joinuid = 437;
            MemberInfo member = new MemberInfo()
            {
                ParentCode = "CHN99999",
                ManagerCode = "CHN100009",
                userId = 100000
            };
            return RedirectToAction("agentresult", "UCenter", new { ParentCode = member.ParentCode, ManagerCode = member.ManagerCode, joinuid = joinuid });
        }


        Dictionary<string, string> Sex = new Dictionary<string, string>() {

        {"0","男"},

        {"1","女"},
         {"2","位置"}
    };

        public ActionResult enumtest()
        {
            //string info = "green";
            //int result = Convert.ToInt32(Enum.Parse(typeof(TestType), info));
            //return Content(result.ToString());

            return Content(Sex["1"].ToString());
        }

        public ActionResult logtest()
        {
            try
            {
                string fileName = "chang";
                Log4NetHelper.WriteLogWithName(fileName);


                //Log.Debug("\n");
                //Log.Debug("Log 开始！");
                //Log.
            }
            catch (Exception ex)
            {
                //Log.Error("Log 异常 ：", ex);
            }
            finally
            {
                //Log.Debug("Log 结束！");
            }
            return Content("111");
        }
        public ActionResult logtest2()
        {
            ILog logger = CustomRollingFileLogger.GetCustomLogger("pay", "10001");
            logger.Debug("debug message");
            return Content("222");
        }
        public ActionResult logtest3()
        {
            ILog logger = CustomRollingFileLogger.GetCustomLogger("pay");
            logger.Debug("debug message");
            return Content("333");
        }
        public ActionResult logtest4()
        {
            LogHelper.WriteOperateLog("test", "日志测试", "测试日志内容", (int)LogLevelEnum.ERROR);
            
            return Content("666");
        }

        /// <summary>
        /// API测试-更新帐号
        /// </summary>
        /// <returns></returns>
        public ActionResult testUpdateAccount(int DSUid)
        {
            //decimal amount=
            // bool FromDirSale = false;// AccountUtils.UpdateAccountForDir(DSUid, (int)AccountType.海米账户, 173.4M, 0, "1120170226192929767582", "结算订单:1120170226192929767582,支出/收入金额:173.4");
            //bool FromDirSale = AccountUtils.UpdateAccountForDir(DSUid, (int)AccountType.海米账户, 5000, 0, "1111111111111111111", "增加测试金额,支出/收入金额:5000");
            //bool FromDirSale1 = AccountUtils.UpdateAccountForDir(DSUid, (int)AccountType.红包账户, 8000, 0, "2222222222222222222", "增加测试金额,支出/收入金额:8000");
            bool FromDirSale =
                 //AccountUtils.UpdateAccountForDir(DSUid, (int)AccountType.海米账户, 35272.29M, 0, "", "海米账户转入锁定余额，金额：35272.29");
                 AccountUtils.UpdateAccountForDir(DSUid, (int)AccountType.商城钱包, 0, 20, "1234577", "订单" + MallKey.MallDiscountName_JiangJin + "抵现：订单号:1234577" + ",抵现金额:20");
            return Content(FromDirSale.ToString());

           
        }


    }
}
