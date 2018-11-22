namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 数据字典子表
    ///</summary>
    public class JCXX_DictItem : BaseEntity
    {
        public static string GetTbName()
        {
            return "JCXX_DictItem";
        }

        ///// <summary>
        ///// 数据字典类型ID
        ///// </summary>
        public string CategoryID
        {
            get;
            set;
        }

        ///// <summary>
        ///// 字典明细编号
        ///// </summary>
        public string ItemCode
        {
            get;
            set;
        }

        ///// <summary>
        ///// 字典明细名称
        ///// </summary>
        public string ItemName
        {
            get;
            set;
        }

        ///// <summary>
        ///// 备注
        ///// </summary>
        public string Note
        {
            get;
            set;
        }
    }
}