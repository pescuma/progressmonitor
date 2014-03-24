using System;
using System.Linq;

namespace org.pescuma.progressmonitor.utils
{
	public class FlatToHierarchicalProgressMonitor : ProgressMonitor
	{
		private readonly FlatProgressMonitor flat;
		private readonly string[] name;
		private readonly Steps parent;
		private bool configured;

		public event Action<string[]> OnStartedStep;
		public event Action<string[]> OnFinishedStep;

		public FlatToHierarchicalProgressMonitor(FlatProgressMonitor flat)
			: this(null, flat)
		{
		}

		public FlatToHierarchicalProgressMonitor(string prefix, FlatProgressMonitor flat)
		{
			this.flat = flat;

			if (!string.IsNullOrEmpty(prefix))
				name = new[] { prefix };
			else
				name = new string[0];
		}

		private FlatToHierarchicalProgressMonitor(Steps parent, FlatProgressMonitor flat, string[] name)
		{
			this.flat = flat;
			this.name = name;
			this.parent = parent;
		}

		public ProgressSteps ConfigureSteps(params int[] steps)
		{
			if (configured)
				throw new InvalidOperationException("Alteady configured");
			configured = true;

			if (steps.Length < 1)
				throw new ArgumentException();
			if (steps.Any(v => v < 1))
				throw new ArgumentException();

			if (steps.Length == 1)
			{
				steps = new int[steps[0]];
				for (var i = 0; i < steps.Length; i++)
					steps[i] = 1;
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

			private bool HasStarted
			{
				get { return currentStep >= 0; }
			}

			private bool HasFinished
			{
				get { return currentStep >= steps.Length; }
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

				var root = GetRoot();

				if (HasStarted)
				{
					var onFinishedStep = root.OnFinishedStep;
					if (onFinishedStep != null)
						onFinishedStep(GetFullStepName());
				}

				currentStep = currentStep + 1;
				currentStepName = stepName ?? "";

				var newName = GetFullStepName();

				OnChildChange(0, 1, newName);

				var onOnStartedStep = root.OnStartedStep;
				if (onOnStartedStep != null)
					onOnStartedStep(newName);
			}

			public void Finished()
			{
				CheckRunning();

				var onFinishedStep = GetRoot()
					.OnFinishedStep;
				if (onFinishedStep != null)
					onFinishedStep(GetFullStepName());

				// The only usefull if the root one, in the other cases the parent 
				// will finish it on the next call to StartStep
				if (monitor.parent == null)
				{
					// We are really finishing the last step
					currentStep = steps.Length - 1;

					OnChildChange(1, 1, monitor.name);
				}

				currentStep = steps.Length;
			}

			public void Dispose()
			{
				if (HasStarted && !HasFinished)
					Finished();
			}

			internal void OnChildChange(int current, int total, string[] stepName)
			{
				var parentPercentage = current / (double) total;

				var percentage = (steps.Take(currentStep)
					.Sum() + steps[currentStep] * parentPercentage) / steps.Sum();

				monitor.SetCurrent((int) Math.Round(percentage * 1000), 1000, stepName);
			}

			public ProgressMonitor CreateSubMonitor()
			{
				CheckRunning();

				return new FlatToHierarchicalProgressMonitor(this, monitor, GetFullStepName());
			}

			private void CheckRunning()
			{
				if (!HasStarted)
					throw new InvalidOperationException("Not started yet");
				if (HasFinished)
					throw new InvalidOperationException("Already finished");
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
