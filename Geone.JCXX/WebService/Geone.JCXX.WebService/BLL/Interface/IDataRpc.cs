using MagicOnion;
using System;

namespace Geone.JCXX.WebService
{
    public interface IDataRpc : IService<IDataRpc>
    {
        /// <summary>
        ///根据立案条件获取处置时限
        /// </summary>
        /// <param name="caseCondition"></param>
        /// <returns></returns>
        UnaryResult<DateTime> GetCLLimit(string caseCondition, DateTime StartTime);

        /// <summary>
        ///根据立案条件获取处置拒签时间
        /// </summary>
        /// <param name="caseCondition"></param>
        /// <returns></returns>
        UnaryResult<DateTime> GetCLRefuse(string caseCondition, DateTime StartTime);

        /// <summary>
        ///根据立案条件获取处置时限小时
        /// </summary>
        /// <param name="caseCondition"></param>
        /// <returns></returns>
        UnaryResult<int> GetCLHour(string caseCondition);

        /// <summary>
        ///获取处置时限(除了处置环节)
        /// </summary>
        /// <param name="nodeType">类型</param>
        /// <returns></returns>
        UnaryResult<DateTime> GetCaseTimeLimit(string nodeType);

        /// <summary>
        /// 获取延期审批权限组信息
        /// </summary>
        /// <param name="AppID"></param>
        /// <param name="CategoryCode"></param>
        /// <returns></returns>
        UnaryResult<string> GetDictItemList(string AppID, string CategoryCode);

        /// <summary>
        /// 根据点位获取网格和标段
        /// </summary>
        /// <param name="point"></param>
        /// <param name="gridType"></param>
        /// <returns></returns>
        UnaryResult<string> GetGridAndArea(string point, string[] gridType = null);
    }
}