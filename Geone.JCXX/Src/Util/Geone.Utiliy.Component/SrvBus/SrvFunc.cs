using Geone.Utiliy.Library;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace Geone.Utiliy.Component
{
    public class SrvFunc
    {
        //获取服务详细配置
        public static dynamic GetSrvConfig(SrvPrd provider, ILogWriter _log)
        {
            try
            {
                switch (provider.Type)
                {
                    case SrvPrdType.Rpc:
                        RpcSrvConfig rpcSrvConfig = JsonConvert.DeserializeObject<RpcSrvConfig>(provider.Config);
                        return rpcSrvConfig;

                    case SrvPrdType.Http:
                        HttpSrvConfig httpSrvConfig = JsonConvert.DeserializeObject<HttpSrvConfig>(provider.Config);
                        return httpSrvConfig;

                    case SrvPrdType.File:
                        FileSrvConfig fileSrvConfig = JsonConvert.DeserializeObject<FileSrvConfig>(provider.Config);
                        return fileSrvConfig;

                    case SrvPrdType.Config:
                        ConfigSrvConfig configSrvConfig = JsonConvert.DeserializeObject<ConfigSrvConfig>(provider.Config);
                        return configSrvConfig;

                    case SrvPrdType.Db:
                        DbSrvConfig dbSrvConfig = JsonConvert.DeserializeObject<DbSrvConfig>(provider.Config);
                        return dbSrvConfig;

                    case SrvPrdType.Redis:
                        RedisSrvConfig redisSrvConfig = JsonConvert.DeserializeObject<RedisSrvConfig>(provider.Config);
                        return redisSrvConfig;

                    case SrvPrdType.Value:
                        ValueSrvConfig valueSrvConfig = JsonConvert.DeserializeObject<ValueSrvConfig>(provider.Config);
                        return valueSrvConfig;

                    default:
                        _log.WriteServiceException(provider.Id, "GetSrvConfig", "提供服务类型错误");
                        return null;
                }
            }
            catch (Exception ex)
            {
                _log.WriteServiceException(provider.Id, "GetSrvConfig", "获取服务详细配置", ex);
                return null;
            }
        }

        //返回值过滤
        public static dynamic ResDataFilter(string srvid, dynamic data, string format, ILogWriter _log)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(format)) return data;

                string[] formats = format.Split('.');

                if (formats.Count() == 0) return data;

                if (formats.Count() == 1 && formats[0] == "root") return data;

                data = GetFilterData(data, formats);

                if (data.GetType() == typeof(JObject)) data = JsonConvert.SerializeObject(data);
                if (data.GetType() == typeof(JValue)) data = data.ToString();

                return data;
            }
            catch (Exception ex)
            {
                _log.WriteServiceException(srvid, "ResDataFilter", "返回值过滤", ex);
                return default;
            }
        }

        private static dynamic GetFilterData(dynamic data, string[] formats, int i = 0)
        {
            if (data.GetType() == typeof(string)) data = JsonConvert.DeserializeObject(data);
            if (data.GetType() == typeof(JObject)) data = data[formats[i]];
            else
            {
                Type type = data.GetType();

                foreach (PropertyInfo pi in type.GetProperties())
                {
                    if (pi.Name.ToLower() == formats[i].ToLower())
                    {
                        data = pi.GetValue(data);
                    }
                };
            }

            if (i + 1 < formats.Count())
            {
                i++;
                return GetFilterData(data, formats, i);
            }

            return data;
        }
    }
}