using Geone.JCXX.BLL;
using Geone.JCXX.Meta;
using Geone.Utiliy.Library;
using Microsoft.AspNetCore.Mvc;

namespace Geone.JCXX.Web.Controllers
{
    public class AppRoleManagerController : Controller
    {
        private IAppRoleBLL bll;

        public AppRoleManagerController(IAppRoleBLL _BLL)
        {
            bll = _BLL;
        }

        public ActionResult RoleList()
        {
            return View();
        }

        public ActionResult RoleForm()
        {
            return View();
        }

        public ActionResult RoleUserForm()
        {
            return View();
        }

        public ActionResult RoleRightForm()
        {
            return View();
        }

        public ActionResult MenuList()
        {
            return View();
        }

        #region 维护角色

        public ActionResult GetGrid(Query_AppRole query)
        {
            return Json(bll.GetGrid(query));
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllList(Query_AppRole query)
        {
            return Json(bll.GetList(query));
        }

        /// <summary>
        /// 获取单元素
        /// </summary>
        /// <returns></returns>
        public ActionResult GetInfo()
        {
            string ID = Request.Form["ID"];
            return Json(bll.GetByID(ID));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult PostInfo()
        {
            string info = Request.Form["info"];
            JCXX_AppRole entity = JsonHelper.JsonDllDeserialize<JCXX_AppRole>(info);
            RepModel result = bll.Save(entity);
            return Json(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public ActionResult DelInfo()
        {
            string ID = Request.Form["ID"].ToString();
            RepModel result = bll.Del(ID);
            return Json(result);
        }

        #endregion 维护角色

        #region 角色用户设置

        public ActionResult GetRoleUserList(Query_RoleUser query)
        {
            return Json(bll.GetRoleUserList(query));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult PostRoleUser()
        {
            string RoleID = Request.Form["RoleID"];
            string UserIDs = Request.Form["UserIDs"];
            RepModel result = bll.SaveRoleUser(RoleID, UserIDs, LoginHelp.GetLoginInfo().Account);
            return Json(result);
        }

        #endregion 角色用户设置

        #region 角色菜单设置

        public ActionResult GetRoleMenuTreeGrid(Query_RoleMenu query)
        {
            return Json(bll.GetRoleMenuTreeGrid(query));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult PostRoleMenu()
        {
            string RoleID = Request.Form["RoleID"];
            string MenuIDs = Request.Form["MenuIDs"];
            RepModel result = bll.SaveRoleMenu(RoleID, MenuIDs, LoginHelp.GetLoginInfo().Account);
            return Json(result);
        }

        #endregion 角色菜单设置
    }
}