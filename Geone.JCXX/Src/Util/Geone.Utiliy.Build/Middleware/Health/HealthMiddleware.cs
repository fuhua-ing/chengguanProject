using Geone.Utiliy.Library;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Geone.Utiliy.Build
{
    public static class HealthMiddleware
    {
        public static IApplicationBuilder UseHealth(
            this IApplicationBuilder builder)
        {
            return builder.Use(async (context, next) =>
            {
                string path = context.Request.Path.Value;

                if (path.Contains("/health"))
                {
                    HeaderSettings.AddHeaders(context);
                    await context.Response.WriteAsync(RepModel.SuccessAsJson(true));
                }
                else
                    await next.Invoke();
            });
        }
    }
}