using System;

namespace org.pescuma.progressmonitor.utils
{
	/// <summary>
	/// Throws an OperationCanceledException when a request was requested and a step related method is called.
	/// </summary>
	public class ThrowOnCancelledProgressMonitor : ProgressMonitor
	{
		private readonly ProgressMonitor next;

		public ThrowOnCancelledProgressMonitor(ProgressMonitor next)
		{
			this.next = next;
		}

		private void ThrowOnCancel()
		{
			if (WasCanceled)
				throw new OperationCanceledException();
		}

		public IDisposable ConfigureSteps(params int[] steps)
		{
			ThrowOnCancel();

			return next.ConfigureSteps(steps);
		}

		public void StartStep(string stepName = null)
		{
			ThrowOnCancel();

			next.StartStep(stepName);
		}

		public ProgressMonitor CreateSubMonitor()
		{
			ThrowOnCancel();

			return new ThrowOnCancelledProgressMonitor(next.CreateSubMonitor());
		}

		public void Finished()
		{
			ThrowOnCancel();

			next.Finished();
		}

		public void Report(string message, params object[] args)
		{
			next.Report(message, args);
		}

		public void ReportDetail(string message, params object[] args)
		{
			next.ReportDetail(message, args);
		}

		public void ReportWarning(string message, params object[] args)
		{
			next.ReportWarning(message, args);
		}

		public void ReportError(string message, params object[] args)
		{
			next.ReportError(message, args);
		}

		public bool WasCanceled
		{
			get { return next.WasCanceled; }
		}
	}
}
