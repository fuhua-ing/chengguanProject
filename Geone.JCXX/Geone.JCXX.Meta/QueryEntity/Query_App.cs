using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public class Query_App : Query_Base
    {
        public string AppCode { get; set; }

        public string Like_AppName { get; set; }
        public string Like_AppCode { get; set; }
    }
}
