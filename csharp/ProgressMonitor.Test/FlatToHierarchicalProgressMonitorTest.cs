using Moq;
using NUnit.Framework;
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

			progress = new FlatToHierarchicalProgressMonitor(flat.Object);
		}

		[Test]
		public void TestStart()
		{
			progress.Start(1);

			flat.Verify(f => f.SetCurrent(0, 1000, ""));
		}

		[Test]
		public void TestFinished()
		{
			progress.Start(1);
			progress.Finished();

			flat.Verify(f => f.SetCurrent(1000, 1000, ""));
		}

		[Test]
		public void Test2Steps()
		{
			progress.Start(2);
			progress.NextStep();

			flat.Verify(f => f.SetCurrent(500, 1000, ""));
		}

		[Test]
		public void Test2Steps19()
		{
			progress.Start(1, 9);
			progress.NextStep();

			flat.Verify(f => f.SetCurrent(100, 1000, ""));
		}

		[Test]
		public void Test2Steps172()
		{
			progress.Start(1, 7, 2);
			progress.NextStep();
			progress.NextStep();

			flat.Verify(f => f.SetCurrent(800, 1000, ""));
		}

		[Test]
		public void TestStepName()
		{
			progress.Start(1);
			progress.SetStepName("A");

			flat.Verify(f => f.SetCurrent(0, 1000, "A"));
		}

		[Test]
		public void TestSubStep_Start()
		{
			progress.Start(2);
			progress.CreateSubMonitor()
				.Start(2);

			flat.Verify(f => f.SetCurrent(0, 1000, ""), Times.Once);
		}

		[Test]
		public void TestSubStep_FinishDoesntPropagate()
		{
			progress.Start(2);

			var sub = progress.CreateSubMonitor();
			sub.Start(2);
			sub.Finished();

			flat.Verify(f => f.SetCurrent(500, 1000, ""), Times.Never);
		}

		[Test]
		public void TestSubStep_StepPropagates()
		{
			progress.Start(2);

			var sub = progress.CreateSubMonitor();
			sub.Start(2);
			sub.NextStep();

			flat.Verify(f => f.SetCurrent(250, 1000, ""));
		}

		[Test]
		public void TestSubStep_SetStepNamePropagates()
		{
			progress.Start(2);
			progress.SetStepName("A");

			var sub = progress.CreateSubMonitor();
			sub.Start(2);
			sub.SetStepName("B");

			flat.Verify(f => f.SetCurrent(0, 1000, "A - B"));
		}
	}
}
