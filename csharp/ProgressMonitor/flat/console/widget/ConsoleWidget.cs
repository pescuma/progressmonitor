using System;

namespace org.pescuma.progressmonitor.flat.console.widget
{
	public interface ConsoleWidget
	{
		void Started();

		bool Grow { get; }

		int ComputeSize(int current, int total, double percent, string[] stepName);

		void Output(Action<string> writer, int width, int current, int total, double percent, string[] stepName);
	}
}
