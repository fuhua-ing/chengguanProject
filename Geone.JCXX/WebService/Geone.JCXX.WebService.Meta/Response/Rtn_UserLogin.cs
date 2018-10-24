using Geone.Utiliy.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace Geone.JCXX.WebService.Meta.Response
{
    public class Rtn_UserLogin
    {
        public Rtn_UserLogin_User Userinfo { get; set; }
        public Token Token { get; set; }
    }

    public class Rtn_UserLogin_User
    {
        public string ID { get; set; }
        public string DeptID { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string Gender { get; set; }
        public string IDNumber { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}
