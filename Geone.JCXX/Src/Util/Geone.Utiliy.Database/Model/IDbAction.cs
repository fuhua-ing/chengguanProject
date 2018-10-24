using Geone.Utiliy.Library;
using System.Collections.Generic;

namespace Geone.Utiliy.Database
{
    public interface IDbAction
    {
        IDbAction SetName(string name);

        IDbAction SetTable(string table);

        int QueryCount(string sql);

        T QueryFirst<T>(string sql);

        List<T> QueryList<T>(string sql);

        Page<T> QueryPage<T>(int pi, int ps, string sql, params string[] orders);

        List<dynamic> Query(string sql);

        bool Insert(string sql);

        bool InsertBatch(params string[] sqls);

        bool Modify(string sql);

        bool ModifyBatch(params string[] sqls);

        bool Remove(string sql);

        bool RemoveBatch(params string[] sqls);

        bool Trans(params string[] sqls);
    }
}