using System;
using Autofac;
using System.IO;
using System.Reflection;
using Sample.ResolvingServices;

namespace Sample
{
    class Program
    {
        public static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            RegisteringComponents(builder);
            ResolvingServices(builder);

            builder.RegisterType<Worker>()
                .InstancePerDependency()
                .OnRelease(instance =>
                    instance.Dispose());

            //Startable Components
            builder.RegisterType<StartupMessageWriter>()
                .As<IStartable>()
                .SingleInstance();

            //This can yield an improvement of up to 10x faster Resolve()  than builder.RegisterType<Component>();
            builder.Register(c => 
            new Component());

            Container = builder.Build();


            //Adding Registrations to a Lifetime Scope
            using (var scope = Container.BeginLifetimeScope(x =>
            {
                x.RegisterType<Override>().As<IService>();
            }))
            {
                // The additional registrations will be available
                // only in this lifetime scope.
                var r = scope.Resolve<IService>();
            }

            using (var scope = Container.BeginLifetimeScope())
            {
                while (true)
                {
                    // Every one of the Worker instances
                    // resolved in this loop will be brand new.
                    // Out of memory!
                    var w = scope.Resolve<Worker>();
                    w.DoWork();
                }
            }

            var card = Container.Resolve<CreditCard>(new NamedParameter("accountId", "12345"));
            var task = Container.Resolve<IRepository<Task>>();
            var logger = Container.Resolve<ILogger>();
            var a = Container.Resolve<A>();

            using (var scope = Container.BeginLifetimeScope())
            {
                // B is automatically injected into A.
                var aDependency = scope.Resolve<ADependency>();

                //Delayed Instantiation
                var aLazy = scope.Resolve<ALazy>();
                aLazy.M();


                // Here we resolve a B that is InstancePerLifetimeScope();
                var b1 = scope.Resolve<BDependency>();
                b1.DoSomething();
                // This will be the same as b1 from above.
                var b2 = scope.Resolve<BDependency>();
                b2.DoSomething();
                // The B used in A will NOT be the same as the others.
                var aOwned = scope.Resolve<AOwned>();
                aOwned.M();
            }

            WriteDate();
        }

        private static void ResolvingServices(ContainerBuilder builder)
        {
            builder.RegisterType<ADependency>();
            builder.RegisterType<ALazy>();
            builder.RegisterType<AOwned>();
            builder.RegisterType<BDependency>();
        }

        private static void RegisteringComponents(ContainerBuilder builder)
        {
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

            //Scanning for Modules
            builder.RegisterAssemblyModules(typeof(AModule), dataAccess);
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
