namespace Geone.Utiliy.Library
{
    /// <summary>
    /// 应用用户信息
    /// </summary>
    public class AppIdentity
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public AppUserResult User { get; set; }
        /// <summary>
        /// 用户角色ID数组
        /// </summary>
        public string[] RoleIds { get; set; }
    }
}