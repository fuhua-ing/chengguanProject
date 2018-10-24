using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Geone.Utiliy.Library
{
    /// <summary>
    /// Rpc服务请求数据模型
    /// </summary>
    [MessagePackObject(true)]
    public class RpcReq
    {
        //请求头（Srvid/Identity/Ticket/Token）
        public string Header { get; set; }

        //请求参数（可为空）
        public string Param { get; set; }

        public static RpcReq Create(params object[] @params)
        {
            JObject obj = new JObject();
            foreach (object p in @params)
            {
                obj.Add(new JProperty(nameof(p), p));
            }
            RpcReq rpcReq = new RpcReq()
            {
                Header = null,
                Param = JsonConvert.SerializeObject(obj)
            };

            return rpcReq;
        }
    }
}