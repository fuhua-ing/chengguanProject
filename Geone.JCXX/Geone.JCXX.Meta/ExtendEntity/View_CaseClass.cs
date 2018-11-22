namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 有效案件权属视图
    ///</summary>
    public class View_CaseClass : JCXX_CaseClass
    {
        /// <summary>
        /// 角色数量
        /// </summary>
        public int? RoleCount
        {
            get;
            set;
        }

        ///// <summary>
        ///// 类型
        ///// </summary>
        public string CaseTypeDesc
        {
            get;
            set;
        }

        ///// <summary>
        ///// 大类
        ///// </summary>
        public string CaseClassIDesc
        {
            get;
            set;
        }

        ///// <summary>
        ///// 小类
        ///// </summary>
        public string CaseClassIIDesc
        {
            get;
            set;
        }

        ///// <summary>
        ///// 子类
        ///// </summary>
        public string CaseClassIIIDesc
        {
            get;
            set;
        }

        ///// <summary>
        ///// 所属类型
        ///// </summary>
        public string RoleTypeDesc
        {
            get;
            set;
        }
    }
}