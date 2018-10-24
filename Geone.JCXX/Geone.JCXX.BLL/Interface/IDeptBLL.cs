using Geone.JCXX.Meta;using Geone.Utiliy.Library;

using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.BLL
{
    public interface IDeptBLL
    {
        
        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns></returns>
        GridData GetGrid(Query_Dept query);
        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <returns></returns>
        List<JCXX_Dept> GetList(Query_Dept query);

        /// <summary>
        /// 获取Easyui树形结构
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<EasyuiTreeNode_Dept> GetTreeList(Query_Dept query);

        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        JCXX_Dept GetByID(string ID);
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RepModel Save(JCXX_Dept entity);
        
        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        RepModel Del(string ID);

    }
}
