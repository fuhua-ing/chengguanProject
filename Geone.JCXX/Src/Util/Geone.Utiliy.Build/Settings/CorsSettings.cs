using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Geone.Utiliy.Build
{
    public class CorsSettings
    {
        //添加跨域设定
        public static void AddCors(CorsPolicyBuilder builder)
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    }
}