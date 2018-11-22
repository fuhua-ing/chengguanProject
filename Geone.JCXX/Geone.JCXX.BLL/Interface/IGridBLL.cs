﻿using Geone.JCXX.Meta;
using Geone.Utiliy.Library;
using System.Collections.Generic;

namespace Geone.JCXX.BLL
{
    public interface IGridBLL
    {
        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns></returns>
        GridData GetGrid(Query_Grid query);

        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <returns></returns>
        List<View_Grid> GetList(Query_Grid query);

        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        View_Grid GetByID(string ID);

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RepModel Save(JCXX_Grid entity);

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        RepModel Del(string ID);

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="list"></param>
        /// <param name="GridType"></param>
        /// <returns></returns>

        RepModel Import(List<JCXX_Grid> list, string GridType);

        #region 权属角色网格设置

        /// <summary>
        ///获取权属角色网格列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<View_QSRoleGrid> GetRoleGridList(Query_Grid query);

        /// <summary>
        /// 保存角色网格
        /// </summary>
        /// <param name="GridID"></param>
        /// <param name="RoleIDs"></param>
        /// <param name="CREATED_MAN"></param>
        /// <returns></returns>
        RepModel SaveRoleGrid(string GridID, string RoleIDs, string CREATED_MAN);

        #endregion 权属角色网格设置
    }
}