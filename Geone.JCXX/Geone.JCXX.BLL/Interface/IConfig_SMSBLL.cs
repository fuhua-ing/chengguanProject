﻿using Geone.JCXX.Meta;
using Geone.Utiliy.Library;
using System.Collections.Generic;

namespace Geone.JCXX.BLL
{
    public interface IConfig_SMSBLL
    {
        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns></returns>
        GridData GetGrid(Query_Config_SMS query);

        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <returns></returns>
        List<JCXX_Config_SMS> GetList(Query_Config_SMS query);

        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        JCXX_Config_SMS GetByID(string ID);

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RepModel Save(JCXX_Config_SMS entity);

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        RepModel Del(string ID);
    }
}
