namespace Geone.JCXX.WebService.Meta.QueryEntity
{
    public class Req_Grid
    {
        /// <summary>
        /// 网格ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 网格类型
        /// </summary>
        public string GridType { get; set; }

        /// <summary>
        /// 网格编号（模糊查询）
        /// </summary>
        public string Like_GridCode { get; set; }

        /// <summary>
        /// 网格名称（模糊查询）
        /// </summary>
        public string Like_GridName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Point { get; set; }

        /// <summary>
        /// 网格有效性 0无效/1有效
        /// </summary>
        public int? Enabled { get; set; }
    }
}