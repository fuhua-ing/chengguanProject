using ServiceStack.Redis;

namespace Geone.Utiliy.Redis
{
    public interface IRedisConnect
    {
        IRedisClient OpenConn(string DbName);
    }
}