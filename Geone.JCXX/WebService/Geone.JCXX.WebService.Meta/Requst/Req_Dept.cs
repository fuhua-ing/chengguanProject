namespace Geone.JCXX.WebService.Meta.QueryEntity
{
    public class Req_Dept
    {
        /// <summary>
        /// 部门类型
        /// </summary>
        public string DeptType { get; set; }

        /// <summary>
        /// 上级部门ID
        /// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// 部门编号（模糊查询）
        /// </summary>
        public string Like_DeptCode { get; set; }

        /// <summary>
        /// 部门名称（模糊查询）
        /// </summary>
        public string Like_DeptName { get; set; }

        /// <summary>
        /// 部门有效性 0无效/1有效
        /// </summary>
        public int? Enabled { get; set; }
    }
}