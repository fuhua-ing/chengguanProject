using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Redis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Geone.Utiliy.Component
{
    public class SrvAccess : ISrvAccess
    {
        private ILogWriter _log;
        private ISrvConnect _connect;
        private IConfigExce _config;
        private IDbAction _db;
        private IRedisAction _redis;
        private IHttpAccess _http;
        private IRpcAccess _rpc;
        private IFileReadWrite _file;

        public SrvAccess(ILogWriter logWriter, ISrvConnect srvConnect, IConfigExce configExce, IDbAction dbAction, IRedisAction redisAction, IHttpAccess httpAccess, IRpcAccess rpcAccess, IFileReadWrite fileReadWrite)
        {
            _log = logWriter;
            _connect = srvConnect;
            _config = configExce;
            _db = dbAction;
            _redis = redisAction;
            _http = httpAccess;
            _rpc = rpcAccess;
            _file = fileReadWrite;
        }

        #region 私有方法

        //参数订阅-提供服务编号
        private JObject ParamSub(string srvid, Dictionary<string, SrvParam> srvparams, JObject @params)
        {
            try
            {
                foreach (KeyValuePair<string, SrvParam> param in srvparams)
                {
                    switch (param.Value.Type)
                    {
                        case SrvParamType.Transfer:
                            break;

                        case SrvParamType.Configure:
                            //新增或替换到参数字典
                            if (@params.ContainsKey(param.Key)) { @params[param.Key] = param.Value.Config; }
                            else { @params.Add(param.Key, param.Value.Config); }
                            break;

                        case SrvParamType.Subscribe:
                            RepModel repModel = SrvPrdAccess(param.Value.Config, @params);
                            //新增或替换到参数字典
                            if (@params.ContainsKey(param.Key)) { @params[param.Key] = repModel.Data.ToString(); }
                            else { @params.Add(param.Key, repModel.Data); }
                            break;

                        default:
                            break;
                    }
                }
                return @params;
            }
            catch (Exception ex)
            {
                _log.WriteServiceException(srvid, "ParamSub", "参数订阅-提供服务编号", ex);
                return default;
            }
        }

        //参数模拟订阅-提供服务编号
        private JObject ParamMock(string srvid, Dictionary<string, SrvParam> srvparams, JObject @params)
        {
            try
            {
                foreach (KeyValuePair<string, SrvParam> param in srvparams)
                {
                    switch (param.Value.Type)
                    {
                        case SrvParamType.Transfer:
                            //新增或替换到参数字典
                            if (@params.ContainsKey(param.Key)) { @params[param.Key] = param.Value.Config; }
                            else { @params.Add(param.Key, param.Value.Config); }
                            break;

                        case SrvParamType.Configure:
                            //新增或替换到参数字典
                            if (@params.ContainsKey(param.Key)) { @params[param.Key] = param.Value.Config; }
                            else { @params.Add(param.Key, param.Value.Config); }
                            break;

                        case SrvParamType.Subscribe:
                            RepModel repModel = SrvPrdAccess(param.Value.Config, @params);
                            //新增或替换到参数字典
                            if (@params.ContainsKey(param.Key)) { @params[param.Key] = repModel.Data.ToString(); }
                            else { @params.Add(param.Key, repModel.Data); }
                            break;

                        default:
                            break;
                    }
                }
                return @params;
            }
            catch (Exception ex)
            {
                _log.WriteServiceException(srvid, "ParamSub", "参数订阅-提供服务编号", ex);
                return default;
            }
        }

        #endregion 私有方法

        //执行访问
        public RepModel SrvPrdAccess(string srvid, JObject @params = default)
        {
            try
            {
                //1.根据注册服务编号，获得服务提供配置
                SrvPrd provider = _connect.GetSrvPrd(srvid);
                if (provider == null)
                    return RepModel.Error(_log.WriteServiceException(srvid, "Access", "无此服务提供配置"));

                //2.根据类型执行服务
                RepModel rep = new RepModel();
                switch (provider.Type)
                {
                    case SrvPrdType.Rpc:
                        rep = RpcSrvAccess(provider, @params);
                        break;

                    case SrvPrdType.Http:
                        rep = HttpSrvAccess(provider, @params);
                        break;

                    case SrvPrdType.File:
                        rep = FileSrvAccess(provider, @params);
                        break;

                    case SrvPrdType.Config:
                        rep = ConfigSrvAccess(provider, @params);
                        break;

                    case SrvPrdType.Db:
                        rep = DbSrvAccess(provider, @params);
                        break;

                    case SrvPrdType.Redis:
                        rep = RedisSrvAccess(provider, @params);
                        break;

                    case SrvPrdType.Value:
                        rep = ValueSrvAccess(provider);
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
                return RepModel.Error(_log.WriteServiceException(srvid, "Access", "执行服务访问出错", ex));
            }
        }

        public RepModel RpcSrvAccess(SrvPrd provider, JObject @params)
        {
            try
            {
                //1.获得详细配置
                RpcSrvConfig rpcSrvConfig = SrvFunc.GetSrvConfig(provider, _log);

                //2.进行参数订阅
                if (rpcSrvConfig.Params != null)
                {
                    if (rpcSrvConfig.Params.Count > 0)
                    {
                        @params = ParamSub(provider.Id, rpcSrvConfig.Params, @params);
                    }
                }

                JObject headers = new JObject();

                if (rpcSrvConfig.Headers != null)
                {
                    headers = rpcSrvConfig.Headers;
                }
                if (!headers.ContainsKey("Srvid")) headers.Add("Srvid", provider.Id);

                RpcReq req = new RpcReq()
                {
                    Header = JsonConvert.SerializeObject(headers),
                    Param = JsonConvert.SerializeObject(@params)
                };

                //3.进行访问
                AppInitialize initialize = AppConfig.Init;
                IBuild build = _rpc.GetService<IBuild>(AppConfig.Set.ConnectHost, (int)AppConfig.Set.ConnectPort);
                AppInitialize init = build.ServiceObtainByName(rpcSrvConfig.ServiceKeyName).ResponseAsync.Result;
                RepModel rep = _rpc.Communicate(init.Host, (int)init.RpcPort, rpcSrvConfig.ServiceName, rpcSrvConfig.MethodName, req);

                //4.进行返回值过滤
                if (rep.StatusCode == 200 && !string.IsNullOrWhiteSpace(rpcSrvConfig.Filter))
                {
                    rep.Data = SrvFunc.ResDataFilter(provider.Id, rep.Data, rpcSrvConfig.Filter, _log);
                }

                return rep;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(provider.Id, "RpcAccess", "远程调用访问出错", ex));
            }
        }

        public RepModel HttpSrvAccess(SrvPrd provider, JObject @params)
        {
            try
            {
                //1.获得详细配置
                HttpSrvConfig httpSrvConfig = SrvFunc.GetSrvConfig(provider, _log);

                //2.进行参数订阅
                if (httpSrvConfig.Params != null)
                {
                    if (httpSrvConfig.Params.Count > 0)
                    {
                        @params = ParamSub(provider.Id, httpSrvConfig.Params, @params);
                    }
                }

                JObject headers = new JObject();

                if (httpSrvConfig.Headers != null)
                {
                    headers = httpSrvConfig.Headers;
                }

                if (!headers.ContainsKey("Srvid")) headers.Add("Srvid", provider.Id);

                //3.进行访问
                AppInitialize initialize = AppConfig.Init;
                IBuild build = _rpc.GetService<IBuild>(AppConfig.Set.ConnectHost, (int)AppConfig.Set.ConnectPort);
                AppInitialize init = build.ServiceObtainByName(httpSrvConfig.ServiceKeyName).ResponseAsync.Result;
                string url = $"{init.Host}:{init.Port}" + httpSrvConfig.Url;
                string param = string.Empty;

                if (httpSrvConfig.Predicate == "Get")
                {
                    url += $"?1=1";

                    foreach (KeyValuePair<string, JToken> parampair in @params)
                    {
                        url += $"&{parampair.Key}={parampair.Value}";
                    }
                }
                else
                {
                    param = JsonConvert.SerializeObject(@params);
                }

                Dictionary<string, string> httpheaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(headers));
                RepModel rep = _http.Communicate(url, httpSrvConfig.Predicate, param, httpheaders);

                //4.进行返回值过滤
                if (rep.StatusCode == 200 && !string.IsNullOrWhiteSpace(httpSrvConfig.Filter))
                {
                    rep.Data = SrvFunc.ResDataFilter(provider.Id, rep.Data, httpSrvConfig.Filter, _log);
                }

                return rep;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(provider.Id, "HttpAccess", "Http服务访问出错", ex));
            }
        }

        public RepModel FileSrvAccess(SrvPrd provider, JObject @params)
        {
            try
            {
                //1.获得详细配置
                FileSrvConfig fileSrvConfig = SrvFunc.GetSrvConfig(provider, _log);

                //2.进行访问
                RepModel rep = new RepModel();
                if (fileSrvConfig.Type == FileSrvPrdType.Path && fileSrvConfig.ExceType == FileExceType.Read)
                {
                    rep = RepModel.Success(_file.Read(fileSrvConfig.Path, fileSrvConfig.Name));
                }
                else if (fileSrvConfig.Type == FileSrvPrdType.Path && fileSrvConfig.ExceType == FileExceType.Write)
                {
                    string content = JsonConvert.SerializeObject(@params);
                    rep = RepModel.Success(_file.Write(fileSrvConfig.Path, fileSrvConfig.Name, content));
                }
                else if (fileSrvConfig.Type == FileSrvPrdType.Url)
                {
                    AppInitialize initialize = AppConfig.Init;
                    IBuild build = _rpc.GetService<IBuild>(AppConfig.Set.ConnectHost, (int)AppConfig.Set.ConnectPort);
                    AppInitialize init = build.ServiceObtainByName(fileSrvConfig.ServiceKeyName).ResponseAsync.Result;
                    string url = $"{init.Host}:{init.Port}" + fileSrvConfig.Url + fileSrvConfig.Name;

                    rep = _http.Get(url);
                }
                else
                {
                    return RepModel.Error(_log.WriteServiceException(provider.Id, "FileAccess-类型错误", "文件服务访问出错"));
                }

                //3.进行返回值过滤
                if (rep.StatusCode == 200 && fileSrvConfig.FileType == FileSrvConfigType.Json && !string.IsNullOrWhiteSpace(fileSrvConfig.Filter))
                {
                    rep.Data = SrvFunc.ResDataFilter(provider.Id, rep.Data, fileSrvConfig.Filter, _log);
                }

                return rep;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(provider.Id, "FileAccess", "文件服务访问出错", ex));
            }
        }

        public RepModel ConfigSrvAccess(SrvPrd provider, JObject @params)
        {
            try
            {
                //1.获得详细配置
                ConfigSrvConfig configSrvConfig = SrvFunc.GetSrvConfig(provider, _log);
                List<object> objs = new List<object>();

                //2.进行参数订阅
                if (configSrvConfig.Params != null)
                {
                    if (configSrvConfig.Params.Count > 0)
                    {
                        @params = ParamSub(provider.Id, configSrvConfig.Params, @params);
                    }
                }

                foreach (KeyValuePair<string, JToken> parampair in @params)
                {
                    objs.Add(parampair.Value);
                }

                return _config.Exce(configSrvConfig.Name, configSrvConfig.Meathod, objs.ToArray());
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(provider.Id, "ConfigSrvAccess", "配置服务访问出错", ex));
            }
        }

        public RepModel DbSrvAccess(SrvPrd provider, JObject @params)
        {
            try
            {
                //1.获得详细配置
                DbSrvConfig dbSrvConfig = SrvFunc.GetSrvConfig(provider, _log);

                //2.进行参数订阅
                if (dbSrvConfig.Params != null)
                {
                    if (dbSrvConfig.Params.Count > 0)
                    {
                        @params = ParamSub(provider.Id, dbSrvConfig.Params, @params);
                    }
                }

                for (int i = 0; i < dbSrvConfig.Sqls.Length; i++)
                {
                    foreach (KeyValuePair<string, JToken> parampair in @params)
                    {
                        dbSrvConfig.Sqls[i] = dbSrvConfig.Sqls[i].Replace($"@{parampair.Key}", $"'{parampair.Value.ToString()}'");
                    }
                }

                //3.进行访问
                switch (dbSrvConfig.ExecType)
                {
                    case ExecuteType.Query:
                        List<dynamic> queryres = _db.Query(dbSrvConfig.Sqls[0]);
                        return RepModel.Success(queryres);

                    case ExecuteType.Insert:
                        return RepModel.Success(_db.Insert(dbSrvConfig.Sqls[0]));

                    case ExecuteType.InsertBatch:
                        return RepModel.Success(_db.InsertBatch(dbSrvConfig.Sqls));

                    case ExecuteType.Modify:
                        return RepModel.Success(_db.Modify(dbSrvConfig.Sqls[0]));

                    case ExecuteType.ModifyBatch:
                        return RepModel.Success(_db.ModifyBatch(dbSrvConfig.Sqls));

                    case ExecuteType.Remove:
                        return RepModel.Success(_db.Remove(dbSrvConfig.Sqls[0]));

                    case ExecuteType.RemoveBatch:
                        return RepModel.Success(_db.RemoveBatch(dbSrvConfig.Sqls));

                    default:
                        return RepModel.Error("提供数据库执行类型错误！");
                }
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(provider.Id, "DbSrvAccess", "数据服务访问出错", ex));
            }
        }

        public RepModel RedisSrvAccess(SrvPrd provider, JObject @params)
        {
            try
            {
                //1.获得详细配置
                RedisSrvConfig redisSrvConfig = SrvFunc.GetSrvConfig(provider, _log);

                //2.进行参数订阅
                if (redisSrvConfig.Params != null)
                {
                    if (redisSrvConfig.Params.Count > 0)
                    {
                        @params = ParamSub(provider.Id, redisSrvConfig.Params, @params);
                    }
                }
                string key = @params["key"].ToString();

                //3.进行访问
                switch (redisSrvConfig.ExecType)
                {
                    case ExecuteType.Query:
                        return RepModel.Success(_redis.Get(key));

                    case ExecuteType.Insert:
                        return RepModel.Success(_redis.Set(key, @params[key].ToString()));

                    case ExecuteType.Modify:
                        return RepModel.Success(_redis.Set(key, @params[key].ToString()));

                    case ExecuteType.Remove:
                        return RepModel.Success(_redis.Remove(key));

                    default:
                        return RepModel.Error("提供数据库执行类型错误！");
                }
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(provider.Id, "RedisSrvAccess", "Redis服务访问出错", ex));
            }
        }

        public RepModel ValueSrvAccess(SrvPrd provider)
        {
            try
            {
                //1.获得详细配置
                ValueSrvConfig valueSrvConfig = SrvFunc.GetSrvConfig(provider, _log);
                string value = valueSrvConfig.Value;

                //2.进行访问
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return RepModel.Success(value);
                }
                else
                    return RepModel.Error(null);
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(provider.Id, "ValueAccess", "值服务访问出错", ex));
            }
        }

        public RepModel RpcSrvAccessMock(SrvPrd provider)
        {
            try
            {
                //1.获得详细配置
                RpcSrvConfig rpcSrvConfig = SrvFunc.GetSrvConfig(provider, _log);

                //2.进行参数订阅
                JObject @params = new JObject();

                if (rpcSrvConfig.Params != null)
                {
                    if (rpcSrvConfig.Params.Count > 0)
                    {
                        @params = ParamMock(provider.Id, rpcSrvConfig.Params, @params);
                    }
                }

                JObject headers = new JObject();

                if (rpcSrvConfig.Headers != null)
                {
                    headers = rpcSrvConfig.Headers;
                }

                if (!headers.ContainsKey("Srvid")) headers.Add("Srvid", provider.Id);

                RpcReq req = new RpcReq()
                {
                    Header = JsonConvert.SerializeObject(headers),
                    Param = JsonConvert.SerializeObject(@params)
                };

                //3.进行访问
                AppInitialize initialize = AppConfig.Init;
                IBuild build = _rpc.GetService<IBuild>(AppConfig.Set.ConnectHost, (int)AppConfig.Set.ConnectPort);
                AppInitialize init = build.ServiceObtainByName(rpcSrvConfig.ServiceKeyName).ResponseAsync.Result;
                RepModel rep = _rpc.Communicate(init.Host, (int)init.RpcPort, rpcSrvConfig.ServiceName, rpcSrvConfig.MethodName, req);

                //4.进行返回值过滤
                if (rep.StatusCode == 200 && !string.IsNullOrWhiteSpace(rpcSrvConfig.Filter))
                {
                    rep.Data = SrvFunc.ResDataFilter(provider.Id, rep.Data, rpcSrvConfig.Filter, _log);
                }

                return rep;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(provider.Id, "RpcAccess", "远程调用模拟服务访问出错", ex));
            }
        }

        public RepModel HttpSrvAccessMock(SrvPrd provider)
        {
            try
            {
                //1.获得详细配置
                HttpSrvConfig httpSrvConfig = SrvFunc.GetSrvConfig(provider, _log);

                //2.进行参数订阅
                JObject @params = new JObject();

                if (httpSrvConfig.Params != null)
                {
                    if (httpSrvConfig.Params.Count > 0)
                    {
                        @params = ParamMock(provider.Id, httpSrvConfig.Params, @params);
                    }
                }

                JObject headers = new JObject();

                if (httpSrvConfig.Headers != null)
                {
                    headers = httpSrvConfig.Headers;
                }

                if (!headers.ContainsKey("Srvid")) headers.Add("Srvid", provider.Id);

                //3.进行访问
                AppInitialize initialize = AppConfig.Init;
                IBuild build = _rpc.GetService<IBuild>(AppConfig.Set.ConnectHost, (int)AppConfig.Set.ConnectPort);
                AppInitialize init = build.ServiceObtainByName(httpSrvConfig.ServiceKeyName).ResponseAsync.Result;
                string url = $"{init.Host}:{init.Port}" + httpSrvConfig.Url;
                string param = string.Empty;

                if (httpSrvConfig.Predicate == "Get")
                {
                    url += $"?1=1";

                    foreach (KeyValuePair<string, JToken> parampair in @params)
                    {
                        url += $"&{parampair.Key}={parampair.Value.ToString()}";
                    }
                }
                else
                {
                    param = JsonConvert.SerializeObject(@params);
                }
                Dictionary<string, string> httpheaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(headers));
                RepModel rep = _http.Communicate(url, httpSrvConfig.Predicate, param, httpheaders);

                //4.进行返回值过滤
                if (rep.StatusCode == 200 && !string.IsNullOrWhiteSpace(httpSrvConfig.Filter))
                {
                    rep.Data = SrvFunc.ResDataFilter(provider.Id, rep.Data, httpSrvConfig.Filter, _log);
                }

                return rep;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(provider.Id, "HttpAccess", "Http访问", ex).错误信息);
            }
        }

        public RepModel ConfigSrvAccessMock(SrvPrd provider)
        {
            try
            {
                //1.获得详细配置
                ConfigSrvConfig configSrvConfig = SrvFunc.GetSrvConfig(provider, _log);
                List<object> objs = new List<object>();

                //2.进行参数订阅
                JObject @params = new JObject();
                if (configSrvConfig.Params != null)
                {
                    if (configSrvConfig.Params.Count > 0)
                    {
                        @params = ParamMock(provider.Id, configSrvConfig.Params, @params);
                    }
                }

                foreach (KeyValuePair<string, JToken> parampair in @params)
                {
                    objs.Add(parampair.Value);
                }

                return _config.Exce(configSrvConfig.Name, configSrvConfig.Meathod, objs.ToArray());
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(provider.Id, "ConfigSrvAccess", "配置服务访问出错", ex));
            }
        }

        public RepModel DbSrvAccessMock(SrvPrd provider)
        {
            try
            {
                //1.获得详细配置
                DbSrvConfig dbSrvConfig = SrvFunc.GetSrvConfig(provider, _log);

                //2.进行参数订阅
                JObject @params = new JObject();
                if (dbSrvConfig.Params != null)
                {
                    if (dbSrvConfig.Params.Count > 0)
                    {
                        @params = ParamMock(provider.Id, dbSrvConfig.Params, @params);
                    }
                }

                for (int i = 0; i < dbSrvConfig.Sqls.Length; i++)
                {
                    foreach (KeyValuePair<string, JToken> parampair in @params)
                    {
                        dbSrvConfig.Sqls[i] = dbSrvConfig.Sqls[i].Replace($"@{parampair.Key}", $"'{parampair.Value.ToString()}'");
                    }
                }

                //3.进行访问
                switch (dbSrvConfig.ExecType)
                {
                    case ExecuteType.Query:
                        List<dynamic> queryres = _db.Query(dbSrvConfig.Sqls[0]);
                        return RepModel.Success(queryres);

                    case ExecuteType.Insert:
                        return RepModel.Success(_db.Insert(dbSrvConfig.Sqls[0]));

                    case ExecuteType.InsertBatch:
                        return RepModel.Success(_db.InsertBatch(dbSrvConfig.Sqls));

                    case ExecuteType.Modify:
                        return RepModel.Success(_db.Modify(dbSrvConfig.Sqls[0]));

                    case ExecuteType.ModifyBatch:
                        return RepModel.Success(_db.ModifyBatch(dbSrvConfig.Sqls));

                    case ExecuteType.Remove:
                        return RepModel.Success(_db.Remove(dbSrvConfig.Sqls[0]));

                    case ExecuteType.RemoveBatch:
                        return RepModel.Success(_db.RemoveBatch(dbSrvConfig.Sqls));

                    default:
                        return RepModel.Error("提供数据库执行类型错误！");
                }
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(provider.Id, "DbSrvAccess", "数据服务访问出错", ex));
            }
        }

        public RepModel RedisSrvAccessMock(SrvPrd provider)
        {
            try
            {
                //1.获得详细配置
                RedisSrvConfig redisSrvConfig = SrvFunc.GetSrvConfig(provider, _log);

                //2.进行参数订阅
                JObject @params = new JObject();
                if (redisSrvConfig.Params != null)
                {
                    if (redisSrvConfig.Params.Count > 0)
                    {
                        @params = ParamSub(provider.Id, redisSrvConfig.Params, @params);
                    }
                }
                string key = @params["key"].ToString();

                //3.进行访问
                switch (redisSrvConfig.ExecType)
                {
                    case ExecuteType.Query:
                        return RepModel.Success(_redis.Get(key));

                    case ExecuteType.Insert:
                        return RepModel.Success(_redis.Set(key, @params[key].ToString()));

                    case ExecuteType.Modify:
                        return RepModel.Success(_redis.Set(key, @params[key].ToString()));

                    case ExecuteType.Remove:
                        return RepModel.Success(_redis.Remove(key));

                    default:
                        return RepModel.Error("提供数据库执行类型错误！");
                }
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(provider.Id, "RedisSrvAccess", "Redis服务访问出错", ex));
            }
        }
    }
}