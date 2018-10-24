namespace Geone.Utiliy.Database
{
    //简单读取-直接返回值
    public class DbSimpleAccess : IDbAccess
    {
        public string GetDbConn(string DbName)
        {
            return DbName;
        }
    }
}