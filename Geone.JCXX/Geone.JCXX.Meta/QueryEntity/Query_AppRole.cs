namespace Geone.JCXX.Meta
{
    public class Query_AppRole : Query_Base
    {
        public string AppID { get; set; }
        public string RoleCode { get; set; }
        public string Like_RoleName { get; set; }
        public string Like_RoleCode { get; set; }
    }
}