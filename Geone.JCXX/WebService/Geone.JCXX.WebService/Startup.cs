using Autofac;
using Autofac.Extensions.DependencyInjection;
using Geone.Utiliy.Build;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using MagicOnion.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Owin;
using System;

namespace Geone.JCXX.WebService
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

            #region 添加依赖注入的第三方支持--Autofac

            Service = services;
            var builder = InitBuilder.Builder(services, (cbuilder) =>
            {
                cbuilder.RegisterType<IndIdentity>().As<IIndIdentity>().SingleInstance();
                //业务逻辑注入
                cbuilder.RegisterType<UserBLLL>().As<IUserService>().SingleInstance();
                cbuilder.RegisterType<DataBLL>().As<IDataService>().SingleInstance();
            });
            return new AutofacServiceProvider(builder.Build());

            #endregion 添加依赖注入的第三方支持--Autofac
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogWriter log, IConfigTool ctool, IIdentityTool itool, IRpcAccess access, IIndIdentity indIdentity)
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
            app.UseLog(ctool);

            //远程调用初始化--Rpc服务
            app.UseRpc(ctool, access, log);

            //身份验证
            app.UseIndTokenIdentity(ctool, itool, indIdentity, log);

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //Http-Nancy
            app.UseOwin(x => x.UseNancy(options => options.Bootstrapper = new Bootstrapper(Service, log)));
        }
    }
}