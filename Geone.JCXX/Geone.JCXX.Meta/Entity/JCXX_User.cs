using Geone.Utiliy.Database;
using System;

namespace Geone.JCXX.Meta
{
    ///<summary>
    /// 用户
    ///</summary>
    public class JCXX_User : BaseEntity
    {
        public static string GetTbName()
        {
            return "JCXX_User";
        }

        ///// <summary>
        ///// 部门ID
        ///// </summary>
        public string DeptID
        {
            get;
            set;
        }
        ///// <summary>
        ///// 人员姓名
        ///// </summary>
        public string UserName
        {
            get;
            set;
        }
        ///// <summary>
        ///// 人员编号
        ///// </summary>
        public string UserCode
        {
            get;
            set;
        }
        ///// <summary>
        ///// 登录账号
        ///// </summary>
        public string Account
        {
            get;
            set;
        }
        ///// <summary>
        ///// 登录密码
        ///// </summary>
        public string Pwd
        {
            get;
            set;
        }
        ///// <summary>
        ///// 性别
        ///// </summary>
        public string Gender
        {
            get;
            set;
        }
        ///// <summary>
        ///// 身份证号
        ///// </summary>
        public string IDNumber
        {
            get;
            set;
        }
        ///// <summary>
        ///// 联系电话
        ///// </summary>
        public string Mobile
        {
            get;
            set;
        }
        ///// <summary>
        ///// 联系邮箱
        ///// </summary>
        public string Email
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