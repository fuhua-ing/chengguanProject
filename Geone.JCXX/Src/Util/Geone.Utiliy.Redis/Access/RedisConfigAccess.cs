using Geone.Utiliy.Library;

namespace Geone.Utiliy.Redis
{
    //Config读取
    public class RedisConfigAccess : IRedisAccess
    {
        public string GetDbConn(string RedisName)
        {
            if (RedisName == null) RedisName = "Default";
            AppInitialize initialize = AppConfig.Init;
            if (initialize.Redis.ContainsKey(RedisName)) return initialize.Redis[RedisName];
            else return null;
        }
    }
}