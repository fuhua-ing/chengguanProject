using Autofac;
using Geone.JCXX.Meta;
using Geone.Utiliy.Build;
using Geone.Utiliy.Database;
using Geone.Utiliy.Logger;
using MagicOnion;
using MagicOnion.Server;
using System;

namespace Geone.JCXX.WebService.BLL
{
    public class DataRpcBLL : ServiceBase<IDataRpc>, IDataRpc
    {
        private static IContainer container = InitBuilder.MockBuilder().Build();
        private IDbEntity<JCXX_CaseLATJ> RespostryLATJ = container.Resolve<IDbEntity<JCXX_CaseLATJ>>();
        private IDbEntity<JCXX_CaseTimeLimit> RespostryTL = container.Resolve<IDbEntity<JCXX_CaseTimeLimit>>();
        private ILogWriter log = container.Resolve<ILogWriter>();

        /// <summary>
        ///根据立案条件获取处置时限
        /// </summary>
        /// <param name="caseCondition"></param>
        /// <returns></returns>
        public UnaryResult<DateTime> GetCLLimit(string caseCondition)
        {
            DateTime limitTime = DateTime.Now;
            try
            {
                RespostryLATJ.SetTable(JCXX_CaseLATJ.GetTbName());
                var q = RespostryLATJ.Select().Where("1=1");
                q.And(t => t.CaseCondition.Eq(caseCondition));
                var list = q.QueryFirst();
                if (list != null && list.TimeLimit != null)
                {
                    IHoliday holiday = new HolidayRpc();
                    limitTime = holiday.CalculateDueTime(limitTime, new TimeSpan(Convert.ToInt32(list.TimeLimit), 0, 0)).ResponseAsync.Result;
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
            }
            return UnaryResult(limitTime);
        }

        /// <summary>
        ///获取处置时限(除了处置环节)
        /// </summary>
        /// <param name="nodeType">类型</param>
        /// <returns></returns>
        public UnaryResult<DateTime> GetCaseTimeLimit(string nodeType)
        {
            DateTime limitTime = DateTime.Now;
            try
            {
                RespostryTL.SetTable(JCXX_CaseTimeLimit.GetTbName());
                var q = RespostryTL.Select().Where("1=1");
                q.And(t => t.NodeType.Eq(nodeType));
                var list = q.QueryFirst();
                if (list != null && list.TimeLimit != null)
                {
                    IHoliday holiday = new HolidayRpc();
                    limitTime = holiday.CalculateDueTime(limitTime, new TimeSpan(Convert.ToInt32(list.TimeLimit), 0, 0)).ResponseAsync.Result;
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
            }
            return UnaryResult(limitTime);
        }
    }
}