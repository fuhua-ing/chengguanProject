using Geone.Utiliy.Build;
using Geone.Utiliy.Component;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using System;

namespace Geone.JCXX.WebService
{
    public class IndIdentity : IIndIdentity
    {
        private ILogWriter _log;
        private string key;

        public IndIdentity(ILogWriter log)
        {
            _log = log;
            key = "geone123";
        }

        public RepModel Verify(VerifyIdentity verifyIdentity)
        {
            string access_token = verifyIdentity.Token;
            if (access_token == null)
            {
                return RepModel.Error(_log.WriteWarn("令牌缺失，身份验证未通过。", 604.3));
            }
            else
            {
                dynamic token = Token.ObtainAccessToken(key, access_token);

                //过期时间-现在时间
                long a = Convert.ToInt64(token.exp.ToString()) - Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                //签发时间-现在时间
                long b = Convert.ToInt64(token.iat.ToString()) - Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));

                if (a < 0)
                {
                    return RepModel.Error(_log.WriteWarn("令牌已过期，身份验证未通过。", 604.3));
                }
                else
                {
                    if (b > 0)
                    {
                        return RepModel.Error(_log.WriteWarn("令牌签发出错，身份验证未通过。", 604.3));
                    }
                    else
                    {
                        if (token.iss == null || token.aud == null || token.user == null || token.role == null)
                        {
                            return RepModel.Error(_log.WriteWarn("令牌信息缺失，身份验证未通过。", 604.3));
                        }
                        else
                        {
                            return RepModel.Success(token);
                        }
                    }
                }
            }
        }

        public RepModel SignIn(SignInIdentity signInIdentity)
        {
            var token = Token.DistributeARToken(key, signInIdentity.AppId, "", "", 6);
            return RepModel.Success(token);
        }

        public RepModel Refresh(RefreshIdentity refreshIdentity)
        {
            if (refreshIdentity == null)
            {
                return RepModel.Error(_log.WriteWarn("参数缺失，刷新失败。", 401.1));
            }

            if (string.IsNullOrEmpty(refreshIdentity.AppId) || string.IsNullOrEmpty(refreshIdentity.AccessToken) || string.IsNullOrEmpty(refreshIdentity.AccessToken))
            {
                return RepModel.Error(_log.WriteWarn("参数缺失，刷新失败。", 401.1));
            }

            bool tokenValidate = Token.VerifyRefreshToken(key, refreshIdentity.AppId, refreshIdentity.AccessToken, refreshIdentity.RefreshToken);

            if (tokenValidate)
            {
                dynamic atoken = Token.ObtainAccessToken(key, refreshIdentity.AccessToken);
                var token = Token.DistributeARToken(key, refreshIdentity.AppId, atoken.user.ToString(), atoken.role.ToString());

                return RepModel.Success(token);
            }
            else
            {
                return RepModel.Error(_log.WriteWarn("令牌刷新失败。", 401.2));
            }
        }

        public RepModel Register(SignInIdentity signInIdentity)
        {
            return RepModel.Success(true);
        }
    }
}