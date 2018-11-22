using Autofac;
using Geone.Utiliy.Build;
using Geone.Utiliy.Library;
using Geone.Utiliy.Logger;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Responses.Negotiation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace Geone.JCXX.WebService
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        private readonly ILogWriter _log;
        private readonly IServiceCollection _services;

        public Bootstrapper(IServiceCollection services, ILogWriter log)
        {
            _log = log;
            _services = services;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            //全局错误钩子
            pipelines.OnError += (ctx, ex) => Error(ctx, ex);

            //响应拦截设置-BeforeRequest
            pipelines.BeforeRequest.AddItemToStartOfPipeline(x => AddItemToStartOfPipeline(x));

            //响应拦截设置-AfterRequest
            pipelines.AfterRequest.AddItemToEndOfPipeline(x => AddItemToEndOfPipeline(x));
        }

        #region //上下文设置

        //全局前置钩子
        public dynamic AddItemToStartOfPipeline(NancyContext context)
        {
            //do sth.
            return context.Response;
        }

        //全局后置钩子
        public dynamic AddItemToEndOfPipeline(NancyContext context)
        {
            //do sth.
            AddHeaders(context);

            return context.Response;
        }

        //全局异常信息 ErrorAssist HeaderAssist
        public dynamic Error(NancyContext context, Exception ex)
        {
            context.Response = RepModel.ErrorAsJson(_log.WriteNancyException(ex, context, "发生内部错误，请查阅日志。"));
            AddHeaders(context);

            return context.Response;
        }

        //添加通用头部
        public static void AddHeaders(NancyContext context)
        {
            #region 跨域

            //context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //context.Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS");
            //context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, Accept, X-Requested-With, Content-Type, Authorization, Token, Ticket, AppId, Identity");

            #endregion 跨域

            context.Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        }

        #endregion //上下文设置

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            var builder = InitBuilder.Builder(_services, (cbuilder) =>
            {
                cbuilder.RegisterType<IndIdentity>().As<IIndIdentity>().SingleInstance();
                //业务逻辑注入
                cbuilder.RegisterType<UserBLLL>().As<IUserService>().SingleInstance();
                cbuilder.RegisterType<DataBLL>().As<IDataService>().SingleInstance();
            });

            builder.Update(existingContainer.ComponentRegistry);
        }
    }

    /// <summary>
    /// 使用Newtonsoft.Json 替换Nancy默认的序列化方式，自定义JsonSerializer
    /// 解决大小写问题
    /// </summary>
    public class CustomJsonNetSerializer : JsonSerializer, ISerializer
    {
        public CustomJsonNetSerializer()
        {
            ContractResolver = new DefaultContractResolver();//不更改元数据的key的大小写
            DateFormatHandling = DateFormatHandling.IsoDateFormat;
            DateFormatString = "yyyy-MM-dd";
            //NullValueHandling = NullValueHandling.Ignore;//不包含属性的默认值序列化
            //ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//忽略循环引用
        }

        public bool CanSerialize(MediaRange mediaRange)
        {
            return mediaRange.ToString().Equals("application/json", StringComparison.OrdinalIgnoreCase);//忽略大小写
        }

        public void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream)
        {
            using (var streamWriter = new StreamWriter(outputStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                Serialize(jsonWriter, model);
            }
        }

        public IEnumerable<string> Extensions { get { yield return "json"; } }
    }
}