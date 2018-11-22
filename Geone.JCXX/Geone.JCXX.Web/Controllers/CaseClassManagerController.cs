using Geone.JCXX.BLL;
using Geone.JCXX.Meta;
using Geone.Utiliy.Library;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Geone.JCXX.Web.Controllers
{
    public class CaseClassManagerController : Controller
    {
        private ICaseClassBLL bll;
        private IHostingEnvironment hostingEnv;

        public CaseClassManagerController(ICaseClassBLL _BLL, IHostingEnvironment _hostingEnv)
        {
            bll = _BLL;
            hostingEnv = _hostingEnv;
        }

        public ActionResult CaseClassList()
        {
            return View();
        }

        public ActionResult CaseClassForm()
        {
            return View();
        }

        public ActionResult RoleCaseForm()
        {
            return View();
        }

        public ActionResult CaseClassImport()
        {
            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGrid(Query_CaseClass query)
        {
            return Content(JsonHelper.JsonDllSerialize<GridData>(bll.GetGrid(query), JsonDateTimeFormat.DateTime));
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllList(Query_CaseClass query)
        {
            return Content(JsonHelper.JsonDllSerializeList<View_CaseClass>(bll.GetList(query), JsonDateTimeFormat.Date));
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
            var entity = JsonHelper.JsonDllDeserialize<JCXX_CaseClass>(info);
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

        #region 角色案件设置

        public ActionResult GetRoleCaseList(Query_CaseClass query)
        {
            return Json(bll.GetRoleCaseList(query));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult PostRoleCase()
        {
            string CaseClassID = Request.Form["CaseClassID"];
            string RoleIDs = Request.Form["RoleIDs"];
            RepModel result = bll.SaveRoleCase(CaseClassID, RoleIDs, LoginHelp.GetLoginInfo().Account);
            return Json(result);
        }

        #endregion 角色案件设置

        /// <summary>
        /// Json文件上传导入
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="ProID"></param>
        /// <param name="FileType"></param>
        /// <returns></returns>
        public ActionResult UploadFile(IFormFile uploadfile)
        {
            if (uploadfile == null || !uploadfile.FileName.Contains(".json"))
                return Json(RepModel.Error("请上传正确的json格式文件"));

            string sWebRootFolder = hostingEnv.WebRootPath;
            string sFileName = $"{Guid.NewGuid()}.json";
            var tmpfile = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            try
            {
                using (FileStream fs = new FileStream(tmpfile.ToString(), FileMode.Create))
                {
                    uploadfile.CopyTo(fs);
                    fs.Flush();
                }
                string json = string.Empty;
                using (FileStream fs = new FileStream(tmpfile.ToString(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        json = sr.ReadToEnd().ToString();
                    }
                }
                var listGrid = JsonHelper.JsonDllDeserializeList<JCXX_CaseClass>(json);
                return Json(bll.Import(listGrid));
            }
            catch (Exception ex)
            {
                return Json(RepModel.Error(ex.Message));
            }
            finally
            {
                tmpfile.Delete();
            }
        }

        public ActionResult DownloadFile(Query_CaseClass query)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(JsonHelper.JsonDllSerializeList<View_CaseClass>(bll.GetList(query), JsonDateTimeFormat.DateTime));
            return File(buffer, "text/plain", "file.txt");
        }

        /// <summary>
        /// 示例文件下载
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadDemo()
        {
            var FilePath = Request.Query["FilePath"];
            var tmpfile = new FileInfo(Path.Combine(hostingEnv.WebRootPath, FilePath));
            return File(tmpfile.OpenRead(), "application/json", tmpfile.Name);
        }
    }
}