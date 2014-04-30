namespace org.pescuma.progressmonitor
{
	/// <summary>
	/// A simple progress monitor that doesn't allow sub-steps.
	/// 
	/// Usually used internally by progress monitors.
	/// </summary>
	public interface FlatProgressMonitor : ProgressReporter
	{
		/// <summary>
		/// Sets the current process stage.
		/// The process will be finished when current >= total.
		/// </summary>
		/// <param name="current"></param>
		/// <param name="total"></param>
		/// <param name="stepName">If multiple step names are be used, they have a parent-child relationship</param>
		void SetCurrent(int current, int total, params string[] stepName);
	}
}
