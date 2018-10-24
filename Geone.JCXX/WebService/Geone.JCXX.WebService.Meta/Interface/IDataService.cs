using Geone.JCXX.WebService.Meta.QueryEntity;
using Geone.Utiliy.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.WebService.Meta.Interface
{
    public interface IDataService
    {
        /// <summary>
        /// 查询字典明细列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RepModel GetDictItemList(Req_DictItem query);

        /// <summary>
        /// 查询权属角色列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RepModel GetQSRoleList(Req_QSRole query);
        
        /// <summary>
        /// 查询网格列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RepModel GetGridList(Req_Grid query);

        /// <summary>
        /// 查询部门列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RepModel GetDeptList(Req_Dept query);

        /// <summary>
        /// 查询立案条件列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RepModel GetCaseLATJList(Req_CaseLATJ query);
    }
}
