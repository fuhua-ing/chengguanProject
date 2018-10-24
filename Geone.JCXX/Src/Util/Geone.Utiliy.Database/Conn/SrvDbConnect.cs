namespace Geone.Utiliy.Database
{
    public class SrvDbConnect : ISrvDbConnect
    {
        private ISrvDbAccess access;

        public SrvDbConnect(ISrvDbAccess SrvAccess)
        {
            access = SrvAccess;
        }

        public ISrvDbConnection OpenSrv(string DbName)
        {
            string connStr = access.GetDbConn(DbName);
            var conn = new SrvDbConnection(connStr);
            conn.Open();
            return conn;
        }
    }
}