using Geone.Utiliy.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class BaseEntity: IEntity
    {
       
        ///// <summary>
        ///// GUID
        ///// </summary>
        [PrimaryKey]
        public string ID
        {
            get;
            set;
        }
        ///// <summary>
        ///// 是否删除
        ///// </summary>
        public int? IsDelete
        {
            get;
            set;
        }
        ///// <summary>
        ///// 是否有效
        ///// </summary>
        public int? Enabled
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
