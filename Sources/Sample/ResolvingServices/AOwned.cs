using Autofac.Features.OwnedInstances;

namespace Sample.ResolvingServices
{
    public class AOwned
    {
        public Owned<BDependency> _b { get; set; }

        public AOwned(Owned<BDependency> b)
        {
            _b = b;
        }

        public void M()
        {
            // _b is used for some task
            _b.Value.DoSomething();

            // Here _b is no longer needed, so
            // it is released
            _b.Dispose();
        }
    }
}