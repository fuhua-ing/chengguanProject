
using System;
using System.IO;

namespace Geone.Utiliy.Library
{
    /// <summary>
    /// 文件的读写
    /// </summary>
    public class FileReadWrite : IFileReadWrite
    {
        private ILogWriter _log;

        public FileReadWrite(ILogWriter log)
        {
            _log = log;
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="file">文件名</param>
        /// <returns></returns>
        public string Read(string path, string file)
        {
            FileStream fs = null;
            StreamReader sr = null;

            try
            {
                //验证路径是否存在,不存在则创建
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                if (!path.EndsWith(@"\")) path += @"\";
                file = path + file;
                //读取文件
                fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fs);

                string CacheStr = sr.ReadToEnd();
                sr.Close();
                fs.Close();

                return CacheStr;
            }
            catch (Exception ex)
            {
                _log.WriteAccessException("Read", "读取文件发生错误", ex);
                return null;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="file">文件名</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public bool Write(string path, string file, string content)
        {
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                //验证路径是否存在,不存在则创建
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                if (!path.EndsWith(@"\")) path += @"\";
                //验证文件是否存在，有则替换，无则创建
                file = path + file;
                fs = new FileStream(file, FileMode.Create, FileAccess.Write);

                sw = new StreamWriter(fs);
                sw.WriteLine(content);

                sw.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                _log.WriteAccessException("Write", "写入文件发生错误", ex);
                return false;
            }
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
    }
}