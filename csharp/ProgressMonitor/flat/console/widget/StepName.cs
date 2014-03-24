using System;
using System.Linq;

namespace org.pescuma.progressmonitor.flat.console.widget
{
	public class StepName : ConsoleWidget
	{
		private readonly Func<string[], string> formater = DefaultFormater;
		private readonly int maxWidth = 30;

		public StepName(Func<string[], string> formater, int maxWidth)
		{
			this.formater = formater;
			this.maxWidth = maxWidth;
		}

		public StepName(Func<string[], string> formater)
		{
			this.formater = formater;
			maxWidth = int.MaxValue;
		}

		public StepName(int maxWidth)
		{
			this.maxWidth = maxWidth;
		}

		public StepName()
		{
		}

		public void Started()
		{
		}

		public bool Grow
		{
			get { return false; }
		}

		public int ComputeSize(int current, int total, double percent, string[] stepName)
		{
			var text = formater(stepName) ?? "";
			return Math.Min(text.Length, maxWidth);
		}

		public void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
		{
			var text = formater(stepName) ?? "";

			if (text.Length > maxWidth)
			{
				if (maxWidth > 5)
					text = text.Substring(0, maxWidth - 3) + "...";
				else
					text = text.Substring(0, maxWidth);
			}

			writer(text);
		}

		public static string DefaultFormater(string[] name)
		{
			if (name == null)
				return "";

			return string.Join(" - ", name.Where(s => !string.IsNullOrEmpty(s)));
		}
	}
}
