using Autofac.Extensions.DependencyInjection;
using Geone.JCXX.BLL;
using Geone.Utiliy.Build;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using MagicOnion.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Geone.JCXX.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Config = configuration;
        }

        public IServiceCollection Service { get; set; }
        public IConfiguration Config { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            #region 基础注入

            services.AddOptions();
            services.Configure<AdminModel>(Config.GetSection("AdminIno"));
            //全局配置Json序列化处理
            services.AddMvc().AddJsonOptions(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                ////设置时间格式
                //options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
            });
            services.AddMvc(options =>
            {
                options.Filters.Add<LoginFilter>();
            });

            //注册session服务
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddSession(options => options.IdleTimeout = TimeSpan.FromSeconds(10));
            services.AddSession(options =>
                options.IdleTimeout = TimeSpan.FromMinutes(Convert.ToDouble(AppConfigurtaionServices.Configuration["SessionTimeOut"]))
            );
            services.AddHttpContextAccessor();

            #endregion 基础注入

            #region 开启远程调用支持

            var MagicOnionService = MagicOnionEngine.BuildServerServiceDefinition(new MagicOnionOptions(true)
            {
                MagicOnionLogger = new MagicOnionLogToGrpcLogger()
            });

            services.Add(new ServiceDescriptor(typeof(MagicOnionServiceDefinition), MagicOnionService));

            #endregion 开启远程调用支持

            #region 添加跨域和响应头设置

            services.AddCors(options =>
            {
                options.AddPolicy("CorsSetting",
                    corsBuilder => CorsSettings.AddCors(corsBuilder));
            });

            #endregion 添加跨域和响应头设置

            //注入BLL层
            BLLDIRegister.Register(services);

            #region 添加依赖注入的第三方支持--Autofac

            Service = services;
            var builder = InitBuilder.MockBuilder(services);
            return new AutofacServiceProvider(builder.Build());

            #endregion 添加依赖注入的第三方支持--Autofac
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogWriter log, IRpcAccess access, IConfigTool tool)
        {
            //跨域设置
            app.UseCors("CorsSetting");

            //全局错误
            app.UseExceptionHandler(handler => handler.Error(log));

            //静态文件支持
            app.UseStaticFiles();

            //健康监控
            app.UseHealth();

            //日志查询
            app.UseLog(tool);

            //使用session
            app.UseSession();
            app.UseStaticHttpContext();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Login}/{id?}");
            });
        }
    }
}