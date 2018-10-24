using System.Collections.Generic;

namespace Geone.Utiliy.Component
{
    public class DbSrvConfig
    {
        //类型
        public SqlType Type { get; set; }

        //名称
        public string ConnectName { get; set; }

        //执行类型
        public ExecuteType ExecType { get; set; }

        //查询字符串
        public string[] Sqls { get; set; }

        //参数列表（key/SrvParam）
        public Dictionary<string, SrvParam> Params { get; set; }
    }
}