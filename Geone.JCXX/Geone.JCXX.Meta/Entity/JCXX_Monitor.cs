using System;

namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 监控设备
    ///</summary>
    public class JCXX_Monitor : BaseEntity
    {
        ///// <summary>
        ///// 设备名称
        ///// </summary>
        public string MonitorName
        {
            get;
            set;
        }
        ///// <summary>
        ///// 设备类型
        ///// </summary>
        public string MonitorType
        {
            get;
            set;
        }
        ///// <summary>
        ///// 视频服务地址
        ///// </summary>
        public string ServiceUrl
        {
            get;
            set;
        }
        ///// <summary>
        ///// 坐标
        ///// </summary>
        public string Shape
        {
            get;
            set;
        }
        ///// <summary>
        ///// 是否带音柱 0无/1有
        ///// </summary>
        public int? IsYZ
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
