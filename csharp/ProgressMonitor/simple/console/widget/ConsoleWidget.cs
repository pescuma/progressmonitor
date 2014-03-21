namespace org.pescuma.progressmonitor.simple.console.widget
{
	internal interface ConsoleWidget
	{
		void Started();

		bool Grow { get; }

		int ComputeSize(int current, int total, float percent, string stepName);

		int OutputToConsole(int width, int current, int total, float percent, string stepName);
	}
}
