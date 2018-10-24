using Geone.JCXX.Meta;using Geone.Utiliy.Library;

using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.BLL
{
    public interface IMonitorBLL
    {
        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns></returns>
        GridData GetGrid(Query_Monitor query);
        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <returns></returns>
        List<JCXX_Monitor> GetList(Query_Monitor query);
        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        JCXX_Monitor GetByID(string ID);
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RepModel Save(JCXX_Monitor entity);
        
        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        RepModel Del(string ID);

    }
}
