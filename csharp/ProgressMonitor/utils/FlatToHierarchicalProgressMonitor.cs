using System;
using System.Linq;

namespace org.pescuma.progressmonitor.utils
{
	public class FlatToHierarchicalProgressMonitor : ProgressMonitor
	{
		private readonly FilteredFlatProgressMonitor flat;
		private readonly FlatToHierarchicalProgressMonitor parent;
		private readonly string[] name;

		private bool configured;
		private int[] steps;
		private int currentStep = -1;
		private string currentStepName;

		public event Action<string[]> AfterStartedStep;
		public event Action<string[]> BeforeFinishedStep;

		public bool DontOutputProgress
		{
			get { return flat.DontOutputProgress; }
			set { flat.DontOutputProgress = value; }
		}

		public bool DontOutputReports
		{
			get { return flat.DontOutputReports; }
			set { flat.DontOutputReports = value; }
		}

		public bool DontOutputReportDetails
		{
			get { return flat.DontOutputReportDetails; }
			set { flat.DontOutputReportDetails = value; }
		}

		public bool DontOutputReportWarnings
		{
			get { return flat.DontOutputReportWarnings; }
			set { flat.DontOutputReportWarnings = value; }
		}

		public bool DontOutputReportErrors
		{
			get { return flat.DontOutputReportErrors; }
			set { flat.DontOutputReportErrors = value; }
		}

		public FlatToHierarchicalProgressMonitor(FlatProgressMonitor flat)
			: this(null, flat)
		{
		}

		public FlatToHierarchicalProgressMonitor(string prefix, FlatProgressMonitor flat)
		{
			this.flat = new FilteredFlatProgressMonitor(flat);
			parent = null;

			if (!string.IsNullOrEmpty(prefix))
				name = new[] { prefix };
			else
				name = new string[0];
		}

		private FlatToHierarchicalProgressMonitor(FlatToHierarchicalProgressMonitor parent, string[] name)
		{
			flat = parent.flat;
			this.parent = parent;
			this.name = name;
		}

		public IDisposable ConfigureSteps(params int[] aSteps)
		{
			if (configured)
				throw new InvalidOperationException("Alteady configured");
			configured = true;

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

			return new ActionDisposable(() =>
			{
				if (HasStarted && !HasFinished)
					Finished();
			});
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
			return name.Concat(new[] { currentStepName })
				.ToArray();
		}

		private FlatToHierarchicalProgressMonitor GetRoot()
		{
			var cur = this;
			while (cur.parent != null)
				cur = cur.parent;
			return cur;
		}

		public void StartStep(string stepName = null)
		{
			if (currentStep + 1 >= steps.Length)
				throw new InvalidOperationException("All configured steps were already used");

			var root = GetRoot();

			if (HasStarted)
			{
				var onFinishedStep = root.BeforeFinishedStep;
				if (onFinishedStep != null)
					onFinishedStep(GetFullStepName());
			}

			currentStep++;
			currentStepName = stepName ?? "";

			var newName = GetFullStepName();

			OnChildChange(0, 1, newName);

			var onOnStartedStep = root.AfterStartedStep;
			if (onOnStartedStep != null)
				onOnStartedStep(newName);
		}

		public void Finished()
		{
			CheckRunning();

			var onFinishedStep = GetRoot()
				.BeforeFinishedStep;
			if (onFinishedStep != null)
				onFinishedStep(GetFullStepName());

			// Only usefull if this is the root. In the other cases the parent 
			// will finish it on the next call to StartStep.
			if (parent == null)
			{
				// We are really finishing the last step
				currentStep = steps.Length - 1;

				OnChildChange(1, 1, name);
			}

			currentStep = steps.Length;
		}

		internal void OnChildChange(int current, int total, string[] stepName)
		{
			var parentPercentage = current / (double) total;

			var percentage = (steps.Take(currentStep)
				.Sum() + steps[currentStep] * parentPercentage) / steps.Sum();

			// If not finished, pass on as not finished (to avoid problems in the flat monitor)
			var parentCurrent = (int) Math.Round(percentage * 1000);
			if (current < total)
				parentCurrent = Math.Min(999, parentCurrent);

			SetCurrent(parentCurrent, 1000, stepName);
		}

		private void SetCurrent(int current, int total, params string[] stepName)
		{
			if (parent == null)
				flat.SetCurrent(current, total, stepName);
			else
				parent.OnChildChange(current, total, stepName);
		}

		public ProgressMonitor CreateSubMonitor()
		{
			CheckRunning();

			return new FlatToHierarchicalProgressMonitor(this, GetFullStepName());
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
