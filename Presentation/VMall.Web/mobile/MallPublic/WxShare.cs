using System;//需添加System.Runtime.Serialization引用  
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using VMall.Core;
using VMall.Services;
namespace VMall.Web.Mobile
{
    public partial class WeiXinShareScript
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        /// <param name="imgurl"></param>
        /// <param name="shareUid"></param>
        /// <param name="type">type为1 表示直接访问首页，扫二维码进来或者分享首页进来，type=2表示非首页，如产品详情页</param>
        /// <returns></returns>
        public string RegisterWeiXinShareScript(string title, string desc, string imgurl, int shareUid, int type = 1)
        {
            string userAgent = HttpContext.Current.Request.UserAgent;
            if (userAgent.ToLower().Contains("micromessenger"))
            {
                //公众号的应用ID  
                string appid = WxLoginConfig.APPID;
                //公众号的应用密钥  
                string secret = WxLoginConfig.APPSECRET;

                //生成签名的时间戳  
                TimeSpan ts = DateTime.Now - DateTime.Parse("1970-01-01 00:00:00");
                string timestamp = ts.TotalSeconds.ToString().Split('.')[0];
                //生成签名的随机串  
                string nonceStr = TenpayUtil.getNoncestr().ToLower();

                //微信access_token，用于获取微信jsapi_ticket  
                string token = GetAccess_token(appid, secret);
                //微信jsapi_ticket  
                string ticket = GetTicket(token);

                //当前网页的URL  
                string pageurl = HttpContext.Current.Request.Url.AbsoluteUri; //string.Format("http://{0}/mob/Home/ShareIndex?uid=" + shareUid + "&type=" + type, BMAConfig.MallConfig.SiteUrl);//需要跳转的地址; //HttpContext.Current.Request.Url.AbsoluteUri;
                //if(type==2)
                //    pageurl = HttpContext.Current.Request.Url.AbsoluteUri;
                //对所有待签名参数按照字段名的ASCII 码从小到大排序（字典序）后，使用URL键值对的格式（即key1=value1&key2=value2…）拼接成字符串  
                string str = "jsapi_ticket=" + ticket + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + pageurl;
                //签名,使用SHA1生成  
                string signature = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1").ToLower();

                LogHelper.WriteOperateLog("WeXinShareLog", "微信分享log", "参数：\r\n appid=" + appid + " \r\n timestamp=" + timestamp + " \r\n noncestr=" + nonceStr + " \r\n url=" + pageurl + " \r\n signature=" + signature + " \r\n token=" + token + " \r\n jsapi_ticket=" + ticket);

                //要注册到页面的JS脚本  
                StringBuilder sbjsApi = new StringBuilder();
                sbjsApi.Append("<script>");
                //sbjsApi.Append("$(function (){");
                //通过config接口注入权限验证配置  
                sbjsApi.Append("wx.config({debug:false,");
                sbjsApi.Append("appId: '" + appid + "',");
                sbjsApi.Append("timestamp: " + timestamp + ",");
                sbjsApi.Append("nonceStr: '" + nonceStr + "',");
                sbjsApi.Append("signature: '" + signature + "',");
                sbjsApi.Append("jsApiList: ['onMenuShareTimeline', 'onMenuShareAppMessage']});");

                //通过ready接口处理成功验证  
                sbjsApi.Append("wx.ready(");
                //sbjsApi.Append("function on_weixin_ready() {weixin_share();}");

                //获取“分享给朋友”按钮点击状态及自定义分享内容接口  
                sbjsApi.Append("function (){");
                sbjsApi.Append("wx.onMenuShareAppMessage({");
                sbjsApi.Append("title:'" + title + "',");
                sbjsApi.Append("desc:'" + desc + "',");
                sbjsApi.Append("link:'" + pageurl + "',");
                //sbjsApi.Append("link:'www.srckf.com/mob/active',");
                sbjsApi.Append("imgUrl:'" + imgurl + "',");
                sbjsApi.Append("type:'link',");
                sbjsApi.Append("dataUrl:'',");
                sbjsApi.Append("success: function () { },");
                sbjsApi.Append("cancel:function () {}");
                sbjsApi.Append("});");

                //获取“分享到朋友圈”按钮点击状态及自定义分享内容接口  
                sbjsApi.Append("wx.onMenuShareTimeline({");
                sbjsApi.Append("title:'" + title + "',");
                //sbjsApi.Append("link:'www.srckf.com/mob/active',");
                sbjsApi.Append("link:'" + pageurl + "',");
                sbjsApi.Append("imgUrl:'" + imgurl + "',");
                sbjsApi.Append("success: function () { },");
                sbjsApi.Append("cancel:function () {}");
                sbjsApi.Append("});");
                sbjsApi.Append("})");
                sbjsApi.Append("</script>");
                return sbjsApi.ToString();
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 取得nonceStr
        /// </summary>
        /// <returns></returns>
        public string GetNonceStr()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            StringBuilder sbResult = new StringBuilder();
            Random random = new Random(chars.Length);
            for (int i = 0; i < 32; i++)
            {
                sbResult.Append(chars[random.Next(chars.Length)]);
            }
            return sbResult.ToString();
        }
        /// <summary>  
        /// 获取微信jsapi_ticket  
        /// </summary>  
        /// <param name="token">access_token</param>  
        /// <returns>jsapi_ticket</returns>  
        public string GetTicket(string token)
        {
            string ticketUrl = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + token + "&type=jsapi";
            string jsonresult = HttpGet(ticketUrl, "UTF-8");
            WX_Ticket wxTicket = JsonDeserialize<WX_Ticket>(jsonresult);
            return wxTicket.ticket;
        }

        /// <summary>  
        /// 获取微信access_token  
        /// </summary>  
        /// <param name="appid">公众号的应用ID</param>  
        /// <param name="secret">公众号的应用密钥</param>  
        /// <returns>access_token</returns>  
        private string GetAccess_token(string appid, string secret)
        {
            string tokenUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret;
            string jsonresult = HttpGet(tokenUrl, "UTF-8");
            WX_Token wx = JsonDeserialize<WX_Token>(jsonresult);
            return wx.access_token;
        }

        /// <summary>  
        /// JSON反序列化  
        /// </summary>  
        /// <typeparam name="T">实体类</typeparam>  
        /// <param name="jsonString">JSON</param>  
        /// <returns>实体类</returns>  
        private T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary>  
        /// HttpGET请求  
        /// </summary>  
        /// <param name="url">请求地址</param>  
        /// <param name="encode">编码方式：GB2312/UTF-8</param>  
        /// <returns>字符串</returns>  
        private string HttpGet(string url, string encode)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=" + encode;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encode));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
    }

    /// <summary>  
    /// 通过微信API获取access_token得到的JSON反序列化后的实体  
    /// </summary>  
    public class WX_Token
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
    }

    /// <summary>  
    /// 通过微信API获取jsapi_ticket得到的JSON反序列化后的实体  
    /// </summary>  
    public class WX_Ticket
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public string ticket { get; set; }
        public string expires_in { get; set; }
    }
}