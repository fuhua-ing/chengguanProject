using Jose;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Geone.Utiliy.Library
{
    /// <summary>
    /// Jwt生成解码操作类
    /// </summary>
    public class JwtEncrypt
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iss"></param>
        /// <param name="aud"></param>
        /// <param name="exp"></param>
        /// <param name="iat"></param>
        /// <param name="jti"></param>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static string Encrypt(string key, string iss, string aud, string exp, string iat, string jti, string user, string role)
        {
            //iss: 签发者
            //aud: 接收者
            //exp: 过期时间
            //iat: 签发时间
            //jti: 唯一身份标识
            //user: 用户
            //role: 角色

            var payload = new Dictionary<string, object>()
            {
                { "iss", iss },
                { "aud", aud },
                { "exp", exp },
                { "iat", iat },
                { "jti", jti },
                { "user", user },
                { "role", role }
            };

            byte[] secretKey = Base64Url.Decode(key);

            string token = JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);

            return token;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static dynamic Decrypt(string key, string source)
        {
            var secretKey = Base64Url.Decode(key);

            //根据token获取用户信息，检测用户是否有效
            string json = JWT.Decode(source, secretKey);
            return JsonConvert.DeserializeObject<dynamic>(json);
        }
    }
}