using System;

namespace Geone.Utiliy.Library
{
    public interface ILogWriter
    {
        //LogLevel.跟踪-仅对于开发人员调试问题有价值的信息。包含敏感数据，生产环境情况下禁用。
        LogShow WriteTrack(dynamic info, string msg = "已记录");

        //LogLevel.调试-在开发和调试过程中短期有用的信息。不包含敏感数据，但数据无长期价值，生产环境情况下禁用。
        LogShow WriteDebug(dynamic info, string msg = "已记录");

        #region Debug扩展

        LogShow WriteSql(string conn, string @event, string sql, string msg = "已记录");

        LogShow WriteNoSql(string conn, string @event, string msg = "已记录");

        #endregion Debug扩展

        //LogLevel.信息-用于跟踪应用程序的常规流。通常有长期价值，生产环境情况下启用。
        LogShow WriteInfo(dynamic info, string msg = "已记录");

        //LogLevel.信息-用于跟踪应用程序的业务流。通常有长期价值，生产环境情况下启用。
        LogShow WriteBusiness(dynamic info, string code, string msg = "已记录");

        #region Info扩展

        LogShow WriteLogin(string appid, string account, string msg = "已记录");

        #endregion Info扩展

        //LogLevel.警告-应用程序流中的异常或意外事件。不会中断应用程序运行但仍需调查的错误或其他条件，生产环境情况下启用。
        LogShow WriteWarn(dynamic info, string msg = "已记录");

        //LogLevel.错误-无法处理的错误和异常。会中断应用程序运行，生产环境情况下启用。
        LogShow WriteException(string msg = "发生错误", Exception ex = null, double code = 999);

        #region Exception扩展

        LogShow WriteAccessException(string @event, string msg = "访问出错", Exception ex = null, double code = 500);

        LogShow WriteServiceException(string srvid, string @event, string msg = "访问出错", Exception ex = null, double code = 600);

        LogShow WriteHttpException(dynamic context, string msg = "访问出错", Exception ex = null, double code = 600.1);

        LogShow WriteNancyException(dynamic context, string msg = "访问出错", Exception ex = null, double code = 600.1);

        LogShow WriteSqlException(string conn, string @event, string sql, string msg = "访问出错", Exception ex = null, double code = 700.2);

        LogShow WriteNoSqlException(string conn, string @event, string msg = "访问出错", Exception ex = null, double code = 700.3);

        #endregion Exception扩展

        //LogLevel.严重-需要立即关注的失败。会中断应用程序运行，生产环境情况下启用。将发送信息给运维人员，慎用。
        LogShow WriteSerious(string msg = "发生错误", Exception ex = null, double code = 999);
    }
}