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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Com.LaKaLa
{
    public class DESCrypto
    {
        private const int KEY_LEN = 8;
        #region ECB模式

        ///<summary><![CDATA[DES加密函数]]></summary>
        ///<param name="str"><![CDATA[被加密数据]]></param>
        ///<param name="key"><![CDATA[密钥 ]]></param> 
        ///<returns><![CDATA[加密后数据]]></returns>   
        public static byte[] Encrypt(byte[] data, string key)
        {
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                provider.Mode = CipherMode.ECB;
                provider.Key = Encoding.ASCII.GetBytes(key.Substring(0, KEY_LEN));
                provider.IV = Encoding.ASCII.GetBytes(key.Substring(0, KEY_LEN));
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
                stream2.Write(data, 0, data.Length);
                stream2.FlushFinalBlock();
                StringBuilder builder = new StringBuilder();
                byte[] result = stream.ToArray();
                stream.Close();
                return result;
            }
            catch (Exception) { return null; }
        }
        ///<summary><![CDATA[DES解密函数]]></summary>
        ///<param name="str"><![CDATA[被解密数据]]></param>
        ///<param name="key"><![CDATA[密钥 ]]></param> 
        ///<returns><![CDATA[解密后数据]]></returns>   
        public static byte[] Decrypt(byte[] data, string key)
        {
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                provider.Mode = CipherMode.ECB;
                provider.Key = Encoding.ASCII.GetBytes(key.Substring(0, KEY_LEN));
                provider.IV = Encoding.ASCII.GetBytes(key.Substring(0, KEY_LEN));
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
                stream2.Write(data, 0, data.Length);
                stream2.FlushFinalBlock();
                stream.Close();
                return stream.ToArray();
            }
            catch (Exception) { return null; }
        }

        #endregion
    }
}
