using Geone.JCXX.Meta;
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

        /// <summary>
        /// 获取Easyui树形结构
        /// </summary>
        /// <param name="GridID"></param>
        /// <returns></returns>
        List<EasyuiTreeNode_GridQSRoleTree> GetTreeList(string GridID);

        /// <summary>
        /// 根据网格ID和角色ID更新父级角色ID
        /// </summary>
        /// <param name="GridID">网格ID</param>
        /// <param name="RoleID">角色ID</param>
        /// <param name="RoleParentID">父级角色ID</param>
        /// <param name=""></param>
        /// <returns></returns>
        RepModel UpdateTreeByID(string GridID, string RoleID, string RoleParentID);

        #region 动态网格参数配置

        /// <summary>
        /// 根据GridID获取单条数据
        /// </summary>
        /// <param name="GridID">网格ID</param>
        /// <returns></returns>
        JCXX_Grid_Config GetConfig(string GridID);

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RepModel SaveConfig(JCXX_Grid_Config entity);

        #endregion
    }
}