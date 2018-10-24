using Geone.Utiliy.Library;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Geone.Utiliy.Component
{
    public class ConfigExce : IConfigExce
    {
        private ILogWriter _log;
        private IJsonFileAccess _fileAccess;
        private IConfigAction _action;
        private string path;

        public ConfigExce(ILogWriter log, IJsonFileAccess fileAccess, IConfigAction configAction)
        {
            _log = log;
            _fileAccess = fileAccess;
            _action = configAction;
            path = _fileAccess.GetValue(Directory.GetCurrentDirectory(), "appsettings.json", "ConfigPath");
        }

        public RepModel Exce(string name, string meathod, object[] @params)
        {
            try
            {
                //获取类类型
                Type type = typeof(IConfigStorage).Assembly.GetTypes().Where(t => t.Name.ToLower() == name.ToLower()).FirstOrDefault();

                //创建服务类
                if (path != null)
                    _action.Init(path);

                //反射创建方法
                MethodInfo mi = _action.GetType().GetMethod(meathod);

                //反射对象调用方法，MakeGenericMethod函数用以泛型类型T
                object resource = mi.MakeGenericMethod(new Type[] { type }).Invoke(_action, @params);

                if (resource == null) return RepModel.Error(_log.WriteException("获取配置发生错误，返回值为空！"));

                return RepModel.Success(resource);
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteException("获取配置发生错误！", ex));
            }
        }
    }
}