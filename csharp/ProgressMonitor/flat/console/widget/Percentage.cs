using System;

namespace org.pescuma.progressmonitor.flat.console.widget
{
	public class Percentage : ConsoleWidget
	{
		public void Started()
		{
		}

		public bool Grow
		{
			get { return false; }
		}

		public int ComputeSize(int current, int total, double percent, string[] stepName)
		{
			var text = Format(percent);
			return text.Length;
		}

		public void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
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
