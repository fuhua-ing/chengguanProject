using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public class JCXX_AppRoleExtend : JCXX_AppRole
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
        /// 用户个数
        /// </summary>
        public int? UserCount
        {
            get;
            set;
        }
    }
}
