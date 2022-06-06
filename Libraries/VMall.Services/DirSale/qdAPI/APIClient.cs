using System;
using System.Collections;
using System.Collections.Generic;

using VMall.Core;


namespace VMall.Services
{
    /// <summary>
    /// 基于REST的客户端。
    /// </summary>
    public class APIClient
    {
        private string serverUrl;
        private string partnerId;
        private string appKey;
        private WebUtils webUtils;

        ////API 通讯协议  系统参数
        //private string format = "json";  //可选，指定响应格式。默认json,目前支持格式为json
        //private string v = "1.0";  //API协议版本，可选值:1.0
        private string sign = ""; //对 API 输入参数进行 md5 加密获得
        //private string sign_method = "md5";  //可选，参数的加密方法选择。默认为md5，可选值是：md5

        //private string timestamp = ""; //对 API 输入参数进行 md5 加密获得

        public APIClient(string serverUrl, string partnerId, string appKey)
        {
            this.partnerId = partnerId;
            this.appKey = appKey;
            this.serverUrl = serverUrl;
            this.webUtils = new WebUtils();
        }
        public void SetTimeout(int timeout)
        {
            webUtils.Timeout = timeout;
        }
        public string Execute(APIDictionary parametersDic, string subUrl)
        {
            // 添加协议级请求参数
            APIDictionary txtParams = new APIDictionary();
            txtParams.Add("partnerId", partnerId);
            txtParams.Add("timestamp", DateTime.Now);
            IDictionary<string, string> parameters = parametersDic;
            foreach (var p in parameters)
            {
                txtParams.Add(p.Key,p.Value);
            }
            txtParams.Add("sign", APIUtils.SignRequest(txtParams, appKey));
            //LogHelper.WriteOperateLog("API参数", "获得会员在直销系统中的重消PV数", "接口调用返回错误：" + ex.Message + "|DirUid：" + DSUid + "|curDate：" + curDate);
            string result = webUtils.DoGet(this.serverUrl + subUrl, txtParams);
            return result;// webUtils.DoGet(this.serverUrl + subUrl, txtParams);
        }
    }
}
