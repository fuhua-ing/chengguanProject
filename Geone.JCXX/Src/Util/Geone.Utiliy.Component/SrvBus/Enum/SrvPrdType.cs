namespace Geone.Utiliy.Component
{
    //服务提供类型
    public enum SrvPrdType
    {
        //远程调用
        Rpc,

        //Http访问
        Http,

        //文件
        File,

        //配置
        Config,

        //数据库
        Db,

        //Redis
        Redis,

        //配置值
        Value
    }
}