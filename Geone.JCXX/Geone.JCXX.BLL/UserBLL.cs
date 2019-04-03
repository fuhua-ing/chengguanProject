using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geone.JCXX.BLL
{
    public class UserBLL : IUserBLL
    {
        private IDbEntity<JCXX_User> Respostry;
        private IDbEntity<JCXX_Dept> Respostry_D;
        private ILogWriter log;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public UserBLL(IDbEntity<JCXX_User> _t, IDbEntity<JCXX_Dept> _d, ILogWriter logWriter)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_User");

            Respostry_D = _d;
            Respostry_D.SetTable("JCXX_Dept");

            log = logWriter;
        }

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_User query)
        {
            var list = SetQuery(Respostry.Select(), query);
            var result = new GridData();
            try
            {
                result.total = list.Count;
                if (query.page > 0 && query.rows > 0)
                    list = (from u in list select u).Skip((int)query.rows * ((int)query.page - 1)).Take((int)query.rows).ToList();
                result.rows = GetExtList(list);
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                result.total = 0;
                result.rows = new List<JCXX_UserExtend>();
            }
            return result;
        }

        /// <summary>
        /// 反射
        /// </summary>
        /// <param name="userList"></param>
        /// <returns></returns>
        private List<JCXX_UserExtend> GetExtList(List<JCXX_User> dataList)
        {
            var extendList = new List<JCXX_UserExtend>();
            var deptList = Respostry_D.Select().Where(t => t.IsDelete.Eq(0)).QueryList();
            if (dataList.Count > 0)
            {
                foreach (var itemModel in dataList)
                {
                    var extendModel = new JCXX_UserExtend();
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
                    if (!string.IsNullOrEmpty(itemModel.DeptID))
                    {
                        var thisModel = deptList.Where(d => d.ID == itemModel.DeptID).FirstOrDefault();
                        extendModel.DeptName = thisModel == null ? string.Empty : thisModel.DeptName;
                    }
                    extendList.Add(extendModel);
                }
            }
            return extendList;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<JCXX_User> GetList(Query_User query)
        {
            try
            {
                var list = SetQuery(Respostry.Select(), query);
                switch (query.sort)
                {
                    case "Account":
                        list = query.order == "asc" ? list.OrderBy(t => t.Account).ToList() : list.OrderByDescending(t => t.Account).ToList();
                        break;

                    default:
                        list = list.OrderByDescending(t => t.CREATED).ToList();
                        break;
                }
                return list;
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new List<JCXX_User>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private List<JCXX_User> SetQuery(IDbEntity<JCXX_User> list, Query_User query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.DeptID))
                list = list.And(t => t.DeptID.Eq(query.DeptID));
            if (!string.IsNullOrEmpty(query.UserName))
                list = list.And(t => t.UserName.Eq(query.UserName));
            if (!string.IsNullOrEmpty(query.Account))
                list = list.And(t => t.Account.Eq(query.Account));
            if (!string.IsNullOrEmpty(query.Like_UserName))
                list = list.And(t => t.UserName.Like("%" + query.Like_UserName + "%"));
            if (!string.IsNullOrEmpty(query.Like_Account))
                list = list.And(t => t.Account.Like("%" + query.Like_Account + "%"));
            if (!string.IsNullOrEmpty(query.Like_AccountOrName))
            {
                list = list.And();
                list = list.Or(t => t.Account.Like("%" + query.Like_AccountOrName + "%"), t => t.UserName.Like("%" + query.Like_AccountOrName + "%"));
            }
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));
            var result = list.QueryList();
            if (!string.IsNullOrEmpty(query.DeptParentID))
            {
                var listDept = Respostry_D.Select().WhereAnd(t => t.IsDelete.Eq(0));
                listDept.And();
                List<JCXX_Dept> deptList = listDept.Or(t => t.ParentID.Eq(query.DeptParentID), t => t.ID.Eq(query.DeptParentID)).QueryList();
                result = GetChildList(result, deptList);
            }
            return result;
        }

        /// <summary>
        /// 递归本级和下级人员
        /// </summary>
        /// <param name="userList"></param>
        /// <param name="deptList"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        private List<JCXX_User> GetChildList(List<JCXX_User> userList, List<JCXX_Dept> deptList)
        {
            var resultList = new List<JCXX_User>();
            if (deptList.Count > 0)
            {
                foreach (var deptModel in deptList)
                {
                    var userModelList = from u in userList where u.DeptID == deptModel.ID select u;
                    resultList.AddRange(userModelList.ToList());
                }
            }
            return resultList;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public RepModel Save(JCXX_User entity)
        {
            RepModel result = new RepModel();
            if (string.IsNullOrEmpty(entity.ID))
            {
                if (!string.IsNullOrEmpty(entity.Account))
                {
                    var accountInfo = Respostry.Select().WhereAnd((t => t.Account.Eq(entity.Account)), (t => t.IsDelete.Eq(0))).QueryList();
                    if (accountInfo.Count() > 0)
                    {
                        result.StatusCode = 300;
                        result.Message = "用户名已存在";
                        return result;
                    }
                }
                entity.Pwd = Md5Encrypt.Encrypt(entity.Pwd, 32);
                entity.ID = Guid.NewGuid().ToString();
                entity.IsDelete = 0;
                entity.Enabled = 1;
                entity.CREATED = DateTime.Now;
                entity.CREATED_MAN = entity.CREATED_MAN;
                return Respostry.Insert(entity).ExecInsert() ? RepModel.Success("新增成功") : RepModel.Error("新增失败");
            }
            else
            {
                JCXX_User oldEntity = GetByID(entity.ID);
                if (oldEntity.Account != entity.Account)
                {
                    var accountInfo = Respostry.Select().WhereAnd((t => t.Account.Eq(entity.Account)),
                        (t => t.IsDelete.Eq(0))).QueryList();
                    if (accountInfo.Count() > 0)
                    {
                        result.StatusCode = 300;
                        result.Message = "用户名已存在";
                        return result;
                    }
                }
                entity.Pwd = oldEntity.Pwd;
                entity.UPDATED = DateTime.Now;
                entity.UPDATED_MAN = entity.UPDATED_MAN;
                return Respostry.Minus(t => t.CREATED, t => t.CREATED_MAN, t => t.IsDelete).
                        UpdateByPKey(entity).ExecModify() ? RepModel.Success("更新成功") : RepModel.Error("更新失败");
            }
        }

        /// <summary>
        /// 密码重置
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public RepModel PwdReset(JCXX_User entity)
        {
            if (string.IsNullOrEmpty(entity.ID))
                return RepModel.Error("用户ID不能为空");
            JCXX_User oldEntity = GetByID(entity.ID);
            oldEntity.Pwd = Md5Encrypt.Encrypt(entity.Pwd, 32);
            oldEntity.UPDATED = DateTime.Now;
            oldEntity.UPDATED_MAN = entity.UPDATED_MAN;
            return Respostry.UpdateByPKey(oldEntity).ExecModify() ? RepModel.Success("更新成功") : RepModel.Error("更新失败");
        }

        public JCXX_User GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_User();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new JCXX_User();
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