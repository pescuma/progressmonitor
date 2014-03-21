using System;

namespace org.pescuma.progressmonitor.simple.console.widget
{
	public class Bar : ConsoleWidget
	{
		public void Started()
		{
		}

		public bool Grow
		{
			get { return true; }
		}

		public int ComputeSize(int current, int total, float percent, string stepName)
		{
			return 5;
		}

		public int OutputToConsole(int width, int current, int total, float percent, string stepName)
		{
			Console.Write("[");

			var hashWidth = (int) Math.Round((width - 2) * percent);
			if (hashWidth > 0)
				Console.Write(new String('#', hashWidth));

			var spaceWidth = width - 2 - hashWidth;
			if (spaceWidth > 0)
				Console.Write(new String(' ', spaceWidth));

			Console.Write("]");

			return width;
		}
	}
}
