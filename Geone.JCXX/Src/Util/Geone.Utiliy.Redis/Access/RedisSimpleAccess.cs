namespace Geone.Utiliy.Redis
{
    //简单读取-直接返回值
    public class RedisSimpleAccess : IRedisAccess
    {
        public string GetDbConn(string RedisName)
        {
            return RedisName;
        }
    }
}