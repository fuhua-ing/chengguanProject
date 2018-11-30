using Geone.JCXX.BLL;
using Geone.JCXX.Meta;
using Geone.Utiliy.Library;
using Microsoft.AspNetCore.Mvc;

namespace Geone.JCXX.Web.Controllers
{
    public class CaseLATJManagerController : Controller
    {
        private ICaseLATJBLL bll;

        public CaseLATJManagerController(ICaseLATJBLL _BLL)
        {
            bll = _BLL;
        }

        public ActionResult CaseLATJList()
        {
            return View();
        }

        public ActionResult CaseLATJForm()
        {
            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGrid(Query_CaseLATJ query)
        {
            return Content(JsonHelper.JsonDllSerialize<GridData>(bll.GetGrid(query), JsonDateTimeFormat.DateTime));
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
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult PostInfo()
        {
            string info = Request.Form["info"];
            JCXX_CaseLATJ entity = JsonHelper.JsonDllDeserialize<JCXX_CaseLATJ>(info);
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
