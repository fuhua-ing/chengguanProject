using Geone.Utiliy.Library;

namespace Geone.Utiliy.Database
{
    //Config读取
    public class DbConfigAccess : IDbAccess
    {
        public string GetDbConn(string DbName)
        {
            if (DbName == null) DbName = "Default";
            AppInitialize initialize = AppConfig.Init;
            if (initialize.Sql.ContainsKey(DbName)) return initialize.Sql[DbName];
            else return null;
        }
    }
}