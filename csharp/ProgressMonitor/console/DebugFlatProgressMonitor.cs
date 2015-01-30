using System;
using System.Diagnostics;
using System.Text;
using org.pescuma.progressmonitor.console.widget;

namespace org.pescuma.progressmonitor.console
{
	public class DebugFlatProgressMonitor : BaseConsoleFlatProgressMonitor
	{
		private readonly WidgetCollection widgets;

		public DebugFlatProgressMonitor(params ConsoleWidget[] widgets)
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
