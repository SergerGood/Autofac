using System;
using Autofac;
using System.IO;
using System.Reflection;

namespace Sample
{
    class Program
    {
        public static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConsoleOutput>()
                .As<IOutput>();

            builder.RegisterType<TodayWriter>()
                .As<IDateWriter>();

            builder.RegisterType<MyComponent>()
                .UsingConstructor(typeof(ILogger), typeof(IConfigReader));

            var output = new StringWriter();
            builder.RegisterInstance(output)
                .As<TextWriter>()
                .ExternallyOwned();

            //Property Injection
            builder.Register(c => new A())
                .OnActivated(e =>
                e.Instance.MyB = e.Context.ResolveOptional<B>());

            //selection of an Implementation by Parameter Value
            builder.Register<CreditCard>(
                (c, p) =>
                {
                    var accountId = p.Named<string>("accountId");
                    if (accountId.StartsWith("9"))
                    {
                        return new GoldCard(accountId);
                    }
                    else
                    {
                        return new StandartCard(accountId);
                    }
                });

            //Generic Components
            builder.RegisterGeneric(typeof(NHibernateRepository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();

            //Default Registrations
            builder.RegisterType<ConsoleLogger>().As<ILogger>();
            builder.RegisterType<FileLogger>().As<ILogger>().PreserveExistingDefaults();

            //Assembly Scanning
            var dataAccess = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(dataAccess)
                .Where(x => x.Name.EndsWith("Repository"))
                .Except<MyUnwantedType>(x => x.As<ISpecial>().SingleInstance())
                .AsImplementedInterfaces();


            Container = builder.Build();

            var card = Container.Resolve<CreditCard>(new NamedParameter("accountId", "12345"));
            var task = Container.Resolve<IRepository<Task>>();
            var logger = Container.Resolve<ILogger>();
            var a = Container.Resolve<A>();

            WriteDate();
        }

        private static void WriteDate()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var writer = scope.Resolve<IDateWriter>();
                writer.WriteDate();
            }
        }
    }
}
