using System;
using System.Linq;

namespace org.pescuma.progressmonitor.utils
{
	public class FlatToHierarchicalProgressMonitor : ProgressMonitor
	{
		private const int TOTAL_STEPS_FOR_FLAT = 1000;

		private readonly FilteredFlatProgressMonitor flat;
		private readonly FlatToHierarchicalProgressMonitor parent;
		private readonly string[] fullStepName;

		private int[] steps;
		private int currentStep = -1;
		private int lastStartStepTickCount;

		// Cached data
		private int stepsSum;
		private int currentStepSum;

		private string CurrentStepName
		{
			get { return fullStepName[fullStepName.Length - 1]; }
			set { fullStepName[fullStepName.Length - 1] = value; }
		}

		private int? MinOutupWaitInMs
		{
			get
			{
				var throughput = flat.Original as MaxThroughputProgressMonitor;
				if (throughput == null)
					return null;
				else
					return throughput.MinOutupWaitInMs;
			}
		}

		protected FlatProgressMonitor Flat
		{
			get { return flat.Original; }
		}

		public FlatToHierarchicalProgressMonitor(string prefix, FlatProgressMonitor flat)
		{
			parent = null;
			this.flat = new FilteredFlatProgressMonitor(flat);

			if (!string.IsNullOrEmpty(prefix))
				fullStepName = new[] { prefix, null };
			else
				fullStepName = new string[] { null };
		}

		private FlatToHierarchicalProgressMonitor(FlatToHierarchicalProgressMonitor parent, string[] name)
		{
			this.parent = parent;
			flat = parent.flat;
			fullStepName = Utils.ArrayAppendEmpty(name, 1);
		}

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

		public IDisposable ConfigureSteps(params int[] aSteps)
		{
			if (WasConfigured)
				throw new InvalidOperationException("Already configured");

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

			stepsSum = steps.Sum();

			return new ActionDisposable(() =>
			{
				if (HasStarted && !HasFinished)
					Finished();
			});
		}

		private bool WasConfigured
		{
			get { return steps != null; }
		}

		private bool HasStarted
		{
			get { return currentStep >= 0; }
		}

		private bool HasFinished
		{
			get { return currentStep >= steps.Length; }
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
			{
				ReportDetail("[ProgressMonitor] All configured steps were already used (inside " + string.Join(" ", fullStepName)
					.Trim() + ")");
				return;
			}

			stepName = stepName ?? "";

			var hasStarted = HasStarted;

			var minWait = MinOutupWaitInMs;
			if (minWait != null)
			{
				var tickCount = Environment.TickCount;

				if (hasStarted && CurrentStepName == stepName && lastStartStepTickCount - tickCount > minWait.Value)
				{
					currentStepSum += steps[currentStep];
					currentStep++;
					return;
				}

				lastStartStepTickCount = tickCount;
			}

			var root = GetRoot();

			if (hasStarted)
			{
				var onFinishedStep = root.BeforeFinishedStep;
				if (onFinishedStep != null)
					onFinishedStep(fullStepName);

				currentStepSum += steps[currentStep];
			}
			currentStep++;
			CurrentStepName = stepName;

			var newName = fullStepName;

			OnChildChange(0, newName);

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
				onFinishedStep(fullStepName);

			currentStep = steps.Length;

			// Only need to set current if this is the root. In the other cases the parent 
			// will finish it on the next call to StartStep.
			if (parent == null)
				flat.SetCurrent(TOTAL_STEPS_FOR_FLAT, TOTAL_STEPS_FOR_FLAT, fullStepName.Take(fullStepName.Length - 1)
					.ToArray());
		}

		internal void OnChildChange(float parentPercentage, string[] stepName)
		{
			var percentage = (currentStepSum + steps[currentStep] * parentPercentage) / stepsSum;

			if (parent == null)
			{
				var current = Math.Min((int) (percentage * TOTAL_STEPS_FOR_FLAT), TOTAL_STEPS_FOR_FLAT - 1);
				flat.SetCurrent(current, TOTAL_STEPS_FOR_FLAT, stepName);
			}
			else
			{
				parent.OnChildChange(percentage, stepName);
			}
		}

		public ProgressMonitor CreateSubMonitor()
		{
			CheckRunning();

			return new FlatToHierarchicalProgressMonitor(this, fullStepName);
		}

		private void CheckRunning()
		{
			if (!HasStarted)
				throw new InvalidOperationException("Not started yet");
			if (HasFinished)
				throw new InvalidOperationException("Already finished");
		}

		public void Report(string message, params object[] args)
		{
			flat.Report(message, args);
		}

		public void ReportDetail(string message, params object[] args)
		{
			flat.ReportDetail(message, args);
		}

		public void ReportWarning(string message, params object[] args)
		{
			flat.ReportWarning(message, args);
		}

		public void ReportError(string message, params object[] args)
		{
			flat.ReportError(message, args);
		}

		public bool WasCanceled
		{
			get { return flat.WasCanceled; }
		}
	}
}
