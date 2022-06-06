using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Globalization;
using System.IO;

namespace Com.LaKaLa
{
    class RSAUtil
    {
        public RSAUtil()
        {
        }


        #region RSA公钥加密
        //RSA公钥加密
        public static byte[] EncryptByPublicKey(byte[] data, string xmlPublicKey)
        {

            byte[] CypherTextBArray;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            CypherTextBArray = rsa.Encrypt(data, false);
            return CypherTextBArray;

        }
        #endregion

        #region RSA私钥解密
        //RSA私钥解密
        public static byte[] DecryptByPrivateKey(byte[] data, string xmlPrivateKey)
        {
            byte[] DypherTextBArray;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            DypherTextBArray = rsa.Decrypt(data, false);
            return DypherTextBArray;

        }
        #endregion

        #region RSA私钥加密
        //RSA私钥加密
        public static byte[] EncryptByPrivateKey(byte[] data, string xmlPrivateKey)
        {
            RSACryptoServiceProvider rcp = new RSACryptoServiceProvider();
            rcp.FromXmlString(xmlPrivateKey);
            RSAParameters pm = rcp.ExportParameters(!rcp.PublicOnly);
            BigInteger mod = new BigInteger(pm.Modulus);
            BigInteger ep = new BigInteger(pm.D != null ? pm.D : pm.Exponent);
            int k_len = pm.Modulus.Length;
            int pad_len = k_len - data.Length - 3;
            byte[] em = new byte[k_len];
            Stream stream = new MemoryStream(em);
            stream.WriteByte(0x00);
            stream.WriteByte(0x01); // 为了与Java兼容，BlockType设为1 
            // 为了与Java兼容，填充字节设为0xff
            for (int i = 0; i < pad_len; i++) stream.WriteByte(0xff);
            stream.WriteByte(0x00);
            stream.Write(data, 0, data.Length); BigInteger m = new BigInteger(em);
            BigInteger c = m.modPow(ep, mod);
            return c.getBytes(); 

        }
        #endregion

        #region RSA公钥解密
        //RSA公钥解密
        public static byte[] DecryptByPublicKey(byte[] data, string xmlPublicKey)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            RSAParameters paramsters = rsa.ExportParameters(false);

            BigInteger e = new BigInteger(paramsters.Exponent);
            BigInteger n = new BigInteger(paramsters.Modulus);

            int bk = 128;
            int len = data.Length;
            int cycle = 0;
            if ((len % bk) == 0) cycle = len / bk; else cycle = len / bk + 1;

            ArrayList temp = new ArrayList();
            int blockLen = 0;
            for (int i = 0; i < cycle; i++)
            {
                if (len >= bk) blockLen = bk; else blockLen = len;

                byte[] context = new byte[blockLen];
                int po = i * bk;
                Array.Copy(data, po, context, 0, blockLen);

                BigInteger biText = new BigInteger(context);
                BigInteger biEnText = biText.modPow(e, n);

                byte[] b = biEnText.getBytes();
                temp.AddRange(b);
                len -= blockLen;
            }

            byte[] resultArr = (byte[])temp.ToArray(typeof(byte));
            ArrayList resultList = new ArrayList();
            bool isStart = false;
            foreach(byte b in resultArr){
                if(isStart){
                    resultList.Add(b);
                }
                if(b == 0x00){
                    isStart = true;
                }
            }

            byte[] result = (byte[])resultList.ToArray(typeof(byte));
            
            return result;
            


        }
        #endregion

        public static byte[] ParseHexString(string text)
        {
            if ((text.Length % 2) != 0)
            {
                return null;
            }

            if (text.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                text = text.Substring(2);
            }

            int arrayLength = text.Length / 2;
            byte[] byteArray = new byte[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                byteArray[i] = byte.Parse(text.Substring(i * 2, 2), NumberStyles.HexNumber);
            }

            return byteArray;
        }
    }
}
