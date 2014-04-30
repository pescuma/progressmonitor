using System;
using System.Collections.Generic;

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
				size = c.Count;
			else
			{
				c = new List<T>();
				foreach (var el in list)
					c.Add(el);
				size = c.Count;
			}

			if (size > 0)
			{
				var steps = monitor.ConfigureSteps(size);

				foreach (var el in c)
				{
					steps.StartStep(stepName(el));
					yield return el;
				}
			}
		}
	}
}
