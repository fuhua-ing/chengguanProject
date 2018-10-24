using Geone.Utiliy.Library;
using Geone.Utiliy.Redis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Geone.Utiliy.Component
{
    public class RunningBlocking : IRunningBlocking
    {
        private ILogWriter _log;
        private IConfigAction _action;
        private ISrvEvent _event;
        private IRedisAction _redis;

        private static string key = "block";

        //执行锁
        private static bool IsLock;

        public RunningBlocking(ILogWriter log, IConfigAction action, ISrvEvent @event, IRedisAction redis)
        {
            IsLock = false;

            _log = log;
            _action = action;
            _event = @event;
            _redis = redis;
        }

        /// <summary>
        /// 运行阻塞队列
        /// </summary>
        /// <param name="queue">阻塞队列</param>
        /// <param name="reqvalue">参数</param>
        /// <returns>是否成功启动阻塞队列</returns>
        public bool Run(string queue, JObject reqvalue)
        {
            //通过线程池开启线程
            bool res = ThreadPool.QueueUserWorkItem(o =>
            {
                Callback(queue, reqvalue);
            });

            return res;
        }

        private void Callback(string queue, JObject reqvalue)
        {
            //正在运行返回
            if (IsLock) return;

            //锁定
            IsLock = true;

            QueueTask block = null;

            try
            {
                block = reqvalue[key].ToObject<QueueTask>();

                //检查停止标志位
                if (!block.StopSign)
                {
                    //检查暂停标志位
                    if (!block.PauseSign)
                    {
                        #region 事先处理

                        string listid = AesEncrypt.Encrypt(queue, key);

                        //阻塞
                        string value = _redis.BlockingDequeue(listid, new TimeSpan(0));

                        //无数据位
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            //手动解锁
                            IsLock = false;
                            //继续运行
                            Callback(queue, reqvalue);
                        }

                        //整合参数
                        JObject blockingvalue = new JObject();

                        if (reqvalue != null)
                            blockingvalue = reqvalue;

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
                        bool runcheck = _event.Event(block.SubSrvId, blockingvalue);

                        #region 事后处理

                        if (runcheck)
                        {
                            if (block.State != QueueStateType.正常运行)
                            {
                                block.State = QueueStateType.正常运行;
                                bool check = _action.PutModel<QueueTask>("QueueTask", block);
                            }
                            //手动解锁
                            IsLock = false;
                            //继续运行
                            Callback(queue, blockingvalue);
                        }
                        else
                        {
                            if (block.State != QueueStateType.异常)
                            {
                                block.State = QueueStateType.异常;
                                bool check = _action.PutModel<QueueTask>("QueueTask", block);
                            }

                            //手动解锁
                            IsLock = false;
                            Callback(queue, blockingvalue);
                        }

                        #endregion 事后处理
                    }
                    else
                    {
                        if (block.State != QueueStateType.暂停中)
                        {
                            block.State = QueueStateType.暂停中;
                            bool check = _action.PutModel<QueueTask>("QueueTask", block);
                        }
                        //休眠一秒防止CPU占用过大
                        Thread.Sleep(1000);
                        //手动解锁
                        IsLock = false;
                        Callback(queue, reqvalue);
                    }
                }
                else
                {
                    if (block.State != QueueStateType.已停止)
                    {
                        block.State = QueueStateType.已停止;
                        bool check = _action.PutModel<QueueTask>("QueueTask", block);
                    }
                }
            }
            catch (Exception ex)
            {
                if (block == null)
                    _log.WriteException($"队列启动读取配置异常", ex);
                else
                {
                    _log.WriteException($"Id:*{ block.Id }* —— *{ block.Name }* 阻塞异常", ex);
                    if (block.State != QueueStateType.异常)
                    {
                        block.State = QueueStateType.异常;
                        bool check = _action.PutModel<QueueTask>("QueueTask", block);
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