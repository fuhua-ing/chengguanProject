using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public class Query_QSRoleUser : Query_Base
    {
        public string AppID { get; set; }
        public string RoleID { get; set; }
        public string UserID { get; set; }
        public string DeptID { get; set; }
    }
}
