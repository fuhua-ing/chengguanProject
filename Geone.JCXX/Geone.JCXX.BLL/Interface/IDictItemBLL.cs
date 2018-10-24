using Geone.JCXX.Meta;using Geone.Utiliy.Library;

using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.BLL
{
    public interface IDictItemBLL
    {
        
        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns></returns>
        GridData GetGrid(Query_DictItem query);
        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <returns></returns>
        List<JCXX_DictItem> GetList(Query_DictItem query);
        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <returns></returns>
        List<View_DictItem> GetExtList(Query_DictItem query);
        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        JCXX_DictItem GetByID(string ID);
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RepModel Save(JCXX_DictItem entity);
        
        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        RepModel Del(string ID);

    }
}
