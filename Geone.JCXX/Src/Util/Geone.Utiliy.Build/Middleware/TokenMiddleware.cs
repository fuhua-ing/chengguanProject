using Geone.Utiliy.Library;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace Geone.Utiliy.Build
{
    public static class TokenMiddleware
    {
        public static IApplicationBuilder UseTokenIdentity(
            this IApplicationBuilder builder, ILogWriter _log)
        {
            return builder.Use(async (context, next) =>
            {
                string path = context.Request.Path.Value;

                if (path.Contains("/api") || path.Contains("/webapi"))
                {
                    try
                    {
                        string access_token = context.Request.Headers["Token"];
                        if (access_token == null)
                        {
                            LogShow log = _log.WriteException("令牌缺失，身份验证未通过。", null, 604.3);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }
                        else
                        {
                            dynamic token = Token.ObtainAccessToken(AppConfig.Init.Key, access_token);

                            //过期时间-现在时间
                            long a = Convert.ToInt64(token.exp.ToString()) - Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                            //签发时间-现在时间
                            long b = Convert.ToInt64(token.iat.ToString()) - Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));

                            if (a < 0)
                            {
                                LogShow log = _log.WriteException("令牌已过期，身份验证未通过。", null, 604.3);
                                HeaderSettings.AddHeaders(context);
                                await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                            }
                            else
                            {
                                if (b > 0)
                                {
                                    LogShow log = _log.WriteException("令牌签发出错，身份验证未通过。", null, 604.3);
                                    HeaderSettings.AddHeaders(context);
                                    await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                                }
                                else
                                {
                                    if (token.iss == null || token.aud == null || token.user == null || token.role == null)
                                    {
                                        LogShow log = _log.WriteException("令牌信息缺失，身份验证未通过。", null, 604.3);
                                        HeaderSettings.AddHeaders(context);
                                        await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                                    }
                                    else
                                    {
                                        string user = token.user.ToString();
                                        string role = token.role.ToString();

                                        AppConfig.Idty = new AppIdentity()
                                        {
                                            AppId = token.aud.ToString(),
                                            User = JsonConvert.DeserializeObject<AppUserResult>(user),
                                            RoleIds = JsonConvert.DeserializeObject<string[]>(role)
                                        };
                                        await next.Invoke();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogShow log = _log.WriteException("身份验证未通过。", ex, 604);
                        HeaderSettings.AddHeaders(context);
                        await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                    }
                }
                else
                    await next.Invoke();
            });
        }
    }
}