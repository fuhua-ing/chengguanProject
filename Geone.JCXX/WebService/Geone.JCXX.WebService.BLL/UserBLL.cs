using System;
using Geone.JCXX.WebService.Meta.Interface;
using Geone.JCXX.WebService.Meta.QueryEntity;
using Geone.Utiliy.Library;
using Geone.Utiliy.Database;
using Geone.JCXX.Meta;
using Geone.JCXX.WebService.Meta.Response;
using System.Linq;
using System.Collections.Generic;

namespace Geone.JCXX.WebService.BLL
{
    public class UserBLLL : IUserService
    {
        private IDbEntity<JCXX_User> Respostry_User;
        private IDbEntity<View_AppRoleUser> Respostry_VRU;
        private IDbEntity<View_AppRoleMenu> Respostry_VRM;
        private IDbEntity<View_QSRoleUser> Respostry_VQSRU;

        LogWriter log = new LogWriter(new FileLogRecord());

        public UserBLLL(IDbEntity<JCXX_User> _Respostry_User, IDbEntity<View_AppRoleUser> _Respostry_VRU,
            IDbEntity<View_AppRoleMenu> _Respostry_VRM, IDbEntity<View_QSRoleUser> _Respostry_VQSRU)
        {
            Respostry_VRU = _Respostry_VRU;
            Respostry_VRU.SetTable(View_AppRoleUser.GetTbName());

            Respostry_User = _Respostry_User;
            Respostry_User.SetTable(JCXX_User.GetTbName());

            Respostry_VRM = _Respostry_VRM;
            Respostry_VRM.SetTable(View_AppRoleMenu.GetTbName());

            Respostry_VQSRU = _Respostry_VQSRU;
            Respostry_VQSRU.SetTable(View_QSRoleUser.GetTbName());
        }

        /// <summary>
        /// 用户登录 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RepModel Login(Req_UserLogin query)
        {
            try
            {
                var result = new Rtn_UserLogin();
                #region 查找用户
                var userList = Respostry_User.Select().WhereAnd(t => t.Account.Eq(query.Account),
                    t => t.Enabled.Eq(1),
                    t => t.IsDelete.Eq(0)
                    ).QueryList();
                if (userList.Count == 0)
                    return RepModel.Error("用户不存在", 400);
                if (!Md5Encrypt.Encrypt(query.Password, 32).Equals(userList[0].Pwd))
                    return RepModel.Error("密码错误", 400);
                var Userinfo = userList[0];
                result.Userinfo = new Rtn_UserLogin_User()
                {
                    ID = Userinfo.ID,
                    DeptID = Userinfo.DeptID,
                    UserName = Userinfo.UserName,
                    UserCode = Userinfo.UserCode,
                    Gender = Userinfo.Gender,
                    IDNumber = Userinfo.IDNumber,
                    Mobile = Userinfo.Mobile,
                    Email = Userinfo.Email
                };
                #endregion
                #region 根据AppID查找用户角色信息
                var listVRU = Respostry_VRU.Select().WhereAnd(t => t.AppID.Eq(query.AppID),
                    t => t.UserID.Eq(userList[0].ID),
                    t => t.AppEnabled.Eq(1),
                    t => t.RoleEnabled.Eq(1)
                    ).QueryList();
                #endregion
                #region 创建tokne
                var token_user = JsonHelper.JsonDllSerialize<AppUserResult>(new AppUserResult()
                {
                    i = Userinfo.ID,
                    n = Userinfo.UserName,
                    a = Userinfo.Account
                });
                var token_role = JsonHelper.JsonDllSerialize<string[]>(listVRU.Select(t => t.RoleID).ToArray());
                result.Token = Token.DistributeARToken(AppConfig.Init.Key, query.AppID, token_user, token_role);
                #endregion
                return RepModel.Success(result);

            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error();
            }

        }
        /// <summary>
        /// 用户密码修改
        /// </summary>
        /// <param name="query"></param>
        /// <param name="idt"></param>
        /// <returns></returns>
        public RepModel EditPwd(Req_EditPwd query, AppIdentity idt)
        {
            try
            {
                var old = Respostry_User.Select().Where(t => t.ID.Eq(query.UserID)).QueryFirst();
                if (!Md5Encrypt.Encrypt(query.Password, 32).Equals(old.Pwd))
                    return RepModel.Error("原密码错误", 400);
                old.Pwd = Md5Encrypt.Encrypt(query.NewPassword, 32);
                old.UPDATED = DateTime.Now;
                old.UPDATED_MAN = idt.User.a;
                return Respostry_User.UpdateByPKey(old).ExecModify() ? RepModel.Success("更新成功") : RepModel.Error("更新失败");
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error();
            }
        }

        /// <summary>
        /// 用户获取菜单
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RepModel GetMenu(AppIdentity idt)
        {
            try
            {
                var list = new List<View_AppRoleMenu>();
                //用户角色如果为空，则没有对应的权限菜单
                if (idt.RoleIds.Count() > 0)
                    list = Respostry_VRM.Select().WhereAnd(t => t.AppID.Eq(idt.AppId),
                          t => t.RoleID.In(idt.RoleIds),
                  t => t.RoleEnabled.Eq(1),
                  t => t.AppEnabled.Eq(1),
                   t => t.MenuEnabled.Eq(1)
                   ).QueryList();
                var result = list.GroupBy(p => new { p.MenuID, p.MenuParentID, p.MenuName, p.MenuCode, p.MenuIcon, p.MenuUrl })
                   .Select(m => new
                   {
                       ID = m.Key.MenuID,
                       ParentID = m.Key.MenuParentID,
                       MenuName = m.Key.MenuName,
                       MenuCode = m.Key.MenuCode,
                       MenuUrl = m.Key.MenuUrl,
                       Icon = m.Key.MenuIcon
                   });
                return RepModel.Success(result);
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error();
            }
        }

        /// <summary>
        /// 用户获取
        /// </summary>
        /// <param name="idt"></param>
        /// <returns></returns>
        public RepModel GetQsRole(AppIdentity idt)
        {
            try
            {
                var list = Respostry_VQSRU.Select().WhereAnd(t => t.UserID.Eq(idt.User.i),
                  t => t.RoleEnabled.Eq(1),
                  t => t.UserEnabled.Eq(1)
                   ).QueryList(); ;
                var result = list.GroupBy(p => new { p.RoleID, p.RoleName, p.RoleCode, p.RoleType })
                      .Select(m => new
                      {
                          ID = m.Key.RoleID,
                          RoleType = m.Key.RoleType,
                          RoleName = m.Key.RoleName,
                          RoleCode = m.Key.RoleCode
                      });
                return RepModel.Success(result);
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error();
            }
        }


    }
}
