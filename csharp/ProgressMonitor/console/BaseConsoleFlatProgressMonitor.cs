using System;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
	public abstract class BaseConsoleFlatProgressMonitor : FlatProgressMonitor
	{
		private const int MIN_UPDATE_TIME_MS = 500;

		private int? lastTickCount;
		protected double LastPercent;
		protected string[] LastStepName;

		protected bool HasFinished;

		public void SetCurrent(int current, int total, params string[] stepName)
		{
			if (current < 0 || total < 0)
				throw new ArgumentException();
			if (HasFinished)
				throw new InvalidOperationException("Alteady finished");

			var finished = (current >= total);
			var tickCount = Environment.TickCount;
			var percent = current / (double) total;

			var output = false;
			if (finished)
				output = true;
			else if (lastTickCount == null)
				output = true;
			else if (!Utils.ArrayEqual(stepName, LastStepName))
				output = true;
			else if (lastTickCount.Value + MIN_UPDATE_TIME_MS > tickCount)
				// ReSharper disable once RedundantAssignment
				output = false;
			else if (percent > LastPercent)
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

		public void Report(params string[] message)
		{
			ReportWithColor(message, null);
		}

		public void ReportDetail(params string[] message)
		{
			ReportWithColor(message, ConsoleColor.DarkGray);
		}

		public void ReportWarning(params string[] message)
		{
			ReportWithColor(message, ConsoleColor.DarkYellow);
		}

		public void ReportError(params string[] message)
		{
			ReportWithColor(message, ConsoleColor.Red);
		}

		protected abstract void ReportWithColor(string[] message, ConsoleColor? color);
	}
}
