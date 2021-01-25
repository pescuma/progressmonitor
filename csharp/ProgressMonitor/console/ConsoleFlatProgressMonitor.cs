using System;
using System.IO;
using System.Text;
using org.pescuma.progressmonitor.console.widget;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
    public class ConsoleFlatProgressMonitor : BaseConsoleFlatProgressMonitor
    {
        private readonly WidgetCollection widgets;
        private bool atEndOfProgressLine;
        private int? consoleWidth;
        private bool? isOutputRedirected;

        public ConsoleFlatProgressMonitor(params ConsoleWidget[] widgets)
        {
            this.widgets = new WidgetCollection(widgets);
        }

        public int ConsoleWidth
        {
            get
            {
                if (consoleWidth != null)
                    return consoleWidth.Value;

                try
                {
                    return Console.BufferWidth - 1;
                }
                catch (IOException)
                {
                    return 79;
                }
            }
            set { consoleWidth = value; }
        }

        public bool IsOutputRedirected
        {
            get
            {
                if (isOutputRedirected != null)
                    return isOutputRedirected.Value;

                try
                {
                    isOutputRedirected = Console.IsOutputRedirected;
                }
                catch (IOException)
                {
                    isOutputRedirected = true;
                }

                return isOutputRedirected.Value;
            }
            set { isOutputRedirected = value; }
        }

        protected override void OnStart()
        {
            widgets.Started();
        }

        protected override void WriteToConsole(int current, int total, string[] stepName)
        {
            if (IsOutputRedirected && MinOutupWaitInMs > 0 && current < total)
            {
                // Really respect min output wait time
                if (LastTickCount == null || Environment.TickCount - LastTickCount < MinOutupWaitInMs)
                    return;
            }

            if (atEndOfProgressLine)
                ClearLine();

            if (current < total || IsOutputRedirected)
                OutputProgress(current, total, stepName);
        }

        private void ClearLine()
        {
            if (!IsOutputRedirected)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', ConsoleWidth));
                Console.SetCursorPosition(0, Console.CursorTop);

                atEndOfProgressLine = false;
            }
        }

        private void OutputProgress(int current, int total, string[] stepName)
        {
            var sb = new StringBuilder();
            widgets.Output(t => sb.Append(t), ConsoleWidth, current, total, Percent(current, total), stepName);
            var line = sb.ToString();

            if (IsOutputRedirected)
            {
                Console.WriteLine(line);
                atEndOfProgressLine = false;
            }
            else
            {
                Console.Write(line);
                atEndOfProgressLine = true;
            }
        }

        protected override void ReportWithColor(string message, object[] args, ConsoleColor? color)
        {
            bool wasShowingProgress = atEndOfProgressLine;

            if (wasShowingProgress)
                ClearLine();

            Utils.ConsoleWriteLine(color, message, args);

            if (wasShowingProgress)
                OutputProgress(LastCurrent, LastTotal, LastStepName);
        }
    }
}
