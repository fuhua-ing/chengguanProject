namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 数据字典视图
    ///</summary>
    public class View_DictItem : JCXX_DictItem
    {
        public new static string GetTbName()
        {
            return "View_DictItem";
        }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID
        {
            get;
            set;
        }

        /// <summary>
        /// 应用名
        /// </summary>
        public string AppName
        {
            get;
            set;
        }

        /// <summary>
        /// 父级编码
        /// </summary>
        public string CategoryCode
        {
            get;
            set;
        }

        /// <summary>
        /// 父级名称
        /// </summary>
        public string CategoryName
        {
            get;
            set;
        }

        /// <summary>
        /// 父级是否有效
        /// </summary>
        public int? CategoryEnabled
        {
            get;
            set;
        }
    }
}