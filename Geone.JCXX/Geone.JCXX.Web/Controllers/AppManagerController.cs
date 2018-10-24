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
    public class AppManagerController : Controller
    {
        private IAppBLL bll;

        public AppManagerController(IAppBLL _BLL)
        {
            bll = _BLL;
        }

        public ActionResult AppList()
        {
            return View();
        }

        public ActionResult AppForm()
        {
            return View();
        }




        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGrid(Query_App query)
        {
            return Content(JsonHelper.JsonDllSerialize<GridData>(bll.GetGrid(query), JsonDateTimeFormat.DateTime));
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllList(Query_App query)
        {
            return Json(bll.GetList(query));
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public ActionResult GetInfo()
        {
            string ID = Request.Form["ID"];
            JCXX_App entity = bll.GetByID(ID);
            return Json(entity);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult PostInfo()
        {
            string info = Request.Form["info"];
            JCXX_App entity = JsonHelper.JsonDllDeserialize<JCXX_App>(info);
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