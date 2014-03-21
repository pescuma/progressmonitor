using System;

namespace org.pescuma.progressmonitor.simple.console.widget
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

		public int ComputeSize(int current, int total, float percent, string stepName)
		{
			if (percent < 0.5)
				return 0;

			eta = ComputeETA(percent);
			return eta.Length;
		}

		public int OutputToConsole(int width, int current, int total, float percent, string stepName)
		{
			Console.Write(eta);

			return eta.Length;
		}

		private string ComputeETA(float percent)
		{
			var now = DateTime.Now;
			var passed = (now - start).TotalMilliseconds / 1000;
			var total = passed / (percent / 100);
			var toGo = total - passed;

			var result = "";
			Append(ref result, ref toGo, 60, "s");
			Append(ref result, ref toGo, 60, "m");
			Append(ref result, ref toGo, 60, "h");
			Append(ref result, ref toGo, 0, "d");
			return result;
		}

		private void Append(ref string result, ref double toGo, int step, string name)
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
