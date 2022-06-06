using System;

using VMall.Core;
using VMall.Services;

namespace VMall.SMSStrategy.VMall
{
    /// <summary>
    /// 简单短信策略
    /// </summary>
    public partial class SMSStrategy : ISMSStrategy
    {
        private string _url;
        private string _username;
        private string _password;

        /// <summary>
        /// 短信服务器地址
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// 短信账号
        /// </summary>
        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }

        /// <summary>
        /// 短信密码
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="to">接收人号码</param>
        /// <param name="body">短信内容</param>
        /// <returns>是否发送成功</returns>
        public bool Send(string to, string body)
        {
            //LogHelper.WriteOperateLog("短信发送成功", "开始", "=========开始发送==========：" );
            string userid = "15";
            string account = "seedqide";
            string password = "seedqide123";
            string action = "send";
            string sendurl = "http://120.26.230.213:8888/sms.aspx";
            //action=send&userid=12&account=账号&password=密码&mobile=15023239810,13527576163&content=内容&sendTime=&extno=
            
            string postData = string.Format("action=send&userid={0}&account={1}&password={2}&mobile={3}&content={4}", userid, account, password, to, body);
            string content =  WebHelper.DoPost(sendurl, postData);
            //LogHelper.WriteOperateLog("短信发送成功", "成功", "发送值：" + postData);
            if (content.Contains("<message>ok</message>"))
            {
                //LogHelper.WriteOperateLog("短信发送成功", "成功", "返回值：" + content);
                return true;
            }
            else if (content.Contains("<returnstatus>Faild</returnstatus>"))
            {
                LogHelper.WriteOperateLog("短信发送记录", "失败", "返回值：" + content);
                return false;
            }
            else
            {
                return false;

            }
        }
    }
}
