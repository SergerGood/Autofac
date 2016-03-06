using System;

namespace Sample
{
    public class TodayWriter : IDateWriter
    {
        private IOutput _output;
        public TodayWriter(IOutput output)
        {
            this._output = output;
        }

        public void WriteDate()
        {
            this._output.Write(DateTime.Today.ToShortDateString());
        }
    }
}
