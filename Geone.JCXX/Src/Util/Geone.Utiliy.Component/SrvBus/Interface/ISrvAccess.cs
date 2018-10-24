using Geone.Utiliy.Library;
using Newtonsoft.Json.Linq;

namespace Geone.Utiliy.Component
{
    public interface ISrvAccess
    {
        RepModel SrvPrdAccess(string srvid, JObject @params = default);

        RepModel RpcSrvAccess(SrvPrd provider, JObject @params);

        RepModel HttpSrvAccess(SrvPrd provider, JObject @params);

        RepModel FileSrvAccess(SrvPrd provider, JObject @params);

        RepModel ConfigSrvAccess(SrvPrd provider, JObject @params);

        RepModel DbSrvAccess(SrvPrd provider, JObject @params);

        RepModel RedisSrvAccess(SrvPrd provider, JObject @params);

        RepModel ValueSrvAccess(SrvPrd provider);

        RepModel RpcSrvAccessMock(SrvPrd provider);

        RepModel HttpSrvAccessMock(SrvPrd provider);

        RepModel ConfigSrvAccessMock(SrvPrd provider);

        RepModel DbSrvAccessMock(SrvPrd provider);

        RepModel RedisSrvAccessMock(SrvPrd provider);
    }
}