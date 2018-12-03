using Geone.JCXX.Meta;
using Geone.JCXX.Meta.ExtendEntity;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using System;
using System.Collections.Generic;

namespace Geone.JCXX.BLL
{
    public class CaseLATJBLL : ICaseLATJBLL
    {
        private IDbEntity<JCXX_CaseLATJ> Respostry;
        private IDbEntity<View_CaseLATJ> Respostry_V;
        private ILogWriter log;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public CaseLATJBLL(IDbEntity<JCXX_CaseLATJ> _t, IDbEntity<View_CaseLATJ> _vt, ILogWriter logWriter)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_CaseLATJ");

            Respostry_V = _vt;
            Respostry_V.SetTable("View_CaseLATJ");

            log = logWriter;
        }

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_CaseLATJ query)
        {
            var list = Respostry_V.Select();
            SetQuery(list, query);
            if (string.IsNullOrEmpty(query.sort))
                query.sort = "CREATED";
            var result = list.QueryPage((int)query.page, (int)query.rows, query.sort);
            return new GridData() { rows = result.Rows, total = result.Total };
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<View_CaseLATJ> GetList(Query_CaseLATJ query)
        {
            try
            {
                var list = Respostry_V;//.Select().Where(t => t.IsDelete.Eq(0));
                SetQuery(list, query);
                switch (query.sort)
                {
                    case "CaseClassI":
                        list = query.order == "asc" ? list.OrderBy(t => t.CaseClassI)
                            : list.OrderByDesc(t => t.CaseClassI);
                        break;

                    default:
                        list = list.OrderByDesc(t => t.CREATED);
                        break;
                }
                return list.QueryList();
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new List<View_CaseLATJ>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<View_CaseLATJ> list, Query_CaseLATJ query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            //if (query.Enabled != null)
            //    list = list.And(t => t.Enabled.Eq(query.Enabled));
            if (!string.IsNullOrEmpty(query.CaseClassI))
                list = list.And(t => t.CaseClassI.Eq(query.CaseClassI));
            if (!string.IsNullOrEmpty(query.CaseClassII))
                list = list.And(t => t.CaseClassII.Eq(query.CaseClassII));
            if (query.Like_TimeLimitDesc != null)
                list = list.And(t => t.TimeLimitDesc.Like("%" + query.Like_TimeLimitDesc + "%"));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public RepModel Save(JCXX_CaseLATJ entity)
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

        public View_CaseLATJ GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new View_CaseLATJ();
                else
                {
                    return Respostry_V.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new View_CaseLATJ();
            }
        }

        public RepModel Del(string ID)
        {
            try
            {
                return Respostry.DeleteByPKey(ID).ExecRemove() ? RepModel.Success("删除成功") : RepModel.Error("删除失败");
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return RepModel.Error(ex.Message);
            }
        }
    }
}
