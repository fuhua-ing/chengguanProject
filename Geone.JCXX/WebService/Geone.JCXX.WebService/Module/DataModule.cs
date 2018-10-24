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
    public class DataModule : BaseModule
    {
        IDataService ss;

        public DataModule(IDataService _ss) : base("/api")
        {
            ss = _ss;
            //字典明细列表
            Get("/DictCategory/ItemList", parameter =>
            {
                var req = this.Bind<Req_DictItem>();
                var rep = ss.GetDictItemList(req);
                return Response.AsJson(rep);
            });
            //权属角色列表
            Get("/QSRole/List", parameter =>
            {
                var req = this.Bind<Req_QSRole>();
                var rep = ss.GetQSRoleList(req);
                return Response.AsJson(rep);
            });
            //网格列表
            Get("/Grid/List", parameter =>
            {
                var req = this.Bind<Req_Grid>();
                var rep = ss.GetGridList(req);
                return Response.AsJson(rep);
            });
            //部门列表
            Get("/Dept/List", parameter =>
            {
                var req = this.Bind<Req_Dept>();
                var rep = ss.GetDeptList(req);
                return Response.AsJson(rep);
            });
            //立案条件列表
            Get("/CaseLATJ/List", parameter =>
            {
                var req = this.Bind<Req_CaseLATJ>();
                var rep = ss.GetCaseLATJList(req);
                return Response.AsJson(rep);
            });
        }
    }
}
