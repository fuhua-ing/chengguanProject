using System.Collections.Generic;

namespace Geone.Utiliy.Component
{
    public class ConfigSrvConfig
    {
        public string Name { get; set; }

        public string Meathod { get; set; }

        //参数列表（key/SrvParam）
        public Dictionary<string, SrvParam> Params { get; set; }
    }
}