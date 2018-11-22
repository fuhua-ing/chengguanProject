namespace Geone.JCXX.Meta
{
    ///<summary>
    ///
    ///</summary>
    public class JCXX_QSRole : BaseEntity
    {
        public static string GetTbName()
        {
            return "JCXX_QSRole";
        }

        ///// <summary>
        ///// 角色类型
        ///// </summary>
        public string RoleType
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
        ///// 备注
        ///// </summary>
        public string Note
        {
            get;
            set;
        }
    }
}