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
        public void PathConnectionRemoveTest1()
        {
            var pathConnection = new PathConnections();

            Assert.IsFalse(pathConnection.HasConnection(0, 0));

            pathConnection.Add(0, 0);

            Assert.IsFalse(pathConnection.IsEmpty());

            Assert.IsTrue(pathConnection.HasConnection(0, 0));

            Assert.IsFalse(pathConnection.HasConnection(0, 1));
            Assert.IsFalse(pathConnection.Remove(0, 1));
            Assert.IsTrue(pathConnection.Remove(0, 0));

            Assert.IsTrue(pathConnection.IsEmpty());
        }

        [Test]
        public void PathConnectionSplitEmptyTest1()
        {
            var pathConnection = new PathConnections();

            pathConnection.Split(0, 0, 0, true);
            Assert.IsTrue(pathConnection.IsEmpty());
        }

        [Test]
        public void PathConnectionSplitBasicInTest1()
        {
            var pathConnection = new PathConnections();
            pathConnection.Add(1, 2);

            pathConnection.Split(1, 4, 3, true);
            Assert.IsTrue(pathConnection.HasConnection(1, 4));
            Assert.IsTrue(pathConnection.HasConnection(3, 4));
            Assert.IsTrue(pathConnection.HasConnection(4, 2));
        }

        [Test]
        public void PathConnectionSplitBasicInTest2()
        {
            var pathConnection = new PathConnections();
            pathConnection.Add(1, 3);
            pathConnection.Add(2, 3);
            pathConnection.Add(3, 4);

            pathConnection.Split(1, 6, 5, true);
            Assert.IsTrue(pathConnection.HasConnection(1, 6));
            Assert.IsTrue(pathConnection.HasConnection(5, 6));
            Assert.IsTrue(pathConnection.HasConnection(6, 3));
            Assert.IsTrue(pathConnection.HasConnection(2, 3));
            Assert.IsTrue(pathConnection.HasConnection(3, 4));
        }

        [Test]
        public void PathConnectionSplitBasicOutTest1()
        {
            var pathConnection = new PathConnections();
            pathConnection.Add(1, 2);

            pathConnection.Split(1, 4, 3, false);
            Assert.IsTrue(pathConnection.HasConnection(1, 4));
            Assert.IsTrue(pathConnection.HasConnection(1, 3));
            Assert.IsTrue(pathConnection.HasConnection(4, 2));
        }

        [Test]
        public void PathConnectionSplitBasicOutTest2()
        {
            var pathConnection = new PathConnections();
            pathConnection.Add(1, 3);
            pathConnection.Add(2, 3);
            pathConnection.Add(3, 4);

            pathConnection.Split(1, 6, 5, false);
            Assert.IsTrue(pathConnection.HasConnection(1, 5));
            Assert.IsTrue(pathConnection.HasConnection(1, 6));
            Assert.IsTrue(pathConnection.HasConnection(6, 3));
            Assert.IsTrue(pathConnection.HasConnection(2, 3));
            Assert.IsTrue(pathConnection.HasConnection(3, 4));
        }

        [Test]
        public void PathConnectionRemovePathIdTest1()
        {
            var pathConnection = new PathConnections();
            pathConnection.Add(1, 3);
            pathConnection.Add(3, 1);
            pathConnection.Add(3, 4);

            pathConnection.RemovePathId(1);

            Assert.IsFalse(pathConnection.HasConnection(1, 3));
            Assert.IsFalse(pathConnection.HasConnection(3, 1));
            Assert.IsTrue(pathConnection.HasConnection(3, 4));
        }
    }
}
