using System;

namespace org.pescuma.progressmonitor.utils
{
	public class ObservableProgressMonitor : ProgressMonitor
	{
		private readonly ProgressMonitor next;
		private readonly Events events;

		public event Action OnConfigureSteps
		{
			add { events.OnConfigureSteps += value; }
			remove { events.OnConfigureSteps -= value; }
		}

		public event Action OnStartStep
		{
			add { events.OnStartStep += value; }
			remove { events.OnStartStep -= value; }
		}

		public event Action OnFinished
		{
			add { events.OnFinished += value; }
			remove { events.OnFinished -= value; }
		}

		public ObservableProgressMonitor(ProgressMonitor next)
			: this(next, new Events())
		{
		}

		private ObservableProgressMonitor(ProgressMonitor next, Events events)
		{
			this.next = next;
			this.events = events;
		}

		public IDisposable ConfigureSteps(params int[] steps)
		{
			var result = next.ConfigureSteps(steps);

			events.NotifyConfigureSteps();

			return result;
		}

		public void StartStep(string stepName = null, params object[] args)
		{
			next.StartStep(stepName, args);

			events.NotifyStartStep();
		}

		public ProgressMonitor CreateSubMonitor()
		{
			return new ObservableProgressMonitor(next.CreateSubMonitor(), events);
		}

		public void Finished()
		{
			next.Finished();

			events.NotifyFinished();
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

		private class Events
		{
			public event Action OnConfigureSteps;
			public event Action OnStartStep;
			public event Action OnFinished;

			public void NotifyConfigureSteps()
			{
				var handler = OnConfigureSteps;
				if (handler != null)
					handler();
			}

			public void NotifyStartStep()
			{
				var handler = OnStartStep;
				if (handler != null)
					handler();
			}

			public void NotifyFinished()
			{
				var handler = OnFinished;
				if (handler != null)
					handler();
			}
		}
	}
}
