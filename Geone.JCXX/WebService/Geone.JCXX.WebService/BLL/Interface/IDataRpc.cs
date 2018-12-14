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
        UnaryResult<DateTime> GetCLLimit(string caseCondition);

        /// <summary>
        ///获取处置时限(除了处置环节)
        /// </summary>
        /// <param name="nodeType">类型</param>
        /// <returns></returns>
        UnaryResult<DateTime> GetCaseTimeLimit(string nodeType);
    }
}