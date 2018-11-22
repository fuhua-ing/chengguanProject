using Autofac;
using Geone.JCXX.Meta;
using Geone.Utiliy.Build;
using Geone.Utiliy.Database;
using Geone.Utiliy.Logger;
using MagicOnion;
using MagicOnion.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geone.JCXX.WebService
{
    public class HolidayRpc : ServiceBase<IHoliday>, IHoliday
    {
        private static IContainer container = InitBuilder.MockBuilder().Build();
        private IDbEntity<JCXX_Holiday> Respostry = container.Resolve<IDbEntity<JCXX_Holiday>>();
        private ILogWriter log = container.Resolve<ILogWriter>();

        private TimeSpan Work_StartTime1 = new TimeSpan(9, 0, 0);
        private TimeSpan Work_EndTime1 = new TimeSpan(11, 0, 0);

        private TimeSpan Work_StartTime2 = new TimeSpan(13, 0, 0);
        private TimeSpan Work_EndTime2 = new TimeSpan(17, 0, 0);

        public HolidayRpc()
        {
            Respostry.SetTable(JCXX_Holiday.GetTbName());
        }

        /// <summary>
        /// 计算两个时间之间的工作时间差(单位：小时)
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public UnaryResult<int> CalculateHour(DateTime StartTime, DateTime EndTime)
        {
            int result = 0;
            DateTime tmpTime;
            if (StartTime < EndTime)
            {
                //获取所有假期
                var listHoliday = Respostry.Select().Where(t => t.IsDelete.Eq(0)).QueryList();
                while (StartTime < EndTime)
                {
                    tmpTime = StartTime.AddHours(1);
                    //如果不是节假日
                    if (!IsHoliday(listHoliday, tmpTime))
                    {
                        //如果当前时间在工作时间内 累加一小时
                        if ((tmpTime >= tmpTime.Date.Add(Work_StartTime1) && tmpTime <= tmpTime.Date.Add(Work_EndTime1))
                            || (tmpTime >= tmpTime.Date.Add(Work_StartTime2) && tmpTime <= tmpTime.Date.Add(Work_EndTime2)))
                            result += 1;
                    }
                    StartTime = tmpTime;
                }
            }
            return UnaryResult(result);
        }

        /// <summary>
        /// 给定时间并加上时限后，得出截止时间
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public UnaryResult<DateTime> CalculateDueTime(DateTime dt, TimeSpan time)
        {
            DateTime result = dt;

            //获取所有假期
            var listHoliday = Respostry.Select().Where(t => t.IsDelete.Eq(0)).QueryList();

            #region 先判断当前时间是否为工作日、工作时间，不是则调整为最近的工作开始时间

            //当前日是否为工作日
            while (IsHoliday(listHoliday, dt))
                dt = dt.AddDays(1);
            //当前时间是否为工作时间
            if (dt < dt.Date.Add(Work_StartTime1))
                dt = dt.Date.Add(Work_StartTime1);
            else if (dt > dt.Date.Add(Work_EndTime1) && dt < dt.Date.Add(Work_StartTime2))
                dt = dt.Date.Add(Work_StartTime2);
            else if (dt > dt.Date.Add(Work_EndTime2))
            {
                dt = dt.AddDays(1).Date.Add(Work_StartTime1);
                while (IsHoliday(listHoliday, dt))
                    dt = dt.AddDays(1);
            }

            #endregion 先判断当前时间是否为工作日、工作时间，不是则调整为最近的工作开始时间

            #region 判断当前所属工作时间段的剩余工作时间是否大于时限，不是则先扣光，然后进入下一个时间段继续计算

            bool isOk = false;
            while (!isOk)
            {
                //处于第一个时间段
                if (dt.TimeOfDay <= Work_EndTime1)
                {
                    if (Work_EndTime1 - dt.TimeOfDay >= time)
                    {
                        result = dt.Add(time);
                        isOk = true;
                    }
                    else
                    {
                        time = time - (Work_EndTime1 - dt.TimeOfDay);
                        dt = dt.Date.Add(Work_StartTime2);
                    }
                }
                //处于第二个时间段
                else
                    if (Work_EndTime2 - dt.TimeOfDay >= time)
                {
                    result = dt.Add(time);
                    isOk = true;
                }
                else
                {
                    time = time - (Work_EndTime2 - dt.TimeOfDay);
                    dt = dt.AddDays(1).Date + Work_StartTime1;
                    while (IsHoliday(listHoliday, dt))
                        dt = dt.AddDays(1);
                }
            }

            #endregion 判断当前所属工作时间段的剩余工作时间是否大于时限，不是则先扣光，然后进入下一个时间段继续计算

            return UnaryResult(result);
        }

        /// <summary>
        /// 判断是否为节假日
        /// </summary>
        /// <param name="listHoliday"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool IsHoliday(List<JCXX_Holiday> listHoliday, DateTime dt)
        {
            return dt.DayOfWeek.ToString() == "Sunday" //非星期日
                || dt.DayOfWeek.ToString() == "Saturday" //非星期六
                || listHoliday.Where(t => t.Holiday.ToShortDateString() == dt.ToShortDateString()).Count() > 0 //非节假日
                ;
        }
    }
}