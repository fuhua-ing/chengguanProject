using Geone.Utiliy.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Geone.Utiliy.Component
{
    public class ListConfigAction : IConfigAction
    {
        private string filepath { get; set; }
        private string fileformat = ".json";

        private IJsonFileAccess fileAccess;

        public ListConfigAction(IJsonFileAccess _fileAccess)
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

            List<T> config = fileAccess.GetFullRoot<List<T>>(filepath, filename);
            if (config == null) config = new List<T>();

            T value = config.Where(p => p.Id == id && p.Isdelete == false).FirstOrDefault();

            return value;
        }

        public List<T> GetModels<T>(string name, string[] ids) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            List<T> config = fileAccess.GetFullRoot<List<T>>(filepath, filename);
            if (config == null) config = new List<T>();

            config = config.Where(p => ids.Contains(p.Id) && p.Isdelete == false).ToList();

            if (config.Count > 0) return config;
            else return new List<T>();
        }

        public List<T> GetAll<T>(string name) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            List<T> config = fileAccess.GetFullRoot<List<T>>(filepath, filename);
            if (config == null) config = new List<T>();

            if (config.Count > 0) return config;
            else return new List<T>();
        }

        public Page<T> GetPage<T>(string name, int pi, int ps) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            List<T> config = fileAccess.GetFullRoot<List<T>>(filepath, filename).Where(p => p.Isdelete == false).ToList();
            if (config == null) config = new List<T>();

            if (config.Count > 0)
            {
                Page<T> page = JConvert.GetPage(pi, ps, config);
                return page;
            }
            else return new Page<T>();
        }

        public Page<T> GetPageEx<T>(string name, int pi, int ps, string content) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            List<T> config = fileAccess.GetFullRoot<List<T>>(filepath, filename).Where(p => p.Isdelete == false).ToList();
            if (config == null) config = new List<T>();

            List<T> res = new List<T>();
            if (string.IsNullOrEmpty(content))
                res = config;
            else
            {
                Type type = typeof(T).GetType();
                foreach (T t in config)
                {
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

            List<T> config = fileAccess.GetFullRoot<List<T>>(filepath, filename);
            if (config == null) config = new List<T>();

            List<string> ids = config.Select(p => p.Id).ToList();

            if (value.Id == null)
                value.Id = Guid.NewGuid().ToString("N");
            value.Isdelete = false;

            if (ids.Contains(value.Id)) return false;

            //新增
            List<dynamic> newconfig = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(config));
            newconfig.Add(value);

            List<T> checkconfig = JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(newconfig));

            bool check = fileAccess.SetFullRoot(filepath, filename, checkconfig);

            return check;
        }

        public bool PostModels<T>(string name, dynamic[] values) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            List<T> config = fileAccess.GetFullRoot<List<T>>(filepath, filename);
            if (config == null) config = new List<T>();

            List<string> ids = config.Select(p => p.Id).ToList();

            List<dynamic> newconfig = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(config));

            foreach (dynamic value in values)
            {
                if (value.Id == null)
                    value.Id = Guid.NewGuid().ToString("N");
                value.Isdelete = false;

                if (ids.Contains(value.Id)) return false;

                //新增
                newconfig.Add(value);
            }

            List<T> checkconfig = JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(newconfig));

            bool check = fileAccess.SetFullRoot(filepath, filename, checkconfig);

            return check;
        }

        public bool PutModel<T>(string name, dynamic value) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            List<T> config = fileAccess.GetFullRoot<List<T>>(filepath, filename);
            if (config == null) config = new List<T>();

            List<string> ids = config.Select(p => p.Id).ToList();

            if (!ids.Contains(value.Id)) return false;

            //修改
            List<dynamic> newconfig = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(config));
            newconfig.Remove(newconfig.Where(p => p.Id == value.Id).Single());
            newconfig.Add(value);

            List<T> checkconfig = JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(newconfig));

            bool check = fileAccess.SetFullRoot(filepath, filename, checkconfig);

            return check;
        }

        public bool PutModels<T>(string name, dynamic[] values) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            List<T> config = fileAccess.GetFullRoot<List<T>>(filepath, filename);
            if (config == null) config = new List<T>();

            List<string> ids = config.Select(p => p.Id).ToList();

            List<dynamic> newconfig = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(config));

            foreach (dynamic value in values)
            {
                if (!ids.Contains(value.Id)) return false;

                //修改
                newconfig.Remove(newconfig.Where(p => p.Id == value.Id).Single());
                newconfig.Add(value);
            }

            List<T> checkconfig = JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(newconfig));

            bool check = fileAccess.SetFullRoot(filepath, filename, checkconfig);

            return check;
        }

        public bool DeleteModel<T>(string name, string key) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            if (string.IsNullOrWhiteSpace(key)) return false;

            List<T> config = fileAccess.GetFullRoot<List<T>>(filepath, filename);
            if (config == null) config = new List<T>();

            List<string> ids = config.Select(p => p.Id).ToList();

            if (!ids.Contains(key)) return false;

            config.Remove(config.Where(p => p.Id == key).Single());

            var data = new { data = config };

            bool check = fileAccess.SetFullRoot(filepath, filename, data);

            return check;
        }

        public bool DeleteModels<T>(string name, string[] keys) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            if (keys == null) return false;
            if (keys.Count() <= 0) return false;

            List<T> config = fileAccess.GetFullRoot<List<T>>(filepath, filename);
            if (config == null) config = new List<T>();

            List<string> ids = config.Select(p => p.Id).ToList();

            List<dynamic> newconfig = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(config));

            newconfig = DeleteList(newconfig, ids, keys);

            var data = new { data = newconfig };

            bool check = fileAccess.SetFullRoot(filepath, filename, data);

            return check;
        }

        private static List<dynamic> DeleteList(List<dynamic> config, List<string> ids, string[] keys)
        {
            foreach (string key in keys)
            {
                if (!ids.Contains(key)) continue;
                else
                {
                    //删除
                    config.Remove(config.Where(p => p.Id == key).Single());
                    return DeleteList(config, ids, keys);
                }
            }
            return config;
        }

        public bool Empty<T>(string name) where T : BaseConfig
        {
            string filename = name.ToLower() + fileformat;

            bool check = fileAccess.SetFullRoot<T>(filepath, filename, null);

            return check;
        }
    }
}