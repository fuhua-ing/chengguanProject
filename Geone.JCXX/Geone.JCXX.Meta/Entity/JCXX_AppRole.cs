namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 系统角色
    ///</summary>
    public class JCXX_AppRole : BaseEntity
    {
        ///// <summary>
        ///// 应用ID
        ///// </summary>
        public string AppID
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