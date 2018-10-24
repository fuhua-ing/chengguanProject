namespace Geone.Utiliy.Component
{
    public class FileSrvConfig
    {
        //文件服务提供类型
        public FileSrvPrdType Type { get; set; }

        //文件名称
        public string Name { get; set; }

        //访问名称
        public string ServiceKeyName { get; set; }

        //文件提供路由
        public string Url { get; set; }

        public FileExceType ExceType { get; set; }

        //文件提供路径
        public string Path { get; set; }

        //文件服务配置类型
        public FileSrvConfigType FileType { get; set; }

        //返回过滤
        public string Filter { get; set; }
    }
}