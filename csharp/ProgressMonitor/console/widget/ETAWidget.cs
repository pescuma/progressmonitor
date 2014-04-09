using System;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console.widget
{
	public class ETAWidget : ConsoleWidget
	{
		private DateTime start;
		private string eta;

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
			if (percent < 0.01)
				return 0;

			var elapsedTime = (DateTime.Now - start).TotalMilliseconds / 1000;
			var totalTime = elapsedTime / percent;
			var toGo = (int) Math.Round(totalTime - elapsedTime);

			eta = Utils.FormatSeconds(toGo);

			return eta.Length;
		}

		public override void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
		{
			writer(eta);
		}
	}
}
