namespace Geone.Utiliy.Database
{
    public interface ISrvDbConnect
    {
        ISrvDbConnection OpenSrv(string DbName);
    }
}