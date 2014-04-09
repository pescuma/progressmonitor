using System;

namespace org.pescuma.progressmonitor.console.widget
{
	public class PercentageWidget : ConsoleWidget
	{
		public override void Started()
		{
		}

		public override bool Grow
		{
			get { return false; }
		}

		public override int ComputeSize(int current, int total, double percent, string[] stepName)
		{
			var text = Format(percent);
			return text.Length;
		}

		public override void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
		{
			var text = Format(percent);
			writer(text);
		}

		private static string Format(double percent)
		{
			return string.Format("{0:F0}%", percent * 100);
		}
	}
}
