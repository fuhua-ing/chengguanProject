using System;
namespace Geone.JCXX.Meta
{
    ///<summary>
    ///
    ///</summary>
    public class JCXX_Vehicle : BaseEntity
    {

        ///// <summary>
        ///// 部门ID
        ///// </summary>
        public string DeptID
        {
            get;
            set;
        }
        ///// <summary>
        ///// 车牌号
        ///// </summary>
        public string CarNo
        {
            get;
            set;
        }
        ///// <summary>
        ///// 设备类型
        ///// </summary>
        public string VehicleType
        {
            get;
            set;
        }
        ///// <summary>
        ///// 车辆类型
        ///// </summary>
        public string CarType
        {
            get;
            set;
        }
        ///// <summary>
        ///// 发动机编号
        ///// </summary>
        public string EngineNo
        {
            get;
            set;
        }
        ///// <summary>
        ///// 车辆识别代码	
        ///// </summary>
        public string FrameNo
        {
            get;
            set;
        }
        ///// <summary>
        ///// 发证日期
        ///// </summary>
        public DateTime ? RegDate
        {
            get;
            set;
        }
        ///// <summary>
        ///// 年检合格期
        ///// </summary>
        public DateTime ? DueDate
        {
            get;
            set;
        }
        ///// <summary>
        ///// GPRS号码
        ///// </summary>
        public string GPRS
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