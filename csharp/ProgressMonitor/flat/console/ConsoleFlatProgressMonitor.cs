using System;
using System.Collections.Generic;
using System.Linq;
using org.pescuma.progressmonitor.flat.console.widget;

namespace org.pescuma.progressmonitor.flat.console
{
	public class ConsoleFlatProgressMonitor : FlatProgressMonitor
	{
		private const int MIN_UPDATE_TIME_MS = 500;

		private ConsoleWidget[] widgets = { new StepName(), new Percentage(), new Bar(), new ETA() };

		private int? lastTickCount;
		private int lastCurrent;
		private int lastTotal;
		private double lastPercent;
		private string lastStepName;

		private int ConsoleWidth
		{
			get { return Console.BufferWidth - 1; }
		}

		public void SetCurrent(int current, int total, string stepName)
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
			else if ((stepName ?? "") != (lastStepName ?? ""))
				output = true;
			else if (lastTickCount.Value + MIN_UPDATE_TIME_MS > tickCount)
// ReSharper disable once RedundantAssignment
				output = false;
			else if (percent > lastPercent)
				output = true;

			if (!output)
				return;

			lastTickCount = (finished ? (int?) null : tickCount);

			lastCurrent = current;
			lastTotal = total;
			lastStepName = stepName;
			lastPercent = percent;

			ClearLine();

			if (!finished)
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
			var widths = new Dictionary<ConsoleWidget, int>();

			var remaining = ConsoleWidth;

			foreach (var widget in widgets)
			{
				var width = widget.ComputeSize(lastCurrent, lastTotal, lastPercent, lastStepName);
				if (width < 1)
					continue;

				var spacing = (widths.Any() ? 1 : 0);

				if (remaining < width + spacing)
					break;

				remaining -= width + spacing;
				widths[widget] = width;
			}

			var used = widgets.Where(widths.ContainsKey)
				.ToList();

			var widgetsToGrow = used.Count(w => w.Grow);
			if (widgetsToGrow > 0)
			{
				var toGrow = ConsoleWidth - (used.Count - 1) - used.Sum(w => widths[w]);

				var each = toGrow / widgetsToGrow;
				foreach (var w in used.Where(w => w.Grow))
					widths[w] += each;

				toGrow -= each * widgetsToGrow;

				widths[used.First(w => w.Grow)] += toGrow;
			}

			for (var i = 0; i < used.Count; i++)
			{
				if (i > 0)
					Console.Write(" ");

				var w = used[i];
				w.Output(Console.Write, widths[w], lastCurrent, lastTotal, lastPercent, lastStepName);
			}
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
			if (lastTickCount != null)
				ClearLine();

			var old = ConsoleColor.Gray;
			if (color != null)
			{
				old = Console.ForegroundColor;
				Console.ForegroundColor = color.Value;
			}

			Console.WriteLine(Format(message));

			if (color != null)
				Console.ForegroundColor = old;

			if (lastTickCount != null)
				OutputProgress();
		}

		private string Format(string[] message)
		{
			if (message.Length < 1)
				return "";
			else if (message.Length == 1)
				return message[0];
			else
// ReSharper disable once RedundantCast
				return string.Format(message[0], (string[]) message.Skip(1)
					.ToArray());
		}
	}
}
