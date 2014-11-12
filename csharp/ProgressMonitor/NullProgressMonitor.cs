using System;

namespace org.pescuma.progressmonitor
{
	public class NullProgressMonitor : ProgressMonitor
	{
		public void Report(string message, params object[] args)
		{
		}

		public void ReportDetail(string message, params object[] args)
		{
		}

		public void ReportWarning(string message, params object[] args)
		{
		}

		public void ReportError(string message, params object[] args)
		{
		}

		public IDisposable ConfigureSteps(params int[] steps)
		{
			return new NullIDisposable();
		}

		public void StartStep(string stepName = null)
		{
		}

		public ProgressMonitor CreateSubMonitor()
		{
			return this;
		}

		public void Finished()
		{
		}

		public bool WasCanceled
		{
			get { return false; }
		}

		private class NullIDisposable : IDisposable
		{
			public void Dispose()
			{
			}
		}
	}
}
