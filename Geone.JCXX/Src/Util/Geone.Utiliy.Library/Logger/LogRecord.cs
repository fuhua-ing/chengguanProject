using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System.Collections.Generic;
using System.IO;

namespace Geone.Utiliy.Library
{

    /// <summary>
    /// 本地文件日志记录，支持远程读取，建议用于开发阶段
    /// </summary>
    public class FileSimpleLogRecord : ILogRecord
    {
        public void RecordLog<TLog>(TLog log, params string[] code)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                //获取写入路径与文件名
                string filepath = Directory.GetCurrentDirectory() + "\\wwwroot\\log";
                string filename = $"log_DevelopmentSimple_" + string.Join("_", code);
                filename = filename.ToLower() + ".json";

                List<TLog> newconfig = new List<TLog>();
                try
                {
                    //如有文件则先读取
                    var Root = new ConfigurationBuilder()
                                .SetBasePath(filepath)
                                .AddJsonFile(filename)
                                .Build();

                    List<TLog> config = Root.GetSection("data").Get<List<TLog>>();
                    newconfig = config;
                }
                catch { }
                newconfig.Add(log);
                var data = new { data = newconfig };

                //验证路径是否存在,不存在则创建
                if (!Directory.Exists(filepath)) Directory.CreateDirectory(filepath);
                if (!filepath.EndsWith(@"\")) filepath += @"\";
                //验证文件是否存在，有则替换，无则创建
                filename = filepath + filename;
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write);

                sw = new StreamWriter(fs);
                sw.WriteLine(JsonConvert.SerializeObject(data));

                sw.Close();
                fs.Close();
            }
            catch { }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        public List<TLog> ReadLog<TLog>(params string[] code)
        {
            List<TLog> newconfig = new List<TLog>();

            try
            {
                //获取写入路径与文件名
                string filepath = Directory.GetCurrentDirectory() + "\\wwwroot\\log";
                string filename = $"log_DevelopmentSimple_" + string.Join("_", code);
                filename = filename.ToLower() + ".json";

                //如有文件则先读取
                var Root = new ConfigurationBuilder()
                            .SetBasePath(filepath)
                            .AddJsonFile(filename)
                            .Build();

                List<TLog> config = Root.GetSection("data").Get<List<TLog>>();
                newconfig = config;
            }
            catch { }

            return newconfig;
        }
    }


    /// <summary>
    /// 本地文件日志记录，支持远程读取，建议用于开发阶段
    /// </summary>
    public class FileLogRecord : ILogRecord
    {
        public void RecordLog<TLog>(TLog log, params string[] code)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                //获取写入路径与文件名
                string filepath = Directory.GetCurrentDirectory() + "\\wwwroot" + AppConfig.Init.Log.LogPath;
                string filename = $"log_{AppConfig.Init.Log.LogKey}_" + string.Join("_", code);
                filename = filename.ToLower() + ".json";

                List<TLog> newconfig = new List<TLog>();
                try
                {
                    //如有文件则先读取
                    var Root = new ConfigurationBuilder()
                                .SetBasePath(filepath)
                                .AddJsonFile(filename)
                                .Build();

                    List<TLog> config = Root.GetSection("data").Get<List<TLog>>();
                    newconfig = config;
                }
                catch { }
                newconfig.Add(log);
                var data = new { data = newconfig };

                //验证路径是否存在,不存在则创建
                if (!Directory.Exists(filepath)) Directory.CreateDirectory(filepath);
                if (!filepath.EndsWith(@"\")) filepath += @"\";
                //验证文件是否存在，有则替换，无则创建
                filename = filepath + filename;
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write);

                sw = new StreamWriter(fs);
                sw.WriteLine(JsonConvert.SerializeObject(data));

                sw.Close();
                fs.Close();
            }
            catch { }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        public List<TLog> ReadLog<TLog>(params string[] code)
        {
            List<TLog> newconfig = new List<TLog>();

            try
            {
                //获取写入路径与文件名
                string filepath = Directory.GetCurrentDirectory() + "\\wwwroot" + AppConfig.Init.Log.LogPath;
                string filename = $"log_{AppConfig.Init.Log.LogKey}_" + string.Join("_", code);
                filename = filename.ToLower() + ".json";

                //如有文件则先读取
                var Root = new ConfigurationBuilder()
                            .SetBasePath(filepath)
                            .AddJsonFile(filename)
                            .Build();

                List<TLog> config = Root.GetSection("data").Get<List<TLog>>();
                newconfig = config;
            }
            catch { }

            return newconfig;
        }
    }

    /// <summary>
    /// 数据库日志记录，支持管理系统可视化，存于Reids，建议用于生产环境
    /// </summary>
    public class RedisLogRecord : ILogRecord
    {
        public void RecordLog<TLog>(TLog log, params string[] code)
        {
            try
            {
                //补全日志信息
                string value = JsonConvert.SerializeObject(log);

                string listid = $"log_{AppConfig.Init.Log.LogKey}_" + string.Join("_", code);

                using (IRedisClient db = new RedisClient(AppConfig.Init.Log.RedisHost, (int)AppConfig.Init.Log.RedisPort))
                {
                    db.AddItemToList(listid, value);
                    db.Save();
                }
            }
            catch { }
        }

        public List<TLog> ReadLog<TLog>(params string[] code)
        {
            List<TLog> newconfig = new List<TLog>();

            try
            {
                string listid = $"log_{AppConfig.Init.Log.LogKey}_" + string.Join("_", code);

                using (IRedisClient db = new RedisClient(AppConfig.Init.Log.RedisHost, (int)AppConfig.Init.Log.RedisPort))
                {
                    List<string> logstr = db.GetAllItemsFromList(listid);
                    foreach (string lstr in logstr)
                    {
                        newconfig.Add(JsonConvert.DeserializeObject<TLog>(lstr));
                    }
                }
            }
            catch { }

            return newconfig;
        }
    }

    /// <summary>
    /// 队列日志记录，支持管理系统可视化，存于Database，建议用于生产环境
    /// </summary>
    public class QueueLogRecord : ILogRecord
    {
        public void RecordLog<TLog>(TLog log, params string[] code)
        {
            try
            {
                //补全日志信息
                string value = JsonConvert.SerializeObject(log);

                string listid = $"log_{AppConfig.Init.Log.LogKey}_" + string.Join("_", code);

                using (IRedisClient db = new RedisClient(AppConfig.Init.Log.RedisHost, (int)AppConfig.Init.Log.RedisPort))
                {
                    db.EnqueueItemOnList(AppConfig.Init.Log.LogQueue, listid + "*#*$*" + value);
                    db.Save();
                }
            }
            catch { }
        }

        public List<TLog> ReadLog<TLog>(params string[] code)
        {
            List<TLog> newconfig = new List<TLog>();

            try
            {
                string listid = $"log_{AppConfig.Init.Log.LogKey}_" + string.Join("_", code);

                using (IRedisClient db = new RedisClient(AppConfig.Init.Log.RedisHost, (int)AppConfig.Init.Log.RedisPort))
                {
                    List<string> logstr = db.GetAllItemsFromList(listid);
                    foreach (string lstr in logstr)
                    {
                        newconfig.Add(JsonConvert.DeserializeObject<TLog>(lstr));
                    }
                }
            }
            catch { }

            return newconfig;
        }
    }
}