using System;
using Autofac;

namespace Sample
{
    internal class StartupMessageWriter : IStartable
    {
        public void Start()
        {
            Console.WriteLine("App is starting up!");
        }
    }
}