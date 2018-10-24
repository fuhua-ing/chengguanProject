using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public class Query_User : Query_Base
    {
        public string DeptParentID { get; set; }
        public string DeptID { get; set; }
        public string Account { get; set; }
        public string UserName { get; set; }
        public string Like_Account { get; set; }
        public string Like_UserName { get; set; }

        public string Like_AccountOrName { get; set; }
    }
}
