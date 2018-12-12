using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using System;
using System.Collections.Generic;

namespace Geone.JCXX.BLL
{
    public class CaseTimeLimitBLL : ICaseTimeLimitBLL
    {
        private IDbEntity<JCXX_CaseTimeLimit> Respostry;
        private ILogWriter log;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public CaseTimeLimitBLL(IDbEntity<JCXX_CaseTimeLimit> _t, ILogWriter logWriter)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_CaseTimeLimit");
            log = logWriter;
        }

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_CaseTimeLimit query)
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
        /// <param name="query"></param>
        /// <returns></returns>
        public List<JCXX_CaseTimeLimit> GetList(Query_CaseTimeLimit query)
        {
            try
            {
                var list = Respostry.Select().Where(t => t.IsDelete.Eq(0));
                SetQuery(list, query);
                switch (query.sort)
                {
                    case "NodeType":
                        list = query.order == "asc" ? list.OrderBy(t => t.NodeType)
                            : list.OrderByDesc(t => t.NodeType);
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
                return new List<JCXX_CaseTimeLimit>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<JCXX_CaseTimeLimit> list, Query_CaseTimeLimit query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq(query.Enabled));
            if (query.Like_NodeTypeDesc != null)
                list = list.And(t => t.NodeTypeDesc.Like("%" + query.Like_NodeTypeDesc + "%"));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public RepModel Save(JCXX_CaseTimeLimit entity)
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

        /// <summary>
        /// 根据ID，获取记录数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JCXX_CaseTimeLimit GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_CaseTimeLimit();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new JCXX_CaseTimeLimit();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
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
