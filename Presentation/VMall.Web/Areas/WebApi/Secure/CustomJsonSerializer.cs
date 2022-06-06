using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using VMall.Core;
namespace JWT.Help
{

    public class CustomJsonSerializer : IJsonSerializer
    {
        public string Serialize(object obj)
        {
            // Implement using favorite JSON Serializer
            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(obj);
            return json;
        }

        public T Deserialize<T>(string json)
        {
            // Implement using favorite JSON Serializer
            JavaScriptSerializer js = new JavaScriptSerializer();
            T t = js.Deserialize<T>(json);
            return t;
        }
    }

}