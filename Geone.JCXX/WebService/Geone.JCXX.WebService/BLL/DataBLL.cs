using Geone.JCXX.Meta;
using Geone.JCXX.WebService.Meta.QueryEntity;
using Geone.JCXX.WebService.Meta.Response;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geone.JCXX.WebService
{
    public class DataBLL : IDataService
    {
        private IDbEntity<View_DictItem> Respostry_DictItem;
        private IDbEntity<View_QSRole> Respostry_QSRole;
        private IDbEntity<View_QSRoleUser> Respostry_QSRoleUser;
        private IDbEntity<JCXX_Grid> Respostry_Grid;
        private IDbEntity<JCXX_Grid_Config> Respostry_GridConfig;
        private IDbEntity<JCXX_GridQSRoleTree> Respostry_qsTree;
        private IDbEntity<View_Grid> Respostry_VGrid;
        private IDbEntity<JCXX_Dept> Respostry_Dept;
        private IDbEntity<JCXX_CaseLATJ> Respostry_LATJ;
        private IDbEntity<View_QSRoleGrid> Respostry_QSRG;
        private ILogWriter log;

        public DataBLL(IDbEntity<View_DictItem> _DictItem,
            IDbEntity<View_QSRole> _QSRole,
            IDbEntity<View_QSRoleUser> _QSRoleUser,
            IDbEntity<JCXX_Grid> _Grid,
             IDbEntity<JCXX_Grid_Config> _GridConfig,
            IDbEntity<View_Grid> _VGrid,
            IDbEntity<JCXX_Dept> _Dept,
            IDbEntity<JCXX_CaseLATJ> _LATJ,
            IDbEntity<View_QSRoleGrid> _QSRG,
            IDbEntity<JCXX_GridQSRoleTree> _QSTree,
            ILogWriter logWriter)
        {
            Respostry_DictItem = _DictItem;
            Respostry_DictItem.SetTable(View_DictItem.GetTbName());

            Respostry_QSRole = _QSRole;
            Respostry_QSRole.SetTable(View_QSRole.GetTbVName());

            Respostry_Grid = _Grid;
            Respostry_Grid.SetTable(JCXX_Grid.GetTbName());

            Respostry_GridConfig = _GridConfig;
            Respostry_GridConfig.SetTable(JCXX_Grid_Config.GetTbName());

            Respostry_VGrid = _VGrid;
            Respostry_VGrid.SetTable(View_Grid.GetTbName());

            Respostry_Dept = _Dept;
            Respostry_Dept.SetTable(JCXX_Dept.GetTbName());

            Respostry_LATJ = _LATJ;
            Respostry_LATJ.SetTable(JCXX_CaseLATJ.GetTbName());

            Respostry_QSRG = _QSRG;
            Respostry_QSRG.SetTable(View_QSRoleGrid.GetTbName());

            Respostry_QSRoleUser = _QSRoleUser;
            Respostry_QSRoleUser.SetTable(View_QSRoleUser.GetTbName());
            Respostry_qsTree = _QSTree;
            Respostry_qsTree.SetTable(JCXX_GridQSRoleTree.GetTbName());

            log = logWriter;
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
                log.WriteException(ex);
                return RepModel.Error();
            }
        }

        /// <summary>
        /// 根据网格ID获取该网格所有的权属角色
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RepModel GetQSRoleTree(Req_Grid query)
        {
            try
            {
                var gridList = Respostry_Grid.Select().Where("1=1");

                if (!string.IsNullOrEmpty(query.Point))
                {
                    gridList.And(t => t.Shape.IsContains(query.Point));
                    var gResult = gridList.QueryFirst();
                    if (gResult != null)
                    {
                        var q = Respostry_qsTree.Select().Where("1=1");
                        q.And(t => t.GridID.Eq(gResult.ID));
                        if (query.Enabled != null)
                            q.And(t => t.Enabled.Eq(query.Enabled));
                        var list = q.QueryList().Select(m => new
                        {
                            ID = m.ID,
                            RoleID = m.RoleID,
                            RoleParentID = m.RoleParentID,
                            RoleName = m.RoleName,
                            Enabled = m.Enabled
                        });
                        return RepModel.Success(list);
                    }
                }

                return RepModel.Success(null);
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return RepModel.Error();
            }
        }

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
                if (!string.IsNullOrEmpty(query.OperatorLevel) && query.OperatorLevel.Contains("2"))
                {
                    q.And(t => t.ParentCode.Eq("SBJPQRJ"));
                }
                else if (!string.IsNullOrEmpty(query.OperatorLevel) && query.OperatorLevel.Contains("1"))
                {
                    q.And(t => t.ParentCode.Ne("SBJPQRJ"));
                    q.And(t => t.Note.Ne(""));
                }
                if (!string.IsNullOrEmpty(query.NoContainsRoleType))
                    q.And(t => t.RoleType.Ne(query.NoContainsRoleType));
                if (!string.IsNullOrEmpty(query.ParentCode))
                    q.And(t => t.ParentCode.Eq(query.ParentCode));
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
                    RoleID = m.ID,
                    RoleType = m.RoleType,
                    RoleTypeDesc = m.RoleTypeDesc,
                    RoleCode = m.RoleCode,
                    RoleParentID = m.ParentCode,
                    RoleName = m.RoleName,
                    Note = m.Note,
                    Enabled = m.Enabled
                });
                List<Rtn_QSRole> rtn_QSRoleList = new List<Rtn_QSRole>();
                var resList = list.ToList();
                //获取角色类型（第一级）
                var RoleTypeList = from p in list
                                   group p by p.RoleType into g
                                   select g;
                var RList = RoleTypeList.ToList();

                if (query.OperatorLevel != "2")
                {
                    foreach (var item in RList)
                    {
                        var value = item.First();
                        rtn_QSRoleList.Add(new Rtn_QSRole { RoleID = value.RoleType, RoleName = value.RoleTypeDesc, RoleParentID = "RoleType" });
                    }

                    var noteList = from p in list
                                   group p by p.Note into g
                                   select g;
                    var NList = noteList.ToList();

                    foreach (var item in RList)
                    {
                        var value = item.First();
                        rtn_QSRoleList.Add(new Rtn_QSRole { RoleID = value.Note, RoleName = value.Note, RoleParentID = value.RoleType });
                    }
                }
                foreach (var item in resList)
                {
                    rtn_QSRoleList.Add(new Rtn_QSRole
                    {
                        RoleID = item.RoleID,
                        RoleType = item.RoleType,
                        RoleTypeDesc = item.RoleTypeDesc,
                        RoleCode = item.RoleCode,
                        RoleParentID = item.Note,
                        RoleName = item.RoleName,
                        Note = item.Note,
                        Enabled = item.Enabled
                    });
                }
                return RepModel.Success(rtn_QSRoleList);
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
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
                if (!string.IsNullOrEmpty(query.GridType))
                    q.And(t => t.RoleType.Eq(query.GridType));
                var list = q.QueryList().Select(m => new
                {
                    ID = m.RoleID,
                    RoleType = m.RoleType,
                    RoleName = m.RoleName,
                    RoleCode = m.RoleCode
                });
                return RepModel.Success(list);
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
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
                if (query.GridTypeList != null && query.GridTypeList.Count > 0)
                    q.And(t => t.GridType.In(query.GridTypeList.Keys.ToArray()));
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
                log.WriteException(ex);
                return RepModel.Error();
            }
        }

        /// <summary>
        /// 查询网格动态参数列表
        /// </summary>
        /// <returns></returns>
        public RepModel GetGridConfigList(Req_Grid query)
        {
            try
            {
                string[] listCode = null;
                var q = Respostry_Grid.Select().Where("1=1");
                if (!string.IsNullOrEmpty(query.GridType))
                {

                    q.And(t => t.GridType.Eq(query.GridType));
                    listCode = q.QueryList().Select(m => m.GridCode).ToArray();
                }
                var q2 = Respostry_GridConfig.Select().Where("1=1");

                if (listCode != null)
                {
                    q2.And(t => t.GridCode.In(listCode));
                }
                var result = q2.QueryList();
                return RepModel.Success(result);
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
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
                log.WriteException(ex);
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
                log.WriteException(ex);
                return RepModel.Error();
            }
        }
    }
}