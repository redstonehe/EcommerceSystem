using JWT.Algorithms;
using JWT.Models;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using VMall.Core;
using VMall.Services;
namespace JWT.Help
{
    public class JwtHelp
    {

        //私钥  web.config中配置
        //"GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        private static string secret = WebHelper.GetConfigSettings("Secret");// ConfigurationManager.AppSettings["Secret"].ToString();

        /// <summary>
        /// 生成JwtToken
        /// </summary>
        /// <param name="payload">不敏感的用户数据</param>
        /// <returns></returns>
        public static string SetJwtEncode(Dictionary<string, object> payload)
        {

            //格式如下
            //var payload = new Dictionary<string, object>
            //{
            //    { "username","admin" },
            //    { "pwd", "claim2-value" }
            //};

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();//HMACSHA256加密
            IJsonSerializer serializer = new CustomJsonSerializer();//自定义序列化和反序列，Newtonsoft.Json 版本不对
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();//Base64编解码
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            
            var token = encoder.Encode(payload, secret);
            return token;

        }

        /// <summary>
        /// 根据jwtToken  获取实体
        /// </summary>
        /// <param name="token">jwtToken</param>
        /// <returns></returns>
        public static TokenInfo GetJwtDecode(string token)
        {
            try
            {
                IJsonSerializer serializer = new CustomJsonSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);

                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
                //token为之前生成的字符串
                var userInfo = decoder.DecodeToObject<TokenInfo>(token, secret, verify: true);//verify:true表示解析JWT时进行验证，该方法会自动调用validator的Validate()方法，不满足验证会抛出异常，因此我们不用写验证的方法

                return userInfo;
            }
            catch (TokenExpiredException ex)//如果当前时间大于负载中的过期时间（负荷中的exp），引发Token过期异常
            {
                LogHelper.WriteOperateLog("JWTTokenExpired","Jwt过期", ex.Message);
                return new TokenInfo();
                //message = "Token已经过期了！";
            }
            catch (SignatureVerificationException ex)//如果签名不匹配，引发签名验证异常
            {
                LogHelper.WriteOperateLog("JWTSignatureError","Jwt签名错误", ex.Message);
                return new TokenInfo();
                //message = "Token签名不正确！";
            }catch(Exception ex){
                LogHelper.WriteOperateLog("JWTError","Jwt异常", ex.Message);
                return new TokenInfo();
            }
        }


    }
}