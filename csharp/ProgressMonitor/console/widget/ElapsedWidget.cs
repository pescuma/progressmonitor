using System;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console.widget
{
	public class ElapsedWidget : ConsoleWidget
	{
		private DateTime start;
		private string elapsed;

		public override void Started()
		{
			start = DateTime.Now;
		}

		public override bool Grow
		{
			get { return false; }
		}

		public override int ComputeSize(int current, int total, double percent, string[] stepName)
		{
			elapsed = Utils.FormatSeconds((int) Math.Round((DateTime.Now - start).TotalMilliseconds / 1000));

			return elapsed.Length;
		}

		public override void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
		{
			writer(elapsed);
		}
	}
}
