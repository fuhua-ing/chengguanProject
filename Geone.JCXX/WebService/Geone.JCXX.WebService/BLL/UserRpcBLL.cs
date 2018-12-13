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
    public class UserRpcBLL : ServiceBase<IUserRpc>, IUserRpc
    {
        private static IContainer container = InitBuilder.MockBuilder().Build();
        private IDbEntity<View_QSRoleUser> Respostry = container.Resolve<IDbEntity<View_QSRoleUser>>();

        private ILogWriter log = container.Resolve<ILogWriter>();

        /// <summary>
        /// 根据用户ID获取权属角色
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UnaryResult<string> GetQSRoleList(string userID)
        {
            try
            {
                Respostry.SetTable(View_QSRoleUser.GetTbName());
                var q = Respostry.Select().Where("1=1");
                q.And(t => t.UserID.Eq(userID));

                var list = q.QueryList();

                return UnaryResult(Newtonsoft.Json.JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return UnaryResult("");
            }
        }

        /// <summary>
        /// 根据用户编号或者角色编号获取部门名称
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UnaryResult<string> GetDeptName(string userOrRoleID)
        {
            string deptName = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(userOrRoleID))
                {
                    Respostry.SetTable(View_QSRoleUser.GetTbName());
                    var q = Respostry.Select().Where("1=1");

                    string[] idList = userOrRoleID.Split(",");
                    q.And();
                    q.Or(t => t.UserID.In(idList), t => t.RoleID.In(idList));

                    var list = q.QueryList();
                    if (list != null && list.Count > 0)
                    {
                        deptName = list[0].DeptName;
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
            }
            return UnaryResult(deptName);
        }
    }
}