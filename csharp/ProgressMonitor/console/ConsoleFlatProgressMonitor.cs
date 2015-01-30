using System;
using System.IO;
using org.pescuma.progressmonitor.console.widget;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
	public class ConsoleFlatProgressMonitor : BaseConsoleFlatProgressMonitor
	{
		private readonly WidgetCollection widgets;
		private bool showingProgress;
		private int lastCurrent;
		private int lastTotal;

		public ConsoleFlatProgressMonitor(params ConsoleWidget[] widgets)
		{
			if (widgets == null || widgets.Length < 1)
				widgets = new ConsoleWidget[]
				{
					new StepNameWidget(),
					new PercentageWidget(),
					new BarWidget(),
					"Elapsed",
					new ElapsedWidget(),
					"| ETA",
					new ETAWidget()
				};

			this.widgets = new WidgetCollection(widgets);
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
			widgets.Started();
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
			widgets.Output(Console.Write, ConsoleWidth, lastCurrent, lastTotal, LastPercent, LastStepName);

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
