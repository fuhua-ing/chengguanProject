
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Geone.JCXX.Web
{
    /// <summary>
    /// 读取配置文件
    /// </summary>
    public class AppConfigurtaionServices
    {
        public static IConfiguration Configuration { get; set; }
        public static AdminModel AdminConfig { get; set; }

        static AppConfigurtaionServices()
        {
            //ReloadOnChange = true 当appsettings.json被修改时重新加载           
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
            var ServiceCollection = new ServiceCollection().AddOptions().Configure<AdminModel>(Configuration.GetSection("AdminInfo")).BuildServiceProvider();
            AdminConfig = ServiceCollection.GetService<IOptions<AdminModel>>().Value;
        }
    }
}
