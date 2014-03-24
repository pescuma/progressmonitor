using System.Linq;
using System.Text.RegularExpressions;

namespace org.pescuma.progressmonitor.console
{
	public static class MachineReadableConsole
	{
		public static string ToConsole(int current, int total, string[] stepName)
		{
			return string.Format("[[PROGRESS {0}/{1} ({2})]]", current, total, string.Join("|", stepName.Select(n => n ?? "")
				.Select(m => m.Replace('|', '-'))));
		}

		private static Regex regex = new Regex(@"^\[\[PROGRESS (\d+)/(\d+) \((.*)\)\]\]$");

		public static StepData FromConsole(string line)
		{
			var ms = regex.Matches(line);
			if (ms.Count < 1)
				return null;

			var m = ms[0];
			return new StepData(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[1].Value), m.Groups[2].Value.Split('|'));
		}

		public class StepData
		{
			public readonly int Current;
			public readonly int Total;
			public readonly string[] StepName;

			public StepData(int current, int total, string[] stepName)
			{
				Current = current;
				Total = total;
				StepName = stepName;
			}
		}
	}
}
