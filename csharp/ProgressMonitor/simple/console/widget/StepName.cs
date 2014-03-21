using System;

namespace org.pescuma.progressmonitor.simple.console.widget
{
	public class StepName : ConsoleWidget
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
			if (string.IsNullOrWhiteSpace(stepName))
				return 0;
			else
				return stepName.Length;
		}

		public int OutputToConsole(int width, int current, int total, float percent, string stepName)
		{
			Console.Write(stepName);

			return stepName.Length;
		}
	}
}
