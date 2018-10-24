using Autofac;
using Geone.Utiliy.Component;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;
using Geone.Utiliy.Redis;

namespace Geone.Utiliy.Build
{
    /// <summary>
    /// Autofac依赖注入设定
    /// </summary>
    public class AutofacSettings : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Add(builder);
        }

        public static IContainer Build()
        {
            var builder = new ContainerBuilder();
            Add(builder);
            IContainer container = builder.Build();
            return container;
        }

        public static void Add(ContainerBuilder builder)
        {
            //文件记录
            builder.RegisterType<FileLogRecord>().As<ILogRecord>().SingleInstance();
            builder.RegisterType<LogWriter>().As<ILogWriter>().SingleInstance();
            builder.RegisterType<FileReadWrite>().As<IFileReadWrite>().SingleInstance();
            builder.RegisterType<JsonFileAccess>().As<IJsonFileAccess>().SingleInstance();

            //服务访问
            builder.RegisterType<RpcAccess>().As<IRpcAccess>().SingleInstance();
            builder.RegisterType<HttpAccess>().As<IHttpAccess>().SingleInstance();

            //关系数据库操作
            builder.RegisterType<DbConfigAccess>().As<IDbAccess>().SingleInstance();
            builder.RegisterType<SQLServerConnect>().As<IDbConnect>().SingleInstance();
            builder.RegisterType<DbAction>().As<IDbAction>().InstancePerDependency();
            builder.RegisterGeneric(typeof(DbEntity<>)).As(typeof(IDbEntity<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(DbLambda<>)).As(typeof(IDbLambda<>)).InstancePerDependency();

            //Redis数据库操作
            builder.RegisterType<RedisConnection>().As<IRedisConnection>().SingleInstance();
            builder.RegisterType<RedisConfigAccess>().As<IRedisAccess>().SingleInstance();
            builder.RegisterType<RedisConnect>().As<IRedisConnect>().SingleInstance();
            builder.RegisterType<RedisAction>().As<IRedisAction>().InstancePerDependency();

            //发布订阅操作
            builder.RegisterType<QueueAction>().As<IQueueAction>().InstancePerDependency();
            builder.RegisterType<QueueRunning>().As<IQueueRunning>().SingleInstance();

            //配置文件操作
            builder.RegisterType<ConfigExce>().As<IConfigExce>().SingleInstance();
            builder.RegisterType<RpcConfigAction>().As<IConfigAction>().SingleInstance();

            //注册服务操作
            builder.RegisterType<SrvBus>().As<ISrvBus>().InstancePerDependency();
            builder.RegisterType<SrvEvent>().As<ISrvEvent>().InstancePerDependency();

            //组件
            builder.RegisterType<RunningBlocking>().As<IRunningBlocking>().InstancePerDependency();
            builder.RegisterType<RunningPubSub>().As<IRunningPubSub>().InstancePerDependency();
            builder.RegisterType<RunningTimer>().As<IRunningTimer>().InstancePerDependency();

            //验证
            builder.RegisterType<GeneralSignIn>().As<ISignIn>().SingleInstance();
            builder.RegisterType<GeneralRegister>().As<IRegister>().SingleInstance();
        }
    }
}