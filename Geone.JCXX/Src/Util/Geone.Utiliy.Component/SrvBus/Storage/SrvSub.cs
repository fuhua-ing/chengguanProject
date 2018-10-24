using Geone.Utiliy.Library;

namespace Geone.Utiliy.Component
{
    /// <summary>
    /// 服务订阅
    /// </summary>
    public class SrvSub : BaseConfig
    {
        //订阅名称
        public string Name { get; set; }

        //订阅描述
        public string Desc { get; set; }

        //订阅服务编号
        public string SubId { get; set; }

        //订阅过滤
        public string Filter { get; set; }
    }
}