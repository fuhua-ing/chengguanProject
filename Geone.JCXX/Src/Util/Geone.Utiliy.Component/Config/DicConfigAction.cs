using Geone.Utiliy.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Geone.Utiliy.Component
{
    public class DicConfigAction : IConfigAction
    {
        private string filepath { get; set; }
        private string fileformat = ".json";

        private IJsonFileAccess fileAccess;

        public DicConfigAction(IJsonFileAccess _fileAccess)
        {
            filepath = Directory.GetCurrentDirectory() + @"\wwwroot\json";
            fileAccess = _fileAccess;
        }

        public void Init(string path)
        {
            filepath = path;
        }

        public T GetModel<T>(string name, string id) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            Dictionary<string, T> config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            if (config == null) config = new Dictionary<string, T>();

            List<T> values = config.Values.ToList();

            T value = values.Where(p => p.Id == id && p.Isdelete == false).FirstOrDefault();

            return value;
        }

        public List<T> GetModels<T>(string name, string[] ids) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            Dictionary<string, T> config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            if (config == null) config = new Dictionary<string, T>();

            List<T> values = config.Values.ToList();

            values = values.Where(p => ids.Contains(p.Id) && p.Isdelete == false).ToList();

            if (values.Count > 0) return values;
            else return new List<T>();
        }

        public List<T> GetAll<T>(string name) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            Dictionary<string, T> config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            if (config == null) config = new Dictionary<string, T>();

            List<T> values = config.Values.ToList();

            if (values.Count > 0) return values;
            else return new List<T>();
        }

        public Page<T> GetPage<T>(string name, int pi, int ps) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            Dictionary<string, T> config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            if (config == null) config = new Dictionary<string, T>();

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
            string filename = name.ToLower() + fileformat;

            Dictionary<string, T> config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            if (config == null) config = new Dictionary<string, T>();

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
            string filename = name.ToLower() + fileformat;

            Dictionary<string, T> config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            if (config == null) config = new Dictionary<string, T>();

            if (value.Id == null)
                value.Id = Guid.NewGuid().ToString("N");

            value.Isdelete = false;

            string id = value.Id;

            if (config.ContainsKey(id)) return false;

            //新增
            Dictionary<string, dynamic> newconfig = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JsonConvert.SerializeObject(config));
            newconfig.Add(id, value);

            Dictionary<string, T> checkconfig = JsonConvert.DeserializeObject<Dictionary<string, T>>(JsonConvert.SerializeObject(newconfig));
            bool check = fileAccess.SetFullPath(filepath, filename, checkconfig);

            return check;
        }

        public bool PostModels<T>(string name, dynamic[] values) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            Dictionary<string, T> config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            if (config == null) config = new Dictionary<string, T>();

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
            bool check = fileAccess.SetFullPath(filepath, filename, checkconfig);

            return check;
        }

        public bool PutModel<T>(string name, dynamic value) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            Dictionary<string, T> config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            if (config == null) config = new Dictionary<string, T>();

            string id = value.Id;

            if (!config.ContainsKey(id)) return false;

            //修改
            Dictionary<string, dynamic> newconfig = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JsonConvert.SerializeObject(config));
            newconfig[id] = value;

            Dictionary<string, T> checkconfig = JsonConvert.DeserializeObject<Dictionary<string, T>>(JsonConvert.SerializeObject(newconfig));
            bool check = fileAccess.SetFullPath(filepath, filename, checkconfig);

            return check;
        }

        public bool PutModels<T>(string name, dynamic[] values) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            Dictionary<string, T> config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            if (config == null) config = new Dictionary<string, T>();

            Dictionary<string, dynamic> newconfig = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JsonConvert.SerializeObject(config));

            foreach (dynamic value in values)
            {
                string id = value.Id;

                if (!config.ContainsKey(id)) return false;

                //修改
                newconfig[id] = value;
            }

            Dictionary<string, T> checkconfig = JsonConvert.DeserializeObject<Dictionary<string, T>>(JsonConvert.SerializeObject(newconfig));
            bool check = fileAccess.SetFullPath(filepath, filename, checkconfig);

            return check;
        }

        public bool DeleteModel<T>(string name, string key) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            if (string.IsNullOrWhiteSpace(key)) return false;

            Dictionary<string, T> config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            if (config == null) config = new Dictionary<string, T>();

            if (!config.ContainsKey(key)) return false;

            //删除
            config.Remove(key);
            bool check = fileAccess.SetFullPath(filepath, filename, config);

            return check;
        }

        public bool DeleteModels<T>(string name, string[] keys) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            if (keys == null) return false;
            if (keys.Count() <= 0) return false;

            Dictionary<string, T> config = fileAccess.GetFullPath<Dictionary<string, T>>(filepath, filename);
            if (config == null) config = new Dictionary<string, T>();
            config = DeleteDic(config, keys);

            bool check = fileAccess.SetFullPath(filepath, filename, config);

            return check;
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
            string filename = name.ToLower() + fileformat;

            bool check = fileAccess.SetFullPath<T>(filepath, filename, null);

            return check;
        }
    }
}