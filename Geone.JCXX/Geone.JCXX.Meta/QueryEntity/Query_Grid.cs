using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public class Query_Grid : Query_Base
    {
        public string ID { get; set; }
        public string GridType { get; set; }
        public string GridCode { get; set; }
        public string Like_GridCode { get; set; }
        public string Like_GridName { get; set; }
        
    }
}
