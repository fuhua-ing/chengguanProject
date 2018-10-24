using System;

namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 数据字典主表
    ///</summary>
    public class JCXX_DictCategory : BaseEntity
    {
        ///// <summary>
        ///// 应用ID
        ///// </summary>
        public string AppID
        {
            get;
            set;
        }
        ///// <summary>
        ///// 字典类型编号
        ///// </summary>
        public string CategoryCode
        {
            get;
            set;
        }
        ///// <summary>
        ///// 字典类型名称
        ///// </summary>
        public string CategoryName
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