using System;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
	public class OnlyConsoleMessagesFlatProgressMonitor : BaseConsoleFlatProgressMonitor
	{
		protected override void WriteToConsole(int current, int total, string[] stepName)
		{
		}

		protected override void ReportWithColor(string[] message, ConsoleColor? color)
		{
			Utils.ConsoleWriteLine(color, message);
		}
	}
}
