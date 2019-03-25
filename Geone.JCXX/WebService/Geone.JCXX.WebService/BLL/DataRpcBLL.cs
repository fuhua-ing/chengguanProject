using Autofac;
using Geone.JCXX.Meta;
using Geone.Utiliy.Build;
using Geone.Utiliy.Database;
using Geone.Utiliy.Logger;
using MagicOnion;
using MagicOnion.Server;
using System;
using System.Linq;

namespace Geone.JCXX.WebService.BLL
{
    public class DataRpcBLL : ServiceBase<IDataRpc>, IDataRpc
    {
        private static IContainer container = InitBuilder.MockBuilder().Build();
        private IDbEntity<JCXX_CaseLATJ> RespostryLATJ = container.Resolve<IDbEntity<JCXX_CaseLATJ>>();
        private IDbEntity<JCXX_CaseTimeLimit> RespostryTL = container.Resolve<IDbEntity<JCXX_CaseTimeLimit>>();
        private IDbEntity<View_DictItem> Respostry_DictItem = container.Resolve<IDbEntity<View_DictItem>>();
        private ILogWriter log = container.Resolve<ILogWriter>();

        /// <summary>
        ///根据立案条件获取处置时限
        /// </summary>
        /// <param name="caseCondition"></param>
        /// <returns></returns>
        public UnaryResult<DateTime> GetCLLimit(string caseCondition, DateTime StartTime)
        {
            try
            {
                RespostryLATJ.SetTable(JCXX_CaseLATJ.GetTbName());
                var q = RespostryLATJ.Select().Where("1=1");
                q.And(t => t.CaseCondition.Eq(caseCondition));
                var list = q.QueryFirst();
                if (list != null && list.TimeLimit != null)
                {
                    IHoliday holiday = new HolidayRpc();
                    StartTime = holiday.CalculateDueTime(StartTime, new TimeSpan(Convert.ToInt32(list.TimeLimit), 0, 0)).ResponseAsync.Result;
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
            }
            return UnaryResult(StartTime);
        }

        /// <summary>
        ///根据立案条件获取处置拒签时间
        /// </summary>
        /// <param name="caseCondition"></param>
        /// <returns></returns>
        public UnaryResult<DateTime> GetCLRefuse(string caseCondition, DateTime StartTime)
        {
            try
            {
                RespostryLATJ.SetTable(JCXX_CaseLATJ.GetTbName());
                var q = RespostryLATJ.Select().Where("1=1");
                q.And(t => t.CaseCondition.Eq(caseCondition));
                var list = q.QueryFirst();
                if (list != null && list.TimeLimit != null)
                {
                    IHoliday holiday = new HolidayRpc();
                    if (list.TimeLimit > 6)//处置时限超过一个工作日（6小时）的工单，给于一个工作日（6小时）的拒签时限
                    {
                        StartTime = holiday.CalculateDueTime(StartTime, new TimeSpan(7, 0, 0)).ResponseAsync.Result;
                        string rTime = StartTime.ToShortDateString() + " 09:00:00";
                        StartTime = Convert.ToDateTime(rTime).ToLocalTime();
                    }
                    else//处置时限小于等于一个工作日（6小时）的工单，给予与处置时限相同的拒签时限
                    {
                        StartTime = holiday.CalculateDueTime(StartTime, new TimeSpan(Convert.ToInt32(list.TimeLimit), 0, 0)).ResponseAsync.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
            }
            return UnaryResult(StartTime);
        }

        /// <summary>
        ///根据立案条件获取处置时限小时
        /// </summary>
        /// <param name="caseCondition"></param>
        /// <returns></returns>
        public UnaryResult<int> GetCLHour(string caseCondition)
        {
            int clHour = 0;
            try
            {
                RespostryLATJ.SetTable(JCXX_CaseLATJ.GetTbName());
                var q = RespostryLATJ.Select().Where("1=1");
                q.And(t => t.CaseCondition.Eq(caseCondition));
                var list = q.QueryFirst();
                if (list != null && list.TimeLimit != null)
                {
                    clHour = Convert.ToInt32(list.TimeLimit);
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
            }
            return UnaryResult(clHour);
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

        /// <summary>
        /// 获取延期审批权限组信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UnaryResult<string> GetDictItemList(string AppID, string CategoryCode)
        {
            try
            {
                Respostry_DictItem.SetTable(View_DictItem.GetTbName());
                var q = Respostry_DictItem.Select().Where("1=1 ");

                q.And(t => t.AppID.Eq(AppID));
                q.And(t => t.CategoryCode.Eq(CategoryCode));

                var list = q.QueryList().Select(m => new
                {
                    ID = m.ID,
                    CategoryID = m.CategoryID,
                    CategoryCode = m.CategoryCode,
                    CategoryName = m.CategoryName,
                    CategoryEnabled = m.CategoryEnabled,
                    ItemCode = m.ItemCode,
                    ItemName = m.ItemName,
                    Enabled = m.Enabled,
                    Note = m.Note
                });

                return UnaryResult(Newtonsoft.Json.JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return UnaryResult("");
            }
        }
    }
}