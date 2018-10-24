using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;

namespace Geone.JCXX.BLL
{
    public class GridBLL : IGridBLL
    {
        private IDbEntity<JCXX_Grid> Respostry;
        private IDbEntity<View_Grid> Respostry_V;
        private IDbEntity<JCXX_QSRole_Grid> Respostry_RG;
        private IDbEntity<View_QSRoleGrid> Respostry_VRG;
        LogWriter log = new LogWriter(new FileLogRecord());

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public GridBLL(IDbEntity<JCXX_Grid> _t, IDbEntity<JCXX_QSRole_Grid> _trg,
            IDbEntity<View_Grid> _tv, IDbEntity<View_QSRoleGrid> _tvrg)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_Grid");

            Respostry_V = _tv;
            Respostry_V.SetTable("View_Grid");

            Respostry_RG = _trg;
            Respostry_RG.SetTable("JCXX_QSRole_Grid");

            Respostry_VRG = _tvrg;
            Respostry_VRG.SetTable("View_QSRoleGrid");
        }


        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_Grid query)
        {
            var list = Respostry_V.Select();
            SetQuery(list, query);
            if (string.IsNullOrEmpty(query.sort))
                query.sort = "CREATED";
            var result = list.QueryPage((int)query.page, (int)query.rows, query.sort);
            return new GridData() { rows = result.Rows, total = result.Total };
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<View_Grid> GetList(Query_Grid query)
        {
            try
            {
                var list = Respostry_V.Select();
                SetQuery(list, query);
                switch (query.sort)
                {
                    case "GridCode":

                        list = query.order == "asc" ? list.OrderBy(t => t.GridCode) : list.OrderByDesc(t => t.GridCode);
                        break;
                    default:
                        list = list.OrderByDesc(t => t.CREATED);
                        break;
                }
                return list.QueryList();
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new List<View_Grid>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<View_Grid> list, Query_Grid query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.GridCode))
                list = list.And(t => t.GridCode.Eq(query.GridCode));
            if (!string.IsNullOrEmpty(query.GridType))
                list = list.And(t => t.GridType.Eq(query.GridType));
            if (!string.IsNullOrEmpty(query.Like_GridCode))
                list = list.And(t => t.GridCode.Like("%" + query.Like_GridCode + "%"));
            if (!string.IsNullOrEmpty(query.Like_GridName))
                list = list.And(t => t.GridName.Like("%" + query.Like_GridName + "%"));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));

        }


        public View_Grid GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new View_Grid();
                else
                {
                    return Respostry_V.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new View_Grid();
            }
        }

        public RepModel Save(JCXX_Grid entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.ID))
                {
                    entity.ID = Guid.NewGuid().ToString();
                    entity.IsDelete = 0;
                    entity.Enabled = 1;
                    entity.CREATED = DateTime.Now;
                    entity.CREATED_MAN = entity.CREATED_MAN;
                    return Respostry.Insert(entity).ExecInsert() ? RepModel.Success("新增成功") : RepModel.Error("新增失败");
                }
                else
                {
                    entity.UPDATED = DateTime.Now;
                    entity.UPDATED_MAN = entity.UPDATED_MAN;
                    return Respostry.Minus(t => t.CREATED, t => t.CREATED_MAN, t => t.IsDelete).
                       UpdateByPKey(entity).ExecModify() ? RepModel.Success("更新成功") : RepModel.Error("更新失败");
                }
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error(ex.Message);
            }
        }

        public RepModel Del(string ID)
        {
            try
            {
                return Respostry.DeleteByPKey(ID).ExecRemove() ? RepModel.Success("删除成功") : RepModel.Error("删除失败");
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error(ex.Message);
            }
        }

        public RepModel Import(List<JCXX_Grid> list, string GridType)
        {
            try
            {
                var models = new List<JCXX_Grid>();
                foreach (var entity in list)
                {
                    if (string.IsNullOrEmpty(entity.Shape))
                        return RepModel.Error("第" + (list.IndexOf(entity) + 1) + "行的Shape不能为空");
                    models.Add(new JCXX_Grid()
                    {
                        ID = Guid.NewGuid().ToString(),
                        GridType = GridType,
                        IsDelete = 0,
                        Enabled = 1,
                        CREATED = DateTime.Now,
                        CREATED_MAN = ""
                    });
                }
                return Respostry.Insert(models).ExecInsertBatch() ? RepModel.Success("导入成功") : RepModel.Error("导入失败");
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error(ex.Message);
            }
        }


        #region 权属角色网格设置

        /// <summary>
        /// 获取权属角色网格列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<View_QSRoleGrid> GetRoleGridList(Query_Grid query)
        {

            var list = Respostry_VRG.Select();
            if (!string.IsNullOrEmpty(query.ID))
                list = list.Where(t => t.GridID.Eq(query.ID));
            return list.QueryList();
        }

        /// <summary>
        /// 保存角色网格
        /// </summary>
        /// <param name="GridID"></param>
        /// <param name="RoleIDs"></param>
        /// <returns></returns>
        public RepModel SaveRoleGrid(string GridID, string RoleIDs, string CREATED_MAN)
        {
            try
            {
                if (string.IsNullOrEmpty(GridID))
                    return RepModel.Error("网格不能为空");
                //删除原有角色网格
                Respostry_RG.Delete().Where(t => t.GridID.Eq(GridID)).ExecRemove();
                //新增现有角色网格
                var listNew = new List<JCXX_QSRole_Grid>();
                foreach (var RoleID in RoleIDs.Split(','))
                {
                    if (string.IsNullOrEmpty(RoleID))
                        continue;
                    listNew.Add(new View_QSRoleGrid()
                    {
                        ID = Guid.NewGuid().ToString(),
                        RoleID = RoleID,
                        GridID = GridID,
                        CREATED = DateTime.Now,
                        CREATED_MAN = CREATED_MAN
                    });
                }
                return listNew.Count == 0 || Respostry_RG.Insert(listNew).ExecInsertBatch() ? RepModel.Success("操作成功") : RepModel.Error("操作失败");

            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error(ex.Message);
            }
        }
        #endregion
    }
}
