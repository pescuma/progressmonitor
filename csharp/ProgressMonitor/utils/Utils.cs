using System;
using System.Linq;
using System.Text;

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

		public static bool ArrayEqual<T>(T[] a, T[] b)
		{
			if (a == null && b == null)
				return true;
			if (a == null || b == null)
				return false;
			if (a.Length != b.Length)
				return false;
			for (int i = 0; i < a.Length; i++)
				if (!Equals(a[i], b[i]))
					return false;
			return true;
		}

		public static string FormatSeconds(int seconds)
		{
			if (seconds < 1)
				return "0s";

			var result = new StringBuilder();
			Append(result, ref seconds, 60, "s");
			Append(result, ref seconds, 60, "m");
			Append(result, ref seconds, 24, "h");
			Append(result, ref seconds, 0, "d");
			return result.ToString();
		}

		private static void Append(StringBuilder result, ref int toGo, int step, string name)
		{
			if (toGo < 1)
				return;

			var val = toGo;

			if (step > 0)
			{
				val = toGo % step;
				toGo /= step;
			}
			else
			{
				toGo = 0;
			}

			if (result.Length > 0)
				result.Insert(0, " ");

			if (toGo > 0)
				result.Insert(0, string.Format("{0,2}{1}", val, name));
			else
				result.Insert(0, string.Format("{0}{1}", val, name));
		}

		// From http://stackoverflow.com/questions/3635564/greatest-common-divisor-from-a-set-of-more-than-2-integers
		public static int GetGCD(int a, int b)
		{
			if (a == 0)
				return b;

			while (b != 0)
			{
				var max = Math.Max(a, b);
				a = Math.Min(a, b);
				b = max % a;
			}

			return a;
		}
	}
}
