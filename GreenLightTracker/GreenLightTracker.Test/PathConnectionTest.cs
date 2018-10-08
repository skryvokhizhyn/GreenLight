using NUnit.Framework;
using GreenLightTracker.Src;

namespace GreenLightTracker.Test
{
    [TestFixture]
    class PathConnectionTest
    {
        [Test]
        public void PathConnectionBasicTest1()
        {
            var pathConnection = new PathConnections();

            Assert.IsFalse(pathConnection.HasConnection(0, 0));

            pathConnection.Add(0, 0);

            Assert.IsTrue(pathConnection.HasConnection(0, 0));

            Assert.IsFalse(pathConnection.HasConnection(0, 1));
            Assert.IsFalse(pathConnection.HasConnection(1, 0));

            pathConnection.Add(0, 1);

            Assert.IsTrue(pathConnection.HasConnection(0, 1));
        }

        [Test]
        public void PathConnectionSplitEmptyTest1()
        {
            var pathConnection = new PathConnections();

            pathConnection.Split(0, 0, 0);
            Assert.IsTrue(pathConnection.IsEmpty());
        }

        [Test]
        public void PathConnectionSplitBasicTest1()
        {
            var pathConnection = new PathConnections();
            pathConnection.Add(2, 1);
            pathConnection.Add(3, 1);
            pathConnection.Add(1, 4);

            pathConnection.Split(1, 5, 2);
            Assert.IsTrue(pathConnection.HasConnection(2, 5));
            Assert.IsTrue(pathConnection.HasConnection(3, 5));
            Assert.IsTrue(pathConnection.HasConnection(1, 5));
            Assert.IsTrue(pathConnection.HasConnection(5, 4));
        }
    }
}
