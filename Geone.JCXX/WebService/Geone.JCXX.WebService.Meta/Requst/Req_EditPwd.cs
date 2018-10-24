using System;

namespace Geone.JCXX.WebService.Meta.QueryEntity
{
    public class Req_EditPwd
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 原密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; }
    }
}
