using System;
using System.Collections;
using System.Collections.Generic;
using VMall.Core;


namespace VMall.Services
{
    /// <summary>
    /// 基于REST的客户端。
    /// </summary>
    public class SDDAPIClient
    {
        private string serverUrl;
        private string username;
        private string appKey;
        private WebUtils webUtils;

        ////API 通讯协议  系统参数
        //private string format = "json";  //可选，指定响应格式。默认json,目前支持格式为json
        //private string v = "1.0";  //API协议版本，可选值:1.0
        private string sign = ""; //对 API 输入参数进行 md5 加密获得
        //private string sign_method = "md5";  //可选，参数的加密方法选择。默认为md5，可选值是：md5

        //private string timestamp = ""; //对 API 输入参数进行 md5 加密获得

        public SDDAPIClient(string serverUrl, string username, string appKey)
        {
            this.username = username;
            this.appKey = appKey;
            this.serverUrl = serverUrl;
            this.webUtils = new WebUtils();
        }
        public void SetTimeout(int timeout)
        {
            webUtils.Timeout = timeout;
        }
        /// <summary>
        /// signtype 为1 sign  为2 signkey
        /// </summary>
        /// <param name="parametersDic"></param>
        /// <param name="subUrl"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string Execute(APIDictionary parametersDic, string subUrl,int signtype)
        {
            // 添加协议级请求参数
            APIDictionary txtParams = new APIDictionary();
            txtParams.Add("username", username);
            txtParams.Add("timestamp", SDDAPIUtils.GetTimeStamp());
            txtParams.Add("ver", 3);
            txtParams.Add("format", "json");
            
            //txtParams.Add("ip", 3);
            IDictionary<string, string> parameters = parametersDic;
            foreach (var p in parameters)
            {
                txtParams.Add(p.Key,p.Value);
            }
            if (signtype==1)
                txtParams.Add("sign", SDDAPIUtils.SignRequest(txtParams, appKey, subUrl));
            if (signtype == 2)
                txtParams.Add("signkey", SDDAPIUtils.SignKeyRequest(txtParams, appKey, subUrl));
            return webUtils.DoGet(this.serverUrl + subUrl, txtParams);
        }
    }
}
