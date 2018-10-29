using Autofac;
using Geone.Utiliy.Library;
using Geone.Utiliy.Build;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy;
using Nancy.Responses.Negotiation;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using Nancy.TinyIoc;

namespace Geone.JCXX.WebService
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {

        private readonly ILogWriter _log;

        public Bootstrapper(ILogWriter log)
        {
            _log = log;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            //全局错误钩子
            pipelines.OnError += (ctx, ex) => NancySettings.Error(ctx, ex, _log);

            //响应拦截设置-BeforeRequest
            pipelines.BeforeRequest.AddItemToStartOfPipeline(x => NancySettings.AddItemToStartOfPipeline(x));

            //响应拦截设置-AfterRequest
            pipelines.AfterRequest.AddItemToEndOfPipeline(x => NancySettings.AddItemToEndOfPipeline(x));
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            var builder = new ContainerBuilder();

            //业务逻辑注入
            builder.RegisterType<UserBLLL>().As<IUserService>().SingleInstance();
            builder.RegisterType<DataBLL>().As<IDataService>().SingleInstance();

            AutofacSettings.Add(builder);
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
