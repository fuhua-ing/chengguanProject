using Geone.Utiliy.Library;
using System;

namespace Geone.Utiliy.Component
{
    /// <summary>
    /// 定时器表-持久类
    /// </summary>
    public class TimerTask : BaseConfig
    {
        //定时器名称
        public string Name { get; set; }

        //定时器描述
        public string Desc { get; set; }

        //订阅服务编号
        public string SubSrvId { get; set; }

        //定时器类型
        public TimerType Type { get; set; }

        //持续性执行时间-间隔秒数
        public double? ContinueInterval { get; set; }

        //固定时间执行时间-固定时刻
        public int? FixedHour { get; set; }

        public int? FixedMinute { get; set; }

        //一次性执行时间-固定时刻
        public string OnceDateTime { get; set; }

        //上次是否已执行
        public bool IsLastExecute { get; set; }

        //上次执行时间
        public DateTime LastExecuteTime { get; set; }

        //运行状况
        public TimerStateType State { get; set; }

        //停止标志位
        public bool StopSign { get; set; }

        //暂停标志位
        public bool PauseSign { get; set; }
    }
}