namespace Geone.JCXX.Meta
{
    /// <summary>
    /// 短信服务参数管理QueryModel
    /// </summary>
    public class Query_Config_SMS : Query_Base
    {
        /// <summary>
        /// 系统标志
        /// </summary>
        public string Like_SystemID { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public new string Enabled { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Like_Username { get; set; }
    }
}
