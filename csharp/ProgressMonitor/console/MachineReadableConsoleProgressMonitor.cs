using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.console
{
    public class MachineReadableConsoleProgressMonitor : FlatToHierarchicalProgressMonitor
    {
        public MachineReadableConsoleProgressMonitor(string prefix = null)
                : base(prefix, new MachineReadableConsoleFlatProgressMonitor())
        {
        }

        public void RequestCancel()
        {
            ((MachineReadableConsoleFlatProgressMonitor) Flat).RequestCancel();
        }
    }
}
