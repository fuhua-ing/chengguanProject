using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public class Query_Dept : Query_Base
    {
        public string DeptName { get; set; }
        public string DeptCode { get; set; }
        public string ParentID { get; set; }
        public string ChoiceAll { get; set; }

        public string Like_DeptName { get; set; }
        public string Like_DeptCode { get; set; }
    }
}
