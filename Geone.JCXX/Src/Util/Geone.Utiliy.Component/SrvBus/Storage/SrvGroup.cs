using Geone.Utiliy.Library;

namespace Geone.Utiliy.Component
{
    /// <summary>
    /// 服务组
    /// </summary>
    public class SrvGroup : BaseConfig
    {
        //服务组名称
        public string Name { get; set; }

        //服务组描述
        public string Desc { get; set; }

        //拥有服务
        public string[] SrvIds { get; set; }
    }
}