using Moq;
using NUnit.Framework;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor
{
	[TestFixture]
	public class FlatToHierarchicalProgressMonitorTest
	{
		private Mock<FlatProgressMonitor> flat;
		private FlatToHierarchicalProgressMonitor progress;
		private int onStartedCalls;
		private int onFinishedCalls;

		[SetUp]
		public void SetUp()
		{
			flat = new Mock<FlatProgressMonitor>();

			progress = new FlatToHierarchicalProgressMonitor(null, flat.Object);

			onStartedCalls = 0;
			progress.OnStartedStep += n => onStartedCalls++;

			onFinishedCalls = 0;
			progress.OnFinishedStep += n => onFinishedCalls++;
		}

		[Test]
		public void TestConfigure()
		{
			progress.ConfigureSteps(1);

			flat.Verify(f => f.SetCurrent(0, 1000, null), Times.Never);
		}

		[Test]
		public void TestStart()
		{
			var steps = progress.ConfigureSteps(1);
			steps.StartStep();

			flat.Verify(f => f.SetCurrent(0, 1000, ""));
		}

		[Test]
		public void Test2Steps()
		{
			var steps = progress.ConfigureSteps(2);
			steps.StartStep();
			steps.StartStep();

			flat.Verify(f => f.SetCurrent(500, 1000, ""));
		}

		[Test]
		public void Test2Steps19()
		{
			var steps = progress.ConfigureSteps(1, 9);
			steps.StartStep();
			steps.StartStep();

			flat.Verify(f => f.SetCurrent(100, 1000, ""));
		}

		[Test]
		public void Test2Steps172()
		{
			var steps = progress.ConfigureSteps(1, 7, 2);
			steps.StartStep();
			steps.StartStep();
			steps.StartStep();

			flat.Verify(f => f.SetCurrent(800, 1000, ""));
		}

		[Test]
		public void TestDispose_NotStarted()
		{
			var steps = progress.ConfigureSteps(1);
			steps.Dispose();

			flat.Verify(f => f.SetCurrent(1000, 1000, ""), Times.Never);
		}

		[Test]
		public void TestFinished_UsedAll()
		{
			var steps = progress.ConfigureSteps(1);
			steps.StartStep();
			steps.Finished();

			flat.Verify(f => f.SetCurrent(1000, 1000));
		}

		[Test]
		public void TestFinished_NotUsedAll()
		{
			var steps = progress.ConfigureSteps(10);
			steps.StartStep();
			steps.Finished();

			flat.Verify(f => f.SetCurrent(1000, 1000));
		}

		[Test]
		public void TestStepName()
		{
			var steps = progress.ConfigureSteps(1);
			steps.StartStep("A");

			flat.Verify(f => f.SetCurrent(0, 1000, "A"));
		}

		[Test]
		public void TestSubStep_ConfigureDoesntPropagate()
		{
			var steps = progress.ConfigureSteps(2);
			steps.StartStep();

			var sub = steps.CreateSubMonitor();
			sub.ConfigureSteps(2);

			flat.Verify(f => f.SetCurrent(0, 1000, ""), Times.Once);
			flat.Verify(f => f.SetCurrent(500, 1000, ""), Times.Never);
		}

		[Test]
		public void TestSubStep_DisposeDoesntPropagate()
		{
			var steps = progress.ConfigureSteps(2);
			steps.StartStep();

			var sub = steps.CreateSubMonitor()
				.ConfigureSteps(2);
			sub.StartStep();
			sub.Finished();

			flat.Verify(f => f.SetCurrent(500, 1000, "", ""), Times.Never);
			flat.Verify(f => f.SetCurrent(500, 1000, ""), Times.Never);
		}

		[Test]
		public void TestSubStep_StepPropagates()
		{
			var steps = progress.ConfigureSteps(2);
			steps.StartStep();

			var sub = steps.CreateSubMonitor()
				.ConfigureSteps(2);
			sub.StartStep();
			sub.StartStep();

			flat.Verify(f => f.SetCurrent(250, 1000, "", ""));
		}

		[Test]
		public void TestSubStep_SetStepNamePropagates()
		{
			var steps = progress.ConfigureSteps(2);
			steps.StartStep("A");

			var sub = steps.CreateSubMonitor()
				.ConfigureSteps(2);
			sub.StartStep("B");

			flat.Verify(f => f.SetCurrent(0, 1000, "A", "B"));
		}

		[Test]
		public void TestOnStarted_NotCalledOnConfigure()
		{
			progress.ConfigureSteps(2);

			Assert.AreEqual(0, onStartedCalls);
		}

		[Test]
		public void TestOnStarted_CalledOnFirstStart()
		{
			var steps = progress.ConfigureSteps(2);
			steps.StartStep();

			Assert.AreEqual(1, onStartedCalls);
		}

		[Test]
		public void TestOnStarted_CalledOnMultipleStart()
		{
			var steps = progress.ConfigureSteps(2);
			steps.StartStep();
			steps.StartStep();

			Assert.AreEqual(2, onStartedCalls);
		}

		[Test]
		public void TestOnStarted_NotCalledOnFinish()
		{
			var steps = progress.ConfigureSteps(2);
			steps.StartStep();
			steps.StartStep();
			steps.Finished();

			Assert.AreEqual(2, onStartedCalls);
		}

		[Test]
		public void TestOnFinished_NotCalledOnStart()
		{
			var steps = progress.ConfigureSteps(2);
			steps.StartStep();

			Assert.AreEqual(0, onFinishedCalls);
		}

		[Test]
		public void TestOnFinished_CalledOnMulitpleStarts()
		{
			var steps = progress.ConfigureSteps(2);
			steps.StartStep();
			steps.StartStep();

			Assert.AreEqual(1, onFinishedCalls);
		}

		[Test]
		public void TestOnFinished_CalledOnFinish()
		{
			var steps = progress.ConfigureSteps(2);
			steps.StartStep();
			steps.StartStep();
			steps.Finished();

			Assert.AreEqual(2, onFinishedCalls);
		}
	}
}
