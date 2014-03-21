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

		public FlatToHierarchicalProgressMonitor(string name, FlatProgressMonitor monitor)
		{
			this.prefix = name;
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

			currentStep = currentStep + 1;
			currentStepName = stepName;

			Propagate();
		}

		public void Finished()
		{
			CheckConfigured();

			steps = null;

			if (parent == null)
				Output(1, "");
		}

		public ProgressMonitor CreateSubMonitor()
		{
			CheckStarted();

			return new FlatToHierarchicalProgressMonitor(this, monitor);
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

		private void OnChildChange(double percentage, string name)
		{
			percentage = (steps.Take(currentStep)
				.Sum() + steps[currentStep] * percentage) / steps.Sum();

			if (parent == null)
				Output(percentage, name);
			else
				parent.OnChildChange(percentage, name);
		}

		private void Output(double percentage, string name)
		{
			monitor.SetCurrent((int) Math.Round(percentage * 1000), 1000, name);
		}

		private string GetFullStepName()
		{
			var names = new List<string>();
			var x = this;
			do
			{
				if (x.currentStepName != null)
					names.Add(x.currentStepName);
				if (x.prefix != null)
					names.Add(x.prefix);

				x = x.parent;
			} while (x != null);

			names.Reverse();

			return string.Join(" - ", names);
		}

		private void CheckNotConfigured()
		{
			if (steps != null)
				throw new InvalidOperationException("Already configured");
		}

		private void CheckConfigured()
		{
			if (steps == null)
				throw new InvalidOperationException("Not configured yet or already finished");
		}

		private void CheckStarted()
		{
			CheckConfigured();

			if (currentStep < 0)
				throw new InvalidOperationException("Not started yet");
		}

		public void Report(params string[] message)
		{
			monitor.Report(message);
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
