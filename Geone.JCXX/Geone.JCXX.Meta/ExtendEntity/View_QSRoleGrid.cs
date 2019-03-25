using Geone.Utiliy.Database;

namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 有效网格权属视图
    ///</summary>
    public class View_QSRoleGrid : JCXX_QSRole_Grid
    {
        public static string GetTbName()
        {
            return "View_QSRoleGrid";
        }

        public string RoleCode
        {
            get;
            set;
        }

        public string RoleName
        {
            get;
            set;
        }

        [Geo]
        public string Shape
        {
            get;
            set;
        }

        public string RoleType
        {
            get;
            set;
        }
    }
}