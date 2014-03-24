using org.pescuma.progressmonitor.flat.console;
using org.pescuma.progressmonitor.flat.console.widget;

namespace org.pescuma.progressmonitor
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
