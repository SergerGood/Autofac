using Autofac;

namespace Sample
{
    class Program
    {
        public static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConsoleOutput>().As<IOutput>();
            builder.RegisterType<TodayWriter>().As<IDateWriter>();

            Container = builder.Build();
        }
    }
}
