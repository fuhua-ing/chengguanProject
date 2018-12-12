using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geone.JCXX.BLL
{
    public class GridBLL : IGridBLL
    {
        private IDbEntity<JCXX_Grid> Respostry;
        private IDbEntity<View_Grid> Respostry_V;
        private IDbEntity<JCXX_QSRole> Respostry_Role;
        private IDbEntity<JCXX_GridQSRoleTree> Respostry_R;
        private IDbEntity<JCXX_QSRole_Grid> Respostry_RG;
        private IDbEntity<View_QSRoleGrid> Respostry_VRG;
        private ILogWriter log;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public GridBLL(IDbEntity<JCXX_Grid> _t, IDbEntity<JCXX_QSRole_Grid> _trg,
            IDbEntity<View_Grid> _tv, IDbEntity<JCXX_GridQSRoleTree> _r, IDbEntity<View_QSRoleGrid> _tvrg, IDbEntity<JCXX_QSRole> _role,
            ILogWriter logWriter)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_Grid");

            Respostry_V = _tv;
            Respostry_V.SetTable("View_Grid");

            Respostry_R = _r;
            Respostry_R.SetTable("JCXX_GridQSRoleTree");

            Respostry_Role = _role;
            Respostry_Role.SetTable("JCXX_QSRole");

            Respostry_RG = _trg;
            Respostry_RG.SetTable("JCXX_QSRole_Grid");

            Respostry_VRG = _tvrg;
            Respostry_VRG.SetTable("View_QSRoleGrid");

            log = logWriter;
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
                log.WriteException(ex);
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
                log.WriteException(ex);
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
                log.WriteException(ex);
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
                log.WriteException(ex);
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
                log.WriteException(ex);
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

                Respostry_RG.Delete().Where(t => t.GridID.Eq(GridID)).ExecRemove();//删除原有角色网格
                Respostry_R.Delete().Where(t => t.GridID.Eq(GridID)).ExecRemove();

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

                //新增现有网格权属角色
                var listRoleTree = new List<JCXX_GridQSRoleTree>();
                foreach (var RoleID in RoleIDs.Split(','))
                {
                    if (string.IsNullOrEmpty(RoleID))
                        continue;
                    var QSRoleEntity = GetQSRoleByID(RoleID);
                    listRoleTree.Add(new JCXX_GridQSRoleTree()
                    {
                        ID = Guid.NewGuid().ToString(),
                        RoleID = RoleID,
                        RoleName = QSRoleEntity.RoleName,
                        GridID = GridID,
                        CREATED = DateTime.Now,
                        CREATED_MAN = CREATED_MAN
                    });
                }

                return (listNew.Count == 0 || Respostry_RG.Insert(listNew).ExecInsertBatch()) && (listRoleTree.Count == 0 || Respostry_R.Insert(listRoleTree).ExecInsertBatch()) ? RepModel.Success("操作成功") : RepModel.Error("操作失败");
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return RepModel.Error(ex.Message);
            }
        }

        #endregion 权属角色网格设置

        #region 网格权属角色多级设置
        /// <summary>
        /// 根据网格ID，获取网格权属角色列表
        /// </summary>
        /// <param name="GridID">网格ID</param>
        /// <returns></returns>
        public List<JCXX_GridQSRoleTree> GetGridQSRoleTreeList(string GridID)
        {
            try
            {
                var list = Respostry_R.Select().Where(r => r.GridID.Eq(GridID));
                return list.QueryList();
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new List<JCXX_GridQSRoleTree>();
            }
        }

        /// <summary>
        /// 根据网格ID，获取网格权属角色树形结构
        /// </summary>
        /// <param name="GridID"></param>
        /// <returns></returns>
        public List<EasyuiTreeNode_GridQSRoleTree> GetTreeList(string GridID)
        {
            var listResut = new List<EasyuiTreeNode_GridQSRoleTree>();
            var listAll = GetGridQSRoleTreeList(GridID);
            if (listAll == null)
            {
                return null;
            }

            var listParent = listAll.Where(t => string.IsNullOrEmpty(t.RoleParentID)).OrderBy(t => t.RoleName).ToList();
            //如果查找结果不为0，根目录为0
            if (listAll.Count > 0 && listParent.Count() == 0)
            {
                var q = from p in listAll
                        group p by p.RoleParentID into g
                        select new { g.Key };
                foreach (var item in q)
                {
                    if (listAll.Where(t => t.RoleID == item.Key).Count() == 0)
                    {
                        listParent.AddRange(listAll.Where(t => t.RoleParentID == item.Key).ToList());
                    }
                }
            }
            foreach (JCXX_GridQSRoleTree parent in listParent)
            {
                var parentNode = new EasyuiTreeNode_GridQSRoleTree()
                {
                    text = parent.RoleName,
                    id = parent.RoleID,
                    parentid = "",
                    children = new List<EasyuiTreeNode_GridQSRoleTree>(),
                    ID = parent.RoleID
                    //Enabled = parent.Enabled
                };
                setSubTreeList(parentNode, listAll);
                listResut.Add(parentNode);
            }
            var firstListResut = new List<EasyuiTreeNode_GridQSRoleTree>();

            var firstPparentNode = new EasyuiTreeNode_GridQSRoleTree()
            {
                text = "父级",
                id = "",
                parentid = null,
                ischecked = 1,
                children = listResut.Where(r => r.parentid == "").ToList()
            };
            firstListResut.Add(firstPparentNode);
            //firstListResut=listResut;
            return firstListResut;
        }

        /// <summary>
        /// 子树
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="listAll"></param>
        private void setSubTreeList(EasyuiTreeNode_GridQSRoleTree ParentNode, List<JCXX_GridQSRoleTree> listAll)
        {
            foreach (JCXX_GridQSRoleTree child in listAll.Where(t => t.RoleParentID == ParentNode.id).OrderBy(t => t.RoleName))
            {
                var childNode = new EasyuiTreeNode_GridQSRoleTree()
                {
                    text = child.RoleName,
                    id = child.RoleID,
                    parentid = ParentNode.id,
                    children = new List<EasyuiTreeNode_GridQSRoleTree>(),
                    ID = child.RoleID
                    //Enabled = child.Enabled
                };
                setSubTreeList(childNode, listAll);
                ParentNode.children.Add(childNode);
            }
        }

        /// <summary>
        /// 根据网格ID和角色ID，更新原归属的父级角色ID
        /// </summary>
        /// <param name="GridID">网格ID</param>
        /// <param name="RoleID">角色ID</param>
        /// <param name="RoleParentID">父级角色ID</param>
        /// <param name=""></param>
        /// <returns></returns>
        public RepModel UpdateTreeByID(string GridID, string RoleID, string RoleParentID)
        {
            RepModel result = new RepModel();

            JCXX_GridQSRoleTree oldEntity = GetGridQSRoleTreeByID(RoleID, GridID);
            oldEntity.RoleParentID = RoleParentID;
            oldEntity.UPDATED = DateTime.Now;
            return Respostry_R.UpdateByPKey(oldEntity).ExecModify() ? RepModel.Success("更新成功") : RepModel.Error("更新失败");
        }

        /// <summary>
        /// 根据网格ID和角色ID，查询数据
        /// </summary>
        /// <param name="GridID">网格ID</param>
        /// <param name="RoleID">角色ID</param>
        /// <returns></returns>
        public JCXX_GridQSRoleTree GetGridQSRoleTreeByID(string RoleID, string GridID)
        {
            try
            {
                if (string.IsNullOrEmpty(RoleID) || string.IsNullOrEmpty(GridID))
                    return new JCXX_GridQSRoleTree();
                else
                {
                    return Respostry_R.Select().Where(t => t.GridID.Eq(GridID)).And(t => t.RoleID.Eq(RoleID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new JCXX_GridQSRoleTree();
            }
        }

        /// <summary>
        /// 根据权属角色ID，获取数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JCXX_QSRole GetQSRoleByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_QSRole();
                else
                {
                    return Respostry_Role.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new JCXX_QSRole();
            }
        }
        #endregion 
    }
}