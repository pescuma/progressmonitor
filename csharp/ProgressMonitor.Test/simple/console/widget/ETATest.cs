using System;
using NUnit.Framework;

namespace org.pescuma.progressmonitor.simple.console.widget
{
	[TestFixture]
	public class ETATest
	{
		[Test]
		public void TestS()
		{
			Assert.AreEqual("9s", ETA.ComputeETA(new TimeSpan(0, 0, 0, 1), 10));
		}

		[Test]
		public void TestM()
		{
			Assert.AreEqual("9m", ETA.ComputeETA(new TimeSpan(0, 0, 1, 0), 10));
		}

		[Test]
// ReSharper disable once InconsistentNaming
		public void TestMS()
		{
			Assert.AreEqual("1m 30s", ETA.ComputeETA(new TimeSpan(0, 0, 0, 10), 10));
		}

		[Test]
		public void TestH()
		{
			Assert.AreEqual("9h", ETA.ComputeETA(new TimeSpan(0, 1, 0, 0), 10));
		}

		[Test]
// ReSharper disable once InconsistentNaming
		public void TestHMS()
		{
			Assert.AreEqual("9h 9m 9s", ETA.ComputeETA(new TimeSpan(0, 1, 1, 1), 10));
		}

		[Test]
// ReSharper disable once InconsistentNaming
		public void TestHS()
		{
			Assert.AreEqual("9h 9s", ETA.ComputeETA(new TimeSpan(0, 1, 0, 1), 10));
		}

		[Test]
		public void TestD()
		{
			Assert.AreEqual("9d", ETA.ComputeETA(new TimeSpan(1, 0, 0, 0), 10));
		}

		[Test]
// ReSharper disable once InconsistentNaming
		public void TestDHMS()
		{
			Assert.AreEqual("9d 9h 9m 9s", ETA.ComputeETA(new TimeSpan(1, 1, 1, 1), 10));
		}
	}
}
