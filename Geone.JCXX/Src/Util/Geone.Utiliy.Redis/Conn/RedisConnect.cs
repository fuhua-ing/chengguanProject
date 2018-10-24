using ServiceStack.Redis;

namespace Geone.Utiliy.Redis
{
    public class RedisConnect : IRedisConnect
    {
        private IRedisAccess access;

        public RedisConnect(IRedisAccess DbAccess)
        {
            access = DbAccess;
        }

        /// <summary>
        /// 开启Redis数据库连接
        /// </summary>
        /// <param name="DBName">数据库名称-配置文件内名称</param>
        /// <returns></returns>
        public IRedisClient OpenConn(string DBName)
        {
            string connStr = access.GetDbConn(DBName);
            var conn = new RedisConnection(connStr);
            var client = conn.Open();
            return client;
        }
    }
}