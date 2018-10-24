using Geone.Utiliy.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 角色-用户-关系
    ///</summary>
    public class JCXX_AppRole_User : IEntity
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
        ///// 人员ID
        ///// </summary>
        public string UserID
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