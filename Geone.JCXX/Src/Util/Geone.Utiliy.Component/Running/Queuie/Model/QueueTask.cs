using Geone.Utiliy.Library;

namespace Geone.Utiliy.Component
{
    /// <summary>
    /// 阻塞队列表-持久类
    /// </summary>
    public class QueueTask : BaseConfig
    {
        //阻塞队列名称
        public string Name { get; set; }

        //阻塞队列描述
        public string Desc { get; set; }

        //订阅服务编号
        public string SubSrvId { get; set; }

        //运行状况
        public QueueStateType State { get; set; }

        //停止标志位
        public bool StopSign { get; set; }

        //暂停标志位
        public bool PauseSign { get; set; }
    }
}