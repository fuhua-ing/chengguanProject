using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public class JCXX_GridQSRoleTree : BaseEntity
    {
        public static string GetTbName()
        {
            return "JCXX_GridQSRoleTree";
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
        ///// 角色Name
        ///// </summary>
        public string RoleName
        {
            get;
            set;
        }

        ///// <summary>
        ///// 上级角色ID
        ///// </summary>
        public string RoleParentID
        {
            get;
            set;
        }

        ///// <summary>
        ///// 网格ID
        ///// </summary>
        public string GridID
        {
            get;
            set;
        }
    }
}
