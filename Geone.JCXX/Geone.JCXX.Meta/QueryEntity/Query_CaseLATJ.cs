using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public class Query_CaseLATJ : Query_Base
    {
        /// <summary>
        /// 立案条件描述
        /// </summary>
        public string Like_CaseConditionDesc { get; set; }

        /// <summary>
        /// 时限描述
        /// </summary>
        public string Like_TimeLimitDesc { get; set; }

        /// <summary>
        /// 案件大类
        /// </summary>
        public string CaseClassI { get; set; }

        /// <summary>
        /// 案件小类
        /// </summary>
        public string CaseClassII { get; set; }
    }
}
