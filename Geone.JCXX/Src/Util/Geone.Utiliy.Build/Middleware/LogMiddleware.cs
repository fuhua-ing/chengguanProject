using Geone.Utiliy.Library;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Geone.Utiliy.Build
{
    public static class LogMiddleware
    {
        public static IApplicationBuilder UseLog(
            this IApplicationBuilder builder, ILogRecord logRecord)
        {
            return builder.Use(async (context, next) =>
            {
                string path = context.Request.Path.Value;
                string date = context.Request.HttpContext.Request.Query["date"];
                string type = context.Request.HttpContext.Request.Query["type"];
                string code = context.Request.HttpContext.Request.Query["code"];

                if (string.IsNullOrWhiteSpace(date))
                    date = DateTime.Now.ToString("yyMMdd");
                if (string.IsNullOrWhiteSpace(type))
                    type = "4";
                if (string.IsNullOrWhiteSpace(code))
                    code = null;

                if (path.Contains("/log"))
                {
                    List<LogConfig> log = new List<LogConfig>();
                    if (code != null)
                        log = logRecord.ReadLog<LogConfig>(date, type, code);
                    else
                        log = logRecord.ReadLog<LogConfig>(date, type);

                    HeaderSettings.AddHeaders(context);
                    await context.Response.WriteAsync(RepModel.SuccessAsJson(log));
                }
                else
                    await next.Invoke();
            });
        }
    }
}