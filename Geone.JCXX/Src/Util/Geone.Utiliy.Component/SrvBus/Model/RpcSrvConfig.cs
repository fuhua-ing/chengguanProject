using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Geone.Utiliy.Component
{
    public class RpcSrvConfig
    {
        //访问名称
        public string ServiceKeyName { get; set; }

        //访问接口名
        public string ServiceName { get; set; }

        //访问方法名
        public string MethodName { get; set; }

        //请求头（key/value）
        public JObject Headers { get; set; }

        //返回过滤
        public string Filter { get; set; }

        //参数列表（key/SrvParam）
        public Dictionary<string, SrvParam> Params { get; set; }

        //订阅列表（key/SrvSub）
        public Dictionary<string, SrvSub> Subs { get; set; }
    }
}