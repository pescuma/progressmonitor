using org.pescuma.progressmonitor.console.widget;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
    public class DebugProgressMonitor : FlatToHierarchicalProgressMonitor
    {
        public DebugProgressMonitor(string prefix, params ConsoleWidget[] widgets)
                : base(prefix, new DebugFlatProgressMonitor(widgets))
        {
        }

        public DebugProgressMonitor(params ConsoleWidget[] widgets)
                : base(null, new DebugFlatProgressMonitor(widgets))
        {
        }

        public void RequestCancel()
        {
            ((DebugFlatProgressMonitor) Flat).RequestCancel();
        }
    }
}
