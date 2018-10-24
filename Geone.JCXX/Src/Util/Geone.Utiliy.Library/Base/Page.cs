using System.Collections.Generic;

namespace Geone.Utiliy.Library
{
    public class Page
    {
        public int Total { get; set; }
        public dynamic Rows { get; set; }
    }

    public class Page<T>
    {
        public int Total { get; set; }
        public List<T> Rows { get; set; }
    }
}