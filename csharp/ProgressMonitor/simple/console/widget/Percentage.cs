using System;

namespace org.pescuma.progressmonitor.simple.console.widget
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

		public int ComputeSize(int current, int total, float percent, string stepName)
		{
			var text = Format(percent);
			return text.Length;
		}

		public int OutputToConsole(int width, int current, int total, float percent, string stepName)
		{
			var text = Format(percent);

			Console.Write(text);

			return text.Length;
		}

		private static string Format(float percent)
		{
			return string.Format("{0:F0}%", percent);
		}
	}
}
