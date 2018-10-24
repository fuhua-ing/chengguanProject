using Geone.Utiliy.Library;
using Newtonsoft.Json.Linq;
using System;

namespace Geone.Utiliy.Component
{
    public class SrvBus : ISrvBus
    {
        private ILogWriter _log;
        private ISrvAccess _access;
        private ISrvConnect _connect;

        public SrvBus(ILogWriter logWriter, ISrvAccess srvAccess, ISrvConnect srvConnect)
        {
            _log = logWriter;
            _access = srvAccess;
            _connect = srvConnect;
        }

        #region 私有方法

        //进行订阅
        private RepModel Sub(SrvSub sub, JObject @params = default)
        {
            try
            {
                RepModel rep = Access(sub.SubId, @params);

                if (rep.StatusCode != 200)
                {
                    _log.WriteServiceException(sub.Name, "Sub", rep.Message, null);
                    return null;
                }

                //订阅值过滤
                if (!string.IsNullOrWhiteSpace(sub.Filter)) rep.Data = SrvFunc.ResDataFilter(sub.Name, rep.Data, sub.Filter, _log);

                return rep;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(sub.Name, "Sub", "进行订阅发生错误", ex));
            }
        }

        #endregion 私有方法

        //执行访问
        public RepModel Access(string srvid, JObject @params = default)
        {
            return _access.SrvPrdAccess(srvid, @params);
        }

        //执行访问
        public RepModel Access<T>(string srvid, T param)
        {
            try
            {
                JObject @params = new JObject();
                @params = JConvert.GetJObject(ref @params, param);
                RepModel rep = Access(srvid, @params);

                if (rep.StatusCode != 200)
                    _log.WriteServiceException(srvid, "Access<T>", rep.Message);

                return rep;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(srvid, "Access<T>", "执行服务访问出错", ex));
            }
        }

        //执行订阅
        public RepModel Exec(string srvid, string key, JObject @params = default)
        {
            try
            {
                //1.根据注册服务编号，获得服务提供配置
                SrvPrd provider = _connect.GetSrvPrd(srvid);
                if (provider == null) return RepModel.Error(_log.WriteServiceException(srvid, "Access", "无此服务提供配置"));

                RepModel rep = new RepModel();

                switch (provider.Type)
                {
                    case SrvPrdType.Rpc:
                        RpcSrvConfig rpcSrvConfig = SrvFunc.GetSrvConfig(provider, _log);
                        SrvSub subrpc = rpcSrvConfig.Subs[key];
                        rep = Sub(subrpc, @params);
                        break;

                    case SrvPrdType.Http:
                        HttpSrvConfig httpSrvConfig = SrvFunc.GetSrvConfig(provider, _log);
                        SrvSub subhttp = httpSrvConfig.Subs[key];
                        rep = Sub(subhttp, @params);
                        break;

                    default:
                        return RepModel.Error(_log.WriteServiceException(provider.Id, "Exec", "提供服务类型错误"));
                }

                if (rep.StatusCode != 200)
                {
                    _log.WriteServiceException(srvid, "Exec", rep.Message, null);
                    return null;
                }

                return rep;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(srvid, "Exec", "执行服务订阅出错", ex));
            }
        }

        //执行订阅
        public RepModel Exec<T>(string srvid, string key, T param)
        {
            try
            {
                JObject @params = new JObject();
                @params = JConvert.GetJObject(ref @params, param);
                RepModel rep = Exec(srvid, key, @params);
                if (rep.StatusCode != 200) _log.WriteServiceException(srvid, "Exec<T>", rep.Message, null);
                return rep;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(srvid, "Exec<T>", "执行服务订阅出错", ex));
            }
        }

        //执行模拟访问
        public RepModel Mock(string srvid)
        {
            try
            {
                //1.根据注册服务编号，获得服务提供配置
                SrvPrd provider = _connect.GetSrvPrd(srvid);
                if (provider == null) return RepModel.Error(_log.WriteServiceException(srvid, "Access", "无此服务提供配置"));

                //2.根据类型执行服务
                RepModel rep = new RepModel();
                switch (provider.Type)
                {
                    case SrvPrdType.Rpc:
                        rep = _access.RpcSrvAccessMock(provider);
                        break;

                    case SrvPrdType.Http:
                        rep = _access.HttpSrvAccessMock(provider);
                        break;

                    case SrvPrdType.File:
                        rep = _access.FileSrvAccess(provider, null);
                        break;

                    case SrvPrdType.Config:
                        rep = _access.ConfigSrvAccessMock(provider);
                        break;

                    case SrvPrdType.Db:
                        rep = _access.DbSrvAccessMock(provider);
                        break;

                    case SrvPrdType.Redis:
                        rep = _access.RedisSrvAccessMock(provider);
                        break;

                    case SrvPrdType.Value:
                        rep = _access.ValueSrvAccess(provider);
                        break;

                    default:
                        return RepModel.Error(_log.WriteServiceException(srvid, "Access", "提供服务类型错误"));
                }

                if (rep.StatusCode != 200)
                {
                    return RepModel.Error(_log.WriteServiceException(srvid, "Access", rep.Message));
                }

                return rep;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(srvid, "Access", "执行模拟服务访问失败", ex));
            }
        }
    }
}