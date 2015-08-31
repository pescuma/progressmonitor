using System.Text;
using NUnit.Framework;
using org.pescuma.progressmonitor.console.widget;

namespace org.pescuma.progressmonitor.flat.console.widget
{
	[TestFixture]
	public class WidgetCollectionTest
	{
		[Test]
		public void ComputeSize_OnlyText()
		{
			var ws = new WidgetCollection(new TextWidget("Asdf"));

			Assert.AreEqual(new AcceptableSizes(4, 4, false), ws.ComputeSize(0, 0, 0, new[] { "a" }));
		}

		[Test]
		public void ComputeSize_OnlyStepName()
		{
			var ws = new WidgetCollection(new StepNameWidget());

			Assert.AreEqual(new AcceptableSizes(3, 10, false), ws.ComputeSize(0, 0, 0, new[] { "0123456789" }));
		}

		[Test]
		public void Draw_OnlyStepName_Small()
		{
			var ws = new WidgetCollection(new StepNameWidget());

			Assert.AreEqual("0123\u2026", Draw(ws, 5, "0123456789"));
		}

		[Test]
		public void Draw_OnlyStepName_Big()
		{
			var ws = new WidgetCollection(new StepNameWidget());

			Assert.AreEqual("0123456789          ", Draw(ws, 20, "0123456789"));
		}

		[Test]
		public void Draw_StepNameAndPercentage_Big()
		{
			var ws = new WidgetCollection(new StepNameWidget(), new BarWidget());

			Assert.AreEqual("0123456789 [####    ]", Draw(ws, 21, "0123456789"));
		}

		[Test]
		public void Draw_StepNameAndPercentage_Small()
		{
			var ws = new WidgetCollection(new StepNameWidget(), new BarWidget());

			Assert.AreEqual("012 [# ]", Draw(ws, 8, "0123456789"));
		}

		[Test]
		public void Draw_GrowTwice()
		{
			var ws = new WidgetCollection(new BarWidget(), new BarWidget());

			Assert.AreEqual("[####    ] [####    ]", Draw(ws, 21, "0123456789"));
		}

		[Test]
		public void Draw_GrowTwice_DifferentSizes()
		{
			var ws = new WidgetCollection(new BarWidget(), new BarWidget());

			Assert.AreEqual("[####   ] [####    ]", Draw(ws, 20, "0123456789"));
		}

		private string Draw(ConsoleWidget ws, int width, string step)
		{
			var result = new StringBuilder();

			ws.Output(s => result.Append(s), width, 1, 2, 0.5, new[] { step });

			return result.ToString();
		}
	}
}
