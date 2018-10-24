using System;

namespace Geone.Utiliy.Database
{
    public interface ISrvDbConnection : IDisposable
    {
        string ConnectionString { get; set; }

        void Open();
    }
}