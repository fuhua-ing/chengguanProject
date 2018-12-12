using Geone.JCXX.BLL;
using Geone.JCXX.Meta;
using Geone.Utiliy.Library;
using Microsoft.AspNetCore.Mvc;

namespace Geone.JCXX.Web.Controllers
{
    public class Config_SMSManagerController : Controller
    {
        private IConfig_SMSBLL bll;

        public Config_SMSManagerController(IConfig_SMSBLL _BLL)
        {
            bll = _BLL;
        }

        public ActionResult Config_SMSList()
        {
            return View();
        }

        public ActionResult Config_SMSForm()
        {
            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="query">筛选条件</param>
        /// <returns></returns>
        public ActionResult GetGrid(Query_Config_SMS query)
        {
            return Content(JsonHelper.JsonDllSerialize<GridData>(bll.GetGrid(query), JsonDateTimeFormat.DateTime));
        }

        /// <summary>
        /// 根据ID，获取记录数据
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
            JCXX_Config_SMS entity = JsonHelper.JsonDllDeserialize<JCXX_Config_SMS>(info);
            if (string.IsNullOrEmpty(entity.ID))
            {
                entity.CREATED_MAN = LoginHelp.GetLoginInfo().Account;
            }
            else
            {
                entity.UPDATED_MAN = LoginHelp.GetLoginInfo().Account;
            }

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
