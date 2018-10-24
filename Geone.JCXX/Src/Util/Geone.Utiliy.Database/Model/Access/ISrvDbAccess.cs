namespace Geone.Utiliy.Database
{
    public interface ISrvDbAccess
    {
        /// <summary>
        /// 获取连接配置
        /// </summary>
        /// <param name="DbName">服务配置值</param>
        /// <returns>连接字符串</returns>
        string GetDbConn(string DbName);
    }
}