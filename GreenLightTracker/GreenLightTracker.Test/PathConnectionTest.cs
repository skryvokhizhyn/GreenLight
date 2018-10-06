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
    }
}
