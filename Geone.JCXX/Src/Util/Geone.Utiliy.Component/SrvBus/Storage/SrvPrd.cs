using Geone.Utiliy.Library;

namespace Geone.Utiliy.Component
{
    /// <summary>
    /// 服务提供
    /// </summary>
    public class SrvPrd : BaseConfig
    {
        //服务名称
        public string Name { get; set; }

        //服务描述
        public string Desc { get; set; }

        //提供类型
        public SrvPrdType Type { get; set; }

        //是否可被订阅
        public bool IsSub { get; set; }

        //是否开启缓存
        //public bool IsCache { get; set; }

        //服务配置
        public string Config { get; set; }
    }
}