using Autofac;
using Geone.Utiliy.Database;
using Geone.Utiliy.Library;

namespace Geone.Utiliy.Build
{
    public class AutofacOnce
    {
        public static IContainer SimpleDb()
        {
            var builder = new ContainerBuilder();
            //日志记录
            builder.RegisterType<FileLogRecord>().As<ILogRecord>().SingleInstance();
            builder.RegisterType<LogWriter>().As<ILogWriter>().SingleInstance();

            //关系数据库操作--简单注入
            builder.RegisterType<DbSimpleAccess>().As<IDbAccess>().SingleInstance();
            builder.RegisterType<SQLServerConnect>().As<IDbConnect>().SingleInstance();
            builder.RegisterType<DbAction>().As<IDbAction>().InstancePerDependency();
            builder.RegisterGeneric(typeof(DbEntity<>)).As(typeof(IDbEntity<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(DbLambda<>)).As(typeof(IDbLambda<>)).InstancePerDependency();
            IContainer container = builder.Build();
            return container;
        }
    }
}