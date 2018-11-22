﻿using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using System;
using System.Collections.Generic;

namespace Geone.JCXX.BLL
{
    public class PhoneGroupItemBLL : IPhoneGroupItemBLL
    {
        private IDbEntity<JCXX_PhoneGroupItem> Respostry;
        private ILogWriter log;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public PhoneGroupItemBLL(IDbEntity<JCXX_PhoneGroupItem> _t, ILogWriter logWriter)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_PhoneGroupItem");
            log = logWriter;
        }

        #region 短信号码维护

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_PhoneGroupItem query)
        {
            var list = Respostry.Select();
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
        public List<JCXX_PhoneGroupItem> GetList(Query_PhoneGroupItem query)
        {
            try
            {
                var list = Respostry.Select();
                SetQuery(list, query);
                switch (query.sort)
                {
                    case "PersonName":
                        list = query.order == "asc" ? list.OrderByDesc(t => t.PersonName) : list.OrderBy(t => t.PersonName);
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
                return new List<JCXX_PhoneGroupItem>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<JCXX_PhoneGroupItem> list, Query_PhoneGroupItem query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.Like_PersonName))
                list = list.And(t => t.PersonName.Like("%" + query.Like_PersonName + "%"));
            if (!string.IsNullOrEmpty(query.GroupID))
                list = list.And(t => t.GroupID.Eq(query.GroupID));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));
        }

        public JCXX_PhoneGroupItem GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_PhoneGroupItem();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new JCXX_PhoneGroupItem();
            }
        }

        public RepModel Save(JCXX_PhoneGroupItem entity)
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

        #endregion 短信号码维护
    }
}