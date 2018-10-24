using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;

namespace Geone.JCXX.BLL
{
    public class MonitorBLL : IMonitorBLL
    {
        private IDbEntity<JCXX_Monitor> Respostry;
        LogWriter log = new LogWriter(new FileLogRecord());

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public MonitorBLL(IDbEntity<JCXX_Monitor> _t)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_Monitor");
        }

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_Monitor query)
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
        public List<JCXX_Monitor> GetList(Query_Monitor query)
        {
            try
            {
                var list = Respostry.Select().Where(t => t.IsDelete.Eq(0));
                SetQuery(list, query);
                switch (query.sort)
                {
                    case "MonitorType":
                        list = query.order == "asc" ? list.OrderBy(t => t.MonitorType)
                            : list.OrderByDesc(t => t.MonitorType);
                        break;
                    default:
                        list = list.OrderByDesc(t => t.CREATED);
                        break;
                }
                return list.QueryList();
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new List<JCXX_Monitor>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<JCXX_Monitor> list, Query_Monitor query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (query.MonitorType != null)
                list = list.And(t => t.MonitorType.Eq(query.MonitorType));
            if (query.Like_MonitorName != null)
                list = list.And(t => t.MonitorName.Like("%" + query.Like_MonitorName + "%"));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));

        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public RepModel Save(JCXX_Monitor entity)
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
                log.WriteException(null, ex);
                return RepModel.Error(ex.Message);
            }
        }


        public JCXX_Monitor GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_Monitor();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new JCXX_Monitor();
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
                log.WriteException(null, ex);
                return RepModel.Error(ex.Message);
            }
        }
    }
}
