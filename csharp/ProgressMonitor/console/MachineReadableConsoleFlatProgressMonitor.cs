using System;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
	public class MachineReadableConsoleFlatProgressMonitor : FlatProgressMonitor
	{
		private const int MIN_UPDATE_TIME_MS = 500;

		private int? lastTickCount;
		private double lastPercent;
		private string[] lastStepName;

		public void SetCurrent(int current, int total, params string[] stepName)
		{
			if (current < 0 || total < 0)
				throw new ArgumentException();

			var finished = (current >= total);
			var tickCount = Environment.TickCount;
			var percent = current / (double) total;

			var output = false;
			if (finished)
				output = true;
			else if (lastTickCount == null)
				output = true;
			else if (!AreEqual(stepName, lastStepName))
				output = true;
			else if (lastTickCount.Value + MIN_UPDATE_TIME_MS > tickCount)
// ReSharper disable once RedundantAssignment
				output = false;
			else if (percent > lastPercent)
				output = true;

			if (!output)
				return;

			lastTickCount = (finished ? (int?) null : tickCount);

			lastStepName = stepName;
			lastPercent = percent;

			Utils.ConsoleWriteLine(MachineReadableConsole.ToConsole(current, total, stepName), ConsoleColor.Blue);
		}

		private bool AreEqual(string[] a, string[] b)
		{
			if (a == null && b == null)
				return true;
			if (a == null || b == null)
				return false;
			if (a.Length != b.Length)
				return false;
			for (int i = 0; i < a.Length; i++)
				if (!Equals(a[i], b[i]))
					return false;

			return true;
		}

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

		private void ReportWithColor(string[] message, ConsoleColor? color)
		{
			Utils.ConsoleWriteLine(Utils.Format(message), color);
		}
	}
}
