using Geone.Utiliy.Database;
using System;

namespace Geone.JCXX.Meta
{
    /// <summary>
    /// 网格动态配置参数表
    /// </summary>
    public class JCXX_Grid_Config : IEntity
    {
        public static string GetTbName()
        {
            return "JCXX_Grid_Config";
        }
        [PrimaryKey]
        ///// <summary>
        ///// GUID
        ///// </summary>
        public string ID
        {
            get;
            set;
        }

        ///// <summary>
        ///// 网格ID
        ///// </summary>
        public string GridID
        {
            get;
            set;
        }

        ///// <summary>
        ///// 动态配置json
        ///// </summary>
        public string Config
        {
            get;
            set;
        }
        ///// <summary>
        ///// 创建时间
        ///// </summary>
        public DateTime? CREATED
        {
            get;
            set;
        }

        ///// <summary>
        ///// 创建人
        ///// </summary>
        public string CREATED_MAN
        {
            get;
            set;
        }

        ///// <summary>
        ///// 更新时间
        ///// </summary>
        public DateTime? UPDATED
        {
            get;
            set;
        }

        ///// <summary>
        ///// 更新人
        ///// </summary>
        public string UPDATED_MAN
        {
            get;
            set;
        }

    }
}
