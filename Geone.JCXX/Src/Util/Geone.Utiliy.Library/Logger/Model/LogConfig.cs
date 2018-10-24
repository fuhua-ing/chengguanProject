namespace Geone.Utiliy.Library
{
    public class LogConfig
    {
        public string Type { get; set; }  //日志类型
        public string Date { get; set; }  //产生时间
        public string Info { get; set; }  //日志信息
        public string Level { get; set; }  //日志级别
        public string Code { get; set; }  //流程编码

        public string Message { get; set; }  //错误信息
        public string Source { get; set; }  //错误来源
        public string StackTrace { get; set; }  //堆栈跟踪
        public string InnerException { get; set; }  //内部异常

        public string Data { get; set; }  //记录数据
        public string Event { get; set; }  //事件名称

        public static LogConfig Convert(LogShow show)
        {
            LogConfig log = new LogConfig()
            {
                Type = show.日志类型,
                Date = show.产生时间,
                Info = show.日志信息,
                Level = show.日志级别,
                Code = show.流程编码,

                Message = show.错误信息,
                Source = show.错误来源,
                StackTrace = show.堆栈跟踪,
                InnerException = show.内部异常,

                Data = show.记录数据,
                Event = show.事件名称
            };

            return log;
        }
    }
}