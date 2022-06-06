using System;
using System.Collections.Generic;
using System.Web;

namespace VMall.PayPlugin.WeChat
{
    public class WxPayException : Exception 
    {
        public WxPayException(string msg) : base(msg) 
        {

        }
     }
}