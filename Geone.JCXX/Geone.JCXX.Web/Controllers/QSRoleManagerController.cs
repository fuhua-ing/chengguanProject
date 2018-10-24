using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geone.JCXX.BLL;
using Geone.JCXX.Meta;

using Microsoft.AspNetCore.Mvc;
using Geone.Utiliy.Library;

namespace Geone.JCXX.Web.Controllers
{
    public class QSRoleManagerController : Controller
    {
        private IQSRoleBLL bll;

        public QSRoleManagerController(IQSRoleBLL BLL)
        {
            bll = BLL;
        }


        public ActionResult QSRoleList()
        {
            return View();
        }
        public ActionResult RoleUserForm()
        {
            return View();
        }
        public ActionResult QSRoleForm()
        {
            return View();
        }



        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGrid(Query_QSRole query)
        {
            return Content(JsonHelper.JsonDllSerialize<GridData>(bll.GetGrid(query), JsonDateTimeFormat.DateTime));
        }

        public ActionResult GetEasyuiTreeList(String RoleName = null)
        {
            Query_QSRole query = new Query_QSRole() { Like_RoleName = RoleName };
            return Json(bll.GetTreeList(query));
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public ActionResult GetInfo()
        {
            string ID = Request.Form["ID"];
            return Json(bll.GetByID(ID));
        }

        /// <summary>
        /// 保存代码
        /// </summary>
        /// <returns></returns>
        public ActionResult PostInfo()
        {
            string info = Request.Form["info"];
            JCXX_QSRole entity = JsonHelper.JsonDllDeserialize<JCXX_QSRole>(info);
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

        #region 角色用户设置

        public ActionResult GetRoleUserList(Query_QSRoleUser query)
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
        #endregion


    }
}