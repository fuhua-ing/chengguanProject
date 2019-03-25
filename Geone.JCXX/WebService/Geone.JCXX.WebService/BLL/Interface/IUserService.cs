using Geone.JCXX.WebService.Meta.QueryEntity;
using Geone.Utiliy.Build;
using Geone.Utiliy.Library;

namespace Geone.JCXX.WebService
{
    public interface IUserService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RepModel Login(Req_UserLogin query);

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RepModel RefreshToken(Req_UserLogin query);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RepModel EditPwd(Req_EditPwd query, AppIdentity currentUser);

        /// <summary>
        /// 获取应用菜单
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        RepModel GetMenu(AppIdentity query);

        /// <summary>
        /// 获取用户角色
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        RepModel GetUserRole(AppIdentity query);

        /// <summary>
        /// 获取用户权属角色
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        RepModel GetQsRole(AppIdentity query);
    }
}