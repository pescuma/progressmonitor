using System;
using System.Linq;

namespace org.pescuma.progressmonitor.utils
{
	public class FlatToHierarchicalProgressMonitor : ProgressMonitor
	{
		private readonly string[] name;
		private readonly FlatProgressMonitor flat;
		private readonly Steps parent;
		private bool configured;

		public event Action<string[]> OnStartedStep;
		public event Action<string[]> OnFinishedStep;

		public FlatToHierarchicalProgressMonitor(string name, FlatProgressMonitor flat)
		{
			this.flat = flat;

			if (!string.IsNullOrEmpty(name))
				this.name = new[] { name };
			else
				this.name = new string[0];
		}

		private FlatToHierarchicalProgressMonitor(Steps parent, FlatProgressMonitor flat, string[] name)
		{
			this.name = name;
			this.parent = parent;
			this.flat = flat;
		}

		public ProgressSteps ConfigureSteps(params int[] aSteps)
		{
			if (configured)
				throw new InvalidOperationException("Alteady configured");
			configured = true;

			if (aSteps.Length < 1)
				throw new ArgumentException();
			if (aSteps.Any(v => v < 1))
				throw new ArgumentException();

			int[] steps;
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

			return new Steps(this, steps);
		}

		public void SetCurrent(int current, int total, params string[] stepName)
		{
			if (parent == null)
				flat.SetCurrent(current, total, stepName);
			else
				parent.OnChildChange(current, total, stepName);
		}

		public void Report(params string[] message)
		{
			flat.Report(message);
		}

		public void ReportDetail(params string[] message)
		{
			flat.ReportDetail(message);
		}

		public void ReportWarning(params string[] message)
		{
			flat.ReportWarning(message);
		}

		public void ReportError(params string[] message)
		{
			flat.ReportError(message);
		}

		private class Steps : ProgressSteps
		{
			private readonly FlatToHierarchicalProgressMonitor monitor;
			private readonly int[] steps;
			private int currentStep = -1;
			private string currentStepName;

			public Steps(FlatToHierarchicalProgressMonitor monitor, int[] steps)
			{
				this.monitor = monitor;
				this.steps = steps;
			}

			private bool IsStarted
			{
				get { return currentStep >= 0; }
			}

			private string[] GetFullStepName()
			{
				return monitor.name.Concat(new[] { currentStepName })
					.ToArray();
			}

			private FlatToHierarchicalProgressMonitor GetRoot()
			{
				var cur = this;
				while (cur.monitor.parent != null)
					cur = cur.monitor.parent;
				return cur.monitor;
			}

			public void StartStep(string stepName = null)
			{
				if (currentStep + 1 >= steps.Length)
					throw new InvalidOperationException("All configured steps were already used");

				var wasStarted = IsStarted;
				string[] oldName = null;
				var onFinishedStep = GetRoot()
					.OnFinishedStep;

				if (wasStarted && onFinishedStep != null)
					oldName = GetFullStepName();

				currentStep = currentStep + 1;
				currentStepName = stepName ?? "";

				var newName = GetFullStepName();

				OnChildChange(0, 1, newName);

				if (wasStarted && onFinishedStep != null)
					onFinishedStep(oldName);

				var onOnStartedStep = GetRoot()
					.OnStartedStep;
				if (onOnStartedStep != null)
					onOnStartedStep(newName);
			}

			public void Finished()
			{
				if (!IsStarted)
					return;

				currentStep = steps.Length - 1;

				var fullName = GetFullStepName();
				var onFinishedStep = GetRoot()
					.OnFinishedStep;

				OnChildChange(1, 1, fullName);

				currentStep = steps.Length;

				if (onFinishedStep != null)
					onFinishedStep(fullName);
			}

			internal void OnChildChange(int current, int total, string[] stepName)
			{
				var parentPercentage = current / (double) total;

				var percentage = (steps.Take(currentStep)
					.Sum() + steps[currentStep] * parentPercentage) / steps.Sum();

				monitor.SetCurrent((int) Math.Round(percentage * 1000), 1000, stepName);
			}

			public void Dispose()
			{
				Finished();
			}

			public ProgressMonitor CreateSubMonitor()
			{
				if (!IsStarted)
					throw new InvalidOperationException("Not started yet");

				return new FlatToHierarchicalProgressMonitor(this, monitor, GetFullStepName());
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
		}
	}
}
