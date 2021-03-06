﻿using Moq;
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
			progress.AfterStartedStep += n => onStartedCalls++;

			onFinishedCalls = 0;
			progress.BeforeFinishedStep += n => onFinishedCalls++;
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
			progress.ConfigureSteps(1);
			progress.StartStep();

			flat.Verify(f => f.SetCurrent(0, 1000, ""));
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
		public void TestDispose_NotStarted()
		{
			var disp = progress.ConfigureSteps(1);
			disp.Dispose();

			flat.Verify(f => f.SetCurrent(1000, 1000, ""), Times.Never);
		}

		[Test]
		public void TestFinished_UsedAll()
		{
			progress.ConfigureSteps(1);
			progress.StartStep();
			progress.Finished();

			flat.Verify(f => f.SetCurrent(1000, 1000));
		}

		[Test]
		public void TestFinished_NotUsedAll()
		{
			progress.ConfigureSteps(10);
			progress.StartStep();
			progress.Finished();

			flat.Verify(f => f.SetCurrent(1000, 1000));
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
		public void TestSubStep_DisposeDoesntPropagate()
		{
			progress.ConfigureSteps(2);
			progress.StartStep();

			var sub = progress.CreateSubMonitor();
			sub.ConfigureSteps(2);
			sub.StartStep();
			sub.Finished();

			flat.Verify(f => f.SetCurrent(500, 1000, "", ""), Times.Never);
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

			flat.Verify(f => f.SetCurrent(250, 1000, "", ""));
		}

		[Test]
		public void TestSubStep_SetStepNamePropagates()
		{
			progress.ConfigureSteps(2);
			progress.StartStep("A");

			var sub = progress.CreateSubMonitor();
			sub.ConfigureSteps(2);
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
			progress.ConfigureSteps(2);
			progress.StartStep();

			Assert.AreEqual(1, onStartedCalls);
		}

		[Test]
		public void TestOnStarted_CalledOnMultipleStart()
		{
			progress.ConfigureSteps(2);
			progress.StartStep();
			progress.StartStep();

			Assert.AreEqual(2, onStartedCalls);
		}

		[Test]
		public void TestOnStarted_NotCalledOnFinish()
		{
			progress.ConfigureSteps(2);
			progress.StartStep();
			progress.StartStep();
			progress.Finished();

			Assert.AreEqual(2, onStartedCalls);
		}

		[Test]
		public void TestOnFinished_NotCalledOnStart()
		{
			progress.ConfigureSteps(2);
			progress.StartStep();

			Assert.AreEqual(0, onFinishedCalls);
		}

		[Test]
		public void TestOnFinished_CalledOnMulitpleStarts()
		{
			progress.ConfigureSteps(2);
			progress.StartStep();
			progress.StartStep();

			Assert.AreEqual(1, onFinishedCalls);
		}

		[Test]
		public void TestOnFinished_CalledOnFinish()
		{
			progress.ConfigureSteps(2);
			progress.StartStep();
			progress.StartStep();
			progress.Finished();

			Assert.AreEqual(2, onFinishedCalls);
		}
	}
}
