namespace Geone.JCXX.Meta
{
    /// <summary>
    /// 短信服务参数管理Model
    /// </summary>
    public class JCXX_Config_SMS : BaseEntity
    {
        /// <summary>
        /// 分配的唯一编号
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 系统标志
        /// </summary>
        public string SystemID { get; set; }

        // <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        // <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        // <summary>
        /// 状态
        /// </summary>
        public string Sms_Status { get; set; }

        // <summary>
        /// 优先级别
        /// </summary>
        public string Precedence { get; set; }

        // <summary>
        /// 发送结果请求
        /// </summary>
        public string DeliveryResultRequest { get; set; }

        // <summary>
        /// 备注说明
        /// </summary>
        public string Note { get; set; }
    }
}
