using ServiceStack.Redis;
using System;

namespace Geone.Utiliy.Redis
{
    public interface IQueueRunning
    {
        void Running(string channel, string value);

        void Start();

        void Stop();

        void Error(Exception ex);

        void Failover(IRedisPubSubServer server);

        void Doing(string channel, string value);

        void Subscribe(string channel);

        void UnSubscribe(string channel);
    }
}