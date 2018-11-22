using Geone.Utiliy.Database;
using System;

namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 立案条件
    ///</summary>
    public class JCXX_CaseLATJ : IEntity
    {
        public static string GetTbName()
        {
            return "JCXX_CaseLATJ";
        }

        ///// <summary>
        ///// 主键ID
        ///// </summary>
        [PrimaryKey]
        public string ID
        {
            get;
            set;
        }

        ///// <summary>
        ///// 案件大类
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
        ///// 立案条件
        ///// </summary>
        public string CaseCondition
        {
            get;
            set;
        }

        ///// <summary>
        ///// 立案条件描述
        ///// </summary>
        public string CaseConditionDesc
        {
            get;
            set;
        }

        ///// <summary>
        ///// 时限
        ///// </summary>
        public int? TimeLimit
        {
            get;
            set;
        }

        ///// <summary>
        ///// 时限描述
        ///// </summary>
        public string TimeLimitDesc
        {
            get;
            set;
        }

        ///// <summary>
        ///// 结案条件
        ///// </summary>
        public string JACondition
        {
            get;
            set;
        }

        ///// <summary>
        /////
        ///// </summary>
        public DateTime CREATED
        {
            get;
            set;
        }

        ///// <summary>
        /////
        ///// </summary>
        public DateTime UPDATED
        {
            get;
            set;
        }

        ///// <summary>
        /////
        ///// </summary>
        public string CREATED_MAN
        {
            get;
            set;
        }

        ///// <summary>
        /////
        ///// </summary>
        public string UPDATED_MAN
        {
            get;
            set;
        }
    }
}