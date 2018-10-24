using Geone.Utiliy.Library;

namespace Geone.Utiliy.Component
{
    /// <summary>
    /// 服务参数
    /// </summary>
    public class SrvParam : BaseConfig
    {
        //参数名称
        public string Name { get; set; }

        //参数描述
        public string Desc { get; set; }

        //参数获取类型-默认传输
        public SrvParamType Type { get; set; }

        //参数过滤
        public string Filter { get; set; }

        //参数获取配置（Transfer-MockValue/Configure-ConfigureValue/Subscribe-SrvPrdId）
        public dynamic Config { get; set; }
    }
}