using Geone.Utiliy.Database;
using System;
namespace Geone.JCXX.Meta
{
    ///<summary>
    ///
    ///</summary>
    public class JCXX_QSRole_CaseClass : IEntity
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
        ///// 权属ID
        ///// </summary>
        public string CaseClassID
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