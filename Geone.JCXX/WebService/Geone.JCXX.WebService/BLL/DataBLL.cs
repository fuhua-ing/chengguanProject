using Autofac;
using Geone.JCXX.Meta;
using Geone.JCXX.WebService.Meta.QueryEntity;
using Geone.Utiliy.Build;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using System;
using System.Linq;

namespace Geone.JCXX.WebService
{
    public class DataBLL : IDataService
    {
        private static IContainer container = AutofacSettings.Build();
        private IDbEntity<View_DictItem> Respostry_DictItem;
        private IDbEntity<JCXX_QSRole> Respostry_QSRole;
        private IDbEntity<View_QSRoleUser> Respostry_QSRoleUser;

        private IDbEntity<JCXX_Grid> Respostry_Grid;
        private IDbEntity<View_Grid> Respostry_VGrid;
        private IDbEntity<JCXX_Dept> Respostry_Dept;
        private IDbEntity<JCXX_CaseLATJ> Respostry_LATJ;
        private IDbEntity<View_QSRoleGrid> Respostry_QSRG;
        private LogWriter log = new LogWriter(new FileLogRecord());

        public DataBLL()
        {
            Respostry_DictItem = container.Resolve<IDbEntity<View_DictItem>>();
            Respostry_DictItem.SetTable(View_DictItem.GetTbName());

            Respostry_QSRole = container.Resolve<IDbEntity<JCXX_QSRole>>();
            Respostry_QSRole.SetTable(JCXX_QSRole.GetTbName());

            Respostry_Grid = container.Resolve<IDbEntity<JCXX_Grid>>();
            Respostry_Grid.SetTable(JCXX_Grid.GetTbName());

            Respostry_VGrid = container.Resolve<IDbEntity<View_Grid>>();
            Respostry_VGrid.SetTable(View_Grid.GetTbName());

            Respostry_Dept = container.Resolve<IDbEntity<JCXX_Dept>>();
            Respostry_Dept.SetTable(JCXX_Dept.GetTbName());

            Respostry_LATJ = container.Resolve<IDbEntity<JCXX_CaseLATJ>>();
            Respostry_LATJ.SetTable(JCXX_CaseLATJ.GetTbName());

            Respostry_QSRG = container.Resolve<IDbEntity<View_QSRoleGrid>>();
            Respostry_QSRG.SetTable(View_QSRoleGrid.GetTbName());

            Respostry_QSRoleUser = container.Resolve<IDbEntity<View_QSRoleUser>>();
            Respostry_QSRoleUser.SetTable(View_QSRoleUser.GetTbName());
        }

        /// <summary>
        /// 查询数据字典明细列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RepModel GetDictItemList(Req_DictItem query)
        {
            try
            {
                var q = Respostry_DictItem.Select().Where("1=1");
                if (!string.IsNullOrEmpty(query.AppID))
                    q.And(t => t.AppID.Eq(query.AppID));
                if (!string.IsNullOrEmpty(query.Note))
                    q.And(t => t.Note.Eq(query.Note));
                if (!string.IsNullOrEmpty(query.CategoryCode))
                    q.And(t => t.CategoryCode.Eq(query.CategoryCode));
                if (query.CategoryEnabled != null)
                    q.And(t => t.CategoryEnabled.Eq(query.CategoryEnabled));
                if (query.ItemEnabled != null)
                    q.And(t => t.Enabled.Eq(query.ItemEnabled));

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
                return RepModel.Success(list);
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error();
            }
        }

        ///// <summary>
        ///// 根据用户ID获取当前权属信息
        ///// </summary>
        ///// <param name="UserID"></param>
        ///// <returns></returns>
        //public UnaryResult<RepModel> GetQSRoleListByUserID(string UserID)
        //{
        //    return null;
        //    //try
        //    //{
        //    //    var q = Respostry_QSRoleUser.Select().Where("1=1");
        //    //    if (!string.IsNullOrEmpty(UserID))
        //    //        q.And(t => t.UserID.Eq(UserID));

        //    //    var list = q.QueryList().Select(m => new
        //    //    {
        //    //        ID = m.ID,
        //    //        RoleType = m.RoleType,
        //    //        RoleCode = m.RoleCode,
        //    //        RoleName = m.RoleName
        //    //    });
        //    //    return UnaryResult(RepModel.Success(JsonConvert.SerializeObject(list)));
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    log.WriteException(null, ex);
        //    //    return UnaryResult(RepModel.Error());
        //    //}
        //}
        /// <summary>
        /// 查询权属角色列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RepModel GetQSRoleList(Req_QSRole query)
        {
            try
            {
                var q = Respostry_QSRole.Select().Where("1=1");
                if (!string.IsNullOrEmpty(query.RoleType))
                    q.And(t => t.RoleType.Eq(query.RoleType));
                if (!string.IsNullOrEmpty(query.Like_RoleCode))
                    q.And(t => t.RoleCode.Like("%" + query.Like_RoleCode + "%"));
                if (!string.IsNullOrEmpty(query.Like_RoleName))
                    q.And(t => t.RoleName.Like("%" + query.Like_RoleName + "%"));
                if (query.Enabled != null)
                    q.And(t => t.Enabled.Eq(query.Enabled));
                var list = q.QueryList().Select(m => new
                {
                    ID = m.ID,
                    RoleType = m.RoleType,
                    RoleCode = m.RoleCode,
                    RoleName = m.RoleName,
                    Note = m.Note,
                    Enabled = m.Enabled
                });
                return RepModel.Success(list);
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error();
            }
        }

        /// <summary>
        /// 根据网格点位获取到对应的网格权属角色
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RepModel GetQSRoleGridList(Req_Grid query)
        {
            try
            {
                var q = Respostry_QSRG.Select().Where("1=1");
                if (!string.IsNullOrEmpty(query.Point))
                    q.And(t => t.Shape.IsContains(query.Point));
                var list = q.QueryList().Select(m => new
                {
                    ID = m.RoleID,
                    RoleType = m.RoleType,
                    RoleName = m.RoleName
                });
                return RepModel.Success(list);
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error();
            }
        }

        /// <summary>
        /// 查询网格列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RepModel GetGridList(Req_Grid query)
        {
            try
            {
                var q = Respostry_VGrid.Select().Where("1=1");
                if (!string.IsNullOrEmpty(query.GridType))
                    q.And(t => t.GridType.Eq(query.GridType));
                if (!string.IsNullOrEmpty(query.Like_GridCode))
                    q.And(t => t.GridCode.Like("%" + query.Like_GridCode + "%"));
                if (!string.IsNullOrEmpty(query.Like_GridName))
                    q.And(t => t.GridName.Like("%" + query.Like_GridName + "%"));
                if (!string.IsNullOrEmpty(query.Point))
                    q.And(t => t.Shape.IsContains(query.Point));
                if (query.Enabled != null)
                    q.And(t => t.Enabled.Eq(query.Enabled));
                var list = q.QueryList().Select(m => new
                {
                    ID = m.ID,
                    GridType = m.GridType,
                    GridCode = m.GridCode,
                    GridName = m.GridName,
                    ShowCode = m.GridCode,
                    ShowName = m.ShowName,
                    Shape = m.Shape,
                    GridArea = m.GridArea,
                    Note = m.Note,
                    Enabled = m.Enabled
                });
                return RepModel.Success(list);
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error();
            }
        }

        /// <summary>
        /// 查询立案条件列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RepModel GetCaseLATJList(Req_CaseLATJ query)
        {
            try
            {
                var q = Respostry_LATJ.Select().Where("1=1");
                if (!string.IsNullOrEmpty(query.CaseClassI))
                    q.And(t => t.CaseClassI.Eq(query.CaseClassI));
                if (!string.IsNullOrEmpty(query.CaseClassII))
                    q.And(t => t.CaseClassII.Eq(query.CaseClassII));
                var list = q.QueryList().Select(m => new
                {
                    ID = m.ID,
                    CaseClassI = m.CaseClassI,
                    CaseClassII = m.CaseClassII,
                    CaseCondition = m.CaseCondition,
                    CaseConditionDesc = m.CaseConditionDesc,
                    TimeLimit = m.TimeLimit,
                    TimeLimitDesc = m.TimeLimitDesc,
                    JACondition = m.JACondition
                });
                return RepModel.Success(list);
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error();
            }
        }

        /// <summary>
        /// 查询部门列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RepModel GetDeptList(Req_Dept query)
        {
            try
            {
                var q = Respostry_Dept.Select().Where("1=1");
                if (!string.IsNullOrEmpty(query.DeptType))
                    q.And(t => t.DeptType.Eq(query.DeptType));
                if (!string.IsNullOrEmpty(query.Like_DeptCode))
                    q.And(t => t.DeptCode.Like("%" + query.Like_DeptCode + "%"));
                if (!string.IsNullOrEmpty(query.Like_DeptName))
                    q.And(t => t.DeptName.Like("%" + query.Like_DeptName + "%"));
                if (query.Enabled != null)
                    q.And(t => t.Enabled.Eq(query.Enabled));
                var list = q.QueryList().Select(m => new
                {
                    ID = m.ID,
                    ParentID = m.ParentID,
                    DeptCode = m.DeptCode,
                    DeptName = m.DeptName,
                    ShortName = m.ShortName,
                    DeptType = m.DeptType,
                    Contact = m.Contact,
                    ContactTel = m.ContactTel,
                    ContactEmail = m.ContactEmail,
                    Note = m.Note,
                    Enabled = m.Enabled
                });
                return RepModel.Success(list);
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error();
            }
        }
    }
}