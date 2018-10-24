using Geone.JCXX.Meta;using Geone.Utiliy.Library;

using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.BLL
{
    public interface IAppMenuBLL
    {
        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns></returns>
        GridData GetGrid(Query_AppMenu query);
        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <returns></returns>
        List<JCXX_AppMenu> GetList(Query_AppMenu query);

        /// <summary>
        /// 获取Easyui树形数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<EasyuiTreeNode> GetTreeList(Query_AppMenu query);

        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        JCXX_AppMenu GetByID(string ID);
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RepModel Save(JCXX_AppMenu entity);

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        RepModel Del(string ID);




    }
}
