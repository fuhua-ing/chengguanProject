using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geone.JCXX.Web
{
    public class LoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isDefined = false;
            var controllerActionDescriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                isDefined = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                   .Any(a => a.GetType().Equals(typeof(NoRequiredAttribute)));
            }
            if (isDefined) return;

            if (HttpContext.Current.Session.GetString("CurrentUser") == null)
            {
                bool result = false;

                var xreq = HttpContext.Current.Request.Headers.ContainsKey("x-requested-with");
                if (xreq)
                {
                    result = HttpContext.Current.Request.Headers["x-requested-with"] == "XMLHttpRequest";
                }

                if (result)
                {
                    if (HttpContext.Current.Request.Path.HasValue)
                    {
                        filterContext.Result = new ContentResult() { Content = HttpContext.Current.Request.Path.Value, ContentType = "application/x-www-form-urlencoded" };
                    }
                }
                else
                {
                    if (HttpContext.Current.Request.Path.HasValue)
                    {
                        var nowUrl = HttpContext.Current.Request.Path.Value;
                        if (nowUrl.Contains("/Home/Index"))
                        {
                            filterContext.Result = new RedirectResult("Login");
                        }
                        else
                        {
                            string script = @"function getWindow(){
                                            var w = window;
                                            while (w.parent.location.href != w.location.href) { w = w.parent; }
                                            return w;
                                        }";
                            //HttpContext.Current.Request.PathBase 获取网站基础路径
                            script += "getWindow().location.href = '"+ HttpContext.Current.Request.PathBase + "/Home/Login" + "';";
                            HttpContext.Current.Response.WriteAsync("<script>"+ script + "</script>");
                        }
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }

        //不需要登入
        public class NoRequiredAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
