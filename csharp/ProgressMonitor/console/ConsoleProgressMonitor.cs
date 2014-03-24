using org.pescuma.progressmonitor.console.widget;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
	public class ConsoleProgressMonitor : FlatToHierarchicalProgressMonitor
	{
		public ConsoleProgressMonitor(string name, params ConsoleWidget[] widgets)
			: base(name, new ConsoleFlatProgressMonitor(widgets))
		{
		}

		public ConsoleProgressMonitor(params ConsoleWidget[] widgets)
			: base(null, new ConsoleFlatProgressMonitor(widgets))
		{
		}
	}
}
