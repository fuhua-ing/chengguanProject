using Autofac;
using Autofac.Extensions.DependencyInjection;
using Geone.Utiliy.Build;
using Geone.Utiliy.Library;
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
            config = configuration;
        }

        public IConfiguration config { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //services.AddOptions();
            ////使用IMvcBuilder 配置Json序列化处理
            //services.AddMvc()
            //    .AddJsonOptions(options =>
            //    {
            //        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //        options.SerializerSettings.DateFormatString = "yyyy-MM-dd ";
            //    });

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
                    builder => CorsSettings.AddCors(builder));
            });

            #endregion 添加跨域和响应头设置

            #region 添加依赖注入的第三方支持--Autofac

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AutofacSettings>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);

            #endregion 添加依赖注入的第三方支持--Autofac
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogRecord record, ILogWriter log, IRpcAccess access, ISignIn _signin, IRegister _register)
        {
            //跨域设置
            app.UseCors("CorsSetting");

            //全局错误
            app.UseExceptionHandler(builder => ErrorSettings.Error(builder, log));

            //静态文件支持
            app.UseStaticFiles();

            //配置初始化
            app.UseConfiguration(config, access, log);

            //日志查询
            app.UseLog(record);

            //token验证
            app.UseTokenIdentity(log);
            app.UseUserIdentity(_signin, _register, log);

            //启用Rpc
            app.UseRpc(access, log);

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
            app.UseOwin(x => x.UseNancy(options => options.Bootstrapper = new Bootstrapper(log)));
        }
    }
}