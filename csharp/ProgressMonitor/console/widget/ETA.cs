using System;

namespace org.pescuma.progressmonitor.console.widget
{
	public class ETA : ConsoleWidget
	{
		private DateTime start;
		private string eta;

		public void Started()
		{
			start = DateTime.Now;
		}

		public bool Grow
		{
			get { return false; }
		}

		public int ComputeSize(int current, int total, double percent, string[] stepName)
		{
			if (percent < 0.01)
				return 0;

			eta = ComputeETA(DateTime.Now - start, percent);

			return eta.Length;
		}

		public void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
		{
			writer(eta);
		}

		// HACK public for tests
		public static string ComputeETA(TimeSpan elapsed, double percent)
		{
			var passed = elapsed.TotalMilliseconds / 1000;
			var total = passed / percent;
			var toGo = (int) Math.Round(total - passed);

			var result = "";
			Append(ref result, ref toGo, 60, "s");
			Append(ref result, ref toGo, 60, "m");
			Append(ref result, ref toGo, 24, "h");
			Append(ref result, ref toGo, 0, "d");
			return result;
		}

		private static void Append(ref string result, ref int toGo, int step, string name)
		{
			var val = toGo;

			if (step > 0)
			{
				val = toGo % step;
				toGo /= step;
			}

			if (val > 0)
				result = val + name + (result == "" ? "" : " " + result);
		}
	}
}
