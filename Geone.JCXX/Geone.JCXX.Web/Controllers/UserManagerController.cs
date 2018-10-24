using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geone.JCXX.BLL;
using Geone.JCXX.Meta;

using Microsoft.AspNetCore.Mvc;
using Geone.Utiliy.Library;

namespace Srv.Backstage.Controllers
{
    public class UserManagerController : Controller
    {
        private IUserBLL bll;

        public UserManagerController(IUserBLL _BLL)
        {
            bll = _BLL;
        }


        public ActionResult UserList()
        {
            return View();
        }

        public ActionResult UserForm()
        {
            return View();
        }

        public ActionResult UserPwd()
        {
            return View();
        }

     
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGrid(Query_User query)
        {
            return Content(JsonHelper.JsonDllSerialize<GridData>(bll.GetGrid(query), JsonDateTimeFormat.DateTime));
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllList(Query_User query)
        {
            var userList = bll.GetList(query);
            return Json(userList);
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
            JCXX_User entity = JsonHelper.JsonDllDeserialize<JCXX_User>(info);
            return Json(bll.Save(entity));
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        public ActionResult PwdReset()
        {
            string info = Request.Form["info"];
            JCXX_User entity = JsonHelper.JsonDllDeserialize<JCXX_User>(info);
            return Json(bll.PwdReset(entity));
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
    }
}