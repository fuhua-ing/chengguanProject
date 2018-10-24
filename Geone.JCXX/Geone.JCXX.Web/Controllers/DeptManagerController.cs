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
    public class DeptManagerController : Controller
    {
        private IDeptBLL bll;
        public DeptManagerController(IDeptBLL deptBLL)
        {
            bll = deptBLL;
        }

        public ActionResult DeptList()
        {
            return View();
        }
        public ActionResult DeptForm()
        {
            return View();
        }



        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGrid(Query_Dept query)
        {
            return Content(JsonHelper.JsonDllSerialize<GridData>(bll.GetGrid(query), JsonDateTimeFormat.DateTime));
        }

        public ActionResult GetEasyuiTreeList(Query_Dept query)
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
            JCXX_Dept entity = JsonHelper.JsonDllDeserialize<JCXX_Dept>(info);
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
        //统一调用
        [HttpGet]
        public ActionResult GetItemList()
        {

            Query_Dept query = new Query_Dept();
            List<JCXX_Dept> list = bll.GetList(query);
            return Content(JsonHelper.JsonDllSerializeList<JCXX_Dept>(list, JsonDateTimeFormat.DateTime));
        }

    }
}