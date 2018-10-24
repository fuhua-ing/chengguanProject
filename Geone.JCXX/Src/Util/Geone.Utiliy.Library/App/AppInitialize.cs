using System.Collections.Generic;

namespace Geone.Utiliy.Library
{
    public class AppInitialize
    {
        //唯一标识符
        //--GUID
        public string Id { get; set; }

        //唯一名称
        public string Name { get; set; }

        //中文描述
        public string Desc { get; set; }

        //生成 Token、Ticket等授权标识用的密钥（长度限制：10字节）
        public string Key { get; set; }

        //宿主的记录主机号
        public string Host { get; set; }

        //宿主的记录端口号
        public int? Port { get; set; }

        //RPC的端口号
        public int? RpcPort { get; set; }

        //数据库连接
        public Dictionary<string, string> Sql { get; set; }

        //Redis数据库连接
        public Dictionary<string, string> Redis { get; set; }

        //日志记录
        public AppLog Log { get; set; }
    }
}