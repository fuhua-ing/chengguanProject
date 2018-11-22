namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 应用菜单
    ///</summary>
    public class JCXX_AppMenu : BaseEntity
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
        ///// 上级ID
        ///// </summary>
        public string ParentID
        {
            get;
            set;
        }

        ///// <summary>
        ///// 菜单名称
        ///// </summary>
        public string MenuName
        {
            get;
            set;
        }

        ///// <summary>
        ///// 菜单编号
        ///// </summary>
        public string MenuCode
        {
            get;
            set;
        }

        ///// <summary>
        ///// 路径地址
        ///// </summary>
        public string MenuUrl
        {
            get;
            set;
        }

        ///// <summary>
        ///// 图标编号
        ///// </summary>
        public string Icon
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