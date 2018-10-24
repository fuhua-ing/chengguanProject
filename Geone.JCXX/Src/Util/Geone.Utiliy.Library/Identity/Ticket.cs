using System;

namespace Geone.Utiliy.Library
{
    /// <summary>
    /// 票据表-持久类
    /// </summary>
    public class Ticket
    {
        //票据
        public string AvailableTicket { get; set; }

        //票据过期时间
        public long AvailableExpireIn { get; set; }

        private static string key = "123@abcd";

        //分发AvailableTicket
        public static Ticket Distribute(string Id, int Day, string User, string Role)
        {
            //令牌唯一标识符
            string guid = Guid.NewGuid().ToString();
            //创建Token
            Ticket token = new Ticket()
            {
                AvailableTicket = Create(Id, Day, guid, User, Role),
                AvailableExpireIn = ObtainExpireTime(Day)
            };

            return token;
        }

        //验证AvailableTicket
        public static bool Verify(string ticket)
        {
            //如果access_token为空返回false，否则判断用户信息
            if (!string.IsNullOrEmpty(ticket))
            {
                dynamic token = Obtain(ticket);

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

        //获取AvailableTicket
        public static dynamic Obtain(string ticket)
        {
            string akey = "tic" + key;

            return JwtEncrypt.Decrypt(akey, ticket);
        }

        //获取AvailableTicket过期时间
        public static long ObtainExpireTime(int days)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long timeStamp = (long)(DateTime.UtcNow.AddDays(days) - startTime).TotalMilliseconds; // 相差毫秒数

            return timeStamp;
        }

        //创建AvailableTicket
        public static string Create(string id, int day, string guid, string user, string role)
        {
            string akey = "tic" + key;

            //设定时间
            var exp = DateTime.UtcNow.AddDays(day).ToString("yyyyMMddHHmmss");
            var iat = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            string access_token = JwtEncrypt.Encrypt(akey, "Core", id, exp, iat, guid, user, role);

            return access_token;
        }
    }
}