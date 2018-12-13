using MagicOnion;

namespace Geone.JCXX.WebService
{
    public interface IUserRpc : IService<IUserRpc>
    {
        /// <summary>
        /// 根据用户ID获取用户对应的权属角色
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        UnaryResult<string> GetQSRoleList(string userID);

        /// <summary>
        /// 根据用户编号或者角色编号获取部门名称
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        UnaryResult<string> GetDeptName(string userOrRoleID);
    }
}