using System;

namespace Sample.ResolvingServices
{
    public class ALazy
    {
        private readonly Lazy<BDependency> _b;

        public ALazy(Lazy<BDependency> b)
        {
            _b = b;
        }

        public void M()
        {
            // The component implementing B is created the
            // first time M() is called
            _b.Value.DoSomething();
        }
    }
}