
using Grpc.Core;
using MagicOnion;
using System.Collections.Generic;

namespace Geone.Utiliy.Library
{
    public delegate RepModel Event(Dictionary<string, object> req, ILogWriter log);

    public interface IRpcAccess
    {
        Server GetServer(string host, int port);

        void Start(Server server);

        void Close(Server server);

        Channel GetChannel(string host, int port);

        T GetService<T>(Channel channel) where T : IService<T>;

        T GetService<T>(string host, int port) where T : IService<T>;

        RepModel Communicate(Channel channel, string service, string method, RpcReq req);

        RepModel Communicate(string host, int post, string service, string method, RpcReq req);

        RepModel Do(Event @event, RpcReq req);
    }
}