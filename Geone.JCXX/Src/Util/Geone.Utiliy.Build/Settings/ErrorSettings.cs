using Geone.Utiliy.Library;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;

namespace Geone.Utiliy.Build
{
    public class ErrorSettings
    {
        //全局异常信息
        public static void Error(IApplicationBuilder builder, ILogWriter _log)
        {
            builder.Run(async context =>
            {
                Exception ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                LogShow log = _log.WriteHttpException(context, "发生内部错误，请查阅日志。", ex);
                HeaderSettings.AddHeaders(context);

                await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
            });
        }
    }
}