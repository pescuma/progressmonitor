using System;
using System.Collections.Generic;

namespace org.pescuma.progressmonitor
{
	/// <summary>
	/// A hierarchical progress monitor.
	/// </summary>
	public interface ProgressMonitor : ProgressReporter, ProgressCanceler
	{
		/// <summary>
		/// Configure the number of steps. Must be called before StartStep
		/// </summary>
		/// <param name="steps">If one element: the number of steps. If more than one: the weighs for each step.</param>
		/// <returns>IDisposable that calls Finished()</returns>
		IDisposable ConfigureSteps(params int[] steps);

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
		/// Finishes all the steps. Must be the last method called.
		/// </summary>
		void Finished();
	}

	public static class ProgressMonitorExtensions
	{
		/// <summary>
		/// Configure the steps based on all the elements of an enumeration.
		/// 
		/// If this enum is not a Collection, all of its elements will be cached to compute the size before the iteration.
		/// </summary>
		public static IEnumerable<T> Wrap<T>(this ProgressMonitor monitor, IEnumerable<T> list, Func<T, string> stepName = null)
		{
			if (stepName == null)
				stepName = a => null;

			var c = list as ICollection<T>;

			int size;
			if (c != null)
			{
				size = c.Count;
			}
			else
			{
				c = new List<T>();
				foreach (var el in list)
					c.Add(el);
				size = c.Count;
			}

			if (size > 0)
			{
				using (monitor.ConfigureSteps(size))
				{
					foreach (var el in c)
					{
						monitor.StartStep(stepName(el));

						yield return el;
					}
				}
			}
		}
	}
}
