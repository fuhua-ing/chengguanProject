using Geone.Utiliy.Library;
using Newtonsoft.Json.Linq;

namespace Geone.Utiliy.Component
{
    public interface ISrvBus
    {
        RepModel Access(string srvid, JObject @params = default);

        RepModel Access<T>(string srvid, T param);

        RepModel Exec(string srvid, string key, JObject @params = default);

        RepModel Exec<T>(string srvid, string key, T param);

        RepModel Mock(string srvid);
    }
}