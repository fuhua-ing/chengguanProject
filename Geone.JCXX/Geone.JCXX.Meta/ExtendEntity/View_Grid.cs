namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 有效用户角色视图
    ///</summary>
    public class View_Grid : JCXX_Grid
    {
        public new static string GetTbName()
        {
            return "View_Grid";
        }

        ///// <summary>
        ///// 网格类型描述
        ///// </summary>
        public string GridTypeDesc
        {
            get;
            set;
        }

        ///// <summary>
        ///// 权属角色数量
        ///// </summary>
        public string RoleCount
        {
            get;
            set;
        }
    }
}