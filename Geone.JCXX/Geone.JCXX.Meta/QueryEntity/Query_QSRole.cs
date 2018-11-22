namespace Geone.JCXX.Meta
{
    public class Query_QSRole : Query_Base
    {
        public string RoleType { get; set; }
        public string RoleCode { get; set; }

        public string Like_RoleName { get; set; }
        public string Like_RoleCode { get; set; }
    }
}