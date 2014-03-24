using System;
using System.Linq;

namespace org.pescuma.progressmonitor.utils
{
	internal static class Utils
	{
		public static string Format(string[] message)
		{
			if (message.Length < 1)
				return "";
			else if (message.Length == 1)
				return message[0];
			else
// ReSharper disable once RedundantCast
				return string.Format(message[0], (string[]) message.Skip(1)
					.ToArray());
		}

		public static void ConsoleWriteLine(string message, ConsoleColor? color)
		{
			var old = ConsoleColor.Gray;
			if (color != null)
			{
				old = Console.ForegroundColor;
				Console.ForegroundColor = color.Value;
			}

			Console.WriteLine(message);

			if (color != null)
				Console.ForegroundColor = old;
		}
	}
}
