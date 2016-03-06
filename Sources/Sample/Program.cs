using System;
using Autofac;
using System.IO;

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

            Container = builder.Build();

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
