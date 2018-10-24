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
    public class AppMenuManagerController : Controller
    {
        private IAppMenuBLL bll;
        public AppMenuManagerController(IAppMenuBLL _BLL)
        {
            bll = _BLL;
        }

        public ActionResult AppMenuList()
        {
            return View();
        }

        public ActionResult AppMenuForm()
        {
            return View();
        }


        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGrid(Query_AppMenu query)
        {
            return Content(JsonHelper.JsonDllSerialize<GridData>(bll.GetGrid(query), JsonDateTimeFormat.DateTime));
        }

        /// <summary>
        /// Easyui树形结构
        /// </summary>
        /// <returns></returns>
        public ActionResult GetEasyuiTreeList(Query_AppMenu query)
        {
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
            JCXX_AppMenu entity = JsonHelper.JsonDllDeserialize<JCXX_AppMenu>(info);
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


       
    }
}