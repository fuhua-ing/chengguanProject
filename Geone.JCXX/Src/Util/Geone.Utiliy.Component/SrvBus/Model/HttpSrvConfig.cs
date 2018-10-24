using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Geone.Utiliy.Component
{
    public class HttpSrvConfig
    {
        //访问名称
        public string ServiceKeyName { get; set; }

        //访问谓词
        public string Predicate { get; set; }

        //访问路由
        public string Url { get; set; }

        //请求头（key/value）
        public JObject Headers { get; set; }

        //超时设置
        public int? Timeout { get; set; }

        //返回过滤
        public string Filter { get; set; }

        //参数列表（key/SrvParam）
        public Dictionary<string, SrvParam> Params { get; set; }

        //订阅列表（key/SrvSub）
        public Dictionary<string, SrvSub> Subs { get; set; }
    }
}