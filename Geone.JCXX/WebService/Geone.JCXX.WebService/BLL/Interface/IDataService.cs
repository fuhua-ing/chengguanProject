using Geone.JCXX.WebService.Meta.QueryEntity;
using Geone.Utiliy.Library;

namespace Geone.JCXX.WebService
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
        /// 查询网格动态配置列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RepModel GetGridConfigList(Req_Grid query);
        /// <summary>
        /// 根据网格点位获取到对应的网格权属角色
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RepModel GetQSRoleGridList(Req_Grid query);

        /// <summary>
        /// 根据网格点位获取到对应的网格权属角色
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RepModel GetQSRoleTree(Req_Grid query);

        ///// <summary>
        ///// 根据用户ID获取当前权属信息
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //UnaryResult<List<string>> GetQSRoleListByUserID(string UserID);

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