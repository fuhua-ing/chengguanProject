using Microsoft.AspNetCore.Http;
using Nancy;

namespace Geone.Utiliy.Build
{
    public class HeaderSettings
    {
        //添加通用头部
        public static void AddHeaders(HttpContext context)
        {
            #region 跨域

            //context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //context.Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS");
            //context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, Accept, X-Requested-With, Content-Type, Authorization, Token, Ticket, AppId, Identity");

            #endregion 跨域

            context.Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        }

        //添加通用头部
        public static void AddHeaders(NancyContext context)
        {
            #region 跨域

            //context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //context.Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS");
            //context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, Accept, X-Requested-With, Content-Type, Authorization, Token, Ticket, AppId, Identity");

            #endregion 跨域

            context.Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        }
    }
}