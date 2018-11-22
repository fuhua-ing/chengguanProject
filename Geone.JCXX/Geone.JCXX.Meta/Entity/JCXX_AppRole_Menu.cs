using Geone.Utiliy.Database;
using System;

namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 角色-菜单-关系
    ///</summary>
    public class JCXX_AppRole_Menu : IEntity
    {
        ///// <summary>
        ///// GUID
        ///// </summary>
        public string ID
        {
            get;
            set;
        }

        ///// <summary>
        ///// 角色ID
        ///// </summary>
        public string RoleID
        {
            get;
            set;
        }

        ///// <summary>
        ///// 菜单ID
        ///// </summary>
        public string MenuID
        {
            get;
            set;
        }

        ///// <summary>
        ///// 创建时间
        ///// </summary>
        public DateTime? CREATED
        {
            get;
            set;
        }

        ///// <summary>
        ///// 创建人
        ///// </summary>
        public string CREATED_MAN
        {
            get;
            set;
        }

        ///// <summary>
        ///// 更新时间
        ///// </summary>
        public DateTime? UPDATED
        {
            get;
            set;
        }

        ///// <summary>
        ///// 更新人
        ///// </summary>
        public string UPDATED_MAN
        {
            get;
            set;
        }
    }
}