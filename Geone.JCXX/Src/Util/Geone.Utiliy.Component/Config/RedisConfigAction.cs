using Geone.Utiliy.Library;
using Geone.Utiliy.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Geone.Utiliy.Component
{
    public class RedisConfigAction : IConfigAction
    {
        private string filepath { get; set; }
        private string fileformat = ".json";

        private IJsonFileAccess fileAccess;
        private IRedisAction redisAction;

        public RedisConfigAction(IJsonFileAccess _fileAccess, IRedisAction _redisAction)
        {
            filepath = Directory.GetCurrentDirectory() + @"\wwwroot\json";
            fileAccess = _fileAccess;
            redisAction = _redisAction;
        }

        public void Init(string path)
        {
            filepath = path;
            redisAction.SetName(path);
        }

        private Dictionary<string, T> GetKeyValuePairs<T>(string name)
        {
            Dictionary<string, T> config = redisAction.ExeRedisAction((db) =>
            {
                string key = "config_" + name;
                return db.Get<Dictionary<string, T>>(key);
            });

            if (config == null)
            {
                string filename = name.ToLower() + fileformat;
                config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            }

            if (config == null) config = new Dictionary<string, T>();

            return config;
        }

        private bool SetKeyValuePairs<T>(string name, Dictionary<string, T> config)
        {
            bool? check1 = redisAction.ExeRedisAction((db) =>
            {
                string key = "config_" + name;
                return db.Set(key, config);
            });

            string filename = name.ToLower() + fileformat;
            bool check2 = fileAccess.SetFullPath(filepath, filename, config);

            if (check1 != null)
            {
                if ((bool)check1 && check2)
                    return true;
            }

            return false;
        }

        public T GetModel<T>(string name, string id) where T : BaseConfig
        {
            Dictionary<string, T> config = redisAction.ExeRedisAction((db) =>
            {
                string key = "config_" + name;
                return db.Get<Dictionary<string, T>>(key);
            });

            if (config == null)
            {
                string filename = name.ToLower() + fileformat;
                config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            }

            if (config == null) config = new Dictionary<string, T>();

            List<T> values = config.Values.ToList();
            T value = values.Where(p => p.Id == id && p.Isdelete == false).FirstOrDefault();
            return value;
        }

        public List<T> GetModels<T>(string name, string[] ids) where T : BaseConfig
        {
            Dictionary<string, T> config = GetKeyValuePairs<T>(name);

            List<T> values = config.Values.ToList();

            values = values.Where(p => ids.Contains(p.Id) && p.Isdelete == false).ToList();

            if (values.Count > 0) return values;
            else return new List<T>();
        }

        public List<T> GetAll<T>(string name) where T : BaseConfig
        {
            Dictionary<string, T> config = GetKeyValuePairs<T>(name);

            List<T> values = config.Values.ToList();

            if (values.Count > 0) return values;
            else return new List<T>();
        }

        public Page<T> GetPage<T>(string name, int pi, int ps) where T : BaseConfig
        {
            Dictionary<string, T> config = GetKeyValuePairs<T>(name);

            List<T> values = config.Values.Where(p => p.Isdelete == false).ToList();

            if (values.Count > 0)
            {
                Page<T> page = JConvert.GetPage(pi, ps, values);
                return page;
            }
            else return new Page<T>();
        }

        public Page<T> GetPageEx<T>(string name, int pi, int ps, string content) where T : BaseConfig
        {
            Dictionary<string, T> config = GetKeyValuePairs<T>(name);

            List<T> values = config.Values.Where(p => p.Isdelete == false).ToList();

            List<T> res = new List<T>();
            if (string.IsNullOrEmpty(content))
                res = values;
            else
            {
                foreach (T t in values)
                {
                    Type type = t.GetType();
                    //反射获取键名集合
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        object Value = property.GetValue(t);

                        if (Value == null)
                            Value = "";

                        if (Value.ToString().Contains(content))
                            res.Add(t);
                    }
                }
            }

            if (res.Count > 0)
            {
                res = res.Distinct().ToList();
                Page<T> page = JConvert.GetPage(pi, ps, res);
                return page;
            }
            else return new Page<T>();
        }

        public bool PostModel<T>(string name, dynamic value) where T : BaseConfig
        {
            Dictionary<string, T> config = GetKeyValuePairs<T>(name);

            if (value.Id == null)
                value.Id = Guid.NewGuid().ToString("N");

            value.Isdelete = false;

            string id = value.Id;

            if (config.ContainsKey(id)) return false;

            //新增
            Dictionary<string, dynamic> newconfig = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JsonConvert.SerializeObject(config));
            newconfig.Add(id, value);

            Dictionary<string, T> checkconfig = JsonConvert.DeserializeObject<Dictionary<string, T>>(JsonConvert.SerializeObject(newconfig));

            return SetKeyValuePairs(name, checkconfig);
        }

        public bool PostModels<T>(string name, dynamic[] values) where T : BaseConfig
        {
            Dictionary<string, T> config = GetKeyValuePairs<T>(name);

            Dictionary<string, dynamic> newconfig = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JsonConvert.SerializeObject(config));

            foreach (dynamic value in values)
            {
                if (value.Id == null)
                    value.Id = Guid.NewGuid().ToString("N");

                value.Isdelete = false;

                string id = value.Id;

                if (config.ContainsKey(id)) return false;

                //新增
                newconfig.Add(id, value);
            }

            Dictionary<string, T> checkconfig = JsonConvert.DeserializeObject<Dictionary<string, T>>(JsonConvert.SerializeObject(newconfig));

            return SetKeyValuePairs(name, checkconfig);
        }

        public bool PutModel<T>(string name, dynamic value) where T : BaseConfig
        {
            Dictionary<string, T> config = GetKeyValuePairs<T>(name);

            string id = value.Id;

            if (!config.ContainsKey(id)) return false;

            //修改
            Dictionary<string, dynamic> newconfig = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JsonConvert.SerializeObject(config));
            newconfig[id] = value;

            Dictionary<string, T> checkconfig = JsonConvert.DeserializeObject<Dictionary<string, T>>(JsonConvert.SerializeObject(newconfig));

            return SetKeyValuePairs(name, checkconfig);
        }

        public bool PutModels<T>(string name, dynamic[] values) where T : BaseConfig
        {
            Dictionary<string, T> config = GetKeyValuePairs<T>(name);

            Dictionary<string, dynamic> newconfig = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JsonConvert.SerializeObject(config));

            foreach (dynamic value in values)
            {
                string id = value.Id;

                if (!config.ContainsKey(id)) return false;

                //修改
                newconfig[id] = value;
            }

            Dictionary<string, T> checkconfig = JsonConvert.DeserializeObject<Dictionary<string, T>>(JsonConvert.SerializeObject(newconfig));

            return SetKeyValuePairs(name, checkconfig);
        }

        public bool DeleteModel<T>(string name, string key) where T : BaseConfig
        {
            if (string.IsNullOrWhiteSpace(key)) return false;

            Dictionary<string, T> config = GetKeyValuePairs<T>(name);

            if (!config.ContainsKey(key)) return false;

            //删除
            config.Remove(key);

            return SetKeyValuePairs(name, config);
        }

        public bool DeleteModels<T>(string name, string[] keys) where T : BaseConfig
        {
            if (keys == null) return false;
            if (keys.Count() <= 0) return false;

            Dictionary<string, T> config = GetKeyValuePairs<T>(name);

            config = DeleteDic(config, keys);

            return SetKeyValuePairs(name, config);
        }

        private static Dictionary<string, T> DeleteDic<T>(Dictionary<string, T> config, string[] keys)
        {
            foreach (string key in keys)
            {
                if (!config.ContainsKey(key)) continue;
                else
                {
                    //删除
                    config.Remove(key);
                    return DeleteDic(config, keys);
                }
            }
            return config;
        }

        public bool Empty<T>(string name) where T : BaseConfig
        {
            return SetKeyValuePairs<T>(name, null);
        }
    }
}