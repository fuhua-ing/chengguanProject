namespace Geone.Utiliy.Library
{
    public class LogShow
    {
        public string 日志类型 { get; set; }
        public string 产生时间 { get; set; }
        public string 日志信息 { get; set; }
        public string 日志级别 { get; set; }
        public string 流程编码 { get; set; }

        public string 错误信息 { get; set; }
        public string 错误来源 { get; set; }
        public string 堆栈跟踪 { get; set; }
        public string 内部异常 { get; set; }

        public string 记录数据 { get; set; }
        public string 事件名称 { get; set; }

        public static LogShow Convert(LogConfig config)
        {
            LogShow log = new LogShow()
            {
                日志类型 = config.Type,
                产生时间 = config.Date,
                日志信息 = config.Info,
                日志级别 = config.Level,
                流程编码 = config.Code,

                错误信息 = config.Message,
                错误来源 = config.Source,
                堆栈跟踪 = config.StackTrace,
                内部异常 = config.InnerException,

                记录数据 = config.Data,
                事件名称 = config.Event
            };

            return log;
        }
    }
}