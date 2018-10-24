using System;
using Newtonsoft.Json;

namespace Geone.Utiliy.Library
{
    public class LogAssist
    {
        private static LogShow GetExLog(LogShow show, Exception ex)
        {
            if (ex != null)
            {
                if (show == null)
                    show = new LogShow();

                show.错误信息 = $"{ex.Message}";
                show.错误来源 = $"{ex.Source}";
                show.堆栈跟踪 = $"{ex.StackTrace}";

                if (ex.InnerException != null)
                {
                    show.内部异常 = $"{ex.InnerException.StackTrace}";
                }
            }
            return show;
        }

        private static LogShow GetLog(LogType type, string msg, LogLevel level, double code)
        {
            LogShow show = new LogShow()
            {
                日志类型 = $"{type}",
                产生时间 = $"{DateTime.Now.ToLocalTime().ToString()} ",
                日志信息 = msg,
                日志级别 = $"{level}",
                流程编码 = code.ToString()
            };

            return show;
        }

        public static LogShow WriteTrack(dynamic info, string msg)
        {
            LogShow show = GetLog(LogType.操作日志, msg, LogLevel.跟踪, 200.1);
            show.记录数据 = JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            return show;
        }

        public static LogShow WriteDebug(dynamic info, string msg)
        {
            LogShow show = GetLog(LogType.操作日志, msg, LogLevel.调试, 200.1);
            show.记录数据 = JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
            return show;
        }

        public static LogShow WriteSql(string conn, string @event, string sql, string msg)
        {
            LogShow show = GetLog(LogType.操作日志, msg, LogLevel.调试, 200.1);
            var info = new { 连接配置 = conn, 操作语句 = sql };
            show.事件名称 = @event;
            show.记录数据 = JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            return show;
        }

        public static LogShow WriteNoSql(string conn, string @event, string msg)
        {
            LogShow show = GetLog(LogType.操作日志, msg, LogLevel.调试, 200.1);
            var info = new { 连接配置 = conn };
            show.事件名称 = @event;
            show.记录数据 = JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            return show;
        }

        public static LogShow WriteInfo(dynamic info, string msg)
        {
            LogShow show = GetLog(LogType.操作日志, msg, LogLevel.信息, 200.1);
            show.记录数据 = JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            return show;
        }

        public static LogShow WriteLogin(string appid, string account, string msg)
        {
            LogShow show = GetLog(LogType.操作日志, msg, LogLevel.信息, 200.1);
            var info = new { 应用 = appid, 帐号 = account };
            show.记录数据 = JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            return show;
        }

        public static LogShow WriteWarn(dynamic info, string msg)
        {
            LogShow show = GetLog(LogType.操作日志, msg, LogLevel.警告, 200.1);
            show.记录数据 = JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            return show;
        }

        public static LogShow WriteException(string msg, Exception ex, double code)
        {
            LogShow show = GetLog(LogType.错误日志, msg, LogLevel.错误, code);

            show = GetExLog(show, ex);

            return show;
        }

        public static LogShow WriteAccessException(string @event, string msg, Exception ex, double code)
        {
            LogShow show = GetLog(LogType.错误日志, msg, LogLevel.错误, code);

            show = GetExLog(show, ex);
            show.事件名称 = @event;

            return show;
        }

        public static LogShow WriteServiceException(string srvid, string @event, string msg, Exception ex, double code)
        {
            LogShow show = GetLog(LogType.错误日志, msg, LogLevel.错误, code);

            show = GetExLog(show, ex);
            show.事件名称 = @event;
            show.记录数据 = srvid;

            return show;
        }

        public static LogShow WriteHttpException(dynamic context, string msg, Exception ex, double code)
        {
            LogShow show = GetLog(LogType.错误日志, msg, LogLevel.错误, code);

            show = GetExLog(show, ex);
            var info = new { 路由 = context.Request.Path.Value, 主机 = context.Request.Host.Value };
            show.记录数据 = JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            return show;
        }

        public static LogShow WriteNancyException(dynamic context, string msg, Exception ex, double code)
        {
            LogShow show = GetLog(LogType.错误日志, msg, LogLevel.错误, code);

            show = GetExLog(show, ex);
            var info = new { 路由 = context.Request.Url, 主机 = context.Request.UserHostAddress };
            show.记录数据 = JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            return show;
        }

        public static LogShow WriteSqlException(string conn, string @event, string sql, string msg, Exception ex, double code)
        {
            LogShow show = GetLog(LogType.错误日志, msg, LogLevel.错误, code);

            show = GetExLog(show, ex);
            var info = new { 连接配置 = conn, 操作语句 = sql };
            show.事件名称 = @event;
            show.记录数据 = JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            return show;
        }

        public static LogShow WriteNoSqlException(string conn, string @event, string msg, Exception ex, double code)
        {
            LogShow show = GetLog(LogType.错误日志, msg, LogLevel.错误, code);

            show = GetExLog(show, ex);
            var info = new { 连接配置 = conn };
            show.事件名称 = @event;
            show.记录数据 = JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            return show;
        }

        public static LogShow WriteSerious(string msg, Exception ex, double code)
        {
            LogShow show = GetLog(LogType.错误日志, msg, LogLevel.严重, code);

            show = GetExLog(show, ex);

            return show;
        }
    }
}