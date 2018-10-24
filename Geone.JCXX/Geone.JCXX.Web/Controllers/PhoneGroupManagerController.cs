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
    public class PhoneGroupManagerController : Controller
    {
        private IPhoneGroupBLL bll;
        private IPhoneGroupItemBLL itemBll;
        public PhoneGroupManagerController(IPhoneGroupBLL _BLL, IPhoneGroupItemBLL _itemBLL)
        {
            bll = _BLL;
            itemBll = _itemBLL;
        }


        public ActionResult PhoneGroupList()
        {
            return View();
        }
        public ActionResult PhoneGroupItemList()
        {
            return View();
        }
        public ActionResult RoleGroupForm()
        {
            return View();
        }
        public ActionResult PhoneGroupForm()
        {
            return View();
        }
        public ActionResult PhoneGroupItemForm()
        {
            return View();
        }



        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGrid(Query_PhoneGroup query)
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
        /// 保存代码
        /// </summary>
        /// <returns></returns>
        public ActionResult PostInfo()
        {
            string info = Request.Form["info"];
            JCXX_PhoneGroup entity = JsonHelper.JsonDllDeserialize<JCXX_PhoneGroup>(info);
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

        public ActionResult GetRoleGroupList(Query_PhoneGroup query)
        {
            return Json(bll.GetRoleGroupList(query));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult PostRoleGroup()
        {
            string GroupID = Request.Form["GroupID"];
            string RoleIDs = Request.Form["RoleIDs"];
            RepModel result = bll.SaveRoleGroup(GroupID, RoleIDs, LoginHelp.GetLoginInfo().Account);
            return Json(result);
        }
        #endregion

        #region 号码设置
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetItemGrid(Query_PhoneGroupItem query)
        {
            return Content(JsonHelper.JsonDllSerialize<GridData>(itemBll.GetGrid(query), JsonDateTimeFormat.DateTime));
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public ActionResult GetItemInfo()
        {
            string ID = Request.Form["ID"];
            return Json(itemBll.GetByID(ID));
        }

        /// <summary>
        /// 保存代码
        /// </summary>
        /// <returns></returns>
        public ActionResult PostItemInfo()
        {
            string info = Request.Form["info"];
            JCXX_PhoneGroupItem entity = JsonHelper.JsonDllDeserialize<JCXX_PhoneGroupItem>(info);
            RepModel result = itemBll.Save(entity);
            return Json(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public ActionResult DelItemInfo()
        {
            string ID = Request.Form["ID"].ToString();
            RepModel result = itemBll.Del(ID);
            return Json(result);
        }
        #endregion
      
    }
}