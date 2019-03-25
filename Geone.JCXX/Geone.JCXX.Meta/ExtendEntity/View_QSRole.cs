namespace Geone.JCXX.Meta
{
    ///<summary>
    ///
    ///</summary>
    public class View_QSRole : JCXX_QSRole
    {
        public static string GetTbVName()
        {
            return "View_QSRole";
        }

        public string RoleTypeDesc { get; set; }
        public int? RoleCount { get; set; }
    }
}