using System;

namespace org.pescuma.progressmonitor.console.widget
{
    public class BarWidget : ConsoleWidget
    {
        public override void Started()
        {
        }

        public override AcceptableSizes ComputeSize(int current, int total, double percent, string[] stepName)
        {
            return new AcceptableSizes(5, 5, true);
        }

        public override void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
        {
            writer("[");

            var hashWidth = (int) Math.Round((width - 2) * percent);
            if (hashWidth > 0)
                writer(new string('#', hashWidth));

            var spaceWidth = width - 2 - hashWidth;
            if (spaceWidth > 0)
                writer(new string(' ', spaceWidth));

            writer("]");
        }
    }
}
