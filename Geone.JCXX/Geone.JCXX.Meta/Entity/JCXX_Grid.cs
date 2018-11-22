using Geone.Utiliy.Database;

namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 网格
    ///</summary>
    public class JCXX_Grid : BaseEntity
    {
        public static string GetTbName()
        {
            return "JCXX_Grid";
        }

        ///// <summary>
        ///// 网格类型
        ///// </summary>
        public string GridType
        {
            get;
            set;
        }

        ///// <summary>
        ///// 网格编号
        ///// </summary>
        public string GridCode
        {
            get;
            set;
        }

        ///// <summary>
        ///// 网格名称
        ///// </summary>
        public string GridName
        {
            get;
            set;
        }

        ///// <summary>
        ///// 网格别名
        ///// </summary>
        public string ShowName
        {
            get;
            set;
        }

        ///// <summary>
        ///// 图层WKT
        ///// </summary>
        [Geo]
        public string Shape
        {
            get;
            set;
        }

        ///// <summary>
        ///// 网格面积
        ///// </summary>
        public decimal? GridArea
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