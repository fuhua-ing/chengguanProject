using Geone.JCXX.Meta;using Geone.Utiliy.Library;

using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.BLL
{
    public interface ICaseClassBLL
    {
        
        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns></returns>
        GridData GetGrid(Query_CaseClass query);
        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <returns></returns>
        List<View_CaseClass> GetList(Query_CaseClass query);
        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        JCXX_CaseClass GetByID(string ID);
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RepModel Save(JCXX_CaseClass entity);
        
        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        RepModel Del(string ID);

        /// <summary>
        /// 获取角色案件列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<View_QSRoleCaseClass> GetRoleCaseList(Query_CaseClass query);
        /// <summary>
        ///  保存角色案件
        /// </summary>
        /// <param name="CaseClassID"></param>
        /// <param name="RoleIDs"></param>
        /// <param name="CREATED_MAN"></param>
        /// <returns></returns>
        RepModel SaveRoleCase(string CaseClassID, string RoleIDs, string CREATED_MAN);
        /// <summary>
        /// 导入角色案件列表
        /// </summary>
        /// <param name="list"></param>
        RepModel Import(List<JCXX_CaseClass> list);
    }
}
