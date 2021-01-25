using System;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console.widget
{
    public class ElapsedWidget : ConsoleWidget
    {
        private DateTime start;
        private string elapsed;

        public override void Started()
        {
            start = DateTime.Now;
        }

        public override AcceptableSizes ComputeSize(int current, int total, double percent, string[] stepName)
        {
            elapsed = Utils.FormatSeconds((int) Math.Round((DateTime.Now - start).TotalMilliseconds / 1000));

            return new AcceptableSizes(elapsed.Length, elapsed.Length, false);
        }

        public override void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
        {
            writer(elapsed);
        }
    }
}
