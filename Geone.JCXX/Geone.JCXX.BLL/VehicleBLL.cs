using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;

namespace Geone.JCXX.BLL
{
    public class VehicleBLL : IVehicleBLL
    {
        private IDbEntity<JCXX_Vehicle> Respostry;
        private IDbEntity<JCXX_Dept> Respostry_D;
        private IDbEntity<View_Vehicle> Respostry_V;
        LogWriter log = new LogWriter(new FileLogRecord());

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public VehicleBLL(IDbEntity<JCXX_Vehicle> _t, IDbEntity<JCXX_Dept> _d, IDbEntity<View_Vehicle> _v)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_Vehicle");

            Respostry_D = _d;
            Respostry_D.SetTable("JCXX_Dept");

            Respostry_V = _v;
            Respostry_V.SetTable("View_Vehicle");
        }

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_Vehicle query)
        {
            var list = SetQuery(Respostry_V.Select(), query);
            var result = new GridData();
            try
            {
                result.total = list.Count;
                if (query.page <= 0 && query.rows <= 0)
                    result.rows = list;
                else
                    result.rows = (from u in list select u).Skip((int)query.rows * ((int)query.page - 1)).Take((int)query.rows).ToList();
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                result.total = 0;
                result.rows = new List<View_Vehicle>();

            }
            return result;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<View_Vehicle> GetList(Query_Vehicle query)
        {
            try
            {
                var list = SetQuery(Respostry_V.Select(), query);
                switch (query.sort)
                {
                    case "CarNo":

                        list = query.order == "asc" ? list.OrderBy(t => t.CarNo).ToList() : list.OrderByDescending(t => t.CarNo).ToList();
                        break;
                    default:
                        list = list.OrderByDescending(t => t.CREATED).ToList();
                        break;
                }
                return list;
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new List<View_Vehicle>();
            }

        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private List<View_Vehicle> SetQuery(IDbEntity<View_Vehicle> list, Query_Vehicle query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.CarNo))
                list = list.And(t => t.CarNo.Eq(query.CarNo));
            if (!string.IsNullOrEmpty(query.VehicleType))
                list = list.And(t => t.VehicleType.Eq(query.VehicleType));
            if (!string.IsNullOrEmpty(query.CarType))
                list = list.And(t => t.CarType.Eq(query.CarType));
            if (!string.IsNullOrEmpty(query.GPRS))
                list = list.And(t => t.GPRS.Eq(query.GPRS));
            if (!string.IsNullOrEmpty(query.Like_GPRS))
                list = list.And(t => t.GPRS.Like("%" + query.Like_GPRS + "%"));
            if (!string.IsNullOrEmpty(query.Like_CarNo))
                list = list.And(t => t.CarNo.Like("%" + query.Like_CarNo + "%"));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled == (int)query.Enabled);
            var result = list.QueryList();
            if (!string.IsNullOrEmpty(query.DeptID))
            {
                var listDept = Respostry_D.Select().WhereAnd(t => t.IsDelete.Eq(0));
                listDept.And();
                List<JCXX_Dept> deptList = listDept.Or(t => t.ParentID.Eq(query.DeptID), t => t.ID.Eq(query.DeptID)).QueryList();
                result = GetChildList(result, deptList);
            }
            return result;
        }

        /// <summary>
        /// 递归本级和下级部门车辆
        /// </summary>
        /// <param name="userList"></param>
        /// <param name="deptList"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        private List<View_Vehicle> GetChildList(List<View_Vehicle> list, List<JCXX_Dept> deptList)
        {
            var resultList = new List<View_Vehicle>();
            if (deptList.Count > 0)
            {
                foreach (var deptModel in deptList)
                {
                    var userModelList = from u in list where u.DeptID == deptModel.ID select u;
                    resultList.AddRange(userModelList.ToList());
                }
            }
            return resultList;
        }



        public JCXX_Vehicle GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_Vehicle();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new JCXX_Vehicle();
            }
        }

        public RepModel Save(JCXX_Vehicle entity)
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
