using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Geone.Utiliy.Library;
using Geone.JCXX.BLL;
using Autofac;
using Geone.Utiliy.Build;
using Autofac.Extensions.DependencyInjection;
using MagicOnion.Server;

namespace Geone.JCXX.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            config = configuration;
        }

        public IConfiguration config { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AdminModel>(config.GetSection("AdminIno"));
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


            //注入BLL层
            BLLDIRegister.Register(services);

            #region 添加依赖注入的第三方支持--Autofac

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AutofacSettings>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);

            #endregion 添加依赖注入的第三方支持--Autofac
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogWriter log, IRpcAccess access)
        {
            //跨域设置
            app.UseCors("CorsSetting");

            //全局错误
            app.UseExceptionHandler(builder => ErrorSettings.Error(builder, log));

            //静态文件支持
            app.UseStaticFiles();

            //配置初始化
            app.UseConfiguration(config, access, log);


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
