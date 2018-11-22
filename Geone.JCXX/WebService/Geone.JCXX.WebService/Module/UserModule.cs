using Geone.JCXX.WebService.Meta.QueryEntity;
using Geone.Utiliy.Build;
using Nancy;
using Nancy.ModelBinding;

namespace Geone.JCXX.WebService
{
    public class UserModule : NancyModule
    {
        public UserModule(IIdentityTool itool, IUserService ss) : base("/api/User")
        {
            //修改密码
            Put("/Pwd", parameter =>
            {
                AppIdentity Idty = itool.Get();
                var req = this.Bind<Req_EditPwd>();
                var rep = ss.EditPwd(req, Idty);
                return Response.AsJson(rep);
            });

            //获取用户菜单
            Get("/Menu", parameter =>
            {
                AppIdentity Idty = itool.Get();
                var rep = ss.GetMenu(Idty);
                return Response.AsJson(rep);
            });

            //获取用户的角色
            Get("/UserRole", parameter =>
            {
                AppIdentity Idty = itool.Get();
                var rep = ss.GetUserRole(Idty);
                return Response.AsJson(rep);
            });

            //获取用户的权属角色
            Get("/QsRole", parameter =>
            {
                AppIdentity Idty = itool.Get();
                var rep = ss.GetQsRole(Idty);
                return Response.AsJson(rep);
            });
        }
    }
}