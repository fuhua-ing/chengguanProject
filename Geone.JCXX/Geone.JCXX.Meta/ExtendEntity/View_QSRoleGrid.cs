using Geone.Utiliy.Database;
using System;
using System.Collections.Generic;
using System.Text;


namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 有效网格权属视图
    ///</summary>
    public class View_QSRoleGrid : JCXX_QSRole_Grid
    {

        public  static string GetTbName()
        {
            return "View_QSRoleGrid";
        }
        public string RoleName
        {
            get;
            set;
        }
        [GeoAttribute]
        public string Shape {
            get;
            set;
        }
        public string RoleType
        {
            get;
            set;
        }
    }
}
