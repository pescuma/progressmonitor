using System;
using System.Collections.Generic;
using System.Linq;
using org.pescuma.progressmonitor.flat;

namespace org.pescuma.progressmonitor
{
	public class FlatToHierarchicalProgressMonitor : ProgressMonitor
	{
		private readonly string prefix;
		private readonly FlatProgressMonitor monitor;
		private readonly FlatToHierarchicalProgressMonitor parent;

		private int[] steps;
		private int currentStep;
		private string currentStepName;

		private bool IsConfigured
		{
			get { return steps != null; }
		}

		private bool IsStarted
		{
			get { return currentStep >= 0; }
		}

		public event Action<string[]> OnStartedStep;
		public event Action<string[]> OnFinishedStep;

		public FlatToHierarchicalProgressMonitor(string name, FlatProgressMonitor monitor)
		{
			prefix = name;
			this.monitor = monitor;
		}

		private FlatToHierarchicalProgressMonitor(FlatToHierarchicalProgressMonitor parent, FlatProgressMonitor monitor)
		{
			this.parent = parent;
			this.monitor = monitor;
		}

		public IDisposable ConfigureSteps(params int[] aSteps)
		{
			CheckNotConfigured();

			if (aSteps.Length < 1)
				throw new ArgumentException();
			if (aSteps.Any(v => v < 1))
				throw new ArgumentException();

			if (aSteps.Length == 1)
			{
				steps = new int[aSteps[0]];
				for (var i = 0; i < steps.Length; i++)
					steps[i] = 1;
			}
			else
			{
				steps = aSteps;
			}

			currentStep = -1;

			return new ActionDisposable(Finished);
		}

		public void StartStep(string stepName = null)
		{
			CheckConfigured();

			if (currentStep + 1 >= steps.Length)
				throw new ArgumentException();

			var wasStarted = IsStarted;
			string[] oldName = null;
			var onFinishedStep = OnFinishedStep;

			if (wasStarted && onFinishedStep != null)
				oldName = GetFullStepName();

			currentStep = currentStep + 1;
			currentStepName = stepName;

			Propagate();

			if (wasStarted && onFinishedStep != null)
				onFinishedStep(oldName);

			var onOnStartedStep = OnStartedStep;
			if (onOnStartedStep != null)
				onOnStartedStep(GetFullStepName());
		}

		public void Finished()
		{
			CheckConfigured();

			var started = IsStarted;
			string[] fullName = null;
			var onFinishedStep = OnFinishedStep;

			if (started && (onFinishedStep != null || parent == null))
				fullName = GetFullStepName();

			steps = null;

			if (started)
			{
				if (parent == null)
					Output(1, fullName);

				if (onFinishedStep != null)
					onFinishedStep(fullName);
			}
		}

		public ProgressMonitor CreateSubMonitor()
		{
			CheckStarted();

			var result = new FlatToHierarchicalProgressMonitor(this, monitor);

			if (OnStartedStep != null)
				result.OnStartedStep += OnStartedStep;

			if (OnFinishedStep != null)
				result.OnFinishedStep += OnFinishedStep;

			return result;
		}

		private void Propagate()
		{
			var percentage = steps.Take(currentStep)
				.Sum() / (double) steps.Sum();
			var name = GetFullStepName();

			if (parent == null)
				Output(percentage, name);
			else
				parent.OnChildChange(percentage, name);
		}

		private void OnChildChange(double percentage, string[] name)
		{
			percentage = (steps.Take(currentStep)
				.Sum() + steps[currentStep] * percentage) / steps.Sum();

			if (parent == null)
				Output(percentage, name);
			else
				parent.OnChildChange(percentage, name);
		}

		private void Output(double percentage, string[] name)
		{
			monitor.SetCurrent((int) Math.Round(percentage * 1000), 1000, name);
		}

		private string[] GetFullStepName()
		{
			var names = new List<string>();
			var x = this;
			do
			{
				names.Add(x.currentStepName ?? "");

				if (x.prefix != null)
					names.Add(x.prefix);

				x = x.parent;
			} while (x != null);

			names.Reverse();

			return names.ToArray();
		}

		private void CheckNotConfigured()
		{
			if (steps != null)
				throw new InvalidOperationException("Already configured");
		}

		private void CheckConfigured()
		{
			if (!IsConfigured)
				throw new InvalidOperationException("Not configured yet or already finished");
		}

		private void CheckStarted()
		{
			CheckConfigured();

			if (!IsStarted)
				throw new InvalidOperationException("Not started yet");
		}

		public void Report(params string[] message)
		{
			monitor.Report(message);
		}

		public void ReportDetail(params string[] message)
		{
			monitor.ReportDetail(message);
		}

		public void ReportWarning(params string[] message)
		{
			monitor.ReportWarning(message);
		}

		public void ReportError(params string[] message)
		{
			monitor.ReportError(message);
		}

		private class ActionDisposable : IDisposable
		{
			private readonly Action dispose;

			public ActionDisposable(Action dispose)
			{
				this.dispose = dispose;
			}

			public void Dispose()
			{
				dispose();
			}
		}
	}
}
