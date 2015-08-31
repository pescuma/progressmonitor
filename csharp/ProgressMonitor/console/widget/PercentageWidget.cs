using System;

namespace org.pescuma.progressmonitor.console.widget
{
	public class PercentageWidget : ConsoleWidget
	{
		public override void Started()
		{
		}

		public override AcceptableSizes ComputeSize(int current, int total, double percent, string[] stepName)
		{
			string text = Format(percent);
			return new AcceptableSizes(text.Length, text.Length, false);
		}

		public override void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
		{
			string text = Format(percent);
			writer(text);
		}

		private static string Format(double percent)
		{
			return string.Format("{0:F0}%", percent * 100);
		}
	}
}
