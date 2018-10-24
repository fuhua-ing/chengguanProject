using System;
using System.Collections.Generic;
using System.Text;
using Geone.JCXX.Meta;

namespace Geone.JCXX.Meta
{
    public class JCXX_DictItemExtend : JCXX_DictItem
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID
        {
            get;
            set;
        }
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName
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
    }
}
