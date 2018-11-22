using Geone.JCXX.Meta;
using Geone.Utiliy.Library;
using System.Collections.Generic;

namespace Geone.JCXX.BLL
{
    public interface IQSRoleBLL
    {
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        List<JCXX_QSRole> GetAll();

        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns></returns>
        GridData GetGrid(Query_QSRole query);

        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <returns></returns>
        List<View_QSRole> GetList(Query_QSRole query);

        /// <summary>
        /// 获取Esyui树形数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<EasyuiTreeNode> GetTreeList(Query_QSRole query);

        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        JCXX_QSRole GetByID(string ID);

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RepModel Save(JCXX_QSRole entity);

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        RepModel Del(string ID);

        #region 角色用户设置

        /// <summary>
        /// 获取角色用户列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        List<View_QSRoleUser> GetRoleUserList(Query_QSRoleUser query);

        /// <summary>
        /// 保存角色用户
        /// </summary>
        /// <param name="RoleID">角色ID</param>
        /// <param name="UserIDs">用户ID(多个)</param>
        /// <param name="CREATED_MAN">创建人</param>
        /// <returns></returns>
        RepModel SaveRoleUser(string RoleID, string UserIDs, string CREATED_MAN);

        #endregion 角色用户设置
    }
}