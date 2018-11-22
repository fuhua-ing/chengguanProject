using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geone.JCXX.BLL
{
    public class DeptBLL : IDeptBLL
    {
        private IDbEntity<JCXX_Dept> Respostry;
        private ILogWriter log;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public DeptBLL(IDbEntity<JCXX_Dept> _t, ILogWriter logWriter)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_Dept");
            log = logWriter;
        }

        public JCXX_Dept GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_Dept();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex);
                return new JCXX_Dept();
            }
        }

        public RepModel Save(JCXX_Dept entity)
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

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_Dept query)
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
        public List<JCXX_Dept> GetList(Query_Dept query)
        {
            try
            {
                var list = SetQuery(Respostry.Select(), query);
                switch (query.sort)
                {
                    case "DeptCode":
                        list = query.order == "asc" ? list.OrderBy(t => t.DeptCode).ToList() : list.OrderByDescending(t => t.DeptCode).ToList();
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
                return new List<JCXX_Dept>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private List<JCXX_Dept> SetQuery(IDbEntity<JCXX_Dept> list, Query_Dept query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.DeptName))
                list = list.And(t => t.DeptName.Eq(query.DeptName));
            if (!string.IsNullOrEmpty(query.DeptCode))
                list = list.And(t => t.DeptCode.Eq(query.DeptCode));
            if (!string.IsNullOrEmpty(query.Like_DeptName))
                list = list.And(t => t.DeptName.Like("%" + query.Like_DeptName + "%"));
            if (!string.IsNullOrEmpty(query.Like_DeptCode))
                list = list.And(t => t.DeptCode.Like("%" + query.Like_DeptCode + "%"));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));
            var result = list.QueryList();
            if (!string.IsNullOrEmpty(query.ParentID))
            {
                result = GetChildList(result, query.ParentID);
            }
            return result;
        }

        /// <summary>
        /// 递归本级和下级部门
        /// </summary>
        /// <param name="deptList"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        private List<JCXX_Dept> GetChildList(List<JCXX_Dept> deptList, string parentID)
        {
            var resultList = new List<JCXX_Dept>();
            var parentModel = new JCXX_Dept();
            if (string.IsNullOrEmpty(parentID))
            {
                var childList = deptList.Where(d => d.ParentID == string.Empty);
                if (childList.Count() > 0)
                {
                    foreach (var childModel in childList)
                    {
                        resultList.AddRange(GetChildList(deptList, childModel.ID).OrderBy(t => t.CREATED));
                    }
                }
            }
            else
            {
                parentModel = deptList.Where(d => d.ID == parentID).FirstOrDefault();
                if (parentModel != null)
                {
                    resultList.Add(parentModel);
                    var childList = deptList.Where(d => d.ParentID == parentID);
                    if (childList.Count() > 0)
                    {
                        foreach (var childModel in childList)
                        {
                            resultList.AddRange(GetChildList(deptList, childModel.ID).OrderBy(t => t.CREATED));
                        }
                    }
                }
            }
            return resultList;
        }

        /// <summary>
        /// 获取Easyui树形结构
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<EasyuiTreeNode_Dept> GetTreeList(Query_Dept query)
        {
            var listResut = new List<EasyuiTreeNode_Dept>();
            var listAll = GetList(query);
            var listParent = listAll.Where(t => string.IsNullOrEmpty(t.ParentID)).OrderBy(t => t.DeptCode).ToList();
            //如果查找结果不为0，根目录为0
            if (listAll.Count > 0 && listParent.Count() == 0)
            {
                var q = from p in listAll
                        group p by p.ParentID into g
                        select new { g.Key };
                foreach (var item in q)
                {
                    if (listAll.Where(t => t.ID == item.Key).Count() == 0)
                    {
                        listParent.AddRange(listAll.Where(t => t.ParentID == item.Key).ToList());
                    }
                }
            }
            foreach (JCXX_Dept parent in listParent)
            {
                var parentNode = new EasyuiTreeNode_Dept()
                {
                    text = parent.DeptName,
                    id = parent.ID,
                    parentid = "",
                    children = new List<EasyuiTreeNode_Dept>(),
                    ID = parent.ID,
                    DeptCode = parent.DeptCode,
                    DeptName = parent.DeptName,
                    ShortName = parent.ShortName,
                    DeptType = parent.DeptType,
                    //DeptTypeName = parent.DeptTypeName,
                    Contact = parent.Contact,
                    ContactTel = parent.ContactTel,
                    ContactEmail = parent.ContactEmail,
                    Note = parent.Note,
                    Enabled = parent.Enabled
                };
                setSubTreeList(parentNode, listAll);
                listResut.Add(parentNode);
            }
            var firstListResut = new List<EasyuiTreeNode_Dept>();
            if (query.ChoiceAll == null)
            {
                var firstPparentNode = new EasyuiTreeNode_Dept()
                {
                    text = "全部",
                    id = "",
                    parentid = null,
                    ischecked = 1,
                    children = listResut.Where(r => r.parentid == "").ToList()
                };
                firstListResut.Add(firstPparentNode);
            }
            else
            {
                firstListResut = listResut;
            }
            return firstListResut;
        }

        private void setSubTreeList(EasyuiTreeNode_Dept ParentNode, List<JCXX_Dept> listAll)
        {
            foreach (JCXX_Dept child in listAll.Where(t => t.ParentID == ParentNode.id).OrderBy(t => t.DeptCode))
            {
                var childNode = new EasyuiTreeNode_Dept()
                {
                    text = child.DeptName,
                    id = child.ID,
                    parentid = ParentNode.id,
                    children = new List<EasyuiTreeNode_Dept>(),
                    ID = child.ID,
                    DeptCode = child.DeptCode,
                    DeptName = child.DeptName,
                    ShortName = child.ShortName,
                    DeptType = child.DeptType,
                    //DeptTypeName = child.DeptTypeName,
                    Contact = child.Contact,
                    ContactTel = child.ContactTel,
                    ContactEmail = child.ContactEmail,
                    Note = child.Note,
                    Enabled = child.Enabled
                };
                setSubTreeList(childNode, listAll);
                ParentNode.children.Add(childNode);
            }
        }
    }
}