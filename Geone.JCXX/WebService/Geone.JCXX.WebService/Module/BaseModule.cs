using Geone.JCXX.WebService.Meta.Interface;
using Geone.JCXX.WebService.Meta.QueryEntity;
using Geone.Utiliy.Library;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public BaseModule()
        {
            Before += ctx =>
            {
                Init = AppConfig.Init;
                return default(Response);
            };

            Get("/", r =>
            {
                return RepModel.SuccessAsJson($"欢迎使用{Init.Name}，本服务为{Init.Desc}。");
            });
        }

        public BaseModule(string path)
            : base(path)
        {
            Before += ctx =>
            {
                Init = AppConfig.Init;
                Idty = AppConfig.Idty;
                return default(Response);
            };
        }

    }

    public class LoginModule : BaseModule
    {
        IUserService ss;

        public LoginModule(IUserService _ss) : base("/User")
        {
            ss = _ss;

            Post("/Login", parameter =>
            {
                try
                {
                    var req = this.Bind<Req_UserLogin>();
                    var rep = ss.Login(req);
                    return Response.AsJson(rep);
                }
                catch (Exception ex)
                {
                    //LogHelper.WriteLog(ex.Message, LogType.Exception);
                    return Response.AsJson(RepModel.Error());
                }
            });

        }
    }
}
