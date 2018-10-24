using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geone.JCXX.Meta;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;

namespace Geone.JCXX.BLL
{
    public class AppMenuBLL : IAppMenuBLL
    {
        private IDbEntity<JCXX_AppMenu> Respostry;
        LogWriter log = new LogWriter(new FileLogRecord());

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="_t"></param>
        public AppMenuBLL(IDbEntity<JCXX_AppMenu> _t)
        {
            Respostry = _t;
            Respostry.SetTable("JCXX_AppMenu");
        }


        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="qf"></param>
        /// <returns></returns>
        public GridData GetGrid(Query_AppMenu query)
        {
            var list = GetList(query);
            var result = new GridData();
            try
            {
                result.total = list.Count;
                if (query.page > 0 && query.rows > 0)
                    list = (from u in list select u).Skip((int)query.rows * ((int)query.page - 1)).Take((int)query.rows).ToList();
                result.rows = list;
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                result.total = 0;
                result.rows = new List<JCXX_AppMenu>();

            }
            return result;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        public List<JCXX_AppMenu> GetList(Query_AppMenu query)
        {
            try
            {
                var list = Respostry.Select();
                SetQuery(list, query);
                var listResult = list.QueryList();
                if (!string.IsNullOrEmpty(query.ParentID))
                {
                    listResult = GetChildList(listResult, query.ParentID);
                }
                switch (query.sort)
                {
                    case "MenuCode":
                        listResult = query.order == "desc" ? listResult.OrderByDescending(t => t.MenuCode).ToList() : listResult.OrderBy(t => t.MenuCode).ToList();
                        break;
                    default:
                        listResult = listResult.OrderByDescending(t => t.CREATED).ToList();
                        break;
                }
                return listResult;
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new List<JCXX_AppMenu>();
            }
        }

        /// <summary>
        /// 查询条件筛选
        /// </summary>
        /// <param name="list"></param>
        /// <param name="query"></param>
        private void SetQuery(IDbEntity<JCXX_AppMenu> list, Query_AppMenu query)
        {
            list = list.Where(t => t.IsDelete.Eq(0));
            if (!string.IsNullOrEmpty(query.AppID))
                list = list.And(t => t.AppID.Eq(query.AppID));
            if (!string.IsNullOrEmpty(query.MenuCode))
                list = list.And(t => t.MenuCode.Eq(query.MenuCode));
            if (!string.IsNullOrEmpty(query.Like_MenuName))
                list = list.And(t => t.MenuName.Like("%"+query.Like_MenuName+"%"));
            if (!string.IsNullOrEmpty(query.Like_MenuCode))
                list = list.And(t => t.MenuCode.Like("%" + query.Like_MenuCode + "%"));
            if (query.Enabled != null)
                list = list.And(t => t.Enabled.Eq((int)query.Enabled));

        }



        /// <summary>
        /// 递归本级和下级菜单
        /// </summary>
        /// <param name="menuList"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public List<JCXX_AppMenu> GetChildList(List<JCXX_AppMenu> menuList, string parentID)
        {
            var resultList = new List<JCXX_AppMenu>();
            var parentModel = new JCXX_AppMenu();
            parentModel = menuList.Where(d => d.ID == parentID).FirstOrDefault();
            if (parentModel != null)
            {
                resultList.Add(parentModel);
                var childList = menuList.Where(d => d.ParentID == parentID);
                if (childList.Count() > 0)
                {
                    foreach (var childModel in childList)
                    {
                        resultList.AddRange(GetChildList(menuList, childModel.ID));
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
        public List<EasyuiTreeNode> GetTreeList(Query_AppMenu query)
        {
            List<EasyuiTreeNode> listResut = new List<EasyuiTreeNode>();
            List<JCXX_AppMenu> listAll = GetList(query);
            List<JCXX_AppMenu> listParent = listAll.Where(t => string.IsNullOrEmpty(t.ParentID)).OrderBy(t => t.MenuCode).ToList();
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
            foreach (JCXX_AppMenu parent in listParent)
            {
                EasyuiTreeNode parentNode = new EasyuiTreeNode()
                {
                    text = parent.MenuName,
                    id = parent.ID,
                    parentid = null,
                    children = new List<EasyuiTreeNode>()
                };
                setSubTreeList(parentNode, listAll);
                listResut.Add(parentNode);
            }
            return listResut;
        }
        void setSubTreeList(EasyuiTreeNode ParentNode, List<JCXX_AppMenu> listAll)
        {
            foreach (JCXX_AppMenu child in listAll.Where(t => t.ParentID == ParentNode.id).OrderBy(t => t.MenuCode))
            {
                EasyuiTreeNode childNode = new EasyuiTreeNode() { text = child.MenuName, id = child.ID, parentid = ParentNode.id, children = new List<EasyuiTreeNode>() };
                setSubTreeList(childNode, listAll);
                ParentNode.children.Add(childNode);
            }
        }

       
        public JCXX_AppMenu GetByID(string ID)
        {
            try
            {
                if (string.IsNullOrEmpty(ID))
                    return new JCXX_AppMenu();
                else
                {
                    return Respostry.Select().Where(t => t.ID.Eq(ID)).QueryFirst();
                }
            }
            catch (Exception ex)
            {
                log.WriteException(null, ex);
                return new JCXX_AppMenu();
            }
        }

        public RepModel Save(JCXX_AppMenu entity)
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
