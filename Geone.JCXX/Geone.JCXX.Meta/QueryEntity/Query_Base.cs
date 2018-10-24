using Geone.Utiliy.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public abstract class Query_Base : IEntity
    {
        public int? page { get; set; }
        public int? rows { get; set; }
        public string sort { get; set; }
        public string order { get; set; }

        public int? Enabled { get; set; }
    }
}
