namespace org.pescuma.progressmonitor
{
	public interface ProgressMonitor : FlatProgressMonitor
	{
		/// <summary>
		/// Configure the number of steps. Must be called before StartStep
		/// </summary>
		/// <param name="steps">If one element: the number of steps. If more than one: the weighs for each step.</param>
		/// <returns>Disposable to stop this monitor. It can also be stopped by calling Finished()</returns>
		ProgressSteps ConfigureSteps(params int[] steps);
	}
}
