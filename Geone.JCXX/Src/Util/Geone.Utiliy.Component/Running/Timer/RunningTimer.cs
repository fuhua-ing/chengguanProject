using Geone.Utiliy.Library;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace Geone.Utiliy.Component
{
    public class RunningTimer : IRunningTimer
    {
        private ILogWriter _log;
        private IConfigAction _action;
        private ISrvEvent _event;

        //定时器缓存
        public Timer runningtimer { get; set; }

        //执行锁
        private static bool IsLock;

        public RunningTimer(ILogWriter log, IConfigAction action, ISrvEvent @event)
        {
            IsLock = false;

            _log = log;
            _action = action;
            _event = @event;
        }

        /// <summary>
        /// 运行定时器
        /// </summary>
        /// <param name="reqvalue">参数</param>
        /// <returns>是否成功启动定时器</returns>
        public bool Run(JObject reqvalue)
        {
            try
            {
                runningtimer = new Timer(Callback, reqvalue, 1000, 1000);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Callback(object state)
        {
            //正在运行则返回
            if (IsLock) return;

            //锁定
            IsLock = true;

            TimerTask timer = new TimerTask();

            try
            {
                JObject reqvalue = (JObject)state;
                timer = reqvalue["timer"].ToObject<TimerTask>();

                //校验时间
                DateTime checktime = DateTime.Now;

                //是否上次执行
                bool IsLastExecute = timer.IsLastExecute;

                //检查停止标志位
                if (!timer.StopSign)
                {
                    //检查暂停标志位
                    if (!timer.PauseSign)
                    {
                        switch (timer.Type)
                        {
                            case TimerType.Continue:
                                {
                                    //执行过则判断时间
                                    if (IsLastExecute)
                                    {
                                        DateTime LastExecuteTime = timer.LastExecuteTime;
                                        TimeSpan day = checktime - LastExecuteTime;

                                        //判断是否到时间执行
                                        if (timer.ContinueInterval <= day.TotalSeconds)
                                        {
                                            //执行定时任务
                                            bool runcheck = _event.Event(timer.SubSrvId, reqvalue);

                                            ChangeState(runcheck, timer, checktime);
                                        }
                                    }
                                    //否则直接执行
                                    else
                                    {
                                        //执行定时任务
                                        bool runcheck = _event.Event(timer.SubSrvId, reqvalue);

                                        ChangeState(runcheck, timer, checktime);
                                    }
                                    break;
                                }
                            case TimerType.Fixed:
                                {
                                    //执行过则判断时间
                                    if (IsLastExecute)
                                    {
                                        //上次执行时间
                                        DateTime LastExecuteTime = timer.LastExecuteTime;

                                        int FixedHour = (int)timer.FixedHour;
                                        int FixedMinute = (int)timer.FixedMinute;

                                        //今日执行时间
                                        DateTime TodayTime = DateTime.Today.AddHours(FixedHour).AddMinutes(FixedMinute);

                                        //判断是否到时间执行
                                        if (LastExecuteTime < TodayTime && TodayTime <= checktime)
                                        {
                                            //执行定时任务
                                            bool runcheck = _event.Event(timer.SubSrvId, reqvalue);

                                            ChangeState(runcheck, timer, checktime);
                                        }
                                    }
                                    //否则直接执行
                                    else
                                    {
                                        //执行定时任务
                                        bool runcheck = _event.Event(timer.SubSrvId, reqvalue);

                                        ChangeState(runcheck, timer, checktime);
                                    }
                                    break;
                                }
                            case TimerType.Once:
                                {
                                    //未执行过则判断时间
                                    if (!IsLastExecute)
                                    {
                                        DateTime OnceDateTime = Convert.ToDateTime(timer.OnceDateTime);

                                        //判断是否到时间执行
                                        if (OnceDateTime <= checktime)
                                        {
                                            //执行定时任务
                                            bool runcheck = _event.Event(timer.SubSrvId, reqvalue);

                                            //执行
                                            if (runcheck)
                                            {
                                                if (timer.State != TimerStateType.已执行)
                                                {
                                                    //执行结束销毁这个Timer
                                                    if (runningtimer != null)
                                                    {
                                                        runningtimer.Change(Timeout.Infinite, Timeout.Infinite);
                                                        runningtimer.Dispose();
                                                    }
                                                    timer.State = TimerStateType.已执行;
                                                    timer.IsLastExecute = true;
                                                    timer.LastExecuteTime = checktime;
                                                    bool check = _action.PutModel<TimerTask>("TimerTask", timer);
                                                }
                                            }
                                            else
                                            {
                                                //停止定时器
                                                if (runningtimer != null)
                                                {
                                                    runningtimer.Change(Timeout.Infinite, Timeout.Infinite);
                                                    runningtimer.Dispose();
                                                }
                                                if (timer.State != TimerStateType.执行失败)
                                                {
                                                    timer.State = TimerStateType.执行失败;
                                                    bool check = _action.PutModel<TimerTask>("TimerTask", timer);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //停止定时器
                                        if (runningtimer != null)
                                        {
                                            runningtimer.Change(Timeout.Infinite, Timeout.Infinite);
                                            runningtimer.Dispose();
                                        }
                                        if (timer.State != TimerStateType.已停止)
                                        {
                                            timer.State = TimerStateType.已停止;
                                            bool check = _action.PutModel<TimerTask>("TimerTask", timer);
                                        }
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    else
                    {
                        //不执行任何任务
                        if (timer.State != TimerStateType.暂停中)
                        {
                            timer.State = TimerStateType.暂停中;
                            bool check = _action.PutModel<TimerTask>("TimerTask", timer);
                        }
                    }
                }
                else
                {
                    //停止定时器
                    if (runningtimer != null)
                    {
                        runningtimer.Change(Timeout.Infinite, Timeout.Infinite);
                        runningtimer.Dispose();
                    }
                    if (timer.State != TimerStateType.已停止)
                    {
                        timer.State = TimerStateType.已停止;
                        bool check = _action.PutModel<TimerTask>("TimerTask", timer);
                    }
                }
            }
            catch (Exception ex)
            {
                //停止定时器
                if (runningtimer != null)
                {
                    runningtimer.Change(Timeout.Infinite, Timeout.Infinite);
                    runningtimer.Dispose();
                }

                if (timer == null)
                    _log.WriteException($"定时任务启动读取配置异常", ex);
                else
                {
                    _log.WriteException($"Id:*{ timer.Id }* —— *{ timer.Name }* 异常", ex);

                    if (timer.State != TimerStateType.异常)
                    {
                        timer.State = TimerStateType.异常;
                        bool check = _action.PutModel<TimerTask>("TimerTask", timer);
                    }
                }
            }
            finally
            {
                //解锁
                IsLock = false;
            }
        }

        private void ChangeState(bool runcheck, TimerTask timer, DateTime checktime)
        {
            //执行
            if (runcheck)
            {
                if (timer.State != TimerStateType.正常运行)
                {
                    timer.State = TimerStateType.正常运行;
                    timer.IsLastExecute = true;
                    timer.LastExecuteTime = checktime;
                    bool check = _action.PutModel<TimerTask>("TimerTask", timer);
                }
            }
            else
            {
                //停止定时器
                if (runningtimer != null)
                {
                    runningtimer.Change(Timeout.Infinite, Timeout.Infinite);
                    runningtimer.Dispose();
                }
                if (timer.State != TimerStateType.执行失败)
                {
                    timer.State = TimerStateType.执行失败;
                    bool check = _action.PutModel<TimerTask>("TimerTask", timer);
                }
            }
        }
    }
}