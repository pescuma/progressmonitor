using System;
using System.Linq;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.devel
{
	public class ComputeRelativeTimesProgressMonitor : ProgressMonitor
	{
		private readonly ProgressMonitor next;

		public ComputeRelativeTimesProgressMonitor(ProgressMonitor next)
		{
			this.next = next;
		}

		public void SetCurrent(int current, int total, params string[] stepName)
		{
			next.SetCurrent(current, total, stepName);
		}

		public ProgressSteps ConfigureSteps(params int[] steps)
		{
			return new Steps(steps.Length == 1 ? steps[0] : steps.Length, next.ConfigureSteps(steps));
		}

		public void Report(params string[] message)
		{
			next.Report(message);
		}

		public void ReportDetail(params string[] message)
		{
			next.ReportDetail(message);
		}

		public void ReportWarning(params string[] message)
		{
			next.ReportWarning(message);
		}

		public void ReportError(params string[] message)
		{
			next.ReportError(message);
		}

		private class Steps : ProgressSteps
		{
			private readonly ProgressSteps next;
			private Step[] steps;
			private int currentStep = -1;
			private DateTime start;
			private bool finished;

			public Steps(int steps, ProgressSteps next)
			{
				this.steps = new Step[steps];
				this.next = next;
			}

			public void StartStep(string stepName = null)
			{
				OnFinishedStep();

				next.StartStep(stepName);

				currentStep++;
				steps[currentStep].Name = stepName;
				start = DateTime.Now;
			}

			private void OnFinishedStep()
			{
				if (currentStep >= 0)
					steps[currentStep].Size = (int) Math.Round((DateTime.Now - start).TotalMilliseconds);
			}

			public ProgressMonitor CreateSubMonitor()
			{
				return next.CreateSubMonitor();
			}

			public void Finished()
			{
				OnFinishedStep();

				next.Finished();

				DumpTimes();

				finished = true;
			}

			public void Dispose()
			{
				if (!finished)
					Finished();
			}

			private void DumpTimes()
			{
				var max = steps.Select(s => s.Size)
					.Aggregate(Math.Max);
				var factor = 100.0 / max;

				for (int i = 0; i < steps.Length; i++)
					steps[i].Size = (int) Math.Max(Math.Round(steps[i].Size * factor), 1);

				var gcd = steps.Select(s => s.Size)
					.Aggregate(Utils.GetGCD);

				for (int i = 0; i < steps.Length; i++)
					steps[i].Size = steps[i].Size / gcd;

				for (int i = 0; i < steps.Length; i++)
					Console.WriteLine("Step {0} - {1} : {2}", i, steps[i].Name, steps[i].Size);
			}

			public void Report(params string[] message)
			{
				next.Report(message);
			}

			public void ReportDetail(params string[] message)
			{
				next.ReportDetail(message);
			}

			public void ReportWarning(params string[] message)
			{
				next.ReportWarning(message);
			}

			public void ReportError(params string[] message)
			{
				next.ReportError(message);
			}

			private struct Step
			{
				public int Size;
				public string Name;
			}
		}
	}
}
