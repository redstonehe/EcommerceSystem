using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Com.LaKaLa
{
    public class MerchantKeyMap
    {
        private static Dictionary<String, String> map = new Dictionary<String, String>();
        private static Object[]  lockObj = new Object[0];
        private MerchantKeyMap(){}

        public static String getKey(String merchantId) {

            if (map.ContainsKey(merchantId)) {
                foreach(var dic in map){
                    if(dic.Key.Equals(merchantId)){
                        return dic.Value;
                    }
                }
            }

            lock (lockObj) {
                if (map.ContainsKey(merchantId)) {
                    foreach(var dic in map){
                        if(dic.Key.Equals(merchantId)){
                            return dic.Value;
                        }
                    }
                };

                String key = Tools.getRandomString(32);
                map.Add(merchantId, key);
                return key;
            }
        }
    }
}
