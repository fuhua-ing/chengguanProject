using System;

namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 节假日
    ///</summary>
    public class JCXX_Holiday : BaseEntity
    {
        public static string GetTbName()
        {
            return "JCXX_Holiday";
        }

        ///// <summary>
        ///// 年份
        ///// </summary>
        public int YearNo
        {
            get;
            set;
        }

        ///// <summary>
        ///// 日期
        ///// </summary>
        public DateTime Holiday
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