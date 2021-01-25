namespace org.pescuma.progressmonitor.utils
{
    public class FilteredFlatProgressMonitor : FlatProgressMonitor
    {
        private readonly FlatProgressMonitor next;

        public bool DontOutputProgress = false;
        public bool DontOutputReports = false;
        public bool DontOutputReportDetails = false;
        public bool DontOutputReportWarnings = false;
        public bool DontOutputReportErrors = false;

        public FlatProgressMonitor Original
        {
            get { return next; }
        }

        public FilteredFlatProgressMonitor(FlatProgressMonitor next)
        {
            this.next = next;
        }

        public void SetCurrent(int current, int total, params string[] stepName)
        {
            if (DontOutputProgress)
                return;

            next.SetCurrent(current, total, stepName);
        }

        public void Report(string message, params object[] args)
        {
            if (DontOutputReports)
                return;

            next.Report(message, args);
        }

        public void ReportDetail(string message, params object[] args)
        {
            if (DontOutputReportDetails)
                return;

            next.ReportDetail(message, args);
        }

        public void ReportWarning(string message, params object[] args)
        {
            if (DontOutputReportWarnings)
                return;

            next.ReportWarning(message, args);
        }

        public void ReportError(string message, params object[] args)
        {
            if (DontOutputReportErrors)
                return;

            next.ReportError(message, args);
        }

        public bool WasCanceled
        {
            get { return next.WasCanceled; }
        }
    }
}
