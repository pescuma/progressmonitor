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

		public void Output(Action<string> writer, int width, int current, int total, float percent, string stepName)
		{
			writer("[");

			var hashWidth = (int) Math.Round((width - 2) * percent);
			if (hashWidth > 0)
				writer(new String('#', hashWidth));

			var spaceWidth = width - 2 - hashWidth;
			if (spaceWidth > 0)
				writer(new String(' ', spaceWidth));

			writer("]");
		}
	}
}
