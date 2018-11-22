using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using System;
using System.Collections.Generic;

namespace Geone.JCXX.BLL
{
    public class DictItemBLL : IDictItemBLL
    {
        private IDbEntity<JCXX_DictItem> Respostry;
        private IDbEntity<View_DictItem> Respostry_V;
        private ILogWriter log;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public DictItemBLL(IDbEntity<JCXX_DictItem> _t, IDbEntity<View_DictItem> _tv, ILogWriter logWriter)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_DictItem");

            Respostry_V = _tv;
            Respostry_V.SetTable("View_DictItem");

            log = logWriter;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_DictItem query)
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
        public List<JCXX_DictItem> GetList(Query_DictItem query)
        {
            try
            {
                var list = Respostry.Select();
                list = list.Where(t => t.IsDelete.Eq(0));
                if (!string.IsNullOrEmpty(query.CategoryID))
                    list = list.And(t => t.CategoryID.Eq(query.CategoryID));
                if (!string.IsNullOrEmpty(query.Like_ItemCode))
                    list = list.And(t => t.ItemCode.Like("%" + query.Like_ItemCode + "%"));
                if (!string.IsNullOrEmpty(query.Like_ItemName))
                    list = list.And(t => t.ItemName.Like("%" + query.Like_ItemName + "%"));
                if (query.Enabled != null)
                    list = list.And(t => t.Enabled.Eq((int)query.Enabled));
                switch (query.sort)
                {
                    case "ItemCode":
                        list = query.order == "asc" ? list.OrderBy(t => t.ItemCode) : list.OrderByDesc(t => t.ItemCode);
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
                return new List<JCXX_DictItem>();
            }
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<View_DictItem> GetExtList(Query_DictItem query)
        {
            try
            {
                var list = Respostry_V.Select();
                SetQuery(list, query);
                switch (query.sort)
                {
                    case "ItemCode":
                        list = query.order == "desc" ? list.OrderByDesc(t => t.ItemCode) : list.OrderByDesc(t => t.ItemCode);
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
                return new List<View_DictItem>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<View_DictItem> list, Query_DictItem query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.AppID))
                list = list.And(t => t.AppID.Eq(query.AppID));
            if (!string.IsNullOrEmpty(query.CategoryCode))
                list = list.And(t => t.CategoryCode.Eq(query.CategoryCode));
            if (!string.IsNullOrEmpty(query.Note))
                list = list.And(t => t.Note.Eq(query.Note));
            if (!string.IsNullOrEmpty(query.CategoryID))
                list = list.And(t => t.CategoryID.Eq(query.CategoryID));
            if (!string.IsNullOrEmpty(query.Like_ItemCode))
                list = list.And(t => t.ItemCode.Like("%" + query.Like_ItemCode + "%"));
            if (!string.IsNullOrEmpty(query.Like_ItemName))
                list = list.And(t => t.ItemName.Like("%" + query.Like_ItemName + "%"));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));
        }

        public JCXX_DictItem GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_DictItem();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new JCXX_DictItem();
            }
        }

        public RepModel Save(JCXX_DictItem entity)
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
    }
}