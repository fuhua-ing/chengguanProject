using Geone.Utiliy.Library;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using System;

namespace Geone.Utiliy.Build
{
    /// <summary>
    /// 配置文件，强类型限定
    /// </summary>
    public static class RpcMiddleware
    {
        public static bool? Flag { get; set; }

        public static IApplicationBuilder UseRpc(
            this IApplicationBuilder builder, IRpcAccess access, ILogWriter _log)
        {
            return builder.Use(async (context, next) =>
            {
                try
                {
                    //开启RPC支持
                    AppInitialize Init = AppConfig.Init;
                    if (Flag == null || Flag == false)
                    {
                        Server server = access.GetServer(Init.Host, (int)Init.RpcPort);
                        access.Start(server);
                        Flag = true;
                    }
                }
                catch (Exception ex)
                {
                    _log.WriteAccessException("UseRpc", "远程调用开启失败", ex);
                    Flag = false;
                }

                await next.Invoke();
            });
        }
    }
}