using Geone.JCXX.Meta;
using Geone.JCXX.Meta.ExtendEntity;
using Geone.Utiliy.Library;
using System.Collections.Generic;

namespace Geone.JCXX.BLL
{
    public interface ICaseLATJBLL
    {
        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns></returns>
        GridData GetGrid(Query_CaseLATJ query);

        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <returns></returns>
        List<View_CaseLATJ> GetList(Query_CaseLATJ query);

        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        View_CaseLATJ GetByID(string ID);

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RepModel Save(JCXX_CaseLATJ entity);

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        RepModel Del(string ID);
    }
}
