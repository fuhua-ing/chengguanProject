using System.Collections.Generic;

namespace Geone.JCXX.Meta
{
    public class EasyuiTreeNode
    {
        public string id { get; set; }
        public string text { get; set; }
        public string parentid { get; set; }
        public int? ischecked { get; set; }
        public List<EasyuiTreeNode> children { get; set; }
    }

    public class EasyuiTreeNode_Dept : JCXX_Dept
    {
        public string id { get; set; }
        public string text { get; set; }
        public string parentid { get; set; }
        public int? ischecked { get; set; }
        public List<EasyuiTreeNode_Dept> children { get; set; }

        public string UserName { get; set; }
        public string Account { get; set; }
        public string Gender { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}