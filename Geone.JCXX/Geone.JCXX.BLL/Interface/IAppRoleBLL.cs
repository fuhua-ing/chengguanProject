using Geone.JCXX.Meta;
using Geone.Utiliy.Library;
using System.Collections.Generic;

namespace Geone.JCXX.BLL
{
    public interface IAppRoleBLL
    {
        #region 维护角色

        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns></returns>
        GridData GetGrid(Query_AppRole query);

        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <returns></returns>
        List<JCXX_AppRole> GetList(Query_AppRole query);

        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        JCXX_AppRole GetByID(string ID);

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RepModel Save(JCXX_AppRole entity);

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        RepModel Del(string ID);

        #endregion 维护角色

        #region 角色用户设置

        /// <summary>
        /// 获取角色用户列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        List<View_AppRoleUser> GetRoleUserList(Query_RoleUser query);

        /// <summary>
        /// 保存角色用户
        /// </summary>
        /// <param name="RoleID">角色ID</param>
        /// <param name="UserIDs">用户ID(多个)</param>
        /// <param name="CREATED_MAN">创建人</param>
        /// <returns></returns>
        RepModel SaveRoleUser(string RoleID, string UserIDs, string CREATED_MAN);

        #endregion 角色用户设置

        #region 角色菜单设置

        /// <summary>
        /// 获取角色菜单的Easyui树形结构
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<EasyuiTreeNode> GetRoleMenuTreeGrid(Query_RoleMenu query);

        /// <summary>
        /// 获取角色菜单的列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<View_AppRoleMenu> GetRoleMenuList(Query_RoleMenu query);

        /// <summary>
        /// 保存角色菜单
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="MenuIDs"></param>
        /// <param name="CREATED_MAN"></param>
        /// <returns></returns>
        RepModel SaveRoleMenu(string RoleID, string MenuIDs, string CREATED_MAN);

        #endregion 角色菜单设置
    }
}