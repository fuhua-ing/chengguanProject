using System;

namespace Geone.Utiliy.Library
{
    /// <summary>
    /// 令牌表-解析类
    /// </summary>
    public class Token
    {
        //连接令牌
        public string AccessToken { get; set; }

        //连接令牌过期时间
        public long AccessExpireIn { get; set; }

        //刷新令牌
        public string RefreshToken { get; set; }

        //刷新令牌过期时间
        public long RefreshExpireIn { get; set; }

        #region 简单Token验证

        //分发Token
        public static Token DistributeToken(string Appkey, string AppId, string User, string Role, int Hour = 6)
        {
            //令牌唯一标识符
            string guid = Guid.NewGuid().ToString();
            //创建Token
            Token token = new Token()
            {
                AccessToken = CreateToken(Appkey, AppId, guid, User, Role, Hour),
                AccessExpireIn = ObtainExpireTime(Hour)
            };

            return token;
        }

        //创建Token
        public static string CreateToken(string key, string appid, string guid, string user, string role, int Hour)
        {
            //设定时间
            var exp = DateTime.UtcNow.AddHours(Hour).ToString("yyyyMMddHHmmss");
            var iat = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            string verify_token = JwtEncrypt.Encrypt(key, "Core", appid, exp, iat, guid, user, role);

            return verify_token;
        }

        //获取Token
        public static dynamic ObtainToken(string key, string verify_token)
        {
            return JwtEncrypt.Decrypt(key, verify_token);
        }

        //获取Token过期时间
        public static long ObtainExpireTime(int Hour)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long timeStamp = (long)(DateTime.Now.AddHours(Hour) - startTime).TotalMilliseconds; // 相差毫秒数

            return timeStamp;
        }

        //验证Token
        public static bool VerifyToken(string key, string appid, string verify_token)
        {
            //如果access_token为空返回false，否则判断用户信息
            if (!string.IsNullOrEmpty(verify_token))
            {
                dynamic token = ObtainToken(key, verify_token);

                //过期时间-现在时间
                long a = Convert.ToInt64(token.exp) - Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                //签发时间-现在时间
                long b = Convert.ToInt64(token.iat) - Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                //检测token值
                if (a < 0 || b > 0)
                {
                    return false;
                }
                else
                    return true;
            }
            else
            {
                return false;
            }
        }

        #endregion 简单Token验证

        #region Access-Refresh验证

        //分发Token
        public static Token DistributeARToken(string Appkey, string AppId, string User, string Role, int Hour = 6)
        {
            //令牌唯一标识符
            string guid = Guid.NewGuid().ToString();
            //创建Token
            Token token = new Token()
            {
                AccessToken = CreateAccessToken(Appkey, AppId, guid, User, Role),
                AccessExpireIn = ObtainAccessExpireTime(),
                RefreshToken = CreateRefreshToken(Appkey, AppId, guid, User, Role, Hour),
                RefreshExpireIn = ObtainRefreshExpireTime(Hour)
            };

            return token;
        }

        //创建AccessToken
        public static string CreateAccessToken(string key, string appid, string guid, string user, string role)
        {
            string akey = "ac" + key;

            //设定时间
            var exp = DateTime.UtcNow.AddMinutes(5).ToString("yyyyMMddHHmmss");
            var iat = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            string access_token = JwtEncrypt.Encrypt(akey, "Core", appid, exp, iat, guid, user, role);

            return access_token;
        }

        //创建RefreshToken
        public static string CreateRefreshToken(string key, string appid, string guid, string user, string role, int hour)
        {
            string rkey = "re" + key;

            //设定时间
            var exp = DateTime.UtcNow.AddHours(hour).ToString("yyyyMMddHHmmss");
            var iat = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            string refresh_token = JwtEncrypt.Encrypt(rkey, "Core", appid, exp, iat, guid, user, role);

            return refresh_token;
        }

        //获取AccessToken
        public static dynamic ObtainAccessToken(string key, string access_token)
        {
            string akey = "ac" + key;

            return JwtEncrypt.Decrypt(akey, access_token);
        }

        //获取RefreshToken
        public static dynamic ObtainRefreshToken(string key, string refresh_token)
        {
            string rkey = "re" + key;

            return JwtEncrypt.Decrypt(rkey, refresh_token);
        }

        //获取AccessToken过期时间
        public static long ObtainAccessExpireTime()
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long timeStamp = (long)(DateTime.UtcNow.AddMinutes(5) - startTime).TotalMilliseconds; // 相差毫秒数

            return timeStamp;
        }

        //获取RefreshToken过期时间
        public static long ObtainRefreshExpireTime(int hour)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long timeStamp = (long)(DateTime.UtcNow.AddHours(hour) - startTime).TotalMilliseconds; // 相差毫秒数

            return timeStamp;
        }

        //验证AccessToken
        public static bool VerifyAccessToken(string key, string appid, string access_token)
        {
            //如果access_token为空返回false，否则判断用户信息
            if (!string.IsNullOrEmpty(access_token))
            {
                dynamic token = ObtainAccessToken(key, access_token);

                //过期时间-现在时间
                long a = Convert.ToInt64(token.exp.ToString()) - Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                //签发时间-现在时间
                long b = Convert.ToInt64(token.iat.ToString()) - Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                //检测token值
                if (a < 0 || b > 0)
                {
                    return false;
                }
                else
                    return true;
            }
            else
            {
                return false;
            }
        }

        //验证RefreshToken
        public static bool VerifyRefreshToken(string key, string appid, string access_token, string refresh_token)
        {
            //如果access_token为空返回false，否则判断用户信息
            if (!string.IsNullOrEmpty(access_token) && !string.IsNullOrEmpty(refresh_token))
            {
                dynamic atoken = ObtainAccessToken(key, access_token);
                dynamic rtoken = ObtainRefreshToken(key, refresh_token);

                //过期时间-现在时间
                long a = Convert.ToInt64(rtoken.exp) - Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                //签发时间-现在时间
                long b = Convert.ToInt64(rtoken.iat) - Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                //检测token值
                if (a < 0 || b > 0 || atoken.jti.ToString() != rtoken.jti.ToString())
                {
                    return false;
                }
                else
                    return true;
            }
            else
            {
                return false;
            }
        }

        #endregion Access-Refresh验证
    }
}