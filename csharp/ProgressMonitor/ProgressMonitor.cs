using System;

namespace org.pescuma.progressmonitor
{
	public interface ProgressMonitor
	{
		/// <summary>
		/// Configure the number of steps. Must be called before StartStep
		/// </summary>
		/// <param name="steps">If one element: the number of steps. If more than one: the weighs for each step.</param>
		/// <returns>Disposable to stop this monitor. It can also be stopped by calling Finished()</returns>
		IDisposable ConfigureSteps(params int[] steps);

		/// <summary>
		/// Starts the next step.
		/// </summary>
		/// <param name="stepName">The next step name. Optional.</param>
		void StartStep(string stepName = null);

		/// <summary>
		/// Create a monitor for sub-steps of the current step.
		/// </summary>
		ProgressMonitor CreateSubMonitor();

		/// <summary>
		/// Reports an information to the user.
		/// This is NOT meant to logging errors, but for communicating with the user.
		/// </summary>
		/// <param name="message">A message. If more than one argument, it will be passed to string.Format</param>
		void Report(params string[] message);

		/// <summary>
		/// Reports the detais of an information to the user.
		/// This is NOT meant to logging errors, but for communicating with the user.
		/// </summary>
		/// <param name="message">A message. If more than one argument, it will be passed to string.Format</param>
		void ReportDetail(params string[] message);

		/// <summary>
		/// Reports a warning to the user.
		/// This is NOT meant to logging errors, but for communicating with the user.
		/// </summary>
		/// <param name="message">A message. If more than one argument, it will be passed to string.Format</param>
		void ReportWarning(params string[] message);

		/// <summary>
		/// Reports an error to the user.
		/// This is NOT meant to logging errors, but for communicating with the user.
		/// </summary>
		/// <param name="message">A message. If more than one argument, it will be passed to string.Format</param>
		void ReportError(params string[] message);

		void Finished();
	}
}
