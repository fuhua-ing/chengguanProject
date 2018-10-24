namespace Geone.Utiliy.Library
{
    public class AppLog
    {
        //是否跟踪
        public bool? IsTrack { get; set; }

        //是否调试
        public bool? IsDebug { get; set; }

        //存放路径
        public string LogKey { get; set; }

        //存放路径
        public string LogPath { get; set; }

        //存放路径
        public string LogQueue { get; set; }

        //Redis主机号
        public string RedisHost { get; set; }

        //Redis端口号
        public int? RedisPort { get; set; }
    }
}