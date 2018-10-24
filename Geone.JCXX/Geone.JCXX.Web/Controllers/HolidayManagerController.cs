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
    public class HolidayManagerController : Controller
    {
        private IHolidayBLL bll;

        public HolidayManagerController(IHolidayBLL _BLL)
        {
            bll = _BLL;
        }

        public ActionResult HolidayList()
        {
            return View();
        }

        public ActionResult HolidayForm()
        {
            return View();
        }





        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetList(Query_Holiday query)
        {
            return Content(JsonHelper.JsonDllSerializeList<JCXX_Holiday>(bll.GetList(query), JsonDateTimeFormat.Date));
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public ActionResult GetInfo()
        {
            string ID = Request.Form["ID"];
            var entity = bll.GetByID(ID);
            return Json(entity);
        }

        /// <summary>
        /// 保存代码
        /// </summary>
        /// <returns></returns>
        public ActionResult PostInfo()
        {
            string info = Request.Form["info"];
            var entity = JsonHelper.JsonDllDeserialize<JCXX_Holiday>(info);
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