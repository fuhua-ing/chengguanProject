using System;

namespace Geone.Utiliy.Library
{
    public class LogWriter : ILogWriter
    {
        private ILogRecord record;

        public LogWriter(ILogRecord logRecord)
        {
            record = logRecord;
        }

        private bool IsUsed(string path)
        {
            try
            {
                if (path == "Log:IsTrack")
                    return (bool)AppConfig.Init.Log.IsTrack;
                else if (path == "Log:IsDebug")
                    return (bool)AppConfig.Init.Log.IsDebug;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private void Record(LogShow show, string code = null)
        {
            int level = (int)Enum.Parse(typeof(LogLevel), show.日志级别);
            if (code != null)
                record.RecordLog(LogConfig.Convert(show), DateTime.Now.ToString("yyMMdd"), level.ToString(), code);
            else
                record.RecordLog(LogConfig.Convert(show), DateTime.Now.ToString("yyMMdd"), level.ToString());
        }

        public LogShow WriteTrack(dynamic info, string msg = "已记录")
        {
            LogShow show = LogAssist.WriteTrack(info, msg);

            if (IsUsed("Log:IsTrack"))
                Record(show);

            return show;
        }

        public LogShow WriteDebug(dynamic info, string msg = "已记录")
        {
            LogShow show = LogAssist.WriteDebug(info, msg);

            if (IsUsed("Log:IsDebug"))
                Record(show);

            return show;
        }

        public LogShow WriteSql(string conn, string @event, string sql, string msg = "已记录")
        {
            LogShow show = LogAssist.WriteSql(conn, @event, sql, msg);

            if (IsUsed("Log:IsDebug"))
                Record(show);

            return show;
        }

        public LogShow WriteNoSql(string conn, string @event, string msg = "已记录")
        {
            LogShow show = LogAssist.WriteNoSql(conn, @event, msg);

            if (IsUsed("Log:IsDebug"))
                Record(show);

            return show;
        }

        public LogShow WriteInfo(dynamic info, string msg = "已记录")
        {
            LogShow show = LogAssist.WriteInfo(info, msg);

            Record(show);

            return show;
        }

        public LogShow WriteBusiness(dynamic info, string code, string msg = "已记录")
        {
            LogShow show = LogAssist.WriteInfo(info, msg);

            Record(show, code);

            return show;
        }

        public LogShow WriteLogin(string appid, string account, string msg = "已记录")
        {
            LogShow show = LogAssist.WriteLogin(appid, account, msg);

            Record(show, "login");

            return show;
        }

        public LogShow WriteWarn(dynamic info, string msg = "已记录")
        {
            LogShow show = LogAssist.WriteWarn(info, msg);

            Record(show);

            return show;
        }

        public LogShow WriteException(string msg = "发生错误", Exception ex = null, double code = 999)
        {
            LogShow show = LogAssist.WriteException(msg, ex, code);

            Record(show);

            return show;
        }

        public LogShow WriteAccessException(string @event, string msg = "访问出错", Exception ex = null, double code = 500)
        {
            LogShow show = LogAssist.WriteAccessException(@event, msg, ex, code);

            Record(show);

            return show;
        }

        public LogShow WriteServiceException(string srvid, string @event, string msg = "访问出错", Exception ex = null, double code = 600)
        {
            LogShow show = LogAssist.WriteServiceException(srvid, @event, msg, ex, code);

            Record(show);

            return show;
        }

        public LogShow WriteHttpException(dynamic context, string msg = "访问出错", Exception ex = null, double code = 600.1)
        {
            LogShow show = LogAssist.WriteHttpException(context, msg, ex, code);

            Record(show);

            return show;
        }

        public LogShow WriteNancyException(dynamic context, string msg = "访问出错", Exception ex = null, double code = 600.1)
        {
            LogShow show = LogAssist.WriteNancyException(context, msg, ex, code);

            Record(show);

            return show;
        }

        public LogShow WriteSqlException(string conn, string @event, string sql, string msg = "访问出错", Exception ex = null, double code = 700.2)
        {
            LogShow show = LogAssist.WriteSqlException(conn, @event, sql, msg, ex, code);

            Record(show);

            return show;
        }

        public LogShow WriteNoSqlException(string conn, string @event, string msg = "访问出错", Exception ex = null, double code = 700.3)
        {
            LogShow show = LogAssist.WriteNoSqlException(conn, @event, msg, ex, code);

            Record(show);

            return show;
        }

        public LogShow WriteSerious(string msg = "发生错误", Exception ex = null, double code = 999)
        {
            LogShow show = LogAssist.WriteSerious(msg, ex, code);

            Record(show);

            return show;
        }
    }
}