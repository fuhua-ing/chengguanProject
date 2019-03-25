using Geone.JCXX.Meta;
using Geone.JCXX.WebService.Meta.QueryEntity;
using Geone.JCXX.WebService.Meta.Response;
using Geone.Utiliy.Build;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geone.JCXX.WebService
{
    public class UserBLLL : IUserService
    {
        private IDbEntity<JCXX_User> Respostry_User;
        private IDbEntity<View_AppRoleUser> Respostry_VRU;
        private IDbEntity<View_AppRoleMenu> Respostry_VRM;
        private IDbEntity<View_QSRoleUser> Respostry_VQSRU;
        private IDbEntity<View_AppRoleUser> Respostry_VARU;
        private ILogWriter log;
        private string key;

        public UserBLLL(IDbEntity<JCXX_User> _User,
            IDbEntity<View_AppRoleUser> _VRU,
            IDbEntity<View_AppRoleMenu> _VRM,
            IDbEntity<View_QSRoleUser> _VQSRU,
            IDbEntity<View_AppRoleUser> _VARU,
            ILogWriter logWriter)
        {
            Respostry_User = _User;
            Respostry_User.SetTable(JCXX_User.GetTbName());

            Respostry_VRU = _VRU;
            Respostry_VRU.SetTable(View_AppRoleUser.GetTbName());

            Respostry_VRM = _VRM;
            Respostry_VRM.SetTable(View_AppRoleMenu.GetTbName());

            Respostry_VQSRU = _VQSRU;
            Respostry_VQSRU.SetTable(View_QSRoleUser.GetTbName());

            Respostry_VARU = _VARU;
            Respostry_VARU.SetTable(View_AppRoleUser.GetTbName());

            log = logWriter;
            key = "geone123";
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

                #endregion 查找用户

                #region 根据AppID查找用户角色信息

                var listVRU = Respostry_VRU.Select().WhereAnd(t => t.AppID.Eq(query.AppId),
                    t => t.UserID.Eq(userList[0].ID),
                    t => t.AppEnabled.Eq(1),
                    t => t.RoleEnabled.Eq(1)
                    ).QueryList();

                #endregion 根据AppID查找用户角色信息

                #region 根据用户ID获取权属角色

                List<Geone.JCXX.WebService.Meta.Response.JCXX_QSRole> jqsList = new List<Geone.JCXX.WebService.Meta.Response.JCXX_QSRole>();

                var rq = Respostry_VQSRU.Select().Where("1=1");
                rq.And(t => t.UserID.Eq(Userinfo.ID));
                var qlist = rq.QueryList();
                foreach (var item in qlist)
                {
                    Geone.JCXX.WebService.Meta.Response.JCXX_QSRole model = new Geone.JCXX.WebService.Meta.Response.JCXX_QSRole();
                    model.ID = item.ID;
                    model.UserEnabled = item.UserEnabled;
                    model.UserID = item.UserID;
                    model.UserName = item.UserName;
                    model.UserCode = item.UserCode;
                    model.Account = item.Account;
                    model.Pwd = item.Pwd;
                    model.Gender = item.Gender;
                    model.IDNumber = item.IDNumber;
                    model.Mobile = item.Mobile;
                    model.Email = item.Email;
                    model.RoleEnabled = item.RoleEnabled;
                    model.RoleID = item.RoleID;
                    model.RoleCode = item.RoleCode;
                    model.RoleName = item.RoleName;
                    model.RoleType = item.RoleType;
                    model.DeptEnabled = item.DeptEnabled;
                    model.DeptID = item.DeptID;
                    model.DeptCode = item.DeptCode;
                    model.DeptName = item.DeptName;
                    model.DeptShortName = item.DeptShortName;
                    model.DeptType = item.DeptType;
                    model.DeptTypeName = item.DeptTypeName;
                    jqsList.Add(model);
                }
                result.JCXX_QSRole = jqsList;

                #endregion 根据用户ID获取权属角色

                #region 创建tokne

                var token_user = JsonHelper.JsonDllSerialize(new AppUserResult()
                {
                    i = Userinfo.ID,
                    n = Userinfo.UserName,
                    a = Userinfo.Account
                });
                var token_role = JsonHelper.JsonDllSerialize<string[]>(listVRU.Select(t => t.RoleID).ToArray());
                result.Token = Token.DistributeARToken(key, query.AppId, token_user, token_role);

                #endregion 创建tokne

                return RepModel.Success(result);
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return RepModel.Error();
            }
        }

        /// <summary>
        /// 用户刷新token
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public RepModel RefreshToken(Req_UserLogin query)
        {
            try
            {
                #region 刷新tokne

                var result = new Rtn_UserLogin();

                bool tokenValidate = Token.VerifyRefreshToken(key, query.AppId, query.AccessToken, query.RefreshToken);

                if (tokenValidate)
                {
                    dynamic atoken = Token.ObtainAccessToken(key, query.AccessToken);

                    var token = Token.DistributeARToken(key, query.AppId, atoken.user.ToString(), atoken.role.ToString(), 6);

                    return RepModel.Success(token);
                }
                else
                {
                    return RepModel.Error(log.WriteWarn("令牌刷新失败。", 401.2));
                }

                #endregion 刷新tokne
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
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
                log.WriteException(ex);
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
                var result = list.OrderBy(p => p.MenuCode).GroupBy(p => new { p.MenuID, p.MenuParentID, p.MenuName, p.MenuCode, p.MenuIcon, p.MenuUrl })
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
                log.WriteException(ex);
                return RepModel.Error();
            }
        }

        /// <summary>
        /// 用户获取角色
        /// </summary>
        /// <param name="idt"></param>
        /// <returns></returns>
        public RepModel GetUserRole(AppIdentity idt)
        {
            try
            {
                var list = Respostry_VARU.Select().WhereAnd(t => t.UserID.Eq(idt.User.i),
                  t => t.RoleEnabled.Eq(1),
                  t => t.UserEnabled.Eq(1)
                   ).QueryList(); ;
                var result = list.GroupBy(p => new { p.RoleID, p.RoleName, p.RoleCode })
                      .Select(m => new
                      {
                          ID = m.Key.RoleID,
                          RoleName = m.Key.RoleName,
                          RoleCode = m.Key.RoleCode
                      });
                return RepModel.Success(result);
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
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
                log.WriteException(ex);
                return RepModel.Error();
            }
        }
    }
}