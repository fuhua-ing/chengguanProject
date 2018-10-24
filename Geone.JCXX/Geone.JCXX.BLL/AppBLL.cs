using Geone.JCXX.Meta;
using Geone.Utiliy.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geone.Utiliy.Database;

namespace Geone.JCXX.BLL
{
    public class AppBLL : IAppBLL
    {
        private IDbEntity<JCXX_App> Respostry;
        LogWriter log = new LogWriter(new FileLogRecord());

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public AppBLL(IDbEntity<JCXX_App> _t)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_App");
        }


        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_App query)
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
        public List<JCXX_App> GetList(Query_App query)
        {
            try
            {
                var list = Respostry.Select();
                SetQuery(list, query);
               
                switch (query.sort)
                {
                    case "AppCode":

                        list = query.order == "asc" ? list.OrderBy(t => t.AppCode) : list.OrderByDesc(t => t.AppCode);
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
                return new List<JCXX_App>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<JCXX_App> list, Query_App query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.AppCode))
                list = list.And(t => t.AppCode.Eq(query.AppCode));
            if (!string.IsNullOrEmpty(query.Like_AppName))
                list = list.And(t => t.AppName.Like("%" + query.Like_AppName + "%"));
            if (!string.IsNullOrEmpty(query.Like_AppCode))
                list = list.And(t => t.AppCode.Like("%"+query.Like_AppCode + "%"));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));

        }

        public List<JCXX_App> GetAll()
        {
            return Respostry.Select().Where(t => t.IsDelete.Eq(0)).QueryList();
        }

        public JCXX_App GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_App();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new JCXX_App();
            }
        }

        public RepModel Save(JCXX_App entity)
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
                    return Respostry.Minus(t => t.CREATED, t => t.CREATED_MAN,t=>t.IsDelete).
                       UpdateByPKey(entity).ExecModify() ? RepModel.Success("更新成功") : RepModel.Error("更新失败");
                }
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
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
                log.WriteException(null, ex);
                return RepModel.Error(ex.Message);
            }
        }
    }
}
