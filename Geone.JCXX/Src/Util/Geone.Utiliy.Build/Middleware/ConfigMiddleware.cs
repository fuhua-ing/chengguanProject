using Geone.Utiliy.Component;
using Geone.Utiliy.Library;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Geone.Utiliy.Build
{
    /// <summary>
    /// 配置文件，强类型限定
    /// </summary>
    public static class ConfigMiddleware
    {
        public static IApplicationBuilder UseConfiguration(
            this IApplicationBuilder builder, IConfiguration config, IRpcAccess access, ILogWriter _log)
        {
            return builder.Use(async (context, next) =>
            {
                AppConfig.Set = config.Get<AppSetting>();
                HostString str = context.Request.Host;
                AppConfig.Set.Host = str.Host;
                AppConfig.Set.Port = str.Port;

                if (AppConfig.Set == null)
                {
                    LogShow log = _log.WriteException("AppSetting-App缺失，构建失败。", null, 401.1);
                    HeaderSettings.AddHeaders(context);
                    await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                }
                else
                {
                    if (AppConfig.Set.RpcPort == null)
                        AppConfig.Set.RpcPort = PortHelper.GetFirstAvailablePort(62510, 65500);

                    if ((bool)AppConfig.Set.IsGovern)
                    {
                        if (AppConfig.Set.IsGovern == null)
                        {
                            LogShow log = _log.WriteException("AppSetting-Build缺失，构建失败。", null, 401.1);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }
                        else
                        {
                            IBuild build = access.GetService<IBuild>(AppConfig.Set.ConnectHost, (int)AppConfig.Set.ConnectPort);
                            AppConfig.Init = build.ServiceRegister(AppConfig.Set).ResponseAsync.Result;

                            if (AppConfig.Init == null)
                            {
                                HeaderSettings.AddHeaders(context);
                                await context.Response.WriteAsync(RepModel.ErrorAsJson("注册服务失败，未成功初始化。"));
                            }
                            else
                            {
                                if (AppConfig.Init.Host == null)
                                    AppConfig.Init.Host = AppConfig.Set.Host;
                                if (AppConfig.Init.Port == null)
                                    AppConfig.Init.Port = AppConfig.Set.Port;
                                if (AppConfig.Init.RpcPort == null)
                                    AppConfig.Init.RpcPort = AppConfig.Set.RpcPort;

                                await next.Invoke();
                            }
                        }
                    }
                    else
                    {
                        AppConfig.Init = config.Get<AppInitialize>();
                        if (AppConfig.Init.Host == null)
                            AppConfig.Init.Host = AppConfig.Set.Host;
                        if (AppConfig.Init.Port == null)
                            AppConfig.Init.Port = AppConfig.Set.Port;
                        if (AppConfig.Init.RpcPort == null)
                            AppConfig.Init.RpcPort = AppConfig.Set.RpcPort;

                        await next.Invoke();
                    }
                }
            });
        }
    }
}