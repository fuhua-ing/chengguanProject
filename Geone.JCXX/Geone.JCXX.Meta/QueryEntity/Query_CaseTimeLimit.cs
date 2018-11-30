using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta
{
    public class Query_CaseTimeLimit : Query_Base
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public string Like_NodeTypeDesc { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public new string Enabled { get; set; }

    }
}
