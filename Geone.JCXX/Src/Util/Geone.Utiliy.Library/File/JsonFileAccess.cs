
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;

namespace Geone.Utiliy.Library
{
    /// <summary>
    /// 读取Json文件
    /// </summary>
    public class JsonFileAccess : IJsonFileAccess
    {
        private IFileReadWrite _write;
        private ILogWriter _log;

        public JsonFileAccess(IFileReadWrite write, ILogWriter log)
        {
            _write = write;
            _log = log;
        }

        #region 私有方法

        private string GetPath(params string[] Sections)
        {
            string Path = string.Empty;

            if (Sections != null)
            {
                if (Sections.Length > 0)
                {
                    foreach (string Section in Sections)
                    {
                        Path += Section + ":";
                    }

                    Path = Path.Substring(0, Path.Length - 1);
                }
            }

            return Path;
        }

        #endregion 私有方法

        public string GetValue(string path, string file, params string[] Sections)
        {
            try
            {
                string Path = GetPath(Sections);

                var Root = new ConfigurationBuilder()
                                .SetBasePath(path)
                                .AddJsonFile(file)
                                .Build();

                if (string.IsNullOrWhiteSpace(Path))
                {
                    return default;
                }

                return Root.GetSection(Path).Value;
            }
            catch (Exception ex)
            {
                _log.WriteAccessException("GetValue", "读取Json文件发生错误", ex);
                return default;
            }
        }

        public T GetFullPath<T>(string path, string file, params string[] Sections)
        {
            try
            {
                string Path = GetPath(Sections);

                var Root = new ConfigurationBuilder()
                                .SetBasePath(path)
                                .AddJsonFile(file)
                                .Build();

                if (string.IsNullOrWhiteSpace(Path))
                {
                    return Root.Get<T>();
                }

                return Root.GetSection(Path).Get<T>();
            }
            catch (Exception ex)
            {
                _log.WriteAccessException("GetFullPath", "读取Json文件发生错误", ex);
                return default;
            }
        }

        public T GetFullRoot<T>(string path, string file, params string[] Sections)
        {
            try
            {
                string Path = GetPath(Sections);

                if (Path == string.Empty)
                {
                    Path = "data";
                }
                else
                {
                    Path += "data:";
                }

                var Root = new ConfigurationBuilder()
                                .SetBasePath(path)
                                .AddJsonFile(file)
                                .Build();

                if (string.IsNullOrWhiteSpace(Path))
                {
                    return Root.Get<T>();
                }

                return Root.GetSection(Path).Get<T>();
            }
            catch (Exception ex)
            {
                _log.WriteAccessException("GetFullRoot", "读取Json文件发生错误", ex);
                return default;
            }
        }

        public bool SetValue(string path, string file, string value)
        {
            try
            {
                return _write.Write(path, file, value);
            }
            catch (Exception ex)
            {
                _log.WriteAccessException("SetValue", "写入Json文件发生错误", ex);
                return false;
            }
        }

        public bool SetFullPath<T>(string path, string file, T model)
        {
            try
            {
                return _write.Write(path, file, JsonConvert.SerializeObject(model));
            }
            catch (Exception ex)
            {
                _log.WriteAccessException("SetFullPath", "写入Json文件发生错误", ex);
                return false;
            }
        }

        public bool SetFullRoot<T>(string path, string file, T model)
        {
            try
            {
                var data = new { data = model };
                return _write.Write(path, file, JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                _log.WriteAccessException("SetFullRoot", "写入Json文件发生错误", ex);
                return false;
            }
        }
    }
}