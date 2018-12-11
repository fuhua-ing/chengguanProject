using Geone.Utiliy.Build;
using Geone.Utiliy.Component;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using Microsoft.Extensions.Configuration;
using Nancy;

namespace Geone.JCXX.WebService
{
    public class BaseModule : NancyModule
    {
        /// <summary>
        /// 访问服务的客户端信息
        /// </summary>
        protected AppInitialize Init { get; set; }

        /// <summary>
        /// 应用用户信息
        /// </summary>
        protected AppIdentity Idty { get; set; }

        public BaseModule(IConfigTool ctool)
        {
            Get("/", r =>
            {
                AppInitialize Init = ctool.Get().Get<AppInitialize>();
                return RepModel.SuccessAsJson($"欢迎使用{Init.Name}，本服务为{Init.Desc}。");
            });
        }
    }
}