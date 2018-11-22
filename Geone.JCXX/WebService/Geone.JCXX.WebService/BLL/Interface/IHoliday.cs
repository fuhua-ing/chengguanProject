using MagicOnion;
using System;

namespace Geone.JCXX.WebService
{
    public interface IHoliday : IService<IHoliday>
    {
        /// <summary>
        /// 计算两个时间之间的工作时间差(单位：小时)
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        UnaryResult<int> CalculateHour(DateTime StartTime, DateTime EndTime);

        /// <summary>
        /// 给定时间并加上时限后，得出截止时间
        /// </summary>
        /// <param name="dt">需要计算的开始时间</param>
        /// <param name="time">需要加上的时间</param>
        /// <returns></returns>
        UnaryResult<DateTime> CalculateDueTime(DateTime dt, TimeSpan time);
    }
}