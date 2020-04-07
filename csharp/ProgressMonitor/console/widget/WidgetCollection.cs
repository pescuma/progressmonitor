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
			foreach (ConsoleWidget widget in widgets)
				widget.Started();
		}

		public override AcceptableSizes ComputeSize(int current, int total, double percent, string[] stepName)
		{
			List<WidgetSize> ws = GetWidgetSizes(current, total, percent, stepName);

			return ComputeSize(ws);
		}

		private static AcceptableSizes ComputeSize(List<WidgetSize> ws)
		{
			int min = 0;
			int max = 0;
			bool grow = false;

			for (int i = 0; i < ws.Count; i++)
			{
				WidgetSize w = ws[i];

				if (i > 0)
				{
					min += 1;
					max += 1;
				}

				min += w.DesiredSize.Min;
				max += w.DesiredSize.Max;
				grow = grow || w.DesiredSize.GrowToUseEmptySpace;
			}

			return new AcceptableSizes(min, max, grow);
		}

		public override void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName)
		{
			List<WidgetSize> ws = GetWidgetSizes(current, total, percent, stepName);

			if (ws.Count < 1)
				return;

			AcceptableSizes desired = ComputeSize(ws);

			int remaining = width;

			for (int i = 0; i < ws.Count && remaining > 0; i++)
			{
				WidgetSize w = ws[i];

				if (i > 0)
					remaining--;

				if (width >= desired.Max)
				{
					w.Size = w.DesiredSize.Max;
				}
				else if (width > desired.Min)
				{
					w.Size = w.DesiredSize.Min
					         + (int) ((w.DesiredSize.Max - w.DesiredSize.Min) / (float) (desired.Max - desired.Min) * (width - desired.Min));
				}
				else
				{
					w.Size = Between(w.DesiredSize.Min, 0, remaining);
				}

				remaining -= w.Size;
			}

			// Remove the ones that don't fit in screen
			ws = ws.Where(w => w.Size > 0)
				.ToList();

			if (remaining > 0)
			{
				List<WidgetSize> toGrow = ws.Where(w => w.DesiredSize.GrowToUseEmptySpace)
					.ToList();

				foreach (WidgetSize w in toGrow)
				{
					int toUse = remaining / toGrow.Count;
					w.Size += toUse;
					remaining -= toUse;
				}
			}

			if (remaining > 0)
			{
				// Left overs from divisions
				WidgetSize growable = ws.LastOrDefault(w => w.DesiredSize.GrowToUseEmptySpace);
				if (growable != null)
				{
					growable.Size += remaining;
					remaining = 0;
				}
			}

			for (var i = 0; i < ws.Count; i++)
			{
				if (i > 0)
					writer(" ");

				WidgetSize w = ws[i];
				w.Widget.Output(writer, w.Size, current, total, percent, stepName);
			}

			if (remaining > 0)
				writer(new string(' ', remaining));
		}

		private List<WidgetSize> GetWidgetSizes(int current, int total, double percent, string[] stepName)
		{
			var ws = new List<WidgetSize>();

			foreach (ConsoleWidget widget in widgets)
			{
				AcceptableSizes desired = widget.ComputeSize(current, total, percent, stepName);
				if (desired.Max < 1)
					continue;

				ws.Add(new WidgetSize { Widget = widget, DesiredSize = desired });
			}
			return ws;
		}

		private int Between(int val, int min, int max)
		{
			if (max < min)
				return min;

			return Math.Min(Math.Max(val, min), max);
		}

		private class WidgetSize
		{
			public ConsoleWidget Widget;
			public AcceptableSizes DesiredSize;
			public int Size;
		}
	}
}
