namespace Geone.Utiliy.Redis
{
    /// <summary>
    /// 数据库连接表_Redis
    /// </summary>
    public class Redis
    {
        //读写服务器
        public string[] Rwservers { get; set; }

        //只读服务器
        public string[] Ronlyservers { get; set; }

        //超时时间
        public int? Timeout { get; set; }

        //最大链接数
        public int? Max { get; set; }

        //自动重启
        public bool Auto { get; set; }
    }
}