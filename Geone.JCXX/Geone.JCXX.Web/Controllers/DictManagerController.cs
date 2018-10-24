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
    public class DictManagerController : Controller
    {
        private IDictCategoryBLL bll;
        private IDictItemBLL itemBll ;

        public DictManagerController(IDictCategoryBLL _BLL, IDictItemBLL _itemBLL)
        {
            bll = _BLL;
            itemBll = _itemBLL;
        }

        public ActionResult DictCategoryList()
        {
            return View();
        }
        public ActionResult DictItemList()
        {
            return View();
        }
        public ActionResult DictCategoryForm()
        {
            return View();
        }
        public ActionResult DictItemForm()
        {
            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ActionResult GetGrid(Query_DictCategory query)
        {
            return Content(JsonHelper.JsonDllSerialize<GridData>(bll.GetGrid(query), JsonDateTimeFormat.DateTime));
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllList(Query_DictCategory query)
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
            return Json(bll.GetByID(ID));
        }
        /// <summary>
        /// 保存代码
        /// </summary>
        /// <returns></returns>
        public ActionResult PostInfo()
        {
            string info = Request.Form["info"];
            JCXX_DictCategory entity = JsonHelper.JsonDllDeserialize<JCXX_DictCategory>(info);
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


        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetItemGrid(Query_DictItem query)
        {
            return Content(JsonHelper.JsonDllSerialize<GridData>(itemBll.GetGrid(query), JsonDateTimeFormat.DateTime));
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetItemAllList(Query_DictItem query)
        {
            return Json(itemBll.GetList(query));
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
            JCXX_DictItem entity = JsonHelper.JsonDllDeserialize<JCXX_DictItem>(info);
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

        //统一调用
        [HttpGet]
        public ActionResult GetItemListByCategoryCode()
        {
            Query_DictItem query = new Query_DictItem();
            query.AppID=AppConfigurtaionServices.Configuration["AppID"];
            query.CategoryCode= Request.Query["CategoryCode"];
            query.Note = Request.Query["Note"];
            query.Enabled = 1;
            query.sort = "ItemCode";
            List<View_DictItem> list = itemBll.GetExtList(query);
            //list.Add(new View_DictItem { AppID= query.AppID , CategoryCode=query.CategoryCode });
            return Content(JsonHelper.JsonDllSerializeList<View_DictItem>(list, JsonDateTimeFormat.DateTime));
        }

      
    }
}