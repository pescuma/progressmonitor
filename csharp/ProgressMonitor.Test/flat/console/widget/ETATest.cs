using System;
using NUnit.Framework;
using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor.flat.console.widget
{
    [TestFixture]
    public class ETATest
    {
        [Test]
        public void TestS()
        {
            var elapsedTime = new TimeSpan(0, 0, 0, 1).TotalMilliseconds / 1000;
            var totalTime = elapsedTime / 0.1;
            var toGo = (int) Math.Round(totalTime - elapsedTime);
            Assert.AreEqual("9s", Utils.FormatSeconds(toGo));
        }

        [Test]
        public void TestM()
        {
            var elapsedTime = new TimeSpan(0, 0, 1, 0).TotalMilliseconds / 1000;
            var totalTime = elapsedTime / 0.1;
            var toGo = (int) Math.Round(totalTime - elapsedTime);
            Assert.AreEqual("9m  0s", Utils.FormatSeconds(toGo));
        }

        [Test]
// ReSharper disable once InconsistentNaming
        public void TestMS()
        {
            var elapsedTime = new TimeSpan(0, 0, 0, 10).TotalMilliseconds / 1000;
            var totalTime = elapsedTime / 0.1;
            var toGo = (int) Math.Round(totalTime - elapsedTime);
            Assert.AreEqual("1m 30s", Utils.FormatSeconds(toGo));
        }

        [Test]
        public void TestH()
        {
            var elapsedTime = new TimeSpan(0, 1, 0, 0).TotalMilliseconds / 1000;
            var totalTime = elapsedTime / 0.1;
            var toGo = (int) Math.Round(totalTime - elapsedTime);
            Assert.AreEqual("9h  0m  0s", Utils.FormatSeconds(toGo));
        }

        [Test]
// ReSharper disable once InconsistentNaming
        public void TestHMS()
        {
            var elapsedTime = new TimeSpan(0, 1, 1, 1).TotalMilliseconds / 1000;
            var totalTime = elapsedTime / 0.1;
            var toGo = (int) Math.Round(totalTime - elapsedTime);
            Assert.AreEqual("9h  9m  9s", Utils.FormatSeconds(toGo));
        }

        [Test]
// ReSharper disable once InconsistentNaming
        public void TestHS()
        {
            var elapsedTime = new TimeSpan(0, 1, 0, 1).TotalMilliseconds / 1000;
            var totalTime = elapsedTime / 0.1;
            var toGo = (int) Math.Round(totalTime - elapsedTime);
            Assert.AreEqual("9h  0m  9s", Utils.FormatSeconds(toGo));
        }

        [Test]
        public void TestD()
        {
            var elapsedTime = new TimeSpan(1, 0, 0, 0).TotalMilliseconds / 1000;
            var totalTime = elapsedTime / 0.1;
            var toGo = (int) Math.Round(totalTime - elapsedTime);
            Assert.AreEqual("9d  0h  0m  0s", Utils.FormatSeconds(toGo));
        }

        [Test]
// ReSharper disable once InconsistentNaming
        public void TestDHMS()
        {
            var elapsedTime = new TimeSpan(1, 1, 1, 1).TotalMilliseconds / 1000;
            var totalTime = elapsedTime / 0.1;
            var toGo = (int) Math.Round(totalTime - elapsedTime);
            Assert.AreEqual("9d  9h  9m  9s", Utils.FormatSeconds(toGo));
        }
    }
}
