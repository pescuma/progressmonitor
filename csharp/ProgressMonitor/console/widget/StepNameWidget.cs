using System;
using System.Linq;

namespace org.pescuma.progressmonitor.console.widget
{
	public class StepNameWidget : ConsoleWidget
	{
		private readonly Func<string[], string> formater = DefaultFormater;
		private readonly int maxWidth = 30;

		public StepNameWidget(Func<string[], string> formater, int maxWidth)
		{
			this.formater = formater;
			this.maxWidth = maxWidth;
		}

		public StepNameWidget(Func<string[], string> formater)
		{
			this.formater = formater;
			maxWidth = int.MaxValue;
		}

		public StepNameWidget(int maxWidth)
		{
			this.maxWidth = maxWidth;
		}

		public StepNameWidget()
		{
		}

		public override void Started()
		{
		}

		public override bool Grow
		{
			get { return false; }
		}

		public override int ComputeSize(int current, int total, double percent, string[] stepName)
		{
			var text = formater(stepName) ?? "";
			return Math.Min(text.Length, maxWidth);
		}

		public override void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
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
