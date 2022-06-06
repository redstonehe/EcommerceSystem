using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using VMall.Core;
using VMall.Web.Framework;
using VMall.PayPlugin.Custompay;
using System.Text;
using VMall.Services;
using System.Web.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VMall.Web.MallAdmin.Controllers
{
    /// <summary>
    /// 商城后台控制器类
    /// </summary>
    public class AdminCustompayController : BaseMallAdminController
    {
        /// <summary>
        /// 配置
        /// </summary>
        [HttpGet]
        [ChildActionOnly]
        public ActionResult Config()
        {
            ConfigModel model = new ConfigModel();

            PluginSetInfo pluginSetInfo = PluginUtils.GetPluginSet();
            model.Partner = pluginSetInfo.Partner;
            model.Key = pluginSetInfo.Key;
            model.Seller = pluginSetInfo.Seller;
            model.PayFee = pluginSetInfo.PayFee;
            model.FreeMoney = pluginSetInfo.FreeMoney;

            return View("~/plugins/VMall.PayPlugin.Custompay/views/adminCustompay/config.cshtml", model);
        }

        /// <summary>
        /// 配置
        /// </summary>
        [HttpPost]
        public ActionResult Config(ConfigModel model)
        {
            if (ModelState.IsValid)
            {
                PluginSetInfo pluginSetInfo = new PluginSetInfo();
                pluginSetInfo.Partner = model.Partner.Trim();
                pluginSetInfo.Key = model.Key.Trim();
                pluginSetInfo.Seller = model.Seller.Trim();
                pluginSetInfo.PayFee = model.PayFee;
                pluginSetInfo.FreeMoney = model.FreeMoney;
                PluginUtils.SavePluginSet(pluginSetInfo);

                AddMallAdminLog("修改微信收款码插件配置信息");
                return PromptView(Url.Action("config", "plugin", new { configController = "AdminCustompay", configAction = "Config" }), "插件配置修改成功");
            }
            return PromptView(Url.Action("config", "plugin", new { configController = "AdminCustompay", configAction = "Config" }), "信息有误，请重新填写");
        }

        
    }
}
