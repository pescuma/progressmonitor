﻿using System;
using System.Diagnostics;
using System.Text;
using org.pescuma.progressmonitor.console.widget;

namespace org.pescuma.progressmonitor.console
{
	public class DebugFlatProgressMonitor : BaseConsoleFlatProgressMonitor
	{
		private readonly WidgetCollection widgets;

		public override int MinOutupWaitInMs
		{
			get { return 2000; }
		}

		public DebugFlatProgressMonitor(params ConsoleWidget[] widgets)
		{
			this.widgets = new WidgetCollection(widgets);
		}

		protected override void OnStart()
		{
			widgets.Started();
		}

		protected override void WriteToConsole(int current, int total, string[] stepName)
		{
			var text = new StringBuilder();
			widgets.Output(t => text.Append(t), 80, current, total, LastPercent, LastStepName);
			Debug.WriteLine(text.ToString());
		}

		protected override void ReportWithColor(string message, object[] args, ConsoleColor? color)
		{
			Debug.WriteLine(message, args);
		}
	}
}
