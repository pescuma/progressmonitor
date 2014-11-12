namespace org.pescuma.progressmonitor
{
	public interface ProgressCanceler
	{
		/// <summary>
		/// Indicates if someone requested the cancelation of this process.
		/// </summary>
		bool WasCanceled { get; }
	}
}
