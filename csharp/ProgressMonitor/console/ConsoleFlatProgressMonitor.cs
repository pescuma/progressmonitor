using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using org.pescuma.progressmonitor.console.widget;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
	public class ConsoleFlatProgressMonitor : BaseConsoleFlatProgressMonitor
	{
		private readonly ConsoleWidget[] widgets;
		private bool showingProgress;
		private int lastCurrent;
		private int lastTotal;

		public ConsoleFlatProgressMonitor(params ConsoleWidget[] widgets)
		{
			if (widgets == null || widgets.Length < 1)
				this.widgets = new ConsoleWidget[]
				{
					new StepNameWidget(),
					new PercentageWidget(),
					new BarWidget(),
					"Elapsed",
					new ElapsedWidget(),
					"| ETA",
					new ETAWidget()
				};
			else
				this.widgets = widgets;
		}

		private int ConsoleWidth
		{
			get
			{
				try
				{
					return Console.BufferWidth - 1;
				}
				catch (IOException)
				{
					return 79;
				}
			}
		}

		protected override void OnStart()
		{
			foreach (var widget in widgets)
				widget.Started();
		}

		protected override void WriteToConsole(int current, int total, string[] stepName)
		{
			if (showingProgress)
				ClearLine();

			lastCurrent = current;
			lastTotal = total;

			if (!HasFinished)
				OutputProgress();
		}

		private void ClearLine()
		{
			Console.Write("\r");
			Console.Write(new String(' ', ConsoleWidth));
			Console.Write("\r");

			showingProgress = false;
		}

		private void OutputProgress()
		{
			var widths = new Dictionary<ConsoleWidget, int>();

			var remaining = ConsoleWidth;

			foreach (var widget in widgets)
			{
				var width = widget.ComputeSize(lastCurrent, lastTotal, LastPercent, LastStepName);
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
				w.Output(Console.Write, widths[w], lastCurrent, lastTotal, LastPercent, LastStepName);
			}

			showingProgress = true;
		}

		protected override void ReportWithColor(string message, object[] args, ConsoleColor? color)
		{
			var wasShowingProgress = showingProgress;

			if (wasShowingProgress)
				ClearLine();

			Utils.ConsoleWriteLine(color, message, args);

			if (wasShowingProgress)
				OutputProgress();
		}
	}
}
