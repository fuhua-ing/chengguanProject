using Geone.Utiliy.Library;
using Newtonsoft.Json.Linq;

namespace Geone.Utiliy.Database
{
    public static class ExtendISrvConnection
    {
        public static int Count(this ISrvDbConnection connection, string resource, JObject criteria)
        {
            return default;
        }

        public static Page<TEntity> Query<TEntity>(this ISrvDbConnection connection, string resource, JObject criteria, int? pageindex = null, int? pagesize = null)
        {
            return default;
        }

        public static bool Insert(this ISrvDbConnection connection, string resource, JObject data)
        {
            return default;
        }

        public static bool InsertBatch(this ISrvDbConnection connection, string resource, JArray datas)
        {
            return default;
        }

        public static bool Modify(this ISrvDbConnection connection, string resource, JObject data)
        {
            return default;
        }

        public static bool ModifyBatch(this ISrvDbConnection connection, string resource, JArray datas)
        {
            return default;
        }

        public static bool Delete(this ISrvDbConnection connection, string resource, JObject data)
        {
            return default;
        }

        public static bool DeleteBatch(this ISrvDbConnection connection, string resource, JArray datas)
        {
            return default;
        }
    }
}