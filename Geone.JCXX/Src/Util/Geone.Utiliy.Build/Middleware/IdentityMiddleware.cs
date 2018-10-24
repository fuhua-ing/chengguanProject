using Geone.Utiliy.Library;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Geone.Utiliy.Build
{
    public static class IdentityMiddleware
    {
        public static IApplicationBuilder UseUserIdentity(
            this IApplicationBuilder builder, ISignIn _signin, IRegister _register, ILogWriter _log)
        {
            return builder.Use(async (context, next) =>
            {
                string path = context.Request.Path.Value;

                //验证信息
                if (path.Contains("/signin"))
                {
                    SignInIdentity signInIdentity = null;
                    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        signInIdentity = JsonConvert.DeserializeObject<SignInIdentity>(reader.ReadToEnd());

                        if (signInIdentity == null)
                        {
                            LogShow log = _log.WriteException("参数缺失，登录失败。", null, 401.1);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }

                        if (string.IsNullOrEmpty(signInIdentity.AppId) || string.IsNullOrEmpty(signInIdentity.Account) || string.IsNullOrEmpty(signInIdentity.Password))
                        {
                            LogShow log = _log.WriteException("参数缺失，登录失败。", null, 401.1);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }

                        bool tokenValidate = _signin.Check(signInIdentity, out string msg, out double? code, out string user, out string role, out int hour);

                        if (tokenValidate)
                        {
                            var token = Token.DistributeARToken(AppConfig.Init.Key, signInIdentity.AppId, user, role, hour);
                            //返回Token
                            _log.WriteLogin(signInIdentity.AppId, signInIdentity.Account);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.SuccessAsJson(token, "登录成功。"));
                        }
                        else
                        {
                            if (msg == null) msg = "登录失败。";
                            if (code == null) code = 400;
                            LogShow log = _log.WriteException(msg, null, (double)code);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }
                    }
                }
                else if (path.Contains("/refresh"))
                {
                    RefreshIdentity refreshIdentity = null;
                    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        refreshIdentity = JsonConvert.DeserializeObject<RefreshIdentity>(reader.ReadToEnd());

                        if (refreshIdentity == null)
                        {
                            LogShow log = _log.WriteException("参数缺失，刷新失败。", null, 401.1);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }

                        if (string.IsNullOrEmpty(refreshIdentity.AppId) || string.IsNullOrEmpty(refreshIdentity.AccessToken) || string.IsNullOrEmpty(refreshIdentity.AccessToken))
                        {
                            LogShow log = _log.WriteException("参数缺失，刷新失败。", null, 401.1);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }

                        bool tokenValidate = Token.VerifyRefreshToken(AppConfig.Init.Key, refreshIdentity.AppId, refreshIdentity.AccessToken, refreshIdentity.RefreshToken);

                        if (tokenValidate)
                        {
                            dynamic atoken = Token.ObtainAccessToken(AppConfig.Init.Key, refreshIdentity.AccessToken);
                            var token = Token.DistributeARToken(AppConfig.Init.Key, refreshIdentity.AppId, atoken.user.ToString(), atoken.role.ToString());

                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.SuccessAsJson((Token)token, "刷新令牌成功。"));
                        }
                        else
                        {
                            LogShow log = _log.WriteException("刷新失败。", null, 401.2);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }
                    }
                }
                else if (path.Contains("/register"))
                {
                    SignInIdentity signInIdentity = null;
                    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        signInIdentity = JsonConvert.DeserializeObject<SignInIdentity>(reader.ReadToEnd());

                        if (signInIdentity == null)
                        {
                            LogShow log = _log.WriteException("参数缺失，注册失败。", null, 401.1);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }

                        if (string.IsNullOrEmpty(signInIdentity.AppId) || string.IsNullOrEmpty(signInIdentity.Account) || string.IsNullOrEmpty(signInIdentity.Password))
                        {
                            LogShow log = _log.WriteException("参数缺失，注册失败。", null, 401.1);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }

                        bool userValidate = _register.Check(signInIdentity, out string msg, out double? code);

                        if (userValidate)
                        {
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.SuccessAsJson(null, "注册成功。"));
                        }
                        else
                        {
                            if (msg == null) msg = "注册失败。";
                            if (code == null) code = 400;
                            LogShow log = _log.WriteException(msg, null, (double)code);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }
                    }
                }
                else
                    await next.Invoke();
            });
        }

        public static IApplicationBuilder UseSimpleUserIdentity(
            this IApplicationBuilder builder, ISignIn _signin, IRegister _register, ILogWriter _log)
        {
            return builder.Use(async (context, next) =>
            {
                string path = context.Request.Path.Value;

                //验证信息
                if (path.Contains("/signin"))
                {
                    SignInIdentity signInIdentity = null;
                    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        signInIdentity = JsonConvert.DeserializeObject<SignInIdentity>(reader.ReadToEnd());

                        if (signInIdentity == null)
                        {
                            LogShow log = _log.WriteException("参数缺失，登录失败。", null, 401.1);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }

                        if (string.IsNullOrEmpty(signInIdentity.AppId) || string.IsNullOrEmpty(signInIdentity.Account) || string.IsNullOrEmpty(signInIdentity.Password))
                        {
                            LogShow log = _log.WriteException("参数缺失，登录失败。", null, 401.1);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }

                        bool tokenValidate = _signin.Check(signInIdentity, out string msg, out double? code, out string user, out string role, out int hour);

                        if (tokenValidate)
                        {
                            var token = Token.DistributeToken(AppConfig.Init.Key, signInIdentity.AppId, user, role, hour);
                            //返回Token
                            _log.WriteLogin(signInIdentity.AppId, signInIdentity.Account);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.SuccessAsJson(token, "登录成功。"));
                        }
                        else
                        {
                            if (msg == null) msg = "登录失败。";
                            if (code == null) code = 400;
                            LogShow log = _log.WriteException(msg, null, (double)code);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }
                    }
                }
                else if (path.Contains("/register"))
                {
                    SignInIdentity signInIdentity = null;
                    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        signInIdentity = JsonConvert.DeserializeObject<SignInIdentity>(reader.ReadToEnd());

                        if (signInIdentity == null)
                        {
                            LogShow log = _log.WriteException("参数缺失，注册失败。", null, 401.1);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }

                        if (string.IsNullOrEmpty(signInIdentity.AppId) || string.IsNullOrEmpty(signInIdentity.Account) || string.IsNullOrEmpty(signInIdentity.Password))
                        {
                            LogShow log = _log.WriteException("参数缺失，注册失败。", null, 401.1);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }

                        bool userValidate = _register.Check(signInIdentity, out string msg, out double? code);

                        if (userValidate)
                        {
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.SuccessAsJson(null, "注册成功。"));
                        }
                        else
                        {
                            if (msg == null) msg = "注册失败。";
                            if (code == null) code = 400;
                            LogShow log = _log.WriteException(msg, null, (double)code);
                            HeaderSettings.AddHeaders(context);
                            await context.Response.WriteAsync(RepModel.ErrorAsJson(log));
                        }
                    }
                }
                else
                    await next.Invoke();
            });
        }
    }
}