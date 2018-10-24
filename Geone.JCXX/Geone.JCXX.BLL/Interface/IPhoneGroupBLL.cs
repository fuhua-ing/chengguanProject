using Geone.JCXX.Meta;using Geone.Utiliy.Library;

using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.BLL
{
    public interface IPhoneGroupBLL
    {

        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns></returns>
        GridData GetGrid(Query_PhoneGroup query);
        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <returns></returns>
        List<View_PhoneGroup> GetList(Query_PhoneGroup query);
        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        JCXX_PhoneGroup GetByID(string ID);
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RepModel Save(JCXX_PhoneGroup entity);

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        RepModel Del(string ID);

        #region 角色分组设置
        /// <summary>
        /// 获取角色分组列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<View_QSRolePhoneGroup> GetRoleGroupList(Query_PhoneGroup query);
        /// <summary>
        /// 保存角色分组
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="RoleIDs"></param>
        /// <returns></returns>
        RepModel SaveRoleGroup(string GroupID, string RoleIDs, string CREATED_MAN);
        #endregion

    }
}
