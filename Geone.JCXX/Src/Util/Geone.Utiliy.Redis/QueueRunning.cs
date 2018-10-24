using Geone.Utiliy.Library;
using ServiceStack.Redis;
using System;

namespace Geone.Utiliy.Redis
{
    public class QueueRunning : IQueueRunning
    {
        protected ILogWriter _log;

        public QueueRunning(ILogWriter Log)
        {
            _log = Log;
        }

        public void Running(string channel, string value)
        {
            _log.WriteBusiness(value, "pub", $"发布服务器：{DateTime.Now.ToString("yyMMddHH")}--{channel}发布信息");
        }

        public void Start()
        {
            _log.WriteBusiness(null, "pub", $"发布服务器：{DateTime.Now.ToString("yyMMddHH")}--开启");
        }

        public void Stop()
        {
            _log.WriteBusiness(null, "pub", $"发布服务器：{DateTime.Now.ToString("yyMMddHH")}--关闭");
        }

        public void Error(Exception ex)
        {
            _log.WriteException($"发布服务器：{DateTime.Now.ToString("yyMMddHH")}--发生错误", ex, 999);
        }

        public void Failover(IRedisPubSubServer server)
        {
            _log.WriteException($"发布服务器：{DateTime.Now.ToString("yyMMddHH")}--更换发布服务器", null, 999);
        }

        public void Doing(string channel, string value)
        {
            _log.WriteBusiness(value, "sub", $"订阅客户端：{DateTime.Now.ToString("yyMMddHH")}--{channel}发布信息");
        }

        public void Subscribe(string channel)
        {
            _log.WriteBusiness(null, "sub", $"订阅客户端：{DateTime.Now.ToString("yyMMddHH")}--开始订阅");
        }

        public void UnSubscribe(string channel)
        {
            _log.WriteBusiness(null, "sub", $"订阅客户端：{DateTime.Now.ToString("yyMMddHH")}--取消订阅");
        }
    }
}