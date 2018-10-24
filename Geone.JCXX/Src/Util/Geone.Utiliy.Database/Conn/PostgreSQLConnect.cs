using Npgsql;
using System.Data;

namespace Geone.Utiliy.Database
{
    public class PostgreSQLConnect : IDbConnect
    {
        private IDbAccess access;

        public PostgreSQLConnect(IDbAccess DbAccess)
        {
            access = DbAccess;
        }

        /// <summary>
        /// 开启SQLServer数据库连接
        /// </summary>
        /// <param name="DbName">数据库名称-配置文件内名称</param>
        /// <returns></returns>
        public IDbConnection OpenConn(string DbName)
        {
            string connStr = access.GetDbConn(DbName);
            var conn = new NpgsqlConnection(connStr);
            conn.Open();
            return conn;
        }
    }
}