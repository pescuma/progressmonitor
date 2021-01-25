using System;

namespace org.pescuma.progressmonitor.console.widget
{
    public abstract class ConsoleWidget
    {
        public abstract void Started();

        public abstract AcceptableSizes ComputeSize(int current, int total, double percent, string[] stepName);

        public abstract void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName);

        // Implicit conversions

        public static implicit operator ConsoleWidget(string text)
        {
            return new TextWidget(text);
        }
    }
}
