namespace org.pescuma.progressmonitor.flat
{
	/// <summary>
	/// A simple progress monitor, that doesn't allow sub-steps.
	/// 
	/// Usually used internally by progress monitors.
	/// </summary>
	public interface FlatProgressMonitor
	{
		/// <summary>
		/// Sets the current process stage.
		/// The process will be finished when current >= total.
		/// </summary>
		void SetCurrent(int current, int total, string stepName);

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
	}
}
