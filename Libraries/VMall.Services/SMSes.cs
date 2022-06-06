using System;
using System.Text;

using VMall.Core;
using Top;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;

namespace VMall.Services
{
    using Aliyun.Acs.Core;
    using Aliyun.Acs.Core.Exceptions;
    using Aliyun.Acs.Core.Profile;
    using Aliyun.Acs.Dysmsapi.Model.V20170525;
    using System.Threading;
    using Qcloud.Sms;
    using System.Collections.Generic;

    /// <summary>
    /// 短信操作管理类
    /// </summary>
    public partial class SMSes
    {
        private static object _locker = new object();//锁对象
        private static ISMSStrategy _ismsstrategy = null;//短信策略
        private static SMSConfigInfo _smsconfiginfo = null;//短信配置
        private static MallConfigInfo _mallconfiginfo = null;//商城配置

        static SMSes()
        {
            _ismsstrategy = BMASMS.Instance;
            _smsconfiginfo = BMAConfig.SMSConfig;
            _mallconfiginfo = BMAConfig.MallConfig;
            _ismsstrategy.Url = _smsconfiginfo.Url;
            _ismsstrategy.UserName = _smsconfiginfo.UserName;
            _ismsstrategy.Password = _smsconfiginfo.Password;
        }

        /// <summary>
        /// 重置短信配置
        /// </summary>
        public static void ResetSMS()
        {
            lock (_locker)
            {
                _smsconfiginfo = BMAConfig.SMSConfig;
                _ismsstrategy.Url = _smsconfiginfo.Url;
                _ismsstrategy.UserName = _smsconfiginfo.UserName;
                _ismsstrategy.Password = _smsconfiginfo.Password;
            }
        }

        /// <summary>
        /// 重置商城信息
        /// </summary>
        public static void ResetMall()
        {
            lock (_locker)
            {
                _mallconfiginfo = BMAConfig.MallConfig;
            }
        }

        #region 发送短信共用方法       
        ///// <summary>
        ///// 发送短信验证码
        ///// </summary>
        ///// <param name="to">接收手机</param>
        ///// <param name="code">验证值</param>
        ///// <returns></returns>
        //public static bool SendMobileMessage(string code, string mobile)
        //{
        //    //return true;
        //    //产品名称:云通信短信API产品,开发者无需替换
        //    const String product = "Dysmsapi";
        //    //产品域名,开发者无需替换
        //    const String domain = "dysmsapi.aliyuncs.com";

        //    // TODO 此处需要替换成开发者自己的AK(在阿里云访问控制台寻找)
        //    const String accessKeyId = "LTAIA0MzfhLZXXXX";
        //    const String accessKeySecret = "Jh07qn3mCRVjIQSpOsoyXXXXXXXXXXX";

        //    IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
        //    DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
        //    IAcsClient acsClient = new DefaultAcsClient(profile);
        //    SendSmsRequest request = new SendSmsRequest();
        //    SendSmsResponse response = null;
        //    try
        //    {
        //        //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
        //        request.PhoneNumbers = mobile;
        //        //必填:短信签名-可在短信控制台中找到
        //        request.SignName = "XXXXX";
        //        //必填:短信模板-可在短信控制台中找到
        //        request.TemplateCode = "SMS_XXXXXXX";
        //        //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
        //        request.TemplateParam = string.Format("{0}\"code\":\"{1}\"{2}", "{", code, "}");
        //        //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
        //        //request.OutId = "yourOutId";
        //        //请求失败这里会抛ClientException异常
        //        response = acsClient.GetAcsResponse(request);

        //    }
        //    catch (ServerException e)
        //    {
        //        LogHelper.WriteOperateLog("SendMobileMsgError", "发送短信验证码异常", "异常信息:" + e.Message, (int)LogLevelEnum.ERROR);
        //        Console.WriteLine(e.ErrorCode);
        //    }
        //    catch (ClientException e)
        //    {
        //        LogHelper.WriteOperateLog("SendMobileMsgError", "发送短信验证码异常", "异常信息:" + e.Message, (int)LogLevelEnum.ERROR);
        //    }
        //    bool flag = false;
        //    if (response.Code == "OK")
        //        flag = true;

        //    return flag;
        //}
        /// <summary>
        /// 发送语音验证码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool SendMobileVoiceMessage(string code, string mobile)
        {
            return false;
            string appkey = "";// BMAConfig.SMSConfig.AppKey;
            string secret = "";// BMAConfig.SMSConfig.AppSecret;
            string url = BMAConfig.SMSConfig.Url;
            ITopClient client = new DefaultTopClient(url, appkey, secret);
            AlibabaAliqinFcTtsNumSinglecallRequest req = new AlibabaAliqinFcTtsNumSinglecallRequest();
            req.Extend = "12345";
            req.TtsParam = string.Format("{0}\"product\":\"汇购商城\",\"code\":\"{1}\"{2}", "{", code, "}");
            req.CalledNum = mobile;
            req.CalledShowNum = "051482043260";
            req.TtsCode = "TTS_9690662";
            AlibabaAliqinFcTtsNumSinglecallResponse rsp = client.Execute(req);

            bool flag = false;
            //return Content(rsp.Body);
            if (rsp.Result != null)
            {
                if (rsp.Result.Success)
                {
                    flag = true;
                }
            }
            return flag;
        }
        #endregion


        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="to">接收手机</param>
        /// <param name="code">验证值</param>
        /// <returns></returns>
        public static bool SendMobileMessage(string code, string mobile)
        {
            //return true;
            // 请根据实际 appid 和 appkey 进行开发，以下只作为演示 sdk 使用
            // appid,appkey,templId申请方式可参考接入指南 https://www.qcloud.com/document/product/382/3785#5-.E7.9F.AD.E4.BF.A1.E5.86.85.E5.AE.B9.E9.85.8D.E7.BD.AE
            int sdkappid = 1400213342;
            string appkey = "87c83289f4957c05180ea0583db29e01";
            int tmplId = 339958;
            try
            {
                SmsSingleSenderResult singleResult;
                SmsSingleSender singleSender = new SmsSingleSender(sdkappid, appkey);
                List<string> templParams = new List<string>();
                templParams.Add(code);
                // 指定模板单发
                // 假设短信模板内容为：测试短信，{1}，{2}，{3}，上学。
                singleResult = singleSender.SendWithParam("86", mobile, tmplId, templParams, "", "", "");
                bool flag = false;
                if (singleResult.result == 0)
                {
                    flag = true;
                    LogHelper.WriteOperateLog("SendMobileMsgSuccess", "发送短信验证码成功", "信息:发送手机：" + mobile + ",计费条数" + singleResult.fee);
                }
                else
                    LogHelper.WriteOperateLog("SendMobileMsgFail", "发送短信验证码失败", "失败信息:" + singleResult.errmsg);

                return flag;

            }
            catch (Exception e)
            {
                LogHelper.WriteOperateLog("SendMobileMsgError", "发送短信验证码异常", "异常信息:" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        //public static bool AliSendMessage(string code, string mobile)
        //{
        //    //return false;
        //    string appkey = "23369177";
        //    string secret = "ac0f0e2863430f7738d07ad90241e652";
        //    string url = "http://gw.api.taobao.com/router/rest";
        //    ITopClient client = new DefaultTopClient(url, appkey, secret);
        //    AlibabaAliqinFcSmsNumSendRequest req = new AlibabaAliqinFcSmsNumSendRequest();
        //    req.Extend = "123456";
        //    req.SmsType = "normal";
        //    req.SmsFreeSignName = "SEED汇购商城";
        //    req.SmsParam = string.Format("{0}\"code\":\"{1}\",\"product\":\"汇购商城\"{2}", "{", code, "}");
        //    req.RecNum = mobile;
        //    req.SmsTemplateCode = "SMS_9690654";
        //    AlibabaAliqinFcSmsNumSendResponse rsp = client.Execute(req);
        //    bool flag = false;
        //    //return Content(rsp.Body);
        //    if (rsp.Result != null)
        //    {
        //        if (rsp.Result.Success)
        //        {
        //            flag = true;
        //        }
        //    }
        //    return flag;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        //public static bool AliSendCallMessage(string code, string mobile)
        //{
        //    return false;
        //    string appkey = "111111111";//"23369177";
        //    string secret = "ac0f0e2863430f7738d07ad90241e652";
        //    string url = "http://gw.api.taobao.com/router/rest";
        //    ITopClient client = new DefaultTopClient(url, appkey, secret);
        //    AlibabaAliqinFcTtsNumSinglecallRequest req = new AlibabaAliqinFcTtsNumSinglecallRequest();
        //    req.Extend = "12345";
        //    req.TtsParam = string.Format("{0}\"product\":\"汇购商城\",\"code\":\"{1}\"{2}", "{", code, "}");
        //    req.CalledNum = mobile;
        //    req.CalledShowNum = "051482043260";
        //    req.TtsCode = "TTS_9690662";
        //    AlibabaAliqinFcTtsNumSinglecallResponse rsp = client.Execute(req);

        //    bool flag = false;
        //    //return Content(rsp.Body);
        //    if (rsp.Result != null)
        //    {
        //        if (rsp.Result.Success)
        //        {
        //            flag = true;
        //        }
        //    }
        //    return flag;
        //}

        /// <summary>
        /// 发送找回密码短信验证码
        /// </summary>
        /// <param name="to">接收手机</param>
        /// <param name="code">验证值</param>
        /// <returns></returns>
        public static bool SendFindPwdMobile(string to, string code)
        {
            return SendMobileMessage(code, to);
        }

        /// <summary>
        /// 发送找回密码语音验证码
        /// </summary>
        /// <param name="to">接收手机</param>
        /// <param name="code">验证值</param>
        /// <returns></returns>
        public static bool SendFindPwdMobileVoice(string to, string code)
        {
            return false ;
        }

        /// <summary>
        /// 安全中心发送验证手机短信
        /// </summary>
        /// <param name="to">接收手机</param>
        /// <param name="code">验证值</param>
        /// <returns></returns>
        public static bool SendSCVerifySMS(string to, string code)
        {
            return SendMobileMessage(code, to);
        }
        /// <summary>
        /// 安全中心发送验证手机--语音验证码
        /// </summary>
        /// <param name="to">接收手机</param>
        /// <param name="code">验证值</param>
        /// <returns></returns>
        public static bool SendSCVerifySMSVoice(string to, string code)
        {
            return false;
        }
        /// <summary>
        /// 安全中心发送确认更新手机短信
        /// </summary>
        /// <param name="to">接收手机</param>
        /// <param name="code">验证值</param>
        /// <returns></returns>
        public static bool SendSCUpdateSMS(string to, string code)
        {
            return SendMobileMessage(code, to);
        }
        /// <summary>
        /// 安全中心发送确认更新手机短信
        /// </summary>
        /// <param name="to">接收手机</param>
        /// <param name="code">验证值</param>
        /// <returns></returns>
        public static bool SendSCUpdateSMSVoice(string to, string code)
        {
            return false;
          
        }
        /// <summary>
        /// 发送注册欢迎短信
        /// </summary>
        /// <param name="to">接收手机</param>
        /// <returns></returns>
        public static bool SendWebcomeSMS(string to)
        {
            StringBuilder body = new StringBuilder(_smsconfiginfo.WebcomeBody);
            body.Replace("{mallname}", _mallconfiginfo.MallName);
            body.Replace("{regtime}", CommonHelper.GetDateTime());
            body.Replace("{mobile}", to);
            return _ismsstrategy.Send(to, body.ToString());
        }

        /// <summary>
        /// 发送注册验证码
        /// </summary>
        /// <param name="to">接收手机</param>
        /// <returns></returns>
        public static bool SendRegisterSMS(string to,string code)
        {
            StringBuilder body = new StringBuilder();
            body.Append("【汇购】您的手机验证码是：");
            body.Append(code);
            body.Append("(请在5分钟内完成验证，打死也不要告诉别人哦！)");
            return _ismsstrategy.Send(to, body.ToString());
        }
        /// <summary>
        /// 发送注册成功验证码
        /// </summary>
        /// <param name="to">接收手机</param>
        /// <returns></returns>
        public static bool SendRegisterSuccessSMS(string to, string password)
        {
            //return true;
            // 请根据实际 appid 和 appkey 进行开发，以下只作为演示 sdk 使用
            // appid,appkey,templId申请方式可参考接入指南 https://www.qcloud.com/document/product/382/3785#5-.E7.9F.AD.E4.BF.A1.E5.86.85.E5.AE.B9.E9.85.8D.E7.BD.AE
            int sdkappid = 1400213342;
            string appkey = "87c83289f4957c05180ea0583db29e01";
            int tmplId = 343372;
            try
            {
                SmsSingleSenderResult singleResult;
                SmsSingleSender singleSender = new SmsSingleSender(sdkappid, appkey);
                List<string> templParams = new List<string>();
                templParams.Add(password);
                // 指定模板单发
                // 假设短信模板内容为：测试短信，{1}，{2}，{3}，上学。
                singleResult = singleSender.SendWithParam("86", to, tmplId, templParams, "", "", "");
                bool flag = false;
                if (singleResult.result == 0)
                {
                    flag = true;
                    LogHelper.WriteOperateLog("SendMobileMsgSuccess", "发送短信验证码成功", "信息:发送手机：" + to + ",计费条数" + singleResult.fee);
                }
                else
                    LogHelper.WriteOperateLog("SendMobileMsgFail", "发送短信验证码失败", "失败信息:" + singleResult.errmsg);

                return flag;

            }
            catch (Exception e)
            {
                LogHelper.WriteOperateLog("SendMobileMsgError", "发送短信验证码异常", "异常信息:" + e.Message);
                return false;
            }
        }
    }
}
