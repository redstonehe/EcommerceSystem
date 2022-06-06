using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JWT.Models
{
    public class TokenInfo
    {
        //public TokenInfo()
        //{
        //    iss = "签发者信息";
        //    aud = "http://example.com";
        //    sub = "HomeCare.VIP";
        //    jti = DateTime.Now.ToString("yyyyMMddhhmmss");
        //    UserName = "jack.chen";
        //    Pwd = "jack123456";
        //    UserRole = "HomeCare.Administrator";
        //}
        //
        public string iss { get; set; }
        public string exp { get; set; }
        public string sub { get; set; }
        public string aud { get; set; }
        public string nbf { get; set; } 
        public string iat { get; set; }
        public string jti { get; set; }
        public string UserName { get; set; }
        public string Uid { get; set; }
        public string UserRole { get; set; }
        /// <summary>
        /// 口令过期时间
        /// </summary>
        public DateTime? ExpiryDateTime { get; set; }
    }

}