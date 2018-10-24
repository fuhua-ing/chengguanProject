using ServiceStack.Redis;

namespace Geone.Utiliy.Redis
{
    public interface IRedisConnection
    {
        PooledRedisClientManager Prcm { get; set; }

        string ConnectionString { get; set; }

        IRedisClient Open();
    }
}