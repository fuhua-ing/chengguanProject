namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 车辆信息
    ///</summary>
    public class View_Vehicle : JCXX_Vehicle
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName
        {
            get;
            set;
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string VehicleTypeDesc
        {
            get;
            set;
        }

        /// <summary>
        /// 车辆类型
        /// </summary>
        public string CarTypeDesc
        {
            get;
            set;
        }

        /// <summary>
        /// 部门父类ID
        /// </summary>
        public string ParentID
        {
            get;
            set;
        }
    }
}