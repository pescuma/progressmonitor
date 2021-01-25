using System;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
    public class MachineReadableConsoleFlatProgressMonitor : BaseConsoleFlatProgressMonitor
    {
        protected override void WriteToConsole(int current, int total, string[] stepName)
        {
            Utils.ConsoleWriteLine(ConsoleColor.Black, MachineReadableConsole.ToConsole(current, total, stepName));
        }

        protected override void ReportWithColor(string message, object[] args, ConsoleColor? color)
        {
            Utils.ConsoleWriteLine(color, message, args);
        }
    }
}
