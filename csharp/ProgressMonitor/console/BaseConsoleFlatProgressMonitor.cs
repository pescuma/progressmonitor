using System;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
    public abstract class BaseConsoleFlatProgressMonitor : FlatProgressMonitor, MaxThroughputProgressMonitor
    {
        protected int? LastTickCount;
        protected int LastCurrent;
        protected int LastTotal;
        protected string[] LastStepName;
        private bool hasFinished;

        public int MinOutupWaitInMs { get; set; } = 500;

        public void SetCurrent(int current, int total, params string[] stepName)
        {
            if (current < 0 || total < 0)
                throw new ArgumentException();
            if (hasFinished)
                throw new InvalidOperationException("Alteady finished");

            var finished = (current >= total);
            var tickCount = Environment.TickCount;
            var percent = Percent(current, total);

            bool output;
            if (finished)
                output = true;
            else if (LastTickCount == null)
                output = true;
            else if (!Utils.ArrayEqual(stepName, LastStepName))
                output = true;
            else if (tickCount - LastTickCount.Value < MinOutupWaitInMs)
                output = false;
            else if (percent > Percent(LastCurrent, LastTotal))
                output = true;
            else
                output = false;

            if (!output)
                return;

            if (LastTickCount == null)
                OnStart();

            WriteToConsole(current, total, stepName);

            LastTickCount = (finished ? (int?) null : tickCount);
            LastCurrent = current;
            LastTotal = total;
            LastStepName = stepName;
            hasFinished = finished;
        }

        protected double Percent(int current, int total)
        {
            return Math.Floor(current / (double) total);
        }

        protected virtual void OnStart()
        {
        }

        protected abstract void WriteToConsole(int current, int total, string[] stepName);

        public void Report(string message, params object[] args)
        {
            ReportWithColor(message, args, null);
        }

        public void ReportDetail(string message, params object[] args)
        {
            ReportWithColor(message, args, ConsoleColor.DarkGray);
        }

        public void ReportWarning(string message, params object[] args)
        {
            ReportWithColor(message, args, ConsoleColor.DarkYellow);
        }

        public void ReportError(string message, params object[] args)
        {
            ReportWithColor(message, args, ConsoleColor.Red);
        }

        protected abstract void ReportWithColor(string message, object[] args, ConsoleColor? color);

        public bool WasCanceled { get; private set; }

        public void RequestCancel()
        {
            WasCanceled = true;
        }
    }
}
