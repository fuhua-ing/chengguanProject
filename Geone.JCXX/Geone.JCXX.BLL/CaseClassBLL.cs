using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geone.JCXX.BLL
{
    public class CaseClassBLL : ICaseClassBLL
    {
        private IDbEntity<JCXX_CaseClass> Respostry;
        private IDbEntity<View_CaseClass> Respostry_V;
        private IDbEntity<JCXX_QSRole_CaseClass> Respostry_RC;
        private IDbEntity<View_QSRoleCaseClass> Respostry_VRC;
        private ILogWriter log;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public CaseClassBLL(IDbEntity<JCXX_CaseClass> _t, IDbEntity<View_CaseClass> _tv,
            IDbEntity<JCXX_QSRole_CaseClass> _trc, IDbEntity<View_QSRoleCaseClass> _tvrc,
            ILogWriter logWriter)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_CaseClass");

            Respostry_V = _tv;
            Respostry_V.SetTable("View_CaseClass");

            Respostry_RC = _trc;
            Respostry_RC.SetTable("JCXX_QSRole_CaseClass");

            Respostry_VRC = _tvrc;
            Respostry_VRC.SetTable("View_QSRoleCaseClass");

            log = logWriter;
        }

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_CaseClass query)
        {
            var list = Respostry_V.Select();
            SetQuery(list, query);
            if (string.IsNullOrEmpty(query.sort))
                query.sort = "CREATED";
            var result = list.QueryPage((int)query.page, (int)query.rows, new string[] { "CaseClassI", "CaseClassII", "CaseClassIII" });
            return new GridData() { rows = result.Rows, total = result.Total };
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<View_CaseClass> GetList(Query_CaseClass query)
        {
            try
            {
                var list = Respostry_V.Select();
                SetQuery(list, query);
                var result = list.QueryList();
                switch (query.sort)
                {
                    default:
                        result = query.order == "asc" ? result.OrderBy(t => t.CaseClassI).OrderBy(t => t.CaseClassII).OrderBy(t => t.CaseClassIII).ToList()
                            : result.OrderByDescending(t => t.CaseClassI).OrderByDescending(t => t.CaseClassII).OrderByDescending(t => t.CaseClassIII).ToList();
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new List<View_CaseClass>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<View_CaseClass> list, Query_CaseClass query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.RoleType))
                list = list.And(t => t.RoleType.Eq(query.RoleType));
            if (!string.IsNullOrEmpty(query.CaseType))
                list = list.And(t => t.CaseType.Eq(query.CaseType));
            if (!string.IsNullOrEmpty(query.CaseClassI))
                list = list.And(t => t.CaseClassI.Eq(query.CaseClassI));
            if (!string.IsNullOrEmpty(query.CaseClassII))
                list = list.And(t => t.CaseClassII.Eq(query.CaseClassII));
            if (!string.IsNullOrEmpty(query.CaseClassIII))
                list = list.And(t => t.CaseClassII.Eq(query.CaseClassIII));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));
        }

        public JCXX_CaseClass GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_CaseClass();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new JCXX_CaseClass();
            }
        }

        public RepModel Save(JCXX_CaseClass entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.ID))
                {
                    entity.ID = Guid.NewGuid().ToString();
                    entity.IsDelete = 0;
                    entity.Enabled = 1;
                    entity.CREATED = DateTime.Now;
                    entity.CREATED_MAN = entity.CREATED_MAN;
                    return Respostry.Insert(entity).ExecInsert() ? RepModel.Success("新增成功") : RepModel.Error("新增失败");
                }
                else
                {
                    entity.UPDATED = DateTime.Now;
                    entity.UPDATED_MAN = entity.UPDATED_MAN;
                    return Respostry.Minus(t => t.CREATED, t => t.CREATED_MAN, t => t.IsDelete).
                        UpdateByPKey(entity).ExecModify() ? RepModel.Success("更新成功") : RepModel.Error("更新失败");
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return RepModel.Error(ex.Message);
            }
        }

        public RepModel Del(string ID)
        {
            try
            {
                var oldEntity = GetByID(ID);
                oldEntity.IsDelete = 1;
                oldEntity.UPDATED = DateTime.Now;
                oldEntity.UPDATED_MAN = oldEntity.UPDATED_MAN;
                return Respostry.UpdateByPKey(oldEntity).ExecModify() ? RepModel.Success("删除成功") : RepModel.Error("删除失败");
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return RepModel.Error(ex.Message);
            }
        }

        #region 角色案件设置

        /// <summary>
        /// 获取角色案件列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<View_QSRoleCaseClass> GetRoleCaseList(Query_CaseClass query)
        {
            var list = Respostry_VRC.Select();
            if (!string.IsNullOrEmpty(query.ID))
                list = list.Where(t => t.CaseClassID.Eq(query.ID));
            return list.QueryList();
        }

        /// <summary>
        /// 保存角色案件
        /// </summary>
        /// <param name="CaseClassID"></param>
        /// <param name="RoleIDs"></param>
        /// <returns></returns>
        public RepModel SaveRoleCase(string CaseClassID, string RoleIDs, string CREATED_MAN)
        {
            try
            {
                if (string.IsNullOrEmpty(CaseClassID))
                    return RepModel.Error("案件类型不能为空");
                //删除原有角色案件
                Respostry_RC.Delete().Where(t => t.CaseClassID.Eq(CaseClassID)).ExecRemove();
                //新增现有角色案件
                var listNew = new List<JCXX_QSRole_CaseClass>();
                foreach (var RoleID in RoleIDs.Split(','))
                {
                    if (string.IsNullOrEmpty(RoleID))
                        continue;
                    listNew.Add(new JCXX_QSRole_CaseClass()
                    {
                        ID = Guid.NewGuid().ToString(),
                        RoleID = RoleID,
                        CaseClassID = CaseClassID,
                        CREATED = DateTime.Now,
                        CREATED_MAN = CREATED_MAN
                    });
                }
                return listNew.Count == 0 || Respostry_RC.Insert(listNew).ExecInsertBatch() ? RepModel.Success("操作成功") : RepModel.Error("操作失败");
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return RepModel.Error(ex.Message);
            }
        }

        public RepModel Import(List<JCXX_CaseClass> list)
        {
            try
            {
                foreach (var entity in list)
                {
                    entity.ID = Guid.NewGuid().ToString();
                    entity.IsDelete = 0;
                    entity.Enabled = 1;
                    entity.CREATED = DateTime.Now;
                    entity.CREATED_MAN = "";
                }
                return Respostry.Insert(list).ExecInsertBatch() ? RepModel.Success("导入成功") : RepModel.Error("导入失败");
            }
            catch (Exception ex)
            {
                //log.WriteException(ex);
                return RepModel.Error(ex.Message);
            }
        }

        #endregion 角色案件设置
    }
}