using MySql.Data.MySqlClient;
using System.Data;

namespace Geone.Utiliy.Database
{
    public class MySQLConnect : IDbConnect
    {
        private IDbAccess access;

        public MySQLConnect(IDbAccess DbAccess)
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
            var conn = new MySqlConnection(connStr);
            conn.Open();
            return conn;
        }
    }
}