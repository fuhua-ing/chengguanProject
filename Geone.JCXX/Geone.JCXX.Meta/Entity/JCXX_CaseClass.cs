
using System;
namespace Geone.JCXX.Meta
{
    ///<summary>
    ///
    ///</summary>
    public class JCXX_CaseClass : BaseEntity
    {      
        
        ///// <summary>
        ///// 案件类型
        ///// </summary>
        public string CaseType
        {
            get;
            set;
        }
        ///// <summary>
        ///// CaseClassI
        ///// </summary>
        public string CaseClassI
        {
            get;
            set;
        }
        ///// <summary>
        ///// 案件小类
        ///// </summary>
        public string CaseClassII
        {
            get;
            set;
        }
        ///// <summary>
        ///// 案件子类
        ///// </summary>
        public string CaseClassIII
        {
            get;
            set;
        }
        ///// <summary>
        ///// 所属类型
        ///// </summary>
        public string RoleType
        {
            get;
            set;
        }
        ///// <summary>
        ///// 是否需要网格 0否 1是
        ///// </summary>
        public int? NeedGrid
        {
            get;
            set;
        }
        ///// <summary>
        ///// 配置状态：0 未配置；1 配置唯一 ；2 配置多个
        ///// </summary>
        public int SetStatus
        {
            get;
            set;
        }
        
    }
   
}