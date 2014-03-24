using System;

namespace org.pescuma.progressmonitor
{
	public interface ProgressSteps : ProgressReporter, IDisposable
	{
		/// <summary>
		/// Finishes the previous step and starts the next one.
		/// </summary>
		/// <param name="stepName">The next step name. Optional.</param>
		void StartStep(string stepName = null);

		/// <summary>
		/// Create a monitor for sub-steps of the current step.
		/// </summary>
		ProgressMonitor CreateSubMonitor();

		/// <summary>
		/// The same as calling Dispose()
		/// </summary>
		void Finished();
	}
}
