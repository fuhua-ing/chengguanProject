using System.Data;

namespace Geone.Utiliy.Database
{
    public interface IDbConnect
    {
        IDbConnection OpenConn(string DbName);
    }
}