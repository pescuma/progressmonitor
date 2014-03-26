using System;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
	public class MachineReadableConsoleFlatProgressMonitor : BaseConsoleFlatProgressMonitor
	{
		protected override void WriteToConsole(int current, int total, string[] stepName)
		{
			Utils.ConsoleWriteLine(MachineReadableConsole.ToConsole(current, total, stepName), ConsoleColor.Black);
		}

		protected override void ReportWithColor(string[] message, ConsoleColor? color)
		{
			Utils.ConsoleWriteLine(Utils.Format(message), color);
		}
	}
}
