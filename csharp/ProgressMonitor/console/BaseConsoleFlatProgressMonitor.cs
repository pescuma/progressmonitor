using System;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
	public abstract class BaseConsoleFlatProgressMonitor : FlatProgressMonitor, MaxThroughputProgressMonitor
	{
		private int? lastTickCount;
		protected double LastPercent;
		protected string[] LastStepName;

		protected bool HasFinished;

		public int MinOutupWaitInMs
		{
			get { return 500; }
		}

		public void SetCurrent(int current, int total, params string[] stepName)
		{
			if (current < 0 || total < 0)
				throw new ArgumentException();
			if (HasFinished)
				throw new InvalidOperationException("Alteady finished");

			var finished = (current >= total);
			var tickCount = Environment.TickCount;
			var percent = current / (double) total;

			bool output;
			if (finished)
				output = true;
			else if (lastTickCount == null)
				output = true;
			else if (!Utils.ArrayEqual(stepName, LastStepName))
				output = true;
			else if (lastTickCount.Value + MinOutupWaitInMs > tickCount)
				output = false;
			else if (percent > LastPercent)
				output = true;
			else
				output = true;

			if (!output)
				return;

			if (lastTickCount == null)
				OnStart();

			lastTickCount = (finished ? (int?) null : tickCount);
			LastStepName = stepName;
			LastPercent = percent;
			HasFinished = finished;

			WriteToConsole(current, total, stepName);
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
	}
}
