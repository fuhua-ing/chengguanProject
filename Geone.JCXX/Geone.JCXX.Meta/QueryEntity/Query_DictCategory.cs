namespace Geone.JCXX.Meta
{
    public class Query_DictCategory : Query_Base
    {
        public string AppID { get; set; }
        public string CategoryCode { get; set; }
        public string Like_CategoryCode { get; set; }
        public string Like_CategoryName { get; set; }
    }
}