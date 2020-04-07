using org.pescuma.progressmonitor.console.widget;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
	public class ConsoleProgressMonitor : FlatToHierarchicalProgressMonitor
	{
		public int ConsoleWidth
		{
			get => ((ConsoleFlatProgressMonitor) Flat).ConsoleWidth;
			set => ((ConsoleFlatProgressMonitor) Flat).ConsoleWidth = value;
		}

		public ConsoleProgressMonitor(string prefix, params ConsoleWidget[] widgets)
				: base(prefix, new ConsoleFlatProgressMonitor(widgets))
		{
		}

		public ConsoleProgressMonitor(params ConsoleWidget[] widgets)
				: base(null, new ConsoleFlatProgressMonitor(widgets))
		{
		}

		public void RequestCancel()
		{
			((ConsoleFlatProgressMonitor) Flat).RequestCancel();
		}
	}
}
