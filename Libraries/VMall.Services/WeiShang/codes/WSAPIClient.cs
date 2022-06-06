using System;
using System.Collections;
using System.Collections.Generic;
using VMall.Core;


namespace VMall.Services
{
    /// <summary>
    /// 基于REST的客户端。
    /// </summary>
    public class WSAPIClient
    {
        private string serverUrl = "http://hgw.51shop.mobi/v2";
        private string Host = "hgw.51shop.mobi";
        //private string ExtUid="1";
        private string IP = "127.0.0.1";
        private string appKey="Xquark";
        private WebUtils webUtils;

        ////API 通讯协议  系统参数
        //private string format = "json";  //可选，指定响应格式。默认json,目前支持格式为json
        //private string v = "1.0";  //API协议版本，可选值:1.0
        private string sign = ""; //对 API 输入参数进行 md5 加密获得
        //private string sign_method = "md5";  //可选，参数的加密方法选择。默认为md5，可选值是：md5
        //private string timestamp = ""; //对 API 输入参数进行 md5 加密获得

        public WSAPIClient(string serverUrl, string Host,  string IP, string appKey)
        {
            this.serverUrl = serverUrl;
            this.Host = Host;
            //this.ExtUid = ExtUid;
            this.IP = IP;
            this.appKey = appKey;
            this.webUtils = new WebUtils();
        }
        public void SetTimeout(int timeout)
        {
            webUtils.Timeout = timeout;
        }
        /// <summary>
        /// </summary>
        /// <param name="parametersDic"></param>
        /// <param name="subUrl"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string Execute(APIDictionary Bus_Params, string subUrl)
        {
            // 添加协议级请求参数
            APIDictionary Sys_Params = new APIDictionary();
            Sys_Params.Add("Host", Host);
            //txtParams.Add("ExtUid", ExtUid);
            Sys_Params.Add("IP", IP);
            //txtParams.Add("timestamp", WSAPIUtils.GetTimeStamp());
            //txtParams.Add("format", "json");

            //parametersDic.Add("ExtUid", ExtUid);
            Bus_Params.Add("Sign", WSAPIUtils.SignRequest(Sys_Params,Bus_Params, appKey));
            return webUtils.DoPost(this.serverUrl + subUrl, Bus_Params);
        }
    }
}
