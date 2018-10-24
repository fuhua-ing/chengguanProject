using System.Collections.Generic;

namespace Geone.Utiliy.Library
{
    public interface ILogRecord
    {
        void RecordLog<TLog>(TLog log, params string[] code);

        List<TLog> ReadLog<TLog>(params string[] code);
    }
}