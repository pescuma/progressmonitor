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

		public void Report(params string[] message)
		{
			if (DontOutputReports)
				return;

			next.Report(message);
		}

		public void ReportDetail(params string[] message)
		{
			if (DontOutputReportDetails)
				return;

			next.ReportDetail(message);
		}

		public void ReportWarning(params string[] message)
		{
			if (DontOutputReportWarnings)
				return;

			next.ReportWarning(message);
		}

		public void ReportError(params string[] message)
		{
			if (DontOutputReportErrors)
				return;

			next.ReportError(message);
		}
	}
}
