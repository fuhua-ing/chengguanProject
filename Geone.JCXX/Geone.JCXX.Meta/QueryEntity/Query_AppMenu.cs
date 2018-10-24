using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public class Query_AppMenu : Query_Base
    {
        public string AppID { get; set; }
        public string MenuCode { get; set; }
        public string ParentID { get; set; }
        public string Like_MenuCode { get; set; }
        public string Like_MenuName { get; set; }

    }
}
