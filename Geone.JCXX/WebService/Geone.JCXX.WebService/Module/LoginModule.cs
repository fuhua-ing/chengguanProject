using Geone.JCXX.WebService.Meta.QueryEntity;
using Geone.Utiliy.Library;
using Nancy;
using Nancy.ModelBinding;
using System;

namespace Geone.JCXX.WebService
{
    public class LoginModule : NancyModule
    {
        private IUserService ss;

        public LoginModule(IUserService _ss)
            : base("/User")
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
                    return Response.AsJson(RepModel.Error(ex.Message));
                }
            });
        }
    }
}