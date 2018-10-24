using System.Data;
using System.Data.SqlClient;

namespace Geone.Utiliy.Database
{
    public class SQLServerConnect : IDbConnect
    {
        private IDbAccess access;

        public SQLServerConnect(IDbAccess DbAccess)
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
            var conn = new SqlConnection(connStr);
            conn.Open();
            return conn;
        }
    }
}