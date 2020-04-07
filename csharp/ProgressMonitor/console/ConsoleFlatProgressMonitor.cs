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
		private int lastCurrent;
		private int lastTotal;
		private int? consoleWidth;

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

		protected override void OnStart()
		{
			widgets.Started();
		}

		protected override void WriteToConsole(int current, int total, string[] stepName)
		{
			if (atEndOfProgressLine)
				ClearLine();

			lastCurrent = current;
			lastTotal = total;

			if (!HasFinished || ConsoleCachedInfo.IsOutputRedirected)
				OutputProgress();
		}

		private void ClearLine()
		{
			if (!ConsoleCachedInfo.IsOutputRedirected)
			{
				Console.SetCursorPosition(0, Console.CursorTop);
				Console.Write(new string(' ', ConsoleWidth));
				Console.SetCursorPosition(0, Console.CursorTop);

				atEndOfProgressLine = false;
			}
		}

		private void OutputProgress()
		{
			var sb = new StringBuilder();
			widgets.Output(t => sb.Append(t), ConsoleWidth, lastCurrent, lastTotal, LastPercent, LastStepName);
			var line = sb.ToString();

			if (ConsoleCachedInfo.IsOutputRedirected)
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
				OutputProgress();
		}

		private static class ConsoleCachedInfo
		{
			public static readonly bool IsOutputRedirected;

			static ConsoleCachedInfo()
			{
				IsOutputRedirected = Console.IsOutputRedirected;
			}
		}
	}
}
