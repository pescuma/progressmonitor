using org.pescuma.progressmonitor.flat.console;

namespace org.pescuma.progressmonitor
{
	public class ConsoleProgressMonitor : FlatToHierarchicalProgressMonitor
	{
		public ConsoleProgressMonitor(string name = null)
			: base(name, new ConsoleFlatProgressMonitor())
		{
		}
	}
}
