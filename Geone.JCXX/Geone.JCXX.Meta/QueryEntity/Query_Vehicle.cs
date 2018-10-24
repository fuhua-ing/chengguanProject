using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public class Query_Vehicle : Query_Base
    {
        public string DeptID { get; set; }
        public string CarNo { get; set; }
        public string VehicleType { get; set; }
        public string CarType { get; set; }
        public string GPRS { get; set; }

        public string Like_CarNo { get; set; }
        public string Like_GPRS { get; set; }
    }
}
