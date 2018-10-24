using Geone.Utiliy.Library;
using Nancy;
using System;

namespace Geone.Utiliy.Build
{
    /// <summary>
    /// NancyFx Bootstrapper设定
    /// </summary>
    public static class NancySettings
    {
        //全局前置钩子
        public static dynamic AddItemToStartOfPipeline(NancyContext context)
        {
            //do sth.
            return context.Response;
        }

        //全局后置钩子
        public static dynamic AddItemToEndOfPipeline(NancyContext context)
        {
            //do sth.
            HeaderSettings.AddHeaders(context);

            return context.Response;
        }

        //全局异常信息 ErrorAssist HeaderAssist
        public static dynamic Error(NancyContext context, Exception ex, ILogWriter _log)
        {
            LogShow log = _log.WriteNancyException(context, "发生内部错误，请查阅日志。", ex);
            context.Response = RepModel.ErrorAsJson(log);
            HeaderSettings.AddHeaders(context);

            return context.Response;
        }
    }
}