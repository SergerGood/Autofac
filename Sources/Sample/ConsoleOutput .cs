using System;

namespace Sample
{
    public class ConsoleOutput : IOutput
    {
        public void Write(string content)
        {
            Console.WriteLine(content);
        }
    }
}
