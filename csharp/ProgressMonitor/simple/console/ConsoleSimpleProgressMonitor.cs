using System;
using System.Linq;
using org.pescuma.progressmonitor.simple.console.widget;

namespace org.pescuma.progressmonitor.simple.console
{
	public class ConsoleSimpleProgressMonitor : SimpleProgressMonitor
	{
		private const int MIN_UPDATE_TIME_MS = 500;

		private int lastCurrent = -1;
		private int lastTotal = -1;
		private float lastPercent = -1;
		private string lastStepName;
		private int? lastTickCount;
		private ConsoleWidget[] widgets = { new StepName(), new Percentage(), new Bar(), new ETA() };

		private int ConsoleWidth
		{
			get { return Console.BufferWidth - 1; }
		}

		public void SetCurrent(int current, int total, string stepName)
		{
			if (current < 0 || total < 0)
				throw new ArgumentException();

			var finised = (current >= total);
			var tickCount = Environment.TickCount;
			var percent = ComputePercent(current, total);

			var output = false;
			if (finised)
				output = true;
			else if (lastTickCount == null)
				output = true;
			else if (lastTickCount.Value + MIN_UPDATE_TIME_MS > tickCount)
				output = false;
			else if (percent > lastPercent || stepName != lastStepName)
				output = true;

			if (!output)
				return;

			lastCurrent = current;
			lastTotal = total;
			lastStepName = stepName;
			lastPercent = percent;
			lastTickCount = tickCount;

			ClearLine();
			OutputProgress();
		}

		private void ClearLine()
		{
			Console.Write("\r");
			Console.Write(new String(' ', ConsoleWidth));
			Console.Write("\r");
		}

		private void OutputProgress()
		{
			throw new NotImplementedException();
		}

		private static float ComputePercent(int current, int total)
		{
			return (float) Math.Round((current / (float) total) * 100 * 10) / 10;
		}

		public void Report(params string[] message)
		{
			if (lastTickCount != null)
				ClearLine();

			Console.WriteLine(Format(message));

			if (lastTickCount != null)
				OutputProgress();
		}

		public void ReportWarning(params string[] message)
		{
			ReportWithColor(message, ConsoleColor.DarkYellow);
		}

		public void ReportError(params string[] message)
		{
			ReportWithColor(message, ConsoleColor.Red);
		}

		private void ReportWithColor(string[] message, ConsoleColor color)
		{
			var old = Console.ForegroundColor;
			Console.ForegroundColor = color;

			Report(message);

			Console.ForegroundColor = old;
		}

		private string Format(string[] message)
		{
			if (message.Length < 1)
				return "";
			else if (message.Length == 1)
				return message[0];
			else
				return string.Format(message[0], (string[]) message.Skip(1)
					.ToArray());
		}
	}
}
