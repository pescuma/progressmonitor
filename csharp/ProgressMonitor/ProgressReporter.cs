namespace org.pescuma.progressmonitor
{
	public interface ProgressReporter
	{
		/// <summary>
		/// Reports an information to the user.
		/// This is NOT meant to logging errors, but for communicating with the user.
		/// </summary>
		/// <param name="message">A message.</param>
		/// <param name="args">If at least one argument, the message will be passed to string.Format</param>
		void Report(string message, params object[] args);

		/// <summary>
		/// Reports the detais of an information to the user.
		/// This is NOT meant to logging errors, but for communicating with the user.
		/// </summary>
		/// <param name="message">A message. If more than one argument, it will be passed to string.Format</param>
		/// <param name="args">If at least one argument, the message will be passed to string.Format</param>
		void ReportDetail(string message, params object[] args);

		/// <summary>
		/// Reports a warning to the user.
		/// This is NOT meant to logging errors, but for communicating with the user.
		/// </summary>
		/// <param name="message">A message. If more than one argument, it will be passed to string.Format</param>
		/// <param name="args">If at least one argument, the message will be passed to string.Format</param>
		void ReportWarning(string message, params object[] args);

		/// <summary>
		/// Reports an error to the user.
		/// This is NOT meant to logging errors, but for communicating with the user.
		/// </summary>
		/// <param name="message">A message. If more than one argument, it will be passed to string.Format</param>
		/// <param name="args">If at least one argument, the message will be passed to string.Format</param>
		void ReportError(string message, params object[] args);
	}
}
