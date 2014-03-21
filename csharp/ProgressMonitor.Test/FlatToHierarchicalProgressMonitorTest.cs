using Moq;
using NUnit.Framework;
using org.pescuma.progressmonitor.flat;
using org.pescuma.progressmonitor.simple;

namespace org.pescuma.progressmonitor
{
	[TestFixture]
	public class FlatToHierarchicalProgressMonitorTest
	{
		private Mock<FlatProgressMonitor> flat;
		private FlatToHierarchicalProgressMonitor progress;

		[SetUp]
		public void SetUp()
		{
			flat = new Mock<FlatProgressMonitor>();

			progress = new FlatToHierarchicalProgressMonitor(null, flat.Object);
		}

		[Test]
		public void TestConfigure()
		{
			progress.ConfigureSteps(1);

			flat.Verify(f => f.SetCurrent(0, 1000, ""), Times.Never);
		}

		[Test]
		public void TestStart()
		{
			progress.ConfigureSteps(1);
			progress.StartStep();

			flat.Verify(f => f.SetCurrent(0, 1000, ""));
		}

		[Test]
		public void TestFinished()
		{
			progress.ConfigureSteps(1);
			progress.Finished();

			flat.Verify(f => f.SetCurrent(1000, 1000, ""));
		}

		[Test]
		public void Test2Steps()
		{
			progress.ConfigureSteps(2);
			progress.StartStep();
			progress.StartStep();

			flat.Verify(f => f.SetCurrent(500, 1000, ""));
		}

		[Test]
		public void Test2Steps19()
		{
			progress.ConfigureSteps(1, 9);
			progress.StartStep();
			progress.StartStep();

			flat.Verify(f => f.SetCurrent(100, 1000, ""));
		}

		[Test]
		public void Test2Steps172()
		{
			progress.ConfigureSteps(1, 7, 2);
			progress.StartStep();
			progress.StartStep();
			progress.StartStep();

			flat.Verify(f => f.SetCurrent(800, 1000, ""));
		}

		[Test]
		public void TestStepName()
		{
			progress.ConfigureSteps(1);
			progress.StartStep("A");

			flat.Verify(f => f.SetCurrent(0, 1000, "A"));
		}

		[Test]
		public void TestSubStep_ConfigureDoesntPropagate()
		{
			progress.ConfigureSteps(2);
			progress.StartStep();

			var sub = progress.CreateSubMonitor();
			sub.ConfigureSteps(2);

			flat.Verify(f => f.SetCurrent(0, 1000, ""), Times.Once);
			flat.Verify(f => f.SetCurrent(500, 1000, ""), Times.Never);
		}

		[Test]
		public void TestSubStep_FinishDoesntPropagate()
		{
			progress.ConfigureSteps(2);
			progress.StartStep();

			var sub = progress.CreateSubMonitor();
			sub.ConfigureSteps(2);
			sub.Finished();

			flat.Verify(f => f.SetCurrent(500, 1000, ""), Times.Never);
		}

		[Test]
		public void TestSubStep_StepPropagates()
		{
			progress.ConfigureSteps(2);
			progress.StartStep();

			var sub = progress.CreateSubMonitor();
			sub.ConfigureSteps(2);
			sub.StartStep();
			sub.StartStep();

			flat.Verify(f => f.SetCurrent(250, 1000, ""));
		}

		[Test]
		public void TestSubStep_SetStepNamePropagates()
		{
			progress.ConfigureSteps(2);
			progress.StartStep("A");

			var sub = progress.CreateSubMonitor();
			sub.ConfigureSteps(2);
			sub.StartStep("B");

			flat.Verify(f => f.SetCurrent(0, 1000, "A - B"));
		}
	}
}
