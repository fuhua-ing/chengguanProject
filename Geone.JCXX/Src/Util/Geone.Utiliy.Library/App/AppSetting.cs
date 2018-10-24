namespace Geone.Utiliy.Library
{
    public class AppSetting
    {
        //运行环境
        public string Environment { get; set; }

        //内容路径
        public string ContentRoot { get; set; }

        //启动路由
        public string Urls { get; set; }

        //项目名称
        public string ApplicationName { get; set; }

        //项目主键
        public string AppKey { get; set; }

        //宿主的记录主机号
        public string Host { get; set; }

        //宿主的记录端口号
        public int? Port { get; set; }

        //是否启动治理
        public bool? IsGovern { get; set; }

        //构建的主机号
        public string ConnectHost { get; set; }

        //构建的端口号
        public int? ConnectPort { get; set; }

        //RPC的端口号
        public int? RpcPort { get; set; }
    }
}