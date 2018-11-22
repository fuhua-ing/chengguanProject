namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 部门
    ///</summary>
    public class JCXX_Dept : BaseEntity
    {
        public static string GetTbName()
        {
            return "JCXX_Dept";
        }

        ///// <summary>
        ///// 上级ID
        ///// </summary>
        public string ParentID
        {
            get;
            set;
        }

        ///// <summary>
        ///// 部门名称
        ///// </summary>
        public string DeptName
        {
            get;
            set;
        }

        ///// <summary>
        ///// 部门编号
        ///// </summary>
        public string DeptCode
        {
            get;
            set;
        }

        ///// <summary>
        ///// 简称
        ///// </summary>
        public string ShortName
        {
            get;
            set;
        }

        ///// <summary>
        ///// 部门性质
        ///// </summary>
        public string DeptType
        {
            get;
            set;
        }

        ///// <summary>
        ///// 负责人
        ///// </summary>
        public string Contact
        {
            get;
            set;
        }

        ///// <summary>
        ///// 联系电话
        ///// </summary>
        public string ContactTel
        {
            get;
            set;
        }

        ///// <summary>
        ///// 联系邮箱
        ///// </summary>
        public string ContactEmail
        {
            get;
            set;
        }

        ///// <summary>
        ///// 备注
        ///// </summary>
        public string Note
        {
            get;
            set;
        }
    }
}