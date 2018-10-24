using Geone.Utiliy.Library;
using ServiceStack.Redis;

namespace Geone.Utiliy.Redis
{
    /// <summary>
    /// Redis发布订阅服务队列数据库模型
    /// </summary>
    public class QueueAction : RedisAction, IQueueAction
    {
        private IQueueRunning run;
        private RedisPubSubServer PubSubServer { get; set; }

        public QueueAction(IRedisConnect RedisConn, IRedisConnection Connection, ILogWriter Log, IQueueRunning queueRunning)
            : base(RedisConn, Connection, Log)
        {
            run = queueRunning;
            SetName("Queue");
        }

        /// <summary>
        /// 发布者
        /// </summary>
        /// <param name="channel">频道</param>
        /// <param name="value">值</param>
        /// <returns>频道订阅数</returns>
        public long? Publish(string channel, string value)
        {
            return ExeRedisAction((db) =>
            {
                return db.PublishMessage(channel, value);
            });
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="channels">频道</param>
        public void Start(params string[] channels)
        {
            //PooledRedisClientManager
            IRedisClientsManager redisClientManager = redis.Prcm;
            //发布、订阅服务 IRedisPubSubServer
            RedisPubSubServer pubSubServer = new RedisPubSubServer(redisClientManager, channels)
            {
                OnMessage = (channel, msg) =>
                {
                    run.Running(channel, msg);
                },
                OnStart = () =>
                {
                    run.Start();
                },
                OnStop = () =>
                {
                    run.Stop();
                },
                OnError = (ex) =>
                {
                    run.Error(ex);
                },
                OnFailover = (server) =>
                {
                    run.Failover(server);
                },
            };
            //接收消息
            pubSubServer.Start();
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="do">订阅事件</param>
        /// <param name="channels">订阅频道</param>
        public void Subscribe(DoMessage @do, params string[] channels)
        {
            ExeRedisAction((sub) =>
            {
                //接受到消息时
                sub.OnMessage = (channel, msg) =>
                {
                    run.Doing(channel, msg);
                    @do(channel, msg);
                };
                //订阅频道时
                sub.OnSubscribe = (channel) =>
                {
                    run.Subscribe(channel);
                };
                //取消订阅频道时
                sub.OnUnSubscribe = (channel) =>
                {
                    run.UnSubscribe(channel);
                };

                //订阅频道
                sub.SubscribeToChannels(channels);
            });
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="channels">订阅频道</param>
        public void UnSubscribe(params string[] channels)
        {
            ExeRedisAction((sub) =>
            {
                //订阅频道
                sub.UnSubscribeFromChannels(channels);
            });
        }
    }
}