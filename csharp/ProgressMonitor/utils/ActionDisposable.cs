using System;

namespace org.pescuma.progressmonitor.utils
{
	internal class ActionDisposable : IDisposable
	{
		private readonly Action dispose;

		public ActionDisposable(Action dispose)
		{
			this.dispose = dispose;
		}

		public void Dispose()
		{
			dispose();
		}
	}
}
