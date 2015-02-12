using System;
using System.Collections.Generic;
using System.Linq;

namespace org.pescuma.progressmonitor.console.widget
{
	public class WidgetCollection : ConsoleWidget
	{
		private readonly ConsoleWidget[] widgets;

		public WidgetCollection(params ConsoleWidget[] widgets)
		{
			if (widgets == null || widgets.Length < 1)
				widgets = new ConsoleWidget[]
				{
					new StepNameWidget(),
					new PercentageWidget(),
					new BarWidget(),
					"Elapsed",
					new ElapsedWidget(),
					"| ETA",
					new ETAWidget()
				};

			this.widgets = widgets;
		}

		public override void Started()
		{
			foreach (var widget in widgets)
				widget.Started();
		}

		public override bool Grow
		{
			get { return widgets.Any(w => w.Grow); }
		}

		public override int ComputeSize(int current, int total, double percent, string[] stepName)
		{
			var result = 0;

			foreach (var widget in widgets)
			{
				var innerWidth = widget.ComputeSize(current, total, percent, stepName);
				if (innerWidth < 1)
					continue;

				if (result > 0)
					result += 1;

				result += innerWidth;
			}

			return result;
		}

		public override void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
		{
			var widths = new Dictionary<ConsoleWidget, int>();

			var remaining = width;

			foreach (var widget in widgets)
			{
				var innerWidth = widget.ComputeSize(current, total, percent, stepName);
				if (innerWidth < 1)
					continue;

				var spacing = (widths.Any() ? 1 : 0);

				if (remaining < innerWidth + spacing)
					continue;

				remaining -= innerWidth + spacing;
				widths[widget] = innerWidth;
			}

			var used = widgets.Where(widths.ContainsKey)
				.ToList();

			var widgetsToGrow = used.Count(w => w.Grow);
			if (widgetsToGrow > 0)
			{
				var toGrow = width - (used.Count - 1) - used.Sum(w => widths[w]);

				var each = toGrow / widgetsToGrow;
				foreach (var w in used.Where(w => w.Grow))
					widths[w] += each;

				toGrow -= each * widgetsToGrow;

				widths[used.First(w => w.Grow)] += toGrow;
			}

			for (var i = 0; i < used.Count; i++)
			{
				if (i > 0)
					writer(" ");

				var w = used[i];
				w.Output(writer, widths[w], current, total, percent, stepName);
			}
		}
	}
}
