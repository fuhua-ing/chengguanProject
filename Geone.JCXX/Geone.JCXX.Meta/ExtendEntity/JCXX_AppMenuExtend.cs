using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public class JCXX_AppMenuExtend : JCXX_AppMenu
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName
        {
            get;
            set;
        }
        /// <summary>
        /// 是否属于当前角色
        /// </summary>
        public int? IsChecked { get; set; }
    }
}
