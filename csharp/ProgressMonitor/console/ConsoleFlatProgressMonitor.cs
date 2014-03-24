using System;
using System.Collections.Generic;
using System.Linq;
using org.pescuma.progressmonitor.console.widget;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
	public class ConsoleFlatProgressMonitor : FlatProgressMonitor
	{
		private const int MIN_UPDATE_TIME_MS = 500;

		private readonly ConsoleWidget[] widgets;

		private int? lastTickCount;
		private int lastCurrent;
		private int lastTotal;
		private double lastPercent;
		private string[] lastStepName;

		public ConsoleFlatProgressMonitor(params ConsoleWidget[] widgets)
		{
			if (widgets == null || widgets.Length < 1)
				this.widgets = new ConsoleWidget[] { new StepName(), new Percentage(), new Bar(), new ETA() };
			else
				this.widgets = widgets;
		}

		private int ConsoleWidth
		{
			get { return Console.BufferWidth - 1; }
		}

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

			lastCurrent = current;
			lastTotal = total;
			lastStepName = stepName;
			lastPercent = percent;

			ClearLine();

			if (!finished)
				OutputProgress();
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
					continue;

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

			Utils.ConsoleWriteLine(Utils.Format(message), color);

			if (lastTickCount != null)
				OutputProgress();
		}
	}
}
