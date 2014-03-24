using System;

namespace org.pescuma.progressmonitor
{
	public class ProgressMonitorWrapper : ProgressMonitor
	{
		private ProgressMonitor next;

		public event Action<string[]> OnStartStep;
		public event Action<string[]> OnFinishStep;

		public ProgressMonitorWrapper(ProgressMonitor next)
		{
			this.next = next;
		}

		public virtual IDisposable ConfigureSteps(params int[] steps)
		{
			return next.ConfigureSteps(steps);
		}

		public virtual void StartStep(string stepName = null)
		{
			next.StartStep(stepName);
		}

		public virtual ProgressMonitor CreateSubMonitor()
		{
			return next.CreateSubMonitor();
		}

		public virtual void Report(params string[] message)
		{
			next.Report(message);
		}

		public virtual void ReportDetail(params string[] message)
		{
			next.ReportDetail(message);
		}

		public virtual void ReportWarning(params string[] message)
		{
			next.ReportWarning(message);
		}

		public virtual void ReportError(params string[] message)
		{
			next.ReportError(message);
		}

		public virtual void Finished()
		{
			next.Finished();
		}
	}
}
