using Geone.Utiliy.Library;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Geone.Utiliy.Component
{
    public class RpcConfigAction : IConfigAction
    {
        private static IConfig config;
        private IRpcAccess access;

        public RpcConfigAction(IRpcAccess rpcAccess)
        {
            access = rpcAccess;
            if (config == null)
            {
                AppInitialize initialize = AppConfig.Init;
                IBuild build = access.GetService<IBuild>(AppConfig.Set.ConnectHost, (int)AppConfig.Set.ConnectPort);
                AppInitialize Init = build.ServiceObtainByName("GeneralConfig").ResponseAsync.Result;
                if (Init != null && Init.Host != null && Init.RpcPort != null)
                    config = access.GetService<IConfig>(Init.Host, (int)Init.RpcPort);
            }
        }

        public void Init(string hostpost)
        {
            string[] hostposts = hostpost.Split(':');

            config = access.GetService<IConfig>(hostposts[0], int.Parse(hostposts[1]));
        }

        public T GetModel<T>(string name, string id) where T : BaseConfig
        {
            RepModel res = config.GetModel(RpcReq.Create(name, id)).ResponseAsync.Result;

            if (res.Data == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(res.Data);
        }

        public List<T> GetModels<T>(string name, string[] ids) where T : BaseConfig
        {
            RepModel res = config.GetModels(RpcReq.Create(name, ids)).ResponseAsync.Result;

            if (res.Data == null)
            {
                return new List<T>();
            }

            return JsonConvert.DeserializeObject<List<T>>(res.Data);
        }

        public List<T> GetAll<T>(string name) where T : BaseConfig
        {
            RepModel res = config.GetAll(RpcReq.Create(name)).ResponseAsync.Result;

            if (res.Data == null)
            {
                return new List<T>();
            }

            return JsonConvert.DeserializeObject<List<T>>(res.Data);
        }

        public Page<T> GetPage<T>(string name, int pi, int ps) where T : BaseConfig
        {
            TableParam table = new TableParam()
            {
                pi = pi,
                ps = ps
            };

            RepModel res = config.GetPage(RpcReq.Create(name, table)).ResponseAsync.Result;

            return JsonConvert.DeserializeObject<Page<T>>(res.Data);
        }

        public Page<T> GetPageEx<T>(string name, int pi, int ps, string content) where T : BaseConfig
        {
            TableParamEx table = new TableParamEx()
            {
                pi = pi,
                ps = ps,
                content = content
            };

            RepModel res = config.GetPageEx(RpcReq.Create(name, table)).ResponseAsync.Result;

            return JsonConvert.DeserializeObject<Page<T>>(res.Data);
        }

        public bool PostModel<T>(string name, dynamic value) where T : BaseConfig
        {
            DataParam data = new DataParam()
            {
                data = value
            };

            RepModel res = config.PostModel(RpcReq.Create(name, data)).ResponseAsync.Result;

            if (res.StatusCode == 200)
                return true;
            else
                return false;
        }

        public bool PostModels<T>(string name, dynamic[] values) where T : BaseConfig
        {
            DataParam data = new DataParam()
            {
                datas = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(values)).ToArray()
            };

            RepModel res = config.PostModels(RpcReq.Create(name, data)).ResponseAsync.Result;

            if (res.StatusCode == 200)
                return true;
            else
                return false;
        }

        public bool PutModel<T>(string name, dynamic value) where T : BaseConfig
        {
            DataParam data = new DataParam()
            {
                data = value
            };

            RepModel res = config.PutModel(RpcReq.Create(name, data)).ResponseAsync.Result;

            if (res.StatusCode == 200)
                return true;
            else
                return false;
        }

        public bool PutModels<T>(string name, dynamic[] values) where T : BaseConfig
        {
            DataParam data = new DataParam()
            {
                datas = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(values)).ToArray()
            };

            RepModel res = config.PutModels(RpcReq.Create(name, data)).ResponseAsync.Result;

            if (res.StatusCode == 200)
                return true;
            else
                return false;
        }

        public bool DeleteModel<T>(string name, string key) where T : BaseConfig
        {
            DeleteParam delete = new DeleteParam()
            {
                key = key
            };

            RepModel res = config.DeleteModel(RpcReq.Create(name, delete)).ResponseAsync.Result;

            if (res.StatusCode == 200)
                return true;
            else
                return false;
        }

        public bool DeleteModels<T>(string name, string[] keys) where T : BaseConfig
        {
            DeleteParam delete = new DeleteParam()
            {
                keys = keys
            };

            RepModel res = config.DeleteModels(RpcReq.Create(name, delete)).ResponseAsync.Result;

            return res.Data;
        }

        public bool Empty<T>(string name) where T : BaseConfig
        {
            RepModel res = config.Empty(RpcReq.Create(name)).ResponseAsync.Result;

            if (res.StatusCode == 200)
                return true;
            else
                return false;
        }
    }
}