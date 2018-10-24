using Geone.Utiliy.Database;
using System;
using System.Collections.Generic;
using System.Text;


namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 有效角色菜单视图
    ///</summary>
    public class View_AppRoleMenu : IEntity
    {
        public static string GetTbName()
        {
            return "View_AppRoleMenu";
        }

        ///// <summary>
        ///// ID
        ///// </summary>
        [PrimaryKey]
        public string ID
        {
            get;
            set;
        }
       
        ///// <summary>
        ///// 角色是否有效
        ///// </summary>
        public int? RoleEnabled
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
        ///// 角色编号
        ///// </summary>
        public string RoleCode
        {
            get;
            set;
        }
        ///// <summary>
        ///// 角色名称
        ///// </summary>
        public string RoleName
        {
            get;
            set;
        }
        ///// <summary>
        ///// 菜单是否有效
        ///// </summary>
        public int? MenuEnabled
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
        ///// 菜单编号
        ///// </summary>
        public string MenuCode
        {
            get;
            set;
        }
        ///// <summary>
        ///// 菜单名称
        ///// </summary>
        public string MenuName
        {
            get;
            set;
        }
        ///// <summary>
        ///// 菜单图标
        ///// </summary>
        public string MenuIcon
        {
            get;
            set;
        }
        ///// <summary>
        ///// 菜单路径
        ///// </summary>
        public string MenuUrl
        {
            get;
            set;
        }
        ///// <summary>
        ///// 菜单上级ID
        ///// </summary>
        public string MenuParentID
        {
            get;
            set;
        }
        
        ///// <summary>
        ///// 应用是否有效
        ///// </summary>
        public int? AppEnabled
        {
            get;
            set;
        }
        ///// <summary>
        ///// 应用ID
        ///// </summary>
        public string AppID
        {
            get;
            set;
        }
        ///// <summary>
        ///// 应用编号
        ///// </summary>
        public string AppCode
        {
            get;
            set;
        }
        ///// <summary>
        ///// 应用名称
        ///// </summary>
        public string AppName
        {
            get;
            set;
        }
    }
}
