using System;

namespace Geone.JCXX.WebService.Meta.QueryEntity
{
    public class Req_DictItem
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID { get; set; }
        /// <summary>
        /// 字典主表编号
        /// </summary>
        public string CategoryCode { get; set; }
        /// <summary>
        /// 字典主表有效 0无效/1有效
        /// </summary>
        public int? CategoryEnabled { get; set; }
        /// <summary>
        /// 字典明细有效 0无效/1有效
        /// </summary>
        public int? ItemEnabled { get; set; }
    }
}
