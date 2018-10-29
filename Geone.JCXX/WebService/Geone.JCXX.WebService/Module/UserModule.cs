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
    public class UserModule : BaseModule
    {

        public UserModule(IUserService ss) : base("/api/User")
        {
            //修改密码
            Put("/Pwd", parameter =>
            {
                var req = this.Bind<Req_EditPwd>();
                var rep = ss.EditPwd(req, Idty);
                return Response.AsJson(rep);
            });
            //获取用户菜单
            Get("/Menu", parameter =>
            {
                var rep = ss.GetMenu(Idty);
                return Response.AsJson(rep);
            });
            //获取用户的角色
            Get("/UserRole", parameter =>
            {
                var rep = ss.GetUserRole(Idty);
                return Response.AsJson(rep);
            });
            //获取用户的权属角色
            Get("/QsRole", parameter =>
            {
                var rep = ss.GetQsRole(Idty);
                return Response.AsJson(rep);
            });

        }
    }
}
