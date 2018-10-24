using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;

namespace Geone.JCXX.BLL
{
    public class QSRoleBLL : IQSRoleBLL
    {
        private IDbEntity<JCXX_QSRole> Respostry;
        private IDbEntity<View_QSRole> Respostry_V;
        private IDbEntity<JCXX_QSRole_User> Respostry_RU;
        private IDbEntity<View_QSRoleUser> Respostry_VRU;
        private DictItemBLL itemBLL;
        LogWriter log = new LogWriter(new FileLogRecord());

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public QSRoleBLL(IDbEntity<JCXX_QSRole> _t, IDbEntity<View_QSRole> _tV, IDbEntity<JCXX_QSRole_User> _tRU
            , IDbEntity<View_QSRoleUser> _tVRU, IDbEntity<JCXX_DictItem> _td
            , IDbEntity<View_DictItem> _tvd)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_QSRole");
            Respostry_V = _tV;
            Respostry_V.SetTable("View_QSRole");

            Respostry_RU = _tRU;
            Respostry_RU.SetTable("JCXX_QSRole_User");

            Respostry_VRU = _tVRU;
            Respostry_VRU.SetTable("View_QSRoleUser");

            itemBLL = new DictItemBLL(_td, _tvd);

        }
        public QSRoleBLL(IDbEntity<JCXX_QSRole> _t)
        {
            Respostry = _t;
            Respostry.SetTable(_t.GetType().Name);
        }

        #region 权属角色维护

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_QSRole query)
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
        public List<View_QSRole> GetList(Query_QSRole query)
        {
            try
            {
                var list = Respostry_V.Select();
                SetQuery(list, query);
                switch (query.sort)
                {
                    case "RoleCode":

                        list = query.order == "asc" ? list.OrderBy(t => t.RoleCode) : list.OrderByDesc(t => t.RoleCode);
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
                return new List<View_QSRole>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<View_QSRole> list, Query_QSRole query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.RoleType))
                list = list.And(t => t.RoleType.Like(query.RoleType));
            if (!string.IsNullOrEmpty(query.RoleCode))
                list = list.And(t => t.RoleCode.Like(query.RoleCode));
            if (!string.IsNullOrEmpty(query.Like_RoleName))
                list = list.And(t => t.RoleName.Like("%" + query.Like_RoleName + "%"));
            if (!string.IsNullOrEmpty(query.Like_RoleCode))
                list = list.And(t => t.RoleCode.Like("%" + query.Like_RoleCode + "%"));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));

        }


        public List<JCXX_QSRole> GetAll()
        {
            return Respostry.Select().Where(t => t.IsDelete.Eq(0)).QueryList();
        }

        public JCXX_QSRole GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_QSRole();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new JCXX_QSRole();
            }
        }

        public RepModel Save(JCXX_QSRole entity)
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
                var oldEntity = GetByID(ID);
                oldEntity.IsDelete = 1;
                oldEntity.UPDATED = DateTime.Now;
                oldEntity.UPDATED_MAN = oldEntity.UPDATED_MAN;
                return Respostry.UpdateByPKey(oldEntity).ExecModify() ? RepModel.Success("删除成功") : RepModel.Error("删除失败");
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error(ex.Message);
            }
        }


        /// <summary>
        /// 获取Easyui树形结构
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<EasyuiTreeNode> GetTreeList(Query_QSRole query)
        {
            List<EasyuiTreeNode> listResut = new List<EasyuiTreeNode>();


            List<View_DictItem> listParent = itemBLL.GetExtList(new Query_DictItem()
            {
                Enabled = 1,
                CategoryCode = "QSRoleType"
            });

            List<View_QSRole> rGList = GetList(query);
            foreach (View_DictItem parent in listParent)
            {
                EasyuiTreeNode parentNode = new EasyuiTreeNode()
                {
                    text = parent.ItemName,
                    id = parent.ItemCode,
                    parentid = "",
                    children = new List<EasyuiTreeNode>()
                };
                setSubTreeList(parentNode, rGList);
                listResut.Add(parentNode);
            }
            EasyuiTreeNode firstPparentNode = new EasyuiTreeNode()
            {
                text = "全部",
                id = "",
                parentid = null,
                ischecked = 1,
                children = listResut.Where(r => r.parentid == "").ToList()
            };
            List<EasyuiTreeNode> firstListResut = new List<EasyuiTreeNode>();
            firstListResut.Add(firstPparentNode);
            return firstListResut;
        }
        void setSubTreeList(EasyuiTreeNode ParentNode, List<View_QSRole> listAll)
        {
            foreach (JCXX_QSRole child in listAll.Where(t => t.RoleType == ParentNode.id).OrderBy(t => t.RoleCode))
            {
                EasyuiTreeNode childNode = new EasyuiTreeNode() { text = child.RoleName, id = child.ID, parentid = ParentNode.id, children = new List<EasyuiTreeNode>() };
                setSubTreeList(childNode, listAll);
                ParentNode.children.Add(childNode);
            }
        }
        #endregion

        #region 角色用户设置

        /// <summary>
        /// 获取角色用户列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<View_QSRoleUser> GetRoleUserList(Query_QSRoleUser query)
        {
            var list = Respostry_VRU.Select().Where("1=1");
            if (!string.IsNullOrEmpty(query.RoleID))
                list = list.And(t => t.RoleID.Eq(query.RoleID));
            if (!string.IsNullOrEmpty(query.UserID))
                list = list.And(t => t.UserID.Eq(query.UserID));
            if (!string.IsNullOrEmpty(query.DeptID))
                list = list.And(t => t.DeptID.Eq(query.DeptID));
            return list.QueryList();
        }

        /// <summary>
        /// 保存角色用户
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="UserIDs"></param>
        /// <returns></returns>
        public RepModel SaveRoleUser(string RoleID, string UserIDs, string CREATED_MAN)
        {
            try
            {
                if (string.IsNullOrEmpty(RoleID))
                    return RepModel.Error("角色不能为空");
                //删除原有角色用户
                Respostry_RU.Delete().Where(t => t.RoleID.Eq(RoleID)).ExecRemove();
                //新增现有角色用户
                var listNew = new List<JCXX_QSRole_User>();
                foreach (var UserID in UserIDs.Split(','))
                {
                    if (string.IsNullOrEmpty(UserID))
                        continue;
                    listNew.Add(new JCXX_QSRole_User()
                    {
                        ID = Guid.NewGuid().ToString(),
                        RoleID = RoleID,
                        UserID = UserID,
                        CREATED = DateTime.Now,
                        CREATED_MAN = CREATED_MAN
                    });
                }
                return listNew.Count == 0 || Respostry_RU.Insert(listNew).ExecInsertBatch() ? RepModel.Success("操作成功") : RepModel.Error("操作失败");
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
