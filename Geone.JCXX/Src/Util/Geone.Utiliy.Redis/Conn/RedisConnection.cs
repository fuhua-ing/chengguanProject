using Newtonsoft.Json;
using ServiceStack.Redis;

namespace Geone.Utiliy.Redis
{
    public class RedisConnection : IRedisConnection
    {
        public string ConnectionString { get; set; }

        public RedisConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public PooledRedisClientManager Prcm { get; set; }

        public IRedisClient Open()
        {
            if (Prcm == null)
                CreateManager();
            return Prcm.GetClient();
        }

        /// <summary>
        /// 创建链接池管理对象
        /// </summary>
        private void CreateManager()
        {
            Redis redisconfig = JsonConvert.DeserializeObject<Redis>(ConnectionString);
            Prcm = new PooledRedisClientManager(
                            redisconfig.Rwservers,
                            redisconfig.Ronlyservers,
                            new RedisClientManagerConfig
                            {
                                MaxWritePoolSize = (int)redisconfig.Max,
                                MaxReadPoolSize = (int)redisconfig.Max,
                                AutoStart = redisconfig.Auto,
                            })
            {
                ConnectTimeout = redisconfig.Timeout
            };
        }
    }
}