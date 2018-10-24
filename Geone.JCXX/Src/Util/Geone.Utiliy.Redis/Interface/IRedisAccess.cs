namespace Geone.Utiliy.Redis
{
    /// <summary>
    /// 数据库配置_Redis
    /// </summary>
    public interface IRedisAccess
    {
        /// <summary>
        /// 获取连接配置
        /// </summary>
        /// <param name="RedisName">数据库配置值</param>
        /// <returns>连接字符串</returns>
        string GetDbConn(string RedisName);
    }
}