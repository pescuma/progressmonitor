using System;
using System.Diagnostics;
using System.Linq;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.devel
{
    public class ComputeRelativeTimesProgressMonitor : ProgressMonitor
    {
        private readonly ProgressMonitor next;
        private readonly Action<string> writer;
        private Step[] steps;
        private int currentStep = -1;
        private DateTime start;

        public ComputeRelativeTimesProgressMonitor(ProgressMonitor next, Action<string> writer = null)
        {
            if (writer == null)
                writer = s => Debug.WriteLine(s);

            this.next = next;
            this.writer = writer;
        }

        public IDisposable ConfigureSteps(params int[] aSteps)
        {
            steps = new Step[aSteps.Length == 1 ? aSteps[0] : aSteps.Length];

            next.ConfigureSteps(aSteps);

            return new ActionDisposable(() =>
            {
                if (currentStep >= 0 && currentStep < steps.Length)
                    Finished();
            });
        }

        public void StartStep(string stepName = null, params object[] args)
        {
            if (currentStep >= 0)
                OnFinishedStep();

            next.StartStep(stepName, args);

            currentStep++;
            steps[currentStep]
                    .Name = Utils.Format(stepName ?? "", args);
            start = DateTime.Now;
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

            currentStep++;
        }

        private void OnFinishedStep()
        {
            steps[currentStep]
                    .Size = (int) Math.Round((DateTime.Now - start).TotalMilliseconds);
        }

        private void DumpTimes()
        {
            var max = steps.Select(s => s.Size)
                           .Aggregate(Math.Max);
            var factor = 100.0 / max;

            for (int i = 0; i < steps.Length; i++)
                steps[i]
                        .Size = (int) Math.Max(Math.Round(steps[i]
                                                                  .Size
                                                          * factor), 1);

            var gcd = steps.Select(s => s.Size)
                           .Aggregate(Utils.GetGCD);

            for (int i = 0; i < steps.Length; i++)
                steps[i]
                                .Size = steps[i]
                                                .Size
                                        / gcd;

            writer(string.Join("\n", steps.Select((s, i) => string.Format("Step {0} - {1} : {2}", i, s.Name, s.Size))));
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

        private struct Step
        {
            public int Size;
            public string Name;
        }
    }
}
