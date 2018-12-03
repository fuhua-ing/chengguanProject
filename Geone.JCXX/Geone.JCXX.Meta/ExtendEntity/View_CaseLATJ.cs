using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.Meta.ExtendEntity
{
    public class View_CaseLATJ : JCXX_CaseLATJ
    {
        public new static string GetTbName()
        {
            return "View_CaseLATJ";
        }

        ///// <summary>
        ///// 案件大类描述
        ///// </summary>
        public string CaseClassIDesc
        {
            get;
            set;
        }

        ///// <summary>
        ///// 案件小类描述
        ///// </summary>
        public string CaseClassIIDesc
        {
            get;
            set;
        }
    }
}
