using org.pescuma.progressmonitor.utils;

namespace org.pescuma.progressmonitor
{
    [TestFixture]
    public class UtilsTest
    {
        private void AssertGDC(int expected, int a, int b)
        {
            Assert.AreEqual(expected, Utils.GetGCD(a, b));
            Assert.AreEqual(expected, Utils.GetGCD(b, a));
        }

        [Test]
        public void Test10()
        {
            AssertGDC(1, 1, 0);
        }

        [Test]
        public void Test24()
        {
            AssertGDC(2, 2, 4);
        }

        [Test]
        public void Test23()
        {
            AssertGDC(1, 2, 3);
        }

        [Test]
        public void Test104()
        {
            AssertGDC(2, 10, 4);
        }
    }
}
