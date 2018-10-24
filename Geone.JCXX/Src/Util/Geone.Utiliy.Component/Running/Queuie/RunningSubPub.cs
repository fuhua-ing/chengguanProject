using Geone.Utiliy.Library;
using Geone.Utiliy.Redis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Geone.Utiliy.Component
{
    public class RunningPubSub : IRunningPubSub
    {
        private ILogWriter _log;
        private IConfigAction _action;
        private ISrvEvent _event;
        private IQueueAction _redis;

        //定时器缓存
        public JObject Reqvalue { get; set; }

        private static readonly string key = "pubsub";

        //执行锁
        private static bool IsLock;

        public RunningPubSub(ILogWriter log, IConfigAction action, ISrvEvent @event, IQueueAction redis)
        {
            IsLock = false;

            _log = log;
            _action = action;
            _event = @event;
            _redis = redis;
        }

        /// <summary>
        /// 运行发布订阅队列
        /// </summary>
        /// <param name="channel">订阅频道</param>
        /// <param name="reqvalue">参数</param>
        /// <returns>是否成功启动阻塞队列</returns>
        public bool Run(string channel, JObject reqvalue)
        {
            try
            {
                _redis.Subscribe(Callback, channel);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Callback(string channel, string value)
        {
            //正在运行返回
            if (IsLock) return;

            //锁定
            IsLock = true;

            QueueTask pubsub = null;

            try
            {
                pubsub = Reqvalue[key].ToObject<QueueTask>();

                //检查停止标志位
                if (!pubsub.StopSign)
                {
                    //检查暂停标志位
                    if (!pubsub.PauseSign)
                    {
                        #region 事先处理

                        //无数据位
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            //手动解锁
                            IsLock = false;
                        }

                        //整合参数
                        JObject blockingvalue = new JObject();

                        if (Reqvalue != null)
                            blockingvalue = Reqvalue;

                        JObject dicvalue = JObject.Parse(value);

                        if (dicvalue.Count > 0)
                        {
                            foreach (KeyValuePair<string, JToken> kv in dicvalue)
                            {
                                if (!blockingvalue.ContainsKey(kv.Key))
                                {
                                    blockingvalue.Add(kv.Key, kv.Value);
                                }
                                else
                                {
                                    blockingvalue[kv.Key] = kv.Value;
                                }
                            }
                        }

                        #endregion 事先处理

                        //执行订阅任务
                        bool runcheck = _event.Event(pubsub.SubSrvId, blockingvalue);

                        #region 事后处理

                        if (runcheck)
                        {
                            if (pubsub.State != QueueStateType.正常运行)
                            {
                                pubsub.State = QueueStateType.正常运行;
                                bool check = _action.PutModel<QueueTask>("QueueTask", pubsub);
                            }
                            //手动解锁
                            IsLock = false;
                        }
                        else
                        {
                            if (pubsub.State != QueueStateType.异常)
                            {
                                pubsub.State = QueueStateType.异常;
                                bool check = _action.PutModel<QueueTask>("QueueTask", pubsub);
                            }

                            //手动解锁
                            IsLock = false;
                        }

                        #endregion 事后处理
                    }
                    else
                    {
                        if (pubsub.State != QueueStateType.暂停中)
                        {
                            pubsub.State = QueueStateType.暂停中;
                            bool check = _action.PutModel<QueueTask>("QueueTask", pubsub);
                        }
                        //手动解锁
                        IsLock = false;
                    }
                }
                else
                {
                    _redis.UnSubscribe(channel);
                    if (pubsub.State != QueueStateType.已停止)
                    {
                        pubsub.State = QueueStateType.已停止;
                        bool check = _action.PutModel<QueueTask>("QueueTask", pubsub);
                    }
                }
            }
            catch (Exception ex)
            {
                if (pubsub == null)
                    _log.WriteException($"队列启动读取配置异常", ex);
                else
                {
                    _log.WriteException($"Id:*{ pubsub.Id }* —— *{ pubsub.Name }* 阻塞异常", ex);
                    if (pubsub.State != QueueStateType.异常)
                    {
                        pubsub.State = QueueStateType.异常;
                        bool check = _action.PutModel<QueueTask>("QueueTask", pubsub);
                    }
                }
            }
            finally
            {
                //解锁
                IsLock = false;
            }
        }
    }
}