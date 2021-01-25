using System;
using System.Linq;

namespace org.pescuma.progressmonitor.console.widget
{
    public class StepNameWidget : ConsoleWidget
    {
        private readonly Func<string[], string> formater;

        public StepNameWidget(Func<string[], string> formater = null)
        {
            this.formater = formater ?? DefaultFormater;
        }

        public override void Started()
        {
        }

        public override AcceptableSizes ComputeSize(int current, int total, double percent, string[] stepName)
        {
            string text = formater(stepName) ?? "";

            return new AcceptableSizes(3, text.Length, false);
        }

        public override void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
        {
            string text = formater(stepName) ?? "";

            if (text.Length > width)
            {
                if (width >= 5)
                    text = text.Substring(0, width - 1) + "\u2026";
                else
                    text = text.Substring(0, width);
            }

            writer(text);
        }

        private static string DefaultFormater(string[] name)
        {
            if (name == null)
                return "";

            return string.Join(" - ", name.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}
