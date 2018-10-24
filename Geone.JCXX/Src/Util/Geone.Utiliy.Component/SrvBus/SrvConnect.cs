using Geone.Utiliy.Library;
using System;

namespace Geone.Utiliy.Component
{
    public class SrvConnect : ISrvConnect
    {
        private ILogWriter log;
        private IConfigAction access;

        public SrvConnect(ILogWriter Log, IConfigAction Access)
        {
            log = Log;
            access = Access;
        }

        public SrvPrd GetSrvPrd(string srvid)
        {
            try
            {
                SrvPrd provider = access.GetModel<SrvPrd>("srvprd", srvid);
                return provider;
            }
            catch (Exception ex)
            {
                log.WriteServiceException(srvid, "GetProvider", "根据注册服务编号，获得服务提供配置", ex);
                return default;
            }
        }
    }
}