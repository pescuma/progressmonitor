using System.Diagnostics;

namespace org.pescuma.progressmonitor.console.widget
{
	[DebuggerDisplay("{Min}-{Max} grow:{GrowToUseEmptySpace}")]
	public class AcceptableSizes
	{
		public readonly int Min;
		public readonly int Max;
		public readonly bool GrowToUseEmptySpace;

		public AcceptableSizes(int min, int max, bool growToUseEmptySpace)
		{
			Min = min;
			Max = max;
			GrowToUseEmptySpace = growToUseEmptySpace;
		}

		public static readonly AcceptableSizes Empty = new AcceptableSizes(0, 0, false);

		protected bool Equals(AcceptableSizes other)
		{
			return Min == other.Min && Max == other.Max && GrowToUseEmptySpace == other.GrowToUseEmptySpace;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((AcceptableSizes) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = Min;
				hashCode = (hashCode * 397) ^ Max;
				hashCode = (hashCode * 397) ^ GrowToUseEmptySpace.GetHashCode();
				return hashCode;
			}
		}
	}
}
