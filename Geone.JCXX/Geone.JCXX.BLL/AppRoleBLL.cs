using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geone.JCXX.BLL
{
    public class AppRoleBLL : IAppRoleBLL
    {
        private IDbEntity<JCXX_AppRole> Respostry;
        private IDbEntity<JCXX_AppRole_User> Respostry_RU;
        private IDbEntity<JCXX_AppRole_Menu> Respostry_RM;
        private IDbEntity<View_AppRoleUser> Respostry_VRU;
        private IDbEntity<View_AppRoleMenu> Respostry_VRM;
        private IAppBLL appBLL;
        private IAppMenuBLL appMenuBLL;
        private ILogWriter log;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public AppRoleBLL(IDbEntity<JCXX_AppRole> _t, IDbEntity<JCXX_App> _tApp,
            IDbEntity<JCXX_AppMenu> _tAppMenu, IDbEntity<JCXX_AppRole_User> _tRU, IDbEntity<JCXX_AppRole_Menu> _tRM,
            IDbEntity<View_AppRoleUser> _tVRU, IDbEntity<View_AppRoleMenu> _tVRM,
            IAppBLL _appBLL, IAppMenuBLL _appMenuBLL, ILogWriter logWriter)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_AppRole");

            Respostry_RU = _tRU;
            Respostry_RU.SetTable("JCXX_AppRole_User");

            Respostry_RM = _tRM;
            Respostry_RM.SetTable("JCXX_AppRole_Menu");

            Respostry_VRU = _tVRU;
            Respostry_VRU.SetTable("View_AppRoleUser");

            Respostry_VRM = _tVRM;
            Respostry_VRM.SetTable("View_AppRoleMenu");

            appBLL = _appBLL;
            appMenuBLL = _appMenuBLL;
            log = logWriter;
        }

        #region 维护角色

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_AppRole query)
        {
            var list = Respostry.Select();
            SetQuery(list, query);
            if (string.IsNullOrEmpty(query.sort))
                query.sort = "CREATED";
            var result = list.QueryPage((int)query.page, (int)query.rows, query.sort);
            return new GridData() { rows = GetExtList(result.Rows), total = result.Total };
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<JCXX_AppRole> GetList(Query_AppRole query)
        {
            try
            {
                var list = Respostry.Select();
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
                log.WriteException(ex);
                return new List<JCXX_AppRole>();
            }
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<JCXX_AppRoleExtend> GetExtList(List<JCXX_AppRole> list)
        {
            try
            {
                var extendList = new List<JCXX_AppRoleExtend>();
                if (list != null && list.Count > 0)
                {
                    //获取所有应用
                    List<JCXX_App> listApp = appBLL.GetAll();
                    //获取所有角色用户
                    var listRoleUser = GetRoleUserList(new Query_RoleUser());

                    foreach (var itemModel in list)
                    {
                        var extendModel = new JCXX_AppRoleExtend();
                        var extendType = extendModel.GetType();
                        var propertysList = extendType.GetProperties();
                        var type = itemModel.GetType();
                        var propertysListAgain = type.GetProperties();
                        foreach (var propertModel in propertysList)
                        {
                            var propertysModel = propertysListAgain.Where(p => p.Name == propertModel.Name).FirstOrDefault();
                            if (propertysModel != null)
                            {
                                propertModel.SetValue(extendModel, propertysModel.GetValue(itemModel));
                            }
                        }
                        //查找应用名
                        if (!string.IsNullOrEmpty(itemModel.AppID))
                        {
                            var thisModel = listApp.Where(d => d.ID == itemModel.AppID).FirstOrDefault();
                            extendModel.AppName = thisModel == null ? string.Empty : thisModel.AppName;
                        }
                        //查找用户数量
                        extendModel.UserCount = listRoleUser.Where(t => t.RoleID == itemModel.ID).Count();
                        extendList.Add(extendModel);
                    }
                }
                return extendList;
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new List<JCXX_AppRoleExtend>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<JCXX_AppRole> list, Query_AppRole query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.AppID))
                list = list.And(t => t.AppID.Eq(query.AppID));
            if (!string.IsNullOrEmpty(query.RoleCode))
                list = list.And(t => t.RoleCode.Eq(query.RoleCode));
            if (!string.IsNullOrEmpty(query.Like_RoleCode))
                list = list.And(t => t.RoleCode.Like("%" + query.Like_RoleCode + "%"));
            if (!string.IsNullOrEmpty(query.Like_RoleName))
                list = list.And(t => t.RoleName.Like("%" + query.Like_RoleName + "%"));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));
        }

        public JCXX_AppRole GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_AppRole();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new JCXX_AppRole();
            }
        }

        public RepModel Save(JCXX_AppRole entity)
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
                var oldEntity = GetByID(ID);
                oldEntity.IsDelete = 1;
                oldEntity.UPDATED = DateTime.Now;
                oldEntity.UPDATED_MAN = oldEntity.UPDATED_MAN;
                return Respostry.UpdateByPKey(oldEntity).ExecModify() ? RepModel.Success("删除成功") : RepModel.Error("删除失败");
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return RepModel.Error(ex.Message);
            }
        }

        #endregion 维护角色

        #region 角色用户设置

        /// <summary>
        /// 获取角色用户列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<View_AppRoleUser> GetRoleUserList(Query_RoleUser query)
        {
            var list = Respostry_VRU.Select().Where("1=1");
            if (!string.IsNullOrEmpty(query.AppID))
                list = list.And(t => t.AppID.Eq(query.AppID));
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
                var listNew = new List<JCXX_AppRole_User>();
                foreach (var UserID in UserIDs.Split(','))
                {
                    if (string.IsNullOrEmpty(UserID))
                        continue;
                    listNew.Add(new JCXX_AppRole_User()
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
                log.WriteException(ex);
                return RepModel.Error(ex.Message);
            }
        }

        #endregion 角色用户设置

        #region 角色菜单设置

        /// <summary>
        /// 获取角色菜单的Easyui树形结构
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<EasyuiTreeNode> GetRoleMenuTreeGrid(Query_RoleMenu query)
        {
            var listResult = new List<EasyuiTreeNode>();

            //获取所有菜单
            var role = GetByID(query.RoleID);
            var listAllMenu = appMenuBLL.GetList(new Query_AppMenu() { AppID = role.AppID });

            //获取所有角色菜单
            var listAllRoleMenu = GetRoleMenuList(query);

            //获取父级菜单
            var listParent = listAllMenu.Where(t => string.IsNullOrEmpty(t.ParentID)).OrderBy(t => t.MenuCode).ToList();
            foreach (var parent in listParent)
            {
                EasyuiTreeNode parentNode = new EasyuiTreeNode()
                {
                    text = parent.MenuName,
                    id = parent.ID,
                    parentid = null,
                    ischecked = listAllRoleMenu.Where(t => t.MenuID == parent.ID).Count() > 0 ? 1 : 0,
                    children = new List<EasyuiTreeNode>()
                };
                setRoleMenuSubTree(parentNode, listAllMenu, listAllRoleMenu);
                listResult.Add(parentNode);
            }
            return listResult;
        }

        private void setRoleMenuSubTree(EasyuiTreeNode ParentNode, List<JCXX_AppMenu> listAllMenu, List<View_AppRoleMenu> listAllRoleMenu)
        {
            foreach (var child in listAllMenu.Where(t => t.ParentID == ParentNode.id).OrderBy(t => t.MenuCode))
            {
                EasyuiTreeNode childNode = new EasyuiTreeNode()
                {
                    text = child.MenuName,
                    id = child.ID,
                    parentid = ParentNode.id,
                    ischecked = listAllRoleMenu.Where(t => t.MenuID == child.ID).Count() > 0 ? 1 : 0,
                    children = new List<EasyuiTreeNode>()
                };
                setRoleMenuSubTree(childNode, listAllMenu, listAllRoleMenu);
                ParentNode.children.Add(childNode);
            }
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<View_AppRoleMenu> GetRoleMenuList(Query_RoleMenu query)
        {
            try
            {
                var list = Respostry_VRM.Select();
                if (!string.IsNullOrEmpty(query.RoleID))
                    list = list.Where(t => t.RoleID.Eq(query.RoleID));
                return list.QueryList();
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new List<View_AppRoleMenu>();
            }
        }

        /// <summary>
        /// 保存角色菜单
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="MenuIDs"></param>
        /// <returns></returns>
        public RepModel SaveRoleMenu(string RoleID, string MenuIDs, string CREATED_MAN)
        {
            try
            {
                if (string.IsNullOrEmpty(RoleID))
                    return RepModel.Error("角色不能为空");
                //删除原有角色菜单
                Respostry_RM.Delete().Where(t => t.RoleID.Eq(RoleID)).ExecRemoveBatch();
                //新增现有角色菜单
                var listNew = new List<JCXX_AppRole_Menu>();
                foreach (var MenuID in MenuIDs.Split(','))
                {
                    if (string.IsNullOrEmpty(MenuID))
                        continue;
                    listNew.Add(new JCXX_AppRole_Menu()
                    {
                        ID = Guid.NewGuid().ToString(),
                        RoleID = RoleID,
                        MenuID = MenuID,
                        CREATED = DateTime.Now,
                        CREATED_MAN = CREATED_MAN
                    });
                }
                return listNew.Count == 0 || Respostry_RM.Insert(listNew).ExecInsertBatch() ? RepModel.Success("操作成功") : RepModel.Error("操作失败");
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return RepModel.Error(ex.Message);
            }
        }

        #endregion 角色菜单设置
    }
}