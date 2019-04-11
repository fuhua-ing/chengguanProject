using Geone.JCXX.WebService.Meta.QueryEntity;
using Nancy;
using Nancy.ModelBinding;

namespace Geone.JCXX.WebService
{
    public class ConfigModule : NancyModule
    {
        public ConfigModule(IDataService ss) : base("/config")
        {
            //网格动态配置列表
            Get("/Grid/ConfigList", parameter =>
            {
                var rep = ss.GetGridConfigList();
                return Response.AsJson(rep);
            });
        }
    }
}