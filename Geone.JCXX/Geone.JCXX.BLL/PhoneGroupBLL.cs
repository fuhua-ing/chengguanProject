using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;

namespace Geone.JCXX.BLL
{
    public class PhoneGroupBLL : IPhoneGroupBLL
    {
        private IDbEntity<JCXX_PhoneGroup> Respostry;
        private IDbEntity<View_PhoneGroup> Respostry_V;
        private IDbEntity<JCXX_QSRole_PhoneGroup> Respostry_RP;
        private IDbEntity<View_QSRolePhoneGroup> Respostry_VRP;

        private DictItemBLL dcbll;
        private QSRoleBLL qsbll;
        LogWriter log = new LogWriter(new FileLogRecord());

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public PhoneGroupBLL(IDbEntity<JCXX_PhoneGroup> _t, IDbEntity<View_PhoneGroup> _tv
            , IDbEntity<JCXX_DictItem> _tdc, IDbEntity<View_DictItem> _tvdc
            , IDbEntity<JCXX_QSRole> _tr, IDbEntity<JCXX_QSRole_PhoneGroup> _trp
            , IDbEntity<View_QSRolePhoneGroup> _tvrp)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_PhoneGroup");

            Respostry_V = _tv;
            Respostry_V.SetTable("View_PhoneGroup");

            Respostry_RP = _trp;
            Respostry_RP.SetTable("JCXX_QSRole_PhoneGroup");

            Respostry_VRP = _tvrp;
            Respostry_VRP.SetTable("View_QSRolePhoneGroup");


            dcbll = new DictItemBLL(_tdc, _tvdc);
            qsbll = new QSRoleBLL(_tr);
        }

        #region 短信号码分组维护
        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_PhoneGroup query)
        {
            var list = Respostry_V.Select();
            SetQuery(list, query);
            if (string.IsNullOrEmpty(query.sort))
                query.sort = "CREATED";
            var result = list.QueryPage((int)query.page, (int)query.rows, query.sort);
            return new GridData() { rows = result.Rows, total = result.Total };
        }

        /// <summary>
        /// 获取列表(View_PhoneGroup视图)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<View_PhoneGroup> GetList(Query_PhoneGroup query)
        {
            try
            {
                var list = Respostry_V.Select();
                SetQuery(list, query);
                switch (query.sort)
                {
                    case "GroupName":
                        list = query.order == "asc" ? list.OrderBy(t => t.GroupName) : list.OrderByDesc(t => t.GroupName);
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
                return new List<View_PhoneGroup>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<View_PhoneGroup> list, Query_PhoneGroup query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.Like_GroupName))
                list = list.And(t => t.GroupName.Eq("%"+query.Like_GroupName+"%"));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));
        }


        public JCXX_PhoneGroup GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_PhoneGroup();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new JCXX_PhoneGroup();
            }
        }

        public RepModel Save(JCXX_PhoneGroup entity)
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

        #endregion

        #region 角色分组设置

        /// <summary>
        /// 获取角色分组列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<View_QSRolePhoneGroup> GetRoleGroupList(Query_PhoneGroup query)
        {
            var list = Respostry_VRP.Select();
            if (!string.IsNullOrEmpty(query.GroupID))
                list = list.Where(t => t.GroupID.Eq(query.GroupID));

            return list.QueryList();
        }

        /// <summary>
        /// 保存角色分组
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="RoleIDs"></param>
        /// <returns></returns>
        public RepModel SaveRoleGroup(string GroupID, string RoleIDs, string CREATED_MAN)
        {
            try
            {
                if (string.IsNullOrEmpty(GroupID))
                    return RepModel.Error("分组不能为空");
                //删除原有角色分组
                Respostry_RP.Delete().Where(t => t.GroupID.Eq(GroupID));
                //新增现有角色分组
                var listNew = new List<JCXX_QSRole_PhoneGroup>();
                foreach (var RoleID in RoleIDs.Split(','))
                {
                    if (string.IsNullOrEmpty(RoleID))
                        continue;
                    listNew.Add(new JCXX_QSRole_PhoneGroup()
                    {
                        ID = Guid.NewGuid().ToString(),
                        RoleID = RoleID,
                        GroupID = GroupID,
                        CREATED = DateTime.Now,
                        CREATED_MAN = CREATED_MAN
                    });
                }
                return listNew.Count == 0 || Respostry_RP.Insert(listNew).ExecInsertBatch() ? RepModel.Success("操作成功") : RepModel.Error("操作失败");
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return RepModel.Error(ex.Message);
            }
        }

        #endregion
    }
}
