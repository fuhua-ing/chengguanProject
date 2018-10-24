using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;

namespace Geone.JCXX.BLL
{
    public class DictCategoryBLL : IDictCategoryBLL
    {
        private IDbEntity<JCXX_DictCategory> Respostry;
        private AppBLL appBLL;
        LogWriter log = new LogWriter(new FileLogRecord());

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public DictCategoryBLL(IDbEntity<JCXX_DictCategory> _t,
             IDbEntity<JCXX_App> _tapp)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_DictCategory");

            appBLL = new AppBLL(_tapp);
        }
        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_DictCategory query)
        {
            var list = Respostry.Select();
            SetQuery(list, query);
            if (string.IsNullOrEmpty(query.sort))
                query.sort = "CREATED";
            var result = list.QueryPage((int)query.page, (int)query.rows, query.sort);
            return new GridData() { rows = GetExtList(result.Rows), total = result.Total };
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<JCXX_DictCategory> GetList(Query_DictCategory query)
        {
            try
            {
                var list = Respostry.Select();
                SetQuery(list, query);
                switch (query.sort)
                {
                    case "CategoryCode":

                        list = query.order == "asc" ? list.OrderBy(t => t.CategoryCode) : list.OrderByDesc(t => t.CategoryCode);
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
                return new List<JCXX_DictCategory>();
            }

        }


        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<JCXX_DictCategory> list, Query_DictCategory query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.Like_CategoryCode))
                list = list.And(t => t.CategoryCode.Like("%" + query.Like_CategoryCode + "%"));
            if (!string.IsNullOrEmpty(query.Like_CategoryName))
                list = list.And(t => t.CategoryName.Like("%" + query.Like_CategoryName + "%"));
            if (!string.IsNullOrEmpty(query.AppID))
                list = list.And(t => t.AppID.Eq(query.AppID));
            if (!string.IsNullOrEmpty(query.CategoryCode))
                list = list.And(t => t.CategoryCode.Eq(query.CategoryCode));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));

        }

        /// <summary>
        /// 反射
        /// </summary>
        /// <param name="userList"></param>
        /// <returns></returns>
        private List<JCXX_DictCategoryExtend> GetExtList(List<JCXX_DictCategory> dataList)
        {
            try
            {
                var extendList = new List<JCXX_DictCategoryExtend>();
                if (dataList != null && dataList.Count > 0)
                {
                    var appList = appBLL.GetAll();
                    foreach (var itemModel in dataList)
                    {
                        var extendModel = new JCXX_DictCategoryExtend();
                        var extendType = extendModel.GetType();
                        var propertysList = extendType.GetProperties();
                        var type = itemModel.GetType();
                        var propertysListAgain = type.GetProperties();
                        foreach (var propertModel in propertysList)
                        {
                            var propertysModel = propertysListAgain.Where(p => p.Name == propertModel.Name).FirstOrDefault();
                            if (propertysModel != null)
                            {
                                propertModel.SetValue(extendModel, propertysModel.GetValue(itemModel));
                            }
                        }
                        if (!string.IsNullOrEmpty(itemModel.AppID))
                        {
                            var thisModel = appList.Where(d => d.ID == itemModel.AppID).FirstOrDefault();
                            extendModel.AppName = thisModel == null ? string.Empty : thisModel.AppName;
                        }
                        extendList.Add(extendModel);
                    }
                }
                return extendList;
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new List<JCXX_DictCategoryExtend>();
            }

        }

        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
        public JCXX_DictCategory GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_DictCategory();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new JCXX_DictCategory();
            }
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public RepModel Save(JCXX_DictCategory entity)
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

        /// <summary>
        /// 通过ID删除
        /// </summary>
        /// <param name="ID">主键ID</param>
        /// <returns></returns>
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
