using Geone.JCXX.BLL;
using Geone.JCXX.Meta;
using Geone.Utiliy.Library;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;

namespace Geone.JCXX.Web.Controllers
{
    public class GridManagerController : Controller
    {
        private IGridBLL bll;
        private IHostingEnvironment hostingEnv;

        public GridManagerController(IGridBLL _BLL, IHostingEnvironment _hostingEnv)
        {
            bll = _BLL;
            hostingEnv = _hostingEnv;
        }

        public ActionResult GridList()
        {
            return View();
        }

        public ActionResult GridForm()
        {
            return View();
        }
        public ActionResult GridSetting()
        {
            return View();
        }


        public ActionResult GridImport()
        {
            return View();
        }

        public ActionResult RoleGridForm()
        {
            return View();
        }

        public ActionResult OwnerShipGuidForm()
        {
            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGrid(Query_Grid query)
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
            JCXX_Grid entity = JsonHelper.JsonDllDeserialize<JCXX_Grid>(info);
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
        /// Json文件上传导入
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="ProID"></param>
        /// <param name="FileType"></param>
        /// <returns></returns>
        public ActionResult UploadFile(IFormFile uploadfile, string GridType)
        {
            if (uploadfile == null || !uploadfile.FileName.Contains(".json"))
                return Json(RepModel.Error("请上传正确的json格式文件"));
            if (string.IsNullOrEmpty(GridType))
                return Json(RepModel.Error("请选择导入的网格类型"));

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
                var listGrid = JsonHelper.JsonDllDeserializeList<JCXX_Grid>(json);
                return Json(bll.Import(listGrid, GridType));
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

        ///// <summary>
        ///// Excel上传导入
        ///// </summary>
        ///// <param name="fileData"></param>
        ///// <param name="ProID"></param>
        ///// <param name="FileType"></param>
        ///// <returns></returns>
        //public ActionResult UploadFile(IFormFile uploadfile,string GridType)
        //{
        //    if (uploadfile == null || !uploadfile.FileName.Contains(".xls"))
        //        return Json(RepModel.Error("请上传正确的工作簿文件"));
        //    if(string.IsNullOrEmpty(GridType))
        //        return Json(RepModel.Error("请选择导入的网格类型"));

        //    string sWebRootFolder = hostingEnv.WebRootPath;
        //    string sFileName = $"{Guid.NewGuid()}.xlsx";
        //    var tmpfile = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //    try
        //    {
        //        using (FileStream fs = new FileStream(tmpfile.ToString(), FileMode.Create))
        //        {
        //            uploadfile.CopyTo(fs);
        //            fs.Flush();
        //            //根据文件流创建excel数据结构
        //            var workbook = WorkbookFactory.Create(fs);
        //            //获取第一个sheet
        //            var sheet = workbook.GetSheetAt(0);
        //            //获取列数
        //            int cellCount = sheet.GetRow(0).LastCellNum;
        //            //获取行数
        //            int rowCount = sheet.LastRowNum;

        //            var listAttr = new List<string>() { "GridCode", "GridName", "ShowName", "Shape", "GridArea", "Note" };
        //            var listGrid = ExcelHelp.ExcelImport<JCXX_Grid>(sheet, 1, listAttr);
        //            return Json(bll.Import(listGrid, GridType));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(RepModel.Error(ex.Message));
        //    }
        //    finally {
        //        tmpfile.Delete();
        //    }
        //}

        public ActionResult DownloadFile(Query_Grid query)
        {
            var buffer = Encoding.UTF8.GetBytes(JsonHelper.JsonDllSerializeList<View_Grid>(bll.GetList(query), JsonDateTimeFormat.DateTime));
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

        #region 角色网格设置

        public ActionResult GetRoleGridList(Query_Grid query)
        {
            return Json(bll.GetRoleGridList(query));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult PostRoleGrid()
        {
            string ID = Request.Form["ID"];
            string RoleIDs = Request.Form["RoleIDs"];
            RepModel result = bll.SaveRoleGrid(ID, RoleIDs, LoginHelp.GetLoginInfo().Account);
            return Json(result);
        }

        #endregion 角色网格设置

        /// <summary>
        /// 网格权属角色多级设置
        /// </summary>
        /// <param name="GridID">网格ID</param>
        /// <returns></returns>
        public ActionResult GetEasyuiTreeList(string GridID)
        {
            return Json(bll.GetTreeList(GridID));
        }

        /// <summary>
        /// 根据网格ID和角色ID，更新父级角色ID
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateTreeByID()
        {
            string GridID = Request.Form["gridid"];         //网格ID
            string RoleID = Request.Form["id"];             //RoleID
            string ParentRoleID = Request.Form["targetId"]; //RoleParentID

            return Json(bll.UpdateTreeByID(GridID, RoleID, ParentRoleID));
        }

        #region 动态网格参数配置
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public ActionResult GetConfig()
        {
            string GridCode = Request.Form["GridCode"];
            return Json(bll.GetConfig(GridCode));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public ActionResult PostConfig()
        {
            string info = Request.Form["info"];
            JCXX_Grid_Config entity = JsonHelper.JsonDllDeserialize<JCXX_Grid_Config>(info);
            RepModel result = bll.SaveConfig(entity);
            return Json(result);
        }
        #endregion
    }
}