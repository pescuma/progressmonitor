using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
	public class OnlyConsoleMessagesProgressMonitor : FlatToHierarchicalProgressMonitor
	{
		public OnlyConsoleMessagesProgressMonitor()
			: base(null, new OnlyConsoleMessagesFlatProgressMonitor())
		{
		}
	}
}
