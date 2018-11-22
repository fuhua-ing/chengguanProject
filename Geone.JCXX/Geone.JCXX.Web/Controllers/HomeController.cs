using Geone.Utiliy.Library;
using Microsoft.AspNetCore.Mvc;

namespace Geone.JCXX.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [LoginFilter.NoRequired]
        public ActionResult Login()
        {
            return View();
        }

        [LoginFilter.NoRequired]
        public IActionResult Error()
        {
            return View();
        }

        [LoginFilter.NoRequired]
        public string Submit_Login(string Account, string Pwd)
        {
            if (AppConfigurtaionServices.AdminConfig.Account != Account)
                return "账号不正确";
            if (AppConfigurtaionServices.AdminConfig.Password != Md5Encrypt.Encrypt(Pwd, 32))
                return "密码不正确";
            LoginHelp.SetLoginInfo(AppConfigurtaionServices.AdminConfig);
            return string.Empty;
        }

        [LoginFilter.NoRequired]
        public ActionResult Logout()
        {
            LoginHelp.CleanLoginInfo();
            return RedirectToAction("Login");
        }
    }
}