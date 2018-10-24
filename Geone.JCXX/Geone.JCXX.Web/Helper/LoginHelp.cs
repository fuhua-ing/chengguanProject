using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Geone.Utiliy.Library;
using Microsoft.Extensions.Options;

namespace Geone.JCXX.Web
{
    public static class LoginHelp
    {
        public static AdminModel GetLoginInfo()
        {
            if (HttpContext.Current.Session.GetString("CurrentUser") != null)
            {
                return JsonHelper.JsonDllDeserialize<AdminModel>(HttpContext.Current.Session.GetString("CurrentUser"));
            }
            else
            {
                return null;
            }
        }

        public static void SetLoginInfo(AdminModel adminModel)
        {
            HttpContext.Current.Session.SetString("CurrentUser", JsonHelper.JsonDllSerialize<AdminModel>(adminModel));
        }

        public static void CleanLoginInfo()
        {
            HttpContext.Current.Session.Clear();
        }
    }

    public class AdminSettingHelp
    {
        private AdminModel AdminSetting { get; set; }

        public AdminSettingHelp(IOptions<AdminModel> adminSetting)
        {
            AdminSetting = adminSetting.Value;
        }
    }

    public class AdminModel
    {
        //管理员账号
        public string Account { get; set; }
        //管理员密码
        public string Password { get; set; }
        //管理员名字
        public string UserName { get; set; }
    }
}
